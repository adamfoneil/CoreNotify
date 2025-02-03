using Microsoft.Extensions.Logging;

namespace MailerSend.Serilog;

public class GenericErrorSender<TKey>(
	MailerSendClient mailerSendClient,
	ILogger<GenericErrorSender<TKey>> logger,
	SinkData<TKey> serilogData) where TKey : notnull
{
	private readonly MailerSendClient _sendClient = mailerSendClient;
	private readonly ILogger<GenericErrorSender<TKey>> _logger = logger;
	private readonly SinkData<TKey> _serilogData = serilogData;
}
