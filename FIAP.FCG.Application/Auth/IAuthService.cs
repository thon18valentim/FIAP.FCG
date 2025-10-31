using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Web;
using System.Net;

namespace FIAP.FCG.Application.Auth
{
	public interface IAuthService
	{
		Task<IApiResponse<string>> Login(LoginDto dto);
        Task<IApiResponse<HttpStatusCode>> Register(UserRegisterDto dto);
    }
}
