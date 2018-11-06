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
	[TypeConverter(typeof(CircularPointerConverter))]
	internal class CircularPointer : PointerBase, ISelectable
	{
		private XamlRenderer xamlRenderer;

		private CircularPointerType type;

		private NeedleStyle needleStyle;

		private bool capVisible = true;

		private bool capOnTop = true;

		private bool capReflection;

		private float capWidth = 26f;

		private string capImage = "";

		private Color capImageTransColor = Color.Empty;

		private Color capImageHueColor = Color.Empty;

		private Point capImageOrigin = Point.Empty;

		private Placement placement = Placement.Cross;

		private CapStyle capStyle;

		private Color capFillColor = Color.Gainsboro;

		private GradientType capFillGradientType = GradientType.DiagonalLeft;

		private Color capFillGradientEndColor = Color.DimGray;

		private GaugeHatchStyle capFillHatchStyle;

		private bool selected;

		private GraphicsPath[] hotRegions = new GraphicsPath[2];

		[SRCategory("CategoryTypeSpecific")]
		[DefaultValue(CircularPointerType.Needle)]
		[SRDescription("DescriptionAttributeCircularPointer_Type")]
		[ParenthesizePropertyName(true)]
		[RefreshProperties(RefreshProperties.All)]
		public CircularPointerType Type
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

		[DefaultValue(NeedleStyle.Style1)]
		[SRCategory("CategoryTypeSpecific")]
		[SRDescription("DescriptionAttributeCircularPointer_NeedleStyle")]
		public NeedleStyle NeedleStyle
		{
			get
			{
				return this.needleStyle;
			}
			set
			{
				this.needleStyle = value;
				this.Invalidate();
			}
		}

		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeCircularPointer_CapVisible")]
		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryPointerCap")]
		public bool CapVisible
		{
			get
			{
				return this.capVisible;
			}
			set
			{
				this.capVisible = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeCircularPointer_CapOnTop")]
		[SRCategory("CategoryPointerCap")]
		[DefaultValue(true)]
		public bool CapOnTop
		{
			get
			{
				return this.capOnTop;
			}
			set
			{
				this.capOnTop = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapReflection")]
		[DefaultValue(false)]
		public bool CapReflection
		{
			get
			{
				return this.capReflection;
			}
			set
			{
				this.capReflection = value;
				this.Invalidate();
			}
		}

		[ValidateBound(0.0, 100.0)]
		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapWidth")]
		[DefaultValue(26f)]
		public float CapWidth
		{
			get
			{
				return this.capWidth;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					this.capWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeCircularPointer_CapImage")]
		[DefaultValue("")]
		public string CapImage
		{
			get
			{
				return this.capImage;
			}
			set
			{
				this.capImage = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeCircularPointer_CapImageTransColor")]
		public Color CapImageTransColor
		{
			get
			{
				return this.capImageTransColor;
			}
			set
			{
				this.capImageTransColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeCapImageHueColor")]
		public Color CapImageHueColor
		{
			get
			{
				return this.capImageHueColor;
			}
			set
			{
				this.capImageHueColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeCircularPointer_CapImageOrigin")]
		[SRCategory("CategoryImage")]
		[DefaultValue(typeof(Point), "0, 0")]
		[TypeConverter(typeof(EmptyPointConverter))]
		public Point CapImageOrigin
		{
			get
			{
				return this.capImageOrigin;
			}
			set
			{
				this.capImageOrigin = value;
				this.Invalidate();
			}
		}

		[DefaultValue(Placement.Cross)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributePlacement")]
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

		[SRCategory("CategoryLayout")]
		[DefaultValue(15f)]
		[SRDescription("DescriptionAttributeWidth")]
		[ValidateBound(0.0, 30.0)]
		public override float Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					base.Width = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
			}
		}

		[DefaultValue(MarkerStyle.Diamond)]
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
		[ValidateBound(0.0, 50.0)]
		[SRCategory("CategoryTypeSpecific")]
		[DefaultValue(10f)]
		public override float MarkerLength
		{
			get
			{
				return base.MarkerLength;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					base.MarkerLength = value;
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
			}
		}

		[SRDescription("DescriptionAttributeFillGradientType6")]
		[DefaultValue(GradientType.LeftRight)]
		[SRCategory("CategoryAppearance")]
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

		[ParenthesizePropertyName(true)]
		[SRDescription("DescriptionAttributeCircularPointer_CapStyle")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(CapStyle.Simple)]
		[SRCategory("CategoryPointerCap")]
		public CapStyle CapStyle
		{
			get
			{
				return this.capStyle;
			}
			set
			{
				this.capStyle = value;
				this.ResetCachedXamlRenderer();
				this.Invalidate();
			}
		}

		[SRCategory("CategoryPointerCap")]
		[DefaultValue(typeof(Color), "Gainsboro")]
		[SRDescription("DescriptionAttributeCircularPointer_CapFillColor")]
		public Color CapFillColor
		{
			get
			{
				return this.capFillColor;
			}
			set
			{
				this.capFillColor = value;
				this.ResetCachedXamlRenderer();
				this.Invalidate();
			}
		}

		[SRCategory("CategoryPointerCap")]
		[SRDescription("DescriptionAttributeCircularPointer_CapFillGradientType")]
		[DefaultValue(GradientType.DiagonalLeft)]
		public GradientType CapFillGradientType
		{
			get
			{
				return this.capFillGradientType;
			}
			set
			{
				this.capFillGradientType = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeCircularPointer_CapFillGradientEndColor")]
		[DefaultValue(typeof(Color), "DimGray")]
		[SRCategory("CategoryPointerCap")]
		public Color CapFillGradientEndColor
		{
			get
			{
				return this.capFillGradientEndColor;
			}
			set
			{
				this.capFillGradientEndColor = value;
				this.Invalidate();
			}
		}

		[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[SRCategory("CategoryPointerCap")]
		[DefaultValue(GaugeHatchStyle.None)]
		[SRDescription("DescriptionAttributeCircularPointer_CapFillHatchStyle")]
		public GaugeHatchStyle CapFillHatchStyle
		{
			get
			{
				return this.capFillHatchStyle;
			}
			set
			{
				this.capFillHatchStyle = value;
				this.Invalidate();
			}
		}

		[DefaultValue(GaugeCursor.Default)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeCursor")]
		[Browsable(false)]
		[SRCategory("CategoryBehavior")]
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

		[SRCategory("CategoryAppearance")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeSelected")]
		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool NeedleCapVisible
		{
			get
			{
				return this.CapVisible;
			}
			set
			{
				this.CapVisible = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public bool NeedleCapOnTop
		{
			get
			{
				return this.CapOnTop;
			}
			set
			{
				this.CapOnTop = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public float NeedleCapWidth
		{
			get
			{
				return this.CapWidth;
			}
			set
			{
				this.CapWidth = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string NeedleCapImage
		{
			get
			{
				return this.CapImage;
			}
			set
			{
				this.CapImage = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Color NeedleCapImageTransColor
		{
			get
			{
				return this.CapImageTransColor;
			}
			set
			{
				this.CapImageTransColor = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public Point NeedleCapImageOrigin
		{
			get
			{
				return this.CapImageOrigin;
			}
			set
			{
				this.CapImageOrigin = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Color NeedleCapFillColor
		{
			get
			{
				return this.CapFillColor;
			}
			set
			{
				this.CapFillColor = value;
			}
		}

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public GradientType NeedleCapFillGradientType
		{
			get
			{
				return this.CapFillGradientType;
			}
			set
			{
				this.CapFillGradientType = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public Color NeedleCapFillGradientEndColor
		{
			get
			{
				return this.CapFillGradientEndColor;
			}
			set
			{
				this.CapFillGradientEndColor = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public GaugeHatchStyle NeedleCapFillHatchStyle
		{
			get
			{
				return this.CapFillHatchStyle;
			}
			set
			{
				this.CapFillHatchStyle = value;
			}
		}

		public CircularPointer()
			: base(MarkerStyle.Diamond, 10f, 15f, GradientType.LeftRight)
		{
		}

		internal Brush GetNeedleFillBrush(GaugeGraphics g, bool primary, GraphicsPath path, PointF pointOrigin, float angle)
		{
			Brush brush = null;
			if (primary)
			{
				if (this.FillHatchStyle != 0)
				{
					brush = GaugeGraphics.GetHatchBrush(this.FillHatchStyle, this.FillColor, this.FillGradientEndColor);
				}
				else if (this.FillGradientType != 0)
				{
					brush = g.GetGradientBrush(path.GetBounds(), this.FillColor, this.FillGradientEndColor, this.FillGradientType);
					if (brush is LinearGradientBrush)
					{
						((LinearGradientBrush)brush).RotateTransform(angle, MatrixOrder.Append);
						((LinearGradientBrush)brush).TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
					}
					else if (brush is PathGradientBrush)
					{
						((PathGradientBrush)brush).RotateTransform(angle, MatrixOrder.Append);
						((PathGradientBrush)brush).TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
					}
				}
				else
				{
					brush = new SolidBrush(this.FillColor);
				}
			}
			else if (this.CapFillHatchStyle != 0)
			{
				brush = GaugeGraphics.GetHatchBrush(this.CapFillHatchStyle, this.CapFillColor, this.CapFillGradientEndColor);
			}
			else if (this.CapFillGradientType != 0)
			{
				RectangleF bounds = path.GetBounds();
				if (this.CapFillGradientType == GradientType.DiagonalLeft)
				{
					brush = g.GetGradientBrush(bounds, this.CapFillColor, this.CapFillGradientEndColor, GradientType.LeftRight);
					using (Matrix matrix = new Matrix())
					{
						matrix.RotateAt(45f, new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0)));
						((LinearGradientBrush)brush).Transform = matrix;
					}
				}
				else if (this.CapFillGradientType == GradientType.DiagonalRight)
				{
					brush = g.GetGradientBrush(bounds, this.CapFillColor, this.CapFillGradientEndColor, GradientType.TopBottom);
					using (Matrix matrix2 = new Matrix())
					{
						matrix2.RotateAt(135f, new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0)));
						((LinearGradientBrush)brush).Transform = matrix2;
					}
				}
				else if (this.CapFillGradientType == GradientType.Center)
				{
					bounds.Inflate(1f, 1f);
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddArc(bounds, 0f, 360f);
						PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
						pathGradientBrush.CenterColor = this.CapFillColor;
						pathGradientBrush.CenterPoint = new PointF((float)(bounds.X + bounds.Width / 2.0), (float)(bounds.Y + bounds.Height / 2.0));
						pathGradientBrush.SurroundColors = new Color[1]
						{
							this.CapFillGradientEndColor
						};
						brush = pathGradientBrush;
					}
				}
				else
				{
					brush = g.GetGradientBrush(path.GetBounds(), this.CapFillColor, this.CapFillGradientEndColor, this.CapFillGradientType);
				}
				if (brush is LinearGradientBrush)
				{
					((LinearGradientBrush)brush).RotateTransform(angle, MatrixOrder.Append);
					((LinearGradientBrush)brush).TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
				else if (brush is PathGradientBrush)
				{
					((PathGradientBrush)brush).RotateTransform(angle, MatrixOrder.Append);
					((PathGradientBrush)brush).TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
			}
			else
			{
				brush = new SolidBrush(this.CapFillColor);
			}
			return brush;
		}

		internal NeedleStyleAttrib GetNeedleStyleAttrib(GaugeGraphics g, PointF pointOrigin, float angle)
		{
			NeedleStyleAttrib needleStyleAttrib = new NeedleStyleAttrib();
			needleStyleAttrib.primaryPath = new GraphicsPath();
			if (this.CapVisible && this.CapWidth > 0.0)
			{
				needleStyleAttrib.secondaryPath = new GraphicsPath();
			}
			if (needleStyleAttrib.primaryPath == null && needleStyleAttrib.secondaryPath == null)
			{
				return needleStyleAttrib;
			}
			float relative = (float)((this.Placement != Placement.Cross) ? ((this.Placement != 0) ? (this.GetScale().GetRadius() + this.GetScale().Width / 2.0 + this.DistanceFromScale) : (this.GetScale().GetRadius() - this.GetScale().Width / 2.0 - this.DistanceFromScale)) : (this.GetScale().GetRadius() - this.DistanceFromScale));
			relative = g.GetAbsoluteDimension(relative);
			float width = this.Width;
			width = g.GetAbsoluteDimension(width);
			float relative2 = this.CapWidth;
			relative2 = g.GetAbsoluteDimension(relative2);
			if (needleStyleAttrib.primaryPath != null)
			{
				PointF[] points = new PointF[0];
				switch (this.NeedleStyle)
				{
				case NeedleStyle.Style1:
					points = new PointF[3]
					{
						new PointF((float)((0.0 - width) / 2.0), (float)((0.0 - width) / 2.0)),
						new PointF((float)(width / 2.0), (float)((0.0 - width) / 2.0)),
						new PointF(0f, relative)
					};
					break;
				case NeedleStyle.Style2:
				{
					float num14 = (float)(relative / 1.6180340051651);
					num14 = (float)(num14 * 1.6180340051651 - num14);
					points = new PointF[4]
					{
						new PointF((float)((0.0 - width) / 2.0), (float)(0.0 - num14)),
						new PointF((float)(width / 2.0), (float)(0.0 - num14)),
						new PointF((float)(width / 2.0), relative),
						new PointF((float)((0.0 - width) / 2.0), relative)
					};
					break;
				}
				case NeedleStyle.Style3:
				{
					float num13 = (float)(relative / 1.6180340051651);
					num13 = (float)(num13 * 1.6180340051651 - num13);
					points = new PointF[5]
					{
						new PointF((float)((0.0 - width) / 2.0), (float)(0.0 - num13)),
						new PointF((float)(width / 2.0), (float)(0.0 - num13)),
						new PointF((float)(width / 4.0), relative - width),
						new PointF(0f, relative),
						new PointF((float)((0.0 - width) / 4.0), relative - width)
					};
					break;
				}
				case NeedleStyle.Style4:
					points = new PointF[5]
					{
						new PointF((float)((0.0 - width) / 2.0), 0f),
						new PointF((float)(width / 2.0), 0f),
						new PointF((float)(width / 3.0), relative - width),
						new PointF(0f, relative),
						new PointF((float)((0.0 - width) / 3.0), relative - width)
					};
					break;
				case NeedleStyle.Style5:
				{
					float num11 = (float)(relative / 1.6180340051651);
					num11 = (float)(num11 * 1.6180340051651 - num11);
					float num12 = (float)(width / 1.6180340051651);
					points = new PointF[7]
					{
						new PointF((float)((0.0 - num12) / 2.0), (float)(0.0 - num11)),
						new PointF((float)(num12 / 2.0), (float)(0.0 - num11)),
						new PointF((float)(num12 / 2.0), relative - width),
						new PointF((float)(width / 2.0), relative - width),
						new PointF(0f, relative),
						new PointF((float)((0.0 - width) / 2.0), relative - width),
						new PointF((float)((0.0 - num12) / 2.0), relative - width)
					};
					break;
				}
				case NeedleStyle.Style6:
				{
					float num9 = (float)(relative / 1.6180340051651);
					num9 = (float)(num9 * 1.6180340051651 - num9);
					float num10 = (float)(width / 1.6180340051651);
					points = new PointF[7]
					{
						new PointF((float)((0.0 - num10) / 2.0), 0f),
						new PointF((float)(num10 / 2.0), 0f),
						new PointF((float)(num10 / 2.0), relative - width),
						new PointF((float)(width / 2.0), relative - width),
						new PointF(0f, relative),
						new PointF((float)((0.0 - width) / 2.0), relative - width),
						new PointF((float)((0.0 - num10) / 2.0), relative - width)
					};
					break;
				}
				case NeedleStyle.Style7:
				{
					float num7 = (float)(relative / 1.6180340051651);
					num7 = (float)(num7 * 1.6180340051651 - num7);
					float num8 = (float)(width / 1.6180340051651);
					points = new PointF[7]
					{
						new PointF((float)((0.0 - num8) / 2.0), (float)(0.0 - num7)),
						new PointF((float)(num8 / 2.0), (float)(0.0 - num7)),
						new PointF((float)(num8 / 2.0), relative - width),
						new PointF((float)(width / 2.0), (float)(relative - width - width / 8.0)),
						new PointF(0f, relative),
						new PointF((float)((0.0 - width) / 2.0), (float)(relative - width - width / 8.0)),
						new PointF((float)((0.0 - num8) / 2.0), relative - width)
					};
					break;
				}
				case NeedleStyle.Style8:
				{
					float num6 = (float)(width / 1.6180340051651);
					points = new PointF[7]
					{
						new PointF((float)((0.0 - num6) / 2.0), 0f),
						new PointF((float)(num6 / 2.0), 0f),
						new PointF((float)(num6 / 2.0), relative - width),
						new PointF((float)(width / 2.0), (float)(relative - width - width / 8.0)),
						new PointF(0f, relative),
						new PointF((float)((0.0 - width) / 2.0), (float)(relative - width - width / 8.0)),
						new PointF((float)((0.0 - num6) / 2.0), relative - width)
					};
					break;
				}
				case NeedleStyle.Style9:
				{
					float num4 = (float)(relative / 1.6180340051651);
					num4 = (float)(num4 * 1.6180340051651 - num4);
					float num5 = (float)(width / 1.6180340051651);
					points = new PointF[8]
					{
						new PointF((float)((0.0 - width) / 2.0), (float)(0.0 - num4)),
						new PointF(0f, (float)(0.0 - num4 + num5 / 2.0)),
						new PointF((float)(width / 2.0), (float)(0.0 - num4)),
						new PointF((float)(num5 / 2.0), relative - width),
						new PointF((float)(width / 2.0), (float)(relative - width - width / 8.0)),
						new PointF(0f, relative),
						new PointF((float)((0.0 - width) / 2.0), (float)(relative - width - width / 8.0)),
						new PointF((float)((0.0 - num5) / 2.0), relative - width)
					};
					break;
				}
				case NeedleStyle.Style10:
				{
					float num2 = (float)(relative / 1.6180340051651);
					num2 = (float)(num2 * 1.6180340051651 - num2);
					float num3 = (float)(width / 1.6180340051651);
					points = new PointF[12]
					{
						new PointF((float)((0.0 - width) / 2.0), (float)(0.0 - num2)),
						new PointF(0f, (float)(0.0 - num2 + num3)),
						new PointF((float)(width / 2.0), (float)(0.0 - num2)),
						new PointF((float)(width / 2.0), (float)(0.0 - num2 + num3 * 1.6180340051651)),
						new PointF((float)(num3 / 2.0), (float)(0.0 - num2 + num3 * 2.5)),
						new PointF((float)(num3 / 2.0), relative - width),
						new PointF((float)(width / 2.0), (float)(relative - width - width / 8.0)),
						new PointF(0f, relative),
						new PointF((float)((0.0 - width) / 2.0), (float)(relative - width - width / 8.0)),
						new PointF((float)((0.0 - num3) / 2.0), relative - width),
						new PointF((float)((0.0 - num3) / 2.0), (float)(0.0 - num2 + num3 * 2.5)),
						new PointF((float)((0.0 - width) / 2.0), (float)(0.0 - num2 + num3 * 1.6180340051651))
					};
					break;
				}
				case NeedleStyle.Style11:
				{
					float num = (float)(width / 4.0);
					needleStyleAttrib.primaryPath.AddLine((float)((0.0 - width) / 2.0), (float)((0.0 - width) / 2.0), (float)(width / 2.0), (float)((0.0 - width) / 2.0));
					needleStyleAttrib.primaryPath.AddLine((float)(width / 2.0), (float)((0.0 - width) / 2.0), num, relative - num);
					needleStyleAttrib.primaryPath.AddArc((float)(0.0 - num), (float)(relative - num * 2.0), (float)(num * 2.0), (float)(num * 2.0), 0f, 180f);
					needleStyleAttrib.primaryPath.AddLine((float)(0.0 - num), relative - num, (float)((0.0 - width) / 2.0), (float)((0.0 - width) / 2.0));
					break;
				}
				default:
					throw new ArgumentException(SR.NotImplemented(this.NeedleStyle.ToString()));
				}
				if (this.NeedleStyle != NeedleStyle.Style11)
				{
					needleStyleAttrib.primaryPath.AddLines(points);
				}
				needleStyleAttrib.primaryPath.CloseFigure();
				needleStyleAttrib.primaryBrush = this.GetNeedleFillBrush(g, true, needleStyleAttrib.primaryPath, pointOrigin, angle);
			}
			if (needleStyleAttrib.secondaryPath != null)
			{
				needleStyleAttrib.secondaryPath.AddEllipse((float)((0.0 - relative2) / 2.0), (float)((0.0 - relative2) / 2.0), relative2, relative2);
				needleStyleAttrib.secondaryBrush = this.GetNeedleFillBrush(g, false, needleStyleAttrib.secondaryPath, pointOrigin, 0f);
				if (this.CapReflection)
				{
					needleStyleAttrib.reflectionPaths = new GraphicsPath[2];
					needleStyleAttrib.reflectionBrushes = new Brush[2];
					g.GetCircularEdgeReflection(needleStyleAttrib.secondaryPath.GetBounds(), 135f, 200, pointOrigin, out needleStyleAttrib.reflectionPaths[0], out needleStyleAttrib.reflectionBrushes[0]);
					g.GetCircularEdgeReflection(needleStyleAttrib.secondaryPath.GetBounds(), 315f, 128, pointOrigin, out needleStyleAttrib.reflectionPaths[1], out needleStyleAttrib.reflectionBrushes[1]);
				}
			}
			using (Matrix matrix = new Matrix())
			{
				matrix.Rotate(angle, MatrixOrder.Append);
				matrix.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				if (needleStyleAttrib.primaryPath != null)
				{
					needleStyleAttrib.primaryPath.Transform(matrix);
				}
				if (needleStyleAttrib.secondaryPath != null)
				{
					needleStyleAttrib.secondaryPath.Transform(matrix);
					return needleStyleAttrib;
				}
				return needleStyleAttrib;
			}
		}

		internal MarkerStyleAttrib GetMarkerStyleAttrib(GaugeGraphics g)
		{
			MarkerStyleAttrib markerStyleAttrib = new MarkerStyleAttrib();
			if (this.Image != "")
			{
				return markerStyleAttrib;
			}
			float positionFromValue = this.GetScale().GetPositionFromValue(base.Position);
			PointF absolutePoint = g.GetAbsolutePoint(this.GetScale().GetPivotPoint());
			float absoluteDimension = g.GetAbsoluteDimension(this.MarkerLength);
			float absoluteDimension2 = g.GetAbsoluteDimension(this.Width);
			float relative = this.CalculateMarkerDistance();
			relative = g.GetAbsoluteDimension(relative);
			PointF point = new PointF(0f, relative);
			markerStyleAttrib.path = g.CreateMarker(point, absoluteDimension2, absoluteDimension, this.MarkerStyle);
			markerStyleAttrib.brush = g.GetMarkerBrush(markerStyleAttrib.path, this.MarkerStyle, absolutePoint, positionFromValue, this.FillColor, this.FillGradientType, this.FillGradientEndColor, this.FillHatchStyle);
			using (Matrix matrix = new Matrix())
			{
				if (this.Placement == Placement.Inside)
				{
					matrix.RotateAt(180f, point, MatrixOrder.Append);
				}
				matrix.Rotate(positionFromValue, MatrixOrder.Append);
				matrix.Translate(absolutePoint.X, absolutePoint.Y, MatrixOrder.Append);
				markerStyleAttrib.path.Transform(matrix);
				return markerStyleAttrib;
			}
		}

		internal float CalculateMarkerDistance()
		{
			if (this.Placement == Placement.Cross)
			{
				return this.GetScale().GetRadius() - this.DistanceFromScale;
			}
			if (this.Placement == Placement.Inside)
			{
				return (float)(this.GetScale().GetRadius() - this.GetScale().Width / 2.0 - this.DistanceFromScale - this.MarkerLength / 2.0);
			}
			return (float)(this.GetScale().GetRadius() + this.GetScale().Width / 2.0 + this.DistanceFromScale + this.MarkerLength / 2.0);
		}

		internal BarStyleAttrib GetBarStyleAttrib(GaugeGraphics g)
		{
			BarStyleAttrib barStyleAttrib = new BarStyleAttrib();
			if (this.Image != "")
			{
				return barStyleAttrib;
			}
			CircularScale scale = this.GetScale();
			double valueLimit = scale.GetValueLimit(this.GetBarStartValue());
			double valueLimit2 = scale.GetValueLimit(base.Position);
			float positionFromValue = scale.GetPositionFromValue(valueLimit);
			float positionFromValue2 = scale.GetPositionFromValue(valueLimit2);
			float num = positionFromValue2 - positionFromValue;
			if (Math.Round((double)num, 4) == 0.0)
			{
				return barStyleAttrib;
			}
			if (base.BarStyle == BarStyle.Style1)
			{
				RectangleF rectangleF = this.CalculateBarRectangle();
				barStyleAttrib.primaryPath = g.GetCircularRangePath(rectangleF, (float)(positionFromValue + 90.0), num, this.Width, this.Width, this.Placement);
				if (barStyleAttrib.primaryPath == null)
				{
					return barStyleAttrib;
				}
				RectangleF rect = rectangleF;
				if (this.Placement != 0)
				{
					if (this.Placement == Placement.Outside)
					{
						rect.Inflate(this.Width, this.Width);
					}
					else
					{
						rect.Inflate((float)(this.Width / 2.0), (float)(this.Width / 2.0));
					}
				}
				barStyleAttrib.primaryBrush = g.GetCircularRangeBrush(rect, (float)(positionFromValue + 90.0), num, this.FillColor, this.FillHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.TopLeft, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), this.FillGradientType.ToString()), this.FillGradientEndColor);
				CircularRange[] colorRanges = this.GetColorRanges();
				if (colorRanges != null)
				{
					barStyleAttrib.secondaryPaths = new GraphicsPath[colorRanges.Length];
					barStyleAttrib.secondaryBrushes = new Brush[colorRanges.Length];
					int num2 = 0;
					CircularRange[] array = colorRanges;
					foreach (CircularRange circularRange in array)
					{
						double num3 = scale.GetValueLimit(circularRange.StartValue);
						if (num3 < valueLimit)
						{
							num3 = valueLimit;
						}
						if (num3 > valueLimit2)
						{
							num3 = valueLimit2;
						}
						double num4 = scale.GetValueLimit(circularRange.EndValue);
						if (num4 < valueLimit)
						{
							num4 = valueLimit;
						}
						if (num4 > valueLimit2)
						{
							num4 = valueLimit2;
						}
						float positionFromValue3 = scale.GetPositionFromValue(num3);
						float positionFromValue4 = scale.GetPositionFromValue(num4);
						float num5 = positionFromValue4 - positionFromValue3;
						if (Math.Round((double)num5, 4) == 0.0)
						{
							barStyleAttrib.secondaryPaths[num2] = null;
							barStyleAttrib.secondaryBrushes[num2] = null;
						}
						else
						{
							barStyleAttrib.secondaryPaths[num2] = g.GetCircularRangePath(rectangleF, (float)(positionFromValue3 + 90.0), num5, this.Width, this.Width, this.Placement);
							barStyleAttrib.secondaryBrushes[num2] = g.GetCircularRangeBrush(rectangleF, (float)(positionFromValue3 + 90.0), num5, circularRange.InRangeBarPointerColor, this.FillHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.TopLeft, (RangeGradientType)Enum.Parse(typeof(RangeGradientType), this.FillGradientType.ToString()), this.FillGradientEndColor);
						}
						num2++;
					}
				}
			}
			return barStyleAttrib;
		}

		private CircularRange[] GetColorRanges()
		{
			CircularGauge gauge = this.GetGauge();
			CircularScale scale = this.GetScale();
			if (gauge != null && scale != null)
			{
				double barStartValue = this.GetBarStartValue();
				double position = base.Position;
				ArrayList arrayList = null;
				foreach (CircularRange range in gauge.Ranges)
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
				return (CircularRange[])arrayList.ToArray(typeof(CircularRange));
			}
			return null;
		}

		private double GetBarStartValue()
		{
			CircularScale scale = this.GetScale();
			if (this.BarStart == BarStart.ScaleStart)
			{
				return scale.MinimumLog;
			}
			if (scale.Logarithmic)
			{
				return 1.0;
			}
			return 0.0;
		}

		internal RectangleF CalculateBarRectangle()
		{
			CircularScale scale = this.GetScale();
			PointF pivotPoint = this.GetScale().GetPivotPoint();
			float radius = scale.GetRadius();
			RectangleF result = new RectangleF(pivotPoint.X - radius, pivotPoint.Y - radius, (float)(radius * 2.0), (float)(radius * 2.0));
			if (this.Placement == Placement.Inside)
			{
				result.Inflate((float)(0.0 - this.DistanceFromScale), (float)(0.0 - this.DistanceFromScale));
				result.Inflate((float)((0.0 - scale.Width) / 2.0), (float)((0.0 - scale.Width) / 2.0));
			}
			else if (this.Placement == Placement.Outside)
			{
				result.Inflate(this.DistanceFromScale, this.DistanceFromScale);
				result.Inflate((float)(scale.Width / 2.0), (float)(scale.Width / 2.0));
			}
			else
			{
				result.Inflate((float)(0.0 - this.DistanceFromScale), (float)(0.0 - this.DistanceFromScale));
			}
			return result;
		}

		internal RectangleF GetNeedleCapBounds(GaugeGraphics g, PointF pointOrigin)
		{
			float absoluteDimension = g.GetAbsoluteDimension(this.CapWidth);
			RectangleF result = new RectangleF((float)((0.0 - absoluteDimension) / 2.0), (float)((0.0 - absoluteDimension) / 2.0), absoluteDimension, absoluteDimension);
			result.Offset(pointOrigin);
			return result;
		}

		internal override void Render(GaugeGraphics g)
		{
			if (this.Common != null && this.Visible && this.GetScale() != null)
			{
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRendering(this.Name));
				g.StartHotRegion(this);
				this.GetScale().SetDrawRegion(g);
				if (this.Image != "" && this.CapImage != "")
				{
					if (this.CapOnTop)
					{
						this.DrawImage(g, true, false);
						if (this.Type == CircularPointerType.Needle && this.CapVisible)
						{
							this.DrawImage(g, false, false);
						}
					}
					else
					{
						if (this.Type == CircularPointerType.Needle && this.CapVisible)
						{
							this.DrawImage(g, false, false);
						}
						this.DrawImage(g, true, false);
					}
					this.SetAllHotRegions(g);
					g.RestoreDrawRegion();
					g.EndHotRegion();
					this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(this.Name));
				}
				else
				{
					if (this.Image != "" && this.CapOnTop)
					{
						this.DrawImage(g, true, false);
					}
					if (this.CapImage != "" && !this.CapOnTop && this.Type == CircularPointerType.Needle && this.CapVisible)
					{
						this.DrawImage(g, false, false);
					}
					float positionFromValue = this.GetScale().GetPositionFromValue(base.Position);
					PointF absolutePoint = g.GetAbsolutePoint(this.GetScale().GetPivotPoint());
					Pen pen = new Pen(base.BorderColor, (float)base.BorderWidth);
					pen.DashStyle = g.GetPenStyle(base.BorderStyle);
					if (pen.DashStyle != 0)
					{
						pen.Alignment = PenAlignment.Center;
					}
					if (this.Type == CircularPointerType.Needle)
					{
						NeedleStyleAttrib needleStyleAttrib = this.GetNeedleStyleAttrib(g, absolutePoint, positionFromValue);
						try
						{
							if (this.CapOnTop)
							{
								if (needleStyleAttrib.primaryPath != null && this.Image == string.Empty)
								{
									g.FillPath(needleStyleAttrib.primaryBrush, needleStyleAttrib.primaryPath, positionFromValue, true, false);
									if (base.BorderWidth > 0 && base.BorderStyle != 0)
									{
										g.DrawPath(pen, needleStyleAttrib.primaryPath);
									}
								}
								if (needleStyleAttrib.secondaryPath != null && this.CapImage == string.Empty)
								{
									if (this.CapStyle == CapStyle.Simple)
									{
										g.FillPath(needleStyleAttrib.secondaryBrush, needleStyleAttrib.secondaryPath, 0f, true, true);
									}
									else
									{
										XamlRenderer cachedXamlRenderer = this.GetCachedXamlRenderer(this.GetNeedleCapBounds(g, absolutePoint));
										XamlLayer[] layers = cachedXamlRenderer.Layers;
										foreach (XamlLayer xamlLayer in layers)
										{
											xamlLayer.Render(g);
										}
									}
									if (needleStyleAttrib.reflectionPaths != null)
									{
										for (int j = 0; j < needleStyleAttrib.reflectionPaths.Length; j++)
										{
											if (needleStyleAttrib.reflectionPaths[j] != null)
											{
												g.FillPath(needleStyleAttrib.reflectionBrushes[j], needleStyleAttrib.reflectionPaths[j]);
											}
										}
									}
									if (base.BorderWidth > 0 && base.BorderStyle != 0)
									{
										g.DrawPath(pen, needleStyleAttrib.secondaryPath);
									}
								}
							}
							else
							{
								if (needleStyleAttrib.secondaryPath != null && this.CapImage == string.Empty)
								{
									if (this.CapStyle == CapStyle.Simple)
									{
										g.FillPath(needleStyleAttrib.secondaryBrush, needleStyleAttrib.secondaryPath, 0f, true, true);
									}
									else
									{
										XamlRenderer cachedXamlRenderer2 = this.GetCachedXamlRenderer(this.GetNeedleCapBounds(g, absolutePoint));
										XamlLayer[] layers2 = cachedXamlRenderer2.Layers;
										foreach (XamlLayer xamlLayer2 in layers2)
										{
											xamlLayer2.Render(g);
										}
									}
									if (base.BorderWidth > 0 && base.BorderStyle != 0)
									{
										g.DrawPath(pen, needleStyleAttrib.secondaryPath);
									}
								}
								if (needleStyleAttrib.primaryPath != null && this.Image == string.Empty)
								{
									g.FillPath(needleStyleAttrib.primaryBrush, needleStyleAttrib.primaryPath, positionFromValue, true, false);
									if (base.BorderWidth > 0 && base.BorderStyle != 0)
									{
										g.DrawPath(pen, needleStyleAttrib.primaryPath);
									}
								}
							}
							if (needleStyleAttrib.primaryPath != null && this.Image == string.Empty)
							{
								this.AddHotRegion((GraphicsPath)needleStyleAttrib.primaryPath.Clone(), true);
							}
							if (needleStyleAttrib.secondaryPath != null && this.CapImage == string.Empty)
							{
								this.AddHotRegion((GraphicsPath)needleStyleAttrib.secondaryPath.Clone(), false);
							}
						}
						finally
						{
							needleStyleAttrib.Dispose();
						}
					}
					else if (this.Type == CircularPointerType.Bar)
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
							if (base.BorderWidth > 0 && barStyleAttrib.primaryPath != null && base.BorderStyle != 0)
							{
								g.DrawPath(pen, barStyleAttrib.primaryPath);
							}
							if (barStyleAttrib.primaryPath != null)
							{
								this.AddHotRegion((GraphicsPath)barStyleAttrib.primaryPath.Clone(), true);
							}
						}
						finally
						{
							barStyleAttrib.Dispose();
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
								g.FillPath(markerStyleAttrib.brush, markerStyleAttrib.path, positionFromValue, true, circularFill);
							}
							if (base.BorderWidth > 0 && markerStyleAttrib.path != null && base.BorderStyle != 0)
							{
								g.DrawPath(pen, markerStyleAttrib.path);
							}
							if (markerStyleAttrib.path != null)
							{
								this.AddHotRegion((GraphicsPath)markerStyleAttrib.path.Clone(), true);
							}
						}
						finally
						{
							markerStyleAttrib.Dispose();
						}
					}
					if (this.Image != "" && !this.CapOnTop)
					{
						this.DrawImage(g, true, false);
					}
					if (this.CapImage != "" && this.CapOnTop && this.Type == CircularPointerType.Needle && this.CapVisible)
					{
						this.DrawImage(g, false, false);
					}
					this.SetAllHotRegions(g);
					g.RestoreDrawRegion();
					g.EndHotRegion();
					this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(this.Name));
				}
			}
		}

		internal void DrawImage(GaugeGraphics g, bool primary, bool drawShadow)
		{
			if (this.Visible)
			{
				if (drawShadow && base.ShadowOffset == 0.0)
				{
					return;
				}
				float relative = (float)((this.Placement != Placement.Cross) ? ((this.Placement != 0) ? (this.GetScale().GetRadius() + this.GetScale().Width / 2.0 + this.DistanceFromScale) : (this.GetScale().GetRadius() - this.GetScale().Width / 2.0 - this.DistanceFromScale)) : (this.GetScale().GetRadius() - this.DistanceFromScale));
				relative = g.GetAbsoluteDimension(relative);
				Image image = null;
				image = ((!primary) ? this.Common.ImageLoader.LoadImage(this.CapImage) : this.Common.ImageLoader.LoadImage(this.Image));
				if (image.Width != 0 && image.Height != 0)
				{
					Point point = Point.Empty;
					if (primary)
					{
						point = this.ImageOrigin;
						if (point.X == 0)
						{
							point.X = image.Width / 2;
						}
					}
					else
					{
						point = this.CapImageOrigin;
						if (point.IsEmpty)
						{
							point.X = image.Width / 2;
							point.Y = image.Height / 2;
						}
					}
					int num = (!primary) ? ((image.Height <= image.Width) ? image.Width : image.Height) : (image.Height - point.Y);
					if (num != 0)
					{
						float num2 = (!primary) ? (g.GetAbsoluteDimension(this.CapWidth) / (float)num) : (relative / (float)num);
						Rectangle rectangle = new Rectangle(0, 0, (int)((float)image.Width * num2), (int)((float)image.Height * num2));
						ImageAttributes imageAttributes = new ImageAttributes();
						if (primary && this.ImageTransColor != Color.Empty)
						{
							imageAttributes.SetColorKey(this.ImageTransColor, this.ImageTransColor, ColorAdjustType.Default);
						}
						if (!primary && this.CapImageTransColor != Color.Empty)
						{
							imageAttributes.SetColorKey(this.CapImageTransColor, this.CapImageTransColor, ColorAdjustType.Default);
						}
						Matrix transform = g.Transform;
						Matrix matrix = g.Transform.Clone();
						float positionFromValue = this.GetScale().GetPositionFromValue(base.Position);
						PointF absolutePoint = g.GetAbsolutePoint(this.GetScale().GetPivotPoint());
						PointF pointF = new PointF((float)point.X * num2, (float)point.Y * num2);
						float offsetX = matrix.OffsetX;
						float offsetY = matrix.OffsetY;
						matrix.Translate(absolutePoint.X - pointF.X, absolutePoint.Y - pointF.Y, MatrixOrder.Append);
						absolutePoint.X += offsetX;
						absolutePoint.Y += offsetY;
						if (primary)
						{
							matrix.RotateAt(positionFromValue, absolutePoint, MatrixOrder.Append);
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
							if (primary && !this.ImageHueColor.IsEmpty)
							{
								Color color = g.TransformHueColor(this.ImageHueColor);
								colorMatrix2.Matrix00 = (float)((float)(int)color.R / 255.0);
								colorMatrix2.Matrix11 = (float)((float)(int)color.G / 255.0);
								colorMatrix2.Matrix22 = (float)((float)(int)color.B / 255.0);
							}
							else if (!primary && !this.CapImageHueColor.IsEmpty)
							{
								Color color2 = g.TransformHueColor(this.CapImageHueColor);
								colorMatrix2.Matrix00 = (float)((float)(int)color2.R / 255.0);
								colorMatrix2.Matrix11 = (float)((float)(int)color2.G / 255.0);
								colorMatrix2.Matrix22 = (float)((float)(int)color2.B / 255.0);
							}
							if (primary)
							{
								colorMatrix2.Matrix33 = (float)(1.0 - this.ImageTransparency / 100.0);
							}
							imageAttributes.SetColorMatrix(colorMatrix2);
						}
						g.Transform = matrix;
						g.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
						g.Transform = transform;
						if (!drawShadow)
						{
							matrix.Translate((float)(0.0 - offsetX), (float)(0.0 - offsetY), MatrixOrder.Append);
							GraphicsPath graphicsPath = new GraphicsPath();
							graphicsPath.AddRectangle(rectangle);
							graphicsPath.Transform(matrix);
							this.AddHotRegion(graphicsPath, primary);
						}
					}
				}
			}
		}

		internal GraphicsPath GetPointerPath(GaugeGraphics g, bool shadowPath)
		{
			if (!this.Visible)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			float positionFromValue = this.GetScale().GetPositionFromValue(base.Position);
			PointF absolutePoint = g.GetAbsolutePoint(this.GetScale().GetPivotPoint());
			if (this.Type == CircularPointerType.Needle || this.Image != string.Empty)
			{
				NeedleStyleAttrib needleStyleAttrib = this.GetNeedleStyleAttrib(g, absolutePoint, positionFromValue);
				if (needleStyleAttrib.primaryPath != null && (this.Image == string.Empty || !shadowPath))
				{
					graphicsPath.AddPath(needleStyleAttrib.primaryPath, false);
				}
				if (needleStyleAttrib.secondaryPath != null && this.CapVisible && this.Type == CircularPointerType.Needle && (this.CapImage == string.Empty || !shadowPath))
				{
					graphicsPath.AddPath(needleStyleAttrib.secondaryPath, false);
				}
			}
			else if (this.Type == CircularPointerType.Bar)
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
			else if (this.Type == CircularPointerType.Marker)
			{
				MarkerStyleAttrib markerStyleAttrib = this.GetMarkerStyleAttrib(g);
				if (markerStyleAttrib.path != null)
				{
					graphicsPath.AddPath(markerStyleAttrib.path, false);
				}
			}
			return graphicsPath;
		}

		internal GraphicsPath GetShadowPath(GaugeGraphics g)
		{
			if (base.ShadowOffset != 0.0 && this.GetScale() != null)
			{
				this.GetScale().SetDrawRegion(g);
				if (this.CapOnTop)
				{
					if (this.Image != "")
					{
						this.DrawImage(g, true, true);
					}
					if (this.CapImage != "" && this.Type == CircularPointerType.Needle && this.CapVisible)
					{
						this.DrawImage(g, false, true);
					}
				}
				else
				{
					if (this.CapImage != "" && this.Type == CircularPointerType.Needle && this.CapVisible)
					{
						this.DrawImage(g, false, true);
					}
					if (this.Image != "")
					{
						this.DrawImage(g, true, true);
					}
				}
				GraphicsPath pointerPath = this.GetPointerPath(g, true);
				if (pointerPath != null && pointerPath.PointCount != 0)
				{
					using (Matrix matrix = new Matrix())
					{
						matrix.Translate(base.ShadowOffset, base.ShadowOffset);
						pointerPath.Transform(matrix);
					}
					PointF pointF = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
					g.RestoreDrawRegion();
					PointF pointF2 = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
					using (Matrix matrix2 = new Matrix())
					{
						matrix2.Translate(pointF.X - pointF2.X, pointF.Y - pointF2.Y);
						pointerPath.Transform(matrix2);
						return pointerPath;
					}
				}
				g.RestoreDrawRegion();
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
			this.Common.GaugeCore.HotRegionList.SetHotRegion(this, g.GetAbsolutePoint(this.GetScale().GetPivotPoint()), this.hotRegions[0], this.hotRegions[1]);
			this.hotRegions[0] = null;
			this.hotRegions[1] = null;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public CircularGauge GetGauge()
		{
			if (this.Collection == null)
			{
				return null;
			}
			return (CircularGauge)this.Collection.parent;
		}

		public CircularScale GetScale()
		{
			if (this.GetGauge() == null)
			{
				return null;
			}
			if (this.ScaleName == string.Empty)
			{
				return null;
			}
			if (this.ScaleName == "Default" && this.GetGauge().Scales.Count == 0)
			{
				return null;
			}
			return this.GetGauge().Scales[this.ScaleName];
		}

		internal float GetNeedleTailLength()
		{
			float num = (float)((this.Placement != Placement.Cross) ? ((this.Placement != 0) ? (this.GetScale().GetRadius() + this.GetScale().Width / 2.0 + this.DistanceFromScale) : (this.GetScale().GetRadius() - this.GetScale().Width / 2.0 - this.DistanceFromScale)) : (this.GetScale().GetRadius() - this.DistanceFromScale));
			if (this.NeedleStyle != NeedleStyle.Style2 && this.NeedleStyle != NeedleStyle.Style3 && this.NeedleStyle != NeedleStyle.Style5 && this.NeedleStyle != NeedleStyle.Style7 && this.NeedleStyle != NeedleStyle.Style9 && this.NeedleStyle != NeedleStyle.Style10)
			{
				return (float)(this.Width / 2.0);
			}
			float num2 = (float)(num / 1.6180340051651);
			return (float)(num2 * 1.6180340051651 - num2);
		}

		internal XamlRenderer GetCachedXamlRenderer(RectangleF bounds)
		{
			if (this.xamlRenderer != null)
			{
				return this.xamlRenderer;
			}
			this.xamlRenderer = new XamlRenderer(this.CapStyle.ToString() + ".xaml");
			this.xamlRenderer.AllowPathGradientTransform = false;
			Color[] layerHues = new Color[1]
			{
				this.CapFillColor
			};
			this.xamlRenderer.ParseXaml(bounds, layerHues, null);
			return this.xamlRenderer;
		}

		internal void ResetCachedXamlRenderer()
		{
			if (this.xamlRenderer != null)
			{
				this.xamlRenderer.Dispose();
				this.xamlRenderer = null;
			}
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
				this.GetScale().SetDrawRegion(g);
				using (GraphicsPath graphicsPath = this.GetPointerPath(g, false))
				{
					if (graphicsPath != null)
					{
						RectangleF bounds = graphicsPath.GetBounds();
						g.DrawSelection(bounds, designTimeSelection, this.Common.GaugeCore.SelectionBorderColor, this.Common.GaugeCore.SelectionMarkerColor);
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

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			CircularPointer circularPointer = new CircularPointer();
			binaryFormatSerializer.Deserialize(circularPointer, stream);
			return circularPointer;
		}
	}
}
