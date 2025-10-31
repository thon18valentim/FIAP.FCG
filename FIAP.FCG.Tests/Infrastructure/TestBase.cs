using AutoMapper;
using FIAP.FCG.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Tests
{
    public abstract class TestBase<TContext> : IDisposable where TContext : DbContext
    {
        protected readonly TContext Ctx;
        protected readonly IMapper Mapper;

        protected TestBase(Func<DbContextOptions<TContext>, TContext> ctxFactory, params Profile[] profiles)
        {
            var options = InMemoryDb.OptionsFor<TContext>();
            Ctx = ctxFactory(options);
            Mapper = TestMapper.Build(profiles);
            Ctx.Database.EnsureCreated();
        }

        public virtual void Dispose()
        {
            Ctx?.Dispose();
        }
    }
}