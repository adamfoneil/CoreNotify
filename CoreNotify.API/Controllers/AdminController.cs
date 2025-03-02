using CoreNotify.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Data;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[VerifyAccount(requireAdmin: true)]
public class AdminController(IDbContextFactory<ApplicationDbContext> dbFactory) : Controller
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	public async Task<IActionResult> Logs(LogsRequest request)
	{
		throw new NotImplementedException();
	}
}
