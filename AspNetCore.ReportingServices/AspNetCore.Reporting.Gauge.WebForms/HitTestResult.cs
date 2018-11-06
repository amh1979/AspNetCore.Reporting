using System.Drawing;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class HitTestResult
	{
		private PointF htPoint;

		private object obj;

		private HotRegion region;

		public object Object
		{
			get
			{
				return this.obj;
			}
		}

		public bool Success
		{
			get
			{
				return this.obj != null;
			}
		}

		public string Name
		{
			get
			{
				if (this.obj != null)
				{
					if (this.obj is NamedElement)
					{
						return ((NamedElement)this.obj).Name;
					}
					return this.obj.ToString();
				}
				return null;
			}
		}

		public double ScaleValue
		{
			get
			{
				if (this.obj is ScaleBase)
				{
					return ((ScaleBase)this.obj).GetValue(this.region.PinPoint, this.htPoint);
				}
				return 0.0;
			}
		}

		internal HitTestResult(HotRegion region, PointF hitTestPoint)
		{
			this.region = region;
			if (region != null)
			{
				this.obj = region.SelectedObject;
			}
			this.htPoint = hitTestPoint;
		}
	}
}
