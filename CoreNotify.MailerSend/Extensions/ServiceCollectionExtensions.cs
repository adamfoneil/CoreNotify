using CoreNotify.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreNotify.MailerSend.Extensions;

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

	public static void AddCoreNotifyGenericEmailSender<TUser>(this IServiceCollection services, string domain, IConfiguration? config = null, string? mailerConfigSection = null, EmailSenderContent? content = null) where TUser : IdentityUser
	{
		services.AddHttpClient();

		if (config is not null)
		{
			services.Configure<MailerSendOptions>(config.GetSection(mailerConfigSection ?? "MailerSend"));
		}

		services.AddSingleton<MailerSendClient>();

		services.AddSingleton<IEmailSender<TUser>, GenericEmailSender<TUser>>(
			sp => new GenericEmailSender<TUser>(
				domain,
				sp.GetRequiredService<MailerSendClient>(),
				sp.GetRequiredService<ILogger<GenericEmailSender<TUser>>>(),
				content));
	}
}
