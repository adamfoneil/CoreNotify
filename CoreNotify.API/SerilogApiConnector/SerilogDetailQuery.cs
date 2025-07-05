using Microsoft.EntityFrameworkCore;
using SerilogBlazor.Abstractions;
using SerilogBlazor.ApiConnector;
using Services.Data;

namespace CoreNotify.API.SerilogApiConnector;

public class SerilogDetailQuery(IDbContextFactory<ApplicationDbContext> dbFactory) : DetailQuery
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	public override Task<SerilogEntry[]> ExecuteAsync(string? search)
	{
		var criteria = (search is not null) ? SerilogQuery.Criteria.ParseExpression(search) : new();


	}
}
