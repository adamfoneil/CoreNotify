using Coravel;
using CoreNotify.MailerSend;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Services;
using Services.Data;
using Services.Models;
using System.Collections.Concurrent;
using System.Globalization;

var defaultCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string 'DefaultConnection' not found");
var serilogRetentionDays = builder.Configuration.GetValue<int?>("SerilogRetentionDays") ?? 5;

Log.Logger = new LoggerConfiguration()
	.Enrich.WithMachineName()
	.MinimumLevel.Warning()
	.MinimumLevel.Override("CoreNotify.API", Serilog.Events.LogEventLevel.Debug)
	.MinimumLevel.Override("Services", Serilog.Events.LogEventLevel.Debug)
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
	.Configure<ExpirationReminder.Options>(builder.Configuration.GetSection("ExpirationReminders"))
	.Configure<CoreNotify.API.LemonSqueezy.Options>(builder.Configuration.GetSection("LemonSqueezy"))
	.AddSingleton<ExpirationReminder>()
	.AddSingleton<MailerSendClient>()
	.AddSingleton<AccountService>()
	.AddSingleton<EmailSenderContent>()
	.AddScoped(sp => new SerilogCleanup(connectionString, serilogRetentionDays, sp.GetRequiredService<ILogger<SerilogCleanup>>()))
	.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString))		
	.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
	
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.Services.UseScheduler(scheduler =>
{
	scheduler.Schedule<SerilogCleanup>().DailyAtHour(23);
	scheduler.Schedule<ExpirationReminder>().DailyAtHour(10);
});

app.UseHttpsRedirection();
app.MapControllers();
app.MapOpenApi();

app.Run();