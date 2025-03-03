using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Data;
using Services.Data.Entities;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[VerifyAccount]
public class MarkerController(IDbContextFactory<ApplicationDbContext> dbFactory) : Controller
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	private static async Task<SerilogContinuationMarker> GetOrCreateAsync(ApplicationDbContext db, Account account, string name) =>
		await db
			.ContinuationMarkers
			.FirstOrDefaultAsync(row =>
				row.AccountId == account.Id &&
				row.Name == name) ?? new() { AccountId = account.Id, Name = name };

	[HttpGet("{name}")]
	public async Task<IActionResult> Get(string name)
	{
		var account = HttpContext.Items["Account"] as Account ?? throw new Exception("account missing");

		using var db = _dbFactory.CreateDbContext();

		var marker = await GetOrCreateAsync(db, account, name);

		return Ok(new { marker.LogEntryId });
	}

	[HttpPut("{name}/{value}")]
	public async Task<IActionResult> Set(string name, long value)
	{
		var account = HttpContext.Items["Account"] as Account ?? throw new Exception("account missing");

		using var db = _dbFactory.CreateDbContext();

		var marker = await GetOrCreateAsync(db, account, name);
		marker.LogEntryId = value;
		
		if (marker.Id == 0)
		{
			db.ContinuationMarkers.Add(marker);
		}
		else
		{
			db.ContinuationMarkers.Update(marker);
		}

		await db.SaveChangesAsync();

		return Ok();
	}
}
