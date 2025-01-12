using API.Shared;
using CoreNotify.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace CoreNotify.API;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class VerifyAccountAttribute : Attribute, IAsyncActionFilter
{
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorization))
		{
			if (AuthorizationHeader.TryDecode(authorization.ToString(), out var decoded))
			{
				var dbFactory = context.HttpContext.RequestServices.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
				using var db = dbFactory.CreateDbContext();
				var account = await db.Accounts.FirstOrDefaultAsync(row => row.ApiKey == decoded.ApiKey && row.Email == decoded.Email);
				if (account is null) goto unauthorized;

				context.HttpContext.Items["Account"] = account;
			}
			else goto unauthorized;	
		}

		await next();
		return;

	unauthorized:
		context.Result = new UnauthorizedResult();		
	}
}
