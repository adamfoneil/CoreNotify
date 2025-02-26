using Coravel.Invocable;
using CoreNotify.Client;
using CoreNotify.SerilogAlerts.Shared;

namespace CoreNotify.SerilogAlerts.SqlServer;

class SerilogAlertService(
	ISerilogQuery query, 
	CoreNotifyClient client) : IInvocable
{
	private readonly ISerilogQuery _query = query;
	private readonly CoreNotifyClient _client = client;

	public Task Invoke()
	{
		throw new NotImplementedException();
	}
}
