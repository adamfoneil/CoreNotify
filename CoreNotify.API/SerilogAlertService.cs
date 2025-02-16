using CoreNotify.SerilogAlerts.Shared;
using MailerSend;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Data;
using Services.Data.Entities;
using System.Text.Json;

namespace CoreNotify.API;

public class SerilogAlertService(
	MailerSendClient mailerSendClient,
	IDbContextFactory<ApplicationDbContext> dbFactory, 
	ILogger<WebhookService> logger, 
	IHttpClientFactory httpClientFactory) : WebhookService(dbFactory, logger, httpClientFactory)
{
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;

	protected override async Task<string> ProcessResponseAsync(HttpResponseMessage? response, ApplicationDbContext db, Webhook webhook)
	{
		if (response == null) throw new Exception("Unexpected empty response");

		var logEntries = await response.Content.ReadFromJsonAsync<SerilogEntry[]>() ?? [];
		



		return JsonSerializer.Serialize(logEntries);
	}
}
