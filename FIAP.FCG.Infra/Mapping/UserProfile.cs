using AutoMapper;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Infra.Mapping;

public class UsersProfile : Profile
{
    public UsersProfile()
    {
        // DTO -> Entidade (Password será sobrescrita com hash no repo)
        CreateMap<UserRegisterDto.UserRegisterRequestDto, User>()
            .ForMember(d => d.IsAdmin, opt => opt.MapFrom(_ => false))
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore()); // default do modelo

        // Entidade -> DTO de resposta
        CreateMap<User, UserRegisterDto.UserRegisterResponseDto>();
    }
}
