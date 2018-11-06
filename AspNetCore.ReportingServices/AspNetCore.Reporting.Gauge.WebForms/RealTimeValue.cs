using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class RealTimeValue
	{
		private string inputValueName = "Default";

		private double value;

		private DateTime timestamp = DateTime.Now;

		public string InputValueName
		{
			get
			{
				return this.inputValueName;
			}
			set
			{
				this.inputValueName = value;
			}
		}

		public double Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		public DateTime Timestamp
		{
			get
			{
				return this.timestamp;
			}
			set
			{
				this.timestamp = value;
			}
		}

		public RealTimeValue()
		{
		}

		public RealTimeValue(string inputValueName, double value, DateTime timestamp)
		{
			this.inputValueName = inputValueName;
			this.value = value;
			this.timestamp = timestamp;
		}
	}
}
