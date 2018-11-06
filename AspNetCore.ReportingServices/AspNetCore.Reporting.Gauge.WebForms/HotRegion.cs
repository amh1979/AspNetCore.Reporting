using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class HotRegion : IDisposable
	{
		private GraphicsPath[] paths;

		private RectangleF boundingRectangle = RectangleF.Empty;

		private object selectedObject;

		private PointF circularPinPoint = PointF.Empty;

		private Matrix relMatrix = new Matrix();

		private Matrix absMatrix = new Matrix();

		private PointF[] pointsRect = new PointF[4];

		private PointF[] pointsPoint = new PointF[1];

		protected bool disposed;

		internal GraphicsPath[] Paths
		{
			get
			{
				return this.paths;
			}
			set
			{
				this.paths = value;
			}
		}

		internal RectangleF BoundingRectangle
		{
			get
			{
				if (this.boundingRectangle == RectangleF.Empty && this.paths.Length > 0)
				{
					RectangleF bounds = this.paths[0].GetBounds();
					for (int i = 1; i < this.paths.Length; i++)
					{
						if (this.paths[i] != null)
						{
							bounds.Intersect(this.paths[i].GetBounds());
						}
					}
					this.boundingRectangle = bounds;
				}
				return this.boundingRectangle;
			}
		}

		internal object SelectedObject
		{
			get
			{
				return this.selectedObject;
			}
			set
			{
				this.selectedObject = value;
			}
		}

		internal PointF PinPoint
		{
			get
			{
				return this.circularPinPoint;
			}
			set
			{
				this.circularPinPoint = value;
			}
		}

		internal HotRegion()
		{
		}

		internal void BuildMatrices(GaugeGraphics g)
		{
			this.absMatrix.Reset();
			RectangleF rectangleF = new RectangleF(0f, 0f, 1f, 1f);
			RectangleF rectangleF2 = g.GetAbsoluteRectangle(rectangleF);
			this.absMatrix.Translate(g.Transform.OffsetX, g.Transform.OffsetY);
			this.absMatrix.Scale(rectangleF2.Size.Width, rectangleF2.Size.Height);
			rectangleF2 = g.GetRelativeRectangle(rectangleF);
			this.relMatrix.Reset();
			this.relMatrix.Scale(rectangleF2.Size.Width, rectangleF2.Size.Height);
			this.relMatrix.Translate((float)(0.0 - g.Transform.OffsetX), (float)(0.0 - g.Transform.OffsetY));
		}

		internal RectangleF GetAbsoluteRectangle(RectangleF relativeRect)
		{
			return new RectangleF(this.GetAbsolutePoint(relativeRect.Location), this.GetAbsoluteSize(relativeRect.Size));
		}

		internal RectangleF GetRelativeRectangle(RectangleF absoluteRect)
		{
			return new RectangleF(this.GetRelativePoint(absoluteRect.Location), this.GetRelativeSize(absoluteRect.Size));
		}

		internal PointF GetAbsolutePoint(PointF relativePoint)
		{
			this.pointsPoint[0] = relativePoint;
			this.absMatrix.TransformPoints(this.pointsPoint);
			return this.pointsPoint[0];
		}

		internal PointF GetRelativePoint(PointF absolutePoint)
		{
			this.pointsPoint[0] = absolutePoint;
			this.relMatrix.TransformPoints(this.pointsPoint);
			return this.pointsPoint[0];
		}

		public SizeF GetAbsoluteSize(SizeF relativeSize)
		{
			return new SizeF(this.GetAbsolutePoint(relativeSize.ToPointF()));
		}

		internal SizeF GetRelativeSize(SizeF absoluteSize)
		{
			return new SizeF(this.GetRelativePoint(absoluteSize.ToPointF()));
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
				if (this.paths != null)
				{
					GraphicsPath[] array = this.paths;
					foreach (GraphicsPath graphicsPath in array)
					{
						if (graphicsPath != null)
						{
							graphicsPath.Dispose();
						}
					}
					this.paths = null;
				}
				if (this.relMatrix != null)
				{
					this.relMatrix.Dispose();
				}
				if (this.absMatrix != null)
				{
					this.absMatrix.Dispose();
				}
			}
			this.disposed = true;
		}
	}
}
