using CoreNotify.SerilogAlerts.Shared;
using Dapper;
using Microsoft.Extensions.Options;
using System.Data;

namespace CoreNotify.SerilogAlerts.SqlServer;

public class SqlServerSerilogContinuationMarker(IOptions<SqlServerSerilogContinuationMarker.Options> options) : ISerilogContinuationMarker
{
	private readonly Options _options = options.Value;

	public class Options
	{
		public string TableName { get; set; } = "SerilogAlertWebhook";
		public string SchemaName { get; set; } = "dbo";
	}

	/// <summary>
	/// help from https://chatgpt.com/share/67b26ad5-ea34-8011-87f4-426da8dc37fd
	/// </summary>
	public async Task EnsureCreatedAsync(IDbConnection connection)
	{
		string schemaCheckQuery = @"
            IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = @SchemaName)
            EXEC('CREATE SCHEMA [' + @SchemaName + ']');
        ";

		string tableCheckQuery = @"
            IF NOT EXISTS (
                SELECT 1 FROM sys.tables t
                JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.name = @TableName AND s.name = @SchemaName
            )
            BEGIN
                EXEC('CREATE TABLE [' + @SchemaName + '].[' + @TableName + '] (
                    [LogEntryId] BIGINT NOT NULL PRIMARY KEY
                )');
            END;
        ";

		string insertSeedValueQuery = $@"
			IF NOT EXISTS (SELECT 1 FROM [{_options.SchemaName}].[{_options.TableName}])
			INSERT INTO [{_options.SchemaName}].[{_options.TableName}] ([LogEntryId]) VALUES (1);
		";	

		await connection.ExecuteAsync(schemaCheckQuery, new { _options.SchemaName });
		await connection.ExecuteAsync(tableCheckQuery, new { _options.SchemaName, _options.TableName });
		await connection.ExecuteAsync(insertSeedValueQuery);
	}

	public async Task<long> GetIdAsync(IDbConnection connection) => 
		await connection.QuerySingleOrDefaultAsync<long>($"SELECT [LogEntryId] FROM [{_options.SchemaName}].[{_options.TableName}]");

	public async Task SetIdAsync(IDbConnection connection, long id) =>
		await connection.ExecuteAsync($@"
			UPDATE [{_options.SchemaName}].[{_options.TableName}] SET [LogEntryId] = @id", new { id });
}
