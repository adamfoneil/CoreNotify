using API.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationBuilder()
	.AddJsonFile("appsettings.Development.json", optional: true)
	.AddEnvironmentVariables()
	.Build();

var services = new ServiceCollection()	
	.AddLogging()
	.AddHttpClient()
	.Configure<Options>(config)
	.AddScoped<CoreNotifyClient>()
	.BuildServiceProvider();
	
var client = services.GetRequiredService<CoreNotifyClient>();

switch (args[0])
{
	case "register":
		await client.CreateAccountAsync(args[1]);
		break;	

	case "query":
		break;
}