using CoreNotify.SerilogAlerts.Shared;
using CoreNotify.SerilogAlerts.SqlServer;
using DemoApp.Components;
using DemoApp.Components.Account;
using DemoApp.Data;
using DemoApp.SerilogAlerts;
using MailerSend.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.WriteTo.MSSqlServer(connectionString, new MSSqlServerSinkOptions() { AutoCreateSqlTable = true, TableName = "Serilog", SchemaName = "log"})
	.WriteTo.Console()
	.CreateLogger();

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddCoreNotify<ApplicationUser>(builder.Configuration);

SerilogQuery.Options options = new();
builder.Configuration.GetSection("SerilogAlerts").Bind(options);
// this comes from secrets, so I have to load it separately
options.ConnectionString = builder.Configuration.GetConnectionString("SpayWise") ?? throw new InvalidOperationException("Connection string 'SpayWise' not found.");

//builder.Services.Configure<SerilogQuery.Options>(builder.Configuration.GetSection("SerilogAlerts"));
builder.Services.AddSingleton(Options.Create(options));
builder.Services.AddSingleton<ISerilogEntryPropertyParser, XmlPropertyParser>();
builder.Services.AddSingleton<ISerilogContinuationMarker, ContinuationMarker>();
builder.Services.AddSingleton<ISerilogQuery, SerilogQuery>();
builder.Services.AddSingleton<SerilogAlertService>();

builder.Services.AddSerilog();

builder.Services.AddAuthentication(options =>
	{
		options.DefaultScheme = IdentityConstants.ApplicationScheme;
		options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
	})
	.AddIdentityCookies();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
