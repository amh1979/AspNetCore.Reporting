namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class GanttChart : BarChart
	{
		public override string Name
		{
			get
			{
				return "Gantt";
			}
		}

		public override bool ZeroCrossing
		{
			get
			{
				return true;
			}
		}

		public override int YValuesPerPoint
		{
			get
			{
				return 2;
			}
		}

		public override bool ExtraYValuesConnectedToYAxis
		{
			get
			{
				return true;
			}
		}

		public GanttChart()
		{
			base.useTwoValues = true;
			base.defLabelDrawingStyle = BarValueLabelDrawingStyle.Center;
		}
	}
}
