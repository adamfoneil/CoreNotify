namespace MailerSend;

public class CoreNotifyOptions
{
	public string AccountEmail { get; set; } = default!;
	public string ApiKey { get; set; } = default!;
	public string DomainName { get; set; } = default!;
	public string SenderMailbox { get; set; } = "noreply";
}
