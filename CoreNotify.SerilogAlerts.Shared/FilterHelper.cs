namespace CoreNotify.SerilogAlerts.Shared;

public static class FilterHelper
{
	public static bool ExcludeByProperty(
		IEnumerable<KeyValuePair<string, string>> excludeProperties,
		Dictionary<string, object?> entryProperties)
	{
		foreach (var (EntryValue, CriteriaValue) in excludeProperties.Join(
			entryProperties, kp => kp.Key, kp => kp.Key,
			(criteria, entry) => (EntryValue: entry.Value?.ToString() ?? string.Empty, CriteriaValue: criteria.Value), StringComparer.OrdinalIgnoreCase))
		{
			if (EntryValue.Contains(CriteriaValue, StringComparison.OrdinalIgnoreCase)) return true;
		}

		return false;
	}
}
