using CoreNotify.SerilogAlerts.Shared;
using Dapper;
using System.Data;

namespace DemoApp.SerilogAlerts;

public class MyContinuationMarker : ISerilogContinuationMarker
{
	public void EnsureCreated(IDbConnection connection)
	{
		connection.Execute(
			@"WITH [src] AS (
				SELECT @name AS [Name]
			) INSERT INTO [changetracking].[Marker] (
				[Name], [Version], [LastSyncUtc]
			) SELECT
				[Name], 0, getutcdate()
			FROM
				[src]
			WHERE
				NOT EXISTS(SELECT 1 FROM [changetracking].[Marker] WHERE [Name]=@name)", new { name = MarkerName });
	}

	private const string MarkerName = "SerilogAlerts";

	public async Task<long> GetIdAsync(IDbConnection connection) => 
		await connection.QuerySingleAsync<long>(
			"SELECT [Version] FROM [changetracking].[Marker] WHERE Name = @marker", 
			new { marker = MarkerName });

	public async Task SetIdAsync(IDbConnection connection, long id) =>
		await connection.ExecuteAsync(
			"UPDATE [changetracking].[Marker] SET [Version]=@id, [LastSyncUtc]=getutcdate() WHERE [Name]=@name", 
			new { id, name = MarkerName });

}
