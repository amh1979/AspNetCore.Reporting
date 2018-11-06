using AspNetCore.Reporting.Chart.WebForms.Borders3D;
using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Data;
using AspNetCore.Reporting.Chart.WebForms.Formulas;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel.Design;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class CommonElements
	{
		internal ChartAreaCollection chartAreaCollection;

		internal ChartGraphics graph;

		internal IServiceContainer container;

		internal bool processModePaint = true;

		internal bool processModeRegions;

		private int width;

		private int height;

		internal DataManager DataManager
		{
			get
			{
				return (DataManager)this.container.GetService(typeof(DataManager));
			}
		}

		public bool ProcessModePaint
		{
			get
			{
				return this.processModePaint;
			}
		}

		public bool ProcessModeRegions
		{
			get
			{
				return this.processModeRegions;
			}
		}

		public HotRegionsList HotRegionsList
		{
			get
			{
				return this.ChartPicture.hotRegionsList;
			}
			set
			{
				this.ChartPicture.hotRegionsList = value;
			}
		}

		public DataManipulator DataManipulator
		{
			get
			{
				return this.ChartPicture.DataManipulator;
			}
		}

		internal ImageLoader ImageLoader
		{
			get
			{
				return (ImageLoader)this.container.GetService(typeof(ImageLoader));
			}
		}

		internal Chart Chart
		{
			get
			{
				return (Chart)this.container.GetService(typeof(Chart));
			}
		}

		internal EventsManager EventsManager
		{
			get
			{
				return (EventsManager)this.container.GetService(typeof(EventsManager));
			}
		}

		internal ChartTypeRegistry ChartTypeRegistry
		{
			get
			{
				return (ChartTypeRegistry)this.container.GetService(typeof(ChartTypeRegistry));
			}
		}

		internal BorderTypeRegistry BorderTypeRegistry
		{
			get
			{
				return (BorderTypeRegistry)this.container.GetService(typeof(BorderTypeRegistry));
			}
		}

		internal FormulaRegistry FormulaRegistry
		{
			get
			{
				return (FormulaRegistry)this.container.GetService(typeof(FormulaRegistry));
			}
		}

		public ChartImage ChartPicture
		{
			get
			{
				return (ChartImage)this.container.GetService(typeof(ChartImage));
			}
		}

		internal int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		internal int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		private CommonElements()
		{
		}

		public CommonElements(IServiceContainer container)
		{
			this.container = container;
		}

		public void TraceWrite(string category, string message)
		{
			if (this.container != null)
			{
				TraceManager traceManager = (TraceManager)this.container.GetService(typeof(TraceManager));
				if (traceManager != null)
				{
					traceManager.Write(category, message);
				}
			}
		}

		internal static double ParseDouble(string stringToParse)
		{
			double num = 0.0;
			try
			{
				return double.Parse(stringToParse, CultureInfo.InvariantCulture);
			}
			catch
			{
				return double.Parse(stringToParse, CultureInfo.CurrentCulture);
			}
		}

		internal static float ParseFloat(string stringToParse)
		{
			float num = 0f;
			try
			{
				return float.Parse(stringToParse, CultureInfo.InvariantCulture);
			}
			catch
			{
				return float.Parse(stringToParse, CultureInfo.CurrentCulture);
			}
		}
	}
}
