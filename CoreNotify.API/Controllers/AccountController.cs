using API.Shared.Models;
using CoreNotify.API.Data;
using CoreNotify.API.Data.Entities;
using MailerSend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
				Email = request.Email,
				DomainName = request.DomainName
			};
			db.Accounts.Add(account);
			await db.SaveChangesAsync();

			await _mailerSendClient.SendAsync(new MailerSendClient.Message()
			{
				To = [request.Email],
				Subject = "CoreNotify Account Created",
				Html = $"<p>Your account has been created. Your API key is: {account.ApiKey}</p>"
			});

			// todo: domain validation?
			_logger.LogInformation("Account created for {email}", request.Email);

			return Ok($"An email with your API key was sent to {request.Email}");
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error creating account for {email}", request.Email);
			return Problem(exc.FullMessage());
		}
	}
}
