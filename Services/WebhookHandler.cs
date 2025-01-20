using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Data;
using Services.Models;
using System.Collections.Concurrent;

namespace Services;

public class WebhookHandler(
	ILogger<WebhookHandler> logger,
	IDbContextFactory<ApplicationDbContext> dbFactory) : BackgroundService
{
	private readonly ConcurrentQueue<Bounce> _bounces = new();
	private readonly ILogger<WebhookHandler> _logger = logger;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	public void AddBounce(Bounce bounce) => _bounces.Enqueue(bounce);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			if (_bounces.TryDequeue(out var bounce))
			{
				using var db = _dbFactory.CreateDbContext();
				var msgId = bounce.Data.Id;
				_logger.LogInformation("Searching for messageId {messageId}", msgId);
				var message = await db.SentMessages.SingleOrDefaultAsync(row => row.MessageId == msgId, stoppingToken) ?? throw new Exception($"MessageId {msgId} not found");
				message.Bounced = true;
				message.BounceDateTime = DateTime.Now;
				await db.SaveChangesAsync(stoppingToken);
			}
			await Task.Delay(1000, stoppingToken);
		}
	}
}
