using CoreNotify.API.Data;
using CoreNotify.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class WebhookController(
	ILogger<WebhookController> logger,
	IDbContextFactory<ApplicationDbContext> dbFactory) : ControllerBase
{
	private readonly ILogger<WebhookController> _logger = logger;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	[HttpPost("bounce")]
	public async Task<IActionResult> Bounce()
	{
		try
		{
			var model = await Request.ReadFromJsonAsync<Bounce>();
			using var db = _dbFactory.CreateDbContext();
			var msgId = model!.Data.Id;
			_logger.LogInformation("Searching for messageId {messageId}", msgId);
			var message = await db.SentMessages.SingleOrDefaultAsync(row => row.MessageId == msgId) ?? throw new Exception($"MessageId {msgId} not found");
			message.Bounced = true;
			message.BounceDateTime = DateTime.Now;
			await db.SaveChangesAsync();
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error processing bounce");			
		}

		return Ok();
	}
}
