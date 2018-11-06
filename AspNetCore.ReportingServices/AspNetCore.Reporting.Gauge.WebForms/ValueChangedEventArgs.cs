using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class ValueChangedEventArgs : EventArgs
	{
		private double value;

		private DateTime date;

		private bool playbackMode;

		private string senderName;

		public double Value
		{
			get
			{
				return this.value;
			}
		}

		public DateTime Date
		{
			get
			{
				return this.date;
			}
		}

		public ValueChangedEventArgs(double value, DateTime date, string senderName, bool playbackMode)
		{
			this.value = value;
			this.date = date;
			this.playbackMode = playbackMode;
			this.senderName = senderName;
		}
	}
}
