namespace CoreNotify.Shared.Models;

public class SendRequestBase
{
	public string Email { get; set; } = default!;
	public string? UserName { get; set; }
	public string SenderMailbox { get; set; } = "noreply";
	public string DomainName { get; set; } = default!;
}
