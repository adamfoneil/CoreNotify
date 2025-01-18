using API.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationBuilder()
	.AddJsonFile("appsettings.Development.json", optional: true)
	.AddUserSecrets("d8a4c5af-79af-4ebd-a6a0-791dbd8ac6a6")
	.AddEnvironmentVariables()
	.Build();

var services = new ServiceCollection()	
	.AddLogging()
	.AddHttpClient()
	.Configure<Options>(config)
	.AddScoped<CoreNotifyClient>()
	.BuildServiceProvider();
	
var client = services.GetRequiredService<CoreNotifyClient>();
var myKey = config["CoreNotifyApiKey"] ?? args[2];

switch (args[0])
{
	case "register":
		await client.CreateAccountAsync(args[1]);
		Console.WriteLine($"Your CoreNotify API key was sent to {args[1]}");
		break;	

	case "usage":
		var usage = await client.GetUsageAsync(args[1], myKey);
		Console.WriteLine($"Renewal date: {usage.RenewalDate:M/d/yy}");
		Console.WriteLine($"Total recent messages: {usage.TotalRecentMessages:n0}");		
		break;
}