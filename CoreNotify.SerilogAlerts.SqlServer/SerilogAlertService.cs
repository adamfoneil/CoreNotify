using Coravel.Invocable;
using CoreNotify.Client;
using CoreNotify.SerilogAlerts.Shared;
using MailerSend;
using Microsoft.Extensions.Options;
using System.Text;

namespace CoreNotify.SerilogAlerts.SqlServer;

class SerilogAlertService(
	ISerilogQuery query, 
	CoreNotifyClient client,
	IOptions<CoreNotifyOptions> options) : IInvocable
{
	private readonly ISerilogQuery _query = query;
	private readonly CoreNotifyClient _client = client;
	private readonly CoreNotifyOptions _options = options.Value;

	public async Task InvokeManual()
	{
		// intended for interactive testing
	}

	public async Task Invoke()
	{
		var data = await _query.QueryNewEntriesAsync();

		if (!data.Any()) return;

		StringBuilder sb = new();

		foreach (var msgGroup in data.GroupBy(row => Left(row.MessageTemplate, 50)).OrderByDescending(grp => grp.Count()))
		{
			sb.AppendLine($"<p><strong>{msgGroup.Count()}</strong> <span>{msgGroup.Key}</span></p>");
		}

		await _client.SendAlertAsync(_options.AccountEmail, _options.ApiKey, new()
		{
			Subject = "Serilog Alert",
			HtmlBody = sb.ToString()
		});
	}

	private static string Left(string input, int length)
	{
		if (input.Length <= length) return input;
		return input[..length] + "...";
	}
}
