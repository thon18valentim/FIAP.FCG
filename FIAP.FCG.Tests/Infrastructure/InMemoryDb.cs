using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Tests.Infrastructure
{
    public static class InMemoryDb
    {
        public static DbContextOptions<TContext> OptionsFor<TContext>(string? nameSuffix = null)
            where TContext : DbContext
        {
            var dbName = $"{typeof(TContext).Name}_{Guid.NewGuid():N}" +
                         (string.IsNullOrWhiteSpace(nameSuffix) ? "" : $"_{nameSuffix}");

            return new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(dbName)
                .EnableSensitiveDataLogging()
                .Options;
        }
    }
}
