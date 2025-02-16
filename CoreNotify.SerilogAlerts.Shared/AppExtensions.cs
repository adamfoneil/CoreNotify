using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoreNotify.SerilogAlerts.Shared;

public static class AppExtensions
{
	public static void AddSerilogAlerts(this IServiceCollection services, Func<IServiceProvider, ISerilogQuery> factory, string apiKey)
	{
		services.AddSingleton(factory);
		services.Configure<WebhookOptions>(options => options.ApiKey = apiKey);
	}

	public static void MapSerilogAlertsWebhook<T>(this IEndpointRouteBuilder endpoints, string? route = null)
	{
		route ??= "/serilog-alerts";

		endpoints.MapGet(route, async (HttpRequest request, IOptions<WebhookOptions> options, ISerilogQuery query, ILogger<T> logger) =>
		{
			if (!Authorized(request, logger, options.Value.ApiKey)) return Results.Unauthorized();

			try
			{
				logger.LogDebug("Received webhook request for Serilog data");
				var logEntries = await query.QueryNewEntriesAsync();
				logger.LogInformation("Returning {count} new Serilog entries", logEntries.Length);
				return Results.Ok(logEntries);
			}
			catch (Exception exc)
			{
				logger.LogError(exc, "Error executing Serilog alert webhook");
				return Results.Problem(exc.Message);
			}
		});
	}

	private static bool Authorized<T>(HttpRequest request, ILogger<T> logger, string apiKey)
	{
		if (request.Headers.TryGetValue("ApiKey", out var apiKeyValues) == false || apiKeyValues.Count == 0)
		{
			logger.LogWarning("No ApiKey header found in request");
			return false;
		}

		if (string.IsNullOrWhiteSpace(apiKey))
		{
			logger.LogWarning("No ApiKey configured for Serilog alert webhook");
			return false;
		}

		if (apiKey != apiKeyValues[0])
		{
			logger.LogWarning("Invalid ApiKey header found in request");
			return false;
		}

		return apiKey.Equals(apiKeyValues.First());
	}
}
