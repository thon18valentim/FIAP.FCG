using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;
using FIAP.FCG.Core.Web;

namespace FIAP.FCG.Application.Services
{
	public interface IGameService
	{
		Task<IApiResponse<IEnumerable<GameResponseDto>>> GetAll();
		Task<IApiResponse<GameResponseDto>> GetById(int id);
		Task<IApiResponse<GameRegisterDto>> Create(GameRegisterDto register);
		Task<IApiResponse<bool>> Update(int id, GameUpdateDto update);
		Task<IApiResponse<bool>> Remove(int id);
    }
}
