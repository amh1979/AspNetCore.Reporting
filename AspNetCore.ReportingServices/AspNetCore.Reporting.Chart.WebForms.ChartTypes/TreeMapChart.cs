using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal sealed class TreeMapChart : IChartType
	{
		private static float ChartAreaMargin = 5f;

		private static float DataPointMargin = 1f;

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
			double value = default(double);
			List<TreeMapNode> list = default(List<TreeMapNode>);
			TreeMapChart.BuildTreeNodes(common, area, out value, out list);
			RectangleF plottingArea = TreeMapChart.GetPlottingArea(graph, area);
			TreeMapSquaringAlgorithm.CalculateRectangles(plottingArea, list, value);
			graph.SetClip(graph.GetRelativeRectangle(plottingArea));
			TreeMapChart.RenderDataPoints(graph, common, list);
			TreeMapChart.RenderLabels(graph, area, list);
			graph.ResetClip();
		}

		private static void BuildTreeNodes(CommonElements common, ChartArea area, out double chartTotal, out List<TreeMapNode> seriesTreeMapNodes)
		{
			chartTotal = 0.0;
			seriesTreeMapNodes = new List<TreeMapNode>();
			foreach (Series item in common.DataManager.Series)
			{
				if (item.IsVisible() && !(item.ChartArea != area.Name))
				{
					double num = 0.0;
					List<TreeMapNode> list = new List<TreeMapNode>();
					foreach (DataPoint point in item.Points)
					{
						TreeMapNode treeMapNode = new TreeMapNode(point);
						list.Add(treeMapNode);
						num += treeMapNode.Value;
					}
					TreeMapNode treeMapNode2 = new TreeMapNode(item, num);
					treeMapNode2.Children = list;
					seriesTreeMapNodes.Add(treeMapNode2);
					chartTotal += treeMapNode2.Value;
				}
			}
		}

		private static void RenderDataPoints(ChartGraphics graph, CommonElements common, List<TreeMapNode> seriesTreeMapNodes)
		{
			foreach (TreeMapNode seriesTreeMapNode in seriesTreeMapNodes)
			{
				int num = 0;
				foreach (TreeMapNode child in seriesTreeMapNode.Children)
				{
					RectangleF rectangle = child.Rectangle;
					TreeMapChart.RenderDataPoint(graph, common, num, child);
					if (seriesTreeMapNode.DataPoint == null && !child.DataPoint.Empty)
					{
						seriesTreeMapNode.DataPoint = child.DataPoint;
					}
					num++;
				}
			}
		}

		private static void RenderDataPoint(ChartGraphics graph, CommonElements common, int index, TreeMapNode dataPointTreeMapNode)
		{
			RectangleF relativeRect = TreeMapChart.GetRelativeRect(graph, dataPointTreeMapNode);
			DataPoint dataPoint = dataPointTreeMapNode.DataPoint;
			graph.FillRectangleRel(relativeRect, dataPoint.Color, dataPoint.BackHatchStyle, dataPoint.BackImage, dataPoint.BackImageMode, dataPoint.BackImageTransparentColor, dataPoint.BackImageAlign, dataPoint.BackGradientType, dataPoint.BackGradientEndColor, dataPoint.BorderColor, dataPoint.BorderWidth, dataPoint.BorderStyle, dataPoint.series.ShadowColor, dataPoint.series.ShadowOffset, PenAlignment.Inset, ChartGraphics.GetBarDrawingStyle(dataPoint), true);
			TreeMapChart.AddDataPointHotRegion(graph, common, index, dataPoint, relativeRect);
		}

		private static void AddDataPointHotRegion(ChartGraphics graph, CommonElements common, int index, DataPoint point, RectangleF dataPointRelativeRect)
		{
			common.HotRegionsList.AddHotRegion(graph, dataPointRelativeRect, point, point.series.Name, index);
			point.positionRel = new PointF((float)(dataPointRelativeRect.X + dataPointRelativeRect.Width / 2.0), dataPointRelativeRect.Y);
		}

		private static void RenderLabels(ChartGraphics graph, ChartArea area, List<TreeMapNode> seriesTreeMapNodes)
		{
			foreach (TreeMapNode seriesTreeMapNode in seriesTreeMapNodes)
			{
				if (seriesTreeMapNode.DataPoint != null)
				{
					RectangleF relativeRect = TreeMapChart.GetRelativeRect(graph, seriesTreeMapNode);
					RectangleF rectangleF = TreeMapChart.GetSeriesLabelRelativeRect(graph, area, seriesTreeMapNode.Series, relativeRect, seriesTreeMapNode.DataPoint);
					if (!TreeMapChart.CanLabelFit(relativeRect, rectangleF))
					{
						rectangleF = RectangleF.Empty;
					}
					int num = 0;
					foreach (TreeMapNode child in seriesTreeMapNode.Children)
					{
						DataPoint dataPoint = child.DataPoint;
						RectangleF rectangle = child.Rectangle;
						TreeMapChart.RenderDataPointLabel(graph, area, num, child, rectangleF);
						num++;
					}
					TreeMapChart.RenderSeriesLabel(graph, seriesTreeMapNode, rectangleF);
				}
			}
		}

		private static void RenderSeriesLabel(ChartGraphics graph, TreeMapNode seriesTreeMapNode, RectangleF labelRelativeRect)
		{
			if (!labelRelativeRect.IsEmpty)
			{
				Series series = seriesTreeMapNode.Series;
				DataPoint dataPoint = seriesTreeMapNode.DataPoint;
				using (Font font = TreeMapChart.GetSeriesLabelFont(dataPoint))
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Near;
					stringFormat.LineAlignment = StringAlignment.Near;
					graph.DrawStringRel(series.legendText, font, new SolidBrush(dataPoint.FontColor), labelRelativeRect.Location, stringFormat, 0);
				}
			}
		}

		private static void RenderDataPointLabel(ChartGraphics graph, ChartArea area, int index, TreeMapNode dataPointTreeMapNode, RectangleF seriesLabelRelativeRect)
		{
			RectangleF relativeRect = TreeMapChart.GetRelativeRect(graph, dataPointTreeMapNode);
			string labelText = TreeMapChart.GetLabelText(dataPointTreeMapNode.DataPoint);
			TreeMapChart.RenderDataPointLabel(graph, area, index, dataPointTreeMapNode, labelText, TreeMapChart.GetDataPointLabelRelativeRect(graph, dataPointTreeMapNode, relativeRect, labelText), relativeRect, seriesLabelRelativeRect);
		}

		private static void RenderDataPointLabel(ChartGraphics graph, ChartArea area, int index, TreeMapNode dataPointTreeMapNode, string text, RectangleF labelRelativeRect, RectangleF dataPointRelativeRect, RectangleF seriesLabelRelativeRect)
		{
			if (!labelRelativeRect.IsEmpty && TreeMapChart.CanLabelFit(dataPointRelativeRect, labelRelativeRect) && !labelRelativeRect.IntersectsWith(seriesLabelRelativeRect))
			{
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Near;
				stringFormat.LineAlignment = StringAlignment.Near;
				DataPoint dataPoint = dataPointTreeMapNode.DataPoint;
				graph.DrawPointLabelStringRel(area.Common, text, dataPoint.Font, new SolidBrush(dataPoint.FontColor), labelRelativeRect.Location, stringFormat, dataPoint.FontAngle, labelRelativeRect, dataPoint.LabelBackColor, dataPoint.LabelBorderColor, dataPoint.LabelBorderWidth, dataPoint.LabelBorderStyle, dataPoint.series, dataPoint, index);
			}
		}

		private static RectangleF GetPlottingArea(ChartGraphics graph, ChartArea area)
		{
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(area.Position.ToRectangleF());
			absoluteRectangle.X += TreeMapChart.ChartAreaMargin;
			absoluteRectangle.Y += TreeMapChart.ChartAreaMargin;
			absoluteRectangle.Width -= (float)(2.0 * TreeMapChart.ChartAreaMargin);
			absoluteRectangle.Height -= (float)(2.0 * TreeMapChart.ChartAreaMargin);
			return absoluteRectangle;
		}

		private static RectangleF GetRelativeRect(ChartGraphics graph, TreeMapNode treeMapNode)
		{
			return graph.GetRelativeRectangle(new RectangleF(treeMapNode.Rectangle.X, treeMapNode.Rectangle.Y, treeMapNode.Rectangle.Width - TreeMapChart.DataPointMargin, treeMapNode.Rectangle.Height - TreeMapChart.DataPointMargin));
		}

		private static RectangleF GetSeriesLabelRelativeRect(ChartGraphics graph, ChartArea area, Series series, RectangleF seriesRelativeRect, DataPoint point)
		{
			if (!string.IsNullOrEmpty(series.legendText) && TreeMapChart.IsLabelVisible(point))
			{
				using (Font font = TreeMapChart.GetSeriesLabelFont(point))
				{
					return TreeMapChart.GetLabelRelativeRect(graph, font, seriesRelativeRect, series.legendText, LabelAlignmentTypes.TopLeft);
				}
			}
			return RectangleF.Empty;
		}

		private static RectangleF GetDataPointLabelRelativeRect(ChartGraphics graph, TreeMapNode dataPointTreeMapNode, RectangleF dataPointRelativeRect, string text)
		{
			DataPoint dataPoint = dataPointTreeMapNode.DataPoint;
			return TreeMapChart.GetLabelRelativeRect(graph, dataPoint.Font, dataPointRelativeRect, text, TreeMapChart.GetLabelAlignment(dataPoint));
		}

		private static RectangleF GetLabelRelativeRect(ChartGraphics graph, Font font, RectangleF treeMapNodeRelativeRect, string text, LabelAlignmentTypes labelAlignment)
		{
			if (string.IsNullOrEmpty(text))
			{
				return RectangleF.Empty;
			}
			SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(text.Replace("\\n", "\n"), font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic)));
			float num = relativeSize.Width + relativeSize.Width / (float)text.Length;
			float num2 = (float)(relativeSize.Height + relativeSize.Height / 8.0);
			return new RectangleF(TreeMapChart.GetLabelXPosition(treeMapNodeRelativeRect, num, labelAlignment), TreeMapChart.GetLabelYPosition(treeMapNodeRelativeRect, num2, labelAlignment), num, num2);
		}

		private static bool CanLabelFit(RectangleF containerRelativeRect, RectangleF labelRelativeRect)
		{
			if (labelRelativeRect.Width <= containerRelativeRect.Width)
			{
				return labelRelativeRect.Height <= containerRelativeRect.Height;
			}
			return false;
		}

		private static Font GetSeriesLabelFont(DataPoint point)
		{
			return new Font(point.Font, point.Font.Style | FontStyle.Bold);
		}

		public static bool IsLabelVisible(DataPointAttributes point)
		{
			if (point.IsAttributeSet("LabelsVisible"))
			{
				return !string.Equals(point.GetAttribute("LabelsVisible"), "false", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}

		private static float GetLabelXPosition(RectangleF treeMapNodeRelativeRect, float labelRelativeWidth, LabelAlignmentTypes labelAlignment)
		{
			float result = 0f;
			switch (labelAlignment)
			{
			case LabelAlignmentTypes.Top:
			case LabelAlignmentTypes.Bottom:
			case LabelAlignmentTypes.Center:
				result = (float)(treeMapNodeRelativeRect.X + (treeMapNodeRelativeRect.Width - labelRelativeWidth) / 2.0);
				break;
			case LabelAlignmentTypes.Left:
			case LabelAlignmentTypes.TopLeft:
			case LabelAlignmentTypes.BottomLeft:
				result = treeMapNodeRelativeRect.X;
				break;
			case LabelAlignmentTypes.Right:
			case LabelAlignmentTypes.TopRight:
			case LabelAlignmentTypes.BottomRight:
				result = treeMapNodeRelativeRect.X + treeMapNodeRelativeRect.Width - labelRelativeWidth;
				break;
			}
			return result;
		}

		private static float GetLabelYPosition(RectangleF treeMapNodeRelativeRect, float labelRelativeHeight, LabelAlignmentTypes labelAlignment)
		{
			float result = 0f;
			switch (labelAlignment)
			{
			case LabelAlignmentTypes.Bottom:
			case LabelAlignmentTypes.BottomLeft:
			case LabelAlignmentTypes.BottomRight:
				result = treeMapNodeRelativeRect.Y + treeMapNodeRelativeRect.Height - labelRelativeHeight;
				break;
			case LabelAlignmentTypes.Right:
			case LabelAlignmentTypes.Left:
			case LabelAlignmentTypes.Center:
				result = (float)(treeMapNodeRelativeRect.Y + (treeMapNodeRelativeRect.Height - labelRelativeHeight) / 2.0);
				break;
			case LabelAlignmentTypes.Top:
			case LabelAlignmentTypes.TopLeft:
			case LabelAlignmentTypes.TopRight:
				result = treeMapNodeRelativeRect.Y;
				break;
			}
			return result;
		}

		private static LabelAlignmentTypes GetLabelAlignment(DataPoint point)
		{
			string text = ((DataPointAttributes)point)["LabelStyle"];
			if (text != null && text.Length > 0)
			{
				if (string.Compare(text, "Center", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.Center;
				}
				if (string.Compare(text, "Bottom", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.Bottom;
				}
				if (string.Compare(text, "TopLeft", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.TopLeft;
				}
				if (string.Compare(text, "TopRight", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.TopRight;
				}
				if (string.Compare(text, "BottomLeft", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.BottomLeft;
				}
				if (string.Compare(text, "BottomRight", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.BottomRight;
				}
				if (string.Compare(text, "Left", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.Left;
				}
				if (string.Compare(text, "Right", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.Right;
				}
				if (string.Compare(text, "Top", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return LabelAlignmentTypes.Top;
				}
			}
			return LabelAlignmentTypes.BottomLeft;
		}

		private static string GetLabelText(DataPoint point)
		{
			if (!TreeMapChart.IsLabelVisible(point))
			{
				return string.Empty;
			}
			return PieChart.GetLabelText(point, true);
		}
	}
}
