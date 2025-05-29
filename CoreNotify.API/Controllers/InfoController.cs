using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Services;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InfoController(IOptions<ExpirationReminder.Options> expirationReminderOptions) : ControllerBase
{
	private readonly ExpirationReminder.Options _expirationReminderOptions = expirationReminderOptions.Value;

	[HttpGet]
	public IActionResult Get() => Ok(new
	{
		BuildInfo = new
		{
			LocalTime = BuildInfo.LocalBuildTime,
			BuildInfo.Git.ShortCommitId,
			Age = BuildInfo.LocalBuildTime.Humanize(),
			Version = VersionReader.Value
		}
	});

	[HttpGet("AccountReminders")]
	public IActionResult AccountReminders() => Ok(_expirationReminderOptions);	
}
