using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class StepLineChart : LineChart
	{
		public override string Name
		{
			get
			{
				return "StepLine";
			}
		}

		public override Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		protected override void DrawLine(ChartGraphics graph, CommonElements common, DataPoint point, Series series, PointF[] points, int pointIndex, float tension)
		{
			if (pointIndex > 0)
			{
				PointF pointF = points[pointIndex - 1];
				PointF pointF2 = new PointF(points[pointIndex].X, points[pointIndex - 1].Y);
				PointF pointF3 = points[pointIndex];
				graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(pointF), graph.GetRelativePoint(pointF2), series.ShadowColor, series.ShadowOffset);
				graph.DrawLineRel(point.Color, point.BorderWidth, point.BorderStyle, graph.GetRelativePoint(pointF2), graph.GetRelativePoint(pointF3), series.ShadowColor, series.ShadowOffset);
				if (common.ProcessModeRegions)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddLine(pointF2, pointF3);
					if (!pointF2.Equals(pointF3))
					{
						ChartGraphics.Widen(graphicsPath, new Pen(point.Color, (float)(point.BorderWidth + 2)));
					}
					PointF pointF4 = PointF.Empty;
					float[] array = new float[graphicsPath.PointCount * 2];
					PointF[] pathPoints = graphicsPath.PathPoints;
					for (int i = 0; i < graphicsPath.PointCount; i++)
					{
						pointF4 = graph.GetRelativePoint(pathPoints[i]);
						array[2 * i] = pointF4.X;
						array[2 * i + 1] = pointF4.Y;
					}
					common.HotRegionsList.AddHotRegion(graph, graphicsPath, false, array, point, series.Name, pointIndex);
					graphicsPath = new GraphicsPath();
					graphicsPath.AddLine(pointF, pointF2);
					if (!pointF.Equals(pointF2))
					{
						ChartGraphics.Widen(graphicsPath, new Pen(point.Color, (float)(point.BorderWidth + 2)));
					}
					array = new float[graphicsPath.PointCount * 2];
					pathPoints = graphicsPath.PathPoints;
					for (int j = 0; j < graphicsPath.PointCount; j++)
					{
						pointF4 = graph.GetRelativePoint(pathPoints[j]);
						array[2 * j] = pointF4.X;
						array[2 * j + 1] = pointF4.Y;
					}
					common.HotRegionsList.AddHotRegion(graph, graphicsPath, false, array, series.Points[pointIndex - 1], series.Name, pointIndex - 1);
				}
			}
		}

		protected override GraphicsPath Draw3DSurface(ChartArea area, ChartGraphics graph, Matrix3D matrix, LightStyle lightStyle, DataPoint3D prevDataPointEx, float positionZ, float depth, ArrayList points, int pointIndex, int pointLoopIndex, float tension, DrawingOperationTypes operationType, float topDarkening, float bottomDarkening, PointF thirdPointPosition, PointF fourthPointPosition, bool clippedSegment)
		{
			GraphicsPath graphicsPath = ((operationType & DrawingOperationTypes.CalcElementPath) == DrawingOperationTypes.CalcElementPath) ? new GraphicsPath() : null;
			if (base.centerPointIndex == 2147483647)
			{
				base.centerPointIndex = base.GetCenterPointIndex(points);
			}
			DataPoint3D dataPoint3D = (DataPoint3D)points[pointIndex];
			int num = pointIndex;
			DataPoint3D dataPoint3D2 = ChartGraphics3D.FindPointByIndex(points, dataPoint3D.index - 1, base.multiSeries ? dataPoint3D : null, ref num);
			DataPoint3D dataPoint3D3 = dataPoint3D;
			if (prevDataPointEx.dataPoint.Empty)
			{
				dataPoint3D3 = prevDataPointEx;
			}
			else if (dataPoint3D2.index > dataPoint3D.index)
			{
				dataPoint3D3 = dataPoint3D2;
			}
			Color backColor = base.useBorderColor ? dataPoint3D3.dataPoint.BorderColor : dataPoint3D3.dataPoint.Color;
			ChartDashStyle borderStyle = dataPoint3D3.dataPoint.BorderStyle;
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.Color == Color.Empty)
			{
				backColor = Color.Gray;
			}
			if (dataPoint3D3.dataPoint.Empty && dataPoint3D3.dataPoint.BorderStyle == ChartDashStyle.NotSet)
			{
				borderStyle = ChartDashStyle.Solid;
			}
			DataPoint3D dataPoint3D4 = new DataPoint3D();
			dataPoint3D4.xPosition = dataPoint3D.xPosition;
			dataPoint3D4.yPosition = dataPoint3D2.yPosition;
			bool flag = true;
			if (pointIndex + 1 < points.Count)
			{
				DataPoint3D dataPoint3D5 = (DataPoint3D)points[pointIndex + 1];
				if (dataPoint3D5.index == dataPoint3D2.index)
				{
					flag = false;
				}
			}
			if (base.centerPointIndex != 2147483647 && pointIndex >= base.centerPointIndex)
			{
				flag = false;
			}
			GraphicsPath graphicsPath2;
			GraphicsPath graphicsPath3;
			if (flag)
			{
				dataPoint3D4.dataPoint = dataPoint3D.dataPoint;
				graphicsPath2 = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D2, dataPoint3D4, points, pointIndex, 0f, operationType, LineSegmentType.First, (byte)(base.showPointLines ? 1 : 0) != 0, false, area.reverseSeriesOrder, base.multiSeries, 0, true);
				graph.frontLinePen = null;
				dataPoint3D4.dataPoint = dataPoint3D2.dataPoint;
				graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D4, dataPoint3D, points, pointIndex, 0f, operationType, LineSegmentType.Last, (byte)(base.showPointLines ? 1 : 0) != 0, false, area.reverseSeriesOrder, base.multiSeries, 0, true);
				graph.frontLinePen = null;
			}
			else
			{
				dataPoint3D4.dataPoint = dataPoint3D2.dataPoint;
				graphicsPath3 = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D4, dataPoint3D, points, pointIndex, 0f, operationType, LineSegmentType.Last, (byte)(base.showPointLines ? 1 : 0) != 0, false, area.reverseSeriesOrder, base.multiSeries, 0, true);
				graph.frontLinePen = null;
				dataPoint3D4.dataPoint = dataPoint3D.dataPoint;
				graphicsPath2 = graph.Draw3DSurface(area, matrix, lightStyle, SurfaceNames.Top, positionZ, depth, backColor, dataPoint3D3.dataPoint.BorderColor, dataPoint3D3.dataPoint.BorderWidth, borderStyle, dataPoint3D2, dataPoint3D4, points, pointIndex, 0f, operationType, LineSegmentType.First, (byte)(base.showPointLines ? 1 : 0) != 0, false, area.reverseSeriesOrder, base.multiSeries, 0, true);
				graph.frontLinePen = null;
			}
			if (graphicsPath != null)
			{
				if (area.Common.ProcessModeRegions && graphicsPath2 != null && graphicsPath2.PointCount > 0)
				{
					area.Common.HotRegionsList.AddHotRegion(graphicsPath2, false, graph, prevDataPointEx.dataPoint, prevDataPointEx.dataPoint.series.Name, prevDataPointEx.index - 1);
				}
				if (graphicsPath3 != null && graphicsPath3.PointCount > 0)
				{
					graphicsPath.AddPath(graphicsPath3, true);
				}
			}
			return graphicsPath;
		}
	}
}
