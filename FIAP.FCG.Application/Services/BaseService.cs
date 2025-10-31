using FIAP.FCG.Core.Web;
using System.Net;

namespace FIAP.FCG.Application.Services
{
    public abstract class BaseService
    {
        // ===== Preferir estes =====

        protected static IApiResponse<T> Ok<T>(T result, string message = "")
            => Build(result, HttpStatusCode.OK, true, message);

        protected static IApiResponse<T> Created<T>(T result, string message = "")
            => Build(result, HttpStatusCode.Created, true, message);

        // Para operações sem payload em sucesso (Update/Delete)
        protected static IApiResponse<bool> NoContent(string message = "")
            => Build(false, HttpStatusCode.NoContent, true, message);

        // NotFound/BadRequest/etc. (mantidos – já estavam corretos)
        public static IApiResponse<T> BadRequest<T>(string message = "")
            => Build(default(T), HttpStatusCode.BadRequest, false, message);

        public static IApiResponse<T> NotFound<T>(string message = "")
            => Build(default(T), HttpStatusCode.NotFound, false, message);

        public static IApiResponse<T> Unauthorized<T>(string message = "")
            => Build(default(T), HttpStatusCode.Unauthorized, false, message);

        public static IApiResponse<T> InternalServerError<T>(string message = "")
            => Build(default(T), HttpStatusCode.InternalServerError, false, message);

        public static IApiResponse<T> RequestTimeout<T>(string message = "")
            => Build(default(T), HttpStatusCode.RequestTimeout, false, message);

        public static IApiResponse<T> GenericError<T>(HttpStatusCode statusCode, string message = "")
            => Build(default(T), statusCode, false, message);

        // ===== Retrocompatibilidade (evitar usar daqui pra frente) =====
        // Estes mantêm o comportamento antigo (status 200 sempre em “success”)
        // Só não remova agora para não quebrar código existente.
        public static IApiResponse<T> Success<T>(T resultValue, string message = "")
            => Build(resultValue, HttpStatusCode.OK, true, message);

        public static IApiResponse<bool> Success(string message = "")
            => Build(true, HttpStatusCode.OK, true, message);

        public static IApiResponse<T> Fail<T>(string message = "")
            => Build(default(T), HttpStatusCode.OK, false, message);
        // ==========================

        // Fábrica única
        private static IApiResponse<T> Build<T>(T? value, HttpStatusCode code, bool isSuccess, string message)
            => new ApiResponse<T>
            {
                ResultValue = value,
                Message = message,
                StatusCode = code,
                IsSuccess = isSuccess
            };

        // Implementação concreta simples do envelope
        private sealed class ApiResponse<T> : IApiResponse<T>
        {
            public T? ResultValue { get; set; }
            public HttpStatusCode StatusCode { get; set; }
            public string Message { get; set; } = string.Empty;
            public bool IsSuccess { get; set; }
        }
    }
}
