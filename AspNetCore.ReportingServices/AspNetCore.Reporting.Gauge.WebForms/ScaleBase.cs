using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal abstract class ScaleBase : NamedElement, IToolTipProvider, IImageMapProvider
	{
		internal const double MaxMajorTickMarks = 16.0;

		internal float _startPosition;

		internal float _endPosition = 100f;

		internal float _sweepPosition = 100f;

		internal float coordSystemRatio = 3.6f;

		internal LinearLabelStyle baseLabelStyle;

		internal ArrayList markers = new ArrayList();

		internal ArrayList labels = new ArrayList();

		internal bool staticRendering = true;

		private CustomLabelCollection customLabels;

		private double minimum;

		private double maximum = 100.0;

		private double multiplier = 1.0;

		private double interval = double.NaN;

		private double intervalOffset = double.NaN;

		private string toolTip = "";

		private string href = "";

		private string mapAreaAttributes = "";

		internal TickMark majorTickMarkA;

		internal TickMark minorTickMarkA;

		private bool tickMarksOnTop;

		private bool reversed;

		private bool logarithmic;

		private double logarithmicBase = 10.0;

		internal SpecialPosition minimumPin;

		internal SpecialPosition maximumPin;

		private bool visible = true;

		private float width = 5f;

		private Color borderColor = Color.Black;

		private GaugeDashStyle borderStyle;

		private int borderWidth = 1;

		private Color fillColor = Color.CornflowerBlue;

		private GradientType fillGradientType;

		private Color fillGradientEndColor = Color.White;

		private GaugeHatchStyle fillHatchStyle;

		private float shadowOffset = 1f;

		private bool selected;

		private object imageMapProviderTag;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CustomLabelCollection CustomLabels
		{
			get
			{
				return this.customLabels;
			}
		}

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeName11")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeMinimum3")]
		[DefaultValue(0.0)]
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
						throw new ArgumentException(Utils.SRGetStr("ExceptionMinMax"));
					}
					if (this.Logarithmic && value < 0.0)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMinLog"));
					}
					if (this.Logarithmic && value == 0.0 && this.Maximum <= 1.0)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMinLog"));
					}
					if (!double.IsNaN(value) && !double.IsInfinity(value))
					{
						goto IL_00b5;
					}
					throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
				}
				goto IL_00b5;
				IL_00b5:
				this.minimum = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMaximum3")]
		[Browsable(false)]
		[SRCategory("CategoryLayout")]
		[DefaultValue(100.0)]
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
						throw new ArgumentException(Utils.SRGetStr("ExceptionMaxMin"));
					}
					if (this.Logarithmic && value == 0.0 && this.Maximum <= 1.0)
					{
						throw new ArgumentException(Utils.SRGetStr("ExceptionMinLog"));
					}
					if (!double.IsNaN(value) && !double.IsInfinity(value))
					{
						goto IL_008b;
					}
					throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidValue"));
				}
				goto IL_008b;
				IL_008b:
				this.maximum = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMultiplier")]
		[SRCategory("CategoryBehavior")]
		[DefaultValue(1.0)]
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

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeInterval4")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[DefaultValue(double.NaN)]
		public double Interval
		{
			get
			{
				return this.interval;
			}
			set
			{
				if (!(value < 0.0) && !(value > 7.9228162514264338E+28))
				{
					if (value == 0.0)
					{
						value = double.NaN;
					}
					this.interval = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 79228162514264337593543950335m));
			}
		}

		[DefaultValue(double.NaN)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeIntervalOffset4")]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionIntervalOffsetNegative"));
				}
				this.intervalOffset = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeToolTip3")]
		[Localizable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
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

		[SRDescription("DescriptionAttributeHref7")]
		[SRCategory("CategoryBehavior")]
		[DefaultValue("")]
		[Localizable(true)]
		[Browsable(false)]
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

		[Browsable(false)]
		[DefaultValue("")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeMapAreaAttributes4")]
		public string MapAreaAttributes
		{
			get
			{
				return this.mapAreaAttributes;
			}
			set
			{
				this.mapAreaAttributes = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[SRCategory("CategoryLabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeMajorTickMarkInt")]
		internal TickMark MajorTickMarkInt
		{
			get
			{
				if (this.majorTickMarkA == null)
				{
					if (this is CircularScale)
					{
						return ((CircularScale)this).MajorTickMark;
					}
					if (this is LinearScale)
					{
						return ((LinearScale)this).MajorTickMark;
					}
				}
				return this.majorTickMarkA;
			}
			set
			{
				this.majorTickMarkA = value;
				this.majorTickMarkA.Parent = this;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMinorTickMarkInt")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryLabelsAndTickMarks")]
		internal TickMark MinorTickMarkInt
		{
			get
			{
				if (this.minorTickMarkA == null)
				{
					if (this is CircularScale)
					{
						return ((CircularScale)this).MinorTickMark;
					}
					if (this is LinearScale)
					{
						return ((LinearScale)this).MinorTickMark;
					}
				}
				return this.minorTickMarkA;
			}
			set
			{
				this.minorTickMarkA = value;
				this.minorTickMarkA.Parent = this;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryLabelsAndTickMarks")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeTickMarksOnTop")]
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

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeReversed")]
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

		[SRDescription("DescriptionAttributeLogarithmic")]
		[DefaultValue(false)]
		[SRCategory("CategoryBehavior")]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionMinLog"));
				}
				this.logarithmic = value;
				this.Invalidate();
			}
		}

		[DefaultValue(10.0)]
		[SRDescription("DescriptionAttributeLogarithmicBase")]
		[SRCategory("CategoryBehavior")]
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
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange_min_open", 1));
				}
				this.logarithmicBase = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRDescription("DescriptionAttributeMinimumPin")]
		internal SpecialPosition MinimumPin
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
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeMaximumPin")]
		internal SpecialPosition MaximumPin
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

		[DefaultValue(true)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeVisible10")]
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

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeWidth9")]
		[ValidateBound(0.0, 30.0)]
		[DefaultValue(5f)]
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[SRDescription("DescriptionAttributeBorderColor6")]
		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAppearance")]
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

		[DefaultValue(GaugeDashStyle.NotSet)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeBorderStyle9")]
		public GaugeDashStyle BorderStyle
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

		[SRCategory("CategoryAppearance")]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeBorderWidth10")]
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillColor5")]
		[DefaultValue(typeof(Color), "CornflowerBlue")]
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

		[DefaultValue(GradientType.None)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientType3")]
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

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientEndColor3")]
		[DefaultValue(typeof(Color), "White")]
		public Color FillGradientEndColor
		{
			get
			{
				return this.fillGradientEndColor;
			}
			set
			{
				this.fillGradientEndColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillHatchStyle5")]
		[DefaultValue(GaugeHatchStyle.None)]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		public GaugeHatchStyle FillHatchStyle
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

		[ValidateBound(-5.0, 5.0)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeShadowOffset3")]
		[DefaultValue(1f)]
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
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

		[DefaultValue(false)]
		[Browsable(false)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeSelected10")]
		public bool Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				this.selected = value;
				this.Invalidate();
			}
		}

		internal float StartPosition
		{
			get
			{
				object parentElement = this.ParentElement;
				if (parentElement is LinearGauge)
				{
					LinearGauge linearGauge = (LinearGauge)parentElement;
					if (linearGauge.GetOrientation() == GaugeOrientation.Vertical)
					{
						if (this.GetReversed())
						{
							return (float)(100.0 - (this._endPosition - this.GetMaxOffset(linearGauge)));
						}
						return (float)(100.0 - this._endPosition);
					}
					if (this.GetReversed())
					{
						return this._startPosition;
					}
					return this._startPosition + this.GetMaxOffset(linearGauge);
				}
				return this._startPosition;
			}
		}

		internal float EndPosition
		{
			get
			{
				object parentElement = this.ParentElement;
				float num = this._endPosition;
				if (parentElement is LinearGauge)
				{
					LinearGauge linearGauge = (LinearGauge)parentElement;
					if (linearGauge.GetOrientation() == GaugeOrientation.Vertical)
					{
						num = (float)((!this.GetReversed()) ? (100.0 - (this._startPosition + this.GetMaxOffset(linearGauge))) : (100.0 - this._startPosition));
					}
					else if (this.GetReversed())
					{
						num = this._endPosition - this.GetMaxOffset(linearGauge);
					}
				}
				if (num < this.StartPosition)
				{
					num = this.StartPosition;
				}
				return num;
			}
		}

		internal float SweepPosition
		{
			get
			{
				return this._sweepPosition;
			}
		}

		object IImageMapProvider.Tag
		{
			get
			{
				return this.imageMapProviderTag;
			}
			set
			{
				this.imageMapProviderTag = value;
			}
		}

		internal ScaleBase()
		{
			this.customLabels = new CustomLabelCollection(this, base.common);
			this.maximumPin = new SpecialPosition(this);
			this.minimumPin = new SpecialPosition(this);
		}

		private float GetMaxOffset(LinearGauge gauge)
		{
			SizeF absoluteSize = gauge.AbsoluteSize;
			if (absoluteSize.IsEmpty)
			{
				return 0f;
			}
			float num = 0f;
			foreach (LinearPointer pointer in gauge.Pointers)
			{
				if (pointer.Type == LinearPointerType.Thermometer)
				{
					float num2 = (gauge.GetOrientation() != GaugeOrientation.Vertical) ? (absoluteSize.Height / absoluteSize.Width) : (absoluteSize.Width / absoluteSize.Height);
					float val = (pointer.ThermometerBulbSize + pointer.ThermometerBulbOffset) * num2;
					num = Math.Max(num, val);
				}
			}
			return num;
		}

		internal GaugeBase GetGauge()
		{
			return (GaugeBase)this.Collection.ParentElement;
		}

		internal bool GetReversed()
		{
			GaugeBase gauge = this.GetGauge();
			if (this.Common != null && this.Common.GaugeContainer != null && this.Common.GaugeContainer.RightToLeft == RightToLeft.Yes)
			{
				if (gauge is CircularGauge)
				{
					return !this.Reversed;
				}
				if (((LinearGauge)gauge).GetOrientation() == GaugeOrientation.Horizontal)
				{
					return !this.Reversed;
				}
			}
			return this.Reversed;
		}

		internal CustomTickMark GetEndLabelTickMark()
		{
			if (this.MinorTickMarkInt.Visible)
			{
				return this.MinorTickMarkInt;
			}
			if (this.MajorTickMarkInt.Visible)
			{
				return this.MajorTickMarkInt;
			}
			return null;
		}

		internal Brush GetLightBrush(GaugeGraphics g, CustomTickMark tickMark, Color fillColor, GraphicsPath path)
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

		internal abstract void DrawTickMark(GaugeGraphics g, CustomTickMark tickMark, double value, float offset);

		internal void DrawTickMark(GaugeGraphics g, CustomTickMark tickMark, double value, float offset, Matrix matrix)
		{
			if (!(tickMark.Width <= 0.0) && !(tickMark.Length <= 0.0))
			{
				float position = this.GetPositionFromValue(value);
				MarkerPosition markerPosition = new MarkerPosition(position, value, tickMark.Placement);
				if (MarkerPosition.IsExistsInArray(this.markers, markerPosition) && !tickMark.GetType().Equals(typeof(CircularSpecialPosition)) && !tickMark.GetType().Equals(typeof(LinearSpecialPosition)))
				{
					return;
				}
				this.markers.Add(markerPosition);
				PointF absolutePoint = g.GetAbsolutePoint(this.GetPoint(position, offset));
				if (tickMark.Image != string.Empty)
				{
					this.DrawTickMarkImage(g, tickMark, matrix, absolutePoint, false);
				}
				else
				{
					SizeF sizeF = new SizeF(g.GetAbsoluteDimension(tickMark.Width), g.GetAbsoluteDimension(tickMark.Length));
					Color rangeTickMarkColor = this.GetRangeTickMarkColor(value, tickMark.FillColor);
					using (GraphicsPath graphicsPath = g.CreateMarker(absolutePoint, sizeF.Width, sizeF.Height, tickMark.Shape))
					{
						using (Brush brush = this.GetLightBrush(g, tickMark, rangeTickMarkColor, graphicsPath))
						{
							graphicsPath.Transform(matrix);
							if (tickMark.EnableGradient && brush is LinearGradientBrush)
							{
								((LinearGradientBrush)brush).Transform = matrix;
							}
							if (this.ShadowOffset != 0.0)
							{
								g.DrawPathShadowAbs(graphicsPath, g.GetShadowColor(), this.ShadowOffset);
							}
							g.FillPath(brush, graphicsPath, 0f, false, false);
							if (tickMark.BorderWidth > 0 && tickMark.BorderStyle != 0)
							{
								using (Pen pen = new Pen(tickMark.BorderColor, (float)tickMark.BorderWidth))
								{
									pen.DashStyle = g.GetPenStyle(tickMark.BorderStyle);
									pen.Alignment = PenAlignment.Outset;
									g.DrawPath(pen, graphicsPath);
								}
							}
						}
					}
				}
			}
		}

		internal void DrawTickMarkImage(GaugeGraphics g, CustomTickMark tickMark, Matrix matrix, PointF centerPoint, bool drawShadow)
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
					colorMatrix.Matrix33 = (float)(this.Common.GaugeCore.ShadowIntensity / 100.0);
					imageAttributes.SetColorMatrix(colorMatrix);
					matrix2.Translate(this.ShadowOffset, this.ShadowOffset, MatrixOrder.Append);
				}
				else if (!tickMark.ImageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(tickMark.ImageHueColor);
					ColorMatrix colorMatrix2 = new ColorMatrix();
					colorMatrix2.Matrix00 = (float)((float)(int)color.R / 255.0);
					colorMatrix2.Matrix11 = (float)((float)(int)color.G / 255.0);
					colorMatrix2.Matrix22 = (float)((float)(int)color.B / 255.0);
					imageAttributes.SetColorMatrix(colorMatrix2);
				}
				destRect.X = (int)(centerPoint.X - (float)(destRect.Width / 2));
				destRect.Y = (int)(centerPoint.Y - (float)(destRect.Height / 2));
				g.Transform = matrix2;
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState.Restore();
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
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionInvalidPlacementType"));
			}
		}

		internal void RenderTicks(GaugeGraphics g, TickMark tickMark, double interval, double max, double min, double intOffset, bool forceLinear)
		{
			float tickMarkOffset = this.GetTickMarkOffset(tickMark);
			double num = min + intOffset;
			while (num <= max)
			{
				this.DrawTickMark(g, tickMark, num, tickMarkOffset);
				try
				{
					num = this.GetNextPosition(num, interval, forceLinear);
				}
				catch (OverflowException)
				{
					return;
				}
			}
		}

		internal void RenderGrid(GaugeGraphics g)
		{
			if (this.MajorTickMarkInt.Visible)
			{
				this.RenderTicks(g, this.MajorTickMarkInt, this.GetInterval(IntervalTypes.Major), this.Maximum, this.MinimumLog, this.GetIntervalOffset(IntervalTypes.Major), false);
			}
			if (this.MinorTickMarkInt.Visible)
			{
				if (!this.Logarithmic)
				{
					this.RenderTicks(g, this.MinorTickMarkInt, this.GetInterval(IntervalTypes.Minor), this.Maximum, this.MinimumLog, this.GetIntervalOffset(IntervalTypes.Minor), false);
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
						this.RenderTicks(g, this.MinorTickMarkInt, nextPosition / num4, Math.Min(nextPosition, this.Maximum), num2, num, true);
						num2 = nextPosition;
						try
						{
							nextPosition = this.GetNextPosition(nextPosition, num3, false);
						}
						catch (OverflowException)
						{
							return;
						}
					}
				}
			}
		}

		internal abstract void DrawCustomLabel(CustomLabel label);

		internal abstract LinearLabelStyle GetLabelStyle();

		internal float GetOffsetLabelPos(Placement placement, float scaleOffset, float scalePosition)
		{
			Gap gap = new Gap(scalePosition);
			gap.SetOffset(Placement.Cross, this.Width);
			gap.SetBase();
			if (this.MajorTickMarkInt.Visible)
			{
				gap.SetOffsetBase(this.MajorTickMarkInt.Placement, this.MajorTickMarkInt.Length);
			}
			if (this.MinorTickMarkInt.Visible)
			{
				gap.SetOffsetBase(this.MinorTickMarkInt.Placement, this.MinorTickMarkInt.Length);
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
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionInvalidPlacementType"));
			}
		}

		internal Font GetResizedFont(Font font, FontUnit fontUnit)
		{
			if (fontUnit == FontUnit.Percent)
			{
				float absoluteDimension = this.Common.Graph.GetAbsoluteDimension(font.Size);
				absoluteDimension = Math.Max(absoluteDimension, 0.001f);
				return new Font(font.FontFamily.Name, absoluteDimension, font.Style, GraphicsUnit.Pixel, font.GdiCharSet, font.GdiVerticalFont);
			}
			return font;
		}

		internal void RenderCustomLabels(GaugeGraphics g)
		{
			foreach (CustomLabel customLabel in this.CustomLabels)
			{
				if (customLabel.Visible)
				{
					this.DrawCustomLabel(customLabel);
				}
			}
		}

		internal abstract void DrawSpecialPosition(GaugeGraphics g, SpecialPosition label, float angle);

		internal void RenderPins(GaugeGraphics g)
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

		internal Color GetRangeTickMarkColor(double value, Color color)
		{
			foreach (RangeBase range in this.GetGauge().GetRanges())
			{
				if (range.InRangeTickMarkColor != Color.Empty && range.ScaleName == this.Name && range.StartValue <= value && range.EndValue >= value)
				{
					return range.InRangeTickMarkColor;
				}
			}
			return color;
		}

		internal Color GetRangeLabelsColor(double value, Color color)
		{
			foreach (RangeBase range in this.GetGauge().GetRanges())
			{
				if (range.InRangeLabelColor != Color.Empty && range.ScaleName == this.Name && range.StartValue <= value && range.EndValue >= value)
				{
					return range.InRangeLabelColor;
				}
			}
			return color;
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
			float position = this.StartPosition - this.MinimumPin.Location;
			float position2 = this.EndPosition + this.MaximumPin.Location;
			if (this.IsReversed())
			{
				position = this.EndPosition + this.MinimumPin.Location;
				position2 = this.StartPosition - this.MaximumPin.Location;
			}
			if (double.IsNaN(value))
			{
				if (this.MinimumPin.Enable)
				{
					return this.GetValueFromPosition(position);
				}
				return this.MinimumLog;
			}
			double num = this.MinimumLog;
			if (this.MinimumPin.Enable)
			{
				num = this.GetValueFromPosition(position);
			}
			double valueFromPosition = this.Maximum;
			if (this.MaximumPin.Enable)
			{
				valueFromPosition = this.GetValueFromPosition(position2);
			}
			if (value < num)
			{
				return num;
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
				num = this.MinorTickMarkInt.IntervalOffset;
				if (double.IsNaN(num))
				{
					num = this.GetIntervalOffset(IntervalTypes.Major) % this.GetInterval(IntervalTypes.Minor);
				}
				break;
			case IntervalTypes.Major:
				num = this.MajorTickMarkInt.IntervalOffset;
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
				num = this.MinorTickMarkInt.Interval;
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
				num = this.MajorTickMarkInt.Interval;
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
						List<double> list = new List<double>();
						bool flag = false;
						while (true)
						{
							if (Math.Round(num3, 0) == num3 && !(num3 > num5))
							{
								break;
							}
							num += num2;
							num3 = (this.Maximum - this.MinimumLog) / num;
							if (num3 <= Math.Max(num5 / 2.0, 1.0))
							{
								if (Math.Round(num3, 0) == num3)
								{
									break;
								}
								list.Add(num);
								if (num3 <= Math.Max(num5 / 3.0, 1.0))
								{
									flag = true;
									break;
								}
							}
						}
						if (flag)
						{
							num = list[0];
						}
					}
					else
					{
						num = 1.0;
					}
				}
				break;
			}
			if (!this.Logarithmic)
			{
				while ((this.Maximum - this.MinimumLog) / num > 1000.0)
				{
					num *= 10.0;
				}
			}
			return num;
		}

		internal double GetNextPosition(double position, double interval, bool forceLinear)
		{
			if (forceLinear || !this.Logarithmic)
			{
				interval = double.Parse(interval.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
				position += interval;
				position = double.Parse(position.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
			}
			else
			{
				position = Math.Pow(this.LogarithmicBase, Math.Log(position, this.LogarithmicBase) + interval);
			}
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

		protected virtual bool IsReversed()
		{
			return this.GetReversed();
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

		internal abstract double GetValue(PointF c, PointF p);

		protected abstract PointF GetPoint(float position, float offset);

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
			throw new ApplicationException(Utils.SRGetStr("ExceptionGdiNonInitialized"));
		}

		internal virtual void PointerValueChanged(PointerBase sender)
		{
			bool playbackMode;
			if (this.Common != null && !double.IsNaN(sender.Data.OldValue))
			{
				playbackMode = false;
				if (((IValueConsumer)sender.Data).GetProvider() != null)
				{
					playbackMode = ((IValueConsumer)sender.Data).GetProvider().GetPlayBackMode();
				}
				if (sender.Data.OldValue >= this.minimum && sender.Data.Value < this.minimum)
				{
					goto IL_0088;
				}
				if (sender.Data.OldValue <= this.maximum && sender.Data.Value > this.maximum)
				{
					goto IL_0088;
				}
				goto IL_00bc;
			}
			return;
			IL_00bc:
			if (!(sender.Data.OldValue < this.minimum) || !(sender.Data.Value >= this.minimum))
			{
				if (!(sender.Data.OldValue > this.maximum))
				{
					return;
				}
				if (!(sender.Data.Value <= this.maximum))
				{
					return;
				}
			}
			this.Common.GaugeContainer.OnValueScaleEnter(this, new ValueRangeEventArgs(sender.Data.Value, sender.Data.DateValueStamp, this.Name, playbackMode, this));
			return;
			IL_0088:
			this.Common.GaugeContainer.OnValueScaleLeave(this, new ValueRangeEventArgs(sender.Data.Value, sender.Data.DateValueStamp, this.Name, playbackMode, sender));
			goto IL_00bc;
		}

		internal override void OnAdded()
		{
			base.OnAdded();
			this.CustomLabels.Common = this.Common;
			this.CustomLabels.parent = this;
			this.Maximum = this.Maximum;
			this.Minimum = this.Minimum;
		}

		internal override void OnRemove()
		{
			base.OnRemove();
			this.CustomLabels.Common = null;
			this.CustomLabels.parent = null;
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			this.CustomLabels.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			this.CustomLabels.EndInit();
		}

		internal override void Invalidate()
		{
			if (this.Common != null)
			{
				this.Common.GaugeCore.Notify(MessageType.DataInvalidated, this, false);
			}
			base.Invalidate();
		}

		string IToolTipProvider.GetToolTip(HitTestResult ht)
		{
			if (this.Common != null && this.Common.GaugeCore != null)
			{
				string original = this.Common.GaugeCore.ResolveAllKeywords(this.ToolTip, this);
				return this.Common.GaugeCore.ResolveKeyword(original, "#VALUE", ht.ScaleValue);
			}
			return this.ToolTip;
		}

		string IImageMapProvider.GetToolTip()
		{
			if (this.Common != null && this.Common.GaugeCore != null)
			{
				return this.Common.GaugeCore.ResolveAllKeywords(this.ToolTip, this);
			}
			return this.ToolTip;
		}

		string IImageMapProvider.GetHref()
		{
			if (this.Common != null && this.Common.GaugeCore != null)
			{
				return this.Common.GaugeCore.ResolveAllKeywords(this.Href, this);
			}
			return this.Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			if (this.Common != null && this.Common.GaugeCore != null)
			{
				return this.Common.GaugeCore.ResolveAllKeywords(this.MapAreaAttributes, this);
			}
			return this.MapAreaAttributes;
		}
	}
}
