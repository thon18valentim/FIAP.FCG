using FIAP.FCG.Core.Models;
using FIAP.FCG.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infra.Repository
{
	public class EFRepository<T> : IRepository<T> where T : EntityBase
	{
		protected ApplicationDbContext _context;
		protected DbSet<T> _dbSet;

		public EFRepository(ApplicationDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<T>();
		}

		public void Edit(T entity)
		{
			_dbSet.Update(entity);
			_context.SaveChanges();
		}

		public void Register(T entity)
		{
			_dbSet.Add(entity);
			_context.SaveChanges();
		}

		public void Delete(int id)
		{
			_dbSet.Remove(Get(id));
			_context.SaveChanges();
		}

		public T Get(int id)
		{
			return _dbSet.FirstOrDefault(entity => entity.Id == id);
		}

		public IList<T> Get()
		{
			return _dbSet.ToList();
		}
	}
}
