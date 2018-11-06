using System;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class DataBindEventArgs : EventArgs
	{
		public new static DataBindEventArgs Empty = new DataBindEventArgs();

		private DataBindingRuleBase dataBinding;

		public DataBindingRuleBase DataBinding
		{
			get
			{
				return this.dataBinding;
			}
		}

		public DataBindEventArgs()
		{
		}

		public DataBindEventArgs(DataBindingRuleBase dataBanding)
		{
			this.dataBinding = dataBanding;
		}
	}
}
