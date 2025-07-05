using Microsoft.EntityFrameworkCore;
using SerilogBlazor.Abstractions;
using SerilogBlazor.ApiConnector;
using Services.Data;

namespace CoreNotify.API.SerilogApiConnector;

public class SerilogMetricsQuery(IDbContextFactory<ApplicationDbContext> dbFactory) : MetricsQuery
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	public override async Task<SourceContextMetricsResult[]> ExecuteAsync()
	{
		using var context = await _dbFactory.CreateDbContextAsync();
		
		var now = DateTime.Now;
		
		var query = from log in context.Serilog
			where log.SourceContext != null
			group log by new { log.SourceContext, log.Level } into g
			select new SourceContextMetricsResult
			{
				SourceContext = g.Key.SourceContext!,
				Level = g.Key.Level.HasValue ? g.Key.Level.Value.ToString() : "Unknown",
				Count = g.Count(),
				LatestTimestamp = g.Max(s => s.Timestamp) ?? DateTime.MinValue,
				AgeText = ParseAgeText((now - (g.Max(s => s.Timestamp) ?? DateTime.MinValue)).TotalMinutes)
			};
			
		var results = await query.ToArrayAsync();
		return results;
	}
	
	private static string ParseAgeText(double ageMinutes)
	{
		if (ageMinutes < 0)
			return "unknown";
		if (ageMinutes < 1)
			return "just now";
		if (ageMinutes < 60)
			return $"{(int)ageMinutes} minute{((int)ageMinutes == 1 ? "" : "s")} ago";
		
		var ageHours = ageMinutes / 60;
		if (ageHours < 24)
			return $"{(int)ageHours} hour{((int)ageHours == 1 ? "" : "s")} ago";
			
		var ageDays = ageHours / 24;
		return $"{(int)ageDays} day{((int)ageDays == 1 ? "" : "s")} ago";
	}
}
