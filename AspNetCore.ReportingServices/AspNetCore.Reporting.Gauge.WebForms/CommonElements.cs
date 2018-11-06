using System;
using System.ComponentModel.Design;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CommonElements
	{
		private GaugeGraphics graph;

		internal IServiceContainer container;

		internal bool processModePaint = true;

		internal bool processModeRegions;

		internal ObjectLinker objectLinker;

		private int width;

		private int height;

		private GaugeCore gaugeCore;

		internal ImageLoader ImageLoader
		{
			get
			{
				return (ImageLoader)this.container.GetService(typeof(ImageLoader));
			}
		}

		internal GaugeCore GaugeCore
		{
			get
			{
				if (this.gaugeCore == null)
				{
					this.gaugeCore = (GaugeCore)this.container.GetService(typeof(GaugeCore));
				}
				return this.gaugeCore;
			}
		}

		internal GaugeContainer GaugeContainer
		{
			get
			{
				return this.GaugeCore.GaugeContainer;
			}
		}

		internal GaugeGraphics Graph
		{
			get
			{
				if (this.graph != null)
				{
					return this.graph;
				}
				throw new ApplicationException(Utils.SRGetStr("ExceptionGdiNonInitialized"));
			}
			set
			{
				this.graph = value;
			}
		}

		internal ObjectLinker ObjectLinker
		{
			get
			{
				return this.objectLinker;
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

		internal CommonElements(IServiceContainer container)
		{
			this.container = container;
			this.objectLinker = new ObjectLinker(this.GaugeCore);
			this.objectLinker.Common = this;
		}

		internal void InvokePrePaint(object sender)
		{
			this.GaugeContainer.OnPrePaint(sender, new GaugePaintEventArgs(this.GaugeContainer, this.graph));
		}

		internal void InvokePostPaint(object sender)
		{
			this.GaugeContainer.OnPostPaint(sender, new GaugePaintEventArgs(this.GaugeContainer, this.graph));
		}
	}
}
