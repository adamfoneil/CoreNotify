namespace API.Shared.Models;

public class CreateAccount
{
	public class Request
	{
		public string Email { get; set; } = default!;
		public string DomainName { get; set; } = default!;
	}

	public class Response
	{
		public string ApiKey { get; set; } = default!;
	}
}
