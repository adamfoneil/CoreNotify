using CoreNotify.API.Data.Entities.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Services.Data.Entities;

public class WebhookLog
{
	public int Id { get; set; }
	public int WebhookId { get; set; }
	public DateTimeOffset Timestamp { get; set; } = DateTime.Now;
	public string Url { get; set; } = default!;
	public bool ManuallyInvoked { get; set; }
	public bool IsSuccessResult { get; set; }
	public string? Response { get; set; }
	public long ElapsedMS { get; set; }

	public Webhook Webhook { get; set; } = default!;
}

public class WebhookLogConfiguration : IEntityTypeConfiguration<WebhookLog>
{
	public void Configure(EntityTypeBuilder<WebhookLog> builder)
	{
		builder.Property(e => e.Url).HasMaxLength(255).IsRequired();
		builder.HasOne(e => e.Webhook).WithMany(w => w.Logs).HasForeignKey(e => e.WebhookId);
	}
}
