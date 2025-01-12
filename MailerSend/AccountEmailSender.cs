using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MailerSend;

public class AccountEmailSenderOptions
{
	public string DomainName { get; set; } = default!;
	public string ConfirmationTemplate { get; set; } = default!;
}

public class AccountEmailSender<TUser>(
	IOptions<AccountEmailSenderOptions> options,
	MailerSendClient mailerSendClient,
	ILogger<AccountEmailSender<TUser>> logger) : IEmailSender<TUser> where TUser : IdentityUser
{
	private readonly AccountEmailSenderOptions _options = options.Value;
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;
	private readonly ILogger<AccountEmailSender<TUser>> _logger = logger;

	public async Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink)
	{
		await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = $"{_options.DomainName} - Please confirm your email",
			Html = $"<p>Hi {user.UserName},</p><p>Please click <a href=\"{confirmationLink}\">here</a> to confirm your email address.</p><p>If you did not register at {_options.DomainName}, please ignore this. Someone entered your email by mistake.</p>"
		});

		_logger.LogInformation("Confirmation email sent to {email}", email);
	}

	public async Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode)
	{
		await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = $"{_options.DomainName} - Password reset code",
			Html = $"<p>Hi {user.UserName},</p><p>Your password reset code is: <strong>{resetCode}</strong></p><p>If you did not request a password reset at {_options.DomainName}, please ignore this email.</p>"
		});

		_logger.LogInformation("Password reset code sent to {email}", email);
	}

	public async Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink)
	{
		await _mailerSendClient.SendAsync(new()
		{
			To = [email],
			Subject = $"{_options.DomainName} - Password reset link",
			Html = $"<p>Hi {user.UserName},</p><p>Please click <a href=\"{resetLink}\">here</a> to reset your password.</p><p>If you did not request a password reset at {_options.DomainName}, please ignore this email.</p>"
		});

		_logger.LogInformation("Password reset link sent to {email}", email);
	}
}
