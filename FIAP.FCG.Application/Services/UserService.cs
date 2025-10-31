using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Web;
using FIAP.FCG.Infra.Repository;

namespace FIAP.FCG.Application.Services
{
	public class UserService(IUserRepository repository) : BaseService, IUserService
	{
		public IApiResponse<bool> Add(UserRegisterDto.UserRegisterRequestDto dto)
		{
			repository.Add(dto);
			return Success();
		}

		public IApiResponse<List<UserRegisterDto.UserRegisterResponseDto>> GetAll()
		{
			var users = repository.GetAll();
			return Success(users);
		}
	}
}
