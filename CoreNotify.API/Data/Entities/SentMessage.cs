using CoreNotify.API.Data.Entities.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreNotify.API.Data.Entities;

public enum MessageType
{
	Confirmation = 1,
	ResetLink = 2,
	ResetCode = 3
}

public class SentMessage : BaseEntity
{
	public string MessageId { get; set; } = default!;	
	public int AccountId { get; set; }
	public MessageType MessageType { get; set; }	
	public string FromMailbox { get; set; } = default!;
	public string FromDomain { get; set; } = default!;
	public string Recipient { get; set; } = default!;
	public bool Bounced { get; set; }
	public DateTime? BounceDateTime { get; set; }

	public Account Account { get; set; } = default!;
}

public class SentMessageConfiguration : IEntityTypeConfiguration<SentMessage>
{
	public void Configure(EntityTypeBuilder<SentMessage> builder)
	{
		builder.HasAlternateKey(nameof(SentMessage.MessageId));
		builder.Property(p => p.MessageId).IsRequired().HasMaxLength(50);		
		builder.Property(p => p.Recipient).IsRequired().HasMaxLength(64);
		builder.Property(p => p.BounceDateTime).HasColumnType("timestamp without time zone");
		builder.Property(p => p.FromDomain).IsRequired().HasMaxLength(50);
		builder.Property(p => p.FromMailbox).IsRequired().HasMaxLength(50);
		builder.HasOne(a => a.Account).WithMany(a => a.SentMessages).HasForeignKey(a => a.AccountId).OnDelete(DeleteBehavior.Restrict);
	}
}
