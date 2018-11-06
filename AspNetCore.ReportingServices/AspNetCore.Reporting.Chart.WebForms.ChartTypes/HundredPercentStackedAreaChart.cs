using System;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class HundredPercentStackedAreaChart : StackedAreaChart
	{
		private double[] totalPerPoint;

		public override string Name
		{
			get
			{
				return "100%StackedArea";
			}
		}

		public override bool HundredPercent
		{
			get
			{
				return true;
			}
		}

		public HundredPercentStackedAreaChart()
		{
			base.hundredPercentStacked = true;
		}

		public override void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.totalPerPoint = null;
			base.Paint(graph, common, area, seriesToDraw);
		}

		public override double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			if (this.totalPerPoint == null)
			{
				int num = 0;
				foreach (Series item in common.DataManager.Series)
				{
					if (string.Compare(item.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) == 0 && item.ChartArea == area.Name && item.IsVisible())
					{
						num++;
					}
				}
				Series[] array = new Series[num];
				int num2 = 0;
				foreach (Series item2 in common.DataManager.Series)
				{
					if (string.Compare(item2.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) == 0 && item2.ChartArea == area.Name && item2.IsVisible())
					{
						array[num2++] = item2;
					}
				}
				common.DataManipulator.CheckXValuesAlignment(array);
				this.totalPerPoint = new double[series.Points.Count];
				for (int i = 0; i < series.Points.Count; i++)
				{
					this.totalPerPoint[i] = 0.0;
					Series[] array2 = array;
					foreach (Series series4 in array2)
					{
						this.totalPerPoint[i] += Math.Abs(series4.Points[i].YValues[0]);
					}
				}
			}
			if (!area.Area3DStyle.Enable3D)
			{
				if (this.totalPerPoint[pointIndex] == 0.0)
				{
					int num4 = 0;
					foreach (Series item3 in common.DataManager.Series)
					{
						if (string.Compare(item3.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) == 0 && item3.ChartArea == area.Name && item3.IsVisible())
						{
							num4++;
						}
					}
					return 100.0 / (double)num4;
				}
				return point.YValues[0] / this.totalPerPoint[pointIndex] * 100.0;
			}
			double num5 = double.NaN;
			if (yValueIndex == -1)
			{
				Axis axis = area.GetAxis(AxisName.Y, series.YAxisType, series.YSubAxisName);
				double num6 = axis.Crossing;
				num5 = this.GetYValue(common, area, series, point, pointIndex, 0);
				if (area.Area3DStyle.Enable3D && num5 < 0.0)
				{
					num5 = 0.0 - num5;
				}
				if (num5 >= 0.0)
				{
					if (!double.IsNaN(base.prevPosY))
					{
						num6 = base.prevPosY;
					}
				}
				else if (!double.IsNaN(base.prevNegY))
				{
					num6 = base.prevNegY;
				}
				return num5 - num6;
			}
			base.prevPosY = double.NaN;
			base.prevNegY = double.NaN;
			base.prevPositionX = double.NaN;
			foreach (Series item4 in common.DataManager.Series)
			{
				if (string.Compare(series.ChartArea, item4.ChartArea, true, CultureInfo.CurrentCulture) == 0 && string.Compare(series.ChartTypeName, item4.ChartTypeName, true, CultureInfo.CurrentCulture) == 0 && series.IsVisible())
				{
					num5 = item4.Points[pointIndex].YValues[0] / this.totalPerPoint[pointIndex] * 100.0;
					if (!double.IsNaN(num5) && area.Area3DStyle.Enable3D && num5 < 0.0)
					{
						num5 = 0.0 - num5;
					}
					if (num5 >= 0.0 && !double.IsNaN(base.prevPosY))
					{
						num5 += base.prevPosY;
					}
					if (num5 < 0.0 && !double.IsNaN(base.prevNegY))
					{
						num5 += base.prevNegY;
					}
					if (string.Compare(series.Name, item4.Name, StringComparison.Ordinal) == 0)
					{
						break;
					}
					if (num5 >= 0.0)
					{
						base.prevPosY = num5;
					}
					else
					{
						base.prevNegY = num5;
					}
					base.prevPositionX = item4.Points[pointIndex].XValue;
					if (base.prevPositionX == 0.0 && ChartElement.IndexedSeries(series))
					{
						base.prevPositionX = (double)(pointIndex + 1);
					}
				}
			}
			if (num5 > 100.0)
			{
				return 100.0;
			}
			return num5;
		}
	}
}
