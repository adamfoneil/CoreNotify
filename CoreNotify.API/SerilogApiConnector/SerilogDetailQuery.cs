using Microsoft.EntityFrameworkCore;
using SerilogBlazor.Abstractions;
using SerilogBlazor.ApiConnector;
using Services.Data;
using Services.Data.Entities;

namespace CoreNotify.API.SerilogApiConnector;

public class SerilogDetailQuery(IDbContextFactory<ApplicationDbContext> dbFactory) : DetailQuery
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	public override async Task<SerilogEntry[]> ExecuteAsync(string? search)
	{
		var criteria = (search is not null) ? SerilogQuery.Criteria.ParseExpression(search) : new();

		using var context = await _dbFactory.CreateDbContextAsync();
		
		var query = context.Serilog.AsQueryable();

		// Apply filters based on criteria
		if (criteria.FromTimestamp.HasValue)
		{
			query = query.Where(s => s.Timestamp >= criteria.FromTimestamp.Value);
		}

		if (criteria.ToTimestamp.HasValue)
		{
			query = query.Where(s => s.Timestamp <= criteria.ToTimestamp.Value);
		}

		if (criteria.Age.HasValue)
		{
			var cutoffTime = DateTime.UtcNow.Subtract(criteria.Age.Value);
			query = query.Where(s => s.Timestamp >= cutoffTime);
		}

		if (!string.IsNullOrEmpty(criteria.SourceContext))
		{
			query = query.Where(s => s.SourceContext != null && s.SourceContext.Contains(criteria.SourceContext));
		}

		if (!string.IsNullOrEmpty(criteria.Level))
		{
			// Convert Level string to int for database query
			var levelValue = MapLevelToInt(criteria.Level);
			query = query.Where(s => s.Level == levelValue);
		}

		if (!string.IsNullOrEmpty(criteria.Message))
		{
			query = query.Where(s => s.Message != null && s.Message.Contains(criteria.Message));
		}

		if (!string.IsNullOrEmpty(criteria.Exception))
		{
			query = query.Where(s => s.Exception != null && s.Exception.Contains(criteria.Exception));
		}

		// Order by timestamp descending and limit results
		var results = await query
			.OrderByDescending(s => s.Timestamp)
			.Take(100)
			.ToArrayAsync();

		// Convert to SerilogEntry objects
		return results.Select(ConvertToSerilogEntry).ToArray();
	}

	private static int? MapLevelToInt(string level)
	{
		return level switch
		{
			"Fatal" => 0,
			"Error" => 1,
			"Warning" => 2,
			"Information" => 3,
			"Debug" => 4,
			"Trace" => 5,
			_ => null
		};
	}

	private static SerilogEntry ConvertToSerilogEntry(Serilog serilog)
	{
		return new SerilogEntry
		{
			Id = serilog.Id,
			Timestamp = serilog.Timestamp ?? DateTime.MinValue,
			AgeText = CalculateAgeText(serilog.Timestamp),
			SourceContext = serilog.SourceContext,
			RequestId = null, // RequestId is not available in the Serilog entity
			Level = MapIntToLevel(serilog.Level),
			MessageTemplate = serilog.MessageTemplate ?? string.Empty,
			Message = serilog.Message ?? string.Empty,
			Exception = serilog.Exception,
			UserName = null, // UserName is not available in the Serilog entity
			Properties = [] // Properties would need to be parsed from LogEvent JSON
		};
	}

	private static string MapIntToLevel(int? level)
	{
		return level switch
		{
			0 => "Fatal",
			1 => "Error",
			2 => "Warning",
			3 => "Information",
			4 => "Debug",
			5 => "Trace",
			_ => "Unknown"
		};
	}

	private static string CalculateAgeText(DateTime? timestamp)
	{
		if (!timestamp.HasValue)
			return "Unknown";

		var age = DateTime.UtcNow - timestamp.Value;
		
		if (age.TotalDays >= 1)
			return $"{(int)age.TotalDays}d ago";
		if (age.TotalHours >= 1)
			return $"{(int)age.TotalHours}h ago";
		if (age.TotalMinutes >= 1)
			return $"{(int)age.TotalMinutes}m ago";
		
		return "Just now";
	}
}
