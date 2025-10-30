using FIAP.FCG.Infra.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.WebApi.Controllers.v1
{
	[ApiController]
	[Route("v1/[controller]")]
	public class GameController(IGameRepository repository) : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			var games = repository.GetAll();
			return Ok(games);
		}
	}
}
