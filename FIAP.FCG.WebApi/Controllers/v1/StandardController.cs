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

                // 204 não deve ter body
                if (result.StatusCode == HttpStatusCode.NoContent)
                    return StatusCode((int)HttpStatusCode.NoContent);

                if (!result.IsSuccess)
                    return StatusCode((int)result.StatusCode, result);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado no TryMethodAsync");
                var problem = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Erro interno no servidor",
                    Detail = "Ocorreu um erro inesperado ao processar sua requisição."
                };
                return StatusCode(problem.Status.Value, problem);
            }
        }

        protected IActionResult TryMethod<TResult>(
            Func<IApiResponse<TResult>> serviceMethod,
            ILogger logger)
        {
            try
            {
                var result = serviceMethod();

                if (result.StatusCode == HttpStatusCode.NoContent)
                    return StatusCode((int)HttpStatusCode.NoContent);

                if (!result.IsSuccess)
                    return StatusCode((int)result.StatusCode, result);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado no TryMethod");
                var problem = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Erro interno no servidor",
                    Detail = "Ocorreu um erro inesperado ao processar sua requisição."
                };
                return StatusCode(problem.Status.Value, problem);
            }
        }

        private static string ToDefaultTitle(HttpStatusCode code) => code switch
        {
            HttpStatusCode.BadRequest => "Requisição inválida",
            HttpStatusCode.Unauthorized => "Não autorizado",
            HttpStatusCode.Forbidden => "Proibido",
            HttpStatusCode.NotFound => "Recurso não encontrado",
            HttpStatusCode.Conflict => "Conflito",
            HttpStatusCode.UnprocessableEntity => "Entidade inválida",
            _ => "Erro"
        };
    }
}
