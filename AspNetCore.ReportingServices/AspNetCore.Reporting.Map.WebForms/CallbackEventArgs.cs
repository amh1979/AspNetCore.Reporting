using System;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class CallbackEventArgs : EventArgs
	{
		private string commandName;

		private string commandArgument;

		private MapControl mapControl;

		private string returnCommandName = string.Empty;

		private string returnCommandArgument = string.Empty;

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

		public MapControl MapControl
		{
			get
			{
				return this.mapControl;
			}
		}

		public string ReturnCommandName
		{
			get
			{
				return this.returnCommandName;
			}
			set
			{
				this.returnCommandName = value;
			}
		}

		public string ReturnCommandArgument
		{
			get
			{
				return this.returnCommandArgument;
			}
			set
			{
				this.returnCommandArgument = value;
			}
		}

		public CallbackEventArgs(string commandName, string commandArgument, MapControl mapControl)
		{
			this.commandName = commandName;
			this.commandArgument = commandArgument;
			this.mapControl = mapControl;
		}
	}
}
