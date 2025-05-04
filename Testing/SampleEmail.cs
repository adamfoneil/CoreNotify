using CoreNotify.MailerSend;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.Data;
using System.Diagnostics;

namespace Testing;

[TestClass]
public sealed class SampleEmail
{
	private static ServiceProvider Services => new ServiceCollection()
		.AddLogging()
		.AddHttpClient()
		.Configure<MailerSendOptions>(Config.GetSection("MailerSend"))
		.Configure<ExpirationReminder.Options>(Config.GetSection("ExpirationReminders"))
		.AddSingleton<MailerSendClient>()
		.AddSingleton<ExpirationReminder>()
		.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(Config.GetConnectionString("DefaultConnection")))
		.BuildServiceProvider();

	[TestMethod]
	public async Task PreviewExpirationReminder()
	{
		using var db = Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext();
		var accounts = await db.Accounts.ToArrayAsync();

		var reminderService = Services.GetRequiredService<ExpirationReminder>();
		foreach (var acct in accounts)
		{
			Debug.Print($"Account: {acct.Email}");
			var html = await reminderService.RenderContentAsync(acct);
			Debug.Print(html);
			Debug.Print("----------------");
		}
	}

	[TestMethod]
	public async Task SendToSelf()
	{		
		var client = Services.GetRequiredService<MailerSendClient>();
		
		await client.SendAsync(new()
		{
			//From = "system2@sample.corenotify.net",
			To = ["adamfoneil@proton.me"],
			Subject = "Test email from MailerSend",
			Html = "<p>This is a test email sent from MailerSend.</p>"
		});
	}

	private static IConfiguration Config => new ConfigurationBuilder()
		.AddUserSecrets("e9c3817b-6baf-4e84-9bf9-9bdde38ae0a3")
		.Build();
}
