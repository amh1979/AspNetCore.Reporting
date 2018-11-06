using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class DistanceScalePanel : DockablePanel, IToolTipProvider
	{
		private enum MeasurementUnit
		{
			km,
			m,
			cm,
			mi,
			ft,
			@in
		}

		private const int ScaleOutlineWidth = 3;

		private const int PanelPadding = 3;

		private float KilometersToMiles = 0.621f;

		private int HorizLabelMargin = 3;

		private int VertLabelMargin;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color scaleBorderColor = Color.DarkGray;

		private Color scaleForeColor = Color.White;

		private Color labelColor = Color.Black;

		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		[SRDescription("DescriptionAttributeDistanceScalePanel_Font")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		public Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.font = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeDistanceScalePanel_ScaleBorderColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRCategory("CategoryAttribute_Appearance")]
		public Color ScaleBorderColor
		{
			get
			{
				return this.scaleBorderColor;
			}
			set
			{
				this.scaleBorderColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeDistanceScalePanel_ScaleForeColor")]
		[SRCategory("CategoryAttribute_Appearance")]
		public Color ScaleForeColor
		{
			get
			{
				return this.scaleForeColor;
			}
			set
			{
				this.scaleForeColor = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeDistanceScalePanel_LabelColor")]
		[SRCategory("CategoryAttribute_Appearance")]
		public Color LabelColor
		{
			get
			{
				return this.labelColor;
			}
			set
			{
				this.labelColor = value;
				this.Invalidate();
			}
		}

		public DistanceScalePanel()
			: this(null)
		{
		}

		internal DistanceScalePanel(CommonElements common)
			: base(common)
		{
			this.Name = "DistanceScalePanel";
			this.SizeUnit = CoordinateUnit.Pixel;
		}

		internal override void Render(MapGraphics g)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			try
			{
				MapCore mapCore = base.GetMapCore();
				if (mapCore != null)
				{
					g.AntiAliasing = AntiAliasing.None;
					base.Render(g);
					float num = 4.5f;
					RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
					absoluteRectangle.Inflate((float)(0.0 - num), (float)(0.0 - num));
					if (!(absoluteRectangle.Width < 3.0) && !(absoluteRectangle.Height < 3.0))
					{
						float num2 = 0f;
						float num3 = 0f;
						string text = "0";
						string text2 = "0";
						MeasurementUnit measurementUnit = MeasurementUnit.km;
						MeasurementUnit measurementUnit2 = MeasurementUnit.mi;
						float num4 = (float)(absoluteRectangle.Width - 6.0);
						float num5 = (float)mapCore.PixelsToKilometers(num4);
						float num6 = num5 * this.KilometersToMiles;
						measurementUnit = this.AdjustMetricUnit(ref num5);
						float num7 = (float)this.FloorDistance((double)num5);
						float num8 = num7 / num5;
						measurementUnit2 = this.AdjustImperialUnit(ref num6);
						float num9 = (float)this.FloorDistance((double)num6);
						float num10 = num9 / num6;
						if (num7 >= 1.0 && num9 >= 1.0)
						{
							num2 = num4 * num8;
							num3 = num4 * num10;
							if (base.GetMapCore().MapControl.FormatNumberHandler != null)
							{
								text = base.GetMapCore().MapControl.FormatNumberHandler(base.GetMapCore().MapControl, num7, "G");
								text2 = base.GetMapCore().MapControl.FormatNumberHandler(base.GetMapCore().MapControl, num9, "G");
							}
							else
							{
								text = num7.ToString(CultureInfo.CurrentCulture);
								text2 = num9.ToString(CultureInfo.CurrentCulture);
							}
						}
						else
						{
							num2 = num4;
							num3 = num4;
						}
						using (GraphicsPath path = this.CreateScalePath(absoluteRectangle, (int)num2, (int)num3))
						{
							using (Brush brush = new SolidBrush(this.ScaleForeColor))
							{
								g.FillPath(brush, path);
							}
							using (Pen pen = new Pen(this.ScaleBorderColor, 1f))
							{
								pen.Alignment = PenAlignment.Center;
								pen.MiterLimit = 0f;
								g.DrawPath(pen, path);
							}
							StringFormat stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
							stringFormat.FormatFlags = StringFormatFlags.NoWrap;
							using (Brush brush2 = new SolidBrush(this.LabelColor))
							{
								RectangleF textBounds = new RectangleF((float)(absoluteRectangle.Left + 3.0), absoluteRectangle.Top, num2, (float)(absoluteRectangle.Height / 2.0 - 3.0));
								string text3 = string.Format(CultureInfo.CurrentCulture, "{0} {1}", text, ((Enum)(object)measurementUnit).ToString((IFormatProvider)CultureInfo.CurrentCulture));
								SizeF textClipSize = g.MeasureString(text3, this.Font, textBounds.Size, stringFormat);
								RectangleF layoutRectangle = this.CreateTextClip(textBounds, textClipSize);
								g.DrawString(text3, this.Font, brush2, layoutRectangle, stringFormat);
								RectangleF textBounds2 = new RectangleF((float)(absoluteRectangle.Left + 3.0), (float)(absoluteRectangle.Top + absoluteRectangle.Height / 2.0 + 3.0), num3, (float)(absoluteRectangle.Height / 2.0 - 3.0));
								string text4 = string.Format(CultureInfo.CurrentCulture, "{0} {1}", text2, ((Enum)(object)measurementUnit2).ToString((IFormatProvider)CultureInfo.CurrentCulture));
								SizeF textClipSize2 = g.MeasureString(text4, this.Font, textBounds2.Size, stringFormat);
								RectangleF layoutRectangle2 = this.CreateTextClip(textBounds2, textClipSize2);
								g.DrawString(text4, this.Font, brush2, layoutRectangle2, stringFormat);
							}
						}
					}
				}
			}
			finally
			{
				g.AntiAliasing = antiAliasing;
			}
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Size":
				return new MapSize(null, 130f, 55f);
			case "SizeUnit":
				return CoordinateUnit.Pixel;
			case "Dock":
				return PanelDockStyle.Bottom;
			case "DockAlignment":
				return DockAlignment.Far;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		private int FloorDistance(double distance)
		{
			int num = (int)Math.Log10(distance);
			double num2 = Math.Pow(10.0, (double)num) / 2.0;
			return (int)(Math.Floor(distance / num2) * num2);
		}

		private MeasurementUnit AdjustMetricUnit(ref float kilometers)
		{
			MeasurementUnit result = MeasurementUnit.km;
			if ((double)kilometers < 0.001)
			{
				kilometers *= 100000f;
				result = MeasurementUnit.cm;
			}
			else if (kilometers < 1.0)
			{
				kilometers *= 1000f;
				result = MeasurementUnit.m;
			}
			return result;
		}

		private MeasurementUnit AdjustImperialUnit(ref float miles)
		{
			MeasurementUnit result = MeasurementUnit.mi;
			if (miles < 0.00018939393339678645)
			{
				miles *= 63360f;
				result = MeasurementUnit.@in;
			}
			else if (miles < 1.0)
			{
				miles *= 5280f;
				result = MeasurementUnit.ft;
			}
			return result;
		}

		private GraphicsPath CreateScalePath(RectangleF drawingBounds, int metricScaleWidth, int imperialScaleWidth)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			Rectangle rectangle = Rectangle.Truncate(drawingBounds);
			rectangle.Inflate(0, -(rectangle.Height - 9) / 2);
			bool flag = metricScaleWidth > imperialScaleWidth;
			int num = rectangle.Y + rectangle.Height / 2;
			int num2 = (rectangle.Height % 2 == 0) ? 1 : 0;
			int num3 = num - 1;
			int num4 = num + 1 + 1;
			Point point = new Point(rectangle.X + 3 + metricScaleWidth, num3);
			Point point2 = new Point(rectangle.X + 6 + metricScaleWidth, flag ? num4 : num3);
			Point point3 = new Point(rectangle.X + 3 + imperialScaleWidth, num4);
			Point point4 = new Point(rectangle.X + 6 + imperialScaleWidth, flag ? num4 : num3);
			graphicsPath.AddPolygon(new Point[14]
			{
				new Point(rectangle.Left, rectangle.Top + num2),
				new Point(rectangle.Left + 3, rectangle.Top + num2),
				new Point(rectangle.Left + 3, num3),
				point,
				new Point(point.X, rectangle.Top + num2),
				new Point(point2.X, rectangle.Top + num2),
				point2,
				point4,
				new Point(point4.X, rectangle.Bottom),
				new Point(point3.X, rectangle.Bottom),
				point3,
				new Point(rectangle.Left + 3, num4),
				new Point(rectangle.Left + 3, rectangle.Bottom),
				new Point(rectangle.Left, rectangle.Bottom)
			});
			return graphicsPath;
		}

		private RectangleF CreateTextClip(RectangleF textBounds, SizeF textClipSize)
		{
			textBounds.Inflate((float)(-this.HorizLabelMargin), (float)(-this.VertLabelMargin));
			textClipSize.Width = Math.Max(1f, Math.Min(textClipSize.Width, textBounds.Width));
			textClipSize.Height = Math.Max(1f, Math.Min(textClipSize.Height, textBounds.Height));
			PointF location = new PointF(textBounds.X, (float)(textBounds.Y + (textBounds.Height - textClipSize.Height) / 2.0));
			return new RectangleF(location, textClipSize);
		}
	}
}
