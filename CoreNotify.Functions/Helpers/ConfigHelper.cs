using Microsoft.Azure.WebJobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CoreNotify.Functions.Helpers
{
    public static class ConfigHelper
    {
        public static IConfiguration GetConfig(this ExecutionContext context) => 
            new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

        public static string GetConnectionString(this ExecutionContext context, string name) => 
            GetConfig(context).GetConnectionString(name);

        public static SqlConnection GetConnection(this ExecutionContext context, string name) =>
            new SqlConnection(GetConnectionString(context, name));

        public static string GetSendGridKey(this ExecutionContext context) =>
            GetConfig(context)["SendGridApiKey"];
    }
}
