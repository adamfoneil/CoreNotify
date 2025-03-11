using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Services.Data;

namespace Services;

public class ExpirationReminder(
	IDbContextFactory<ApplicationDbContext> dbFactory) : IInvocable
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
	//private readonly MailerSendClient _mailerSendClient = mailerSendClient;

	public async Task Invoke()
	{
		using var db = _dbFactory.CreateDbContext();

		
	}
}
