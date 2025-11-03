using AutoMapper;
using FIAP.FCG.Application.Services;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;
using FIAP.FCG.Infra.Context;
using FIAP.FCG.Infra.Repository;
using FIAP.FCG.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using FIAP.FCG.Tests.Builders;

namespace FIAP.FCG.Tests.Features.Games;

public class GamesLocalDbTests : IDisposable
{
    private readonly string _dbName;
    private readonly string _cs;
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly IGameRepository _repo;
    private readonly IGameService _service;

    public GamesLocalDbTests()
    {
        _cs = LocalDb.CreateDatabase(out _dbName);
        _ctx = new ApplicationDbContext(_cs);
        _ctx.Database.EnsureCreated();

        _mapper = TestMapper.Build();

        _repo = new GameRepository(_ctx, _mapper);
        _service = new GameService(_repo);
    }

    public void Dispose()
    {
        _ctx.Dispose();
        LocalDb.DropDatabase(_dbName);
    }

    [Fact(DisplayName = "Create → Created (201) e persiste hash keys")]
    public async Task Create_Created()
    {
        var dto = GameBuilder.NewRegisterDto(
            name: "Hades",
            description: "Roguelike de ação",
            price: 79.90);

        var resp = await _service.Create(dto);

        resp.IsSuccess.Should().BeTrue();
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        resp.ResultValue.Should().BeOfType(typeof(int));

        var saved = await _ctx.Games.AsNoTracking()
            .FirstOrDefaultAsync(g => g.Name == dto.Name);
        saved.Should().NotBeNull();
        saved!.Price.Should().Be(dto.Price);
        saved.Description.Should().Be(dto.Description);
        saved.Id.Should().Be(resp.ResultValue);
    }

    [Fact(DisplayName = "Create → Price <= 0 deve falhar (400)")]
    public async Task Create_InvalidPrice_BadRequest()
    {
        var dto = GameBuilder.NewRegisterDto(
            name: "Hades",
            description: "Roguelike de ação",
            price: 0d);

        var resp = await _service.Create(dto);
        Console.WriteLine(resp);

        resp.IsSuccess.Should().BeFalse();
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        resp.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = "GetById → 200 quando existe")]
    public async Task GetById_Ok_WhenExists()
    {
        var seed = GameBuilder.NewEntity(name: "Celeste", description: "Plataforma desafiadora", price: 49.90d);
        _ctx.Games.Add(seed);
        await _ctx.SaveChangesAsync();

        var resp = await _service.GetById(seed.Id);

        resp.IsSuccess.Should().BeTrue();
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.ResultValue.Should().NotBeNull();
        resp.ResultValue!.Name.Should().Be("Celeste");
        resp.ResultValue!.Description.Should().Be("Plataforma desafiadora");
        resp.ResultValue!.Price.Should().Be(49.90d);
    }

    [Fact(DisplayName = "GetById → 404 quando não existe")]
    public async Task GetById_NotFound()
    {
        var resp = await _service.GetById(9999);

        resp.IsSuccess.Should().BeFalse();
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        resp.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = "Update → 204 atualiza campos principais")]
    public async Task Update_NoContent()
    {
        var seed = GameBuilder.NewEntity(name: "Ori and the Blind Forest", description: "Original", price: 89.99d);
        _ctx.Games.Add(seed);
        await _ctx.SaveChangesAsync();

        var dto = new GameUpdateDto
        {
            Name = "Ori and the Will of the Wisps",
            Description = "Atualizado",
            Price = 99.99d
        };

        var resp = await _service.Update(seed.Id, dto);

        resp.IsSuccess.Should().BeTrue();
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updated = await _ctx.Games.AsNoTracking().FirstAsync(g => g.Id == seed.Id);
        updated.Name.Should().Be(dto.Name);
        updated.Description.Should().Be(dto.Description);
        updated.Price.Should().Be(dto.Price);
    }

    [Fact(DisplayName = "Delete → 204 remove do banco")]
    public async Task Delete_NoContent()
    {
        var seed = GameBuilder.NewEntity(name: "Dead Cells", description: "Roguelike metroidvania", price: 59.99d);
        _ctx.Games.Add(seed);
        await _ctx.SaveChangesAsync();

        var resp = await _service.Remove(seed.Id);

        resp.IsSuccess.Should().BeTrue();
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var exists = await _ctx.Games.AnyAsync(g => g.Id == seed.Id);
        exists.Should().BeFalse();
    }
}
