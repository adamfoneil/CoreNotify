using CoreNotify.Shared;
using CoreNotify.Shared.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace CoreNotify.Client;

public class CoreNotifyClient(
	ILogger<CoreNotifyClient> logger,
    IHttpClientFactory httpClientFactory, 
	IOptions<Options> options)
{
    private readonly ILogger<CoreNotifyClient> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
	private readonly Options _options = options.Value;

	public async Task CreateAccountAsync(string accountEmail)
	{
		_logger.LogDebug("Creating account for {accountEmail}", accountEmail);
		var client = GetHttpClient();
		var response = await client.PostAsJsonAsync("api/account/register", new CreateAccountRequest { Email = accountEmail });
		await response.ThrowIfProblemResponseAsync();
	}

	public async Task ResendApiKeyAsync(string accountEmail)
	{
		_logger.LogDebug("Resending API key for {accountEmail}", accountEmail);
        var client = GetHttpClient();
		var response = await client.PostAsJsonAsync("api/account/resend", new CreateAccountRequest() { Email = accountEmail });
		await response.ThrowIfProblemResponseAsync();
	}

	public async Task<AccountUsageResponse> GetUsageAsync(string accountEmail, string apiKey)
	{
		_logger.LogDebug("Getting usage for {accountEmail}", accountEmail);
        var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.GetAsync("api/account/usage");		
		await response.ThrowIfProblemResponseAsync();
		return await response.Content.ReadFromJsonAsync<AccountUsageResponse>() ?? new();
	}

	public async Task<string> SendConfirmationAsync(string accountEmail, string apiKey, SendConfirmationRequest request)
	{
		_logger.LogDebug("Sending confirmation email for {accountEmail} to {email}", accountEmail, request.Email);
        var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PostAsJsonAsync("api/send/confirmation", request);
		await response.ThrowIfProblemResponseAsync();
		return await response.Content.ReadAsStringAsync();
	}
	
	public async Task<string> SendResetLinkAsync(string accountEmail, string apiKey, SendResetLinkRequest request)
	{
		_logger.LogDebug("Sending password reset link email for {accountEmail} to {email}", accountEmail, request.Email);
        var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PostAsJsonAsync("api/send/resetlink", request);
		await response.ThrowIfProblemResponseAsync();
		return await response.Content.ReadAsStringAsync();
	}

	public async Task<string> SendResetCodeAsync(string accountEmail, string apiKey, SendResetCodeRequest request)
	{
		_logger.LogDebug("Sending password reset code email for {accountEmail} to {email}", accountEmail, request.Email);
        var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PostAsJsonAsync("api/send/resetcode", request);
		await response.ThrowIfProblemResponseAsync();
		return await response.Content.ReadAsStringAsync();
	}

	public async Task<string> RecycleKeyAsync(string accountEmail, string apiKey)
	{
		_logger.LogDebug("Recycling API key for {accountEmail}", accountEmail);
        var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PostAsJsonAsync("api/account/recycle", new CreateAccountRequest() { Email = accountEmail });
		await response.ThrowIfProblemResponseAsync();
		return await response.Content.ReadAsStringAsync();
	}

	public async Task SendAlertAsync(string accountEmail, string apiKey, SendAlertRequest request)
	{
		_logger.LogDebug("Sending alert for {accountEmail} to {email}", accountEmail, request.Email);
        var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PostAsJsonAsync("api/send/alert", request);
		await response.ThrowIfProblemResponseAsync();
	}

	public async Task<long> GetContinuationMarkerAsync(string accountEmail, string apiKey, string name)
	{
		_logger.LogDebug("Getting continuation marker {name} for {accountEmail}", name, accountEmail);
        var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.GetAsync($"api/marker/{name}");
		await response.ThrowIfProblemResponseAsync();

		using JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
		return doc.RootElement.GetProperty("logEntryId").GetInt64();
	}

	public async Task SetContinuationMarker(string accountEmail, string apiKey, string name, long value)
	{
		_logger.LogDebug("Setting continuation marker {name} to {value} for {accountEmail}", name, value, accountEmail);
        var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PutAsync($"api/marker/{name}/{value}", new StringContent(""));
		await response.ThrowIfProblemResponseAsync();
	}

	private HttpClient GetHttpClient()
	{
		var client = _httpClientFactory.CreateClient();
		client.BaseAddress = new Uri(_options.BaseUrl);
		return client;
	}

	public string ServiceUrl => _options.BaseUrl;
}
