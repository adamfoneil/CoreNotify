using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Services.Data;

namespace Services;

public class ExpirationReminder(IDbContextFactory<ApplicationDbContext> dbFactory) : IInvocable
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	public Task Invoke()
	{
		throw new NotImplementedException();
	}
}
