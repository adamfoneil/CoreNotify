using Serilog.Core;
using Serilog.Events;
using SerilogBlazor.Abstractions;

namespace CoreNotify.API;

public class ApiLogLevels() : LogLevels(LogEventLevel.Debug)
{
	private readonly Dictionary<string, LoggingLevelSwitch> _levels = new()
	{
		["CoreNotify.API"] = new(LogEventLevel.Debug),
		["Services"] = new(LogEventLevel.Debug)
	};

	public override Dictionary<string, LoggingLevelSwitch> LoggingLevels => _levels;
}
