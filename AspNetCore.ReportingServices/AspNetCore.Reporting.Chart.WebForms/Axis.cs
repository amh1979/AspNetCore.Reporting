using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeAxis_Axis")]
	[DefaultProperty("Enabled")]
	internal class Axis : AxisLabels
	{
		internal const float elementSpacing = 1f;

		private const float maxAxisElementsSize = 75f;

		private const float maxAxisTitleSize = 20f;

		private const float maxAxisLabelRow2Size = 45f;

		private const float maxAxisMarkSize = 20f;

		public const double maxdecimal = 7.9228162514264338E+28;

		public const double smallestPositiveDecimal = 7.9228162514264341E-28;

		internal Chart chart;

		private bool storeValuesEnabled = true;

		private string name = "";

		private Font titleFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private Color titleColor = Color.Black;

		private StringAlignment titleAlignment = StringAlignment.Center;

		private string title = "";

		private int lineWidth = 1;

		private ChartDashStyle lineDashStyle = ChartDashStyle.Solid;

		private Color lineColor = Color.Black;

		private bool autoFit = true;

		private ArrowsType arrows;

		private StripLinesCollection stripLines;

		private bool nextToAxis = true;

		private TextOrientation textOrientation;

		internal float titleSize;

		internal float labelSize;

		internal float labelNearOffset;

		internal float labelFarOffset;

		internal float unRotatedLabelSize;

		internal float markSize;

		internal float scrollBarSize;

		internal float totlaGroupingLabelsSize;

		internal float[] groupingLabelSizes;

		internal float totlaGroupingLabelsSizeAdjustment;

		private LabelsAutoFitStyles labelsAutoFitStyle = LabelsAutoFitStyles.IncreaseFont | LabelsAutoFitStyles.DecreaseFont | LabelsAutoFitStyles.OffsetLabels | LabelsAutoFitStyles.LabelsAngleStep30 | LabelsAutoFitStyles.WordWrap;

		internal Font autoLabelFont;

		internal int autoLabelAngle = -1000;

		internal int autoLabelOffset = -1;

		private float aveLabelFontSize = 10f;

		private float minLabelFontSize = 5f;

		private RectangleF titlePosition = RectangleF.Empty;

		internal double minimumFromData = double.NaN;

		internal double maximumFromData = double.NaN;

		internal bool refreshMinMaxFromData = true;

		internal int numberOfPointsInAllSeries;

		private double originalViewPosition = double.NaN;

		private bool interlaced;

		private Color interlacedColor = Color.Empty;

		private double intervalOffset;

		internal double interval;

		internal DateTimeIntervalType intervalType;

		internal DateTimeIntervalType intervalOffsetType;

		internal int labelsAutoFitMinFontSize = 6;

		internal int labelsAutoFitMaxFontSize = 10;

		private string toolTip = string.Empty;

		private string href = string.Empty;

		private string mapAreaAttributes = string.Empty;

		private ChartValueTypes valueType;

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(TextOrientation.Auto)]
		[SRDescription("DescriptionAttribute_TextOrientation")]
		public TextOrientation TextOrientation
		{
			get
			{
				return this.textOrientation;
			}
			set
			{
				this.textOrientation = value;
				base.Invalidate();
			}
		}

		internal virtual string SubAxisName
		{
			get
			{
				return string.Empty;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeInterlaced")]
		public bool Interlaced
		{
			get
			{
				return this.interlaced;
			}
			set
			{
				this.interlaced = value;
				base.Invalidate();
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeInterlacedColor")]
		[NotifyParentProperty(true)]
		public Color InterlacedColor
		{
			get
			{
				return this.interlacedColor;
			}
			set
			{
				this.interlacedColor = value;
				base.Invalidate();
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeAxis_Name")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[Browsable(false)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeType")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public virtual AxisName Type
		{
			get
			{
				return base.axisType;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ArrowsType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeArrows")]
		public ArrowsType Arrows
		{
			get
			{
				return this.arrows;
			}
			set
			{
				this.arrows = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeGridTickMarks")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMajorGrid")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Grid MajorGrid
		{
			get
			{
				return base.majorGrid;
			}
			set
			{
				base.majorGrid = value;
				base.majorGrid.axis = this;
				base.majorGrid.majorGridTick = true;
				if (!base.majorGrid.intervalChanged)
				{
					base.majorGrid.Interval = double.NaN;
				}
				if (!base.majorGrid.intervalOffsetChanged)
				{
					base.majorGrid.IntervalOffset = double.NaN;
				}
				if (!base.majorGrid.intervalTypeChanged)
				{
					base.majorGrid.IntervalType = DateTimeIntervalType.NotSet;
				}
				if (!base.majorGrid.intervalOffsetTypeChanged)
				{
					base.majorGrid.IntervalOffsetType = DateTimeIntervalType.NotSet;
				}
				base.Invalidate();
			}
		}

		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMinorGrid")]
		[SRCategory("CategoryAttributeGridTickMarks")]
		public Grid MinorGrid
		{
			get
			{
				return base.minorGrid;
			}
			set
			{
				base.minorGrid = value;
				base.minorGrid.Initialize(this, false);
				base.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeGridTickMarks")]
		[SRDescription("DescriptionAttributeMajorTickMark")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public TickMark MajorTickMark
		{
			get
			{
				return base.majorTickMark;
			}
			set
			{
				base.majorTickMark = value;
				base.majorTickMark.axis = this;
				base.majorTickMark.majorGridTick = true;
				if (!base.majorTickMark.intervalChanged)
				{
					base.majorTickMark.Interval = double.NaN;
				}
				if (!base.majorTickMark.intervalOffsetChanged)
				{
					base.majorTickMark.IntervalOffset = double.NaN;
				}
				if (!base.majorTickMark.intervalTypeChanged)
				{
					base.majorTickMark.IntervalType = DateTimeIntervalType.NotSet;
				}
				if (!base.majorTickMark.intervalOffsetTypeChanged)
				{
					base.majorTickMark.IntervalOffsetType = DateTimeIntervalType.NotSet;
				}
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeGridTickMarks")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMinorTickMark")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public TickMark MinorTickMark
		{
			get
			{
				return base.minorTickMark;
			}
			set
			{
				base.minorTickMark = value;
				base.minorTickMark.Initialize(this, false);
				base.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLabelsAutoFit")]
		[SRCategory("CategoryAttributeLabels")]
		[RefreshProperties(RefreshProperties.All)]
		public bool LabelsAutoFit
		{
			get
			{
				return this.autoFit;
			}
			set
			{
				this.autoFit = value;
				base.Invalidate();
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		[DefaultValue(6)]
		[SRDescription("DescriptionAttributeLabelsAutoFitMinFontSize")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeLabels")]
		public int LabelsAutoFitMinFontSize
		{
			get
			{
				return this.labelsAutoFitMinFontSize;
			}
			set
			{
				if (value < 5)
				{
					throw new InvalidOperationException(SR.ExceptionAxisLabelsAutoFitMinFontSizeValueInvalid);
				}
				this.labelsAutoFitMinFontSize = value;
				base.Invalidate();
			}
		}

		[DefaultValue(10)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeLabels")]
		[SRDescription("DescriptionAttributeLabelsAutoFitMaxFontSize")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public int LabelsAutoFitMaxFontSize
		{
			get
			{
				return this.labelsAutoFitMaxFontSize;
			}
			set
			{
				if (value < 5)
				{
					throw new InvalidOperationException(SR.ExceptionAxisLabelsAutoFitMaxFontSizeInvalid);
				}
				this.labelsAutoFitMaxFontSize = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeLabels")]
		[Bindable(true)]
		[DefaultValue(LabelsAutoFitStyles.IncreaseFont | LabelsAutoFitStyles.DecreaseFont | LabelsAutoFitStyles.OffsetLabels | LabelsAutoFitStyles.LabelsAngleStep30 | LabelsAutoFitStyles.WordWrap)]
		[SRDescription("DescriptionAttributeLabelsAutoFitStyle")]
		[NotifyParentProperty(true)]
		public LabelsAutoFitStyles LabelsAutoFitStyle
		{
			get
			{
				return this.labelsAutoFitStyle;
			}
			set
			{
				this.labelsAutoFitStyle = value;
				base.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeMarksNextToAxis")]
		[SRCategory("CategoryAttributeAppearance")]
		public virtual bool MarksNextToAxis
		{
			get
			{
				return this.nextToAxis;
			}
			set
			{
				this.nextToAxis = value;
				base.Invalidate();
			}
		}

		[DefaultValue("")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeTitle")]
		[SRDescription("DescriptionAttributeTitle6")]
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
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTitleColor")]
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
				base.Invalidate();
			}
		}

		[DefaultValue(typeof(StringAlignment), "Center")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeTitle")]
		[SRDescription("DescriptionAttributeTitleAlignment")]
		[NotifyParentProperty(true)]
		public StringAlignment TitleAlignment
		{
			get
			{
				return this.titleAlignment;
			}
			set
			{
				this.titleAlignment = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeTitle")]
		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTitleFont5")]
		[NotifyParentProperty(true)]
		public Font TitleFont
		{
			get
			{
				return this.titleFont;
			}
			set
			{
				this.titleFont = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLineColor8")]
		[NotifyParentProperty(true)]
		public Color LineColor
		{
			get
			{
				return this.lineColor;
			}
			set
			{
				this.lineColor = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLineWidth9")]
		[NotifyParentProperty(true)]
		public int LineWidth
		{
			get
			{
				return this.lineWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAxisWidthIsNegative);
				}
				this.lineWidth = value;
				base.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLineStyle7")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		public ChartDashStyle LineStyle
		{
			get
			{
				return this.lineDashStyle;
			}
			set
			{
				this.lineDashStyle = value;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeStripLines")]
		public StripLinesCollection StripLines
		{
			get
			{
				return this.stripLines;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeToolTip")]
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

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeAxis_Href")]
		[DefaultValue("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeAxis_MapAreaAttributes")]
		[DefaultValue("")]
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

		[Bindable(true)]
		[SRCategory("CategoryAttributeInterval")]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeInterval4")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(AxisIntervalValueConverter))]
		public double Interval
		{
			get
			{
				return this.interval;
			}
			set
			{
				if (double.IsNaN(value))
				{
					this.interval = 0.0;
				}
				else
				{
					this.interval = value;
				}
				base.majorGrid.interval = base.tempMajorGridInterval;
				base.majorTickMark.interval = base.tempMajorTickMarkInterval;
				base.minorGrid.interval = base.tempMinorGridInterval;
				base.minorTickMark.interval = base.tempMinorTickMarkInterval;
				base.labelStyle.interval = base.tempLabelInterval;
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeIntervalOffset6")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(AxisIntervalValueConverter))]
		public double IntervalOffset
		{
			get
			{
				return this.intervalOffset;
			}
			set
			{
				if (double.IsNaN(value))
				{
					this.intervalOffset = 0.0;
				}
				else
				{
					this.intervalOffset = value;
				}
				base.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeInterval")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[SRDescription("DescriptionAttributeIntervalType4")]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalType
		{
			get
			{
				return this.intervalType;
			}
			set
			{
				if (value == DateTimeIntervalType.NotSet)
				{
					this.intervalType = DateTimeIntervalType.Auto;
				}
				else
				{
					this.intervalType = value;
				}
				base.majorGrid.intervalType = base.tempGridIntervalType;
				base.majorTickMark.intervalType = base.tempTickMarkIntervalType;
				base.labelStyle.intervalType = base.tempLabelIntervalType;
				base.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeIntervalOffsetType4")]
		[SRCategory("CategoryAttributeInterval")]
		[Bindable(true)]
		[DefaultValue(DateTimeIntervalType.Auto)]
		[RefreshProperties(RefreshProperties.All)]
		public DateTimeIntervalType IntervalOffsetType
		{
			get
			{
				return this.intervalOffsetType;
			}
			set
			{
				if (value == DateTimeIntervalType.NotSet)
				{
					this.intervalOffsetType = DateTimeIntervalType.Auto;
				}
				else
				{
					this.intervalOffsetType = value;
				}
				base.Invalidate();
			}
		}

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

		[SRDescription("DescriptionAttributeAxis_ValueType")]
		[Bindable(true)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeData")]
		[DefaultValue(ChartValueTypes.Auto)]
		public ChartValueTypes ValueType
		{
			get
			{
				return this.valueType;
			}
			set
			{
				this.valueType = value;
				base.Invalidate();
			}
		}

		public Axis()
		{
			this.Initialize(null, AxisName.X);
		}

		internal void Initialize(ChartArea chartArea, AxisName axisType)
		{
			base.chartArea = chartArea;
			base.axisType = axisType;
			if (chartArea != null && chartArea.Common != null && chartArea.Common.Chart != null)
			{
				this.chart = chartArea.Common.Chart;
			}
			this.SetName();
			if (base.minorTickMark == null)
			{
				base.minorTickMark = new TickMark(this, false);
			}
			if (base.majorTickMark == null)
			{
				base.majorTickMark = new TickMark(this, true);
				base.majorTickMark.Interval = double.NaN;
				base.majorTickMark.IntervalOffset = double.NaN;
				base.majorTickMark.IntervalType = DateTimeIntervalType.NotSet;
				base.majorTickMark.IntervalOffsetType = DateTimeIntervalType.NotSet;
			}
			if (base.minorGrid == null)
			{
				base.minorGrid = new Grid(this, false);
			}
			if (base.majorGrid == null)
			{
				base.majorGrid = new Grid(this, true);
				base.majorGrid.Interval = double.NaN;
				base.majorGrid.IntervalOffset = double.NaN;
				base.majorGrid.IntervalType = DateTimeIntervalType.NotSet;
				base.majorGrid.IntervalOffsetType = DateTimeIntervalType.NotSet;
			}
			if (this.stripLines == null)
			{
				this.stripLines = new StripLinesCollection(this);
			}
			base.ScrollBar.Initialize();
			if (base.scaleSegments == null)
			{
				base.scaleSegments = new AxisScaleSegmentCollection(this);
			}
			if (base.axisScaleBreakStyle == null)
			{
				base.axisScaleBreakStyle = new AxisScaleBreakStyle(this);
			}
		}

		internal void SetName()
		{
			switch (base.axisType)
			{
			case AxisName.X:
				this.name = SR.TitleAxisX;
				break;
			case AxisName.Y:
				this.name = SR.TitleAxisY;
				break;
			case AxisName.X2:
				this.name = SR.TitleAxisX2;
				break;
			case AxisName.Y2:
				this.name = SR.TitleAxisY2;
				break;
			}
		}

		private TextOrientation GetTextOrientation()
		{
			if (this.TextOrientation == TextOrientation.Auto)
			{
				if (this.AxisPosition == AxisPosition.Left)
				{
					return TextOrientation.Rotated270;
				}
				if (this.AxisPosition == AxisPosition.Right)
				{
					return TextOrientation.Rotated90;
				}
				return TextOrientation.Horizontal;
			}
			return this.TextOrientation;
		}

		internal void PrePaint(ChartGraphics graph)
		{
			if (base.enabled)
			{
				base.majorTickMark.Paint(graph, true);
				base.minorTickMark.Paint(graph, true);
				this.DrawAxisLine(graph, true);
				base.labelStyle.Paint(graph, true);
			}
		}

		internal void Paint(ChartGraphics graph)
		{
			if (base.chartArea != null && base.chartArea.chartAreaIsCurcular)
			{
				if (base.axisType == AxisName.Y && base.enabled)
				{
					ICircularChartType circularChartType = base.chartArea.GetCircularChartType();
					if (circularChartType != null)
					{
						Matrix transform = graph.Transform;
						float[] yAxisLocations = circularChartType.GetYAxisLocations(base.chartArea);
						bool flag = true;
						float[] array = yAxisLocations;
						foreach (float num in array)
						{
							Matrix matrix = transform.Clone();
							matrix.RotateAt(num, graph.GetAbsolutePoint(base.chartArea.circularCenter));
							graph.Transform = matrix;
							base.minorTickMark.Paint(graph, false);
							base.majorTickMark.Paint(graph, false);
							this.DrawAxisLine(graph, false);
							if (flag)
							{
								flag = false;
								int fontAngle = base.labelStyle.FontAngle;
								if (base.labelStyle.FontAngle == 0)
								{
									if (num >= 45.0 && num <= 180.0)
									{
										base.labelStyle.fontAngle = -90;
									}
									else if (num > 180.0 && num <= 315.0)
									{
										base.labelStyle.fontAngle = 90;
									}
								}
								base.labelStyle.Paint(graph, false);
								base.labelStyle.fontAngle = fontAngle;
							}
						}
						graph.Transform = transform;
					}
				}
				if (base.axisType == AxisName.X && base.enabled)
				{
					base.labelStyle.PaintCircular(graph);
				}
				this.DrawAxisTitle(graph);
			}
			else
			{
				if (base.enabled)
				{
					base.minorTickMark.Paint(graph, false);
					base.majorTickMark.Paint(graph, false);
					this.DrawAxisLine(graph, false);
					base.labelStyle.Paint(graph, false);
					if (base.chartArea != null && !base.chartArea.Area3DStyle.Enable3D)
					{
						base.ScrollBar.Paint(graph);
					}
				}
				this.DrawAxisTitle(graph);
				base.ResetTempAxisOffset();
			}
		}

		internal void PaintOnSegmentedScalePassOne(ChartGraphics graph)
		{
			if (base.enabled)
			{
				base.minorTickMark.Paint(graph, false);
				base.majorTickMark.Paint(graph, false);
			}
		}

		internal void PaintOnSegmentedScalePassTwo(ChartGraphics graph)
		{
			if (base.enabled)
			{
				this.DrawAxisLine(graph, false);
				base.labelStyle.Paint(graph, false);
			}
			this.DrawAxisTitle(graph);
			base.ResetTempAxisOffset();
		}

		private void DrawAxisTitle(ChartGraphics graph)
		{
			if (base.enabled && this.Title.Length > 0)
			{
				Matrix matrix = null;
				if (base.chartArea.Area3DStyle.Enable3D && !base.chartArea.chartAreaIsCurcular)
				{
					this.DrawAxis3DTitle(graph);
				}
				else
				{
					string text = this.Title;
					if (this.chart != null && this.chart.LocalizeTextHandler != null)
					{
						text = this.chart.LocalizeTextHandler(this, text, 0, ChartElementType.Axis);
					}
					float num = (float)this.GetAxisPosition();
					if (this.AxisPosition == AxisPosition.Bottom)
					{
						if (!this.IsMarksNextToAxis())
						{
							num = base.chartArea.PlotAreaPosition.Bottom();
						}
						num = base.chartArea.PlotAreaPosition.Bottom() - num;
					}
					else if (this.AxisPosition == AxisPosition.Top)
					{
						if (!this.IsMarksNextToAxis())
						{
							num = base.chartArea.PlotAreaPosition.Y;
						}
						num -= base.chartArea.PlotAreaPosition.Y;
					}
					else if (this.AxisPosition == AxisPosition.Right)
					{
						if (!this.IsMarksNextToAxis())
						{
							num = base.chartArea.PlotAreaPosition.Right();
						}
						num = base.chartArea.PlotAreaPosition.Right() - num;
					}
					else if (this.AxisPosition == AxisPosition.Left)
					{
						if (!this.IsMarksNextToAxis())
						{
							num = base.chartArea.PlotAreaPosition.X;
						}
						num -= base.chartArea.PlotAreaPosition.X;
					}
					float num2 = this.markSize + this.labelSize;
					num2 -= num;
					if (num2 < 0.0)
					{
						num2 = 0f;
					}
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = this.TitleAlignment;
					stringFormat.Trimming = StringTrimming.EllipsisCharacter;
					stringFormat.FormatFlags |= StringFormatFlags.FitBlackBox;
					this.titlePosition = base.chartArea.PlotAreaPosition.ToRectangleF();
					float num3 = (float)(this.titleSize - 1.0);
					if (this.AxisPosition == AxisPosition.Left)
					{
						this.titlePosition.X = base.chartArea.PlotAreaPosition.X - num3 - num2;
						this.titlePosition.Y = base.chartArea.PlotAreaPosition.Y;
						if (!this.IsTextVertical)
						{
							SizeF sizeF = new SizeF(num3, base.chartArea.PlotAreaPosition.Height);
							this.titlePosition.Width = sizeF.Width;
							this.titlePosition.Height = sizeF.Height;
							stringFormat.Alignment = StringAlignment.Center;
							if (this.TitleAlignment == StringAlignment.Far)
							{
								stringFormat.LineAlignment = StringAlignment.Near;
							}
							else if (this.TitleAlignment == StringAlignment.Near)
							{
								stringFormat.LineAlignment = StringAlignment.Far;
							}
							else
							{
								stringFormat.LineAlignment = StringAlignment.Center;
							}
						}
						else
						{
							SizeF sizeF2 = graph.GetAbsoluteSize(new SizeF(num3, base.chartArea.PlotAreaPosition.Height));
							sizeF2 = graph.GetRelativeSize(new SizeF(sizeF2.Height, sizeF2.Width));
							this.titlePosition.Width = sizeF2.Width;
							this.titlePosition.Height = sizeF2.Height;
							this.titlePosition.Y += (float)(base.chartArea.PlotAreaPosition.Height / 2.0 - this.titlePosition.Height / 2.0);
							this.titlePosition.X += (float)(this.titleSize / 2.0 - this.titlePosition.Width / 2.0);
							matrix = this.SetRotationTransformation(graph, this.titlePosition);
							stringFormat.LineAlignment = StringAlignment.Center;
						}
					}
					else if (this.AxisPosition == AxisPosition.Right)
					{
						this.titlePosition.X = base.chartArea.PlotAreaPosition.Right() + num2;
						this.titlePosition.Y = base.chartArea.PlotAreaPosition.Y;
						if (!this.IsTextVertical)
						{
							SizeF sizeF3 = new SizeF(num3, base.chartArea.PlotAreaPosition.Height);
							this.titlePosition.Width = sizeF3.Width;
							this.titlePosition.Height = sizeF3.Height;
							stringFormat.Alignment = StringAlignment.Center;
							if (this.TitleAlignment == StringAlignment.Far)
							{
								stringFormat.LineAlignment = StringAlignment.Near;
							}
							else if (this.TitleAlignment == StringAlignment.Near)
							{
								stringFormat.LineAlignment = StringAlignment.Far;
							}
							else
							{
								stringFormat.LineAlignment = StringAlignment.Center;
							}
						}
						else
						{
							SizeF sizeF4 = graph.GetAbsoluteSize(new SizeF(num3, base.chartArea.PlotAreaPosition.Height));
							sizeF4 = graph.GetRelativeSize(new SizeF(sizeF4.Height, sizeF4.Width));
							this.titlePosition.Width = sizeF4.Width;
							this.titlePosition.Height = sizeF4.Height;
							this.titlePosition.Y += (float)(base.chartArea.PlotAreaPosition.Height / 2.0 - this.titlePosition.Height / 2.0);
							this.titlePosition.X += (float)(this.titleSize / 2.0 - this.titlePosition.Width / 2.0);
							matrix = this.SetRotationTransformation(graph, this.titlePosition);
							stringFormat.LineAlignment = StringAlignment.Center;
						}
					}
					else if (this.AxisPosition == AxisPosition.Top)
					{
						this.titlePosition.Y = base.chartArea.PlotAreaPosition.Y - num3 - num2;
						this.titlePosition.Height = num3;
						this.titlePosition.X = base.chartArea.PlotAreaPosition.X;
						this.titlePosition.Width = base.chartArea.PlotAreaPosition.Width;
						if (this.IsTextVertical)
						{
							matrix = this.SetRotationTransformation(graph, this.titlePosition);
						}
						stringFormat.LineAlignment = StringAlignment.Center;
					}
					else if (this.AxisPosition == AxisPosition.Bottom)
					{
						this.titlePosition.Y = base.chartArea.PlotAreaPosition.Bottom() + num2;
						this.titlePosition.Height = num3;
						this.titlePosition.X = base.chartArea.PlotAreaPosition.X;
						this.titlePosition.Width = base.chartArea.PlotAreaPosition.Width;
						if (this.IsTextVertical)
						{
							matrix = this.SetRotationTransformation(graph, this.titlePosition);
						}
						stringFormat.LineAlignment = StringAlignment.Center;
					}
					graph.DrawStringRel(text.Replace("\\n", "\n"), this.TitleFont, new SolidBrush(this.TitleColor), this.titlePosition, stringFormat, this.GetTextOrientation());
					if (base.Common.ProcessModeRegions)
					{
						RectangleF rectArea = graph.GetAbsoluteRectangle(this.titlePosition);
						PointF[] array = new PointF[2]
						{
							new PointF(rectArea.X, rectArea.Y),
							new PointF(rectArea.Right, rectArea.Bottom)
						};
						graph.Transform.TransformPoints(array);
						rectArea = new RectangleF(array[0].X, array[0].Y, array[1].X - array[0].X, array[1].Y - array[0].Y);
						if (rectArea.Width < 0.0)
						{
							rectArea.Width = Math.Abs(rectArea.Width);
							rectArea.X -= rectArea.Width;
						}
						if (rectArea.Height < 0.0)
						{
							rectArea.Height = Math.Abs(rectArea.Height);
							rectArea.Y -= rectArea.Height;
						}
						base.Common.HotRegionsList.AddHotRegion(rectArea, this, ChartElementType.AxisTitle, false, false);
					}
					if (matrix != null)
					{
						graph.Transform = matrix;
					}
				}
			}
		}

		private Matrix SetRotationTransformation(ChartGraphics graph, RectangleF titlePosition)
		{
			Matrix result = graph.Transform.Clone();
			PointF empty = PointF.Empty;
			empty.X = (float)(titlePosition.X + titlePosition.Width / 2.0);
			empty.Y = (float)(titlePosition.Y + titlePosition.Height / 2.0);
			float angle = (float)((this.GetTextOrientation() == TextOrientation.Rotated90) ? 90.0 : -90.0);
			Matrix matrix = graph.Transform.Clone();
			matrix.RotateAt(angle, graph.GetAbsolutePoint(empty));
			graph.Transform = matrix;
			return result;
		}

		internal void DrawRadialLine(object obj, ChartGraphics graph, Color color, int width, ChartDashStyle style, double position)
		{
			RectangleF rectangleF = base.chartArea.PlotAreaPosition.ToRectangleF();
			rectangleF = graph.GetAbsoluteRectangle(rectangleF);
			if (rectangleF.Width != rectangleF.Height)
			{
				if (rectangleF.Width > rectangleF.Height)
				{
					rectangleF.X += (float)((rectangleF.Width - rectangleF.Height) / 2.0);
					rectangleF.Width = rectangleF.Height;
				}
				else
				{
					rectangleF.Y += (float)((rectangleF.Height - rectangleF.Width) / 2.0);
					rectangleF.Height = rectangleF.Width;
				}
			}
			float angle = base.chartArea.CircularPositionToAngle(position);
			Region clip = null;
			if (base.chartArea.CircularUsePolygons)
			{
				clip = graph.Clip;
				graph.Clip = new Region(graph.GetPolygonCirclePath(rectangleF, base.chartArea.CircularSectorsNumber));
			}
			PointF absolutePoint = graph.GetAbsolutePoint(base.chartArea.circularCenter);
			Matrix transform = graph.Transform;
			Matrix matrix = transform.Clone();
			matrix.RotateAt(angle, absolutePoint);
			graph.Transform = matrix;
			PointF pointF = new PointF((float)(rectangleF.X + rectangleF.Width / 2.0), rectangleF.Y);
			graph.DrawLineAbs(color, width, style, absolutePoint, pointF);
			if (base.Common.ProcessModeRegions)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddLine(absolutePoint, pointF);
				graphicsPath.Transform(matrix);
				try
				{
					using (Pen pen = new Pen(Color.Black, (float)(width + 2)))
					{
						ChartGraphics.Widen(graphicsPath, pen);
						base.Common.HotRegionsList.AddHotRegion(graphicsPath, false, graph, ChartElementType.Gridlines, obj);
					}
				}
				catch
				{
				}
			}
			graph.Transform = transform;
			matrix.Dispose();
			if (base.chartArea.CircularUsePolygons)
			{
				graph.Clip = clip;
			}
		}

		internal void DrawCircularLine(object obj, ChartGraphics graph, Color color, int width, ChartDashStyle style, float position)
		{
			RectangleF rectangleF = base.chartArea.PlotAreaPosition.ToRectangleF();
			rectangleF = graph.GetAbsoluteRectangle(rectangleF);
			if (rectangleF.Width != rectangleF.Height)
			{
				if (rectangleF.Width > rectangleF.Height)
				{
					rectangleF.X += (float)((rectangleF.Width - rectangleF.Height) / 2.0);
					rectangleF.Width = rectangleF.Height;
				}
				else
				{
					rectangleF.Y += (float)((rectangleF.Height - rectangleF.Width) / 2.0);
					rectangleF.Height = rectangleF.Width;
				}
			}
			float num = graph.GetAbsolutePoint(new PointF(position, position)).Y - rectangleF.Top;
			rectangleF.Inflate((float)(0.0 - num), (float)(0.0 - num));
			Pen pen = new Pen(color, (float)width);
			pen.DashStyle = graph.GetPenStyle(style);
			if (base.chartArea.CircularUsePolygons)
			{
				graph.DrawCircleAbs(pen, null, rectangleF, base.chartArea.CircularSectorsNumber, false);
			}
			else
			{
				graph.DrawEllipse(pen, rectangleF);
			}
			if (base.Common.ProcessModeRegions && rectangleF.Width >= 1.0 && rectangleF.Height > 1.0)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				if (base.chartArea.CircularUsePolygons)
				{
					graphicsPath = graph.GetPolygonCirclePath(rectangleF, base.chartArea.CircularSectorsNumber);
				}
				else
				{
					graphicsPath.AddEllipse(rectangleF);
				}
				try
				{
					pen.Width += 2f;
					ChartGraphics.Widen(graphicsPath, pen);
					base.Common.HotRegionsList.AddHotRegion(graphicsPath, false, graph, ChartElementType.Gridlines, obj);
				}
				catch
				{
				}
			}
		}

		private void DrawAxis3DTitle(ChartGraphics graph)
		{
			if (base.enabled)
			{
				string text = this.Title;
				if (this.chart != null && this.chart.LocalizeTextHandler != null)
				{
					text = this.chart.LocalizeTextHandler(this, text, 0, ChartElementType.Axis);
				}
				PointF pointF = PointF.Empty;
				int num = 0;
				StringFormat stringFormat = new StringFormat();
				stringFormat.Alignment = this.TitleAlignment;
				stringFormat.Trimming = StringTrimming.EllipsisCharacter;
				stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
				SizeF sizeF = graph.MeasureString(text.Replace("\\n", "\n"), this.TitleFont, new SizeF(10000f, 10000f), stringFormat, this.GetTextOrientation());
				SizeF size = SizeF.Empty;
				if (stringFormat.Alignment != StringAlignment.Center)
				{
					size = sizeF;
					if (this.IsTextVertical)
					{
						float height = size.Height;
						size.Height = size.Width;
						size.Width = height;
					}
					size = graph.GetRelativeSize(size);
					if (base.chartArea.reverseSeriesOrder)
					{
						if (stringFormat.Alignment == StringAlignment.Near)
						{
							stringFormat.Alignment = StringAlignment.Far;
						}
						else
						{
							stringFormat.Alignment = StringAlignment.Near;
						}
					}
				}
				if (this.GetTextOrientation() == TextOrientation.Rotated90)
				{
					num = 90;
				}
				else if (this.GetTextOrientation() == TextOrientation.Rotated270)
				{
					num = -90;
				}
				if (this.AxisPosition == AxisPosition.Left)
				{
					pointF = new PointF(base.chartArea.PlotAreaPosition.X, (float)(base.chartArea.PlotAreaPosition.Y + base.chartArea.PlotAreaPosition.Height / 2.0));
					if (stringFormat.Alignment == StringAlignment.Near)
					{
						pointF.Y = (float)(base.chartArea.PlotAreaPosition.Bottom() - size.Height / 2.0);
					}
					else if (stringFormat.Alignment == StringAlignment.Far)
					{
						pointF.Y = (float)(base.chartArea.PlotAreaPosition.Y + size.Height / 2.0);
					}
				}
				else if (this.AxisPosition == AxisPosition.Right)
				{
					pointF = new PointF(base.chartArea.PlotAreaPosition.Right(), (float)(base.chartArea.PlotAreaPosition.Y + base.chartArea.PlotAreaPosition.Height / 2.0));
					if (stringFormat.Alignment == StringAlignment.Near)
					{
						pointF.Y = (float)(base.chartArea.PlotAreaPosition.Bottom() - size.Height / 2.0);
					}
					else if (stringFormat.Alignment == StringAlignment.Far)
					{
						pointF.Y = (float)(base.chartArea.PlotAreaPosition.Y + size.Height / 2.0);
					}
				}
				else if (this.AxisPosition == AxisPosition.Top)
				{
					pointF = new PointF((float)(base.chartArea.PlotAreaPosition.X + base.chartArea.PlotAreaPosition.Width / 2.0), base.chartArea.PlotAreaPosition.Y);
					if (stringFormat.Alignment == StringAlignment.Near)
					{
						pointF.X = (float)(base.chartArea.PlotAreaPosition.X + size.Width / 2.0);
					}
					else if (stringFormat.Alignment == StringAlignment.Far)
					{
						pointF.X = (float)(base.chartArea.PlotAreaPosition.Right() - size.Width / 2.0);
					}
				}
				else if (this.AxisPosition == AxisPosition.Bottom)
				{
					pointF = new PointF((float)(base.chartArea.PlotAreaPosition.X + base.chartArea.PlotAreaPosition.Width / 2.0), base.chartArea.PlotAreaPosition.Bottom());
					if (stringFormat.Alignment == StringAlignment.Near)
					{
						pointF.X = (float)(base.chartArea.PlotAreaPosition.X + size.Width / 2.0);
					}
					else if (stringFormat.Alignment == StringAlignment.Far)
					{
						pointF.X = (float)(base.chartArea.PlotAreaPosition.Right() - size.Width / 2.0);
					}
				}
				bool flag = false;
				float marksZPosition = this.GetMarksZPosition(out flag);
				Point3D[] array = null;
				float num2 = 0f;
				if (this.AxisPosition == AxisPosition.Top || this.AxisPosition == AxisPosition.Bottom)
				{
					array = new Point3D[2]
					{
						new Point3D(pointF.X, pointF.Y, marksZPosition),
						new Point3D((float)(pointF.X - 20.0), pointF.Y, marksZPosition)
					};
					base.chartArea.matrix3D.TransformPoints(array);
					pointF = array[0].PointF;
					array[0].PointF = graph.GetAbsolutePoint(array[0].PointF);
					array[1].PointF = graph.GetAbsolutePoint(array[1].PointF);
					num2 = (float)Math.Atan((double)((array[1].Y - array[0].Y) / (array[1].X - array[0].X)));
				}
				else
				{
					array = new Point3D[2]
					{
						new Point3D(pointF.X, pointF.Y, marksZPosition),
						new Point3D(pointF.X, (float)(pointF.Y - 20.0), marksZPosition)
					};
					base.chartArea.matrix3D.TransformPoints(array);
					pointF = array[0].PointF;
					array[0].PointF = graph.GetAbsolutePoint(array[0].PointF);
					array[1].PointF = graph.GetAbsolutePoint(array[1].PointF);
					if (array[1].Y != array[0].Y)
					{
						num2 = (float)(0.0 - (float)Math.Atan((double)((array[1].X - array[0].X) / (array[1].Y - array[0].Y))));
					}
				}
				num += (int)Math.Round(num2 * 180.0 / 3.1415927410125732);
				float num3 = (float)(this.labelSize + this.markSize + this.titleSize / 2.0);
				float num4 = 0f;
				float num5 = 0f;
				if (this.AxisPosition == AxisPosition.Left)
				{
					num4 = (float)((double)num3 * Math.Cos((double)num2));
					pointF.X -= num4;
				}
				else if (this.AxisPosition == AxisPosition.Right)
				{
					num4 = (float)((double)num3 * Math.Cos((double)num2));
					pointF.X += num4;
				}
				else if (this.AxisPosition == AxisPosition.Top)
				{
					num5 = (float)((double)num3 * Math.Cos((double)num2));
					num4 = (float)((double)num3 * Math.Sin((double)num2));
					pointF.Y -= num5;
					if (num5 > 0.0)
					{
						pointF.X += num4;
					}
					else
					{
						pointF.X -= num4;
					}
				}
				else if (this.AxisPosition == AxisPosition.Bottom)
				{
					num5 = (float)((double)num3 * Math.Cos((double)num2));
					num4 = (float)((double)num3 * Math.Sin((double)num2));
					pointF.Y += num5;
					if (num5 > 0.0)
					{
						pointF.X -= num4;
					}
					else
					{
						pointF.X += num4;
					}
				}
				stringFormat.LineAlignment = StringAlignment.Center;
				stringFormat.Alignment = StringAlignment.Center;
				if (!pointF.IsEmpty && !float.IsNaN(pointF.X) && !float.IsNaN(pointF.Y))
				{
					graph.DrawStringRel(text.Replace("\\n", "\n"), this.TitleFont, new SolidBrush(this.TitleColor), pointF, stringFormat, num, this.GetTextOrientation());
					if (base.Common.ProcessModeRegions)
					{
						GraphicsPath tranformedTextRectPath = graph.GetTranformedTextRectPath(pointF, sizeF, num);
						base.Common.HotRegionsList.AddHotRegion(tranformedTextRectPath, false, graph, ChartElementType.AxisTitle, this);
					}
				}
			}
		}

		internal void DrawAxisLine(ChartGraphics graph, bool backElements)
		{
			this.DrawAxisLine(graph, false, backElements);
		}

		internal void DrawAxisLine(ChartGraphics graph, bool selectionMode, bool backElements)
		{
			ArrowOrientation arrowOrientation = ArrowOrientation.Top;
			PointF pointF = Point.Empty;
			PointF pointF2 = Point.Empty;
			switch (this.AxisPosition)
			{
			case AxisPosition.Left:
				pointF.X = (float)this.GetAxisPosition();
				pointF.Y = base.PlotAreaPosition.Bottom();
				pointF2.X = (float)this.GetAxisPosition();
				pointF2.Y = base.PlotAreaPosition.Y;
				arrowOrientation = (ArrowOrientation)((!base.reverse) ? 2 : 3);
				break;
			case AxisPosition.Right:
				pointF.X = (float)this.GetAxisPosition();
				pointF.Y = base.PlotAreaPosition.Bottom();
				pointF2.X = (float)this.GetAxisPosition();
				pointF2.Y = base.PlotAreaPosition.Y;
				arrowOrientation = (ArrowOrientation)((!base.reverse) ? 2 : 3);
				break;
			case AxisPosition.Bottom:
				pointF.X = base.PlotAreaPosition.X;
				pointF.Y = (float)this.GetAxisPosition();
				pointF2.X = base.PlotAreaPosition.Right();
				pointF2.Y = (float)this.GetAxisPosition();
				arrowOrientation = (ArrowOrientation)((!base.reverse) ? 1 : 0);
				break;
			case AxisPosition.Top:
				pointF.X = base.PlotAreaPosition.X;
				pointF.Y = (float)this.GetAxisPosition();
				pointF2.X = base.PlotAreaPosition.Right();
				pointF2.Y = (float)this.GetAxisPosition();
				arrowOrientation = (ArrowOrientation)((!base.reverse) ? 1 : 0);
				break;
			}
			if (base.chartArea.chartAreaIsCurcular)
			{
				pointF.Y = (float)(base.PlotAreaPosition.Y + base.PlotAreaPosition.Height / 2.0);
			}
			if (base.Common.ProcessModeRegions)
			{
				if (base.chartArea.chartAreaIsCurcular)
				{
					GraphicsPath graphicsPath = new GraphicsPath();
					PointF absolutePoint = graph.GetAbsolutePoint(pointF);
					PointF absolutePoint2 = graph.GetAbsolutePoint(pointF2);
					if (this.AxisPosition == AxisPosition.Bottom)
					{
						graphicsPath.AddLine(absolutePoint.X, (float)(absolutePoint.Y - 1.5), absolutePoint2.X, (float)(absolutePoint2.Y - 1.5));
						graphicsPath.AddLine(absolutePoint2.X, (float)(absolutePoint2.Y + 1.5), absolutePoint.X, (float)(absolutePoint.Y + 1.5));
						graphicsPath.CloseAllFigures();
					}
					else if (this.AxisPosition == AxisPosition.Top)
					{
						graphicsPath.AddLine(absolutePoint.X, (float)(absolutePoint.Y - 1.5), absolutePoint2.X, (float)(absolutePoint2.Y - 1.5));
						graphicsPath.AddLine(absolutePoint2.X, (float)(absolutePoint2.Y + 1.5), absolutePoint.X, (float)(absolutePoint.Y + 1.5));
						graphicsPath.CloseAllFigures();
					}
					else if (this.AxisPosition == AxisPosition.Left)
					{
						graphicsPath.AddLine((float)(absolutePoint.X - 1.5), absolutePoint.Y, (float)(absolutePoint2.X - 1.5), absolutePoint2.Y);
						graphicsPath.AddLine((float)(absolutePoint2.X + 1.5), absolutePoint2.Y, (float)(absolutePoint.X + 1.5), absolutePoint.Y);
						graphicsPath.CloseAllFigures();
					}
					else if (this.AxisPosition == AxisPosition.Right)
					{
						graphicsPath.AddLine((float)(absolutePoint.X - 1.5), absolutePoint.Y, (float)(absolutePoint2.X - 1.5), absolutePoint2.Y);
						graphicsPath.AddLine((float)(absolutePoint2.X + 1.5), absolutePoint2.Y, (float)(absolutePoint.X + 1.5), absolutePoint.Y);
						graphicsPath.CloseAllFigures();
					}
					graphicsPath.Transform(graph.Transform);
					base.Common.HotRegionsList.AddHotRegion(graph, graphicsPath, false, this.toolTip, this.href, this.mapAreaAttributes, this, ChartElementType.Axis);
				}
				else if (!base.chartArea.Area3DStyle.Enable3D)
				{
					GraphicsPath graphicsPath2 = new GraphicsPath();
					PointF absolutePoint3 = graph.GetAbsolutePoint(pointF);
					PointF absolutePoint4 = graph.GetAbsolutePoint(pointF2);
					if (this.AxisPosition == AxisPosition.Bottom)
					{
						graphicsPath2.AddLine(absolutePoint3.X, (float)(absolutePoint3.Y - 1.5), absolutePoint4.X, (float)(absolutePoint4.Y - 1.5));
						graphicsPath2.AddLine(absolutePoint4.X, (float)(absolutePoint4.Y + 1.5), absolutePoint3.X, (float)(absolutePoint3.Y + 1.5));
						graphicsPath2.CloseAllFigures();
					}
					else if (this.AxisPosition == AxisPosition.Top)
					{
						graphicsPath2.AddLine(absolutePoint3.X, (float)(absolutePoint3.Y - 1.5), absolutePoint4.X, (float)(absolutePoint4.Y - 1.5));
						graphicsPath2.AddLine(absolutePoint4.X, (float)(absolutePoint4.Y + 1.5), absolutePoint3.X, (float)(absolutePoint3.Y + 1.5));
						graphicsPath2.CloseAllFigures();
					}
					else if (this.AxisPosition == AxisPosition.Left)
					{
						graphicsPath2.AddLine((float)(absolutePoint3.X - 1.5), absolutePoint3.Y, (float)(absolutePoint4.X - 1.5), absolutePoint4.Y);
						graphicsPath2.AddLine((float)(absolutePoint4.X + 1.5), absolutePoint4.Y, (float)(absolutePoint3.X + 1.5), absolutePoint3.Y);
						graphicsPath2.CloseAllFigures();
					}
					else if (this.AxisPosition == AxisPosition.Right)
					{
						graphicsPath2.AddLine((float)(absolutePoint3.X - 1.5), absolutePoint3.Y, (float)(absolutePoint4.X - 1.5), absolutePoint4.Y);
						graphicsPath2.AddLine((float)(absolutePoint4.X + 1.5), absolutePoint4.Y, (float)(absolutePoint3.X + 1.5), absolutePoint3.Y);
						graphicsPath2.CloseAllFigures();
					}
					base.Common.HotRegionsList.AddHotRegion(graph, graphicsPath2, false, this.toolTip, this.href, this.mapAreaAttributes, this, ChartElementType.Axis);
				}
				else
				{
					this.Draw3DAxisLine(graph, pointF, pointF2, this.AxisPosition == AxisPosition.Top || this.AxisPosition == AxisPosition.Bottom, backElements, false);
				}
			}
			if (base.Common.ProcessModePaint)
			{
				if (!base.chartArea.Area3DStyle.Enable3D || base.chartArea.chartAreaIsCurcular)
				{
					graph.StartHotRegion(this.href, this.toolTip);
					this.InitAnimation(graph, pointF, pointF2);
					graph.StartAnimation();
					graph.DrawLineRel(this.lineColor, this.lineWidth, this.lineDashStyle, pointF, pointF2);
					graph.StopAnimation();
					graph.EndHotRegion();
					Axis axis;
					switch (arrowOrientation)
					{
					case ArrowOrientation.Left:
						axis = base.chartArea.AxisX;
						break;
					case ArrowOrientation.Right:
						axis = base.chartArea.AxisX2;
						break;
					case ArrowOrientation.Top:
						axis = base.chartArea.AxisY2;
						break;
					case ArrowOrientation.Bottom:
						axis = base.chartArea.AxisY;
						break;
					default:
						axis = base.chartArea.AxisX;
						break;
					}
					PointF position = (!base.reverse) ? pointF2 : pointF;
					this.InitAnimation(graph, pointF, pointF2);
					graph.StartAnimation();
					graph.DrawArrowRel(position, arrowOrientation, this.arrows, this.lineColor, this.lineWidth, this.lineDashStyle, (double)axis.majorTickMark.Size, (double)this.lineWidth);
					graph.StopAnimation();
				}
				else
				{
					this.Draw3DAxisLine(graph, pointF, pointF2, this.AxisPosition == AxisPosition.Top || this.AxisPosition == AxisPosition.Bottom, backElements, false);
				}
			}
		}

		private void InitAnimation(ChartGraphics graph, PointF firstPoint, PointF secondPoint)
		{
		}

		internal void Draw3DAxisLine(ChartGraphics graph, PointF point1, PointF point2, bool horizontal, bool backElements, bool selectionMode)
		{
			bool flag = this.IsAxisOnAreaEdge();
			bool flag2 = flag;
			if ((!flag2 || this.MajorTickMark.Style != TickMarkStyle.Cross) && this.MajorTickMark.Style != TickMarkStyle.Inside && this.MinorTickMark.Style != TickMarkStyle.Cross && this.MinorTickMark.Style != TickMarkStyle.Inside)
			{
				goto IL_0046;
			}
			flag2 = false;
			goto IL_0046;
			IL_0046:
			if (horizontal && point1.X > point2.X)
			{
				goto IL_006e;
			}
			if (!horizontal && point1.Y > point2.Y)
			{
				goto IL_006e;
			}
			goto IL_00a2;
			IL_00a2:
			float z = (float)(base.chartArea.IsMainSceneWallOnFront() ? base.chartArea.areaSceneDepth : 0.0);
			SurfaceNames surfaceNames = (SurfaceNames)(base.chartArea.IsMainSceneWallOnFront() ? 1 : 2);
			if (base.chartArea.ShouldDrawOnSurface(SurfaceNames.Back, backElements, flag2))
			{
				graph.StartHotRegion(this.href, this.toolTip);
				graph.StartAnimation();
				graph.Draw3DLine(base.chartArea.matrix3D, this.lineColor, this.lineWidth, this.lineDashStyle, new Point3D(point1.X, point1.Y, z), new Point3D(point2.X, point2.Y, z), base.Common, this, ChartElementType.Axis);
				graph.StopAnimation();
				graph.EndHotRegion();
			}
			z = (float)(base.chartArea.IsMainSceneWallOnFront() ? 0.0 : base.chartArea.areaSceneDepth);
			surfaceNames = (SurfaceNames)((!base.chartArea.IsMainSceneWallOnFront()) ? 1 : 2);
			if (base.chartArea.ShouldDrawOnSurface(surfaceNames, backElements, flag2) && (!flag || (this.AxisPosition == AxisPosition.Bottom && base.chartArea.IsBottomSceneWallVisible()) || (this.AxisPosition == AxisPosition.Left && base.chartArea.IsSideSceneWallOnLeft()) || (this.AxisPosition == AxisPosition.Right && !base.chartArea.IsSideSceneWallOnLeft())))
			{
				graph.StartHotRegion(this.href, this.toolTip);
				graph.StartAnimation();
				graph.Draw3DLine(base.chartArea.matrix3D, this.lineColor, this.lineWidth, this.lineDashStyle, new Point3D(point1.X, point1.Y, z), new Point3D(point2.X, point2.Y, z), base.Common, this, ChartElementType.Axis);
				graph.StopAnimation();
				graph.EndHotRegion();
			}
			SurfaceNames surfaceName = (SurfaceNames)((this.AxisPosition == AxisPosition.Left || this.AxisPosition == AxisPosition.Right) ? 16 : 4);
			if (base.chartArea.ShouldDrawOnSurface(surfaceName, backElements, flag2) && (!flag || (this.AxisPosition == AxisPosition.Bottom && (base.chartArea.IsBottomSceneWallVisible() || base.chartArea.IsSideSceneWallOnLeft())) || (this.AxisPosition == AxisPosition.Left && base.chartArea.IsSideSceneWallOnLeft()) || (this.AxisPosition == AxisPosition.Right && !base.chartArea.IsSideSceneWallOnLeft()) || (this.AxisPosition == AxisPosition.Top && base.chartArea.IsSideSceneWallOnLeft())))
			{
				graph.StartHotRegion(this.href, this.toolTip);
				graph.StartAnimation();
				graph.Draw3DLine(base.chartArea.matrix3D, this.lineColor, this.lineWidth, this.lineDashStyle, new Point3D(point1.X, point1.Y, base.chartArea.areaSceneDepth), new Point3D(point1.X, point1.Y, 0f), base.Common, this, ChartElementType.Axis);
				graph.StopAnimation();
				graph.EndHotRegion();
			}
			surfaceName = (SurfaceNames)((this.AxisPosition == AxisPosition.Left || this.AxisPosition == AxisPosition.Right) ? 32 : 8);
			if (base.chartArea.ShouldDrawOnSurface(surfaceName, backElements, flag2))
			{
				if (flag && (this.AxisPosition != AxisPosition.Bottom || (!base.chartArea.IsBottomSceneWallVisible() && base.chartArea.IsSideSceneWallOnLeft())) && (this.AxisPosition != 0 || (!base.chartArea.IsSideSceneWallOnLeft() && !base.chartArea.IsBottomSceneWallVisible())) && (this.AxisPosition != AxisPosition.Right || (base.chartArea.IsSideSceneWallOnLeft() && !base.chartArea.IsBottomSceneWallVisible())))
				{
					if (this.AxisPosition != AxisPosition.Top)
					{
						return;
					}
					if (base.chartArea.IsSideSceneWallOnLeft())
					{
						return;
					}
				}
				graph.StartHotRegion(this.href, this.toolTip);
				graph.StartAnimation();
				graph.Draw3DLine(base.chartArea.matrix3D, this.lineColor, this.lineWidth, this.lineDashStyle, new Point3D(point2.X, point2.Y, base.chartArea.areaSceneDepth), new Point3D(point2.X, point2.Y, 0f), base.Common, this, ChartElementType.Axis);
				graph.StopAnimation();
				graph.EndHotRegion();
			}
			return;
			IL_006e:
			PointF pointF = new PointF(point1.X, point1.Y);
			point1.X = point2.X;
			point1.Y = point2.Y;
			point2 = pointF;
			goto IL_00a2;
		}

		internal float GetMarksZPosition(out bool axisOnEdge)
		{
			axisOnEdge = this.IsAxisOnAreaEdge();
			if (!this.IsMarksNextToAxis())
			{
				axisOnEdge = true;
			}
			float num = 0f;
			if (this.AxisPosition == AxisPosition.Bottom && (base.chartArea.IsBottomSceneWallVisible() || !axisOnEdge))
			{
				num = base.chartArea.areaSceneDepth;
			}
			if (this.AxisPosition == AxisPosition.Left && (base.chartArea.IsSideSceneWallOnLeft() || !axisOnEdge))
			{
				num = base.chartArea.areaSceneDepth;
			}
			if (this.AxisPosition == AxisPosition.Right && (!base.chartArea.IsSideSceneWallOnLeft() || !axisOnEdge))
			{
				num = base.chartArea.areaSceneDepth;
			}
			if (this.AxisPosition == AxisPosition.Top && !axisOnEdge)
			{
				num = base.chartArea.areaSceneDepth;
			}
			if (base.chartArea.IsMainSceneWallOnFront())
			{
				num = (float)((num == 0.0) ? base.chartArea.areaSceneDepth : 0.0);
			}
			return num;
		}

		internal void PaintGrids(ChartGraphics graph)
		{
			object obj = default(object);
			this.PaintGrids(graph, false, 0, 0, out obj);
		}

		internal void PaintGrids(ChartGraphics graph, bool selectionMode, int x, int y, out object obj)
		{
			obj = null;
			if (base.enabled)
			{
				base.minorGrid.Paint(graph);
				base.majorGrid.Paint(graph);
			}
		}

		internal void PaintStrips(ChartGraphics graph, bool drawLinesOnly)
		{
			object obj = default(object);
			this.PaintStrips(graph, false, 0, 0, out obj, drawLinesOnly);
		}

		internal void PaintStrips(ChartGraphics graph, bool selectionMode, int x, int y, out object obj, bool drawLinesOnly)
		{
			obj = null;
			if (base.enabled)
			{
				bool flag = this.AddInterlacedStrip();
				foreach (StripLine stripLine in this.StripLines)
				{
					stripLine.Paint(graph, base.Common, drawLinesOnly);
				}
				if (flag)
				{
					this.StripLines.RemoveAt(0);
				}
			}
		}

		private bool AddInterlacedStrip()
		{
			bool flag = false;
			if (this.Interlaced)
			{
				StripLine stripLine = new StripLine();
				stripLine.interlaced = true;
				stripLine.BorderColor = Color.Empty;
				if (this.MajorGrid.Enabled && this.MajorGrid.Interval != 0.0)
				{
					flag = true;
					stripLine.Interval = this.MajorGrid.Interval * 2.0;
					stripLine.IntervalType = this.MajorGrid.IntervalType;
					stripLine.IntervalOffset = this.MajorGrid.IntervalOffset;
					stripLine.IntervalOffsetType = this.MajorGrid.IntervalOffsetType;
					stripLine.StripWidth = this.MajorGrid.Interval;
					stripLine.StripWidthType = this.MajorGrid.IntervalType;
				}
				else if (this.MajorTickMark.Enabled && this.MajorTickMark.Interval != 0.0)
				{
					flag = true;
					stripLine.Interval = this.MajorTickMark.Interval * 2.0;
					stripLine.IntervalType = this.MajorTickMark.IntervalType;
					stripLine.IntervalOffset = this.MajorTickMark.IntervalOffset;
					stripLine.IntervalOffsetType = this.MajorTickMark.IntervalOffsetType;
					stripLine.StripWidth = this.MajorTickMark.Interval;
					stripLine.StripWidthType = this.MajorTickMark.IntervalType;
				}
				else if (base.LabelStyle.Enabled && base.LabelStyle.Interval != 0.0)
				{
					flag = true;
					stripLine.Interval = base.LabelStyle.Interval * 2.0;
					stripLine.IntervalType = base.LabelStyle.IntervalType;
					stripLine.IntervalOffset = base.LabelStyle.IntervalOffset;
					stripLine.IntervalOffsetType = base.LabelStyle.IntervalOffsetType;
					stripLine.StripWidth = base.LabelStyle.Interval;
					stripLine.StripWidthType = base.LabelStyle.IntervalType;
				}
				if (flag)
				{
					if (this.InterlacedColor != Color.Empty)
					{
						stripLine.BackColor = this.InterlacedColor;
					}
					else if (base.chartArea.BackColor == Color.Empty)
					{
						stripLine.BackColor = (base.chartArea.Area3DStyle.Enable3D ? Color.DarkGray : Color.LightGray);
					}
					else if (base.chartArea.BackColor == Color.Transparent)
					{
						if (this.chart.BackColor != Color.Transparent && this.chart.BackColor != Color.Black)
						{
							stripLine.BackColor = ChartGraphics.GetGradientColor(this.chart.BackColor, Color.Black, 0.2);
						}
						else
						{
							stripLine.BackColor = Color.LightGray;
						}
					}
					else
					{
						stripLine.BackColor = ChartGraphics.GetGradientColor(base.chartArea.BackColor, Color.Black, 0.2);
					}
					this.StripLines.Insert(0, stripLine);
				}
			}
			return flag;
		}

		public void RoundAxisValues()
		{
			base.roundedXValues = true;
		}

		internal void ReCalc(ElementPosition position)
		{
			base.PlotAreaPosition = position;
		}

		internal void StoreAxisValues()
		{
			base.tempLabels = new CustomLabelsCollection(this);
			foreach (CustomLabel customLabel in base.CustomLabels)
			{
				base.tempLabels.Add(customLabel.Clone());
			}
			base.paintMode = true;
			if (this.storeValuesEnabled)
			{
				base.tempMaximum = base.maximum;
				base.tempMinimum = base.minimum;
				base.tempCrossing = base.crossing;
				base.tempAutoMinimum = base.autoMinimum;
				base.tempAutoMaximum = base.autoMaximum;
				base.tempMajorGridInterval = base.majorGrid.interval;
				base.tempMajorTickMarkInterval = base.majorTickMark.interval;
				base.tempMinorGridInterval = base.minorGrid.interval;
				base.tempMinorTickMarkInterval = base.minorTickMark.interval;
				base.tempGridIntervalType = base.majorGrid.intervalType;
				base.tempTickMarkIntervalType = base.majorTickMark.intervalType;
				base.tempLabelInterval = base.labelStyle.interval;
				base.tempLabelIntervalType = base.labelStyle.intervalType;
				this.originalViewPosition = base.View.Position;
				this.storeValuesEnabled = false;
			}
		}

		internal void ResetAxisValues()
		{
			base.paintMode = false;
			this.ResetAutoValues();
			if (base.tempLabels != null)
			{
				base.CustomLabels.Clear();
				foreach (CustomLabel tempLabel in base.tempLabels)
				{
					base.CustomLabels.Add(tempLabel.Clone());
				}
				base.tempLabels = null;
			}
		}

		internal void ResetAutoValues()
		{
			this.refreshMinMaxFromData = true;
			base.maximum = base.tempMaximum;
			base.minimum = base.tempMinimum;
			base.crossing = base.tempCrossing;
			base.autoMinimum = base.tempAutoMinimum;
			base.autoMaximum = base.tempAutoMaximum;
			base.majorGrid.interval = base.tempMajorGridInterval;
			base.majorTickMark.interval = base.tempMajorTickMarkInterval;
			base.minorGrid.interval = base.tempMinorGridInterval;
			base.minorTickMark.interval = base.tempMinorTickMarkInterval;
			base.labelStyle.interval = base.tempLabelInterval;
			base.majorGrid.intervalType = base.tempGridIntervalType;
			base.majorTickMark.intervalType = base.tempTickMarkIntervalType;
			base.labelStyle.intervalType = base.tempLabelIntervalType;
			if (this.chart != null && !this.chart.serializing)
			{
				base.View.Position = this.originalViewPosition;
			}
			this.storeValuesEnabled = true;
		}

		internal virtual void Resize(ChartGraphics chartGraph, ElementPosition chartAreaPosition, RectangleF plotArea, float axesNumber, bool autoPlotPosition)
		{
			base.PlotAreaPosition = chartAreaPosition;
			base.PlotAreaPosition.FromRectangleF(plotArea);
			this.titleSize = 0f;
			if (this.Title.Length > 0)
			{
				SizeF sizeF = chartGraph.MeasureStringRel(this.Title.Replace("\\n", "\n"), this.TitleFont, new SizeF(10000f, 10000f), new StringFormat(), this.GetTextOrientation());
				float num = 0f;
				if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
				{
					num = (float)(plotArea.Height / 100.0 * (20.0 / axesNumber));
					if (this.IsTextVertical)
					{
						this.titleSize = Math.Min(sizeF.Width, num);
					}
					else
					{
						this.titleSize = Math.Min(sizeF.Height, num);
					}
				}
				else
				{
					num = (float)(plotArea.Width / 100.0 * (20.0 / axesNumber));
					if (this.IsTextVertical)
					{
						sizeF.Width = chartGraph.GetAbsoluteSize(new SizeF(sizeF.Height, sizeF.Height)).Height;
						sizeF.Width = chartGraph.GetRelativeSize(new SizeF(sizeF.Width, sizeF.Width)).Width;
						this.titleSize = Math.Min(sizeF.Width, (float)(plotArea.Width / 100.0 * (20.0 / axesNumber)));
					}
					else
					{
						this.titleSize = Math.Min(sizeF.Width, (float)(plotArea.Width / 100.0 * (20.0 / axesNumber)));
					}
				}
			}
			if (this.titleSize > 0.0)
			{
				this.titleSize += 1f;
			}
			float num2 = 0f;
			SizeF sizeF2 = SizeF.Empty;
			SizeF sizeF3 = SizeF.Empty;
			ArrowOrientation arrowOrientation = ArrowOrientation.Bottom;
			if (base.axisType == AxisName.X || base.axisType == AxisName.X2)
			{
				if (base.chartArea.AxisY.Arrows != 0)
				{
					sizeF2 = base.chartArea.AxisY.GetArrowSize(out arrowOrientation);
					if (!this.IsArrowInAxis(arrowOrientation, this.AxisPosition))
					{
						sizeF2 = SizeF.Empty;
					}
				}
				if (base.chartArea.AxisY2.Arrows != 0)
				{
					sizeF3 = base.chartArea.AxisY2.GetArrowSize(out arrowOrientation);
					if (!this.IsArrowInAxis(arrowOrientation, this.AxisPosition))
					{
						sizeF3 = SizeF.Empty;
					}
				}
			}
			else
			{
				if (base.chartArea.AxisX.Arrows != 0)
				{
					sizeF2 = base.chartArea.AxisX.GetArrowSize(out arrowOrientation);
					if (!this.IsArrowInAxis(arrowOrientation, this.AxisPosition))
					{
						sizeF2 = SizeF.Empty;
					}
				}
				if (base.chartArea.AxisX2.Arrows != 0)
				{
					sizeF3 = base.chartArea.AxisX2.GetArrowSize(out arrowOrientation);
					if (!this.IsArrowInAxis(arrowOrientation, this.AxisPosition))
					{
						sizeF3 = SizeF.Empty;
					}
				}
			}
			num2 = ((this.AxisPosition != AxisPosition.Bottom && this.AxisPosition != AxisPosition.Top) ? Math.Max(sizeF2.Width, sizeF3.Width) : Math.Max(sizeF2.Height, sizeF3.Height));
			this.markSize = 0f;
			float val = 0f;
			if (this.MajorTickMark.Enabled && this.MajorTickMark.Style != 0)
			{
				if (this.MajorTickMark.Style == TickMarkStyle.Inside)
				{
					val = 0f;
				}
				else if (this.MajorTickMark.Style == TickMarkStyle.Cross)
				{
					val = (float)(this.MajorTickMark.Size / 2.0);
				}
				else if (this.MajorTickMark.Style == TickMarkStyle.Outside)
				{
					val = this.MajorTickMark.Size;
				}
			}
			float val2 = 0f;
			if (this.MinorTickMark.Enabled && this.MinorTickMark.Style != 0 && this.MinorTickMark.Interval != 0.0)
			{
				if (this.MinorTickMark.Style == TickMarkStyle.Inside)
				{
					val2 = 0f;
				}
				else if (this.MinorTickMark.Style == TickMarkStyle.Cross)
				{
					val2 = (float)(this.MinorTickMark.Size / 2.0);
				}
				else if (this.MinorTickMark.Style == TickMarkStyle.Outside)
				{
					val2 = this.MinorTickMark.Size;
				}
			}
			this.markSize += Math.Max(val, val2);
			SizeF relativeSize = chartGraph.GetRelativeSize(new SizeF((float)this.LineWidth, (float)this.LineWidth));
			if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
			{
				this.markSize += (float)(relativeSize.Height / 2.0);
				this.markSize = Math.Min(this.markSize, (float)(plotArea.Height / 100.0 * (20.0 / axesNumber)));
			}
			else
			{
				this.markSize += (float)(relativeSize.Width / 2.0);
				this.markSize = Math.Min(this.markSize, (float)(plotArea.Width / 100.0 * (20.0 / axesNumber)));
			}
			this.scrollBarSize = 0f;
			if (base.ScrollBar.IsVisible() && (this.IsAxisOnAreaEdge() || !this.MarksNextToAxis))
			{
				if (base.ScrollBar.PositionInside)
				{
					this.markSize += (float)base.ScrollBar.GetScrollBarRelativeSize();
				}
				else
				{
					this.scrollBarSize = (float)base.ScrollBar.GetScrollBarRelativeSize();
				}
			}
			if (base.chartArea.Area3DStyle.Enable3D && !base.chartArea.chartAreaIsCurcular && base.chartArea.BackColor != Color.Transparent && base.chartArea.Area3DStyle.WallWidth > 0)
			{
				SizeF relativeSize2 = chartGraph.GetRelativeSize(new SizeF((float)base.chartArea.Area3DStyle.WallWidth, (float)base.chartArea.Area3DStyle.WallWidth));
				if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
				{
					this.markSize += relativeSize2.Height;
				}
				else
				{
					this.markSize += relativeSize2.Width;
				}
			}
			if (num2 > this.markSize + this.scrollBarSize + this.titleSize)
			{
				this.markSize = Math.Max(this.markSize, num2 - (this.markSize + this.scrollBarSize + this.titleSize));
				this.markSize = Math.Min(this.markSize, (float)(plotArea.Width / 100.0 * (20.0 / axesNumber)));
			}
			float num3 = 0f;
			if (!autoPlotPosition)
			{
				if (this.IsMarksNextToAxis())
				{
					if (this.AxisPosition == AxisPosition.Top)
					{
						num3 = (float)this.GetAxisPosition() - base.chartArea.Position.Y;
					}
					else if (this.AxisPosition == AxisPosition.Bottom)
					{
						num3 = base.chartArea.Position.Bottom() - (float)this.GetAxisPosition();
					}
					if (this.AxisPosition == AxisPosition.Left)
					{
						num3 = (float)this.GetAxisPosition() - base.chartArea.Position.X;
					}
					else if (this.AxisPosition == AxisPosition.Right)
					{
						num3 = base.chartArea.Position.Right() - (float)this.GetAxisPosition();
					}
				}
				else
				{
					if (this.AxisPosition == AxisPosition.Top)
					{
						num3 = plotArea.Y - base.chartArea.Position.Y;
					}
					else if (this.AxisPosition == AxisPosition.Bottom)
					{
						num3 = base.chartArea.Position.Bottom() - plotArea.Bottom;
					}
					if (this.AxisPosition == AxisPosition.Left)
					{
						num3 = plotArea.X - base.chartArea.Position.X;
					}
					else if (this.AxisPosition == AxisPosition.Right)
					{
						num3 = base.chartArea.Position.Right() - plotArea.Right;
					}
				}
				num3 = (float)(num3 * 2.0);
			}
			else
			{
				num3 = ((this.AxisPosition != AxisPosition.Bottom && this.AxisPosition != AxisPosition.Top) ? plotArea.Width : plotArea.Height);
			}
			if (base.Enabled != AxisEnabled.False && base.LabelStyle.Enabled && this.IsVariableLabelCountModeEnabled())
			{
				float num4 = 3f;
				if ((this.AxisPosition == AxisPosition.Left || this.AxisPosition == AxisPosition.Right) && (base.LabelStyle.FontAngle == 90 || base.LabelStyle.FontAngle == -90))
				{
					num4 = 0f;
				}
				if ((this.AxisPosition == AxisPosition.Top || this.AxisPosition == AxisPosition.Bottom) && (base.LabelStyle.FontAngle == 180 || base.LabelStyle.FontAngle == 0))
				{
					num4 = 0f;
				}
				if (base.chartArea.Area3DStyle.Enable3D)
				{
					num4 = (float)(num4 + 1.0);
				}
				this.autoLabelFont = new Font(base.LabelStyle.Font.FontFamily, base.LabelStyle.Font.Size + num4, base.LabelStyle.Font.Style, GraphicsUnit.Point);
				this.autoLabelAngle = base.LabelStyle.FontAngle;
				this.autoLabelOffset = (base.LabelStyle.OffsetLabels ? 1 : 0);
				this.AdjustIntervalToFitLabels(chartGraph, autoPlotPosition, false);
			}
			this.autoLabelFont = null;
			this.autoLabelAngle = -1000;
			this.autoLabelOffset = -1;
			if (this.LabelsAutoFit && this.LabelsAutoFitStyle != 0 && !base.chartArea.chartAreaIsCurcular)
			{
				bool flag = false;
				bool flag2 = false;
				this.autoLabelAngle = 0;
				this.autoLabelOffset = 0;
				CustomLabelsCollection customLabelsCollection = null;
				float num5 = 8f;
				num5 = (float)Math.Max(this.LabelsAutoFitMaxFontSize, this.LabelsAutoFitMinFontSize);
				this.minLabelFontSize = (float)Math.Min(this.LabelsAutoFitMinFontSize, this.LabelsAutoFitMaxFontSize);
				this.aveLabelFontSize = (float)(this.minLabelFontSize + Math.Abs(num5 - this.minLabelFontSize) / 2.0);
				if (base.chartArea.EquallySizedAxesFont)
				{
					num5 = Math.Min(num5, base.chartArea.axesAutoFontSize);
				}
				this.autoLabelFont = new Font(base.LabelStyle.Font.FontFamily, num5, base.LabelStyle.Font.Style, GraphicsUnit.Point);
				if ((this.LabelsAutoFitStyle & LabelsAutoFitStyles.IncreaseFont) != LabelsAutoFitStyles.IncreaseFont)
				{
					this.autoLabelFont = (Font)base.LabelStyle.Font.Clone();
				}
				float num6 = 0f;
				while (!flag)
				{
					bool checkLabelsFirstRowOnly = true;
					if ((this.LabelsAutoFitStyle & LabelsAutoFitStyles.DecreaseFont) == LabelsAutoFitStyles.DecreaseFont)
					{
						checkLabelsFirstRowOnly = false;
					}
					flag = this.CheckLabelsFit(chartGraph, this.markSize + this.scrollBarSize + this.titleSize + num6, autoPlotPosition, checkLabelsFirstRowOnly, false);
					if (!flag)
					{
						if (this.autoLabelFont.SizeInPoints >= this.aveLabelFontSize && (this.LabelsAutoFitStyle & LabelsAutoFitStyles.DecreaseFont) == LabelsAutoFitStyles.DecreaseFont)
						{
							this.autoLabelFont = new Font(this.autoLabelFont.FontFamily, (float)(this.autoLabelFont.SizeInPoints - 0.5), this.autoLabelFont.Style, GraphicsUnit.Point);
						}
						else if (!base.chartArea.Area3DStyle.Enable3D && !base.chartArea.chartAreaIsCurcular && customLabelsCollection == null && this.autoLabelAngle == 0 && this.autoLabelOffset == 0 && (this.LabelsAutoFitStyle & LabelsAutoFitStyles.OffsetLabels) == LabelsAutoFitStyles.OffsetLabels)
						{
							this.autoLabelOffset = 1;
						}
						else if (!flag2 && (this.LabelsAutoFitStyle & LabelsAutoFitStyles.WordWrap) == LabelsAutoFitStyles.WordWrap)
						{
							bool flag3 = false;
							this.autoLabelOffset = 0;
							if (customLabelsCollection == null)
							{
								customLabelsCollection = new CustomLabelsCollection(this);
								foreach (CustomLabel customLabel3 in base.CustomLabels)
								{
									customLabelsCollection.Add(customLabel3.Clone());
								}
							}
							if (!this.WordWrapLongestLabel(base.CustomLabels))
							{
								flag2 = true;
								if (customLabelsCollection != null)
								{
									base.CustomLabels.Clear();
									foreach (CustomLabel item in customLabelsCollection)
									{
										base.CustomLabels.Add(item.Clone());
									}
									customLabelsCollection = null;
								}
								if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
								{
									if (num6 != 0.0 && num6 != 30.0 && num6 != 20.0)
									{
										goto IL_0cab;
									}
									if ((this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep30) != LabelsAutoFitStyles.LabelsAngleStep30 && (this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep45) != LabelsAutoFitStyles.LabelsAngleStep45 && (this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep90) != LabelsAutoFitStyles.LabelsAngleStep90)
									{
										goto IL_0cab;
									}
									this.autoLabelAngle = 90;
									flag2 = false;
									if (num6 == 0.0)
									{
										num6 = 30f;
									}
									else if (num6 == 30.0)
									{
										num6 = 20f;
									}
									else if (num6 == 20.0)
									{
										num6 = 5f;
									}
									else
									{
										this.autoLabelAngle = 0;
										flag2 = true;
									}
								}
							}
						}
						else if (this.autoLabelAngle != 90 && ((this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep30) == LabelsAutoFitStyles.LabelsAngleStep30 || (this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep45) == LabelsAutoFitStyles.LabelsAngleStep45 || (this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep90) == LabelsAutoFitStyles.LabelsAngleStep90))
						{
							num6 = 0f;
							this.autoLabelOffset = 0;
							if ((this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep30) == LabelsAutoFitStyles.LabelsAngleStep30)
							{
								this.autoLabelAngle += (base.chartArea.Area3DStyle.Enable3D ? 45 : 30);
							}
							else if ((this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep45) == LabelsAutoFitStyles.LabelsAngleStep45)
							{
								this.autoLabelAngle += 45;
							}
							else if ((this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep90) == LabelsAutoFitStyles.LabelsAngleStep90)
							{
								this.autoLabelAngle += 90;
							}
						}
						else if (this.autoLabelFont.SizeInPoints > this.minLabelFontSize && (this.LabelsAutoFitStyle & LabelsAutoFitStyles.DecreaseFont) == LabelsAutoFitStyles.DecreaseFont)
						{
							this.autoLabelAngle = 0;
							this.autoLabelFont = new Font(this.autoLabelFont.FontFamily, (float)(this.autoLabelFont.SizeInPoints - 0.5), this.autoLabelFont.Style, GraphicsUnit.Point);
						}
						else
						{
							if ((this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep30) == LabelsAutoFitStyles.LabelsAngleStep30 || (this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep45) == LabelsAutoFitStyles.LabelsAngleStep45 || (this.LabelsAutoFitStyle & LabelsAutoFitStyles.LabelsAngleStep90) == LabelsAutoFitStyles.LabelsAngleStep90)
							{
								if (this.AxisPosition == AxisPosition.Top || this.AxisPosition == AxisPosition.Bottom)
								{
									this.autoLabelAngle = 90;
								}
								else
								{
									this.autoLabelAngle = 0;
								}
							}
							if ((this.LabelsAutoFitStyle & LabelsAutoFitStyles.OffsetLabels) == LabelsAutoFitStyles.OffsetLabels)
							{
								this.autoLabelOffset = 0;
							}
							flag = true;
						}
					}
					else if (base.chartArea.Area3DStyle.Enable3D && !base.chartArea.chartAreaIsCurcular && this.autoLabelFont.SizeInPoints > this.minLabelFontSize)
					{
						this.autoLabelFont = new Font(this.autoLabelFont.FontFamily, (float)(this.autoLabelFont.SizeInPoints - 0.5), this.autoLabelFont.Style, GraphicsUnit.Point);
					}
					continue;
					IL_0cab:
					num6 = 0f;
				}
				if ((this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top) && this.autoLabelAngle == 90)
				{
					this.autoLabelAngle = -90;
				}
			}
			this.labelSize = 0f;
			if (base.LabelStyle.Enabled)
			{
				this.labelSize = (float)(75.0 - this.markSize - this.scrollBarSize - this.titleSize);
				if (this.labelSize > 0.0)
				{
					this.groupingLabelSizes = this.GetRequiredGroupLabelSize(chartGraph, (float)(num3 / 100.0 * 45.0));
					this.totlaGroupingLabelsSize = this.GetGroupLablesToatalSize();
				}
				this.labelSize -= this.totlaGroupingLabelsSize;
				if (this.labelSize > 0.0)
				{
					if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
					{
						this.labelSize = (float)(1.0 + this.GetRequiredLabelSize(chartGraph, (float)(num3 / 100.0 * (75.0 - this.markSize - this.scrollBarSize - this.titleSize)), out this.unRotatedLabelSize));
					}
					else
					{
						this.labelSize = (float)(1.0 + this.GetRequiredLabelSize(chartGraph, (float)(num3 / 100.0 * (75.0 - this.markSize - this.scrollBarSize - this.titleSize)), out this.unRotatedLabelSize));
					}
					if (!base.LabelStyle.Enabled)
					{
						this.labelSize -= 1f;
					}
				}
				else
				{
					this.labelSize = 0f;
				}
				this.labelSize += this.totlaGroupingLabelsSize;
			}
		}

		private void AdjustIntervalToFitLabels(ChartGraphics chartGraph, bool autoPlotPosition, bool onlyIncreaseInterval)
		{
			if (base.ScaleSegments.Count == 0)
			{
				this.AdjustIntervalToFitLabels(chartGraph, autoPlotPosition, null, onlyIncreaseInterval);
			}
			else
			{
				base.ScaleSegments.AllowOutOfScaleValues = true;
				foreach (AxisScaleSegment scaleSegment in base.ScaleSegments)
				{
					this.AdjustIntervalToFitLabels(chartGraph, autoPlotPosition, scaleSegment, onlyIncreaseInterval);
				}
				bool removeFirstRow = true;
				int num = 0;
				ArrayList arrayList = new ArrayList();
				ArrayList arrayList2 = new ArrayList();
				foreach (AxisScaleSegment scaleSegment2 in base.ScaleSegments)
				{
					scaleSegment2.SetTempAxisScaleAndInterval();
					base.FillLabels(removeFirstRow);
					removeFirstRow = false;
					scaleSegment2.RestoreAxisScaleAndInterval();
					if (num < base.ScaleSegments.Count - 1 && base.CustomLabels.Count > 0)
					{
						arrayList.Add(base.CustomLabels[base.CustomLabels.Count - 1]);
						arrayList2.Add(base.CustomLabels.Count - 1);
						base.CustomLabels.RemoveAt(base.CustomLabels.Count - 1);
					}
					num++;
				}
				int num2 = 0;
				int num3 = 0;
				foreach (CustomLabel item in arrayList)
				{
					int index = (int)arrayList2[num3] + num2;
					if (num3 < base.CustomLabels.Count)
					{
						base.CustomLabels.Insert(index, item);
					}
					else
					{
						base.CustomLabels.Add(item);
					}
					ArrayList arrayList3 = new ArrayList();
					bool flag = this.CheckLabelsFit(chartGraph, this.markSize + this.scrollBarSize + this.titleSize, autoPlotPosition, true, false, (byte)((this.AxisPosition != 0 && this.AxisPosition != AxisPosition.Right) ? 1 : 0) != 0, (byte)((this.AxisPosition == AxisPosition.Left || this.AxisPosition == AxisPosition.Right) ? 1 : 0) != 0, arrayList3);
					if (flag)
					{
						int num4 = 0;
						while (flag && num4 < arrayList3.Count)
						{
							RectangleF rectangleF = (RectangleF)arrayList3[num4];
							int num5 = num4 + 1;
							while (flag && num5 < arrayList3.Count)
							{
								RectangleF rect = (RectangleF)arrayList3[num5];
								if (rectangleF.IntersectsWith(rect))
								{
									flag = false;
								}
								num5++;
							}
							num4++;
						}
					}
					if (!flag)
					{
						base.CustomLabels.RemoveAt(index);
					}
					else
					{
						num2++;
					}
					num3++;
				}
				base.ScaleSegments.AllowOutOfScaleValues = false;
			}
		}

		private bool IsVariableLabelCountModeEnabled()
		{
			if ((base.IntervalAutoMode == IntervalAutoMode.VariableCount || base.ScaleSegments.Count > 0) && !base.Logarithmic && (base.tempLabelInterval <= 0.0 || (double.IsNaN(base.tempLabelInterval) && this.Interval <= 0.0)))
			{
				if (!base.chartArea.requireAxes)
				{
					return false;
				}
				bool flag = false;
				foreach (CustomLabel customLabel in base.CustomLabels)
				{
					if (customLabel.customLabel && customLabel.RowIndex == 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return true;
				}
			}
			return false;
		}

		private void AdjustIntervalToFitLabels(ChartGraphics chartGraph, bool autoPlotPosition, AxisScaleSegment axisScaleSegment, bool onlyIncreaseInterval)
		{
			if (axisScaleSegment != null)
			{
				if (base.tempLabels != null)
				{
					base.CustomLabels.Clear();
					foreach (CustomLabel tempLabel in base.tempLabels)
					{
						base.CustomLabels.Add(tempLabel.Clone());
					}
				}
				axisScaleSegment.SetTempAxisScaleAndInterval();
				base.FillLabels(true);
				axisScaleSegment.RestoreAxisScaleAndInterval();
			}
			double minInterval = double.NaN;
			ArrayList axisSeries = AxisScaleBreakStyle.GetAxisSeries(this);
			foreach (Series item in axisSeries)
			{
				if (base.axisType == AxisName.X || base.axisType == AxisName.X2)
				{
					if (ChartElement.IndexedSeries(item))
					{
						minInterval = 1.0;
					}
					else if (item.XValueType == ChartValueTypes.String || item.XValueType == ChartValueTypes.Int || item.XValueType == ChartValueTypes.UInt || item.XValueType == ChartValueTypes.ULong || item.XValueType == ChartValueTypes.Long)
					{
						minInterval = 1.0;
					}
				}
				else if (item.YValueType == ChartValueTypes.String || item.YValueType == ChartValueTypes.Int || item.YValueType == ChartValueTypes.UInt || item.YValueType == ChartValueTypes.ULong || item.YValueType == ChartValueTypes.Long)
				{
					minInterval = 1.0;
				}
			}
			bool flag = true;
			bool flag2 = true;
			double num = (axisScaleSegment == null) ? base.labelStyle.Interval : axisScaleSegment.Interval;
			DateTimeIntervalType dateTimeIntervalType = (axisScaleSegment == null) ? base.labelStyle.IntervalType : axisScaleSegment.IntervalType;
			DateTimeIntervalType dateTimeIntervalType2 = dateTimeIntervalType;
			double num2 = num;
			ArrayList arrayList = new ArrayList();
			bool flag3 = false;
			int num3 = 0;
			while (!flag3 && num3 <= 1000)
			{
				bool flag4 = true;
				bool flag5 = this.CheckLabelsFit(chartGraph, this.markSize + this.scrollBarSize + this.titleSize, autoPlotPosition, true, false, (byte)((this.AxisPosition != 0 && this.AxisPosition != AxisPosition.Right) ? 1 : 0) != 0, (byte)((this.AxisPosition == AxisPosition.Left || this.AxisPosition == AxisPosition.Right) ? 1 : 0) != 0, null);
				if (flag)
				{
					flag = false;
					flag2 = ((byte)(flag5 ? 1 : 0) != 0);
					if (onlyIncreaseInterval && flag2)
					{
						flag3 = true;
						continue;
					}
				}
				double num4 = 0.0;
				DateTimeIntervalType dateTimeIntervalType3 = DateTimeIntervalType.Number;
				if (flag2)
				{
					if (flag5)
					{
						num2 = num;
						dateTimeIntervalType2 = dateTimeIntervalType;
						arrayList.Clear();
						foreach (CustomLabel customLabel3 in base.CustomLabels)
						{
							arrayList.Add(customLabel3);
						}
						dateTimeIntervalType3 = dateTimeIntervalType;
						num4 = this.ReduceLabelInterval(num, minInterval, axisScaleSegment, ref dateTimeIntervalType3);
					}
					else
					{
						num4 = num2;
						dateTimeIntervalType3 = dateTimeIntervalType2;
						flag3 = true;
						flag4 = false;
						base.CustomLabels.Clear();
						foreach (CustomLabel item2 in arrayList)
						{
							base.CustomLabels.Add(item2);
						}
					}
				}
				else if (!flag5 && base.CustomLabels.Count > 1)
				{
					dateTimeIntervalType3 = dateTimeIntervalType;
					num4 = this.IncreaseLabelInterval(num, axisScaleSegment, ref dateTimeIntervalType3);
				}
				else
				{
					flag3 = true;
				}
				if (num4 != 0.0)
				{
					num = num4;
					dateTimeIntervalType = dateTimeIntervalType3;
					if (axisScaleSegment == null)
					{
						base.SetIntervalAndType(num4, dateTimeIntervalType3);
					}
					else
					{
						axisScaleSegment.Interval = num4;
						axisScaleSegment.IntervalType = dateTimeIntervalType3;
					}
					if (flag4)
					{
						if (base.tempLabels != null)
						{
							base.CustomLabels.Clear();
							foreach (CustomLabel tempLabel2 in base.tempLabels)
							{
								base.CustomLabels.Add(tempLabel2.Clone());
							}
						}
						if (axisScaleSegment == null)
						{
							base.FillLabels(true);
						}
						else
						{
							axisScaleSegment.SetTempAxisScaleAndInterval();
							base.FillLabels(true);
							axisScaleSegment.RestoreAxisScaleAndInterval();
						}
					}
				}
				else
				{
					flag3 = true;
				}
				num3++;
			}
		}

		private double ReduceLabelInterval(double oldInterval, double minInterval, AxisScaleSegment axisScaleSegment, ref DateTimeIntervalType intervalType)
		{
			double num = oldInterval;
			double num2 = base.maximum - base.minimum;
			int num3 = 0;
			if (intervalType == DateTimeIntervalType.Auto || intervalType == DateTimeIntervalType.NotSet || intervalType == DateTimeIntervalType.Number)
			{
				double num4 = 2.0;
				do
				{
					num = base.CalcInterval(num2 / (num2 / (num / num4)));
					if (num == oldInterval)
					{
						num4 *= 2.0;
					}
					num3++;
				}
				while (num == oldInterval && num3 <= 100);
			}
			else
			{
				if (oldInterval > 1.0 || oldInterval < 1.0)
				{
					if (intervalType == DateTimeIntervalType.Minutes || intervalType == DateTimeIntervalType.Seconds)
					{
						if (oldInterval >= 60.0)
						{
							num = Math.Round(oldInterval / 2.0);
						}
						else if (oldInterval >= 30.0)
						{
							num = 15.0;
						}
						else if (oldInterval >= 15.0)
						{
							num = 5.0;
						}
						else if (oldInterval >= 5.0)
						{
							num = 1.0;
						}
					}
					else
					{
						num = Math.Round(oldInterval / 2.0);
					}
					if (num < 1.0)
					{
						num = 1.0;
					}
				}
				if (oldInterval == 1.0)
				{
					if (intervalType == DateTimeIntervalType.Years)
					{
						num = 6.0;
						intervalType = DateTimeIntervalType.Months;
					}
					else if (intervalType == DateTimeIntervalType.Months)
					{
						num = 2.0;
						intervalType = DateTimeIntervalType.Weeks;
					}
					else if (intervalType == DateTimeIntervalType.Weeks)
					{
						num = 2.0;
						intervalType = DateTimeIntervalType.Days;
					}
					else if (intervalType == DateTimeIntervalType.Days)
					{
						num = 12.0;
						intervalType = DateTimeIntervalType.Hours;
					}
					else if (intervalType == DateTimeIntervalType.Hours)
					{
						num = 30.0;
						intervalType = DateTimeIntervalType.Minutes;
					}
					else if (intervalType == DateTimeIntervalType.Minutes)
					{
						num = 30.0;
						intervalType = DateTimeIntervalType.Seconds;
					}
					else if (intervalType == DateTimeIntervalType.Seconds)
					{
						num = 100.0;
						intervalType = DateTimeIntervalType.Milliseconds;
					}
				}
			}
			if (!double.IsNaN(minInterval) && num < minInterval)
			{
				num = 0.0;
			}
			return num;
		}

		private double IncreaseLabelInterval(double oldInterval, AxisScaleSegment axisScaleSegment, ref DateTimeIntervalType intervalType)
		{
			double num = oldInterval;
			double num2 = base.maximum - base.minimum;
			int num3 = 0;
			if (intervalType == DateTimeIntervalType.Auto || intervalType == DateTimeIntervalType.NotSet || intervalType == DateTimeIntervalType.Number)
			{
				double num4 = 2.0;
				do
				{
					num = base.CalcInterval(num2 / (num2 / (num * num4)));
					if (num == oldInterval)
					{
						num4 *= 2.0;
					}
					num3++;
				}
				while (num == oldInterval && num3 <= 100);
			}
			else
			{
				num = oldInterval * 2.0;
				if (intervalType != DateTimeIntervalType.Years)
				{
					if (intervalType == DateTimeIntervalType.Months)
					{
						if (num >= 12.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Years;
						}
					}
					else if (intervalType == DateTimeIntervalType.Weeks)
					{
						if (num >= 4.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Months;
						}
					}
					else if (intervalType == DateTimeIntervalType.Days)
					{
						if (num >= 7.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Weeks;
						}
					}
					else if (intervalType == DateTimeIntervalType.Hours)
					{
						if (num >= 60.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Days;
						}
					}
					else if (intervalType == DateTimeIntervalType.Minutes)
					{
						if (num >= 60.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Hours;
						}
					}
					else if (intervalType == DateTimeIntervalType.Seconds)
					{
						if (num >= 60.0)
						{
							num = 1.0;
							intervalType = DateTimeIntervalType.Minutes;
						}
					}
					else if (intervalType == DateTimeIntervalType.Milliseconds && num >= 1000.0)
					{
						num = 1.0;
						intervalType = DateTimeIntervalType.Seconds;
					}
				}
			}
			return num;
		}

		private bool WordWrapLongestLabel(CustomLabelsCollection labels)
		{
			bool flag = false;
			ArrayList arrayList = new ArrayList(labels.Count);
			foreach (CustomLabel label in labels)
			{
				arrayList.Add(label.Text.Split('\n'));
			}
			int num = 5;
			int num2 = -1;
			int num3 = -1;
			int num4 = 0;
			foreach (string[] item in arrayList)
			{
				for (int i = 0; i < item.Length; i++)
				{
					if (item[i].Length > num && item[i].Trim().IndexOf(' ') > 0)
					{
						num = item[i].Length;
						num2 = num4;
						num3 = i;
					}
				}
				num4++;
			}
			if (num2 >= 0 && num3 >= 0)
			{
				string text = ((string[])arrayList[num2])[num3];
				for (num4 = 0; num4 < text.Length / 2 - 1; num4++)
				{
					if (text[text.Length / 2 - num4] == ' ')
					{
						text = text.Substring(0, text.Length / 2 - num4) + "\n" + text.Substring(text.Length / 2 - num4 + 1);
						flag = true;
					}
					else if (text[text.Length / 2 + num4] == ' ')
					{
						text = text.Substring(0, text.Length / 2 + num4) + "\n" + text.Substring(text.Length / 2 + num4 + 1);
						flag = true;
					}
					if (flag)
					{
						((string[])arrayList[num2])[num3] = text;
						break;
					}
				}
				if (flag)
				{
					CustomLabel customLabel2 = labels[num2];
					customLabel2.Text = string.Empty;
					for (int j = 0; j < ((string[])arrayList[num2]).Length; j++)
					{
						if (j > 0)
						{
							CustomLabel customLabel3 = customLabel2;
							customLabel3.Text += "\n";
						}
						CustomLabel customLabel4 = customLabel2;
						customLabel4.Text += ((string[])arrayList[num2])[j];
					}
				}
			}
			return flag;
		}

		internal void GetCircularAxisLabelsAutoFitFont(ChartGraphics graph, ArrayList axisList, CircularAxisLabelsStyle labelsStyle, RectangleF plotAreaRectAbs, RectangleF areaRectAbs, float labelsSizeEstimate)
		{
			this.autoLabelFont.Dispose();
			this.autoLabelFont = null;
			if (this.LabelsAutoFit && this.LabelsAutoFitStyle != 0 && base.LabelStyle.Enabled)
			{
				this.minLabelFontSize = (float)Math.Min(this.LabelsAutoFitMinFontSize, this.LabelsAutoFitMaxFontSize);
				this.autoLabelFont = new Font(base.LabelStyle.Font.FontFamily, (float)Math.Max(this.LabelsAutoFitMaxFontSize, this.LabelsAutoFitMinFontSize), base.LabelStyle.Font.Style, GraphicsUnit.Point);
				if ((this.LabelsAutoFitStyle & LabelsAutoFitStyles.IncreaseFont) != LabelsAutoFitStyles.IncreaseFont)
				{
					this.autoLabelFont = (Font)base.LabelStyle.Font.Clone();
				}
				bool flag = false;
				while (!flag)
				{
					flag = this.CheckCircularLabelsFit(graph, axisList, labelsStyle, plotAreaRectAbs, areaRectAbs, labelsSizeEstimate);
					if (!flag)
					{
						if (this.autoLabelFont.SizeInPoints > this.minLabelFontSize && (this.LabelsAutoFitStyle & LabelsAutoFitStyles.DecreaseFont) == LabelsAutoFitStyles.DecreaseFont)
						{
							this.autoLabelFont = new Font(this.autoLabelFont.FontFamily, (float)(this.autoLabelFont.SizeInPoints - 1.0), this.autoLabelFont.Style, GraphicsUnit.Point);
						}
						else
						{
							this.autoLabelAngle = 0;
							this.autoLabelOffset = 0;
							flag = true;
						}
					}
				}
			}
		}

		internal bool CheckCircularLabelsFit(ChartGraphics graph, ArrayList axisList, CircularAxisLabelsStyle labelsStyle, RectangleF plotAreaRectAbs, RectangleF areaRectAbs, float labelsSizeEstimate)
		{
			bool result = true;
			PointF absolutePoint = graph.GetAbsolutePoint(base.chartArea.circularCenter);
			float y = graph.GetAbsolutePoint(new PointF(0f, (float)(this.markSize + 1.0))).Y;
			RectangleF rect = RectangleF.Empty;
			float num = float.NaN;
			foreach (CircularChartAreaAxis axis in axisList)
			{
				SizeF sizeF = graph.MeasureString(axis.Title.Replace("\\n", "\n"), this.autoLabelFont);
				switch (labelsStyle)
				{
				case CircularAxisLabelsStyle.Circular:
				case CircularAxisLabelsStyle.Radial:
				{
					if (labelsStyle == CircularAxisLabelsStyle.Radial)
					{
						float width = sizeF.Width;
						sizeF.Width = sizeF.Height;
						sizeF.Height = width;
					}
					float num3 = absolutePoint.Y - plotAreaRectAbs.Y;
					num3 -= labelsSizeEstimate;
					num3 += y;
					float num4 = (float)(Math.Atan(sizeF.Width / 2.0 / num3) * 180.0 / 3.1415926535897931);
					float num5 = axis.AxisPosition + num4;
					num4 = axis.AxisPosition - num4;
					if (!float.IsNaN(num) && num > num4)
					{
						return false;
					}
					num = (float)(num5 - 1.0);
					PointF pointF = new PointF(absolutePoint.X, plotAreaRectAbs.Y);
					pointF.Y += labelsSizeEstimate;
					pointF.Y -= sizeF.Height;
					pointF.Y -= y;
					PointF[] array2 = new PointF[1]
					{
						pointF
					};
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt(axis.AxisPosition, absolutePoint);
					matrix2.TransformPoints(array2);
					if (areaRectAbs.Contains(array2[0]))
					{
						break;
					}
					return false;
				}
				case CircularAxisLabelsStyle.Horizontal:
				{
					float num2 = axis.AxisPosition;
					if (num2 > 180.0)
					{
						num2 = (float)(num2 - 180.0);
					}
					PointF[] array = new PointF[1]
					{
						new PointF(absolutePoint.X, plotAreaRectAbs.Y)
					};
					array[0].Y += labelsSizeEstimate;
					array[0].Y -= y;
					Matrix matrix = new Matrix();
					matrix.RotateAt(num2, absolutePoint);
					matrix.TransformPoints(array);
					RectangleF rectangleF = new RectangleF(array[0].X, (float)(array[0].Y - sizeF.Height / 2.0), sizeF.Width, sizeF.Height);
					if (num2 < 5.0)
					{
						rectangleF.X = (float)(array[0].X - sizeF.Width / 2.0);
						rectangleF.Y = array[0].Y - sizeF.Height;
					}
					if (num2 > 175.0)
					{
						rectangleF.X = (float)(array[0].X - sizeF.Width / 2.0);
						rectangleF.Y = array[0].Y;
					}
					rectangleF.Inflate(0f, (float)((0.0 - rectangleF.Height) * 0.15000000596046448));
					if (!areaRectAbs.Contains(rectangleF))
					{
						return false;
					}
					if (!rect.IsEmpty && rectangleF.IntersectsWith(rect))
					{
						return false;
					}
					rect = rectangleF;
					break;
				}
				}
			}
			return result;
		}

		internal void AdjustLabelFontAtSecondPass(ChartGraphics chartGraph, bool autoPlotPosition)
		{
			if (base.Enabled != AxisEnabled.False && base.LabelStyle.Enabled && this.IsVariableLabelCountModeEnabled())
			{
				if (this.autoLabelFont == null)
				{
					this.autoLabelFont = base.LabelStyle.Font;
				}
				if (this.autoLabelAngle < 0)
				{
					this.autoLabelAngle = base.LabelStyle.FontAngle;
				}
				if (this.autoLabelOffset < 0)
				{
					this.autoLabelOffset = (base.LabelStyle.OffsetLabels ? 1 : 0);
				}
				if (!this.CheckLabelsFit(chartGraph, this.markSize + this.scrollBarSize + this.titleSize, autoPlotPosition, true, true, (byte)((this.AxisPosition != 0 && this.AxisPosition != AxisPosition.Right) ? 1 : 0) != 0, (byte)((this.AxisPosition == AxisPosition.Left || this.AxisPosition == AxisPosition.Right) ? 1 : 0) != 0, null))
				{
					this.AdjustIntervalToFitLabels(chartGraph, autoPlotPosition, true);
				}
			}
			this.totlaGroupingLabelsSizeAdjustment = 0f;
			if (this.LabelsAutoFit && this.LabelsAutoFitStyle != 0 && base.Enabled != AxisEnabled.False)
			{
				bool flag = false;
				if (this.autoLabelFont == null)
				{
					this.autoLabelFont = base.LabelStyle.Font;
				}
				float num = this.totlaGroupingLabelsSize;
				while (!flag)
				{
					flag = this.CheckLabelsFit(chartGraph, this.markSize + this.scrollBarSize + this.titleSize, autoPlotPosition, true, true);
					if (!flag)
					{
						if (this.autoLabelFont.SizeInPoints > this.minLabelFontSize)
						{
							if (base.chartArea != null && base.chartArea.EquallySizedAxesFont)
							{
								Axis[] axes = base.chartArea.Axes;
								foreach (Axis axis in axes)
								{
									if (axis.enabled && axis.LabelsAutoFit && axis.autoLabelFont != null)
									{
										axis.autoLabelFont = new Font(axis.autoLabelFont.FontFamily, (float)(this.autoLabelFont.SizeInPoints - 1.0), axis.autoLabelFont.Style, GraphicsUnit.Point);
									}
								}
							}
							else if ((this.LabelsAutoFitStyle & LabelsAutoFitStyles.DecreaseFont) == LabelsAutoFitStyles.DecreaseFont)
							{
								this.autoLabelFont = new Font(this.autoLabelFont.FontFamily, (float)(this.autoLabelFont.SizeInPoints - 1.0), this.autoLabelFont.Style, GraphicsUnit.Point);
							}
							else
							{
								flag = true;
							}
						}
						else
						{
							flag = true;
						}
					}
				}
				this.totlaGroupingLabelsSizeAdjustment = num - this.totlaGroupingLabelsSize;
			}
		}

		internal double GetLogValue(double yValue)
		{
			if (base.Logarithmic)
			{
				yValue = Math.Log(yValue, base.logarithmBase);
			}
			return yValue;
		}

		private bool CheckLabelsFit(ChartGraphics chartGraph, float otherElementsSize, bool autoPlotPosition, bool checkLabelsFirstRowOnly, bool secondPass)
		{
			return this.CheckLabelsFit(chartGraph, otherElementsSize, autoPlotPosition, checkLabelsFirstRowOnly, secondPass, true, true, null);
		}

		private bool CheckLabelsFit(ChartGraphics chartGraph, float otherElementsSize, bool autoPlotPosition, bool checkLabelsFirstRowOnly, bool secondPass, bool checkWidth, bool checkHeight, ArrayList labelPositions)
		{
			if (labelPositions != null)
			{
				labelPositions.Clear();
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			RectangleF empty = RectangleF.Empty;
			float num = 0f;
			if (!autoPlotPosition)
			{
				if (this.IsMarksNextToAxis())
				{
					if (this.AxisPosition == AxisPosition.Top)
					{
						num = (float)this.GetAxisPosition() - base.chartArea.Position.Y;
					}
					else if (this.AxisPosition == AxisPosition.Bottom)
					{
						num = base.chartArea.Position.Bottom() - (float)this.GetAxisPosition();
					}
					if (this.AxisPosition == AxisPosition.Left)
					{
						num = (float)this.GetAxisPosition() - base.chartArea.Position.X;
					}
					else if (this.AxisPosition == AxisPosition.Right)
					{
						num = base.chartArea.Position.Right() - (float)this.GetAxisPosition();
					}
				}
				else
				{
					if (this.AxisPosition == AxisPosition.Top)
					{
						num = base.PlotAreaPosition.Y - base.chartArea.Position.Y;
					}
					else if (this.AxisPosition == AxisPosition.Bottom)
					{
						num = base.chartArea.Position.Bottom() - base.PlotAreaPosition.Bottom();
					}
					if (this.AxisPosition == AxisPosition.Left)
					{
						num = base.PlotAreaPosition.X - base.chartArea.Position.X;
					}
					else if (this.AxisPosition == AxisPosition.Right)
					{
						num = base.chartArea.Position.Right() - base.PlotAreaPosition.Right();
					}
				}
				num = (float)(num * 2.0);
			}
			else
			{
				num = ((this.AxisPosition != AxisPosition.Bottom && this.AxisPosition != AxisPosition.Top) ? base.chartArea.Position.Width : base.chartArea.Position.Height);
			}
			this.totlaGroupingLabelsSize = 0f;
			int groupLabelLevelCount = this.GetGroupLabelLevelCount();
			if (groupLabelLevelCount > 0)
			{
				this.groupingLabelSizes = new float[groupLabelLevelCount];
				bool flag = true;
				for (int i = 1; i <= groupLabelLevelCount; i++)
				{
					this.groupingLabelSizes[i - 1] = 0f;
					foreach (CustomLabel customLabel3 in base.CustomLabels)
					{
						if (customLabel3.RowIndex == 0)
						{
							double num2 = (customLabel3.From + customLabel3.To) / 2.0;
							if (!(num2 < base.GetViewMinimum()) && !(num2 > base.GetViewMaximum()))
							{
								goto IL_0256;
							}
							continue;
						}
						goto IL_0256;
						IL_0256:
						if (customLabel3.RowIndex == i)
						{
							double linearPosition = base.GetLinearPosition(customLabel3.From);
							double linearPosition2 = base.GetLinearPosition(customLabel3.To);
							if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
							{
								empty.Height = (float)(num / 100.0 * 45.0 / (float)groupLabelLevelCount);
								empty.X = (float)Math.Min(linearPosition, linearPosition2);
								empty.Width = (float)Math.Max(linearPosition, linearPosition2) - empty.X;
							}
							else
							{
								empty.Width = (float)(num / 100.0 * 45.0 / (float)groupLabelLevelCount);
								empty.Y = (float)Math.Min(linearPosition, linearPosition2);
								empty.Height = (float)Math.Max(linearPosition, linearPosition2) - empty.Y;
							}
							SizeF sizeF = chartGraph.MeasureStringRel(customLabel3.Text.Replace("\\n", "\n"), this.autoLabelFont);
							if (customLabel3.Image.Length > 0)
							{
								SizeF size = default(SizeF);
								if (base.Common.ImageLoader.GetAdjustedImageSize(customLabel3.Image, chartGraph.Graphics, ref size))
								{
									SizeF relativeSize = chartGraph.GetRelativeSize(size);
									sizeF.Width += relativeSize.Width;
									sizeF.Height = Math.Max(sizeF.Height, relativeSize.Height);
								}
							}
							if (customLabel3.LabelMark == LabelMark.Box)
							{
								SizeF relativeSize2 = chartGraph.GetRelativeSize(new SizeF(4f, 4f));
								sizeF.Width += relativeSize2.Width;
								sizeF.Height += relativeSize2.Height;
							}
							if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
							{
								this.groupingLabelSizes[i - 1] = Math.Max(this.groupingLabelSizes[i - 1], sizeF.Height);
							}
							else
							{
								sizeF.Width = chartGraph.GetAbsoluteSize(new SizeF(sizeF.Height, sizeF.Height)).Height;
								sizeF.Width = chartGraph.GetRelativeSize(new SizeF(sizeF.Width, sizeF.Width)).Width;
								this.groupingLabelSizes[i - 1] = Math.Max(this.groupingLabelSizes[i - 1], sizeF.Width);
							}
							if (Math.Round((double)sizeF.Width) >= Math.Round((double)empty.Width) && checkWidth)
							{
								flag = false;
							}
							if (Math.Round((double)sizeF.Height) >= Math.Round((double)empty.Height) && checkHeight)
							{
								flag = false;
							}
						}
					}
				}
				this.totlaGroupingLabelsSize = this.GetGroupLablesToatalSize();
				if (!flag && !checkLabelsFirstRowOnly)
				{
					return false;
				}
			}
			float num3 = (float)this.autoLabelAngle;
			int num4 = 0;
			foreach (CustomLabel customLabel4 in base.CustomLabels)
			{
				if (customLabel4.RowIndex == 0)
				{
					double num5 = (customLabel4.From + customLabel4.To) / 2.0;
					if (!(num5 < base.GetViewMinimum()) && !(num5 > base.GetViewMaximum()))
					{
						goto IL_059b;
					}
					continue;
				}
				goto IL_059b;
				IL_059b:
				if (customLabel4.RowIndex == 0)
				{
					if (labelPositions != null)
					{
						base.ScaleSegments.EnforceSegment(base.ScaleSegments.FindScaleSegmentForAxisValue((customLabel4.From + customLabel4.To) / 2.0));
					}
					double linearPosition3 = base.GetLinearPosition(customLabel4.From);
					double linearPosition4 = base.GetLinearPosition(customLabel4.To);
					if (labelPositions != null)
					{
						base.ScaleSegments.EnforceSegment(null);
					}
					empty.X = base.PlotAreaPosition.X;
					empty.Y = (float)Math.Min(linearPosition3, linearPosition4);
					empty.Height = (float)Math.Max(linearPosition3, linearPosition4) - empty.Y;
					float num6 = 75f;
					if (75.0 - this.totlaGroupingLabelsSize > 55.0)
					{
						num6 = (float)(55.0 + this.totlaGroupingLabelsSize);
					}
					if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
					{
						empty.Width = (float)(num / 100.0 * (num6 - this.totlaGroupingLabelsSize - otherElementsSize - 1.0));
					}
					else
					{
						empty.Width = (float)(num / 100.0 * (num6 - this.totlaGroupingLabelsSize - otherElementsSize - 1.0));
					}
					if (this.autoLabelOffset == 1)
					{
						empty.Y -= (float)(empty.Height / 2.0);
						empty.Height *= 2f;
						empty.Width /= 2f;
					}
					if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
					{
						float height = empty.Height;
						empty.Height = empty.Width;
						empty.Width = height;
						if (num3 != 0.0)
						{
							stringFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
						}
					}
					else if (num3 == 90.0 || num3 == -90.0)
					{
						num3 = 0f;
						stringFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
					}
					SizeF sizeF2 = chartGraph.MeasureStringRel(customLabel4.Text.Replace("\\n", "\n") + "W", this.autoLabelFont, secondPass ? empty.Size : base.chartArea.Position.ToRectangleF().Size, stringFormat);
					if (customLabel4.Text.Length > 0 && (sizeF2.Width == 0.0 || sizeF2.Height == 0.0))
					{
						stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
						sizeF2 = chartGraph.MeasureStringRel(customLabel4.Text.Replace("\\n", "\n"), this.autoLabelFont, secondPass ? empty.Size : base.chartArea.Position.ToRectangleF().Size, stringFormat);
						stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
					}
					if (customLabel4.Image.Length > 0)
					{
						SizeF size2 = default(SizeF);
						if (base.Common.ImageLoader.GetAdjustedImageSize(customLabel4.Image, chartGraph.Graphics, ref size2))
						{
							SizeF relativeSize3 = chartGraph.GetRelativeSize(size2);
							if ((stringFormat.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical)
							{
								sizeF2.Height += relativeSize3.Height;
								sizeF2.Width = Math.Max(sizeF2.Width, relativeSize3.Width);
							}
							else
							{
								sizeF2.Width += relativeSize3.Width;
								sizeF2.Height = Math.Max(sizeF2.Height, relativeSize3.Height);
							}
						}
					}
					if (customLabel4.LabelMark == LabelMark.Box)
					{
						SizeF relativeSize4 = chartGraph.GetRelativeSize(new SizeF(4f, 4f));
						sizeF2.Width += relativeSize4.Width;
						sizeF2.Height += relativeSize4.Height;
					}
					float num7 = sizeF2.Width;
					float num8 = sizeF2.Height;
					if (num3 != 0.0)
					{
						empty.Width *= 0.97f;
						if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
						{
							num7 = (float)Math.Cos(Math.Abs(num3) / 180.0 * 3.1415926535897931) * sizeF2.Height;
							num7 += (float)Math.Sin(Math.Abs(num3) / 180.0 * 3.1415926535897931) * sizeF2.Width;
							num8 = (float)Math.Sin(Math.Abs(num3) / 180.0 * 3.1415926535897931) * sizeF2.Height;
							num8 += (float)Math.Cos(Math.Abs(num3) / 180.0 * 3.1415926535897931) * sizeF2.Width;
						}
						else
						{
							num7 = (float)Math.Cos(Math.Abs(num3) / 180.0 * 3.1415926535897931) * sizeF2.Width;
							num7 += (float)Math.Sin(Math.Abs(num3) / 180.0 * 3.1415926535897931) * sizeF2.Height;
							num8 = (float)Math.Sin(Math.Abs(num3) / 180.0 * 3.1415926535897931) * sizeF2.Width;
							num8 += (float)Math.Cos(Math.Abs(num3) / 180.0 * 3.1415926535897931) * sizeF2.Height;
						}
					}
					if (labelPositions != null)
					{
						RectangleF rectangleF = empty;
						if (num3 == 0.0 || num3 == 90.0 || num3 == -90.0)
						{
							if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
							{
								rectangleF.X = (float)(rectangleF.X + rectangleF.Width / 2.0 - num7 / 2.0);
								rectangleF.Width = num7;
							}
							else
							{
								rectangleF.Y = (float)(rectangleF.Y + rectangleF.Height / 2.0 - num8 / 2.0);
								rectangleF.Height = num8;
							}
						}
						labelPositions.Add(rectangleF);
					}
					if (num3 == 0.0)
					{
						if (num7 >= empty.Width && checkWidth)
						{
							return false;
						}
						if (num8 >= empty.Height && checkHeight)
						{
							return false;
						}
					}
					if (num3 == 90.0 || num3 == -90.0)
					{
						if (num7 >= empty.Width && checkWidth)
						{
							return false;
						}
						if (num8 >= empty.Height && checkHeight)
						{
							return false;
						}
					}
					else if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
					{
						if (num7 >= empty.Width * 2.0 && checkWidth)
						{
							return false;
						}
						if (num8 >= empty.Height * 2.0 && checkHeight)
						{
							return false;
						}
					}
					else
					{
						if (num7 >= empty.Width * 2.0 && checkWidth)
						{
							return false;
						}
						if (num8 >= empty.Height * 2.0 && checkHeight)
						{
							return false;
						}
					}
					num4++;
				}
			}
			return true;
		}

		private float GetRequiredLabelSize(ChartGraphics chartGraph, float maxLabelSize, out float resultSize)
		{
			float num = 0f;
			resultSize = 0f;
			float num2 = (float)((this.autoLabelAngle < -90) ? base.LabelStyle.FontAngle : this.autoLabelAngle);
			this.labelNearOffset = 3.40282347E+38f;
			this.labelFarOffset = -3.40282347E+38f;
			StringFormat stringFormat = new StringFormat();
			stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			RectangleF rectangleF = base.chartArea.Position.ToRectangleF();
			foreach (CustomLabel customLabel in base.CustomLabels)
			{
				if (customLabel.RowIndex == 0)
				{
					double value = (customLabel.From + customLabel.To) / 2.0;
					if (!(Axis.RemoveNoiseFromDoubleMath(value) < Axis.RemoveNoiseFromDoubleMath(base.GetViewMinimum())) && !(Axis.RemoveNoiseFromDoubleMath(value) > Axis.RemoveNoiseFromDoubleMath(base.GetViewMaximum())))
					{
						goto IL_00e4;
					}
					continue;
				}
				goto IL_00e4;
				IL_00e4:
				if (customLabel.RowIndex == 0)
				{
					RectangleF rectangleF2 = rectangleF;
					rectangleF2.Width = maxLabelSize;
					double linearPosition = base.GetLinearPosition(customLabel.From);
					double linearPosition2 = base.GetLinearPosition(customLabel.To);
					rectangleF2.Y = (float)Math.Min(linearPosition, linearPosition2);
					rectangleF2.Height = (float)Math.Max(linearPosition, linearPosition2) - rectangleF2.Y;
					if ((this.autoLabelOffset == -1) ? base.LabelStyle.OffsetLabels : (this.autoLabelOffset == 1))
					{
						rectangleF2.Y -= (float)(rectangleF2.Height / 2.0);
						rectangleF2.Height *= 2f;
					}
					if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
					{
						float height = rectangleF2.Height;
						rectangleF2.Height = rectangleF2.Width;
						rectangleF2.Width = height;
						if (num2 != 0.0)
						{
							stringFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
						}
					}
					else if (num2 == 90.0 || num2 == -90.0)
					{
						num2 = 0f;
						stringFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
					}
					rectangleF2.Width = (float)Math.Ceiling((double)rectangleF2.Width);
					rectangleF2.Height = (float)Math.Ceiling((double)rectangleF2.Height);
					SizeF sizeF = chartGraph.MeasureStringRel(customLabel.Text.Replace("\\n", "\n"), (this.autoLabelFont != null) ? this.autoLabelFont : base.LabelStyle.Font, rectangleF2.Size, stringFormat);
					if (sizeF.Width == 0.0 || sizeF.Height == 0.0)
					{
						stringFormat.FormatFlags ^= StringFormatFlags.LineLimit;
						sizeF = chartGraph.MeasureStringRel(customLabel.Text.Replace("\\n", "\n"), (this.autoLabelFont != null) ? this.autoLabelFont : base.LabelStyle.Font, rectangleF2.Size, stringFormat);
						stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
					}
					if (customLabel.Image.Length > 0)
					{
						SizeF size = default(SizeF);
						if (base.Common.ImageLoader.GetAdjustedImageSize(customLabel.Image, chartGraph.Graphics, ref size))
						{
							SizeF relativeSize = chartGraph.GetRelativeSize(size);
							if ((stringFormat.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical)
							{
								sizeF.Height += relativeSize.Height;
								sizeF.Width = Math.Max(sizeF.Width, relativeSize.Width);
							}
							else
							{
								sizeF.Width += relativeSize.Width;
								sizeF.Height = Math.Max(sizeF.Height, relativeSize.Height);
							}
						}
					}
					if (customLabel.LabelMark == LabelMark.Box)
					{
						SizeF relativeSize2 = chartGraph.GetRelativeSize(new SizeF(4f, 4f));
						sizeF.Width += relativeSize2.Width;
						sizeF.Height += relativeSize2.Height;
					}
					float num3 = sizeF.Width;
					float num4 = sizeF.Height;
					if (num2 != 0.0)
					{
						num3 = (float)Math.Cos((90.0 - Math.Abs(num2)) / 180.0 * 3.1415926535897931) * sizeF.Width;
						num3 += (float)Math.Cos(Math.Abs(num2) / 180.0 * 3.1415926535897931) * sizeF.Height;
						num4 = (float)Math.Sin(Math.Abs(num2) / 180.0 * 3.1415926535897931) * sizeF.Height;
						num4 += (float)Math.Sin((90.0 - Math.Abs(num2)) / 180.0 * 3.1415926535897931) * sizeF.Width;
					}
					num3 = (float)((float)Math.Ceiling((double)num3) * 1.0499999523162842);
					num4 = (float)((float)Math.Ceiling((double)num4) * 1.0499999523162842);
					sizeF.Width = (float)((float)Math.Ceiling((double)sizeF.Width) * 1.0499999523162842);
					sizeF.Height = (float)((float)Math.Ceiling((double)sizeF.Height) * 1.0499999523162842);
					if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
					{
						if (num2 == 90.0 || num2 == -90.0 || num2 == 0.0)
						{
							resultSize = Math.Max(resultSize, sizeF.Height);
							num = Math.Max(num, sizeF.Height);
							this.labelNearOffset = (float)Math.Min((double)this.labelNearOffset, (linearPosition + linearPosition2) / 2.0 - sizeF.Width / 2.0);
							this.labelFarOffset = (float)Math.Max((double)this.labelFarOffset, (linearPosition + linearPosition2) / 2.0 + sizeF.Width / 2.0);
						}
						else
						{
							resultSize = Math.Max(resultSize, sizeF.Height);
							num = Math.Max(num, num4);
							if (num2 > 0.0)
							{
								this.labelFarOffset = (float)Math.Max((double)this.labelFarOffset, (linearPosition + linearPosition2) / 2.0 + num3 * 1.1000000238418579);
							}
							else
							{
								this.labelNearOffset = (float)Math.Min((double)this.labelNearOffset, (linearPosition + linearPosition2) / 2.0 - num3 * 1.1000000238418579);
							}
						}
					}
					else if (num2 == 90.0 || num2 == -90.0 || num2 == 0.0)
					{
						resultSize = Math.Max(resultSize, sizeF.Width);
						num = Math.Max(num, sizeF.Width);
						this.labelNearOffset = (float)Math.Min((double)this.labelNearOffset, (linearPosition + linearPosition2) / 2.0 - sizeF.Height / 2.0);
						this.labelFarOffset = (float)Math.Max((double)this.labelFarOffset, (linearPosition + linearPosition2) / 2.0 + sizeF.Height / 2.0);
					}
					else
					{
						resultSize = Math.Max(resultSize, sizeF.Width);
						num = Math.Max(num, num3);
						if (num2 > 0.0)
						{
							this.labelFarOffset = (float)Math.Max((double)this.labelFarOffset, (linearPosition + linearPosition2) / 2.0 + num4 * 1.1000000238418579);
						}
						else
						{
							this.labelNearOffset = (float)Math.Min((double)this.labelNearOffset, (linearPosition + linearPosition2) / 2.0 - num4 * 1.1000000238418579);
						}
					}
					if (resultSize > maxLabelSize)
					{
						resultSize = maxLabelSize;
					}
				}
			}
			if ((this.autoLabelOffset == -1) ? base.LabelStyle.OffsetLabels : (this.autoLabelOffset == 1))
			{
				resultSize *= 2f;
				num = (float)(num * 2.0);
				if (resultSize > maxLabelSize)
				{
					resultSize = maxLabelSize;
					num = maxLabelSize;
				}
			}
			if (base.chartArea.Area3DStyle.Enable3D && !base.chartArea.chartAreaIsCurcular)
			{
				resultSize *= 1.1f;
				num = (float)(num * 1.1000000238418579);
			}
			return num;
		}

		internal float GetGroupLablesToatalSize()
		{
			float num = 0f;
			if (this.groupingLabelSizes != null && this.groupingLabelSizes.Length > 0)
			{
				float[] array = this.groupingLabelSizes;
				foreach (float num2 in array)
				{
					num += num2;
				}
			}
			return num;
		}

		internal int GetGroupLabelLevelCount()
		{
			int num = 0;
			foreach (CustomLabel customLabel in base.CustomLabels)
			{
				if (customLabel.RowIndex > 0)
				{
					num = Math.Max(num, customLabel.RowIndex);
				}
			}
			return num;
		}

		private float[] GetRequiredGroupLabelSize(ChartGraphics chartGraph, float maxLabelSize)
		{
			float[] array = null;
			int groupLabelLevelCount = this.GetGroupLabelLevelCount();
			if (groupLabelLevelCount > 0)
			{
				array = new float[groupLabelLevelCount];
				for (int i = 1; i <= groupLabelLevelCount; i++)
				{
					array[i - 1] = 0f;
					foreach (CustomLabel customLabel in base.CustomLabels)
					{
						if (customLabel.RowIndex == 0)
						{
							double num = (customLabel.From + customLabel.To) / 2.0;
							if (!(num < base.GetViewMinimum()) && !(num > base.GetViewMaximum()))
							{
								goto IL_0082;
							}
							continue;
						}
						goto IL_0082;
						IL_0082:
						if (customLabel.RowIndex == i)
						{
							SizeF sizeF = chartGraph.MeasureStringRel(customLabel.Text.Replace("\\n", "\n"), (this.autoLabelFont != null) ? this.autoLabelFont : base.LabelStyle.Font);
							sizeF.Width = (float)Math.Ceiling((double)sizeF.Width);
							sizeF.Height = (float)Math.Ceiling((double)sizeF.Height);
							if (customLabel.Image.Length > 0)
							{
								SizeF size = default(SizeF);
								if (base.Common.ImageLoader.GetAdjustedImageSize(customLabel.Image, chartGraph.Graphics, ref size))
								{
									SizeF relativeSize = chartGraph.GetRelativeSize(size);
									sizeF.Width += relativeSize.Width;
									sizeF.Height = Math.Max(sizeF.Height, relativeSize.Height);
								}
							}
							if (customLabel.LabelMark == LabelMark.Box)
							{
								SizeF relativeSize2 = chartGraph.GetRelativeSize(new SizeF(4f, 4f));
								sizeF.Width += relativeSize2.Width;
								sizeF.Height += relativeSize2.Height;
							}
							if (this.AxisPosition == AxisPosition.Bottom || this.AxisPosition == AxisPosition.Top)
							{
								array[i - 1] = Math.Max(array[i - 1], sizeF.Height);
							}
							else
							{
								sizeF.Width = chartGraph.GetAbsoluteSize(new SizeF(sizeF.Height, sizeF.Height)).Height;
								sizeF.Width = chartGraph.GetRelativeSize(new SizeF(sizeF.Width, sizeF.Width)).Width;
								array[i - 1] = Math.Max(array[i - 1], sizeF.Width);
							}
							float num2 = array[i - 1];
							float num3 = maxLabelSize / (float)groupLabelLevelCount;
						}
					}
				}
			}
			return array;
		}

		internal static double RemoveNoiseFromDoubleMath(double value)
		{
			if (value != 0.0 && (!(Math.Abs(value) > 7.9228162514264341E-28) || !(Math.Abs(value) < 7.9228162514264338E+28)))
			{
				if (!(Math.Abs(value) >= 1.7976931348623157E+308) && !(Math.Abs(value) <= 4.94065645841247E-324))
				{
					return double.Parse(value.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
				}
				return value;
			}
			return (double)(decimal)value;
		}

		internal Axis GetSubAxis(string subAxisName)
		{
			return this;
		}

		internal bool IsMarksNextToAxis()
		{
			if (base.chartArea != null && base.chartArea.chartAreaIsCurcular)
			{
				return true;
			}
			return this.MarksNextToAxis;
		}

		internal bool IsSerializing()
		{
			if (this.chart == null && base.Common != null && base.Common.container != null)
			{
				this.chart = (Chart)base.Common.container.GetService(typeof(Chart));
			}
			if (this.chart != null)
			{
				return this.chart.serializing;
			}
			if (base.chartArea != null)
			{
				if (base.chartArea.chart == null && base.chartArea.Common != null && base.chartArea.Common.container != null)
				{
					base.chartArea.chart = (Chart)base.chartArea.Common.container.GetService(typeof(Chart));
				}
				if (base.chartArea.chart != null)
				{
					return base.chartArea.chart.serializing;
				}
			}
			return false;
		}

		internal DateTimeIntervalType GetAxisIntervalType()
		{
			if (base.InternalIntervalType == DateTimeIntervalType.Auto)
			{
				if (this.GetAxisValuesType() != ChartValueTypes.DateTime && this.GetAxisValuesType() != ChartValueTypes.Date && this.GetAxisValuesType() != ChartValueTypes.Time && this.GetAxisValuesType() != ChartValueTypes.DateTimeOffset)
				{
					return DateTimeIntervalType.Number;
				}
				return DateTimeIntervalType.Years;
			}
			return base.InternalIntervalType;
		}

		internal ChartValueTypes GetAxisValuesType()
		{
			ChartValueTypes result = ChartValueTypes.Double;
			if (base.Common != null && base.Common.DataManager.Series != null && base.chartArea != null)
			{
				{
					foreach (Series item in base.Common.DataManager.Series)
					{
						bool flag = false;
						if (item.ChartArea == base.chartArea.Name && item.IsVisible())
						{
							if (base.axisType == AxisName.X && item.XAxisType == AxisType.Primary)
							{
								flag = true;
							}
							else if (base.axisType == AxisName.X2 && item.XAxisType == AxisType.Secondary)
							{
								flag = true;
							}
							else if (base.axisType == AxisName.Y && item.YAxisType == AxisType.Primary)
							{
								flag = true;
							}
							else if (base.axisType == AxisName.Y2 && item.YAxisType == AxisType.Secondary)
							{
								flag = true;
							}
						}
						if (flag)
						{
							if (base.axisType != 0 && base.axisType != AxisName.X2)
							{
								if (base.axisType != AxisName.Y && base.axisType != AxisName.Y2)
								{
									return result;
								}
								return item.YValueType;
							}
							return item.XValueType;
						}
					}
					return result;
				}
			}
			return result;
		}

		internal SizeF GetArrowSize(out ArrowOrientation arrowOrientation)
		{
			arrowOrientation = ArrowOrientation.Top;
			switch (this.AxisPosition)
			{
			case AxisPosition.Left:
				if (base.reverse)
				{
					arrowOrientation = ArrowOrientation.Bottom;
				}
				else
				{
					arrowOrientation = ArrowOrientation.Top;
				}
				break;
			case AxisPosition.Right:
				if (base.reverse)
				{
					arrowOrientation = ArrowOrientation.Bottom;
				}
				else
				{
					arrowOrientation = ArrowOrientation.Top;
				}
				break;
			case AxisPosition.Bottom:
				if (base.reverse)
				{
					arrowOrientation = ArrowOrientation.Left;
				}
				else
				{
					arrowOrientation = ArrowOrientation.Right;
				}
				break;
			case AxisPosition.Top:
				if (base.reverse)
				{
					arrowOrientation = ArrowOrientation.Left;
				}
				else
				{
					arrowOrientation = ArrowOrientation.Right;
				}
				break;
			}
			Axis axis;
			switch (arrowOrientation)
			{
			case ArrowOrientation.Left:
				axis = base.chartArea.AxisX;
				break;
			case ArrowOrientation.Right:
				axis = base.chartArea.AxisX2;
				break;
			case ArrowOrientation.Top:
				axis = base.chartArea.AxisY2;
				break;
			case ArrowOrientation.Bottom:
				axis = base.chartArea.AxisY;
				break;
			default:
				axis = base.chartArea.AxisX;
				break;
			}
			double num;
			double num2;
			if (arrowOrientation == ArrowOrientation.Top || arrowOrientation == ArrowOrientation.Bottom)
			{
				num = (double)this.lineWidth;
				num2 = (double)this.lineWidth * (double)base.Common.Width / (double)base.Common.Height;
			}
			else
			{
				num = (double)this.lineWidth * (double)base.Common.Width / (double)base.Common.Height;
				num2 = (double)this.lineWidth;
			}
			if (this.arrows == ArrowsType.SharpTriangle)
			{
				if (arrowOrientation != ArrowOrientation.Top && arrowOrientation != ArrowOrientation.Bottom)
				{
					return new SizeF((float)((double)axis.MajorTickMark.Size + num2 * 4.0), (float)(num * 2.0));
				}
				return new SizeF((float)(num * 2.0), (float)((double)axis.MajorTickMark.Size + num2 * 4.0));
			}
			if (this.arrows == ArrowsType.None)
			{
				return new SizeF(0f, 0f);
			}
			if (arrowOrientation != ArrowOrientation.Top && arrowOrientation != ArrowOrientation.Bottom)
			{
				return new SizeF((float)((double)axis.MajorTickMark.Size + num2 * 2.0), (float)(num * 2.0));
			}
			return new SizeF((float)(num * 2.0), (float)((double)axis.MajorTickMark.Size + num2 * 2.0));
		}

		private bool IsArrowInAxis(ArrowOrientation arrowOrientation, AxisPosition axisPosition)
		{
			if (axisPosition == AxisPosition.Top && arrowOrientation == ArrowOrientation.Top)
			{
				return true;
			}
			if (axisPosition == AxisPosition.Bottom && arrowOrientation == ArrowOrientation.Bottom)
			{
				return true;
			}
			if (axisPosition == AxisPosition.Left && arrowOrientation == ArrowOrientation.Left)
			{
				return true;
			}
			if (axisPosition == AxisPosition.Right && arrowOrientation == ArrowOrientation.Right)
			{
				return true;
			}
			return false;
		}

		internal float GetPixelInterval(double realInterval)
		{
			double num = (this.AxisPosition != AxisPosition.Top && this.AxisPosition != AxisPosition.Bottom) ? ((double)(base.PlotAreaPosition.Bottom() - base.PlotAreaPosition.Y)) : ((double)(base.PlotAreaPosition.Right() - base.PlotAreaPosition.X));
			if (base.GetViewMaximum() - base.GetViewMinimum() == 0.0)
			{
				return (float)(num / realInterval);
			}
			return (float)(num / (base.GetViewMaximum() - base.GetViewMinimum()) * realInterval);
		}

		internal bool IsAxisOnAreaEdge()
		{
			double num = 0.0;
			if (this.AxisPosition == AxisPosition.Bottom)
			{
				num = (double)base.PlotAreaPosition.Bottom();
			}
			else if (this.AxisPosition == AxisPosition.Left)
			{
				num = (double)base.PlotAreaPosition.X;
			}
			else if (this.AxisPosition == AxisPosition.Right)
			{
				num = (double)base.PlotAreaPosition.Right();
			}
			else if (this.AxisPosition == AxisPosition.Top)
			{
				num = (double)base.PlotAreaPosition.Y;
			}
			if (Math.Abs(this.GetAxisPosition() - num) < 0.0015)
			{
				return true;
			}
			return false;
		}

		internal double GetAxisPosition()
		{
			return this.GetAxisPosition(false);
		}

		internal virtual double GetAxisPosition(bool ignoreCrossing)
		{
			Axis oppositeAxis = base.GetOppositeAxis();
			if (base.chartArea != null && base.chartArea.chartAreaIsCurcular)
			{
				return base.PlotAreaPosition.X + base.PlotAreaPosition.Width / 2.0;
			}
			if (oppositeAxis.maximum == oppositeAxis.minimum || double.IsNaN(oppositeAxis.maximum) || double.IsNaN(oppositeAxis.minimum) || base.maximum == base.minimum || double.IsNaN(base.maximum) || double.IsNaN(base.minimum))
			{
				switch (this.AxisPosition)
				{
				case AxisPosition.Top:
					return (double)base.PlotAreaPosition.Y;
				case AxisPosition.Bottom:
					return (double)base.PlotAreaPosition.Bottom();
				case AxisPosition.Right:
					return (double)base.PlotAreaPosition.Right();
				case AxisPosition.Left:
					return (double)base.PlotAreaPosition.X;
				}
			}
			if (!double.IsNaN(oppositeAxis.crossing) && !ignoreCrossing)
			{
				oppositeAxis.crossing = oppositeAxis.tempCrossing;
				if (oppositeAxis.crossing < oppositeAxis.GetViewMinimum())
				{
					oppositeAxis.crossing = oppositeAxis.GetViewMinimum();
				}
				else if (oppositeAxis.crossing > oppositeAxis.GetViewMaximum())
				{
					oppositeAxis.crossing = oppositeAxis.GetViewMaximum();
				}
				return oppositeAxis.GetLinearPosition(oppositeAxis.crossing);
			}
			if (base.axisType != 0 && base.axisType != AxisName.Y)
			{
				return oppositeAxis.GetLinearPosition(oppositeAxis.GetViewMaximum());
			}
			return oppositeAxis.GetLinearPosition(oppositeAxis.GetViewMinimum());
		}

		internal double GetAxisProjectionAngle()
		{
			bool flag = default(bool);
			float marksZPosition = this.GetMarksZPosition(out flag);
			float num = (float)this.GetAxisPosition();
			Point3D[] array = new Point3D[2];
			if (this.AxisPosition == AxisPosition.Top || this.AxisPosition == AxisPosition.Bottom)
			{
				array[0] = new Point3D(0f, num, marksZPosition);
				array[1] = new Point3D(100f, num, marksZPosition);
			}
			else
			{
				array[0] = new Point3D(num, 0f, marksZPosition);
				array[1] = new Point3D(num, 100f, marksZPosition);
			}
			base.chartArea.matrix3D.TransformPoints(array);
			array[0].X = (float)Math.Round((double)array[0].X, 4);
			array[0].Y = (float)Math.Round((double)array[0].Y, 4);
			array[1].X = (float)Math.Round((double)array[1].X, 4);
			array[1].Y = (float)Math.Round((double)array[1].Y, 4);
			double num2 = 0.0;
			num2 = ((this.AxisPosition != AxisPosition.Top && this.AxisPosition != AxisPosition.Bottom) ? Math.Atan((double)((array[1].X - array[0].X) / (array[1].Y - array[0].Y))) : Math.Atan((double)((array[1].Y - array[0].Y) / (array[1].X - array[0].X))));
			return num2 * 180.0 / 3.1415926535897931;
		}
	}
}
