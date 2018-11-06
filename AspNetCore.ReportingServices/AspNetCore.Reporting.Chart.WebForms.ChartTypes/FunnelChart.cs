using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal class FunnelChart : IChartType
	{
		protected ArrayList segmentList;

		protected ArrayList labelInfoList;

		protected ChartGraphics graph;

		protected ChartArea area;

		protected CommonElements common;

		protected RectangleF plotAreaSpacing = new RectangleF(3f, 3f, 3f, 3f);

		private Series chartTypeSeries;

		protected double yValueTotal;

		private double yValueMax;

		private double xValueTotal;

		protected int pointNumber;

		protected RectangleF plotAreaPosition = RectangleF.Empty;

		private FunnelStyle funnelStyle;

		private SizeF funnelNeckSize = new SizeF(50f, 30f);

		protected float funnelSegmentGap;

		private int rotation3D = 5;

		protected bool round3DShape = true;

		protected bool isPyramid;

		private float funnelMinPointHeight;

		protected string funnelPointGapAttributeName = "FunnelPointGap";

		protected string funnelRotationAngleAttributeName = "Funnel3DRotationAngle";

		protected string funnelPointMinHeight = "FunnelMinPointHeight";

		protected string funnel3DDrawingStyleAttributeName = "Funnel3DDrawingStyle";

		protected string funnelInsideLabelAlignmentAttributeName = "FunnelInsideLabelAlignment";

		protected string funnelOutsideLabelPlacementAttributeName = "FunnelOutsideLabelPlacement";

		protected string funnelLabelStyleAttributeName = "FunnelLabelStyle";

		private double[] valuePercentages;

		public virtual string Name
		{
			get
			{
				return "Funnel";
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

		public virtual bool SecondYScale
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
				return true;
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

		public virtual bool DataPointsInLegend
		{
			get
			{
				return true;
			}
		}

		public virtual bool ZeroCrossing
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

		public virtual int YValuesPerPoint
		{
			get
			{
				return 1;
			}
		}

		public virtual LegendImageStyle GetLegendImageStyle(Series series)
		{
			return LegendImageStyle.Rectangle;
		}

		public virtual Image GetImage(ChartTypeRegistry registry)
		{
			return (Image)registry.ResourceManager.GetObject(this.Name + "ChartType");
		}

		public virtual void Paint(ChartGraphics graph, CommonElements common, ChartArea area, Series seriesToDraw)
		{
			this.chartTypeSeries = null;
			this.funnelMinPointHeight = 0f;
			this.graph = graph;
			this.common = common;
			this.area = area;
			this.GetDataPointValuesStatistic();
			if (this.yValueTotal != 0.0 && this.pointNumber != 0)
			{
				this.funnelStyle = this.GetFunnelStyle(this.GetDataSeries());
				if (this.funnelStyle == FunnelStyle.YIsWidth && this.pointNumber == 1)
				{
					return;
				}
				this.GetFunnelMinPointHeight(this.GetDataSeries());
				this.labelInfoList = this.CreateLabelsInfoList();
				this.GetPlotAreaSpacing();
				this.ProcessChartType();
				this.DrawLabels();
			}
		}

		private void ProcessChartType()
		{
			if (this.area.Area3DStyle.Enable3D)
			{
				if (this.rotation3D > 0 && !this.isPyramid)
				{
					goto IL_0034;
				}
				if (this.rotation3D < 0 && this.isPyramid)
				{
					goto IL_0034;
				}
			}
			goto IL_003f;
			IL_003f:
			bool flag = true;
			bool flag2 = (byte)((!this.area.Area3DStyle.Enable3D) ? 1 : 0) != 0;
			Series dataSeries = this.GetDataSeries();
			if (flag2 && flag && dataSeries != null && dataSeries.ShadowOffset != 0)
			{
				foreach (FunnelSegmentInfo segment in this.segmentList)
				{
					this.DrawFunnelCircularSegment(segment.Point, segment.PointIndex, segment.StartWidth, segment.EndWidth, segment.Location, segment.Height, segment.NothingOnTop, segment.NothingOnBottom, false, true);
				}
				flag2 = false;
			}
			foreach (FunnelSegmentInfo segment2 in this.segmentList)
			{
				this.DrawFunnelCircularSegment(segment2.Point, segment2.PointIndex, segment2.StartWidth, segment2.EndWidth, segment2.Location, segment2.Height, segment2.NothingOnTop, segment2.NothingOnBottom, true, flag2);
			}
			return;
			IL_0034:
			this.segmentList.Reverse();
			goto IL_003f;
		}

		protected virtual void GetPointWidthAndHeight(Series series, int pointIndex, float location, out float height, out float startWidth, out float endWidth)
		{
			PointF empty = PointF.Empty;
			RectangleF absoluteRectangle = this.graph.GetAbsoluteRectangle(this.plotAreaPosition);
			float num = absoluteRectangle.Height - this.funnelSegmentGap * (float)(this.pointNumber - (this.ShouldDrawFirstPoint() ? 1 : 2));
			if (num < 0.0)
			{
				num = 0f;
			}
			if (this.funnelStyle == FunnelStyle.YIsWidth)
			{
				if (this.xValueTotal == 0.0)
				{
					height = num / (float)(this.pointNumber - 1);
				}
				else
				{
					height = (float)((double)num * (this.GetXValue(series.Points[pointIndex]) / this.xValueTotal));
				}
				height = this.CheckMinHeight(height);
				startWidth = (float)((double)absoluteRectangle.Width * (this.GetYValue(series.Points[pointIndex - 1], pointIndex - 1) / this.yValueMax));
				endWidth = (float)((double)absoluteRectangle.Width * (this.GetYValue(series.Points[pointIndex], pointIndex) / this.yValueMax));
				empty = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), location + height);
				goto IL_02aa;
			}
			if (this.funnelStyle == FunnelStyle.YIsHeight)
			{
				height = (float)((double)num * (this.GetYValue(series.Points[pointIndex], pointIndex) / this.yValueTotal));
				height = this.CheckMinHeight(height);
				PointF linesIntersection = ChartGraphics3D.GetLinesIntersection(absoluteRectangle.X, location, absoluteRectangle.Right, location, absoluteRectangle.X, absoluteRectangle.Y, (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0 - this.funnelNeckSize.Width / 2.0), absoluteRectangle.Bottom - this.funnelNeckSize.Height);
				PointF linesIntersection2 = ChartGraphics3D.GetLinesIntersection(absoluteRectangle.X, location + height, absoluteRectangle.Right, location + height, absoluteRectangle.X, absoluteRectangle.Y, (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0 - this.funnelNeckSize.Width / 2.0), absoluteRectangle.Bottom - this.funnelNeckSize.Height);
				startWidth = (float)((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0 - linesIntersection.X) * 2.0);
				endWidth = (float)((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0 - linesIntersection2.X) * 2.0);
				empty = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0), (float)(location + height / 2.0));
				goto IL_02aa;
			}
			throw new InvalidOperationException(SR.ExceptionFunnelStyleUnknown(this.funnelStyle.ToString()));
			IL_02aa:
			series.Points[pointIndex].positionRel = this.graph.GetRelativePoint(empty);
		}

		protected virtual bool ShouldDrawFirstPoint()
		{
			if (this.funnelStyle != 0)
			{
				return this.isPyramid;
			}
			return true;
		}

		private void DrawFunnel3DSquareSegment(DataPoint point, int pointIndex, float startWidth, float endWidth, float location, float height, bool nothingOnTop, bool nothingOnBottom, bool drawSegment, bool drawSegmentShadow)
		{
			if (!nothingOnBottom)
			{
				height = (float)(height + 0.30000001192092896);
			}
			Color gradientColor = ChartGraphics.GetGradientColor(point.Color, Color.White, 0.3);
			Color gradientColor2 = ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.3);
			if (this.funnelStyle == FunnelStyle.YIsHeight && !this.isPyramid)
			{
				if (startWidth < this.funnelNeckSize.Width)
				{
					startWidth = this.funnelNeckSize.Width;
				}
				if (endWidth < this.funnelNeckSize.Width)
				{
					endWidth = this.funnelNeckSize.Width;
				}
			}
			float num = (float)(startWidth / 2.0 * Math.Sin((float)this.rotation3D / 180.0 * 3.1415926535897931));
			float num2 = (float)(endWidth / 2.0 * Math.Sin((float)this.rotation3D / 180.0 * 3.1415926535897931));
			RectangleF absoluteRectangle = this.graph.GetAbsoluteRectangle(this.plotAreaPosition);
			float num3 = (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0);
			this.graph.StartHotRegion(point);
			this.graph.StartAnimation();
			GraphicsPath graphicsPath = new GraphicsPath();
			if (startWidth > 0.0)
			{
				graphicsPath.AddLine((float)(num3 - startWidth / 2.0), location, num3, location + num);
			}
			graphicsPath.AddLine(num3, location + num, num3, location + height + num2);
			if (endWidth > 0.0)
			{
				graphicsPath.AddLine(num3, location + height + num2, (float)(num3 - endWidth / 2.0), location + height);
			}
			graphicsPath.AddLine((float)(num3 - endWidth / 2.0), location + height, (float)(num3 - startWidth / 2.0), location);
			if (this.common.ProcessModePaint)
			{
				this.graph.DrawPathAbs(graphicsPath, drawSegment ? gradientColor : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
			}
			if (this.common.ProcessModeRegions)
			{
				this.common.HotRegionsList.AddHotRegion(graphicsPath, false, this.graph, point, point.series.Name, pointIndex);
			}
			else
			{
				graphicsPath.Dispose();
			}
			graphicsPath = new GraphicsPath();
			if (startWidth > 0.0)
			{
				graphicsPath.AddLine((float)(num3 + startWidth / 2.0), location, num3, location + num);
			}
			graphicsPath.AddLine(num3, location + num, num3, location + height + num2);
			if (endWidth > 0.0)
			{
				graphicsPath.AddLine(num3, location + height + num2, (float)(num3 + endWidth / 2.0), location + height);
			}
			graphicsPath.AddLine((float)(num3 + endWidth / 2.0), location + height, (float)(num3 + startWidth / 2.0), location);
			if (this.common.ProcessModePaint)
			{
				this.graph.DrawPathAbs(graphicsPath, drawSegment ? gradientColor2 : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
			}
			if (this.common.ProcessModeRegions)
			{
				this.common.HotRegionsList.AddHotRegion(graphicsPath, false, this.graph, point, point.series.Name, pointIndex);
			}
			else
			{
				graphicsPath.Dispose();
			}
			if ((float)this.rotation3D > 0.0 && startWidth > 0.0 && nothingOnTop && this.area.Area3DStyle.Enable3D)
			{
				PointF[] points = new PointF[4]
				{
					new PointF((float)(num3 + startWidth / 2.0), location),
					new PointF(num3, location + num),
					new PointF((float)(num3 - startWidth / 2.0), location),
					new PointF(num3, location - num)
				};
				GraphicsPath graphicsPath2 = new GraphicsPath();
				graphicsPath2.AddLines(points);
				graphicsPath2.CloseAllFigures();
				if (this.common.ProcessModePaint)
				{
					this.graph.DrawPathAbs(graphicsPath2, drawSegment ? ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4) : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
				}
				if (this.common.ProcessModeRegions)
				{
					this.common.HotRegionsList.AddHotRegion(graphicsPath2, false, this.graph, point, point.series.Name, pointIndex);
				}
				else
				{
					graphicsPath2.Dispose();
				}
			}
			if ((float)this.rotation3D < 0.0 && startWidth > 0.0 && nothingOnBottom && this.area.Area3DStyle.Enable3D)
			{
				PointF[] points2 = new PointF[4]
				{
					new PointF((float)(num3 + endWidth / 2.0), location + height),
					new PointF(num3, location + height + num2),
					new PointF((float)(num3 - endWidth / 2.0), location + height),
					new PointF(num3, location + height - num2)
				};
				GraphicsPath graphicsPath3 = new GraphicsPath();
				graphicsPath3.AddLines(points2);
				graphicsPath3.CloseAllFigures();
				if (this.common.ProcessModePaint)
				{
					this.graph.DrawPathAbs(graphicsPath3, drawSegment ? ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4) : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
				}
				if (this.common.ProcessModeRegions)
				{
					this.common.HotRegionsList.AddHotRegion(graphicsPath3, false, this.graph, point, point.series.Name, pointIndex);
				}
				else
				{
					graphicsPath3.Dispose();
				}
			}
			this.graph.StopAnimation();
			this.graph.EndHotRegion();
		}

		private void DrawFunnelCircularSegment(DataPoint point, int pointIndex, float startWidth, float endWidth, float location, float height, bool nothingOnTop, bool nothingOnBottom, bool drawSegment, bool drawSegmentShadow)
		{
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			if (this.area.Area3DStyle.Enable3D && !this.round3DShape)
			{
				this.DrawFunnel3DSquareSegment(point, pointIndex, startWidth, endWidth, location, height, nothingOnTop, nothingOnBottom, drawSegment, drawSegmentShadow);
			}
			else
			{
				if (!nothingOnBottom)
				{
					height = (float)(height + 0.30000001192092896);
				}
				float num = startWidth;
				float num2 = endWidth;
				if (this.funnelStyle == FunnelStyle.YIsHeight && !this.isPyramid)
				{
					if (startWidth < this.funnelNeckSize.Width)
					{
						startWidth = this.funnelNeckSize.Width;
					}
					if (endWidth < this.funnelNeckSize.Width)
					{
						endWidth = this.funnelNeckSize.Width;
					}
				}
				float tension = 0.8f;
				float num3 = (float)(startWidth / 2.0 * Math.Sin((float)this.rotation3D / 180.0 * 3.1415926535897931));
				float num4 = (float)(endWidth / 2.0 * Math.Sin((float)this.rotation3D / 180.0 * 3.1415926535897931));
				RectangleF absoluteRectangle = this.graph.GetAbsoluteRectangle(this.plotAreaPosition);
				float num5 = (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0);
				this.graph.StartHotRegion(point);
				this.graph.StartAnimation();
				GraphicsPath graphicsPath = new GraphicsPath();
				if (startWidth > 0.0)
				{
					if (this.area.Area3DStyle.Enable3D)
					{
						PointF[] points = new PointF[4]
						{
							new PointF((float)(num5 + startWidth / 2.0), location),
							new PointF(num5, location + num3),
							new PointF((float)(num5 - startWidth / 2.0), location),
							new PointF(num5, location - num3)
						};
						GraphicsPath graphicsPath2 = new GraphicsPath();
						graphicsPath2.AddClosedCurve(points, tension);
						graphicsPath2.Flatten();
						graphicsPath2.Reverse();
						((ChartGraphics3D)this.graph).AddEllipseSegment(graphicsPath, graphicsPath2, (GraphicsPath)null, true, 0f, out empty, out empty2);
					}
					else
					{
						graphicsPath.AddLine((float)(num5 - startWidth / 2.0), location, (float)(num5 + startWidth / 2.0), location);
					}
				}
				if (this.funnelStyle == FunnelStyle.YIsHeight && !this.isPyramid && startWidth > this.funnelNeckSize.Width && endWidth <= this.funnelNeckSize.Width)
				{
					PointF linesIntersection = ChartGraphics3D.GetLinesIntersection((float)(num5 + this.funnelNeckSize.Width / 2.0), absoluteRectangle.Top, (float)(num5 + this.funnelNeckSize.Width / 2.0), absoluteRectangle.Bottom, (float)(num5 + num / 2.0), location, (float)(num5 + num2 / 2.0), location + height);
					linesIntersection.Y = absoluteRectangle.Bottom - this.funnelNeckSize.Height;
					graphicsPath.AddLine((float)(num5 + startWidth / 2.0), location, linesIntersection.X, linesIntersection.Y);
					graphicsPath.AddLine(linesIntersection.X, linesIntersection.Y, linesIntersection.X, location + height);
				}
				else
				{
					graphicsPath.AddLine((float)(num5 + startWidth / 2.0), location, (float)(num5 + endWidth / 2.0), location + height);
				}
				if (endWidth > 0.0)
				{
					if (this.area.Area3DStyle.Enable3D)
					{
						PointF[] points2 = new PointF[4]
						{
							new PointF((float)(num5 + endWidth / 2.0), location + height),
							new PointF(num5, location + height + num4),
							new PointF((float)(num5 - endWidth / 2.0), location + height),
							new PointF(num5, location + height - num4)
						};
						GraphicsPath graphicsPath3 = new GraphicsPath();
						graphicsPath3.AddClosedCurve(points2, tension);
						graphicsPath3.Flatten();
						graphicsPath3.Reverse();
						GraphicsPath graphicsPath4 = new GraphicsPath();
						((ChartGraphics3D)this.graph).AddEllipseSegment(graphicsPath4, graphicsPath3, (GraphicsPath)null, true, 0f, out empty, out empty2);
						graphicsPath4.Reverse();
						if (graphicsPath4.PointCount > 0)
						{
							graphicsPath.AddPath(graphicsPath4, false);
						}
					}
					else
					{
						graphicsPath.AddLine((float)(num5 + endWidth / 2.0), location + height, (float)(num5 - endWidth / 2.0), location + height);
					}
				}
				if (this.funnelStyle == FunnelStyle.YIsHeight && !this.isPyramid && startWidth > this.funnelNeckSize.Width && endWidth <= this.funnelNeckSize.Width)
				{
					PointF linesIntersection2 = ChartGraphics3D.GetLinesIntersection((float)(num5 - this.funnelNeckSize.Width / 2.0), absoluteRectangle.Top, (float)(num5 - this.funnelNeckSize.Width / 2.0), absoluteRectangle.Bottom, (float)(num5 - num / 2.0), location, (float)(num5 - num2 / 2.0), location + height);
					linesIntersection2.Y = absoluteRectangle.Bottom - this.funnelNeckSize.Height;
					graphicsPath.AddLine(linesIntersection2.X, location + height, linesIntersection2.X, linesIntersection2.Y);
					graphicsPath.AddLine(linesIntersection2.X, linesIntersection2.Y, (float)(num5 - startWidth / 2.0), location);
				}
				else
				{
					graphicsPath.AddLine((float)(num5 - endWidth / 2.0), location + height, (float)(num5 - startWidth / 2.0), location);
				}
				if (this.common.ProcessModePaint)
				{
					if (this.area.Area3DStyle.Enable3D && this.graph.ActiveRenderingType == RenderingType.Gdi)
					{
						Color gradientColor = ChartGraphics.GetGradientColor(point.Color, Color.White, 0.3);
						Color gradientColor2 = ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.3);
						RectangleF bounds = graphicsPath.GetBounds();
						if (bounds.Width == 0.0)
						{
							bounds.Width = 1f;
						}
						if (bounds.Height == 0.0)
						{
							bounds.Height = 1f;
						}
						using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(bounds, gradientColor, gradientColor2, 0f))
						{
							ColorBlend colorBlend = new ColorBlend(5);
							colorBlend.Colors[0] = gradientColor2;
							colorBlend.Colors[1] = gradientColor2;
							colorBlend.Colors[2] = gradientColor;
							colorBlend.Colors[3] = gradientColor2;
							colorBlend.Colors[4] = gradientColor2;
							colorBlend.Positions[0] = 0f;
							colorBlend.Positions[1] = 0f;
							colorBlend.Positions[2] = 0.5f;
							colorBlend.Positions[3] = 1f;
							colorBlend.Positions[4] = 1f;
							linearGradientBrush.InterpolationColors = colorBlend;
							this.graph.Graphics.FillPath(linearGradientBrush, graphicsPath);
							Pen pen = new Pen(point.BorderColor, (float)point.BorderWidth);
							pen.DashStyle = this.graph.GetPenStyle(point.BorderStyle);
							if (point.BorderWidth == 0 || point.BorderStyle == ChartDashStyle.NotSet || point.BorderColor == Color.Empty)
							{
								pen = new Pen(ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.3), 1f);
								pen.Alignment = PenAlignment.Inset;
							}
							pen.StartCap = LineCap.Round;
							pen.EndCap = LineCap.Round;
							pen.LineJoin = LineJoin.Bevel;
							this.graph.DrawPath(pen, graphicsPath);
							pen.Dispose();
						}
					}
					else
					{
						this.graph.DrawPathAbs(graphicsPath, drawSegment ? point.Color : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
					}
				}
				if (this.common.ProcessModeRegions)
				{
					this.common.HotRegionsList.AddHotRegion(graphicsPath, false, this.graph, point, point.series.Name, pointIndex);
				}
				else
				{
					graphicsPath.Dispose();
				}
				if ((float)this.rotation3D > 0.0 && startWidth > 0.0 && nothingOnTop && this.area.Area3DStyle.Enable3D)
				{
					PointF[] points3 = new PointF[4]
					{
						new PointF((float)(num5 + startWidth / 2.0), location),
						new PointF(num5, location + num3),
						new PointF((float)(num5 - startWidth / 2.0), location),
						new PointF(num5, location - num3)
					};
					GraphicsPath graphicsPath5 = new GraphicsPath();
					graphicsPath5.AddClosedCurve(points3, tension);
					if (this.common.ProcessModePaint)
					{
						this.graph.DrawPathAbs(graphicsPath5, drawSegment ? ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4) : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
					}
					if (this.common.ProcessModeRegions)
					{
						this.common.HotRegionsList.AddHotRegion(graphicsPath5, false, this.graph, point, point.series.Name, pointIndex);
					}
					else
					{
						graphicsPath5.Dispose();
					}
				}
				if ((float)this.rotation3D < 0.0 && startWidth > 0.0 && nothingOnBottom && this.area.Area3DStyle.Enable3D)
				{
					PointF[] points4 = new PointF[4]
					{
						new PointF((float)(num5 + endWidth / 2.0), location + height),
						new PointF(num5, location + height + num4),
						new PointF((float)(num5 - endWidth / 2.0), location + height),
						new PointF(num5, location + height - num4)
					};
					GraphicsPath graphicsPath6 = new GraphicsPath();
					graphicsPath6.AddClosedCurve(points4, tension);
					if (this.common.ProcessModePaint)
					{
						this.graph.DrawPathAbs(graphicsPath6, drawSegment ? ChartGraphics.GetGradientColor(point.Color, Color.Black, 0.4) : Color.Transparent, point.BackHatchStyle, point.BackImage, point.BackImageMode, point.BackImageTransparentColor, point.BackImageAlign, point.BackGradientType, drawSegment ? point.BackGradientEndColor : Color.Transparent, drawSegment ? point.BorderColor : Color.Transparent, point.BorderWidth, point.BorderStyle, PenAlignment.Center, drawSegmentShadow ? point.series.ShadowOffset : 0, point.series.ShadowColor);
					}
					if (this.common.ProcessModeRegions)
					{
						this.common.HotRegionsList.AddHotRegion(graphicsPath6, false, this.graph, point, point.series.Name, pointIndex);
					}
					else
					{
						graphicsPath6.Dispose();
					}
				}
				this.graph.StopAnimation();
				this.graph.EndHotRegion();
			}
		}

		private ArrayList GetFunnelSegmentPositions()
		{
			ArrayList arrayList = new ArrayList();
			Series dataSeries = this.GetDataSeries();
			if (dataSeries != null)
			{
				this.funnelStyle = this.GetFunnelStyle(dataSeries);
				this.round3DShape = (this.GetFunnel3DDrawingStyle(dataSeries) == Funnel3DDrawingStyle.CircularBase);
				this.funnelSegmentGap = this.GetFunnelPointGap(dataSeries);
				this.funnelNeckSize = this.GetFunnelNeckSize(dataSeries);
				float num = this.graph.GetAbsolutePoint(this.plotAreaPosition.Location).Y;
				if (this.isPyramid)
				{
					num = this.graph.GetAbsoluteRectangle(this.plotAreaPosition).Bottom;
				}
				for (int i = 0; i >= 0 && i < dataSeries.Points.Count; i++)
				{
					DataPoint point = dataSeries.Points[i];
					if (i > 0 || this.ShouldDrawFirstPoint())
					{
						float startWidth = 0f;
						float endWidth = 0f;
						float num2 = 0f;
						this.GetPointWidthAndHeight(dataSeries, i, num, out num2, out startWidth, out endWidth);
						bool nothingOnTop = false;
						bool nothingOnBottom = false;
						if (this.funnelSegmentGap > 0.0)
						{
							nothingOnTop = true;
							nothingOnBottom = true;
						}
						else
						{
							if (this.ShouldDrawFirstPoint())
							{
								if (i == 0 || dataSeries.Points[i - 1].Color.A != 255)
								{
									if (this.isPyramid)
									{
										nothingOnBottom = true;
									}
									else
									{
										nothingOnTop = true;
									}
								}
							}
							else if (i == 1 || dataSeries.Points[i - 1].Color.A != 255)
							{
								if (this.isPyramid)
								{
									nothingOnBottom = true;
								}
								else
								{
									nothingOnTop = true;
								}
							}
							if (i == dataSeries.Points.Count - 1)
							{
								if (this.isPyramid)
								{
									nothingOnTop = true;
								}
								else
								{
									nothingOnBottom = true;
								}
							}
							else if (dataSeries.Points[i + 1].Color.A != 255)
							{
								if (this.isPyramid)
								{
									nothingOnTop = true;
								}
								else
								{
									nothingOnBottom = true;
								}
							}
						}
						FunnelSegmentInfo funnelSegmentInfo = new FunnelSegmentInfo();
						funnelSegmentInfo.Point = point;
						funnelSegmentInfo.PointIndex = i;
						funnelSegmentInfo.StartWidth = startWidth;
						funnelSegmentInfo.EndWidth = endWidth;
						funnelSegmentInfo.Location = (this.isPyramid ? (num - num2) : num);
						funnelSegmentInfo.Height = num2;
						funnelSegmentInfo.NothingOnTop = nothingOnTop;
						funnelSegmentInfo.NothingOnBottom = nothingOnBottom;
						arrayList.Add(funnelSegmentInfo);
						num = ((!this.isPyramid) ? (num + (num2 + this.funnelSegmentGap)) : (num - (num2 + this.funnelSegmentGap)));
					}
				}
			}
			return arrayList;
		}

		private void DrawLabels()
		{
			foreach (FunnelPointLabelInfo labelInfo in this.labelInfoList)
			{
				if (!labelInfo.Position.IsEmpty && !float.IsNaN(labelInfo.Position.X) && !float.IsNaN(labelInfo.Position.Y) && !float.IsNaN(labelInfo.Position.Width) && !float.IsNaN(labelInfo.Position.Height))
				{
					this.graph.StartHotRegion(labelInfo.Point);
					this.graph.StartAnimation();
					SizeF sizeF = this.graph.MeasureString("W", labelInfo.Point.Font, new SizeF(1000f, 1000f), new StringFormat(StringFormat.GenericTypographic));
					if (!labelInfo.CalloutPoint1.IsEmpty && !labelInfo.CalloutPoint2.IsEmpty && !float.IsNaN(labelInfo.CalloutPoint1.X) && !float.IsNaN(labelInfo.CalloutPoint1.Y) && !float.IsNaN(labelInfo.CalloutPoint2.X) && !float.IsNaN(labelInfo.CalloutPoint2.Y))
					{
						if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
						{
							labelInfo.CalloutPoint2.X -= (float)(sizeF.Width / 2.0);
							labelInfo.CalloutPoint1.X += 2f;
						}
						else
						{
							labelInfo.CalloutPoint2.X += (float)(sizeF.Width / 2.0);
							labelInfo.CalloutPoint1.X += 2f;
						}
						Color calloutLineColor = this.GetCalloutLineColor(labelInfo.Point);
						this.graph.DrawLineAbs(calloutLineColor, 1, ChartDashStyle.Solid, labelInfo.CalloutPoint1, labelInfo.CalloutPoint2);
					}
					RectangleF rectangleF = labelInfo.Position;
					rectangleF.Inflate((float)(sizeF.Width / 2.0), (float)(sizeF.Height / 8.0));
					rectangleF = this.graph.GetRelativeRectangle(rectangleF);
					StringFormat stringFormat = (StringFormat)labelInfo.Format.Clone();
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Center;
					this.graph.DrawPointLabelStringRel(this.common, labelInfo.Text, labelInfo.Point.Font, new SolidBrush(labelInfo.Point.FontColor), rectangleF, stringFormat, labelInfo.Point.FontAngle, rectangleF, labelInfo.Point.LabelBackColor, labelInfo.Point.LabelBorderColor, labelInfo.Point.LabelBorderWidth, labelInfo.Point.LabelBorderStyle, labelInfo.Point.series, labelInfo.Point, labelInfo.PointIndex);
					this.graph.StopAnimation();
					this.graph.EndHotRegion();
				}
			}
		}

		private ArrayList CreateLabelsInfoList()
		{
			ArrayList arrayList = new ArrayList();
			RectangleF absoluteRectangle = this.graph.GetAbsoluteRectangle(this.area.Position.ToRectangleF());
			Series dataSeries = this.GetDataSeries();
			if (dataSeries != null)
			{
				int num = 0;
				{
					foreach (DataPoint point in dataSeries.Points)
					{
						if (!point.Empty)
						{
							string label = point.Label;
							if (point.ShowLabelAsValue || label.Length > 0)
							{
								FunnelPointLabelInfo funnelPointLabelInfo = new FunnelPointLabelInfo();
								funnelPointLabelInfo.Point = point;
								funnelPointLabelInfo.PointIndex = num;
								if (label.Length == 0)
								{
									funnelPointLabelInfo.Text = ValueConverter.FormatValue(point.series.chart, point, point.YValues[0], point.LabelFormat, point.series.YValueType, ChartElementType.DataPoint);
								}
								else
								{
									funnelPointLabelInfo.Text = point.ReplaceKeywords(label);
								}
								funnelPointLabelInfo.Style = this.GetLabelStyle(point);
								if (funnelPointLabelInfo.Style == FunnelLabelStyle.Inside)
								{
									funnelPointLabelInfo.VerticalAlignment = this.GetInsideLabelAlignment(point);
								}
								if (funnelPointLabelInfo.Style != 0)
								{
									funnelPointLabelInfo.OutsidePlacement = this.GetOutsideLabelPlacement(point);
								}
								funnelPointLabelInfo.Size = this.graph.MeasureString(funnelPointLabelInfo.Text, point.Font, absoluteRectangle.Size, new StringFormat(StringFormat.GenericTypographic));
								if (funnelPointLabelInfo.Text.Length > 0 && funnelPointLabelInfo.Style != FunnelLabelStyle.Disabled)
								{
									arrayList.Add(funnelPointLabelInfo);
								}
							}
						}
						num++;
					}
					return arrayList;
				}
			}
			return arrayList;
		}

		private bool FitPointLabels()
		{
			RectangleF absoluteRectangle = this.graph.GetAbsoluteRectangle(this.plotAreaPosition);
			absoluteRectangle.Inflate(-4f, -4f);
			this.GetLabelsPosition();
			RectangleF absolute = this.graph.GetAbsoluteRectangle(new RectangleF(1f, 1f, 1f, 1f));
			foreach (FunnelPointLabelInfo labelInfo in this.labelInfoList)
			{
				RectangleF position = labelInfo.Position;
				if (labelInfo.Style == FunnelLabelStyle.Outside || labelInfo.Style == FunnelLabelStyle.OutsideInColumn)
				{
					float num = 10f;
					if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
					{
						position.Width += num;
					}
					else if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Left)
					{
						position.X -= num;
						position.Width += num;
					}
				}
				if (labelInfo.Style != 0)
				{
					if (absoluteRectangle.X - position.X > absolute.X)
					{
						absolute.X = absoluteRectangle.X - position.X;
					}
					if (position.Right - absoluteRectangle.Right > absolute.Width)
					{
						absolute.Width = position.Right - absoluteRectangle.Right;
					}
				}
				if (absoluteRectangle.Y - position.Y > absolute.Y)
				{
					absolute.Y = absoluteRectangle.Y - position.Y;
				}
				if (position.Bottom - absoluteRectangle.Bottom > absolute.Height)
				{
					absolute.Height = position.Bottom - absoluteRectangle.Bottom;
				}
			}
			absolute = this.graph.GetRelativeRectangle(absolute);
			if (!(absolute.X > 1.0) && !(absolute.Y > 1.0) && !(absolute.Width > 1.0) && !(absolute.Height > 1.0))
			{
				return true;
			}
			this.plotAreaSpacing = absolute;
			this.plotAreaPosition = this.GetPlotAreaPosition();
			this.segmentList = this.GetFunnelSegmentPositions();
			this.GetLabelsPosition();
			return false;
		}

		private void GetLabelsPosition()
		{
			RectangleF absoluteRectangle = this.graph.GetAbsoluteRectangle(this.plotAreaPosition);
			float num = (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0);
			SizeF sizeF = new SizeF(3f, 3f);
			foreach (FunnelPointLabelInfo labelInfo in this.labelInfoList)
			{
				bool flag = false;
				int num2 = labelInfo.PointIndex + ((!this.ShouldDrawFirstPoint()) ? 1 : 0);
				if (num2 > this.segmentList.Count && !this.ShouldDrawFirstPoint())
				{
					num2 = this.segmentList.Count;
					flag = true;
				}
				FunnelSegmentInfo funnelSegmentInfo = null;
				foreach (FunnelSegmentInfo segment in this.segmentList)
				{
					if (segment.PointIndex == num2)
					{
						funnelSegmentInfo = segment;
						break;
					}
				}
				if (funnelSegmentInfo != null)
				{
					labelInfo.Position.Width = labelInfo.Size.Width;
					labelInfo.Position.Height = labelInfo.Size.Height;
					if (labelInfo.Style == FunnelLabelStyle.Outside || labelInfo.Style == FunnelLabelStyle.OutsideInColumn)
					{
						if (this.funnelStyle == FunnelStyle.YIsHeight)
						{
							float num3 = funnelSegmentInfo.StartWidth;
							float num4 = funnelSegmentInfo.EndWidth;
							if (!this.isPyramid)
							{
								if (num3 < this.funnelNeckSize.Width)
								{
									num3 = this.funnelNeckSize.Width;
								}
								if (num4 < this.funnelNeckSize.Width)
								{
									num4 = this.funnelNeckSize.Width;
								}
								if (funnelSegmentInfo.StartWidth >= this.funnelNeckSize.Width && funnelSegmentInfo.EndWidth < this.funnelNeckSize.Width)
								{
									num4 = funnelSegmentInfo.EndWidth;
								}
							}
							labelInfo.Position.Y = (float)(funnelSegmentInfo.Location + funnelSegmentInfo.Height / 2.0 - labelInfo.Size.Height / 2.0);
							if (labelInfo.Style == FunnelLabelStyle.OutsideInColumn)
							{
								if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
								{
									labelInfo.Position.X = (float)(absoluteRectangle.Right + 4.0 * sizeF.Width);
									if (!this.isPyramid)
									{
										labelInfo.CalloutPoint1.X = num + Math.Max((float)(this.funnelNeckSize.Width / 2.0), (float)((num3 + num4) / 4.0));
									}
									else
									{
										labelInfo.CalloutPoint1.X = (float)(num + (num3 + num4) / 4.0);
									}
									labelInfo.CalloutPoint2.X = labelInfo.Position.X;
								}
								else
								{
									labelInfo.Position.X = (float)(absoluteRectangle.X - labelInfo.Size.Width - 4.0 * sizeF.Width);
									if (!this.isPyramid)
									{
										labelInfo.CalloutPoint1.X = num - Math.Max((float)(this.funnelNeckSize.Width / 2.0), (float)((num3 + num4) / 4.0));
									}
									else
									{
										labelInfo.CalloutPoint1.X = (float)(num - (num3 + num4) / 4.0);
									}
									labelInfo.CalloutPoint2.X = labelInfo.Position.Right;
								}
								labelInfo.CalloutPoint1.Y = (float)(funnelSegmentInfo.Location + funnelSegmentInfo.Height / 2.0);
								labelInfo.CalloutPoint2.Y = labelInfo.CalloutPoint1.Y;
							}
							else if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
							{
								labelInfo.Position.X = (float)(num + (num3 + num4) / 4.0 + 4.0 * sizeF.Width);
							}
							else
							{
								labelInfo.Position.X = (float)(num - labelInfo.Size.Width - (num3 + num4) / 4.0 - 4.0 * sizeF.Width);
							}
						}
						else
						{
							if (flag)
							{
								if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
								{
									labelInfo.Position.X = (float)(num + funnelSegmentInfo.EndWidth / 2.0 + 4.0 * sizeF.Width);
								}
								else
								{
									labelInfo.Position.X = (float)(num - labelInfo.Size.Width - funnelSegmentInfo.EndWidth / 2.0 - 4.0 * sizeF.Width);
								}
								labelInfo.Position.Y = (float)(funnelSegmentInfo.Location + funnelSegmentInfo.Height - labelInfo.Size.Height / 2.0);
							}
							else
							{
								if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
								{
									labelInfo.Position.X = (float)(num + funnelSegmentInfo.StartWidth / 2.0 + 4.0 * sizeF.Width);
								}
								else
								{
									labelInfo.Position.X = (float)(num - labelInfo.Size.Width - funnelSegmentInfo.StartWidth / 2.0 - 4.0 * sizeF.Width);
								}
								labelInfo.Position.Y = (float)(funnelSegmentInfo.Location - labelInfo.Size.Height / 2.0);
							}
							if (labelInfo.Style == FunnelLabelStyle.OutsideInColumn)
							{
								if (labelInfo.OutsidePlacement == FunnelLabelPlacement.Right)
								{
									labelInfo.Position.X = (float)(absoluteRectangle.Right + 4.0 * sizeF.Width);
									labelInfo.CalloutPoint1.X = (float)(num + (flag ? funnelSegmentInfo.EndWidth : funnelSegmentInfo.StartWidth) / 2.0);
									labelInfo.CalloutPoint2.X = labelInfo.Position.X;
								}
								else
								{
									labelInfo.Position.X = (float)(absoluteRectangle.X - labelInfo.Size.Width - 4.0 * sizeF.Width);
									labelInfo.CalloutPoint1.X = (float)(num - (flag ? funnelSegmentInfo.EndWidth : funnelSegmentInfo.StartWidth) / 2.0);
									labelInfo.CalloutPoint2.X = labelInfo.Position.Right;
								}
								labelInfo.CalloutPoint1.Y = funnelSegmentInfo.Location;
								if (flag)
								{
									labelInfo.CalloutPoint1.Y += funnelSegmentInfo.Height;
								}
								labelInfo.CalloutPoint2.Y = labelInfo.CalloutPoint1.Y;
							}
						}
					}
					else if (labelInfo.Style == FunnelLabelStyle.Inside)
					{
						labelInfo.Position.X = (float)(num - labelInfo.Size.Width / 2.0);
						if (this.funnelStyle == FunnelStyle.YIsHeight)
						{
							labelInfo.Position.Y = (float)(funnelSegmentInfo.Location + funnelSegmentInfo.Height / 2.0 - labelInfo.Size.Height / 2.0);
							if (labelInfo.VerticalAlignment == FunnelLabelVerticalAlignment.Top)
							{
								labelInfo.Position.Y -= (float)(funnelSegmentInfo.Height / 2.0 - labelInfo.Size.Height / 2.0 - sizeF.Height);
							}
							else if (labelInfo.VerticalAlignment == FunnelLabelVerticalAlignment.Bottom)
							{
								labelInfo.Position.Y += (float)(funnelSegmentInfo.Height / 2.0 - labelInfo.Size.Height / 2.0 - sizeF.Height);
							}
						}
						else
						{
							labelInfo.Position.Y = (float)(funnelSegmentInfo.Location - labelInfo.Size.Height / 2.0);
							if (labelInfo.VerticalAlignment == FunnelLabelVerticalAlignment.Top)
							{
								labelInfo.Position.Y -= (float)(labelInfo.Size.Height / 2.0 + sizeF.Height);
							}
							else if (labelInfo.VerticalAlignment == FunnelLabelVerticalAlignment.Bottom)
							{
								labelInfo.Position.Y += (float)(labelInfo.Size.Height / 2.0 + sizeF.Height);
							}
							if (flag)
							{
								labelInfo.Position.Y += funnelSegmentInfo.Height;
							}
						}
						if (this.area.Area3DStyle.Enable3D)
						{
							labelInfo.Position.Y += (float)((funnelSegmentInfo.EndWidth + funnelSegmentInfo.StartWidth) / 4.0 * Math.Sin((float)this.rotation3D / 180.0 * 3.1415926535897931));
						}
					}
					int num5 = 0;
					while (this.IsLabelsOverlap(labelInfo) && num5 < 1000)
					{
						float num6 = (float)(this.isPyramid ? -3.0 : 3.0);
						labelInfo.Position.Y += num6;
						if (!labelInfo.CalloutPoint2.IsEmpty)
						{
							labelInfo.CalloutPoint2.Y += num6;
						}
						num5++;
					}
				}
			}
		}

		private bool IsLabelsOverlap(FunnelPointLabelInfo testLabelInfo)
		{
			RectangleF position = testLabelInfo.Position;
			position.Inflate(1f, 1f);
			if (!testLabelInfo.Point.LabelBackColor.IsEmpty || (testLabelInfo.Point.LabelBorderWidth > 0 && !testLabelInfo.Point.LabelBorderColor.IsEmpty && testLabelInfo.Point.LabelBorderStyle != 0))
			{
				position.Inflate(4f, 4f);
			}
			foreach (FunnelPointLabelInfo labelInfo in this.labelInfoList)
			{
				if (labelInfo.PointIndex == testLabelInfo.PointIndex)
				{
					break;
				}
				if (!labelInfo.Position.IsEmpty && labelInfo.Position.IntersectsWith(position))
				{
					return true;
				}
			}
			return false;
		}

		private FunnelLabelStyle GetLabelStyle(DataPointAttributes attributes)
		{
			FunnelLabelStyle funnelLabelStyle = FunnelLabelStyle.OutsideInColumn;
			string text = attributes[this.funnelLabelStyleAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					funnelLabelStyle = (FunnelLabelStyle)Enum.Parse(typeof(FunnelLabelStyle), text, true);
					return funnelLabelStyle;
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(funnelLabelStyle.ToString(), this.funnelLabelStyleAttributeName));
				}
			}
			return funnelLabelStyle;
		}

		private void GetPlotAreaSpacing()
		{
			this.plotAreaSpacing = new RectangleF(1f, 1f, 1f, 1f);
			this.plotAreaPosition = this.GetPlotAreaPosition();
			this.segmentList = this.GetFunnelSegmentPositions();
			if (this.area.InnerPlotPosition.Auto)
			{
				int num = 0;
				while (!this.FitPointLabels() && num < 5)
				{
					num++;
				}
			}
			else
			{
				this.GetLabelsPosition();
			}
		}

		private RectangleF GetPlotAreaPosition()
		{
			RectangleF rectangleF = this.area.InnerPlotPosition.Auto ? this.area.Position.ToRectangleF() : this.area.PlotAreaPosition.ToRectangleF();
			if (this.plotAreaSpacing.Y > rectangleF.Height / 2.0)
			{
				this.plotAreaSpacing.Y = (float)(rectangleF.Height / 2.0);
			}
			if (this.plotAreaSpacing.Height > rectangleF.Height / 2.0)
			{
				this.plotAreaSpacing.Height = (float)(rectangleF.Height / 2.0);
			}
			rectangleF.X += this.plotAreaSpacing.X;
			rectangleF.Y += this.plotAreaSpacing.Y;
			rectangleF.Width -= this.plotAreaSpacing.X + this.plotAreaSpacing.Width;
			rectangleF.Height -= this.plotAreaSpacing.Y + this.plotAreaSpacing.Height;
			if (this.area.Area3DStyle.Enable3D)
			{
				RectangleF absoluteRectangle = this.graph.GetAbsoluteRectangle(rectangleF);
				Series dataSeries = this.GetDataSeries();
				if (dataSeries != null)
				{
					this.rotation3D = this.GetFunnelRotation(dataSeries);
				}
				float num = (float)Math.Abs(absoluteRectangle.Width / 2.0 * Math.Sin((float)this.rotation3D / 180.0 * 3.1415926535897931));
				float num2 = (float)Math.Abs(absoluteRectangle.Width / 2.0 * Math.Sin((float)this.rotation3D / 180.0 * 3.1415926535897931));
				if (this.isPyramid)
				{
					absoluteRectangle.Height -= num2;
				}
				else
				{
					absoluteRectangle.Y += num;
					absoluteRectangle.Height -= num + num2;
				}
				rectangleF = this.graph.GetRelativeRectangle(absoluteRectangle);
			}
			return rectangleF;
		}

		protected float CheckMinHeight(float height)
		{
			float num = Math.Min(2f, (float)(this.funnelSegmentGap / 2.0));
			if (this.funnelSegmentGap > 0.0 && height < num)
			{
				return num;
			}
			return height;
		}

		private void GetFunnelMinPointHeight(DataPointAttributes attributes)
		{
			this.funnelMinPointHeight = 0f;
			string text = attributes[this.funnelPointMinHeight];
			if (text == null)
			{
				return;
			}
			if (text.Length <= 0)
			{
				return;
			}
			try
			{
				this.funnelMinPointHeight = float.Parse(text, CultureInfo.InvariantCulture);
			}
			catch
			{
				throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, this.funnelPointMinHeight));
			}
			if (!(this.funnelMinPointHeight < 0.0) && !(this.funnelMinPointHeight > 100.0))
			{
				this.funnelMinPointHeight = (float)(this.yValueTotal * (double)this.funnelMinPointHeight / 100.0);
				this.GetDataPointValuesStatistic();
				return;
			}
			throw new InvalidOperationException(SR.ExceptionFunnelMinimumPointHeightAttributeInvalid);
		}

		private int GetFunnelRotation(DataPointAttributes attributes)
		{
			int num = 5;
			string text = attributes[this.funnelRotationAngleAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					num = int.Parse(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, this.funnelRotationAngleAttributeName));
				}
				if (num >= -10 && num <= 10)
				{
					goto IL_0051;
				}
				throw new InvalidOperationException(SR.ExceptionFunnelAngleRangeInvalid);
			}
			goto IL_0051;
			IL_0051:
			return num;
		}

		private Color GetCalloutLineColor(DataPointAttributes attributes)
		{
			Color result = Color.Black;
			string text = attributes["CalloutLineColor"];
			if (text != null && text.Length > 0)
			{
				bool flag = false;
				ColorConverter colorConverter = new ColorConverter();
				try
				{
					result = (Color)colorConverter.ConvertFromInvariantString(text);
				}
				catch
				{
					flag = true;
				}
				if (flag)
				{
					try
					{
						return (Color)colorConverter.ConvertFromString(text);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "CalloutLineColor"));
					}
				}
			}
			return result;
		}

		private SizeF GetFunnelNeckSize(DataPointAttributes attributes)
		{
			SizeF relative = new SizeF(5f, 5f);
			string text = attributes["FunnelNeckWidth"];
			if (text != null && text.Length > 0)
			{
				try
				{
					relative.Width = float.Parse(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "FunnelNeckWidth"));
				}
				if (!(relative.Width < 0.0) && !(relative.Width > 100.0))
				{
					goto IL_0076;
				}
				throw new InvalidOperationException(SR.ExceptionFunnelNeckWidthInvalid);
			}
			goto IL_0076;
			IL_0076:
			text = attributes["FunnelNeckHeight"];
			if (text != null && text.Length > 0)
			{
				try
				{
					relative.Height = float.Parse(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "FunnelNeckHeight"));
				}
				if (!(relative.Height < 0.0) && !(relative.Height > 100.0))
				{
					goto IL_00db;
				}
				throw new InvalidOperationException(SR.ExceptionFunnelNeckHeightInvalid);
			}
			goto IL_00db;
			IL_00db:
			if (relative.Height > this.plotAreaPosition.Height / 2.0)
			{
				relative.Height = (float)(this.plotAreaPosition.Height / 2.0);
			}
			if (relative.Width > this.plotAreaPosition.Width / 2.0)
			{
				relative.Width = (float)(this.plotAreaPosition.Width / 2.0);
			}
			return this.graph.GetAbsoluteSize(relative);
		}

		private float GetFunnelPointGap(DataPointAttributes attributes)
		{
			float result = 0f;
			string text = attributes[this.funnelPointGapAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					result = float.Parse(text, CultureInfo.InvariantCulture);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, this.funnelPointGapAttributeName));
				}
				float num = this.plotAreaPosition.Height / (float)(this.pointNumber - (this.ShouldDrawFirstPoint() ? 1 : 2));
				if (result > num)
				{
					result = num;
				}
				if (result < 0.0)
				{
					result = 0f;
				}
				result = this.graph.GetAbsoluteSize(new SizeF(result, result)).Height;
			}
			return result;
		}

		private FunnelStyle GetFunnelStyle(DataPointAttributes attributes)
		{
			FunnelStyle result = FunnelStyle.YIsHeight;
			if (!this.isPyramid)
			{
				string text = attributes["FunnelStyle"];
				if (text != null && text.Length > 0)
				{
					try
					{
						return (FunnelStyle)Enum.Parse(typeof(FunnelStyle), text, true);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "FunnelStyle"));
					}
				}
			}
			return result;
		}

		private FunnelLabelPlacement GetOutsideLabelPlacement(DataPointAttributes attributes)
		{
			FunnelLabelPlacement result = FunnelLabelPlacement.Right;
			string text = attributes[this.funnelOutsideLabelPlacementAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					return (FunnelLabelPlacement)Enum.Parse(typeof(FunnelLabelPlacement), text, true);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, this.funnelOutsideLabelPlacementAttributeName));
				}
			}
			return result;
		}

		private FunnelLabelVerticalAlignment GetInsideLabelAlignment(DataPointAttributes attributes)
		{
			FunnelLabelVerticalAlignment result = FunnelLabelVerticalAlignment.Center;
			string text = attributes[this.funnelInsideLabelAlignmentAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					return (FunnelLabelVerticalAlignment)Enum.Parse(typeof(FunnelLabelVerticalAlignment), text, true);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, this.funnelInsideLabelAlignmentAttributeName));
				}
			}
			return result;
		}

		private Funnel3DDrawingStyle GetFunnel3DDrawingStyle(DataPointAttributes attributes)
		{
			Funnel3DDrawingStyle result = (Funnel3DDrawingStyle)(this.isPyramid ? 1 : 0);
			string text = attributes[this.funnel3DDrawingStyleAttributeName];
			if (text != null && text.Length > 0)
			{
				try
				{
					return (Funnel3DDrawingStyle)Enum.Parse(typeof(Funnel3DDrawingStyle), text, true);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, this.funnel3DDrawingStyleAttributeName));
				}
			}
			return result;
		}

		private void GetDataPointValuesStatistic()
		{
			Series dataSeries = this.GetDataSeries();
			if (dataSeries != null)
			{
				this.yValueTotal = 0.0;
				this.xValueTotal = 0.0;
				this.yValueMax = 0.0;
				this.pointNumber = 0;
				this.valuePercentages = null;
				PyramidValueType pyramidValueType = this.GetPyramidValueType(dataSeries);
				if (pyramidValueType == PyramidValueType.Surface)
				{
					double num = 0.0;
					int num2 = 0;
					foreach (DataPoint point in dataSeries.Points)
					{
						if (!point.Empty)
						{
							num += this.GetYValue(point, num2);
						}
						num2++;
					}
					double num3 = 100.0;
					double num4 = 2.0 * num / num3;
					double num5 = num4 / num3;
					double[] array = new double[dataSeries.Points.Count];
					double num6 = 0.0;
					for (int i = 0; i < array.Length; i++)
					{
						double yValue = this.GetYValue(dataSeries.Points[i], i);
						num6 += yValue;
						array[i] = Math.Sqrt(2.0 * num6 / num5);
					}
					this.valuePercentages = array;
				}
				foreach (DataPoint point2 in dataSeries.Points)
				{
					if (!point2.Empty)
					{
						double yValue2 = this.GetYValue(point2, this.pointNumber);
						this.yValueTotal += yValue2;
						this.yValueMax = Math.Max(this.yValueMax, yValue2);
						this.xValueTotal += this.GetXValue(point2);
					}
					this.pointNumber++;
				}
			}
		}

		private Series GetDataSeries()
		{
			if (this.chartTypeSeries == null)
			{
				Series series = null;
				foreach (Series item in this.common.DataManager.Series)
				{
					if (item.IsVisible() && item.ChartArea == this.area.Name)
					{
						if (string.Compare(item.ChartTypeName, this.Name, true, CultureInfo.CurrentCulture) == 0)
						{
							if (series == null)
							{
								series = item;
							}
						}
						else if (!this.common.ChartPicture.SuppressExceptions)
						{
							throw new InvalidOperationException(SR.ExceptionFunnelCanNotCombine);
						}
					}
				}
				this.chartTypeSeries = series;
			}
			return this.chartTypeSeries;
		}

		private PyramidValueType GetPyramidValueType(DataPointAttributes attributes)
		{
			PyramidValueType result = PyramidValueType.Linear;
			if (this.isPyramid)
			{
				string text = attributes["PyramidValueType"];
				if (text != null && text.Length > 0)
				{
					try
					{
						return (PyramidValueType)Enum.Parse(typeof(PyramidValueType), text, true);
					}
					catch
					{
						throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "PyramidValueType"));
					}
				}
			}
			return result;
		}

		public virtual double GetYValue(DataPoint point, int pointIndex)
		{
			double num = 0.0;
			if (!point.Empty)
			{
				num = point.YValues[0];
				if (this.valuePercentages != null && this.valuePercentages.Length > pointIndex)
				{
					num = num / 100.0 * this.valuePercentages[pointIndex];
				}
				if (this.area.AxisY.Logarithmic)
				{
					num = Math.Abs(Math.Log(num, this.area.AxisY.LogarithmBase));
				}
				else
				{
					num = Math.Abs(num);
					if (num < (double)this.funnelMinPointHeight)
					{
						num = (double)this.funnelMinPointHeight;
					}
				}
			}
			return num;
		}

		public virtual double GetXValue(DataPoint point)
		{
			if (this.area.AxisX.Logarithmic)
			{
				return Math.Abs(Math.Log(point.XValue, this.area.AxisX.LogarithmBase));
			}
			return Math.Abs(point.XValue);
		}

		public virtual double GetYValue(CommonElements common, ChartArea area, Series series, DataPoint point, int pointIndex, int yValueIndex)
		{
			return point.YValues[yValueIndex];
		}

		public void AddSmartLabelMarkerPositions(CommonElements common, ChartArea area, Series series, ArrayList list)
		{
		}
	}
}
