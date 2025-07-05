using Microsoft.EntityFrameworkCore;
using SerilogBlazor.Abstractions;
using SerilogBlazor.ApiConnector;
using Services.Data;

namespace CoreNotify.API.SerilogApiConnector;

public class SerilogMetricsQuery(IDbContextFactory<ApplicationDbContext> dbFactory) : MetricsQuery
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	public override Task<SourceContextMetricsResult[]> ExecuteAsync()
	{
		throw new NotImplementedException();
	}
}
