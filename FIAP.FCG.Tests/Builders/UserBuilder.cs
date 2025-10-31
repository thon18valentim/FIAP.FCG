using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Tests.Builders
{
    public static class UserBuilders
    {
        public static UserRegisterDto NewRegisterDto(
            string email = "user@mail.com",
            string cpf = "12345678909",
            string password = "SenhaForte@123",
            string name = "User Test")
            => new UserRegisterDto { Email = email, Cpf = cpf, Password = password, Name = name };

        public static LoginDto NewLogin(string email, string password)
            => new LoginDto { Email = email, Password = password };

        public static User NewEntity(
            string email = "user@mail.com",
            string cpf = "12345678909",
            string address = "R. Teste",
            string passwordHash = "HASH",
            string name = "User Test")
            => new User { Email = email, Cpf = cpf, Password = passwordHash, Name = name, Address = address, IsAdmin = false };
    }
}