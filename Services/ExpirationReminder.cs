using Coravel.Invocable;
using CoreNotify.MailerSend;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Data;
using Services.Data.Entities;
using Stubble.Core.Builders;
using static CoreNotify.MailerSend.MailerSendClient;

namespace Services;

public class ExpirationReminder(
	IDbContextFactory<ApplicationDbContext> dbFactory,
	IOptions<ExpirationReminder.Options> options,
	MailerSendClient mailerSendClient,
	ILogger<ExpirationReminder> logger) : IInvocable
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
	private readonly Options _options = options.Value;
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;
	private readonly ILogger<ExpirationReminder> _logger = logger;

	public class Options
	{
		public int IntervalDays { get; set; } = 3;
		public decimal MonthlyPrice { get; set; } = 6.00m;
		public string PaymentLink { get; set; } = default!;
		public string ContactEmail { get; set; } = default!;
	}

	public async Task<string> RenderContentAsync(Account account)
	{
		const string PortalLink = "https://adamfoneil.lemonsqueezy.com/billing";
		const string GitHubLink = "https://github.com/adamfoneil/CoreNotify";

		var template =
			"""
			<p>Your account will expire on {{RenewalDate}}.</p>
			<p>If you paid through Lemon Squeezy, it should auotmatically renew. Manage your subscription here: <a href="{{PortalLink}}">{{PortalLink}}</a></p>
			<p>If you paid me directly with PayPal, please send {{MonthlyPrice}} x the number of months you'd like to renew to {{PaymentLink}}.</p>
			<p>Please see the GitHub repo here: <a href="{{GitHubLink}}">{{GitHubLink}}</a> for more info.</p>
			<p>For human contact, please reach out to {{ContactEmail}}.</p>
			""";

		var stubble = new StubbleBuilder().Build();

		var data = new
		{
			GitHubLink,
			PortalLink,
			RenewalDate = account.RenewalDate.ToString("M/d/yy"),
			MonthlyPrice = _options.MonthlyPrice.ToString("c2"),
			_options.PaymentLink,
			_options.ContactEmail
		};

		return await stubble.RenderAsync(template, data);
	}

	public async Task Invoke()
	{
		using var db = _dbFactory.CreateDbContext();

		var expirationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(_options.IntervalDays));

		var accounts = await db.Accounts
			.Where(row => row.RenewalDate == expirationDate)
			.ToArrayAsync();

		if (!accounts.Any())
		{
			_logger.LogInformation("No accounts found for expiration reminder on {ExpirationDate}", expirationDate);
			return;
		}

		foreach (var account in accounts)
		{
			try
			{
				_logger.LogDebug("Rendering html content for {account}", account.Email);

				var html = await RenderContentAsync(account);

				_logger.LogDebug("Sending expiration reminder to {Email} for {RenewalDate}: {Content}", account.Email, account.RenewalDate, html);
				
				await _mailerSendClient.SendAsync(new Message()
				{
					To = [account.Email],
					Subject = $"CoreNotify Account Expiration on {account.RenewalDate:M/d/yy}",
					Html = html
				});

				_logger.LogInformation("Sent expiration reminder to {Email} for {RenewalDate}", account.Email, account.RenewalDate);
			}
			catch (Exception exc)
			{
				_logger.LogError(exc, "Failed to send expiration reminder to {Email} for {RenewalDate}", account.Email, account.RenewalDate);
			}
		}
	}
}
