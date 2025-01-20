namespace CoreNotify.Shared.Models;

public class SendResetCodeRequest : SendRequestBase
{
	public string Code { get; set; } = default!;
}
