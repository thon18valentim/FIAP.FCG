using AutoMapper;
using FIAP.FCG.Infra.Mapping;
using Microsoft.Extensions.Logging;

namespace FIAP.FCG.Tests.Infrastructure
{
    public static class TestMapper
    {
        public static IMapper Build(params Profile[] profiles)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var cfg = new MapperConfiguration(exp =>
            {
                foreach (var p in profiles) exp.AddProfile(p);
                    exp.AddProfile(new UsersProfile());
                // Se não tiver Profiles: configurar maps mínimos aqui
                // exp.CreateMap<UserRegisterDto, User>();
            }, loggerFactory);
            cfg.AssertConfigurationIsValid();
            return cfg.CreateMapper();
        }
    }
}
