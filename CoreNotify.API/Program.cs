using Bramblelog;
using Coravel;
using CoreNotify.MailerSend;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddLogging(config => config.AddBramblelog(builder.Configuration));

builder.Services
	.AddHttpClient()
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
	.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString))
	.AddMemoryCache()
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
app.MapBramblelogWebhook();

app.Run();