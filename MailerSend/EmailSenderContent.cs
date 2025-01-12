namespace MailerSend;

public class EmailSenderContent
{
	public virtual string ConfirmationSubject(string userName, string domainName) => $"{domainName} - Please confirm your email";
	public virtual string ConfirmationBody(string userName, string domainName, string confirmationLink) => $"<p>Hi {userName},</p><p>Please click <a href=\"{confirmationLink}\">here</a> to confirm your email address.</p><p>If you did not register at {domainName}, please ignore this. Someone entered your email by mistake.</p>";
	public virtual string PasswordResetCodeSubject(string userName, string domainName) => $"{domainName} - Password reset code";
	public virtual string PasswordResetCodeBody(string userName, string domainName, string resetCode) => $"<p>Hi {userName},</p><p>Your password reset code is: <strong>{resetCode}</strong></p><p>If you did not request a password reset at {domainName}, please ignore this email.</p>";
	public virtual string PasswordResetLinkSubject(string userName, string domainName) => $"{domainName} - Password reset link";
	public virtual string PasswordResetLinkBody(string userName, string domainName, string resetLink) => $"<p>Hi {userName},</p><p>Please click <a href=\"{resetLink}\">here</a> to reset your password.</p><p>If you did not request a password reset at {domainName}, please ignore this email.</p>";
}
