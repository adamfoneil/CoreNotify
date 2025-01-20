using System.Text.Json.Serialization;

namespace Services.Models;

public class Bounce
{
	[JsonPropertyName("url")]
	public string Url { get; set; } = default!;
	[JsonPropertyName("data")]
	public Data Data { get; set; } = new();
	[JsonPropertyName("type")]
	public string Type { get; set; } = default!;
	[JsonPropertyName("domain_id")]
	public string DomainId { get; set; } = default!;
	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; }
	[JsonPropertyName("webhook_id")]
	public string WebhookId { get; set; } = default!;
}

public class Data
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = default!;
	[JsonPropertyName("type")]
	public string Type { get; set; } = default!;
	[JsonPropertyName("email")]
	public Email Email { get; set; } = new();
	[JsonPropertyName("morph")]
	public Morph Morph { get; set; } = default!;
	[JsonPropertyName("_object")]
	public string? Object { get; set; }
	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; }
	[JsonPropertyName("template_id")]
	public string TemplateId { get; set; } = default!;
}

public class Email
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = default!;
	[JsonPropertyName("from")]
	public string From { get; set; } = default!;
	[JsonPropertyName("tags")]
	public object? Tags { get; set; }
	[JsonPropertyName("_object")]
	public string? Object { get; set; }
	[JsonPropertyName("status")]
	public string Status { get; set; } = default!;
	[JsonPropertyName("headers")]
	public object? Headers { get; set; }
	[JsonPropertyName("message")]
	public Message Message { get; set; } = new();
	[JsonPropertyName("subject")]
	public string Subject { get; set; } = default!;
	[JsonPropertyName("recipient")]
	public Recipient Recipient { get; set; } = new();
	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; }
}

public class Message
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = default!;
	[JsonPropertyName("_object")]
	public string? Object { get; set; }
	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; }
}

public class Recipient
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = default!;
	[JsonPropertyName("email")]
	public string Email { get; set; } = default!;
	[JsonPropertyName("_object")]
	public string? Object { get; set; }
	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; }
}

public class Morph
{
	[JsonPropertyName("_object")]
	public string? Object { get; set; }
	[JsonPropertyName("reason")]
	public string Reason { get; set; } = default!;
}
