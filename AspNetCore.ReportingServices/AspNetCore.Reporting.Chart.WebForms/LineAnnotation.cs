using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLineAnnotation_LineAnnotation")]
	internal class LineAnnotation : Annotation
	{
		private bool drawInfinitive;

		private LineAnchorCap startCap;

		private LineAnchorCap endCap;

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeDrawInfinitive")]
		[DefaultValue(false)]
		public virtual bool DrawInfinitive
		{
			get
			{
				return this.drawInfinitive;
			}
			set
			{
				this.drawInfinitive = value;
				base.Invalidate();
			}
		}

		[DefaultValue(LineAnchorCap.None)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeStartCap3")]
		public virtual LineAnchorCap StartCap
		{
			get
			{
				return this.startCap;
			}
			set
			{
				this.startCap = value;
				base.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeStartCap3")]
		[DefaultValue(LineAnchorCap.None)]
		[SRCategory("CategoryAttributeAppearance")]
		public virtual LineAnchorCap EndCap
		{
			get
			{
				return this.endCap;
			}
			set
			{
				this.endCap = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[Browsable(false)]
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
			}
		}

		[SRDescription("DescriptionAttributeTextColor3")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "Black")]
		public override Color TextColor
		{
			get
			{
				return base.TextColor;
			}
			set
			{
				base.TextColor = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override Font TextFont
		{
			get
			{
				return base.TextFont;
			}
			set
			{
				base.TextFont = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
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
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
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

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[Browsable(false)]
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

		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
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

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[Browsable(false)]
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

		[SRCategory("CategoryAttributePosition")]
		[SRDescription("DescriptionAttributeSizeAlwaysRelative3")]
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
			}
		}

		[SRDescription("DescriptionAttributeAnchorAlignment4")]
		[SRCategory("CategoryAttributeAnchor")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(ContentAlignment), "TopLeft")]
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
				return "Line";
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal override SelectionPointsStyle SelectionPointsStyle
		{
			get
			{
				return SelectionPointsStyle.TwoPoints;
			}
		}

		public LineAnnotation()
		{
			base.anchorAlignment = ContentAlignment.TopLeft;
		}

		internal virtual void AdjustLineCoordinates(ref PointF point1, ref PointF point2, ref RectangleF selectionRect)
		{
			if (this.DrawInfinitive)
			{
				if (Math.Round((double)point1.X, 3) == Math.Round((double)point2.X, 3))
				{
					point1.Y = (float)((point1.Y < point2.Y) ? 0.0 : 100.0);
					point2.Y = (float)((point1.Y < point2.Y) ? 100.0 : 0.0);
				}
				else if (Math.Round((double)point1.Y, 3) == Math.Round((double)point2.Y, 3))
				{
					point1.X = (float)((point1.X < point2.X) ? 0.0 : 100.0);
					point2.X = (float)((point1.X < point2.X) ? 100.0 : 0.0);
				}
				else
				{
					PointF empty = PointF.Empty;
					empty.Y = 0f;
					empty.X = (float)((0.0 - point1.Y) * (point2.X - point1.X) / (point2.Y - point1.Y) + point1.X);
					PointF empty2 = PointF.Empty;
					empty2.Y = 100f;
					empty2.X = (float)((100.0 - point1.Y) * (point2.X - point1.X) / (point2.Y - point1.Y) + point1.X);
					point1 = ((point1.Y < point2.Y) ? empty : empty2);
					point2 = ((point1.Y < point2.Y) ? empty2 : empty);
				}
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
			RectangleF rect = new RectangleF(empty, new SizeF(pointF.X - empty.X, pointF.Y - empty.Y));
			this.AdjustLineCoordinates(ref empty, ref pointF, ref rect);
			if (!float.IsNaN(empty.X) && !float.IsNaN(empty.Y) && !float.IsNaN(pointF.X) && !float.IsNaN(pointF.Y))
			{
				bool flag = false;
				LineCap lineCap = LineCap.Flat;
				LineCap lineCap2 = LineCap.Flat;
				if (this.startCap != 0 || this.endCap != 0)
				{
					flag = true;
					lineCap = graphics.pen.StartCap;
					lineCap2 = graphics.pen.EndCap;
					if (this.startCap == LineAnchorCap.Arrow)
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
					else if (this.startCap == LineAnchorCap.Diamond)
					{
						graphics.pen.StartCap = LineCap.DiamondAnchor;
					}
					else if (this.startCap == LineAnchorCap.Round)
					{
						graphics.pen.StartCap = LineCap.RoundAnchor;
					}
					else if (this.startCap == LineAnchorCap.Square)
					{
						graphics.pen.StartCap = LineCap.SquareAnchor;
					}
					if (this.endCap == LineAnchorCap.Arrow)
					{
						if (this.LineWidth < 4)
						{
							int num2 = 3 - this.LineWidth;
							graphics.pen.EndCap = LineCap.Custom;
							graphics.pen.CustomEndCap = new AdjustableArrowCap((float)(this.LineWidth + num2), (float)(this.LineWidth + num2), true);
						}
						else
						{
							graphics.pen.EndCap = LineCap.ArrowAnchor;
						}
					}
					else if (this.endCap == LineAnchorCap.Diamond)
					{
						graphics.pen.EndCap = LineCap.DiamondAnchor;
					}
					else if (this.endCap == LineAnchorCap.Round)
					{
						graphics.pen.EndCap = LineCap.RoundAnchor;
					}
					else if (this.endCap == LineAnchorCap.Square)
					{
						graphics.pen.EndCap = LineCap.SquareAnchor;
					}
				}
				if (this.Chart.chartPicture.common.ProcessModePaint)
				{
					graphics.DrawLineRel(this.LineColor, this.LineWidth, this.LineStyle, empty, pointF, this.ShadowColor, this.ShadowOffset);
				}
				if (this.Chart.chartPicture.common.ProcessModeRegions)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddLine(graphics.GetAbsolutePoint(empty), graphics.GetAbsolutePoint(pointF));
					using (Pen pen = (Pen)graphics.pen.Clone())
					{
						pen.DashStyle = DashStyle.Solid;
						pen.Width += 2f;
						ChartGraphics.Widen(graphicsPath, pen);
					}
					this.Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, graphicsPath, false, base.ReplaceKeywords(this.ToolTip), base.ReplaceKeywords(this.Href), base.ReplaceKeywords(this.MapAreaAttributes), this, ChartElementType.Annotation);
				}
				if (flag)
				{
					graphics.pen.StartCap = lineCap;
					graphics.pen.EndCap = lineCap2;
				}
				this.PaintSelectionHandles(graphics, rect, null);
			}
		}
	}
}
