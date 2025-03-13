using Coravel.Invocable;
using CoreNotify.MailerSend;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.Data;
using static CoreNotify.MailerSend.MailerSendClient;

namespace Services;

public class ExpirationReminder(
	IDbContextFactory<ApplicationDbContext> dbFactory,
	IOptions<ExpirationReminder.Options> options,
	MailerSendClient mailerSendClient) : IInvocable
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
	private readonly Options _options = options.Value;
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;

	public class Options
	{
		public int IntervalDays { get; set; } = 3;
		public decimal MonthlyPrice { get; set; } = 5.00m;
		public string PaymentLink { get; set; } = default!;
	}

	public async Task Invoke()
	{
		using var db = _dbFactory.CreateDbContext();

		var expirationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(_options.IntervalDays));

		var accounts = await db.Accounts
			.Where(row => row.RenewalDate == expirationDate)
			.ToArrayAsync();

		foreach (var account in accounts)
		{
			await _mailerSendClient.SendAsync(new Message()
			{
				To = [account.Email],
				Subject = "CoreNotify Account Expiration",
				Html =
					$"<p>Your account will expire on {account.RenewalDate:M/d/yy}. CoreNotify does not have automatic renewal.</p>" +
					"<p>Please see the GitHub repo here: <a href=\"https://github.com/adamfoneil/CoreNotify\">https://github.com/adamfoneil/CoreNotify</a></p>" +
					$"<p>If you like the service, please send {_options.MonthlyPrice:c2} * the number of months you'd like to renew to .</p>" +
					"<p>Please see <a href=\""
			});
		}
	}
}
