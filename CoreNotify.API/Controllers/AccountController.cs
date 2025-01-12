using API.Shared.Models;
using CoreNotify.API.Data;
using CoreNotify.API.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(
	ILogger<AccountController> logger,
	IDbContextFactory<ApplicationDbContext> dbFactory) : ControllerBase
{
	private readonly ILogger<AccountController> _logger = logger;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	[HttpPost("register")]
	public async Task<IActionResult> Register(CreateAccount.Request request)
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
			return Ok(new CreateAccount.Response() { ApiKey = account.ApiKey });
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error creating account");
			return Problem(exc.FullMessage());
		}
	}
}
