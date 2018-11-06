using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CallbackEventArgs : EventArgs
	{
		private string commandName;

		private string commandArgument;

		public string CommandName
		{
			get
			{
				return this.commandName;
			}
		}

		public string CommandArgument
		{
			get
			{
				return this.commandArgument;
			}
		}

		public CallbackEventArgs(string commandName, string commandArgument)
		{
			this.commandName = commandName;
			this.commandArgument = commandArgument;
		}
	}
}
