using API.Shared.Models;
using CoreNotify.API.Data;
using CoreNotify.API.Data.Entities;
using MailerSend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreNotify.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[VerifyAccount]
public class SendController(
	IDbContextFactory<ApplicationDbContext> dbFactory,
	ILogger<SendController> logger,
	MailerSendClient mailerSendClient,
	EmailSenderContent content) : ControllerBase
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
	private readonly ILogger<SendController> _logger = logger;
	private readonly MailerSendClient _mailerSendClient = mailerSendClient;
	private readonly EmailSenderContent _content = content;

	[HttpPost("confirmation")]
	public async Task<IActionResult> Confirmation(SendConfirmation.Request request)
	{
		var account = HttpContext.Items["Account"] as Account ?? throw new Exception("account missing");

		var msgId = await _mailerSendClient.SendAsync(new()
		{
			From = request.SenderMailbox + $"@{request.DomainName}.corenotify.net",
			To = [request.Email],
			Subject = _content.ConfirmationSubject(request.UserName, request.DomainName),
			Html = _content.ConfirmationBody(request.UserName, request.DomainName, request.ConfirmationLink)
		});

		_logger.LogInformation(
			"{account} sent confirmation email {msgId} from {mailbox}@{domain} to {email}", 
			account.Email, msgId, request.SenderMailbox, request.DomainName, request.Email);
		
		return Ok(new SendConfirmation.Response() { MessageId = msgId! });
	}
}
