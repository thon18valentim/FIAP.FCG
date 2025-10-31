using AutoMapper;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Infra.Mapping;

public class UsersProfile : Profile
{
    public UsersProfile()
    {
        // DTO -> Entidade (Password será sobrescrita com hash no repo)
        CreateMap<UserRegisterDto, User>()
            .ForMember(d => d.IsAdmin, opt => opt.MapFrom(_ => false))
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore()); // default do modelo

        CreateMap<UserUpdateDto, User>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Entidade -> DTO de resposta
        CreateMap<User, UserResponseDto>();
    }
}
