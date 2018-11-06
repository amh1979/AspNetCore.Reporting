using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
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

		private PointF lastOffset = PointF.Empty;

		private bool doNotDispose;

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
				this.BoundingRectangle = RectangleF.Empty;
			}
		}

		internal RectangleF BoundingRectangle
		{
			get
			{
				if (this.boundingRectangle == RectangleF.Empty && this.paths.Length > 0)
				{
					RectangleF a = this.paths[0].GetBounds();
					for (int i = 1; i < this.paths.Length; i++)
					{
						if (this.paths[i] != null)
						{
							a = RectangleF.Union(a, this.paths[i].GetBounds());
						}
					}
					this.boundingRectangle = a;
				}
				return this.boundingRectangle;
			}
			set
			{
				this.boundingRectangle = value;
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

		internal Matrix AbsMatrix
		{
			get
			{
				return this.absMatrix;
			}
		}

		internal Matrix RelMatrix
		{
			get
			{
				return this.relMatrix;
			}
		}

		internal bool DoNotDispose
		{
			get
			{
				return this.doNotDispose;
			}
			set
			{
				this.doNotDispose = value;
			}
		}

		internal HotRegion()
		{
		}

		internal void BuildMatrices(MapGraphics g)
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

		internal RectangleF GetAbsRectangle(RectangleF relRect)
		{
			return new RectangleF(this.GetAbsPoint(relRect.Location), this.GetAbsSize(relRect.Size));
		}

		internal RectangleF GetRelRectangle(RectangleF absRect)
		{
			return new RectangleF(this.GetRelPoint(absRect.Location), this.GetRelSize(absRect.Size));
		}

		internal PointF GetAbsPoint(PointF relPoint)
		{
			this.pointsPoint[0] = relPoint;
			this.absMatrix.TransformPoints(this.pointsPoint);
			return this.pointsPoint[0];
		}

		internal PointF GetRelPoint(PointF absPoint)
		{
			this.pointsPoint[0] = absPoint;
			this.relMatrix.TransformPoints(this.pointsPoint);
			return this.pointsPoint[0];
		}

		public SizeF GetAbsSize(SizeF relSize)
		{
			return new SizeF(this.GetAbsPoint(relSize.ToPointF()));
		}

		internal SizeF GetRelSize(SizeF absSize)
		{
			return new SizeF(this.GetRelPoint(absSize.ToPointF()));
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && !this.DoNotDispose && disposing && this.paths != null)
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
			this.disposed = true;
		}

		internal void OffsetBy(PointF sectionOffset)
		{
			PointF pointF = new PointF(sectionOffset.X - this.lastOffset.X, sectionOffset.Y - this.lastOffset.Y);
			using (Matrix matrix = new Matrix())
			{
				matrix.Translate(pointF.X, pointF.Y);
				GraphicsPath[] array = this.Paths;
				foreach (GraphicsPath graphicsPath in array)
				{
					if (graphicsPath != null)
					{
						graphicsPath.Transform(matrix);
					}
				}
			}
			this.lastOffset = sectionOffset;
		}
	}
}
