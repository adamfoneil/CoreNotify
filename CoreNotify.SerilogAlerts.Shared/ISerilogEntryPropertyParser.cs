namespace CoreNotify.SerilogAlerts.Shared;

public interface ISerilogEntryPropertyParser
{
	Dictionary<string, object?> ParseProperties(string? properties);
}
