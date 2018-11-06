using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(LinearScaleConverter))]
	internal sealed class LinearScale : ScaleBase, ISelectable
	{
		private float position = 50f;

		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[SRCategory("CategoryLabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeLabelStyle")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearLabelStyle LabelStyle
		{
			get
			{
				return base.baseLabelStyle;
			}
			set
			{
				base.baseLabelStyle = value;
				base.baseLabelStyle.Parent = this;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[ValidateBound(5.0, 90.0)]
		[SRDescription("DescriptionAttributeLinearScale_Position")]
		[TypeConverter(typeof(FloatAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(50f)]
		public float Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[ValidateBound(0.0, 100.0)]
		[SRDescription("DescriptionAttributeLinearScale_StartMargin")]
		[DefaultValue(8f)]
		public float StartMargin
		{
			get
			{
				return base._startPosition;
			}
			set
			{
				if (!(value > 100.0) && !(value < 0.0))
				{
					base._startPosition = Math.Min(value, base._endPosition);
					base.InvalidateSweepPosition();
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
			}
		}

		[ValidateBound(0.0, 100.0)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeLinearScale_EndMargin")]
		[DefaultValue(8f)]
		public float EndMargin
		{
			get
			{
				return (float)(100.0 - base._endPosition);
			}
			set
			{
				if (!(value > 100.0) && !(value < 0.0))
				{
					base._endPosition = Math.Max((float)(100.0 - value), base._startPosition);
					base.InvalidateSweepPosition();
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
			}
		}

		[SRDescription("DescriptionAttributeMajorTickMarkInt")]
		[SRCategory("CategoryLabelsAndTickMarks")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public LinearMajorTickMark MajorTickMark
		{
			get
			{
				return (LinearMajorTickMark)base.MajorTickMarkInt;
			}
			set
			{
				base.MajorTickMarkInt = value;
			}
		}

		[SRCategory("CategoryLabelsAndTickMarks")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMinorTickMarkInt")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public LinearMinorTickMark MinorTickMark
		{
			get
			{
				return (LinearMinorTickMark)base.MinorTickMarkInt;
			}
			set
			{
				base.MinorTickMarkInt = value;
			}
		}

		[DefaultValue(5f)]
		[ValidateBound(0.0, 30.0)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeLinearScale_Width")]
		public override float Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				base.Width = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeMinimumPin")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public new LinearSpecialPosition MinimumPin
		{
			get
			{
				return (LinearSpecialPosition)base.minimumPin;
			}
			set
			{
				base.minimumPin = value;
				base.minimumPin.Parent = this;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMaximumPin")]
		[SRCategory("CategoryLayout")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public new LinearSpecialPosition MaximumPin
		{
			get
			{
				return (LinearSpecialPosition)base.maximumPin;
			}
			set
			{
				base.maximumPin = value;
				base.maximumPin.Parent = this;
				this.Invalidate();
			}
		}

		internal LinearGauge ParentGauge
		{
			get
			{
				return this.GetGauge();
			}
		}

		public LinearScale()
		{
			base._startPosition = 8f;
			base._endPosition = 92f;
			base.coordSystemRatio = 1f;
			this.Width = 5f;
			base.InvalidateSweepPosition();
			base.MajorTickMarkInt = new LinearMajorTickMark(this);
			base.MinorTickMarkInt = new LinearMinorTickMark(this);
			base.baseLabelStyle = new LinearLabelStyle(this);
			base.maximumPin = new LinearSpecialPosition(this);
			base.minimumPin = new LinearSpecialPosition(this);
		}

		private GraphicsPath GetBarPath(float barOffsetInside, float barOffsetOutside)
		{
			GaugeGraphics graph = this.Common.Graph;
			GraphicsPath graphicsPath = new GraphicsPath();
			float num = 0f;
			if (this.MajorTickMark.Visible)
			{
				num = (float)(this.MajorTickMark.Width / 2.0);
			}
			if (this.MinorTickMark.Visible)
			{
				num = Math.Max(num, (float)(this.MinorTickMark.Width / 2.0));
			}
			RectangleF rectangleF = new RectangleF(0f, 0f, 0f, 0f);
			if (this.ParentGauge.GetOrientation() == GaugeOrientation.Horizontal)
			{
				rectangleF.X = base.StartPosition;
				rectangleF.Width = base.EndPosition - base.StartPosition;
				rectangleF.Y = this.Position - barOffsetInside;
				rectangleF.Height = barOffsetInside + barOffsetOutside;
				rectangleF = graph.GetAbsoluteRectangle(rectangleF);
				rectangleF.Inflate(graph.GetAbsoluteDimension(num), 0f);
			}
			else
			{
				rectangleF.Y = base.StartPosition;
				rectangleF.Height = base.EndPosition - base.StartPosition;
				rectangleF.X = this.Position - barOffsetInside;
				rectangleF.Width = barOffsetInside + barOffsetOutside;
				rectangleF = graph.GetAbsoluteRectangle(rectangleF);
				rectangleF.Inflate(0f, graph.GetAbsoluteDimension(num));
			}
			if (rectangleF.Width <= 0.0)
			{
				rectangleF.Width = 1E-06f;
			}
			if (rectangleF.Height <= 0.0)
			{
				rectangleF.Height = 1E-06f;
			}
			graphicsPath.AddRectangle(rectangleF);
			return graphicsPath;
		}

		private void SetScaleHitTestPath(GaugeGraphics g)
		{
			Gap gap = new Gap(this.Position);
			gap.SetOffset(Placement.Cross, this.Width);
			gap.SetBase();
			if (this.MajorTickMark.Visible)
			{
				gap.SetOffsetBase(this.MajorTickMark.Placement, this.MajorTickMark.Length);
				if (this.MajorTickMark.Placement == Placement.Outside)
				{
					gap.Outside = Math.Max(gap.Outside, (float)(this.MajorTickMark.DistanceFromScale + this.MajorTickMark.Length + this.Width / 2.0));
					gap.Inside = Math.Max(gap.Inside, (float)(0.0 - this.MajorTickMark.DistanceFromScale));
				}
				else if (this.MajorTickMark.Placement == Placement.Cross)
				{
					gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.MajorTickMark.DistanceFromScale + this.MajorTickMark.Length / 2.0));
					gap.Inside = Math.Max(gap.Inside, (float)(this.MajorTickMark.DistanceFromScale + this.MajorTickMark.Length / 2.0));
				}
				else
				{
					gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.MajorTickMark.DistanceFromScale));
					gap.Inside = Math.Max(gap.Inside, (float)(this.MajorTickMark.DistanceFromScale + this.MajorTickMark.Length + this.Width / 2.0));
				}
			}
			if (this.MinorTickMark.Visible)
			{
				gap.SetOffsetBase(this.MinorTickMark.Placement, this.MinorTickMark.Length);
				if (this.MinorTickMark.Placement == Placement.Outside)
				{
					gap.Outside = Math.Max(gap.Outside, (float)(this.MinorTickMark.DistanceFromScale + this.MinorTickMark.Length + this.Width / 2.0));
					gap.Inside = Math.Max(gap.Inside, (float)(0.0 - this.MinorTickMark.DistanceFromScale));
				}
				else if (this.MinorTickMark.Placement == Placement.Cross)
				{
					gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.MinorTickMark.DistanceFromScale + this.MinorTickMark.Length / 2.0));
					gap.Inside = Math.Max(gap.Inside, (float)(this.MinorTickMark.DistanceFromScale + this.MinorTickMark.Length / 2.0));
				}
				else
				{
					gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.MinorTickMark.DistanceFromScale));
					gap.Inside = Math.Max(gap.Inside, (float)(this.MinorTickMark.DistanceFromScale + this.MinorTickMark.Length + this.Width / 2.0));
				}
			}
			if (this.LabelStyle.Visible)
			{
				if (this.LabelStyle.Placement == Placement.Outside)
				{
					gap.Outside = Math.Max(gap.Outside, (float)(this.LabelStyle.DistanceFromScale + (float)this.LabelStyle.Font.Height + this.Width / 2.0));
					gap.Inside = Math.Max(gap.Inside, (float)(0.0 - this.LabelStyle.DistanceFromScale));
				}
				else if (this.LabelStyle.Placement == Placement.Cross)
				{
					gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.LabelStyle.DistanceFromScale + (float)this.LabelStyle.Font.Height / 2.0));
					gap.Inside = Math.Max(gap.Inside, (float)(this.LabelStyle.DistanceFromScale + (float)this.LabelStyle.Font.Height / 2.0));
				}
				else
				{
					gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.LabelStyle.DistanceFromScale));
					gap.Inside = Math.Max(gap.Inside, (float)(this.LabelStyle.DistanceFromScale + (float)this.LabelStyle.Font.Height + this.Width / 2.0));
				}
			}
			GraphicsPath barPath = this.GetBarPath(gap.Inside, gap.Outside);
			this.Common.GaugeCore.HotRegionList.SetHotRegion(this, PointF.Empty, barPath);
		}

		internal GraphicsPath GetShadowPath()
		{
			if (base.Visible && base.ShadowOffset != 0.0 && this.Width > 0.0)
			{
				GraphicsPath barPath = this.GetBarPath((float)(this.Width / 2.0), (float)(this.Width / 2.0));
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate(base.ShadowOffset, base.ShadowOffset);
					barPath.Transform(matrix);
					return barPath;
				}
			}
			return null;
		}

		private void RenderBar(GaugeGraphics g)
		{
			using (GraphicsPath path = this.GetBarPath((float)(this.Width / 2.0), (float)(this.Width / 2.0)))
			{
				g.DrawPathAbs(path, base.FillColor, base.FillHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.Center, base.FillGradientType, base.FillGradientEndColor, base.BorderColor, base.BorderWidth, base.BorderStyle, PenAlignment.Outset);
			}
		}

		internal override void DrawTickMark(GaugeGraphics g, CustomTickMark tickMark, double value, float offset)
		{
			float num = this.GetPositionFromValue(value);
			PointF absolutePoint = g.GetAbsolutePoint(this.GetPoint(num, offset));
			using (Matrix matrix = new Matrix())
			{
				if (this.ParentGauge.GetOrientation() == GaugeOrientation.Vertical)
				{
					matrix.RotateAt(90f, absolutePoint);
				}
				if (tickMark.Placement == Placement.Outside)
				{
					matrix.RotateAt(180f, absolutePoint);
				}
				base.DrawTickMark(g, tickMark, value, offset, matrix);
			}
		}

		internal override LinearLabelStyle GetLabelStyle()
		{
			return this.LabelStyle;
		}

		private void DrawLabel(Placement placement, string labelStr, double value, float labelPos, float rotateLabelAngle, Font font, Color color, FontUnit fontUnit)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Near;
			float num = this.GetPositionFromValue(value);
			MarkerPosition markerPosition = new MarkerPosition((float)Math.Round((double)num), value, placement);
			if (!MarkerPosition.IsExistsInArray(base.labels, markerPosition))
			{
				if (labelStr.Length > 0)
				{
					base.labels.Add(markerPosition);
				}
				GaugeGraphics graph = this.Common.Graph;
				using (Brush brush2 = new SolidBrush(color))
				{
					Font resizedFont = base.GetResizedFont(font, fontUnit);
					try
					{
						float num2 = 0f;
						if (this.ParentGauge.GetOrientation() == GaugeOrientation.Vertical)
						{
							num2 = 90f;
						}
						SizeF size = graph.MeasureString(labelStr, resizedFont);
						float contactPointOffset = Utils.GetContactPointOffset(size, rotateLabelAngle - num2);
						PointF absolutePoint = graph.GetAbsolutePoint(this.GetPoint(num, labelPos));
						switch (placement)
						{
						case Placement.Inside:
							if (this.ParentGauge.GetOrientation() == GaugeOrientation.Vertical)
							{
								absolutePoint.X -= contactPointOffset;
							}
							else
							{
								absolutePoint.Y -= contactPointOffset;
							}
							break;
						case Placement.Outside:
							if (this.ParentGauge.GetOrientation() == GaugeOrientation.Vertical)
							{
								absolutePoint.X += contactPointOffset;
							}
							else
							{
								absolutePoint.Y += contactPointOffset;
							}
							break;
						}
						RectangleF rectangleF = new RectangleF(absolutePoint, new SizeF(0f, 0f));
						rectangleF.Inflate((float)(size.Width / 2.0), (float)(size.Height / 2.0));
						Matrix transform = graph.Transform;
						Matrix matrix = graph.Transform.Clone();
						try
						{
							if (rotateLabelAngle == 0.0)
							{
								if (base.ShadowOffset != 0.0)
								{
									using (Brush brush = graph.GetShadowBrush())
									{
										RectangleF layoutRectangle = rectangleF;
										layoutRectangle.Offset(base.ShadowOffset, base.ShadowOffset);
										graph.DrawString(labelStr, resizedFont, brush, layoutRectangle, stringFormat);
									}
								}
								graph.DrawString(labelStr, resizedFont, brush2, rectangleF, stringFormat);
							}
							else
							{
								TextRenderingHint textRenderingHint = graph.TextRenderingHint;
								try
								{
									if (textRenderingHint == TextRenderingHint.ClearTypeGridFit)
									{
										graph.TextRenderingHint = TextRenderingHint.AntiAlias;
									}
									if (base.ShadowOffset != 0.0)
									{
										using (Brush brush3 = graph.GetShadowBrush())
										{
											using (Matrix matrix2 = matrix.Clone())
											{
												matrix2.Translate(base.ShadowOffset, base.ShadowOffset);
												matrix2.RotateAt(rotateLabelAngle, absolutePoint);
												graph.Transform = matrix2;
												graph.DrawString(labelStr, resizedFont, brush3, rectangleF, stringFormat);
											}
										}
									}
									matrix.RotateAt(rotateLabelAngle, absolutePoint);
									graph.Transform = matrix;
									graph.DrawString(labelStr, resizedFont, brush2, rectangleF, stringFormat);
								}
								finally
								{
									graph.TextRenderingHint = textRenderingHint;
								}
							}
						}
						finally
						{
							matrix.Dispose();
							graph.Transform = transform;
						}
					}
					finally
					{
						if (resizedFont != font)
						{
							resizedFont.Dispose();
						}
					}
				}
			}
		}

		private void RenderLabels(GaugeGraphics g)
		{
			if (this.LabelStyle.Visible)
			{
				double interval = base.GetInterval(IntervalTypes.Labels);
				float offsetLabelPos = base.GetOffsetLabelPos(this.LabelStyle.Placement, this.LabelStyle.DistanceFromScale, this.Position);
				double minimumLog = base.MinimumLog;
				double intervalOffset = base.GetIntervalOffset(IntervalTypes.Labels);
				Color textColor = this.LabelStyle.TextColor;
				CustomTickMark endLabelTickMark = base.GetEndLabelTickMark();
				if (this.LabelStyle.ShowEndLabels && intervalOffset > 0.0)
				{
					textColor = base.GetRangeLabelsColor(minimumLog, this.LabelStyle.TextColor);
					string labelStr = (this.Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, this.LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : this.Common.GaugeContainer.FormatNumberHandler(this.Common.GaugeContainer, minimumLog * base.Multiplier, this.LabelStyle.FormatString);
					this.DrawLabel(this.LabelStyle.Placement, labelStr, minimumLog, offsetLabelPos, this.LabelStyle.FontAngle, this.LabelStyle.Font, textColor, this.LabelStyle.FontUnit);
					if (endLabelTickMark != null)
					{
						this.DrawTickMark(g, endLabelTickMark, minimumLog, base.GetTickMarkOffset(endLabelTickMark));
					}
				}
				minimumLog += intervalOffset;
				double num = 0.0;
				while (minimumLog <= base.Maximum)
				{
					bool flag = true;
					foreach (CustomLabel customLabel in base.CustomLabels)
					{
						if (Math.Abs(customLabel.Value - minimumLog) < 1E-07 && customLabel.Placement == this.LabelStyle.Placement && Math.Abs(customLabel.DistanceFromScale - this.LabelStyle.DistanceFromScale) < 1.0)
						{
							flag = false;
						}
					}
					if (!this.LabelStyle.ShowEndLabels && (minimumLog == base.MinimumLog || minimumLog == base.Maximum))
					{
						flag = false;
					}
					if (flag)
					{
						textColor = base.GetRangeLabelsColor(minimumLog, this.LabelStyle.TextColor);
						string labelStr2 = (this.Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, this.LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : this.Common.GaugeContainer.FormatNumberHandler(this.Common.GaugeContainer, minimumLog * base.Multiplier, this.LabelStyle.FormatString);
						this.DrawLabel(this.LabelStyle.Placement, labelStr2, minimumLog, offsetLabelPos, this.LabelStyle.FontAngle, this.LabelStyle.Font, textColor, this.LabelStyle.FontUnit);
					}
					num = minimumLog;
					minimumLog = base.GetNextPosition(minimumLog, interval, false);
				}
				if (this.LabelStyle.ShowEndLabels && num < base.Maximum)
				{
					minimumLog = base.Maximum;
					textColor = base.GetRangeLabelsColor(minimumLog, this.LabelStyle.TextColor);
					string labelStr3 = (this.Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, this.LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : this.Common.GaugeContainer.FormatNumberHandler(this.Common.GaugeContainer, minimumLog * base.Multiplier, this.LabelStyle.FormatString);
					this.DrawLabel(this.LabelStyle.Placement, labelStr3, minimumLog, offsetLabelPos, this.LabelStyle.FontAngle, this.LabelStyle.Font, textColor, this.LabelStyle.FontUnit);
					if (endLabelTickMark != null)
					{
						this.DrawTickMark(g, endLabelTickMark, minimumLog, base.GetTickMarkOffset(endLabelTickMark));
					}
				}
			}
		}

		internal override void DrawCustomLabel(CustomLabel label)
		{
			if (base.staticRendering)
			{
				float offsetLabelPos = base.GetOffsetLabelPos(label.Placement, label.DistanceFromScale, this.Position);
				string text = label.Text;
				this.DrawLabel(label.Placement, text, label.Value, offsetLabelPos, label.FontAngle, label.Font, label.TextColor, label.FontUnit);
			}
			if ((!label.TickMarkStyle.Visible || base.TickMarksOnTop) && base.staticRendering)
			{
				return;
			}
			this.DrawTickMark(this.Common.Graph, label.TickMarkStyle, label.Value, base.GetTickMarkOffset(label.TickMarkStyle));
		}

		internal override void DrawSpecialPosition(GaugeGraphics g, SpecialPosition label, float angle)
		{
			if (label.Enable)
			{
				LinearPinLabel labelStyle = ((LinearSpecialPosition)label).LabelStyle;
				if (labelStyle.Text != string.Empty && base.staticRendering)
				{
					this.DrawLabel(labelStyle.Placement, labelStyle.Text, this.GetValueFromPosition(angle), base.GetOffsetLabelPos(labelStyle.Placement, labelStyle.DistanceFromScale, this.Position), labelStyle.FontAngle, labelStyle.Font, labelStyle.TextColor, labelStyle.FontUnit);
				}
				if ((!label.Visible || base.TickMarksOnTop) && base.staticRendering)
				{
					return;
				}
				float tickMarkOffset = base.GetTickMarkOffset(label);
				this.DrawTickMark(g, label, this.GetValueFromPosition(angle), tickMarkOffset);
			}
		}

		internal void RenderStaticElements(GaugeGraphics g)
		{
			if (base.Visible)
			{
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRendering(this.Name));
				g.StartHotRegion(this);
				GraphicsState gstate = g.Save();
				try
				{
					base.staticRendering = true;
					if (!base.TickMarksOnTop)
					{
						base.markers.Clear();
					}
					base.labels.Clear();
					this.RenderBar(g);
					base.RenderCustomLabels(g);
					if (!base.TickMarksOnTop)
					{
						base.RenderGrid(g);
					}
					this.RenderLabels(g);
					base.RenderPins(g);
					this.SetScaleHitTestPath(g);
					if (!base.TickMarksOnTop)
					{
						base.markers.Sort();
					}
				}
				finally
				{
					g.Restore(gstate);
					g.EndHotRegion();
				}
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(this.Name));
			}
		}

		internal void RenderDynamicElements(GaugeGraphics g)
		{
			if (base.Visible && base.TickMarksOnTop)
			{
				GraphicsState gstate = g.Save();
				try
				{
					base.staticRendering = false;
					base.markers.Clear();
					base.RenderCustomLabels(g);
					base.RenderGrid(g);
					base.RenderPins(g);
					base.markers.Sort();
				}
				finally
				{
					g.Restore(gstate);
				}
			}
		}

		public override string ToString()
		{
			return this.Name;
		}

		public new LinearGauge GetGauge()
		{
			if (this.Collection != null)
			{
				return (LinearGauge)this.Collection.parent;
			}
			return null;
		}

		protected override bool IsReversed()
		{
			if (this.ParentElement != null && this.ParentGauge.GetOrientation() == GaugeOrientation.Vertical)
			{
				return !base.IsReversed();
			}
			return base.IsReversed();
		}

		protected override PointF GetPoint(float position, float offset)
		{
			PointF empty = PointF.Empty;
			if (this.ParentGauge.GetOrientation() == GaugeOrientation.Horizontal)
			{
				empty.X = position;
				empty.Y = this.Position + offset;
			}
			else
			{
				empty.Y = position;
				empty.X = this.Position + offset;
			}
			return empty;
		}

		internal override double GetValue(PointF c, PointF p)
		{
			if (this.Common != null)
			{
				HotRegionList hotRegionList = this.Common.GaugeCore.HotRegionList;
				int num = hotRegionList.LocateObject(this.GetGauge());
				if (num != -1)
				{
					HotRegion hotRegion = (HotRegion)hotRegionList.List[num];
					RectangleF boundingRectangle = hotRegion.BoundingRectangle;
					float num2 = (float)((this.ParentGauge.GetOrientation() != 0) ? ((p.Y - boundingRectangle.Y) / boundingRectangle.Height * 100.0) : ((p.X - boundingRectangle.X) / boundingRectangle.Width * 100.0));
					return this.GetValueFromPosition(num2);
				}
			}
			return double.NaN;
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			if (this.GetGauge() != null)
			{
				Stack stack = new Stack();
				for (NamedElement namedElement = this.GetGauge().ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
				{
					stack.Push(namedElement);
				}
				foreach (IRenderable item in stack)
				{
					g.CreateDrawRegion(item.GetBoundRect(g));
				}
				g.CreateDrawRegion(((IRenderable)this.GetGauge()).GetBoundRect(g));
				Gap gap = new Gap(this.Position);
				gap.SetOffset(Placement.Cross, this.Width);
				gap.SetBase();
				if (this.MajorTickMark.Visible)
				{
					gap.SetOffsetBase(this.MajorTickMark.Placement, this.MajorTickMark.Length);
					if (this.MajorTickMark.Placement == Placement.Outside)
					{
						gap.Outside = Math.Max(gap.Outside, (float)(this.MajorTickMark.DistanceFromScale + this.MajorTickMark.Length + this.Width / 2.0));
						gap.Inside = Math.Max(gap.Inside, (float)(0.0 - this.MajorTickMark.DistanceFromScale));
					}
					else if (this.MajorTickMark.Placement == Placement.Cross)
					{
						gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.MajorTickMark.DistanceFromScale + this.MajorTickMark.Length / 2.0));
						gap.Inside = Math.Max(gap.Inside, (float)(this.MajorTickMark.DistanceFromScale + this.MajorTickMark.Length / 2.0));
					}
					else
					{
						gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.MajorTickMark.DistanceFromScale));
						gap.Inside = Math.Max(gap.Inside, (float)(this.MajorTickMark.DistanceFromScale + this.MajorTickMark.Length + this.Width / 2.0));
					}
				}
				if (this.MinorTickMark.Visible)
				{
					gap.SetOffsetBase(this.MinorTickMark.Placement, this.MinorTickMark.Length);
					if (this.MinorTickMark.Placement == Placement.Outside)
					{
						gap.Outside = Math.Max(gap.Outside, (float)(this.MinorTickMark.DistanceFromScale + this.MinorTickMark.Length + this.Width / 2.0));
						gap.Inside = Math.Max(gap.Inside, (float)(0.0 - this.MinorTickMark.DistanceFromScale));
					}
					else if (this.MinorTickMark.Placement == Placement.Cross)
					{
						gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.MinorTickMark.DistanceFromScale + this.MinorTickMark.Length / 2.0));
						gap.Inside = Math.Max(gap.Inside, (float)(this.MinorTickMark.DistanceFromScale + this.MinorTickMark.Length / 2.0));
					}
					else
					{
						gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.MinorTickMark.DistanceFromScale));
						gap.Inside = Math.Max(gap.Inside, (float)(this.MinorTickMark.DistanceFromScale + this.MinorTickMark.Length + this.Width / 2.0));
					}
				}
				if (this.LabelStyle.Visible)
				{
					if (this.LabelStyle.Placement == Placement.Outside)
					{
						gap.Outside = Math.Max(gap.Outside, (float)(this.LabelStyle.DistanceFromScale + (float)this.LabelStyle.Font.Height + this.Width / 2.0));
						gap.Inside = Math.Max(gap.Inside, (float)(0.0 - this.LabelStyle.DistanceFromScale));
					}
					else if (this.LabelStyle.Placement == Placement.Cross)
					{
						gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.LabelStyle.DistanceFromScale + (float)this.LabelStyle.Font.Height / 2.0));
						gap.Inside = Math.Max(gap.Inside, (float)(this.LabelStyle.DistanceFromScale + (float)this.LabelStyle.Font.Height / 2.0));
					}
					else
					{
						gap.Outside = Math.Max(gap.Outside, (float)(0.0 - this.LabelStyle.DistanceFromScale));
						gap.Inside = Math.Max(gap.Inside, (float)(this.LabelStyle.DistanceFromScale + (float)this.LabelStyle.Font.Height + this.Width / 2.0));
					}
				}
				using (GraphicsPath graphicsPath = this.GetBarPath(gap.Inside, gap.Outside))
				{
					if (graphicsPath != null)
					{
						RectangleF bounds = graphicsPath.GetBounds();
						g.DrawSelection(bounds, designTimeSelection, this.Common.GaugeCore.SelectionBorderColor, this.Common.GaugeCore.SelectionMarkerColor);
					}
				}
				g.RestoreDrawRegion();
				foreach (IRenderable item2 in stack)
				{
					IRenderable renderable = item2;
					g.RestoreDrawRegion();
				}
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			LinearScale linearScale = new LinearScale();
			binaryFormatSerializer.Deserialize(linearScale, stream);
			return linearScale;
		}
	}
}
