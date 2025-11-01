using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;
using FIAP.FCG.Core.Web;
using FIAP.FCG.Infra.Repository;

namespace FIAP.FCG.Application.Services
{
	public class GameService(IGameRepository repository) : BaseService, IGameService
	{
        public async Task<IApiResponse<GameRegisterDto>> Create(GameRegisterDto gameRegister) => Success(await repository.Create(gameRegister));

        public async Task<IApiResponse<bool>> Remove(int id) => Success(await repository.Remove(id));

        public async Task<IApiResponse<IEnumerable<GameResponseDto>>> GetAll() => Success(await repository.GetAll());

        public async Task<IApiResponse<GameResponseDto>> GetById(int id) => Success(await repository.GetById(id));

        public async Task<IApiResponse<bool>> Update(int id, GameUpdateDto update) => Success(await repository.Update(id, update));

    }
}
