using System;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class BubbleChart : PointChart
	{
		private bool scaleDetected;

		private double maxPossibleBubbleSize = 15.0;

		private double minPossibleBubbleSize = 3.0;

		private float maxBubleSize;

		private float minBubleSize;

		private double minAll = 1.7976931348623157E+308;

		private double maxAll = -1.7976931348623157E+308;

		private double valueDiff;

		private double valueScale = 1.0;

		public override string Name
		{
			get
			{
				return "Bubble";
			}
		}

		public override int YValuesPerPoint
		{
			get
			{
				return 2;
			}
		}

		public override bool SecondYScale
		{
			get
			{
				return true;
			}
		}

		public BubbleChart()
			: base(true)
		{
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		protected override void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.scaleDetected = false;
			base.ProcessChartType(selection, graph, common, area, seriesToDraw);
		}

		protected override int GetMarkerBorderSize(DataPointAttributes point)
		{
			return point.BorderWidth;
		}

		protected override SizeF GetMarkerSize(ChartGraphics graph, CommonElements common, ChartArea area, DataPoint point, int markerSize, string markerImage)
		{
			if (point.YValues.Length < this.YValuesPerPoint)
			{
				throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(this.Name, this.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
			}
			SizeF result = new SizeF((float)markerSize, (float)markerSize);
			if (point.series.YValuesPerPoint > 1 && !point.Empty)
			{
				result.Width = this.ScaleBubbleSize(graph, common, area, point.YValues[1]);
				result.Height = this.ScaleBubbleSize(graph, common, area, point.YValues[1]);
			}
			return result;
		}

		private float ScaleBubbleSize(ChartGraphics graph, CommonElements common, ChartArea area, double value)
		{
			if (!this.scaleDetected)
			{
				this.minAll = 1.7976931348623157E+308;
				this.maxAll = -1.7976931348623157E+308;
				foreach (Series item in common.DataManager.Series)
				{
					if (string.Compare(item.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) == 0 && item.ChartArea == area.Name && item.IsVisible())
					{
						if (item.IsAttributeSet("BubbleScaleMin"))
						{
							this.minAll = Math.Min(this.minAll, CommonElements.ParseDouble(((DataPointAttributes)item)["BubbleScaleMin"]));
						}
						if (item.IsAttributeSet("BubbleScaleMax"))
						{
							this.maxAll = Math.Max(this.maxAll, CommonElements.ParseDouble(((DataPointAttributes)item)["BubbleScaleMax"]));
						}
						if (item.IsAttributeSet("BubbleMaxSize"))
						{
							this.maxPossibleBubbleSize = CommonElements.ParseDouble(((DataPointAttributes)item)["BubbleMaxSize"]);
							if (!(this.maxPossibleBubbleSize < 0.0) && !(this.maxPossibleBubbleSize > 100.0))
							{
								goto IL_013b;
							}
							throw new ArgumentException(SR.ExceptionCustomAttributeIsNotInRange0to100("BubbleMaxSize"));
						}
						goto IL_013b;
					}
					continue;
					IL_013b:
					if (item.IsAttributeSet("BubbleMinSize"))
					{
						this.minPossibleBubbleSize = CommonElements.ParseDouble(((DataPointAttributes)item)["BubbleMinSize"]);
						if (!(this.minPossibleBubbleSize < 0.0) && !(this.minPossibleBubbleSize > 100.0))
						{
							goto IL_0190;
						}
						throw new ArgumentException(SR.ExceptionCustomAttributeIsNotInRange0to100("BubbleMinSize"));
					}
					goto IL_0190;
					IL_0190:
					base.labelYValueIndex = 0;
					if (item.IsAttributeSet("BubbleUseSizeForLabel") && string.Compare(((DataPointAttributes)item)["BubbleUseSizeForLabel"], "true", StringComparison.OrdinalIgnoreCase) == 0)
					{
						base.labelYValueIndex = 1;
						break;
					}
				}
				if (this.minAll == 1.7976931348623157E+308 || this.maxAll == -1.7976931348623157E+308)
				{
					double val = 1.7976931348623157E+308;
					double val2 = -1.7976931348623157E+308;
					foreach (Series item2 in common.DataManager.Series)
					{
						if (item2.ChartTypeName == this.Name && item2.ChartArea == area.Name && item2.IsVisible())
						{
							foreach (DataPoint point in item2.Points)
							{
								if (!point.Empty)
								{
									if (point.YValues.Length < this.YValuesPerPoint)
									{
										throw new InvalidOperationException(SR.ExceptionChartTypeRequiresYValues(this.Name, this.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)));
									}
									val = Math.Min(val, point.YValues[1]);
									val2 = Math.Max(val2, point.YValues[1]);
								}
							}
						}
					}
					if (this.minAll == 1.7976931348623157E+308)
					{
						this.minAll = val;
					}
					if (this.maxAll == -1.7976931348623157E+308)
					{
						this.maxAll = val2;
					}
				}
				SizeF absoluteSize = graph.GetAbsoluteSize(area.PlotAreaPosition.GetSize());
				this.maxBubleSize = (float)((double)Math.Min(absoluteSize.Width, absoluteSize.Height) / (100.0 / this.maxPossibleBubbleSize));
				this.minBubleSize = (float)((double)Math.Min(absoluteSize.Width, absoluteSize.Height) / (100.0 / this.minPossibleBubbleSize));
				if (this.maxAll == this.minAll)
				{
					this.valueScale = 1.0;
					this.valueDiff = this.minAll - (this.maxBubleSize - this.minBubleSize) / 2.0;
				}
				else
				{
					this.valueScale = (double)(this.maxBubleSize - this.minBubleSize) / (this.maxAll - this.minAll);
					this.valueDiff = this.minAll;
				}
				this.scaleDetected = true;
			}
			if (value > this.maxAll)
			{
				return 0f;
			}
			if (value < this.minAll)
			{
				return 0f;
			}
			return (float)((value - this.valueDiff) * this.valueScale) + this.minBubleSize;
		}

		internal static double AxisScaleBubbleSize(ChartGraphics graph, CommonElements common, ChartArea area, double value, bool yValue)
		{
			double num = 1.7976931348623157E+308;
			double num2 = -1.7976931348623157E+308;
			double num3 = 15.0;
			double num4 = 3.0;
			foreach (Series item in common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, "Bubble", StringComparison.OrdinalIgnoreCase) == 0 && item.ChartArea == area.Name && item.IsVisible())
				{
					if (item.IsAttributeSet("BubbleScaleMin"))
					{
						num = Math.Min(num, CommonElements.ParseDouble(((DataPointAttributes)item)["BubbleScaleMin"]));
					}
					if (item.IsAttributeSet("BubbleScaleMax"))
					{
						num2 = Math.Max(num2, CommonElements.ParseDouble(((DataPointAttributes)item)["BubbleScaleMax"]));
					}
					if (item.IsAttributeSet("BubbleMaxSize"))
					{
						num3 = CommonElements.ParseDouble(((DataPointAttributes)item)["BubbleMaxSize"]);
						if (!(num3 < 0.0) && !(num3 > 100.0))
						{
							goto IL_011b;
						}
						throw new ArgumentException(SR.ExceptionCustomAttributeIsNotInRange0to100("BubbleMaxSize"));
					}
					goto IL_011b;
				}
				continue;
				IL_011b:
				if (item.IsAttributeSet("BubbleUseSizeForLabel") && string.Compare(((DataPointAttributes)item)["BubbleUseSizeForLabel"], "true", StringComparison.OrdinalIgnoreCase) == 0)
				{
					break;
				}
			}
			double num5 = 1.7976931348623157E+308;
			double num6 = -1.7976931348623157E+308;
			double num7 = 1.7976931348623157E+308;
			double num8 = -1.7976931348623157E+308;
			foreach (Series item2 in common.DataManager.Series)
			{
				if (string.Compare(item2.ChartTypeName, "Bubble", StringComparison.OrdinalIgnoreCase) == 0 && item2.ChartArea == area.Name && item2.IsVisible())
				{
					foreach (DataPoint point in item2.Points)
					{
						if (!point.Empty)
						{
							num7 = Math.Min(num7, point.YValues[1]);
							num8 = Math.Max(num8, point.YValues[1]);
							if (yValue)
							{
								num5 = Math.Min(num5, point.YValues[0]);
								num6 = Math.Max(num6, point.YValues[0]);
							}
							else
							{
								num5 = Math.Min(num5, point.XValue);
								num6 = Math.Max(num6, point.XValue);
							}
						}
					}
				}
			}
			if (num == 1.7976931348623157E+308)
			{
				num = num7;
			}
			if (num2 == -1.7976931348623157E+308)
			{
				num2 = num8;
			}
			graph.GetAbsoluteSize(area.PlotAreaPosition.GetSize());
			float num9 = (float)((num6 - num5) / (100.0 / num3));
			float num10 = (float)((num6 - num5) / (100.0 / num4));
			double num11;
			double num12;
			if (num2 == num)
			{
				num11 = 1.0;
				num12 = num - (num9 - num10) / 2.0;
			}
			else
			{
				num11 = (double)(num9 - num10) / (num2 - num);
				num12 = num;
			}
			if (value > num2)
			{
				return 0.0;
			}
			if (value < num)
			{
				return 0.0;
			}
			return (double)((float)((value - num12) * num11) + num10);
		}

		internal static double GetBubbleMaxSize(ChartArea area)
		{
			double num = 15.0;
			foreach (Series item in area.Common.DataManager.Series)
			{
				if (string.Compare(item.ChartTypeName, "Bubble", StringComparison.OrdinalIgnoreCase) == 0 && item.ChartArea == area.Name && item.IsVisible() && item.IsAttributeSet("BubbleMaxSize"))
				{
					num = CommonElements.ParseDouble(((DataPointAttributes)item)["BubbleMaxSize"]);
					if (!(num < 0.0) && !(num > 100.0))
					{
						continue;
					}
					throw new ArgumentException(SR.ExceptionCustomAttributeIsNotInRange0to100("BubbleMaxSize"));
				}
			}
			return num / 100.0;
		}
	}
}
