﻿namespace CoreNotify.Shared.Models;

public class SendResetLinkRequest : SendRequestBase
{
	public string ResetLink { get; set; } = default!;
}
