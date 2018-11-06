namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class HitTestResult
	{
		private object obj;

		private Series series;

		private int dataPoint = -1;

		private ChartArea chartArea;

		private Axis axis;

		private ChartElementType type;

		private object subObject;

		public Series Series
		{
			get
			{
				return this.series;
			}
			set
			{
				this.series = value;
			}
		}

		public int PointIndex
		{
			get
			{
				return this.dataPoint;
			}
			set
			{
				this.dataPoint = value;
			}
		}

		public ChartArea ChartArea
		{
			get
			{
				return this.chartArea;
			}
			set
			{
				this.chartArea = value;
			}
		}

		public Axis Axis
		{
			get
			{
				return this.axis;
			}
			set
			{
				this.axis = value;
			}
		}

		public ChartElementType ChartElementType
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public object Object
		{
			get
			{
				return this.obj;
			}
			set
			{
				this.obj = value;
			}
		}

		public object SubObject
		{
			get
			{
				return this.subObject;
			}
			set
			{
				this.subObject = value;
			}
		}
	}
}
