using System.Net;
using FIAP.FCG.Application.Services;   // IGameService, GameService
using FIAP.FCG.Core.Inputs;            // GameUpdateDto, GameResponseDto
using FIAP.FCG.Core.Web;               // IApiResponse<T>
using FIAP.FCG.Infra.Repository;       // IGameRepository
using FIAP.FCG.Tests.Builders;
using FluentAssertions;
using Moq;

namespace FIAP.FCG.Tests.Application.Games;

public class GamesServiceTests
{
    private readonly Mock<IGameRepository> _repoMock = new();
    private readonly IGameService _service;

    public GamesServiceTests()
    {
        _service = new GameService(_repoMock.Object);
    }

    [Fact]
    public async Task GetById_DeveRetornar200_ComJogo_QuandoExiste()
    {
        // arrange: repositório retorna DTO existente
        var expected = GameBuilder.NewResponseDto();

        _repoMock.Setup(r => r.GetById(1))
                 .ReturnsAsync(expected);

        // act
        IApiResponse<GameResponseDto?> response = await _service.GetById(1);

        // assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.IsSuccess.Should().BeTrue();
        response.ResultValue.Should().NotBeNull();

        response.ResultValue!.Id.Should().Be(1);
        response.ResultValue!.Name.Should().Be("Game Test");
        response.ResultValue!.Platform.Should().Be("PC");
        response.ResultValue!.PublisherName.Should().Be("Publisher Test");
        response.ResultValue!.Description.Should().Be("Game Description");
        response.ResultValue!.Price.Should().Be(199.99);
        response.ResultValue!.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetById_DeveRetornar404_QuandoNaoExiste()
    {
        // arrange: repositório não encontrou
        _repoMock.Setup(r => r.GetById(999))
                 .ReturnsAsync((GameResponseDto?)null);

        // act
        IApiResponse<GameResponseDto?> response = await _service.GetById(999);

        // assert
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
        var dto = new GameUpdateDto
        {
            Name = "Novo Game",
            Platform = "Xbox",
            PublisherName = "Nova Publisher",
            Description = "Descrição Atualizada",
            Price = 299.90
        };

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
