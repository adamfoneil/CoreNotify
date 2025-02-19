namespace CoreNotify.Shared.Models;

public class SendAlertRequest
{
	public string Subject { get; set; } = default!;
	public string HtmlBody { get; set; } = default!;
}
