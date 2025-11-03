using System.ComponentModel.DataAnnotations;
using FIAP.FCG.Infra.Repository;
using FIAP.FCG.Core.Validation;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Web;

namespace FIAP.FCG.Application.Services
{
	public class GameService(IGameRepository repository) : BaseService, IGameService
	{
        private readonly IGameRepository _repository = repository;

        public async Task<IApiResponse<int>> Create(GameRegisterDto gameRegisterDto)
        {
            try
            {
                DtoValidator.ValidateObject(gameRegisterDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest<int>($"Dados de jogo inválidos: {ex.Message}");
            }
            
            var id = await _repository.Create(gameRegisterDto);
            return Created(id, "Jogo registrado com sucesso.");
        }

        public async Task<IApiResponse<bool>> Remove(int id)
        {
            var removed =  await _repository.Remove(id);
            return removed
                ? NoContent()
                : NotFound<bool>("Jogo não encontrado para remoção.");
        }

        public async Task<IApiResponse<IEnumerable<GameResponseDto>>> GetAll() => Ok(await _repository.GetAll());

        public async Task<IApiResponse<GameResponseDto?>> GetById(int id)
        {
            var dto = await _repository.GetById(id);
            return dto is null
                ? NotFound<GameResponseDto?>("Jogo não encontrado.")
                : Ok<GameResponseDto?>(dto);
        }

        public async Task<IApiResponse<bool>> Update(int id, GameUpdateDto updateDto)
        {
            var ok = await _repository.Update(id, updateDto);
            return ok
                ? NoContent()
                : NotFound<bool>("Usuário não encontrado para atualização.");
        }
    }
}
