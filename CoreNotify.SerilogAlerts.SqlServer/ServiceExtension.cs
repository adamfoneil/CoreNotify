﻿using CoreNotify.SerilogAlerts.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CoreNotify.SerilogAlerts.SqlServer;

public static class ServiceExtension
{
	public static void AddCoreNotifySerilogAlerts(this IServiceCollection services, IConfiguration config, string configSection = "SerilogAlerts") =>
		AddCoreNotifySerilogAlerts<SerilogContinuationMarker>(services, config, configSection);

	public static void AddCoreNotifySerilogAlerts<TContinuationMarker>(this IServiceCollection services, IConfiguration config, string configSection = "SerilogAlerts") where TContinuationMarker : class, ISerilogContinuationMarker
	{
		SerilogQuery.Options options = new();
		config.GetSection(configSection).Bind(options);

		if (options.ConnectionString.StartsWith('@'))
		{
			var connectionName = options.ConnectionString[1..];
			options.ConnectionString = config.GetConnectionString(connectionName) ?? throw new InvalidOperationException($"Connection string '{connectionName}' not found.");
		}

		services.AddSingleton(Options.Create(options));
		services.AddSingleton<ISerilogContinuationMarker, TContinuationMarker>();
		services.AddSingleton<ISerilogQuery, SerilogQuery>();
		services.AddSingleton<SerilogAlertService>();
	}
}
