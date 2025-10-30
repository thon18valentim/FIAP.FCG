using AutoMapper;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;
using FIAP.FCG.Core.Validation;
using FIAP.FCG.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace FIAP.FCG.Infra.Repository
{
    public class UserRepository(ApplicationDbContext context, IMapper mapper) : EFRepository<User>(context), IUserRepository
    {
        private readonly IMapper _mapper = mapper;

        public void Add(UserRegisterDto.UserRegisterRequestDto dto)
        {
            DtoValidator.ValidateObject(dto);

            if (_dbSet.AsNoTracking().Any(u => u.Email == dto.Email))
                throw new ValidationException("E-mail já cadastrado.");

            var entity = _mapper.Map<User>(dto);

            entity.Password = HashPassword(dto.Password);

            Register(entity);
        }

        public List<UserRegisterDto.UserRegisterResponseDto> GetAll()
        {
            var users = _dbSet.AsNoTracking().ToList();
            return [.. users.Select(u => _mapper.Map<UserRegisterDto.UserRegisterResponseDto>(u))];
        }

        private static string HashPassword(string raw)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(bytes);
        }
    }
}
