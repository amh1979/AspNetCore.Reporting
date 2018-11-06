using System;
using System.Data;
using System.Diagnostics;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[DebuggerStepThrough]
	internal class ValuesRowChangeEventArgs : EventArgs
	{
		private ValuesRow eventRow;

		private DataRowAction eventAction;

		public ValuesRow Row
		{
			get
			{
				return this.eventRow;
			}
		}

		public DataRowAction Action
		{
			get
			{
				return this.eventAction;
			}
		}

		public ValuesRowChangeEventArgs(ValuesRow row, DataRowAction action)
		{
			this.eventRow = row;
			this.eventAction = action;
		}
	}
}
