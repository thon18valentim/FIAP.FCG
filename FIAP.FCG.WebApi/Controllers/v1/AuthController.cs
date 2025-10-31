using FIAP.FCG.Application.Auth;
using FIAP.FCG.Core.Inputs;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.WebApi.Controllers.v1
{
	public class AuthController(IAuthService service, ILogger<AuthController> logger) : StandardController
	{
		[HttpPost("login")]
		public IActionResult Login(LoginDto dto)
		{
			return TryMethod(() => service.Login(dto), logger);
		}
	}
}
