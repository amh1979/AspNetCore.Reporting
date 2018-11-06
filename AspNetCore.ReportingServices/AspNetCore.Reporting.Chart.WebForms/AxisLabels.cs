using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class AxisLabels : AxisScale
	{
		private CustomLabelsCollection customLabels;

		[SRCategory("CategoryAttributeLabels")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLabelStyle")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Label LabelStyle
		{
			get
			{
				return base.labelStyle;
			}
			set
			{
				base.labelStyle = value;
				base.labelStyle.axis = (Axis)this;
				this.CustomLabels.axis = (Axis)this;
				base.Invalidate();
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeLabels")]
		[SRDescription("DescriptionAttributeCustomLabels")]
		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public CustomLabelsCollection CustomLabels
		{
			get
			{
				return this.customLabels;
			}
		}

		public AxisLabels()
		{
			base.labelStyle = new Label((Axis)this);
			this.customLabels = new CustomLabelsCollection((Axis)this);
		}

		internal bool IsCustomGridLines()
		{
			if (this.CustomLabels.Count > 0)
			{
				foreach (CustomLabel customLabel in this.CustomLabels)
				{
					if ((customLabel.GridTicks & GridTicks.Gridline) == GridTicks.Gridline)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal bool IsCustomTickMarks()
		{
			if (this.CustomLabels.Count > 0)
			{
				foreach (CustomLabel customLabel in this.CustomLabels)
				{
					if ((customLabel.GridTicks & GridTicks.TickMark) == GridTicks.TickMark)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal AxisType GetAxisType()
		{
			if (base.axisType != 0 && base.axisType != AxisName.Y)
			{
				return AxisType.Secondary;
			}
			return AxisType.Primary;
		}

		internal ArrayList GetAxisSeries()
		{
			ArrayList arrayList = new ArrayList();
			foreach (string item in base.chartArea.Series)
			{
				Series series = base.Common.DataManager.Series[item];
				if (base.axisType == AxisName.X || base.axisType == AxisName.X2)
				{
					if (series.XAxisType == this.GetAxisType())
					{
						arrayList.Add(series);
					}
				}
				else if (series.YAxisType == this.GetAxisType())
				{
					arrayList.Add(series);
				}
			}
			return arrayList;
		}

		internal Axis GetOtherTypeAxis()
		{
			return base.chartArea.GetAxis(base.axisType, (AxisType)((this.GetAxisType() == AxisType.Primary) ? 1 : 0), string.Empty);
		}

		internal void PostFillLabels()
		{
			foreach (CustomLabel customLabel3 in this.CustomLabels)
			{
				if (customLabel3.customLabel)
				{
					return;
				}
			}
			if (this.LabelStyle.Enabled && base.enabled && string.IsNullOrEmpty(((Axis)this).SubAxisName) && base.axisType != AxisName.Y && base.axisType != AxisName.Y2 && this.GetAxisSeries().Count <= 0)
			{
				this.CustomLabels.Clear();
				foreach (CustomLabel customLabel4 in this.GetOtherTypeAxis().CustomLabels)
				{
					this.CustomLabels.Add(customLabel4.Clone());
				}
			}
		}

		internal void FillLabels(bool removeFirstRow)
		{
			if (this.LabelStyle.Enabled && base.enabled)
			{
				if (base.chartArea != null && base.chartArea.chartAreaIsCurcular && base.axisType != AxisName.Y)
				{
					ICircularChartType circularChartType = base.chartArea.GetCircularChartType();
					if (circularChartType == null)
					{
						return;
					}
					if (!circularChartType.XAxisLabelsSupported())
					{
						return;
					}
				}
				bool flag = false;
				foreach (CustomLabel customLabel in this.CustomLabels)
				{
					if (customLabel.customLabel && (customLabel.RowIndex == 0 || base.chartArea.chartAreaIsCurcular))
					{
						flag = true;
					}
					if (customLabel.customLabel && base.Common.Chart != null && base.Common.Chart.LocalizeTextHandler != null)
					{
						customLabel.Text = base.Common.Chart.LocalizeTextHandler(customLabel, customLabel.Text, 0, ChartElementType.AxisLabels);
					}
				}
				if (removeFirstRow)
				{
					if (flag)
					{
						return;
					}
					for (int i = 0; i < this.CustomLabels.Count; i++)
					{
						if (this.CustomLabels[i].RowIndex == 0)
						{
							this.CustomLabels.RemoveAt(i);
							i = -1;
						}
					}
				}
				ArrayList arrayList = null;
				switch (base.axisType)
				{
				case AxisName.X:
					arrayList = base.chartArea.GetXAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
					break;
				case AxisName.Y:
					arrayList = base.chartArea.GetYAxesSeries(AxisType.Primary, ((Axis)this).SubAxisName);
					break;
				case AxisName.X2:
					arrayList = base.chartArea.GetXAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
					break;
				case AxisName.Y2:
					arrayList = base.chartArea.GetYAxesSeries(AxisType.Secondary, ((Axis)this).SubAxisName);
					break;
				}
				if (arrayList.Count != 0)
				{
					string[] array = new string[arrayList.Count];
					for (int j = 0; j < arrayList.Count; j++)
					{
						array[j] = (string)arrayList[j];
					}
					bool flag2 = base.SeriesXValuesZeros(array);
					bool indexedSeries = true;
					if (!flag2)
					{
						indexedSeries = base.IndexedSeries(array);
					}
					int num = 0;
					if (base.labelStyle.ShowEndLabels)
					{
						num = 1;
					}
					IChartType chartType = base.Common.ChartTypeRegistry.GetChartType(base.chartArea.GetFirstSeries().ChartTypeName);
					bool flag3 = false;
					if (chartType.RequireAxes)
					{
						flag3 = ((byte)((base.axisType != AxisName.Y && base.axisType != AxisName.Y2) ? 1 : 0) != 0);
						if (flag3 && !base.SeriesXValuesZeros((string[])arrayList.ToArray(typeof(string))))
						{
							flag3 = false;
						}
						if (flag3 && (base.labelStyle.IntervalOffset != 0.0 || base.labelStyle.Interval != 0.0))
						{
							flag3 = false;
						}
						ChartValueTypes chartValueTypes = (base.axisType != 0 && base.axisType != AxisName.X2) ? base.Common.DataManager.Series[arrayList[0]].YValueType : base.Common.DataManager.Series[arrayList[0]].indexedXValueType;
						if (base.labelStyle.IntervalType != 0 && base.labelStyle.IntervalType != DateTimeIntervalType.Number && chartValueTypes != ChartValueTypes.Time && chartValueTypes != ChartValueTypes.Date && chartValueTypes != ChartValueTypes.DateTimeOffset)
						{
							chartValueTypes = ChartValueTypes.DateTime;
						}
						double viewMaximum = base.GetViewMaximum();
						double viewMinimum = base.GetViewMinimum();
						if (flag3)
						{
							int numberOfPoints = base.Common.DataManager.GetNumberOfPoints((string[])arrayList.ToArray(typeof(string)));
							if (num == 1)
							{
								this.CustomLabels.Add(-0.5, 0.5, ValueConverter.FormatValue(base.Common.Chart, this, 0.0, this.LabelStyle.Format, chartValueTypes, ChartElementType.AxisLabels), false);
							}
							for (int k = 0; k < numberOfPoints; k++)
							{
								this.CustomLabels.Add((double)k + 0.5, (double)k + 1.5, ValueConverter.FormatValue(base.Common.Chart, this, (double)(k + 1), this.LabelStyle.Format, chartValueTypes, ChartElementType.AxisLabels), false);
							}
							if (num == 1)
							{
								this.CustomLabels.Add((double)numberOfPoints + 0.5, (double)numberOfPoints + 1.5, ValueConverter.FormatValue(base.Common.Chart, this, (double)(numberOfPoints + 1), this.LabelStyle.Format, chartValueTypes, ChartElementType.AxisLabels), false);
							}
							foreach (string item in arrayList)
							{
								int l = (num == 1) ? 1 : 0;
								foreach (DataPoint point in base.Common.DataManager.Series[item].Points)
								{
									for (; this.CustomLabels[l].RowIndex > 0; l++)
									{
									}
									if ((base.axisType == AxisName.X || base.axisType == AxisName.X2) && point.AxisLabel.Length > 0)
									{
										this.CustomLabels[l].Text = point.AxisLabel;
										if (base.Common.Chart != null && base.Common.Chart.LocalizeTextHandler != null)
										{
											this.CustomLabels[l].Text = base.Common.Chart.LocalizeTextHandler(point, this.CustomLabels[l].Text, point.ElementId, ChartElementType.DataPoint);
										}
									}
									l++;
								}
							}
						}
						else if (viewMinimum != viewMaximum)
						{
							Series series = null;
							if (base.axisType == AxisName.X || base.axisType == AxisName.X2)
							{
								ArrayList xAxesSeries = base.chartArea.GetXAxesSeries((AxisType)((base.axisType != 0) ? 1 : 0), ((Axis)this).SubAxisName);
								if (xAxesSeries.Count > 0)
								{
									series = base.Common.DataManager.Series[xAxesSeries[0]];
									if (series != null && !series.XValueIndexed)
									{
										series = null;
									}
								}
							}
							DateTimeIntervalType dateTimeIntervalType = (base.labelStyle.IntervalOffsetType == DateTimeIntervalType.Auto) ? base.labelStyle.IntervalType : base.labelStyle.IntervalOffsetType;
							double num2 = viewMinimum;
							if (!base.chartArea.chartAreaIsCurcular || base.axisType == AxisName.Y || base.axisType == AxisName.Y2)
							{
								num2 = base.AlignIntervalStart(num2, base.labelStyle.Interval, base.labelStyle.IntervalType, series);
							}
							if (base.labelStyle.IntervalOffset != 0.0 && series == null)
							{
								num2 += base.GetIntervalSize(num2, base.labelStyle.IntervalOffset, dateTimeIntervalType, series, 0.0, DateTimeIntervalType.Number, true, false);
							}
							if (chartValueTypes == ChartValueTypes.DateTime || chartValueTypes == ChartValueTypes.Date || chartValueTypes == ChartValueTypes.Time || chartValueTypes == ChartValueTypes.DateTimeOffset || series != null)
							{
								double num3 = num2;
								if (!((viewMaximum - num2) / base.GetIntervalSize(num2, base.labelStyle.Interval, base.labelStyle.IntervalType, series, 0.0, DateTimeIntervalType.Number, true) > 10000.0))
								{
									int num4 = 0;
									double num5 = viewMaximum - base.GetIntervalSize(viewMaximum, base.labelStyle.Interval, base.labelStyle.IntervalType, series, base.labelStyle.IntervalOffset, dateTimeIntervalType, true) / 2.0;
									double num6 = viewMinimum + base.GetIntervalSize(viewMinimum, base.labelStyle.Interval, base.labelStyle.IntervalType, series, base.labelStyle.IntervalOffset, dateTimeIntervalType, true) / 2.0;
									while ((decimal)num3 <= (decimal)viewMaximum)
									{
										double intervalSize = base.GetIntervalSize(num3, base.labelStyle.Interval, base.labelStyle.IntervalType, series, base.labelStyle.IntervalOffset, dateTimeIntervalType, true);
										double num7 = num3;
										if (base.Logarithmic)
										{
											num7 = Math.Pow(base.logarithmBase, num7);
										}
										if (num4++ > 10000 || (num == 0 && num3 >= num5))
										{
											break;
										}
										double num9 = num3 - intervalSize * 0.5;
										double toPosition = num3 + intervalSize * 0.5;
										if (num == 0 && num3 <= num6)
										{
											num3 += intervalSize;
										}
										else if ((decimal)num9 > (decimal)viewMaximum)
										{
											num3 += intervalSize;
										}
										else
										{
											string pointLabel = this.GetPointLabel(arrayList, num7, !flag2, indexedSeries);
											if (pointLabel.Length == 0)
											{
												if (num3 <= base.maximum && (num3 != base.maximum || !base.Common.DataManager.Series[arrayList[0]].XValueIndexed))
												{
													this.CustomLabels.Add(num9, toPosition, ValueConverter.FormatValue(base.Common.Chart, this, num7, this.LabelStyle.Format, chartValueTypes, ChartElementType.AxisLabels), false);
												}
											}
											else
											{
												this.CustomLabels.Add(num9, toPosition, pointLabel, false);
											}
											num3 += intervalSize;
										}
									}
								}
							}
							else
							{
								if (num2 != viewMinimum)
								{
									num = 1;
								}
								int num10 = 0;
								for (double num11 = num2 - (double)num * base.labelStyle.Interval; num11 < viewMaximum - 1.5 * base.labelStyle.Interval * (double)(1 - num); num11 = (double)((decimal)num11 + (decimal)base.labelStyle.Interval))
								{
									num10++;
									if (num10 > 10000)
									{
										break;
									}
									double num7 = Axis.RemoveNoiseFromDoubleMath(num11) + Axis.RemoveNoiseFromDoubleMath(base.labelStyle.Interval);
									double value = Math.Log(base.labelStyle.Interval);
									double value2 = Math.Log(Math.Abs(num7));
									int num12 = (int)Math.Abs(value) + 5;
									if (num12 > 15)
									{
										num12 = 15;
									}
									if (Math.Abs(value) < Math.Abs(value2) - 5.0)
									{
										num7 = Math.Round(num7, num12);
									}
									if ((viewMaximum - num2) / base.labelStyle.Interval > 10000.0)
									{
										break;
									}
									if (base.Logarithmic)
									{
										num7 = Math.Pow(base.logarithmBase, num7);
									}
									double num9 = (double)((decimal)num11 + (decimal)base.labelStyle.Interval * 0.5m);
									double toPosition = (double)((decimal)num11 + (decimal)base.labelStyle.Interval * 1.5m);
									if (!((decimal)num9 > (decimal)viewMaximum) && !((decimal)((num9 + toPosition) / 2.0) > (decimal)viewMaximum))
									{
										string pointLabel2 = this.GetPointLabel(arrayList, num7, !flag2, indexedSeries);
										if (pointLabel2.Length > 15 && num7 < 1E-06)
										{
											num7 = 0.0;
										}
										if (pointLabel2.Length == 0)
										{
											if (!base.Common.DataManager.Series[arrayList[0]].XValueIndexed || !(num11 > base.maximum))
											{
												this.CustomLabels.Add(num9, toPosition, ValueConverter.FormatValue(base.Common.Chart, this, num7, this.LabelStyle.Format, chartValueTypes, ChartElementType.AxisLabels), false);
											}
										}
										else
										{
											this.CustomLabels.Add(num9, toPosition, pointLabel2, false);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private string GetPointLabel(ArrayList series, double valuePosition, bool nonZeroXValues, bool indexedSeries)
		{
			int num = 0;
			foreach (string item in series)
			{
				Series series2 = base.Common.DataManager.Series[item];
				num = Math.Max(num, series2.Points.Count);
			}
			bool flag = true;
			foreach (string item2 in series)
			{
				Series series3 = base.Common.DataManager.Series[item2];
				if ((base.axisType == AxisName.X || base.axisType == AxisName.X2) && (base.margin != 0.0 || num == 1) && !series3.XValueIndexed && series3.Points[0].AxisLabel.Length > 0 && series3.Points[series3.Points.Count - 1].AxisLabel.Length > 0)
				{
					flag = false;
				}
				if (!series3.noLabelsInPoints || (nonZeroXValues && indexedSeries))
				{
					string pointLabel = this.GetPointLabel(series3, valuePosition, nonZeroXValues, indexedSeries);
					if (pointLabel != "")
					{
						return pointLabel;
					}
				}
				string text = ((DataPointAttributes)series3)["__IndexedSeriesLabelsSource__"];
				if (!string.IsNullOrEmpty(text))
				{
					Series series4 = base.Common.DataManager.Series[text];
					if (series4 != null)
					{
						string pointLabel2 = this.GetPointLabel(series4, valuePosition, nonZeroXValues, true);
						if (pointLabel2 != "")
						{
							return pointLabel2;
						}
					}
				}
			}
			if (!flag)
			{
				return " ";
			}
			return "";
		}

		private string GetPointLabel(Series series, double valuePosition, bool nonZeroXValues, bool indexedSeries)
		{
			int num = 1;
			if (base.axisType != AxisName.Y && base.axisType != AxisName.Y2)
			{
				if (base.axisType == AxisName.X && series.XAxisType == AxisType.Primary)
				{
					goto IL_0042;
				}
				if (base.axisType == AxisName.X2 && series.XAxisType == AxisType.Secondary)
				{
					goto IL_0042;
				}
				return "";
			}
			return "";
			IL_0042:
			foreach (DataPoint point in series.Points)
			{
				if (indexedSeries)
				{
					if (valuePosition == (double)num)
					{
						if (point.AxisLabel.Length == 0 && nonZeroXValues)
						{
							return ValueConverter.FormatValue(base.Common.Chart, this, point.XValue, this.LabelStyle.Format, series.XValueType, ChartElementType.AxisLabels);
						}
						string text = point.ReplaceKeywords(point.AxisLabel);
						if (text.Length > 0 && series.chart != null && series.chart.LocalizeTextHandler != null)
						{
							text = series.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
						}
						return text;
					}
				}
				else if (point.XValue == valuePosition)
				{
					string text2 = point.ReplaceKeywords(point.AxisLabel);
					if (text2.Length > 0 && series.chart != null && series.chart.LocalizeTextHandler != null)
					{
						text2 = series.chart.LocalizeTextHandler(point, text2, point.ElementId, ChartElementType.DataPoint);
					}
					return text2;
				}
				num++;
			}
			return "";
		}
	}
}
