using FIAP.FCG.Core.Inputs;
using FIAP.FCG.Core.Models;

namespace FIAP.FCG.Tests.Builders
{
    public static class GameBuilder
    {
        public static GameRegisterDto NewRegisterDto(
            string name = "Game Test",
            string platform = "PC",
            string publisherName = "Publisher Test",
            string description = "Game Description",
            double price = 199.99)
            => new GameRegisterDto { Name = name, Description = description, Platform = platform, Price = price, PublisherName = publisherName};

        public static GameResponseDto NewResponseDto(
            int id = 1,
            string name = "Game Test",
            string platform = "PC",
            string publisherName = "Publisher Test",
            string description = "Game Description",
            double price = 199.99)
            => new GameResponseDto(id, name, platform, publisherName, description, price, DateTime.UtcNow);


        public static LoginDto NewLogin(string email, string password)
            => new LoginDto { Email = email, Password = password };

        public static Game NewEntity(
            string name = "Game Test",
            string platform = "PC",
            string publisherName = "Publisher Test",
            string description = "Game Description",
            double price = 199.99)
            => new Game { Name = name, Description = description, Platform = platform, Price = price, PublisherName = publisherName };
    }
}