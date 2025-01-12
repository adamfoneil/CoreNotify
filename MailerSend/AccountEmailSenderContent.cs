using Microsoft.AspNetCore.Identity;

namespace MailerSend;

public class AccountEmailSenderContent<TUser> where TUser : IdentityUser
{
	public virtual string ConfirmationSubject(TUser user, string domainName) => $"{domainName} - Please confirm your email";
	public virtual string ConfirmationBody(TUser user, string domainName, string confirmationLink) => $"<p>Hi {user.UserName},</p><p>Please click <a href=\"{confirmationLink}\">here</a> to confirm your email address.</p><p>If you did not register at {domainName}, please ignore this. Someone entered your email by mistake.</p>";
	public virtual string PasswordResetCodeSubject(TUser user, string domainName) => $"{domainName} - Password reset code";
	public virtual string PasswordResetCodeBody(TUser user, string domainName, string resetCode) => $"<p>Hi {user.UserName},</p><p>Your password reset code is: <strong>{resetCode}</strong></p><p>If you did not request a password reset at {domainName}, please ignore this email.</p>";
	public virtual string PasswordResetLinkSubject(TUser user, string domainName) => $"{domainName} - Password reset link";
	public virtual string PasswordResetLinkBody(TUser user, string domainName, string resetLink) => $"<p>Hi {user.UserName},</p><p>Please click <a href=\"{resetLink}\">here</a> to reset your password.</p><p>If you did not request a password reset at {domainName}, please ignore this email.</p>";
}
