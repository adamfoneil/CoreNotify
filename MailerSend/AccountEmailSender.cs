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

public class AccountEmailSender<TUser>(
	IOptions<AccountEmailSenderOptions> options,
	MailerSendClient mailerSendClient,
	ILogger<AccountEmailSender<TUser>> logger) : IEmailSender<TUser> where TUser : IdentityUser
{
	private readonly AccountEmailSenderOptions _options = options.Value;
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;
	private readonly ILogger<AccountEmailSender<TUser>> _logger = logger;

	protected virtual string ConfirmationSubject(TUser user) => $"{_options.DomainName} - Please confirm your email";
	protected virtual string ConfirmationBody(TUser user, string confirmationLink) => $"<p>Hi {user.UserName},</p><p>Please click <a href=\"{confirmationLink}\">here</a> to confirm your email address.</p><p>If you did not register at {_options.DomainName}, please ignore this. Someone entered your email by mistake.</p>";
	protected virtual string PasswordResetCodeSubject(TUser user) => $"{_options.DomainName} - Password reset code";
	protected virtual string PasswordResetCodeBody(TUser user, string resetCode) => $"<p>Hi {user.UserName},</p><p>Your password reset code is: <strong>{resetCode}</strong></p><p>If you did not request a password reset at {_options.DomainName}, please ignore this email.</p>";
	protected virtual string PasswordResetLinkSubject(TUser user) => $"{_options.DomainName} - Password reset link";
	protected virtual string PasswordResetLinkBody(TUser user, string resetLink) => $"<p>Hi {user.UserName},</p><p>Please click <a href=\"{resetLink}\">here</a> to reset your password.</p><p>If you did not request a password reset at {_options.DomainName}, please ignore this email.</p>";

	public async Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink)
	{
		await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = ConfirmationSubject(user),
			Html = ConfirmationBody(user, confirmationLink)
		});

		_logger.LogInformation("Confirmation email sent to {email}", email);
	}

	public async Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode)
	{
		await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = PasswordResetCodeSubject(user),
			Html = PasswordResetCodeBody(user, resetCode)
		});

		_logger.LogInformation("Password reset code sent to {email}", email);
	}

	public async Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink)
	{
		await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = PasswordResetLinkSubject(user),
			Html = PasswordResetLinkBody(user, resetLink)
		});

		_logger.LogInformation("Password reset link sent to {email}", email);
	}
}
