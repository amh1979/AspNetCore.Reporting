using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class PointA
	{
		private ValueA x = new ValueA();

		private ValueA y = new ValueA();

		public ValueA X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
			}
		}

		public ValueA Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
			}
		}

		public PointF ToPointF()
		{
			return new PointF(this.x.EndValue, this.y.EndValue);
		}
	}
}
