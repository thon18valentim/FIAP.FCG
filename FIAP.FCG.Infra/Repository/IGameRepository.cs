using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Infra.Repository
{
	public interface IGameRepository
	{
		List<Game> GetAll();
	}
}
