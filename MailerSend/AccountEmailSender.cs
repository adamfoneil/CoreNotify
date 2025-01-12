using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MailerSend;

public class AccountEmailSenderOptions
{
	/// <summary>
	/// your application domain name to appear in outgoing emails
	/// </summary>
	public string DomainName { get; set; } = default!;
}

/// <summary>
/// provides account notifications for ASP.NET Core Identity
/// </summary>
public class AccountEmailSender<TUser>(
	IOptions<AccountEmailSenderOptions> options,
	MailerSendClient mailerSendClient,
	ILogger<AccountEmailSender<TUser>> logger,
	AccountEmailSenderContent<TUser>? content = null) : IEmailSender<TUser> where TUser : IdentityUser
{
	private readonly AccountEmailSenderOptions _options = options.Value;
	private readonly AccountEmailSenderContent<TUser> _content = content ?? new();
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;
	private readonly ILogger<AccountEmailSender<TUser>> _logger = logger;

	public async Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink)
	{
		var msgId = await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = _content.ConfirmationSubject(user, _options.DomainName),
			Html = _content.ConfirmationBody(user, _options.DomainName, confirmationLink)
		});

		_logger.LogInformation("Confirmation email {msgId} sent to {email}", msgId, email);
	}

	public async Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode)
	{
		var msgId = await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = _content.PasswordResetCodeSubject(user, _options.DomainName),
			Html = _content.PasswordResetCodeBody(user, _options.DomainName, resetCode)
		});

		_logger.LogInformation("Password reset code {msgId} sent to {email}", msgId, email);
	}

	public async Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink)
	{
		var msgId = await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = _content.PasswordResetLinkSubject(user, _options.DomainName),
			Html = _content.PasswordResetLinkBody(user, _options.DomainName, resetLink)
		});

		_logger.LogInformation("Password reset link {msgId} sent to {email}", msgId, email);
	}
}
