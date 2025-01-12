namespace API.Shared.Models;

public class CreateAccountRequest
{
	public string Email { get; set; } = default!;
	public string DomainName { get; set; } = default!;
}
