namespace Services.Extensions;

public static class UrlBuilder
{
	public static string AppendQueryString(string baseUrl, string? queryString)
	{
		if (string.IsNullOrWhiteSpace(queryString)) return baseUrl;		

		return baseUrl + (baseUrl.Contains('?') ? "&" : "?") + queryString;
	}
}
