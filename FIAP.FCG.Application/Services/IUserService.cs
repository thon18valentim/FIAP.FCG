using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Web;

namespace FIAP.FCG.Application.Services
{
	public interface IUserService
	{
		IApiResponse<List<UserRegisterDto.UserRegisterResponseDto>> GetAll();
		IApiResponse<bool> Add(UserRegisterDto.UserRegisterRequestDto dto);
	}
}
