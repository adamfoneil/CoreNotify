using CoreNotify.Client;
using CoreNotify.MailerSend;
using CoreNotify.SerilogAlerts.Shared;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace CoreNotify.SerilogAlerts.SqlServer;

public class SerilogQuery(
	IOptions<SerilogQuery.Options> queryOptions,	
	IOptions<CoreNotifyOptions> coreNotifyOptions,
	CoreNotifyClient coreNotifyClient,
	ILogger<SerilogQuery> logger) : ISerilogQuery
{
	private readonly Options _queryOptions = queryOptions.Value;
	private readonly CoreNotifyOptions _coreNotifyOptions = coreNotifyOptions.Value;
	private readonly CoreNotifyClient _coreNotifyClient = coreNotifyClient;
	private readonly ILogger<SerilogQuery> _logger = logger;

	public class Options
	{
		public string ConnectionString { get; set; } = default!; // tip: use '@' to reference a named connection string that's valid in your IConfiguration scope
		public string TableName { get; set; } = "Serilog";
		public string SchemaName { get; set; } = "dbo";
		public string ApplicationName { get; set; } = "MyApp";
		public int? MaxRows { get; set; }
		public int QueryTimeout { get; set; } = 30;
		public string QueryCriteria { get; set; } = "[Level]='Error'";
		public KeyValuePair<string, string>[] ExcludeProperties { get; set; } = [];
		public string[] ExcludeMessageTemplates { get; set; } = [];
	}

	public async Task<SerilogEntry[]> TestAsync(int limitRows)
	{
		using var cn = new SqlConnection(_queryOptions.ConnectionString);
		var (logRows, _) = await QueryInternalAsync(cn, 0, $"TOP {limitRows}");
		return [.. logRows];
	}

	public async Task<SerilogEntry[]> QueryNewEntriesAsync()
	{
		try
		{
			using var cn = new SqlConnection(_queryOptions.ConnectionString);

			_logger.LogDebug("Getting serilog continuation marker...");
			var id = await _coreNotifyClient.GetContinuationMarkerAsync(_coreNotifyOptions.AccountEmail, _coreNotifyOptions.ApiKey, _queryOptions.ApplicationName);

			var top = _queryOptions.MaxRows.HasValue ? $" TOP ({_queryOptions.MaxRows.Value})" : string.Empty;

			_logger.LogDebug("Querying serilog with starting Id {id}...", id);

			(List<SerilogEntry> logRows, long maxId) = await QueryInternalAsync(cn, id, top);

			await _coreNotifyClient.SetContinuationMarker(_coreNotifyOptions.AccountEmail, _coreNotifyOptions.ApiKey, _queryOptions.ApplicationName, maxId);
			_logger.LogDebug("Marked serilog entries up to Id {maxId}", maxId);

			return [.. logRows];
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error querying serilog entries");
			return [];
		}
	}

	private async Task<(List<SerilogEntry> logRows, long maxId)> QueryInternalAsync(SqlConnection cn, long id, string top)
	{
		var parser = new XmlPropertyParser();
		var sw = Stopwatch.StartNew();

		var sql = $"SELECT {top} * FROM [{_queryOptions.SchemaName}].[{_queryOptions.TableName}] WHERE [Id] > @Id AND {_queryOptions.QueryCriteria}";

		var logRows = (await cn.QueryAsync<SerilogEntry>(
			sql, new { id }, commandTimeout: _queryOptions.QueryTimeout)).ToList();

		_logger.LogDebug("Queried {count} serilog entries in {elapsed}", logRows.Count, sw.Elapsed);

		long maxId = id;
		HashSet<long> exclude = [];
		foreach (var entry in logRows)
		{
			if (entry.Id > maxId) maxId = entry.Id;
			entry.PropertyDictionary = parser.ParseProperties(entry.Properties);
			entry.Properties = null; // after parsing, we don't need this value anymore

			if (Exclude(entry)) exclude.Add(entry.Id);
		}

		logRows.RemoveAll(row => exclude.Contains(row.Id));

		sw.Stop();
		_logger.LogDebug("Parsed properties and applied filters in {elapsed}", sw.Elapsed);
		return (logRows, maxId);
	}

	private bool Exclude(SerilogEntry entry)
	{
		foreach (var excludeTemplate in _queryOptions.ExcludeMessageTemplates)
		{
			if (entry.MessageTemplate.Contains(excludeTemplate, StringComparison.OrdinalIgnoreCase))
			{
				_logger.LogDebug("Excluding entry id {id} with message template: {template}", entry.Id, excludeTemplate);
				return true;
			}
		}

		if (FilterHelper.ExcludeByProperty(_queryOptions.ExcludeProperties, entry.PropertyDictionary, out var excluded))
		{
			_logger.LogDebug("Excluding entry id {id} because {entry} contained {value}", 
				entry.Id, excluded!.Value.entryValue, excluded.Value.criteriaValue);
			return true;
		}

		return false;
	}
}
