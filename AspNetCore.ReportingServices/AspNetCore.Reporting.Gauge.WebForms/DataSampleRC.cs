using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class DataSampleRC : HistoryEntry
	{
		internal bool Invalid = true;

		public override DateTime Timestamp
		{
			get
			{
				return base.Timestamp;
			}
			set
			{
				base.Timestamp = value;
				this.Invalid = false;
			}
		}

		public override double Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				base.Value = value;
				this.Invalid = false;
			}
		}

		internal void Assign(DataSampleRC data)
		{
			this.Timestamp = data.Timestamp;
			this.Value = data.Value;
		}
	}
}
