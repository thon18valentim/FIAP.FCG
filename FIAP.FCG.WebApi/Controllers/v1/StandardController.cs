using FIAP.FCG.Core.Web;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIAP.FCG.WebApi.Controllers.v1
{
	[ApiController]
	[Route("v1/[controller]")]
	public class StandardController : ControllerBase
	{
		protected IActionResult TryMethod<TResult>(Func<IApiResponse<TResult>> serviceMethod, ILogger logger)
		{
			try
			{
				var result = serviceMethod();
				return StatusCode(result.StatusCode.GetHashCode(), result);
			}
			catch (Exception ex)
			{
				logger.LogError(exception: ex, message: ex.Message);
				return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), ex?.Message);
			}
		}
	}
}
