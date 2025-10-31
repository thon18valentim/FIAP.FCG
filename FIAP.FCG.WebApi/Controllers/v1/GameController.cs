using FIAP.FCG.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.WebApi.Controllers.v1
{
	[ApiController]
	[Route("v1/[controller]")]
	public class GameController(IGameService service, ILogger<GameController> logger) : StandardController
	{
		[HttpGet]
		public IActionResult Get()
		{
			logger.LogInformation("GET - Listar jogos");
			return TryMethod(service.GetAll, logger);
		}
	}
}
