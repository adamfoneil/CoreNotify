using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;

[assembly: FunctionsStartup(typeof(CoreNotify.Functions.Startup))]
namespace CoreNotify.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            //builder.Services.AddHttpClient<ISendGridClient, SendGridClient>();
        }
    }
}
