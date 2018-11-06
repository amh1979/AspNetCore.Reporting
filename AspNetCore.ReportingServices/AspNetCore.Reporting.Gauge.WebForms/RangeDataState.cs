namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class RangeDataState
	{
		internal bool IsInRange;

		internal bool IsTimerExceed;

		internal DataAttributes data;

		internal bool IsRangeActive
		{
			get
			{
				if (this.IsInRange)
				{
					return this.IsTimerExceed;
				}
				return false;
			}
		}

		internal RangeDataState(Range range, DataAttributes data)
		{
			this.data = data;
		}
	}
}
