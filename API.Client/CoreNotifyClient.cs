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

	public async Task ValidateAsync(string accountEmail, string apiKey)
	{
		var client = _httpClientFactory.CreateClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.GetAsync("api/account/validate");
		response.ThrowIfProblemResponse();
	}	

	public async Task<SendConfirmation.Response?> SendConfirmationAsync(string accountEmail, string apiKey, SendConfirmation.Request request)
	{
		var client = _httpClientFactory.CreateClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PostAsJsonAsync("api/send/confirmation", request);
		response.ThrowIfProblemResponse();
		return await response.Content.ReadFromJsonAsync<SendConfirmation.Response>();
	}	

	private HttpClient GetHttpClient()
	{
		var client = _httpClientFactory.CreateClient();
		client.BaseAddress = new Uri(_options.BaseUrl);
		return client;
	}
}
