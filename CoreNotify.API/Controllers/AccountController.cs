using Microsoft.AspNetCore.Mvc;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
	[HttpPost("register")]
	public async Task<IActionResult> Register()
	{
		throw new NotImplementedException();
	}
}
