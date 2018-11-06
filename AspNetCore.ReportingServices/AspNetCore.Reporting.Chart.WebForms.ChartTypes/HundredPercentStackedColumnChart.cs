using System;
using System.Collections;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class HundredPercentStackedColumnChart : StackedColumnChart
	{
		private Hashtable stackedGroupsTotalPerPoint;

		public override string Name
		{
			get
			{
				return "100%StackedColumn";
			}
		}

		public override bool HundredPercent
		{
			get
			{
				return true;
			}
		}

		public override bool HundredPercentSupportNegative
		{
			get
			{
				return true;
			}
		}

		public HundredPercentStackedColumnChart()
		{
			base.hundredPercentStacked = true;
		}

		public override void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.stackedGroupsTotalPerPoint = null;
			base.Paint(graph, common, area, seriesToDraw);
		}

		public override double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			double[] array = null;
			string seriesStackGroupName = StackedColumnChart.GetSeriesStackGroupName(series);
			if (this.stackedGroupsTotalPerPoint == null)
			{
				this.stackedGroupsTotalPerPoint = new Hashtable();
				foreach (string stackGroupName in base.stackGroupNames)
				{
					Series[] seriesByStackedGroupName = StackedColumnChart.GetSeriesByStackedGroupName(common, stackGroupName, series.ChartTypeName, series.ChartArea);
					common.DataManipulator.CheckXValuesAlignment(seriesByStackedGroupName);
					double[] array2 = new double[series.Points.Count];
					for (int i = 0; i < series.Points.Count; i++)
					{
						array2[i] = 0.0;
						Series[] array3 = seriesByStackedGroupName;
						foreach (Series series2 in array3)
						{
							array2[i] += Math.Abs(series2.Points[i].YValues[0]);
						}
					}
					this.stackedGroupsTotalPerPoint.Add(stackGroupName, array2);
				}
			}
			array = (double[])this.stackedGroupsTotalPerPoint[seriesStackGroupName];
			if (!area.Area3DStyle.Enable3D && (point.YValues[0] == 0.0 || point.Empty))
			{
				return 0.0;
			}
			if (area.Area3DStyle.Enable3D && yValueIndex != -2)
			{
				double num = double.NaN;
				if (yValueIndex == -1)
				{
					Axis axis = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
					double num2 = axis.Crossing;
					num = this.GetYValue(common, area, series, point, pointIndex, 0);
					if (num >= 0.0)
					{
						if (!double.IsNaN(base.prevPosY))
						{
							num2 = base.prevPosY;
						}
					}
					else if (!double.IsNaN(base.prevNegY))
					{
						num2 = base.prevNegY;
					}
					return num - num2;
				}
				base.prevPosY = double.NaN;
				base.prevNegY = double.NaN;
				foreach (Series item in common.DataManager.Series)
				{
					if (string.Compare(series.ChartArea, item.ChartArea, StringComparison.Ordinal) == 0 && string.Compare(series.ChartTypeName, item.ChartTypeName, StringComparison.OrdinalIgnoreCase) == 0 && item.IsVisible() && !(seriesStackGroupName != StackedColumnChart.GetSeriesStackGroupName(item)))
					{
						if (double.IsNaN(num))
						{
							num = ((array[pointIndex] != 0.0) ? (item.Points[pointIndex].YValues[0] / array[pointIndex] * 100.0) : 0.0);
						}
						else
						{
							num = ((array[pointIndex] != 0.0) ? (item.Points[pointIndex].YValues[0] / array[pointIndex] * 100.0) : 0.0);
							if (num >= 0.0 && !double.IsNaN(base.prevPosY))
							{
								num += base.prevPosY;
							}
							if (num < 0.0 && !double.IsNaN(base.prevNegY))
							{
								num += base.prevNegY;
							}
						}
						if (string.Compare(series.Name, item.Name, StringComparison.Ordinal) == 0)
						{
							break;
						}
						if (num >= 0.0)
						{
							base.prevPosY = num;
						}
						else
						{
							base.prevNegY = num;
						}
					}
				}
				if (!(num > 100.0))
				{
					return num;
				}
				return 100.0;
			}
			if (array[pointIndex] == 0.0)
			{
				return 0.0;
			}
			return point.YValues[0] / array[pointIndex] * 100.0;
		}
	}
}
