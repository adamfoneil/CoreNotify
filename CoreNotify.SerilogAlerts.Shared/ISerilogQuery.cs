namespace CoreNotify.SerilogAlerts.Shared;

public interface ISerilogQuery
{
	/// <summary>
	/// get entries that are newer than the last time this method was called
	/// </summary>
	Task<SerilogEntry[]> QueryNewEntriesAsync();
}
