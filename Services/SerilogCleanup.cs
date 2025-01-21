using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Diagnostics;

namespace Services;

public class SerilogCleanup(string connectionString, int retentionDays, ILogger<SerilogCleanup> logger)
{
	private readonly string _connectionString = connectionString;
	private readonly int _retentionDays = retentionDays;
	private readonly ILogger<SerilogCleanup> _logger = logger;

	public bool Success { get; private set; }

	public async Task ExecuteAsync()
	{
		Success = false;
		try
		{
			using var cn = new NpgsqlConnection(_connectionString);
			const int ChunkSize = 50;

			var sql =
				$@"DELETE FROM serilog
				WHERE id IN (
					SELECT id
					FROM serilog
					WHERE timestamp < NOW() - INTERVAL '1 day' * @retentionDays
					ORDER BY timestamp
					LIMIT {ChunkSize}
				)";

			int rows = 0;
			do
			{				
				var sw = Stopwatch.StartNew();
				rows = await cn.ExecuteAsync(sql, new { retentionDays = _retentionDays }, commandTimeout: 0);
				sw.Stop();

				if (rows > 0)
				{
					_logger.LogInformation("Deleted {rows} rows older than {days} from Serilog in {elapsed}", rows, _retentionDays, sw.Elapsed);
				}

			} while (rows > 0);

			Success = true;
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error cleaning up Serilog");
		}
	}
}
