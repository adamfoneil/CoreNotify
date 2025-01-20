using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfoController : ControllerBase
{
	[HttpGet]
	public IActionResult Get()
	{
		return Ok(new
		{
			BuildInfo = new
			{
				BuildInfo.LocalBuildTime,
				BuildInfo.Git.ShortCommitId,
				BuildAge = BuildInfo.LocalBuildTime.Humanize()
			}
		});
	}
}
