using AutoMapper;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;
using FIAP.FCG.Infra.Context;
using FIAP.FCG.Infra.Repository;
using FIAP.FCG.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Tests.Features.Users;

public class UsersLocalDbTests : IDisposable
{
    private readonly string _dbName;
    private readonly string _cs;
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repo;

    public UsersLocalDbTests()
    {
        _cs = LocalDb.CreateDatabase(out _dbName);
        _ctx = new ApplicationDbContext(_cs);
        _ctx.Database.EnsureCreated();

        _mapper = TestMapper.Build();
        _repo = new UserRepository(_ctx, _mapper);
    }

    public void Dispose()
    {
        _ctx.Dispose();
        LocalDb.DropDatabase(_dbName);
    }

    [Fact(DisplayName = "GetAll → retorna usuários persistidos")]
    public async Task GetAll_ReturnsUsers()
    {
        _ctx.Users.AddRange(
            new User { Email = "ana@mail.com", Cpf = "11111111111", Password = "HASH", Name = "Ana", Address = "Rua A", IsAdmin = false },
            new User { Email = "bia@mail.com", Cpf = "22222222222", Password = "HASH", Name = "Bia", Address = "Rua B", IsAdmin = true }
        );
        await _ctx.SaveChangesAsync();

        var list = await _repo.GetAll();

        list.Should().NotBeNull();
        list.Should().HaveCountGreaterThanOrEqualTo(2);
        list.Should().Contain(u => u.Email == "ana@mail.com");
        list.Should().Contain(u => u.Email == "bia@mail.com");
    }

    [Fact(DisplayName = "GetById → retorna quando existe")]
    public async Task GetById_Found()
    {
        var u = new User { Email = "carol@mail.com", Cpf = "33333333333", Password = "HASH", Name = "Carol", Address = "Rua C", IsAdmin = false };
        _ctx.Users.Add(u);
        await _ctx.SaveChangesAsync();

        var found = await _repo.GetById(u.Id);

        found.Should().NotBeNull();
        found!.Email.Should().Be("carol@mail.com");
    }

    [Fact(DisplayName = "GetById → null quando não existe")]
    public async Task GetById_Null()
    {
        var found = await _repo.GetById(987654);

        found.Should().BeNull();
    }

    [Fact(DisplayName = "Update → altera campos (204 esperado na API)")]
    public async Task Update_ChangesFields()
    {
        var u = new User { Email = "diego@mail.com", Cpf = "44444444444", Password = "HASH", Name = "Diego", Address = "Rua D", IsAdmin = false };
        string oldPassword = u.Password;
        _ctx.Users.Add(u);
        await _ctx.SaveChangesAsync();

        var dto = new UserUpdateDto
        {
            Name = "Diego Atualizado",
            Address = "Rua D, 123",
            Email = "diego@mail.com", // mantendo mesmo e-mail
            Password = "NovaSenha@123"
        };

        await _repo.Update(u.Id, dto);

        var updated = await _ctx.Users.AsNoTracking().FirstAsync(x => x.Id == u.Id);
        updated.Name.Should().Be(dto.Name);
        updated.Address.Should().Be(dto.Address);
        updated.Email.Should().Be(dto.Email.ToLower());
        updated.Password.Should().NotBe(oldPassword);
        updated.Password.Should().NotBeNullOrWhiteSpace();
        updated.Password.Length.Should().Be(64);
    }

    [Fact(DisplayName = "Delete → remove registro")]
    public async Task Delete_Removes()
    {
        var u = new User { Email = "excluir@mail.com", Cpf = "55555555555", Password = "HASH", Name = "Excluir", Address = "Rua E", IsAdmin = false };
        _ctx.Users.Add(u);
        await _ctx.SaveChangesAsync();

        await _repo.Remove(u.Id);

        var exists = await _ctx.Users.AnyAsync(x => x.Id == u.Id);
        exists.Should().BeFalse();
    }
}
