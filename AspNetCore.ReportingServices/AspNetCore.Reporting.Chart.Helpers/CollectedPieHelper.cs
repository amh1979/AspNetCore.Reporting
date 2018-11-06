using AspNetCore.Reporting.Chart.WebForms;
using AspNetCore.Reporting.Chart.WebForms.Data;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;

namespace AspNetCore.Reporting.Chart.Helpers
{
	internal class CollectedPieHelper
	{
		public double CollectedPercentage = 5.0;

		protected RectangleF ChartAreaPosition = new RectangleF(5f, 5f, 90f, 90f);

		public bool ShowCollectedDataAsOneSlice;

		public Color SliceColor = Color.Empty;

		public float ChartAreaSpacing = 5f;

		public float SupplementedAreaSizeRatio = 0.9f;

		public Color ConnectionLinesColor = Color.FromArgb(64, 64, 64);

		public string CollectedLabel = "Other";

		public bool ShowCollectedLegend;

		public bool ShowCollectedPointLabels;

		private AspNetCore.Reporting.Chart.WebForms.Chart chartControl;

		private Series series;

		private Series supplementalSeries;

		private ChartArea originalChartArea;

		private ChartArea supplementalChartArea;

		private float collectedPieSliceAngle;

		private bool ignorePaintEvent = true;

		public CollectedPieHelper(AspNetCore.Reporting.Chart.WebForms.Chart chartControl)
		{
			this.chartControl = chartControl;
			this.chartControl.PostPaint += this.chart_PostPaint;
		}

		public void ShowSmallSegmentsAsSupplementalPie(Series collectedSeries)
		{
			this.series = collectedSeries;
			if (this.chartControl == null)
			{
				throw new ArgumentNullException("chartControl");
			}
			if (!(this.CollectedPercentage > 100.0) && !(this.CollectedPercentage < 0.0))
			{
				if (this.series.ChartType != SeriesChartType.Pie && this.series.ChartType != SeriesChartType.Doughnut)
				{
					throw new InvalidOperationException("Only series with Pie or Doughnut chart type can be used.");
				}
				if (this.series.Points.Count == 0)
				{
					throw new InvalidOperationException("Cannot perform operatiuon on an empty series.");
				}
				this.supplementalChartArea = null;
				this.ignorePaintEvent = true;
				if (this.CreateCollectedPie())
				{
					float num = (float)((this.ChartAreaPosition.Width - this.ChartAreaSpacing) / 2.0 * this.SupplementedAreaSizeRatio);
					this.originalChartArea = this.chartControl.ChartAreas[this.series.ChartArea];
					foreach (Legend legend in this.chartControl.Legends)
					{
						legend.Position.Auto = false;
						legend.DockInsideChartArea = false;
						legend.DockToChartArea = "";
					}
					foreach (Title title in this.chartControl.Titles)
					{
						title.Position.Auto = false;
						title.DockInsideChartArea = false;
						title.DockToChartArea = "";
					}
					this.originalChartArea.Position.X = this.ChartAreaPosition.X;
					this.originalChartArea.Position.Y = this.ChartAreaPosition.Y;
					this.originalChartArea.Position.Width = this.ChartAreaPosition.Width - num - this.ChartAreaSpacing;
					this.originalChartArea.Position.Height = this.ChartAreaPosition.Height;
					this.originalChartArea.Area3DStyle.Enable3D = false;
					this.supplementalChartArea = new ChartArea();
					this.supplementalChartArea.Name = this.originalChartArea.Name + "_Supplemental";
					this.supplementalChartArea.Position.X = this.originalChartArea.Position.Right() + this.ChartAreaSpacing;
					this.supplementalChartArea.Position.Y = this.ChartAreaPosition.Y;
					this.supplementalChartArea.Position.Width = num;
					this.supplementalChartArea.Position.Height = this.ChartAreaPosition.Height;
					this.supplementalSeries.ChartArea = this.supplementalChartArea.Name;
					this.chartControl.ChartAreas.Add(this.supplementalChartArea);
					this.supplementalChartArea.BackColor = this.originalChartArea.BackColor;
					this.supplementalChartArea.BorderColor = this.originalChartArea.BorderColor;
					this.supplementalChartArea.BorderWidth = this.originalChartArea.BorderWidth;
					this.supplementalChartArea.ShadowOffset = this.originalChartArea.ShadowOffset;
					foreach (ChartArea chartArea in this.chartControl.ChartAreas)
					{
						chartArea.Position.Auto = false;
					}
					this.ignorePaintEvent = false;
				}
				return;
			}
			throw new ArgumentException("Value must be in range from 0 to 100 percent.", "CollectedPercentage");
		}

		private bool CreateCollectedPie()
		{
			this.supplementalSeries = new Series();
			double num = 0.0;
			foreach (DataPoint point in this.series.Points)
			{
				if (!point.Empty && !double.IsNaN(point.YValues[0]))
				{
					num += Math.Abs(point.YValues[0]);
				}
			}
			double num2 = num / 100.0 * this.CollectedPercentage;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < this.series.Points.Count; i++)
			{
				if (this.series.Points[i].Empty || double.IsNaN(this.series.Points[i].YValues[0]))
				{
					num4++;
				}
				else
				{
					double num5 = Math.Abs(this.series.Points[i].YValues[0]);
					if (num5 <= num2)
					{
						num3++;
					}
				}
			}
			if (this.series.Points.Count - num4 - num3 > 1 && num3 > 1)
			{
				DataPoint dataPoint2 = null;
				if (this.ShowCollectedDataAsOneSlice)
				{
					dataPoint2 = this.series.Points[0].Clone();
					dataPoint2.YValues[0] = 0.0;
					this.series.Points.Add(dataPoint2);
				}
				double num6 = 0.0;
				for (int j = 0; j < this.series.Points.Count; j++)
				{
					if (!this.series.Points[j].Empty && !double.IsNaN(this.series.Points[j].YValues[0]))
					{
						double num7 = Math.Abs(this.series.Points[j].YValues[0]);
						if (num7 <= num2 && this.series.Points[j] != dataPoint2)
						{
							num6 += num7;
							this.supplementalSeries.Points.Add(this.series.Points[j].Clone());
							this.series.Points.RemoveAt(j);
							j--;
						}
					}
				}
				if (num6 > 0.0)
				{
					this.supplementalSeries.Name = this.series.Name + "_Supplemental";
					this.supplementalSeries.ChartArea = this.series.ChartArea;
					this.chartControl.Series.Add(this.supplementalSeries);
					this.supplementalSeries.ChartType = this.series.ChartType;
					this.supplementalSeries.Palette = this.series.Palette;
					this.supplementalSeries.ShadowOffset = this.series.ShadowOffset;
					this.supplementalSeries.BorderColor = this.series.BorderColor;
					this.supplementalSeries.BorderWidth = this.series.BorderWidth;
					this.supplementalSeries.ShowLabelAsValue = this.series.ShowLabelAsValue;
					this.supplementalSeries.LabelBackColor = this.series.LabelBackColor;
					this.supplementalSeries.LabelBorderColor = this.series.LabelBorderColor;
					this.supplementalSeries.LabelBorderWidth = this.series.LabelBorderWidth;
					this.supplementalSeries.Label = this.series.Label;
					this.supplementalSeries.LabelFormat = this.series.LabelFormat;
					this.supplementalSeries.labelBorderStyle = this.series.LabelBorderStyle;
					this.supplementalSeries.Font = this.series.Font;
					this.supplementalSeries.CustomAttributes = this.series.CustomAttributes;
					if (this.ShowCollectedLegend)
					{
						this.supplementalSeries.Legend = this.series.Legend;
						this.supplementalSeries.ShowInLegend = true;
						foreach (DataPoint point2 in this.supplementalSeries.Points)
						{
							point2.ShowInLegend = true;
						}
					}
					else
					{
						this.supplementalSeries.ShowInLegend = false;
						foreach (DataPoint point3 in this.supplementalSeries.Points)
						{
							point3.ShowInLegend = false;
						}
					}
					if (!this.ShowCollectedPointLabels)
					{
						this.supplementalSeries.Label = string.Empty;
						this.supplementalSeries.LabelFormat = string.Empty;
						this.supplementalSeries.ShowLabelAsValue = false;
						this.supplementalSeries.SetAttribute("AutoAxisLabels", "false");
						foreach (DataPoint point4 in this.supplementalSeries.Points)
						{
							point4.ShowLabelAsValue = false;
						}
					}
					else
					{
						this.supplementalSeries.ShowLabelAsValue = true;
					}
					if (this.ShowCollectedDataAsOneSlice)
					{
						dataPoint2.YValues[0] = num6;
						dataPoint2.Label = this.CollectedLabel;
						dataPoint2.AxisLabel = this.CollectedLabel;
						dataPoint2.LegendText = this.CollectedLabel;
						if (!this.SliceColor.IsEmpty)
						{
							dataPoint2.Color = this.SliceColor;
						}
					}
					this.collectedPieSliceAngle = (float)(3.6 * (num6 / (num / 100.0)));
					int num8 = (int)Math.Round((double)this.collectedPieSliceAngle / 2.0);
					((DataPointAttributes)this.series)["PieStartAngle"] = num8.ToString(CultureInfo.InvariantCulture);
					this.ApplyPaletteColors();
					MemoryStream imageStream = new MemoryStream();
					this.chartControl.Save(imageStream);
					this.ChartAreaPosition = new RectangleF(this.chartControl.ChartAreas[this.series.ChartArea].Position.X, this.chartControl.ChartAreas[this.series.ChartArea].Position.Y, this.chartControl.ChartAreas[this.series.ChartArea].Position.Width, this.chartControl.ChartAreas[this.series.ChartArea].Position.Height);
					return true;
				}
				if (dataPoint2 != null)
				{
					this.series.Points.Remove(dataPoint2);
				}
				return false;
			}
			return false;
		}

		private void ApplyPaletteColors()
		{
			ChartColorPalette palette = this.series.Palette;
			DataManager dataManager = (DataManager)this.series.serviceContainer.GetService(typeof(DataManager));
			if (palette == ChartColorPalette.None)
			{
				palette = dataManager.Palette;
			}
			if (palette == ChartColorPalette.None && dataManager.PaletteCustomColors.Length <= 0)
			{
				return;
			}
			int num = 0;
			Color[] array = (dataManager.PaletteCustomColors.Length > 0) ? dataManager.PaletteCustomColors : ChartPaletteColors.GetPaletteColors(palette);
			ArrayList arrayList = new ArrayList(this.series.Points);
			arrayList.AddRange(this.supplementalSeries.Points);
			foreach (DataPoint item in arrayList)
			{
				if (!item.Empty && !double.IsNaN(item.YValues[0]) && (!item.IsAttributeSet(CommonAttributes.Color) || item.tempColorIsSet))
				{
					item.Color = array[num];
					num++;
					if (num >= array.Length)
					{
						num = 0;
					}
				}
			}
		}

		private void chart_PostPaint(object sender, ChartPaintEventArgs e)
		{
			if (!this.ignorePaintEvent && sender is ChartArea)
			{
				ChartArea chartArea = (ChartArea)sender;
				if (this.supplementalChartArea != null && chartArea.Name == this.supplementalChartArea.Name)
				{
					RectangleF chartAreaPlottingPosition = this.GetChartAreaPlottingPosition(this.originalChartArea, e.ChartGraphics);
					RectangleF chartAreaPlottingPosition2 = this.GetChartAreaPlottingPosition(this.supplementalChartArea, e.ChartGraphics);
					PointF rotatedPlotAreaPoint = this.GetRotatedPlotAreaPoint(chartAreaPlottingPosition2, 325f);
					PointF rotatedPlotAreaPoint2 = this.GetRotatedPlotAreaPoint(chartAreaPlottingPosition2, 215f);
					PointF rotatedPlotAreaPoint3 = this.GetRotatedPlotAreaPoint(chartAreaPlottingPosition, (float)(90.0 - this.collectedPieSliceAngle / 2.0));
					PointF rotatedPlotAreaPoint4 = this.GetRotatedPlotAreaPoint(chartAreaPlottingPosition, (float)(90.0 + this.collectedPieSliceAngle / 2.0));
					using (Pen pen = new Pen(this.ConnectionLinesColor, 1f))
					{
						e.ChartGraphics.DrawLine(pen, rotatedPlotAreaPoint, rotatedPlotAreaPoint3);
						e.ChartGraphics.DrawLine(pen, rotatedPlotAreaPoint2, rotatedPlotAreaPoint4);
					}
				}
			}
		}

		private PointF GetRotatedPlotAreaPoint(RectangleF areaPosition, float angle)
		{
			PointF[] array = new PointF[1]
			{
				new PointF((float)(areaPosition.X + areaPosition.Width / 2.0), areaPosition.Y)
			};
			using (Matrix matrix = new Matrix())
			{
				matrix.RotateAt(angle, new PointF((float)(areaPosition.X + areaPosition.Width / 2.0), (float)(areaPosition.Y + areaPosition.Height / 2.0)));
				matrix.TransformPoints(array);
			}
			return array[0];
		}

		private RectangleF GetChartAreaPlottingPosition(ChartArea area, ChartGraphics chartGraphics)
		{
			RectangleF rectangleF = area.Position.ToRectangleF();
			rectangleF.X += (float)(area.Position.Width / 100.0 * area.InnerPlotPosition.X);
			rectangleF.Y += (float)(area.Position.Height / 100.0 * area.InnerPlotPosition.Y);
			rectangleF.Width = (float)(area.Position.Width / 100.0 * area.InnerPlotPosition.Width);
			rectangleF.Height = (float)(area.Position.Height / 100.0 * area.InnerPlotPosition.Height);
			rectangleF = chartGraphics.GetAbsoluteRectangle(rectangleF);
			return rectangleF;
		}
	}
}
