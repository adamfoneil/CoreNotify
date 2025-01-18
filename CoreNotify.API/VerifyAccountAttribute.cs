using API.Shared;
using CoreNotify.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace CoreNotify.API;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class VerifyAccountAttribute : Attribute, IAsyncActionFilter
{
	private ILogger<VerifyAccountAttribute>? _logger;

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		_logger ??= context.HttpContext.RequestServices.GetRequiredService<ILogger<VerifyAccountAttribute>>();

		if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorization))
		{
			if (AuthorizationHeader.TryDecode(authorization.ToString(), out var decoded))
			{
				var dbFactory = context.HttpContext.RequestServices.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
				using var db = dbFactory.CreateDbContext();
				var account = await db.Accounts.FirstOrDefaultAsync(row => row.ApiKey == decoded.ApiKey && row.Email == decoded.Email);

				if (account is null)
				{
					_logger.LogDebug("Unauthorized request from {email} using key {key}", decoded.Email, decoded.ApiKey);
					goto unauthorized;
				}

				if (account!.RenewalDate < DateTime.UtcNow)
				{
					_logger.LogDebug("Attempted use of expired key {key} from {email}", decoded.ApiKey, decoded.Email);
					goto unauthorized;
				}

				context.HttpContext.Items["Account"] = account;

				await next();
				return;
			}
			else
			{
				_logger.LogDebug("Invalid Authorization header {header}", authorization.ToString() ?? "<not set>");
				goto unauthorized;
			}
		}
	
	unauthorized:
		context.Result = new UnauthorizedResult();		
	}
}
