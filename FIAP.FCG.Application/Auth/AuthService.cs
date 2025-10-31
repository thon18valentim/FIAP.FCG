using FIAP.FCG.Application.Services;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace FIAP.FCG.Application.Auth
{
	public class AuthService(IConfiguration configuration) : BaseService, IAuthService
	{
		public IApiResponse<string> Login(LoginDto dto)
		{
			return Success(GenerateToken(), "Login realizado com sucesso");
		}

		#region private ::

		private string GenerateToken()
		{
			var secretKey = configuration["Jwt:Key"];
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity([
					new Claim(JwtRegisteredClaimNames.Sid, "123"),
					new Claim(JwtRegisteredClaimNames.NameId, "username"),
					new Claim(JwtRegisteredClaimNames.Email, "user@mail.com")
					]),
				Expires = DateTime.Now.AddMinutes(int.Parse(configuration["Jwt:Expiration"]!)),
				SigningCredentials = credentials,
				Issuer = configuration["Jwt:Issuer"],
				Audience = configuration["Jwt:Audience"]
			};

			var handler = new JsonWebTokenHandler();
			return handler.CreateToken(tokenDescriptor);
		}

		#endregion
	}
}
