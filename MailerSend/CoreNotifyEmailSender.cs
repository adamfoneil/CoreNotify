using CoreNotify.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MailerSend;

public class CoreNotifyOptions
{
	public string AccountEmail { get; set; } = default!;
	public string ApiKey { get; set; } = default!;
	public string DomainName { get; set; } = default!;
	public string SenderMailbox { get; set; } = default!;
}

/// <summary>
/// provides account notifications (confirmations, resets) for ASP.NET Core Identity using your CoreNotify account
/// </summary>
public class CoreNotifyEmailSender<TUser>(	
	CoreNotifyClient coreNotifyClient,
	IOptions<CoreNotifyOptions> options) : IEmailSender<TUser> where TUser : IdentityUser
{	
	private readonly CoreNotifyClient _coreNotifyClient = coreNotifyClient;
	private readonly CoreNotifyOptions _options = options.Value;

	public async Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink) =>
		await _coreNotifyClient.SendConfirmationAsync(_options.AccountEmail, _options.ApiKey, new()
		{
			UserName = user.UserName,
			Email = email,
			ConfirmationLink = confirmationLink,
			SenderMailbox = _options.SenderMailbox,
			DomainName = _options.DomainName
		});

	public async Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode) =>
		await _coreNotifyClient.SendResetCodeAsync(_options.AccountEmail, _options.ApiKey, new()
		{
			UserName = user.UserName,
			Email = email,
			Code = resetCode,
			SenderMailbox = _options.SenderMailbox,
			DomainName = _options.DomainName
		});

	public Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink) =>
		_coreNotifyClient.SendResetLinkAsync(_options.AccountEmail, _options.ApiKey, new()
		{
			UserName = user.UserName,
			Email = email,
			ResetLink = resetLink,
			SenderMailbox = _options.SenderMailbox,
			DomainName = _options.DomainName
		});
}
