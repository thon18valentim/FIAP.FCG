using System.Net;
using FIAP.FCG.Application.Services;   // IUserService, UserService
using FIAP.FCG.Core.Inputs;            // UserUpdateDto, UserResponseDto
using FIAP.FCG.Core.Models;            // (se o service mapear models -> dto internamente)
using FIAP.FCG.Core.Web;               // IApiResponse<T>
using FIAP.FCG.Infra.Repository;       // IUserRepository
using FluentAssertions;
using Moq;

namespace FIAP.FCG.Tests.Application.Users;

public class UsersServiceTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly IUserService _service;

    public UsersServiceTests()
    {
        _service = new UserService(_repoMock.Object);
    }

    [Fact]
    public async Task GetById_DeveRetornar200_ComUsuario_QuandoExiste()
    {
        // arrange: repo retorna DTO existente
        var expected = new UserResponseDto(
            Id: 1,
            Name: "Ana",
            Email: "ana@fcg.com",
            Address: "R. Teste",
            Cpf: "12345678910",
            CreatedAtUtc: DateTime.UtcNow);

        _repoMock.Setup(r => r.GetById(1))
                 .ReturnsAsync(expected);

        // act
        IApiResponse<UserResponseDto?> response = await _service.GetById(1);

        // assert: service mapeia para IApiResponse 200
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.ResultValue.Should().NotBeNull();

        response.ResultValue!.Id.Should().Be(1);
        response.ResultValue!.Name.Should().Be("Ana");
        response.ResultValue!.Email.Should().Be("ana@fcg.com");
        response.ResultValue!.Address.Should().Be("R. Teste");
        response.ResultValue!.Cpf.Should().Be("12345678910");
        response.ResultValue!.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetById_DeveRetornar404_QuandoNaoExiste()
    {
        // arrange: repo não encontrou
        _repoMock.Setup(r => r.GetById(999))
                 .ReturnsAsync((UserResponseDto?)null);

        // act
        IApiResponse<UserResponseDto?> response = await _service.GetById(999);

        // assert: NÃO é null; é um envelope IApiResponse com 404
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.IsSuccess.Should().BeFalse();
        response.ResultValue.Should().BeNull();
        response.Message.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_DeveChamarRepositorioComIdECorpoCorretos()
    {
        // arrange
        var dto = new UserUpdateDto { Name = "Novo", Email = "novo@fcg.com" };

        _repoMock.Setup(r => r.Update(10, dto))
                 .ReturnsAsync(true);

        // act
        await _service.Update(10, dto);

        // assert
        _repoMock.Verify(r => r.Update(10, dto), Times.Once);
    }

    [Fact]
    public async Task Remove_DeveChamarRepositorio()
    {
        // arrange
        _repoMock.Setup(r => r.Remove(5))
                 .ReturnsAsync(true);

        // act
        await _service.Remove(5);

        // assert
        _repoMock.Verify(r => r.Remove(5), Times.Once);
    }
}
