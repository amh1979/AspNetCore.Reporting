using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAnnotationGroup_AnnotationGroup")]
	internal class AnnotationGroup : Annotation
	{
		internal AnnotationCollection annotations;

		[DefaultValue("NotSet")]
		[SRDescription("DescriptionAttributeAnnotationGroup_ClipToChartArea")]
		[SRCategory("CategoryAttributeMisc")]
		[TypeConverter(typeof(LegendAreaNameConverter))]
		public override string ClipToChartArea
		{
			get
			{
				return base.ClipToChartArea;
			}
			set
			{
				base.ClipToChartArea = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.ClipToChartArea = value;
				}
			}
		}

		[SRDescription("DescriptionAttributeAnnotationGroup_SizeAlwaysRelative")]
		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(true)]
		public override bool SizeAlwaysRelative
		{
			get
			{
				return base.SizeAlwaysRelative;
			}
			set
			{
				base.SizeAlwaysRelative = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.SizeAlwaysRelative = value;
				}
			}
		}

		[SRDescription("DescriptionAttributeAnnotationGroup_Selected")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(false)]
		public override bool Selected
		{
			get
			{
				return base.Selected;
			}
			set
			{
				base.Selected = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.Selected = false;
				}
			}
		}

		[SRDescription("DescriptionAttributeAnnotationGroup_Visible")]
		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(true)]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.Visible = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeAlignment7")]
		[DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
		public override ContentAlignment Alignment
		{
			get
			{
				return base.Alignment;
			}
			set
			{
				base.Alignment = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.Alignment = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTextColor6")]
		public override Color TextColor
		{
			get
			{
				return base.TextColor;
			}
			set
			{
				base.TextColor = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.TextColor = value;
				}
			}
		}

		[SRDescription("DescriptionAttributeTextFont")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		public override Font TextFont
		{
			get
			{
				return base.TextFont;
			}
			set
			{
				base.TextFont = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.TextFont = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTextStyle3")]
		[DefaultValue(typeof(TextStyle), "Default")]
		public override TextStyle TextStyle
		{
			get
			{
				return base.TextStyle;
			}
			set
			{
				base.TextStyle = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.TextStyle = value;
				}
			}
		}

		[SRDescription("DescriptionAttributeLineColor")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		public override Color LineColor
		{
			get
			{
				return base.LineColor;
			}
			set
			{
				base.LineColor = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.LineColor = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLineWidth7")]
		public override int LineWidth
		{
			get
			{
				return base.LineWidth;
			}
			set
			{
				base.LineWidth = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.LineWidth = value;
				}
			}
		}

		[DefaultValue(ChartDashStyle.Solid)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLineStyle6")]
		public override ChartDashStyle LineStyle
		{
			get
			{
				return base.LineStyle;
			}
			set
			{
				base.LineStyle = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.LineStyle = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeBackColor8")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.BackColor = value;
				}
			}
		}

		[DefaultValue(ChartHatchStyle.None)]
		[SRDescription("DescriptionAttributeBackHatchStyle")]
		[SRCategory("CategoryAttributeAppearance")]
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
				foreach (Annotation annotation in this.annotations)
				{
					annotation.BackHatchStyle = value;
				}
			}
		}

		[SRDescription("DescriptionAttributeBackGradientType8")]
		[SRCategory("CategoryAttributeAppearance")]
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
				foreach (Annotation annotation in this.annotations)
				{
					annotation.BackGradientType = value;
				}
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeAnnotationGroup_BackGradientEndColor")]
		public override Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.BackGradientEndColor = value;
				}
			}
		}

		[DefaultValue(typeof(Color), "128,0,0,0")]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeAnnotationGroup_ShadowColor")]
		public override Color ShadowColor
		{
			get
			{
				return base.ShadowColor;
			}
			set
			{
				base.ShadowColor = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.ShadowColor = value;
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeAnnotationGroup_ShadowOffset")]
		public override int ShadowOffset
		{
			get
			{
				return base.ShadowOffset;
			}
			set
			{
				base.ShadowOffset = value;
				foreach (Annotation annotation in this.annotations)
				{
					annotation.ShadowOffset = value;
				}
			}
		}

		[SRDescription("DescriptionAttributeAnnotationGroup_Annotations")]
		[SRCategory("CategoryAttributeAnnotations")]
		public AnnotationCollection Annotations
		{
			get
			{
				return this.annotations;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override string AnnotationType
		{
			get
			{
				return "Group";
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal override SelectionPointsStyle SelectionPointsStyle
		{
			get
			{
				return SelectionPointsStyle.Rectangle;
			}
		}

		public AnnotationGroup()
		{
			this.annotations = new AnnotationCollection();
			this.annotations.annotationGroup = this;
		}

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			this.Chart = chart;
			foreach (Annotation annotation in this.annotations)
			{
				annotation.Paint(chart, graphics);
			}
			if ((!this.Chart.chartPicture.common.ProcessModePaint || !this.Selected) && !this.Chart.chartPicture.common.ProcessModeRegions)
			{
				return;
			}
			PointF empty = PointF.Empty;
			PointF empty2 = PointF.Empty;
			SizeF empty3 = SizeF.Empty;
			((Annotation)this).GetRelativePosition(out empty, out empty3, out empty2);
			PointF pointF = new PointF(empty.X + empty3.Width, empty.Y + empty3.Height);
			RectangleF rectangleF = new RectangleF(empty, new SizeF(pointF.X - empty.X, pointF.Y - empty.Y));
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
			if (!rectangleF.IsEmpty && !float.IsNaN(rectangleF.X) && !float.IsNaN(rectangleF.Y) && !float.IsNaN(rectangleF.Right) && !float.IsNaN(rectangleF.Bottom))
			{
				bool processModePaint = this.Chart.chartPicture.common.ProcessModePaint;
				if (this.Chart.chartPicture.common.ProcessModeRegions)
				{
					this.Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, rectangleF, base.ReplaceKeywords(this.ToolTip), base.ReplaceKeywords(this.Href), base.ReplaceKeywords(this.MapAreaAttributes), this, ChartElementType.Annotation, string.Empty);
				}
				this.PaintSelectionHandles(graphics, rectangleF, null);
			}
		}
	}
}
