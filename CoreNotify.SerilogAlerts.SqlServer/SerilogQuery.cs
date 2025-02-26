using CoreNotify.SerilogAlerts.Shared;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace CoreNotify.SerilogAlerts.SqlServer;

public class SerilogQuery(
	IOptions<SerilogQuery.Options> options,
	ISerilogEntryPropertyParser parser,
	ISerilogContinuationMarker marker,
	ILogger<SerilogQuery> logger) : ISerilogQuery
{
	private readonly Options _options = options.Value;
	private readonly ISerilogEntryPropertyParser _parser = parser;
	private readonly ISerilogContinuationMarker _marker = marker;
	private readonly ILogger<SerilogQuery> _logger = logger;

	public class Options
	{
		public string ConnectionString { get; set; } = default!;
		public string TableName { get; set; } = "Serilog";
		public string SchemaName { get; set; } = "dbo";
		public int? MaxRows { get; set; }
		public int QueryTimeout { get; set; } = 30;
		public string QueryCriteria { get; set; } = "[Level]='Error'";
		public KeyValuePair<string, string>[] ExcludeProperties { get; set; } = [];
		public string[] ExcludeMessageTemplates { get; set; } = [];
	}

	public async Task<SerilogEntry[]> QueryNewEntriesAsync()
	{
		try
		{
			using var cn = new SqlConnection(_options.ConnectionString);

			_logger.LogDebug("Getting serilog continuation marker...");
			var id = await _marker.GetIdAsync(cn);			

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
				entry.Properties = null; // after parsing, we don't need this value anymore

				if (Exclude(entry)) exclude.Add(entry.Id);
			}

			logRows.RemoveAll(row => exclude.Contains(row.Id));			

			sw.Stop();
			_logger.LogDebug("Parsed properties and applied filters in {elapsed}", sw.Elapsed);

			await _marker.SetIdAsync(cn, maxId);
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
