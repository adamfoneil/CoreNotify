
using Dapper;
using Npgsql;
using System.Diagnostics;

namespace CoreNotify.API;

internal class SerilogCleanup(string connectionString, int retentionDays, ILogger<SerilogCleanup> logger) 
{
	private readonly string _connectionString = connectionString;
	private readonly int _retentionDays = retentionDays;
	private readonly ILogger<SerilogCleanup> _logger = logger;

	public async Task ExecuteAsync()
	{
		try
		{
			using var cn = new NpgsqlConnection(_connectionString);
			const int ChunkSize = 50;

			int rows = 0;
			do
			{
				var sw = Stopwatch.StartNew();
				rows = await cn.ExecuteAsync(
					$@"DELETE FROM serilog
					WHERE id IN (
						SELECT id
						FROM serilog
						WHERE timestamp < NOW() - INTERVAL @retentionDays || ' days'
						ORDER BY timestamp
						LIMIT {ChunkSize};
					)", new { retentionDays = _retentionDays }, commandTimeout: 0);
				sw.Stop();

				if (rows > 0)
				{
					_logger.LogInformation("Deleted {rows} rows older than {days} from Serilog in {elapsed}", rows, _retentionDays, sw.Elapsed);
				}

			} while (rows > 0);
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error cleaning up Serilog");
		}
	}
}
