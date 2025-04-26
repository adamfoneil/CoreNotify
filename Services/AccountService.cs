using CoreNotify.MailerSend;
using CoreNotify.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Data;
using Services.Data.Entities;

namespace Services;

public class AccountService(
	ILogger<AccountService> logger,
	IDbContextFactory<ApplicationDbContext> dbFactory,
	MailerSendClient mailerSendClient)
{
	private readonly ILogger<AccountService> _logger = logger;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;

	public async Task<Account> CreateAsync(CreateAccountRequest request)
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

		return account;
	}
}
