using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Infra.Repository
{
	public interface IUserRepository
	{
		List<User> GetAll();
	}
}
