﻿namespace API.Shared.Models;

public class SendConfirmationRequest : SendRequestBase
{
	public string ConfirmationLink { get; set; } = default!;
}
