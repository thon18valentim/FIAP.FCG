using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Infra.Repository
{
	public interface IRepository<T> where T : EntityBase
	{
		IList<T> Get();
		T Get(int id);
		void Register(T entity);
		void Edit(T entity);
		void Delete(int id);
	}
}
