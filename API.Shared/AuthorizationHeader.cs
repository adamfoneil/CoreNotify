using System.Text;

namespace API.Shared;

public static class AuthorizationHeader
{
	public static void AddAuthorization(this HttpClient client, string email, string apiKey) => client.DefaultRequestHeaders.Add("Authorization", Encode(email, apiKey));

	public static string Encode(string email, string apiKey) => $"Bearer {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{apiKey}"))}";

	public static (string Email, string ApiKey) Decode(string header)
	{
		const string prefix = "Bearer ";

		if (!header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
		{
			throw new ArgumentException("Invalid header", nameof(header));
		}

		var encodedHeader = header[prefix.Length..].Trim();

		var text = Encoding.UTF8.GetString(Convert.FromBase64String(encodedHeader));
		var parts = text.Split(':');
		return (parts[0], parts[1]);
	}

	public static bool TryDecode(string header, out (string Email, string ApiKey) result)
	{
		try
		{
			result = Decode(header);
			return true;
		}
		catch
		{
			result = (string.Empty, string.Empty);
			return false;
		}
	}
}
