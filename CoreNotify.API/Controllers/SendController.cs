﻿using CoreNotify.API.Data.Entities;
using CoreNotify.Shared.Models;
using MailerSend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Data;
using Services.Data.Entities;

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

	[HttpPost("Confirmation")]
	public async Task<IActionResult> Confirmation(SendConfirmationRequest request)
	{
		var account = HttpContext.Items["Account"] as Account ?? throw new Exception("account missing");

		var msgId = await SendInnerAsync(MessageType.Confirmation, account, request,
			_content.ConfirmationSubject(request.UserName ?? request.Email, request.DomainName),
			_content.ConfirmationBody(request.UserName ?? request.Email, request.DomainName, request.ConfirmationLink));

		return Ok(msgId);
	}

	[HttpPost("ResetLink")]
	public async Task<IActionResult> ResetLink(SendResetLinkRequest request)
	{
		var account = HttpContext.Items["Account"] as Account ?? throw new Exception("account missing");

		var msgId = await SendInnerAsync(MessageType.ResetLink, account, request,
			_content.PasswordResetLinkSubject(request.UserName ?? request.Email, request.DomainName),
			_content.PasswordResetLinkBody(request.UserName ?? request.Email, request.DomainName, request.ResetLink));

		return Ok(msgId);
	}

	[HttpPost("ResetCode")]
	public async Task<IActionResult> ResetCode(SendResetCodeRequest request)
	{
		var account = HttpContext.Items["Account"] as Account ?? throw new Exception("account missing");

		var msgId = await SendInnerAsync(MessageType.ResetCode, account, request,
			_content.PasswordResetCodeSubject(request.UserName ?? request.Email, request.DomainName),
			_content.PasswordResetCodeBody(request.UserName ?? request.Email, request.DomainName, request.Code));

		return Ok(msgId);
	}

	private async Task<string> SendInnerAsync(MessageType messageType, Account account, SendRequestBase request, string subject, string html)
	{
		var msgId = await _mailerSendClient.SendAsync(new()
		{
			From = request.SenderMailbox + $"@{request.DomainName}.corenotify.net",
			To = [request.Email],
			Subject = subject,
			Html = html
		});

		_logger.LogInformation(
			"{account} sent confirmation email {msgId} from {mailbox}@{domain} to {email}",
			account.Email, msgId, request.SenderMailbox, request.DomainName, request.Email);

		using var db = _dbFactory.CreateDbContext();
		await db.LogActivityAsync(new()
		{
			AccountId = account.Id,
			MessageId = msgId!,
			Recipient = request.Email,
			MessageType = messageType,
			FromDomain = request.DomainName,
			FromMailbox = request.SenderMailbox
		});

		return msgId;
	}
}
