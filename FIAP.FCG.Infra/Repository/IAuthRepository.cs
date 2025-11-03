using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace FIAP.FCG.Infra.Repository
{
    public interface IAuthRepository
    {
        Task<int> Create(UserRegisterDto dto);
        Task<User?> FindByCredentialsAsync(LoginDto dto);
        string GenerateToken(IConfiguration configuration, User user);
    }
}
