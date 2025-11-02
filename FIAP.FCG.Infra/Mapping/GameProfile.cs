using AutoMapper;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Infra.Mapping;

public class GameProfile : Profile
{
    public GameProfile()
    {
        // REGISTER: DTO -> Entity
        CreateMap<GameRegisterDto, Game>()
            // Gerenciados pela aplicação/EF
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore());
        // Password pode vir do DTO; o repo/service fará o hash antes de salvar

        // UPDATE: DTO -> Entity (aplica apenas quando vier valor)
        CreateMap<GameUpdateDto, Game>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Entity -> DTO de resposta
        CreateMap<Game, GameResponseDto>();
    }
}
