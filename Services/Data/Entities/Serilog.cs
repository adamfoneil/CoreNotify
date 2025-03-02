using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Services.Data.Entities;

public partial class Serilog
{
    public string? Message { get; set; }

    public string? MessageTemplate { get; set; }

    public int? Level { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Exception { get; set; }

    public string? LogEvent { get; set; }

    public int Id { get; set; }

    public string? SourceContext { get; set; }
}

public class SerilogConfiguration : IEntityTypeConfiguration<Serilog>
{
	public void Configure(EntityTypeBuilder<Serilog> builder)
	{
		builder.HasKey(e => e.Id).HasName("serilog_pkey");
		builder.ToTable("serilog");
		builder.HasIndex(e => e.SourceContext, "idx_serilog_source_context");
		builder.Property(e => e.Id).HasColumnName("id");
		builder.Property(e => e.Exception).HasColumnName("exception");
		builder.Property(e => e.Level).HasColumnName("level");
		builder.Property(e => e.LogEvent)
			.HasColumnType("jsonb")
			.HasColumnName("log_event");
		builder.Property(e => e.Message).HasColumnName("message");
		builder.Property(e => e.MessageTemplate).HasColumnName("message_template");
		builder.Property(e => e.SourceContext)
			.HasComputedColumnSql("((log_event -> 'Properties'::text) ->> 'SourceContext'::text)", true)
			.HasColumnName("source_context");
		builder.Property(e => e.Timestamp)
			.HasColumnType("timestamp without time zone")
			.HasColumnName("timestamp");
	}
}
