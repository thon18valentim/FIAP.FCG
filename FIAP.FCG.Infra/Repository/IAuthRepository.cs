using Microsoft.Extensions.Configuration;
using FIAP.FCG.Core.Inputs;
using System.Net;

namespace FIAP.FCG.Infra.Repository
{
    public interface IAuthRepository
    {
        Task<HttpStatusCode> Add(UserRegisterDto dto);
        Task<string> Login(LoginDto dto, IConfiguration configuration);
    }
}
