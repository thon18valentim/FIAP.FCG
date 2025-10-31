using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Web;

namespace FIAP.FCG.Application.Auth
{
	public interface IAuthService
	{
		IApiResponse<string> Login(LoginDto dto);
	}
}
