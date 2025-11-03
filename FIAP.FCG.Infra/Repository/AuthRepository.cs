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


        public string GenerateToken(IConfiguration configuration, User user)
        {
            return GenerateTokenPrivate(configuration, user);
        }

        private static string GenerateTokenPrivate(IConfiguration configuration, User user)
        {
            var key = configuration["Jwt:Key"];
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var expirationStr = configuration["Jwt:Expiration"];

            if (string.IsNullOrWhiteSpace(key))
                throw new ValidationException("JWT Key is not configured (Jwt:Key).");
            if (!int.TryParse(expirationStr, out var expirationMinutes))
                throw new ValidationException("JWT Expiration is not configured or invalid (Jwt:Expiration).");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Name),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roleNames = user.UserRoles?.Select(ur => ur.Role.Name).Distinct().ToList() ?? new List<string>();

            if (user.IsAdmin && !roleNames.Contains("Admin", StringComparer.OrdinalIgnoreCase))
                roleNames.Add("Admin");

            foreach (var role in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = credentials,
                Issuer = issuer,
                Audience = audience
            };

            var handler = new JsonWebTokenHandler();
            return handler.CreateToken(tokenDescriptor);
        }
    }
}
