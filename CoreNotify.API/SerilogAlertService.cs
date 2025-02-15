using CoreNotify.SerilogAlerts.Shared;
using MailerSend;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Data;
using Services.Data.Entities;

namespace CoreNotify.API;

public class SerilogAlertService(
	MailerSendClient mailerSendClient,
	IDbContextFactory<ApplicationDbContext> dbFactory, 
	ILogger<WebhookService> logger, 
	IHttpClientFactory httpClientFactory) : WebhookService(dbFactory, logger, httpClientFactory)
{
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;

	protected override async Task<(string Response, bool WebhookModified)> ProcessResponseAsync(HttpResponseMessage? response, ApplicationDbContext db, Webhook webhook)
	{
		if (response == null) return ("No response", false);

		var logEntries = await response.Content.ReadFromJsonAsync<SerilogEntry[]>() ?? [];
		var nextId = logEntries.Any() ? logEntries.Max(entry => entry.Id) : 0;

		webhook.QueryString = $"?fromId={nextId}";
		return ($"Received {logEntries.Length} new log entries", true);
	}
}
