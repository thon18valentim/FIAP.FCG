using Microsoft.AspNetCore.Mvc;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace FIAP.FCG.WebApi.Controllers.v1
{
	[Authorize]
	public class UserController(IUserService service, ILogger<UserController> logger) : StandardController
	{
		[HttpGet]
		public IActionResult Get()
		{
			logger.LogInformation("GET - Listar usuários");
			return TryMethod(service.GetAll, logger);
		}

		[HttpPost]
        public IActionResult Post([FromBody] UserRegisterDto.UserRegisterRequestDto userRegisterRequestDto)
        {
			logger.LogInformation("POST - Adicionar novo usuário");
			return TryMethod(() => service.Add(userRegisterRequestDto), logger);
        }
    }
}
