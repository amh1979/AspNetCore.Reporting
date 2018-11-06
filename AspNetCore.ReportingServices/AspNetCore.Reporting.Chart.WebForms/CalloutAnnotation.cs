using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeCalloutAnnotation_CalloutAnnotation")]
	internal class CalloutAnnotation : TextAnnotation
	{
		private LineAnchorCap calloutAnchorCap = LineAnchorCap.Arrow;

		private CalloutStyle calloutStyle = CalloutStyle.Rectangle;

		private static GraphicsPath cloudPath = null;

		private static GraphicsPath cloudOutlinePath = null;

		private static RectangleF cloudBounds = RectangleF.Empty;

		[ParenthesizePropertyName(true)]
		[Bindable(true)]
		[DefaultValue(CalloutStyle.Rectangle)]
		[SRDescription("DescriptionAttributeCalloutAnnotation_CalloutStyle")]
		[SRCategory("CategoryAttributeAppearance")]
		public virtual CalloutStyle CalloutStyle
		{
			get
			{
				return this.calloutStyle;
			}
			set
			{
				this.calloutStyle = value;
				base.ResetCurrentRelativePosition();
				base.contentSize = SizeF.Empty;
				base.Invalidate();
			}
		}

		[DefaultValue(LineAnchorCap.Arrow)]
		[SRDescription("DescriptionAttributeCalloutAnnotation_CalloutAnchorCap")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		public virtual LineAnchorCap CalloutAnchorCap
		{
			get
			{
				return this.calloutAnchorCap;
			}
			set
			{
				this.calloutAnchorCap = value;
				base.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[Browsable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLineColor3")]
		public override Color LineColor
		{
			get
			{
				return base.LineColor;
			}
			set
			{
				base.LineColor = value;
			}
		}

		[SRDescription("DescriptionAttributeCalloutAnnotation_LineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(1)]
		public override int LineWidth
		{
			get
			{
				return base.LineWidth;
			}
			set
			{
				base.LineWidth = value;
			}
		}

		[SRDescription("DescriptionAttributeLineStyle6")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		public override ChartDashStyle LineStyle
		{
			get
			{
				return base.LineStyle;
			}
			set
			{
				base.LineStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[Browsable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBackColor9")]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[Browsable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeCalloutAnnotation_BackHatchStyle")]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		public override ChartHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeCalloutAnnotation_BackGradientType")]
		[Browsable(true)]
		[DefaultValue(GradientType.None)]
		[SRCategory("CategoryAttributeAppearance")]
		public override GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
			}
		}

		[SRDescription("DescriptionAttributeCalloutAnnotation_BackGradientEndColor")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		public override Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(3.0)]
		[SRCategory("CategoryAttributeAnchor")]
		[SRDescription("DescriptionAttributeCalloutAnnotation_AnchorOffsetX")]
		public override double AnchorOffsetX
		{
			get
			{
				return base.AnchorOffsetX;
			}
			set
			{
				base.AnchorOffsetX = value;
			}
		}

		[SRDescription("DescriptionAttributeCalloutAnnotation_AnchorOffsetY")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(3.0)]
		[SRCategory("CategoryAttributeAnchor")]
		public override double AnchorOffsetY
		{
			get
			{
				return base.AnchorOffsetY;
			}
			set
			{
				base.AnchorOffsetY = value;
			}
		}

		[SRCategory("CategoryAttributeAnchor")]
		[SRDescription("DescriptionAttributeAnchorAlignment")]
		[DefaultValue(typeof(ContentAlignment), "BottomLeft")]
		public override ContentAlignment AnchorAlignment
		{
			get
			{
				return base.AnchorAlignment;
			}
			set
			{
				base.AnchorAlignment = value;
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType
		{
			get
			{
				return "Callout";
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal override SelectionPointsStyle SelectionPointsStyle
		{
			get
			{
				return SelectionPointsStyle.Rectangle;
			}
		}

		public CalloutAnnotation()
		{
			base.anchorOffsetX = 3.0;
			base.anchorOffsetY = 3.0;
			base.anchorAlignment = ContentAlignment.BottomLeft;
		}

		internal override RectangleF GetTextSpacing(out bool annotationRelative)
		{
			RectangleF result = base.GetTextSpacing(out annotationRelative);
			if (this.calloutStyle == CalloutStyle.Cloud || this.calloutStyle == CalloutStyle.Ellipse)
			{
				result = new RectangleF(4f, 4f, 4f, 4f);
				annotationRelative = true;
			}
			else if (this.calloutStyle == CalloutStyle.RoundedRectangle)
			{
				result = new RectangleF(1f, 1f, 1f, 1f);
				annotationRelative = true;
			}
			return result;
		}

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			this.Chart = chart;
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			SizeF empty3 = SizeF.Empty;
			((Annotation)this).GetRelativePosition(out empty, out empty3, out empty2);
			PointF pointF = new PointF(empty.X + empty3.Width, empty.Y + empty3.Height);
			RectangleF rect = new RectangleF(empty, new SizeF(pointF.X - empty.X, pointF.Y - empty.Y));
			RectangleF rectangleF = new RectangleF(rect.Location, rect.Size);
			if (rectangleF.Width < 0.0)
			{
				rectangleF.X = rectangleF.Right;
				rectangleF.Width = (float)(0.0 - rectangleF.Width);
			}
			if (rectangleF.Height < 0.0)
			{
				rectangleF.Y = rectangleF.Bottom;
				rectangleF.Height = (float)(0.0 - rectangleF.Height);
			}
			if (!float.IsNaN(rectangleF.X) && !float.IsNaN(rectangleF.Y) && !float.IsNaN(rectangleF.Right) && !float.IsNaN(rectangleF.Bottom))
			{
				GraphicsPath graphicsPath = null;
				if (this.Chart.chartPicture.common.ProcessModePaint)
				{
					switch (this.calloutStyle)
					{
					case CalloutStyle.SimpleLine:
						graphicsPath = this.DrawRectangleLineCallout(graphics, rectangleF, empty2, false);
						break;
					case CalloutStyle.Borderline:
						graphicsPath = this.DrawRectangleLineCallout(graphics, rectangleF, empty2, true);
						break;
					case CalloutStyle.Perspective:
						graphicsPath = this.DrawPerspectiveCallout(graphics, rectangleF, empty2);
						break;
					case CalloutStyle.Cloud:
						graphicsPath = this.DrawCloudCallout(graphics, rectangleF, empty2);
						break;
					case CalloutStyle.Rectangle:
						graphicsPath = this.DrawRectangleCallout(graphics, rectangleF, empty2);
						break;
					case CalloutStyle.Ellipse:
						graphicsPath = this.DrawRoundedRectCallout(graphics, rectangleF, empty2, true);
						break;
					case CalloutStyle.RoundedRectangle:
						graphicsPath = this.DrawRoundedRectCallout(graphics, rectangleF, empty2, false);
						break;
					}
				}
				if (this.Chart.chartPicture.common.ProcessModeRegions)
				{
					if (graphicsPath != null)
					{
						GraphicsPathIterator graphicsPathIterator = new GraphicsPathIterator(graphicsPath);
						GraphicsPath graphicsPath2 = new GraphicsPath();
						while (graphicsPathIterator.NextMarker(graphicsPath2) > 0)
						{
							this.Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, (GraphicsPath)graphicsPath2.Clone(), false, base.ReplaceKeywords(this.ToolTip), base.ReplaceKeywords(this.Href), base.ReplaceKeywords(this.MapAreaAttributes), this, ChartElementType.Annotation);
							graphicsPath2.Reset();
						}
					}
					else
					{
						this.Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, rectangleF, base.ReplaceKeywords(this.ToolTip), base.ReplaceKeywords(this.Href), base.ReplaceKeywords(this.MapAreaAttributes), this, ChartElementType.Annotation, string.Empty);
					}
				}
				this.PaintSelectionHandles(graphics, rect, null);
			}
		}

		private GraphicsPath DrawRoundedRectCallout(ChartGraphics graphics, RectangleF rectanglePosition, PointF anchorPoint, bool isEllipse)
		{
			RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(rectanglePosition);
			if (!(absoluteRectangle.Width <= 0.0) && !(absoluteRectangle.Height <= 0.0))
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				if (isEllipse)
				{
					graphicsPath.AddEllipse(absoluteRectangle);
				}
				else
				{
					float num = Math.Min(absoluteRectangle.Width, absoluteRectangle.Height);
					num = (float)(num / 5.0);
					graphicsPath = this.CreateRoundedRectPath(absoluteRectangle, num);
				}
				if (!float.IsNaN(anchorPoint.X) && !float.IsNaN(anchorPoint.Y) && !rectanglePosition.Contains(anchorPoint.X, anchorPoint.Y))
				{
					PointF absolutePoint = graphics.GetAbsolutePoint(new PointF(anchorPoint.X, anchorPoint.Y));
					graphicsPath.Flatten();
					PointF[] pathPoints = graphicsPath.PathPoints;
					int num2 = 0;
					int num3 = 0;
					float num4 = 3.40282347E+38f;
					PointF[] array = pathPoints;
					for (int i = 0; i < array.Length; i++)
					{
						PointF pointF = array[i];
						float num5 = pointF.X - absolutePoint.X;
						float num6 = pointF.Y - absolutePoint.Y;
						float num7 = num5 * num5 + num6 * num6;
						if (num7 < num4)
						{
							num4 = num7;
							num2 = num3;
						}
						num3++;
					}
					pathPoints[num2] = absolutePoint;
					graphicsPath.Reset();
					graphicsPath.AddLines(pathPoints);
					graphicsPath.CloseAllFigures();
				}
				graphics.DrawPathAbs(graphicsPath, this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, this.LineWidth, this.LineStyle, PenAlignment.Center, this.ShadowOffset, this.ShadowColor);
				base.DrawText(graphics, rectanglePosition, true, false);
				return graphicsPath;
			}
			return null;
		}

		private GraphicsPath DrawRectangleCallout(ChartGraphics graphics, RectangleF rectanglePosition, PointF anchorPoint)
		{
			GraphicsPath graphicsPath = null;
			bool flag = false;
			RectangleF empty = RectangleF.Empty;
			if (!float.IsNaN(anchorPoint.X) && !float.IsNaN(anchorPoint.Y))
			{
				SizeF relativeSize = graphics.GetRelativeSize(new SizeF(1f, 1f));
				RectangleF rectangleF = new RectangleF(rectanglePosition.Location, rectanglePosition.Size);
				rectangleF.Inflate(relativeSize);
				if (!rectangleF.Contains(anchorPoint.X, anchorPoint.Y))
				{
					flag = true;
					RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(rectanglePosition);
					PointF absolutePoint = graphics.GetAbsolutePoint(new PointF(anchorPoint.X, anchorPoint.Y));
					float num = Math.Min(absoluteRectangle.Width, absoluteRectangle.Height);
					num = (float)(num / 4.0);
					PointF[] array = new PointF[7];
					if (anchorPoint.X < rectanglePosition.X && anchorPoint.Y > rectanglePosition.Bottom)
					{
						array[0] = absoluteRectangle.Location;
						array[1] = new PointF(absoluteRectangle.Right, absoluteRectangle.Y);
						array[2] = new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom);
						array[3] = new PointF(absoluteRectangle.X + num, absoluteRectangle.Bottom);
						array[4] = absolutePoint;
						array[5] = new PointF(absoluteRectangle.X, absoluteRectangle.Bottom - num);
						array[6] = new PointF(absoluteRectangle.X, absoluteRectangle.Bottom - num);
					}
					else if (anchorPoint.X >= rectanglePosition.X && anchorPoint.X <= rectanglePosition.Right && anchorPoint.Y > rectanglePosition.Bottom)
					{
						array[0] = absoluteRectangle.Location;
						array[1] = new PointF(absoluteRectangle.Right, absoluteRectangle.Y);
						array[2] = new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom);
						array[3] = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0 + num), absoluteRectangle.Bottom);
						array[4] = absolutePoint;
						array[5] = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0 - num), absoluteRectangle.Bottom);
						array[6] = new PointF(absoluteRectangle.X, absoluteRectangle.Bottom);
					}
					else if (anchorPoint.X > rectanglePosition.Right && anchorPoint.Y > rectanglePosition.Bottom)
					{
						array[0] = absoluteRectangle.Location;
						array[1] = new PointF(absoluteRectangle.Right, absoluteRectangle.Y);
						array[2] = new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom - num);
						array[3] = absolutePoint;
						array[4] = new PointF(absoluteRectangle.Right - num, absoluteRectangle.Bottom);
						array[5] = new PointF(absoluteRectangle.X, absoluteRectangle.Bottom);
						array[6] = new PointF(absoluteRectangle.X, absoluteRectangle.Bottom);
					}
					else if (anchorPoint.X > rectanglePosition.Right && anchorPoint.Y <= rectanglePosition.Bottom && anchorPoint.Y >= rectanglePosition.Y)
					{
						array[0] = absoluteRectangle.Location;
						array[1] = new PointF(absoluteRectangle.Right, absoluteRectangle.Y);
						array[2] = new PointF(absoluteRectangle.Right, (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0 - num));
						array[3] = absolutePoint;
						array[4] = new PointF(absoluteRectangle.Right, (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0 + num));
						array[5] = new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom);
						array[6] = new PointF(absoluteRectangle.X, absoluteRectangle.Bottom);
					}
					else if (anchorPoint.X > rectanglePosition.Right && anchorPoint.Y < rectanglePosition.Y)
					{
						array[0] = absoluteRectangle.Location;
						array[1] = new PointF(absoluteRectangle.Right - num, absoluteRectangle.Y);
						array[2] = absolutePoint;
						array[3] = new PointF(absoluteRectangle.Right, absoluteRectangle.Y + num);
						array[4] = new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom);
						array[5] = new PointF(absoluteRectangle.X, absoluteRectangle.Bottom);
						array[6] = new PointF(absoluteRectangle.X, absoluteRectangle.Bottom);
					}
					else if (anchorPoint.X >= rectanglePosition.X && anchorPoint.X <= rectanglePosition.Right && anchorPoint.Y < rectanglePosition.Y)
					{
						array[0] = absoluteRectangle.Location;
						array[1] = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0 - num), absoluteRectangle.Y);
						array[2] = absolutePoint;
						array[3] = new PointF((float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0 + num), absoluteRectangle.Y);
						array[4] = new PointF(absoluteRectangle.Right, absoluteRectangle.Y);
						array[5] = new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom);
						array[6] = new PointF(absoluteRectangle.X, absoluteRectangle.Bottom);
					}
					else if (anchorPoint.X < rectanglePosition.X && anchorPoint.Y < rectanglePosition.Y)
					{
						array[0] = absolutePoint;
						array[1] = new PointF(absoluteRectangle.X + num, absoluteRectangle.Y);
						array[2] = new PointF(absoluteRectangle.Right, absoluteRectangle.Y);
						array[3] = new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom);
						array[4] = new PointF(absoluteRectangle.X, absoluteRectangle.Bottom);
						array[5] = new PointF(absoluteRectangle.X, absoluteRectangle.Y + num);
						array[6] = new PointF(absoluteRectangle.X, absoluteRectangle.Y + num);
					}
					else if (anchorPoint.X < rectanglePosition.X && anchorPoint.Y >= rectanglePosition.Y && anchorPoint.Y <= rectanglePosition.Bottom)
					{
						array[0] = absoluteRectangle.Location;
						array[1] = new PointF(absoluteRectangle.Right, absoluteRectangle.Y);
						array[2] = new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom);
						array[3] = new PointF(absoluteRectangle.X, absoluteRectangle.Bottom);
						array[4] = new PointF(absoluteRectangle.X, (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0 + num));
						array[5] = absolutePoint;
						array[6] = new PointF(absoluteRectangle.X, (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0 - num));
					}
					graphicsPath = new GraphicsPath();
					graphicsPath.AddLines(array);
					graphicsPath.CloseAllFigures();
					graphics.GetRelativeRectangle(graphicsPath.GetBounds());
					graphics.DrawPathAbs(graphicsPath, this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, this.LineWidth, this.LineStyle, PenAlignment.Center, this.ShadowOffset, this.ShadowColor);
				}
			}
			if (!flag)
			{
				graphics.FillRectangleRel(rectanglePosition, this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, this.LineWidth, this.LineStyle, this.ShadowColor, this.ShadowOffset, PenAlignment.Center);
				graphicsPath = new GraphicsPath();
				graphicsPath.AddRectangle(graphics.GetAbsoluteRectangle(rectanglePosition));
			}
			base.DrawText(graphics, rectanglePosition, false, false);
			return graphicsPath;
		}

		private GraphicsPath DrawCloudCallout(ChartGraphics graphics, RectangleF rectanglePosition, PointF anchorPoint)
		{
			RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(rectanglePosition);
			if (!float.IsNaN(anchorPoint.X) && !float.IsNaN(anchorPoint.Y) && !rectanglePosition.Contains(anchorPoint.X, anchorPoint.Y))
			{
				PointF absolutePoint = graphics.GetAbsolutePoint(new PointF((float)(rectanglePosition.X + rectanglePosition.Width / 2.0), (float)(rectanglePosition.Y + rectanglePosition.Height / 2.0)));
				SizeF absoluteSize = graphics.GetAbsoluteSize(new SizeF(rectanglePosition.Width, rectanglePosition.Height));
				absoluteSize.Width /= 10f;
				absoluteSize.Height /= 10f;
				PointF absolutePoint2 = graphics.GetAbsolutePoint(new PointF(anchorPoint.X, anchorPoint.Y));
				PointF pointF = absolutePoint2;
				float num = absolutePoint2.X - absolutePoint.X;
				float num2 = absolutePoint2.Y - absolutePoint.Y;
				PointF pointF2 = PointF.Empty;
				if (anchorPoint.Y < rectanglePosition.Y)
				{
					pointF2 = CalloutAnnotation.GetIntersectionY(absolutePoint, absolutePoint2, absoluteRectangle.Y);
					if (pointF2.X < absoluteRectangle.X)
					{
						pointF2 = CalloutAnnotation.GetIntersectionX(absolutePoint, absolutePoint2, absoluteRectangle.X);
					}
					else if (pointF2.X > absoluteRectangle.Right)
					{
						pointF2 = CalloutAnnotation.GetIntersectionX(absolutePoint, absolutePoint2, absoluteRectangle.Right);
					}
				}
				else if (anchorPoint.Y > rectanglePosition.Bottom)
				{
					pointF2 = CalloutAnnotation.GetIntersectionY(absolutePoint, absolutePoint2, absoluteRectangle.Bottom);
					if (pointF2.X < absoluteRectangle.X)
					{
						pointF2 = CalloutAnnotation.GetIntersectionX(absolutePoint, absolutePoint2, absoluteRectangle.X);
					}
					else if (pointF2.X > absoluteRectangle.Right)
					{
						pointF2 = CalloutAnnotation.GetIntersectionX(absolutePoint, absolutePoint2, absoluteRectangle.Right);
					}
				}
				else
				{
					pointF2 = ((!(anchorPoint.X < rectanglePosition.X)) ? CalloutAnnotation.GetIntersectionX(absolutePoint, absolutePoint2, absoluteRectangle.Right) : CalloutAnnotation.GetIntersectionX(absolutePoint, absolutePoint2, absoluteRectangle.X));
				}
				SizeF sizeF = new SizeF(Math.Abs(absolutePoint.X - pointF2.X), Math.Abs(absolutePoint.Y - pointF2.Y));
				num = ((!(num > 0.0)) ? (num + sizeF.Width) : (num - sizeF.Width));
				num2 = ((!(num2 > 0.0)) ? (num2 + sizeF.Height) : (num2 - sizeF.Height));
				for (int i = 0; i < 3; i++)
				{
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddEllipse((float)(pointF.X - absoluteSize.Width / 2.0), (float)(pointF.Y - absoluteSize.Height / 2.0), absoluteSize.Width, absoluteSize.Height);
						graphics.DrawPathAbs(graphicsPath, this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, 1, this.LineStyle, PenAlignment.Center, this.ShadowOffset, this.ShadowColor);
						absoluteSize.Width *= 1.5f;
						absoluteSize.Height *= 1.5f;
						pointF.X -= (float)(num / 3.0 + (float)i * (num / 10.0));
						pointF.Y -= (float)(num2 / 3.0 + (float)i * (num2 / 10.0));
					}
				}
			}
			GraphicsPath graphicsPath2 = CalloutAnnotation.GetCloudPath(absoluteRectangle);
			graphics.DrawPathAbs(graphicsPath2, this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, 1, this.LineStyle, PenAlignment.Center, this.ShadowOffset, this.ShadowColor);
			using (GraphicsPath path = CalloutAnnotation.GetCloudOutlinePath(absoluteRectangle))
			{
				graphics.DrawPathAbs(path, this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, 1, this.LineStyle, PenAlignment.Center);
			}
			base.DrawText(graphics, rectanglePosition, true, false);
			return graphicsPath2;
		}

		private GraphicsPath DrawPerspectiveCallout(ChartGraphics graphics, RectangleF rectanglePosition, PointF anchorPoint)
		{
			graphics.FillRectangleRel(rectanglePosition, this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, this.LineWidth, this.LineStyle, this.ShadowColor, 0, PenAlignment.Center);
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(graphics.GetAbsoluteRectangle(rectanglePosition));
			base.DrawText(graphics, rectanglePosition, false, false);
			if (!float.IsNaN(anchorPoint.X) && !float.IsNaN(anchorPoint.Y) && !rectanglePosition.Contains(anchorPoint.X, anchorPoint.Y))
			{
				Color[] array = new Color[2];
				Color beginColor = this.BackColor.IsEmpty ? Color.White : this.BackColor;
				array[0] = graphics.GetBrightGradientColor(beginColor, 0.6);
				array[1] = graphics.GetBrightGradientColor(beginColor, 0.8);
				GraphicsPath[] array2 = new GraphicsPath[2];
				using (array2[0] = new GraphicsPath())
				{
					using (array2[1] = new GraphicsPath())
					{
						RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(rectanglePosition);
						PointF absolutePoint = graphics.GetAbsolutePoint(anchorPoint);
						if (anchorPoint.Y < rectanglePosition.Y)
						{
							PointF[] points = new PointF[3]
							{
								new PointF(absoluteRectangle.X, absoluteRectangle.Y),
								new PointF(absoluteRectangle.Right, absoluteRectangle.Y),
								new PointF(absolutePoint.X, absolutePoint.Y)
							};
							array2[0].AddLines(points);
							if (anchorPoint.X < rectanglePosition.X)
							{
								PointF[] points2 = new PointF[3]
								{
									new PointF(absoluteRectangle.X, absoluteRectangle.Bottom),
									new PointF(absoluteRectangle.X, absoluteRectangle.Y),
									new PointF(absolutePoint.X, absolutePoint.Y)
								};
								array2[1].AddLines(points2);
							}
							else if (anchorPoint.X > rectanglePosition.Right)
							{
								PointF[] points3 = new PointF[3]
								{
									new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom),
									new PointF(absoluteRectangle.Right, absoluteRectangle.Y),
									new PointF(absolutePoint.X, absolutePoint.Y)
								};
								array2[1].AddLines(points3);
							}
						}
						else if (anchorPoint.Y > rectanglePosition.Bottom)
						{
							PointF[] points4 = new PointF[3]
							{
								new PointF(absoluteRectangle.X, absoluteRectangle.Bottom),
								new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom),
								new PointF(absolutePoint.X, absolutePoint.Y)
							};
							array2[0].AddLines(points4);
							if (anchorPoint.X < rectanglePosition.X)
							{
								PointF[] points5 = new PointF[3]
								{
									new PointF(absoluteRectangle.X, absoluteRectangle.Bottom),
									new PointF(absoluteRectangle.X, absoluteRectangle.Y),
									new PointF(absolutePoint.X, absolutePoint.Y)
								};
								array2[1].AddLines(points5);
							}
							else if (anchorPoint.X > rectanglePosition.Right)
							{
								PointF[] points6 = new PointF[3]
								{
									new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom),
									new PointF(absoluteRectangle.Right, absoluteRectangle.Y),
									new PointF(absolutePoint.X, absolutePoint.Y)
								};
								array2[1].AddLines(points6);
							}
						}
						else if (anchorPoint.X < rectanglePosition.X)
						{
							PointF[] points7 = new PointF[3]
							{
								new PointF(absoluteRectangle.X, absoluteRectangle.Bottom),
								new PointF(absoluteRectangle.X, absoluteRectangle.Y),
								new PointF(absolutePoint.X, absolutePoint.Y)
							};
							array2[1].AddLines(points7);
						}
						else if (anchorPoint.X > rectanglePosition.Right)
						{
							PointF[] points8 = new PointF[3]
							{
								new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom),
								new PointF(absoluteRectangle.Right, absoluteRectangle.Y),
								new PointF(absolutePoint.X, absolutePoint.Y)
							};
							array2[1].AddLines(points8);
						}
						int num = 0;
						GraphicsPath[] array3 = array2;
						foreach (GraphicsPath graphicsPath2 in array3)
						{
							if (graphicsPath2.PointCount > 0)
							{
								graphicsPath2.CloseAllFigures();
								graphics.DrawPathAbs(graphicsPath2, array[num], this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, this.LineWidth, this.LineStyle, PenAlignment.Center);
								graphicsPath.SetMarkers();
								graphicsPath.AddPath(graphicsPath2, false);
							}
							num++;
						}
						return graphicsPath;
					}
				}
			}
			return graphicsPath;
		}

		private GraphicsPath DrawRectangleLineCallout(ChartGraphics graphics, RectangleF rectanglePosition, PointF anchorPoint, bool drawRectangle)
		{
			if (drawRectangle)
			{
				graphics.FillRectangleRel(rectanglePosition, this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, this.LineWidth, this.LineStyle, this.ShadowColor, this.ShadowOffset, PenAlignment.Center);
				base.DrawText(graphics, rectanglePosition, false, false);
			}
			else
			{
				rectanglePosition = base.DrawText(graphics, rectanglePosition, false, true);
				SizeF relativeSize = graphics.GetRelativeSize(new SizeF(2f, 2f));
				rectanglePosition.Inflate(relativeSize);
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(graphics.GetAbsoluteRectangle(rectanglePosition));
			PointF relative = new PointF(rectanglePosition.X, rectanglePosition.Bottom);
			PointF relative2 = new PointF(rectanglePosition.Right, rectanglePosition.Bottom);
			if (!float.IsNaN(anchorPoint.X) && !float.IsNaN(anchorPoint.Y))
			{
				if (!rectanglePosition.Contains(anchorPoint.X, anchorPoint.Y))
				{
					PointF empty = PointF.Empty;
					if (anchorPoint.X < rectanglePosition.X)
					{
						empty.X = rectanglePosition.X;
					}
					else if (anchorPoint.X > rectanglePosition.Right)
					{
						empty.X = rectanglePosition.Right;
					}
					else
					{
						empty.X = (float)(rectanglePosition.X + rectanglePosition.Width / 2.0);
					}
					if (anchorPoint.Y < rectanglePosition.Y)
					{
						empty.Y = rectanglePosition.Y;
					}
					else if (anchorPoint.Y > rectanglePosition.Bottom)
					{
						empty.Y = rectanglePosition.Bottom;
					}
					else
					{
						empty.Y = (float)(rectanglePosition.Y + rectanglePosition.Height / 2.0);
					}
					bool flag = false;
					LineCap startCap = LineCap.Flat;
					if (this.CalloutAnchorCap != 0)
					{
						flag = true;
						startCap = graphics.pen.StartCap;
						if (this.CalloutAnchorCap == LineAnchorCap.Arrow)
						{
							if (this.LineWidth < 4)
							{
								int num = 3 - this.LineWidth;
								graphics.pen.StartCap = LineCap.Custom;
								graphics.pen.CustomStartCap = new AdjustableArrowCap((float)(this.LineWidth + num), (float)(this.LineWidth + num), true);
							}
							else
							{
								graphics.pen.StartCap = LineCap.ArrowAnchor;
							}
						}
						else if (this.CalloutAnchorCap == LineAnchorCap.Diamond)
						{
							graphics.pen.StartCap = LineCap.DiamondAnchor;
						}
						else if (this.CalloutAnchorCap == LineAnchorCap.Round)
						{
							graphics.pen.StartCap = LineCap.RoundAnchor;
						}
						else if (this.CalloutAnchorCap == LineAnchorCap.Square)
						{
							graphics.pen.StartCap = LineCap.SquareAnchor;
						}
					}
					graphics.DrawLineAbs(this.LineColor, this.LineWidth, this.LineStyle, graphics.GetAbsolutePoint(anchorPoint), graphics.GetAbsolutePoint(empty), this.ShadowColor, this.ShadowOffset);
					using (GraphicsPath graphicsPath2 = new GraphicsPath())
					{
						graphicsPath2.AddLine(graphics.GetAbsolutePoint(anchorPoint), graphics.GetAbsolutePoint(empty));
						ChartGraphics.Widen(graphicsPath2, new Pen(Color.Black, (float)(this.LineWidth + 2)));
						graphicsPath.SetMarkers();
						graphicsPath.AddPath(graphicsPath2, false);
					}
					if (flag)
					{
						graphics.pen.StartCap = startCap;
					}
					if (anchorPoint.Y < rectanglePosition.Y)
					{
						relative.Y = rectanglePosition.Y;
						relative2.Y = rectanglePosition.Y;
					}
					else if (anchorPoint.Y > rectanglePosition.Y && anchorPoint.Y < rectanglePosition.Bottom)
					{
						relative.Y = rectanglePosition.Y;
						relative2.Y = rectanglePosition.Bottom;
						if (anchorPoint.X < rectanglePosition.X)
						{
							relative.X = rectanglePosition.X;
							relative2.X = rectanglePosition.X;
						}
						else
						{
							relative.X = rectanglePosition.Right;
							relative2.X = rectanglePosition.Right;
						}
					}
				}
				if (!drawRectangle)
				{
					graphics.DrawLineAbs(this.LineColor, this.LineWidth, this.LineStyle, graphics.GetAbsolutePoint(relative), graphics.GetAbsolutePoint(relative2), this.ShadowColor, this.ShadowOffset);
					using (GraphicsPath graphicsPath3 = new GraphicsPath())
					{
						graphicsPath3.AddLine(graphics.GetAbsolutePoint(relative), graphics.GetAbsolutePoint(relative2));
						ChartGraphics.Widen(graphicsPath3, new Pen(Color.Black, (float)(this.LineWidth + 2)));
						graphicsPath.SetMarkers();
						graphicsPath.AddPath(graphicsPath3, false);
						return graphicsPath;
					}
				}
			}
			return graphicsPath;
		}

		internal override bool IsAnchorDrawn()
		{
			return true;
		}

		private static GraphicsPath GetCloudOutlinePath(RectangleF position)
		{
			if (CalloutAnnotation.cloudOutlinePath == null)
			{
				CalloutAnnotation.GetCloudPath(position);
			}
			GraphicsPath graphicsPath = (GraphicsPath)CalloutAnnotation.cloudOutlinePath.Clone();
			Matrix matrix = new Matrix();
			matrix.Translate((float)(0.0 - CalloutAnnotation.cloudBounds.X), (float)(0.0 - CalloutAnnotation.cloudBounds.Y));
			graphicsPath.Transform(matrix);
			matrix = new Matrix();
			matrix.Translate(position.X, position.Y);
			matrix.Scale(position.Width / CalloutAnnotation.cloudBounds.Width, position.Height / CalloutAnnotation.cloudBounds.Height);
			graphicsPath.Transform(matrix);
			return graphicsPath;
		}

		private static GraphicsPath GetCloudPath(RectangleF position)
		{
			if (CalloutAnnotation.cloudPath == null)
			{
				CalloutAnnotation.cloudPath = new GraphicsPath();
				CalloutAnnotation.cloudPath.AddBezier(1689.5f, 1998.6f, 1581.8f, 2009.4f, 1500f, 2098.1f, 1500f, 2204f);
				CalloutAnnotation.cloudPath.AddBezier(1500f, 2204f, 1499.9f, 2277.2f, 1539.8f, 2345.1f, 1604.4f, 2382.1f);
				CalloutAnnotation.cloudPath.AddBezier(1603.3f, 2379.7f, 1566.6f, 2417.8f, 1546.2f, 2468.1f, 1546.2f, 2520.1f);
				CalloutAnnotation.cloudPath.AddBezier(1546.2f, 2520.1f, 1546.2f, 2633.7f, 1641.1f, 2725.7f, 1758.1f, 2725.7f);
				CalloutAnnotation.cloudPath.AddBezier(1758.1f, 2725.7f, 1766.3f, 2725.6f, 1774.6f, 2725.2f, 1782.8f, 2724.2f);
				CalloutAnnotation.cloudPath.AddBezier(1781.7f, 2725.6f, 1848.5f, 2839.4f, 1972.8f, 2909.7f, 2107.3f, 2909.7f);
				CalloutAnnotation.cloudPath.AddBezier(2107.3f, 2909.7f, 2175.4f, 2909.7f, 2242.3f, 2891.6f, 2300.6f, 2857.4f);
				CalloutAnnotation.cloudPath.AddBezier(2300f, 2857.6f, 2360.9f, 2946.5f, 2463.3f, 2999.7f, 2572.9f, 2999.7f);
				CalloutAnnotation.cloudPath.AddBezier(2572.9f, 2999.7f, 2717.5f, 2999.7f, 2845.2f, 2907.4f, 2887.1f, 2772.5f);
				CalloutAnnotation.cloudPath.AddBezier(2887.4f, 2774.3f, 2932.1f, 2801.4f, 2983.6f, 2815.7f, 3036.3f, 2815.7f);
				CalloutAnnotation.cloudPath.AddBezier(3036.3f, 2815.7f, 3190.7f, 2815.7f, 3316.3f, 2694.8f, 3317.5f, 2544.8f);
				CalloutAnnotation.cloudPath.AddBezier(3317f, 2544.1f, 3479.2f, 2521.5f, 3599.7f, 2386.5f, 3599.7f, 2227.2f);
				CalloutAnnotation.cloudPath.AddBezier(3599.7f, 2227.2f, 3599.7f, 2156.7f, 3575.7f, 2088.1f, 3531.6f, 2032.2f);
				CalloutAnnotation.cloudPath.AddBezier(3530.9f, 2032f, 3544.7f, 2000.6f, 3551.9f, 1966.7f, 3551.9f, 1932.5f);
				CalloutAnnotation.cloudPath.AddBezier(3551.9f, 1932.5f, 3551.9f, 1818.6f, 3473.5f, 1718.8f, 3360.7f, 1688.8f);
				CalloutAnnotation.cloudPath.AddBezier(3361.6f, 1688.3f, 3341.4f, 1579.3f, 3243.5f, 1500f, 3129.3f, 1500f);
				CalloutAnnotation.cloudPath.AddBezier(3129.3f, 1500f, 3059.8f, 1499.9f, 2994f, 1529.6f, 2949.1f, 1580.9f);
				CalloutAnnotation.cloudPath.AddBezier(2949.5f, 1581.3f, 2909.4f, 1530f, 2847f, 1500f, 2780.8f, 1500f);
				CalloutAnnotation.cloudPath.AddBezier(2780.8f, 1500f, 2700.4f, 1499.9f, 2626.8f, 1544.2f, 2590.9f, 1614.2f);
				CalloutAnnotation.cloudPath.AddBezier(2591.7f, 1617.6f, 2543.2f, 1571.1f, 2477.9f, 1545.1f, 2409.8f, 1545.1f);
				CalloutAnnotation.cloudPath.AddBezier(2409.8f, 1545.1f, 2313.9f, 1545.1f, 2225.9f, 1596.6f, 2180.8f, 1679f);
				CalloutAnnotation.cloudPath.AddBezier(2180.1f, 1680.7f, 2129.7f, 1652f, 2072.4f, 1636.9f, 2014.1f, 1636.9f);
				CalloutAnnotation.cloudPath.AddBezier(2014.1f, 1636.9f, 1832.8f, 1636.9f, 1685.9f, 1779.8f, 1685.9f, 1956f);
				CalloutAnnotation.cloudPath.AddBezier(1685.9f, 1956f, 1685.8f, 1970.4f, 1686.9f, 1984.8f, 1688.8f, 1999f);
				CalloutAnnotation.cloudPath.CloseAllFigures();
				CalloutAnnotation.cloudOutlinePath = new GraphicsPath();
				CalloutAnnotation.cloudOutlinePath.AddBezier(1604.4f, 2382.1f, 1636.8f, 2400.6f, 1673.6f, 2410.3f, 1711.2f, 2410.3f);
				CalloutAnnotation.cloudOutlinePath.AddBezier(1711.2f, 2410.3f, 1716.6f, 2410.3f, 1722.2f, 2410.2f, 1727.6f, 2409.8f);
				CalloutAnnotation.cloudOutlinePath.StartFigure();
				CalloutAnnotation.cloudOutlinePath.AddBezier(1782.8f, 2724.2f, 1801.3f, 2722.2f, 1819.4f, 2717.7f, 1836.7f, 2711f);
				CalloutAnnotation.cloudOutlinePath.StartFigure();
				CalloutAnnotation.cloudOutlinePath.AddBezier(2267.6f, 2797.2f, 2276.1f, 2818.4f, 2287f, 2838.7f, 2300f, 2857.6f);
				CalloutAnnotation.cloudOutlinePath.StartFigure();
				CalloutAnnotation.cloudOutlinePath.AddBezier(2887.1f, 2772.5f, 2893.8f, 2750.9f, 2898.1f, 2728.7f, 2900f, 2706.3f);
				CalloutAnnotation.cloudOutlinePath.StartFigure();
				CalloutAnnotation.cloudOutlinePath.AddBezier(3460.5f, 2124.9f, 3491f, 2099.7f, 3515f, 2067.8f, 3530.9f, 2032f);
				CalloutAnnotation.cloudOutlinePath.StartFigure();
				CalloutAnnotation.cloudOutlinePath.AddBezier(3365.3f, 1732.2f, 3365.3f, 1731.1f, 3365.4f, 1730.1f, 3365.4f, 1729f);
				CalloutAnnotation.cloudOutlinePath.AddBezier(3365.4f, 1729f, 3365.4f, 1715.3f, 3364.1f, 1701.7f, 3361.6f, 1688.3f);
				CalloutAnnotation.cloudOutlinePath.StartFigure();
				CalloutAnnotation.cloudOutlinePath.AddBezier(2949.1f, 1580.9f, 2934.4f, 1597.8f, 2922.3f, 1616.6f, 2913.1f, 1636.9f);
				CalloutAnnotation.cloudOutlinePath.CloseFigure();
				CalloutAnnotation.cloudOutlinePath.StartFigure();
				CalloutAnnotation.cloudOutlinePath.AddBezier(2590.9f, 1614.2f, 2583.1f, 1629.6f, 2577.2f, 1645.8f, 2573.4f, 1662.5f);
				CalloutAnnotation.cloudOutlinePath.StartFigure();
				CalloutAnnotation.cloudOutlinePath.AddBezier(2243.3f, 1727.5f, 2224.2f, 1709.4f, 2203f, 1693.8f, 2180.1f, 1680.7f);
				CalloutAnnotation.cloudOutlinePath.StartFigure();
				CalloutAnnotation.cloudOutlinePath.AddBezier(1688.8f, 1999f, 1691.1f, 2015.7f, 1694.8f, 2032.2f, 1699.9f, 2048.3f);
				CalloutAnnotation.cloudOutlinePath.CloseAllFigures();
				CalloutAnnotation.cloudBounds = CalloutAnnotation.cloudPath.GetBounds();
			}
			GraphicsPath graphicsPath = (GraphicsPath)CalloutAnnotation.cloudPath.Clone();
			Matrix matrix = new Matrix();
			matrix.Translate((float)(0.0 - CalloutAnnotation.cloudBounds.X), (float)(0.0 - CalloutAnnotation.cloudBounds.Y));
			graphicsPath.Transform(matrix);
			matrix = new Matrix();
			matrix.Translate(position.X, position.Y);
			matrix.Scale(position.Width / CalloutAnnotation.cloudBounds.Width, position.Height / CalloutAnnotation.cloudBounds.Height);
			graphicsPath.Transform(matrix);
			return graphicsPath;
		}

		internal static PointF GetIntersectionY(PointF firstPoint, PointF secondPoint, float pointY)
		{
			PointF result = default(PointF);
			result.Y = pointY;
			result.X = (pointY - firstPoint.Y) * (secondPoint.X - firstPoint.X) / (secondPoint.Y - firstPoint.Y) + firstPoint.X;
			return result;
		}

		internal static PointF GetIntersectionX(PointF firstPoint, PointF secondPoint, float pointX)
		{
			PointF result = default(PointF);
			result.X = pointX;
			result.Y = (pointX - firstPoint.X) * (secondPoint.Y - firstPoint.Y) / (secondPoint.X - firstPoint.X) + firstPoint.Y;
			return result;
		}

		private void PathAddLineAsSegments(GraphicsPath path, float x1, float y1, float x2, float y2, int segments)
		{
			if (x1 == x2)
			{
				float num = (y2 - y1) / (float)segments;
				for (int i = 0; i < segments; i++)
				{
					path.AddLine(x1, y1, x1, y1 + num);
					y1 += num;
				}
				return;
			}
			if (y1 == y2)
			{
				float num2 = (x2 - x1) / (float)segments;
				for (int j = 0; j < segments; j++)
				{
					path.AddLine(x1, y1, x1 + num2, y1);
					x1 += num2;
				}
				return;
			}
			throw new InvalidOperationException(SR.ExceptionAnnotationPathAddLineAsSegmentsInvalid);
		}

		private GraphicsPath CreateRoundedRectPath(RectangleF rect, float cornerRadius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			int segments = 10;
			this.PathAddLineAsSegments(graphicsPath, rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius, rect.Y, segments);
			graphicsPath.AddArc((float)(rect.Right - 2.0 * cornerRadius), rect.Y, (float)(2.0 * cornerRadius), (float)(2.0 * cornerRadius), 270f, 90f);
			this.PathAddLineAsSegments(graphicsPath, rect.Right, rect.Y + cornerRadius, rect.Right, rect.Bottom - cornerRadius, segments);
			graphicsPath.AddArc((float)(rect.Right - 2.0 * cornerRadius), (float)(rect.Bottom - 2.0 * cornerRadius), (float)(2.0 * cornerRadius), (float)(2.0 * cornerRadius), 0f, 90f);
			this.PathAddLineAsSegments(graphicsPath, rect.Right - cornerRadius, rect.Bottom, rect.X + cornerRadius, rect.Bottom, segments);
			graphicsPath.AddArc(rect.X, (float)(rect.Bottom - 2.0 * cornerRadius), (float)(2.0 * cornerRadius), (float)(2.0 * cornerRadius), 90f, 90f);
			this.PathAddLineAsSegments(graphicsPath, rect.X, rect.Bottom - cornerRadius, rect.X, rect.Y + cornerRadius, segments);
			graphicsPath.AddArc(rect.X, rect.Y, (float)(2.0 * cornerRadius), (float)(2.0 * cornerRadius), 180f, 90f);
			return graphicsPath;
		}
	}
}
