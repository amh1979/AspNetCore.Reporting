using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Data;
using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeSeries_Series")]
	[DefaultProperty("Points")]
	internal class Series : DataPointAttributes
	{
		internal enum SeriesValuesFormulaType
		{
			Total,
			Average,
			Maximum,
			Minimum,
			First,
			Last
		}

		private string name = "";

		private ChartValueTypes xValueType;

		private ChartValueTypes yValueType;

		private bool xValueIndexed;

		private int yValuesPerPoint = 1;

		private int markersStep = 1;

		private ChartColorPalette colorPalette;

		private AxisType xAxisType;

		private AxisType yAxisType;

		private DataPointAttributes emptyPointAttributes = new DataPointAttributes(null, false);

		private DataPointCollection points;

		private int shadowOffset;

		private Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		private string chartType = "Column";

		private string chartArea = "Default";

		private bool enabled = true;

		private string legend = "Default";

		private string dataSourceMemberX = string.Empty;

		private string dataSourceMemberY = string.Empty;

		internal bool autoXValueType;

		internal bool autoYValueType;

		private double totalYvalue = double.NaN;

		private double[] dummyDoubleValues;

		internal ChartValueTypes indexedXValueType;

		internal static DataPointAttributes defaultAttributes;

		internal bool tempMarkerStyleIsSet;

		private bool defaultChartArea;

		private bool checkPointsNumber = true;

		internal Chart chart;

		internal FinancialMarkersCollection financialMarkers;

		private SmartLabelsStyle smartLabels;

		internal bool noLabelsInPoints = true;

		internal bool xValuesZeros;

		internal bool xValuesZerosChecked;

		internal DataPointCollection fakeDataPoints;

		internal string label = "";

		internal string axisLabel = "";

		internal string labelFormat = "";

		internal bool showLabelAsValue;

		internal Color color = Color.Empty;

		internal Color borderColor = Color.Empty;

		internal ChartDashStyle borderStyle = ChartDashStyle.Solid;

		internal int borderWidth = 1;

		internal int markerBorderWidth = 1;

		internal string backImage = "";

		internal ChartImageWrapMode backImageMode;

		internal Color backImageTranspColor = Color.Empty;

		internal ChartImageAlign backImageAlign;

		internal GradientType backGradientType;

		internal Color backGradientEndColor = Color.Empty;

		internal ChartHatchStyle backHatchStyle;

		internal Font font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		internal Color fontColor = Color.Black;

		internal int fontAngle;

		internal MarkerStyle markerStyle;

		internal int markerSize = 5;

		internal string markerImage = "";

		internal Color markerImageTranspColor = Color.Empty;

		internal Color markerColor = Color.Empty;

		internal Color markerBorderColor = Color.Empty;

		internal string toolTip = "";

		internal string href = "";

		internal string mapAreaAttributes = "";

		internal bool showInLegend = true;

		internal string legendText = "";

		internal string legendToolTip = "";

		internal Color labelBackColor = Color.Empty;

		internal Color labelBorderColor = Color.Empty;

		internal ChartDashStyle labelBorderStyle = ChartDashStyle.Solid;

		internal int labelBorderWidth = 1;

		internal string labelToolTip = "";

		internal string labelHref = "";

		internal string labelMapAreaAttributes = "";

		internal string legendHref = "";

		internal string legendMapAreaAttributes = "";

		[SRCategory("CategoryAttributeData")]
		[SRDescription("DescriptionAttributeSeries_Name")]
		[Bindable(true)]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (!(value != this.name))
				{
					return;
				}
				if (value != null && value.Length != 0)
				{
					if (base.serviceContainer != null)
					{
						DataManager dataManager = (DataManager)base.serviceContainer.GetService(typeof(DataManager));
						if (dataManager != null && dataManager.Series.GetIndex(value) != -1)
						{
							throw new ArgumentException(SR.ExceptionSeriesNameIsNotUnique(value));
						}
					}
					this.name = value;
					this.Invalidate(false, true);
					return;
				}
				throw new ArgumentException(SR.ExceptionSeriesNameIsEmpty);
			}
		}

		[DefaultValue("")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeDataSource")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_ValueMemberX")]
		public string ValueMemberX
		{
			get
			{
				return this.dataSourceMemberX;
			}
			set
			{
				if (value == "(none)")
				{
					this.dataSourceMemberX = string.Empty;
				}
				else
				{
					this.dataSourceMemberX = value;
				}
				if (base.serviceContainer != null)
				{
					ChartImage chartImage = (ChartImage)base.serviceContainer.GetService(typeof(ChartImage));
					if (chartImage != null)
					{
						chartImage.boundToDataSource = false;
					}
				}
			}
		}

		[SRDescription("DescriptionAttributeSeries_ValueMembersY")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeDataSource")]
		[Bindable(true)]
		[DefaultValue("")]
		public string ValueMembersY
		{
			get
			{
				return this.dataSourceMemberY;
			}
			set
			{
				if (value == "(none)")
				{
					this.dataSourceMemberY = string.Empty;
				}
				else
				{
					this.dataSourceMemberY = value;
				}
				if (base.serviceContainer != null)
				{
					ChartImage chartImage = (ChartImage)base.serviceContainer.GetService(typeof(ChartImage));
					if (chartImage != null)
					{
						chartImage.boundToDataSource = false;
					}
				}
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeLegend")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeSeries_Legend")]
		[DefaultValue("Default")]
		[TypeConverter(typeof(SeriesLegendNameConverter))]
		public string Legend
		{
			get
			{
				return this.legend;
			}
			set
			{
				if (value.Length == 0)
				{
					this.legend = "Default";
				}
				else
				{
					this.legend = value;
				}
				this.Invalidate(false, true);
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_XValueType")]
		[DefaultValue(ChartValueTypes.Auto)]
		public ChartValueTypes XValueType
		{
			get
			{
				return this.xValueType;
			}
			set
			{
				this.xValueType = value;
				this.autoXValueType = false;
				this.Invalidate(true, false);
			}
		}

		[SRDescription("DescriptionAttributeSeries_XValueIndexed")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeData")]
		[DefaultValue(false)]
		public bool XValueIndexed
		{
			get
			{
				return this.xValueIndexed;
			}
			set
			{
				this.xValueIndexed = value;
				this.Invalidate(true, false);
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_YValueType")]
		[DefaultValue(ChartValueTypes.Auto)]
		[TypeConverter(typeof(SeriesYValueTypeConverter))]
		public ChartValueTypes YValueType
		{
			get
			{
				return this.yValueType;
			}
			set
			{
				this.yValueType = value;
				this.autoYValueType = false;
				this.Invalidate(true, false);
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_YValuesPerPoint")]
		[DefaultValue(1)]
		public int YValuesPerPoint
		{
			get
			{
				if (this.checkPointsNumber && this.ChartTypeName.Length > 0 && base.serviceContainer != null)
				{
					this.checkPointsNumber = false;
					ChartTypeRegistry chartTypeRegistry = (ChartTypeRegistry)base.serviceContainer.GetService(typeof(ChartTypeRegistry));
					IChartType chartType = chartTypeRegistry.GetChartType(this.ChartTypeName);
					if (chartType.YValuesPerPoint > this.yValuesPerPoint)
					{
						this.yValuesPerPoint = chartType.YValuesPerPoint;
						if (this.points.Count > 0)
						{
							foreach (DataPoint point in this.points)
							{
								point.ResizeYValueArray(this.yValuesPerPoint);
							}
						}
					}
				}
				return this.yValuesPerPoint;
			}
			set
			{
				if (value >= 1 && value <= 32)
				{
					this.checkPointsNumber = true;
					if (this.points.Count > 0)
					{
						foreach (DataPoint point in this.points)
						{
							point.ResizeYValueArray(value);
						}
					}
					this.yValuesPerPoint = value;
					this.Invalidate(true, false);
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionDataSeriesYValueNumberInvalid);
			}
		}

		[SRCategory("CategoryAttributeData")]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_Points")]
		public DataPointCollection Points
		{
			get
			{
				return this.points;
			}
		}

		[SRCategory("CategoryAttributeEmptyPoints")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_EmptyPointStyle")]
		public DataPointAttributes EmptyPointStyle
		{
			get
			{
				return this.emptyPointAttributes;
			}
			set
			{
				if (value.series == null && this.emptyPointAttributes.series != null)
				{
					value.series = this.emptyPointAttributes.series;
				}
				this.emptyPointAttributes = value;
				this.emptyPointAttributes.pointAttributes = false;
				this.emptyPointAttributes.SetDefault(false);
				this.emptyPointAttributes.pointAttributes = true;
				this.Invalidate(true, false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartColorPalette.None)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributePalette")]
		public ChartColorPalette Palette
		{
			get
			{
				return this.colorPalette;
			}
			set
			{
				this.colorPalette = value;
				this.Invalidate(true, true);
			}
		}

		[DefaultValue(1)]
		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_MarkerStep")]
		public int MarkerStep
		{
			get
			{
				return this.markersStep;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException(SR.ExceptionMarkerStepNegativeValue, "value");
				}
				this.markersStep = value;
				this.Invalidate(true, false);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeSeries_ShadowOffset")]
		public int ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				this.shadowOffset = value;
				this.Invalidate(true, true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeSeries_ShadowColor")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "128,0,0,0")]
		public Color ShadowColor
		{
			get
			{
				return this.shadowColor;
			}
			set
			{
				this.shadowColor = value;
				this.Invalidate(true, true);
			}
		}

		[SRDescription("DescriptionAttributeSeries_FinancialMarkers")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeFinancialMarkers")]
		[Bindable(true)]
		public FinancialMarkersCollection FinancialMarkers
		{
			get
			{
				this.financialMarkers.series = this;
				return this.financialMarkers;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAxes")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_YSubAxisName")]
		[DefaultValue("")]
		internal string YSubAxisName
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[Bindable(true)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAxes")]
		[SRDescription("DescriptionAttributeSeries_XSubAxisName")]
		[DefaultValue("")]
		internal string XSubAxisName
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		[SRCategory("CategoryAttributeAxes")]
		[Browsable(false)]
		[DefaultValue(AxisType.Primary)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_XAxisType")]
		public AxisType XAxisType
		{
			get
			{
				return this.xAxisType;
			}
			set
			{
				this.xAxisType = value;
				this.Invalidate(true, false);
			}
		}

		[Browsable(false)]
		[DefaultValue(AxisType.Primary)]
		[SRCategory("CategoryAttributeAxes")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_YAxisType")]
		public AxisType YAxisType
		{
			get
			{
				return this.yAxisType;
			}
			set
			{
				this.yAxisType = value;
				this.Invalidate(true, false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[ParenthesizePropertyName(true)]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeSeries_Enabled")]
		[NotifyParentProperty(true)]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
				this.Invalidate(true, true);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeChart")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_Type")]
		[DefaultValue(SeriesChartType.Column)]
		[RefreshProperties(RefreshProperties.All)]
		public SeriesChartType ChartType
		{
			get
			{
				SeriesChartType result = SeriesChartType.Column;
				if (string.Compare(this.ChartTypeName, "100%StackedArea", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return SeriesChartType.StackedArea100;
				}
				if (string.Compare(this.ChartTypeName, "100%StackedBar", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return SeriesChartType.StackedBar100;
				}
				if (string.Compare(this.ChartTypeName, "100%StackedColumn", StringComparison.OrdinalIgnoreCase) == 0)
				{
					return SeriesChartType.StackedColumn100;
				}
				try
				{
					result = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), this.ChartTypeName, true);
					return result;
				}
				catch
				{
					return result;
				}
			}
			set
			{
				this.ChartTypeName = Series.GetChartTypeName(value);
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeChart")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_Type")]
		[DefaultValue("Column")]
		[TypeConverter(typeof(ChartTypeConverter))]
		[RefreshProperties(RefreshProperties.All)]
		public string ChartTypeName
		{
			get
			{
				return this.chartType;
			}
			set
			{
				if (this.chartType != value && value.Length > 0 && base.serviceContainer != null)
				{
					ChartTypeRegistry chartTypeRegistry = (ChartTypeRegistry)base.serviceContainer.GetService(typeof(ChartTypeRegistry));
					if (chartTypeRegistry != null)
					{
						IChartType chartType = chartTypeRegistry.GetChartType(value);
						if (this.yValuesPerPoint < chartType.YValuesPerPoint)
						{
							this.yValuesPerPoint = chartType.YValuesPerPoint;
							if (this.points.Count > 0)
							{
								foreach (DataPoint point in this.points)
								{
									point.ResizeYValueArray(this.yValuesPerPoint);
								}
							}
						}
					}
				}
				this.chartType = value;
				this.Invalidate(false, true);
			}
		}

		[Browsable(false)]
		[DefaultValue("Default")]
		[SRCategory("CategoryAttributeChart")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_ChartArea")]
		[TypeConverter(typeof(SeriesAreaNameConverter))]
		public string ChartArea
		{
			get
			{
				return this.chartArea;
			}
			set
			{
				this.chartArea = value;
				this.defaultChartArea = false;
				this.Invalidate(false, true);
			}
		}

		[Browsable(false)]
		[SRDescription("DescriptionAttributeAxisLabel")]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[DefaultValue("")]
		public override string AxisLabel
		{
			get
			{
				return base.AxisLabel;
			}
			set
			{
				base.AxisLabel = value;
				this.Invalidate(true, false);
			}
		}

		[Browsable(true)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeSeries_SmartLabels")]
		[SRCategory("CategoryAttributeLabel")]
		public SmartLabelsStyle SmartLabels
		{
			get
			{
				return this.smartLabels;
			}
			set
			{
				value.chartElement = this;
				this.smartLabels = value;
				this.Invalidate(false, false);
			}
		}

		static Series()
		{
			Series.defaultAttributes = new DataPointAttributes(null, false);
			Series.defaultAttributes.SetDefault(true);
			Series.defaultAttributes.pointAttributes = true;
		}

		public Series()
			: base(null, false)
		{
			this.InitProperties(null, 0);
		}

		public Series(string name)
			: base(null, false)
		{
			if (name == null)
			{
				throw new ArgumentNullException(SR.ExceptionDataSeriesNameIsEmpty);
			}
			this.InitProperties(name, 0);
		}

		public Series(string name, int yValues)
			: base(null, false)
		{
			if (name == null)
			{
				throw new ArgumentNullException(SR.ExceptionDataSeriesNameIsEmpty);
			}
			if (this.YValuesPerPoint < 1)
			{
				throw new ArgumentOutOfRangeException("yValues", SR.ExceptionDataSeriesYValuesPerPointIsZero);
			}
			this.InitProperties(name, yValues);
		}

		private void InitProperties(string name, int YValuesPerPoint)
		{
			base.series = this;
			this.emptyPointAttributes.series = this;
			this.points = new DataPointCollection(this);
			this.fakeDataPoints = new DataPointCollection(this);
			if (name != null)
			{
				this.name = name;
			}
			if (YValuesPerPoint != 0)
			{
				this.yValuesPerPoint = YValuesPerPoint;
			}
			base.SetDefault(true);
			this.emptyPointAttributes.SetDefault(true);
			this.emptyPointAttributes.pointAttributes = true;
			if (this.financialMarkers == null)
			{
				this.financialMarkers = new FinancialMarkersCollection();
			}
			this.smartLabels = new SmartLabelsStyle(this);
		}

		internal string GetCaption()
		{
			if (this.IsAttributeSet("SeriesCaption"))
			{
				return base["SeriesCaption"];
			}
			return this.Name;
		}

		internal void GetPointDepthAndGap(ChartGraphics graph, Axis axis, ref double pointDepth, ref double pointGapDepth)
		{
			string text = base["PixelPointDepth"];
			if (text != null)
			{
				try
				{
					pointDepth = CommonElements.ParseDouble(text);
					SizeF relativeSize = graph.GetRelativeSize(new SizeF((float)pointDepth, (float)pointDepth));
					pointDepth = (double)relativeSize.Width;
					if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
					{
						pointDepth = (double)relativeSize.Height;
					}
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("PixelPointDepth"));
				}
			}
			text = base["PixelPointGapDepth"];
			if (text != null)
			{
				try
				{
					pointGapDepth = CommonElements.ParseDouble(text);
					SizeF relativeSize2 = graph.GetRelativeSize(new SizeF((float)pointGapDepth, (float)pointGapDepth));
					pointGapDepth = (double)relativeSize2.Width;
					if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
					{
						pointGapDepth = (double)relativeSize2.Height;
					}
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("PixelPointGapDepth"));
				}
			}
		}

		internal double GetPointWidth(ChartGraphics graph, Axis axis, double interval, double defaultWidth)
		{
			double num = defaultWidth;
			double num2 = 0.0;
			string text = base["PointWidth"];
			if (text != null)
			{
				num = CommonElements.ParseDouble(text);
			}
			num2 = (double)axis.GetPixelInterval(interval * num);
			SizeF absoluteSize = graph.GetAbsoluteSize(new SizeF((float)num2, (float)num2));
			double num3 = (double)absoluteSize.Width;
			if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
			{
				num3 = (double)absoluteSize.Height;
			}
			bool flag = false;
			string text2 = base["MinPixelPointWidth"];
			if (text2 != null)
			{
				double num4 = 0.0;
				try
				{
					num4 = CommonElements.ParseDouble(text2);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("MinPixelPointWidth"));
				}
				if (num4 <= 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotLargerThenZiro("MinPixelPointWidth"));
				}
				if (num3 < num4)
				{
					flag = true;
					num3 = num4;
				}
			}
			text2 = base["MaxPixelPointWidth"];
			if (text2 != null)
			{
				double num5 = 0.0;
				try
				{
					num5 = CommonElements.ParseDouble(text2);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("MaxPixelPointWidth"));
				}
				if (num5 <= 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotLargerThenZiro("MaxPixelPointWidth"));
				}
				if (num3 > num5)
				{
					flag = true;
					num3 = num5;
				}
			}
			text2 = base["PixelPointWidth"];
			if (text2 != null)
			{
				flag = true;
				num3 = 0.0;
				try
				{
					num3 = CommonElements.ParseDouble(text2);
				}
				catch
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid2("PixelPointWidth"));
				}
				if (num3 <= 0.0)
				{
					throw new InvalidOperationException(SR.ExceptionCustomAttributeIsNotLargerThenZiro("PixelPointWidth"));
				}
			}
			if (flag)
			{
				SizeF relativeSize = graph.GetRelativeSize(new SizeF((float)num3, (float)num3));
				num2 = (double)relativeSize.Width;
				if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
				{
					num2 = (double)relativeSize.Height;
				}
			}
			return num2;
		}

		internal static string GetChartTypeName(SeriesChartType type)
		{
			switch (type)
			{
			case SeriesChartType.StackedArea100:
				return "100%StackedArea";
			case SeriesChartType.StackedBar100:
				return "100%StackedBar";
			case SeriesChartType.StackedColumn100:
				return "100%StackedColumn";
			default:
				return Enum.GetName(typeof(SeriesChartType), type);
			}
		}

		internal bool IsYValueDateTime()
		{
			if (this.YValueType != ChartValueTypes.Date && this.YValueType != ChartValueTypes.DateTime && this.YValueType != ChartValueTypes.Time && this.YValueType != ChartValueTypes.DateTimeOffset)
			{
				return false;
			}
			return true;
		}

		internal bool IsXValueDateTime()
		{
			if (this.XValueType != ChartValueTypes.Date && this.XValueType != ChartValueTypes.DateTime && this.XValueType != ChartValueTypes.Time && this.XValueType != ChartValueTypes.DateTimeOffset)
			{
				return false;
			}
			return true;
		}

		internal bool IsVisible()
		{
			if (this.Enabled && this.ChartArea.Length > 0)
			{
				return true;
			}
			return false;
		}

		internal bool IsFastChartType()
		{
			if (this.ChartType == SeriesChartType.FastLine)
			{
				return true;
			}
			if (this.ChartType == SeriesChartType.FastPoint)
			{
				return true;
			}
			return false;
		}

		internal void CheckSupportedTypes(Type type)
		{
			if (type != typeof(double) && type != typeof(DateTime) && type != typeof(string) && type != typeof(int) && type != typeof(uint) && type != typeof(decimal) && type != typeof(float) && type != typeof(short) && type != typeof(ushort) && type != typeof(long) && type != typeof(ulong) && type != typeof(byte) && type != typeof(sbyte) && type != typeof(DBNull) && type != typeof(bool))
			{
				throw new ArrayTypeMismatchException(SR.ExceptionDataSeriesPointTypeUnsupported(type.ToString()));
			}
		}

		internal void ApplyPaletteColors()
		{
			ChartColorPalette palette = this.Palette;
			DataManager dataManager = (DataManager)base.serviceContainer.GetService(typeof(DataManager));
			if (palette == ChartColorPalette.None)
			{
				palette = dataManager.Palette;
			}
			if (palette == ChartColorPalette.None && dataManager.PaletteCustomColors.Length <= 0)
			{
				return;
			}
			int num = 0;
			Color[] array = (dataManager.PaletteCustomColors.Length > 0) ? dataManager.PaletteCustomColors : ChartPaletteColors.GetPaletteColors(palette);
			bool flag = !this.IsAttributeSet("SkipPaletteColorForEmptyPoint") || string.Compare(base["SkipPaletteColorForEmptyPoint"], "FALSE", StringComparison.OrdinalIgnoreCase) != 0;
			foreach (DataPoint point in this.points)
			{
				if ((!flag || !point.Empty) && (!point.IsAttributeSet(CommonAttributes.Color) || point.tempColorIsSet))
				{
					if (!point.Empty)
					{
						point.SetAttributeObject(CommonAttributes.Color, array[num]);
						point.tempColorIsSet = true;
					}
					num++;
					if (num >= array.Length)
					{
						num = 0;
					}
				}
			}
		}

		internal IEnumerable GetDummyData(ChartValueTypes type)
		{
			string[] result = new string[6]
			{
				"abc1",
				"abc2",
				"abc3",
				"abc4",
				"abc5",
				"abc6"
			};
			DateTime[] result2 = new DateTime[6]
			{
				DateTime.Now.Date,
				DateTime.Now.Date.AddDays(1.0),
				DateTime.Now.Date.AddDays(2.0),
				DateTime.Now.Date.AddDays(3.0),
				DateTime.Now.Date.AddDays(4.0),
				DateTime.Now.Date.AddDays(4.0)
			};
			if (this.dummyDoubleValues == null)
			{
				int num = 0;
				for (int i = 0; i < this.name.Length; i++)
				{
					num += this.name[i];
				}
				Random random = new Random(num);
				this.dummyDoubleValues = new double[6];
				for (int j = 0; j < 6; j++)
				{
					this.dummyDoubleValues[j] = (double)random.Next(10, 100);
				}
			}
			switch (type)
			{
			case ChartValueTypes.DateTime:
			case ChartValueTypes.Date:
			case ChartValueTypes.DateTimeOffset:
				return result2;
			case ChartValueTypes.Time:
				return new DateTime[6]
				{
					DateTime.Now,
					DateTime.Now.AddMinutes(1.0),
					DateTime.Now.AddMinutes(2.0),
					DateTime.Now.AddMinutes(3.0),
					DateTime.Now.AddMinutes(4.0),
					DateTime.Now.AddMinutes(4.0)
				};
			case ChartValueTypes.String:
				return result;
			default:
				return this.dummyDoubleValues;
			}
		}

		internal double GetTotalYValue()
		{
			return this.GetTotalYValue(0);
		}

		internal double GetTotalYValue(int yValueIndex)
		{
			if (yValueIndex == 0)
			{
				if (!double.IsNaN(this.totalYvalue))
				{
					return this.totalYvalue;
				}
				this.totalYvalue = 0.0;
				foreach (DataPoint point in this.Points)
				{
					this.totalYvalue += point.YValues[yValueIndex];
				}
				return this.totalYvalue;
			}
			if (yValueIndex >= this.YValuesPerPoint)
			{
				throw new InvalidOperationException(SR.ExceptionDataSeriesYValueIndexNotExists(yValueIndex.ToString(CultureInfo.InvariantCulture), this.Name));
			}
			double num = 0.0;
			foreach (DataPoint point2 in this.Points)
			{
				num += point2.YValues[yValueIndex];
			}
			return num;
		}

		internal string ReplaceKeywords(string strOriginal)
		{
			if (strOriginal != null && strOriginal.Length != 0)
			{
				string text = strOriginal.Replace("\\n", "\n");
				text = text.Replace("#SERIESNAME", this.Name);
				text = text.Replace("#SER", this.Name);
				text = this.ReplaceOneKeyword(this.chart, this, ChartElementType.Nothing, text, "#TOTAL", SeriesValuesFormulaType.Total, this.YValueType, "");
				text = this.ReplaceOneKeyword(this.chart, this, ChartElementType.Nothing, text, "#AVG", SeriesValuesFormulaType.Average, this.YValueType, "");
				text = this.ReplaceOneKeyword(this.chart, this, ChartElementType.Nothing, text, "#MAX", SeriesValuesFormulaType.Maximum, this.YValueType, "");
				text = this.ReplaceOneKeyword(this.chart, this, ChartElementType.Nothing, text, "#MIN", SeriesValuesFormulaType.Minimum, this.YValueType, "");
				text = this.ReplaceOneKeyword(this.chart, this, ChartElementType.Nothing, text, "#FIRST", SeriesValuesFormulaType.First, this.YValueType, "");
				text = this.ReplaceOneKeyword(this.chart, this, ChartElementType.Nothing, text, "#LAST", SeriesValuesFormulaType.Last, this.YValueType, "");
				return text.Replace("#LEGENDTEXT", base.LegendText);
			}
			return strOriginal;
		}

		internal string ReplaceOneKeyword(Chart chart, object obj, ChartElementType elementType, string strOriginal, string keyword, SeriesValuesFormulaType formulaType, ChartValueTypes valueType, string defaultFormat)
		{
			string text = strOriginal;
			int num = -1;
			while ((num = text.IndexOf(keyword, StringComparison.Ordinal)) != -1)
			{
				int num2 = num + keyword.Length;
				int num3 = 0;
				if (text.Length > num2 + 1 && text[num2] == 'Y' && char.IsDigit(text[num2 + 1]))
				{
					num3 = int.Parse(text.Substring(num2 + 1, 1), CultureInfo.InvariantCulture);
					num2 += 2;
				}
				string format = defaultFormat;
				if (text.Length > num2 && text[num2] == '{')
				{
					int num4 = text.IndexOf('}', num2);
					if (num4 == -1)
					{
						throw new InvalidOperationException(SR.ExceptionDataSeriesKeywordFormatInvalid(text));
					}
					format = text.Substring(num2, num4 - num2).Trim('{', '}');
					num2 = num4 + 1;
				}
				text = text.Remove(num, num2 - num);
				double totalYValue = this.GetTotalYValue(num3);
				double num5 = 0.0;
				switch (formulaType)
				{
				case SeriesValuesFormulaType.Average:
					if (this.Points.Count > 0)
					{
						num5 = totalYValue / (double)this.Points.Count;
					}
					break;
				case SeriesValuesFormulaType.First:
					if (this.Points.Count > 0)
					{
						num5 = this.Points[0].YValues[num3];
					}
					break;
				case SeriesValuesFormulaType.Last:
					if (this.Points.Count > 0)
					{
						num5 = this.Points[this.Points.Count - 1].YValues[num3];
					}
					break;
				case SeriesValuesFormulaType.Maximum:
					if (this.Points.Count > 0)
					{
						num5 = -1.7976931348623157E+308;
						foreach (DataPoint point in this.Points)
						{
							num5 = Math.Max(num5, point.YValues[num3]);
						}
					}
					break;
				case SeriesValuesFormulaType.Minimum:
					if (this.Points.Count > 0)
					{
						num5 = 1.7976931348623157E+308;
						foreach (DataPoint point2 in this.Points)
						{
							num5 = Math.Min(num5, point2.YValues[num3]);
						}
					}
					break;
				case SeriesValuesFormulaType.Total:
					num5 = totalYValue;
					break;
				}
				text = text.Insert(num, ValueConverter.FormatValue(chart, obj, num5, format, valueType, elementType));
			}
			return text;
		}

		internal string ReplaceOneKeyword(Chart chart, object obj, ChartElementType elementType, string strOriginal, string keyword, double value, ChartValueTypes valueType, string defaultFormat)
		{
			string text = strOriginal;
			int num = -1;
			while ((num = text.IndexOf(keyword, StringComparison.Ordinal)) != -1)
			{
				int num2 = num + keyword.Length;
				string format = defaultFormat;
				if (text.Length > num2 && text[num2] == '{')
				{
					int num3 = text.IndexOf('}', num2);
					if (num3 == -1)
					{
						throw new InvalidOperationException(SR.ExceptionDataSeriesKeywordFormatInvalid(text));
					}
					format = text.Substring(num2, num3 - num2).Trim('{', '}');
					num2 = num3 + 1;
				}
				text = text.Remove(num, num2 - num);
				text = text.Insert(num, ValueConverter.FormatValue(chart, obj, value, format, valueType, elementType));
			}
			return text;
		}

		internal void TraceWrite(string category, string message)
		{
			if (base.serviceContainer != null)
			{
				TraceManager traceManager = (TraceManager)base.serviceContainer.GetService(typeof(TraceManager));
				if (traceManager != null)
				{
					traceManager.Write(category, message);
				}
			}
		}

		public void Sort(PointsSortOrder order, string sortBy)
		{
			this.TraceWrite("ChartData", SR.TraceMessageBeginSortingDataPoints(this.Name));
			DataPointComparer comparer = new DataPointComparer(this, order, sortBy);
			this.Points.array.Sort(comparer);
			this.Invalidate(true, false);
			this.TraceWrite("ChartData", SR.TraceMessageEndSortingDataPoints(this.Name));
		}

		public void Sort(PointsSortOrder order)
		{
			this.Sort(order, "Y");
		}

		public void Sort(IComparer comparer)
		{
			this.TraceWrite("ChartData", SR.TraceMessageBeginSortingDataPoints(this.Name));
			this.Points.array.Sort(comparer);
			this.Invalidate(true, false);
			this.TraceWrite("ChartData", SR.TraceMessageEndSortingDataPoints(this.Name));
		}

		internal bool UnPrepareData(ISite controlSite)
		{
			bool result = false;
			if (RenkoChart.UnPrepareData(this, base.serviceContainer))
			{
				result = true;
			}
			if (ThreeLineBreakChart.UnPrepareData(this, base.serviceContainer))
			{
				result = true;
			}
			if (KagiChart.UnPrepareData(this, base.serviceContainer))
			{
				result = true;
			}
			if (PointAndFigureChart.UnPrepareData(this, base.serviceContainer))
			{
				result = true;
			}
			if (PieChart.UnPrepareData(this, base.serviceContainer))
			{
				result = true;
			}
			if (this.xValueIndexed)
			{
				this.xValueType = this.indexedXValueType;
			}
			bool reset = true;
			this.ResetAutoValues(reset);
			return result;
		}

		internal void ResetAutoValues()
		{
			this.ResetAutoValues(true);
		}

		internal void ResetAutoValues(bool reset)
		{
			if (this.IsAttributeSet("TempDesignData"))
			{
				this.DeleteAttribute("TempDesignData");
				bool flag = true;
				if (this.chart != null && !this.chart.IsDesignMode())
				{
					flag = false;
				}
				if (flag)
				{
					this.fakeDataPoints.Clear();
					foreach (DataPoint point in this.Points)
					{
						this.fakeDataPoints.Add(point);
					}
				}
				this.Points.Clear();
			}
			if (this.defaultChartArea)
			{
				this.defaultChartArea = false;
				this.ChartArea = "Default";
			}
			if (base.tempColorIsSet)
			{
				base.tempColorIsSet = false;
				base.Color = Color.Empty;
			}
			if (this.tempMarkerStyleIsSet)
			{
				this.tempMarkerStyleIsSet = false;
				base.MarkerStyle = MarkerStyle.None;
			}
			foreach (DataPoint point2 in this.points)
			{
				if (point2.tempColorIsSet)
				{
					point2.Color = Color.Empty;
				}
			}
			if (reset)
			{
				if (this.chart != null && this.chart.serializing)
				{
					return;
				}
				if (this.autoXValueType)
				{
					this.xValueType = ChartValueTypes.Auto;
					this.autoXValueType = false;
				}
				if (this.autoYValueType)
				{
					this.yValueType = ChartValueTypes.Auto;
					this.autoYValueType = false;
				}
			}
		}

		internal void PrepareData(ISite controlSite, bool applyPaletteColors)
		{
			if (this.IsVisible())
			{
				this.TraceWrite("ChartData", SR.TraceMessageBeginPreparingChartDataInSeries(this.Name));
				bool flag = false;
				if (this.ChartArea.Length > 0)
				{
					ChartImage chartImage = (ChartImage)base.serviceContainer.GetService(typeof(ChartImage));
					if (chartImage != null)
					{
						foreach (ChartArea chartArea2 in chartImage.ChartAreas)
						{
							if (chartArea2.Name == this.ChartArea)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag && this.ChartArea == "Default" && this.ChartArea.Length > 0 && chartImage.ChartAreas.Count > 0)
					{
						flag = true;
						this.ChartArea = chartImage.ChartAreas[0].Name;
						this.defaultChartArea = true;
					}
				}
				if (this.Points.Count > 0 && this.Points[0].YValues.Length < this.YValuesPerPoint)
				{
					foreach (DataPoint point in this.Points)
					{
						point.ResizeYValueArray(this.YValuesPerPoint);
					}
				}
				if (this.ChartArea.Length > 0 && !flag)
				{
					throw new InvalidOperationException(SR.ExceptionDataSeriesChartAreaInvalid(this.ChartArea, this.Name));
				}
				bool flag2 = false;
				if (this.Points.Count == 0 && flag)
				{
					if (controlSite != null && controlSite.DesignMode)
					{
						flag2 = true;
					}
					else if (this.IsAttributeSet("UseDummyData") && string.Compare(base["UseDummyData"], "True", StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					if (this.IsXValueDateTime() || this.xValueType == ChartValueTypes.String)
					{
						this.Points.DataBindXY(this.GetDummyData(this.xValueType), this.GetDummyData(this.yValueType));
					}
					else
					{
						double[] xValue = new double[6]
						{
							2.0,
							3.0,
							4.0,
							5.0,
							6.0,
							7.0
						};
						if (this.ChartType == SeriesChartType.Polar)
						{
							xValue = new double[6]
							{
								0.0,
								45.0,
								115.0,
								145.0,
								180.0,
								220.0
							};
						}
						this.Points.DataBindXY(xValue, this.GetDummyData(this.yValueType));
					}
					if (this.YValuesPerPoint > 1)
					{
						foreach (DataPoint point2 in this.Points)
						{
							for (int i = 1; i < this.YValuesPerPoint; i++)
							{
								point2.YValues[i] = point2.YValues[0];
							}
							if (this.YValuesPerPoint >= 2)
							{
								point2.YValues[1] = point2.YValues[0] / 2.0 - 1.0;
							}
							if (this.YValuesPerPoint >= 4)
							{
								point2.YValues[2] = point2.YValues[1] + (point2.YValues[0] - point2.YValues[1]) / 3.0;
								point2.YValues[3] = point2.YValues[2] + (point2.YValues[0] - point2.YValues[1]) / 3.0;
							}
							if (this.YValuesPerPoint >= 6)
							{
								point2.YValues[4] = point2.YValues[2] + (point2.YValues[3] - point2.YValues[2]) / 2.0;
								point2.YValues[5] = point2.YValues[2] + (point2.YValues[3] - point2.YValues[2]) / 3.0;
							}
						}
					}
					base["TempDesignData"] = "true";
				}
				if (this.xValueType == ChartValueTypes.Auto)
				{
					this.xValueType = ChartValueTypes.Double;
					this.autoXValueType = true;
				}
				if (this.yValueType == ChartValueTypes.Auto)
				{
					this.yValueType = ChartValueTypes.Double;
					this.autoYValueType = true;
				}
				this.indexedXValueType = this.xValueType;
				this.totalYvalue = double.NaN;
				if (this.chart == null)
				{
					this.chart = (Chart)base.serviceContainer.GetService(typeof(Chart));
				}
				if (this.chart != null && this.chart.chartPicture.SuppressExceptions)
				{
					Axis axis = this.chart.ChartAreas[this.ChartArea].GetAxis(AxisName.Y, this.YAxisType, this.YSubAxisName);
					Axis axis2 = this.chart.ChartAreas[this.ChartArea].GetAxis(AxisName.X, this.XAxisType, this.XSubAxisName);
					foreach (DataPoint point3 in this.Points)
					{
						if (axis2.Logarithmic && point3.XValue <= 0.0)
						{
							point3.XValue = 1.0;
							for (int j = 0; j < point3.YValues.Length; j++)
							{
								point3.YValues[j] = 0.0;
							}
							point3.Empty = true;
						}
						for (int k = 0; k < point3.YValues.Length; k++)
						{
							if (axis.Logarithmic && point3.YValues[k] <= 0.0)
							{
								point3.YValues[k] = 1.0;
								point3.Empty = true;
							}
							if (double.IsNaN(point3.YValues[k]))
							{
								point3.YValues[k] = 0.0;
								point3.Empty = true;
							}
						}
					}
				}
				ErrorBarChart.GetDataFromLinkedSeries(this, base.serviceContainer);
				ErrorBarChart.CalculateErrorAmount(this);
				BoxPlotChart.CalculateBoxPlotFromLinkedSeries(this, base.serviceContainer);
				RenkoChart.PrepareData(this, base.serviceContainer);
				ThreeLineBreakChart.PrepareData(this, base.serviceContainer);
				KagiChart.PrepareData(this, base.serviceContainer);
				PointAndFigureChart.PrepareData(this, base.serviceContainer);
				PieChart.PrepareData(this, base.serviceContainer);
				if (applyPaletteColors)
				{
					this.ApplyPaletteColors();
				}
				this.TraceWrite("ChartData", SR.TraceMessageEndPreparingChartDataInSeries(this.Name));
			}
		}

		internal void Invalidate(bool invalidateAreaOnly, bool invalidateLegend)
		{
		}
	}
}
