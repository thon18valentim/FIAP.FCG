using AutoMapper;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Infra.Mapping;

public class UsersProfile : Profile
{
    public UsersProfile()
    {
        // REGISTER: DTO -> Entity
        CreateMap<UserRegisterDto, User>()
            // Gerenciados pela aplicação/EF
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())
            // Política padrão (se não vier do DTO)
            .ForMember(d => d.IsAdmin, opt => opt.MapFrom(_ => false));
        // Password pode vir do DTO; o repo/service fará o hash antes de salvar

        // UPDATE: DTO -> Entity (aplica apenas quando vier valor)
        CreateMap<UserUpdateDto, User>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Cpf, opt => opt.Ignore())      // não permitir alterar CPF por update
            .ForMember(d => d.IsAdmin, opt => opt.Ignore())      // evitar escalonamento por esse DTO
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(d => d.Password, opt => opt.Ignore())      // se quiser trocar senha, use DTO próprio e hash no service
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Entity -> DTO de resposta
        CreateMap<User, UserResponseDto>();
    }
}
