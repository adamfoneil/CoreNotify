using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace CoreNotify.Client;

internal static class ProblemResponseHelper
{
	public static async Task ThrowIfProblemResponseAsync(this HttpResponseMessage response)
	{
		try
		{
			response.EnsureSuccessStatusCode();
		}
		catch (HttpRequestException exc)
		{
			var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>() ?? throw new Exception("unable to parse problem details");
			throw new ApiException(problem, exc);
		}
	}
}

public class ApiException(ProblemDetails problemDetails, Exception innerException) : Exception(problemDetails.Detail, innerException)
{
	public ProblemDetails ProblemDetails { get; } = problemDetails;
}
