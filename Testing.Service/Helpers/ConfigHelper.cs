using Microsoft.Extensions.Configuration;

namespace Testing.Service.Helpers
{
    public static class ConfigHelper
    {
        public static IConfiguration GetValue => new ConfigurationBuilder()
            .AddUserSecrets("30e2e871-bf9f-483f-beca-2d5784b2f16a")
            .Build();
    }
}
