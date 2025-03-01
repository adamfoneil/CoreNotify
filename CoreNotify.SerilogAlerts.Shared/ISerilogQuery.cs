namespace CoreNotify.SerilogAlerts.Shared;

public interface ISerilogQuery
{
	/// <summary>
	/// get entries that are newer than the last time this method was called
	/// </summary>
	Task<SerilogEntry[]> QueryNewEntriesAsync();
	/// <summary>
	/// run your defined query without advancing the continuation marker
	/// to see if your criteria works as expected
	/// </summary>	
	Task<SerilogEntry[]> TestAsync(int limitRows);
}
