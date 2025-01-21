using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services;

namespace Testing;

[TestClass]
public class ServiceTests
{
	[TestMethod]
	public async Task PurgeLogAsync()
	{
		var connectionString = Config.GetConnectionString("DefaultConnection") ?? throw new Exception("connection string not found");
		var logger = new LoggerFactory().CreateLogger<SerilogCleanup>();

		var cleanup = new SerilogCleanup(connectionString, 10, logger);
		await cleanup.ExecuteAsync();

		Assert.IsTrue(cleanup.Success);
	}

	private static IConfiguration Config => new ConfigurationBuilder()
		.AddUserSecrets("e9c3817b-6baf-4e84-9bf9-9bdde38ae0a3")
		.Build();

}
