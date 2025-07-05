namespace CoreNotify.API.SerilogApiConnector;

internal static class PostgresHelpers
{
	internal static string MapIntToLevel(int? level) => level switch
	{
		0 => "Verbose",
		1 => "Debug",
		2 => "Information",
		3 => "Warning",
		4 => "Error",
		5 => "Fatal",
		_ => "Unknown"
	};
}
