using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAxisScaleBreakStyle_AxisScaleBreakStyle")]
	[DefaultProperty("Enabled")]
	internal class AxisScaleBreakStyle
	{
		internal Axis axis;

		private bool enabled;

		private BreakLineType breakLineType = BreakLineType.Ragged;

		private double segmentSpacing = 1.5;

		private Color breakLineColor = Color.Black;

		private int breakLineWidth = 1;

		private ChartDashStyle breakLineStyle = ChartDashStyle.Solid;

		private double minSegmentSize = 10.0;

		private int totalNumberOfSegments = 100;

		private int minimumNumberOfEmptySegments = 25;

		private int maximumNumberOfBreaks = 2;

		private AutoBool startFromZero;

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(AutoBool.Auto)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_StartFromZero")]
		public AutoBool StartFromZero
		{
			get
			{
				return this.startFromZero;
			}
			set
			{
				this.startFromZero = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(2)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_MaxNumberOfBreaks")]
		public int MaxNumberOfBreaks
		{
			get
			{
				return this.maximumNumberOfBreaks;
			}
			set
			{
				if (value >= 1 && value <= 5)
				{
					this.maximumNumberOfBreaks = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleBreaksNumberInvalid);
			}
		}

		[DefaultValue(25)]
		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_CollapsibleSpaceThreshold")]
		public int CollapsibleSpaceThreshold
		{
			get
			{
				return this.minimumNumberOfEmptySegments;
			}
			set
			{
				if (value >= 10 && value <= 90)
				{
					this.minimumNumberOfEmptySegments = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleBreaksCollapsibleSpaceInvalid);
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_Enabled")]
		[ParenthesizePropertyName(true)]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_BreakLineType")]
		[DefaultValue(BreakLineType.Ragged)]
		[SRCategory("CategoryAttributeAppearance")]
		public BreakLineType BreakLineType
		{
			get
			{
				return this.breakLineType;
			}
			set
			{
				this.breakLineType = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(1.5)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_Spacing")]
		public double Spacing
		{
			get
			{
				return this.segmentSpacing;
			}
			set
			{
				if (!(value < 0.0) && !(value > 10.0))
				{
					this.segmentSpacing = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleBreaksSpacingInvalid);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_LineColor")]
		public Color LineColor
		{
			get
			{
				return this.breakLineColor;
			}
			set
			{
				this.breakLineColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_LineWidth")]
		public int LineWidth
		{
			get
			{
				return this.breakLineWidth;
			}
			set
			{
				if (!((double)value < 1.0) && value <= 10)
				{
					this.breakLineWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisScaleBreaksLineWidthInvalid);
			}
		}

		[SRDescription("DescriptionAttributeAxisScaleBreakStyle_LineWidth")]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRCategory("CategoryAttributeAppearance")]
		public ChartDashStyle LineStyle
		{
			get
			{
				return this.breakLineStyle;
			}
			set
			{
				this.breakLineStyle = value;
				this.Invalidate();
			}
		}

		public AxisScaleBreakStyle()
		{
		}

		public AxisScaleBreakStyle(Axis axis)
		{
			this.axis = axis;
		}

		internal bool IsEnabled()
		{
			if (this.Enabled && this.CanUseAxisScaleBreaks())
			{
				return true;
			}
			return false;
		}

		internal bool CanUseAxisScaleBreaks()
		{
			if (this.axis != null && this.axis.chartArea != null && this.axis.chartArea.Common.Chart != null)
			{
				if (this.axis.chartArea.Area3DStyle.Enable3D)
				{
					return false;
				}
				if (this.axis.axisType != 0 && this.axis.axisType != AxisName.X2)
				{
					if (this.axis.Logarithmic)
					{
						return false;
					}
					if (this.axis.View.IsZoomed)
					{
						return false;
					}
					ArrayList axisSeries = AxisScaleBreakStyle.GetAxisSeries(this.axis);
					foreach (Series item in axisSeries)
					{
						if (item.ChartType == SeriesChartType.Renko || item.ChartType == SeriesChartType.PointAndFigure)
						{
							return false;
						}
						IChartType chartType = this.axis.chartArea.Common.ChartTypeRegistry.GetChartType(item.ChartTypeName);
						if (chartType == null)
						{
							return false;
						}
						if (chartType.CircularChartArea || chartType.Stacked || !chartType.RequireAxes)
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
			return false;
		}

		internal static ArrayList GetAxisSeries(Axis axis)
		{
			ArrayList arrayList = new ArrayList();
			if (axis != null && axis.chartArea != null && axis.chartArea.Common.Chart != null)
			{
				{
					foreach (Series item in axis.chartArea.Common.Chart.Series)
					{
						if (item.ChartArea == axis.chartArea.Name && item.Enabled && (axis.axisType != AxisName.Y || item.YAxisType != AxisType.Secondary) && (axis.axisType != AxisName.Y2 || item.YAxisType != 0))
						{
							arrayList.Add(item);
						}
					}
					return arrayList;
				}
			}
			return arrayList;
		}

		private void Invalidate()
		{
			if (this.axis != null)
			{
				this.axis.Invalidate();
			}
		}

		internal void GetAxisSegmentForScaleBreaks(AxisScaleSegmentCollection axisSegments)
		{
			axisSegments.Clear();
			if (this.IsEnabled())
			{
				this.FillAxisSegmentCollection(axisSegments);
				if (axisSegments.Count >= 1)
				{
					int startScaleFromZeroSegmentIndex = this.GetStartScaleFromZeroSegmentIndex(axisSegments);
					int num = 0;
					foreach (AxisScaleSegment axisSegment in axisSegments)
					{
						bool shouldStartFromZero = (byte)((num == startScaleFromZeroSegmentIndex) ? 1 : 0) != 0;
						double scaleMinimum = axisSegment.ScaleMinimum;
						double scaleMaximum = axisSegment.ScaleMaximum;
						axisSegment.Interval = this.axis.EstimateNumberAxis(ref scaleMinimum, ref scaleMaximum, shouldStartFromZero, this.axis.prefferedNumberofIntervals, this.axis.Crossing, true, true);
						axisSegment.ScaleMinimum = scaleMinimum;
						axisSegment.ScaleMaximum = scaleMaximum;
						if (axisSegment.ScaleMinimum < this.axis.Minimum)
						{
							axisSegment.ScaleMinimum = this.axis.Minimum;
						}
						if (axisSegment.ScaleMaximum > this.axis.Maximum)
						{
							axisSegment.ScaleMaximum = this.axis.Maximum;
						}
						num++;
					}
					bool flag = false;
					AxisScaleSegment axisScaleSegment2 = axisSegments[0];
					for (int i = 1; i < axisSegments.Count; i++)
					{
						AxisScaleSegment axisScaleSegment3 = axisSegments[i];
						if (axisScaleSegment3.ScaleMinimum <= axisScaleSegment2.ScaleMaximum)
						{
							if (axisScaleSegment3.ScaleMaximum > axisScaleSegment2.ScaleMaximum)
							{
								axisScaleSegment2.ScaleMaximum = axisScaleSegment3.ScaleMaximum;
							}
							flag = true;
							axisSegments.RemoveAt(i);
							i--;
						}
						else
						{
							axisScaleSegment2 = axisScaleSegment3;
						}
					}
					if (flag)
					{
						this.SetAxisSegmentPosition(axisSegments);
					}
				}
			}
		}

		private int GetStartScaleFromZeroSegmentIndex(AxisScaleSegmentCollection axisSegments)
		{
			if (this.StartFromZero == AutoBool.Auto || this.StartFromZero == AutoBool.True)
			{
				int num = 0;
				foreach (AxisScaleSegment axisSegment in axisSegments)
				{
					if (axisSegment.ScaleMinimum < 0.0 && axisSegment.ScaleMaximum > 0.0)
					{
						return -1;
					}
					if (!(axisSegment.ScaleMinimum > 0.0) && num != axisSegments.Count - 1)
					{
						num++;
						continue;
					}
					if (this.StartFromZero != 0 || !(axisSegment.ScaleMinimum > 2.0 * (axisSegment.ScaleMaximum - axisSegment.ScaleMinimum)))
					{
						return num;
					}
					return -1;
				}
			}
			return -1;
		}

		private void SetAxisSegmentPosition(AxisScaleSegmentCollection axisSegments)
		{
			int num = 0;
			foreach (AxisScaleSegment axisSegment in axisSegments)
			{
				if (axisSegment.Tag is int)
				{
					num += (int)axisSegment.Tag;
				}
			}
			double num2 = Math.Min(this.minSegmentSize, Math.Floor(100.0 / (double)axisSegments.Count));
			double num3 = 0.0;
			for (int i = 0; i < axisSegments.Count; i++)
			{
				axisSegments[i].Position = ((num3 > 100.0) ? 100.0 : num3);
				axisSegments[i].Size = Math.Round((double)(int)axisSegments[i].Tag / ((double)num / 100.0), 5);
				if (axisSegments[i].Size < num2)
				{
					axisSegments[i].Size = num2;
				}
				if (i < axisSegments.Count - 1)
				{
					axisSegments[i].Spacing = this.segmentSpacing;
				}
				num3 += axisSegments[i].Size;
			}
			double num4 = 0.0;
			do
			{
				num4 = 0.0;
				double num5 = -1.7976931348623157E+308;
				int num6 = -1;
				for (int j = 0; j < axisSegments.Count; j++)
				{
					num4 += axisSegments[j].Size;
					if (axisSegments[j].Size > num5)
					{
						num5 = axisSegments[j].Size;
						num6 = j;
					}
				}
				if (num4 > 100.0)
				{
					axisSegments[num6].Size -= num4 - 100.0;
					if (axisSegments[num6].Size < num2)
					{
						axisSegments[num6].Size = num2;
					}
					double num7 = axisSegments[num6].Position + axisSegments[num6].Size;
					for (int k = num6 + 1; k < axisSegments.Count; k++)
					{
						axisSegments[k].Position = num7;
						num7 += axisSegments[k].Size;
					}
				}
			}
			while (num4 > 100.0);
		}

		private void FillAxisSegmentCollection(AxisScaleSegmentCollection axisSegments)
		{
			axisSegments.Clear();
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double[] array = null;
			double[] array2 = null;
			int[] seriesDataStatistics = this.GetSeriesDataStatistics(this.totalNumberOfSegments, out num, out num2, out num3, out array, out array2);
			if (seriesDataStatistics != null)
			{
				double num4 = num;
				double num5 = num2;
				this.axis.EstimateNumberAxis(ref num4, ref num5, this.axis.StartFromZero, this.axis.prefferedNumberofIntervals, this.axis.Crossing, true, true);
				if (num2 != num)
				{
					double num6 = (num2 - num) / ((num5 - num4) / 100.0);
					ArrayList arrayList = new ArrayList();
					bool flag = false;
					while (!flag)
					{
						flag = true;
						int num7 = 0;
						int num8 = 0;
						this.GetLargestSequenseOfSegmentsWithNoPoints(seriesDataStatistics, out num7, out num8);
						int num9 = (int)((double)this.minimumNumberOfEmptySegments * (100.0 / num6));
						if (axisSegments.Count > 0 && num8 > 0)
						{
							foreach (AxisScaleSegment axisSegment in axisSegments)
							{
								if (num7 > 0 && num7 + num8 <= array.Length - 1 && array[num7 - 1] >= axisSegment.ScaleMinimum && array2[num7 + num8] <= axisSegment.ScaleMaximum)
								{
									double num10 = axisSegment.ScaleMaximum - axisSegment.ScaleMinimum;
									double num11 = array2[num7 + num8] - array[num7 - 1];
									double num12 = num11 / (num10 / 100.0);
									num12 = num12 / 100.0 * axisSegment.Size;
									if (num12 > (double)num9 && (double)num8 > this.minSegmentSize)
									{
										num9 = num8;
									}
								}
							}
						}
						if (num8 >= num9)
						{
							flag = false;
							arrayList.Add(num7);
							arrayList.Add(num8);
							axisSegments.Clear();
							if (arrayList.Count > 0)
							{
								double num13 = double.NaN;
								double num14 = double.NaN;
								int num15 = 0;
								for (int i = 0; i < seriesDataStatistics.Length; i++)
								{
									bool flag2 = this.IsExcludedSegment(arrayList, i);
									if (!flag2 && !double.IsNaN(array2[i]) && !double.IsNaN(array[i]))
									{
										num15 += seriesDataStatistics[i];
										if (double.IsNaN(num13))
										{
											num13 = array2[i];
											num14 = array[i];
										}
										else
										{
											num14 = array[i];
										}
									}
									if (!double.IsNaN(num13) && (flag2 || i == seriesDataStatistics.Length - 1))
									{
										if (num14 == num13)
										{
											num13 -= num3;
											num14 += num3;
										}
										AxisScaleSegment axisScaleSegment2 = new AxisScaleSegment();
										axisScaleSegment2.ScaleMaximum = num14;
										axisScaleSegment2.ScaleMinimum = num13;
										axisScaleSegment2.Tag = num15;
										axisSegments.Add(axisScaleSegment2);
										num13 = double.NaN;
										num14 = double.NaN;
										num15 = 0;
									}
								}
							}
							this.SetAxisSegmentPosition(axisSegments);
						}
						if (axisSegments.Count - 1 >= this.maximumNumberOfBreaks)
						{
							flag = true;
						}
					}
				}
			}
		}

		private bool IsExcludedSegment(ArrayList excludedSegments, int segmentIndex)
		{
			for (int i = 0; i < excludedSegments.Count; i += 2)
			{
				if (segmentIndex >= (int)excludedSegments[i] && segmentIndex < (int)excludedSegments[i] + (int)excludedSegments[i + 1])
				{
					return true;
				}
			}
			return false;
		}

		internal int[] GetSeriesDataStatistics(int segmentCount, out double minYValue, out double maxYValue, out double segmentSize, out double[] segmentMaxValue, out double[] segmentMinValue)
		{
			ArrayList axisSeries = AxisScaleBreakStyle.GetAxisSeries(this.axis);
			minYValue = 0.0;
			maxYValue = 0.0;
			this.axis.Common.DataManager.GetMinMaxYValue(axisSeries, out minYValue, out maxYValue);
			if (axisSeries.Count == 0)
			{
				segmentSize = 0.0;
				segmentMaxValue = null;
				segmentMinValue = null;
				return null;
			}
			segmentSize = (maxYValue - minYValue) / (double)segmentCount;
			int[] array = new int[segmentCount];
			segmentMaxValue = new double[segmentCount];
			segmentMinValue = new double[segmentCount];
			for (int i = 0; i < segmentCount; i++)
			{
				segmentMaxValue[i] = double.NaN;
				segmentMinValue[i] = double.NaN;
			}
			foreach (Series item in axisSeries)
			{
				int num = 1;
				IChartType chartType = this.axis.chartArea.Common.ChartTypeRegistry.GetChartType(item.ChartTypeName);
				if (chartType != null && chartType.ExtraYValuesConnectedToYAxis && chartType.YValuesPerPoint > 1)
				{
					num = chartType.YValuesPerPoint;
				}
				foreach (DataPoint point in item.Points)
				{
					if (!point.Empty)
					{
						for (int j = 0; j < num; j++)
						{
							int num2 = (int)Math.Floor((point.YValues[j] - minYValue) / segmentSize);
							if (num2 < 0)
							{
								num2 = 0;
							}
							if (num2 > segmentCount - 1)
							{
								num2 = segmentCount - 1;
							}
							array[num2]++;
							if (array[num2] == 1)
							{
								segmentMaxValue[num2] = point.YValues[j];
								segmentMinValue[num2] = point.YValues[j];
							}
							else
							{
								segmentMaxValue[num2] = Math.Max(segmentMaxValue[num2], point.YValues[j]);
								segmentMinValue[num2] = Math.Min(segmentMinValue[num2], point.YValues[j]);
							}
						}
					}
				}
			}
			return array;
		}

		internal bool GetLargestSequenseOfSegmentsWithNoPoints(int[] segmentPointNumber, out int startSegment, out int numberOfSegments)
		{
			startSegment = -1;
			numberOfSegments = 0;
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < segmentPointNumber.Length; i++)
			{
				if (segmentPointNumber[i] == 0)
				{
					if (num == -1)
					{
						num = i;
						num2 = 1;
					}
					else
					{
						num2++;
					}
				}
				if (num2 > 0 && (segmentPointNumber[i] != 0 || i == segmentPointNumber.Length - 1))
				{
					if (num2 > numberOfSegments)
					{
						startSegment = num;
						numberOfSegments = num2;
					}
					num = -1;
					num2 = 0;
				}
			}
			if (numberOfSegments != 0)
			{
				for (int j = startSegment; j < startSegment + numberOfSegments; j++)
				{
					segmentPointNumber[j] = -1;
				}
				return true;
			}
			return false;
		}
	}
}
