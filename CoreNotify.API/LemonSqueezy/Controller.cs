using CoreNotify.MailerSend;
using CoreNotify.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services;
using Services.Data;
using System.Text.Json;

namespace CoreNotify.API.LemonSqueezy;

[ApiController]
[Route("api/LemonSqueezy")]
[AllowAnonymous]
public class Controller(
	IOptions<Options> options,
	AccountService accountService,
	ILogger<Controller> logger,
	IDbContextFactory<ApplicationDbContext> dbFactory) : ControllerBase
{
	private readonly Options _options = options.Value;
	private readonly AccountService _accountService = accountService;
	private readonly ILogger<Controller> _logger = logger;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;	

	[HttpPost("OrderCreated")]
	public async Task<IActionResult> OrderCreated()
	{		
		using var logScope = _logger.BeginScope("{traceId}", HttpContext.TraceIdentifier);
		try
		{			
			if (!Request.Headers.TryGetValue("X-Signature", out var values))
			{
				_logger.LogWarning("Missing webhook secret");
				return Ok();
			}

			if (!values.FirstOrDefault()?.Equals(_options.WebhookSecret, StringComparison.OrdinalIgnoreCase) ?? true)
			{
				_logger.LogWarning("Invalid webhook secret");
				return Ok();
			}

			_logger.LogDebug("Reading body");
			var body = await new StreamReader(Request.Body).ReadToEndAsync();

			_logger.LogDebug("Deserializing body");
			var doc = JsonDocument.Parse(body);
			
			_logger.LogDebug("Finding user email");
			var userEmail = doc.RootElement
				.GetProperty("data")
				.GetProperty("attributes")
				.GetProperty("user_email")
				.GetString() ?? throw new Exception("Couldn't find user email");

			_logger.LogDebug("Finding order number");
			var orderNum = doc.RootElement
				.GetProperty("data")				
				.GetProperty("attributes")
				.GetProperty("order_number")
				.GetInt32();

			_logger.LogDebug("Finding test_mode status");
			var mode = doc.RootElement
				.GetProperty("meta")
				.GetProperty("test_mode")
				.GetBoolean() ? "Test" : "Live";

			using var db = _dbFactory.CreateDbContext();

			var account = 
				await db.Accounts.SingleOrDefaultAsync(row => row.Email == userEmail) ?? 
				await _accountService.CreateAsync(new CreateAccountRequest() { Email = userEmail }); ;

			var originalRenewalDate = account.RenewalDate;
			account.RenewalDate = account.RenewalDate.AddMonths(1);
			await db.SaveChangesAsync();

			_logger.LogInformation(
				"Set {accountEmail} renewal date from {oldDate} to {newDate} via order Id {orderNum} ({mode})", 
				userEmail, originalRenewalDate, account.RenewalDate, orderNum, mode);
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error in OrderCreated webhook");			
		}

		return Ok();
	}
}
