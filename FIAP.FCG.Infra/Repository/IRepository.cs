using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Infra.Repository
{
	public interface IRepository<T> where T : EntityBase
	{
		Task<IEnumerable<T>> Get();
		Task<T?> Get(int id);
		Task<bool> Register(T entity);
		Task<bool> Edit(T entity);
		Task<bool> Delete(int id);
	}
}
