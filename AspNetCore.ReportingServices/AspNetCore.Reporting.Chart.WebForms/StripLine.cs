using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeStripLine_StripLine")]
	[DefaultProperty("IntervalOffset")]
	internal class StripLine : IMapAreaAttributes
	{
		internal Axis axis;

		private double intervalOffset;

		private double interval;

		private DateTimeIntervalType intervalType;

		internal DateTimeIntervalType intervalOffsetType;

		internal bool interlaced;

		private double stripWidth;

		private DateTimeIntervalType stripWidthType;

		private Color backColor = Color.Empty;

		private ChartHatchStyle backHatchStyle;

		private string backImage = "";

		private ChartImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private ChartImageAlign backImageAlign;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private Color borderColor = Color.Black;

		private int borderWidth = 1;

		private ChartDashStyle borderStyle = ChartDashStyle.Solid;

		private string title = "";

		private Color titleColor = Color.Black;

		private Font titleFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private StringAlignment titleAlignment = StringAlignment.Far;

		private StringAlignment titleLineAlignment;

		private string toolTip = "";

		private string href = "";

		private string attributes = "";

		private object mapAreaTag;

		private TextOrientation textOrientation;

		private bool IsTextVertical
		{
			get
			{
				TextOrientation textOrientation = this.GetTextOrientation();
				if (textOrientation != TextOrientation.Rotated90)
				{
					return textOrientation == TextOrientation.Rotated270;
				}
				return true;
			}
		}

		[Bindable(true)]
		[DefaultValue(TextOrientation.Auto)]
		[SRDescription("DescriptionAttribute_TextOrientation")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		public TextOrientation TextOrientation
		{
			get
			{
				return this.textOrientation;
			}
			set
			{
				this.textOrientation = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeData")]
		[SRDescription("DescriptionAttributeStripLine_IntervalOffset")]
		[TypeConverter(typeof(AxisLabelDateValueConverter))]
		[DefaultValue(0.0)]
		public double IntervalOffset
		{
			get
			{
				return this.intervalOffset;
			}
			set
			{
				this.intervalOffset = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeStripLine_IntervalOffsetType")]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttributeData")]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				return this.intervalOffsetType;
			}
			set
			{
				this.intervalOffsetType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[DefaultValue(0.0)]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeStripLine_Interval")]
		[SRCategory("CategoryAttributeData")]
		public double Interval
		{
			get
			{
				return this.interval;
			}
			set
			{
				this.interval = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeStripLine_IntervalType")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeData")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				return this.intervalType;
			}
			set
			{
				this.intervalType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeData")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeStripLine_StripWidth")]
		[Bindable(true)]
		public double StripWidth
		{
			get
			{
				return this.stripWidth;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentException(SR.ExceptionStripLineWidthIsNegative, "value");
				}
				this.stripWidth = value;
				this.Invalidate();
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeStripLine_StripWidthType")]
		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		public DateTimeIntervalType StripWidthType
		{
			get
			{
				return this.stripWidthType;
			}
			set
			{
				this.stripWidthType = ((value != DateTimeIntervalType.NotSet) ? value : DateTimeIntervalType.Auto);
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeStripLine_BackColor")]
		[Bindable(true)]
		public Color BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeStripLine_BorderColor")]
		[SRCategory("CategoryAttributeAppearance")]
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

		[SRDescription("DescriptionAttributeStripLine_BorderStyle")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartDashStyle.Solid)]
		public ChartDashStyle BorderStyle
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

		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeStripLine_BorderWidth")]
		[SRCategory("CategoryAttributeAppearance")]
		public int BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				this.borderWidth = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeBackImage18")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		public string BackImage
		{
			get
			{
				return this.backImage;
			}
			set
			{
				this.backImage = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeStripLine_BackImageMode")]
		[DefaultValue(ChartImageWrapMode.Tile)]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				return this.backImageMode;
			}
			set
			{
				this.backImageMode = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageTransparentColor")]
		[SRCategory("CategoryAttributeAppearance")]
		public Color BackImageTransparentColor
		{
			get
			{
				return this.backImageTranspColor;
			}
			set
			{
				this.backImageTranspColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(ChartImageAlign.TopLeft)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageAlign")]
		[Bindable(true)]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				return this.backImageAlign;
			}
			set
			{
				this.backImageAlign = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeStripLine_BackGradientType")]
		[Bindable(true)]
		public GradientType BackGradientType
		{
			get
			{
				return this.backGradientType;
			}
			set
			{
				this.backGradientType = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeStripLine_BackGradientEndColor")]
		[DefaultValue(typeof(Color), "")]
		public Color BackGradientEndColor
		{
			get
			{
				return this.backGradientEndColor;
			}
			set
			{
				this.backGradientEndColor = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		[SRCategory("CategoryAttributeAppearance")]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return this.backHatchStyle;
			}
			set
			{
				this.backHatchStyle = value;
				this.Invalidate();
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		[DefaultValue("StripLine")]
		[SRDescription("DescriptionAttributeStripLine_Name")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeAppearance")]
		public string Name
		{
			get
			{
				return "StripLine";
			}
		}

		[DefaultValue("")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeStripLine_Title")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeTitle")]
		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				this.title = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeStripLine_TitleColor")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeTitle")]
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

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeStripLine_TitleAlignment")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(StringAlignment), "Far")]
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

		[SRDescription("DescriptionAttributeStripLine_TitleLineAlignment")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeTitle")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(StringAlignment), "Near")]
		public StringAlignment TitleLineAlignment
		{
			get
			{
				return this.titleLineAlignment;
			}
			set
			{
				this.titleLineAlignment = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeStripLine_TitleFont")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeTitle")]
		public Font TitleFont
		{
			get
			{
				return this.titleFont;
			}
			set
			{
				this.titleFont = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeStripLine_ToolTip")]
		[DefaultValue("")]
		[Browsable(false)]
		public string ToolTip
		{
			get
			{
				return this.toolTip;
			}
			set
			{
				this.Invalidate();
				this.toolTip = value;
			}
		}

		[SRDescription("DescriptionAttributeStripLine_Href")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRCategory("CategoryAttributeMapArea")]
		public string Href
		{
			get
			{
				return this.href;
			}
			set
			{
				this.href = value;
				this.Invalidate();
			}
		}

		object IMapAreaAttributes.Tag
		{
			get
			{
				return this.mapAreaTag;
			}
			set
			{
				this.mapAreaTag = value;
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeStripLine_MapAreaAttributes")]
		[DefaultValue("")]
		[SRCategory("CategoryAttributeMapArea")]
		public string MapAreaAttributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = value;
				this.Invalidate();
			}
		}

		internal Axis GetAxis()
		{
			return this.axis;
		}

		private TextOrientation GetTextOrientation()
		{
			if (this.TextOrientation == TextOrientation.Auto && this.axis != null)
			{
				if (this.axis.AxisPosition != AxisPosition.Bottom && this.axis.AxisPosition != AxisPosition.Top)
				{
					return TextOrientation.Horizontal;
				}
				return TextOrientation.Rotated270;
			}
			return this.TextOrientation;
		}

		internal void Paint(ChartGraphics graph, CommonElements common, bool drawLinesOnly)
		{
			if (!this.axis.chartArea.chartAreaIsCurcular)
			{
				RectangleF rect = this.axis.chartArea.PlotAreaPosition.ToRectangleF();
				bool flag = true;
				if (this.axis.AxisPosition == AxisPosition.Bottom || this.axis.AxisPosition == AxisPosition.Top)
				{
					flag = false;
				}
				Series series = null;
				if (this.axis.axisType == AxisName.X || this.axis.axisType == AxisName.X2)
				{
					ArrayList xAxesSeries = this.axis.chartArea.GetXAxesSeries((AxisType)((this.axis.axisType != 0) ? 1 : 0), this.axis.SubAxisName);
					if (xAxesSeries.Count > 0)
					{
						series = this.axis.Common.DataManager.Series[xAxesSeries[0]];
						if (series != null && !series.XValueIndexed)
						{
							series = null;
						}
					}
				}
				double num = this.axis.GetViewMinimum();
				if (!this.axis.chartArea.chartAreaIsCurcular || this.axis.axisType == AxisName.Y || this.axis.axisType == AxisName.Y2)
				{
					double num2 = this.Interval;
					if (this.interlaced)
					{
						num2 /= 2.0;
					}
					num = this.axis.AlignIntervalStart(num, num2, this.IntervalType, series);
				}
				if (this.Interval != 0.0 && (this.axis.GetViewMaximum() - this.axis.GetViewMinimum()) / this.axis.GetIntervalSize(num, this.interval, this.intervalType, series, 0.0, DateTimeIntervalType.Number, false) > 10000.0)
				{
					return;
				}
				DateTimeIntervalType type = (this.IntervalOffsetType == DateTimeIntervalType.Auto) ? this.IntervalType : this.IntervalOffsetType;
				if (this.Interval == 0.0)
				{
					num = this.IntervalOffset;
				}
				else if (this.IntervalOffset > 0.0)
				{
					num += this.axis.GetIntervalSize(num, this.IntervalOffset, type, series, 0.0, DateTimeIntervalType.Number, false);
				}
				else if (this.IntervalOffset < 0.0)
				{
					num -= this.axis.GetIntervalSize(num, 0.0 - this.IntervalOffset, type, series, 0.0, DateTimeIntervalType.Number, false);
				}
				int num3 = 0;
				while (num3++ <= 10000)
				{
					if (this.StripWidth > 0.0 && !drawLinesOnly)
					{
						double num5 = num + this.axis.GetIntervalSize(num, this.StripWidth, this.StripWidthType, series, this.IntervalOffset, type, false);
						if (num5 > this.axis.GetViewMinimum() && num < this.axis.GetViewMaximum())
						{
							RectangleF empty = RectangleF.Empty;
							double val = (double)(float)this.axis.GetLinearPosition(num);
							double val2 = (double)(float)this.axis.GetLinearPosition(num5);
							if (flag)
							{
								empty.X = rect.X;
								empty.Width = rect.Width;
								empty.Y = (float)Math.Min(val, val2);
								empty.Height = (float)Math.Max(val, val2) - empty.Y;
								empty.Intersect(rect);
							}
							else
							{
								empty.Y = rect.Y;
								empty.Height = rect.Height;
								empty.X = (float)Math.Min(val, val2);
								empty.Width = (float)Math.Max(val, val2) - empty.X;
								empty.Intersect(rect);
							}
							if (empty.Width > 0.0 && empty.Height > 0.0)
							{
								graph.StartHotRegion(this.href, this.toolTip);
								if (!this.axis.chartArea.Area3DStyle.Enable3D)
								{
									graph.StartAnimation();
									graph.FillRectangleRel(empty, this.BackColor, this.BackHatchStyle, this.BackImage, this.BackImageMode, this.BackImageTransparentColor, this.BackImageAlign, this.BackGradientType, this.BackGradientEndColor, this.BorderColor, this.BorderWidth, this.BorderStyle, Color.Empty, 0, PenAlignment.Inset);
									graph.StopAnimation();
								}
								else
								{
									this.Draw3DStrip(graph, empty, flag);
								}
								graph.EndHotRegion();
								this.PaintTitle(graph, empty);
								if (common.ProcessModeRegions && !this.axis.chartArea.Area3DStyle.Enable3D)
								{
									common.HotRegionsList.AddHotRegion(graph, empty, this.ToolTip, this.Href, this.MapAreaAttributes, this, ChartElementType.StripLines, string.Empty);
								}
							}
						}
					}
					else if (this.StripWidth == 0.0 && drawLinesOnly && num > this.axis.GetViewMinimum() && num < this.axis.GetViewMaximum())
					{
						PointF empty2 = PointF.Empty;
						PointF empty3 = PointF.Empty;
						if (flag)
						{
							empty2.X = rect.X;
							empty2.Y = (float)this.axis.GetLinearPosition(num);
							empty3.X = rect.Right;
							empty3.Y = empty2.Y;
						}
						else
						{
							empty2.X = (float)this.axis.GetLinearPosition(num);
							empty2.Y = rect.Y;
							empty3.X = empty2.X;
							empty3.Y = rect.Bottom;
						}
						graph.StartHotRegion(this.href, this.toolTip);
						if (!this.axis.chartArea.Area3DStyle.Enable3D)
						{
							graph.DrawLineRel(this.BorderColor, this.BorderWidth, this.BorderStyle, empty2, empty3);
						}
						else
						{
							graph.Draw3DGridLine(this.axis.chartArea, this.borderColor, this.borderWidth, this.borderStyle, empty2, empty3, flag, this.axis.Common, this);
						}
						graph.EndHotRegion();
						this.PaintTitle(graph, empty2, empty3);
						if (common.ProcessModeRegions)
						{
							SizeF size = new SizeF((float)(this.BorderWidth + 1), (float)(this.BorderWidth + 1));
							size = graph.GetRelativeSize(size);
							RectangleF empty4 = RectangleF.Empty;
							if (flag)
							{
								empty4.X = empty2.X;
								empty4.Y = (float)(empty2.Y - size.Height / 2.0);
								empty4.Width = empty3.X - empty2.X;
								empty4.Height = size.Height;
							}
							else
							{
								empty4.X = (float)(empty2.X - size.Width / 2.0);
								empty4.Y = empty2.Y;
								empty4.Width = size.Width;
								empty4.Height = empty3.Y - empty2.Y;
							}
							common.HotRegionsList.AddHotRegion(graph, empty4, this.ToolTip, this.Href, this.MapAreaAttributes, this, ChartElementType.StripLines, string.Empty);
						}
					}
					if (this.Interval > 0.0)
					{
						num += this.axis.GetIntervalSize(num, this.Interval, this.IntervalType, series, this.IntervalOffset, type, false);
					}
					if (!(this.Interval > 0.0))
					{
						break;
					}
					if (!(num <= this.axis.GetViewMaximum()))
					{
						break;
					}
				}
			}
		}

		private void Draw3DStrip(ChartGraphics graph, RectangleF rect, bool horizontal)
		{
			ChartArea chartArea = this.axis.chartArea;
			GraphicsPath graphicsPath = null;
			DrawingOperationTypes drawingOperationTypes = DrawingOperationTypes.DrawElement;
			if (this.axis.Common.ProcessModeRegions)
			{
				drawingOperationTypes |= DrawingOperationTypes.CalcElementPath;
			}
			this.InitAnimation3D(graph.common, rect, (float)(chartArea.IsMainSceneWallOnFront() ? chartArea.areaSceneDepth : 0.0), 0f, chartArea.matrix3D, graph, this.axis);
			graph.StartAnimation();
			graphicsPath = graph.Fill3DRectangle(rect, (float)(chartArea.IsMainSceneWallOnFront() ? chartArea.areaSceneDepth : 0.0), 0f, chartArea.matrix3D, chartArea.Area3DStyle.Light, this.BackColor, this.BackHatchStyle, this.BackImage, this.BackImageMode, this.BackImageTransparentColor, this.BackImageAlign, this.BackGradientType, this.BackGradientEndColor, this.BorderColor, this.BorderWidth, this.BorderStyle, PenAlignment.Outset, drawingOperationTypes);
			graph.StopAnimation();
			if (this.axis.Common.ProcessModeRegions)
			{
				this.axis.Common.HotRegionsList.AddHotRegion(graph, graphicsPath, false, this.ToolTip, this.Href, this.MapAreaAttributes, this, ChartElementType.StripLines);
			}
			if (horizontal)
			{
				if (!chartArea.IsSideSceneWallOnLeft())
				{
					rect.X = rect.Right;
				}
				rect.Width = 0f;
				this.InitAnimation3D(graph.common, rect, 0f, chartArea.areaSceneDepth, chartArea.matrix3D, graph, this.axis);
				graph.StartAnimation();
				graphicsPath = graph.Fill3DRectangle(rect, 0f, chartArea.areaSceneDepth, chartArea.matrix3D, chartArea.Area3DStyle.Light, this.BackColor, this.BackHatchStyle, this.BackImage, this.BackImageMode, this.BackImageTransparentColor, this.BackImageAlign, this.BackGradientType, this.BackGradientEndColor, this.BorderColor, this.BorderWidth, this.BorderStyle, PenAlignment.Outset, drawingOperationTypes);
				graph.StopAnimation();
			}
			else if (chartArea.IsBottomSceneWallVisible())
			{
				rect.Y = rect.Bottom;
				rect.Height = 0f;
				this.InitAnimation3D(graph.common, rect, 0f, chartArea.areaSceneDepth, chartArea.matrix3D, graph, this.axis);
				graph.StartAnimation();
				graphicsPath = graph.Fill3DRectangle(rect, 0f, chartArea.areaSceneDepth, chartArea.matrix3D, chartArea.Area3DStyle.Light, this.BackColor, this.BackHatchStyle, this.BackImage, this.BackImageMode, this.BackImageTransparentColor, this.BackImageAlign, this.BackGradientType, this.BackGradientEndColor, this.BorderColor, this.BorderWidth, this.BorderStyle, PenAlignment.Outset, drawingOperationTypes);
				graph.StopAnimation();
			}
			if (this.axis.Common.ProcessModeRegions)
			{
				this.axis.Common.HotRegionsList.AddHotRegion(graph, graphicsPath, false, this.ToolTip, this.Href, this.MapAreaAttributes, this, ChartElementType.StripLines);
			}
		}

		private void InitAnimation3D(CommonElements common, RectangleF position, float positionZ, float depth, Matrix3D matrix, ChartGraphics graph, Axis axis)
		{
		}

		private void PaintTitle(ChartGraphics graph, PointF point1, PointF point2)
		{
			if (this.Title.Length > 0)
			{
				RectangleF empty = RectangleF.Empty;
				empty.X = point1.X;
				empty.Y = point1.Y;
				empty.Height = point2.Y - empty.Y;
				empty.Width = point2.X - empty.X;
				this.PaintTitle(graph, empty);
			}
		}

		private void PaintTitle(ChartGraphics graph, RectangleF rect)
		{
			if (this.Title.Length > 0)
			{
				string text = this.Title;
				if (this.axis != null && this.axis.chart != null && this.axis.chart.LocalizeTextHandler != null)
				{
					text = this.axis.chart.LocalizeTextHandler(this, text, 0, ChartElementType.StripLines);
				}
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = this.TitleAlignment;
				stringFormat.LineAlignment = this.TitleLineAlignment;
				int num = 0;
				switch (this.TextOrientation)
				{
				case TextOrientation.Rotated90:
					num = 90;
					break;
				case TextOrientation.Rotated270:
					num = 270;
					break;
				case TextOrientation.Auto:
					if (this.axis.AxisPosition != AxisPosition.Bottom && this.axis.AxisPosition != AxisPosition.Top)
					{
						break;
					}
					num = 270;
					break;
				}
				switch (num)
				{
				case 90:
					stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;
					num = 0;
					break;
				case 270:
					stringFormat.FormatFlags = StringFormatFlags.DirectionVertical;
					num = 180;
					break;
				}
				SizeF sizeF = graph.MeasureStringRel(text.Replace("\\n", "\n"), this.TitleFont, new SizeF(100f, 100f), stringFormat, this.GetTextOrientation());
				float z = 0f;
				if (this.axis.chartArea.Area3DStyle.Enable3D)
				{
					Point3D[] array = new Point3D[3];
					z = (float)(this.axis.chartArea.IsMainSceneWallOnFront() ? this.axis.chartArea.areaSceneDepth : 0.0);
					array[0] = new Point3D(0f, 0f, z);
					array[1] = new Point3D(sizeF.Width, 0f, z);
					array[2] = new Point3D(0f, sizeF.Height, z);
					this.axis.chartArea.matrix3D.TransformPoints(array);
					int num2 = (!this.axis.chartArea.IsMainSceneWallOnFront()) ? 1 : 0;
					sizeF.Width *= sizeF.Width / (array[num2].X - array[(num2 == 0) ? 1 : 0].X);
					sizeF.Height *= sizeF.Height / (array[2].Y - array[0].Y);
				}
				SizeF relativeSize = graph.GetRelativeSize(new SizeF((float)this.BorderWidth, (float)this.BorderWidth));
				PointF position = PointF.Empty;
				if (stringFormat.Alignment == StringAlignment.Near)
				{
					position.X = (float)(rect.X + sizeF.Width / 2.0 + relativeSize.Width);
				}
				else if (stringFormat.Alignment == StringAlignment.Far)
				{
					position.X = (float)(rect.Right - sizeF.Width / 2.0 - relativeSize.Width);
				}
				else
				{
					position.X = (float)((rect.Left + rect.Right) / 2.0);
				}
				if (stringFormat.LineAlignment == StringAlignment.Near)
				{
					position.Y = (float)(rect.Top + sizeF.Height / 2.0 + relativeSize.Height);
				}
				else if (stringFormat.LineAlignment == StringAlignment.Far)
				{
					position.Y = (float)(rect.Bottom - sizeF.Height / 2.0 - relativeSize.Height);
				}
				else
				{
					position.Y = (float)((rect.Bottom + rect.Top) / 2.0);
				}
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				if (this.axis.chartArea.Area3DStyle.Enable3D)
				{
					Point3D[] array2 = new Point3D[2]
					{
						new Point3D(position.X, position.Y, z),
						null
					};
					if (stringFormat.FormatFlags == StringFormatFlags.DirectionVertical)
					{
						array2[1] = new Point3D(position.X, (float)(position.Y - 20.0), z);
					}
					else
					{
						array2[1] = new Point3D((float)(position.X - 20.0), position.Y, z);
					}
					this.axis.chartArea.matrix3D.TransformPoints(array2);
					position = array2[0].PointF;
					if (num == 0 || num == 180 || num == 90 || num == 270)
					{
						if (stringFormat.FormatFlags == StringFormatFlags.DirectionVertical)
						{
							num += 90;
						}
						array2[0].PointF = graph.GetAbsolutePoint(array2[0].PointF);
						array2[1].PointF = graph.GetAbsolutePoint(array2[1].PointF);
						float num3 = (float)Math.Atan((double)((array2[1].Y - array2[0].Y) / (array2[1].X - array2[0].X)));
						num3 = (float)Math.Round(num3 * 180.0 / 3.1415927410125732);
						num += (int)num3;
					}
				}
				graph.DrawStringRel(text.Replace("\\n", "\n"), this.TitleFont, new SolidBrush(this.TitleColor), position, stringFormat, num, this.GetTextOrientation());
			}
		}

		private void Invalidate()
		{
		}
	}
}
