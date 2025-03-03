using CoreNotify.API.Data.Entities.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Services.Data.Entities;

public class SerilogContinuationMarker : BaseEntity
{
	public int AccountId { get; set; }
	public string Name { get; set; } = default!;
	public long LogEntryId { get; set; }

	public Account Account { get; set; } = default!;
}

public class SerilogContinuationMarkerConfiguration : IEntityTypeConfiguration<SerilogContinuationMarker>
{
	public void Configure(EntityTypeBuilder<SerilogContinuationMarker> builder)
	{
		builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
		builder.HasIndex(e => new { e.AccountId, e.Name }).IsUnique();
		builder.HasOne(a => a.Account).WithMany(a => a.SerilogContinuationMarkers).HasForeignKey(a => a.AccountId);
	}
}


