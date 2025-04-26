using CoreNotify.MailerSend;
using CoreNotify.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Data;
using Services.Data.Entities;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(
	AccountService accountService,
	ILogger<AccountController> logger,	
	MailerSendClient mailerSendClient,
	IDbContextFactory<ApplicationDbContext> dbFactory) : ControllerBase
{
	private readonly AccountService _accountService = accountService;
	private readonly ILogger<AccountController> _logger = logger;	
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	[HttpPost("register")]
	public async Task<IActionResult> Register(CreateAccountRequest request)
	{		
		try
		{
			await _accountService.CreateAsync(request);

			return Ok($"An email with your API key was sent to {request.Email}");
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error creating account for {email}", request.Email);
			return Problem(exc.FullMessage());
		}
	}

	[HttpPost("resend")]
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

	[HttpPost("recycle")]
	[VerifyAccount]
	public async Task<IActionResult> Recycle()
	{
		var account = HttpContext.Items["Account"] as Account ?? throw new Exception("account missing");

		try
		{
			using var db = _dbFactory.CreateDbContext();
			var acct = await db.Accounts.SingleOrDefaultAsync(row => row.Email == account.Email) ?? throw new Exception($"Account not found: {account.Email}");

			acct.ApiKey = ApiKey.Generate(Account.KeyLength);
			await db.SaveChangesAsync();

			await _mailerSendClient.SendAsync(new MailerSendClient.Message()
			{
				To = [account.Email],
				Subject = "CoreNotify API Key - Recycle",
				Html = $"<p>Your new API key is: <strong>{acct.ApiKey}</strong></p>"
			});

			_logger.LogInformation("Recycle API key sent to {email}", account.Email);
			return Ok($"An email with your API key was sent to {account.Email}");
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error recycling account for {email}", account.Email);
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
