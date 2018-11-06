using System;
using System.Collections;
using System.Data;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class DataManipulator : FormulaData
	{
		private class PointElementFilter : IDataPointFilter
		{
			private DataManipulator dataManipulator;

			private DateRangeType dateRange;

			private int[] rangeElements;

			private PointElementFilter()
			{
			}

			public PointElementFilter(DataManipulator dataManipulator, DateRangeType dateRange, string rangeElements)
			{
				this.dataManipulator = dataManipulator;
				this.dateRange = dateRange;
				this.rangeElements = dataManipulator.ConvertElementIndexesToArray(rangeElements);
			}

			public bool FilterDataPoint(DataPoint point, Series series, int pointIndex)
			{
				return this.dataManipulator.CheckFilterElementCriteria(this.dateRange, this.rangeElements, point, series, pointIndex);
			}
		}

		private class PointValueFilter : IDataPointFilter
		{
			private DataManipulator dataManipulator;

			private CompareMethod compareMethod;

			private string usingValue;

			private double compareValue;

			private PointValueFilter()
			{
			}

			public PointValueFilter(DataManipulator dataManipulator, CompareMethod compareMethod, double compareValue, string usingValue)
			{
				this.dataManipulator = dataManipulator;
				this.compareMethod = compareMethod;
				this.usingValue = usingValue;
				this.compareValue = compareValue;
			}

			public bool FilterDataPoint(DataPoint point, Series series, int pointIndex)
			{
				bool result = false;
				switch (this.compareMethod)
				{
				case CompareMethod.Equal:
					result = (point.GetValueByName(this.usingValue) == this.compareValue);
					break;
				case CompareMethod.Less:
					result = (point.GetValueByName(this.usingValue) < this.compareValue);
					break;
				case CompareMethod.LessOrEqual:
					result = (point.GetValueByName(this.usingValue) <= this.compareValue);
					break;
				case CompareMethod.More:
					result = (point.GetValueByName(this.usingValue) > this.compareValue);
					break;
				case CompareMethod.MoreOrEqual:
					result = (point.GetValueByName(this.usingValue) >= this.compareValue);
					break;
				case CompareMethod.NotEqual:
					result = (point.GetValueByName(this.usingValue) != this.compareValue);
					break;
				}
				return result;
			}
		}

		private class GroupingFunctionInfo
		{
			internal GroupingFunction function;

			internal int outputIndex;

			internal GroupingFunctionInfo()
			{
			}
		}

		private bool filterSetEmptyPoints;

		private bool filterMatchedPoints = true;

		public bool FilterSetEmptyPoints
		{
			get
			{
				return this.filterSetEmptyPoints;
			}
			set
			{
				this.filterSetEmptyPoints = value;
			}
		}

		public bool FilterMatchedPoints
		{
			get
			{
				return this.filterMatchedPoints;
			}
			set
			{
				this.filterMatchedPoints = value;
			}
		}

		internal Series[] ConvertToSeriesArray(object obj, bool createNew)
		{
			Series[] array = null;
			if (obj == null)
			{
				return null;
			}
			if (obj.GetType() == typeof(Series))
			{
				array = new Series[1]
				{
					(Series)obj
				};
			}
			else if (obj.GetType() == typeof(string))
			{
				string text = (string)obj;
				int num = 0;
				if (text == "*")
				{
					array = new Series[base.Common.DataManager.Series.Count];
					{
						foreach (Series item in base.Common.DataManager.Series)
						{
							Series series = array[num] = item;
							num++;
						}
						return array;
					}
				}
				if (text.Length > 0)
				{
					text = text.Replace("\\,", "\\x45");
					text = text.Replace("\\=", "\\x46");
					string[] array2 = text.Split(',');
					array = new Series[array2.Length];
					string[] array3 = array2;
					foreach (string text2 in array3)
					{
						string text3 = text2.Replace("\\x45", ",");
						text3 = text3.Replace("\\x46", "=");
						try
						{
							array[num] = base.Common.DataManager.Series[text3.Trim()];
						}
						catch (Exception)
						{
							if (createNew)
							{
								array[num] = base.Common.DataManager.Series.Add(text3.Trim());
								goto end_IL_016f;
							}
							throw;
							end_IL_016f:;
						}
						num++;
					}
				}
			}
			return array;
		}

		private void Sort(PointsSortOrder order, string sortBy, Series[] series)
		{
			if (series != null && series.Length != 0)
			{
				DataPointComparer comparer = new DataPointComparer(series[0], order, sortBy);
				this.Sort(comparer, series);
			}
		}

		private void Sort(IComparer comparer, Series[] series)
		{
			if (series != null && series.Length != 0)
			{
				if (series.Length > 1)
				{
					base.CheckXValuesAlignment(series);
					int num = 0;
					foreach (DataPoint point in series[0].Points)
					{
						((DataPointAttributes)point)["_Index"] = num.ToString(CultureInfo.InvariantCulture);
						num++;
					}
				}
				series[0].Sort(comparer);
				if (series.Length > 1)
				{
					int num2 = 0;
					int num3 = 0;
					foreach (DataPoint point2 in series[0].Points)
					{
						num3 = int.Parse(((DataPointAttributes)point2)["_Index"], CultureInfo.InvariantCulture);
						for (int i = 1; i < series.Length; i++)
						{
							series[i].Points.Insert(num2, series[i].Points[num2 + num3]);
						}
						num2++;
					}
					for (int j = 1; j < series.Length; j++)
					{
						while (series[j].Points.Count > series[0].Points.Count)
						{
							series[j].Points.RemoveAt(series[j].Points.Count - 1);
						}
					}
					foreach (DataPoint point3 in series[0].Points)
					{
						point3.DeleteAttribute("_Index");
					}
				}
			}
		}

		public void Sort(PointsSortOrder order, string sortBy, string seriesName)
		{
			this.Sort(order, sortBy, this.ConvertToSeriesArray(seriesName, false));
		}

		public void Sort(PointsSortOrder order, Series series)
		{
			this.Sort(order, "Y", this.ConvertToSeriesArray(series, false));
		}

		public void Sort(PointsSortOrder order, string seriesName)
		{
			this.Sort(order, "Y", this.ConvertToSeriesArray(seriesName, false));
		}

		public void Sort(PointsSortOrder order, string sortBy, Series series)
		{
			this.Sort(order, sortBy, this.ConvertToSeriesArray(series, false));
		}

		internal void Sort(IComparer comparer, Series series)
		{
			this.Sort(comparer, this.ConvertToSeriesArray(series, false));
		}

		internal void Sort(IComparer comparer, string seriesName)
		{
			this.Sort(comparer, this.ConvertToSeriesArray(seriesName, false));
		}

		private void InsertEmptyPoints(double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, double fromXValue, double toXValue, Series[] series)
		{
			double num = Math.Min(fromXValue, toXValue);
			double num2 = Math.Max(fromXValue, toXValue);
			bool flag = double.IsNaN(num);
			bool flag2 = double.IsNaN(num2);
			foreach (Series series2 in series)
			{
				if (series2.Points.Count >= 1)
				{
					if (flag2)
					{
						num2 = ((!double.IsNaN(num2)) ? Math.Max(num2, series2.Points[series2.Points.Count - 1].XValue) : series2.Points[series2.Points.Count - 1].XValue);
					}
					if (flag)
					{
						num = ((!double.IsNaN(num)) ? Math.Min(num, series2.Points[0].XValue) : series2.Points[0].XValue);
					}
					if (num > num2)
					{
						double num3 = num;
						num = num2;
						num2 = num3;
					}
				}
			}
			double num4 = num;
			num = base.AlignIntervalStart(num, interval, this.ConvertIntervalType(intervalType));
			if (intervalOffset != 0.0)
			{
				num += base.GetIntervalSize(num, intervalOffset, this.ConvertIntervalType(intervalOffsetType));
			}
			foreach (Series series3 in series)
			{
				int num5 = 0;
				int num6 = 0;
				double num7 = num;
				while (num7 <= num2)
				{
					bool flag3 = false;
					if (double.IsNaN(fromXValue) && num7 < num4)
					{
						goto IL_0164;
					}
					if (!double.IsNaN(fromXValue) && num7 < fromXValue)
					{
						goto IL_0164;
					}
					if (num7 > toXValue)
					{
						flag3 = true;
					}
					goto IL_0172;
					IL_0172:
					if (!flag3)
					{
						int num8 = num6;
						int num9 = num6;
						while (num9 < series3.Points.Count)
						{
							if (series3.Points[num9].XValue != num7)
							{
								if (series3.Points[num9].XValue > num7)
								{
									num8 = num9;
									break;
								}
								if (num9 == series3.Points.Count - 1)
								{
									num8 = series3.Points.Count;
								}
								num9++;
								continue;
							}
							num8 = -1;
							break;
						}
						if (num8 != -1)
						{
							num6 = num8;
							num5++;
							DataPoint dataPoint = new DataPoint(series3);
							dataPoint.XValue = num7;
							dataPoint.Empty = true;
							series3.Points.Insert(num8, dataPoint);
						}
					}
					num7 += base.GetIntervalSize(num7, interval, this.ConvertIntervalType(intervalType));
					if (num5 > 1000)
					{
						num7 = num2 + 1.0;
					}
					continue;
					IL_0164:
					flag3 = true;
					goto IL_0172;
				}
			}
		}

		private DateTimeIntervalType ConvertIntervalType(IntervalType type)
		{
			switch (type)
			{
			case IntervalType.Milliseconds:
				return DateTimeIntervalType.Milliseconds;
			case IntervalType.Seconds:
				return DateTimeIntervalType.Seconds;
			case IntervalType.Days:
				return DateTimeIntervalType.Days;
			case IntervalType.Hours:
				return DateTimeIntervalType.Hours;
			case IntervalType.Minutes:
				return DateTimeIntervalType.Minutes;
			case IntervalType.Months:
				return DateTimeIntervalType.Months;
			case IntervalType.Number:
				return DateTimeIntervalType.Number;
			case IntervalType.Weeks:
				return DateTimeIntervalType.Weeks;
			case IntervalType.Years:
				return DateTimeIntervalType.Years;
			default:
				return DateTimeIntervalType.Auto;
			}
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, Series series)
		{
			this.InsertEmptyPoints(interval, intervalType, 0.0, IntervalType.Number, series);
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, string seriesName)
		{
			this.InsertEmptyPoints(interval, intervalType, 0.0, IntervalType.Number, seriesName);
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, string seriesName)
		{
			this.InsertEmptyPoints(interval, intervalType, intervalOffset, intervalOffsetType, double.NaN, double.NaN, seriesName);
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, Series series)
		{
			this.InsertEmptyPoints(interval, intervalType, intervalOffset, intervalOffsetType, double.NaN, double.NaN, series);
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, double fromXValue, double toXValue, string seriesName)
		{
			this.InsertEmptyPoints(interval, intervalType, intervalOffset, intervalOffsetType, fromXValue, toXValue, this.ConvertToSeriesArray(seriesName, false));
		}

		public void InsertEmptyPoints(double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, double fromXValue, double toXValue, Series series)
		{
			this.InsertEmptyPoints(interval, intervalType, intervalOffset, intervalOffsetType, fromXValue, toXValue, this.ConvertToSeriesArray(series, false));
		}

		internal DataSet ExportSeriesValues(Series[] series)
		{
			DataSet dataSet = new DataSet();
			dataSet.Locale = CultureInfo.CurrentCulture;
			if (series != null)
			{
				foreach (Series series2 in series)
				{
					bool flag = true;
					foreach (DataPoint point in series2.Points)
					{
						if (point.XValue != 0.0)
						{
							flag = false;
							break;
						}
					}
					if (flag && series2.XValueType == ChartValueTypes.String)
					{
						flag = false;
					}
					DataTable dataTable = new DataTable(series2.Name);
					dataTable.Locale = CultureInfo.CurrentCulture;
					Type typeFromHandle = typeof(double);
					if (series2.IsXValueDateTime())
					{
						typeFromHandle = typeof(DateTime);
					}
					else if (series2.XValueType == ChartValueTypes.String)
					{
						typeFromHandle = typeof(string);
					}
					dataTable.Columns.Add("X", typeFromHandle);
					typeFromHandle = typeof(double);
					if (series2.IsYValueDateTime())
					{
						typeFromHandle = typeof(DateTime);
					}
					else if (series2.YValueType == ChartValueTypes.String)
					{
						typeFromHandle = typeof(string);
					}
					for (int j = 0; j < series2.YValuesPerPoint; j++)
					{
						if (j == 0)
						{
							dataTable.Columns.Add("Y", typeFromHandle);
						}
						else
						{
							dataTable.Columns.Add("Y" + (j + 1).ToString(CultureInfo.InvariantCulture), typeFromHandle);
						}
					}
					double num = 1.0;
					foreach (DataPoint point2 in series2.Points)
					{
						if (!point2.Empty || !base.IgnoreEmptyPoints)
						{
							DataRow dataRow = dataTable.NewRow();
							object obj = point2.XValue;
							if (series2.IsXValueDateTime())
							{
								obj = DateTime.FromOADate(point2.XValue);
							}
							else if (series2.XValueType == ChartValueTypes.String)
							{
								obj = point2.AxisLabel;
							}
							dataRow["X"] = (flag ? ((object)num) : obj);
							for (int k = 0; k < series2.YValuesPerPoint; k++)
							{
								object value = point2.YValues[k];
								if (!point2.Empty)
								{
									if (series2.IsYValueDateTime())
									{
										value = DateTime.FromOADate(point2.YValues[k]);
									}
									else if (series2.YValueType == ChartValueTypes.String)
									{
										value = point2.AxisLabel;
									}
								}
								else if (!base.IgnoreEmptyPoints)
								{
									value = DBNull.Value;
								}
								if (k == 0)
								{
									dataRow["Y"] = value;
								}
								else
								{
									dataRow["Y" + (k + 1).ToString(CultureInfo.InvariantCulture)] = value;
								}
							}
							dataTable.Rows.Add(dataRow);
							num += 1.0;
						}
					}
					dataTable.AcceptChanges();
					dataSet.Tables.Add(dataTable);
				}
			}
			return dataSet;
		}

		public DataSet ExportSeriesValues()
		{
			return this.ExportSeriesValues("*");
		}

		public DataSet ExportSeriesValues(string seriesNames)
		{
			return this.ExportSeriesValues(this.ConvertToSeriesArray(seriesNames, false));
		}

		public DataSet ExportSeriesValues(Series series)
		{
			return this.ExportSeriesValues(this.ConvertToSeriesArray(series, false));
		}

		private void FilterTopN(int pointCount, Series[] inputSeries, Series[] outputSeries, string usingValue, bool getTopValues)
		{
			this.CheckSeriesArrays(inputSeries, outputSeries);
			base.CheckXValuesAlignment(inputSeries);
			if (pointCount <= 0)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorPointCountIsZero, "pointCount");
			}
			Series[] array = new Series[inputSeries.Length];
			for (int i = 0; i < inputSeries.Length; i++)
			{
				array[i] = inputSeries[i];
				if (outputSeries != null && outputSeries.Length > i)
				{
					array[i] = outputSeries[i];
				}
				if (array[i] != inputSeries[i])
				{
					array[i].Points.Clear();
					array[i].YValuesPerPoint = inputSeries[i].YValuesPerPoint;
					if (array[i].XValueType == ChartValueTypes.Auto || array[i].autoXValueType)
					{
						array[i].XValueType = inputSeries[i].XValueType;
						array[i].autoXValueType = true;
					}
					if (array[i].YValueType == ChartValueTypes.Auto || array[i].autoYValueType)
					{
						array[i].YValueType = inputSeries[i].YValueType;
						array[i].autoYValueType = true;
					}
					foreach (DataPoint point in inputSeries[i].Points)
					{
						array[i].Points.Add(point.Clone());
					}
				}
			}
			if (inputSeries[0].Points.Count != 0)
			{
				this.Sort((PointsSortOrder)(getTopValues ? 1 : 0), usingValue, array);
				for (int j = 0; j < inputSeries.Length; j++)
				{
					while (array[j].Points.Count > pointCount)
					{
						if (this.FilterSetEmptyPoints)
						{
							array[j].Points[pointCount].Empty = true;
							pointCount++;
						}
						else
						{
							array[j].Points.RemoveAt(pointCount);
						}
					}
				}
			}
		}

		private void Filter(IDataPointFilter filterInterface, Series[] inputSeries, Series[] outputSeries)
		{
			this.CheckSeriesArrays(inputSeries, outputSeries);
			base.CheckXValuesAlignment(inputSeries);
			if (filterInterface == null)
			{
				throw new ArgumentNullException("filterInterface");
			}
			Series[] array = new Series[inputSeries.Length];
			for (int i = 0; i < inputSeries.Length; i++)
			{
				array[i] = inputSeries[i];
				if (outputSeries != null && outputSeries.Length > i)
				{
					array[i] = outputSeries[i];
				}
				if (array[i] != inputSeries[i])
				{
					array[i].Points.Clear();
					array[i].YValuesPerPoint = inputSeries[i].YValuesPerPoint;
					if (array[i].XValueType == ChartValueTypes.Auto || array[i].autoXValueType)
					{
						array[i].XValueType = inputSeries[i].XValueType;
						array[i].autoXValueType = true;
					}
					if (array[i].YValueType == ChartValueTypes.Auto || array[i].autoYValueType)
					{
						array[i].YValueType = inputSeries[i].YValueType;
						array[i].autoYValueType = true;
					}
				}
			}
			if (inputSeries[0].Points.Count != 0)
			{
				int num = 0;
				int num2 = 0;
				while (num2 < inputSeries[0].Points.Count)
				{
					bool flag = false;
					bool flag2 = filterInterface.FilterDataPoint(inputSeries[0].Points[num2], inputSeries[0], num) == this.FilterMatchedPoints;
					for (int j = 0; j < inputSeries.Length; j++)
					{
						bool flag3 = flag2;
						if (array[j] != inputSeries[j])
						{
							if (flag3 && !this.FilterSetEmptyPoints)
							{
								flag3 = false;
							}
							else
							{
								array[j].Points.Add(inputSeries[j].Points[num2].Clone());
							}
						}
						if (flag3)
						{
							if (this.FilterSetEmptyPoints)
							{
								array[j].Points[num2].Empty = true;
								for (int k = 0; k < array[j].Points[num2].YValues.Length; k++)
								{
									array[j].Points[num2].YValues[k] = 0.0;
								}
							}
							else
							{
								array[j].Points.RemoveAt(num2);
								flag = true;
							}
						}
					}
					if (flag)
					{
						num2--;
					}
					num2++;
					num++;
				}
			}
		}

		private int[] ConvertElementIndexesToArray(string rangeElements)
		{
			string[] array = rangeElements.Split(',');
			if (array.Length == 0)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorIndexUndefined, "rangeElements");
			}
			int[] array2 = new int[array.Length * 2];
			int i = 0;
			string[] array3 = array;
			for (int j = 0; j < array3.Length; i += 2, j++)
			{
				string text = array3[j];
				if (text.IndexOf('-') != -1)
				{
					string[] array4 = text.Split('-');
					if (array4.Length == 2)
					{
						try
						{
							array2[i] = int.Parse(array4[0], CultureInfo.InvariantCulture);
							array2[i + 1] = int.Parse(array4[1], CultureInfo.InvariantCulture);
							if (array2[i + 1] < array2[i])
							{
								int num = array2[i];
								array2[i] = array2[i + 1];
								array2[i + 1] = num;
							}
						}
						catch (Exception)
						{
							throw new ArgumentException(SR.ExceptionDataManipulatorIndexFormatInvalid, "rangeElements");
						}
						continue;
					}
					throw new ArgumentException(SR.ExceptionDataManipulatorIndexFormatInvalid, "rangeElements");
				}
				try
				{
					array2[i] = int.Parse(text, CultureInfo.InvariantCulture);
					array2[i + 1] = array2[i];
				}
				catch (Exception)
				{
					throw new ArgumentException(SR.ExceptionDataManipulatorIndexFormatInvalid, "rangeElements");
				}
			}
			return array2;
		}

		private bool CheckFilterElementCriteria(DateRangeType dateRange, int[] rangeElements, DataPoint point, Series series, int pointIndex)
		{
			DateTime dateTime = DateTime.FromOADate(point.XValue);
			for (int i = 0; i < rangeElements.Length; i += 2)
			{
				switch (dateRange)
				{
				case DateRangeType.Year:
					if (dateTime.Year < rangeElements[i])
					{
						break;
					}
					if (dateTime.Year > rangeElements[i + 1])
					{
						break;
					}
					return true;
				case DateRangeType.Month:
					if (dateTime.Month < rangeElements[i])
					{
						break;
					}
					if (dateTime.Month > rangeElements[i + 1])
					{
						break;
					}
					return true;
				case DateRangeType.DayOfWeek:
					if ((int)dateTime.DayOfWeek < rangeElements[i])
					{
						break;
					}
					if ((int)dateTime.DayOfWeek > rangeElements[i + 1])
					{
						break;
					}
					return true;
				case DateRangeType.DayOfMonth:
					if (dateTime.Day < rangeElements[i])
					{
						break;
					}
					if (dateTime.Day > rangeElements[i + 1])
					{
						break;
					}
					return true;
				case DateRangeType.Hour:
					if (dateTime.Hour < rangeElements[i])
					{
						break;
					}
					if (dateTime.Hour > rangeElements[i + 1])
					{
						break;
					}
					return true;
				case DateRangeType.Minute:
					if (dateTime.Minute < rangeElements[i])
					{
						break;
					}
					if (dateTime.Minute > rangeElements[i + 1])
					{
						break;
					}
					return true;
				}
			}
			return false;
		}

		public void Filter(DateRangeType dateRange, string rangeElements, string inputSeriesNames, string outputSeriesNames)
		{
			this.Filter(new PointElementFilter(this, dateRange, rangeElements), this.ConvertToSeriesArray(inputSeriesNames, false), this.ConvertToSeriesArray(outputSeriesNames, true));
		}

		public void Filter(DateRangeType dateRange, string rangeElements, Series inputSeries)
		{
			this.Filter(dateRange, rangeElements, inputSeries, null);
		}

		public void Filter(DateRangeType dateRange, string rangeElements, Series inputSeries, Series outputSeries)
		{
			this.Filter(new PointElementFilter(this, dateRange, rangeElements), this.ConvertToSeriesArray(inputSeries, false), this.ConvertToSeriesArray(outputSeries, false));
		}

		public void Filter(DateRangeType dateRange, string rangeElements, string inputSeriesNames)
		{
			this.Filter(dateRange, rangeElements, inputSeriesNames, "");
		}

		public void Filter(CompareMethod compareMethod, double compareValue, Series inputSeries)
		{
			this.Filter(compareMethod, compareValue, inputSeries, null, "Y");
		}

		public void Filter(CompareMethod compareMethod, double compareValue, Series inputSeries, Series outputSeries)
		{
			this.Filter(new PointValueFilter(this, compareMethod, compareValue, "Y"), this.ConvertToSeriesArray(inputSeries, false), this.ConvertToSeriesArray(outputSeries, false));
		}

		public void Filter(CompareMethod compareMethod, double compareValue, Series inputSeries, Series outputSeries, string usingValue)
		{
			this.Filter(new PointValueFilter(this, compareMethod, compareValue, usingValue), this.ConvertToSeriesArray(inputSeries, false), this.ConvertToSeriesArray(outputSeries, false));
		}

		public void Filter(CompareMethod compareMethod, double compareValue, string inputSeriesNames)
		{
			this.Filter(compareMethod, compareValue, inputSeriesNames, "", "Y");
		}

		public void Filter(CompareMethod compareMethod, double compareValue, string inputSeriesNames, string outputSeriesNames)
		{
			this.Filter(new PointValueFilter(this, compareMethod, compareValue, "Y"), this.ConvertToSeriesArray(inputSeriesNames, false), this.ConvertToSeriesArray(outputSeriesNames, true));
		}

		public void Filter(CompareMethod compareMethod, double compareValue, string inputSeriesNames, string outputSeriesNames, string usingValue)
		{
			this.Filter(new PointValueFilter(this, compareMethod, compareValue, usingValue), this.ConvertToSeriesArray(inputSeriesNames, false), this.ConvertToSeriesArray(outputSeriesNames, true));
		}

		public void FilterTopN(int pointCount, string inputSeriesNames, string outputSeriesNames, string usingValue, bool getTopValues)
		{
			this.FilterTopN(pointCount, this.ConvertToSeriesArray(inputSeriesNames, false), this.ConvertToSeriesArray(outputSeriesNames, true), usingValue, getTopValues);
		}

		public void FilterTopN(int pointCount, Series inputSeries)
		{
			this.FilterTopN(pointCount, this.ConvertToSeriesArray(inputSeries, false), null, "Y", true);
		}

		public void FilterTopN(int pointCount, Series inputSeries, Series outputSeries)
		{
			this.FilterTopN(pointCount, this.ConvertToSeriesArray(inputSeries, false), this.ConvertToSeriesArray(outputSeries, false), "Y", true);
		}

		public void FilterTopN(int pointCount, Series inputSeries, Series outputSeries, string usingValue)
		{
			this.FilterTopN(pointCount, this.ConvertToSeriesArray(inputSeries, false), this.ConvertToSeriesArray(outputSeries, false), usingValue, true);
		}

		public void FilterTopN(int pointCount, Series inputSeries, Series outputSeries, string usingValue, bool getTopValues)
		{
			this.FilterTopN(pointCount, this.ConvertToSeriesArray(inputSeries, false), this.ConvertToSeriesArray(outputSeries, false), usingValue, getTopValues);
		}

		public void FilterTopN(int pointCount, string inputSeriesNames)
		{
			this.FilterTopN(pointCount, this.ConvertToSeriesArray(inputSeriesNames, false), null, "Y", true);
		}

		public void FilterTopN(int pointCount, string inputSeriesNames, string outputSeriesNames)
		{
			this.FilterTopN(pointCount, this.ConvertToSeriesArray(inputSeriesNames, false), this.ConvertToSeriesArray(outputSeriesNames, true), "Y", true);
		}

		public void FilterTopN(int pointCount, string inputSeriesNames, string outputSeriesNames, string usingValue)
		{
			this.FilterTopN(pointCount, this.ConvertToSeriesArray(inputSeriesNames, false), this.ConvertToSeriesArray(outputSeriesNames, true), usingValue, true);
		}

		internal void Filter(IDataPointFilter filterInterface, Series inputSeries)
		{
			this.Filter(filterInterface, this.ConvertToSeriesArray(inputSeries, false), null);
		}

		internal void Filter(IDataPointFilter filterInterface, Series inputSeries, Series outputSeries)
		{
			this.Filter(filterInterface, this.ConvertToSeriesArray(inputSeries, false), this.ConvertToSeriesArray(outputSeries, false));
		}

		internal void Filter(IDataPointFilter filterInterface, string inputSeriesNames)
		{
			this.Filter(filterInterface, this.ConvertToSeriesArray(inputSeriesNames, false), null);
		}

		internal void Filter(IDataPointFilter filterInterface, string inputSeriesNames, string outputSeriesNames)
		{
			this.Filter(filterInterface, this.ConvertToSeriesArray(inputSeriesNames, false), this.ConvertToSeriesArray(outputSeriesNames, true));
		}

		private void GroupByAxisLabel(string formula, Series[] inputSeries, Series[] outputSeries)
		{
			this.CheckSeriesArrays(inputSeries, outputSeries);
			int num = 1;
			GroupingFunctionInfo[] groupingFunctions = this.GetGroupingFunctions(inputSeries, formula, out num);
			for (int i = 0; i < inputSeries.Length; i++)
			{
				Series series = inputSeries[i];
				Series series2 = series;
				if (outputSeries != null && i < outputSeries.Length)
				{
					series2 = outputSeries[i];
					if (series2.Name != series.Name)
					{
						series2.Points.Clear();
						if (series2.XValueType == ChartValueTypes.Auto || series2.autoXValueType)
						{
							series2.XValueType = series.XValueType;
							series2.autoXValueType = true;
						}
						if (series2.YValueType == ChartValueTypes.Auto || series2.autoYValueType)
						{
							series2.YValueType = series.YValueType;
							series2.autoYValueType = true;
						}
					}
				}
				if (series != series2)
				{
					Series series3 = new Series("Temp", series.YValuesPerPoint);
					foreach (DataPoint point in series.Points)
					{
						DataPoint dataPoint2 = new DataPoint(series3);
						dataPoint2.AxisLabel = point.AxisLabel;
						dataPoint2.XValue = point.XValue;
						point.YValues.CopyTo(dataPoint2.YValues, 0);
						dataPoint2.Empty = point.Empty;
						series3.Points.Add(dataPoint2);
					}
					series = series3;
				}
				if (series.Points.Count != 0)
				{
					series2.YValuesPerPoint = num - 1;
					series.Sort(PointsSortOrder.Ascending, "AxisLabel");
					int num2 = 0;
					int num3 = 0;
					double[] array = new double[num];
					string text = null;
					bool flag = false;
					int num4 = 0;
					for (int j = 0; j <= series.Points.Count; j++)
					{
						if (flag)
						{
							break;
						}
						bool flag2 = false;
						if (j == series.Points.Count)
						{
							flag = true;
							num3 = j - 1;
							j = num3;
							flag2 = true;
						}
						if (!flag2 && text == null)
						{
							text = series.Points[j].AxisLabel;
						}
						if (!flag2 && series.Points[j].AxisLabel != text)
						{
							num3 = j - 1;
							flag2 = true;
						}
						if (flag2)
						{
							this.ProcessPointValues(groupingFunctions, array, inputSeries[i], series.Points[j], j, num2, num3, true, ref num4);
							if (groupingFunctions[0].function == GroupingFunction.Center)
							{
								array[0] = (inputSeries[i].Points[num2].XValue + inputSeries[i].Points[num3].XValue) / 2.0;
							}
							else if (groupingFunctions[0].function == GroupingFunction.First)
							{
								array[0] = inputSeries[i].Points[num2].XValue;
							}
							if (groupingFunctions[0].function == GroupingFunction.Last)
							{
								array[0] = inputSeries[i].Points[num3].XValue;
							}
							DataPoint dataPoint3 = new DataPoint();
							dataPoint3.ResizeYValueArray(num - 1);
							dataPoint3.XValue = array[0];
							dataPoint3.AxisLabel = text;
							for (int k = 1; k < array.Length; k++)
							{
								dataPoint3.YValues[k - 1] = array[k];
							}
							int num5 = series2.Points.Count;
							if (series2 == series)
							{
								num5 = num2;
								j = num5 + 1;
								for (int l = num2; l <= num3; l++)
								{
									series2.Points.RemoveAt(num2);
								}
							}
							series2.Points.Insert(num5, dataPoint3);
							num2 = j;
							num3 = j;
							num4 = 0;
							text = null;
							j--;
						}
						else
						{
							this.ProcessPointValues(groupingFunctions, array, inputSeries[i], series.Points[j], j, num2, num3, false, ref num4);
						}
					}
				}
			}
		}

		private void Group(string formula, double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, Series[] inputSeries, Series[] outputSeries)
		{
			this.CheckSeriesArrays(inputSeries, outputSeries);
			int num = 1;
			GroupingFunctionInfo[] groupingFunctions = this.GetGroupingFunctions(inputSeries, formula, out num);
			for (int i = 0; i < inputSeries.Length; i++)
			{
				Series series = inputSeries[i];
				Series series2 = series;
				if (outputSeries != null && i < outputSeries.Length)
				{
					series2 = outputSeries[i];
					if (series2.Name != series.Name)
					{
						series2.Points.Clear();
						if (series2.XValueType == ChartValueTypes.Auto || series2.autoXValueType)
						{
							series2.XValueType = series.XValueType;
							series2.autoXValueType = true;
						}
						if (series2.YValueType == ChartValueTypes.Auto || series2.autoYValueType)
						{
							series2.YValueType = series.YValueType;
							series2.autoYValueType = true;
						}
					}
				}
				if (series.Points.Count != 0)
				{
					series2.YValuesPerPoint = num - 1;
					int num2 = 0;
					int num3 = 0;
					double num4 = 0.0;
					double num5 = 0.0;
					num4 = series.Points[0].XValue;
					num4 = base.AlignIntervalStart(num4, interval, this.ConvertIntervalType(intervalType));
					double num6 = 0.0;
					if (intervalOffset != 0.0)
					{
						num6 = num4 + base.GetIntervalSize(num4, intervalOffset, this.ConvertIntervalType(intervalOffsetType));
						if (series.Points[0].XValue < num6)
						{
							num4 = ((intervalType != 0) ? (num6 - base.GetIntervalSize(num6, interval, this.ConvertIntervalType(intervalType))) : (num6 + base.GetIntervalSize(num6, 0.0 - interval, this.ConvertIntervalType(intervalType))));
							num5 = num6;
						}
						else
						{
							num4 = num6;
							num5 = num4 + base.GetIntervalSize(num4, interval, this.ConvertIntervalType(intervalType));
						}
					}
					else
					{
						num5 = num4 + base.GetIntervalSize(num4, interval, this.ConvertIntervalType(intervalType));
					}
					double[] array = new double[num];
					bool flag = false;
					int num7 = 0;
					int num8 = 0;
					for (int j = 0; j <= series.Points.Count && !flag; j++)
					{
						bool flag2 = false;
						if (j > 0 && j < series.Points.Count && series.Points[j].XValue < series.Points[j - 1].XValue)
						{
							throw new InvalidOperationException(SR.ExceptionDataManipulatorGroupedSeriesNotSorted);
						}
						if (j == series.Points.Count)
						{
							flag = true;
							num3 = j - 1;
							j = num3;
							flag2 = true;
						}
						if (!flag2 && series.Points[j].XValue >= num5)
						{
							if (j == 0)
							{
								continue;
							}
							num3 = j - 1;
							flag2 = true;
						}
						if (flag2)
						{
							if (num8 > num7)
							{
								this.ProcessPointValues(groupingFunctions, array, inputSeries[i], series.Points[j], j, num2, num3, true, ref num7);
								if (groupingFunctions[0].function == GroupingFunction.Center)
								{
									array[0] = (num4 + num5) / 2.0;
								}
								else if (groupingFunctions[0].function == GroupingFunction.First)
								{
									array[0] = num4;
								}
								if (groupingFunctions[0].function == GroupingFunction.Last)
								{
									array[0] = num5;
								}
								DataPoint dataPoint = new DataPoint();
								dataPoint.ResizeYValueArray(num - 1);
								dataPoint.XValue = array[0];
								for (int k = 1; k < array.Length; k++)
								{
									dataPoint.YValues[k - 1] = array[k];
								}
								int num9 = series2.Points.Count;
								if (series2 == series)
								{
									num9 = num2;
									j = num9 + 1;
									for (int l = num2; l <= num3; l++)
									{
										series2.Points.RemoveAt(num2);
									}
								}
								series2.Points.Insert(num9, dataPoint);
							}
							num4 = num5;
							num5 = num4 + base.GetIntervalSize(num4, interval, this.ConvertIntervalType(intervalType));
							num2 = j;
							num3 = j;
							num8 = 0;
							num7 = 0;
							j--;
						}
						else
						{
							this.ProcessPointValues(groupingFunctions, array, inputSeries[i], series.Points[j], j, num2, num3, false, ref num7);
							num8++;
						}
					}
				}
			}
		}

		private void ProcessPointValues(GroupingFunctionInfo[] functions, double[] pointTempValues, Series series, DataPoint point, int pointIndex, int intervalFirstIndex, int intervalLastIndex, bool finalPass, ref int numberOfEmptyPoints)
		{
			if (pointIndex == intervalFirstIndex && !finalPass)
			{
				int num = 0;
				foreach (GroupingFunctionInfo groupingFunctionInfo in functions)
				{
					if (num > point.YValues.Length)
					{
						break;
					}
					pointTempValues[groupingFunctionInfo.outputIndex] = 0.0;
					if (groupingFunctionInfo.function == GroupingFunction.Min)
					{
						pointTempValues[groupingFunctionInfo.outputIndex] = 1.7976931348623157E+308;
					}
					else if (groupingFunctionInfo.function == GroupingFunction.Max)
					{
						pointTempValues[groupingFunctionInfo.outputIndex] = -1.7976931348623157E+308;
					}
					else if (groupingFunctionInfo.function == GroupingFunction.First)
					{
						if (num == 0)
						{
							pointTempValues[0] = point.XValue;
						}
						else
						{
							pointTempValues[groupingFunctionInfo.outputIndex] = point.YValues[num - 1];
						}
					}
					else if (groupingFunctionInfo.function == GroupingFunction.HiLo || groupingFunctionInfo.function == GroupingFunction.HiLoOpCl)
					{
						pointTempValues[groupingFunctionInfo.outputIndex] = -1.7976931348623157E+308;
						pointTempValues[groupingFunctionInfo.outputIndex + 1] = 1.7976931348623157E+308;
						if (groupingFunctionInfo.function == GroupingFunction.HiLoOpCl)
						{
							pointTempValues[groupingFunctionInfo.outputIndex + 2] = point.YValues[num - 1];
							pointTempValues[groupingFunctionInfo.outputIndex + 3] = 0.0;
						}
					}
					num++;
				}
			}
			if (!finalPass)
			{
				if (point.Empty && base.IgnoreEmptyPoints)
				{
					numberOfEmptyPoints++;
					return;
				}
				int num2 = 0;
				foreach (GroupingFunctionInfo groupingFunctionInfo2 in functions)
				{
					if (num2 > point.YValues.Length)
					{
						break;
					}
					if (groupingFunctionInfo2.function == GroupingFunction.Min && !point.Empty && base.IgnoreEmptyPoints)
					{
						pointTempValues[groupingFunctionInfo2.outputIndex] = Math.Min(pointTempValues[groupingFunctionInfo2.outputIndex], point.YValues[num2 - 1]);
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.Max)
					{
						pointTempValues[groupingFunctionInfo2.outputIndex] = Math.Max(pointTempValues[groupingFunctionInfo2.outputIndex], point.YValues[num2 - 1]);
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.Ave || groupingFunctionInfo2.function == GroupingFunction.Sum)
					{
						if (num2 == 0)
						{
							pointTempValues[0] += point.XValue;
						}
						else
						{
							pointTempValues[groupingFunctionInfo2.outputIndex] += point.YValues[num2 - 1];
						}
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.Variance || groupingFunctionInfo2.function == GroupingFunction.Deviation)
					{
						pointTempValues[groupingFunctionInfo2.outputIndex] += point.YValues[num2 - 1];
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.Last)
					{
						if (num2 == 0)
						{
							pointTempValues[0] = point.XValue;
						}
						else
						{
							pointTempValues[groupingFunctionInfo2.outputIndex] = point.YValues[num2 - 1];
						}
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.Count)
					{
						pointTempValues[groupingFunctionInfo2.outputIndex] += 1.0;
					}
					else if (groupingFunctionInfo2.function == GroupingFunction.HiLo || groupingFunctionInfo2.function == GroupingFunction.HiLoOpCl)
					{
						pointTempValues[groupingFunctionInfo2.outputIndex] = Math.Max(pointTempValues[groupingFunctionInfo2.outputIndex], point.YValues[num2 - 1]);
						pointTempValues[groupingFunctionInfo2.outputIndex + 1] = Math.Min(pointTempValues[groupingFunctionInfo2.outputIndex + 1], point.YValues[num2 - 1]);
						if (groupingFunctionInfo2.function == GroupingFunction.HiLoOpCl)
						{
							pointTempValues[groupingFunctionInfo2.outputIndex + 3] = point.YValues[num2 - 1];
						}
					}
					num2++;
				}
			}
			if (finalPass)
			{
				int num3 = 0;
				foreach (GroupingFunctionInfo groupingFunctionInfo3 in functions)
				{
					if (num3 > point.YValues.Length)
					{
						break;
					}
					if (groupingFunctionInfo3.function == GroupingFunction.Ave)
					{
						pointTempValues[groupingFunctionInfo3.outputIndex] /= (double)(intervalLastIndex - intervalFirstIndex - numberOfEmptyPoints + 1);
					}
					if (groupingFunctionInfo3.function == GroupingFunction.DistinctCount)
					{
						pointTempValues[groupingFunctionInfo3.outputIndex] = 0.0;
						ArrayList arrayList = new ArrayList(intervalLastIndex - intervalFirstIndex + 1);
						for (int l = intervalFirstIndex; l <= intervalLastIndex; l++)
						{
							if ((!series.Points[l].Empty || !base.IgnoreEmptyPoints) && !arrayList.Contains(series.Points[l].YValues[num3 - 1]))
							{
								arrayList.Add(series.Points[l].YValues[num3 - 1]);
							}
						}
						pointTempValues[groupingFunctionInfo3.outputIndex] = (double)arrayList.Count;
					}
					else if (groupingFunctionInfo3.function == GroupingFunction.Variance || groupingFunctionInfo3.function == GroupingFunction.Deviation)
					{
						double num4 = pointTempValues[groupingFunctionInfo3.outputIndex] / (double)(intervalLastIndex - intervalFirstIndex - numberOfEmptyPoints + 1);
						pointTempValues[groupingFunctionInfo3.outputIndex] = 0.0;
						for (int m = intervalFirstIndex; m <= intervalLastIndex; m++)
						{
							if (!series.Points[m].Empty || !base.IgnoreEmptyPoints)
							{
								pointTempValues[groupingFunctionInfo3.outputIndex] += Math.Pow(series.Points[m].YValues[num3 - 1] - num4, 2.0);
							}
						}
						pointTempValues[groupingFunctionInfo3.outputIndex] /= (double)(intervalLastIndex - intervalFirstIndex - numberOfEmptyPoints + 1);
						if (groupingFunctionInfo3.function == GroupingFunction.Deviation)
						{
							pointTempValues[groupingFunctionInfo3.outputIndex] = Math.Sqrt(pointTempValues[groupingFunctionInfo3.outputIndex]);
						}
					}
					num3++;
				}
			}
		}

		private GroupingFunctionInfo[] GetGroupingFunctions(Series[] inputSeries, string formula, out int outputValuesNumber)
		{
			int num = 0;
			foreach (Series series in inputSeries)
			{
				num = Math.Max(num, series.YValuesPerPoint);
			}
			GroupingFunctionInfo[] array = new GroupingFunctionInfo[num + 1];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = new GroupingFunctionInfo();
			}
			string[] array2 = formula.Split(',');
			if (array2.Length == 0)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaUndefined);
			}
			GroupingFunctionInfo groupingFunctionInfo = new GroupingFunctionInfo();
			string[] array3 = array2;
			foreach (string text in array3)
			{
				string text2 = text.Trim();
				text2 = text2.ToUpper(CultureInfo.InvariantCulture);
				int num2 = 1;
				GroupingFunction function = this.ParseFormulaAndValueType(text2, out num2);
				if (groupingFunctionInfo.function == GroupingFunction.None)
				{
					groupingFunctionInfo.function = function;
				}
				if (num2 >= array.Length)
				{
					throw new ArgumentException(SR.ExceptionDataManipulatorYValuesIndexExceeded(text2));
				}
				if (array[num2].function != 0)
				{
					throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaAlreadyDefined(text2));
				}
				array[num2].function = function;
			}
			if (array[0].function == GroupingFunction.None)
			{
				array[0].function = GroupingFunction.First;
			}
			for (int l = 1; l < array.Length; l++)
			{
				if (array[l].function == GroupingFunction.None)
				{
					array[l].function = groupingFunctionInfo.function;
				}
			}
			outputValuesNumber = 0;
			for (int m = 0; m < array.Length; m++)
			{
				array[m].outputIndex = outputValuesNumber;
				if (array[m].function == GroupingFunction.HiLoOpCl)
				{
					outputValuesNumber += 3;
				}
				else if (array[m].function == GroupingFunction.HiLo)
				{
					outputValuesNumber++;
				}
				outputValuesNumber++;
			}
			if (array[0].function != GroupingFunction.First && array[0].function != GroupingFunction.Last && array[0].function != GroupingFunction.Center)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaUnsupported);
			}
			return array;
		}

		private GroupingFunction ParseFormulaAndValueType(string formulaString, out int valueIndex)
		{
			valueIndex = 1;
			string[] array = formulaString.Split(':');
			if (array.Length < 1 && array.Length > 2)
			{
				throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaFormatInvalid(formulaString));
			}
			if (array.Length == 2)
			{
				if (!(array[0] == "X"))
				{
					if (array[0].StartsWith("Y", StringComparison.Ordinal))
					{
						array[0] = array[0].TrimStart('Y');
						if (array[0].Length == 0)
						{
							valueIndex = 1;
						}
						else
						{
							try
							{
								valueIndex = int.Parse(array[0], CultureInfo.InvariantCulture);
							}
							catch (Exception)
							{
								throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaFormatInvalid(formulaString));
							}
						}
						goto IL_00aa;
					}
					throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaFormatInvalid(formulaString));
				}
				valueIndex = 0;
			}
			goto IL_00aa;
			IL_00aa:
			if (array[array.Length - 1] == "MIN")
			{
				return GroupingFunction.Min;
			}
			if (array[array.Length - 1] == "MAX")
			{
				return GroupingFunction.Max;
			}
			if (array[array.Length - 1] == "AVE")
			{
				return GroupingFunction.Ave;
			}
			if (array[array.Length - 1] == "SUM")
			{
				return GroupingFunction.Sum;
			}
			if (array[array.Length - 1] == "FIRST")
			{
				return GroupingFunction.First;
			}
			if (array[array.Length - 1] == "LAST")
			{
				return GroupingFunction.Last;
			}
			if (array[array.Length - 1] == "HILOOPCL")
			{
				return GroupingFunction.HiLoOpCl;
			}
			if (array[array.Length - 1] == "HILO")
			{
				return GroupingFunction.HiLo;
			}
			if (array[array.Length - 1] == "COUNT")
			{
				return GroupingFunction.Count;
			}
			if (array[array.Length - 1] == "DISTINCTCOUNT")
			{
				return GroupingFunction.DistinctCount;
			}
			if (array[array.Length - 1] == "VARIANCE")
			{
				return GroupingFunction.Variance;
			}
			if (array[array.Length - 1] == "DEVIATION")
			{
				return GroupingFunction.Deviation;
			}
			if (array[array.Length - 1] == "CENTER")
			{
				return GroupingFunction.Center;
			}
			throw new ArgumentException(SR.ExceptionDataManipulatorGroupingFormulaNameInvalid(formulaString));
		}

		private void CheckSeriesArrays(Series[] inputSeries, Series[] outputSeries)
		{
			if (inputSeries != null && inputSeries.Length != 0)
			{
				if (outputSeries == null)
				{
					return;
				}
				if (outputSeries.Length == inputSeries.Length)
				{
					return;
				}
				throw new ArgumentException(SR.ExceptionDataManipulatorGroupingInputOutputSeriesNumberMismatch);
			}
			throw new ArgumentException(SR.ExceptionDataManipulatorGroupingInputSeriesUndefined);
		}

		public void Group(string formula, double interval, IntervalType intervalType, Series inputSeries)
		{
			this.Group(formula, interval, intervalType, inputSeries, null);
		}

		public void Group(string formula, double interval, IntervalType intervalType, string inputSeriesName)
		{
			this.Group(formula, interval, intervalType, inputSeriesName, "");
		}

		public void Group(string formula, double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, Series inputSeries)
		{
			this.Group(formula, interval, intervalType, intervalOffset, intervalOffsetType, inputSeries, null);
		}

		public void Group(string formula, double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, string inputSeriesName)
		{
			this.Group(formula, interval, intervalType, intervalOffset, intervalOffsetType, inputSeriesName, "");
		}

		public void GroupByAxisLabel(string formula, string inputSeriesName, string outputSeriesName)
		{
			this.GroupByAxisLabel(formula, this.ConvertToSeriesArray(inputSeriesName, false), this.ConvertToSeriesArray(outputSeriesName, true));
		}

		public void GroupByAxisLabel(string formula, Series inputSeries)
		{
			this.GroupByAxisLabel(formula, inputSeries, null);
		}

		public void GroupByAxisLabel(string formula, string inputSeriesName)
		{
			this.GroupByAxisLabel(formula, inputSeriesName, null);
		}

		public void Group(string formula, double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, string inputSeriesName, string outputSeriesName)
		{
			this.Group(formula, interval, intervalType, intervalOffset, intervalOffsetType, this.ConvertToSeriesArray(inputSeriesName, false), this.ConvertToSeriesArray(outputSeriesName, true));
		}

		public void Group(string formula, double interval, IntervalType intervalType, Series inputSeries, Series outputSeries)
		{
			this.Group(formula, interval, intervalType, 0.0, IntervalType.Number, inputSeries, outputSeries);
		}

		public void Group(string formula, double interval, IntervalType intervalType, string inputSeriesName, string outputSeriesName)
		{
			this.Group(formula, interval, intervalType, 0.0, IntervalType.Number, inputSeriesName, outputSeriesName);
		}

		public void Group(string formula, double interval, IntervalType intervalType, double intervalOffset, IntervalType intervalOffsetType, Series inputSeries, Series outputSeries)
		{
			this.Group(formula, interval, intervalType, intervalOffset, intervalOffsetType, this.ConvertToSeriesArray(inputSeries, false), this.ConvertToSeriesArray(outputSeries, false));
		}

		public void GroupByAxisLabel(string formula, Series inputSeries, Series outputSeries)
		{
			this.GroupByAxisLabel(formula, this.ConvertToSeriesArray(inputSeries, false), this.ConvertToSeriesArray(outputSeries, false));
		}
	}
}
