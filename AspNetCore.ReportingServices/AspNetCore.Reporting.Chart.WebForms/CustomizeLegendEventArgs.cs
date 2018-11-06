using System;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class CustomizeLegendEventArgs : EventArgs
	{
		private LegendItemsCollection legendItems;

		private string legendName = "";

		public string LegendName
		{
			get
			{
				return this.legendName;
			}
		}

		public LegendItemsCollection LegendItems
		{
			get
			{
				return this.legendItems;
			}
		}

		private CustomizeLegendEventArgs()
		{
		}

		public CustomizeLegendEventArgs(LegendItemsCollection legendItems)
		{
			this.legendItems = legendItems;
		}

		public CustomizeLegendEventArgs(LegendItemsCollection legendItems, string legendName)
		{
			this.legendItems = legendItems;
			this.legendName = legendName;
		}
	}
}
