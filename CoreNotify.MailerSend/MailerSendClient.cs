using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoreNotify.MailerSend;

public class MailerSendOptions
{
	public string Url { get; set; } = "https://api.mailersend.com/v1";
	public string ApiKey { get; set; } = default!;
	public string SenderEmail { get; set; } = default!;
	public int SendDelayMS { get; set; } = 1000; // to prevent throttling
}

public class MailerSendClient(
	IHttpClientFactory httpClientFactory,
	IOptions<MailerSendOptions> options,
	ILogger<MailerSendClient> logger)
{
	private readonly MailerSendOptions _options = options.Value;
	private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
	private readonly ILogger<MailerSendClient> _logger = logger;

	public async Task<string> SendAsync(Message message)
	{
		if (string.IsNullOrEmpty(_options.ApiKey))
		{
			throw new InvalidOperationException("MailerSend API key not set");
		}

		if (message.From.Email is null)
		{
			message.From = _options.SenderEmail;
		}

		// prevent Too Many Requests
		await Task.Delay(_options.SendDelayMS);

		_httpClient.DefaultRequestHeaders.Clear();
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

		var response = await _httpClient.PostAsync(
			   new Uri(_options.Url + "/email"),
			   JsonContent.Create(message, options: SerializerOptions));

		//var responseContent = await response.Content.ReadAsStringAsync();

		try
		{
			response.EnsureSuccessStatusCode();
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error sending email: {responseContent}", await response.Content.ReadAsStringAsync());
			throw;
		}

		if (response.Headers.TryGetValues("X-Message-Id", out var values))
		{
			return values.First();
		}
		
		_logger.LogWarning("Email sent successfully to {recipient} with subject {subject}, but X-Message-Id header not found in response", 
			string.Join(", ", message.To.Select(r => r.Email)), message.Subject);

		return Guid.NewGuid().ToString();
	}

	private static JsonSerializerOptions SerializerOptions => new()
	{
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // to allow inline html in HtmlBody
		WriteIndented = true
	};

	/// <summary>
	/// based on https://developers.mailersend.com/api/v1/email.html#send-an-email
	/// </summary>
	public class Message
	{
		[JsonPropertyName("from")]
		public Recipient From { get; set; } = new();
		[JsonPropertyName("to")]
		public Recipient[] To { get; set; } = [];
		[JsonPropertyName("reply_to")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Recipient? ReplyTo { get; set; }
		[JsonPropertyName("subject")]
		public string Subject { get; set; } = default!;
		[JsonPropertyName("text")]
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string? Text { get; set; } = default!;
		[JsonPropertyName("html")]
		public string? Html { get; set; } = default!;
	}

	public class Recipient
	{
		[JsonPropertyName("email")]
		public string Email { get; set; } = default!;
		[JsonPropertyName("name")]
		public string Name { get; set; } = default!;

		public static implicit operator Recipient(string email) => new() { Email = email };
	}
}
