namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ValueA
	{
		private float startValue;

		private float endValue;

		private double startTime;

		private double endTime;

		private bool repeat;

		private double repeatDelay;

		public float StartValue
		{
			get
			{
				return this.startValue;
			}
			set
			{
				this.startValue = value;
			}
		}

		public float EndValue
		{
			get
			{
				return this.endValue;
			}
			set
			{
				this.endValue = value;
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
	}
}
