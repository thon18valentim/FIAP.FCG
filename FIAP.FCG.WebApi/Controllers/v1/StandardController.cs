using FIAP.FCG.Core.Web;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FIAP.FCG.WebApi.Controllers.v1
{
	[ApiController]
	[Route("v1/[controller]")]
	public class StandardController : ControllerBase
	{
        protected async Task<IActionResult> TryMethodAsync<TResult>(
            Func<Task<IApiResponse<TResult>>> serviceMethod,
            ILogger logger)
        {
            try
            {
                var result = await serviceMethod();
                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                // Opcional: retornar ProblemDetails consistente
                var problem = new ProblemDetails
                {
                    Title = "Erro interno no servidor",
                    Detail = ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };

                return StatusCode(problem.Status.Value, problem);
            }
        }
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

                var problem = new ProblemDetails
                {
                    Title = "Erro interno no servidor",
                    Detail = ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };

                return StatusCode(HttpStatusCode.InternalServerError.GetHashCode(), problem);
			}
		}
	}
}
