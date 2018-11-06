using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class LinearScale : MapObject, IToolTipProvider
	{
		internal const double MaxMajorTickMarks = 16.0;

		private LinearLabelStyle labelStyle;

		private float position = 50f;

		private LinearMajorTickMark majorTickMark;

		private LinearMinorTickMark minorTickMark;

		internal LinearSpecialPosition minimumPin;

		internal LinearSpecialPosition maximumPin;

		internal float _startPosition;

		internal float _endPosition = 100f;

		internal float _sweepPosition = 100f;

		internal float coordSystemRatio = 3.6f;

		internal ArrayList markers = new ArrayList();

		internal ArrayList labels = new ArrayList();

		internal bool staticRendering = true;

		private double minimum;

		private double maximum = 100.0;

		private double multiplier = 1.0;

		private double interval = double.NaN;

		private double intervalOffset = double.NaN;

		private string toolTip = "";

		private string href = "";

		private bool tickMarksOnTop;

		private bool reversed;

		private bool logarithmic;

		private double logarithmicBase = 10.0;

		private bool visible = true;

		private float width = 5f;

		private Color borderColor = Color.DarkGray;

		private MapDashStyle borderStyle = MapDashStyle.Solid;

		private int borderWidth = 1;

		private Color fillColor = Color.CornflowerBlue;

		private GradientType fillGradientType;

		private Color fillSecondaryColor = Color.White;

		private MapHatchStyle fillHatchStyle;

		private float shadowOffset = 1f;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_LabelsAndTickMarks")]
		[Description("The label attributes of this scale.")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		internal LinearLabelStyle LabelStyle
		{
			get
			{
				return this.labelStyle;
			}
			set
			{
				this.labelStyle = value;
				this.labelStyle.Parent = this;
				this.Invalidate();
			}
		}

		[DefaultValue(50f)]
		[TypeConverter(typeof(FloatAutoValueConverter))]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLinearScale_Position")]
		[SRCategory("CategoryAttribute_Layout")]
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

		[DefaultValue(8f)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_StartMargin")]
		public float StartMargin
		{
			get
			{
				return this._startPosition;
			}
			set
			{
				if (!(value > 100.0) && !(value < 0.0))
				{
					this._startPosition = Math.Min(value, this._endPosition);
					this.InvalidateSweepPosition();
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 100.0));
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(8f)]
		[SRDescription("DescriptionAttributeLinearScale_EndMargin")]
		public float EndMargin
		{
			get
			{
				return (float)(100.0 - this._endPosition);
			}
			set
			{
				if (!(value > 100.0) && !(value < 0.0))
				{
					this._endPosition = Math.Max((float)(100.0 - value), this._startPosition);
					this.InvalidateSweepPosition();
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(0.0, 100.0));
			}
		}

		[SRDescription("DescriptionAttributeLinearScale_MajorTickMark")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[SRCategory("CategoryAttribute_LabelsAndTickMarks")]
		public LinearMajorTickMark MajorTickMark
		{
			get
			{
				return this.majorTickMark;
			}
			set
			{
				this.majorTickMark = value;
				this.majorTickMark.Parent = this;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_LabelsAndTickMarks")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeLinearScale_MinorTickMark")]
		public LinearMinorTickMark MinorTickMark
		{
			get
			{
				return this.minorTickMark;
			}
			set
			{
				this.minorTickMark = value;
				this.minorTickMark.Parent = this;
				this.Invalidate();
			}
		}

		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_MinimumPin")]
		public LinearSpecialPosition MinimumPin
		{
			get
			{
				return this.minimumPin;
			}
			set
			{
				this.minimumPin = value;
				this.minimumPin.Parent = this;
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeLinearScale_MaximumPin")]
		[SRCategory("CategoryAttribute_Layout")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public LinearSpecialPosition MaximumPin
		{
			get
			{
				return this.maximumPin;
			}
			set
			{
				this.maximumPin = value;
				this.maximumPin.Parent = this;
				this.Invalidate();
			}
		}

		internal ZoomPanel ParentGauge
		{
			get
			{
				return this.GetGauge();
			}
		}

		[DefaultValue(0.0)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_Minimum")]
		public double Minimum
		{
			get
			{
				return this.minimum;
			}
			set
			{
				if (this.Common != null)
				{
					if (value >= this.Maximum)
					{
						throw new ArgumentException(SR.min_max_error);
					}
					if (this.Logarithmic && value < 0.0)
					{
						throw new ArgumentException(SR.min_log_error);
					}
					if (this.Logarithmic && value == 0.0 && this.Maximum <= 1.0)
					{
						throw new ArgumentException(SR.min_log_error);
					}
					if (!double.IsNaN(value) && !double.IsInfinity(value))
					{
						goto IL_0087;
					}
					throw new ArgumentException(SR.invalid_param(value));
				}
				goto IL_0087;
				IL_0087:
				this.minimum = value;
				this.Invalidate();
			}
		}

		[DefaultValue(100.0)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_Maximum")]
		public double Maximum
		{
			get
			{
				return this.maximum;
			}
			set
			{
				if (this.Common != null)
				{
					if (value <= this.Minimum)
					{
						throw new ArgumentException(SR.max_min_error);
					}
					if (this.Logarithmic && value == 0.0 && this.Maximum <= 1.0)
					{
						throw new ArgumentException(SR.min_log_error);
					}
					if (!double.IsNaN(value) && !double.IsInfinity(value))
					{
						goto IL_0068;
					}
					throw new ArgumentException(SR.invalid_param(value));
				}
				goto IL_0068;
				IL_0068:
				this.maximum = value;
				this.Invalidate();
			}
		}

		[DefaultValue(1.0)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearScale_Multiplier")]
		public double Multiplier
		{
			get
			{
				return Math.Round(this.multiplier, 8);
			}
			set
			{
				this.multiplier = value;
				this.Invalidate();
			}
		}

		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_Interval")]
		[DefaultValue(double.NaN)]
		public double Interval
		{
			get
			{
				return this.interval;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.interval_negative);
				}
				if (value == 0.0)
				{
					value = double.NaN;
				}
				this.interval = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeLinearScale_IntervalOffset")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		public double IntervalOffset
		{
			get
			{
				return this.intervalOffset;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.interval_offset_negative);
				}
				this.intervalOffset = value;
				this.Invalidate();
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearScale_ToolTip")]
		[Localizable(true)]
		public string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.toolTip = value;
			}
		}

		[SRDescription("DescriptionAttributeLinearScale_Href")]
		[SRCategory("CategoryAttribute_Behavior")]
		[Localizable(true)]
		[DefaultValue("")]
		public string Href
		{
			get
			{
				return this.href;
			}
			set
			{
				this.href = value;
			}
		}

		[SRCategory("CategoryAttribute_LabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeLinearScale_TickMarksOnTop")]
		[DefaultValue(false)]
		public bool TickMarksOnTop
		{
			get
			{
				return this.tickMarksOnTop;
			}
			set
			{
				this.tickMarksOnTop = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearScale_Reversed")]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(false)]
		public bool Reversed
		{
			get
			{
				return this.reversed;
			}
			set
			{
				this.reversed = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLinearScale_Logarithmic")]
		[DefaultValue(false)]
		public bool Logarithmic
		{
			get
			{
				return this.logarithmic;
			}
			set
			{
				if (value && (this.Minimum < 0.0 || (this.Minimum == 0.0 && this.Maximum < 1.0)))
				{
					throw new ArgumentException(SR.min_log_error);
				}
				this.logarithmic = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearScale_LogarithmicBase")]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(10.0)]
		public double LogarithmicBase
		{
			get
			{
				return this.logarithmicBase;
			}
			set
			{
				if (value <= 1.0)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range_min_open(1.0));
				}
				this.logarithmicBase = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearScale_Visible")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(true)]
		[ParenthesizePropertyName(true)]
		public bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
				this.Invalidate();
			}
		}

		[DefaultValue(5f)]
		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeLinearScale_Width")]
		public virtual float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.width = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRDescription("DescriptionAttributeLinearScale_BorderColor")]
		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(MapDashStyle.Solid)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_BorderStyle")]
		public MapDashStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.Invalidate();
			}
		}

		[DefaultValue(1)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_BorderWidth")]
		public int BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					this.borderWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 100.0));
			}
		}

		[DefaultValue(typeof(Color), "CornflowerBlue")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_FillColor")]
		public Color FillColor
		{
			get
			{
				return this.fillColor;
			}
			set
			{
				this.fillColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearScale_FillGradientType")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(GradientType.None)]
		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		public GradientType FillGradientType
		{
			get
			{
				return this.fillGradientType;
			}
			set
			{
				this.fillGradientType = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_FillSecondaryColor")]
		public Color FillSecondaryColor
		{
			get
			{
				return this.fillSecondaryColor;
			}
			set
			{
				this.fillSecondaryColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(MapHatchStyle.None)]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLinearScale_FillHatchStyle")]
		public MapHatchStyle FillHatchStyle
		{
			get
			{
				return this.fillHatchStyle;
			}
			set
			{
				this.fillHatchStyle = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(1f)]
		[SRDescription("DescriptionAttributeLinearScale_ShadowOffset")]
		public float ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				if (!(value < -100.0) && !(value > 100.0))
				{
					this.shadowOffset = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(-100.0, 100.0));
			}
		}

		internal double MinimumLog
		{
			get
			{
				if (this.Minimum == 0.0 && this.Logarithmic)
				{
					return 1.0;
				}
				return this.Minimum;
			}
		}

		internal float StartPosition
		{
			get
			{
				object parent = this.Parent;
				if (parent is ZoomPanel)
				{
					ZoomPanel zoomPanel = (ZoomPanel)parent;
					if (zoomPanel.GetOrientation() == Orientation.Vertical)
					{
						if (this.GetReversed())
						{
							return (float)(100.0 - (this._endPosition - this.GetMaxOffset(zoomPanel)));
						}
						return (float)(100.0 - this._endPosition);
					}
					if (this.GetReversed())
					{
						return this._startPosition;
					}
					return this._startPosition + this.GetMaxOffset(zoomPanel);
				}
				return this._startPosition;
			}
		}

		internal float EndPosition
		{
			get
			{
				object parent = this.Parent;
				if (parent is ZoomPanel)
				{
					ZoomPanel zoomPanel = (ZoomPanel)parent;
					if (zoomPanel.GetOrientation() == Orientation.Vertical)
					{
						if (this.GetReversed())
						{
							return (float)(100.0 - this._startPosition);
						}
						return (float)(100.0 - (this._startPosition + this.GetMaxOffset(zoomPanel)));
					}
					if (this.GetReversed())
					{
						return this._endPosition - this.GetMaxOffset(zoomPanel);
					}
				}
				return this._endPosition;
			}
		}

		internal float SweepPosition
		{
			get
			{
				return this._sweepPosition;
			}
		}

		public LinearScale()
			: this(null)
		{
		}

		public LinearScale(object parent)
			: base(parent)
		{
			this._startPosition = 8f;
			this._endPosition = 92f;
			this.coordSystemRatio = 1f;
			this.width = 5f;
			this.InvalidateSweepPosition();
			this.majorTickMark = new LinearMajorTickMark(this);
			this.minorTickMark = new LinearMinorTickMark(this);
			this.labelStyle = new LinearLabelStyle(this);
			this.maximumPin = new LinearSpecialPosition(this);
			this.minimumPin = new LinearSpecialPosition(this);
		}

		public ZoomPanel GetGauge()
		{
			return (ZoomPanel)this.Parent;
		}

		private GraphicsPath GetBarPath(float barOffsetInside, float barOffsetOutside)
		{
			MapGraphics graph = this.Common.Graph;
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
			if (this.ParentGauge.GetOrientation() == Orientation.Horizontal)
			{
				rectangleF.X = this.StartPosition;
				rectangleF.Width = this.EndPosition - this.StartPosition;
				rectangleF.Y = this.Position - barOffsetInside;
				rectangleF.Height = barOffsetInside + barOffsetOutside;
				rectangleF = graph.GetAbsoluteRectangle(rectangleF);
				rectangleF.Inflate(graph.GetAbsoluteDimension(num), 0f);
			}
			else
			{
				rectangleF.Y = this.StartPosition;
				rectangleF.Height = this.EndPosition - this.StartPosition;
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

		private void SetScaleHitTestPath(MapGraphics g)
		{
			Gap gap = new Gap(this.Position);
			gap.SetOffset(Placement.Cross, this.Width);
			gap.SetBase();
			if (this.MajorTickMark.Visible)
			{
				gap.SetOffsetBase(this.MajorTickMark.Placement, this.MajorTickMark.Length);
			}
			if (this.MinorTickMark.Visible)
			{
				gap.SetOffsetBase(this.MinorTickMark.Placement, this.MinorTickMark.Length);
			}
			using (GraphicsPath graphicsPath = this.GetBarPath(gap.Inside, gap.Outside))
			{
				this.Common.MapCore.HotRegionList.SetHotRegion(g, this, graphicsPath);
			}
		}

		internal GraphicsPath GetShadowPath()
		{
			if (this.Visible && this.ShadowOffset != 0.0 && this.Width > 0.0)
			{
				GraphicsPath barPath = this.GetBarPath((float)(this.Width / 2.0), (float)(this.Width / 2.0));
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate(this.ShadowOffset, this.ShadowOffset);
					barPath.Transform(matrix);
					return barPath;
				}
			}
			return null;
		}

		private void RenderBar(MapGraphics g)
		{
			using (GraphicsPath path = this.GetBarPath((float)(this.Width / 2.0), (float)(this.Width / 2.0)))
			{
				g.DrawPathAbs(path, this.FillColor, this.FillHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, this.FillGradientType, this.FillSecondaryColor, this.BorderColor, this.BorderWidth, this.BorderStyle, PenAlignment.Outset);
			}
		}

		internal void DrawTickMark(MapGraphics g, CustomTickMark tickMark, double value, float offset)
		{
			float num = this.GetPositionFromValue(value);
			PointF absolutePoint = g.GetAbsolutePoint(this.GetPoint(num, offset));
			using (Matrix matrix = new Matrix())
			{
				if (this.ParentGauge.GetOrientation() == Orientation.Vertical)
				{
					matrix.RotateAt(90f, absolutePoint);
				}
				if (tickMark.Placement == Placement.Outside)
				{
					matrix.RotateAt(180f, absolutePoint);
				}
				this.DrawTickMark(g, tickMark, value, offset, matrix);
			}
		}

		internal LinearLabelStyle GetLabelStyle()
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
			if (!MarkerPosition.IsExistsInArray(this.labels, markerPosition))
			{
				if (!string.IsNullOrEmpty(labelStr))
				{
					this.labels.Add(markerPosition);
				}
				MapGraphics graph = this.Common.Graph;
				using (Brush brush2 = new SolidBrush(color))
				{
					Font resizedFont = this.GetResizedFont(font, fontUnit);
					try
					{
						float num2 = 0f;
						if (this.ParentGauge.GetOrientation() == Orientation.Vertical)
						{
							num2 = 90f;
						}
						SizeF size = graph.MeasureString(labelStr, resizedFont);
						float contactPointOffset = Utils.GetContactPointOffset(size, rotateLabelAngle - num2);
						PointF absolutePoint = graph.GetAbsolutePoint(this.GetPoint(num, labelPos));
						switch (placement)
						{
						case Placement.Inside:
							if (this.ParentGauge.GetOrientation() == Orientation.Vertical)
							{
								absolutePoint.X -= contactPointOffset;
							}
							else
							{
								absolutePoint.Y -= contactPointOffset;
							}
							break;
						case Placement.Outside:
							if (this.ParentGauge.GetOrientation() == Orientation.Vertical)
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
								if (this.ShadowOffset != 0.0)
								{
									using (Brush brush = graph.GetShadowBrush())
									{
										RectangleF layoutRectangle = rectangleF;
										layoutRectangle.Offset(this.ShadowOffset, this.ShadowOffset);
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
									if (this.ShadowOffset != 0.0)
									{
										using (Brush brush3 = graph.GetShadowBrush())
										{
											using (Matrix matrix2 = matrix.Clone())
											{
												matrix2.Translate(this.ShadowOffset, this.ShadowOffset);
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

		private void RenderLabels(MapGraphics g)
		{
			if (this.LabelStyle.Visible)
			{
				double num = this.GetInterval(IntervalTypes.Labels);
				float offsetLabelPos = this.GetOffsetLabelPos(this.LabelStyle.Placement, this.LabelStyle.DistanceFromScale, this.Position);
				double minimumLog = this.MinimumLog;
				double num2 = this.GetIntervalOffset(IntervalTypes.Labels);
				Color textColor = this.LabelStyle.TextColor;
				if (this.LabelStyle.ShowEndLabels && num2 > 0.0)
				{
					textColor = this.LabelStyle.TextColor;
					string labelStr = string.Format(CultureInfo.CurrentCulture, this.LabelStyle.GetFormatStr(), minimumLog * this.Multiplier);
					this.DrawLabel(this.LabelStyle.Placement, labelStr, minimumLog, offsetLabelPos, this.LabelStyle.FontAngle, this.LabelStyle.Font, textColor, this.LabelStyle.FontUnit);
					this.DrawTickMark(g, this.MinorTickMark, minimumLog, this.GetTickMarkOffset(this.MinorTickMark));
				}
				minimumLog += num2;
				double num3 = 0.0;
				while (minimumLog <= this.Maximum)
				{
					bool flag = true;
					if (!this.LabelStyle.ShowEndLabels && (minimumLog == this.MinimumLog || minimumLog == this.Maximum))
					{
						flag = false;
					}
					if (flag)
					{
						textColor = this.LabelStyle.TextColor;
						string labelStr2 = string.Format(CultureInfo.CurrentCulture, this.LabelStyle.GetFormatStr(), minimumLog * this.Multiplier);
						this.DrawLabel(this.LabelStyle.Placement, labelStr2, minimumLog, offsetLabelPos, this.LabelStyle.FontAngle, this.LabelStyle.Font, textColor, this.LabelStyle.FontUnit);
					}
					num3 = minimumLog;
					minimumLog = this.GetNextPosition(minimumLog, num, false);
				}
				if (this.LabelStyle.ShowEndLabels && num3 < this.Maximum)
				{
					minimumLog = this.Maximum;
					textColor = this.LabelStyle.TextColor;
					string labelStr3 = string.Format(CultureInfo.CurrentCulture, this.LabelStyle.GetFormatStr(), minimumLog * this.Multiplier);
					this.DrawLabel(this.LabelStyle.Placement, labelStr3, minimumLog, offsetLabelPos, this.LabelStyle.FontAngle, this.LabelStyle.Font, textColor, this.LabelStyle.FontUnit);
					this.DrawTickMark(g, this.MinorTickMark, minimumLog, this.GetTickMarkOffset(this.MinorTickMark));
				}
			}
		}

		internal void DrawSpecialPosition(MapGraphics g, SpecialPosition label, float angle)
		{
			if (label.Enable)
			{
				LinearPinLabel linearPinLabel = ((LinearSpecialPosition)label).LabelStyle;
				if (linearPinLabel.Text != string.Empty && this.staticRendering)
				{
					this.DrawLabel(linearPinLabel.Placement, linearPinLabel.Text, this.GetValueFromPosition(angle), this.GetOffsetLabelPos(linearPinLabel.Placement, linearPinLabel.DistanceFromScale, this.Position), linearPinLabel.FontAngle, linearPinLabel.Font, linearPinLabel.TextColor, linearPinLabel.FontUnit);
				}
				if ((!label.Visible || this.TickMarksOnTop) && this.staticRendering)
				{
					return;
				}
				float tickMarkOffset = this.GetTickMarkOffset(label);
				this.DrawTickMark(g, label, this.GetValueFromPosition(angle), tickMarkOffset);
			}
		}

		internal void RenderStaticElements(MapGraphics g)
		{
			if (this.Visible)
			{
				g.StartHotRegion(this);
				GraphicsState gstate = g.Save();
				try
				{
					this.staticRendering = true;
					if (!this.TickMarksOnTop)
					{
						this.markers.Clear();
					}
					this.labels.Clear();
					this.RenderBar(g);
					if (!this.TickMarksOnTop)
					{
						this.RenderGrid(g);
					}
					this.RenderLabels(g);
					this.RenderPins(g);
					this.SetScaleHitTestPath(g);
					if (!this.TickMarksOnTop)
					{
						this.markers.Sort();
					}
				}
				finally
				{
					g.Restore(gstate);
					g.EndHotRegion();
				}
			}
		}

		internal void RenderDynamicElements(MapGraphics g)
		{
			if (this.Visible && this.TickMarksOnTop)
			{
				GraphicsState gstate = g.Save();
				try
				{
					this.staticRendering = false;
					this.markers.Clear();
					this.RenderGrid(g);
					this.RenderPins(g);
					this.markers.Sort();
				}
				finally
				{
					g.Restore(gstate);
				}
			}
		}

		protected bool IsReversed()
		{
			if (this.Parent != null && this.ParentGauge.GetOrientation() == Orientation.Vertical)
			{
				return !this.GetReversed();
			}
			return this.GetReversed();
		}

		protected PointF GetPoint(float position, float offset)
		{
			PointF empty = PointF.Empty;
			if (this.ParentGauge.GetOrientation() == Orientation.Horizontal)
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

		internal double GetValue(PointF c, PointF p)
		{
			if (this.Common != null)
			{
				HotRegionList hotRegionList = this.Common.MapCore.HotRegionList;
				int num = hotRegionList.FindHotRegionOfObject(this.GetGauge());
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

		private float GetMaxOffset(ZoomPanel gauge)
		{
			if (gauge.AbsoluteSize.IsEmpty)
			{
				return 0f;
			}
			return 0f;
		}

		internal bool GetReversed()
		{
			return this.Reversed;
		}

		internal Brush GetLightBrush(MapGraphics g, CustomTickMark tickMark, Color fillColor, GraphicsPath path)
		{
			Brush brush = null;
			if (tickMark.EnableGradient)
			{
				HSV hsv = ColorHandler.ColorToHSV(fillColor);
				hsv.value = (int)((double)hsv.value * 0.2);
				Color color = ColorHandler.HSVtoColor(hsv);
				color = Color.FromArgb(fillColor.A, color.R, color.G, color.B);
				RectangleF bounds = path.GetBounds();
				float num = (float)(1.0 - tickMark.GradientDensity / 100.0);
				if (tickMark.Shape == MarkerStyle.Circle)
				{
					brush = new PathGradientBrush(path);
					((PathGradientBrush)brush).CenterColor = fillColor;
					((PathGradientBrush)brush).CenterPoint = new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0));
					((PathGradientBrush)brush).SurroundColors = new Color[1]
					{
						color
					};
					Blend blend = new Blend();
					blend.Factors = new float[2]
					{
						num,
						1f
					};
					blend.Positions = new float[2]
					{
						0f,
						1f
					};
					((PathGradientBrush)brush).Blend = blend;
				}
				else
				{
					brush = new LinearGradientBrush(path.GetBounds(), color, fillColor, LinearGradientMode.Vertical);
					Blend blend2 = new Blend();
					blend2.Factors = new float[3]
					{
						num,
						1f,
						num
					};
					blend2.Positions = new float[3]
					{
						0f,
						0.5f,
						1f
					};
					((LinearGradientBrush)brush).Blend = blend2;
				}
			}
			else
			{
				brush = new SolidBrush(fillColor);
			}
			return brush;
		}

		internal void DrawTickMark(MapGraphics g, CustomTickMark tickMark, double value, float offset, Matrix matrix)
		{
			if (!(tickMark.Width <= 0.0) && !(tickMark.Length <= 0.0))
			{
				float num = this.GetPositionFromValue(value);
				MarkerPosition markerPosition = new MarkerPosition((float)Math.Round((double)num), value, tickMark.Placement);
				if (!MarkerPosition.IsExistsInArray(this.markers, markerPosition))
				{
					this.markers.Add(markerPosition);
					PointF absolutePoint = g.GetAbsolutePoint(this.GetPoint(num, offset));
					if (tickMark.Image != string.Empty)
					{
						this.DrawTickMarkImage(g, tickMark, matrix, absolutePoint, false);
					}
					else
					{
						SizeF sizeF = new SizeF(g.GetAbsoluteDimension(tickMark.Width), g.GetAbsoluteDimension(tickMark.Length));
						Color color = tickMark.FillColor;
						using (GraphicsPath graphicsPath = g.CreateMarker(absolutePoint, sizeF.Width, sizeF.Height, tickMark.Shape))
						{
							using (Brush brush = this.GetLightBrush(g, tickMark, color, graphicsPath))
							{
								graphicsPath.Transform(matrix);
								if (tickMark.EnableGradient)
								{
									if (brush is LinearGradientBrush)
									{
										((LinearGradientBrush)brush).Transform = matrix;
									}
									else if (brush is PathGradientBrush)
									{
										((PathGradientBrush)brush).Transform = matrix;
									}
								}
								if (this.ShadowOffset != 0.0)
								{
									g.DrawPathShadowAbs(graphicsPath, g.GetShadowColor(), this.ShadowOffset);
								}
								g.FillPath(brush, graphicsPath, 0f, false, false);
								if (tickMark.BorderWidth > 0)
								{
									using (Pen pen = new Pen(tickMark.BorderColor, (float)tickMark.BorderWidth))
									{
										pen.Alignment = PenAlignment.Outset;
										g.DrawPath(pen, graphicsPath);
									}
								}
							}
						}
					}
				}
			}
		}

		internal void DrawTickMarkImage(MapGraphics g, CustomTickMark tickMark, Matrix matrix, PointF centerPoint, bool drawShadow)
		{
			float absoluteDimension = g.GetAbsoluteDimension(tickMark.Length);
			Image image = null;
			image = this.Common.ImageLoader.LoadImage(tickMark.Image);
			if (image.Width != 0 && image.Height != 0)
			{
				float num = (float)image.Height;
				float num2 = absoluteDimension / num;
				Rectangle destRect = new Rectangle(0, 0, (int)((float)image.Width * num2), (int)((float)image.Height * num2));
				ImageAttributes imageAttributes = new ImageAttributes();
				if (tickMark.ImageTransColor != Color.Empty)
				{
					imageAttributes.SetColorKey(tickMark.ImageTransColor, tickMark.ImageTransColor, ColorAdjustType.Default);
				}
				Matrix transform = g.Transform;
				Matrix matrix2 = g.Transform.Clone();
				matrix2.Multiply(matrix, MatrixOrder.Prepend);
				if (drawShadow)
				{
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = 0f;
					colorMatrix.Matrix11 = 0f;
					colorMatrix.Matrix22 = 0f;
					colorMatrix.Matrix33 = (float)(this.Common.MapCore.ShadowIntensity / 100.0);
					imageAttributes.SetColorMatrix(colorMatrix);
					matrix2.Translate(this.ShadowOffset, this.ShadowOffset, MatrixOrder.Append);
				}
				destRect.X = (int)(centerPoint.X - (float)(destRect.Width / 2));
				destRect.Y = (int)(centerPoint.Y - (float)(destRect.Height / 2));
				g.Transform = matrix2;
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				g.Transform = transform;
			}
		}

		internal float GetTickMarkOffset(CustomTickMark tickMark)
		{
			float num = 0f;
			switch (tickMark.Placement)
			{
			case Placement.Inside:
				return (float)((0.0 - this.Width) / 2.0 - tickMark.Length / 2.0 - tickMark.DistanceFromScale);
			case Placement.Cross:
				return (float)(0.0 - tickMark.DistanceFromScale);
			case Placement.Outside:
				return (float)(this.Width / 2.0 + tickMark.Length / 2.0 + tickMark.DistanceFromScale);
			default:
				throw new InvalidOperationException(SR.invalid_placement_type);
			}
		}

		internal void RenderTicks(MapGraphics g, TickMark tickMark, double interval, double max, double min, double intOffset, bool forceLinear)
		{
			float tickMarkOffset = this.GetTickMarkOffset(tickMark);
			for (double num = min + intOffset; num <= max; num = this.GetNextPosition(num, interval, forceLinear))
			{
				this.DrawTickMark(g, tickMark, num, tickMarkOffset);
			}
		}

		internal void RenderGrid(MapGraphics g)
		{
			if (this.MajorTickMark.Visible)
			{
				this.RenderTicks(g, this.MajorTickMark, this.GetInterval(IntervalTypes.Major), this.Maximum, this.MinimumLog, this.GetIntervalOffset(IntervalTypes.Major), false);
			}
			if (this.MinorTickMark.Visible)
			{
				if (!this.Logarithmic)
				{
					this.RenderTicks(g, this.MinorTickMark, this.GetInterval(IntervalTypes.Minor), this.Maximum, this.MinimumLog, this.GetIntervalOffset(IntervalTypes.Minor), false);
				}
				else
				{
					double num = this.GetIntervalOffset(IntervalTypes.Minor);
					double num2 = this.MinimumLog + num;
					double num3 = this.GetInterval(IntervalTypes.Major);
					double nextPosition = this.GetNextPosition(num2, num3, false);
					double num4 = this.GetInterval(IntervalTypes.Minor);
					num4 = 1.0 / num4 * this.LogarithmicBase;
					while (num2 <= nextPosition && num2 < this.Maximum)
					{
						this.RenderTicks(g, this.MinorTickMark, nextPosition / num4, Math.Min(nextPosition, this.Maximum), num2, num, true);
						num2 = nextPosition;
						nextPosition = this.GetNextPosition(nextPosition, num3, false);
					}
				}
			}
		}

		internal float GetOffsetLabelPos(Placement placement, float scaleOffset, float scalePosition)
		{
			Gap gap = new Gap(scalePosition);
			gap.SetOffset(Placement.Cross, this.Width);
			gap.SetBase();
			if (this.MajorTickMark.Visible)
			{
				gap.SetOffsetBase(this.MajorTickMark.Placement, this.MajorTickMark.Length);
			}
			if (this.MinorTickMark.Visible)
			{
				gap.SetOffsetBase(this.MinorTickMark.Placement, this.MinorTickMark.Length);
			}
			gap.SetBase();
			float num = 0f;
			switch (placement)
			{
			case Placement.Inside:
				return (float)(0.0 - gap.Inside - scaleOffset);
			case Placement.Cross:
				return (float)(0.0 - scaleOffset);
			case Placement.Outside:
				return gap.Outside + scaleOffset;
			default:
				throw new InvalidOperationException(SR.invalid_placement_type);
			}
		}

		internal Font GetResizedFont(Font font, FontUnit fontUnit)
		{
			if (fontUnit == FontUnit.Percent)
			{
				float absoluteDimension = this.Common.Graph.GetAbsoluteDimension(font.Size);
				return new Font(font.FontFamily.Name, absoluteDimension, font.Style, GraphicsUnit.Pixel, font.GdiCharSet, font.GdiVerticalFont);
			}
			return font;
		}

		internal void RenderPins(MapGraphics g)
		{
			if (!this.IsReversed())
			{
				this.DrawSpecialPosition(g, this.MinimumPin, this.StartPosition - this.MinimumPin.Location);
				this.DrawSpecialPosition(g, this.MaximumPin, this.EndPosition + this.MaximumPin.Location);
			}
			else
			{
				this.DrawSpecialPosition(g, this.MinimumPin, this.EndPosition + this.MinimumPin.Location);
				this.DrawSpecialPosition(g, this.MaximumPin, this.StartPosition - this.MaximumPin.Location);
			}
		}

		protected void InvalidateEndPosition()
		{
			this._endPosition = this._startPosition + this._sweepPosition;
		}

		protected void InvalidateSweepPosition()
		{
			this._sweepPosition = this._endPosition - this._startPosition;
		}

		internal virtual double GetValueLimit(double value, bool snapEnable, double snapInterval)
		{
			double valueLimit = this.GetValueLimit(value);
			if (snapEnable)
			{
				if (snapInterval == 0.0)
				{
					return MarkerPosition.Snap(this.markers, valueLimit);
				}
				if (this.Logarithmic)
				{
					snapInterval = Math.Pow(this.LogarithmicBase, snapInterval);
					return Math.Max(this.minimum, this.GetValueLimit(snapInterval * Math.Round(valueLimit / snapInterval)));
				}
				return this.GetValueLimit(snapInterval * Math.Round(valueLimit / snapInterval));
			}
			return valueLimit;
		}

		internal virtual double GetValueLimit(double value)
		{
			float num = this.StartPosition - this.MinimumPin.Location;
			float num2 = this.EndPosition + this.MaximumPin.Location;
			if (this.IsReversed())
			{
				num = this.EndPosition + this.MinimumPin.Location;
				num2 = this.StartPosition - this.MaximumPin.Location;
			}
			if (double.IsNaN(value))
			{
				if (this.MinimumPin.Enable)
				{
					return this.GetValueFromPosition(num);
				}
				return this.MinimumLog;
			}
			double num3 = this.MinimumLog;
			if (this.MinimumPin.Enable)
			{
				num3 = this.GetValueFromPosition(num);
			}
			double valueFromPosition = this.Maximum;
			if (this.MaximumPin.Enable)
			{
				valueFromPosition = this.GetValueFromPosition(num2);
			}
			if (value < num3)
			{
				return num3;
			}
			if (value > valueFromPosition)
			{
				return valueFromPosition;
			}
			return value;
		}

		internal double GetIntervalOffset(IntervalTypes type)
		{
			double num = 0.0;
			switch (type)
			{
			case IntervalTypes.Minor:
				num = this.MinorTickMark.IntervalOffset;
				if (double.IsNaN(num))
				{
					num = this.GetIntervalOffset(IntervalTypes.Major) % this.GetInterval(IntervalTypes.Minor);
				}
				break;
			case IntervalTypes.Major:
				num = this.MajorTickMark.IntervalOffset;
				if (double.IsNaN(num))
				{
					num = this.GetIntervalOffset(IntervalTypes.Main);
				}
				break;
			case IntervalTypes.Labels:
				num = this.GetLabelStyle().IntervalOffset;
				if (double.IsNaN(num))
				{
					num = this.GetIntervalOffset(IntervalTypes.Major);
				}
				break;
			case IntervalTypes.Main:
				num = this.IntervalOffset;
				if (double.IsNaN(num))
				{
					num = 0.0;
				}
				break;
			}
			return num;
		}

		internal double GetInterval(IntervalTypes type)
		{
			double num = (this.Maximum - this.MinimumLog) / 10.0;
			switch (type)
			{
			case IntervalTypes.Minor:
				num = this.MinorTickMark.Interval;
				if (double.IsNaN(num))
				{
					if (!this.Logarithmic)
					{
						double num6 = this.GetInterval(IntervalTypes.Major);
						double num7 = (double)(this.SweepPosition / this.coordSystemRatio);
						if (this.coordSystemRatio < 3.0)
						{
							num7 /= 2.0;
						}
						double a = Math.Round(96.0 * (num7 / 100.0)) / ((this.Maximum - this.MinimumLog) / num6);
						if (Math.Pow(10.0, Math.Round(Math.Log10(num6))) == num6)
						{
							return num6 / 5.0;
						}
						int num8 = (int)(0.0 - (Math.Round(Math.Log10(num6)) - 1.0));
						double num9 = Math.Pow(10.0, (double)(-num8)) * 2.0;
						for (int i = 0; i < 2; i++)
						{
							for (int num10 = (int)Math.Round(a); num10 > 0; num10--)
							{
								double num11 = num6 / (double)num10;
								if ((num11 % num9 != 0.0 || i != 0) && Utils.Round(num11, num8) == num11)
								{
									return num11;
								}
							}
						}
						num = Math.Pow(10.0, Math.Floor(Math.Log10(num6)) - 1.0);
						if (num6 % 2.0 == 0.0)
						{
							return num * 2.0;
						}
						if (num6 % 5.0 == 0.0)
						{
							return num * 5.0;
						}
						if (num6 % 3.0 == 0.0)
						{
							return num * 3.0;
						}
						return num;
					}
					num = 1.0;
				}
				break;
			case IntervalTypes.Major:
				num = this.MajorTickMark.Interval;
				if (double.IsNaN(num))
				{
					num = this.GetInterval(IntervalTypes.Main);
				}
				break;
			case IntervalTypes.Labels:
				num = this.GetLabelStyle().Interval;
				if (double.IsNaN(num))
				{
					num = this.GetInterval(IntervalTypes.Major);
				}
				break;
			case IntervalTypes.Main:
				num = this.Interval;
				if (double.IsNaN(num))
				{
					if (!this.Logarithmic)
					{
						double num2 = Math.Pow(10.0, Math.Round(Math.Log10(this.Maximum - this.MinimumLog)) - 1.0);
						if ((this.Maximum - this.MinimumLog) / num2 < 7.0)
						{
							num2 /= 10.0;
						}
						num = num2;
						double num3 = (this.Maximum - this.MinimumLog) / num;
						double num4 = (double)(this.SweepPosition / this.coordSystemRatio);
						if (this.coordSystemRatio < 3.0)
						{
							num4 /= 2.0;
						}
						double num5 = Math.Round(16.0 * (num4 / 100.0));
						do
						{
							if (Math.Round(num3, 0) == num3 && !(num3 > num5))
							{
								break;
							}
							num += num2;
							num3 = (this.Maximum - this.MinimumLog) / num;
						}
						while (!(num3 <= Math.Max(num5 / 2.0, 1.0)));
					}
					else
					{
						num = 1.0;
					}
				}
				break;
			}
			if ((this.Maximum - this.MinimumLog) / num > 1000.0)
			{
				return (this.Maximum - this.MinimumLog) / 1000.0;
			}
			return num;
		}

		internal double GetNextPosition(double position, double interval, bool forceLinear)
		{
			position = ((!forceLinear && this.Logarithmic) ? Math.Pow(this.LogarithmicBase, Math.Log(position, this.LogarithmicBase) + interval) : (position + interval));
			return position;
		}

		protected virtual double GetValueAgainstScaleRatio(double value)
		{
			double result = 0.0;
			if (!this.Logarithmic)
			{
				result = (value - this.MinimumLog) / (this.Maximum - this.MinimumLog);
			}
			else if (this.Logarithmic)
			{
				double num = Math.Log(this.Maximum, this.LogarithmicBase);
				double num2 = Math.Log(this.MinimumLog, this.LogarithmicBase);
				result = (Math.Log(value, this.LogarithmicBase) - num2) / (num - num2);
			}
			return result;
		}

		protected virtual double GetValueByRatio(float ratio)
		{
			double result = 0.0;
			if (!this.Logarithmic)
			{
				result = this.MinimumLog + (this.Maximum - this.MinimumLog) * (double)ratio;
			}
			else if (this.Logarithmic)
			{
				double num = Math.Log(this.Maximum, this.LogarithmicBase);
				double num2 = Math.Log(this.MinimumLog, this.LogarithmicBase);
				result = Math.Pow(this.LogarithmicBase, num2 + (num - num2) * (double)ratio);
			}
			return result;
		}

		protected float GetPositionFromValue(double value, float startPos, float endPos)
		{
			double valueAgainstScaleRatio = this.GetValueAgainstScaleRatio(value);
			double num = (double)(endPos - startPos);
			float num2 = 0f;
			if (this.IsReversed())
			{
				return (float)((double)endPos - num * valueAgainstScaleRatio);
			}
			return (float)((double)startPos + num * valueAgainstScaleRatio);
		}

		internal virtual float GetPositionFromValue(double value)
		{
			return this.GetPositionFromValue(value, this.StartPosition / this.coordSystemRatio, this.EndPosition / this.coordSystemRatio) * this.coordSystemRatio;
		}

		internal virtual double GetValueFromPosition(float position)
		{
			double num = (double)((position - this.StartPosition) / (this.EndPosition - this.StartPosition));
			if (this.IsReversed())
			{
				num = 1.0 - num;
			}
			return this.GetValueByRatio((float)num);
		}

		internal virtual PointF GetPointRel(double value, float offset)
		{
			return this.GetPoint(this.GetPositionFromValue(value), offset);
		}

		internal virtual PointF GetPointAbs(double value, float offset)
		{
			if (this.Common != null)
			{
				return this.Common.Graph.GetAbsolutePoint(this.GetPointRel(value, offset));
			}
			throw new ApplicationException(SR.gdi_noninitialized);
		}

		internal override void BeginInit()
		{
			base.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
		}

		internal override void Invalidate()
		{
			base.Invalidate();
		}

		string IToolTipProvider.GetToolTip()
		{
			return this.ToolTip;
		}
	}
}
