using CoreNotify.MailerSend;
using CoreNotify.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Data;
using Services.Data.Entities;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(
	ILogger<AccountController> logger,	
	MailerSendClient mailerSendClient,
	IDbContextFactory<ApplicationDbContext> dbFactory) : ControllerBase
{
	private readonly ILogger<AccountController> _logger = logger;	
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	[HttpPost("register")]
	public async Task<IActionResult> Register(CreateAccountRequest request)
	{		
		try
		{
			using var db = _dbFactory.CreateDbContext();
			var account = new Account()
			{
				Email = request.Email				
			};
			db.Accounts.Add(account);
			await db.SaveChangesAsync();

			await _mailerSendClient.SendAsync(new MailerSendClient.Message()
			{
				To = [request.Email],
				Subject = "CoreNotify Account Created",
				Html = 
					$"<p>Your account has been created with a renewal date of {account.RenewalDate:M/d/yy}. Your API key is: <strong>{account.ApiKey}</strong></p>" +
					$"<p>If you did not do this, please ignore. Someone entered your email by mistake.</p>" +
					"<p>Please see <a href=\"https://github.com/adamfoneil/CoreNotify\">https://github.com/adamfoneil/CoreNotify</a> for more info.</p>"
			});
			
			_logger.LogInformation("Account created for {email}", request.Email);

			return Ok($"An email with your API key was sent to {request.Email}");
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error creating account for {email}", request.Email);
			return Problem(exc.FullMessage());
		}
	}

	public async Task<IActionResult> Resend(CreateAccountRequest request)
	{
		try
		{
			using var db = _dbFactory.CreateDbContext();
			var account = await db.Accounts.SingleOrDefaultAsync(row => row.Email == request.Email);
			if (account is null) return NotFound();

			await _mailerSendClient.SendAsync(new MailerSendClient.Message()
			{
				To = [request.Email],
				Subject = "CoreNotify API Key",
				Html = $"<p>Your API key is: <strong>{account.ApiKey}</strong></p><p>If you did not do this, please ignore. Someone entered your email by mistake.</p>"
			});

			_logger.LogInformation("API key resent to {email}", request.Email);
			return Ok($"An email with your API key was sent to {request.Email}");
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error resending API key to {email}", request.Email);
			return Problem(exc.FullMessage());
		}
	}

	[HttpGet("usage")]
	[VerifyAccount]
	public async Task<IActionResult> Usage()
	{
		var account = HttpContext.Items["Account"] as Account ?? throw new Exception("account missing");

		using var db = _dbFactory.CreateDbContext();
		var recent = await db.DailyUsage
			.Where(row => row.AccountId == account.Id)
			.OrderByDescending(row => row.Date).Take(7).ToListAsync();

		try
		{			
			return Ok(new AccountUsageResponse()
			{
				RenewalDate = account.RenewalDate,
				RecentUsage = [.. recent.Select(row => new AccountUsageResponse.DailyUsage()
				{
					Date = row.Date,
					Confirmations = row.Confirmations,
					ResetCodes = row.ResetCodes,
					ResetLinks = row.ResetLinks,
					Alerts = row.Alerts
				})]
			});
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error getting usage for {email}", account.Email);
			return Problem(exc.FullMessage());
		}
	}
}
