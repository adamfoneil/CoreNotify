using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Data;
using Services.Models;
using System.Collections.Concurrent;

namespace Services;

public class BounceHandler(
	ILogger<BounceHandler> logger,	
	ConcurrentQueue<Bounce> bounceQueue,
	IDbContextFactory<ApplicationDbContext> dbFactory) : BackgroundService
{	
	private readonly ILogger<BounceHandler> _logger = logger;
	private readonly ConcurrentQueue<Bounce> _bounceQueue = bounceQueue;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			if (_bounceQueue.TryDequeue(out var bounce))
			{
				try
				{
					using var db = _dbFactory.CreateDbContext();
					var msgId = bounce.Data.Id;
					_logger.LogInformation("Searching for messageId {messageId}", msgId);
					var message = await db.SentMessages.SingleOrDefaultAsync(row => row.MessageId == msgId, stoppingToken) ?? throw new Exception($"MessageId {msgId} not found");
					message.Bounced = true;
					message.BounceDateTime = DateTime.Now;
					await db.SaveChangesAsync(stoppingToken);
				}
				catch (Exception exc)
				{
					_logger.LogError(exc, "Error processing bounce");
				}
			}
			await Task.Delay(3000, stoppingToken);
		}
	}
}
