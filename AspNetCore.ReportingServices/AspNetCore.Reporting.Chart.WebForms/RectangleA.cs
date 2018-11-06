using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class RectangleA
	{
		private ValueA x = new ValueA();

		private ValueA y = new ValueA();

		private ValueA width = new ValueA();

		private ValueA height = new ValueA();

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

		public ValueA Width
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

		public ValueA Height
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

		public RectangleF ToRectangleF()
		{
			return new RectangleF(this.x.EndValue, this.y.EndValue, this.width.EndValue, this.height.EndValue);
		}
	}
}
