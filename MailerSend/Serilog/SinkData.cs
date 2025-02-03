namespace MailerSend.Serilog;

public abstract class SinkData<TKey> where TKey : notnull
{
	public abstract Task<IEnumerable<LogEntry<TKey>>> QueryEntriesAsync();
	public abstract Task MarkEntriesAsync(IEnumerable<TKey> entryIds);
	
	protected abstract Dictionary<string, object?> ParseProperties(string? properties);

	public async Task<(IEnumerable<LogEntry<TKey>> Entries, Dictionary<TKey, Dictionary<string, object?>> Properties)> QueryAsync()
	{
		var newEntries = await QueryEntriesAsync();
		var properties = newEntries.ToDictionary(row => row.Id, row => ParseProperties(row.Properties));
		return (newEntries, properties);
	}
}

public class LogEntry<TKey> where TKey : notnull
{
	public TKey Id { get; set; } = default!;
	public DateTime Timestamp { get; set; }
	public string Message { get; set; } = default!;
	public string Level { get; set; } = default!;
	public string? Exception { get; set; }
	public string? Properties { get; set; }
}
