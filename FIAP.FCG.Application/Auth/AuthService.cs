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
	public class AuthService(IConfiguration configuration, IAuthRepository repository) : BaseService, IAuthService
	{
		public async Task<IApiResponse<string>> Login(LoginDto dto)
		{

			return Success(await repository.Login(dto, configuration), "Login realizado com sucesso");
		}

        public async Task<IApiResponse<HttpStatusCode>> Register(UserRegisterDto dto) => Success(await repository.Add(dto), "Usuário cadastrado com sucesso");

        #region private ::

        

		#endregion
	}
}
