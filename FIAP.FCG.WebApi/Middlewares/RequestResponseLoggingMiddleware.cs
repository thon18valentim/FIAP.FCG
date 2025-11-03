using System.Diagnostics;
using System.Text;          
using System.Text.Json;

namespace FIAP.FCG.WebApi.Middlewares
{
    public sealed class RequestResponseLoggingMiddleware
    {
        private const int BodyLogLimit = 2048; 
        private static readonly string[] SensitiveKeys = ["password", "senha", "token", "secret", "authorization"];

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            var correlationId = GetOrCreateCorrelationId(context);
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

            context.Response.Headers["X-Correlation-Id"] = correlationId;

            using (_logger.BeginScope(new Dictionary<string, object?>
            {
                ["CorrelationId"] = correlationId,
                ["TraceId"] = traceId
            }))
            {
                var requestInfo = await ReadRequestAsync(context);

                _logger.LogInformation(
                    "HTTP request {Method} {Path}{Query} from {ClientIp} ua={UserAgent} auth={IsAuth} cid={CorrelationId} tid={TraceId}",
                    requestInfo.Method, requestInfo.Path, requestInfo.QueryString,
                    requestInfo.ClientIp, requestInfo.UserAgent, requestInfo.IsAuthenticated,
                    correlationId, traceId
                );

                var originalBody = context.Response.Body;
                await using var captureStream = new MemoryStream();
                context.Response.Body = captureStream;

                try
                {
                    await _next(context);
                }
                finally
                {
                    sw.Stop();

                    var responseInfo = await ReadResponseAsync(context, captureStream);

                    _logger.LogInformation(
                        "HTTP response {StatusCode} for {Method} {Path}{Query} in {ElapsedMs}ms ct={ContentType} len={ContentLength} cid={CorrelationId} tid={TraceId}",
                        responseInfo.StatusCode, requestInfo.Method, requestInfo.Path, requestInfo.QueryString,
                        sw.Elapsed.TotalMilliseconds, responseInfo.ContentType, responseInfo.ContentLength,
                        correlationId, traceId
                    );

                    captureStream.Position = 0;
                    await captureStream.CopyToAsync(originalBody);
                    context.Response.Body = originalBody;

                    if (requestInfo.BodySample is not null)
                        _logger.LogDebug("RequestBodySample: {Body}", requestInfo.BodySample);

                    if (responseInfo.BodySample is not null)
                        _logger.LogDebug("ResponseBodySample: {Body}", responseInfo.BodySample);
                }
            }
        }

        private static string GetOrCreateCorrelationId(HttpContext ctx)
        {
            if (ctx.Request.Headers.TryGetValue("X-Correlation-Id", out var h) && !string.IsNullOrWhiteSpace(h))
                return h!;
            return Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
        }

        private static async Task<(string Method, string Path, string QueryString, string? UserId, string? UserName, string ClientIp, string? UserAgent, bool IsAuthenticated, string? BodySample)> ReadRequestAsync(HttpContext ctx)
        {
            string method = ctx.Request.Method;
            string path = ctx.Request.Path.Value ?? "/";
            string query = ctx.Request.QueryString.HasValue ? ctx.Request.QueryString.Value! : string.Empty;

            string clientIp = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            string? userAgent = ctx.Request.Headers.UserAgent.ToString();
            bool isAuth = ctx.User?.Identity?.IsAuthenticated == true;

            string? uid = ctx.User?.FindFirst("sub")?.Value ?? ctx.User?.FindFirst("nameid")?.Value ?? ctx.User?.FindFirst("sid")?.Value;
            string? uname = ctx.User?.Identity?.Name ?? ctx.User?.FindFirst("email")?.Value ?? ctx.User?.FindFirst("unique_name")?.Value;

            string? bodySample = null;
            if (ShouldReadBody(ctx.Request))
            {
                ctx.Request.EnableBuffering();

                using var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8, leaveOpen: true);
                var raw = await reader.ReadToEndAsync();

                ctx.Request.Body.Position = 0;
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    var truncated = Truncate(raw, BodyLogLimit);
                    bodySample = TryRedactJson(truncated);
                }
            }

            return (method, path, query, uid, uname, clientIp, userAgent, isAuth, bodySample);
        }

        private static async Task<(int StatusCode, string? ContentType, long ContentLength, string? BodySample)> ReadResponseAsync(HttpContext ctx, MemoryStream buffer)
        {
            int status = ctx.Response.StatusCode;
            string? ct = ctx.Response.ContentType;
            long len = 0;
            string? sample = null;

            if (buffer.CanRead)
            {
                len = buffer.Length;

                if (len > 0 && IsTextual(ct))
                {
                    buffer.Position = 0;
                    using var reader = new StreamReader(buffer, Encoding.UTF8, leaveOpen: true);
                    var raw = await reader.ReadToEndAsync();
                    buffer.Position = 0;

                    var truncated = Truncate(raw, BodyLogLimit);
                    sample = TryRedactJson(truncated);
                }
            }

            return (status, ct, len, sample);
        }

        private static bool ShouldReadBody(HttpRequest req)
        {
            if (req.ContentLength == 0) return false;
            if (req.Method is not ("POST" or "PUT" or "PATCH")) return false;
            return IsTextual(req.ContentType);
        }

        private static bool IsTextual(string? contentType)
            => !string.IsNullOrWhiteSpace(contentType) &&
               (contentType!.Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
                contentType!.Contains("text/", StringComparison.OrdinalIgnoreCase));

        private static string Truncate(string input, int max)
        {
            if (input.Length <= max) return input;
            return input[..max] + "...(truncated)";
        }

        private static string TryRedactJson(string input)
        {
            try
            {
                using var doc = JsonDocument.Parse(input);
                var redacted = RedactElement(doc.RootElement);
                return JsonSerializer.Serialize(redacted);
            }
            catch
            {
                return input;
            }
        }

        private static object? RedactElement(JsonElement el)
        {
            return el.ValueKind switch
            {
                JsonValueKind.Object => el.EnumerateObject().ToDictionary(
                    p => p.Name,
                    p => IsSensitive(p.Name) ? (object?)"***REDACTED***" : RedactElement(p.Value)),
                JsonValueKind.Array => el.EnumerateArray().Select(RedactElement).ToArray(),
                _ => el.ValueKind switch
                {
                    JsonValueKind.String => el.GetString(),
                    JsonValueKind.Number => el.GetRawText(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => el.GetRawText()
                }
            };
        }

        private static bool IsSensitive(string name)
            => SensitiveKeys.Any(k => string.Equals(k, name, StringComparison.OrdinalIgnoreCase));
    }
}
