namespace CoreNotify.Shared.Models;

public class AccountUsageResponse
{
	public DateOnly RenewalDate { get; set; }
	public DailyUsage[] RecentUsage { get; set; } = [];
	public int TotalRecentMessages => RecentUsage.Sum(x => x.Confirmations + x.ResetCodes + x.ResetLinks + x.Alerts);

	public class DailyUsage
	{
		public DateOnly Date { get; set; }
		public int Confirmations { get; set; }
		public int ResetCodes { get; set; }
		public int ResetLinks { get; set; }
		public int Alerts { get; set; }
	}
}
