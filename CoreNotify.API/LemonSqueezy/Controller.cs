using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Data;

namespace CoreNotify.API.LemonSqueezy;

[ApiController]
[Route("api/LemonSqueezy")]
[AllowAnonymous]
public class Controller(
	ILogger<Controller> logger,
	IDbContextFactory<ApplicationDbContext> dbFactory) : ControllerBase
{
	private readonly ILogger<Controller> _logger = logger;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	[HttpPost]
	public async Task<IActionResult> OrderCreated(Request request)
	{
		using var logScope = _logger.BeginScope("{id}", request.data.id);
		_logger.LogInformation("OrderCreated webhook received");

		try
		{

		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error in OrderCreated webhook");			
		}

		return Ok();
	}
}
