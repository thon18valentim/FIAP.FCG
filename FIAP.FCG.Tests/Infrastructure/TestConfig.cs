using Microsoft.Extensions.Configuration;

namespace FIAP.FCG.Tests.Infrastructure
{
    public static class TestConfig
    {
        public static IConfiguration Build() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "super_secret_test_key_1234567890",
                    ["Jwt:Issuer"] = "fcg-tests",
                    ["Jwt:Audience"] = "fcg-tests",
                    ["Jwt:Expiration"] = "60"
                })
                .Build();
    }
}