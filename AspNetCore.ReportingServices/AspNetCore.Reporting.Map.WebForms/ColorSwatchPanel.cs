using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(AutoSizePanelConverter))]
	internal class ColorSwatchPanel : AutoSizePanel
	{
		private const int DummyItemsCount = 5;

		private int PanelPadding = 8;

		private int TitleSeparatorSize = 8;

		private int TickMarkLabelGapSize = 2;

		private float TrimmingProtector;

		private bool showSelectedTitle;

		private RectangleF titlePosition;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color outlineColor = Color.DimGray;

		private Color labelColor = Color.Black;

		private LabelAlignment labelAlignment = LabelAlignment.Alternate;

		private Color rangeGapsColor = Color.White;

		private string numricLabelFormat = "#,##0.##";

		private SwatchLabelType labelType;

		private bool showEndLabels = true;

		private int tickMarkLength = 3;

		private int labelInterval = 1;

		private SwatchColorCollection swatchColors;

		private string noDataText = "N/A";

		private Font titleFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);

		private Color titleColor = Color.Black;

		private StringAlignment titleAlignment = StringAlignment.Center;

		private string title = "";

		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[Browsable(false)]
		internal bool ShowSelectedTitle
		{
			get
			{
				return this.showSelectedTitle;
			}
			set
			{
				this.showSelectedTitle = value;
				base.Invalidate(true);
			}
		}

		[Browsable(false)]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		internal RectangleF TitleSelectionRectangle
		{
			get
			{
				RectangleF result = this.titlePosition;
				result.Offset(base.GetAbsoluteLocation());
				return result;
			}
		}

		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeColorSwatchPanel_Font")]
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
				base.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeColorSwatchPanel_OutlineColor")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DimGray")]
		[SRCategory("CategoryAttribute_Appearance")]
		public Color OutlineColor
		{
			get
			{
				return this.outlineColor;
			}
			set
			{
				this.outlineColor = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_LabelColor")]
		[DefaultValue(typeof(Color), "Black")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeColorSwatchPanel_LabelAlignment")]
		[DefaultValue(LabelAlignment.Alternate)]
		public LabelAlignment LabelAlignment
		{
			get
			{
				return this.labelAlignment;
			}
			set
			{
				this.labelAlignment = value;
				base.Invalidate(true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_RangeGapColor")]
		[DefaultValue(typeof(Color), "White")]
		public Color RangeGapColor
		{
			get
			{
				return this.rangeGapsColor;
			}
			set
			{
				this.rangeGapsColor = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue("#,##0.##")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_NumericLabelFormat")]
		public string NumericLabelFormat
		{
			get
			{
				return this.numricLabelFormat;
			}
			set
			{
				this.numricLabelFormat = value;
				base.Invalidate(true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_LabelType")]
		[DefaultValue(SwatchLabelType.Auto)]
		public SwatchLabelType LabelType
		{
			get
			{
				return this.labelType;
			}
			set
			{
				this.labelType = value;
				base.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeColorSwatchPanel_ShowEndLabels")]
		[DefaultValue(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		public bool ShowEndLabels
		{
			get
			{
				return this.showEndLabels;
			}
			set
			{
				this.showEndLabels = value;
				base.Invalidate(true);
			}
		}

		[DefaultValue(3)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_TickMarkLength")]
		public int TickMarkLength
		{
			get
			{
				return this.tickMarkLength;
			}
			set
			{
				this.tickMarkLength = value;
				base.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_LabelInterval")]
		[DefaultValue(1)]
		[NotifyParentProperty(true)]
		public int LabelInterval
		{
			get
			{
				return this.labelInterval;
			}
			set
			{
				this.labelInterval = value;
				base.Invalidate(true);
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeColorSwatchPanel_Colors")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory("CategoryAttribute_Data")]
		public SwatchColorCollection Colors
		{
			get
			{
				if (this.swatchColors == null)
				{
					this.swatchColors = new SwatchColorCollection(this, base.common);
				}
				return this.swatchColors;
			}
		}

		[DefaultValue("N/A")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_NoDataText")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Data")]
		public string NoDataText
		{
			get
			{
				return this.noDataText;
			}
			set
			{
				this.noDataText = value;
				base.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttribute_Title")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt, style=Bold")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_TitleFont")]
		public Font TitleFont
		{
			get
			{
				return this.titleFont;
			}
			set
			{
				this.titleFont = value;
				base.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeColorSwatchPanel_TitleColor")]
		[SRCategory("CategoryAttribute_Title")]
		[DefaultValue(typeof(Color), "Black")]
		[NotifyParentProperty(true)]
		public Color TitleColor
		{
			get
			{
				return this.titleColor;
			}
			set
			{
				this.titleColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(StringAlignment.Center)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Title")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_TitleAlignment")]
		public StringAlignment TitleAlignment
		{
			get
			{
				return this.titleAlignment;
			}
			set
			{
				this.titleAlignment = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Title")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_Title")]
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				this.title = value;
				base.Invalidate(true);
			}
		}

		internal override bool IsEmpty
		{
			get
			{
				if (this.Common != null && this.Common.MapCore.IsDesignMode())
				{
					return false;
				}
				return this.Colors.Count == 0;
			}
		}

		public override RectangleF GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			RectangleF result = base.GetSelectionRectangle(g, clipRect);
			if (this.ShowSelectedTitle && !this.TitleSelectionRectangle.IsEmpty)
			{
				result = this.TitleSelectionRectangle;
			}
			return result;
		}

		public ColorSwatchPanel()
			: this(null)
		{
		}

		internal ColorSwatchPanel(CommonElements common)
			: base(common)
		{
			this.Name = "ColorSwatchPanel";
			this.MaxAutoSize = 100f;
		}

		public SwatchLabelType GetLabelType()
		{
			if (this.LabelType != 0)
			{
				return this.LabelType;
			}
			SwatchLabelType result = SwatchLabelType.ShowBorderValue;
			foreach (SwatchColor color in this.Colors)
			{
				if (color.HasTextValue)
				{
					return SwatchLabelType.ShowMiddleValue;
				}
			}
			return result;
		}

		internal override void Render(MapGraphics g)
		{
			base.Render(g);
			if (!this.IsEmpty)
			{
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
				bool flag = false;
				try
				{
					if (this.Colors.Count == 0 && this.Common != null && this.Common.MapCore.IsDesignMode())
					{
						this.PopulateDummyData();
						flag = true;
					}
					int num = (this.LabelAlignment != LabelAlignment.Alternate) ? 1 : 2;
					SwatchLabelType swatchLabelType = this.GetLabelType();
					SizeF colorBoxSize = default(SizeF);
					SizeF labelBoxSize = default(SizeF);
					SizeF firstCaptionSize = default(SizeF);
					SizeF lastCaptionSize = default(SizeF);
					this.CalculateFontDependentData(g, absoluteRectangle.Size);
					absoluteRectangle.Inflate((float)(-this.PanelPadding), (float)(-this.PanelPadding));
					if (!(absoluteRectangle.Width < 1.0) && !(absoluteRectangle.Height < 1.0))
					{
						int[] colorsRef = this.GetColorsRef(swatchLabelType);
						float num2 = 0f;
						if (this.LabelInterval > 0 && this.ShowEndLabels)
						{
							firstCaptionSize = g.MeasureString(this.GetLabelCaption(0, true, swatchLabelType), this.Font, absoluteRectangle.Size, StringFormat.GenericTypographic);
							firstCaptionSize.Width += this.TrimmingProtector;
							lastCaptionSize = g.MeasureString(this.GetLabelCaption(this.Colors.Count - 1, false, swatchLabelType), this.Font, absoluteRectangle.Size, StringFormat.GenericTypographic);
							lastCaptionSize.Width += this.TrimmingProtector;
							num2 = Math.Max(firstCaptionSize.Width, lastCaptionSize.Width);
						}
						bool flag2 = !string.IsNullOrEmpty(this.Title);
						RectangleF layoutRectangle = absoluteRectangle;
						if (flag2)
						{
							float height = absoluteRectangle.Height;
							SizeF sizeF = g.MeasureString(this.Title, this.TitleFont, layoutRectangle.Size, StringFormat.GenericTypographic);
							float num4 = layoutRectangle.Height = Math.Min(height, sizeF.Height + (float)this.TitleSeparatorSize);
							absoluteRectangle.Y += num4;
							absoluteRectangle.Height -= num4;
							this.titlePosition = layoutRectangle;
						}
						RectangleF colorBarBounds = this.CalculateMaxColorBarBounds(g, absoluteRectangle, num2, colorsRef.Length, swatchLabelType);
						float num5 = 0f;
						float num6 = 0f;
						if (this.LabelInterval > 0)
						{
							num5 = this.GetLabelMaxSize(g, absoluteRectangle.Size, swatchLabelType).Height;
							num6 = (float)(this.TickMarkLength + this.TickMarkLabelGapSize);
						}
						float val = Math.Max(3f, (float)((absoluteRectangle.Height - num6) / 5.0));
						colorBoxSize.Height = Math.Max(val, absoluteRectangle.Height - (float)num * (num6 + num5));
						colorBoxSize.Width = colorBarBounds.Width / (float)colorsRef.Length;
						colorBarBounds.Height = colorBoxSize.Height;
						labelBoxSize.Height = Math.Max(0f, absoluteRectangle.Height - colorBoxSize.Height) / (float)num - num6;
						labelBoxSize.Width = colorBoxSize.Width * (float)this.LabelInterval * (float)num;
						if (this.LabelAlignment == LabelAlignment.Top || this.LabelAlignment == LabelAlignment.Alternate)
						{
							colorBarBounds.Y += labelBoxSize.Height + num6;
						}
						AntiAliasing antiAliasing = g.AntiAliasing;
						try
						{
							g.AntiAliasing = AntiAliasing.None;
							GraphicsPath graphicsPath = default(GraphicsPath);
							GraphicsPath graphicsPath2 = default(GraphicsPath);
							this.CreateColorBarPath(absoluteRectangle, colorBarBounds, colorsRef, swatchLabelType, out graphicsPath, out graphicsPath2);
							GraphicsPathIterator graphicsPathIterator = new GraphicsPathIterator(graphicsPath2);
							GraphicsPath graphicsPath3 = new GraphicsPath();
							Pen pen = new Pen(this.OutlineColor);
							try
							{
								int[] array = colorsRef;
								foreach (int colorIndex in array)
								{
									graphicsPath3.Reset();
									bool flag3 = default(bool);
									graphicsPathIterator.NextSubpath(graphicsPath3, out flag3);
									if (flag3)
									{
										using (Brush brush = this.CreateColorBoxBrush(g, graphicsPath3.GetBounds(), colorIndex))
										{
											g.FillPath(brush, graphicsPath3);
										}
									}
								}
								g.DrawPath(pen, graphicsPath);
							}
							finally
							{
								graphicsPath.Dispose();
								graphicsPath2.Dispose();
								graphicsPathIterator.Dispose();
								graphicsPath3.Dispose();
								pen.Dispose();
							}
						}
						finally
						{
							g.AntiAliasing = antiAliasing;
						}
						if (flag2)
						{
							using (Brush brush2 = new SolidBrush(this.TitleColor))
							{
								using (StringFormat stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone())
								{
									stringFormat.Alignment = this.TitleAlignment;
									stringFormat.LineAlignment = StringAlignment.Near;
									stringFormat.Trimming = StringTrimming.EllipsisCharacter;
									stringFormat.FormatFlags = StringFormatFlags.NoWrap;
									g.DrawString(this.Title, this.TitleFont, brush2, layoutRectangle, stringFormat);
								}
							}
						}
						if (this.Colors.Count != 0 && this.LabelInterval != 0)
						{
							using (StringFormat stringFormat2 = (StringFormat)StringFormat.GenericTypographic.Clone())
							{
								stringFormat2.Alignment = StringAlignment.Center;
								stringFormat2.LineAlignment = StringAlignment.Near;
								stringFormat2.Trimming = StringTrimming.EllipsisCharacter;
								stringFormat2.FormatFlags = StringFormatFlags.NoWrap;
								using (Brush brush3 = new SolidBrush(this.LabelColor))
								{
									bool flag4 = this.LabelAlignment != LabelAlignment.Top;
									if (swatchLabelType == SwatchLabelType.ShowMiddleValue)
									{
										for (int j = 0; j < colorsRef.Length; j++)
										{
											if (this.MustPrintLabel(colorsRef, j, true, swatchLabelType))
											{
												StringAlignment alignment = default(StringAlignment);
												RectangleF labelBounds = this.GetLabelBounds(j, colorsRef, absoluteRectangle, colorBarBounds, labelBoxSize, colorBoxSize, num2, true, swatchLabelType, flag4, firstCaptionSize, lastCaptionSize, out alignment);
												string labelCaption = this.GetLabelCaption(j, true, swatchLabelType);
												if (labelBounds.Width > 1.0 && labelBounds.Height > 1.0)
												{
													if (flag4)
													{
														labelBounds.Offset(0f, 1f);
													}
													stringFormat2.Alignment = alignment;
													g.DrawString(labelCaption, this.Font, brush3, labelBounds, stringFormat2);
												}
												flag4 = ((this.LabelAlignment == LabelAlignment.Alternate) ? (!flag4) : flag4);
											}
										}
									}
									else
									{
										for (int k = 0; k < colorsRef.Length; k++)
										{
											RectangleF labelBounds2;
											if (this.MustPrintLabel(colorsRef, k, true, swatchLabelType))
											{
												StringAlignment alignment2 = default(StringAlignment);
												labelBounds2 = this.GetLabelBounds(colorsRef[k], colorsRef, absoluteRectangle, colorBarBounds, labelBoxSize, colorBoxSize, num2, true, swatchLabelType, flag4, firstCaptionSize, lastCaptionSize, out alignment2);
												string labelCaption2 = this.GetLabelCaption(colorsRef[k], true, swatchLabelType);
												if (labelBounds2.Width > 1.0 && labelBounds2.Height > 1.0)
												{
													if (flag4)
													{
														labelBounds2.Offset(0f, 1f);
													}
													stringFormat2.Alignment = alignment2;
													g.DrawString(labelCaption2, this.Font, brush3, labelBounds2, stringFormat2);
												}
												flag4 = ((this.LabelAlignment == LabelAlignment.Alternate) ? (!flag4) : flag4);
											}
											if (this.MustPrintLabel(colorsRef, k, false, swatchLabelType))
											{
												StringAlignment alignment3 = default(StringAlignment);
												labelBounds2 = this.GetLabelBounds(colorsRef[k], colorsRef, absoluteRectangle, colorBarBounds, labelBoxSize, colorBoxSize, num2, false, swatchLabelType, flag4, firstCaptionSize, lastCaptionSize, out alignment3);
												string labelCaption2 = this.GetLabelCaption(colorsRef[k], false, swatchLabelType);
												if (labelBounds2.Width > 1.0 && labelBounds2.Height > 1.0)
												{
													if (flag4)
													{
														labelBounds2.Offset(0f, 1f);
													}
													stringFormat2.Alignment = alignment3;
													g.DrawString(labelCaption2, this.Font, brush3, labelBounds2, stringFormat2);
												}
												flag4 = ((this.LabelAlignment == LabelAlignment.Alternate) ? (!flag4) : flag4);
											}
										}
									}
								}
							}
						}
					}
				}
				finally
				{
					if (flag)
					{
						this.Colors.Clear();
					}
				}
			}
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Dock":
				return PanelDockStyle.Bottom;
			case "Size":
				return new MapSize(null, 350f, 60f);
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		private void CreateColorBarPath(RectangleF panelBounds, RectangleF colorBarBounds, int[] colorsRef, SwatchLabelType currentLabelType, out GraphicsPath outlinePath, out GraphicsPath fillPath)
		{
			colorBarBounds = Rectangle.Round(colorBarBounds);
			float width = colorBarBounds.Width / (float)colorsRef.Length;
			RectangleF rect = new RectangleF(colorBarBounds.X, colorBarBounds.Y, width, colorBarBounds.Height);
			outlinePath = new GraphicsPath();
			fillPath = new GraphicsPath();
			PointF pointF = new PointF(colorBarBounds.Left, colorBarBounds.Bottom);
			PointF pointF2 = new PointF(colorBarBounds.Left, colorBarBounds.Top);
			float num = Math.Min((float)(this.TickMarkLength + 1), panelBounds.Bottom - pointF.Y);
			float num2 = Math.Min((float)this.TickMarkLength, pointF.Y - panelBounds.Top);
			pointF2.Y -= num2;
			if (!(colorBarBounds.Width <= 0.0) && !(colorBarBounds.Width <= 0.0))
			{
				outlinePath.AddRectangle(colorBarBounds);
				for (int i = 0; i < colorsRef.Length; i++)
				{
					fillPath.StartFigure();
					fillPath.AddRectangle(rect);
					outlinePath.StartFigure();
					outlinePath.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom);
					rect.Offset(rect.Width, 0f);
				}
				bool flag = this.LabelAlignment != LabelAlignment.Top;
				if (this.LabelInterval > 0 && this.TickMarkLength > 0)
				{
					switch (currentLabelType)
					{
					case SwatchLabelType.ShowMiddleValue:
						pointF.X += (float)(rect.Width / 2.0);
						pointF2.X += (float)(rect.Width / 2.0);
						for (int k = 0; k < colorsRef.Length; k += this.LabelInterval)
						{
							if (this.MustPrintLabel(colorsRef, k, true, currentLabelType))
							{
								PointF pointF4 = PointF.Empty;
								float num5 = 0f;
								if (flag)
								{
									pointF4 = pointF;
									num5 = num;
								}
								else
								{
									pointF4 = pointF2;
									num5 = num2;
								}
								outlinePath.StartFigure();
								outlinePath.AddLine(pointF4.X + (float)k * rect.Width, pointF4.Y, pointF4.X + (float)k * rect.Width, pointF4.Y + num5);
								flag = ((this.LabelAlignment == LabelAlignment.Alternate) ? (!flag) : flag);
							}
						}
						break;
					case SwatchLabelType.ShowBorderValue:
					{
						PointF pointF3 = PointF.Empty;
						float num3 = 0f;
						for (int j = 0; j < colorsRef.Length * 2; j++)
						{
							if (flag)
							{
								pointF3 = pointF;
								num3 = num;
							}
							else
							{
								pointF3 = pointF2;
								num3 = num2;
							}
							if (this.MustPrintLabel(colorsRef, j / 2, j % 2 == 0, currentLabelType))
							{
								float num4 = 0f;
								if (this.Colors[colorsRef[j / 2]].NoData)
								{
									num4 = (float)(rect.Width / 2.0);
								}
								outlinePath.StartFigure();
								outlinePath.AddLine(pointF3.X + num4 + (float)(j / 2 + j % 2) * rect.Width, pointF3.Y, pointF3.X + num4 + (float)(j / 2 + j % 2) * rect.Width, pointF3.Y + num3);
								flag = ((this.LabelAlignment == LabelAlignment.Alternate) ? (!flag) : flag);
							}
						}
						break;
					}
					}
				}
			}
		}

		private int[] GetColorsRef(SwatchLabelType currentLabelType)
		{
			if (this.Colors.Count == 0)
			{
				return new int[0];
			}
			int num = this.Colors.Count;
			if (currentLabelType == SwatchLabelType.ShowBorderValue)
			{
				for (int i = 1; i < this.Colors.Count; i++)
				{
					if (!this.Colors[i - 1].NoData || !this.Colors[i].NoData)
					{
						if (this.Colors[i - 1].NoData || this.Colors[i].NoData)
						{
							num++;
						}
						else if (!this.Colors[i - 1].HasTextValue && !this.Colors[i].HasTextValue && this.Colors[i - 1].ToValue != this.Colors[i].FromValue)
						{
							num++;
						}
					}
				}
			}
			int[] array = new int[num];
			array[0] = 0;
			int num2 = 1;
			for (int j = 1; j < this.Colors.Count; j++)
			{
				if (currentLabelType == SwatchLabelType.ShowBorderValue && (!this.Colors[j - 1].NoData || !this.Colors[j].NoData))
				{
					if (this.Colors[j - 1].NoData || this.Colors[j].NoData)
					{
						array[num2++] = -1;
					}
					else if (!this.Colors[j - 1].HasTextValue && !this.Colors[j].HasTextValue && this.Colors[j - 1].ToValue != this.Colors[j].FromValue)
					{
						array[num2++] = -1;
					}
				}
				array[num2++] = j;
			}
			return array;
		}

		private RectangleF GetLabelBounds(int colorIndex, int[] colorRef, RectangleF panelBounds, RectangleF colorBarBounds, SizeF labelBoxSize, SizeF colorBoxSize, float longestEndLabelWidth, bool getFromValue, SwatchLabelType curLabelType, bool alignedBottom, SizeF firstCaptionSize, SizeF lastCaptionSize, out StringAlignment horizontalAlignemnt)
		{
			RectangleF rectangleF = new RectangleF(colorBarBounds.Location, labelBoxSize);
			float num = 0f;
			if (colorIndex == 0 && getFromValue)
			{
				goto IL_002e;
			}
			if (colorIndex == colorRef.Length - 1 && (!getFromValue || curLabelType == SwatchLabelType.ShowMiddleValue))
			{
				goto IL_002e;
			}
			goto IL_0074;
			IL_0074:
			if (alignedBottom)
			{
				rectangleF.Offset(0f, colorBarBounds.Height + (float)this.TickMarkLength + (float)this.TickMarkLabelGapSize);
			}
			else
			{
				rectangleF.Offset(0f, (float)(0.0 - (labelBoxSize.Height + (float)this.TickMarkLength + (float)this.TickMarkLabelGapSize)));
			}
			int num2 = Array.IndexOf(colorRef, colorIndex);
			rectangleF.X += (float)num2 * colorBoxSize.Width;
			rectangleF.X += (float)((colorBoxSize.Width - rectangleF.Width) / 2.0);
			rectangleF.X += num;
			if (rectangleF.Bottom >= panelBounds.Bottom)
			{
				rectangleF.Height = Math.Max(1f, rectangleF.Height - (rectangleF.Bottom - panelBounds.Bottom));
				rectangleF.Y = panelBounds.Bottom - rectangleF.Height;
			}
			if (!this.Colors[colorIndex].NoData && curLabelType == SwatchLabelType.ShowBorderValue)
			{
				rectangleF.Offset((float)(getFromValue ? ((0.0 - colorBoxSize.Width) / 2.0) : (colorBoxSize.Width / 2.0)), 0f);
			}
			float num3 = panelBounds.Left - rectangleF.Left;
			bool flag = false;
			bool flag2 = false;
			if (num3 > 0.0)
			{
				RectangleF rectangleF2 = rectangleF;
				rectangleF2.Inflate((float)(0.0 - num3), 0f);
				if (rectangleF2.Width > firstCaptionSize.Width)
				{
					rectangleF = rectangleF2;
				}
				else
				{
					rectangleF.X += num3;
					rectangleF.Width -= num3;
					flag = true;
				}
			}
			float num4 = rectangleF.Right - panelBounds.Right;
			if (num4 > 0.0)
			{
				RectangleF rectangleF3 = rectangleF;
				rectangleF3.Inflate((float)(0.0 - num4), 0f);
				if (rectangleF3.Width > lastCaptionSize.Width)
				{
					rectangleF = rectangleF3;
				}
				else
				{
					rectangleF.Width -= num4;
					flag2 = true;
				}
			}
			if (flag && flag2)
			{
				horizontalAlignemnt = StringAlignment.Center;
			}
			else if (flag)
			{
				horizontalAlignemnt = StringAlignment.Near;
			}
			else if (flag2)
			{
				horizontalAlignemnt = StringAlignment.Far;
			}
			else
			{
				horizontalAlignemnt = StringAlignment.Center;
			}
			rectangleF.Height += 1f;
			return Rectangle.Round(rectangleF);
			IL_002e:
			rectangleF.Width = Math.Max(rectangleF.Width, longestEndLabelWidth);
			num = (float)((rectangleF.Width - labelBoxSize.Width) / 2.0);
			num = (float)((!(num > 0.0)) ? 0.0 : (num * (float)((colorIndex != 0) ? 1 : (-1))));
			goto IL_0074;
		}

		private RectangleF CalculateMaxColorBarBounds(MapGraphics g, RectangleF bounds, float endCaptionWidth, int colorsNumber, SwatchLabelType currentLabelType)
		{
			if (this.LabelInterval == 0)
			{
				return bounds;
			}
			LabelAlignment labelAlignment2 = this.LabelAlignment;
			float num = endCaptionWidth;
			if (currentLabelType == SwatchLabelType.ShowMiddleValue)
			{
				num = ((this.Colors.Count != 1) ? (((float)this.Colors.Count * endCaptionWidth - bounds.Width) / (float)(this.Colors.Count - 1)) : (num - bounds.Width / (float)colorsNumber));
				num = (float)((num < 0.0) ? 0.0 : num);
			}
			bounds.Inflate((float)((0.0 - num) / 2.0), 0f);
			return bounds;
		}

		private string GetLabelCaption(int swatchColorIndex, bool getFromValue, SwatchLabelType currentLabelType)
		{
			SwatchColor swatchColor = this.Colors[swatchColorIndex];
			if (swatchColor.NoData)
			{
				return this.NoDataText;
			}
			string result = "";
			if (currentLabelType == SwatchLabelType.ShowBorderValue)
			{
				if (!swatchColor.HasTextValue)
				{
					double num = (!getFromValue) ? swatchColor.ToValue : swatchColor.FromValue;
					result = ((base.GetMapCore().MapControl.FormatNumberHandler == null) ? num.ToString(this.NumericLabelFormat, CultureInfo.CurrentCulture) : base.GetMapCore().MapControl.FormatNumberHandler(base.GetMapCore().MapControl, num, this.NumericLabelFormat));
				}
			}
			else
			{
				if (swatchColor.HasTextValue)
				{
					return swatchColor.TextValue;
				}
				double num2 = (swatchColor.FromValue + swatchColor.ToValue) / 2.0;
				result = ((base.GetMapCore().MapControl.FormatNumberHandler == null) ? num2.ToString(this.NumericLabelFormat, CultureInfo.CurrentCulture) : base.GetMapCore().MapControl.FormatNumberHandler(base.GetMapCore().MapControl, num2, this.NumericLabelFormat));
			}
			return result;
		}

		private SizeF GetLabelMaxSize(MapGraphics g, SizeF layoutArea, SwatchLabelType currentLabelType)
		{
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < this.Colors.Count; i++)
			{
				string labelCaption = this.GetLabelCaption(i, true, currentLabelType);
				SizeF sizeF = g.MeasureString(labelCaption, this.Font, layoutArea, StringFormat.GenericTypographic);
				num = Math.Max(num, sizeF.Height);
				num2 = Math.Max(num2, sizeF.Width + this.TrimmingProtector);
				if (currentLabelType == SwatchLabelType.ShowBorderValue)
				{
					labelCaption = this.GetLabelCaption(i, false, currentLabelType);
					sizeF = g.MeasureString(labelCaption, this.Font, layoutArea, StringFormat.GenericTypographic);
					num = Math.Max(num, sizeF.Height);
					num2 = Math.Max(num2, sizeF.Width + this.TrimmingProtector);
				}
			}
			return System.Drawing.Size.Round(new SizeF(num2, num));
		}

		private bool MustPrintLabel(int[] colorsRef, int colorRefIndex, bool isFromValue, SwatchLabelType currentLabelType)
		{
			if (this.LabelInterval <= 0)
			{
				return false;
			}
			int num = colorRefIndex + ((currentLabelType == SwatchLabelType.ShowBorderValue && !isFromValue) ? 1 : 0);
			if (colorRefIndex == 0 && isFromValue)
			{
				if (this.ShowEndLabels)
				{
					return true;
				}
				return this.Colors[colorsRef[colorRefIndex]].NoData;
			}
			if (colorRefIndex == colorsRef.Length - 1 && !this.ShowEndLabels && (currentLabelType == SwatchLabelType.ShowMiddleValue || !isFromValue))
			{
				return false;
			}
			if (currentLabelType == SwatchLabelType.ShowMiddleValue && num % this.LabelInterval == 0)
			{
				return true;
			}
			if (colorsRef[colorRefIndex] != -1 && (this.Colors[colorsRef[colorRefIndex]].NoData || !this.Colors[colorsRef[colorRefIndex]].HasTextValue) && num % this.LabelInterval == 0)
			{
				if (!isFromValue)
				{
					if (!this.Colors[colorsRef[colorRefIndex]].NoData)
					{
						return true;
					}
					return false;
				}
				if (this.MustPrintLabel(colorsRef, colorRefIndex - 1, false, currentLabelType))
				{
					return this.Colors[colorsRef[colorRefIndex]].NoData;
				}
				return true;
			}
			return false;
		}

		private Brush CreateColorBoxBrush(MapGraphics g, RectangleF colorBoxBoundsAbs, int colorIndex)
		{
			if (colorIndex < 0)
			{
				return new SolidBrush(this.RangeGapColor);
			}
			Brush brush = null;
			SwatchColor swatchColor = this.Colors[colorIndex];
			Color color = swatchColor.Color;
			Color secondaryColor = swatchColor.SecondaryColor;
			GradientType gradientType = swatchColor.GradientType;
			MapHatchStyle hatchStyle = swatchColor.HatchStyle;
			if (hatchStyle != 0)
			{
				return MapGraphics.GetHatchBrush(hatchStyle, color, secondaryColor);
			}
			if (gradientType != 0)
			{
				return g.GetGradientBrush(colorBoxBoundsAbs, color, secondaryColor, gradientType);
			}
			return new SolidBrush(color);
		}

		private void CalculateFontDependentData(MapGraphics g, SizeF layoutSize)
		{
			SizeF sizeF = g.MeasureString("M", this.Font, layoutSize, StringFormat.GenericTypographic);
			SizeF sizeF2 = g.MeasureString("MM", this.Font, layoutSize, StringFormat.GenericTypographic);
			float height = sizeF.Height;
			float width = sizeF2.Width;
			float width2 = sizeF.Width;
			this.PanelPadding = (int)Math.Round((double)height);
			this.TitleSeparatorSize = (int)Math.Round((double)height * 0.4);
			this.TickMarkLabelGapSize = (int)Math.Round((double)height * 0.1);
			this.TrimmingProtector = g.MeasureString("..", this.Font, layoutSize, StringFormat.GenericTypographic).Width;
		}

		private void PopulateDummyData()
		{
			if (this.Colors.Count == 0)
			{
				ColorGenerator colorGenerator = new ColorGenerator();
				Color[] array = colorGenerator.GenerateColors(MapColorPalette.Light, 5);
				for (int i = 0; i < array.Length; i++)
				{
					SwatchColor swatchColor = new SwatchColor("", (double)((i + 1) * 100), (double)((i + 2) * 100));
					swatchColor.Color = array[i];
					this.Colors.Add(swatchColor);
				}
			}
		}

		internal override SizeF GetOptimalSize(MapGraphics g, SizeF maxSizeAbs)
		{
			if (!this.IsVisible())
			{
				return SizeF.Empty;
			}
			bool flag = false;
			try
			{
				if (this.Colors.Count == 0 && this.Common != null && this.Common.MapCore.IsDesignMode())
				{
					this.PopulateDummyData();
					flag = true;
				}
				this.CalculateFontDependentData(g, maxSizeAbs);
				SwatchLabelType swatchLabelType = this.GetLabelType();
				float num = (float)(2 * this.PanelPadding);
				float val = 0f;
				SizeF sizeF = default(SizeF);
				SizeF sizeF2 = default(SizeF);
				float num2 = 0f;
				if (this.LabelInterval > 0 && this.ShowEndLabels)
				{
					sizeF = g.MeasureString(this.GetLabelCaption(0, true, swatchLabelType), this.Font, maxSizeAbs, StringFormat.GenericTypographic);
					sizeF.Width += this.TrimmingProtector;
					sizeF2 = g.MeasureString(this.GetLabelCaption(this.Colors.Count - 1, false, swatchLabelType), this.Font, maxSizeAbs, StringFormat.GenericTypographic);
					sizeF2.Width += this.TrimmingProtector;
					num2 = (float)Math.Round((double)Math.Max(sizeF.Width, sizeF2.Width));
				}
				int num3 = (this.LabelAlignment != LabelAlignment.Alternate) ? 1 : 2;
				int[] colorsRef = this.GetColorsRef(swatchLabelType);
				SizeF labelMaxSize = this.GetLabelMaxSize(g, maxSizeAbs, swatchLabelType);
				float height = labelMaxSize.Height;
				float num4 = Math.Max((float)(2.0 * height), labelMaxSize.Width / (float)(num3 * this.LabelInterval));
				num += height + (float)num3 * (labelMaxSize.Height + (float)this.TickMarkLength + (float)this.TickMarkLabelGapSize);
				if (!string.IsNullOrEmpty(this.Title))
				{
					SizeF sizeF3 = g.MeasureString(this.Title, this.TitleFont, maxSizeAbs, StringFormat.GenericTypographic);
					num += sizeF3.Height + (float)this.TitleSeparatorSize;
					val = sizeF3.Width + this.TrimmingProtector;
				}
				val = Math.Max(val, num4 * (float)colorsRef.Length);
				float num5 = num2;
				if (swatchLabelType == SwatchLabelType.ShowMiddleValue)
				{
					num5 -= num4;
					num5 = (float)((num5 < 0.0) ? 0.0 : num5);
				}
				val += num5;
				val += (float)(2 * this.PanelPadding + 2 * this.BorderWidth);
				num += (float)(2 * this.BorderWidth);
				SizeF result = new SizeF(Math.Min(maxSizeAbs.Width, val), Math.Min(maxSizeAbs.Height, num));
				result.Width = Math.Max(2f, result.Width);
				result.Height = Math.Max(2f, result.Height);
				return result;
			}
			finally
			{
				if (flag)
				{
					this.Colors.Clear();
				}
			}
		}
	}
}
