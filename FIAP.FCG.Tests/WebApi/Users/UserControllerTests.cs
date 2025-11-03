// tests/Controllers/StandardController_TryMethodAsync_Tests.cs
using System.ComponentModel.DataAnnotations;
using System.Net;
using FIAP.FCG.Core.Web;
using FIAP.FCG.WebApi.Controllers.v1;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FIAP.FCG.Tests.Controllers;

public class StandardController_TryMethodAsync_Tests
{
    private sealed class FakeController : StandardController
    {
        private readonly ILogger _logger;
        public FakeController(ILogger logger) { _logger = logger; }
        public Task<IActionResult> CallAsync<T>(Func<Task<IApiResponse<T>>> f) => TryMethodAsync(f, _logger);
    }

    private static FakeController Create(out Mock<ILogger> loggerMock)
    {
        loggerMock = new Mock<ILogger>();
        var ctrl = new FakeController(loggerMock.Object);
        // Necessário para popular problem.Extensions["traceId"]
        var http = new DefaultHttpContext();
        ctrl.ControllerContext = new ControllerContext { HttpContext = http };
        return ctrl;
    }

    private sealed class TestResponse<T> : IApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = "";
        public T? ResultValue { get; set; }

        public static TestResponse<T> Ok(T value) => new() { IsSuccess = true, StatusCode = HttpStatusCode.OK, ResultValue = value };
        public static TestResponse<T> NoContent() => new() { IsSuccess = true, StatusCode = HttpStatusCode.NoContent };
        public static TestResponse<T> Fail(HttpStatusCode code, string msg = "") => new() { IsSuccess = false, StatusCode = code, Message = msg };
    }

    [Fact(DisplayName = "TryMethodAsync: Return 500 + ProblemDetails")]
    public async Task TryMethodAsync_DeveRetornar500_EmExcecao()
    {
        var ctrl = Create(out _);

        var result = await ctrl.CallAsync<string>(() => throw new Exception("boom"));

        var obj = result as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var problem = obj.Value as ProblemDetails;
        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Erro");                  // << Atualizado conforme ToDefaultTitle
        problem.Status.Should().Be(StatusCodes.Status500InternalServerError);
        problem.Detail.Should().Be("boom");                  // detail usa ex.Message
        problem.Extensions.ContainsKey("traceId").Should().BeTrue();
        problem.Extensions["traceId"].Should().NotBeNull();
    }

    [Fact(DisplayName = "TryMethodAsync: 204 NoContent sem body")]
    public async Task TryMethodAsync_DeveRetornar204_SemBody()
    {
        var ctrl = Create(out _);

        var result = await ctrl.CallAsync<object>(() => Task.FromResult<IApiResponse<object>>(TestResponse<object>.NoContent()));

        result.Should().BeOfType<StatusCodeResult>();
        (result as StatusCodeResult)!.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }

    [Fact(DisplayName = "TryMethodAsync: BadRequest (ValidationException)")]
    public async Task TryMethodAsync_BadRequest_ValidationException()
    {
        var ctrl = Create(out _);

        var result = await ctrl.CallAsync<object>(() => throw new ValidationException("dados inválidos"));

        var obj = result as ObjectResult;
        obj!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        var problem = obj.Value as ProblemDetails;
        problem!.Title.Should().Be("Requisição inválida");
        problem.Detail.Should().Be("dados inválidos");
    }

    [Fact(DisplayName = "TryMethodAsync: Unauthorized")]
    public async Task TryMethodAsync_Unauthorized()
    {
        var ctrl = Create(out _);

        var result = await ctrl.CallAsync<object>(() => throw new UnauthorizedAccessException("sem permissão"));

        var obj = result as ObjectResult;
        obj!.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        var problem = obj.Value as ProblemDetails;
        problem!.Title.Should().Be("Não autorizado");
        problem.Detail.Should().Be("sem permissão");
    }

    [Fact(DisplayName = "TryMethodAsync: NotFound (KeyNotFoundException)")]
    public async Task TryMethodAsync_NotFound()
    {
        var ctrl = Create(out _);

        var result = await ctrl.CallAsync<object>(() => throw new KeyNotFoundException("não achei"));

        var obj = result as ObjectResult;
        obj!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        var problem = obj.Value as ProblemDetails;
        problem!.Title.Should().Be("Recurso não encontrado");
        problem.Detail.Should().Be("não achei");
    }

    [Fact(DisplayName = "TryMethodAsync: Conflict (DbUpdateException)")]
    public async Task TryMethodAsync_Conflict_DbUpdateException()
    {
        var ctrl = Create(out _);

        var dbEx = new Microsoft.EntityFrameworkCore.DbUpdateException("violação de unicidade");
        var result = await ctrl.CallAsync<object>(() => throw dbEx);

        var obj = result as ObjectResult;
        obj!.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        var problem = obj.Value as ProblemDetails;
        problem!.Title.Should().Be("Conflito");
        problem.Detail.Should().Be("violação de unicidade");
    }

    [Fact(DisplayName = "TryMethodAsync: Propaga sucesso 200 com body de IApiResponse")]
    public async Task TryMethodAsync_SucessoComBody()
    {
        var ctrl = Create(out _);

        var response = TestResponse<string>.Ok("ok");
        var result = await ctrl.CallAsync<string>(() => Task.FromResult<IApiResponse<string>>(response));

        var obj = result as ObjectResult;
        obj!.StatusCode.Should().Be(StatusCodes.Status200OK);
        obj.Value.Should().BeSameAs(response);
    }

    [Fact(DisplayName = "TryMethodAsync: Propaga erro custom do service (ex: 422)")]
    public async Task TryMethodAsync_PropagaErroService()
    {
        var ctrl = Create(out _);

        var response = TestResponse<string>.Fail((HttpStatusCode)422, "entidade inválida");
        var result = await ctrl.CallAsync<string>(() => Task.FromResult<IApiResponse<string>>(response));

        var obj = result as ObjectResult;
        obj!.StatusCode.Should().Be(422);
        obj.Value.Should().BeSameAs(response);
    }
}
