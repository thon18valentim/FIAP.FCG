using FIAP.FCG.Core.Models;
using FIAP.FCG.Infra.Context;

namespace FIAP.FCG.Infra.Repository
{
	public class GameRepository(ApplicationDbContext context) : EFRepository<Game>(context), IGameRepository
	{
		public async Task<IEnumerable<Game>> GetAll()
		{
			return [.. await Get()];
		}
    }
}
