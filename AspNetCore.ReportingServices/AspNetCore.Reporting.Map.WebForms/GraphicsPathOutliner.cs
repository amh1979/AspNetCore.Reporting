using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class GraphicsPathOutliner
	{
		private RectangleF[] boundsArray;

		private Hashtable pointsTable;

		private GraphicsPath[] paths;

		private Graphics graphics;

		private Hashtable visitedPoints;

		public GraphicsPathOutliner(Graphics graphics)
		{
			this.graphics = graphics;
		}

		public GraphicsPath GetOutlinePath(GraphicsPath[] paths)
		{
			this.paths = paths;
			Pen pen = new Pen(Brushes.Blue, 0f);
			RectangleF a = RectangleF.Empty;
			this.boundsArray = new RectangleF[paths.Length];
			this.pointsTable = new Hashtable();
			for (int i = 0; i < paths.Length; i++)
			{
				this.boundsArray[i] = paths[i].GetBounds();
				a = ((!a.IsEmpty) ? RectangleF.Union(a, this.boundsArray[i]) : this.boundsArray[i]);
			}
			this.visitedPoints = new Hashtable();
			PointInfo currentPoint = default(PointInfo);
			int num = 0;
			while (num < paths.Length)
			{
				if (this.boundsArray[num].X != a.X)
				{
					num++;
					continue;
				}
				currentPoint.Path = paths[num];
				currentPoint.Points = paths[num].PathPoints;
				break;
			}
			this.pointsTable.Add(currentPoint.Path, currentPoint.Points);
			int num2 = 0;
			while (num2 < currentPoint.Points.Length)
			{
				if (currentPoint.Points[num2].X != a.X)
				{
					num2++;
					continue;
				}
				currentPoint.Point = currentPoint.Points[num2];
				currentPoint.Index = num2;
				break;
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(currentPoint.Point);
			PointF previousPoint = new PointF(a.X - a.Width, (float)(a.Y + 2.0 * a.Height));
			PointInfo currentPoint2 = this.GetNextPoint(currentPoint, previousPoint, pen);
			previousPoint = currentPoint.Point;
			while (arrayList.Count < 3000 && currentPoint2.Point != currentPoint.Point)
			{
				arrayList.Add(currentPoint2.Point);
				if (!this.visitedPoints.Contains(currentPoint2.Point))
				{
					this.visitedPoints.Add(currentPoint2.Point, null);
				}
				PointInfo nextPoint = this.GetNextPoint(currentPoint2, previousPoint, pen);
				previousPoint = currentPoint2.Point;
				currentPoint2 = nextPoint;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.StartFigure();
			graphicsPath.AddPolygon((PointF[])arrayList.ToArray(typeof(PointF)));
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		private PointInfo GetNextPoint(PointInfo currentPoint, PointF previousPoint, Pen pen)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < this.boundsArray.Length; i++)
			{
				if (this.paths[i] != currentPoint.Path)
				{
					RectangleF rectangleF = this.boundsArray[i];
					rectangleF.Inflate(2f, 2f);
					if (rectangleF.Contains(currentPoint.Point) && (this.paths[i].IsOutlineVisible(currentPoint.Point, pen) || this.paths[i].IsVisible(currentPoint.Point)))
					{
						this.DrawMarker(currentPoint.Point, 2f);
						PointF[] pathPoints = this.GetPathPoints(this.paths[i]);
						PointInfo closestPoint = this.GetClosestPoint(currentPoint.Point, pathPoints, this.paths[i]);
						arrayList.Add(closestPoint);
					}
				}
			}
			if (arrayList.Count == 0)
			{
				return currentPoint.GetNextPoint(currentPoint.Direction);
			}
			PointInfo result = currentPoint.GetNextPoint(currentPoint.Direction);
			double num = this.CalculateAngle(previousPoint, currentPoint.Point, result.Point);
			foreach (PointInfo item in arrayList)
			{
				PointInfo nextPoint = item.GetNextPoint(Direction.Forward);
				double num2 = this.CalculateAngle(previousPoint, currentPoint.Point, nextPoint.Point);
				PointInfo nextPoint2 = item.GetNextPoint(Direction.Backward);
				double num3 = this.CalculateAngle(previousPoint, currentPoint.Point, nextPoint2.Point);
				if (num2 > num && !this.visitedPoints.Contains(nextPoint.Point))
				{
					num = num2;
					result = nextPoint;
				}
				if (num3 > num && !this.visitedPoints.Contains(nextPoint2.Point))
				{
					num = num3;
					result = nextPoint2;
				}
			}
			return result;
		}

		private void DrawMarker(PointF point, float size)
		{
			RectangleF rect = new RectangleF(point.X, point.Y, 0f, 0f);
			rect.Inflate((float)(size / 5.0 + 2.0), (float)(size / 5.0 + 2.0));
			this.graphics.FillEllipse(Brushes.Blue, rect);
		}

		private double CalculateAngle(PointF previousPoint, PointF point, PointF forwardPoint)
		{
			double num = Math.Atan2((double)(previousPoint.Y - point.Y), (double)(previousPoint.X - point.X));
			if (num < 0.0)
			{
				num += 6.2831853071795862;
			}
			double num2 = Math.Atan2((double)(forwardPoint.Y - point.Y), (double)(forwardPoint.X - point.X));
			if (num2 < 0.0)
			{
				num2 += 6.2831853071795862;
			}
			double num3 = num - num2;
			if (num3 < 0.0)
			{
				num3 += 6.2831853071795862;
			}
			return num3;
		}

		private PointInfo GetClosestPoint(PointF point, PointF[] points, GraphicsPath graphicsPath)
		{
			PointInfo result = default(PointInfo);
			result.Points = points;
			result.Path = graphicsPath;
			double num = double.PositiveInfinity;
			for (int i = 0; i < points.Length; i++)
			{
				double num2 = (double)(points[i].X - point.X);
				double num3 = (double)(points[i].Y - point.Y);
				double num4 = num2 * num2 + num3 * num3;
				if (num4 < num)
				{
					result.Point = points[i];
					result.Index = i;
					num = num4;
				}
			}
			return result;
		}

		private PointF[] GetPathPoints(GraphicsPath graphicsPath)
		{
			if (this.pointsTable.Contains(graphicsPath))
			{
				return (PointF[])this.pointsTable[graphicsPath];
			}
			PointF[] pathPoints = graphicsPath.PathPoints;
			this.pointsTable.Add(graphicsPath, pathPoints);
			return pathPoints;
		}
	}
}
