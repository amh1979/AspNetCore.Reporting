using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class PointerPositionChangeEventArgs : ValueChangedEventArgs
	{
		private bool accept = true;

		public bool Accept
		{
			get
			{
				return this.accept;
			}
		}

		public PointerPositionChangeEventArgs(double value, DateTime date, string senderName, bool playbackMode)
			: base(value, date, senderName, playbackMode)
		{
		}
	}
}
