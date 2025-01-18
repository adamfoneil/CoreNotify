﻿namespace API.Shared.Models;

public class AccountUsageResponse
{
	public DateTime RenewalDate { get; set; }
	public DailyUsage[] RecentUsage { get; set; } = [];

	public class DailyUsage
	{
		public DateOnly Date { get; set; }
		public int Confirmations { get; set; }
		public int ResetCodes { get; set; }
		public int ResetLinks { get; set; }
	}
}
