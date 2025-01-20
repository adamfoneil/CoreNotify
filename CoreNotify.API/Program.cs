using Coravel;
using CoreNotify.API.Data;
using MailerSend;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string 'DefaultConnection' not found");
var serilogRetentionDays = builder.Configuration.GetValue<int?>("SerilogRetentionDays") ?? 15;

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Warning()
	.MinimumLevel.Override("CoreNotify", Serilog.Events.LogEventLevel.Information)
	.WriteTo.Console()
	.WriteTo.PostgreSQL(connectionString, "serilog", needAutoCreateTable: true)
	.CreateLogger();

builder.Services
	.AddHttpClient()
	.AddSerilog()
	.AddScheduler()
	.Configure<MailerSendOptions>(builder.Configuration.GetSection("MailerSend"))
	.AddSingleton<MailerSendClient>()
	.AddSingleton<EmailSenderContent>()	
	.AddSingleton(sp => new SerilogCleanup(connectionString, serilogRetentionDays, sp.GetRequiredService<ILogger<SerilogCleanup>>()))
	.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString))
	.AddControllers();

var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
	var cleanup = app.Services.GetRequiredService<SerilogCleanup>();
	scheduler.Schedule(async () => await cleanup.ExecuteAsync()).DailyAtHour(23);
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

