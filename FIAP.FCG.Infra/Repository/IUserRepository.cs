using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Infra.Repository
{
	public interface IUserRepository
	{
		Task<IEnumerable<UserResponseDto>> GetAll();
        Task<UserResponseDto?> GetById(int id);
		Task<bool> Update(int id, UserUpdateDto userUpdateDto);
		Task<bool> Remove(int id);
    }
}
