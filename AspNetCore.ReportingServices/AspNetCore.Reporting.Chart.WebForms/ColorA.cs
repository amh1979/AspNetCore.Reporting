using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ColorA
	{
		private Color startColor = Color.Transparent;

		private Color endColor = Color.Transparent;

		private double startTime;

		private double endTime;

		private bool repeat;

		private double repeatDelay;

		public Color StartColor
		{
			get
			{
				return this.startColor;
			}
			set
			{
				this.startColor = value;
			}
		}

		public Color EndColor
		{
			get
			{
				return this.endColor;
			}
			set
			{
				this.endColor = value;
			}
		}

		public double StartTime
		{
			get
			{
				return this.startTime;
			}
			set
			{
				this.startTime = value;
			}
		}

		public double EndTime
		{
			get
			{
				return this.endTime;
			}
			set
			{
				this.endTime = value;
			}
		}

		public bool Repeat
		{
			get
			{
				return this.repeat;
			}
			set
			{
				this.repeat = value;
			}
		}

		public double RepeatDelay
		{
			get
			{
				return this.repeatDelay;
			}
			set
			{
				this.repeatDelay = value;
			}
		}

		internal ColorA Copy()
		{
			ColorA colorA = new ColorA();
			colorA.endColor = this.endColor;
			colorA.endTime = this.endTime;
			colorA.repeat = this.repeat;
			colorA.startColor = this.startColor;
			colorA.startTime = this.startTime;
			return colorA;
		}
	}
}
