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
        [HttpPost]
        public IActionResult Post([FromBody] UserRegisterDto.UserRegisterRequestDto userRegisterRequestDto)
        {
            logger.LogInformation("POST - Adicionar novo usuário");
            return TryMethod(() => service.Add(userRegisterRequestDto), logger);
        }
        
        [HttpGet("GetAllUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            logger.LogInformation("GET - Listar usuários");
            return TryMethod(service.GetAll, logger);
        }
        [HttpGet("GetUserById/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var users = await repository.GetById(id);
            return Ok(users);
        }

        [HttpPut("UpdateUser/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)] // ValidationException etc.
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]   // violação unique/NOT NULL (DbUpdateException)
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            await repository.Update(id, dto);
            return NoContent();
        }

        [HttpDelete("DeleteUser/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Remove(int id)
        {
            await repository.Remove(id);
            return NoContent();
        }
	}
}
