using CoreNotify.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailerSend.Extensions;

public static class ServiceCollectionExtensions
{
	public static void AddCoreNotify<TUser>(this IServiceCollection services, IConfiguration configuration) where TUser : IdentityUser
	{
		services.Configure<Options>(options => options.BaseUrl = "https://plankton-app-evwok.ondigitalocean.app");
		services.AddSingleton<CoreNotifyClient>();
		services.Configure<CoreNotifyOptions>(configuration.GetSection("CoreNotify"));
		services.AddHttpClient();
		services.AddSingleton<IEmailSender<TUser>, CoreNotifyEmailSender<TUser>>();
	}
}
