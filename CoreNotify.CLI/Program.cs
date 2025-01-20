using API.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationBuilder()
#if DEBUG
	.AddJsonFile("appsettings.Development.json", optional: false)
	.AddUserSecrets("d8a4c5af-79af-4ebd-a6a0-791dbd8ac6a6")
#else
	.AddJsonFile("appsettings.json", optional: false)
#endif
	.AddEnvironmentVariables()
	.Build();

var services = new ServiceCollection()	
	.AddLogging()
	.AddHttpClient()
	.Configure<Options>(config)
	.AddScoped<CoreNotifyClient>()
	.BuildServiceProvider();
	
var client = services.GetRequiredService<CoreNotifyClient>();

Console.ResetColor();
try
{
	switch (args[0])
	{
		case "register":
			await client.CreateAccountAsync(args[1]);
			Console.WriteLine($"Your CoreNotify API key was sent to {args[1]}");
			break;

		case "usage":
			var myKey = config["CoreNotifyApiKey"] ?? args[2];
			var usage = await client.GetUsageAsync(args[1], myKey);
			Console.WriteLine($"Renewal date: {usage.RenewalDate:M/d/yy}");
			Console.WriteLine($"Total recent messages: {usage.TotalRecentMessages:n0}");
			Console.WriteLine($"Service URL: {client.ServiceUrl}");
			break;

		case "resend":
			await client.ResendApiKeyAsync(args[1]);
			Console.WriteLine($"Your CoreNotify API key was resent to {args[1]}");
			break;

		case "info":
			Console.WriteLine($"Service URL: {client.ServiceUrl}");
			break;
	}	
}
catch (Exception exc)
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.WriteLine(exc.Message);
	Console.ResetColor();
	Console.WriteLine("Args received:");
	foreach (var arg in args) Console.WriteLine(arg);
	Console.WriteLine("Service URL: {client.ServiceUrl}");
}

Console.ResetColor();