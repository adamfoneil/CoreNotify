using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Models;
using System.Collections.Concurrent;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class WebhookController(
	ILogger<WebhookController> logger,
	ConcurrentQueue<Bounce> bounceQueue) : ControllerBase
{
	private readonly ILogger<WebhookController> _logger = logger;
	private readonly ConcurrentQueue<Bounce> _bounceQueue = bounceQueue;	

	[HttpPost("bounce")]
	public async Task<IActionResult> Bounce()
	{
		try
		{
			var model = await Request.ReadFromJsonAsync<Bounce>() ?? throw new Exception("Couldn't deserialize bounce model");
			_bounceQueue.Enqueue(model);
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error processing bounce");			
		}

		return Ok();
	}
}
