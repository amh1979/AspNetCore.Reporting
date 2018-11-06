using AspNetCore.Reporting.Chart.WebForms.Borders3D;
using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeBorder3DAnnotation_Border3DAnnotation")]
	internal class Border3DAnnotation : RectangleAnnotation
	{
		private BorderSkinAttributes borderSkin = new BorderSkinAttributes();

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
				return "Border3D";
			}
		}

		[DefaultValue(null)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(LegendConverter))]
		public BorderSkinAttributes BorderSkin
		{
			get
			{
				return this.borderSkin;
			}
			set
			{
				this.borderSkin = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeChart")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal override Chart Chart
		{
			get
			{
				return base.Chart;
			}
			set
			{
				base.Chart = value;
				if (value != null)
				{
					this.borderSkin.serviceContainer = value.serviceContainer;
				}
			}
		}

		public Border3DAnnotation()
		{
			base.isRectVisible = false;
			this.borderSkin.PageColor = Color.Transparent;
			this.borderSkin.SkinStyle = BorderSkinStyle.Raised;
			base.lineColor = Color.Empty;
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
			RectangleF rectangleF2 = new RectangleF(rectangleF.Location, rectangleF.Size);
			if (rectangleF2.Width < 0.0)
			{
				rectangleF2.X = rectangleF2.Right;
				rectangleF2.Width = (float)(0.0 - rectangleF2.Width);
			}
			if (rectangleF2.Height < 0.0)
			{
				rectangleF2.Y = rectangleF2.Bottom;
				rectangleF2.Height = (float)(0.0 - rectangleF2.Height);
			}
			if (!float.IsNaN(rectangleF2.X) && !float.IsNaN(rectangleF2.Y) && !float.IsNaN(rectangleF2.Right) && !float.IsNaN(rectangleF2.Bottom))
			{
				if (this.Chart.chartPicture.common.ProcessModePaint)
				{
					RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(rectangleF2);
					if (absoluteRectangle.Width > 30.0 && absoluteRectangle.Height > 30.0)
					{
						graphics.Draw3DBorderRel(this.borderSkin, rectangleF2, this.BackColor, this.BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, this.BackGradientType, this.BackGradientEndColor, this.LineColor, this.LineWidth, this.LineStyle);
					}
				}
				base.Paint(chart, graphics);
			}
		}

		internal override RectangleF GetTextSpacing(out bool annotationRelative)
		{
			annotationRelative = false;
			RectangleF rectangleF = new RectangleF(3f, 3f, 3f, 3f);
			if (base.GetGraphics() != null)
			{
				rectangleF = base.GetGraphics().GetRelativeRectangle(rectangleF);
			}
			if (this.borderSkin.SkinStyle != 0 && base.GetGraphics() != null && this.Chart != null && this.Chart.chartPicture != null && this.Chart.chartPicture.common != null)
			{
				IBorderType borderType = this.Chart.chartPicture.common.BorderTypeRegistry.GetBorderType(this.borderSkin.SkinStyle.ToString());
				if (borderType != null)
				{
					RectangleF rectangleF2 = new RectangleF(0f, 0f, 100f, 100f);
					borderType.AdjustAreasPosition(base.GetGraphics(), ref rectangleF2);
					rectangleF = new RectangleF((float)(rectangleF2.X + 1.0), (float)(rectangleF2.Y + 1.0), (float)(100.0 - rectangleF2.Right + 2.0), (float)(100.0 - rectangleF2.Bottom + 2.0));
				}
			}
			return rectangleF;
		}
	}
}
