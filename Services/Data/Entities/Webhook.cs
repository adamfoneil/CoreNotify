using CoreNotify.API.Data.Entities.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Services.Data.Entities;

public class Webhook : BaseEntity
{
	public int AccountId { get; set; }
	public string Name { get; set; } = default!;
	public string Url { get; set; } = default!;	
	public string? CronExpression { get; set; }
	/// <summary>
	/// true when the background service is currently executing
	/// </summary>
	public bool IsLocked { get; set; }
	public bool IsActive { get; set; } = true;

	public Account Account { get; set; } = default!;
	public ICollection<WebhookLog> Logs { get; set; } = [];
}

public class WebhookConfiguration : IEntityTypeConfiguration<Webhook>
{
	public void Configure(EntityTypeBuilder<Webhook> builder)
	{
		builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
		builder.HasIndex(e => new { e.AccountId, e.Name }).IsUnique();
		builder.Property(e => e.CronExpression).HasMaxLength(50);
		builder.Property(e => e.Url).HasMaxLength(255).IsRequired();
		builder.HasOne(e => e.Account).WithMany(a => a.Webhooks).HasForeignKey(e => e.AccountId).OnDelete(DeleteBehavior.Restrict);
	}
}
