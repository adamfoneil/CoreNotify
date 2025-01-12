namespace MailerSend;

public class CoreNotifyOptions
{
	/// <summary>
	/// your application domain name to appear in outgoing emails
	/// </summary>
	public string DomainName { get; set; } = default!;
	public string ApiKey { get; set; } = default!;
}
