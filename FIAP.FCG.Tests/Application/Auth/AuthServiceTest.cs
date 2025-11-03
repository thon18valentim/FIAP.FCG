using FIAP.FCG.Application.Auth;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;
using FIAP.FCG.Core.Web;
using FIAP.FCG.Infra.Repository;
using FIAP.FCG.Tests.Builders;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace FIAP.FCG.Tests.Application.Auth;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _repo = new();
    private readonly IConfiguration _config;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "unit_test_secret_key_0123456789",
                ["Jwt:Issuer"] = "fcg-tests",
                ["Jwt:Audience"] = "fcg-tests",
                ["Jwt:Expiration"] = "60"
            })
            .Build();

        _service = new AuthService(_repo.Object, _config);
    }

    [Fact(DisplayName = "Register → 201 Created e retorna Id")]
    public async Task Register_Should_Return_Created_With_Id()
    {
        var dto = UserBuilders.NewRegisterDto();

        _repo.Setup(r => r.Create(dto)).ReturnsAsync(1);

        IApiResponse<int> resp = await _service.Register(dto);

        resp.IsSuccess.Should().BeTrue();
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        resp.ResultValue.Should().Be(1);
        resp.Message.Should().NotBeNullOrWhiteSpace();

        _repo.Verify(r => r.Create(dto), Times.Once);
    }

    [Fact(DisplayName = "Register → 400 BadRequest quando DTO inválido (não chama repo)")]
    public async Task Register_Should_Return_BadRequest_When_Dto_Invalid()
    {
        var dto = new UserRegisterDto
        {
            Email = "",
            Cpf = "",
            Password = "fraca",
            Name = "",
            Address = ""
        };

        _repo.Setup(r => r.Create(It.IsAny<UserRegisterDto>()))
             .ThrowsAsync(new Exception("NÃO DEVERIA CHAMAR O REPO"));

        IApiResponse<int> resp = await _service.Register(dto);

        resp.IsSuccess.Should().BeFalse();
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        resp.ResultValue.Should().Be(0);
        resp.Message.Should().NotBeNullOrWhiteSpace();

        _repo.Verify(r => r.Create(It.IsAny<UserRegisterDto>()), Times.Never);
    }

    [Fact(DisplayName = "Login → 200 OK com token quando credenciais válidas")]
    public async Task Login_Should_Return_Token_On_Success()
    {
        var dto = new LoginDto { Email = "ok@fcg.com", Password = "SenhaForte@123" };

        // Service valida DTO, consulta repo, e pede token ao repo
        _repo.Setup(r => r.FindByCredentialsAsync(dto))
             .ReturnsAsync(UserBuilders.NewEntity());

        _repo.Setup(r => r.GenerateToken(_config))
             .Returns("header.payload.signature");

        IApiResponse<string> resp = await _service.Login(dto);

        resp.IsSuccess.Should().BeTrue();
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.ResultValue.Should().Be("header.payload.signature");

        _repo.Verify(r => r.FindByCredentialsAsync(dto), Times.Once);
        _repo.Verify(r => r.GenerateToken(_config), Times.Once);
    }

    [Fact(DisplayName = "Login → 401 Unauthorized quando credenciais inválidas")]
    public async Task Login_Should_Return_Unauthorized_On_Invalid_Credentials()
    {
        var dto = new LoginDto { Email = "fail@fcg.com", Password = "Errada#123" };

        _repo.Setup(r => r.FindByCredentialsAsync(dto))
             .ReturnsAsync((User?)null);

        IApiResponse<string> resp = await _service.Login(dto);

        resp.IsSuccess.Should().BeFalse();
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        resp.ResultValue.Should().BeNull();
        resp.Message.Should().NotBeNullOrWhiteSpace();

        _repo.Verify(r => r.FindByCredentialsAsync(dto), Times.Once);
        _repo.Verify(r => r.GenerateToken(It.IsAny<IConfiguration>()), Times.Never);
    }

    [Fact(DisplayName = "Login → 400 BadRequest quando DTO inválido (não chama repo)")]
    public async Task Login_Should_Return_BadRequest_When_Dto_Invalid()
    {
        var dto = new LoginDto { Email = "", Password = "" }; // invalidez óbvia

        _repo.Setup(r => r.FindByCredentialsAsync(It.IsAny<LoginDto>()))
             .ThrowsAsync(new Exception("NÃO DEVERIA CHAMAR O REPO"));
        _repo.Setup(r => r.GenerateToken(It.IsAny<IConfiguration>()))
             .Throws(new Exception("NÃO DEVERIA CHAMAR O REPO"));

        IApiResponse<string> resp = await _service.Login(dto);

        resp.IsSuccess.Should().BeFalse();
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        resp.ResultValue.Should().BeNull();
        resp.Message.Should().NotBeNullOrWhiteSpace();

        _repo.Verify(r => r.FindByCredentialsAsync(It.IsAny<LoginDto>()), Times.Never);
        _repo.Verify(r => r.GenerateToken(It.IsAny<IConfiguration>()), Times.Never);
    }

    [Fact(DisplayName = "Login → Propaga exceção inesperada do repositório")]
    public async Task Login_Should_Propagate_Unexpected_Exception()
    {
        var dto = new LoginDto { Email = "boom@fcg.com", Password = "Qualquer@123" };

        _repo.Setup(r => r.FindByCredentialsAsync(dto))
             .ThrowsAsync(new Exception("boom"));

        var act = async () => await _service.Login(dto);

        var ex = await Assert.ThrowsAsync<Exception>(act);
        ex.Message.Should().Be("boom");

        _repo.Verify(r => r.FindByCredentialsAsync(dto), Times.Once);
        _repo.Verify(r => r.GenerateToken(It.IsAny<IConfiguration>()), Times.Never);
    }
}
