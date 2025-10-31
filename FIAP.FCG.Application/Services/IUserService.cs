using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Web;

namespace FIAP.FCG.Application.Services
{
	public interface IUserService
	{
		Task<IApiResponse<IEnumerable<UserResponseDto>>> GetAll();
        Task<IApiResponse<UserResponseDto?>> GetById(int id);
        Task<IApiResponse<bool>> Update(int id, UserUpdateDto userUpdateDto);
        Task<IApiResponse<bool>> Remove(int id);
    }
}
