using Coravel;
using MailerSend;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Services;
using Services.Data;
using Services.Models;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string 'DefaultConnection' not found");
var serilogRetentionDays = builder.Configuration.GetValue<int?>("SerilogRetentionDays") ?? 5;

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.MinimumLevel.Override("CoreNotify", Serilog.Events.LogEventLevel.Debug)
	.WriteTo.Console()
	.WriteTo.PostgreSQL(connectionString, "serilog", needAutoCreateTable: true)
	.CreateLogger();

builder.Services
	.AddHttpClient()
	.AddSerilog()
	.AddScheduler()
	.AddHostedService<BounceHandler>()
	.AddSingleton<ConcurrentQueue<Bounce>>()
	.Configure<MailerSendOptions>(builder.Configuration.GetSection("MailerSend"))
	.AddSingleton<MailerSendClient>()
	.AddSingleton<EmailSenderContent>()
	.AddScoped(sp => new SerilogCleanup(connectionString, serilogRetentionDays, sp.GetRequiredService<ILogger<SerilogCleanup>>()))
	.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString))	
	.AddControllers();
	
var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
	scheduler.Schedule<SerilogCleanup>().Hourly();
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();