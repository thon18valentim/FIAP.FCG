using FIAP.FCG.Application.Services;
using FIAP.FCG.Core.Inputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.WebApi.Controllers.v1
{
    [Authorize]
    public class GameController(IGameService service, ILogger<GameController> logger) : StandardController
    {
        [HttpPost("RegisterGame")]
        public Task<IActionResult> Post([FromBody] GameRegisterDto register)
        {
            logger.LogInformation("POST - Criar jogo");
            return TryMethodAsync(() => service.Create(register), logger);
        }

        [HttpGet("GetAllGames")]
        public Task<IActionResult> Get()
        {
            try
            {
                logger.LogInformation("GET - Listar jogos");
                return TryMethodAsync(() => service.GetAll(), logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao listar jogos");
                throw;
            }
        }
        [HttpGet("GetGameById{id}")]
        public Task<IActionResult> GetById(int id)
        {
            try
            {
                logger.LogInformation("GET - Listar jogo por ID: {Id}", id);
                return TryMethodAsync(() => service.GetById(id), logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao listar jogo por ID: {Id}", id);
                throw;
            }
        }

        [HttpPut("UpdateGame/{id:int}")]
        public Task<IActionResult> Put(int id, [FromBody] GameUpdateDto update)
        {
            logger.LogInformation("PUT - Atualizar jogo com ID: {Id}", id);
            return TryMethodAsync(() => service.Update(id, update), logger);
        }

        [HttpDelete("DeleteGame/{id:int}")]
        public Task<IActionResult> Delete(int id)
        {
            logger.LogInformation("DELETE - Excluir jogo com ID: {Id}", id);
            return TryMethodAsync(() => service.Remove(id), logger);
        }
    }
}
