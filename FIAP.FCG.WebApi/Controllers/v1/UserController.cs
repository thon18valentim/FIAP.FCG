using Microsoft.AspNetCore.Mvc;
using FIAP.FCG.Core.Inputs;
using System.ComponentModel;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace FIAP.FCG.WebApi.Controllers.v1
{
    [Authorize]
    public class UserController(IUserService service, ILogger<UserController> logger) : StandardController
    {
        [HttpGet("GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            logger.LogInformation("GET - Listar usuários");
            return await TryMethodAsync(() => service.GetAll(), logger);
        }

        [HttpGet("GetUserById/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            logger.LogInformation($"GET BY ID - Listar usuário de ID: {id}");
            return await TryMethodAsync(() => service.GetById(id), logger);
        }

        [HttpPut("UpdateUser/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)] // ValidationException etc.
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]   // violação unique/NOT NULL (DbUpdateException)
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            logger.LogInformation($"PUT - Atualizar usuário de ID: {id}");
            return await TryMethodAsync(() => service.Update(id, dto), logger);
        }

        [HttpDelete("DeleteUser/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Remove(int id)
        {
            logger.LogInformation($"DELETE - Remover usuário de ID: {id}");
            return await TryMethodAsync(() => service.Remove(id), logger);
        }
	}
}
