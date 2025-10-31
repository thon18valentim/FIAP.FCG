using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Web;
using FIAP.FCG.Infra.Repository;

namespace FIAP.FCG.Application.Services
{
	public class UserService(IUserRepository repository) : BaseService, IUserService
	{
		public async Task<IApiResponse<IEnumerable<UserResponseDto>>> GetAll() => Success(await repository.GetAll());

        public async Task<IApiResponse<UserResponseDto?>> GetById(int id) => Success(await repository.GetById(id));

        public async Task<IApiResponse<bool>> Remove(int id) => Success(await repository.Remove(id));

        public async Task<IApiResponse<bool>> Update(int id, UserUpdateDto userUpdateDto) => Success(await repository.Update(id, userUpdateDto));
    }
}
