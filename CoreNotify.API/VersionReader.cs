namespace CoreNotify.API;

/// <summary>
/// assumes a file .\version.txt in the build output,
/// written by https://github.com/adamfoneil/set-version
/// </summary>
internal static class VersionReader
{
	internal static string Value
	{
		get
		{
			var versionFile = Path.Combine(AppContext.BaseDirectory, "version.txt");
			return (File.Exists(versionFile)) ? File.ReadAllText(versionFile).Trim() : "<unknown>";
		}
	}
}