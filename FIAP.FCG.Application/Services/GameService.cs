using FIAP.FCG.Core.Models;
using FIAP.FCG.Core.Web;
using FIAP.FCG.Infra.Repository;

namespace FIAP.FCG.Application.Services
{
	public class GameService(IGameRepository repository) : BaseService, IGameService
	{
		public IApiResponse<List<Game>> GetAll()
		{
			return Success(repository.GetAll());
		}
	}
}
