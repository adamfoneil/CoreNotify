using CoreNotify.API.Data.Entities.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreNotify.API.Data.Entities;

public class Account : BaseEntity
{	
	public string Email { get; set; } = default!;
	public bool EmailConfirmed { get; set; }	
	public string ApiKey { get; set; } = API.ApiKey.Generate(32);
	public DateTime RenewalDate { get; set; } = DateTime.Today.AddDays(30);

	public ICollection<DailyUsage> DailyUsage { get; set; } = [];
	public ICollection<SentMessage> SentMessages { get; set; } = [];
}

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
	public void Configure(EntityTypeBuilder<Account> builder)
	{
		builder.HasIndex(p => p.Email).IsUnique();
		builder.Property(p => p.Email).IsRequired().HasMaxLength(50);
		builder.Property(p => p.ApiKey).IsRequired().HasMaxLength(32);		
		builder.Property(p => p.RenewalDate).HasColumnType("timestamp without time zone");
	}
}
