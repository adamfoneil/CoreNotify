using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class WebhookController(
	ILogger<WebhookController> logger,
	WebhookHandler handler) : ControllerBase
{
	private readonly ILogger<WebhookController> _logger = logger;
	private readonly WebhookHandler _handler = handler;

	[HttpPost("bounce")]
	public async Task<IActionResult> Bounce()
	{
		try
		{
			var model = await Request.ReadFromJsonAsync<Bounce>() ?? throw new Exception("Couldn't deserialize bounce model");
			_handler.AddBounce(model);
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error processing bounce");			
		}

		return Ok();
	}
}
