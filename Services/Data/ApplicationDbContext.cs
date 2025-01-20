using CoreNotify.API.Data.Entities;
using CoreNotify.API.Data.Entities.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Services.Data.Entities;

namespace Services.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<Account> Accounts { get; set; }
	public DbSet<DailyUsage> DailyUsage { get; set; }
	public DbSet<SentMessage> SentMessages { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

		foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		{
			if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
			{
				modelBuilder.Entity(entityType.ClrType)
					.Property(nameof(BaseEntity.CreatedAt))
					.HasColumnType("timestamp without time zone");

				modelBuilder.Entity(entityType.ClrType)
					.Property(nameof(BaseEntity.UpdatedAt))
					.HasColumnType("timestamp without time zone");
			}
		}
	}

	public async Task LogActivityAsync(SentMessage message)
	{
		message.Bounced = false;
		message.BounceDateTime = null;
		SentMessages.Add(message);

		var today = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
		var usage = await DailyUsage.SingleOrDefaultAsync(u => u.AccountId == message.AccountId && u.Date == today) ?? new()
		{
			AccountId = message.AccountId,
			Date = today
		};

		switch (message.MessageType)
		{
			case MessageType.Confirmation:
				usage.Confirmations++;
				break;
			case MessageType.ResetCode:
				usage.ResetCodes++;
				break;
			case MessageType.ResetLink:
				usage.ResetLinks++;
				break;
		}

		if (usage.Id == 0)
		{
			DailyUsage.Add(usage);
		}
		else
		{
			DailyUsage.Update(usage);
		}

		await SaveChangesAsync();
	}

	public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
	{
		foreach (var entry in ChangeTracker.Entries<BaseEntity>())
		{
			if (entry.State == EntityState.Modified) entry.Entity.UpdatedAt = DateTime.Now;
		}

		return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}
}

public class AppDbFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	private static IConfiguration Config => new ConfigurationBuilder()
		.AddUserSecrets("ea86e09b-6761-4135-8e22-91768b02d916")
		.Build();

	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var connectionString = Config.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string 'DefaultConnection' not found");
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		optionsBuilder.UseNpgsql(connectionString);
		return new ApplicationDbContext(optionsBuilder.Options);
	}
}
