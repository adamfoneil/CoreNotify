namespace CoreNotify.Shared.Models;

public class SendAlertRequest : SendRequestBase
{
	public string Subject { get; set; } = default!;
	public string HtmlBody { get; set; } = default!;
}
