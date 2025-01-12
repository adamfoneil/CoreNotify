using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MailerSend;

/// <summary>
/// provides account notifications (confirmations, resets) for ASP.NET Core Identity using your CoreNotify account
/// </summary>
public class CoreNotifyEmailSender<TUser>(
	IHttpClientFactory httpClientFactory,
	IOptions<CoreNotifyOptions> options) : IEmailSender<TUser> where TUser : IdentityUser
{
	private readonly CoreNotifyOptions _options = options.Value;
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

	public Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink)
	{
		throw new NotImplementedException();
	}

	public Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode)
	{
		throw new NotImplementedException();
	}

	public Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink)
	{
		throw new NotImplementedException();
	}
}
