using FIAP.FCG.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.WebApi.Controllers.v1
{
	[Authorize]
	public class GameController(IGameService service, ILogger<GameController> logger) : StandardController
	{
		[HttpGet]
		public Task<IActionResult> Get()
		{
			logger.LogInformation("GET - Listar jogos");
			return TryMethodAsync(() => service.GetAll(), logger);
		}
	}
}
