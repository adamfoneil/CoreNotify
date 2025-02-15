using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace CoreNotify.SerilogAlerts;

public static class AppExtensions
{
	public static void MapSerilogAlertWebhook(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/")

	}
}
