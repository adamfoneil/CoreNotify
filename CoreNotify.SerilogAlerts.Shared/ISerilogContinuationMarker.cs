namespace CoreNotify.SerilogAlerts.Shared;

public interface ISerilogContinuationMarker
{
	Task<long> GetIdAsync();
	Task SetIdAsync(long id);
}
