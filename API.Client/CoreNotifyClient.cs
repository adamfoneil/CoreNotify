using API.Shared;
using API.Shared.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace API.Client;

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

	private HttpClient GetHttpClient()
	{
		var client = _httpClientFactory.CreateClient();
		client.BaseAddress = new Uri(_options.BaseUrl);
		return client;
	}
}
