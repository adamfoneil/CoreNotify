using System.Data;

namespace CoreNotify.SerilogAlerts.Shared;

public interface ISerilogContinuationMarker
{
	void EnsureCreated(IDbConnection connection);
	Task<long> GetIdAsync(IDbConnection connection);
	Task SetIdAsync(IDbConnection connection, long id);
}
