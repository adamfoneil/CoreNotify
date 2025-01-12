using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MailerSend;

/// <summary>
/// provides account notifications (confirmations, resets) for ASP.NET Core Identity using your MailerSend account
/// </summary>
public class GenericEmailSender<TUser>(
	string domainName,
	MailerSendClient mailerSendClient,
	ILogger<GenericEmailSender<TUser>> logger,
	EmailSenderContent<TUser>? content = null) : IEmailSender<TUser> where TUser : IdentityUser
{	
	private readonly EmailSenderContent<TUser> _content = content ?? new();
	private readonly string _domainName = domainName;
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;
	private readonly ILogger<GenericEmailSender<TUser>> _logger = logger;

	public async Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink)
	{
		var msgId = await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = _content.ConfirmationSubject(user, _domainName),
			Html = _content.ConfirmationBody(user, _domainName, confirmationLink)
		});

		_logger.LogInformation("Confirmation email {msgId} sent to {email}", msgId, email);
	}

	public async Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode)
	{
		var msgId = await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = _content.PasswordResetCodeSubject(user, _domainName),
			Html = _content.PasswordResetCodeBody(user, _domainName, resetCode)
		});

		_logger.LogInformation("Password reset code {msgId} sent to {email}", msgId, email);
	}

	public async Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink)
	{
		var msgId = await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = _content.PasswordResetLinkSubject(user, _domainName),
			Html = _content.PasswordResetLinkBody(user, _domainName, resetLink)
		});

		_logger.LogInformation("Password reset link {msgId} sent to {email}", msgId, email);
	}
}
