using AutoMapper;
using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;
using FIAP.FCG.Core.Validation;
using FIAP.FCG.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FIAP.FCG.Infra.Repository
{
    public class AuthRepository(ApplicationDbContext context, IMapper mapper) : EFRepository<User>(context), IAuthRepository
    {
        private readonly IMapper _mapper = mapper;

        public async Task<int> Create(UserRegisterDto dto)
        {
            if (await _dbSet.AsNoTracking().AnyAsync(u => u.Email == dto.Email))
                throw new ValidationException("E-mail já cadastrado.");

            if(await _dbSet.AsNoTracking().AnyAsync(u => u.Cpf == dto.Cpf))
                throw new ValidationException("CPF já cadastrado.");

            var entity = _mapper.Map<User>(dto);

            entity.Password = HashPassword(dto.Password);

            await Register(entity);
            return entity.Id;
        }

        public async Task<User?> FindByCredentialsAsync(LoginDto dto)
        {
            var hashedPassword = HashPassword(dto.Password);
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == hashedPassword);
        }

        public static string HashPassword(string raw)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(bytes);
        }

        public string GenerateToken(IConfiguration configuration)
        {
            return GenerateTokenPrivate(configuration);
        }

        private static string GenerateTokenPrivate(IConfiguration configuration)
        {
            var secretKey = configuration["Jwt:Key"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(JwtRegisteredClaimNames.Sid, "123"),
                    new Claim(JwtRegisteredClaimNames.NameId, "username"),
                    new Claim(JwtRegisteredClaimNames.Email, "user@mail.com")
                    ]),
                Expires = DateTime.Now.AddMinutes(int.Parse(configuration["Jwt:Expiration"]!)),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"]
            };

            var handler = new JsonWebTokenHandler();
            return handler.CreateToken(tokenDescriptor);
        }
    }
}
