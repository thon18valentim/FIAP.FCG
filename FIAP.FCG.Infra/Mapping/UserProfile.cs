using AutoMapper;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Infra.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // REGISTER: DTO -> Entity
        CreateMap<UserRegisterDto, User>()
            // Gerenciados pela aplicação/EF
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())
            // Ignora navegação controlada pelo EF/repo
            .ForMember(d => d.UserRoles, opt => opt.Ignore())
            // Política padrão (se não vier do DTO)
            .ForMember(d => d.IsAdmin, opt => opt.MapFrom(_ => false));
        // Password pode vir do DTO; o repo/service fará o hash antes de salvar

        // UPDATE: DTO -> Entity (aplica apenas quando vier valor)
        CreateMap<UserUpdateDto, User>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Cpf, opt => opt.Ignore())      // não permitir alterar CPF por update
            .ForMember(d => d.IsAdmin, opt => opt.Ignore())  // evitar escalonamento por esse DTO
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(d => d.Password, opt => opt.Ignore()) // troca de senha via DTO próprio + hash
            .ForMember(d => d.UserRoles, opt => opt.Ignore())// navegação gerenciada fora do AutoMapper
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Entity -> DTO de resposta
        CreateMap<User, UserResponseDto>();
    }
}
