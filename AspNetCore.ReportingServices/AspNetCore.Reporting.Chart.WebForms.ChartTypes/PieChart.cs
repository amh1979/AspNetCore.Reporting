using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class PieChart : IChartType
	{
		private enum LabelsMode
		{
			Off,
			Draw,
			EstimateSize,
			LabelsOverlap
		}

		internal class LabelColumn
		{
			private RectangleF chartAreaPosition;

			private RectangleF innerPlotPosition;

			internal float columnHeight;

			internal int numOfItems;

			private int numOfInsertedLabels;

			private DataPoint[] points;

			private float[] yPositions;

			private bool rightPosition = true;

			private float labelLineSize;

			public LabelColumn(RectangleF position)
			{
				this.chartAreaPosition = position;
			}

			internal int GetLabelIndex(float y)
			{
				if (y < this.chartAreaPosition.Y)
				{
					y = this.chartAreaPosition.Y;
				}
				else if (y > this.chartAreaPosition.Bottom)
				{
					y = this.chartAreaPosition.Bottom - this.columnHeight;
				}
				return (int)((y - this.chartAreaPosition.Y) / this.columnHeight);
			}

			internal void Sort()
			{
				for (int i = 0; i < this.points.Length; i++)
				{
					for (int j = 0; j < i; j++)
					{
						if (this.yPositions[i] < this.yPositions[j] && this.points[i] != null && this.points[j] != null)
						{
							float num = this.yPositions[i];
							DataPoint dataPoint = this.points[i];
							this.yPositions[i] = this.yPositions[j];
							this.points[i] = this.points[j];
							this.yPositions[j] = num;
							this.points[j] = dataPoint;
						}
					}
				}
			}

			internal float GetLabelPosition(int index)
			{
				if (index >= 0 && index <= this.numOfItems - 1)
				{
					return (float)(this.chartAreaPosition.Y + this.columnHeight * (float)index + this.columnHeight / 2.0);
				}
				throw new InvalidOperationException(SR.Exception3DPieLabelsIndexInvalid);
			}

			internal PointF GetLabelPosition(DataPoint dataPoint)
			{
				PointF empty = PointF.Empty;
				int num = 0;
				DataPoint[] array = this.points;
				foreach (DataPoint dataPoint2 in array)
				{
					if (dataPoint2 == dataPoint)
					{
						empty.Y = this.GetLabelPosition(num);
						break;
					}
					num++;
				}
				if (this.rightPosition)
				{
					empty.X = this.innerPlotPosition.Right + this.chartAreaPosition.Width * this.labelLineSize;
				}
				else
				{
					empty.X = this.innerPlotPosition.Left - this.chartAreaPosition.Width * this.labelLineSize;
				}
				float num2 = (float)Math.Atan((empty.Y - this.innerPlotPosition.Top - this.innerPlotPosition.Height / 2.0) / (empty.X - this.innerPlotPosition.Left - this.innerPlotPosition.Width / 2.0));
				float num3 = (float)((Math.Cos((double)num2) != 0.0) ? ((float)((double)this.innerPlotPosition.Width * 0.4 - (double)this.innerPlotPosition.Width * 0.4 / Math.Cos((double)num2))) : 0.0);
				if (this.rightPosition)
				{
					empty.X += num3;
				}
				else
				{
					empty.X -= num3;
				}
				return empty;
			}

			internal void InsertLabel(DataPoint point, float yCoordinate, int pointIndx)
			{
				int labelIndex = this.GetLabelIndex(yCoordinate);
				if (this.points[labelIndex] != null)
				{
					if (pointIndx % 2 == 0)
					{
						if (this.CheckFreeSpace(labelIndex, false))
						{
							this.MoveLabels(labelIndex, false);
						}
						else
						{
							this.MoveLabels(labelIndex, true);
						}
					}
					else if (this.CheckFreeSpace(labelIndex, true))
					{
						this.MoveLabels(labelIndex, true);
					}
					else
					{
						this.MoveLabels(labelIndex, false);
					}
				}
				this.points[labelIndex] = point;
				this.yPositions[labelIndex] = yCoordinate;
				this.numOfInsertedLabels++;
			}

			private void MoveLabels(int position, bool upDirection)
			{
				if (upDirection)
				{
					DataPoint dataPoint = this.points[position];
					float num = this.yPositions[position];
					this.points[position] = null;
					this.yPositions[position] = 0f;
					int num2 = position;
					while (true)
					{
						if (num2 > 0)
						{
							if (this.points[num2 - 1] != null)
							{
								DataPoint dataPoint2 = this.points[num2 - 1];
								float num3 = this.yPositions[num2 - 1];
								this.points[num2 - 1] = dataPoint;
								this.yPositions[num2 - 1] = num;
								dataPoint = dataPoint2;
								num = num3;
								num2--;
								continue;
							}
							break;
						}
						return;
					}
					this.points[num2 - 1] = dataPoint;
					this.yPositions[num2 - 1] = num;
				}
				else
				{
					DataPoint dataPoint3 = this.points[position];
					float num4 = this.yPositions[position];
					this.points[position] = null;
					this.yPositions[position] = 0f;
					int num5 = position;
					while (true)
					{
						if (num5 < this.numOfItems - 1)
						{
							if (this.points[num5 + 1] != null)
							{
								DataPoint dataPoint4 = this.points[num5 + 1];
								float num6 = this.yPositions[num5 + 1];
								this.points[num5 + 1] = dataPoint3;
								this.yPositions[num5 + 1] = num4;
								dataPoint3 = dataPoint4;
								num4 = num6;
								num5++;
								continue;
							}
							break;
						}
						return;
					}
					this.points[num5 + 1] = dataPoint3;
					this.yPositions[num5 + 1] = num4;
				}
			}

			internal void AdjustPositions()
			{
				int num = 0;
				int num2 = 0;
				if (this.numOfInsertedLabels >= this.points.Length / 2)
				{
					for (int i = 0; i < this.points.Length && this.points[i] == null; i++)
					{
						num++;
					}
					int num3 = this.points.Length - 1;
					while (num3 >= 0 && this.points[num3] == null)
					{
						num2++;
						num3--;
					}
					bool flag = (byte)((num > num2) ? 1 : 0) != 0;
					int num4 = (num + num2) / 2;
					if (Math.Abs(num - num2) >= 2)
					{
						if (flag)
						{
							int num5 = 0;
							for (int j = num4; j < this.points.Length; j++)
							{
								if (num + num5 > this.points.Length - 1)
								{
									break;
								}
								this.points[j] = this.points[num + num5];
								this.points[num + num5] = null;
								num5++;
							}
						}
						else
						{
							int num6 = this.points.Length - 1;
							int num7 = this.points.Length - 1 - num4;
							while (num7 >= 0 && num6 - num2 >= 0)
							{
								this.points[num7] = this.points[num6 - num2];
								this.points[num6 - num2] = null;
								num6--;
								num7--;
							}
						}
					}
				}
			}

			private bool CheckFreeSpace(int position, bool upDirection)
			{
				if (upDirection)
				{
					if (position == 0)
					{
						return false;
					}
					for (int num = position - 1; num >= 0; num--)
					{
						if (this.points[num] == null)
						{
							return true;
						}
					}
				}
				else
				{
					if (position == this.numOfItems - 1)
					{
						return false;
					}
					for (int i = position + 1; i < this.numOfItems; i++)
					{
						if (this.points[i] == null)
						{
							return true;
						}
					}
				}
				return false;
			}

			internal void Initialize(RectangleF rectangle, bool rightPosition, int maxNumOfRows, float labelLineSize)
			{
				this.numOfItems = Math.Max(this.numOfItems, maxNumOfRows);
				this.columnHeight = this.chartAreaPosition.Height / (float)this.numOfItems;
				this.innerPlotPosition = rectangle;
				this.points = new DataPoint[this.numOfItems];
				this.yPositions = new float[this.numOfItems];
				this.rightPosition = rightPosition;
				this.labelLineSize = labelLineSize;
			}
		}

		private bool labelsFit = true;

		private float sizeCorrection = 0.95f;

		private bool sliceExploded;

		private bool labelsOverlap;

		internal LabelColumn labelColumnLeft;

		internal LabelColumn labelColumnRight;

		private ArrayList labelsRectangles = new ArrayList();

		public virtual string Name
		{
			get
			{
				return "Pie";
			}
		}

		public virtual bool Stacked
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportStackedGroups
		{
			get
			{
				return false;
			}
		}

		public bool StackSign
		{
			get
			{
				return false;
			}
		}

		public virtual bool RequireAxes
		{
			get
			{
				return false;
			}
		}

		public bool SecondYScale
		{
			get
			{
				return false;
			}
		}

		public bool CircularChartArea
		{
			get
			{
				return false;
			}
		}

		public virtual bool SupportLogarithmicAxes
		{
			get
			{
				return false;
			}
		}

		public virtual bool SwitchValueAxes
		{
			get
			{
				return false;
			}
		}

		public virtual bool SideBySideSeries
		{
			get
			{
				return false;
			}
		}

		public virtual bool ZeroCrossing
		{
			get
			{
				return false;
			}
		}

		public virtual bool DataPointsInLegend
		{
			get
			{
				return true;
			}
		}

		public virtual bool ExtraYValuesConnectedToYAxis
		{
			get
			{
				return false;
			}
		}

		public virtual bool HundredPercent
		{
			get
			{
				return false;
			}
		}

		public virtual bool HundredPercentSupportNegative
		{
			get
			{
				return false;
			}
		}

		public virtual bool ApplyPaletteColorsToPoints
		{
			get
			{
				return true;
			}
		}

		public virtual int YValuesPerPoint
		{
			get
			{
				return 1;
			}
		}

		public virtual bool Doughnut
		{
			get
			{
				return false;
			}
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		internal static void PrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (string.Compare(series.ChartTypeName, "Pie", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(series.ChartTypeName, "Doughnut", StringComparison.OrdinalIgnoreCase) != 0)
			{
				return;
			}
			double num = 0.0;
			if (series.IsAttributeSet("CollectedThreshold"))
			{
				try
				{
					num = double.Parse(((DataPointAttributes)series)["CollectedThreshold"], CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionDoughnutCollectedThresholdInvalidFormat);
				}
				if (num < 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionDoughnutThresholdInvalid);
				}
			}
			if (series.IsAttributeSet("CollectedStyle") && string.Equals(((DataPointAttributes)series)["CollectedStyle"], CollectedPieStyle.SingleSlice.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				if (!series.IsAttributeSet("CollectedThreshold"))
				{
					num = 5.0;
				}
			}
			else
			{
				num = 0.0;
			}
			if (!(num > 0.0))
			{
				return;
			}
			Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
			if (chart == null)
			{
				throw new InvalidOperationException(SR.ExceptionDoughnutNullReference);
			}
			Series series2 = new Series("PIE_ORIGINAL_DATA_" + series.Name, series.YValuesPerPoint);
			series2.Enabled = false;
			series2.ShowInLegend = false;
			chart.Series.Add(series2);
			foreach (DataPoint point in series.Points)
			{
				series2.Points.Add(point.Clone());
			}
			if (series.IsAttributeSet("TempDesignData"))
			{
				((DataPointAttributes)series2)["TempDesignData"] = "true";
			}
			double num2 = 0.0;
			foreach (DataPoint point2 in series.Points)
			{
				if (!point2.Empty)
				{
					num2 += Math.Abs(point2.YValues[0]);
				}
			}
			bool flag = true;
			if (series.IsAttributeSet("CollectedThresholdUsePercent"))
			{
				if (string.Compare(((DataPointAttributes)series)["CollectedThresholdUsePercent"], "True", StringComparison.OrdinalIgnoreCase) != 0)
				{
					if (string.Compare(((DataPointAttributes)series)["CollectedThresholdUsePercent"], "False", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = false;
						goto IL_0255;
					}
					throw new InvalidOperationException(SR.ExceptionDoughnutCollectedThresholdUsePercentInvalid);
				}
				flag = true;
			}
			goto IL_0255;
			IL_0255:
			if (flag)
			{
				if (num > 100.0)
				{
					throw new InvalidOperationException(SR.ExceptionDoughnutCollectedThresholdInvalidRange);
				}
				num = num2 * num / 100.0;
			}
			DataPoint dataPoint3 = null;
			double num3 = 0.0;
			int num4 = 0;
			int index = 0;
			int num5 = 0;
			for (int i = 0; i < series.Points.Count; i++)
			{
				DataPoint dataPoint4 = series.Points[i];
				if (!dataPoint4.Empty && Math.Abs(dataPoint4.YValues[0]) <= num)
				{
					num4++;
					num3 += Math.Abs(dataPoint4.YValues[0]);
					if (dataPoint3 == null)
					{
						index = i;
						dataPoint3 = dataPoint4.Clone();
						dataPoint3.ToolTip = string.Empty;
					}
					if (num4 == 2)
					{
						series.Points.RemoveAt(index);
						i--;
					}
					if (num4 > 1)
					{
						series.Points.RemoveAt(i);
						i--;
					}
				}
				((DataPointAttributes)dataPoint4)["OriginalPointIndex"] = num5.ToString(CultureInfo.InvariantCulture);
				num5++;
			}
			if (num4 > 1 && dataPoint3 != null)
			{
				((DataPointAttributes)dataPoint3)["_COLLECTED_DATA_POINT"] = "TRUE";
				dataPoint3.YValues[0] = num3;
				series.Points.Add(dataPoint3);
				if (series.IsAttributeSet("CollectedColor"))
				{
					ColorConverter colorConverter = new ColorConverter();
					try
					{
						dataPoint3.Color = (Color)colorConverter.ConvertFromString(null, CultureInfo.InvariantCulture, ((DataPointAttributes)series)["CollectedColor"]);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionDoughnutCollectedColorInvalidFormat);
					}
				}
				if (series.IsAttributeSet("CollectedSliceExploded"))
				{
					((DataPointAttributes)dataPoint3)["Exploded"] = ((DataPointAttributes)series)["CollectedSliceExploded"];
				}
				if (series.IsAttributeSet("CollectedToolTip"))
				{
					dataPoint3.ToolTip = ((DataPointAttributes)series)["CollectedToolTip"];
				}
				if (series.IsAttributeSet("CollectedLegendText"))
				{
					dataPoint3.LegendText = ((DataPointAttributes)series)["CollectedLegendText"];
				}
				else
				{
					dataPoint3.LegendText = "Other";
				}
				if (series.IsAttributeSet("CollectedLabel"))
				{
					dataPoint3.Label = ((DataPointAttributes)series)["CollectedLabel"];
				}
			}
		}

		internal static bool UnPrepareData(Series series, IServiceContainer serviceContainer)
		{
			if (series.Name.StartsWith("PIE_ORIGINAL_DATA_", StringComparison.Ordinal))
			{
				Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
				if (chart == null)
				{
					throw new InvalidOperationException(SR.ExceptionDoughnutNullReference);
				}
				Series series2 = chart.Series[series.Name.Substring(18)];
				series2.Points.Clear();
				if (!series.IsAttributeSet("TempDesignData"))
				{
					foreach (DataPoint point in series.Points)
					{
						series2.Points.Add(point);
					}
				}
				chart.Series.Remove(series);
				return true;
			}
			return false;
		}

		public void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			foreach (Series item in common.DataManager.Series)
			{
				if (item.IsVisible() && item.ChartArea == area.Name && string.Compare(item.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) != 0 && !common.ChartPicture.SuppressExceptions)
				{
					throw new InvalidOperationException(SR.ExceptionChartCanNotCombine(this.Name));
				}
			}
			if (area.Area3DStyle.Enable3D)
			{
				float pieWidth = (float)(10 * area.Area3DStyle.PointDepth / 100);
				graph.SetClip(area.Position.ToRectangleF());
				area.Area3DStyle.XAngle *= -1;
				int yAngle = area.Area3DStyle.YAngle;
				area.Area3DStyle.YAngle = area.GetRealYAngle();
				this.ProcessChartType3D(false, graph, common, area, false, LabelsMode.Off, seriesToDraw, pieWidth);
				area.Area3DStyle.XAngle *= -1;
				area.Area3DStyle.YAngle = yAngle;
				graph.ResetClip();
			}
			else
			{
				this.labelsOverlap = false;
				graph.SetClip(area.Position.ToRectangleF());
				this.SizeCorrection(graph, common, area);
				this.ProcessChartType(false, graph, common, area, false, LabelsMode.LabelsOverlap, seriesToDraw);
				if (this.labelsOverlap)
				{
					this.SizeCorrection(graph, common, area);
					this.labelsOverlap = false;
					this.ProcessChartType(false, graph, common, area, false, LabelsMode.LabelsOverlap, seriesToDraw);
				}
				this.ProcessChartType(false, graph, common, area, true, LabelsMode.Off, seriesToDraw);
				this.ProcessChartType(false, graph, common, area, false, LabelsMode.Off, seriesToDraw);
				this.ProcessChartType(false, graph, common, area, false, LabelsMode.Draw, seriesToDraw);
				graph.ResetClip();
			}
		}

		private double MinimumRelativePieSize(ChartArea area)
		{
			double num = 0.3;
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(this.Name);
			SeriesCollection series = area.Common.DataManager.Series;
			if (series[seriesFromChartType[0]].IsAttributeSet("MinimumRelativePieSize"))
			{
				num = (double)CommonElements.ParseFloat(((DataPointAttributes)series[seriesFromChartType[0]])["MinimumRelativePieSize"]) / 100.0;
				if (!(num < 0.1) && !(num > 0.7))
				{
					goto IL_008c;
				}
				throw new ArgumentException(SR.ExceptionPieMinimumRelativePieSizeInvalid);
			}
			goto IL_008c;
			IL_008c:
			return num;
		}

		private void SizeCorrection(ChartGraphics graph, CommonElements common, ChartArea area)
		{
			float num = (float)(this.labelsOverlap ? this.sizeCorrection : 0.949999988079071);
			this.sliceExploded = false;
			if (area.InnerPlotPosition.Auto)
			{
				while (num >= (float)this.MinimumRelativePieSize(area))
				{
					this.sizeCorrection = num;
					this.ProcessChartType(false, graph, common, area, false, LabelsMode.EstimateSize, null);
					if (this.labelsFit)
					{
						break;
					}
					num = (float)(num - 0.05000000074505806);
				}
				if (this.sliceExploded && this.sizeCorrection > 0.800000011920929)
				{
					this.sizeCorrection = 0.8f;
				}
			}
			else
			{
				this.sizeCorrection = 0.95f;
			}
		}

		private void ProcessChartType(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, bool shadow, LabelsMode labels, Series seriesToDraw)
		{
			float num = 0f;
			string text = "";
			SeriesCollection series = common.DataManager.Series;
			if (labels == LabelsMode.LabelsOverlap)
			{
				this.labelsRectangles.Clear();
			}
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(this.Name);
			if (seriesFromChartType.Count != 0)
			{
				if (seriesFromChartType.Count > 0 && series[seriesFromChartType[0]].IsAttributeSet("PieStartAngle"))
				{
					try
					{
						num = float.Parse(((DataPointAttributes)series[seriesFromChartType[0]])["PieStartAngle"], CultureInfo.InvariantCulture);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeAngleOutOfRange("PieStartAngle"));
					}
					if (!(num > 360.0) && !(num < 0.0))
					{
						goto IL_00be;
					}
					throw new InvalidOperationException(SR.ExceptionCustomAttributeAngleOutOfRange("PieStartAngle"));
				}
				goto IL_00be;
			}
			return;
			IL_00be:
			if (!selection)
			{
				common.EventsManager.OnBackPaint(series[seriesFromChartType[0]], new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
			}
			double num2 = 0.0;
			foreach (DataPoint point in series[seriesFromChartType[0]].Points)
			{
				if (!point.Empty)
				{
					num2 += Math.Abs(point.YValues[0]);
				}
			}
			bool explodedShadow;
			float num3;
			if (num2 != 0.0)
			{
				explodedShadow = false;
				foreach (DataPoint point2 in series[seriesFromChartType[0]].Points)
				{
					if (point2.IsAttributeSet("Exploded"))
					{
						text = ((DataPointAttributes)point2)["Exploded"];
						if (string.Compare(text, "true", StringComparison.OrdinalIgnoreCase) == 0)
						{
							explodedShadow = true;
						}
					}
				}
				num3 = 60f;
				if (series[seriesFromChartType[0]].IsAttributeSet("DoughnutRadius"))
				{
					num3 = CommonElements.ParseFloat(((DataPointAttributes)series[seriesFromChartType[0]])["DoughnutRadius"]);
					if (!(num3 < 0.0) && !(num3 > 99.0))
					{
						goto IL_0242;
					}
					throw new ArgumentException(SR.ExceptionPieRadiusInvalid);
				}
				goto IL_0242;
			}
			return;
			IL_0242:
			this.CheckPaleteColors(series[seriesFromChartType[0]].Points);
			int num4 = 0;
			int num5 = 0;
			foreach (DataPoint point3 in series[seriesFromChartType[0]].Points)
			{
				if (point3.Empty)
				{
					num4++;
					continue;
				}
				RectangleF rectangleF = (!area.InnerPlotPosition.Auto) ? new RectangleF(area.PlotAreaPosition.ToRectangleF().X, area.PlotAreaPosition.ToRectangleF().Y, area.PlotAreaPosition.ToRectangleF().Width, area.PlotAreaPosition.ToRectangleF().Height) : new RectangleF(area.Position.ToRectangleF().X, area.Position.ToRectangleF().Y, area.Position.ToRectangleF().Width, area.Position.ToRectangleF().Height);
				if (rectangleF.Width < 0.0 || rectangleF.Height < 0.0)
				{
					return;
				}
				SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(rectangleF.Width, rectangleF.Height));
				float num6 = (absoluteSize.Width < absoluteSize.Height) ? absoluteSize.Width : absoluteSize.Height;
				SizeF relativeSize = graph.GetRelativeSize(new SizeF(num6, num6));
				PointF middlePoint = new PointF((float)(rectangleF.X + rectangleF.Width / 2.0), (float)(rectangleF.Y + rectangleF.Height / 2.0));
				rectangleF = new RectangleF((float)(middlePoint.X - relativeSize.Width / 2.0), (float)(middlePoint.Y - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height);
				if (this.sizeCorrection != 1.0)
				{
					rectangleF.X += (float)(rectangleF.Width * (1.0 - this.sizeCorrection) / 2.0);
					rectangleF.Y += (float)(rectangleF.Height * (1.0 - this.sizeCorrection) / 2.0);
					rectangleF.Width *= this.sizeCorrection;
					rectangleF.Height *= this.sizeCorrection;
					if (area.InnerPlotPosition.Auto)
					{
						RectangleF rectangleF2 = rectangleF;
						rectangleF2.X = (float)((rectangleF2.X - area.Position.X) / area.Position.Width * 100.0);
						rectangleF2.Y = (float)((rectangleF2.Y - area.Position.Y) / area.Position.Height * 100.0);
						rectangleF2.Width = (float)(rectangleF2.Width / area.Position.Width * 100.0);
						rectangleF2.Height = (float)(rectangleF2.Height / area.Position.Height * 100.0);
						area.InnerPlotPosition.SetPositionNoAuto(rectangleF2.X, rectangleF2.Y, rectangleF2.Width, rectangleF2.Height);
					}
				}
				float num7 = (float)(Math.Abs(point3.YValues[0]) / num2 * 360.0);
				bool flag = false;
				if (point3.IsAttributeSet("Exploded"))
				{
					text = ((DataPointAttributes)point3)["Exploded"];
					flag = ((byte)((string.Compare(text, "true", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) != 0);
				}
				Color pieLineColor = Color.Empty;
				ColorConverter colorConverter = new ColorConverter();
				if (point3.IsAttributeSet("PieLineColor") || series[seriesFromChartType[0]].IsAttributeSet("PieLineColor"))
				{
					try
					{
						pieLineColor = (Color)colorConverter.ConvertFromString(point3.IsAttributeSet("PieLineColor") ? ((DataPointAttributes)point3)["PieLineColor"] : ((DataPointAttributes)series[seriesFromChartType[0]])["PieLineColor"]);
					}
					catch
					{
						pieLineColor = (Color)colorConverter.ConvertFromInvariantString(point3.IsAttributeSet("PieLineColor") ? ((DataPointAttributes)point3)["PieLineColor"] : ((DataPointAttributes)series[seriesFromChartType[0]])["PieLineColor"]);
					}
				}
				float num8;
				if (flag)
				{
					this.sliceExploded = true;
					num8 = (float)((2.0 * num + num7) / 2.0);
					double num9 = Math.Cos((double)num8 * 3.1415926535897931 / 180.0) * (double)rectangleF.Width / 10.0;
					double num10 = Math.Sin((double)num8 * 3.1415926535897931 / 180.0) * (double)rectangleF.Height / 10.0;
					rectangleF.Offset((float)num9, (float)num10);
				}
				if (common.ProcessModeRegions && labels == LabelsMode.Draw)
				{
					PieChart.Map(common, point3, num, num7, rectangleF, this.Doughnut, num3, graph, num4);
				}
				if (common.ProcessModePaint)
				{
					if (shadow)
					{
						double num11 = (double)graph.GetRelativeSize(new SizeF((float)point3.series.ShadowOffset, (float)point3.series.ShadowOffset)).Width;
						if (num11 == 0.0)
						{
							break;
						}
						RectangleF rect = new RectangleF(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
						rect.Offset((float)num11, (float)num11);
						Color color = default(Color);
						Color color2 = default(Color);
						Color color3 = default(Color);
						color = ((point3.Color.A == 255) ? point3.series.ShadowColor : Color.FromArgb((int)point3.Color.A / 2, point3.series.ShadowColor));
						color2 = (point3.BackGradientEndColor.IsEmpty ? Color.Empty : ((point3.BackGradientEndColor.A == 255) ? point3.series.ShadowColor : Color.FromArgb((int)point3.BackGradientEndColor.A / 2, point3.series.ShadowColor)));
						color3 = (point3.BorderColor.IsEmpty ? Color.Empty : ((point3.BorderColor.A == 255) ? point3.series.ShadowColor : Color.FromArgb((int)point3.BorderColor.A / 2, point3.series.ShadowColor)));
						graph.StartAnimation();
						graph.DrawPieRel(rect, num, num7, color, ChartHatchStyle.None, "", point3.BackImageMode, point3.BackImageTransparentColor, point3.BackImageAlign, point3.BackGradientType, color2, color3, point3.BorderWidth, point3.BorderStyle, PenAlignment.Inset, true, (double)point3.series.ShadowOffset, this.Doughnut, num3, explodedShadow, PieDrawingStyle.Default);
						graph.StopAnimation();
					}
					else if (labels == LabelsMode.Off)
					{
						graph.StartHotRegion(point3);
						graph.StartAnimation();
						graph.DrawPieRel(rectangleF, num, num7, point3.Color, point3.BackHatchStyle, point3.BackImage, point3.BackImageMode, point3.BackImageTransparentColor, point3.BackImageAlign, point3.BackGradientType, point3.BackGradientEndColor, point3.BorderColor, point3.BorderWidth, point3.BorderStyle, PenAlignment.Inset, false, (double)point3.series.ShadowOffset, this.Doughnut, num3, explodedShadow, ChartGraphics.GetPieDrawingStyle(point3));
						graph.StopAnimation();
						graph.EndHotRegion();
					}
					if (labels == LabelsMode.EstimateSize)
					{
						this.EstimateLabels(graph, middlePoint, rectangleF.Size, num, num7, point3, num3, flag, area);
						if (!this.labelsFit)
						{
							return;
						}
					}
					if (labels == LabelsMode.LabelsOverlap)
					{
						this.DrawLabels(graph, middlePoint, rectangleF.Size, num, num7, point3, num3, flag, area, true, num5, pieLineColor);
					}
					graph.StartAnimation();
					if (labels == LabelsMode.Draw)
					{
						this.DrawLabels(graph, middlePoint, rectangleF.Size, num, num7, point3, num3, flag, area, false, num5, pieLineColor);
					}
					graph.StopAnimation();
				}
				if (common.ProcessModeRegions && labels == LabelsMode.Draw && !common.ProcessModePaint)
				{
					this.DrawLabels(graph, middlePoint, rectangleF.Size, num, num7, point3, num3, flag, area, false, num5, pieLineColor);
				}
				point3.positionRel = new PointF(float.NaN, float.NaN);
				float num12 = 1f;
				if (flag)
				{
					num12 = 1.2f;
				}
				num8 = (float)(num + num7 / 2.0);
				point3.positionRel.X = (float)((float)Math.Cos((double)num8 * 3.1415926535897931 / 180.0) * rectangleF.Width * num12 / 2.0 + middlePoint.X);
				point3.positionRel.Y = (float)((float)Math.Sin((double)num8 * 3.1415926535897931 / 180.0) * rectangleF.Height * num12 / 2.0 + middlePoint.Y);
				num4++;
				num5++;
				num += num7;
				if (num >= 360.0)
				{
					num = (float)(num - 360.0);
				}
			}
			if (labels == LabelsMode.LabelsOverlap && this.labelsOverlap)
			{
				this.labelsOverlap = this.PrepareLabels(area.Position.ToRectangleF());
			}
			if (!selection)
			{
				common.EventsManager.OnPaint(series[seriesFromChartType[0]], new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
			}
		}

		public void DrawLabels(ChartGraphics graph, PointF middlePoint, SizeF relativeSize, float startAngle, float sweepAngle, DataPoint point, float doughnutRadius, bool exploded, ChartArea area, bool overlapTest, int pointIndex, Color pieLineColor)
		{
			bool flag = false;
			float num = 1f;
			float num2 = 1f;
			Region clip = graph.Clip;
			graph.Clip = new Region();
			string labelText = PieChart.GetLabelText(point, false);
			Series series;
			PieLabelStyle pieLabelStyle;
			if (labelText.Length != 0)
			{
				series = point.series;
				pieLabelStyle = PieLabelStyle.Inside;
				if (series.IsAttributeSet("LabelStyle"))
				{
					string strA = ((DataPointAttributes)series)["LabelStyle"];
					pieLabelStyle = (PieLabelStyle)((string.Compare(strA, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
				}
				else if (series.IsAttributeSet("PieLabelStyle"))
				{
					string strA2 = ((DataPointAttributes)series)["PieLabelStyle"];
					pieLabelStyle = (PieLabelStyle)((string.Compare(strA2, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA2, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
				}
				if (point.IsAttributeSet("LabelStyle"))
				{
					string strA3 = ((DataPointAttributes)point)["LabelStyle"];
					pieLabelStyle = (PieLabelStyle)((string.Compare(strA3, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA3, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
				}
				else if (point.IsAttributeSet("PieLabelStyle"))
				{
					string strA4 = ((DataPointAttributes)point)["PieLabelStyle"];
					pieLabelStyle = (PieLabelStyle)((string.Compare(strA4, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA4, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
				}
				if (series.IsAttributeSet("LabelsRadialLineSize"))
				{
					string stringToParse = ((DataPointAttributes)series)["LabelsRadialLineSize"];
					num2 = CommonElements.ParseFloat(stringToParse);
					if (!(num2 < 0.0) && !(num2 > 100.0))
					{
						goto IL_01a3;
					}
					throw new ArgumentException(SR.ExceptionPieRadialLineSizeInvalid, "LabelsRadialLineSize");
				}
				goto IL_01a3;
			}
			return;
			IL_02fb:
			float num5;
			float num3;
			float num6;
			float num4;
			if (this.Doughnut)
			{
				num5 = (float)(relativeSize.Width * num3 / num4 * (1.0 + (100.0 - doughnutRadius) / 100.0));
				num6 = (float)(relativeSize.Height * num3 / num4 * (1.0 + (100.0 - doughnutRadius) / 100.0));
			}
			else
			{
				num5 = relativeSize.Width * num3 / num4;
				num6 = relativeSize.Height * num3 / num4;
			}
			float x = (float)Math.Cos((startAngle + sweepAngle / 2.0) * 3.1415926535897931 / 180.0) * num5 + middlePoint.X;
			float y = (float)Math.Sin((startAngle + sweepAngle / 2.0) * 3.1415926535897931 / 180.0) * num6 + middlePoint.Y;
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			SizeF relativeSize2 = graph.GetRelativeSize(graph.MeasureString(labelText.Replace("\\n", "\n"), point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
			RectangleF empty = RectangleF.Empty;
			SizeF size = new SizeF(relativeSize2.Width, relativeSize2.Height);
			size.Height += (float)(size.Height / 8.0);
			size.Width += size.Width / (float)labelText.Length;
			empty = PointChart.GetLabelPosition(graph, new PointF(x, y), size, stringFormat, true);
			graph.DrawPointLabelStringRel(area.Common, labelText, point.Font, new SolidBrush(point.FontColor), new PointF(x, y), stringFormat, point.FontAngle, empty, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, series, point, pointIndex);
			goto IL_0a7d;
			IL_022f:
			if (point.IsAttributeSet("LabelsHorizontalLineSize"))
			{
				string stringToParse2 = ((DataPointAttributes)point)["LabelsHorizontalLineSize"];
				num = CommonElements.ParseFloat(stringToParse2);
				if (!(num < 0.0) && !(num > 100.0))
				{
					goto IL_0276;
				}
				throw new ArgumentException(SR.ExceptionPieHorizontalLineSizeInvalid, "LabelsHorizontalLineSize");
			}
			goto IL_0276;
			IL_0276:
			num3 = 1f;
			if (pieLabelStyle == PieLabelStyle.Inside && !overlapTest)
			{
				if (exploded)
				{
					num3 = 1.4f;
				}
				num4 = 4f;
				if (point.IsAttributeSet("InsideLabelOffset"))
				{
					num4 = float.Parse(((DataPointAttributes)point)["InsideLabelOffset"], CultureInfo.InvariantCulture);
					if (!(num4 < 0.0) && !(num4 > 100.0))
					{
						num4 = (float)(4.0 / (1.0 + num4 / 100.0));
						goto IL_02fb;
					}
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotInRange0to100("InsideLabelOffset"));
				}
				goto IL_02fb;
			}
			if (pieLabelStyle == PieLabelStyle.Outside)
			{
				float num7 = (float)(0.5 + num2 * 0.10000000149011612);
				if (exploded)
				{
					num3 = 1.2f;
				}
				float num8 = (float)(startAngle + sweepAngle / 2.0);
				float x2 = (float)((float)Math.Cos((double)num8 * 3.1415926535897931 / 180.0) * relativeSize.Width * num3 / 2.0 + middlePoint.X);
				float y2 = (float)((float)Math.Sin((double)num8 * 3.1415926535897931 / 180.0) * relativeSize.Height * num3 / 2.0 + middlePoint.Y);
				float x3 = (float)Math.Cos((double)num8 * 3.1415926535897931 / 180.0) * relativeSize.Width * num7 * num3 + middlePoint.X;
				float y3 = (float)Math.Sin((double)num8 * 3.1415926535897931 / 180.0) * relativeSize.Height * num7 * num3 + middlePoint.Y;
				if (pieLineColor == Color.Empty)
				{
					pieLineColor = point.BorderColor;
				}
				if (!overlapTest)
				{
					graph.DrawLineRel(pieLineColor, point.BorderWidth, ChartDashStyle.Solid, new PointF(x2, y2), new PointF(x3, y3));
				}
				StringFormat stringFormat2 = new StringFormat();
				stringFormat2.Alignment = StringAlignment.Center;
				stringFormat2.LineAlignment = StringAlignment.Center;
				float y4 = (float)Math.Sin((double)num8 * 3.1415926535897931 / 180.0) * relativeSize.Height * num7 * num3 + middlePoint.Y;
				RectangleF rectangleF = RectangleF.Empty;
				RectangleF labelRect = RectangleF.Empty;
				float x4;
				float num9;
				if (num8 > 90.0 && num8 < 270.0)
				{
					stringFormat2.Alignment = StringAlignment.Far;
					x4 = (float)((0.0 - relativeSize.Width) * num7 * num3 + middlePoint.X - relativeSize.Width / 10.0 * num);
					num9 = (float)((float)Math.Cos((double)num8 * 3.1415926535897931 / 180.0) * relativeSize.Width * num7 * num3 + middlePoint.X - relativeSize.Width / 10.0 * num);
					if (overlapTest)
					{
						x4 = num9;
					}
					bool labelsOverlap2 = this.labelsOverlap;
					rectangleF = this.GetLabelRect(new PointF(num9, y4), area, labelText, stringFormat2, graph, point, true);
					labelRect = this.GetLabelRect(new PointF(x4, y4), area, labelText, stringFormat2, graph, point, true);
				}
				else
				{
					stringFormat2.Alignment = StringAlignment.Near;
					x4 = (float)(relativeSize.Width * num7 * num3 + middlePoint.X + relativeSize.Width / 10.0 * num);
					num9 = (float)((float)Math.Cos((double)num8 * 3.1415926535897931 / 180.0) * relativeSize.Width * num7 * num3 + middlePoint.X + relativeSize.Width / 10.0 * num);
					if (overlapTest)
					{
						x4 = num9;
					}
					bool labelsOverlap3 = this.labelsOverlap;
					rectangleF = this.GetLabelRect(new PointF(num9, y4), area, labelText, stringFormat2, graph, point, false);
					labelRect = this.GetLabelRect(new PointF(x4, y4), area, labelText, stringFormat2, graph, point, false);
				}
				if (!overlapTest)
				{
					if (this.labelsOverlap)
					{
						float y5 = (float)((((RectangleF)this.labelsRectangles[pointIndex]).Top + ((RectangleF)this.labelsRectangles[pointIndex]).Bottom) / 2.0);
						graph.DrawLineRel(pieLineColor, point.BorderWidth, ChartDashStyle.Solid, new PointF(x3, y3), new PointF(x4, y5));
					}
					else
					{
						graph.DrawLineRel(pieLineColor, point.BorderWidth, ChartDashStyle.Solid, new PointF(x3, y3), new PointF(num9, y4));
					}
				}
				if (!overlapTest)
				{
					RectangleF position = new RectangleF(rectangleF.Location, rectangleF.Size);
					if (this.labelsOverlap)
					{
						position = (RectangleF)this.labelsRectangles[pointIndex];
						position.X = labelRect.X;
						position.Width = labelRect.Width;
					}
					SizeF sizeF = graph.MeasureStringRel(labelText.Replace("\\n", "\n"), point.Font);
					sizeF.Height += (float)(sizeF.Height / 8.0);
					float num10 = (float)(sizeF.Width / (float)labelText.Length / 2.0);
					sizeF.Width += num10;
					RectangleF backPosition = new RectangleF(position.X, (float)(position.Y + position.Height / 2.0 - sizeF.Height / 2.0), sizeF.Width, sizeF.Height);
					if (stringFormat2.Alignment == StringAlignment.Near)
					{
						backPosition.X -= (float)(num10 / 2.0);
					}
					else if (stringFormat2.Alignment == StringAlignment.Center)
					{
						backPosition.X = (float)(position.X + (position.Width - sizeF.Width) / 2.0);
					}
					else if (stringFormat2.Alignment == StringAlignment.Far)
					{
						backPosition.X = (float)(position.Right - sizeF.Width - num10 / 2.0);
					}
					graph.DrawPointLabelStringRel(area.Common, labelText, point.Font, new SolidBrush(point.FontColor), position, stringFormat2, point.FontAngle, backPosition, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, series, point, pointIndex);
				}
				else
				{
					this.InsertOverlapLabel(labelRect);
					flag = true;
				}
			}
			goto IL_0a7d;
			IL_01a3:
			if (point.IsAttributeSet("LabelsRadialLineSize"))
			{
				string stringToParse3 = ((DataPointAttributes)point)["LabelsRadialLineSize"];
				num2 = CommonElements.ParseFloat(stringToParse3);
				if (!(num2 < 0.0) && !(num2 > 100.0))
				{
					goto IL_01ea;
				}
				throw new ArgumentException(SR.ExceptionPieRadialLineSizeInvalid, "LabelsRadialLineSize");
			}
			goto IL_01ea;
			IL_01ea:
			if (series.IsAttributeSet("LabelsHorizontalLineSize"))
			{
				string stringToParse4 = ((DataPointAttributes)series)["LabelsHorizontalLineSize"];
				num = CommonElements.ParseFloat(stringToParse4);
				if (!(num < 0.0) && !(num > 100.0))
				{
					goto IL_022f;
				}
				throw new ArgumentException(SR.ExceptionPieHorizontalLineSizeInvalid, "LabelsHorizontalLineSize");
			}
			goto IL_022f;
			IL_0a7d:
			graph.Clip = clip;
			if (!flag)
			{
				this.InsertOverlapLabel(RectangleF.Empty);
			}
		}

		private RectangleF GetLabelRect(PointF labelPosition, ChartArea area, string text, StringFormat format, ChartGraphics graph, DataPoint point, bool leftOrientation)
		{
			RectangleF empty = RectangleF.Empty;
			if (leftOrientation)
			{
				empty.X = area.Position.X;
				empty.Y = area.Position.Y;
				empty.Width = labelPosition.X - area.Position.X;
				empty.Height = area.Position.Height;
			}
			else
			{
				empty.X = labelPosition.X;
				empty.Y = area.Position.Y;
				empty.Width = area.Position.Right() - labelPosition.X;
				empty.Height = area.Position.Height;
			}
			SizeF sizeF = graph.MeasureStringRel(text.Replace("\\n", "\n"), point.Font, empty.Size, format);
			empty.Y = (float)(labelPosition.Y - sizeF.Height / 2.0 * 1.7999999523162842);
			empty.Height = (float)(sizeF.Height * 1.7999999523162842);
			return empty;
		}

		private PieLabelStyle GetLabelStyle(DataPoint point)
		{
			Series series = point.series;
			PieLabelStyle result = PieLabelStyle.Inside;
			if (series.IsAttributeSet("LabelStyle"))
			{
				string strA = ((DataPointAttributes)series)["LabelStyle"];
				result = (PieLabelStyle)((string.Compare(strA, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
			}
			else if (series.IsAttributeSet("PieLabelStyle"))
			{
				string strA2 = ((DataPointAttributes)series)["PieLabelStyle"];
				result = (PieLabelStyle)((string.Compare(strA2, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA2, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
			}
			if (point.IsAttributeSet("LabelStyle"))
			{
				string strA3 = ((DataPointAttributes)point)["LabelStyle"];
				result = (PieLabelStyle)((string.Compare(strA3, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA3, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
			}
			else if (point.IsAttributeSet("PieLabelStyle"))
			{
				string strA4 = ((DataPointAttributes)point)["PieLabelStyle"];
				result = (PieLabelStyle)((string.Compare(strA4, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA4, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
			}
			return result;
		}

		public bool EstimateLabels(ChartGraphics graph, PointF middlePoint, SizeF relativeSize, float startAngle, float sweepAngle, DataPoint point, float doughnutRadius, bool exploded, ChartArea area)
		{
			float num = 1f;
			float num2 = 1f;
			string pointLabel = PieChart.GetPointLabel(point, false);
			Series series = point.series;
			PieLabelStyle pieLabelStyle = PieLabelStyle.Inside;
			if (series.IsAttributeSet("LabelStyle"))
			{
				string strA = ((DataPointAttributes)series)["LabelStyle"];
				pieLabelStyle = (PieLabelStyle)((string.Compare(strA, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
			}
			else if (series.IsAttributeSet("PieLabelStyle"))
			{
				string strA2 = ((DataPointAttributes)series)["PieLabelStyle"];
				pieLabelStyle = (PieLabelStyle)((string.Compare(strA2, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA2, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
			}
			if (point.IsAttributeSet("LabelStyle"))
			{
				string strA3 = ((DataPointAttributes)point)["LabelStyle"];
				pieLabelStyle = (PieLabelStyle)((string.Compare(strA3, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA3, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
			}
			else if (point.IsAttributeSet("PieLabelStyle"))
			{
				string strA4 = ((DataPointAttributes)point)["PieLabelStyle"];
				pieLabelStyle = (PieLabelStyle)((string.Compare(strA4, "disabled", StringComparison.OrdinalIgnoreCase) != 0) ? ((string.Compare(strA4, "outside", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) : 2);
			}
			if (series.IsAttributeSet("LabelsRadialLineSize"))
			{
				string stringToParse = ((DataPointAttributes)series)["LabelsRadialLineSize"];
				num2 = CommonElements.ParseFloat(stringToParse);
				if (!(num2 < 0.0) && !(num2 > 100.0))
				{
					goto IL_0185;
				}
				throw new ArgumentException(SR.ExceptionPieRadialLineSizeInvalid, "LabelsRadialLineSize");
			}
			goto IL_0185;
			IL_0251:
			float num3 = 1f;
			if (pieLabelStyle == PieLabelStyle.Outside)
			{
				float num4 = (float)(0.5 + num2 * 0.10000000149011612);
				if (exploded)
				{
					num3 = 1.2f;
				}
				float num5 = (float)(startAngle + sweepAngle / 2.0);
				double num10 = (float)Math.Cos((double)num5 * 3.1415926535897931 / 180.0) * relativeSize.Width * num3 / 2.0;
				float x = middlePoint.X;
				double num11 = (float)Math.Sin((double)num5 * 3.1415926535897931 / 180.0) * relativeSize.Height * num3 / 2.0;
				float y = middlePoint.Y;
				Math.Cos((double)num5 * 3.1415926535897931 / 180.0);
				float width = relativeSize.Width;
				float x2 = middlePoint.X;
				Math.Sin((double)num5 * 3.1415926535897931 / 180.0);
				float height = relativeSize.Height;
				float y2 = middlePoint.Y;
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				float num6 = (float)Math.Sin((double)num5 * 3.1415926535897931 / 180.0) * relativeSize.Height * num4 * num3 + middlePoint.Y;
				float num7;
				if (num5 > 90.0 && num5 < 270.0)
				{
					num7 = (float)((float)Math.Cos((double)num5 * 3.1415926535897931 / 180.0) * relativeSize.Width * num4 * num3 + middlePoint.X - relativeSize.Width / 10.0 * num);
					stringFormat.Alignment = StringAlignment.Far;
				}
				else
				{
					num7 = (float)((float)Math.Cos((double)num5 * 3.1415926535897931 / 180.0) * relativeSize.Width * num4 * num3 + middlePoint.X + relativeSize.Width / 10.0 * num);
					stringFormat.Alignment = StringAlignment.Near;
				}
				string text;
				if (pointLabel.Length == 0 && point.ShowLabelAsValue)
				{
					text = ValueConverter.FormatValue(series.chart, point, point.YValues[0], point.LabelFormat, point.series.YValueType, ChartElementType.DataPoint);
				}
				else
				{
					text = pointLabel;
					if (series.chart != null && series.chart.LocalizeTextHandler != null)
					{
						text = series.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
					}
				}
				SizeF sizeF = graph.MeasureStringRel(text.Replace("\\n", "\n"), point.Font);
				this.labelsFit = true;
				if (this.labelsOverlap)
				{
					if (num5 > 90.0 && num5 < 270.0)
					{
						float num8 = (float)((0.0 - relativeSize.Width) * num4 * num3 + middlePoint.X - relativeSize.Width / 10.0 * num);
						if (num8 - sizeF.Width < area.Position.X)
						{
							this.labelsFit = false;
						}
					}
					else
					{
						float num9 = (float)(relativeSize.Width * num4 * num3 + middlePoint.X + relativeSize.Width / 10.0 * num);
						if (num9 + sizeF.Width > area.Position.Right())
						{
							this.labelsFit = false;
						}
					}
				}
				else
				{
					if (num5 > 90.0 && num5 < 270.0)
					{
						if (num7 - sizeF.Width < area.PlotAreaPosition.ToRectangleF().Left)
						{
							this.labelsFit = false;
						}
					}
					else if (num7 + sizeF.Width > area.PlotAreaPosition.ToRectangleF().Right)
					{
						this.labelsFit = false;
					}
					if (num5 > 180.0 && num5 < 360.0)
					{
						if (num6 - sizeF.Height / 2.0 < area.PlotAreaPosition.ToRectangleF().Top)
						{
							this.labelsFit = false;
						}
					}
					else if (num6 + sizeF.Height / 2.0 > area.PlotAreaPosition.ToRectangleF().Bottom)
					{
						this.labelsFit = false;
					}
				}
			}
			return true;
			IL_0185:
			if (point.IsAttributeSet("LabelsRadialLineSize"))
			{
				string stringToParse2 = ((DataPointAttributes)point)["LabelsRadialLineSize"];
				num2 = CommonElements.ParseFloat(stringToParse2);
				if (!(num2 < 0.0) && !(num2 > 100.0))
				{
					goto IL_01c9;
				}
				throw new ArgumentException(SR.ExceptionPieRadialLineSizeInvalid, "LabelsRadialLineSize");
			}
			goto IL_01c9;
			IL_01c9:
			if (series.IsAttributeSet("LabelsHorizontalLineSize"))
			{
				string stringToParse3 = ((DataPointAttributes)series)["LabelsHorizontalLineSize"];
				num = CommonElements.ParseFloat(stringToParse3);
				if (!(num < 0.0) && !(num > 100.0))
				{
					goto IL_020d;
				}
				throw new ArgumentException(SR.ExceptionPieHorizontalLineSizeInvalid, "LabelsHorizontalLineSize");
			}
			goto IL_020d;
			IL_020d:
			if (point.IsAttributeSet("LabelsHorizontalLineSize"))
			{
				string stringToParse4 = ((DataPointAttributes)point)["LabelsHorizontalLineSize"];
				num = CommonElements.ParseFloat(stringToParse4);
				if (!(num < 0.0) && !(num > 100.0))
				{
					goto IL_0251;
				}
				throw new ArgumentException(SR.ExceptionPieHorizontalLineSizeInvalid, "LabelsHorizontalLineSize");
			}
			goto IL_0251;
		}

		public static bool CreateMapAreaPath(float startAngle, float sweepAngle, RectangleF rectangle, bool doughnut, float doughnutRadius, ChartGraphics graph, out GraphicsPath path, out float[] coord)
		{
			path = new GraphicsPath();
			RectangleF relative = RectangleF.Empty;
			relative.X = (float)(rectangle.X + rectangle.Width * (1.0 - (100.0 - doughnutRadius) / 100.0) / 2.0);
			relative.Y = (float)(rectangle.Y + rectangle.Height * (1.0 - (100.0 - doughnutRadius) / 100.0) / 2.0);
			relative.Width = (float)(rectangle.Width * (100.0 - doughnutRadius) / 100.0);
			relative.Height = (float)(rectangle.Height * (100.0 - doughnutRadius) / 100.0);
			rectangle = graph.GetAbsoluteRectangle(rectangle);
			path.AddPie(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, startAngle, sweepAngle);
			if (sweepAngle <= 0.0)
			{
				path = null;
				coord = null;
				return false;
			}
			if (doughnut)
			{
				relative = graph.GetAbsoluteRectangle(relative);
				path.AddPie(relative.X, relative.Y, relative.Width, relative.Width, startAngle, sweepAngle);
			}
			path.Flatten(new Matrix(), 1f);
			PointF[] array = new PointF[path.PointCount];
			for (int i = 0; i < path.PointCount; i++)
			{
				array[i] = graph.GetRelativePoint(path.PathPoints[i]);
			}
			coord = new float[path.PointCount * 2];
			for (int j = 0; j < path.PointCount; j++)
			{
				coord[2 * j] = array[j].X;
				coord[2 * j + 1] = array[j].Y;
			}
			return true;
		}

		public static void Map(CommonElements common, DataPoint point, float startAngle, float sweepAngle, RectangleF rectangle, bool doughnut, float doughnutRadius, ChartGraphics graph, int pointIndex)
		{
			GraphicsPath path = default(GraphicsPath);
			float[] coord = default(float[]);
			if (PieChart.CreateMapAreaPath(startAngle, sweepAngle, rectangle, doughnut, doughnutRadius, graph, out path, out coord))
			{
				if (point.IsAttributeSet("_COLLECTED_DATA_POINT"))
				{
					common.HotRegionsList.AddHotRegion(graph, path, false, point.ReplaceKeywords(point.ToolTip), point.ReplaceKeywords(point.Href), point.ReplaceKeywords(point.MapAreaAttributes), point, ChartElementType.DataPoint);
				}
				else
				{
					common.HotRegionsList.AddHotRegion(graph, path, false, coord, point, point.series.Name, pointIndex);
				}
			}
		}

		private void CheckPaleteColors(DataPointCollection points)
		{
			DataPoint dataPoint = points[0];
			DataPoint dataPoint2 = points[points.Count - 1];
			if (dataPoint.tempColorIsSet && dataPoint2.tempColorIsSet && dataPoint.Color == dataPoint2.Color)
			{
				dataPoint2.Color = points[points.Count / 2].Color;
				dataPoint2.tempColorIsSet = true;
			}
		}

		private bool PrepareLabels(RectangleF area)
		{
			float num = (float)(area.X + area.Width / 2.0);
			int num2 = 0;
			int num3 = 0;
			foreach (RectangleF labelsRectangle in this.labelsRectangles)
			{
				if (labelsRectangle.X < num)
				{
					num2++;
				}
				else
				{
					num3++;
				}
			}
			bool flag = true;
			if (num2 > 0)
			{
				double[] array = new double[num2];
				double[] array2 = new double[num2];
				int[] array3 = new int[num2];
				int num4 = 0;
				for (int i = 0; i < this.labelsRectangles.Count; i++)
				{
					RectangleF rectangleF = (RectangleF)this.labelsRectangles[i];
					if (rectangleF.X < num)
					{
						array[num4] = (double)rectangleF.Top;
						array2[num4] = (double)rectangleF.Bottom;
						array3[num4] = i;
						num4++;
					}
				}
				this.SortIntervals(array, array2, array3);
				if (this.ArrangeOverlappingIntervals(array, array2, (double)area.Top, (double)area.Bottom))
				{
					num4 = 0;
					for (int j = 0; j < this.labelsRectangles.Count; j++)
					{
						RectangleF rectangleF2 = (RectangleF)this.labelsRectangles[j];
						if (rectangleF2.X < num)
						{
							rectangleF2.Y = (float)array[num4];
							rectangleF2.Height = (float)(array2[num4] - (double)rectangleF2.Top);
							this.labelsRectangles[array3[num4]] = rectangleF2;
							num4++;
						}
					}
				}
				else
				{
					flag = false;
				}
			}
			bool flag2 = true;
			if (num3 > 0)
			{
				double[] array4 = new double[num3];
				double[] array5 = new double[num3];
				int[] array6 = new int[num3];
				int num5 = 0;
				for (int k = 0; k < this.labelsRectangles.Count; k++)
				{
					RectangleF rectangleF3 = (RectangleF)this.labelsRectangles[k];
					if (rectangleF3.X >= num)
					{
						array4[num5] = (double)rectangleF3.Top;
						array5[num5] = (double)rectangleF3.Bottom;
						array6[num5] = k;
						num5++;
					}
				}
				this.SortIntervals(array4, array5, array6);
				if (this.ArrangeOverlappingIntervals(array4, array5, (double)area.Top, (double)area.Bottom))
				{
					num5 = 0;
					for (int l = 0; l < this.labelsRectangles.Count; l++)
					{
						RectangleF rectangleF4 = (RectangleF)this.labelsRectangles[l];
						if (rectangleF4.X >= num)
						{
							rectangleF4.Y = (float)array4[num5];
							rectangleF4.Height = (float)(array5[num5] - (double)rectangleF4.Top);
							this.labelsRectangles[array6[num5]] = rectangleF4;
							num5++;
						}
					}
				}
				else
				{
					flag2 = false;
				}
			}
			if (flag && flag2)
			{
				return false;
			}
			return true;
		}

		private void SortIntervals(double[] startOfIntervals, double[] endOfIntervals, int[] positinIndex)
		{
			for (int i = 0; i < startOfIntervals.Length; i++)
			{
				for (int j = i; j < startOfIntervals.Length; j++)
				{
					double num = (startOfIntervals[i] + endOfIntervals[i]) / 2.0;
					double num2 = (startOfIntervals[j] + endOfIntervals[j]) / 2.0;
					if (num > num2)
					{
						double num3 = startOfIntervals[i];
						startOfIntervals[i] = startOfIntervals[j];
						startOfIntervals[j] = num3;
						num3 = endOfIntervals[i];
						endOfIntervals[i] = endOfIntervals[j];
						endOfIntervals[j] = num3;
						int num4 = positinIndex[i];
						positinIndex[i] = positinIndex[j];
						positinIndex[j] = num4;
					}
				}
			}
		}

		private void InsertOverlapLabel(RectangleF labelRect)
		{
			if (!labelRect.IsEmpty)
			{
				foreach (RectangleF labelsRectangle in this.labelsRectangles)
				{
					if (labelRect.IntersectsWith(labelsRectangle))
					{
						this.labelsOverlap = true;
					}
				}
			}
			this.labelsRectangles.Add(labelRect);
		}

		private bool ArrangeOverlappingIntervals(double[] startOfIntervals, double[] endOfIntervals, double startArea, double endArea)
		{
			if (startOfIntervals.Length != endOfIntervals.Length)
			{
				throw new InvalidOperationException(SR.ExceptionPieIntervalsInvalid);
			}
			this.ShiftOverlappingIntervals(startOfIntervals, endOfIntervals);
			double num = 0.0;
			for (int i = 0; i < startOfIntervals.Length - 1; i++)
			{
				double num3 = startOfIntervals[i + 1];
				double num4 = endOfIntervals[i];
				num += startOfIntervals[i + 1] - endOfIntervals[i];
			}
			double num2 = endOfIntervals[endOfIntervals.Length - 1] - endArea + (startArea - startOfIntervals[0]);
			if (num2 <= 0.0)
			{
				this.ShiftIntervals(startOfIntervals, endOfIntervals, startArea, endArea);
				return true;
			}
			if (num2 > num)
			{
				return false;
			}
			this.ReduceEmptySpace(startOfIntervals, endOfIntervals, (num - num2) / num);
			this.ShiftIntervals(startOfIntervals, endOfIntervals, startArea, endArea);
			return true;
		}

		private void ReduceEmptySpace(double[] startOfIntervals, double[] endOfIntervals, double reduction)
		{
			for (int i = 0; i < startOfIntervals.Length - 1; i++)
			{
				double num2 = startOfIntervals[i + 1];
				double num3 = endOfIntervals[i];
				double num = startOfIntervals[i + 1] - endOfIntervals[i] - (startOfIntervals[i + 1] - endOfIntervals[i]) * reduction;
				for (int j = i + 1; j < startOfIntervals.Length; j++)
				{
					startOfIntervals[j] -= num;
					endOfIntervals[j] -= num;
				}
			}
		}

		private void ShiftIntervals(double[] startOfIntervals, double[] endOfIntervals, double startArea, double endArea)
		{
			double num = 0.0;
			if (startOfIntervals[0] < startArea)
			{
				num = startArea - startOfIntervals[0];
			}
			else if (endOfIntervals[endOfIntervals.Length - 1] > endArea)
			{
				num = endArea - endOfIntervals[endOfIntervals.Length - 1];
			}
			for (int i = 0; i < startOfIntervals.Length; i++)
			{
				startOfIntervals[i] += num;
				endOfIntervals[i] += num;
			}
		}

		private void ShiftOverlappingIntervals(double[] startOfIntervals, double[] endOfIntervals)
		{
			if (startOfIntervals.Length != endOfIntervals.Length)
			{
				throw new InvalidOperationException(SR.ExceptionPieIntervalsInvalid);
			}
			for (int i = 0; i < startOfIntervals.Length - 1; i++)
			{
				if (endOfIntervals[i] > startOfIntervals[i + 1])
				{
					double num = endOfIntervals[i] - startOfIntervals[i + 1];
					this.SpreadInterval(startOfIntervals, endOfIntervals, i, Math.Floor(num / 2.0));
				}
			}
		}

		private void SpreadInterval(double[] startOfIntervals, double[] endOfIntervals, int splitIndex, double overlapShift)
		{
			endOfIntervals[splitIndex] -= overlapShift;
			startOfIntervals[splitIndex] -= overlapShift;
			endOfIntervals[splitIndex + 1] += overlapShift;
			startOfIntervals[splitIndex + 1] += overlapShift;
			if (splitIndex > 0)
			{
				int num = splitIndex - 1;
				while (num >= 0 && endOfIntervals[num] > startOfIntervals[num + 1] - overlapShift)
				{
					endOfIntervals[num] -= overlapShift;
					startOfIntervals[num] -= overlapShift;
					num--;
				}
			}
			if (splitIndex + 2 < startOfIntervals.Length - 1)
			{
				for (int i = splitIndex + 2; i < startOfIntervals.Length && startOfIntervals[i] > endOfIntervals[i - 1] + overlapShift; i++)
				{
					endOfIntervals[i] += overlapShift;
					startOfIntervals[i] += overlapShift;
				}
			}
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		private void ProcessChartType3D(bool selection, ChartGraphics graph, CommonElements common, ChartArea area, bool shadow, LabelsMode labels, Series seriesToDraw, float pieWidth)
		{
			string text = "";
			SeriesCollection series = common.DataManager.Series;
			ArrayList seriesFromChartType = area.GetSeriesFromChartType(this.Name);
			if (seriesFromChartType.Count != 0)
			{
				if (series[seriesFromChartType[0]].IsAttributeSet("PieStartAngle"))
				{
					try
					{
						int num = int.Parse(((DataPointAttributes)series[seriesFromChartType[0]])["PieStartAngle"], CultureInfo.InvariantCulture);
						if (num > 180 && num <= 360)
						{
							num = -(360 - num);
						}
						area.Area3DStyle.YAngle = num;
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeAngleOutOfRange("PieStartAngle"));
					}
					if (area.Area3DStyle.YAngle <= 180 && area.Area3DStyle.YAngle >= -180)
					{
						goto IL_00e0;
					}
					throw new InvalidOperationException(SR.ExceptionCustomAttributeAngleOutOfRange("PieStartAngle"));
				}
				goto IL_00e0;
			}
			return;
			IL_0251:
			float num2 = 100f;
			if (series[seriesFromChartType[0]].IsAttributeSet("3DLabelLineSize"))
			{
				num2 = CommonElements.ParseFloat(((DataPointAttributes)series[seriesFromChartType[0]])["3DLabelLineSize"]);
				if (!(num2 < 30.0) && !(num2 > 200.0))
				{
					goto IL_02ae;
				}
				throw new ArgumentException(SR.ExceptionPie3DLabelLineSizeInvalid);
			}
			goto IL_02ae;
			IL_02ae:
			num2 = (float)(num2 * 0.10000000149011612 / 100.0);
			this.CheckPaleteColors(series[seriesFromChartType[0]].Points);
			float[] array = default(float[]);
			float[] array2 = default(float[]);
			int[] array3 = default(int[]);
			bool sameBackFront = default(bool);
			DataPoint[] array4 = this.PointOrder(series[seriesFromChartType[0]], area, out array, out array2, out array3, out sameBackFront);
			bool flag3;
			float num11;
			if (array4 != null)
			{
				RectangleF innerPlotRectangle = new RectangleF((float)(area.Position.ToRectangleF().X + 1.0), (float)(area.Position.ToRectangleF().Y + 1.0), (float)(area.Position.ToRectangleF().Width - 2.0), (float)(area.Position.ToRectangleF().Height - 2.0));
				bool flag = false;
				DataPoint[] array5 = array4;
				foreach (DataPoint point in array5)
				{
					if (this.GetLabelStyle(point) == PieLabelStyle.Outside)
					{
						flag = true;
					}
				}
				if (flag)
				{
					this.InitPieSize(graph, area, ref innerPlotRectangle, ref pieWidth, array4, array, array2, array3, series[seriesFromChartType[0]], num2);
				}
				area.matrix3D.Initialize(innerPlotRectangle, pieWidth, (float)area.Area3DStyle.XAngle, 0f, 0f, false);
				area.matrix3D.InitLight(area.Area3DStyle.Light);
				for (int j = 0; j < 5; j++)
				{
					int num3 = 0;
					DataPoint[] array6 = array4;
					foreach (DataPoint dataPoint in array6)
					{
						dataPoint.positionRel = PointF.Empty;
						if (dataPoint.Empty)
						{
							num3++;
						}
						else
						{
							float num4 = array2[num3];
							float num5 = array[num3];
							RectangleF rectangleF = (!area.InnerPlotPosition.Auto) ? new RectangleF(area.PlotAreaPosition.ToRectangleF().X, area.PlotAreaPosition.ToRectangleF().Y, area.PlotAreaPosition.ToRectangleF().Width, area.PlotAreaPosition.ToRectangleF().Height) : new RectangleF(innerPlotRectangle.X, innerPlotRectangle.Y, innerPlotRectangle.Width, innerPlotRectangle.Height);
							SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(rectangleF.Width, rectangleF.Height));
							float num6 = (absoluteSize.Width < absoluteSize.Height) ? absoluteSize.Width : absoluteSize.Height;
							SizeF relativeSize = graph.GetRelativeSize(new SizeF(num6, num6));
							PointF pointF = new PointF((float)(rectangleF.X + rectangleF.Width / 2.0), (float)(rectangleF.Y + rectangleF.Height / 2.0));
							rectangleF = new RectangleF((float)(pointF.X - relativeSize.Width / 2.0), (float)(pointF.Y - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height);
							bool flag2 = false;
							if (dataPoint.IsAttributeSet("Exploded"))
							{
								text = ((DataPointAttributes)dataPoint)["Exploded"];
								flag2 = ((byte)((string.Compare(text, "true", StringComparison.OrdinalIgnoreCase) == 0) ? 1 : 0) != 0);
							}
							float num7 = 1f;
							if (flag3)
							{
								num7 = 0.82f;
								rectangleF.X += (float)(rectangleF.Width * (1.0 - num7) / 2.0);
								rectangleF.Y += (float)(rectangleF.Height * (1.0 - num7) / 2.0);
								rectangleF.Width *= num7;
								rectangleF.Height *= num7;
							}
							if (flag2)
							{
								this.sliceExploded = true;
								float num8 = (float)((2.0 * num5 + num4) / 2.0);
								double num9 = Math.Cos((double)num8 * 3.1415926535897931 / 180.0) * (double)rectangleF.Width / 10.0;
								double num10 = Math.Sin((double)num8 * 3.1415926535897931 / 180.0) * (double)rectangleF.Height / 10.0;
								rectangleF.Offset((float)num9, (float)num10);
							}
							if (area.InnerPlotPosition.Auto)
							{
								RectangleF rectangleF2 = rectangleF;
								rectangleF2.X = (float)((rectangleF2.X - area.Position.X) / area.Position.Width * 100.0);
								rectangleF2.Y = (float)((rectangleF2.Y - area.Position.Y) / area.Position.Height * 100.0);
								rectangleF2.Width = (float)(rectangleF2.Width / area.Position.Width * 100.0);
								rectangleF2.Height = (float)(rectangleF2.Height / area.Position.Height * 100.0);
								area.InnerPlotPosition.SetPositionNoAuto(rectangleF2.X, rectangleF2.Y, rectangleF2.Width, rectangleF2.Height);
							}
							graph.StartHotRegion(dataPoint);
							bool flag4 = false;
							this.Draw3DPie(j, graph, dataPoint, area, rectangleF, num5, num4, num11, pieWidth, selection, ref flag4, sameBackFront, flag2, array3[num3]);
							graph.EndHotRegion();
							if (j == 1 && this.GetLabelStyle(dataPoint) == PieLabelStyle.Outside)
							{
								this.FillPieLabelOutside(graph, area, rectangleF, pieWidth, dataPoint, num5, num4, num3, num11, flag2);
							}
							if (j == 2 && this.GetLabelStyle(dataPoint) == PieLabelStyle.Outside && num3 == 0)
							{
								this.labelColumnLeft.Sort();
								this.labelColumnLeft.AdjustPositions();
								this.labelColumnRight.Sort();
								this.labelColumnRight.AdjustPositions();
							}
							num3++;
						}
					}
				}
				if (!selection)
				{
					common.EventsManager.OnPaint(series[seriesFromChartType[0]], new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
				}
			}
			return;
			IL_00e0:
			if (!selection)
			{
				common.EventsManager.OnBackPaint(series[seriesFromChartType[0]], new ChartPaintEventArgs(graph, common, area.PlotAreaPosition));
			}
			double num12 = 0.0;
			foreach (DataPoint point2 in series[seriesFromChartType[0]].Points)
			{
				if (!point2.Empty)
				{
					num12 += Math.Abs(point2.YValues[0]);
				}
			}
			flag3 = false;
			foreach (DataPoint point3 in series[seriesFromChartType[0]].Points)
			{
				if (point3.IsAttributeSet("Exploded"))
				{
					text = ((DataPointAttributes)point3)["Exploded"];
					if (string.Compare(text, "true", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag3 = true;
					}
				}
			}
			num11 = 60f;
			if (series[seriesFromChartType[0]].IsAttributeSet("DoughnutRadius"))
			{
				num11 = CommonElements.ParseFloat(((DataPointAttributes)series[seriesFromChartType[0]])["DoughnutRadius"]);
				if (!(num11 < 0.0) && !(num11 > 99.0))
				{
					goto IL_0251;
				}
				throw new ArgumentException(SR.ExceptionPieRadiusInvalid);
			}
			goto IL_0251;
		}

		private void Draw3DPie(int turn, ChartGraphics graph, DataPoint point, ChartArea area, RectangleF rectangle, float startAngle, float sweepAngle, float doughnutRadius, float pieWidth, bool selection, ref bool isSelected, bool sameBackFront, bool exploded, int pointIndex)
		{
			CommonElements common = area.Common;
			SolidBrush solidBrush = new SolidBrush(point.Color);
			Color empty = Color.Empty;
			Color color = Color.Empty;
			empty = ((!(point.BorderColor == Color.Empty) || area.Area3DStyle.Light != 0) ? ((!(point.BorderColor == Color.Empty)) ? point.BorderColor : point.Color) : ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.5));
			if (point.BorderColor != Color.Empty || area.Area3DStyle.Light == LightStyle.None)
			{
				color = empty;
			}
			Pen pen = new Pen(empty, (float)point.BorderWidth);
			pen.DashStyle = graph.GetPenStyle(point.BorderStyle);
			Pen pen2 = (!(point.BorderColor == Color.Empty)) ? pen : new Pen(point.Color);
			Pen pen3 = new Pen(color, (float)point.BorderWidth);
			pen3.DashStyle = graph.GetPenStyle(point.BorderStyle);
			PointF[] piePoints = this.GetPiePoints(graph, area, pieWidth, rectangle, startAngle, sweepAngle, true, doughnutRadius, exploded);
			if (piePoints != null)
			{
				point.positionRel.X = piePoints[10].X;
				point.positionRel.Y = piePoints[10].Y;
				point.positionRel = graph.GetRelativePoint(point.positionRel);
				float num = (float)(startAngle + sweepAngle / 2.0);
				float num2 = startAngle + sweepAngle;
				int num3;
				bool flag;
				switch (turn)
				{
				case 0:
					graph.StartAnimation();
					if (!this.Doughnut)
					{
						graph.FillPieSlice(area, point, solidBrush, pen2, piePoints[15], piePoints[6], piePoints[16], piePoints[7], piePoints[9], startAngle, sweepAngle, false, pointIndex);
					}
					else
					{
						graph.FillDoughnutSlice(area, point, solidBrush, pen2, piePoints[15], piePoints[6], piePoints[16], piePoints[7], piePoints[24], piePoints[23], piePoints[9], startAngle, sweepAngle, false, doughnutRadius, pointIndex);
					}
					graph.StopAnimation();
					break;
				case 1:
					graph.StartAnimation();
					if (sameBackFront)
					{
						if (num > -90.0 && num < 90.0)
						{
							goto IL_029c;
						}
						if (num > 270.0 && num < 450.0)
						{
							goto IL_029c;
						}
						if (this.Doughnut)
						{
							this.DrawDoughnutCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, true, true, pointIndex);
						}
						this.DrawPieCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, false, true, pointIndex);
						goto IL_030a;
					}
					if (this.Doughnut)
					{
						this.DrawDoughnutCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, false, false, pointIndex);
					}
					graph.FillPieSides(area, (float)area.Area3DStyle.XAngle, startAngle, sweepAngle, piePoints, solidBrush, pen, this.Doughnut);
					this.DrawPieCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, false, false, pointIndex);
					goto IL_0388;
				case 2:
					graph.StartAnimation();
					if (sameBackFront && sweepAngle > 180.0)
					{
						if (startAngle > -180.0 && startAngle < 0.0)
						{
							goto IL_03d3;
						}
						if (startAngle > 180.0 && startAngle < 360.0)
						{
							goto IL_03d3;
						}
						num3 = 0;
						goto IL_0400;
					}
					goto IL_04c4;
				case 3:
					graph.StartAnimation();
					if (!this.Doughnut)
					{
						graph.FillPieSlice(area, point, solidBrush, pen, piePoints[13], piePoints[4], piePoints[14], piePoints[5], piePoints[8], startAngle, sweepAngle, true, pointIndex);
						graph.FillPieSlice(area, point, solidBrush, pen, piePoints[13], piePoints[4], piePoints[14], piePoints[5], piePoints[8], startAngle, sweepAngle, false, pointIndex);
					}
					else
					{
						graph.FillDoughnutSlice(area, point, solidBrush, pen, piePoints[13], piePoints[4], piePoints[14], piePoints[5], piePoints[22], piePoints[21], piePoints[8], startAngle, sweepAngle, true, doughnutRadius, pointIndex);
						graph.FillDoughnutSlice(area, point, solidBrush, pen, piePoints[13], piePoints[4], piePoints[14], piePoints[5], piePoints[22], piePoints[21], piePoints[8], startAngle, sweepAngle, false, doughnutRadius, pointIndex);
					}
					graph.StopAnimation();
					graph.StartAnimation();
					if (this.GetLabelStyle(point) == PieLabelStyle.Outside)
					{
						if (point.IsAttributeSet("PieLineColor") || (point.series != null && point.series.IsAttributeSet("PieLineColor")))
						{
							ColorConverter colorConverter = new ColorConverter();
							Color color2 = pen.Color;
							try
							{
								if (point.IsAttributeSet("PieLineColor"))
								{
									color2 = (Color)colorConverter.ConvertFromString(((DataPointAttributes)point)["PieLineColor"]);
								}
								else if (point.series != null && point.series.IsAttributeSet("PieLineColor"))
								{
									color2 = (Color)colorConverter.ConvertFromString(((DataPointAttributes)point.series)["PieLineColor"]);
								}
							}
							catch
							{
								if (point.IsAttributeSet("PieLineColor"))
								{
									color2 = (Color)colorConverter.ConvertFromInvariantString(((DataPointAttributes)point)["PieLineColor"]);
								}
								else if (point.series != null && point.series.IsAttributeSet("PieLineColor"))
								{
									color2 = (Color)colorConverter.ConvertFromInvariantString(((DataPointAttributes)point.series)["PieLineColor"]);
								}
							}
							pen = new Pen(color2, pen.Width);
						}
						this.Draw3DOutsideLabels(graph, area, pen, piePoints, point, num, pointIndex);
					}
					graph.StopAnimation();
					break;
				default:
					{
						graph.StartAnimation();
						if (this.GetLabelStyle(point) == PieLabelStyle.Inside)
						{
							this.Draw3DInsideLabels(graph, piePoints, point, pointIndex);
						}
						graph.StopAnimation();
						break;
					}
					IL_0400:
					flag = ((byte)num3 != 0);
					if (area.Area3DStyle.XAngle > 0)
					{
						flag = !flag;
					}
					if (num > -90.0 && num < 90.0)
					{
						goto IL_043c;
					}
					if (num > 270.0 && num < 450.0)
					{
						goto IL_043c;
					}
					if (this.Doughnut && flag && sweepAngle > 300.0)
					{
						this.DrawDoughnutCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, false, true, pointIndex);
					}
					this.DrawPieCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, true, true, pointIndex);
					goto IL_04c4;
					IL_043c:
					if (this.Doughnut && flag && sweepAngle > 300.0)
					{
						this.DrawDoughnutCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, true, true, pointIndex);
					}
					this.DrawPieCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, false, true, pointIndex);
					goto IL_04c4;
					IL_029c:
					if (this.Doughnut)
					{
						this.DrawDoughnutCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, false, true, pointIndex);
					}
					this.DrawPieCurves(graph, area, point, startAngle, sweepAngle, piePoints, solidBrush, pen3, true, true, pointIndex);
					goto IL_030a;
					IL_030a:
					graph.FillPieSides(area, (float)area.Area3DStyle.XAngle, startAngle, sweepAngle, piePoints, solidBrush, pen, this.Doughnut);
					goto IL_0388;
					IL_0388:
					graph.StopAnimation();
					break;
					IL_04c4:
					graph.StopAnimation();
					break;
					IL_03d3:
					num3 = (((num2 > -180.0 && num2 < 0.0) || (num2 > 180.0 && num2 < 360.0)) ? 1 : 0);
					goto IL_0400;
				}
			}
		}

		private PointF[] GetPiePoints(ChartGraphics graph, ChartArea area, float pieWidth, RectangleF rectangle, float startAngle, float sweepAngle, bool relativeCoordinates, float doughnutRadius, bool exploded)
		{
			doughnutRadius = (float)(1.0 - doughnutRadius / 100.0);
			Point3D[] array;
			PointF[] array2;
			if (this.Doughnut)
			{
				array = new Point3D[29];
				array2 = new PointF[29];
			}
			else
			{
				array = new Point3D[17];
				array2 = new PointF[17];
			}
			array[0] = new Point3D((float)(rectangle.X + (float)Math.Cos(3.1415926535897931) * rectangle.Width / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin(3.1415926535897931) * rectangle.Height / 2.0 + rectangle.Height / 2.0), pieWidth);
			array[1] = new Point3D((float)(rectangle.X + (float)Math.Cos(3.1415926535897931) * rectangle.Width / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin(3.1415926535897931) * rectangle.Height / 2.0 + rectangle.Height / 2.0), 0f);
			array[2] = new Point3D((float)(rectangle.X + (float)Math.Cos(0.0) * rectangle.Width / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin(0.0) * rectangle.Height / 2.0 + rectangle.Height / 2.0), pieWidth);
			array[3] = new Point3D((float)(rectangle.X + (float)Math.Cos(0.0) * rectangle.Width / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin(0.0) * rectangle.Height / 2.0 + rectangle.Height / 2.0), 0f);
			array[4] = new Point3D((float)(rectangle.X + (float)Math.Cos((double)startAngle * 3.1415926535897931 / 180.0) * rectangle.Width / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((double)startAngle * 3.1415926535897931 / 180.0) * rectangle.Height / 2.0 + rectangle.Height / 2.0), pieWidth);
			array[5] = new Point3D((float)(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle) * 3.1415926535897931 / 180.0) * rectangle.Width / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle) * 3.1415926535897931 / 180.0) * rectangle.Height / 2.0 + rectangle.Height / 2.0), pieWidth);
			array[6] = new Point3D((float)(rectangle.X + (float)Math.Cos((double)startAngle * 3.1415926535897931 / 180.0) * rectangle.Width / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((double)startAngle * 3.1415926535897931 / 180.0) * rectangle.Height / 2.0 + rectangle.Height / 2.0), 0f);
			array[7] = new Point3D((float)(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle) * 3.1415926535897931 / 180.0) * rectangle.Width / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle) * 3.1415926535897931 / 180.0) * rectangle.Height / 2.0 + rectangle.Height / 2.0), 0f);
			array[8] = new Point3D((float)(rectangle.X + rectangle.Width / 2.0), (float)(rectangle.Y + rectangle.Height / 2.0), pieWidth);
			array[9] = new Point3D((float)(rectangle.X + rectangle.Width / 2.0), (float)(rectangle.Y + rectangle.Height / 2.0), 0f);
			array[10] = new Point3D((float)(rectangle.X + (float)Math.Cos((startAngle + sweepAngle / 2.0) * 3.1415926535897931 / 180.0) * rectangle.Width / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((startAngle + sweepAngle / 2.0) * 3.1415926535897931 / 180.0) * rectangle.Height / 2.0 + rectangle.Height / 2.0), pieWidth);
			float num = (float)((!exploded) ? 1.2999999523162842 : 1.1000000238418579);
			array[11] = new Point3D((float)(rectangle.X + (float)Math.Cos((startAngle + sweepAngle / 2.0) * 3.1415926535897931 / 180.0) * rectangle.Width * num / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((startAngle + sweepAngle / 2.0) * 3.1415926535897931 / 180.0) * rectangle.Height * num / 2.0 + rectangle.Height / 2.0), pieWidth);
			if (this.Doughnut)
			{
				array[12] = new Point3D((float)(rectangle.X + (float)Math.Cos((startAngle + sweepAngle / 2.0) * 3.1415926535897931 / 180.0) * rectangle.Width * (1.0 + doughnutRadius) / 4.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((startAngle + sweepAngle / 2.0) * 3.1415926535897931 / 180.0) * rectangle.Height * (1.0 + doughnutRadius) / 4.0 + rectangle.Height / 2.0), pieWidth);
			}
			else
			{
				array[12] = new Point3D((float)(rectangle.X + (float)Math.Cos((startAngle + sweepAngle / 2.0) * 3.1415926535897931 / 180.0) * rectangle.Width * 0.5 / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((startAngle + sweepAngle / 2.0) * 3.1415926535897931 / 180.0) * rectangle.Height * 0.5 / 2.0 + rectangle.Height / 2.0), pieWidth);
			}
			array[13] = new Point3D(rectangle.X, rectangle.Y, pieWidth);
			array[14] = new Point3D(rectangle.Right, rectangle.Bottom, pieWidth);
			array[15] = new Point3D(rectangle.X, rectangle.Y, 0f);
			array[16] = new Point3D(rectangle.Right, rectangle.Bottom, 0f);
			if (this.Doughnut)
			{
				array[17] = new Point3D((float)(rectangle.X + (float)Math.Cos(3.1415926535897931) * rectangle.Width * doughnutRadius / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin(3.1415926535897931) * rectangle.Height * doughnutRadius / 2.0 + rectangle.Height / 2.0), pieWidth);
				array[18] = new Point3D((float)(rectangle.X + (float)Math.Cos(3.1415926535897931) * rectangle.Width * doughnutRadius / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin(3.1415926535897931) * rectangle.Height * doughnutRadius / 2.0 + rectangle.Height / 2.0), 0f);
				array[19] = new Point3D((float)(rectangle.X + (float)Math.Cos(0.0) * rectangle.Width * doughnutRadius / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin(0.0) * rectangle.Height * doughnutRadius / 2.0 + rectangle.Height / 2.0), pieWidth);
				array[20] = new Point3D((float)(rectangle.X + (float)Math.Cos(0.0) * rectangle.Width * doughnutRadius / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin(0.0) * rectangle.Height * doughnutRadius / 2.0 + rectangle.Height / 2.0), 0f);
				array[21] = new Point3D((float)(rectangle.X + (float)Math.Cos((double)startAngle * 3.1415926535897931 / 180.0) * rectangle.Width * doughnutRadius / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((double)startAngle * 3.1415926535897931 / 180.0) * rectangle.Height * doughnutRadius / 2.0 + rectangle.Height / 2.0), pieWidth);
				array[22] = new Point3D((float)(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle) * 3.1415926535897931 / 180.0) * rectangle.Width * doughnutRadius / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle) * 3.1415926535897931 / 180.0) * rectangle.Height * doughnutRadius / 2.0 + rectangle.Height / 2.0), pieWidth);
				array[23] = new Point3D((float)(rectangle.X + (float)Math.Cos((double)startAngle * 3.1415926535897931 / 180.0) * rectangle.Width * doughnutRadius / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((double)startAngle * 3.1415926535897931 / 180.0) * rectangle.Height * doughnutRadius / 2.0 + rectangle.Height / 2.0), 0f);
				array[24] = new Point3D((float)(rectangle.X + (float)Math.Cos((double)(startAngle + sweepAngle) * 3.1415926535897931 / 180.0) * rectangle.Width * doughnutRadius / 2.0 + rectangle.Width / 2.0), (float)(rectangle.Y + (float)Math.Sin((double)(startAngle + sweepAngle) * 3.1415926535897931 / 180.0) * rectangle.Height * doughnutRadius / 2.0 + rectangle.Height / 2.0), 0f);
				rectangle.Inflate((float)((0.0 - rectangle.Width) * (1.0 - doughnutRadius) / 2.0), (float)((0.0 - rectangle.Height) * (1.0 - doughnutRadius) / 2.0));
				array[25] = new Point3D(rectangle.X, rectangle.Y, pieWidth);
				array[26] = new Point3D(rectangle.Right, rectangle.Bottom, pieWidth);
				array[27] = new Point3D(rectangle.X, rectangle.Y, 0f);
				array[28] = new Point3D(rectangle.Right, rectangle.Bottom, 0f);
			}
			area.matrix3D.TransformPoints(array);
			int num2 = 0;
			Point3D[] array3 = array;
			foreach (Point3D point3D in array3)
			{
				array2[num2] = point3D.PointF;
				if (relativeCoordinates)
				{
					array2[num2] = graph.GetAbsolutePoint(array2[num2]);
				}
				num2++;
			}
			return array2;
		}

		private void DrawPieCurves(ChartGraphics graph, ChartArea area, DataPoint dataPoint, float startAngle, float sweepAngle, PointF[] points, SolidBrush brushWithoutLight, Pen pen, bool rightPosition, bool sameBackFront, int pointIndex)
		{
			Brush brush = (area.Area3DStyle.Light != 0) ? graph.GetGradientBrush(graph.GetAbsoluteRectangle(area.Position.ToRectangleF()), Color.FromArgb(brushWithoutLight.Color.A, 0, 0, 0), brushWithoutLight.Color, GradientType.VerticalCenter) : brushWithoutLight;
			float num = startAngle + sweepAngle;
			if (sweepAngle > 180.0 && this.DrawPieCurvesBigSlice(graph, area, dataPoint, startAngle, sweepAngle, points, brush, pen, rightPosition, sameBackFront, pointIndex))
			{
				return;
			}
			if (startAngle < 180.0 && num > 180.0)
			{
				if (area.Area3DStyle.XAngle < 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[0], points[6], points[1], startAngle, (float)(180.0 - startAngle), pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[0], points[5], points[1], points[7], 180f, (float)(startAngle + sweepAngle - 180.0), pointIndex);
				}
			}
			else if (startAngle < 0.0 && num > 0.0)
			{
				if (area.Area3DStyle.XAngle > 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[2], points[6], points[3], startAngle, (float)(0.0 - startAngle), pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[5], points[3], points[7], 0f, sweepAngle + startAngle, pointIndex);
				}
			}
			else if (startAngle < 360.0 && num > 360.0)
			{
				if (area.Area3DStyle.XAngle > 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[2], points[6], points[3], startAngle, (float)(360.0 - startAngle), pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[5], points[3], points[7], 0f, (float)(num - 360.0), pointIndex);
				}
			}
			else
			{
				if ((!(startAngle < 180.0) || !(startAngle >= 0.0) || area.Area3DStyle.XAngle >= 0) && (!(startAngle < 540.0) || !(startAngle >= 360.0) || area.Area3DStyle.XAngle >= 0) && (!(startAngle >= 180.0) || !(startAngle < 360.0) || area.Area3DStyle.XAngle <= 0))
				{
					if (!(startAngle >= -180.0))
					{
						return;
					}
					if (!(startAngle < 0.0))
					{
						return;
					}
					if (area.Area3DStyle.XAngle <= 0)
					{
						return;
					}
				}
				graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[5], points[6], points[7], startAngle, sweepAngle, pointIndex);
			}
		}

		private bool DrawPieCurvesBigSlice(ChartGraphics graph, ChartArea area, DataPoint dataPoint, float startAngle, float sweepAngle, PointF[] points, Brush brush, Pen pen, bool rightPosition, bool sameBackFront, int pointIndex)
		{
			float num = startAngle + sweepAngle;
			if (area.Area3DStyle.XAngle > 0)
			{
				if (startAngle < 180.0 && num > 360.0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[0], points[3], points[1], 0f, -180f, pointIndex);
					goto IL_05de;
				}
				if (startAngle < 0.0 && num > 180.0)
				{
					if (sameBackFront)
					{
						if (rightPosition)
						{
							graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[0], points[5], points[1], points[7], 180f, (float)(num - 180.0), pointIndex);
						}
						else
						{
							graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[2], points[6], points[3], startAngle, (float)(0.0 - startAngle), pointIndex);
						}
					}
					else
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[2], points[6], points[3], startAngle, (float)(0.0 - startAngle), pointIndex);
						graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[0], points[5], points[1], points[7], 180f, (float)(num - 180.0), pointIndex);
					}
					goto IL_05de;
				}
				return false;
			}
			if (startAngle < 0.0 && num > 180.0)
			{
				graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[0], points[3], points[1], 0f, 180f, pointIndex);
				goto IL_05de;
			}
			if (startAngle < 180.0 && num > 360.0)
			{
				if (sameBackFront)
				{
					if (rightPosition)
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[0], points[6], points[1], startAngle, (float)(180.0 - startAngle), pointIndex);
					}
					else
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[5], points[3], points[7], 0f, (float)(num - 360.0), pointIndex);
					}
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[2], points[5], points[3], points[7], 0f, (float)(num - 360.0), pointIndex);
					graph.FillPieCurve(area, dataPoint, brush, pen, points[13], points[14], points[15], points[16], points[4], points[0], points[6], points[1], startAngle, (float)(180.0 - startAngle), pointIndex);
				}
				goto IL_05de;
			}
			return false;
			IL_05de:
			return true;
		}

		private void DrawDoughnutCurves(ChartGraphics graph, ChartArea area, DataPoint dataPoint, float startAngle, float sweepAngle, PointF[] points, SolidBrush brushWithoutLight, Pen pen, bool rightPosition, bool sameBackFront, int pointIndex)
		{
			Brush brush = (area.Area3DStyle.Light != 0) ? graph.GetGradientBrush(graph.GetAbsoluteRectangle(area.Position.ToRectangleF()), Color.FromArgb(brushWithoutLight.Color.A, 0, 0, 0), brushWithoutLight.Color, GradientType.VerticalCenter) : brushWithoutLight;
			float num = startAngle + sweepAngle;
			if (sweepAngle > 180.0 && this.DrawDoughnutCurvesBigSlice(graph, area, dataPoint, startAngle, sweepAngle, points, brush, pen, rightPosition, sameBackFront, pointIndex))
			{
				return;
			}
			if (startAngle < 180.0 && num > 180.0)
			{
				if (area.Area3DStyle.XAngle > 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[17], points[23], points[18], startAngle, (float)(180.0 - startAngle), pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[17], points[22], points[18], points[24], 180f, (float)(startAngle + sweepAngle - 180.0), pointIndex);
				}
			}
			else if (startAngle < 0.0 && num > 0.0)
			{
				if (area.Area3DStyle.XAngle < 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[19], points[23], points[20], startAngle, (float)(0.0 - startAngle), pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[22], points[20], points[24], 0f, sweepAngle + startAngle, pointIndex);
				}
			}
			else if (startAngle < 360.0 && num > 360.0)
			{
				if (area.Area3DStyle.XAngle < 0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[19], points[23], points[20], startAngle, (float)(360.0 - startAngle), pointIndex);
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[22], points[20], points[24], 0f, (float)(num - 360.0), pointIndex);
				}
			}
			else
			{
				if ((!(startAngle < 180.0) || !(startAngle >= 0.0) || area.Area3DStyle.XAngle <= 0) && (!(startAngle < 540.0) || !(startAngle >= 360.0) || area.Area3DStyle.XAngle <= 0) && (!(startAngle >= 180.0) || !(startAngle < 360.0) || area.Area3DStyle.XAngle >= 0))
				{
					if (!(startAngle >= -180.0))
					{
						return;
					}
					if (!(startAngle < 0.0))
					{
						return;
					}
					if (area.Area3DStyle.XAngle >= 0)
					{
						return;
					}
				}
				graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[22], points[23], points[24], startAngle, sweepAngle, pointIndex);
			}
		}

		private bool DrawDoughnutCurvesBigSlice(ChartGraphics graph, ChartArea area, DataPoint dataPoint, float startAngle, float sweepAngle, PointF[] points, Brush brush, Pen pen, bool rightPosition, bool sameBackFront, int pointIndex)
		{
			float num = startAngle + sweepAngle;
			if (area.Area3DStyle.XAngle < 0)
			{
				if (startAngle < 180.0 && num > 360.0)
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[17], points[20], points[18], 0f, -180f, pointIndex);
					goto IL_0606;
				}
				if (startAngle < 0.0 && num > 180.0)
				{
					if (sameBackFront)
					{
						if (rightPosition)
						{
							graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[17], points[22], points[18], points[24], 180f, (float)(num - 180.0), pointIndex);
						}
						else
						{
							graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[19], points[23], points[20], startAngle, (float)(0.0 - startAngle), pointIndex);
						}
					}
					else
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[19], points[23], points[20], startAngle, (float)(0.0 - startAngle), pointIndex);
						graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[17], points[22], points[18], points[24], 180f, (float)(num - 180.0), pointIndex);
					}
					goto IL_0606;
				}
				return false;
			}
			if (startAngle < 0.0 && num > 180.0)
			{
				graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[17], points[20], points[18], 0f, 180f, pointIndex);
				goto IL_0606;
			}
			if (startAngle < 180.0 && num > 360.0)
			{
				if (sameBackFront)
				{
					if (rightPosition)
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[17], points[23], points[18], startAngle, (float)(180.0 - startAngle), pointIndex);
					}
					else
					{
						graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[22], points[20], points[24], 0f, (float)(num - 360.0), pointIndex);
					}
				}
				else
				{
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[19], points[22], points[20], points[24], 0f, (float)(num - 360.0), pointIndex);
					graph.FillPieCurve(area, dataPoint, brush, pen, points[25], points[26], points[27], points[28], points[21], points[17], points[23], points[18], startAngle, (float)(180.0 - startAngle), pointIndex);
				}
				goto IL_0606;
			}
			return false;
			IL_0606:
			return true;
		}

		private DataPoint[] PointOrder(Series series, ChartArea area, out float[] newStartAngleList, out float[] newSweepAngleList, out int[] newPointIndexList, out bool sameBackFrontPoint)
		{
			int num = -1;
			int num2 = -1;
			sameBackFrontPoint = false;
			double num3 = 0.0;
			int num4 = 0;
			foreach (DataPoint point in series.Points)
			{
				if (point.Empty)
				{
					num4++;
				}
				if (!point.Empty)
				{
					num3 += Math.Abs(point.YValues[0]);
				}
			}
			int num5 = series.Points.Count - num4;
			DataPoint[] array = new DataPoint[num5];
			float[] array2 = new float[num5];
			float[] array3 = new float[num5];
			int[] array4 = new int[num5];
			newStartAngleList = new float[num5];
			newSweepAngleList = new float[num5];
			newPointIndexList = new int[num5];
			if (num3 <= 0.0)
			{
				return null;
			}
			int num6 = 0;
			double num7 = (double)area.Area3DStyle.YAngle;
			foreach (DataPoint point2 in series.Points)
			{
				double num8;
				double num9;
				if (!point2.Empty)
				{
					num8 = (double)(float)(Math.Abs(point2.YValues[0]) * 360.0 / num3);
					num9 = num7 + num8;
					array2[num6] = (float)num7;
					array3[num6] = (float)num8;
					array4[num6] = num6;
					if (num7 <= -90.0 && num9 > -90.0)
					{
						goto IL_0188;
					}
					if (num7 <= 270.0 && num9 > 270.0 && array[0] == null)
					{
						goto IL_0188;
					}
					goto IL_01ae;
				}
				continue;
				IL_0230:
				num6++;
				num7 += num8;
				continue;
				IL_01f5:
				num2 = num6;
				array[array.Length - 1] = point2;
				newStartAngleList[array.Length - 1] = array2[num6];
				newSweepAngleList[array.Length - 1] = array3[num6];
				newPointIndexList[array.Length - 1] = array4[num6];
				goto IL_0230;
				IL_01ae:
				if (num7 <= 90.0 && num9 > 90.0)
				{
					goto IL_01f5;
				}
				if (num7 <= 450.0 && num9 > 450.0 && num2 == -1 && (array[array.Length - 1] == null || array.Length == 1))
				{
					goto IL_01f5;
				}
				goto IL_0230;
				IL_0188:
				num = num6;
				array[0] = point2;
				newStartAngleList[0] = array2[num6];
				newSweepAngleList[0] = array3[num6];
				newPointIndexList[0] = array4[num6];
				goto IL_01ae;
			}
			bool flag;
			if (num2 != -1 && num != -1)
			{
				if (num2 == num && array.Length != 1)
				{
					array[array.Length - 1] = null;
					newStartAngleList[array.Length - 1] = 0f;
					newSweepAngleList[array.Length - 1] = 0f;
					newPointIndexList[array.Length - 1] = 0;
					sameBackFrontPoint = true;
				}
				if (num2 == num)
				{
					float num10 = (float)(array2[num] + array3[num] / 2.0);
					flag = false;
					if (num10 > -90.0 && num10 < 90.0)
					{
						goto IL_02f3;
					}
					if (num10 > 270.0 && num10 < 450.0)
					{
						goto IL_02f3;
					}
					goto IL_02f6;
				}
				if (num2 < num)
				{
					num6 = 0;
					int num11 = 1;
					foreach (DataPoint point3 in series.Points)
					{
						if (!point3.Empty)
						{
							if (num6 == num2 || num6 == num)
							{
								num6++;
							}
							else
							{
								if (num6 > num)
								{
									if (array[num11] != null)
									{
										throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
									}
									array[num11] = point3;
									newStartAngleList[num11] = array2[num6];
									newSweepAngleList[num11] = array3[num6];
									newPointIndexList[num11] = array4[num6];
									num11++;
								}
								num6++;
							}
						}
					}
					num6 = 0;
					foreach (DataPoint point4 in series.Points)
					{
						if (!point4.Empty)
						{
							if (num6 == num2 || num6 == num)
							{
								num6++;
							}
							else
							{
								if (num6 < num2)
								{
									if (array[num11] != null)
									{
										throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
									}
									array[num11] = point4;
									newStartAngleList[num11] = array2[num6];
									newSweepAngleList[num11] = array3[num6];
									newPointIndexList[num11] = array4[num6];
									num11++;
								}
								num6++;
							}
						}
					}
					num11 = array.Length - 2;
					num6 = 0;
					foreach (DataPoint point5 in series.Points)
					{
						if (!point5.Empty)
						{
							if (num6 == num2 || num6 == num)
							{
								num6++;
							}
							else
							{
								if (num6 > num2 && num6 < num)
								{
									if (array[num11] != null)
									{
										throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
									}
									array[num11] = point5;
									newStartAngleList[num11] = array2[num6];
									newSweepAngleList[num11] = array3[num6];
									newPointIndexList[num11] = array4[num6];
									num11--;
								}
								num6++;
							}
						}
					}
				}
				else
				{
					int num12 = 1;
					num6 = 0;
					foreach (DataPoint point6 in series.Points)
					{
						if (!point6.Empty)
						{
							if (num6 == num2 || num6 == num)
							{
								num6++;
							}
							else
							{
								if (num6 > num && num6 < num2)
								{
									if (array[num12] != null)
									{
										throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
									}
									array[num12] = point6;
									newStartAngleList[num12] = array2[num6];
									newSweepAngleList[num12] = array3[num6];
									newPointIndexList[num12] = array4[num6];
									num12++;
								}
								num6++;
							}
						}
					}
					num12 = array.Length - 2;
					num6 = 0;
					foreach (DataPoint point7 in series.Points)
					{
						if (!point7.Empty)
						{
							if (num6 == num2 || num6 == num)
							{
								num6++;
							}
							else
							{
								if (num6 > num2)
								{
									if (array[num12] != null)
									{
										throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
									}
									array[num12] = point7;
									newStartAngleList[num12] = array2[num6];
									newSweepAngleList[num12] = array3[num6];
									newPointIndexList[num12] = array4[num6];
									num12--;
								}
								num6++;
							}
						}
					}
					num6 = 0;
					foreach (DataPoint point8 in series.Points)
					{
						if (!point8.Empty)
						{
							if (num6 == num2 || num6 == num)
							{
								num6++;
							}
							else
							{
								if (num6 < num)
								{
									if (array[num12] != null)
									{
										throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
									}
									array[num12] = point8;
									newStartAngleList[num12] = array2[num6];
									newSweepAngleList[num12] = array3[num6];
									newPointIndexList[num12] = array4[num6];
									num12--;
								}
								num6++;
							}
						}
					}
				}
				goto IL_087d;
			}
			throw new InvalidOperationException(SR.ExceptionPieUnassignedFrontBackPoints);
			IL_02f3:
			flag = true;
			goto IL_02f6;
			IL_02f6:
			int num13 = num5 - num2;
			num6 = 0;
			foreach (DataPoint point9 in series.Points)
			{
				if (!point9.Empty)
				{
					if (num6 == num2)
					{
						num6++;
					}
					else
					{
						if (num6 < num2)
						{
							if (array[num13] != null)
							{
								throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
							}
							array[num13] = point9;
							newStartAngleList[num13] = array2[num6];
							newSweepAngleList[num13] = array3[num6];
							newPointIndexList[num13] = array4[num6];
							num13++;
						}
						num6++;
					}
				}
			}
			num6 = 0;
			num13 = 1;
			foreach (DataPoint point10 in series.Points)
			{
				if (!point10.Empty)
				{
					if (num6 == num2)
					{
						num6++;
					}
					else
					{
						if (num6 > num2)
						{
							if (array[num13] != null)
							{
								throw new InvalidOperationException(SR.ExceptionPiePointOrderInvalid);
							}
							array[num13] = point10;
							newStartAngleList[num13] = array2[num6];
							newSweepAngleList[num13] = array3[num6];
							newPointIndexList[num13] = array4[num6];
							num13++;
						}
						num6++;
					}
				}
			}
			if (flag)
			{
				this.SwitchPoints(num5, ref array, ref newStartAngleList, ref newSweepAngleList, ref newPointIndexList, num == num2);
			}
			goto IL_087d;
			IL_087d:
			if (area.Area3DStyle.XAngle > 0)
			{
				this.SwitchPoints(num5, ref array, ref newStartAngleList, ref newSweepAngleList, ref newPointIndexList, num == num2);
			}
			return array;
		}

		private void SwitchPoints(int numOfPoints, ref DataPoint[] points, ref float[] newStartAngleList, ref float[] newSweepAngleList, ref int[] newPointIndexList, bool sameBackFront)
		{
			float[] array = new float[numOfPoints];
			float[] array2 = new float[numOfPoints];
			int[] array3 = new int[numOfPoints];
			DataPoint[] array4 = new DataPoint[numOfPoints];
			int num = 0;
			if (sameBackFront)
			{
				num = 1;
				array4[0] = points[0];
				array[0] = newStartAngleList[0];
				array2[0] = newSweepAngleList[0];
				array3[0] = newPointIndexList[0];
			}
			for (int i = num; i < numOfPoints; i++)
			{
				if (points[i] == null)
				{
					throw new InvalidOperationException(SR.ExceptionPieOrderOperationInvalid);
				}
				array4[numOfPoints - i - 1 + num] = points[i];
				array[numOfPoints - i - 1 + num] = newStartAngleList[i];
				array2[numOfPoints - i - 1 + num] = newSweepAngleList[i];
				array3[numOfPoints - i - 1 + num] = newPointIndexList[i];
			}
			points = array4;
			newStartAngleList = array;
			newSweepAngleList = array2;
			newPointIndexList = array3;
		}

		private void InitPieSize(ChartGraphics graph, ChartArea area, ref RectangleF pieRectangle, ref float pieWidth, DataPoint[] dataPoints, float[] startAngleList, float[] sweepAngleList, int[] pointIndexList, Series series, float labelLineSize)
		{
			this.labelColumnLeft = new LabelColumn(area.Position.ToRectangleF());
			this.labelColumnRight = new LabelColumn(area.Position.ToRectangleF());
			float num = -3.40282347E+38f;
			float num2 = -3.40282347E+38f;
			int num3 = 0;
			foreach (DataPoint dataPoint in dataPoints)
			{
				if (!dataPoint.Empty)
				{
					float num4 = (float)(startAngleList[num3] + sweepAngleList[num3] / 2.0);
					if (num4 >= -90.0 && num4 < 90.0)
					{
						goto IL_008c;
					}
					if (num4 >= 270.0 && num4 < 450.0)
					{
						goto IL_008c;
					}
					this.labelColumnLeft.numOfItems++;
					goto IL_00b4;
				}
				continue;
				IL_008c:
				this.labelColumnRight.numOfItems++;
				goto IL_00b4;
				IL_00b4:
				SizeF sizeF = graph.MeasureStringRel(PieChart.GetLabelText(dataPoint, false).Replace("\\n", "\n"), dataPoint.Font);
				num = Math.Max(sizeF.Width, num);
				num2 = Math.Max(sizeF.Height, num2);
				num3++;
			}
			float width = pieRectangle.Width;
			float height = pieRectangle.Height;
			pieRectangle.Width = (float)(pieRectangle.Width - 2.0 * num - 2.0 * pieRectangle.Width * labelLineSize);
			pieRectangle.Height -= (float)(pieRectangle.Height * 0.30000001192092896);
			if (pieRectangle.Width < width * (float)this.MinimumRelativePieSize(area))
			{
				pieRectangle.Width = width * (float)this.MinimumRelativePieSize(area);
			}
			if (pieRectangle.Height < height * (float)this.MinimumRelativePieSize(area))
			{
				pieRectangle.Height = height * (float)this.MinimumRelativePieSize(area);
			}
			if (width * 0.800000011920929 < pieRectangle.Width)
			{
				pieRectangle.Width *= 0.8f;
			}
			pieRectangle.X += (float)((width - pieRectangle.Width) / 2.0);
			pieWidth = pieRectangle.Width / width * pieWidth;
			pieRectangle.Y += (float)((height - pieRectangle.Height) / 2.0);
			SizeF size = new SizeF((float)(1.3999999761581421 * series.Font.Size), (float)(1.3999999761581421 * series.Font.Size));
			size = graph.GetRelativeSize(size);
			int maxNumOfRows = (int)(pieRectangle.Height / num2);
			this.labelColumnRight.Initialize(pieRectangle, true, maxNumOfRows, labelLineSize);
			this.labelColumnLeft.Initialize(pieRectangle, false, maxNumOfRows, labelLineSize);
		}

		private void FillPieLabelOutside(ChartGraphics graph, ChartArea area, RectangleF pieRectangle, float pieWidth, DataPoint point, float startAngle, float sweepAngle, int pointIndx, float doughnutRadius, bool exploded)
		{
			float num = (float)(startAngle + sweepAngle / 2.0);
			PointF[] piePoints = this.GetPiePoints(graph, area, pieWidth, pieRectangle, startAngle, sweepAngle, false, doughnutRadius, exploded);
			float y = piePoints[11].Y;
			if (num >= -90.0 && num < 90.0)
			{
				goto IL_004f;
			}
			if (num >= 270.0 && num < 450.0)
			{
				goto IL_004f;
			}
			this.labelColumnLeft.InsertLabel(point, y, pointIndx);
			return;
			IL_004f:
			this.labelColumnRight.InsertLabel(point, y, pointIndx);
		}

		private void Draw3DOutsideLabels(ChartGraphics graph, ChartArea area, Pen pen, PointF[] points, DataPoint point, float midAngle, int pointIndex)
		{
			string labelText = PieChart.GetLabelText(point, false);
			StringFormat stringFormat;
			RectangleF absoluteRectangle;
			RectangleF rectangleF;
			PointF absolutePoint;
			LabelColumn labelColumn;
			if (labelText.Length != 0)
			{
				graph.DrawLine(pen, points[10], points[11]);
				stringFormat = new StringFormat();
				stringFormat.LineAlignment = StringAlignment.Center;
				absoluteRectangle = graph.GetAbsoluteRectangle(area.Position.ToRectangleF());
				rectangleF = RectangleF.Empty;
				if (midAngle >= -90.0 && midAngle < 90.0)
				{
					goto IL_0085;
				}
				if (midAngle >= 270.0 && midAngle < 450.0)
				{
					goto IL_0085;
				}
				labelColumn = this.labelColumnLeft;
				stringFormat.Alignment = StringAlignment.Far;
				float height = graph.GetAbsoluteSize(new SizeF(0f, this.labelColumnLeft.columnHeight)).Height;
				absolutePoint = graph.GetAbsolutePoint(labelColumn.GetLabelPosition(point));
				if (points[11].X < absolutePoint.X)
				{
					absolutePoint.X = (float)(points[11].X - 10.0);
				}
				rectangleF.X = absoluteRectangle.X;
				rectangleF.Width = absolutePoint.X - rectangleF.X;
				rectangleF.Y = (float)(absolutePoint.Y - height / 2.0);
				rectangleF.Height = height;
				goto IL_01fe;
			}
			return;
			IL_01fe:
			stringFormat.FormatFlags = (StringFormatFlags.NoWrap | StringFormatFlags.LineLimit);
			stringFormat.Trimming = StringTrimming.EllipsisWord;
			graph.DrawLine(pen, points[11], absolutePoint);
			rectangleF = graph.GetRelativeRectangle(rectangleF);
			SizeF sizeF = graph.MeasureStringRel(labelText.Replace("\\n", "\n"), point.Font);
			sizeF.Height += (float)(sizeF.Height / 8.0);
			float num = (float)(sizeF.Width / (float)labelText.Length / 2.0);
			sizeF.Width += num;
			RectangleF backPosition = new RectangleF(rectangleF.X, (float)(rectangleF.Y + rectangleF.Height / 2.0 - sizeF.Height / 2.0), sizeF.Width, sizeF.Height);
			if (stringFormat.Alignment == StringAlignment.Near)
			{
				backPosition.X -= (float)(num / 2.0);
			}
			else if (stringFormat.Alignment == StringAlignment.Center)
			{
				backPosition.X = (float)(rectangleF.X + (rectangleF.Width - sizeF.Width) / 2.0);
			}
			else if (stringFormat.Alignment == StringAlignment.Far)
			{
				backPosition.X = (float)(rectangleF.Right - sizeF.Width - num / 2.0);
			}
			graph.DrawPointLabelStringRel(graph.common, labelText, point.Font, new SolidBrush(point.FontColor), rectangleF, stringFormat, 0, backPosition, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, point.series, point, pointIndex);
			return;
			IL_0085:
			labelColumn = this.labelColumnRight;
			stringFormat.Alignment = StringAlignment.Near;
			float height2 = graph.GetAbsoluteSize(new SizeF(0f, this.labelColumnRight.columnHeight)).Height;
			absolutePoint = graph.GetAbsolutePoint(labelColumn.GetLabelPosition(point));
			if (points[11].X > absolutePoint.X)
			{
				absolutePoint.X = (float)(points[11].X + 10.0);
			}
			rectangleF.X = absolutePoint.X;
			rectangleF.Width = absoluteRectangle.Right - rectangleF.X;
			rectangleF.Y = (float)(absolutePoint.Y - height2 / 2.0);
			rectangleF.Height = height2;
			goto IL_01fe;
		}

		private void Draw3DInsideLabels(ChartGraphics graph, PointF[] points, DataPoint point, int pointIndex)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.LineAlignment = StringAlignment.Center;
			stringFormat.Alignment = StringAlignment.Center;
			string labelText = PieChart.GetLabelText(point, false);
			PointF relativePoint = graph.GetRelativePoint(points[12]);
			SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(labelText.Replace("\\n", "\n"), point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
			RectangleF empty = RectangleF.Empty;
			SizeF sizeF = new SizeF(relativeSize.Width, relativeSize.Height);
			sizeF.Height += (float)(relativeSize.Height / 8.0);
			sizeF.Width += sizeF.Width / (float)labelText.Length;
			empty = new RectangleF((float)(relativePoint.X - sizeF.Width / 2.0), (float)(relativePoint.Y - sizeF.Height / 2.0 - relativeSize.Height / 10.0), sizeF.Width, sizeF.Height);
			graph.DrawPointLabelStringRel(graph.common, labelText, point.Font, new SolidBrush(point.FontColor), relativePoint, stringFormat, 0, empty, point.LabelBackColor, point.LabelBorderColor, point.LabelBorderWidth, point.LabelBorderStyle, point.series, point, pointIndex);
		}

		private static string GetPointLabel(DataPoint point, bool alwaysIncludeAxisLabel = false)
		{
			string empty = string.Empty;
			if (point.Label.Length == 0)
			{
				empty = point.AxisLabel;
				if (point.series != null && point.series.IsAttributeSet("AutoAxisLabels") && string.Equals(point.series.GetAttribute("AutoAxisLabels"), "false", StringComparison.OrdinalIgnoreCase) && !alwaysIncludeAxisLabel)
				{
					empty = string.Empty;
				}
			}
			else
			{
				empty = point.Label;
			}
			return point.ReplaceKeywords(empty);
		}

		internal static string GetLabelText(DataPoint point, bool alwaysIncludeAxisLabel = false)
		{
			string pointLabel = PieChart.GetPointLabel(point, alwaysIncludeAxisLabel);
			string text;
			if (point.Label.Length == 0 && point.ShowLabelAsValue)
			{
				text = ValueConverter.FormatValue(point.series.chart, point, point.YValues[0], point.LabelFormat, point.series.YValueType, ChartElementType.DataPoint);
			}
			else
			{
				text = pointLabel;
				if (point.series.chart != null && point.series.chart.LocalizeTextHandler != null)
				{
					text = point.series.chart.LocalizeTextHandler(point, text, point.ElementId, ChartElementType.DataPoint);
				}
			}
			return text;
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
