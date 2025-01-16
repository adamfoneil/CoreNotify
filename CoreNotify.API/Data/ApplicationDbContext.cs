using CoreNotify.API.Data.Entities;
using CoreNotify.API.Data.Entities.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoreNotify.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<Account> Accounts { get; set; }
	public DbSet<DailyUsage> DailyUsage { get; set; }
	public DbSet<SentMessage> SentMessage { get; set; }

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

	//public Task LogActivityAsync(string messageId, )
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
