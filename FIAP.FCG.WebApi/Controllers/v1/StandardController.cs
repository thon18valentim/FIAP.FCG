using FIAP.FCG.Core.Web;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.EntityFrameworkCore;

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
            // ✅ mapeia erros conhecidos COM a mensagem real
            catch (ValidationException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.BadRequest, ex);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.BadRequest, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.Unauthorized, ex);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.NotFound, ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, ex.Message);
                // Conflito de integridade, chave única, FK, etc.
                return CreateProblem(HttpStatusCode.Conflict, ex);
            }
            // ✅ fallback genérico mantém seu comportamento atual
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado no TryMethodAsync");
                return CreateProblem(HttpStatusCode.InternalServerError, ex, genericOnProd: true);
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
            catch (ValidationException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.BadRequest, ex);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.BadRequest, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.Unauthorized, ex);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, ex.Message);
                return CreateProblem(HttpStatusCode.NotFound, ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, ex.Message);
                return CreateProblem(HttpStatusCode.Conflict, ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado no TryMethod");
                return CreateProblem(HttpStatusCode.InternalServerError, ex, genericOnProd: true);
            }
        }

        private IActionResult CreateProblem(HttpStatusCode code, Exception ex, bool genericOnProd = false)
        {
            var problem = new ProblemDetails
            {
                Status = (int)code,
                Title = ToDefaultTitle(code),
                Detail = ex.Message
            };

            // útil para correlação em logs
            problem.Extensions["traceId"] = HttpContext?.TraceIdentifier;

            return StatusCode(problem.Status.Value, problem);
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
