using CoreNotify.API.Data.Entities.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Services.Data.Entities;

namespace CoreNotify.API.Data.Entities;

public class DailyUsage : BaseEntity
{
	public int AccountId { get; set; }
	public DateOnly Date { get; set; }
	public int Confirmations { get; set; }
	public int ResetCodes { get; set; }
	public int ResetLinks { get; set; }
	public int Alerts { get; set; }

	public Account Account { get; set; } = default!;
}

public class UsageConfiguration : IEntityTypeConfiguration<DailyUsage>
{
	public void Configure(EntityTypeBuilder<DailyUsage> builder)
	{
		builder.HasIndex(e => new { e.AccountId, e.Date }).IsUnique();
		builder.Property(e => e.Date).HasColumnType("date");
		builder.HasOne(a => a.Account).WithMany(a => a.DailyUsage).HasForeignKey(a => a.AccountId).OnDelete(DeleteBehavior.Restrict);
	}
}