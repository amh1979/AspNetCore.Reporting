using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using System;
using System.Collections;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ChartAreaAxes : ChartElement
	{
		internal Axis axisY;

		internal Axis axisX;

		internal Axis axisX2;

		internal Axis axisY2;

		internal ArrayList series = new ArrayList();

		internal ArrayList chartTypes = new ArrayList();

		internal string name = "";

		private string intervalSeriesList = "";

		internal double intervalData = double.NaN;

		internal double intervalLogData = double.NaN;

		internal Series intervalSeries;

		internal bool intervalSameSize;

		internal bool diffIntervalAlignmentChecked;

		internal bool stacked;

		internal bool secondYScale;

		internal bool switchValueAxes;

		internal bool requireAxes = true;

		internal bool chartAreaIsCurcular;

		internal bool hundredPercent;

		internal bool hundredPercentNegative;

		internal bool IsSubAxesSupported
		{
			get
			{
				if (!((ChartArea)this).Area3DStyle.Enable3D && !((ChartArea)this).chartAreaIsCurcular)
				{
					return true;
				}
				return false;
			}
		}

		internal ArrayList Series
		{
			get
			{
				return this.series;
			}
		}

		internal ArrayList ChartTypes
		{
			get
			{
				return this.chartTypes;
			}
		}

		internal Axis GetAxis(AxisName axisName, AxisType axisType, string subAxisName)
		{
			if (((ChartArea)this).Area3DStyle.Enable3D)
			{
				subAxisName = string.Empty;
			}
			if (axisName != 0 && axisName != AxisName.X2)
			{
				if (axisType == AxisType.Primary)
				{
					return ((ChartArea)this).AxisY.GetSubAxis(subAxisName);
				}
				return ((ChartArea)this).AxisY2.GetSubAxis(subAxisName);
			}
			if (axisType == AxisType.Primary)
			{
				return ((ChartArea)this).AxisX.GetSubAxis(subAxisName);
			}
			return ((ChartArea)this).AxisX2.GetSubAxis(subAxisName);
		}

		internal void SetDefaultAxesValues()
		{
			if (this.switchValueAxes)
			{
				this.axisY.AxisPosition = AxisPosition.Bottom;
				this.axisX.AxisPosition = AxisPosition.Left;
				this.axisX2.AxisPosition = AxisPosition.Right;
				this.axisY2.AxisPosition = AxisPosition.Top;
			}
			else
			{
				this.axisY.AxisPosition = AxisPosition.Left;
				this.axisX.AxisPosition = AxisPosition.Bottom;
				this.axisX2.AxisPosition = AxisPosition.Top;
				this.axisY2.AxisPosition = AxisPosition.Right;
			}
			Axis[] axes = ((ChartArea)this).Axes;
			foreach (Axis axis in axes)
			{
				axis.oppositeAxis = null;
			}
			if (this.chartAreaIsCurcular)
			{
				this.axisX.SetAutoMaximum(360.0);
				this.axisX.SetAutoMinimum(0.0);
				this.axisX.SetInterval = Math.Abs(this.axisX.maximum - this.axisX.minimum) / 12.0;
			}
			else
			{
				this.SetDefaultFromIndexesOrData(this.axisX, AxisType.Primary);
			}
			this.SetDefaultFromIndexesOrData(this.axisX2, AxisType.Secondary);
			if (this.GetYAxesSeries(AxisType.Primary, string.Empty).Count != 0)
			{
				this.SetDefaultFromData(this.axisY);
				this.axisY.EstimateAxis();
			}
			if (this.GetYAxesSeries(AxisType.Secondary, string.Empty).Count != 0)
			{
				this.SetDefaultFromData(this.axisY2);
				this.axisY2.EstimateAxis();
			}
			this.axisX.SetAxisPosition();
			this.axisX2.SetAxisPosition();
			this.axisY.SetAxisPosition();
			this.axisY2.SetAxisPosition();
			this.EnableAxes();
			Axis[] array = new Axis[2]
			{
				this.axisY,
				this.axisY2
			};
			Axis[] array2 = array;
			foreach (Axis axis2 in array2)
			{
				axis2.ScaleBreakStyle.GetAxisSegmentForScaleBreaks(axis2.ScaleSegments);
				if (axis2.ScaleSegments.Count > 0)
				{
					axis2.scaleSegmentsUsed = true;
					if (axis2.minimum < axis2.ScaleSegments[0].ScaleMinimum)
					{
						axis2.minimum = axis2.ScaleSegments[0].ScaleMinimum;
					}
					if (axis2.minimum > axis2.ScaleSegments[axis2.ScaleSegments.Count - 1].ScaleMaximum)
					{
						axis2.minimum = axis2.ScaleSegments[axis2.ScaleSegments.Count - 1].ScaleMaximum;
					}
				}
			}
			bool flag = false;
			Axis[] array3 = new Axis[4]
			{
				this.axisX,
				this.axisX2,
				this.axisY,
				this.axisY2
			};
			Axis[] array4 = array3;
			foreach (Axis axis3 in array4)
			{
				if (axis3.ScaleSegments.Count <= 0)
				{
					axis3.FillLabels(true);
				}
				else
				{
					bool removeFirstRow = true;
					int num = 0;
					foreach (AxisScaleSegment scaleSegment in axis3.ScaleSegments)
					{
						scaleSegment.SetTempAxisScaleAndInterval();
						axis3.FillLabels(removeFirstRow);
						removeFirstRow = false;
						scaleSegment.RestoreAxisScaleAndInterval();
						if (num < axis3.ScaleSegments.Count - 1 && axis3.CustomLabels.Count > 0)
						{
							axis3.CustomLabels.RemoveAt(axis3.CustomLabels.Count - 1);
						}
						num++;
					}
				}
			}
			Axis[] array5 = array3;
			foreach (Axis axis4 in array5)
			{
				axis4.PostFillLabels();
			}
		}

		private void SetDefaultFromIndexesOrData(Axis axis, AxisType axisType)
		{
			ArrayList xAxesSeries = this.GetXAxesSeries(axisType, axis.SubAxisName);
			bool flag = true;
			foreach (string item in xAxesSeries)
			{
				Series series = base.Common.DataManager.Series[item];
				if (!ChartElement.IndexedSeries(series))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				if (axis.Logarithmic)
				{
					throw new InvalidOperationException(SR.ExceptionChartAreaAxisScaleLogarithmicUnsuitable);
				}
				this.SetDefaultFromIndexes(axis);
			}
			else
			{
				this.SetDefaultFromData(axis);
				axis.EstimateAxis();
			}
		}

		private void EnableAxes()
		{
			if (this.series != null)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				foreach (string item in this.series)
				{
					Series series = base.Common.DataManager.Series[item];
					if (series.XAxisType == AxisType.Primary)
					{
						flag = true;
						this.Activate(this.axisX, true);
					}
					else
					{
						flag3 = true;
						this.Activate(this.axisX2, true);
					}
					if (series.YAxisType == AxisType.Primary)
					{
						flag2 = true;
						this.Activate(this.axisY, true);
					}
					else
					{
						flag4 = true;
						this.Activate(this.axisY2, true);
					}
				}
				if (!flag)
				{
					this.Activate(this.axisX, false);
				}
				if (!flag2)
				{
					this.Activate(this.axisY, false);
				}
				if (!flag3)
				{
					this.Activate(this.axisX2, false);
				}
				if (!flag4)
				{
					this.Activate(this.axisY2, false);
				}
			}
		}

		private void Activate(Axis axis, bool active)
		{
			if (axis.autoEnabled)
			{
				axis.enabled = active;
			}
		}

		private bool AllEmptyPoints()
		{
			foreach (string item in this.series)
			{
				Series series = base.Common.DataManager.Series[item];
				foreach (DataPoint point in series.Points)
				{
					if (!point.EmptyX && !point.Empty)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void SetDefaultFromData(Axis axis)
		{
			if (!double.IsNaN(axis.View.Position) && !double.IsNaN(axis.View.Size) && !axis.refreshMinMaxFromData && axis.Logarithmic)
			{
				return;
			}
			double num = default(double);
			double num2 = default(double);
			this.GetValuesFromData(axis, out num, out num2);
			if (!axis.enabled || (!axis.autoMaximum && !double.IsNaN(axis.Maximum)) || (num2 != 1.7976931348623157E+308 && num2 != -1.7976931348623157E+308))
			{
				if (!axis.autoMinimum && !double.IsNaN(axis.Minimum))
				{
					goto IL_00dd;
				}
				if (num != 1.7976931348623157E+308 && num != -1.7976931348623157E+308)
				{
					goto IL_00dd;
				}
			}
			if (this.AllEmptyPoints())
			{
				num2 = 8.0;
				num = 1.0;
			}
			else if (!base.Common.ChartPicture.SuppressExceptions)
			{
				throw new InvalidOperationException(SR.ExceptionAxisMinimumMaximumInvalid);
			}
			goto IL_00dd;
			IL_00dd:
			axis.marginView = 0.0;
			if (axis.margin == 100.0 && (axis.axisType == AxisName.X || axis.axisType == AxisName.X2))
			{
				axis.marginView = this.GetPointsInterval(false, 10.0);
			}
			if (num2 == num && axis.Maximum == axis.Minimum)
			{
				axis.marginView = 1.0;
			}
			if (axis.Logarithmic)
			{
				axis.marginView = 0.0;
			}
			if (axis.autoMaximum)
			{
				if (!axis.roundedXValues && (axis.axisType == AxisName.X || axis.axisType == AxisName.X2))
				{
					axis.SetAutoMaximum(num2 + axis.marginView);
				}
				else if (axis.startFromZero && num2 < 0.0)
				{
					axis.SetAutoMaximum(0.0);
				}
				else
				{
					axis.SetAutoMaximum(num2);
				}
			}
			if (axis.autoMinimum)
			{
				if (axis.Logarithmic)
				{
					if (num < 1.0)
					{
						axis.SetAutoMinimum(num);
					}
					else if (axis.startFromZero)
					{
						axis.SetAutoMinimum(1.0);
					}
					else
					{
						axis.SetAutoMinimum(num);
					}
				}
				else if (num > 0.0)
				{
					if (!axis.roundedXValues && (axis.axisType == AxisName.X || axis.axisType == AxisName.X2))
					{
						axis.SetAutoMinimum(num - axis.marginView);
					}
					else if (axis.startFromZero && !this.SeriesDateTimeType(axis.axisType, axis.SubAxisName))
					{
						axis.SetAutoMinimum(0.0);
					}
					else
					{
						axis.SetAutoMinimum(num);
					}
				}
				else if (axis.axisType == AxisName.X || axis.axisType == AxisName.X2)
				{
					axis.SetAutoMinimum(num - axis.marginView);
				}
				else
				{
					axis.SetAutoMinimum(num);
				}
			}
			if (axis.Logarithmic && axis.logarithmicConvertedToLinear)
			{
				if (!axis.autoMinimum)
				{
					axis.minimum = axis.logarithmicMinimum;
				}
				if (!axis.autoMaximum)
				{
					axis.maximum = axis.logarithmicMaximum;
				}
				axis.logarithmicConvertedToLinear = false;
			}
			if (base.Common.ChartPicture.SuppressExceptions && axis.maximum == axis.minimum)
			{
				axis.minimum = axis.maximum;
				axis.maximum = axis.minimum + 1.0;
			}
		}

		internal bool SeriesIntegerType(AxisName axisName, string subAxisName)
		{
			foreach (string item in this.series)
			{
				Series series = base.Common.DataManager.Series[item];
				switch (axisName)
				{
				case AxisName.X:
					if (series.XAxisType != 0)
					{
						break;
					}
					if (series.XValueType != ChartValueTypes.Int && series.XValueType != ChartValueTypes.UInt && series.XValueType != ChartValueTypes.ULong && series.XValueType != ChartValueTypes.Long)
					{
						return false;
					}
					return true;
				case AxisName.X2:
					if (series.XAxisType != AxisType.Secondary)
					{
						break;
					}
					if (series.XValueType != ChartValueTypes.Int && series.XValueType != ChartValueTypes.UInt && series.XValueType != ChartValueTypes.ULong && series.XValueType != ChartValueTypes.Long)
					{
						return false;
					}
					return true;
				case AxisName.Y:
					if (series.YAxisType != 0)
					{
						break;
					}
					if (series.YValueType != ChartValueTypes.Int && series.YValueType != ChartValueTypes.UInt && series.YValueType != ChartValueTypes.ULong && series.YValueType != ChartValueTypes.Long)
					{
						return false;
					}
					return true;
				case AxisName.Y2:
					if (series.YAxisType != AxisType.Secondary)
					{
						break;
					}
					if (series.YValueType != ChartValueTypes.Int && series.YValueType != ChartValueTypes.UInt && series.YValueType != ChartValueTypes.ULong && series.YValueType != ChartValueTypes.Long)
					{
						return false;
					}
					return true;
				}
			}
			return false;
		}

		internal bool SeriesDateTimeType(AxisName axisName, string subAxisName)
		{
			foreach (string item in this.series)
			{
				Series series = base.Common.DataManager.Series[item];
				switch (axisName)
				{
				case AxisName.X:
					if (series.XAxisType != 0)
					{
						break;
					}
					if (series.XValueType != ChartValueTypes.Date && series.XValueType != ChartValueTypes.DateTime && series.XValueType != ChartValueTypes.Time && series.XValueType != ChartValueTypes.DateTimeOffset)
					{
						return false;
					}
					return true;
				case AxisName.X2:
					if (series.XAxisType != AxisType.Secondary)
					{
						break;
					}
					if (series.XValueType != ChartValueTypes.Date && series.XValueType != ChartValueTypes.DateTime && series.XValueType != ChartValueTypes.Time && series.XValueType != ChartValueTypes.DateTimeOffset)
					{
						return false;
					}
					return true;
				case AxisName.Y:
					if (series.YAxisType != 0)
					{
						break;
					}
					if (series.YValueType != ChartValueTypes.Date && series.YValueType != ChartValueTypes.DateTime && series.YValueType != ChartValueTypes.Time && series.YValueType != ChartValueTypes.DateTimeOffset)
					{
						return false;
					}
					return true;
				case AxisName.Y2:
					if (series.YAxisType != AxisType.Secondary)
					{
						break;
					}
					if (series.YValueType != ChartValueTypes.Date && series.YValueType != ChartValueTypes.DateTime && series.YValueType != ChartValueTypes.Time && series.YValueType != ChartValueTypes.DateTimeOffset)
					{
						return false;
					}
					return true;
				}
			}
			return false;
		}

		private void GetValuesFromData(Axis axis, out double autoMinimum, out double autoMaximum)
		{
			int numberOfAllPoints = this.GetNumberOfAllPoints();
			if (!axis.refreshMinMaxFromData && !double.IsNaN(axis.minimumFromData) && !double.IsNaN(axis.maximumFromData) && axis.numberOfPointsInAllSeries == numberOfAllPoints)
			{
				autoMinimum = axis.minimumFromData;
				autoMaximum = axis.maximumFromData;
			}
			else
			{
				AxisType type = AxisType.Primary;
				if (axis.axisType == AxisName.X2 || axis.axisType == AxisName.Y2)
				{
					type = AxisType.Secondary;
				}
				string[] array = (string[])this.GetXAxesSeries(type, axis.SubAxisName).ToArray(typeof(string));
				string[] seriesNames = (string[])this.GetYAxesSeries(type, axis.SubAxisName).ToArray(typeof(string));
				if (axis.axisType == AxisName.X2 || axis.axisType == AxisName.X)
				{
					if (this.stacked)
					{
						try
						{
							base.Common.DataManager.GetMinMaxXValue(out autoMinimum, out autoMaximum, array);
						}
						catch (Exception)
						{
							throw new InvalidOperationException(SR.ExceptionAxisStackedChartsDataPointsNumberMismatch);
						}
					}
					else if (this.secondYScale)
					{
						autoMaximum = base.Common.DataManager.GetMaxXWithRadiusValue((ChartArea)this, array);
						autoMinimum = base.Common.DataManager.GetMinXWithRadiusValue((ChartArea)this, array);
						ChartValueTypes xValueType = base.Common.DataManager.Series[array[0]].XValueType;
						if (xValueType != ChartValueTypes.Date && xValueType != ChartValueTypes.DateTime && xValueType != ChartValueTypes.Time && xValueType != ChartValueTypes.DateTimeOffset)
						{
							axis.roundedXValues = true;
						}
					}
					else
					{
						base.Common.DataManager.GetMinMaxXValue(out autoMinimum, out autoMaximum, array);
					}
				}
				else if (this.stacked)
				{
					try
					{
						if (this.hundredPercent)
						{
							autoMaximum = base.Common.DataManager.GetMaxHundredPercentStackedYValue(this.hundredPercentNegative, 0, seriesNames);
							autoMinimum = base.Common.DataManager.GetMinHundredPercentStackedYValue(this.hundredPercentNegative, 0, seriesNames);
						}
						else
						{
							double val = -1.7976931348623157E+308;
							double val2 = 1.7976931348623157E+308;
							double num = -1.7976931348623157E+308;
							double num2 = 1.7976931348623157E+308;
							ArrayList arrayList = this.SplitSeriesInStackedGroups(seriesNames);
							foreach (string[] item in arrayList)
							{
								double maxStackedYValue = base.Common.DataManager.GetMaxStackedYValue(0, item);
								double minStackedYValue = base.Common.DataManager.GetMinStackedYValue(0, item);
								double maxUnsignedStackedYValue = base.Common.DataManager.GetMaxUnsignedStackedYValue(0, item);
								double minUnsignedStackedYValue = base.Common.DataManager.GetMinUnsignedStackedYValue(0, item);
								val = Math.Max(val, maxStackedYValue);
								val2 = Math.Min(val2, minStackedYValue);
								num = Math.Max(num, maxUnsignedStackedYValue);
								num2 = Math.Min(num2, minUnsignedStackedYValue);
							}
							autoMaximum = Math.Max(val, num);
							autoMinimum = Math.Min(val2, num2);
						}
						if (axis.Logarithmic && autoMinimum < 1.0)
						{
							autoMinimum = 1.0;
						}
					}
					catch (Exception)
					{
						throw new InvalidOperationException(SR.ExceptionAxisStackedChartsDataPointsNumberMismatch);
					}
				}
				else if (this.secondYScale)
				{
					autoMaximum = base.Common.DataManager.GetMaxYWithRadiusValue((ChartArea)this, seriesNames);
					autoMinimum = base.Common.DataManager.GetMinYWithRadiusValue((ChartArea)this, seriesNames);
				}
				else
				{
					bool flag = false;
					if (base.Common != null && base.Common.Chart != null)
					{
						foreach (Series item2 in base.Common.Chart.Series)
						{
							if (item2.ChartArea == ((ChartArea)this).Name)
							{
								IChartType chartType = base.Common.ChartTypeRegistry.GetChartType(item2.ChartTypeName);
								if (chartType != null && chartType.ExtraYValuesConnectedToYAxis)
								{
									flag = true;
									break;
								}
							}
						}
					}
					if (flag)
					{
						base.Common.DataManager.GetMinMaxYValue(out autoMinimum, out autoMaximum, seriesNames);
					}
					else
					{
						base.Common.DataManager.GetMinMaxYValue(0, out autoMinimum, out autoMaximum, seriesNames);
					}
				}
				axis.maximumFromData = autoMaximum;
				axis.minimumFromData = autoMinimum;
				axis.refreshMinMaxFromData = false;
				axis.numberOfPointsInAllSeries = numberOfAllPoints;
			}
		}

		private ArrayList SplitSeriesInStackedGroups(string[] seriesNames)
		{
			Hashtable hashtable = new Hashtable();
			foreach (string text in seriesNames)
			{
				Series series = base.Common.Chart.Series[text];
				string key = string.Empty;
				if (StackedColumnChart.IsSeriesStackGroupNameSupported(series))
				{
					key = StackedColumnChart.GetSeriesStackGroupName(series);
				}
				if (hashtable.ContainsKey(key))
				{
					ArrayList arrayList = (ArrayList)hashtable[key];
					arrayList.Add(text);
				}
				else
				{
					ArrayList arrayList2 = new ArrayList();
					arrayList2.Add(text);
					hashtable.Add(key, arrayList2);
				}
			}
			ArrayList arrayList3 = new ArrayList();
			IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ArrayList arrayList4 = (ArrayList)((DictionaryEntry)enumerator.Current).Value;
					if (arrayList4.Count > 0)
					{
						int num = 0;
						string[] array = new string[arrayList4.Count];
						foreach (string item in arrayList4)
						{
							array[num++] = item;
						}
						arrayList3.Add(array);
					}
				}
				return arrayList3;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		private int GetNumberOfAllPoints()
		{
			int num = 0;
			foreach (Series item in base.Common.DataManager.Series)
			{
				num += item.Points.Count;
			}
			return num;
		}

		private void SetDefaultFromIndexes(Axis axis)
		{
			axis.SetTempAxisOffset();
			AxisType type = AxisType.Primary;
			if (axis.axisType == AxisName.X2 || axis.axisType == AxisName.Y2)
			{
				type = AxisType.Secondary;
			}
			double num = (double)base.Common.DataManager.GetNumberOfPoints((string[])this.GetXAxesSeries(type, axis.SubAxisName).ToArray(typeof(string)));
			double num2 = 0.0;
			axis.marginView = 0.0;
			if (axis.margin == 100.0)
			{
				axis.marginView = 1.0;
			}
			if (num + axis.margin / 100.0 == num2 - axis.margin / 100.0 + 1.0)
			{
				axis.SetAutoMaximum(num + 1.0);
				axis.SetAutoMinimum(num2);
			}
			else
			{
				axis.SetAutoMaximum(num + axis.margin / 100.0);
				axis.SetAutoMinimum(num2 - axis.margin / 100.0 + 1.0);
			}
			double num3 = (!(axis.GetViewMaximum() - axis.GetViewMinimum() <= 10.0)) ? axis.CalcInterval((axis.GetViewMaximum() - axis.GetViewMinimum()) / 5.0) : 1.0;
			ChartArea chartArea = (ChartArea)this;
			if (chartArea.Area3DStyle.Enable3D && !double.IsNaN(axis.interval3DCorrection))
			{
				num3 = Math.Ceiling(num3 / axis.interval3DCorrection);
				axis.interval3DCorrection = double.NaN;
				if (num3 > 1.0 && num3 < 4.0 && axis.GetViewMaximum() - axis.GetViewMinimum() <= 4.0)
				{
					num3 = 1.0;
				}
			}
			axis.SetInterval = num3;
			if (axis.offsetTempSet)
			{
				axis.minorGrid.intervalOffset -= axis.MajorGrid.Interval;
				axis.minorTickMark.intervalOffset -= axis.MajorTickMark.Interval;
			}
		}

		internal void SetData()
		{
			this.SetData(true);
		}

		internal void SetData(bool initializeAxes)
		{
			this.stacked = false;
			this.switchValueAxes = false;
			this.requireAxes = true;
			this.hundredPercent = false;
			this.hundredPercentNegative = false;
			this.chartAreaIsCurcular = false;
			this.secondYScale = false;
			bool flag = false;
			this.series.Clear();
			ChartAreaCollection chartAreas = base.Common.Chart.ChartAreas;
			bool flag2 = chartAreas.Count > 0 && chartAreas[0] == this && chartAreas.GetIndex("Default") == -1;
			foreach (Series item in base.Common.DataManager.Series)
			{
				if (item.IsVisible() && (this.name == item.ChartArea || (flag2 && string.Compare(item.ChartArea, "Default", StringComparison.Ordinal) == 0)) && base.Common.DataManager.GetNumberOfPoints(item.Name) != 0)
				{
					this.series.Add(item.Name);
				}
			}
			this.chartTypes.Clear();
			foreach (Series item2 in base.Common.DataManager.Series)
			{
				IChartType chartType = base.Common.ChartTypeRegistry.GetChartType(item2.ChartTypeName);
				bool flag3 = false;
				if ((item2.IsVisible() || chartType.RequireAxes) && (this.name == item2.ChartArea || (flag2 && string.Compare(item2.ChartArea, "Default", StringComparison.Ordinal) == 0)))
				{
					foreach (string chartType2 in this.chartTypes)
					{
						if (chartType2 == item2.ChartTypeName)
						{
							flag3 = true;
						}
					}
					if (!flag3)
					{
						if (chartType.Stacked)
						{
							this.stacked = true;
						}
						if (!flag)
						{
							if (chartType.SwitchValueAxes)
							{
								this.switchValueAxes = true;
							}
							if (!chartType.RequireAxes)
							{
								this.requireAxes = false;
							}
							if (chartType.CircularChartArea)
							{
								this.chartAreaIsCurcular = true;
							}
							if (chartType.HundredPercent)
							{
								this.hundredPercent = true;
							}
							if (chartType.HundredPercentSupportNegative)
							{
								this.hundredPercentNegative = true;
							}
							if (chartType.SecondYScale)
							{
								this.secondYScale = true;
							}
							flag = true;
						}
						else if (chartType.SwitchValueAxes != this.switchValueAxes)
						{
							throw new InvalidOperationException(SR.ExceptionChartAreaChartTypesCanNotCombine);
						}
						if (base.Common.DataManager.GetNumberOfPoints(item2.Name) != 0)
						{
							this.chartTypes.Add(item2.ChartTypeName);
						}
					}
				}
			}
			for (int i = 0; i <= 1; i++)
			{
				ArrayList xAxesSeries = this.GetXAxesSeries((AxisType)((i != 0) ? 1 : 0), string.Empty);
				if (xAxesSeries.Count > 0)
				{
					bool flag4 = false;
					string text = "";
					foreach (string item3 in xAxesSeries)
					{
						text = text + item3.Replace(",", "\\,") + ",";
						if (base.Common.DataManager.Series[item3].XValueIndexed)
						{
							flag4 = true;
						}
					}
					if (flag4)
					{
						try
						{
							base.Common.DataManipulator.CheckXValuesAlignment(base.Common.DataManipulator.ConvertToSeriesArray(text.TrimEnd(','), false));
						}
						catch (Exception ex)
						{
							throw new ArgumentException(SR.ExceptionAxisSeriesNotAligned + ex.Message);
						}
					}
				}
			}
			if (initializeAxes)
			{
				this.SetDefaultAxesValues();
			}
		}

		internal ArrayList GetSeriesFromChartType(string chartType)
		{
			ArrayList arrayList = new ArrayList();
			foreach (string item in this.series)
			{
				if (string.Compare(chartType, base.Common.DataManager.Series[item].ChartTypeName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					arrayList.Add(base.Common.DataManager.Series[item].Name);
				}
			}
			return arrayList;
		}

		internal ArrayList GetSeries()
		{
			ArrayList arrayList = new ArrayList();
			foreach (string item in this.series)
			{
				arrayList.Add(base.Common.DataManager.Series[item]);
			}
			return arrayList;
		}

		internal ArrayList GetXAxesSeries(AxisType type, string subAxisName)
		{
			ArrayList arrayList = new ArrayList();
			if (this.series.Count == 0)
			{
				return arrayList;
			}
			if (!this.IsSubAxesSupported && subAxisName.Length > 0)
			{
				return arrayList;
			}
			foreach (string item in this.series)
			{
				if (base.Common.DataManager.Series[item].XAxisType == type)
				{
					arrayList.Add(item);
				}
			}
			if (arrayList.Count == 0)
			{
				if (type == AxisType.Secondary)
				{
					return this.GetXAxesSeries(AxisType.Primary, string.Empty);
				}
				return this.GetXAxesSeries(AxisType.Secondary, string.Empty);
			}
			return arrayList;
		}

		internal ArrayList GetYAxesSeries(AxisType type, string subAxisName)
		{
			ArrayList arrayList = new ArrayList();
			foreach (string item in this.series)
			{
				AxisType axisType = base.Common.DataManager.Series[item].YAxisType;
				if (base.Common.DataManager.Series[item].ChartType == SeriesChartType.Radar || base.Common.DataManager.Series[item].ChartType == SeriesChartType.Polar)
				{
					axisType = AxisType.Primary;
					string empty = string.Empty;
				}
				if (axisType == type)
				{
					arrayList.Add(item);
				}
			}
			if (arrayList.Count == 0 && type == AxisType.Secondary)
			{
				return this.GetYAxesSeries(AxisType.Primary, string.Empty);
			}
			return arrayList;
		}

		internal Series GetFirstSeries()
		{
			if (this.series.Count == 0)
			{
				throw new InvalidOperationException(SR.ExceptionChartAreaSeriesNotFound);
			}
			return base.Common.DataManager.Series[this.series[0]];
		}

		internal double GetPointsInterval(bool logarithmic, double logarithmBase)
		{
			bool flag = default(bool);
			return this.GetPointsInterval(this.series, logarithmic, logarithmBase, false, out flag);
		}

		internal double GetPointsInterval(ArrayList seriesList, bool logarithmic, double logarithmBase, bool checkSameInterval, out bool sameInterval)
		{
			Series series = null;
			return this.GetPointsInterval(seriesList, logarithmic, logarithmBase, checkSameInterval, out sameInterval, out series);
		}

		internal double GetPointsInterval(ArrayList seriesList, bool logarithmic, double logarithmicBase, bool checkSameInterval, out bool sameInterval, out Series series)
		{
			long num = 9223372036854775807L;
			int num2 = 0;
			double num3 = -1.7976931348623157E+308;
			double num4 = 1.7976931348623157E+308;
			sameInterval = true;
			series = null;
			string text = "";
			if (seriesList != null)
			{
				foreach (string series4 in seriesList)
				{
					text = text + series4 + ",";
				}
			}
			if (!checkSameInterval || this.diffIntervalAlignmentChecked)
			{
				if (!logarithmic)
				{
					if (!double.IsNaN(this.intervalData) && this.intervalSeriesList == text)
					{
						sameInterval = this.intervalSameSize;
						series = this.intervalSeries;
						return this.intervalData;
					}
				}
				else if (!double.IsNaN(this.intervalLogData) && this.intervalSeriesList == text)
				{
					sameInterval = this.intervalSameSize;
					series = this.intervalSeries;
					return this.intervalLogData;
				}
			}
			int num5 = 0;
			Series series2 = null;
			ArrayList[] array = new ArrayList[seriesList.Count];
			foreach (string series5 in seriesList)
			{
				Series series3 = base.Common.DataManager.Series[series5];
				bool flag = series3.IsXValueDateTime();
				array[num5] = new ArrayList();
				bool flag2 = false;
				double num6 = -1.7976931348623157E+308;
				double num7 = 0.0;
				if (series3.Points.Count > 0)
				{
					num6 = ((!logarithmic) ? series3.Points[0].XValue : Math.Log(series3.Points[0].XValue, logarithmicBase));
				}
				foreach (DataPoint point in series3.Points)
				{
					num7 = ((!logarithmic) ? point.XValue : Math.Log(point.XValue, logarithmicBase));
					if (num6 > num7)
					{
						flag2 = true;
					}
					array[num5].Add(num7);
					num6 = num7;
				}
				if (flag2)
				{
					array[num5].Sort();
				}
				for (int i = 1; i < array[num5].Count; i++)
				{
					double num8 = Math.Abs((double)array[num5][i - 1] - (double)array[num5][i]);
					if (sameInterval)
					{
						if (flag)
						{
							if (num == 9223372036854775807L)
							{
								this.GetDateInterval((double)array[num5][i - 1], (double)array[num5][i], out num2, out num);
							}
							else
							{
								long num9 = 9223372036854775807L;
								int num10 = 0;
								this.GetDateInterval((double)array[num5][i - 1], (double)array[num5][i], out num10, out num9);
								if (num10 != num2 || num9 != num)
								{
									sameInterval = false;
								}
							}
						}
						else if (num3 != num8 && num3 != -1.7976931348623157E+308)
						{
							sameInterval = false;
						}
					}
					num3 = num8;
					if (num4 > num8 && num8 != 0.0)
					{
						num4 = num8;
						series2 = series3;
					}
				}
				num5++;
			}
			this.diffIntervalAlignmentChecked = false;
			if (checkSameInterval && !sameInterval && array.Length > 1)
			{
				bool flag3 = false;
				this.diffIntervalAlignmentChecked = true;
				int num11 = 0;
				ArrayList[] array2 = array;
				foreach (ArrayList arrayList in array2)
				{
					for (int k = 0; k < arrayList.Count; k++)
					{
						if (flag3)
						{
							break;
						}
						double num12 = (double)arrayList[k];
						for (int l = num11 + 1; l < array.Length; l++)
						{
							if (flag3)
							{
								break;
							}
							if (k < array[l].Count && (double)array[l][k] == num12)
							{
								goto IL_03ff;
							}
							if (array[l].Contains(num12))
							{
								goto IL_03ff;
							}
							continue;
							IL_03ff:
							flag3 = true;
							break;
						}
					}
					num11++;
				}
				if (flag3)
				{
					sameInterval = true;
				}
			}
			if (num4 == 1.7976931348623157E+308)
			{
				num4 = 1.0;
			}
			this.intervalSameSize = sameInterval;
			if (!logarithmic)
			{
				this.intervalData = num4;
				this.intervalSeries = series2;
				series = this.intervalSeries;
				this.intervalSeriesList = text;
				return this.intervalData;
			}
			this.intervalLogData = num4;
			this.intervalSeries = series2;
			series = this.intervalSeries;
			this.intervalSeriesList = text;
			return this.intervalLogData;
		}

		private void GetDateInterval(double value1, double value2, out int monthsInteval, out long ticksInterval)
		{
			DateTime dateTime = DateTime.FromOADate(value1);
			DateTime dateTime2 = DateTime.FromOADate(value2);
			monthsInteval = dateTime2.Month - dateTime.Month;
			monthsInteval += (dateTime2.Year - dateTime.Year) * 12;
			ticksInterval = 0L;
			ticksInterval += (dateTime2.Day - dateTime.Day) * 864000000000L;
			ticksInterval += (dateTime2.Hour - dateTime.Hour) * 36000000000L;
			ticksInterval += (long)(dateTime2.Minute - dateTime.Minute) * 600000000L;
			ticksInterval += (long)(dateTime2.Second - dateTime.Second) * 10000000L;
			ticksInterval += (long)(dateTime2.Millisecond - dateTime.Millisecond) * 10000L;
		}
	}
}
