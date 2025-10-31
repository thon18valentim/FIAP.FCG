using System.Net;
using FIAP.FCG.Core.Web;

namespace FIAP.FCG.Tests._TestDoubles;

public sealed class TestApiResponse<T> : IApiResponse<T>
{
    public T? ResultValue { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }

    public static TestApiResponse<T> Ok(T value, string? message = null) => new()
    {
        ResultValue = value,
        StatusCode = HttpStatusCode.OK,
        Message = message ?? string.Empty,
        IsSuccess = true
    };

    public static TestApiResponse<T> Created(T value, string? message = null) => new()
    {
        ResultValue = value,
        StatusCode = HttpStatusCode.Created,
        Message = message ?? string.Empty,
        IsSuccess = true
    };

    public static TestApiResponse<T> NoContent(string? message = null) => new()
    {
        ResultValue = default,
        StatusCode = HttpStatusCode.NoContent,
        Message = message ?? string.Empty,
        IsSuccess = true
    };

    public static TestApiResponse<T> NotFound(string? message = null) => new()
    {
        ResultValue = default,
        StatusCode = HttpStatusCode.NotFound,
        Message = message ?? "Recurso não encontrado.",
        IsSuccess = false
    };

    public static TestApiResponse<T> BadRequest(string? message = null) => new()
    {
        ResultValue = default,
        StatusCode = HttpStatusCode.BadRequest,
        Message = message ?? "Requisição inválida.",
        IsSuccess = false
    };

    public static TestApiResponse<T> Unauthorized(string? message = null) => new()
    {
        ResultValue = default,
        StatusCode = HttpStatusCode.Unauthorized,
        Message = message ?? "Não autorizado.",
        IsSuccess = false
    };
}
