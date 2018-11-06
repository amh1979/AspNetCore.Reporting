using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class XamlLayer : IDisposable
	{
		private PointF origin = PointF.Empty;

		private GraphicsPath[] paths;

		private Brush[] brushes;

		private Pen[] pens;

		private XamlLayer[] innerLayers;

		private bool disposed;

		public PointF Origin
		{
			get
			{
				return this.origin;
			}
		}

		public GraphicsPath[] Paths
		{
			get
			{
				return this.paths;
			}
			set
			{
				if (this.paths != value && this.paths != null)
				{
					GraphicsPath[] array = this.paths;
					foreach (GraphicsPath graphicsPath in array)
					{
						if (graphicsPath != null)
						{
							graphicsPath.Dispose();
						}
					}
				}
				this.paths = value;
			}
		}

		public Brush[] Brushes
		{
			get
			{
				return this.brushes;
			}
			set
			{
				if (this.brushes != value && this.brushes != null)
				{
					Brush[] array = this.brushes;
					foreach (Brush brush in array)
					{
						if (brush != null)
						{
							brush.Dispose();
						}
					}
				}
				this.brushes = value;
			}
		}

		public Pen[] Pens
		{
			get
			{
				return this.pens;
			}
			set
			{
				if (this.pens != value && this.pens != null)
				{
					Pen[] array = this.pens;
					foreach (Pen pen in array)
					{
						if (pen != null)
						{
							pen.Dispose();
						}
					}
				}
				this.pens = value;
			}
		}

		public XamlLayer[] InnerLayers
		{
			get
			{
				return this.innerLayers;
			}
			set
			{
				if (this.innerLayers != value && this.innerLayers != null)
				{
					XamlLayer[] array = this.innerLayers;
					foreach (XamlLayer xamlLayer in array)
					{
						if (xamlLayer != null)
						{
							xamlLayer.Dispose();
						}
					}
				}
				this.innerLayers = value;
			}
		}

		public XamlLayer(PointF origin)
		{
			this.origin = origin;
		}

		public void Render(MapGraphics g)
		{
			if (this.InnerLayers != null)
			{
				for (int i = 0; i < this.InnerLayers.Length; i++)
				{
					this.InnerLayers[i].Render(g);
				}
			}
			if (this.Paths != null)
			{
				g.TranslateTransform(this.Origin.X, this.Origin.Y);
				for (int j = 0; j < this.Paths.Length; j++)
				{
					if (this.Brushes[j] != null)
					{
						g.FillPath(this.Brushes[j], this.Paths[j]);
					}
					if (this.Pens[j] != null)
					{
						g.DrawPath(this.Pens[j], this.Paths[j]);
					}
				}
				g.TranslateTransform((float)(0.0 - this.Origin.X), (float)(0.0 - this.Origin.Y));
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.Paths = null;
				this.Brushes = null;
				this.Pens = null;
				this.InnerLayers = null;
			}
			this.disposed = true;
		}

		~XamlLayer()
		{
			this.Dispose(false);
		}
	}
}
