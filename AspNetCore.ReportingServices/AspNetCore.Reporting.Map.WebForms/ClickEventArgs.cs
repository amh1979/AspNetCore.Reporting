using System;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ClickEventArgs : EventArgs
	{
		private int x;

		private int y;

		private MapControl mapControl;

		private string returnCommandName = string.Empty;

		private string returnCommandArgument = string.Empty;

		public int X
		{
			get
			{
				return this.x;
			}
		}

		public int Y
		{
			get
			{
				return this.y;
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

		public ClickEventArgs(int x, int y, MapControl mapControl)
		{
			this.x = x;
			this.y = y;
			this.mapControl = mapControl;
		}
	}
}
