using CoreNotify.SerilogAlerts.Shared;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace CoreNotify.SerilogAlerts.SqlServer;

public class SqlServerSerilogQuery(
	IOptions<SerilogQueryOptions> options,
	ISerilogEntryPropertyParser parser,
	ISerilogContinuationMarker marker,
	ILogger<SqlServerSerilogQuery> logger) : ISerilogQuery
{
	private readonly SerilogQueryOptions _options = options.Value;
	private readonly ISerilogEntryPropertyParser _parser = parser;
	private readonly ISerilogContinuationMarker _marker = marker;
	private readonly ILogger<SqlServerSerilogQuery> _logger = logger;

	public async Task<SerilogEntry[]> QueryNewEntriesAsync()
	{
		try
		{
			_logger.LogDebug("Getting serilog continuation marker...");
			var id = await _marker.GetIdAsync();

			using var cn = new SqlConnection(_options.ConnectionString);

			var top = _options.MaxRows.HasValue ? $" TOP ({_options.MaxRows.Value})" : string.Empty;

			_logger.LogDebug("Querying serilog with starting Id {id}...", id);

			var sw = Stopwatch.StartNew();

			var logRows = (await cn.QueryAsync<SerilogEntry>(
				$"SELECT{top} * FROM [{_options.SchemaName}].[{_options.TableName}] WHERE [Id] > @Id AND {_options.QueryCriteria}",
				new { id }, commandTimeout: _options.QueryTimeout)).ToList();
			
			_logger.LogDebug("Queried {count} serilog entries in {elapsed}", logRows.Count, sw.Elapsed);
						
			long maxId = id;
			HashSet<long> exclude = [];
			foreach (var entry in logRows)
			{
				if (entry.Id > maxId) maxId = entry.Id;
				entry.PropertyDictionary = _parser.ParseProperties(entry.Properties);
				entry.Properties = null; // after parsing, we don't need this value anymore, so let's save some webhook bandwidth

				if (Exclude(entry)) exclude.Add(entry.Id);
			}

			logRows.RemoveAll(row => exclude.Contains(row.Id));			

			sw.Stop();
			_logger.LogDebug("Parsed properties and applied filters in {elapsed}", sw.Elapsed);

			await _marker.SetIdAsync(maxId);
			_logger.LogDebug("Marked serilog entries up to Id {maxId}", maxId);

			return [.. logRows];
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error querying serilog entries");
			return [];
		}
	}

	private bool Exclude(SerilogEntry entry)
	{
		foreach (var excludeTemplate in _options.ExcludeMessageTemplates)
		{
			if (entry.MessageTemplate.Contains(excludeTemplate, StringComparison.OrdinalIgnoreCase)) return true;
		}

		if (FilterHelper.ExcludeByProperty(_options.ExcludeProperties, entry.PropertyDictionary)) return true;

		return false;
	}
}
