using MailerSend;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Testing;

[TestClass]
public sealed class SampleEmail
{
	[TestMethod]
	public async Task SendToSelf()
	{
		var services = new ServiceCollection()
			.AddLogging()
			.AddHttpClient()
			.Configure<MailerSendOptions>(Config.GetSection("MailerSend"))
			.AddSingleton<MailerSendClient>()
			.BuildServiceProvider();

		var client = services.GetRequiredService<MailerSendClient>();
		var msgId = await client.SendAsync(new()
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
