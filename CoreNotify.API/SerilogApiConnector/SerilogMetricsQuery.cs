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
		
		// Query database for raw grouped data
		var rawResults = await context.Serilog
			.Where(s => s.SourceContext != null)
			.GroupBy(s => new { s.SourceContext, s.Level })
			.Select(g => new
			{
				SourceContext = g.Key.SourceContext!,
				Level = g.Key.Level,
				Count = g.Count(),
				LatestTimestamp = g.Max(s => s.Timestamp)
			})
			.ToArrayAsync();
		
		// Calculate age text in memory and create final results
		var now = DateTime.Now;
		var results = rawResults.Select(r => new SourceContextMetricsResult
		{
			SourceContext = r.SourceContext,
			Level = r.Level.HasValue ? r.Level.Value.ToString() : "Unknown",
			Count = r.Count,
			LatestTimestamp = r.LatestTimestamp ?? DateTime.MinValue,
			AgeText = ParseAgeText((now - (r.LatestTimestamp ?? DateTime.MinValue)).TotalMinutes)
		})
		.OrderByDescending(r => r.LatestTimestamp)
		.ThenBy(r => r.SourceContext)
		.ToArray();
		
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
