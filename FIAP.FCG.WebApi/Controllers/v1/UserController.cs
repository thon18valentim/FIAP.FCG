using FIAP.FCG.Infra.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.WebApi.Controllers.v1
{
	[ApiController]
	[Route("v1/[controller]")]
	public class UserController(IUserRepository repository) : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			var users = repository.GetAll();
			return Ok(users);
		}
	}
}
