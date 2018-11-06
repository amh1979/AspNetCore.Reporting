using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
//using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms.Data
{
	internal class DataManager : IServiceProvider
	{
		private SeriesCollection series;

		internal IServiceContainer serviceContainer;

		private ChartColorPalette colorPalette = ChartColorPalette.BrightPastel;

		private Color[] paletteCustomColors = new Color[0];

		[Bindable(true)]
		[SRCategory("CategoryAttributeData")]
		public SeriesCollection Series
		{
			get
			{
				return this.series;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartColorPalette.BrightPastel)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributePalette")]
		public ChartColorPalette Palette
		{
			get
			{
				return this.colorPalette;
			}
			set
			{
				this.colorPalette = value;
			}
		}

		[SerializationVisibility(SerializationVisibility.Attribute)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		//[TypeConverter(typeof(ColorArrayConverter))]
		[SRDescription("DescriptionAttributeDataManager_PaletteCustomColors")]
		[SRCategory("CategoryAttributeAppearance")]
		public Color[] PaletteCustomColors
		{
			get
			{
				return this.paletteCustomColors;
			}
			set
			{
				this.paletteCustomColors = value;
			}
		}

		private DataManager()
		{
		}

		public DataManager(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			this.serviceContainer = container;
			this.series = new SeriesCollection(container);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(DataManager))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionDataManagerUnsupportedType(serviceType.ToString()));
		}

		internal void Initialize()
		{
			ChartImage chartImage = (ChartImage)this.serviceContainer.GetService(typeof(ChartImage));
			chartImage.BeforePaint += this.ChartPicture_BeforePaint;
			chartImage.AfterPaint += this.ChartPicture_AfterPaint;
		}

		private void ChartPicture_BeforePaint(object sender, ChartPaintEventArgs e)
		{
			int num = 1;
			for (int i = 0; i < this.Series.Count; i++)
			{
				Series series = this.Series[i];
				series.xValuesZerosChecked = false;
				series.xValuesZeros = false;
				IChartType chartType = e.CommonElements.ChartTypeRegistry.GetChartType(series.ChartTypeName);
				bool pointsApplyPaletteColors = chartType.ApplyPaletteColorsToPoints;
				if (series.Palette != 0)
				{
					pointsApplyPaletteColors = true;
				}
				this.PrepareData(this.Palette != 0 || this.PaletteCustomColors.Length > 0, pointsApplyPaletteColors, series.Name);
				if (series.tempMarkerStyleIsSet)
				{
					series.MarkerStyle = MarkerStyle.None;
					series.tempMarkerStyleIsSet = false;
				}
				if (chartType.GetLegendImageStyle(series) == LegendImageStyle.Marker && series.MarkerStyle == MarkerStyle.None)
				{
					series.MarkerStyle = (MarkerStyle)(num++);
					series.tempMarkerStyleIsSet = true;
					if (num > 9)
					{
						num = 1;
					}
				}
			}
		}

		private void ChartPicture_AfterPaint(object sender, ChartPaintEventArgs e)
		{
			Chart chart = (Chart)this.serviceContainer.GetService(typeof(Chart));
			if (chart != null)
			{
				for (int i = 0; i < this.Series.Count; i++)
				{
					Series series = this.Series[i];
					if (series.UnPrepareData(null))
					{
						i--;
					}
				}
			}
		}

		internal void ApplyPaletteColors()
		{
			if (this.Palette == ChartColorPalette.None && this.PaletteCustomColors.Length <= 0)
			{
				return;
			}
			int num = 0;
			Color[] array = (this.PaletteCustomColors.Length > 0) ? this.PaletteCustomColors : ChartPaletteColors.GetPaletteColors(this.colorPalette);
			foreach (Series item in this.series)
			{
				bool flag = false;
				if (item.ChartArea.Length > 0)
				{
					ChartImage chartImage = (ChartImage)this.serviceContainer.GetService(typeof(ChartImage));
					if (chartImage != null)
					{
						foreach (ChartArea chartArea in chartImage.ChartAreas)
						{
							if (chartArea.Name == item.ChartArea)
							{
								flag = true;
								break;
							}
						}
						if (!flag && item.ChartArea == "Default" && chartImage.ChartAreas.Count > 0)
						{
							flag = true;
						}
					}
				}
				if (flag && (item.Color == Color.Empty || item.tempColorIsSet))
				{
					item.color = array[num++];
					item.tempColorIsSet = true;
					if (num >= array.Length)
					{
						num = 0;
					}
				}
			}
		}

		internal void PrepareData(bool seriesApplyPaletteColors, bool pointsApplyPaletteColors, params string[] series)
		{
			if (seriesApplyPaletteColors)
			{
				this.ApplyPaletteColors();
			}
			Chart chart = (Chart)this.serviceContainer.GetService(typeof(Chart));
			if (chart != null)
			{
				foreach (string parameter in series)
				{
					this.Series[parameter].PrepareData(null, pointsApplyPaletteColors);
				}
			}
		}

		private bool IsPointSkipped(DataPoint point)
		{
			if (point.Empty)
			{
				return true;
			}
			return false;
		}

		internal int GetNumberOfPoints(params string[] series)
		{
			int num = 0;
			foreach (string parameter in series)
			{
				num = Math.Max(num, this.series[parameter].Points.Count);
			}
			return num;
		}

		internal double GetMaxYValue(int valueIndex, params string[] series)
		{
			double num = -1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!this.IsPointSkipped(point) && !double.IsNaN(point.YValues[valueIndex]))
					{
						num = Math.Max(num, point.YValues[valueIndex]);
					}
				}
			}
			return num;
		}

		internal double GetMaxYWithRadiusValue(ChartArea area, params string[] series)
		{
			double num = -1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!this.IsPointSkipped(point) && !double.IsNaN(point.YValues[0]))
					{
						num = ((point.YValues.Length <= 1) ? Math.Max(num, point.YValues[0]) : Math.Max(num, point.YValues[0] + BubbleChart.AxisScaleBubbleSize(area.Common.graph, area.Common, area, point.YValues[1], true)));
					}
				}
			}
			return num;
		}

		internal double GetMaxXWithRadiusValue(ChartArea area, params string[] series)
		{
			double num = -1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!point.EmptyX && (!this.series[parameter].EmptyX || point.XValue != 0.0) && !this.IsPointSkipped(point) && !double.IsNaN(point.XValue))
					{
						num = ((point.YValues.Length <= 1) ? Math.Max(num, point.XValue) : Math.Max(num, point.XValue + BubbleChart.AxisScaleBubbleSize(area.Common.graph, area.Common, area, point.XValue, false)));
					}
				}
			}
			return num;
		}

		internal double GetMinXWithRadiusValue(ChartArea area, params string[] series)
		{
			double num = 1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!point.EmptyX && (!this.series[parameter].EmptyX || point.XValue != 0.0) && !this.IsPointSkipped(point) && !double.IsNaN(point.XValue))
					{
						num = ((point.YValues.Length <= 1) ? Math.Min(num, point.XValue) : Math.Min(num, point.XValue - BubbleChart.AxisScaleBubbleSize(area.Common.graph, area.Common, area, point.YValues[1], false)));
					}
				}
			}
			return num;
		}

		internal double GetMaxYValue(params string[] series)
		{
			double num = -1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!this.IsPointSkipped(point))
					{
						double[] yValues = point.YValues;
						foreach (double num2 in yValues)
						{
							if (!double.IsNaN(num2))
							{
								num = Math.Max(num, num2);
							}
						}
					}
				}
			}
			return num;
		}

		internal double GetMaxXValue(params string[] series)
		{
			double num = -1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!point.EmptyX && (!this.series[parameter].EmptyX || point.XValue != 0.0))
					{
						num = Math.Max(num, point.XValue);
					}
				}
			}
			return num;
		}

		internal void GetMinMaxXValue(out double min, out double max, params string[] series)
		{
			max = -1.7976931348623157E+308;
			min = 1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					max = Math.Max(max, point.XValue);
					min = Math.Min(min, point.XValue);
				}
			}
		}

		internal void GetMinMaxYValue(int valueIndex, out double min, out double max, params string[] series)
		{
			max = -1.7976931348623157E+308;
			min = 1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!this.IsPointSkipped(point))
					{
						double num = point.YValues[valueIndex];
						if (!double.IsNaN(num))
						{
							max = Math.Max(max, num);
							min = Math.Min(min, num);
						}
					}
				}
			}
		}

		internal void GetMinMaxYValue(out double min, out double max, params string[] series)
		{
			max = -1.7976931348623157E+308;
			min = 1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!this.IsPointSkipped(point))
					{
						double[] yValues = point.YValues;
						foreach (double num in yValues)
						{
							if (!double.IsNaN(num))
							{
								max = Math.Max(max, num);
								min = Math.Min(min, num);
							}
						}
					}
				}
			}
		}

		internal void GetMinMaxYValue(ArrayList seriesList, out double min, out double max)
		{
			max = -1.7976931348623157E+308;
			min = 1.7976931348623157E+308;
			foreach (Series series2 in seriesList)
			{
				foreach (DataPoint point in series2.Points)
				{
					if (!this.IsPointSkipped(point))
					{
						double[] yValues = point.YValues;
						foreach (double num in yValues)
						{
							if (!double.IsNaN(num))
							{
								max = Math.Max(max, num);
								min = Math.Min(min, num);
							}
						}
					}
				}
			}
		}

		internal double GetMaxStackedYValue(int valueIndex, params string[] series)
		{
			double num = 0.0;
			double num2 = (double)this.GetNumberOfPoints(series);
			for (int i = 0; (double)i < num2; i++)
			{
				double num3 = 0.0;
				double num4 = 0.0;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points.Count > i)
					{
						ChartTypeRegistry chartTypeRegistry = (ChartTypeRegistry)this.serviceContainer.GetService(typeof(ChartTypeRegistry));
						IChartType chartType = chartTypeRegistry.GetChartType(this.series[parameter].ChartTypeName);
						if (chartType.StackSign)
						{
							if (chartType.Stacked)
							{
								if (this.series[parameter].Points[i].YValues[valueIndex] > 0.0)
								{
									num3 += this.series[parameter].Points[i].YValues[valueIndex];
								}
							}
							else
							{
								num4 = Math.Max(num4, this.series[parameter].Points[i].YValues[valueIndex]);
							}
						}
					}
				}
				num3 = Math.Max(num3, num4);
				num = Math.Max(num, num3);
			}
			return num;
		}

		internal double GetMaxUnsignedStackedYValue(int valueIndex, params string[] series)
		{
			double num = 0.0;
			double num2 = -1.7976931348623157E+308;
			double num3 = (double)this.GetNumberOfPoints(series);
			for (int i = 0; (double)i < num3; i++)
			{
				double num4 = 0.0;
				double num5 = 0.0;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points.Count > i)
					{
						ChartTypeRegistry chartTypeRegistry = (ChartTypeRegistry)this.serviceContainer.GetService(typeof(ChartTypeRegistry));
						IChartType chartType = chartTypeRegistry.GetChartType(this.series[parameter].ChartTypeName);
						if (!chartType.StackSign && !double.IsNaN(this.series[parameter].Points[i].YValues[valueIndex]))
						{
							if (chartType.Stacked)
							{
								num2 = -1.7976931348623157E+308;
								num4 += this.series[parameter].Points[i].YValues[valueIndex];
								if (num4 > num2)
								{
									num2 = num4;
								}
							}
							else
							{
								num5 = Math.Max(num5, this.series[parameter].Points[i].YValues[valueIndex]);
							}
						}
					}
				}
				num2 = Math.Max(num2, num5);
				num = Math.Max(num, num2);
			}
			return num;
		}

		internal double GetMaxStackedXValue(params string[] series)
		{
			double num = 0.0;
			double num2 = (double)this.GetNumberOfPoints(series);
			for (int i = 0; (double)i < num2; i++)
			{
				double num3 = 0.0;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points.Count > i && this.series[parameter].Points[i].XValue > 0.0)
					{
						num3 += this.series[parameter].Points[i].XValue;
					}
				}
				num = Math.Max(num, num3);
			}
			return num;
		}

		internal double GetMinYValue(int valueIndex, params string[] series)
		{
			double num = 1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!this.IsPointSkipped(point) && !double.IsNaN(point.YValues[valueIndex]))
					{
						num = Math.Min(num, point.YValues[valueIndex]);
					}
				}
			}
			return num;
		}

		internal double GetMinYWithRadiusValue(ChartArea area, params string[] series)
		{
			double num = 1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!this.IsPointSkipped(point) && !double.IsNaN(point.YValues[0]))
					{
						num = ((point.YValues.Length <= 1) ? Math.Min(num, point.YValues[0]) : Math.Min(num, point.YValues[0] - BubbleChart.AxisScaleBubbleSize(area.Common.graph, area.Common, area, point.YValues[1], true)));
					}
				}
			}
			return num;
		}

		internal double GetMinYValue(params string[] series)
		{
			double num = 1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!this.IsPointSkipped(point))
					{
						double[] yValues = point.YValues;
						foreach (double num2 in yValues)
						{
							if (!double.IsNaN(num2))
							{
								num = Math.Min(num, num2);
							}
						}
					}
				}
			}
			return num;
		}

		internal double GetMinXValue(params string[] series)
		{
			double num = 1.7976931348623157E+308;
			foreach (string parameter in series)
			{
				foreach (DataPoint point in this.series[parameter].Points)
				{
					if (!point.EmptyX && (!this.series[parameter].EmptyX || point.XValue != 0.0))
					{
						num = Math.Min(num, point.XValue);
					}
				}
			}
			return num;
		}

		internal double GetMinStackedYValue(int valueIndex, params string[] series)
		{
			double num = 1.7976931348623157E+308;
			double num2 = (double)this.GetNumberOfPoints(series);
			for (int i = 0; (double)i < num2; i++)
			{
				double num3 = 0.0;
				double num4 = 0.0;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points.Count > i)
					{
						ChartTypeRegistry chartTypeRegistry = (ChartTypeRegistry)this.serviceContainer.GetService(typeof(ChartTypeRegistry));
						IChartType chartType = chartTypeRegistry.GetChartType(this.series[parameter].ChartTypeName);
						if (chartType.StackSign && !double.IsNaN(this.series[parameter].Points[i].YValues[valueIndex]))
						{
							if (chartType.Stacked)
							{
								if (this.series[parameter].Points[i].YValues[valueIndex] < 0.0)
								{
									num3 += this.series[parameter].Points[i].YValues[valueIndex];
								}
							}
							else
							{
								num4 = Math.Min(num4, this.series[parameter].Points[i].YValues[valueIndex]);
							}
						}
					}
				}
				num3 = Math.Min(num3, num4);
				if (num3 == 0.0)
				{
					num3 = this.series[series[0]].Points[this.series[series[0]].Points.Count - 1].YValues[valueIndex];
				}
				num = Math.Min(num, num3);
			}
			return num;
		}

		internal double GetMinUnsignedStackedYValue(int valueIndex, params string[] series)
		{
			double num = 1.7976931348623157E+308;
			double num2 = 1.7976931348623157E+308;
			double num3 = (double)this.GetNumberOfPoints(series);
			for (int i = 0; (double)i < num3; i++)
			{
				double num4 = 0.0;
				double val = 0.0;
				num2 = 1.7976931348623157E+308;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points.Count > i)
					{
						ChartTypeRegistry chartTypeRegistry = (ChartTypeRegistry)this.serviceContainer.GetService(typeof(ChartTypeRegistry));
						IChartType chartType = chartTypeRegistry.GetChartType(this.series[parameter].ChartTypeName);
						if (!chartType.StackSign && !double.IsNaN(this.series[parameter].Points[i].YValues[valueIndex]))
						{
							if (chartType.Stacked)
							{
								if (this.series[parameter].Points[i].YValues[valueIndex] < 0.0)
								{
									num4 += this.series[parameter].Points[i].YValues[valueIndex];
									if (num4 < num2)
									{
										num2 = num4;
									}
								}
							}
							else
							{
								val = Math.Min(val, this.series[parameter].Points[i].YValues[valueIndex]);
							}
						}
					}
				}
				num2 = Math.Min(val, num2);
				num = Math.Min(num, num2);
			}
			return num;
		}

		internal double GetMinStackedXValue(params string[] series)
		{
			double num = 0.0;
			double num2 = (double)this.GetNumberOfPoints(series);
			for (int i = 0; (double)i < num2; i++)
			{
				double num3 = 0.0;
				foreach (string parameter in series)
				{
					if (this.series[parameter].Points[i].XValue < 0.0)
					{
						num3 += this.series[parameter].Points[i].XValue;
					}
				}
				num = Math.Min(num, num3);
			}
			return num;
		}

		internal double GetMaxHundredPercentStackedYValue(bool supportNegative, int valueIndex, params string[] series)
		{
			double num = 0.0;
			Series[] array = new Series[series.Length];
			int num2 = 0;
			foreach (string parameter in series)
			{
				array[num2++] = this.series[parameter];
			}
			try
			{
				for (int j = 0; j < this.series[series[0]].Points.Count; j++)
				{
					double num4 = 0.0;
					double num5 = 0.0;
					Series[] array2 = array;
					foreach (Series series2 in array2)
					{
						num4 = ((!supportNegative) ? (num4 + series2.Points[j].YValues[0]) : (num4 + Math.Abs(series2.Points[j].YValues[0])));
						if (series2.Points[j].YValues[0] > 0.0 || !supportNegative)
						{
							num5 += series2.Points[j].YValues[0];
						}
					}
					num4 = Math.Abs(num4);
					if (num4 != 0.0)
					{
						num = Math.Max(num, num5 / num4 * 100.0);
					}
				}
				return num;
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionDataManager100StackedSeriesPointsNumeberMismatch);
			}
		}

		internal double GetMinHundredPercentStackedYValue(bool supportNegative, int valueIndex, params string[] series)
		{
			double num = 0.0;
			Series[] array = new Series[series.Length];
			int num2 = 0;
			foreach (string parameter in series)
			{
				array[num2++] = this.series[parameter];
			}
			try
			{
				for (int j = 0; j < this.series[series[0]].Points.Count; j++)
				{
					double num4 = 0.0;
					double num5 = 0.0;
					Series[] array2 = array;
					foreach (Series series2 in array2)
					{
						num4 = ((!supportNegative) ? (num4 + series2.Points[j].YValues[0]) : (num4 + Math.Abs(series2.Points[j].YValues[0])));
						if (series2.Points[j].YValues[0] < 0.0 || !supportNegative)
						{
							num5 += series2.Points[j].YValues[0];
						}
					}
					num4 = Math.Abs(num4);
					if (num4 != 0.0)
					{
						num = Math.Min(num, num5 / num4 * 100.0);
					}
				}
				return num;
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionDataManager100StackedSeriesPointsNumeberMismatch);
			}
		}
	}
}
