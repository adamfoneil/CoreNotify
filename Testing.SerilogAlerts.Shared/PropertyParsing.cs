using CoreNotify.SerilogAlerts.Shared;

namespace Testing.SerilogAlerts.Shared;

[TestClass]
public sealed class PropertyParsing
{
	[TestMethod]
	public void ShouldExcludeSourceContext()
	{
		KeyValuePair<string, string>[] excludeProperties = DefaultExclusions();

		Dictionary<string, object?> entryProperties = new()
		{
			["SourceContext"] = "Microsoft.AspNetCore.Hosting.Diagnostics",
			["RequestId"] = "0HM9T9H9V6F1F:00000001"
		};

		Assert.IsTrue(FilterHelper.ExcludeByProperty(excludeProperties, entryProperties, out _));
	}

	[TestMethod]
	public void ShouldNotExcludeSourceContext()
	{		
		Dictionary<string, object?> entryProperties = new()
		{
			["SourceContext"] = "Hs5.Services.DataContext",
			["RequestId"] = "0HM9T9H9V6F1F:00000001"
		};

		Assert.IsFalse(FilterHelper.ExcludeByProperty(DefaultExclusions(), entryProperties, out _));
	}

	private static KeyValuePair<string, string>[] DefaultExclusions() =>
	[
		new("SourceContext", "Microsoft.AspNetCore"),
		new("SourceContext", "Whatever")
	];

}
