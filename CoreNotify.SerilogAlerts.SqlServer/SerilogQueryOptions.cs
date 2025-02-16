namespace CoreNotify.SerilogAlerts.SqlServer;

public class SerilogQueryOptions
{
	public string ConnectionString { get; set; } = default!;
	public string TableName { get; set; } = "Serilog";
	public string SchemaName { get; set; } = "dbo";	
	public int? MaxRows { get; set; }
	public int QueryTimeout { get; set; } = 30;
	public string QueryCriteria { get; set; } = "[Level]='Error'";	
	public KeyValuePair<string, string>[] ExcludeProperties { get; set; } = [];
	public string[] ExcludeMessageTemplates { get; set; } = [];
}
