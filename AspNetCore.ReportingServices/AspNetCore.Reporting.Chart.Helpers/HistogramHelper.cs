using AspNetCore.Reporting.Chart.WebForms;
using System;

namespace AspNetCore.Reporting.Chart.Helpers
{
	internal class HistogramHelper
	{
		public int SegmentIntervalNumber = 20;

		public double SegmentIntervalWidth;

		public bool ShowPercentOnSecondaryYAxis = true;

		public void CreateHistogram(AspNetCore.Reporting.Chart.WebForms.Chart chartControl, string dataSeriesName, string histogramSeriesName, string histogramLegendText)
		{
			if (chartControl == null)
			{
				throw new ArgumentNullException("chartControl");
			}
			int index = chartControl.Series.GetIndex(dataSeriesName);
			if (index < 0)
			{
				throw new ArgumentException("Series with name'" + dataSeriesName + "' was not found.", "dataSeriesName");
			}
			Series series = chartControl.Series[dataSeriesName];
			Series series2 = null;
			if (chartControl.Series.GetIndex(histogramSeriesName) < 0)
			{
				series2 = new Series(histogramSeriesName);
				chartControl.Series.Insert(index, series2);
				series2.ChartType = series.ChartType;
				series2.LegendText = histogramLegendText;
				if (series.Points.Count > 0)
				{
					series2.BorderColor = series.Points[0].BorderColor;
					series2.BorderWidth = series.Points[0].BorderWidth;
					series2.BorderStyle = series.Points[0].BorderStyle;
				}
				series2.Color = series.Color;
				series2.BackGradientEndColor = series.BackGradientEndColor;
				series2.BackGradientType = series.BackGradientType;
				series2.Legend = series.Legend;
				series2.ChartArea = series.ChartArea;
				DataPointAttributes dataPointAttributes = series;
				int num = 0;
				while (num < series.Points.Count)
				{
					if (series.Points[num].Empty)
					{
						num++;
						continue;
					}
					dataPointAttributes = series.Points[num];
					break;
				}
				series2.LabelBackColor = dataPointAttributes.LabelBackColor;
				series2.LabelBorderColor = dataPointAttributes.LabelBorderColor;
				series2.LabelBorderWidth = dataPointAttributes.LabelBorderWidth;
				series2.LabelBorderStyle = dataPointAttributes.LabelBorderStyle;
				series2.LabelFormat = dataPointAttributes.LabelFormat;
				series2.ShowLabelAsValue = dataPointAttributes.ShowLabelAsValue;
				series2.Font = dataPointAttributes.Font;
				series2.FontColor = dataPointAttributes.FontColor;
				series2.FontAngle = dataPointAttributes.FontAngle;
			}
			double num2 = 1.7976931348623157E+308;
			double num3 = -1.7976931348623157E+308;
			int num4 = 0;
			foreach (DataPoint point in series.Points)
			{
				if (!point.Empty)
				{
					if (point.YValues[0] > num3)
					{
						num3 = point.YValues[0];
					}
					if (point.YValues[0] < num2)
					{
						num2 = point.YValues[0];
					}
					num4++;
				}
			}
			if (this.SegmentIntervalWidth == 0.0)
			{
				this.SegmentIntervalWidth = (num3 - num2) / (double)this.SegmentIntervalNumber;
				this.SegmentIntervalWidth = this.RoundInterval(this.SegmentIntervalWidth);
			}
			num2 = Math.Floor(num2 / this.SegmentIntervalWidth) * this.SegmentIntervalWidth;
			num3 = Math.Ceiling(num3 / this.SegmentIntervalWidth) * this.SegmentIntervalWidth;
			double num5 = num2;
			for (num5 = num2; num5 <= num3; num5 += this.SegmentIntervalWidth)
			{
				int num6 = 0;
				foreach (DataPoint point2 in series.Points)
				{
					if (!point2.Empty)
					{
						double num7 = num5 + this.SegmentIntervalWidth;
						if (point2.YValues[0] >= num5 && point2.YValues[0] < num7)
						{
							num6++;
						}
						else if (num7 >= num3 && point2.YValues[0] >= num5 && point2.YValues[0] <= num7)
						{
							num6++;
						}
					}
				}
				series2.Points.AddXY(num5 + this.SegmentIntervalWidth / 2.0, (double)num6);
			}
			((DataPointAttributes)series2)["PointWidth"] = "1";
			ChartArea chartArea = chartControl.ChartAreas[series2.ChartArea];
			chartArea.AxisY.Title = "Frequency";
			chartArea.AxisX.Minimum = num2;
			chartArea.AxisX.Maximum = num3;
			double num8 = this.SegmentIntervalWidth;
			bool flag = false;
			while ((num3 - num2) / num8 > 10.0)
			{
				num8 *= 2.0;
				flag = true;
			}
			chartArea.AxisX.Interval = num8;
			if (chartArea.AxisX.LabelStyle.ShowEndLabels && flag)
			{
				chartArea.AxisX.Maximum = num2 + Math.Ceiling((num3 - num2) / num8) * num8;
			}
			chartControl.Series.Remove(series);
			chartArea.AxisY2.Enabled = AxisEnabled.Auto;
			if (this.ShowPercentOnSecondaryYAxis)
			{
				chartArea.Recalculate();
				chartArea.AxisY2.Enabled = AxisEnabled.True;
				chartArea.AxisY2.LabelStyle.Format = "P0";
				chartArea.AxisY2.MajorGrid.Enabled = false;
				chartArea.AxisY2.Title = "Percent of Total";
				chartArea.AxisY2.Minimum = chartArea.AxisY.Minimum / (double)num4;
				chartArea.AxisY2.Maximum = chartArea.AxisY.Maximum / (double)num4;
				double num9 = (chartArea.AxisY2.Maximum > 0.2) ? 0.05 : 0.01;
				chartArea.AxisY2.Interval = Math.Ceiling(chartArea.AxisY2.Maximum / 5.0 / num9) * num9;
			}
		}

		internal double RoundInterval(double interval)
		{
			if (interval == 0.0)
			{
				throw new ArgumentOutOfRangeException("interval", "Interval can not be zero.");
			}
			double num = -1.0;
			double num2 = interval;
			while (num2 > 1.0)
			{
				num += 1.0;
				num2 /= 10.0;
				if (num > 1000.0)
				{
					throw new InvalidOperationException("Auto interval error due to invalid point values or axis minimum/maximum.");
				}
			}
			num2 = interval;
			if (num2 < 1.0)
			{
				num = 0.0;
			}
			while (num2 < 1.0)
			{
				num -= 1.0;
				num2 *= 10.0;
				if (num < -1000.0)
				{
					throw new InvalidOperationException("Auto interval error due to invalid point values or axis minimum/maximum.");
				}
			}
			double num3 = interval / Math.Pow(10.0, num);
			num3 = ((!(num3 < 3.0)) ? ((!(num3 < 7.0)) ? 10.0 : 5.0) : 2.0);
			return num3 * Math.Pow(10.0, num);
		}
	}
}
