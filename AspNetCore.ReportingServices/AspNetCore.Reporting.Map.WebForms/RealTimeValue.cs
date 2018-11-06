using System;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class RealTimeValue
	{
		private string inputValueName = "Default";

		private double value;

		private DateTime timeStamp = DateTime.Now;

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

		public DateTime TimeStamp
		{
			get
			{
				return this.timeStamp;
			}
			set
			{
				this.timeStamp = value;
			}
		}

		public RealTimeValue()
		{
		}

		public RealTimeValue(string inputValueName, double value, DateTime timeStamp)
		{
			this.inputValueName = inputValueName;
			this.value = value;
			this.timeStamp = timeStamp;
		}
	}
}
