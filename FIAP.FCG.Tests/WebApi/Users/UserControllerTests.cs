using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Tests._TestDoubles;
using FIAP.FCG.WebApi.Controllers.v1;
using FIAP.FCG.Core.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace FIAP.FCG.Tests.WebApi.Users;

public class StandardControllerTests
{
    // Fake controller que expõe um método público para testar TryMethodAsync
    private sealed class FakeController : StandardController
    {
        private readonly ILogger _logger;
        public FakeController(ILogger logger) => _logger = logger;

        public Task<IActionResult> CallAsync<T>(Func<Task<IApiResponse<T>>> fn)
            => TryMethodAsync(fn, _logger);
    }

    private static FakeController Create(out DefaultHttpContext http)
    {
        var logger = new Mock<ILogger>().Object;
        var ctrl = new FakeController(logger);
        http = new DefaultHttpContext();
        ctrl.ControllerContext = new ControllerContext { HttpContext = http };
        return ctrl;
    }

    [Fact(DisplayName = "TryMethodAsync: Return 200 + Body")]
    public async Task TryMethodAsync_DevePropagar200EBody()
    {
        var ctrl = Create(out _);

        var response = TestApiResponse<UserResponseDto>.Ok(
            new UserResponseDto(10, "Ana", "ana@fcg.com", "Rua 1", "12345678901", DateTime.UtcNow));

        var result = await ctrl.CallAsync(() => Task.FromResult<IApiResponse<UserResponseDto>>(response));

        var obj = result as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        obj.Value.Should().BeSameAs(response);
    }

    [Fact(DisplayName = "TryMethodAsync: Return 201 + Body")]
    public async Task TryMethodAsync_DevePropagar201EBody()
    {
        var ctrl = Create(out _);

        var created = TestApiResponse<UserResponseDto>.Created(
            new UserResponseDto(11, "Bia", "bia@fcg.com", "Rua 2", "10987654321", DateTime.UtcNow));

        var result = await ctrl.CallAsync(() => Task.FromResult<IApiResponse<UserResponseDto>>(created));

        var obj = result as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be((int)HttpStatusCode.Created);
        obj.Value.Should().BeSameAs(created);
    }

    [Fact(DisplayName = "TryMethodAsync: Return 404")]
    public async Task TryMethodAsync_DevePropagar404()
    {
        var ctrl = Create(out _);

        var notFound = TestApiResponse<UserResponseDto>.NotFound("Usuário não encontrado");

        var result = await ctrl.CallAsync(() => Task.FromResult<IApiResponse<UserResponseDto>>(notFound));

        var obj = result as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        obj.Value.Should().BeSameAs(notFound);
    }

    [Fact(DisplayName = "TryMethodAsync: Return 500 + Excecao")]
    public async Task TryMethodAsync_DeveRetornar500_EmExcecao()
    {
        var ctrl = Create(out _);

        var result = await ctrl.CallAsync<UserResponseDto>(() => throw new Exception("boom"));

        var obj = result as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var problem = obj.Value as ProblemDetails;
        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Erro interno no servidor");
    }
}
