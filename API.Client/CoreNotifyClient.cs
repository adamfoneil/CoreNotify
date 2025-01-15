using API.Shared;
using API.Shared.Models;
using System.Net.Http.Json;

namespace API.Client;

public class CoreNotifyClient(IHttpClientFactory httpClientFactory)
{
	private readonly IHttpClientFactory httpClientFactory = httpClientFactory;

	public async Task CreateAccountAsync(string accountEmail)
	{
		var client = httpClientFactory.CreateClient();
		var response = await client.PostAsJsonAsync("api/account/register", new CreateAccountRequest { Email = accountEmail });
		response.ThrowIfProblemResponse();
	}

	public async Task<SendConfirmation.Response?> SendConfirmationAsync(string accountEmail, string apiKey, SendConfirmation.Request request)
	{
		var client = httpClientFactory.CreateClient();
		client.AddAuthorization(accountEmail, apiKey);
		var response = await client.PostAsJsonAsync("api/send/confirmation", request);
		response.ThrowIfProblemResponse();
		return await response.Content.ReadFromJsonAsync<SendConfirmation.Response>();
	}	
}
