using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(LinearPointerConverter))]
	internal class LinearPointer : PointerBase, ISelectable
	{
		private LinearPointerType type;

		private Placement placement = Placement.Outside;

		private float thermometerBulbOffset = 5f;

		private float thermometerBulbSize = 50f;

		private Color thermometerBackColor = Color.Empty;

		private GradientType thermometerBackGradientType;

		private Color thermometerBackGradientEndColor = Color.Empty;

		private GaugeHatchStyle thermometerBackHatchStyle;

		private ThermometerStyle thermometerStyle;

		private bool selected;

		private GraphicsPath[] hotRegions = new GraphicsPath[2];

		[SRDescription("DescriptionAttributeLinearPointer_Type")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(LinearPointerType.Marker)]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryTypeSpecific")]
		public LinearPointerType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributePlacement")]
		[SRCategory("CategoryLayout")]
		[DefaultValue(Placement.Outside)]
		public Placement Placement
		{
			get
			{
				return this.placement;
			}
			set
			{
				this.placement = value;
				this.Invalidate();
			}
		}

		[ValidateBound(0.0, 100.0)]
		[DefaultValue(20f)]
		[SRDescription("DescriptionAttributeWidth")]
		[SRCategory("CategoryLayout")]
		public override float Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				if (!(value < 0.0) && !(value > 200.0))
				{
					base.Width = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 200));
			}
		}

		[DefaultValue(MarkerStyle.Triangle)]
		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeMarkerStyle4")]
		public override MarkerStyle MarkerStyle
		{
			get
			{
				return base.MarkerStyle;
			}
			set
			{
				base.MarkerStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMarkerLength4")]
		[SRCategory("CategoryTypeSpecific")]
		[DefaultValue(20f)]
		[ValidateBound(0.0, 100.0)]
		public override float MarkerLength
		{
			get
			{
				return base.MarkerLength;
			}
			set
			{
				if (!(value < 0.0) && !(value > 200.0))
				{
					base.MarkerLength = value;
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 200));
			}
		}

		[DefaultValue(GradientType.DiagonalLeft)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeFillGradientType6")]
		public override GradientType FillGradientType
		{
			get
			{
				return base.FillGradientType;
			}
			set
			{
				base.FillGradientType = value;
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[DefaultValue(5f)]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBulbOffset")]
		[ValidateBound(0.0, 30.0)]
		public float ThermometerBulbOffset
		{
			get
			{
				return this.thermometerBulbOffset;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					this.thermometerBulbOffset = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[DefaultValue(50f)]
		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBulbSize")]
		[ValidateBound(0.0, 100.0)]
		public float ThermometerBulbSize
		{
			get
			{
				return this.thermometerBulbSize;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					this.thermometerBulbSize = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
			}
		}

		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBackColor")]
		[SRCategory("CategoryTypeSpecific")]
		[DefaultValue(typeof(Color), "Empty")]
		public Color ThermometerBackColor
		{
			get
			{
				return this.thermometerBackColor;
			}
			set
			{
				this.thermometerBackColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBackGradientType")]
		public GradientType ThermometerBackGradientType
		{
			get
			{
				return this.thermometerBackGradientType;
			}
			set
			{
				this.thermometerBackGradientType = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Empty")]
		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBackGradientEndColor")]
		public Color ThermometerBackGradientEndColor
		{
			get
			{
				return this.thermometerBackGradientEndColor;
			}
			set
			{
				this.thermometerBackGradientEndColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLinearPointer_ThermometerBackHatchStyle")]
		[SRCategory("CategoryTypeSpecific")]
		[DefaultValue(GaugeHatchStyle.None)]
		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		public GaugeHatchStyle ThermometerBackHatchStyle
		{
			get
			{
				return this.thermometerBackHatchStyle;
			}
			set
			{
				this.thermometerBackHatchStyle = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryTypeSpecific")]
		[DefaultValue(ThermometerStyle.Standard)]
		[SRDescription("DescriptionAttributeLinearPointer_ThermometerStyle")]
		public ThermometerStyle ThermometerStyle
		{
			get
			{
				return this.thermometerStyle;
			}
			set
			{
				this.thermometerStyle = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeCursor")]
		[DefaultValue(GaugeCursor.Default)]
		[Browsable(false)]
		public override GaugeCursor Cursor
		{
			get
			{
				return base.Cursor;
			}
			set
			{
				base.Cursor = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeDampeningEnabled3")]
		[DefaultValue(false)]
		[Browsable(false)]
		public override bool DampeningEnabled
		{
			get
			{
				return base.DampeningEnabled;
			}
			set
			{
				base.DampeningEnabled = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeSelected")]
		[DefaultValue(false)]
		[Browsable(false)]
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

		public LinearPointer()
			: base(MarkerStyle.Triangle, 20f, 20f, GradientType.DiagonalLeft)
		{
		}

		internal override void Render(GaugeGraphics g)
		{
			if (this.Common != null && this.Visible && this.GetScale() != null)
			{
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRendering(this.Name));
				g.StartHotRegion(this);
				if (this.Image != "")
				{
					this.DrawImage(g, false);
					this.SetAllHotRegions(g);
					g.EndHotRegion();
					this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(this.Name));
				}
				else
				{
					Pen pen = new Pen(base.BorderColor, (float)base.BorderWidth);
					pen.DashStyle = g.GetPenStyle(base.BorderStyle);
					if (pen.DashStyle != 0)
					{
						pen.Alignment = PenAlignment.Center;
					}
					if (this.Type == LinearPointerType.Bar)
					{
						BarStyleAttrib barStyleAttrib = this.GetBarStyleAttrib(g);
						try
						{
							if (barStyleAttrib.primaryPath != null)
							{
								g.FillPath(barStyleAttrib.primaryBrush, barStyleAttrib.primaryPath);
							}
							if (barStyleAttrib.secondaryPaths != null)
							{
								int num = 0;
								GraphicsPath[] secondaryPaths = barStyleAttrib.secondaryPaths;
								foreach (GraphicsPath graphicsPath in secondaryPaths)
								{
									if (graphicsPath != null && barStyleAttrib.secondaryBrushes[num] != null)
									{
										g.FillPath(barStyleAttrib.secondaryBrushes[num], graphicsPath);
									}
									num++;
								}
							}
							if (base.BorderWidth > 0 && barStyleAttrib.primaryBrush != null && base.BorderStyle != 0)
							{
								g.DrawPath(pen, barStyleAttrib.primaryPath);
							}
						}
						catch (Exception)
						{
							barStyleAttrib.Dispose();
						}
						if (barStyleAttrib.primaryPath != null)
						{
							this.AddHotRegion(barStyleAttrib.primaryPath, true);
						}
					}
					else if (this.Type == LinearPointerType.Thermometer)
					{
						BarStyleAttrib thermometerStyleAttrib = this.GetThermometerStyleAttrib(g);
						try
						{
							if (thermometerStyleAttrib.totalPath != null)
							{
								g.FillPath(thermometerStyleAttrib.totalBrush, thermometerStyleAttrib.totalPath);
							}
							if (thermometerStyleAttrib.primaryPath != null)
							{
								g.FillPath(thermometerStyleAttrib.primaryBrush, thermometerStyleAttrib.primaryPath);
							}
							if (thermometerStyleAttrib.secondaryPaths != null)
							{
								int num2 = 0;
								GraphicsPath[] secondaryPaths2 = thermometerStyleAttrib.secondaryPaths;
								foreach (GraphicsPath graphicsPath2 in secondaryPaths2)
								{
									if (graphicsPath2 != null && thermometerStyleAttrib.secondaryBrushes[num2] != null)
									{
										g.FillPath(thermometerStyleAttrib.secondaryBrushes[num2], graphicsPath2);
									}
									num2++;
								}
							}
							if (base.BorderWidth > 0 && thermometerStyleAttrib.primaryBrush != null && base.BorderStyle != 0)
							{
								g.DrawPath(pen, thermometerStyleAttrib.totalPath);
							}
						}
						catch (Exception)
						{
							thermometerStyleAttrib.Dispose();
						}
						if (thermometerStyleAttrib.primaryPath != null)
						{
							this.AddHotRegion(thermometerStyleAttrib.primaryPath, true);
						}
					}
					else
					{
						MarkerStyleAttrib markerStyleAttrib = this.GetMarkerStyleAttrib(g);
						try
						{
							if (markerStyleAttrib.path != null)
							{
								bool circularFill = (byte)((this.MarkerStyle == MarkerStyle.Circle) ? 1 : 0) != 0;
								g.FillPath(markerStyleAttrib.brush, markerStyleAttrib.path, 0f, true, circularFill);
							}
							if (base.BorderWidth > 0 && markerStyleAttrib.path != null && base.BorderStyle != 0)
							{
								g.DrawPath(pen, markerStyleAttrib.path);
							}
						}
						catch (Exception)
						{
							markerStyleAttrib.Dispose();
						}
						if (markerStyleAttrib.path != null)
						{
							this.AddHotRegion(markerStyleAttrib.path, true);
						}
					}
					this.SetAllHotRegions(g);
					g.EndHotRegion();
					this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(this.Name));
				}
			}
		}

		internal BarStyleAttrib GetBarStyleAttrib(GaugeGraphics g)
		{
			BarStyleAttrib barStyleAttrib = new BarStyleAttrib();
			if (this.Image != "")
			{
				return barStyleAttrib;
			}
			LinearScale scale = this.GetScale();
			double num = scale.GetValueLimit(this.GetBarStartValue());
			if ((this.Type == LinearPointerType.Thermometer || this.BarStart == BarStart.ScaleStart) && num > scale.Minimum)
			{
				num = scale.Minimum;
			}
			double valueLimit = scale.GetValueLimit(base.Position);
			float positionFromValue = scale.GetPositionFromValue(num);
			float thermometerBulbOffset2 = this.ThermometerBulbOffset;
			float positionFromValue2 = scale.GetPositionFromValue(valueLimit);
			float num2 = positionFromValue2 - positionFromValue;
			if (Math.Round((double)num2, 4) == 0.0)
			{
				return barStyleAttrib;
			}
			if (base.BarStyle == BarStyle.Style1)
			{
				barStyleAttrib.primaryPath = g.GetLinearRangePath(positionFromValue, positionFromValue2, this.Width, this.Width, scale.Position, this.GetGauge().GetOrientation(), this.DistanceFromScale, this.Placement, scale.Width);
				if (barStyleAttrib.primaryPath == null)
				{
					return barStyleAttrib;
				}
				barStyleAttrib.primaryBrush = g.GetLinearRangeBrush(barStyleAttrib.primaryPath.GetBounds(), this.FillColor, this.FillHatchStyle, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), this.FillGradientType.ToString()), this.FillGradientEndColor, this.GetGauge().GetOrientation(), this.GetScale().GetReversed(), 0.0, 0.0);
				LinearRange[] colorRanges = this.GetColorRanges();
				if (colorRanges != null)
				{
					barStyleAttrib.secondaryPaths = new GraphicsPath[colorRanges.Length];
					barStyleAttrib.secondaryBrushes = new Brush[colorRanges.Length];
					int num3 = 0;
					LinearRange[] array = colorRanges;
					foreach (LinearRange linearRange in array)
					{
						double num4 = scale.GetValueLimit(linearRange.StartValue);
						if (num4 < num)
						{
							num4 = num;
						}
						if (num4 > valueLimit)
						{
							num4 = valueLimit;
						}
						double num5 = scale.GetValueLimit(linearRange.EndValue);
						if (num5 < num)
						{
							num5 = num;
						}
						if (num5 > valueLimit)
						{
							num5 = valueLimit;
						}
						float positionFromValue3 = scale.GetPositionFromValue(num4);
						float positionFromValue4 = scale.GetPositionFromValue(num5);
						float num6 = positionFromValue4 - positionFromValue3;
						if (Math.Round((double)num6, 4) == 0.0)
						{
							barStyleAttrib.secondaryPaths[num3] = null;
							barStyleAttrib.secondaryBrushes[num3] = null;
						}
						else
						{
							barStyleAttrib.secondaryPaths[num3] = g.GetLinearRangePath(positionFromValue3, positionFromValue4, this.Width, this.Width, scale.Position, this.GetGauge().GetOrientation(), this.DistanceFromScale, this.Placement, scale.Width);
							barStyleAttrib.secondaryBrushes[num3] = g.GetLinearRangeBrush(barStyleAttrib.primaryPath.GetBounds(), linearRange.InRangeBarPointerColor, this.FillHatchStyle, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), this.FillGradientType.ToString()), this.FillGradientEndColor, this.GetGauge().GetOrientation(), this.GetScale().GetReversed(), 0.0, 0.0);
						}
						num3++;
					}
				}
			}
			return barStyleAttrib;
		}

		internal BarStyleAttrib GetThermometerStyleAttrib(GaugeGraphics g)
		{
			BarStyleAttrib barStyleAttrib = new BarStyleAttrib();
			if (this.Image != "")
			{
				return barStyleAttrib;
			}
			LinearScale scale = this.GetScale();
			double num = scale.GetValueLimit(this.GetBarStartValue());
			if ((this.Type == LinearPointerType.Thermometer || this.BarStart == BarStart.ScaleStart) && num > scale.MinimumLog)
			{
				num = scale.MinimumLog;
			}
			double valueLimit = scale.GetValueLimit(base.Position);
			float positionFromValue = scale.GetPositionFromValue(num);
			float positionFromValue2 = scale.GetPositionFromValue(valueLimit);
			float num2 = positionFromValue2 - positionFromValue;
			float width = this.Width;
			float bulbSize = this.ThermometerBulbSize;
			float bulbOffset = this.thermometerBulbOffset;
			float distanceFromScale = this.DistanceFromScale;
			if (Math.Round((double)num2, 4) == 0.0 && this.Type != LinearPointerType.Thermometer)
			{
				return barStyleAttrib;
			}
			double num3 = scale.GetValueLimit(double.PositiveInfinity);
			if (num3 < scale.Maximum)
			{
				num3 = scale.Maximum;
			}
			float positionFromValue3 = scale.GetPositionFromValue(num3);
			barStyleAttrib.primaryPath = g.GetThermometerPath(positionFromValue, positionFromValue2, width, scale.Position, this.GetGauge().GetOrientation(), distanceFromScale, this.Placement, scale.GetReversed(), scale.Width, bulbOffset, bulbSize, this.ThermometerStyle);
			if (barStyleAttrib.primaryPath == null)
			{
				return barStyleAttrib;
			}
			barStyleAttrib.totalPath = g.GetThermometerPath(positionFromValue, positionFromValue3, this.Width, scale.Position, this.GetGauge().GetOrientation(), this.DistanceFromScale, this.Placement, scale.GetReversed(), scale.Width, this.ThermometerBulbOffset, this.ThermometerBulbSize, this.ThermometerStyle);
			barStyleAttrib.totalBrush = g.GetLinearRangeBrush(barStyleAttrib.totalPath.GetBounds(), this.ThermometerBackColor, this.ThermometerBackHatchStyle, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), this.ThermometerBackGradientType.ToString()), this.ThermometerBackGradientEndColor, this.GetGauge().GetOrientation(), this.GetScale().GetReversed(), 0.0, 0.0);
			barStyleAttrib.primaryBrush = g.GetLinearRangeBrush(barStyleAttrib.primaryPath.GetBounds(), this.FillColor, this.FillHatchStyle, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), this.FillGradientType.ToString()), this.FillGradientEndColor, this.GetGauge().GetOrientation(), this.GetScale().GetReversed(), 0.0, 0.0);
			LinearRange[] colorRanges = this.GetColorRanges();
			if (colorRanges != null)
			{
				barStyleAttrib.secondaryPaths = new GraphicsPath[colorRanges.Length];
				barStyleAttrib.secondaryBrushes = new Brush[colorRanges.Length];
				int num4 = 0;
				LinearRange[] array = colorRanges;
				foreach (LinearRange linearRange in array)
				{
					double num5 = scale.GetValueLimit(linearRange.StartValue);
					if (num5 < num)
					{
						num5 = num;
					}
					if (num5 > valueLimit)
					{
						num5 = valueLimit;
					}
					double num6 = scale.GetValueLimit(linearRange.EndValue);
					if (num6 < num)
					{
						num6 = num;
					}
					if (num6 > valueLimit)
					{
						num6 = valueLimit;
					}
					float positionFromValue4 = scale.GetPositionFromValue(num5);
					float positionFromValue5 = scale.GetPositionFromValue(num6);
					float num7 = positionFromValue5 - positionFromValue4;
					if (Math.Round((double)num7, 4) == 0.0)
					{
						barStyleAttrib.secondaryPaths[num4] = null;
						barStyleAttrib.secondaryBrushes[num4] = null;
					}
					else
					{
						barStyleAttrib.secondaryPaths[num4] = g.GetLinearRangePath(positionFromValue4, positionFromValue5, width, width, scale.Position, this.GetGauge().GetOrientation(), distanceFromScale, this.Placement, scale.Width);
						barStyleAttrib.secondaryBrushes[num4] = g.GetLinearRangeBrush(barStyleAttrib.primaryPath.GetBounds(), linearRange.InRangeBarPointerColor, this.FillHatchStyle, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), this.FillGradientType.ToString()), this.FillGradientEndColor, this.GetGauge().GetOrientation(), this.GetScale().GetReversed(), 0.0, 0.0);
					}
					num4++;
				}
			}
			return barStyleAttrib;
		}

		private LinearRange[] GetColorRanges()
		{
			LinearGauge gauge = this.GetGauge();
			LinearScale scale = this.GetScale();
			if (gauge != null && scale != null)
			{
				double barStartValue = this.GetBarStartValue();
				double position = base.Position;
				ArrayList arrayList = null;
				foreach (LinearRange range in gauge.Ranges)
				{
					if (range.GetScale() == scale && range.InRangeBarPointerColor != Color.Empty)
					{
						double valueLimit = scale.GetValueLimit(range.StartValue);
						double valueLimit2 = scale.GetValueLimit(range.EndValue);
						if (barStartValue >= valueLimit && barStartValue <= valueLimit2)
						{
							goto IL_00aa;
						}
						if (position >= valueLimit && position <= valueLimit2)
						{
							goto IL_00aa;
						}
						if (valueLimit >= barStartValue && valueLimit <= position)
						{
							goto IL_00aa;
						}
						if (valueLimit2 >= barStartValue && valueLimit2 <= position)
						{
							goto IL_00aa;
						}
					}
					continue;
					IL_00aa:
					if (arrayList == null)
					{
						arrayList = new ArrayList();
					}
					arrayList.Add(range);
				}
				if (arrayList == null)
				{
					return null;
				}
				return (LinearRange[])arrayList.ToArray(typeof(LinearRange));
			}
			return null;
		}

		private double GetBarStartValue()
		{
			LinearScale scale = this.GetScale();
			if (this.Type == LinearPointerType.Thermometer)
			{
				return double.NegativeInfinity;
			}
			if (this.BarStart == BarStart.ScaleStart)
			{
				return double.NegativeInfinity;
			}
			if (scale.Logarithmic)
			{
				return 1.0;
			}
			return 0.0;
		}

		internal void DrawImage(GaugeGraphics g, bool drawShadow)
		{
			if (this.Visible)
			{
				if (drawShadow && base.ShadowOffset == 0.0)
				{
					return;
				}
				Image image = this.Common.ImageLoader.LoadImage(this.Image);
				if (image.Width != 0 && image.Height != 0)
				{
					Point point = new Point(this.ImageOrigin.X, this.ImageOrigin.Y);
					if (point.X == 0)
					{
						point.X = image.Width / 2;
					}
					if (point.Y == 0)
					{
						point.Y = image.Height / 2;
					}
					float absoluteDimension = g.GetAbsoluteDimension(this.Width);
					float absoluteDimension2 = g.GetAbsoluteDimension(this.MarkerLength);
					float num = absoluteDimension / (float)image.Width;
					float num2 = absoluteDimension2 / (float)image.Height;
					float num3 = this.CalculateMarkerDistance();
					float positionFromValue = this.GetScale().GetPositionFromValue(base.Position);
					PointF pointF = Point.Empty;
					pointF = ((this.GetGauge().GetOrientation() != 0) ? g.GetAbsolutePoint(new PointF(num3, positionFromValue)) : g.GetAbsolutePoint(new PointF(positionFromValue, num3)));
					Matrix transform = g.Transform;
					Matrix matrix = g.Transform.Clone();
					Rectangle rectangle = new Rectangle((int)(pointF.X - (float)point.X * num) + 1, (int)(pointF.Y - (float)point.Y * num2) + 1, (int)((float)image.Width * num) + 1, (int)((float)image.Height * num2) + 1);
					ImageAttributes imageAttributes = new ImageAttributes();
					if (this.ImageTransColor != Color.Empty)
					{
						imageAttributes.SetColorKey(this.ImageTransColor, this.ImageTransColor, ColorAdjustType.Default);
					}
					if (drawShadow)
					{
						ColorMatrix colorMatrix = new ColorMatrix();
						colorMatrix.Matrix00 = 0f;
						colorMatrix.Matrix11 = 0f;
						colorMatrix.Matrix22 = 0f;
						colorMatrix.Matrix33 = (float)(this.Common.GaugeCore.ShadowIntensity / 100.0);
						imageAttributes.SetColorMatrix(colorMatrix);
						matrix.Translate(base.ShadowOffset, base.ShadowOffset, MatrixOrder.Append);
					}
					else
					{
						ColorMatrix colorMatrix2 = new ColorMatrix();
						if (!this.ImageHueColor.IsEmpty)
						{
							Color color = g.TransformHueColor(this.ImageHueColor);
							colorMatrix2.Matrix00 = (float)((float)(int)color.R / 255.0);
							colorMatrix2.Matrix11 = (float)((float)(int)color.G / 255.0);
							colorMatrix2.Matrix22 = (float)((float)(int)color.B / 255.0);
						}
						colorMatrix2.Matrix33 = (float)(1.0 - this.ImageTransparency / 100.0);
						imageAttributes.SetColorMatrix(colorMatrix2);
					}
					g.Transform = matrix;
					g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
					g.Transform = transform;
					if (!drawShadow)
					{
						GraphicsPath graphicsPath = new GraphicsPath();
						graphicsPath.AddRectangle(rectangle);
						this.AddHotRegion(graphicsPath, true);
					}
				}
			}
		}

		internal MarkerStyleAttrib GetMarkerStyleAttrib(GaugeGraphics g)
		{
			MarkerStyleAttrib markerStyleAttrib = new MarkerStyleAttrib();
			float absoluteDimension = g.GetAbsoluteDimension(this.MarkerLength);
			float absoluteDimension2 = g.GetAbsoluteDimension(this.Width);
			markerStyleAttrib.path = g.CreateMarker(new PointF(0f, 0f), absoluteDimension2, absoluteDimension, this.MarkerStyle);
			float num = 0f;
			if (this.Placement == Placement.Cross || this.Placement == Placement.Inside)
			{
				num = (float)(num + 180.0);
			}
			if (this.GetGauge().GetOrientation() == GaugeOrientation.Vertical)
			{
				num = (float)(num + 270.0);
			}
			if (num > 0.0)
			{
				using (Matrix matrix = new Matrix())
				{
					matrix.Rotate(num);
					markerStyleAttrib.path.Transform(matrix);
				}
			}
			float num2 = this.CalculateMarkerDistance();
			ScaleBase scale = this.GetScale();
			float positionFromValue = scale.GetPositionFromValue(scale.GetValueLimit(base.Position));
			PointF pointOrigin = Point.Empty;
			pointOrigin = ((this.GetGauge().GetOrientation() != 0) ? g.GetAbsolutePoint(new PointF(num2, positionFromValue)) : g.GetAbsolutePoint(new PointF(positionFromValue, num2)));
			markerStyleAttrib.brush = g.GetMarkerBrush(markerStyleAttrib.path, this.MarkerStyle, pointOrigin, 0f, this.FillColor, this.FillGradientType, this.FillGradientEndColor, this.FillHatchStyle);
			using (Matrix matrix2 = new Matrix())
			{
				matrix2.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				markerStyleAttrib.path.Transform(matrix2);
				return markerStyleAttrib;
			}
		}

		internal float CalculateMarkerDistance()
		{
			if (this.Placement == Placement.Cross)
			{
				return this.GetScale().Position - this.DistanceFromScale;
			}
			if (this.Placement == Placement.Inside)
			{
				return (float)(this.GetScale().Position - this.GetScale().Width / 2.0 - this.DistanceFromScale - this.MarkerLength / 2.0);
			}
			return (float)(this.GetScale().Position + this.GetScale().Width / 2.0 + this.DistanceFromScale + this.MarkerLength / 2.0);
		}

		internal GraphicsPath GetPointerPath(GaugeGraphics g)
		{
			if (!this.Visible)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			ScaleBase scale = this.GetScale();
			scale.GetPositionFromValue(scale.GetValueLimit(base.Position));
			if (this.Type == LinearPointerType.Marker || this.Image != string.Empty)
			{
				MarkerStyleAttrib markerStyleAttrib = this.GetMarkerStyleAttrib(g);
				if (markerStyleAttrib.path != null)
				{
					graphicsPath.AddPath(markerStyleAttrib.path, false);
				}
			}
			else if (this.Type == LinearPointerType.Bar)
			{
				BarStyleAttrib barStyleAttrib = this.GetBarStyleAttrib(g);
				if (barStyleAttrib.primaryPath == null)
				{
					graphicsPath.Dispose();
					return null;
				}
				if (barStyleAttrib.primaryPath != null)
				{
					graphicsPath.AddPath(barStyleAttrib.primaryPath, false);
				}
			}
			else if (this.Type == LinearPointerType.Thermometer)
			{
				BarStyleAttrib thermometerStyleAttrib = this.GetThermometerStyleAttrib(g);
				if (thermometerStyleAttrib.primaryPath == null)
				{
					graphicsPath.Dispose();
					return null;
				}
				if (thermometerStyleAttrib.totalPath != null)
				{
					graphicsPath.AddPath(thermometerStyleAttrib.primaryPath, false);
				}
			}
			return graphicsPath;
		}

		internal GraphicsPath GetShadowPath(GaugeGraphics g)
		{
			if (base.ShadowOffset != 0.0 && this.GetScale() != null)
			{
				if (this.Image != "")
				{
					this.DrawImage(g, true);
					return null;
				}
				GraphicsPath pointerPath = this.GetPointerPath(g);
				if (pointerPath != null && pointerPath.PointCount != 0)
				{
					using (Matrix matrix = new Matrix())
					{
						matrix.Translate(base.ShadowOffset, base.ShadowOffset);
						pointerPath.Transform(matrix);
						return pointerPath;
					}
				}
				return null;
			}
			return null;
		}

		internal void AddHotRegion(GraphicsPath path, bool primary)
		{
			if (primary)
			{
				this.hotRegions[0] = path;
			}
			else
			{
				this.hotRegions[1] = path;
			}
		}

		internal void SetAllHotRegions(GaugeGraphics g)
		{
			this.Common.GaugeCore.HotRegionList.SetHotRegion(this, PointF.Empty, this.hotRegions[0], this.hotRegions[1]);
			this.hotRegions[0] = null;
			this.hotRegions[1] = null;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public LinearGauge GetGauge()
		{
			if (this.Collection == null)
			{
				return null;
			}
			return (LinearGauge)this.Collection.parent;
		}

		public LinearScale GetScale()
		{
			return (LinearScale)base.GetScaleBase();
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			if (this.GetGauge() != null && this.GetScale() != null)
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
				using (GraphicsPath graphicsPath = this.GetPointerPath(g))
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
			LinearPointer linearPointer = new LinearPointer();
			binaryFormatSerializer.Deserialize(linearPointer, stream);
			return linearPointer;
		}
	}
}
