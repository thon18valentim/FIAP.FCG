using FIAP.FCG.Application.Services;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Web;
using FIAP.FCG.Infra.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace FIAP.FCG.Application.Auth
{
	public class AuthService(IAuthRepository repo, IConfiguration config) : BaseService, IAuthService
	{
        private readonly IAuthRepository _repository = repo;
        private readonly IConfiguration _configuration = config;

        public async Task<IApiResponse<string>> Login(LoginDto dto)
		{
			var user = await _repository.FindByCredentialsAsync(dto);
            if (user == null)
            {
                return Unauthorized<string>("Credenciais inválidas.");
            }
			var token = _repository.GenerateToken(_configuration);
            return Ok(token);
        }

        public async Task<IApiResponse<int>> Register(UserRegisterDto dto) 
        { 
            var id = await _repository.Add(dto);
            return Created(id, "Usuário registrado com sucesso.");
        }
	}
}
