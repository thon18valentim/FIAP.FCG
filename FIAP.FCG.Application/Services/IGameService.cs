using FIAP.FCG.Core.Models;
using FIAP.FCG.Core.Web;

namespace FIAP.FCG.Application.Services
{
	public interface IGameService
	{
		Task<IApiResponse<IEnumerable<Game>>> GetAll();
	}
}
