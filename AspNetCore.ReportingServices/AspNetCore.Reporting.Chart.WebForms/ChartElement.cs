using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal abstract class ChartElement
	{
		internal const int MaxNumOfGridlines = 10000;

		private ElementPosition plotAreaPosition;

		private CommonElements common;

		private object tag;

		internal CommonElements Common
		{
			get
			{
				CommonElements common2 = this.common;
				return this.common;
			}
			set
			{
				this.common = value;
			}
		}

		internal ElementPosition PlotAreaPosition
		{
			get
			{
				return this.plotAreaPosition;
			}
			set
			{
				this.plotAreaPosition = value;
			}
		}

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public object Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		internal ChartElement()
		{
		}

		internal double AlignIntervalStart(double start, double intervalSize, DateTimeIntervalType type)
		{
			return this.AlignIntervalStart(start, intervalSize, type, null);
		}

		internal double AlignIntervalStart(double start, double intervalSize, DateTimeIntervalType type, Series series)
		{
			return this.AlignIntervalStart(start, intervalSize, type, series, true);
		}

		internal double AlignIntervalStart(double start, double intervalSize, DateTimeIntervalType type, Series series, bool majorInterval)
		{
			if (series != null && series.XValueIndexed)
			{
				if (type != 0 && type != DateTimeIntervalType.Number)
				{
					return (double)(-(series.Points.Count + 1));
				}
				if (majorInterval)
				{
					return 1.0;
				}
				return 0.0;
			}
			if (type != 0 && type != DateTimeIntervalType.Number)
			{
				DateTime dateTime = DateTime.FromOADate(start);
				if (intervalSize > 0.0 && intervalSize != 1.0 && type == DateTimeIntervalType.Months && intervalSize <= 12.0 && intervalSize > 1.0)
				{
					DateTime dateTime2 = dateTime;
					DateTime dateTime3 = new DateTime(dateTime.Year, 1, 1, 0, 0, 0);
					while (dateTime3 < dateTime)
					{
						dateTime2 = dateTime3;
						dateTime3 = dateTime3.AddMonths((int)intervalSize);
					}
					dateTime = dateTime2;
					return dateTime.ToOADate();
				}
				switch (type)
				{
				case DateTimeIntervalType.Years:
				{
					int num2 = (int)((double)(int)((double)dateTime.Year / intervalSize) * intervalSize);
					if (num2 <= 0)
					{
						num2 = 1;
					}
					dateTime = new DateTime(num2, 1, 1, 0, 0, 0);
					break;
				}
				case DateTimeIntervalType.Months:
				{
					int num3 = (int)((double)(int)((double)dateTime.Month / intervalSize) * intervalSize);
					if (num3 <= 0)
					{
						num3 = 1;
					}
					dateTime = new DateTime(dateTime.Year, num3, 1, 0, 0, 0);
					break;
				}
				case DateTimeIntervalType.Days:
				{
					int num = (int)((double)(int)((double)dateTime.Day / intervalSize) * intervalSize);
					if (num <= 0)
					{
						num = 1;
					}
					dateTime = new DateTime(dateTime.Year, dateTime.Month, num, 0, 0, 0);
					break;
				}
				case DateTimeIntervalType.Hours:
				{
					int hour = (int)((double)(int)((double)dateTime.Hour / intervalSize) * intervalSize);
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, 0, 0);
					break;
				}
				case DateTimeIntervalType.Minutes:
				{
					int minute = (int)((double)(int)((double)dateTime.Minute / intervalSize) * intervalSize);
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, minute, 0);
					break;
				}
				case DateTimeIntervalType.Seconds:
				{
					int second = (int)((double)(int)((double)dateTime.Second / intervalSize) * intervalSize);
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, second, 0);
					break;
				}
				case DateTimeIntervalType.Milliseconds:
				{
					int millisecond = (int)((double)(int)((double)dateTime.Millisecond / intervalSize) * intervalSize);
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, millisecond);
					break;
				}
				case DateTimeIntervalType.Weeks:
					dateTime = dateTime.AddDays((double)(0 - dateTime.DayOfWeek));
					dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
					break;
				}
				return dateTime.ToOADate();
			}
			return start;
		}

		internal double GetIntervalSize(double current, double interval, DateTimeIntervalType type)
		{
			return this.GetIntervalSize(current, interval, type, null, 0.0, DateTimeIntervalType.Number, true, true);
		}

		internal double GetIntervalSize(double current, double interval, DateTimeIntervalType type, Series series, double intervalOffset, DateTimeIntervalType intervalOffsetType, bool forceIntIndex)
		{
			return this.GetIntervalSize(current, interval, type, series, intervalOffset, intervalOffsetType, forceIntIndex, true);
		}

		internal double GetIntervalSize(double current, double interval, DateTimeIntervalType type, Series series, double intervalOffset, DateTimeIntervalType intervalOffsetType, bool forceIntIndex, bool forceAbsInterval)
		{
			double num2;
			double num3;
			int num;
			if (type != DateTimeIntervalType.Number && type != 0)
			{
				if (series != null && series.XValueIndexed)
				{
					num = (int)Math.Ceiling(current - 1.0);
					if (num < 0)
					{
						num = 0;
					}
					if (num < series.Points.Count && series.Points.Count > 1)
					{
						num2 = 0.0;
						double xValue = series.Points[num].XValue;
						xValue = this.AlignIntervalStart(xValue, 1.0, type, null);
						num3 = xValue + this.GetIntervalSize(xValue, interval, type);
						num3 += this.GetIntervalSize(num3, intervalOffset, intervalOffsetType);
						xValue += this.GetIntervalSize(xValue, intervalOffset, intervalOffsetType);
						if (intervalOffset < 0.0)
						{
							xValue += this.GetIntervalSize(xValue, interval, type);
							num3 += this.GetIntervalSize(num3, interval, type);
						}
						if (num == 0 && current < 0.0)
						{
							DateTime dateTime = DateTime.FromOADate(series.Points[num].XValue);
							DateTime dateTime2 = dateTime;
							switch (type)
							{
							case DateTimeIntervalType.Years:
								dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
								break;
							case DateTimeIntervalType.Months:
								dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
								break;
							case DateTimeIntervalType.Days:
								dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
								break;
							case DateTimeIntervalType.Hours:
								dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
								break;
							case DateTimeIntervalType.Minutes:
								dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
								break;
							case DateTimeIntervalType.Seconds:
								dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0);
								break;
							case DateTimeIntervalType.Weeks:
								dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
								break;
							}
							if (dateTime2.ToOADate() != xValue && dateTime2.ToOADate() != num3)
							{
								goto IL_0273;
							}
							return 0.0 - current + 1.0;
						}
						goto IL_0273;
					}
					return interval;
				}
				DateTime dateTime3 = DateTime.FromOADate(current);
				TimeSpan value = new TimeSpan(0L);
				switch (type)
				{
				case DateTimeIntervalType.Days:
					value = TimeSpan.FromDays(interval);
					break;
				case DateTimeIntervalType.Hours:
					value = TimeSpan.FromHours(interval);
					break;
				case DateTimeIntervalType.Milliseconds:
					value = TimeSpan.FromMilliseconds(interval);
					break;
				case DateTimeIntervalType.Seconds:
					value = TimeSpan.FromSeconds(interval);
					break;
				case DateTimeIntervalType.Minutes:
					value = TimeSpan.FromMinutes(interval);
					break;
				case DateTimeIntervalType.Weeks:
					value = TimeSpan.FromDays(7.0 * interval);
					break;
				case DateTimeIntervalType.Months:
				{
					bool flag = false;
					if (dateTime3.Day == DateTime.DaysInMonth(dateTime3.Year, dateTime3.Month))
					{
						flag = true;
					}
					dateTime3 = dateTime3.AddMonths((int)Math.Floor(interval));
					value = TimeSpan.FromDays(30.0 * (interval - Math.Floor(interval)));
					if (flag && value.Ticks == 0)
					{
						int num4 = DateTime.DaysInMonth(dateTime3.Year, dateTime3.Month);
						dateTime3 = dateTime3.AddDays((double)(num4 - dateTime3.Day));
					}
					break;
				}
				case DateTimeIntervalType.Years:
					dateTime3 = dateTime3.AddYears((int)Math.Floor(interval));
					value = TimeSpan.FromDays(365.0 * (interval - Math.Floor(interval)));
					break;
				}
				double num5 = dateTime3.Add(value).ToOADate() - current;
				if (forceAbsInterval)
				{
					num5 = Math.Abs(num5);
				}
				return num5;
			}
			return interval;
			IL_0273:
			num++;
			while (num < series.Points.Count)
			{
				if (!(series.Points[num].XValue >= num3))
				{
					num++;
					continue;
				}
				if (!(series.Points[num].XValue > num3) || forceIntIndex)
				{
					break;
				}
				num2 = -0.5;
				break;
			}
			if (num == series.Points.Count)
			{
				num += series.Points.Count / 5 + 1;
			}
			double num6 = (double)(num + 1) - current + num2;
			if (num6 == 0.0)
			{
				return interval;
			}
			return num6;
		}

		internal static bool IndexedSeries(Series series)
		{
			if (series.XValueIndexed)
			{
				return true;
			}
			return ChartElement.SeriesXValuesZeros(series);
		}

		internal static bool SeriesXValuesZeros(Series series)
		{
			if (series.xValuesZerosChecked)
			{
				return series.xValuesZeros;
			}
			series.xValuesZerosChecked = true;
			series.xValuesZeros = true;
			if (series.Points.Count == 1 && (series.ChartType == SeriesChartType.Point || series.ChartType == SeriesChartType.Bubble))
			{
				series.xValuesZeros = !string.IsNullOrEmpty(series.Points[0].AxisLabel);
			}
			foreach (DataPoint point in series.Points)
			{
				if (point.XValue != 0.0)
				{
					series.xValuesZeros = false;
					break;
				}
			}
			return series.xValuesZeros;
		}

		internal bool IndexedSeries(params string[] series)
		{
			bool flag = true;
			foreach (string parameter in series)
			{
				if (this.Common.DataManager.Series[parameter].XValueIndexed)
				{
					return true;
				}
				if (flag && !this.SeriesXValuesZeros(series))
				{
					flag = false;
				}
			}
			return flag;
		}

		internal bool SeriesXValuesZeros(params string[] series)
		{
			foreach (string parameter in series)
			{
				if (!ChartElement.SeriesXValuesZeros(this.Common.DataManager.Series[parameter]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
