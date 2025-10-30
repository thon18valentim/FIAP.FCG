using FIAP.FCG.Core.Inputs;

namespace FIAP.FCG.Infra.Repository
{
	public interface IUserRepository
	{
		List<UserRegisterDto.UserRegisterResponseDto> GetAll();
		void Add(UserRegisterDto.UserRegisterRequestDto dto);
	}
}
