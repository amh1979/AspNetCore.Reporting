using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal sealed class SunburstChart : IChartType
	{
		private static StringFormat format;

		public string Name
		{
			get
			{
				return "TreeMap";
			}
		}

		public bool Stacked
		{
			get
			{
				return false;
			}
		}

		public bool SupportStackedGroups
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

		public bool RequireAxes
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

		public bool SupportLogarithmicAxes
		{
			get
			{
				return false;
			}
		}

		public bool SwitchValueAxes
		{
			get
			{
				return false;
			}
		}

		public bool SideBySideSeries
		{
			get
			{
				return false;
			}
		}

		public bool ZeroCrossing
		{
			get
			{
				return false;
			}
		}

		public bool DataPointsInLegend
		{
			get
			{
				return false;
			}
		}

		public bool ExtraYValuesConnectedToYAxis
		{
			get
			{
				return false;
			}
		}

		public bool HundredPercent
		{
			get
			{
				return false;
			}
		}

		public bool HundredPercentSupportNegative
		{
			get
			{
				return false;
			}
		}

		public bool ApplyPaletteColorsToPoints
		{
			get
			{
				return false;
			}
		}

		public int YValuesPerPoint
		{
			get
			{
				return 1;
			}
		}

		public bool Doughnut
		{
			get
			{
				return false;
			}
		}

		static SunburstChart()
		{
			SunburstChart.format = new StringFormat();
			SunburstChart.format.Alignment = StringAlignment.Center;
			SunburstChart.format.LineAlignment = StringAlignment.Center;
			SunburstChart.format.FormatFlags = (StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip);
			SunburstChart.format.Trimming = StringTrimming.None;
		}

		public Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
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
			RectangleF plottingAreaRelative = SunburstChart.GetPlottingAreaRelative(graph, area);
			graph.SetClip(plottingAreaRelative);
			SunburstChart.RenderNodes(common, graph, area, plottingAreaRelative);
			graph.ResetClip();
		}

		private static RectangleF GetPlottingAreaRelative(ChartGraphics graph, ChartArea area)
		{
			RectangleF rectangleF = (!area.InnerPlotPosition.Auto) ? new RectangleF(area.PlotAreaPosition.ToRectangleF().X, area.PlotAreaPosition.ToRectangleF().Y, area.PlotAreaPosition.ToRectangleF().Width, area.PlotAreaPosition.ToRectangleF().Height) : new RectangleF(area.Position.ToRectangleF().X, area.Position.ToRectangleF().Y, area.Position.ToRectangleF().Width, area.Position.ToRectangleF().Height);
			SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF(rectangleF.Width, rectangleF.Height));
			float num = (absoluteSize.Width < absoluteSize.Height) ? absoluteSize.Width : absoluteSize.Height;
			SizeF relativeSize = graph.GetRelativeSize(new SizeF(num, num));
			PointF pointF = new PointF((float)(rectangleF.X + rectangleF.Width / 2.0), (float)(rectangleF.Y + rectangleF.Height / 2.0));
			return new RectangleF((float)(pointF.X - relativeSize.Width / 2.0), (float)(pointF.Y - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height);
		}

		private static void RenderNodes(CommonElements common, ChartGraphics graph, ChartArea area, RectangleF plottingAreaRelative)
		{
			if (!(plottingAreaRelative.Width < 0.0) && !(plottingAreaRelative.Height < 0.0))
			{
				CategoryNodeCollection categoryNodes = area.CategoryNodes;
				if (categoryNodes != null)
				{
					List<Series> chartAreaSeries = SunburstChart.GetChartAreaSeries(area.Name, common.DataManager.Series);
					categoryNodes.Calculate(chartAreaSeries);
					double totalAbsoluetValue = categoryNodes.GetTotalAbsoluetValue();
					SunburstChart.SortSeriesByAbsoluteValue(chartAreaSeries, categoryNodes);
					int num = 2 * (categoryNodes.GetDepth() + 1);
					float num2 = plottingAreaRelative.Width / (float)num;
					float num3 = plottingAreaRelative.Height / (float)num;
					float num4 = (float)(num2 * 4.0);
					float num5 = (float)(num3 * 4.0);
					RectangleF rectRelative = new RectangleF((float)(plottingAreaRelative.X + plottingAreaRelative.Width / 2.0 - num4 / 2.0), (float)(plottingAreaRelative.Y + plottingAreaRelative.Height / 2.0 - num5 / 2.0), num4, num5);
					SunburstChart.RenderNodes(common, graph, categoryNodes, rectRelative, totalAbsoluetValue, num2, num3, chartAreaSeries);
				}
			}
		}

		private static void RenderNodes(CommonElements common, ChartGraphics graph, CategoryNodeCollection nodes, RectangleF rectRelative, double chartTotal, float incrementXRelative, float incrementYRelative, List<Series> seriesCollection)
		{
			float num = 270f;
			foreach (Series item in seriesCollection)
			{
				SunburstChart.RenderNodes(common, graph, nodes, rectRelative, 1, chartTotal, ref num, 360f, incrementXRelative, incrementYRelative, item, SunburstChart.GetFirstNonEmptyDataPointsAttributes(item));
			}
		}

		private static DataPointAttributes GetFirstNonEmptyDataPointsAttributes(Series series)
		{
			foreach (DataPoint point in series.Points)
			{
				if (!point.Empty)
				{
					return point;
				}
			}
			return series;
		}

		private static void RenderNodes(CommonElements common, ChartGraphics graph, CategoryNodeCollection nodes, RectangleF rectRelative, int level, double parentValue, ref float startAngle, float parentSweepAngle, float incrementXRelative, float incrementYRelative, Series series, DataPointAttributes dataPointAttributes)
		{
			if (!nodes.AreAllNodesEmpty(series))
			{
				nodes.SortByAbsoluteValue(series);
				RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(rectRelative);
				float thresholdAngle = (float)(360.0 / (6.2831853071795862 * (double)absoluteRectangle.Width));
				PointF centerAbsolute = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0));
				float centerRadiusAbsolute = (float)(graph.GetAbsoluteWidth(rectRelative.Width - incrementXRelative) / 2.0);
				float absoluteWidth = graph.GetAbsoluteWidth((float)(rectRelative.Width / 2.0));
				foreach (CategoryNode node in nodes)
				{
					SunburstChart.RenderNode(common, graph, node, rectRelative, level, parentValue, ref startAngle, parentSweepAngle, thresholdAngle, incrementXRelative, incrementYRelative, centerAbsolute, centerRadiusAbsolute, absoluteWidth, series, dataPointAttributes);
				}
			}
		}

		private static void RenderNode(CommonElements common, ChartGraphics graph, CategoryNode node, RectangleF rectRelative, int level, double parentValue, ref float startAngle, float parentSweepAngle, float thresholdAngle, float incrementXRelative, float incrementYRelative, PointF centerAbsolute, float centerRadiusAbsolute, float edgeRadiusAbsolute, Series series, DataPointAttributes dataPointAttributes)
		{
			double absoluteValue = node.GetValues(series).AbsoluteValue;
			if (absoluteValue != 0.0)
			{
				CategoryNode dataPointNode = node.GetDataPointNode(series);
				DataPoint dataPoint;
				int dataPointIndex;
				if (dataPointNode != null)
				{
					dataPoint = dataPointNode.GetDataPoint(series);
					dataPointIndex = dataPointNode.Index;
				}
				else
				{
					dataPoint = null;
					dataPointIndex = -1;
				}
				DataPointAttributes dataPointAttributes2 = (dataPoint != null) ? dataPoint : dataPointAttributes;
				float num = (float)(absoluteValue / parentValue * (double)parentSweepAngle);
				float sweepAngle = num - thresholdAngle;
				using (GraphicsPath sliceGraphicsPath = SunburstChart.RenderSlice(common, graph, node, dataPoint, dataPointAttributes2, rectRelative, startAngle, sweepAngle, centerAbsolute, edgeRadiusAbsolute, level, dataPointIndex))
				{
					SunburstChart.RenderLabel(common, graph, node, dataPoint, dataPointAttributes2, SunburstChart.GetLabelText(node, dataPoint, series, dataPointAttributes2), startAngle, num, centerAbsolute, centerRadiusAbsolute, dataPointIndex, sliceGraphicsPath);
				}
				if (node.Children != null)
				{
					float num2 = startAngle;
					SunburstChart.RenderNodes(common, graph, node.Children, RectangleF.Inflate(rectRelative, incrementXRelative, incrementYRelative), level + 1, absoluteValue, ref num2, num, incrementXRelative, incrementYRelative, series, dataPointAttributes);
				}
				startAngle += num;
			}
		}

		private static string GetLabelText(CategoryNode categoryNode, DataPoint dataPoint, Series series, DataPointAttributes dataPointAttributes)
		{
			if (dataPoint != null)
			{
				if (TreeMapChart.IsLabelVisible(dataPoint))
				{
					string labelText = PieChart.GetLabelText(dataPoint, false);
					if (!string.IsNullOrEmpty(labelText))
					{
						return labelText;
					}
					return SunburstChart.GetCategoryNodeLabelText(categoryNode, series, dataPoint);
				}
			}
			else if (TreeMapChart.IsLabelVisible(dataPointAttributes))
			{
				return SunburstChart.GetCategoryNodeLabelText(categoryNode, series, dataPointAttributes);
			}
			return string.Empty;
		}

		private static string GetCategoryNodeLabelText(CategoryNode categoryNode, Series series, DataPointAttributes dataPointAttributes)
		{
			if (dataPointAttributes.ShowLabelAsValue)
			{
				return ValueConverter.FormatValue(series.chart, null, categoryNode.GetValues(series).Value, dataPointAttributes.LabelFormat, series.YValueType, ChartElementType.DataPoint);
			}
			return categoryNode.Label;
		}

		private static GraphicsPath RenderSlice(CommonElements common, ChartGraphics graph, CategoryNode node, DataPoint dataPoint, DataPointAttributes dataPointAttributes, RectangleF rectRelative, float startAngle, float sweepAngle, PointF centerAbsolute, float radiusAbsolute, int level, int dataPointIndex)
		{
			float doughnutRadius = (float)(1.0 / (float)(level + 1) * 100.0);
			GraphicsPath result = null;
			graph.DrawPieRel(rectRelative, startAngle, sweepAngle, dataPointAttributes.Color, dataPointAttributes.BackHatchStyle, dataPointAttributes.BackImage, dataPointAttributes.BackImageMode, dataPointAttributes.BackImageTransparentColor, dataPointAttributes.BackImageAlign, dataPointAttributes.BackGradientType, dataPointAttributes.BackGradientEndColor, dataPointAttributes.BorderColor, dataPointAttributes.BorderWidth, dataPointAttributes.BorderStyle, PenAlignment.Inset, false, 0.0, true, doughnutRadius, false, PieDrawingStyle.Default, out result);
			if (dataPoint != null)
			{
				PieChart.Map(common, dataPoint, startAngle, sweepAngle, rectRelative, true, doughnutRadius, graph, dataPointIndex);
				dataPoint.positionRel = SunburstChart.GetSliceCenterRelative(graph, SunburstChart.GetSliceCenterAngle(startAngle, sweepAngle), centerAbsolute, radiusAbsolute);
			}
			else
			{
				SunburstChart.MapCategoryNode(common, node, startAngle, sweepAngle, rectRelative, doughnutRadius, graph);
			}
			return result;
		}

		public static void MapCategoryNode(CommonElements common, CategoryNode node, float startAngle, float sweepAngle, RectangleF rectangle, float doughnutRadius, ChartGraphics graph)
		{
			GraphicsPath path = default(GraphicsPath);
			float[] array = default(float[]);
			if (PieChart.CreateMapAreaPath(startAngle, sweepAngle, rectangle, true, doughnutRadius, graph, out path, out array))
			{
				common.HotRegionsList.AddHotRegion(graph, path, false, node.ToolTip, node.Href, "", node, ChartElementType.Nothing);
			}
		}

		private static float GetSliceCenterAngle(float startAngle, float sweepAngle)
		{
			return (float)(startAngle + sweepAngle / 2.0);
		}

		private static float NormalizeAngle(float angle)
		{
			if (angle > 360.0)
			{
				return (float)(angle - 360.0);
			}
			if (angle < 0.0)
			{
				return (float)(360.0 - angle);
			}
			return angle;
		}

		private static float GetLabelAngle(float sliceCenterAngle)
		{
			float num = SunburstChart.NormalizeAngle(sliceCenterAngle);
			if (90.0 < num && num < 270.0)
			{
				if (num < 180.0)
				{
					return (float)(num + 180.0);
				}
				return (float)(num - 180.0);
			}
			return num;
		}

		private static PointF GetSliceCenterRelative(ChartGraphics graph, float centerAngle, PointF centerAbsolute, float radiusAbsolute)
		{
			double num = (double)centerAngle * 3.1415926535897931 / 180.0;
			PointF absolute = new PointF((float)((double)centerAbsolute.X + Math.Cos(num) * (double)radiusAbsolute), (float)((double)centerAbsolute.Y + Math.Sin(num) * (double)radiusAbsolute));
			return graph.GetRelativePoint(absolute);
		}

		private static float FindOptimalWidth(float maxWidth, ChartGraphics graph, GraphicsPath sliceGraphicsPath, RectangleF labelRelativeRect, int labelRotationAngle)
		{
			RectangleF labelRelativeRect2 = new RectangleF(labelRelativeRect.Location, labelRelativeRect.Size);
			float width = labelRelativeRect.Width;
			int num = 4;
			while (graph.CanLabelFitInSlice(sliceGraphicsPath, labelRelativeRect2, labelRotationAngle) && num-- >= 0)
			{
				width = labelRelativeRect2.Width;
				labelRelativeRect2.Width = (float)(width + (maxWidth - width) / 2.0);
			}
			return width;
		}

		private static bool CanFitInResizedArea(string text, Font textFont, SizeF relativeSize, PointF sliceCenterRelative, ChartGraphics graph, GraphicsPath sliceGraphicsPath, RectangleF labelRelativeRect, int labelRotationAngle, float radiusAbsolute, out RectangleF resizedRect)
		{
			float num = relativeSize.Width / (float)text.Length;
			float num2 = (float)(relativeSize.Height / 8.0);
			float num3 = relativeSize.Width + num;
			float width = labelRelativeRect.Width;
			float height = relativeSize.Height;
			resizedRect = labelRelativeRect;
			for (int i = 2; i <= 4; i++)
			{
				float num4 = num3 / (float)i + num;
				float num5 = height * (float)i + num2;
				labelRelativeRect = new RectangleF((float)(sliceCenterRelative.X - num4 / 2.0), (float)(sliceCenterRelative.Y - num5 / 2.0), num4, num5);
				if (graph.CanLabelFitInSlice(sliceGraphicsPath, labelRelativeRect, labelRotationAngle))
				{
					labelRelativeRect.Width = SunburstChart.FindOptimalWidth(width, graph, sliceGraphicsPath, labelRelativeRect, labelRotationAngle);
					StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic);
					stringFormat.FormatFlags = SunburstChart.format.FormatFlags;
					int num6 = default(int);
					int num7 = default(int);
					((ChartRenderingEngine)graph).MeasureString(text.Replace("\\n", "\n"), textFont, labelRelativeRect.Size, stringFormat, out num6, out num7);
					if (num6 == text.Length)
					{
						resizedRect = labelRelativeRect;
						return true;
					}
				}
			}
			return false;
		}

		private static void RenderLabel(CommonElements common, ChartGraphics graph, CategoryNode node, DataPoint dataPoint, DataPointAttributes dataPointAttributes, string text, float startAngle, float sweepAngle, PointF centerAbsolute, float radiusAbsolute, int dataPointIndex, GraphicsPath sliceGraphicsPath)
		{
			if (!string.IsNullOrEmpty(text))
			{
				SizeF size = graph.MeasureString(text.Replace("\\n", "\n"), dataPointAttributes.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic));
				SizeF relativeSize = graph.GetRelativeSize(size);
				float num = relativeSize.Width / (float)text.Length;
				float num2 = relativeSize.Width + num;
				float num3 = (float)(relativeSize.Height + relativeSize.Height / 8.0);
				float sliceCenterAngle = SunburstChart.GetSliceCenterAngle(startAngle, sweepAngle);
				float labelAngle = SunburstChart.GetLabelAngle(sliceCenterAngle);
				PointF sliceCenterRelative = SunburstChart.GetSliceCenterRelative(graph, sliceCenterAngle, centerAbsolute, radiusAbsolute);
				RectangleF rectangleF = new RectangleF((float)(sliceCenterRelative.X - num2 / 2.0), (float)(sliceCenterRelative.Y - num3 / 2.0), num2, num3);
				if (!rectangleF.IsEmpty)
				{
					int num4 = (int)labelAngle + dataPointAttributes.FontAngle;
					if (!graph.CanLabelFitInSlice(sliceGraphicsPath, rectangleF, num4) && !SunburstChart.CanFitInResizedArea(text, dataPointAttributes.Font, relativeSize, sliceCenterRelative, graph, sliceGraphicsPath, rectangleF, num4, radiusAbsolute, out rectangleF))
					{
						return;
					}
					if (dataPoint != null)
					{
						graph.DrawPointLabelStringRel(common, text, dataPoint.Font, new SolidBrush(dataPoint.FontColor), rectangleF, SunburstChart.format, (int)labelAngle + dataPoint.FontAngle, rectangleF, dataPoint.LabelBackColor, dataPoint.LabelBorderColor, dataPoint.LabelBorderWidth, dataPoint.LabelBorderStyle, dataPoint.series, dataPoint, dataPointIndex);
					}
					else
					{
						graph.DrawLabelBackground(num4, sliceCenterRelative, rectangleF, dataPointAttributes.LabelBackColor, dataPointAttributes.LabelBorderColor, dataPointAttributes.LabelBorderWidth, dataPointAttributes.LabelBorderStyle);
						graph.MapCategoryNodeLabel(common, node, rectangleF);
						graph.DrawStringRel(text, dataPointAttributes.Font, new SolidBrush(dataPointAttributes.FontColor), rectangleF, SunburstChart.format, num4);
					}
				}
			}
		}

		private static List<Series> GetChartAreaSeries(string chartAreaName, SeriesCollection chartSeries)
		{
			List<Series> list = new List<Series>();
			foreach (Series item in chartSeries)
			{
				if (item.IsVisible() && !(item.ChartArea != chartAreaName))
				{
					list.Add(item);
				}
			}
			return list;
		}

		private static void SortSeriesByAbsoluteValue(List<Series> seriesCollection, CategoryNodeCollection nodes)
		{
			seriesCollection.Sort((Series series1, Series series2) => nodes.GetTotalAbsoluteValue(series2).CompareTo(nodes.GetTotalAbsoluteValue(series1)));
		}
	}
}
