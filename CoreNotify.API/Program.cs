using MailerSend;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddHttpClient()
	.AddLogging()	
	.Configure<MailerSendOptions>(builder.Configuration.GetSection("MailerSend"))
	.AddSingleton<MailerSendClient>()
	.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

