using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeArrowAnnotation_ArrowAnnotation")]
	internal class ArrowAnnotation : Annotation
	{
		private ArrowStyle arrowStyle;

		private int arrowSize = 5;

		[Bindable(true)]
		[SRDescription("DescriptionAttributeArrowAnnotation_ArrowStyle")]
		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ArrowStyle.Simple)]
		public virtual ArrowStyle ArrowStyle
		{
			get
			{
				return this.arrowStyle;
			}
			set
			{
				this.arrowStyle = value;
				base.Invalidate();
			}
		}

		[Bindable(true)]
		[DefaultValue(5)]
		[ParenthesizePropertyName(true)]
		[SRDescription("DescriptionAttributeArrowAnnotation_ArrowSize")]
		[SRCategory("CategoryAttributeAppearance")]
		public virtual int ArrowSize
		{
			get
			{
				return this.arrowSize;
			}
			set
			{
				if (this.arrowSize <= 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAnnotationArrowSizeIsZero);
				}
				if (this.arrowSize > 100)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAnnotationArrowSizeMustBeLessThen100);
				}
				this.arrowSize = value;
				base.Invalidate();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(ContentAlignment), "TopLeft")]
		[SRDescription("DescriptionAttributeAnchorAlignment")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAnchor")]
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

		[SRDescription("DescriptionAttributeAnnotationType4")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		public override string AnnotationType
		{
			get
			{
				return "Arrow";
			}
		}

		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[SRCategory("CategoryAttributeAppearance")]
		internal override SelectionPointsStyle SelectionPointsStyle
		{
			get
			{
				return SelectionPointsStyle.TwoPoints;
			}
		}

		public ArrowAnnotation()
		{
			base.AnchorAlignment = ContentAlignment.TopLeft;
		}

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			this.Chart = chart;
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			SizeF empty3 = SizeF.Empty;
			((Annotation)this).GetRelativePosition(out empty, out empty3, out empty2);
			PointF pointF = new PointF(empty.X + empty3.Width, empty.Y + empty3.Height);
			RectangleF rectangleF = new RectangleF(empty, new SizeF(pointF.X - empty.X, pointF.Y - empty.Y));
			if (!float.IsNaN(empty.X) && !float.IsNaN(empty.Y) && !float.IsNaN(pointF.X) && !float.IsNaN(pointF.Y))
			{
				GraphicsPath arrowPath = this.GetArrowPath(graphics, rectangleF);
				if (this.Chart.chartPicture.common.ProcessModePaint)
				{
					graphics.DrawPathAbs(arrowPath, this.BackColor.IsEmpty ? Color.White : this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, this.LineWidth, this.LineStyle, PenAlignment.Center, this.ShadowOffset, this.ShadowColor);
				}
				if (this.Chart.chartPicture.common.ProcessModeRegions)
				{
					this.Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, arrowPath, false, base.ReplaceKeywords(this.ToolTip), base.ReplaceKeywords(this.Href), base.ReplaceKeywords(this.MapAreaAttributes), this, ChartElementType.Annotation);
				}
				this.PaintSelectionHandles(graphics, rectangleF, null);
			}
		}

		private GraphicsPath GetArrowPath(ChartGraphics graphics, RectangleF position)
		{
			RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(position);
			PointF location = absoluteRectangle.Location;
			PointF pointF = new PointF(absoluteRectangle.Right, absoluteRectangle.Bottom);
			float num = pointF.X - location.X;
			float num2 = pointF.Y - location.Y;
			float num3 = (float)Math.Sqrt((double)(num * num + num2 * num2));
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF[] array = null;
			float num4 = 2.1f;
			if (this.ArrowStyle == ArrowStyle.Simple)
			{
				array = new PointF[7]
				{
					location,
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y - (float)this.ArrowSize * num4),
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y - (float)this.ArrowSize),
					new PointF(location.X + num3, location.Y - (float)this.ArrowSize),
					new PointF(location.X + num3, location.Y + (float)this.ArrowSize),
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y + (float)this.ArrowSize),
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y + (float)this.ArrowSize * num4)
				};
				goto IL_0551;
			}
			if (this.ArrowStyle == ArrowStyle.DoubleArrow)
			{
				array = new PointF[10]
				{
					location,
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y - (float)this.ArrowSize * num4),
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y - (float)this.ArrowSize),
					new PointF(location.X + num3 - (float)this.ArrowSize * num4, location.Y - (float)this.ArrowSize),
					new PointF(location.X + num3 - (float)this.ArrowSize * num4, location.Y - (float)this.ArrowSize * num4),
					new PointF(location.X + num3, location.Y),
					new PointF(location.X + num3 - (float)this.ArrowSize * num4, location.Y + (float)this.ArrowSize * num4),
					new PointF(location.X + num3 - (float)this.ArrowSize * num4, location.Y + (float)this.ArrowSize),
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y + (float)this.ArrowSize),
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y + (float)this.ArrowSize * num4)
				};
				goto IL_0551;
			}
			if (this.ArrowStyle == ArrowStyle.Tailed)
			{
				float num5 = 2.1f;
				array = new PointF[8]
				{
					location,
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y - (float)this.ArrowSize * num4),
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y - (float)this.ArrowSize),
					new PointF(location.X + num3, location.Y - (float)this.ArrowSize * num5),
					new PointF(location.X + num3 - (float)this.ArrowSize * num5, location.Y),
					new PointF(location.X + num3, location.Y + (float)this.ArrowSize * num5),
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y + (float)this.ArrowSize),
					new PointF(location.X + (float)this.ArrowSize * num4, location.Y + (float)this.ArrowSize * num4)
				};
				goto IL_0551;
			}
			throw new InvalidOperationException(SR.ExceptionAnnotationArrowStyleUnknown);
			IL_0551:
			graphicsPath.AddLines(array);
			graphicsPath.CloseAllFigures();
			float num6 = (float)(Math.Atan((double)(num2 / num)) * 180.0 / 3.1415926535897931);
			if (num < 0.0)
			{
				num6 = (float)(num6 + 180.0);
			}
			using (Matrix matrix = new Matrix())
			{
				matrix.RotateAt(num6, location);
				graphicsPath.Transform(matrix);
				return graphicsPath;
			}
		}
	}
}
