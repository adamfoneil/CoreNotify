using CoreNotify.API.Data;
using MailerSend;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string 'DefaultConnection' not found");

builder.Services
	.AddHttpClient()
	.AddLogging()	
	.Configure<MailerSendOptions>(builder.Configuration.GetSection("MailerSend"))
	.AddSingleton<MailerSendClient>()
	.AddSingleton<EmailSenderContent>()
	.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString))
	.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

