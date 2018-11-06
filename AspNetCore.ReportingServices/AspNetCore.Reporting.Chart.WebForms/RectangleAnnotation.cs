using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeRectangleAnnotation_RectangleAnnotation")]
	internal class RectangleAnnotation : TextAnnotation
	{
		internal bool isRectVisible = true;

		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLineColor")]
		[Browsable(true)]
		[SRCategory("CategoryAttributeAppearance")]
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

		[SRDescription("DescriptionAttributeLineWidth")]
		[Browsable(true)]
		[SRCategory("CategoryAttributeAppearance")]
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

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLineStyle4")]
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

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[Browsable(true)]
		[SRDescription("DescriptionAttributeBackColor8")]
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

		[SRDescription("DescriptionAttributeBackHatchStyle5")]
		[NotifyParentProperty(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
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

		[SRDescription("DescriptionAttributeBackGradientType12")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(true)]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
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

		[Browsable(true)]
		[SRDescription("DescriptionAttributeBackGradientEndColor8")]
		[SRCategory("CategoryAttributeAppearance")]
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType
		{
			get
			{
				return "Rectangle";
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

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			this.Chart = chart;
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			SizeF empty3 = SizeF.Empty;
			((Annotation)this).GetRelativePosition(out empty, out empty3, out empty2);
			PointF pointF = new PointF(empty.X + empty3.Width, empty.Y + empty3.Height);
			RectangleF rectangleF = new RectangleF(empty, new SizeF(pointF.X - empty.X, pointF.Y - empty.Y));
			RectangleF rectF = new RectangleF(rectangleF.Location, rectangleF.Size);
			if (rectF.Width < 0.0)
			{
				rectF.X = rectF.Right;
				rectF.Width = (float)(0.0 - rectF.Width);
			}
			if (rectF.Height < 0.0)
			{
				rectF.Y = rectF.Bottom;
				rectF.Height = (float)(0.0 - rectF.Height);
			}
			if (!float.IsNaN(rectF.X) && !float.IsNaN(rectF.Y) && !float.IsNaN(rectF.Right) && !float.IsNaN(rectF.Bottom))
			{
				if (this.isRectVisible && this.Chart.chartPicture.common.ProcessModePaint)
				{
					graphics.FillRectangleRel(rectF, this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, this.LineWidth, this.LineStyle, this.ShadowColor, this.ShadowOffset, PenAlignment.Center, base.isEllipse, 1, false);
				}
				base.Paint(chart, graphics);
			}
		}
	}
}
