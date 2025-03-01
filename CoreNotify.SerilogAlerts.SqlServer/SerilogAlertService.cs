﻿using Coravel.Invocable;
using CoreNotify.Client;
using CoreNotify.SerilogAlerts.Shared;
using MailerSend;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace CoreNotify.SerilogAlerts.SqlServer;

public class SerilogAlertService(
	ISerilogQuery query, 
	CoreNotifyClient client,
	IOptions<CoreNotifyOptions> options, 
	ILogger<SerilogAlertService> logger) : IInvocable
{
	private readonly ISerilogQuery _query = query;
	private readonly CoreNotifyClient _client = client;
	private readonly ILogger<SerilogAlertService> _logger = logger;
	private readonly CoreNotifyOptions _options = options.Value;

	public async Task TestAsync(int limitRows)
	{
		_logger.LogDebug("Testing serilog alert with limit {limitRows} rows...", limitRows);
		var data = await _query.TestAsync(limitRows);
		await SendAlertInnerAsync(data);
	}

	public async Task Invoke()
	{
		_logger.LogDebug("Invoking scheduled Serilog alert...");
		var data = await _query.QueryNewEntriesAsync();
		await SendAlertInnerAsync(data);
	}

	private async Task SendAlertInnerAsync(SerilogEntry[] data)
	{
		if (!data.Any()) return;

		_logger.LogDebug("Sending alert with {count} entries...", data.Length);

		StringBuilder sb = new();

		foreach (var msgGroup in data.GroupBy(row => Left(row.MessageTemplate, 50)).OrderByDescending(grp => grp.Count()))
		{
			sb.AppendLine($"<p><strong>{msgGroup.Count()}</strong>: <span>{msgGroup.Key}</span></p>");
		}

		try
		{
			await _client.SendAlertAsync(_options.AccountEmail, _options.ApiKey, new()
			{
				Email = _options.AccountEmail,
				DomainName = _options.DomainName,
				Subject = "Serilog Alert",
				HtmlBody = sb.ToString()
			});
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error sending alert");
		}
	}

	private static string Left(string input, int length)
	{
		if (input.Length <= length) return input;
		return input[..length] + "...";
	}
}
