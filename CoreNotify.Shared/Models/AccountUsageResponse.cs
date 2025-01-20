namespace CoreNotify.Shared.Models;

public class AccountUsageResponse
{
	public DateTime RenewalDate { get; set; }
	public DailyUsage[] RecentUsage { get; set; } = [];
	public int TotalRecentMessages => RecentUsage.Sum(x => x.Confirmations + x.ResetCodes + x.ResetLinks);

	public class DailyUsage
	{
		public DateOnly Date { get; set; }
		public int Confirmations { get; set; }
		public int ResetCodes { get; set; }
		public int ResetLinks { get; set; }
	}
}
