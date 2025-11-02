using AutoMapper;
using FIAP.FCG.Application.Auth;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Infra.Context;
using FIAP.FCG.Infra.Repository;
using FIAP.FCG.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace FIAP.FCG.Tests.Features.Auth;
public class AuthLocalDbTests : IDisposable
{
    private readonly string _dbName;
    private readonly string _cs;
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly AuthRepository _repo;
    private readonly AuthService _service;

    public AuthLocalDbTests()
    {
        _cs = LocalDb.CreateDatabase(out _dbName);
        _ctx = new ApplicationDbContext(_cs);
        _ctx.Database.EnsureCreated(); // simples e suficiente (ou .Migrate())

        _mapper = TestMapper.Build();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super_secret_test_key_1234567890",
                ["Jwt:Issuer"] = "fcg-tests",
                ["Jwt:Audience"] = "fcg-tests",
                ["Jwt:Expiration"] = "60"
            })
            .Build();

        _repo = new AuthRepository(_ctx, _mapper);
        _service = new AuthService(_repo, config);
    }

    public void Dispose()
    {
        _ctx.Dispose();
        LocalDb.DropDatabase(_dbName);
    }

    [Theory(DisplayName = "Register → Created")]
    [InlineData("alice@mail.com", "12345678909", "SenhaForte@123", "Alice", "R. Alice")]
    [InlineData("bob@mail.com", "98765432100", "OutraForte#987", "Bob", "R. Bob")]
    public async Task Register_Created(string email, string cpf, string password, string name, string address)
    {
        var resp = await _service.Register(new UserRegisterDto
        {
            Email = email,
            Cpf = cpf,
            Password = password,
            Address = address,
            Name = name
        });

        resp.IsSuccess.Should().BeTrue();
        resp.StatusCode.Should().Be(HttpStatusCode.Created);

        var saved = await _ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email.ToLower());
        saved.Should().NotBeNull();
        saved!.Password.Should().NotBe(password);
    }

    [Theory(DisplayName = "Login → sucesso retorna token")]
    [InlineData("log1@mail.com", "12312312312", "SenhaForte@123", "User 1", "R. Teste 1")]
    [InlineData("log2@mail.com", "78978978978", "OutraForte#987", "User 2", "R. Teste 2")]
    public async Task Login_Success(string email, string cpf, string password, string name, string address)
    {
        (await _service.Register(new UserRegisterDto { Email = email, Cpf = cpf, Password = password, Name = name, Address = address }))
            .IsSuccess.Should().BeTrue();

        var resp = await _service.Login(new LoginDto { Email = email, Password = password });

        resp.IsSuccess.Should().BeTrue();
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.ResultValue.Should().NotBeNullOrWhiteSpace();
    }

    [Theory(DisplayName = "Login → credenciais inválidas")]
    [InlineData("inexistente@mail.com", "Qualquer@123")]
    [InlineData("log3@mail.com", "Errada#999")]
    public async Task Login_Fail(string email, string password)
    {
        // cria um usuário válido para garantir 401 quando credenciais não batem
        (await _service.Register(new UserRegisterDto
        {
            Email = "log3@mail.com",
            Cpf = "32132132132",
            Password = "SenhaCorreta@123",
            Address = "R. Teste",
            Name = "User3"
        })).IsSuccess.Should().BeTrue();

        var resp = await _service.Login(new LoginDto { Email = email, Password = password });

        resp.IsSuccess.Should().BeFalse();
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        resp.Message.Should().NotBeNullOrWhiteSpace();
    }
}
