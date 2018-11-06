using System;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class PyramidChart : FunnelChart
	{
		public override string Name
		{
			get
			{
				return "Pyramid";
			}
		}

		public PyramidChart()
		{
			base.isPyramid = true;
			base.round3DShape = false;
			base.funnelLabelStyleAttributeName = "PyramidLabelStyle";
			base.funnelPointGapAttributeName = "PyramidPointGap";
			base.funnelRotationAngleAttributeName = "Pyramid3DRotationAngle";
			base.funnelPointMinHeight = "PyramidMinPointHeight";
			base.funnel3DDrawingStyleAttributeName = "Pyramid3DDrawingStyle";
			base.funnelInsideLabelAlignmentAttributeName = "PyramidInsideLabelAlignment";
			base.funnelOutsideLabelPlacementAttributeName = "PyramidOutsideLabelPlacement";
		}

		protected override void GetPointWidthAndHeight(Series series, int pointIndex, float location, out float height, out float startWidth, out float endWidth)
		{
			PointF empty = PointF.Empty;
			RectangleF absoluteRectangle = base.graph.GetAbsoluteRectangle(base.plotAreaPosition);
			float num = absoluteRectangle.Height - base.funnelSegmentGap * (float)(base.pointNumber - (this.ShouldDrawFirstPoint() ? 1 : 2));
			if (num < 0.0)
			{
				num = 0f;
			}
			height = (float)((double)num * (this.GetYValue(series.Points[pointIndex], pointIndex) / base.yValueTotal));
			height = base.CheckMinHeight(height);
			PointF linesIntersection = ChartGraphics3D.GetLinesIntersection(absoluteRectangle.X, location - height, absoluteRectangle.Right, location - height, absoluteRectangle.X, absoluteRectangle.Bottom, (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), absoluteRectangle.Y);
			if (linesIntersection.X > absoluteRectangle.X + absoluteRectangle.Width / 2.0)
			{
				linesIntersection.X = (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0);
			}
			PointF linesIntersection2 = ChartGraphics3D.GetLinesIntersection(absoluteRectangle.X, location, absoluteRectangle.Right, location, absoluteRectangle.X, absoluteRectangle.Bottom, (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), absoluteRectangle.Y);
			if (linesIntersection2.X > absoluteRectangle.X + absoluteRectangle.Width / 2.0)
			{
				linesIntersection2.X = (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0);
			}
			startWidth = (float)(Math.Abs((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0 - linesIntersection.X)) * 2.0);
			endWidth = (float)(Math.Abs((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0 - linesIntersection2.X)) * 2.0);
			empty = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(location - height / 2.0));
			series.Points[pointIndex].positionRel = base.graph.GetRelativePoint(empty);
		}
	}
}
