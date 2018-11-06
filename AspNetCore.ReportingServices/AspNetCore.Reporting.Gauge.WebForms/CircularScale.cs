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
	[TypeConverter(typeof(CircularScaleConverter))]
	internal sealed class CircularScale : ScaleBase, ISelectable
	{
		private float radius = 37f;

		[SRCategory("CategoryLabelsAndTickMarks")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeLabelStyle")]
		public CircularLabelStyle LabelStyle
		{
			get
			{
				return (CircularLabelStyle)base.baseLabelStyle;
			}
			set
			{
				base.baseLabelStyle = value;
				base.baseLabelStyle.Parent = this;
				this.Invalidate();
			}
		}

		[TypeConverter(typeof(FloatAutoValueConverter))]
		[SRDescription("DescriptionAttributeCircularScale_Radius")]
		[NotifyParentProperty(true)]
		[DefaultValue(37f)]
		[ValidateBound(5.0, 90.0)]
		[SRCategory("CategoryLayout")]
		public float Radius
		{
			get
			{
				return this.radius;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange_min_open", 0));
				}
				this.radius = value;
				this.Invalidate();
			}
		}

		[DefaultValue(20f)]
		[SRDescription("DescriptionAttributeCircularScale_StartAngle")]
		[ValidateBound(0.0, 360.0)]
		[SRCategory("CategoryLayout")]
		public float StartAngle
		{
			get
			{
				return base.StartPosition;
			}
			set
			{
				if (!(value > 360.0) && !(value < 0.0))
				{
					base._startPosition = value;
					base.InvalidateEndPosition();
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 360));
			}
		}

		[SRDescription("DescriptionAttributeCircularScale_SweepAngle")]
		[DefaultValue(320f)]
		[ValidateBound(0.0, 360.0)]
		[SRCategory("CategoryLayout")]
		public float SweepAngle
		{
			get
			{
				return base.SweepPosition;
			}
			set
			{
				if (!(value > 360.0) && !(value < 0.0))
				{
					base._sweepPosition = value;
					base.InvalidateEndPosition();
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 360));
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryLabelsAndTickMarks")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[SRDescription("DescriptionAttributeCircularScale_MajorTickMark")]
		public CircularMajorTickMark MajorTickMark
		{
			get
			{
				return (CircularMajorTickMark)base.MajorTickMarkInt;
			}
			set
			{
				base.MajorTickMarkInt = value;
			}
		}

		[SRDescription("DescriptionAttributeCircularScale_MinorTickMark")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryLabelsAndTickMarks")]
		public CircularMinorTickMark MinorTickMark
		{
			get
			{
				return (CircularMinorTickMark)base.MinorTickMarkInt;
			}
			set
			{
				base.MinorTickMarkInt = value;
			}
		}

		[SRCategory("CategoryLayout")]
		[TypeConverter(typeof(LocationConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DefaultValue(typeof(GaugeLocation), "50F, 50F")]
		[ValidateBound(100.0, 100.0)]
		[SRDescription("DescriptionAttributeCircularScale_GaugePivotPoint")]
		public GaugeLocation GaugePivotPoint
		{
			get
			{
				CircularGauge gauge = this.GetGauge();
				if (gauge != null)
				{
					return gauge.PivotPoint;
				}
				return null;
			}
			set
			{
				CircularGauge gauge = this.GetGauge();
				if (gauge != null)
				{
					gauge.PivotPoint = value;
				}
			}
		}

		[SRDescription("DescriptionAttributeMinimumPin")]
		[SRCategory("CategoryLayout")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CircularSpecialPosition MinimumPin
		{
			get
			{
				return (CircularSpecialPosition)base.minimumPin;
			}
			set
			{
				base.minimumPin = value;
				base.minimumPin.Parent = this;
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMaximumPin")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[SRCategory("CategoryLayout")]
		public new CircularSpecialPosition MaximumPin
		{
			get
			{
				return (CircularSpecialPosition)base.maximumPin;
			}
			set
			{
				base.maximumPin = value;
				base.maximumPin.Parent = this;
				this.Invalidate();
			}
		}

		public CircularScale()
		{
			base._startPosition = 20f;
			base._sweepPosition = 320f;
			base.InvalidateEndPosition();
			base.MajorTickMarkInt = new CircularMajorTickMark(this);
			base.MinorTickMarkInt = new CircularMinorTickMark(this);
			base.baseLabelStyle = new CircularLabelStyle(this);
			base.maximumPin = new CircularSpecialPosition(this);
			base.minimumPin = new CircularSpecialPosition(this);
		}

		internal float GetRadius()
		{
			return 100f;
		}

		internal PointF GetPivotPoint()
		{
			return new PointF(50f, 50f);
		}

		private GraphicsPath GetBarPath(float barOffsetInside, float barOffsetOutside, float angularMargin)
		{
			GaugeGraphics graph = this.Common.Graph;
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF location = new PointF(this.GetPivotPoint().X - this.GetRadius(), this.GetPivotPoint().Y - this.GetRadius());
			RectangleF relative = new RectangleF(location, new SizeF((float)(this.GetRadius() * 2.0), (float)(this.GetRadius() * 2.0)));
			float num = 0f;
			if (this.MajorTickMark.Visible)
			{
				num = (float)Math.Atan(this.MajorTickMark.Width / 2.0 / this.GetRadius());
			}
			if (this.MinorTickMark.Visible)
			{
				num = (float)Math.Max((double)num, Math.Atan(this.MinorTickMark.Width / 2.0 / this.GetRadius()));
			}
			num = Utils.Rad2Deg(num);
			num += angularMargin;
			float startAngle = Utils.ToGDIAngle(base.StartPosition - num);
			float startAngle2 = Utils.ToGDIAngle(base.EndPosition + num);
			float num2 = (float)(base.EndPosition - base.StartPosition + num * 2.0);
			graphicsPath.StartFigure();
			relative.Inflate(barOffsetOutside, barOffsetOutside);
			graphicsPath.AddArc(graph.GetAbsoluteRectangle(relative), startAngle, num2);
			relative.Inflate((float)(0.0 - (barOffsetInside + barOffsetOutside)), (float)(0.0 - (barOffsetInside + barOffsetOutside)));
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(relative);
			if (absoluteRectangle.Width > 0.0 && absoluteRectangle.Height > 0.0)
			{
				graphicsPath.AddArc(absoluteRectangle, startAngle2, (float)(0.0 - num2));
			}
			graphicsPath.CloseAllFigures();
			return graphicsPath;
		}

		private void SetScaleHitTestPath(GaugeGraphics g)
		{
			Gap gap = new Gap(this.GetRadius());
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
			GraphicsPath barPath = this.GetBarPath(gap.Inside, gap.Outside, 0f);
			this.Common.GaugeCore.HotRegionList.SetHotRegion(this, g.GetAbsolutePoint(this.GetPivotPoint()), barPath);
		}

		internal GraphicsPath GetCompoundPath(GaugeGraphics g)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF location = new PointF(this.GetPivotPoint().X - this.GetRadius(), this.GetPivotPoint().Y - this.GetRadius());
			RectangleF relative = new RectangleF(location, new SizeF((float)(this.GetRadius() * 2.0), (float)(this.GetRadius() * 2.0)));
			float num = 0f;
			if (this.MajorTickMark.Visible)
			{
				num = (float)Math.Atan(this.MajorTickMark.Width / 2.0 / this.GetRadius());
			}
			if (this.MinorTickMark.Visible)
			{
				num = (float)Math.Max((double)num, Math.Atan(this.MinorTickMark.Width / 2.0 / this.GetRadius()));
			}
			num = Utils.Rad2Deg(num);
			float startAngle = Utils.ToGDIAngle(base.StartPosition - num);
			float startAngle2 = Utils.ToGDIAngle(base.EndPosition + num);
			float num2 = (float)(base.EndPosition - base.StartPosition + num * 2.0);
			graphicsPath.StartFigure();
			graphicsPath.AddArc(g.GetAbsoluteRectangle(relative), startAngle, num2);
			relative.Inflate((float)((0.0 - relative.Width) / 2.0), (float)((0.0 - relative.Height) / 2.0));
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(relative);
			absoluteRectangle.Inflate(15f, 15f);
			graphicsPath.AddArc(absoluteRectangle, startAngle2, (float)(360.0 - num2));
			graphicsPath.CloseAllFigures();
			return graphicsPath;
		}

		internal GraphicsPath GetShadowPath(GaugeGraphics g)
		{
			if (base.Visible && base.ShadowOffset != 0.0 && this.Width > 0.0)
			{
				this.SetDrawRegion(g);
				GraphicsPath graphicsPath = new GraphicsPath();
				using (GraphicsPath addingPath = this.GetBarPath((float)(this.Width / 2.0), (float)(this.Width / 2.0), 0f))
				{
					graphicsPath.AddPath(addingPath, false);
				}
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate(base.ShadowOffset, base.ShadowOffset);
					graphicsPath.Transform(matrix);
				}
				PointF pointF = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
				g.RestoreDrawRegion();
				PointF pointF2 = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
				using (Matrix matrix2 = new Matrix())
				{
					matrix2.Translate(pointF.X - pointF2.X, pointF.Y - pointF2.Y);
					graphicsPath.Transform(matrix2);
					return graphicsPath;
				}
			}
			return null;
		}

		private void RenderBar(GaugeGraphics g)
		{
			if (this.Width > 0.0)
			{
				using (GraphicsPath path = this.GetBarPath((float)(this.Width / 2.0), (float)(this.Width / 2.0), 0f))
				{
					g.DrawPathAbs(path, base.FillColor, base.FillHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.Center, base.FillGradientType, base.FillGradientEndColor, base.BorderColor, base.BorderWidth, base.BorderStyle, PenAlignment.Outset);
				}
			}
		}

		internal override void DrawTickMark(GaugeGraphics g, CustomTickMark tickMark, double value, float offset)
		{
			float num = this.GetPositionFromValueNormalized(value);
			PointF absolutePoint = g.GetAbsolutePoint(this.GetPoint(num, offset));
			using (Matrix matrix = new Matrix())
			{
				matrix.RotateAt(num, absolutePoint);
				if (tickMark.Placement == Placement.Inside)
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

		private void DrawLabel(Placement placement, string labelStr, double position, float labelPos, float rotateLabelAngle, Font font, Color color, bool rotateLabels, bool allowUpsideDown, FontUnit fontUnit)
		{
			float num = this.GetPositionFromValueNormalized(position);
			if (rotateLabels)
			{
				rotateLabelAngle = (float)(rotateLabelAngle + (num + 180.0));
				rotateLabelAngle = (float)(rotateLabelAngle % 360.0);
				if (!allowUpsideDown && rotateLabelAngle > 90.0 && rotateLabelAngle < 270.0)
				{
					rotateLabelAngle = (float)(rotateLabelAngle + 180.0);
					rotateLabelAngle = (float)(rotateLabelAngle % 360.0);
				}
			}
			MarkerPosition markerPosition = new MarkerPosition((float)Math.Round((double)num), position, placement);
			if (!MarkerPosition.IsExistsInArray(base.labels, markerPosition))
			{
				if (labelStr.Length > 0)
				{
					base.labels.Add(markerPosition);
				}
				GaugeGraphics graph = this.Common.Graph;
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				using (Brush brush2 = new SolidBrush(color))
				{
					Font resizedFont = base.GetResizedFont(font, fontUnit);
					try
					{
						SizeF sizeF = graph.GetRelativeSize(graph.MeasureString(labelStr, resizedFont));
						sizeF.Height -= (float)(sizeF.Height / 8.0);
						float contactPointOffset = Utils.GetContactPointOffset(sizeF, num - rotateLabelAngle);
						float offset = labelPos;
						switch (placement)
						{
						case Placement.Inside:
							offset = labelPos - Math.Max(0f, contactPointOffset);
							break;
						case Placement.Outside:
							offset = labelPos + Math.Max(0f, contactPointOffset);
							break;
						}
						PointF absolutePoint = graph.GetAbsolutePoint(this.GetPoint(num, offset));
						sizeF = graph.GetAbsoluteSize(sizeF);
						RectangleF layoutRectangle = new RectangleF(absolutePoint, new SizeF(0f, 0f));
						layoutRectangle.Inflate((float)(sizeF.Width / 2.0), (float)(sizeF.Height / 2.0));
						Matrix transform = graph.Transform;
						Matrix matrix = graph.Transform.Clone();
						try
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
									using (Brush brush = graph.GetShadowBrush())
									{
										using (Matrix matrix2 = matrix.Clone())
										{
											matrix2.Translate(base.ShadowOffset, base.ShadowOffset);
											matrix2.RotateAt(rotateLabelAngle, absolutePoint);
											graph.Transform = matrix2;
											graph.DrawString(labelStr, resizedFont, brush, layoutRectangle, stringFormat);
										}
									}
								}
								matrix.RotateAt(rotateLabelAngle, absolutePoint);
								graph.Transform = matrix;
								graph.DrawString(labelStr, resizedFont, brush2, layoutRectangle, stringFormat);
							}
							finally
							{
								graph.TextRenderingHint = textRenderingHint;
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
				float offsetLabelPos = base.GetOffsetLabelPos(this.LabelStyle.Placement, this.LabelStyle.DistanceFromScale, this.GetRadius());
				double minimumLog = base.MinimumLog;
				double intervalOffset = base.GetIntervalOffset(IntervalTypes.Labels);
				Color textColor = this.LabelStyle.TextColor;
				CustomTickMark endLabelTickMark = base.GetEndLabelTickMark();
				if (this.LabelStyle.ShowEndLabels && intervalOffset > 0.0)
				{
					textColor = base.GetRangeLabelsColor(minimumLog, this.LabelStyle.TextColor);
					string labelStr = (this.Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, this.LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : this.Common.GaugeContainer.FormatNumberHandler(this.Common.GaugeContainer, minimumLog * base.Multiplier, this.LabelStyle.FormatString);
					this.DrawLabel(this.LabelStyle.Placement, labelStr, minimumLog, offsetLabelPos, this.LabelStyle.FontAngle, this.LabelStyle.Font, textColor, this.LabelStyle.RotateLabels, this.LabelStyle.AllowUpsideDown, this.LabelStyle.FontUnit);
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
					if (base.SweepPosition > 359.0 && minimumLog == base.MinimumLog)
					{
						flag = false;
					}
					if (flag)
					{
						textColor = base.GetRangeLabelsColor(minimumLog, this.LabelStyle.TextColor);
						string labelStr2 = (this.Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, this.LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : this.Common.GaugeContainer.FormatNumberHandler(this.Common.GaugeContainer, minimumLog * base.Multiplier, this.LabelStyle.FormatString);
						this.DrawLabel(this.LabelStyle.Placement, labelStr2, minimumLog, offsetLabelPos, this.LabelStyle.FontAngle, this.LabelStyle.Font, textColor, this.LabelStyle.RotateLabels, this.LabelStyle.AllowUpsideDown, this.LabelStyle.FontUnit);
					}
					num = minimumLog;
					minimumLog = base.GetNextPosition(minimumLog, interval, false);
				}
				if (this.LabelStyle.ShowEndLabels && num < base.Maximum)
				{
					minimumLog = base.Maximum;
					textColor = base.GetRangeLabelsColor(minimumLog, this.LabelStyle.TextColor);
					string labelStr3 = (this.Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, this.LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : this.Common.GaugeContainer.FormatNumberHandler(this.Common.GaugeContainer, minimumLog * base.Multiplier, this.LabelStyle.FormatString);
					this.DrawLabel(this.LabelStyle.Placement, labelStr3, minimumLog, offsetLabelPos, this.LabelStyle.FontAngle, this.LabelStyle.Font, textColor, this.LabelStyle.RotateLabels, this.LabelStyle.AllowUpsideDown, this.LabelStyle.FontUnit);
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
				float offsetLabelPos = base.GetOffsetLabelPos(label.Placement, label.DistanceFromScale, this.GetRadius());
				string text = label.Text;
				this.DrawLabel(label.Placement, text, label.Value, offsetLabelPos, label.FontAngle, label.Font, label.TextColor, label.RotateLabel, label.AllowUpsideDown, label.FontUnit);
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
				CircularPinLabel labelStyle = ((CircularSpecialPosition)label).LabelStyle;
				if (labelStyle.Text != string.Empty && base.staticRendering)
				{
					this.DrawLabel(labelStyle.Placement, labelStyle.Text, base.GetValueFromPosition(angle), base.GetOffsetLabelPos(labelStyle.Placement, labelStyle.DistanceFromScale, this.GetRadius()), labelStyle.FontAngle, labelStyle.Font, labelStyle.TextColor, labelStyle.RotateLabel, labelStyle.AllowUpsideDown, labelStyle.FontUnit);
				}
				if ((!label.Visible || base.TickMarksOnTop) && base.staticRendering)
				{
					return;
				}
				float tickMarkOffset = base.GetTickMarkOffset(label);
				this.DrawTickMark(g, label, base.GetValueFromPosition(angle), tickMarkOffset);
			}
		}

		internal void RenderStaticElements(GaugeGraphics g)
		{
			if (base.Visible)
			{
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRendering(this.Name));
				g.StartHotRegion(this);
				GraphicsState gstate = g.Save();
				this.SetDrawRegion(g);
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
					g.RestoreDrawRegion();
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
				this.SetDrawRegion(g);
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
					g.RestoreDrawRegion();
					g.Restore(gstate);
				}
			}
		}

		internal void SetDrawRegion(GaugeGraphics g)
		{
			RectangleF rect = new RectangleF(this.GetGauge().PivotPoint.ToPoint(), new SizeF(0f, 0f));
			rect.Inflate((float)(this.radius / 2.0), (float)(this.radius / 2.0));
			g.CreateDrawRegion(rect);
		}

		public override string ToString()
		{
			return this.Name;
		}

		public new CircularGauge GetGauge()
		{
			if (this.Collection != null)
			{
				return (CircularGauge)this.Collection.parent;
			}
			return null;
		}

		private float GetPositionFromValueNormalized(double value)
		{
			float positionFromValue = base.GetPositionFromValue(value);
			return Utils.NormalizeAngle(positionFromValue);
		}

		protected override PointF GetPoint(float position, float offset)
		{
			PointF pivotPoint = this.GetPivotPoint();
			float num = this.GetRadius() + offset;
			PointF[] array = new PointF[1]
			{
				new PointF(pivotPoint.X, pivotPoint.Y + num)
			};
			using (Matrix matrix = new Matrix())
			{
				matrix.RotateAt(position, pivotPoint);
				matrix.TransformPoints(array);
			}
			return array[0];
		}

		internal override double GetValue(PointF c, PointF p)
		{
			Math.Sqrt(Math.Pow((double)(p.X - c.X), 2.0) + Math.Pow((double)(p.Y - c.Y), 2.0));
			float num = (float)(Math.Atan2((double)(c.X - p.X), (double)(c.Y - p.Y)) * 180.0 / 3.1415926535897931);
			num = (float)(num + 180.0);
			num = (float)(360.0 - num);
			float num2 = (float)(this.StartAngle + this.SweepAngle - 360.0 + (360.0 - this.SweepAngle) / 2.0);
			if (num2 > 0.0 && num < num2)
			{
				num = (float)(num + 360.0);
			}
			return this.GetValueFromPosition(num);
		}

		internal float GetLargestRadius(GaugeGraphics g)
		{
			float num = 0f;
			if (this.MajorTickMark.Visible)
			{
				num = Math.Max(num, this.GetTickMarkOffset(g, this.MajorTickMark));
			}
			if (this.MinorTickMark.Visible)
			{
				num = Math.Max(num, this.GetTickMarkOffset(g, this.MinorTickMark));
			}
			if (this.LabelStyle.Visible)
			{
				num = Math.Max(num, this.GetLabelsOffset(g));
			}
			if (this.MinimumPin.Enable)
			{
				num = Math.Max(num, base.GetTickMarkOffset(this.MinimumPin));
			}
			if (this.MaximumPin.Enable)
			{
				num = Math.Max(num, base.GetTickMarkOffset(this.MaximumPin));
			}
			if (this.LabelStyle.Visible && this.MajorTickMark.Visible)
			{
				num = Math.Max(num, this.GetLabelsOffset(g) + this.GetTickMarkOffset(g, this.MajorTickMark));
			}
			if (this.LabelStyle.Visible && this.MinorTickMark.Visible)
			{
				num = Math.Max(num, this.GetLabelsOffset(g) + this.GetTickMarkOffset(g, this.MinorTickMark));
			}
			foreach (CustomLabel customLabel in base.CustomLabels)
			{
				if (customLabel.Visible)
				{
					num = Math.Max(num, this.GetCustomLabelOffset(g, customLabel));
				}
			}
			return (float)(this.Radius + num * this.Radius / 100.0);
		}

		internal float GetCustomLabelOffset(GaugeGraphics g, CustomLabel customLabel)
		{
			float num = (float)((customLabel.FontUnit != 0) ? (g.GetRelativeSize(new SizeF((float)(customLabel.Font.Size * 1.2999999523162842), 0f)).Width * (100.0 / this.Radius)) : customLabel.Font.Size);
			float num2 = 0f;
			num2 = (float)((customLabel.Placement != 0) ? ((customLabel.Placement != Placement.Cross) ? (customLabel.DistanceFromScale + num) : (0.0 - customLabel.DistanceFromScale + num / 2.0)) : (0.0 - this.Width / 2.0 - customLabel.DistanceFromScale));
			float val = (float)((customLabel.TickMarkStyle.Placement != 0) ? ((customLabel.TickMarkStyle.Placement != Placement.Cross) ? (customLabel.TickMarkStyle.DistanceFromScale + customLabel.TickMarkStyle.Length) : (0.0 - customLabel.TickMarkStyle.DistanceFromScale + customLabel.TickMarkStyle.Length / 2.0)) : (0.0 - this.Width / 2.0 - customLabel.TickMarkStyle.DistanceFromScale));
			return Math.Max(num2, val);
		}

		internal float GetTickMarkOffset(GaugeGraphics g, TickMark tickMark)
		{
			if (tickMark.Placement == Placement.Inside)
			{
				return (float)(0.0 - this.Width / 2.0 - tickMark.DistanceFromScale);
			}
			if (tickMark.Placement == Placement.Cross)
			{
				return (float)(0.0 - tickMark.DistanceFromScale + tickMark.Length / 2.0);
			}
			return tickMark.DistanceFromScale + tickMark.Length;
		}

		internal float GetLabelsOffset(GaugeGraphics g)
		{
			float num = 0f;
			float num2 = (float)((this.LabelStyle.FontUnit != 0) ? (g.GetRelativeSize(new SizeF((float)(this.LabelStyle.Font.Size * 1.2999999523162842), 0f)).Width * (100.0 / this.Radius)) : this.LabelStyle.Font.Size);
			if (this.LabelStyle.Placement == Placement.Inside)
			{
				return (float)(0.0 - this.Width / 2.0 - this.LabelStyle.DistanceFromScale);
			}
			if (this.LabelStyle.Placement == Placement.Cross)
			{
				return (float)(0.0 - this.LabelStyle.DistanceFromScale + num2 / 2.0);
			}
			return this.LabelStyle.DistanceFromScale + num2;
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
				this.SetDrawRegion(g);
				Gap gap = new Gap(this.GetRadius());
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
				float angularMargin = 4f;
				float num = 5f;
				using (GraphicsPath graphicsPath = this.GetBarPath(gap.Inside + num, gap.Outside + num, angularMargin))
				{
					if (graphicsPath != null)
					{
						PointF[] selectionMarkers = this.GetSelectionMarkers(g, gap.Inside + num, gap.Outside + num, angularMargin);
						g.DrawRadialSelection(g, graphicsPath, selectionMarkers, designTimeSelection, this.Common.GaugeCore.SelectionBorderColor, this.Common.GaugeCore.SelectionMarkerColor);
					}
				}
				g.RestoreDrawRegion();
				g.RestoreDrawRegion();
				foreach (IRenderable item2 in stack)
				{
					IRenderable renderable = item2;
					g.RestoreDrawRegion();
				}
			}
		}

		internal PointF[] GetSelectionMarkers(GaugeGraphics g, float barOffsetInside, float barOffsetOutside, float angularMargin)
		{
			PointF location = new PointF(this.GetPivotPoint().X - this.GetRadius(), this.GetPivotPoint().Y - this.GetRadius());
			RectangleF relative = new RectangleF(location, new SizeF((float)(this.GetRadius() * 2.0), (float)(this.GetRadius() * 2.0)));
			float num = 0f;
			if (this.MajorTickMark.Visible)
			{
				num = (float)Math.Atan(this.MajorTickMark.Width / 2.0 / this.GetRadius());
			}
			if (this.MinorTickMark.Visible)
			{
				num = (float)Math.Max((double)num, Math.Atan(this.MinorTickMark.Width / 2.0 / this.GetRadius()));
			}
			num = Utils.Rad2Deg(num);
			num += angularMargin;
			float startAngle = Utils.ToGDIAngle(base.StartPosition - num);
			Utils.ToGDIAngle(base.EndPosition + num);
			float sweepAngle = (float)(base.EndPosition - base.StartPosition + num * 2.0);
			ArrayList arrayList = new ArrayList();
			relative.Inflate(barOffsetOutside, barOffsetOutside);
			float flatness = 0.1f;
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				graphicsPath.AddArc(g.GetAbsoluteRectangle(relative), startAngle, sweepAngle);
				graphicsPath.Flatten(null, flatness);
				PointF[] pathPoints = graphicsPath.PathPoints;
				PointF pointF = default(PointF);
				PointF pointF2 = default(PointF);
				this.GetBoundsFromPoints(pathPoints, out pointF, out pointF2);
				if (this.SweepAngle + num * 2.0 < 360.0)
				{
					arrayList.Add(pathPoints[0]);
					arrayList.Add(pathPoints[pathPoints.Length - 1]);
				}
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				for (int i = 1; i < pathPoints.Length - 1; i++)
				{
					if (!flag && pathPoints[i].X == pointF.X)
					{
						flag = true;
						arrayList.Add(pathPoints[i]);
					}
					if (!flag2 && pathPoints[i].Y == pointF.Y)
					{
						flag2 = true;
						arrayList.Add(pathPoints[i]);
					}
					if (!flag3 && pathPoints[i].X == pointF2.X)
					{
						flag3 = true;
						arrayList.Add(pathPoints[i]);
					}
					if (!flag4 && pathPoints[i].Y == pointF2.Y)
					{
						flag3 = true;
						arrayList.Add(pathPoints[i]);
					}
				}
			}
			relative.Inflate((float)(0.0 - (barOffsetInside + barOffsetOutside)), (float)(0.0 - (barOffsetInside + barOffsetOutside)));
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(relative);
			if (absoluteRectangle.Width > 0.0 && absoluteRectangle.Height > 0.0)
			{
				using (GraphicsPath graphicsPath2 = new GraphicsPath())
				{
					graphicsPath2.AddArc(absoluteRectangle, startAngle, sweepAngle);
					graphicsPath2.Flatten(null, flatness);
					PointF[] pathPoints2 = graphicsPath2.PathPoints;
					PointF pointF3 = default(PointF);
					PointF pointF4 = default(PointF);
					this.GetBoundsFromPoints(pathPoints2, out pointF3, out pointF4);
					if (this.SweepAngle + num * 2.0 < 360.0)
					{
						arrayList.Add(pathPoints2[0]);
						arrayList.Add(pathPoints2[pathPoints2.Length - 1]);
					}
					bool flag5 = false;
					bool flag6 = false;
					bool flag7 = false;
					bool flag8 = false;
					for (int j = 1; j < pathPoints2.Length - 1; j++)
					{
						if (!flag5 && pathPoints2[j].X == pointF3.X)
						{
							flag5 = true;
							arrayList.Add(pathPoints2[j]);
						}
						if (!flag6 && pathPoints2[j].Y == pointF3.Y)
						{
							flag6 = true;
							arrayList.Add(pathPoints2[j]);
						}
						if (!flag7 && pathPoints2[j].X == pointF4.X)
						{
							flag7 = true;
							arrayList.Add(pathPoints2[j]);
						}
						if (!flag8 && pathPoints2[j].Y == pointF4.Y)
						{
							flag7 = true;
							arrayList.Add(pathPoints2[j]);
						}
					}
				}
			}
			return (PointF[])arrayList.ToArray(typeof(PointF));
		}

		internal void GetBoundsFromPoints(PointF[] points, out PointF minPoint, out PointF maxPoint)
		{
			minPoint = new PointF(3.40282347E+38f, 3.40282347E+38f);
			maxPoint = new PointF(-3.40282347E+38f, -3.40282347E+38f);
			for (int i = 0; i < points.Length; i++)
			{
				minPoint.X = Math.Min(minPoint.X, points[i].X);
				minPoint.Y = Math.Min(minPoint.Y, points[i].Y);
				maxPoint.X = Math.Max(maxPoint.X, points[i].X);
				maxPoint.Y = Math.Max(maxPoint.Y, points[i].Y);
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			CircularScale circularScale = new CircularScale();
			binaryFormatSerializer.Deserialize(circularScale, stream);
			return circularScale;
		}
	}
}
