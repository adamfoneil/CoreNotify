using CoreNotify.Shared;
using CoreNotify.Shared.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace CoreNotify.Client;

public class CoreNotifyClient(IHttpClientFactory httpClientFactory, IOptions<Options> options)
{
	private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
	private readonly Options _options = options.Value;

	public async Task CreateAccountAsync(string accountEmail)
	{
		var client = GetHttpClient();
		var response = await client.PostAsJsonAsync("api/account/register", new CreateAccountRequest { Email = accountEmail });
		response.ThrowIfProblemResponse();
	}

	public async Task ResendApiKeyAsync(string accountEmail)
	{
		var client = GetHttpClient();
		var response = await client.PostAsJsonAsync("api/account/resend", new CreateAccountRequest() { Email = accountEmail });
		response.ThrowIfProblemResponse();
	}

	public async Task<AccountUsageResponse> GetUsageAsync(string accountEmail, string apiKey)
	{
		var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.GetAsync("api/account/usage");		
		response.ThrowIfProblemResponse();
		return await response.Content.ReadFromJsonAsync<AccountUsageResponse>() ?? new();
	}

	public async Task<string> SendConfirmationAsync(string accountEmail, string apiKey, SendConfirmationRequest request)
	{
		var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PostAsJsonAsync("api/send/confirmation", request);
		response.ThrowIfProblemResponse();
		return await response.Content.ReadAsStringAsync();
	}
	
	public async Task<string> SendResetLinkAsync(string accountEmail, string apiKey, SendResetLinkRequest request)
	{
		var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PostAsJsonAsync("api/send/resetlink", request);
		response.ThrowIfProblemResponse();
		return await response.Content.ReadAsStringAsync();
	}

	public async Task<string> SendResetCodeAsync(string accountEmail, string apiKey, SendResetCodeRequest request)
	{
		var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PostAsJsonAsync("api/send/resetcode", request);
		response.ThrowIfProblemResponse();
		return await response.Content.ReadAsStringAsync();
	}

	public async Task SendAlertAsync(string accountEmail, string apiKey, SendAlertRequest request)
	{
		var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PostAsJsonAsync("api/send/alert", request);
		response.ThrowIfProblemResponse();
	}

	public async Task<long> GetContinuationMarkerAsync(string accountEmail, string apiKey, string name)
	{
		var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.GetAsync($"api/marker/{name}");
		response.ThrowIfProblemResponse();

		using JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
		return doc.RootElement.GetProperty("logEntryId").GetInt64();
	}

	public async Task SetContinuationMarker(string accountEmail, string apiKey, string name, long value)
	{
		var client = GetHttpClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PutAsync($"api/marker/{name}/{value}", new StringContent(""));
		response.ThrowIfProblemResponse();
	}

	private HttpClient GetHttpClient()
	{
		var client = _httpClientFactory.CreateClient();
		client.BaseAddress = new Uri(_options.BaseUrl);
		return client;
	}

	public string ServiceUrl => _options.BaseUrl;
}
