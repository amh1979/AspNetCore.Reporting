using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class SegmentsCache
	{
		private Hashtable cacheTable = new Hashtable();

		private Matrix matrix = new Matrix();

		private float size = -1f;

		internal GraphicsPath GetSegment(Enum segments, PointF p, float size)
		{
			this.CheckCache(size);
			if (this.cacheTable.Contains(segments))
			{
				GraphicsPath graphicsPath = (GraphicsPath)((GraphicsPath)this.cacheTable[segments]).Clone();
				this.matrix.Reset();
				this.matrix.Translate(p.X, p.Y);
				graphicsPath.Transform(this.matrix);
				return graphicsPath;
			}
			return null;
		}

		internal void Reset()
		{
			foreach (object value in this.cacheTable.Values)
			{
				if (value is IDisposable)
				{
					((IDisposable)value).Dispose();
				}
			}
			this.cacheTable.Clear();
		}

		private void CheckCache(float size)
		{
			if (Math.Abs(this.size - size) > 1.4012984643248171E-45)
			{
				this.Reset();
				this.size = size;
			}
		}

		internal void SetSegment(Enum segment, GraphicsPath path, PointF p, float size)
		{
			this.CheckCache(size);
			GraphicsPath graphicsPath = (GraphicsPath)path.Clone();
			this.matrix.Reset();
			this.matrix.Translate((float)(0.0 - p.X), (float)(0.0 - p.Y));
			graphicsPath.Transform(this.matrix);
			this.cacheTable[segment] = graphicsPath;
		}
	}
}
