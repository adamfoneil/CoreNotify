namespace CoreNotify.SerilogAlerts.Shared;

public class SerilogEntry
{
	public long Id { get; set; }
	public DateTimeOffset Timestamp { get; set; }
	public string Message { get; set; } = default!;
	public string MessageTemplate { get; set; } = default!;
	public string Level { get; set; } = default!;
	public string? Exception { get; set; }
	public string? Properties { get; set; }
	public Dictionary<string, object?> PropertyDictionary { get; set; } = [];
}
