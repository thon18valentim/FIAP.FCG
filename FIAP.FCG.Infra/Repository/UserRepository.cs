using FIAP.FCG.Core.Models;
using FIAP.FCG.Infra.Context;

namespace FIAP.FCG.Infra.Repository
{
	public class UserRepository(ApplicationDbContext context) : EFRepository<User>(context), IUserRepository
	{
		public List<User> GetAll()
		{
			return [.. Get()];
		}
	}
}
