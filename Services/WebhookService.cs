using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Data;
using Services.Data.Entities;
using Services.Extensions;
using System.Diagnostics;

namespace Services;

public abstract class WebhookService(
	IDbContextFactory<ApplicationDbContext> dbFactory,
	ILogger<WebhookService> logger,
	IHttpClientFactory httpClientFactory)
{
	protected readonly IDbContextFactory<ApplicationDbContext> DbFactory = dbFactory;
	protected readonly ILogger<WebhookService> Logger = logger;

	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

	public async Task<Webhook[]> GetActiveWebhooksAsync()
	{
		using var db = DbFactory.CreateDbContext();

		var results = await db
			.Webhooks
			.Include(w => w.Account)
			.Where(w => w.Account.RenewalDate > DateTime.Now && w.IsActive)
			.AsSplitQuery()
			.ToArrayAsync();

		var uniqueWebhookIds = results.Select(w => w.Id).ToHashSet();

		var latestLogs = (await db.WebhookLogs
			.Where(row => uniqueWebhookIds.Contains(row.WebhookId))
			.GroupBy(log => log.WebhookId)
			.SelectMany(group => group
				.OrderByDescending(log => log.Timestamp)
				.Take(10))
			.ToListAsync()).ToLookup(row => row.WebhookId);

		foreach (var wh in results) wh.Logs = [.. latestLogs[wh.Id]];

		return results;
	}

	protected abstract Task<(string Response, bool WebhookModified)> ProcessResponseAsync(HttpResponseMessage? response, ApplicationDbContext db, Webhook webhook);

	public async Task ExecuteAsync(Webhook webhook, bool manualInvocation)
	{
		if (webhook.IsLocked)
		{
			Logger.LogWarning("Can't execute webhook {name} while it's locked", webhook.Name);
			return;
		}

		using var db = DbFactory.CreateDbContext();

		Logger.LogDebug("Locking webhook {name}", webhook.Name);
		webhook.IsLocked = true;
		await db.SaveChangesAsync();

		var url = UrlBuilder.AppendQueryString(webhook.Url, webhook.QueryString);

		var log = new WebhookLog
		{
			WebhookId = webhook.Id,
			Url = url,
			ManuallyInvoked = manualInvocation,
			Timestamp = DateTimeOffset.Now
		};

		var client = _httpClientFactory.CreateClient();
		try
		{
			Logger.LogDebug("Calling webhook {name} at {url}", webhook.Name, url);
			var sw = Stopwatch.StartNew();			
			var response = await client.GetAsync(url);
			sw.Stop();
			log.IsSuccessResult = response.IsSuccessStatusCode;

			var (logResponse, webhookModified) = await ProcessResponseAsync(response, db, webhook);
			log.Response = logResponse;
			log.ElapsedMS = sw.ElapsedMilliseconds;

			if (webhookModified)
			{
				Logger.LogDebug("Webhook {name} was modified by response handler, saving changes", webhook.Name);
				db.Webhooks.Update(webhook);
				await db.SaveChangesAsync();
			}
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error calling webhook {name} at {url}", webhook.Name, url);
			log.IsSuccessResult = false;
			log.Response = ex.Message;
		}
		finally
		{
			Logger.LogDebug("Unlocking webhook {name}", webhook.Name);
			webhook.IsLocked = false;
			await db.SaveChangesAsync();			
		}

		Logger.LogDebug("Logging webhook {name} call", webhook.Name);
		db.WebhookLogs.Add(log);
		await db.SaveChangesAsync();
	}
}
