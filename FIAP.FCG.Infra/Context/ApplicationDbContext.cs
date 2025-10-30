using FIAP.FCG.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FIAP.FCG.Infra.Context
{
	public class ApplicationDbContext : DbContext
	{
		private readonly string _connectinoString;

		public ApplicationDbContext()
		{
			// precisa comentar esse código para usar o Migrations

			IConfiguration configuration = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("appsettings.json")
				.Build();

			_connectinoString = configuration.GetConnectionString("Core");
		}

		public ApplicationDbContext(string connectionString)
		{
			_connectinoString = connectionString;
		}

		public DbSet<User> User { get; set; }
		public DbSet<Game> Game { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(_connectinoString);
			optionsBuilder.UseLazyLoadingProxies();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
		}
	}
}
