namespace API.Shared.Models;

public class SendConfirmation
{
	public class Request
	{
		public string UserName { get; set; } = default!;
		public string SenderMailbox { get; set; } = "noreply";
		public string Email { get; set; } = default!;
		public string DomainName { get; set; } = default!;
		public string ConfirmationLink { get; set; } = default!;
	}

	public class Response
	{
		public string MessageId { get; set; } = default!;
	}
}
