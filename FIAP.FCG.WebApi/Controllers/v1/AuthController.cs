using FIAP.FCG.Application.Auth;
using FIAP.FCG.Core.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIAP.FCG.WebApi.Controllers.v1
{
    [AllowAnonymous]
	public class AuthController(IAuthService service, ILogger<AuthController> logger) : StandardController
	{
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDto loginDto)
		{
			return await TryMethodAsync(() => service.Login(loginDto), logger);
		}

        [HttpPost("Register")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterRequestDto)
        {
            var response = await service.Register(userRegisterRequestDto);

            if (response.StatusCode == HttpStatusCode.Created)
                return Created(string.Empty, $"Usuário {response.ResultValue} cadastrado com sucesso!");

            return StatusCode((int)response.StatusCode);
            //return await TryMethodAsync(() => service.Register(userRegisterRequestDto), logger);
        }
    }
}
