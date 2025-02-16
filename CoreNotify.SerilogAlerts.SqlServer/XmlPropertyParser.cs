using CoreNotify.SerilogAlerts.Shared;
using System.Xml.Linq;

namespace CoreNotify.SerilogAlerts.SqlServer;

public class XmlPropertyParser : ISerilogEntryPropertyParser
{
	public Dictionary<string, object?> ParseProperties(string? properties)
	{
		if (string.IsNullOrWhiteSpace(properties)) return [];

		return ParseXmlToDictionary(properties);
	}

	/// <summary>
	/// help from https://chatgpt.com/share/67b2613f-9da0-8011-8256-e572587b9a0b
	/// </summary>
	private static Dictionary<string, object?> ParseXmlToDictionary(string xml)
	{
		// Ensure valid XML by replacing single quotes with double quotes
		xml = xml.Replace("key='", "key=\"").Replace("type='", "type=\"").Replace("'>", "\">");

		var doc = XDocument.Parse(xml);
		var root = doc.Root ?? throw new InvalidOperationException("Invalid XML format.");

		return ParseProperties(root);
	}

	private static Dictionary<string, object?> ParseProperties(XElement element)
	{
		Dictionary<string, object?> result = [];

		foreach (var prop in element.Elements("property"))
		{
			var key = prop.Attribute("key")?.Value ?? throw new InvalidOperationException("Property missing 'key' attribute.");

			if (prop.Element("structure") != null)
			{
				// Recursively parse structure elements
				result[key] = ParseProperties(prop.Element("structure")!);
			}
			else
			{
				// Store direct values
				result[key] = prop.Value.Trim();
			}
		}

		return result;
	}
}
