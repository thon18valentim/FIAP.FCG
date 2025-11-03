using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.WebApi.Middlewares;

public sealed class ProblemDetailsExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ProblemDetailsExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ProblemDetailsExceptionMiddleware(RequestDelegate next, ILogger<ProblemDetailsExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(ex, "Unhandled exception after response started. Path={Path}", context.Request.Path);
                throw;
            }

            await WriteProblemAsync(context, ex);
        }
    }

    private async Task WriteProblemAsync(HttpContext context, Exception ex)
    {
        var (status, title) = MapStatusAndTitle(ex);

        var correlationId = GetOrCreateCorrelationId(context);
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        // Always log full error internally
        _logger.LogError(ex,
            "Unhandled exception handled by middleware. status={Status} title={Title} path={Path} cid={CorrelationId} tid={TraceId}",
            status, title, context.Request.Path, correlationId, traceId);

        var problem = new ProblemDetails
        {
            Type = "about:blank",
            Title = title,
            Status = status,
            Detail = BuildClientDetail(ex, status),
            Instance = context.Request.Path
        };

        problem.Extensions["correlationId"] = correlationId;
        problem.Extensions["traceId"] = traceId;

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }

    private static (int Status, string Title) MapStatusAndTitle(Exception ex) => ex switch
    {
        ValidationException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
        ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Não autorizado"),
        KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
        DbUpdateException => (StatusCodes.Status409Conflict, "Conflito"),
        _ => (StatusCodes.Status500InternalServerError, "Erro")
    };

    // Hide stack in Production; show stack only in Development
    private string BuildClientDetail(Exception ex, int status)
    {
        if (_env.IsDevelopment())
            return ex.ToString();

        // Production: only safe messages for known cases; generic for 500
        return status switch
        {
            StatusCodes.Status400BadRequest => ex.Message,
            StatusCodes.Status401Unauthorized => string.IsNullOrWhiteSpace(ex.Message) ? "Não autorizado." : ex.Message,
            StatusCodes.Status404NotFound => ex.Message,
            StatusCodes.Status409Conflict => "Violação de restrição no banco de dados.",
            _ => "Ocorreu um erro inesperado. Tente novamente mais tarde."
        };
    }

    private static string GetOrCreateCorrelationId(HttpContext ctx)
    {
        if (ctx.Request.Headers.TryGetValue("X-Correlation-Id", out var h) && !string.IsNullOrWhiteSpace(h))        
        {
            var id = h.ToString();
            ctx.Response.Headers["X-Correlation-Id"] = id;
            return id;
        }

        if (ctx.Response.Headers.TryGetValue("X-Correlation-Id", out var rh) && !string.IsNullOrWhiteSpace(rh))
            return rh.ToString();

        var generated = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
        ctx.Response.Headers["X-Correlation-Id"] = generated;
        return generated;
    }
}