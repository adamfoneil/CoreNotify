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
				LocalTime = BuildInfo.LocalBuildTime,
				BuildInfo.Git.ShortCommitId,
				Age = BuildInfo.LocalBuildTime.Humanize(),
				Version = VersionReader.Value
			}
		});
	}
}
