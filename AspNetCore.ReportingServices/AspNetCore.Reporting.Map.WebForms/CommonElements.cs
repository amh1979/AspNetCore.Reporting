using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class CommonElements
	{
		private MapGraphics graph;

		internal IServiceContainer container;

		internal bool processModePaint = true;

		internal bool processModeRegions;

		private int width;

		private int height;

		private MapCore mapCore;

		internal Size Size
		{
			get
			{
				return new Size(this.width, this.height);
			}
		}

		internal bool ProcessModePaint
		{
			get
			{
				return this.processModePaint;
			}
		}

		internal bool ProcessModeRegions
		{
			get
			{
				return this.processModeRegions;
			}
		}

		internal ImageLoader ImageLoader
		{
			get
			{
				return (ImageLoader)this.container.GetService(typeof(ImageLoader));
			}
		}

		internal MapCore MapCore
		{
			get
			{
				if (this.mapCore == null)
				{
					this.mapCore = (MapCore)this.container.GetService(typeof(MapCore));
				}
				return this.mapCore;
			}
		}

		internal MapControl MapControl
		{
			get
			{
				return this.MapCore.MapControl;
			}
		}

		internal bool IsGraphicsInitialized
		{
			get
			{
				return this.graph != null;
			}
		}

		internal MapGraphics Graph
		{
			get
			{
				if (this.graph != null)
				{
					return this.graph;
				}
				throw new ApplicationException(SR.gdi_noninitialized);
			}
			set
			{
				this.graph = value;
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

		internal BorderTypeRegistry BorderTypeRegistry
		{
			get
			{
				return (BorderTypeRegistry)this.container.GetService(typeof(BorderTypeRegistry));
			}
		}

		internal CommonElements(IServiceContainer container)
		{
			this.container = container;
		}

		internal void InvokePrePaint(NamedElement sender)
		{
			this.MapControl.OnPrePaint(this.MapControl, new MapPaintEventArgs(this.MapControl, sender, this.graph));
		}

		internal void InvokePostPaint(NamedElement sender)
		{
			this.MapControl.OnPostPaint(this.MapControl, new MapPaintEventArgs(this.MapControl, sender, this.graph));
		}

		internal void InvokeElementAdded(NamedElement sender)
		{
			this.MapControl.OnElementAdded(this.MapControl, new ElementEventArgs(this.MapControl, sender));
		}

		internal void InvokeElementRemoved(NamedElement sender)
		{
			this.MapControl.OnElementRemoved(this.MapControl, new ElementEventArgs(this.MapControl, sender));
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
