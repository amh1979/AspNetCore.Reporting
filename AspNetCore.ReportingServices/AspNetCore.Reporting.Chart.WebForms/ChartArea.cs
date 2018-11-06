using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeChartArea_ChartArea")]
	[DefaultProperty("Axes")]
	internal class ChartArea : ChartArea3D
	{
		internal class ChartTypeAndSeriesInfo
		{
			internal string ChartType = string.Empty;

			internal Series Series;

			public ChartTypeAndSeriesInfo()
			{
			}

			public ChartTypeAndSeriesInfo(string chartType)
			{
				this.ChartType = chartType;
			}

			public ChartTypeAndSeriesInfo(Series series)
			{
				this.ChartType = series.ChartTypeName;
				this.Series = series;
			}
		}

		internal Chart chart;

		private Axis[] axisArray = new Axis[4];

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

		private ChartDashStyle borderStyle;

		private int shadowOffset;

		private Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		private ElementPosition areaPosition = new ElementPosition();

		private ElementPosition innerPlotPosition = new ElementPosition();

		private Cursor cursorX = new Cursor();

		private Cursor cursorY = new Cursor();

		internal int IterationCounter;

		private bool equallySizedAxesFont;

		internal float axesAutoFontSize = 8f;

		private string alignWithChartArea = "NotSet";

		private AreaAlignOrientations alignOrientation = AreaAlignOrientations.Vertical;

		private AreaAlignTypes alignType = AreaAlignTypes.All;

		private int circularSectorNumber = -2147483648;

		private int circularUsePolygons = -2147483648;

		internal bool alignmentInProcess;

		internal RectangleF originalAreaPosition = RectangleF.Empty;

		internal RectangleF originalInnerPlotPosition = RectangleF.Empty;

		internal PointF circularCenter = PointF.Empty;

		private ArrayList circularAxisList;

		internal SmartLabels smartLabels = new SmartLabels();

		private bool visible = true;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[Browsable(false)]
		[SRCategory("CategoryAttributeCursor")]
		[Bindable(true)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeChartArea_CursorX")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Cursor CursorX
		{
			get
			{
				return this.cursorX;
			}
			set
			{
				this.cursorX = value;
				this.cursorX.Initialize(this, AxisName.X);
			}
		}

		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeCursor")]
		[Bindable(true)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeChartArea_CursorY")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Cursor CursorY
		{
			get
			{
				return this.cursorY;
			}
			set
			{
				this.cursorY = value;
				this.cursorY.Initialize(this, AxisName.Y);
			}
		}

		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeChartArea_Visible")]
		public virtual bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
				this.Invalidate(false);
			}
		}

		[TypeConverter(typeof(LegendAreaNameConverter))]
		[Bindable(true)]
		[DefaultValue("NotSet")]
		[SRDescription("DescriptionAttributeChartArea_AlignWithChartArea")]
		[SRCategory("CategoryAttributeAlignment")]
		public string AlignWithChartArea
		{
			get
			{
				return this.alignWithChartArea;
			}
			set
			{
				if (value.Length == 0)
				{
					this.alignWithChartArea = "NotSet";
				}
				else
				{
					this.alignWithChartArea = value;
				}
				this.Invalidate(false);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAlignment")]
		[DefaultValue(AreaAlignOrientations.Vertical)]
		[SRDescription("DescriptionAttributeChartArea_AlignOrientation")]
		public AreaAlignOrientations AlignOrientation
		{
			get
			{
				return this.alignOrientation;
			}
			set
			{
				this.alignOrientation = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAlignment")]
		[Bindable(true)]
		[DefaultValue(AreaAlignTypes.All)]
		[SRDescription("DescriptionAttributeChartArea_AlignType")]
		public AreaAlignTypes AlignType
		{
			get
			{
				return this.alignType;
			}
			set
			{
				this.alignType = value;
				this.Invalidate(false);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAxes")]
		[SRDescription("DescriptionAttributeChartArea_Axes")]
		[TypeConverter(typeof(AxesArrayConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public Axis[] Axes
		{
			get
			{
				return this.axisArray;
			}
			set
			{
				this.AxisX = value[0];
				this.AxisY = value[1];
				this.AxisX2 = value[2];
				this.AxisY2 = value[3];
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeChartArea_AxisY")]
		[SRCategory("CategoryAttributeAxis")]
		[Bindable(true)]
		[Browsable(false)]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		public Axis AxisY
		{
			get
			{
				return base.axisY;
			}
			set
			{
				base.axisY = value;
				base.axisY.Initialize(this, AxisName.Y);
				this.axisArray[1] = base.axisY;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAxis")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeChartArea_AxisX")]
		public Axis AxisX
		{
			get
			{
				return base.axisX;
			}
			set
			{
				base.axisX = value;
				base.axisX.Initialize(this, AxisName.X);
				this.axisArray[0] = base.axisX;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeAxis")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[Bindable(true)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeChartArea_AxisX2")]
		public Axis AxisX2
		{
			get
			{
				return base.axisX2;
			}
			set
			{
				base.axisX2 = value;
				base.axisX2.Initialize(this, AxisName.X2);
				this.axisArray[2] = base.axisX2;
				this.Invalidate(true);
			}
		}

		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[SRCategory("CategoryAttributeAxis")]
		[Bindable(true)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeChartArea_AxisY2")]
		public Axis AxisY2
		{
			get
			{
				return base.axisY2;
			}
			set
			{
				base.axisY2 = value;
				base.axisY2.Initialize(this, AxisName.Y2);
				this.axisArray[3] = base.axisY2;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeChartArea_Position")]
		[DefaultValue(typeof(ElementPosition), "Auto")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(ElementPositionConverter))]
		[SerializationVisibility(SerializationVisibility.Element)]
		public ElementPosition Position
		{
			get
			{
				if (base.Common != null && base.Common.Chart != null && base.Common.Chart.serializationStatus == SerializationStatus.Saving)
				{
					if (this.areaPosition.Auto)
					{
						return new ElementPosition();
					}
					ElementPosition elementPosition = new ElementPosition();
					elementPosition.Auto = true;
					elementPosition.SetPositionNoAuto(this.areaPosition.X, this.areaPosition.Y, this.areaPosition.Width, this.areaPosition.Height);
					return elementPosition;
				}
				return this.areaPosition;
			}
			set
			{
				this.areaPosition = value;
				this.areaPosition.common = base.Common;
				this.areaPosition.resetAreaAutoPosition = true;
				this.Invalidate(false);
			}
		}

		[SerializationVisibility(SerializationVisibility.Element)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChartArea_InnerPlotPosition")]
		[DefaultValue(typeof(ElementPosition), "Auto")]
		[NotifyParentProperty(true)]
		[TypeConverter(typeof(ElementPositionConverter))]
		[SRCategory("CategoryAttributeAppearance")]
		public ElementPosition InnerPlotPosition
		{
			get
			{
				if (base.Common != null && base.Common.Chart != null && base.Common.Chart.serializationStatus == SerializationStatus.Saving)
				{
					if (this.innerPlotPosition.Auto)
					{
						return new ElementPosition();
					}
					ElementPosition elementPosition = new ElementPosition();
					elementPosition.Auto = true;
					elementPosition.SetPositionNoAuto(this.innerPlotPosition.X, this.innerPlotPosition.Y, this.innerPlotPosition.Width, this.innerPlotPosition.Height);
					return elementPosition;
				}
				return this.innerPlotPosition;
			}
			set
			{
				this.innerPlotPosition = value;
				this.innerPlotPosition.common = base.Common;
				this.Invalidate(true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeChartArea_BackColor")]
		[Browsable(false)]
		public Color BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				return this.backHatchStyle;
			}
			set
			{
				this.backHatchStyle = value;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeBackImage15")]
		[NotifyParentProperty(true)]
		public string BackImage
		{
			get
			{
				return this.backImage;
			}
			set
			{
				this.backImage = value;
				this.Invalidate(true);
			}
		}

		[DefaultValue(ChartImageWrapMode.Tile)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeChartArea_BackImageMode")]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				return this.backImageMode;
			}
			set
			{
				this.backImageMode = value;
				this.Invalidate(true);
			}
		}

		[DefaultValue(typeof(Color), "")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageTransparentColor")]
		public Color BackImageTransparentColor
		{
			get
			{
				return this.backImageTranspColor;
			}
			set
			{
				this.backImageTranspColor = value;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartImageAlign.TopLeft)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackImageAlign")]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				return this.backImageAlign;
			}
			set
			{
				this.backImageAlign = value;
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeChartArea_BackGradientType")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		public GradientType BackGradientType
		{
			get
			{
				return this.backGradientType;
			}
			set
			{
				this.backGradientType = value;
				this.Invalidate(true);
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeChartArea_BackGradientEndColor")]
		public Color BackGradientEndColor
		{
			get
			{
				return this.backGradientEndColor;
			}
			set
			{
				this.backGradientEndColor = value;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "128,0,0,0")]
		[SRDescription("DescriptionAttributeChartArea_ShadowColor")]
		[NotifyParentProperty(true)]
		public Color ShadowColor
		{
			get
			{
				return this.shadowColor;
			}
			set
			{
				this.shadowColor = value;
				this.Invalidate(true);
			}
		}

		[DefaultValue(0)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChartArea_ShadowOffset")]
		[NotifyParentProperty(true)]
		public int ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				this.shadowOffset = value;
				this.Invalidate(true);
			}
		}

		[Browsable(false)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeChartArea_BorderColor")]
		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.Invalidate(true);
			}
		}

		[Browsable(false)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeChartArea_BorderWidth")]
		public int BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionBorderWidthIsNegative);
				}
				this.borderWidth = value;
				this.Invalidate(true);
			}
		}

		[NotifyParentProperty(true)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.NotSet)]
		[SRDescription("DescriptionAttributeChartArea_BorderStyle")]
		public ChartDashStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeChartArea_Name")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		public string Name
		{
			get
			{
				return base.name;
			}
			set
			{
				if (!(base.name != value))
				{
					return;
				}
				if (base.Common != null)
				{
					foreach (ChartArea item in base.Common.chartAreaCollection)
					{
						if (item.Name == value)
						{
							throw new ArgumentException(SR.ExceptionChartAreaAlreadyExistsShort);
						}
					}
				}
				if (value != null && value.Length != 0)
				{
					base.name = value;
					return;
				}
				throw new ArgumentException(SR.ExceptionChartAreaNameIsEmpty);
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChartArea_EquallySizedAxesFont")]
		[SRCategory("CategoryAttributeAppearance")]
		public bool EquallySizedAxesFont
		{
			get
			{
				return this.equallySizedAxesFont;
			}
			set
			{
				this.equallySizedAxesFont = value;
				this.Invalidate(true);
			}
		}

		public CategoryNodeCollection CategoryNodes
		{
			get;
			set;
		}

		internal bool CircularUsePolygons
		{
			get
			{
				if (this.circularUsePolygons == -2147483648)
				{
					this.circularUsePolygons = 0;
					foreach (Series item in base.Common.DataManager.Series)
					{
						if (item.ChartArea == this.Name && item.IsVisible() && item.IsAttributeSet("AreaDrawingStyle"))
						{
							if (string.Compare(((DataPointAttributes)item)["AreaDrawingStyle"], "Polygon", StringComparison.OrdinalIgnoreCase) == 0)
							{
								this.circularUsePolygons = 1;
								break;
							}
							if (string.Compare(((DataPointAttributes)item)["AreaDrawingStyle"], "Circle", StringComparison.OrdinalIgnoreCase) == 0)
							{
								this.circularUsePolygons = 0;
								break;
							}
							throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(((DataPointAttributes)item)["AreaDrawingStyle"], "AreaDrawingStyle"));
						}
					}
				}
				return this.circularUsePolygons == 1;
			}
		}

		internal int CircularSectorsNumber
		{
			get
			{
				if (this.circularSectorNumber == -2147483648)
				{
					this.circularSectorNumber = this.GetCircularSectorNumber();
				}
				return this.circularSectorNumber;
			}
		}

		protected bool ShouldSerializeAxes()
		{
			return false;
		}

		public ChartArea()
		{
			this.Initialize();
		}

		internal void Invalidate(bool invalidateAreaOnly)
		{
		}

		internal void Restore3DAnglesAndReverseMode()
		{
			if (base.Area3DStyle.Enable3D && !base.chartAreaIsCurcular)
			{
				this.AxisX.Reverse = base.oldReverseX;
				this.AxisX2.Reverse = base.oldReverseX;
				this.AxisY.Reverse = base.oldReverseY;
				this.AxisY2.Reverse = base.oldReverseY;
				base.Area3DStyle.YAngle = base.oldYAngle;
			}
		}

		internal void Set3DAnglesAndReverseMode()
		{
			base.reverseSeriesOrder = false;
			if (base.Area3DStyle.Enable3D)
			{
				this.AxisX2.Reverse = this.AxisX.Reverse;
				this.AxisY2.Reverse = this.AxisY.Reverse;
				base.oldReverseX = this.AxisX.Reverse;
				base.oldReverseY = this.AxisY.Reverse;
				base.oldYAngle = base.Area3DStyle.YAngle;
				if (base.Area3DStyle.YAngle <= 90 && base.Area3DStyle.YAngle >= -90)
				{
					return;
				}
				base.reverseSeriesOrder = true;
				if (!base.switchValueAxes)
				{
					this.AxisX.Reverse = !this.AxisX.Reverse;
					this.AxisX2.Reverse = !this.AxisX2.Reverse;
				}
				else
				{
					this.AxisY.Reverse = !this.AxisY.Reverse;
					this.AxisY2.Reverse = !this.AxisY2.Reverse;
				}
				if (base.Area3DStyle.YAngle > 90)
				{
					base.Area3DStyle.YAngle = base.Area3DStyle.YAngle - 90 - 90;
				}
				else if (base.Area3DStyle.YAngle < -90)
				{
					base.Area3DStyle.YAngle = base.Area3DStyle.YAngle + 90 + 90;
				}
			}
		}

		internal void SetTempValues()
		{
			if (!this.Position.Auto)
			{
				this.originalAreaPosition = this.Position.ToRectangleF();
			}
			if (!this.InnerPlotPosition.Auto)
			{
				this.originalInnerPlotPosition = this.InnerPlotPosition.ToRectangleF();
			}
			this.circularSectorNumber = -2147483648;
			this.circularUsePolygons = -2147483648;
			this.circularAxisList = null;
			base.axisX.StoreAxisValues();
			base.axisX2.StoreAxisValues();
			base.axisY.StoreAxisValues();
			base.axisY2.StoreAxisValues();
		}

		internal void GetTempValues()
		{
			base.axisX.ResetAxisValues();
			base.axisX2.ResetAxisValues();
			base.axisY.ResetAxisValues();
			base.axisY2.ResetAxisValues();
			if (!this.originalAreaPosition.IsEmpty)
			{
				this.Position.SetPositionNoAuto(this.originalAreaPosition.X, this.originalAreaPosition.Y, this.originalAreaPosition.Width, this.originalAreaPosition.Height);
				this.originalAreaPosition = RectangleF.Empty;
			}
			if (!this.originalInnerPlotPosition.IsEmpty)
			{
				this.InnerPlotPosition.SetPositionNoAuto(this.originalInnerPlotPosition.X, this.originalInnerPlotPosition.Y, this.originalInnerPlotPosition.Width, this.originalInnerPlotPosition.Height);
				this.originalInnerPlotPosition = RectangleF.Empty;
			}
		}

		internal void Initialize()
		{
			base.axisY = new Axis();
			base.axisX = new Axis();
			base.axisX2 = new Axis();
			base.axisY2 = new Axis();
			base.axisX.Initialize(this, AxisName.X);
			base.axisY.Initialize(this, AxisName.Y);
			base.axisX2.Initialize(this, AxisName.X2);
			base.axisY2.Initialize(this, AxisName.Y2);
			this.axisArray[0] = base.axisX;
			this.axisArray[1] = base.axisY;
			this.axisArray[2] = base.axisX2;
			this.axisArray[3] = base.axisY2;
			this.areaPosition.resetAreaAutoPosition = true;
			if (base.PlotAreaPosition == null)
			{
				base.PlotAreaPosition = new ElementPosition();
			}
			this.cursorX.Initialize(this, AxisName.X);
			this.cursorY.Initialize(this, AxisName.Y);
		}

		internal void ResetMinMaxFromData()
		{
			this.axisArray[0].refreshMinMaxFromData = true;
			this.axisArray[1].refreshMinMaxFromData = true;
			this.axisArray[2].refreshMinMaxFromData = true;
			this.axisArray[3].refreshMinMaxFromData = true;
		}

		public void Recalculate()
		{
			this.ResetMinMaxFromData();
			this.axisArray[0].ReCalc(base.PlotAreaPosition);
			this.axisArray[1].ReCalc(base.PlotAreaPosition);
			this.axisArray[2].ReCalc(base.PlotAreaPosition);
			this.axisArray[3].ReCalc(base.PlotAreaPosition);
			base.SetData();
		}

		internal void ReCalcInternal()
		{
			this.axisArray[0].ReCalc(base.PlotAreaPosition);
			this.axisArray[1].ReCalc(base.PlotAreaPosition);
			this.axisArray[2].ReCalc(base.PlotAreaPosition);
			this.axisArray[3].ReCalc(base.PlotAreaPosition);
			base.SetData();
		}

		internal void ResetAutoValues()
		{
			this.axisArray[0].ResetAutoValues();
			this.axisArray[1].ResetAutoValues();
			this.axisArray[2].ResetAutoValues();
			this.axisArray[3].ResetAutoValues();
		}

		internal RectangleF GetBackgroundPosition(bool withScrollBars)
		{
			RectangleF result = base.PlotAreaPosition.ToRectangleF();
			if (!base.requireAxes)
			{
				result = this.Position.ToRectangleF();
			}
			if (!withScrollBars)
			{
				return result;
			}
			RectangleF result2 = new RectangleF(result.Location, result.Size);
			if (base.requireAxes)
			{
				Axis[] axes = this.Axes;
				foreach (Axis axis in axes)
				{
					if (axis.ScrollBar.IsVisible() && axis.ScrollBar.PositionInside)
					{
						if (axis.AxisPosition == AxisPosition.Bottom)
						{
							result2.Height += (float)axis.ScrollBar.GetScrollBarRelativeSize();
						}
						else if (axis.AxisPosition == AxisPosition.Top)
						{
							result2.Y -= (float)axis.ScrollBar.GetScrollBarRelativeSize();
							result2.Height += (float)axis.ScrollBar.GetScrollBarRelativeSize();
						}
						else if (axis.AxisPosition == AxisPosition.Left)
						{
							result2.X -= (float)axis.ScrollBar.GetScrollBarRelativeSize();
							result2.Width += (float)axis.ScrollBar.GetScrollBarRelativeSize();
						}
						else if (axis.AxisPosition == AxisPosition.Left)
						{
							result2.Width += (float)axis.ScrollBar.GetScrollBarRelativeSize();
						}
					}
				}
			}
			return result2;
		}

		internal void SetCommon(CommonElements common)
		{
			base.Common = common;
			base.axisX.Common = common;
			base.axisX2.Common = common;
			base.axisY.Common = common;
			base.axisY2.Common = common;
			this.areaPosition.common = common;
			this.innerPlotPosition.common = common;
			if (common != null)
			{
				this.chart = common.Chart;
			}
			base.axisX.Initialize(this, AxisName.X);
			base.axisY.Initialize(this, AxisName.Y);
			base.axisX2.Initialize(this, AxisName.X2);
			base.axisY2.Initialize(this, AxisName.Y2);
			this.cursorX.Initialize(this, AxisName.X);
			this.cursorY.Initialize(this, AxisName.Y);
		}

		internal void Resize(ChartGraphics chartGraph)
		{
			RectangleF rectangleF = this.Position.ToRectangleF();
			if (!this.InnerPlotPosition.Auto)
			{
				rectangleF.X += (float)(this.Position.Width / 100.0 * this.InnerPlotPosition.X);
				rectangleF.Y += (float)(this.Position.Height / 100.0 * this.InnerPlotPosition.Y);
				rectangleF.Width = (float)(this.Position.Width / 100.0 * this.InnerPlotPosition.Width);
				rectangleF.Height = (float)(this.Position.Height / 100.0 * this.InnerPlotPosition.Height);
			}
			int num = 0;
			int num2 = 0;
			Axis[] axes = this.Axes;
			foreach (Axis axis in axes)
			{
				if (axis.enabled)
				{
					if (axis.AxisPosition == AxisPosition.Bottom)
					{
						num2++;
					}
					else if (axis.AxisPosition == AxisPosition.Top)
					{
						num2++;
					}
					else if (axis.AxisPosition == AxisPosition.Left)
					{
						num++;
					}
					else if (axis.AxisPosition == AxisPosition.Right)
					{
						num++;
					}
				}
			}
			if (num2 <= 0)
			{
				num2 = 1;
			}
			if (num <= 0)
			{
				num = 1;
			}
			Axis[] array = base.switchValueAxes ? new Axis[4]
			{
				this.AxisX,
				this.AxisX2,
				this.AxisY,
				this.AxisY2
			} : new Axis[4]
			{
				this.AxisY,
				this.AxisY2,
				this.AxisX,
				this.AxisX2
			};
			if (this.EquallySizedAxesFont)
			{
				this.axesAutoFontSize = 20f;
				Axis[] array2 = array;
				foreach (Axis axis2 in array2)
				{
					if (axis2.enabled)
					{
						if (axis2.AxisPosition == AxisPosition.Bottom || axis2.AxisPosition == AxisPosition.Top)
						{
							axis2.Resize(chartGraph, base.PlotAreaPosition, rectangleF, (float)num2, this.InnerPlotPosition.Auto);
						}
						else
						{
							axis2.Resize(chartGraph, base.PlotAreaPosition, rectangleF, (float)num, this.InnerPlotPosition.Auto);
						}
						if (axis2.LabelsAutoFit && axis2.autoLabelFont != null)
						{
							this.axesAutoFontSize = Math.Min(this.axesAutoFontSize, axis2.autoLabelFont.Size);
						}
					}
				}
			}
			RectangleF empty = RectangleF.Empty;
			Axis[] array3 = array;
			foreach (Axis axis3 in array3)
			{
				if (!axis3.enabled)
				{
					if (this.InnerPlotPosition.Auto && base.Area3DStyle.Enable3D && !base.chartAreaIsCurcular)
					{
						SizeF relativeSize = chartGraph.GetRelativeSize(new SizeF((float)base.Area3DStyle.WallWidth, (float)base.Area3DStyle.WallWidth));
						if (axis3.AxisPosition == AxisPosition.Bottom)
						{
							rectangleF.Height -= relativeSize.Height;
						}
						else if (axis3.AxisPosition == AxisPosition.Top)
						{
							rectangleF.Y += relativeSize.Height;
							rectangleF.Height -= relativeSize.Height;
						}
						else if (axis3.AxisPosition == AxisPosition.Right)
						{
							rectangleF.Width -= relativeSize.Width;
						}
						else if (axis3.AxisPosition == AxisPosition.Left)
						{
							rectangleF.X += relativeSize.Width;
							rectangleF.Width -= relativeSize.Width;
						}
					}
				}
				else
				{
					if (axis3.AxisPosition == AxisPosition.Bottom || axis3.AxisPosition == AxisPosition.Top)
					{
						axis3.Resize(chartGraph, base.PlotAreaPosition, rectangleF, (float)num2, this.InnerPlotPosition.Auto);
					}
					else
					{
						axis3.Resize(chartGraph, base.PlotAreaPosition, rectangleF, (float)num, this.InnerPlotPosition.Auto);
					}
					this.PreventTopBottomAxesLabelsOverlapping(axis3);
					float num3 = (float)axis3.GetAxisPosition();
					if (axis3.AxisPosition == AxisPosition.Bottom)
					{
						if (!axis3.IsMarksNextToAxis())
						{
							num3 = rectangleF.Bottom;
						}
						num3 = rectangleF.Bottom - num3;
					}
					else if (axis3.AxisPosition == AxisPosition.Top)
					{
						if (!axis3.IsMarksNextToAxis())
						{
							num3 = rectangleF.Y;
						}
						num3 -= rectangleF.Top;
					}
					else if (axis3.AxisPosition == AxisPosition.Right)
					{
						if (!axis3.IsMarksNextToAxis())
						{
							num3 = rectangleF.Right;
						}
						num3 = rectangleF.Right - num3;
					}
					else if (axis3.AxisPosition == AxisPosition.Left)
					{
						if (!axis3.IsMarksNextToAxis())
						{
							num3 = rectangleF.X;
						}
						num3 -= rectangleF.Left;
					}
					float num4 = axis3.markSize + axis3.labelSize;
					num4 -= num3;
					if (num4 < 0.0)
					{
						num4 = 0f;
					}
					num4 += axis3.titleSize + axis3.scrollBarSize;
					if (base.chartAreaIsCurcular && (axis3.AxisPosition == AxisPosition.Top || axis3.AxisPosition == AxisPosition.Bottom))
					{
						num4 = axis3.titleSize + axis3.markSize + axis3.scrollBarSize;
					}
					if (this.InnerPlotPosition.Auto)
					{
						if (axis3.AxisPosition == AxisPosition.Bottom)
						{
							rectangleF.Height -= num4;
						}
						else if (axis3.AxisPosition == AxisPosition.Top)
						{
							rectangleF.Y += num4;
							rectangleF.Height -= num4;
						}
						else if (axis3.AxisPosition == AxisPosition.Left)
						{
							rectangleF.X += num4;
							rectangleF.Width -= num4;
						}
						else if (axis3.AxisPosition == AxisPosition.Right)
						{
							rectangleF.Width -= num4;
						}
						if (true)
						{
							if (axis3.AxisPosition == AxisPosition.Bottom || axis3.AxisPosition == AxisPosition.Top)
							{
								if (axis3.labelNearOffset != 0.0 && axis3.labelNearOffset < this.Position.X)
								{
									float num5 = this.Position.X - axis3.labelNearOffset;
									if (Math.Abs(num5) > rectangleF.Width * 0.30000001192092896)
									{
										num5 = (float)(rectangleF.Width * 0.30000001192092896);
									}
									empty.X = Math.Max(num5, empty.X);
								}
								if (axis3.labelFarOffset > this.Position.Right())
								{
									if (axis3.labelFarOffset - this.Position.Right() < rectangleF.Width * 0.30000001192092896)
									{
										empty.Width = Math.Max(axis3.labelFarOffset - this.Position.Right(), empty.Width);
									}
									else
									{
										empty.Width = Math.Max((float)(rectangleF.Width * 0.30000001192092896), empty.Width);
									}
								}
							}
							else
							{
								if (axis3.labelNearOffset != 0.0 && axis3.labelNearOffset < this.Position.Y)
								{
									float num6 = this.Position.Y - axis3.labelNearOffset;
									if (Math.Abs(num6) > rectangleF.Height * 0.30000001192092896)
									{
										num6 = (float)(rectangleF.Height * 0.30000001192092896);
									}
									empty.Y = Math.Max(num6, empty.Y);
								}
								if (axis3.labelFarOffset > this.Position.Bottom())
								{
									if (axis3.labelFarOffset - this.Position.Bottom() < rectangleF.Height * 0.30000001192092896)
									{
										empty.Height = Math.Max(axis3.labelFarOffset - this.Position.Bottom(), empty.Height);
									}
									else
									{
										empty.Height = Math.Max((float)(rectangleF.Height * 0.30000001192092896), empty.Height);
									}
								}
							}
						}
					}
				}
			}
			if (!base.chartAreaIsCurcular)
			{
				try
				{
					if (empty.Y > 0.0 && empty.Y > rectangleF.Y - this.Position.Y)
					{
						float num7 = rectangleF.Y - this.Position.Y - empty.Y;
						rectangleF.Y -= num7;
						rectangleF.Height += num7;
					}
					if (empty.X > 0.0 && empty.X > rectangleF.X - this.Position.X)
					{
						float num8 = rectangleF.X - this.Position.X - empty.X;
						rectangleF.X -= num8;
						rectangleF.Width += num8;
					}
					if (empty.Height > 0.0 && empty.Height > this.Position.Bottom() - rectangleF.Bottom)
					{
						rectangleF.Height += this.Position.Bottom() - rectangleF.Bottom - empty.Height;
					}
					if (empty.Width > 0.0 && empty.Width > this.Position.Right() - rectangleF.Right)
					{
						rectangleF.Width += this.Position.Right() - rectangleF.Right - empty.Width;
					}
				}
				catch (Exception)
				{
				}
			}
			if (base.chartAreaIsCurcular)
			{
				float num9 = Math.Max(this.AxisY.titleSize, this.AxisY2.titleSize);
				if (num9 > 0.0)
				{
					rectangleF.X += num9;
					rectangleF.Width -= (float)(2.0 * num9);
				}
				float num10 = Math.Max(this.AxisX.titleSize, this.AxisX2.titleSize);
				if (num10 > 0.0)
				{
					rectangleF.Y += num10;
					rectangleF.Height -= (float)(2.0 * num10);
				}
				RectangleF absoluteRectangle = chartGraph.GetAbsoluteRectangle(rectangleF);
				if (absoluteRectangle.Width > absoluteRectangle.Height)
				{
					absoluteRectangle.X += (float)((absoluteRectangle.Width - absoluteRectangle.Height) / 2.0);
					absoluteRectangle.Width = absoluteRectangle.Height;
				}
				else
				{
					absoluteRectangle.Y += (float)((absoluteRectangle.Height - absoluteRectangle.Width) / 2.0);
					absoluteRectangle.Height = absoluteRectangle.Width;
				}
				rectangleF = chartGraph.GetRelativeRectangle(absoluteRectangle);
				this.circularCenter = new PointF((float)(rectangleF.X + rectangleF.Width / 2.0), (float)(rectangleF.Y + rectangleF.Height / 2.0));
				this.FitCircularLabels(chartGraph, base.PlotAreaPosition, ref rectangleF, num9, num10);
			}
			if (rectangleF.Width < 0.0)
			{
				rectangleF.Width = 0f;
			}
			if (rectangleF.Height < 0.0)
			{
				rectangleF.Height = 0f;
			}
			base.PlotAreaPosition.FromRectangleF(rectangleF);
			this.InnerPlotPosition.SetPositionNoAuto((float)Math.Round((rectangleF.X - this.Position.X) / (this.Position.Width / 100.0), 5), (float)Math.Round((rectangleF.Y - this.Position.Y) / (this.Position.Height / 100.0), 5), (float)Math.Round(rectangleF.Width / (this.Position.Width / 100.0), 5), (float)Math.Round(rectangleF.Height / (this.Position.Height / 100.0), 5));
			this.AxisY2.AdjustLabelFontAtSecondPass(chartGraph, this.InnerPlotPosition.Auto);
			this.AxisY.AdjustLabelFontAtSecondPass(chartGraph, this.InnerPlotPosition.Auto);
			if (this.InnerPlotPosition.Auto)
			{
				this.AxisX2.AdjustLabelFontAtSecondPass(chartGraph, this.InnerPlotPosition.Auto);
				this.AxisX.AdjustLabelFontAtSecondPass(chartGraph, this.InnerPlotPosition.Auto);
			}
		}

		private Axis FindAxis(AxisPosition axisPosition)
		{
			Axis[] axes = this.Axes;
			foreach (Axis axis in axes)
			{
				if (axis.AxisPosition == axisPosition)
				{
					return axis;
				}
			}
			return null;
		}

		private void PreventTopBottomAxesLabelsOverlapping(Axis axis)
		{
			if (axis.IsAxisOnAreaEdge())
			{
				if (axis.AxisPosition == AxisPosition.Bottom)
				{
					float num = (float)axis.GetAxisPosition();
					if (!axis.IsMarksNextToAxis())
					{
						num = axis.PlotAreaPosition.Bottom();
					}
					if (!(Math.Round((double)num, 2) < Math.Round((double)axis.PlotAreaPosition.Bottom(), 2)))
					{
						Axis axis2 = this.FindAxis(AxisPosition.Left);
						if (axis2 != null && axis2.enabled && axis2.labelFarOffset != 0.0 && axis2.labelFarOffset > num && axis.labelNearOffset != 0.0 && axis.labelNearOffset < base.PlotAreaPosition.X)
						{
							float num2 = (float)((axis2.labelFarOffset - num) * 0.75);
							if (num2 > axis.markSize)
							{
								axis.markSize += num2 - axis.markSize;
							}
						}
						Axis axis3 = this.FindAxis(AxisPosition.Right);
						if (axis3 != null && axis3.enabled && axis3.labelFarOffset != 0.0 && axis3.labelFarOffset > num && axis.labelFarOffset != 0.0 && axis.labelFarOffset > base.PlotAreaPosition.Right())
						{
							float num3 = (float)((axis3.labelFarOffset - num) * 0.75);
							if (num3 > axis.markSize)
							{
								axis.markSize += num3 - axis.markSize;
							}
						}
					}
				}
				else if (axis.AxisPosition == AxisPosition.Top)
				{
					float num4 = (float)axis.GetAxisPosition();
					if (!axis.IsMarksNextToAxis())
					{
						num4 = axis.PlotAreaPosition.Y;
					}
					if (!(Math.Round((double)num4, 2) < Math.Round((double)axis.PlotAreaPosition.Y, 2)))
					{
						Axis axis4 = this.FindAxis(AxisPosition.Left);
						if (axis4 != null && axis4.enabled && axis4.labelNearOffset != 0.0 && axis4.labelNearOffset < num4 && axis.labelNearOffset != 0.0 && axis.labelNearOffset < base.PlotAreaPosition.X)
						{
							float num5 = (float)((num4 - axis4.labelNearOffset) * 0.75);
							if (num5 > axis.markSize)
							{
								axis.markSize += num5 - axis.markSize;
							}
						}
						Axis axis5 = this.FindAxis(AxisPosition.Right);
						if (axis5 != null && axis5.enabled && axis5.labelNearOffset != 0.0 && axis5.labelNearOffset < num4 && axis.labelFarOffset != 0.0 && axis.labelFarOffset > base.PlotAreaPosition.Right())
						{
							float num6 = (float)((num4 - axis5.labelNearOffset) * 0.75);
							if (num6 > axis.markSize)
							{
								axis.markSize += num6 - axis.markSize;
							}
						}
					}
				}
			}
		}

		private void PaintAreaBack(ChartGraphics graph, RectangleF position, bool borderOnly)
		{
			if (!borderOnly)
			{
				if (!base.Area3DStyle.Enable3D || !base.requireAxes || base.chartAreaIsCurcular)
				{
					graph.FillRectangleRel(position, this.BackColor, this.BackHatchStyle, this.BackImage, this.BackImageMode, this.BackImageTransparentColor, this.BackImageAlign, this.BackGradientType, this.BackGradientEndColor, base.requireAxes ? Color.Empty : this.BorderColor, (!base.requireAxes) ? this.BorderWidth : 0, this.BorderStyle, this.ShadowColor, this.ShadowOffset, PenAlignment.Outset, base.chartAreaIsCurcular, (base.chartAreaIsCurcular && this.CircularUsePolygons) ? this.CircularSectorsNumber : 0, base.Area3DStyle.Enable3D);
				}
				else
				{
					base.DrawArea3DScene(graph, position);
				}
			}
			else
			{
				if (base.Area3DStyle.Enable3D && base.requireAxes && !base.chartAreaIsCurcular)
				{
					return;
				}
				if (this.BorderColor != Color.Empty && this.BorderWidth > 0)
				{
					graph.FillRectangleRel(position, Color.Transparent, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, this.BorderColor, this.BorderWidth, this.BorderStyle, Color.Empty, 0, PenAlignment.Outset, base.chartAreaIsCurcular, (base.chartAreaIsCurcular && this.CircularUsePolygons) ? this.CircularSectorsNumber : 0, base.Area3DStyle.Enable3D);
				}
			}
		}

		internal void Paint(ChartGraphics graph)
		{
			if (base.PlotAreaPosition.Width == 0.0 && base.PlotAreaPosition.Height == 0.0 && !this.InnerPlotPosition.Auto && !this.Position.Auto)
			{
				RectangleF rect = this.Position.ToRectangleF();
				if (!this.InnerPlotPosition.Auto)
				{
					rect.X += (float)(this.Position.Width / 100.0 * this.InnerPlotPosition.X);
					rect.Y += (float)(this.Position.Height / 100.0 * this.InnerPlotPosition.Y);
					rect.Width = (float)(this.Position.Width / 100.0 * this.InnerPlotPosition.Width);
					rect.Height = (float)(this.Position.Height / 100.0 * this.InnerPlotPosition.Height);
				}
				base.PlotAreaPosition.FromRectangleF(rect);
			}
			RectangleF backgroundPosition = this.GetBackgroundPosition(true);
			RectangleF backgroundPosition2 = this.GetBackgroundPosition(false);
			if (base.Common.ProcessModeRegions)
			{
				base.Common.HotRegionsList.AddHotRegion(backgroundPosition2, this, ChartElementType.PlottingArea, true);
			}
			graph.StartAnimation();
			this.PaintAreaBack(graph, backgroundPosition, false);
			graph.StopAnimation();
			base.Common.EventsManager.OnBackPaint(this, new ChartPaintEventArgs(graph, base.Common, base.PlotAreaPosition));
			if (!base.requireAxes && base.ChartTypes.Count != 0)
			{
				for (int i = 0; i < base.ChartTypes.Count; i++)
				{
					IChartType chartType = base.Common.ChartTypeRegistry.GetChartType((string)base.ChartTypes[i]);
					if (!chartType.RequireAxes)
					{
						chartType.Paint(graph, base.Common, this, null);
						break;
					}
				}
				base.Common.EventsManager.OnPaint(this, new ChartPaintEventArgs(graph, base.Common, base.PlotAreaPosition));
			}
			else
			{
				this.smartLabels.Reset(base.Common, this);
				Axis[] array = this.axisArray;
				foreach (Axis axis in array)
				{
					axis.optimizedGetPosition = true;
					axis.paintViewMax = axis.GetViewMaximum();
					axis.paintViewMin = axis.GetViewMinimum();
					axis.paintRange = axis.paintViewMax - axis.paintViewMin;
					axis.paintAreaPosition = base.PlotAreaPosition.ToRectangleF();
					if (axis.chartArea != null && axis.chartArea.chartAreaIsCurcular)
					{
						axis.paintAreaPosition.Width /= 2f;
						axis.paintAreaPosition.Height /= 2f;
					}
					axis.paintAreaPositionBottom = (double)(axis.paintAreaPosition.Y + axis.paintAreaPosition.Height);
					axis.paintAreaPositionRight = (double)(axis.paintAreaPosition.X + axis.paintAreaPosition.Width);
					if (axis.AxisPosition == AxisPosition.Top || axis.AxisPosition == AxisPosition.Bottom)
					{
						axis.paintChartAreaSize = (double)axis.paintAreaPosition.Width;
					}
					else
					{
						axis.paintChartAreaSize = (double)axis.paintAreaPosition.Height;
					}
					axis.valueMultiplier = 0.0;
					if (axis.paintRange != 0.0)
					{
						axis.valueMultiplier = axis.paintChartAreaSize / axis.paintRange;
					}
				}
				bool flag = false;
				Axis[] array2 = new Axis[4]
				{
					base.axisY,
					base.axisY2,
					base.axisX,
					base.axisX2
				};
				Axis[] array3 = array2;
				foreach (Axis axis2 in array3)
				{
					if (axis2.ScaleSegments.Count <= 0)
					{
						axis2.PaintStrips(graph, false);
					}
					else
					{
						foreach (AxisScaleSegment scaleSegment in axis2.ScaleSegments)
						{
							scaleSegment.SetTempAxisScaleAndInterval();
							axis2.PaintStrips(graph, false);
							scaleSegment.RestoreAxisScaleAndInterval();
						}
					}
				}
				array2 = new Axis[4]
				{
					base.axisY,
					base.axisX2,
					base.axisY2,
					base.axisX
				};
				Axis[] array4 = array2;
				foreach (Axis axis3 in array4)
				{
					if (axis3.ScaleSegments.Count <= 0)
					{
						axis3.PaintGrids(graph);
					}
					else
					{
						foreach (AxisScaleSegment scaleSegment2 in axis3.ScaleSegments)
						{
							scaleSegment2.SetTempAxisScaleAndInterval();
							axis3.PaintGrids(graph);
							scaleSegment2.RestoreAxisScaleAndInterval();
						}
					}
				}
				Axis[] array5 = array2;
				foreach (Axis axis4 in array5)
				{
					if (axis4.ScaleSegments.Count <= 0)
					{
						axis4.PaintStrips(graph, true);
					}
					else
					{
						foreach (AxisScaleSegment scaleSegment3 in axis4.ScaleSegments)
						{
							scaleSegment3.SetTempAxisScaleAndInterval();
							axis4.PaintStrips(graph, true);
							scaleSegment3.RestoreAxisScaleAndInterval();
						}
					}
				}
				if (base.Area3DStyle.Enable3D && !base.chartAreaIsCurcular)
				{
					Axis[] array6 = array2;
					foreach (Axis axis5 in array6)
					{
						if (axis5.ScaleSegments.Count <= 0)
						{
							axis5.PrePaint(graph);
						}
						else
						{
							foreach (AxisScaleSegment scaleSegment4 in axis5.ScaleSegments)
							{
								scaleSegment4.SetTempAxisScaleAndInterval();
								axis5.PrePaint(graph);
								scaleSegment4.RestoreAxisScaleAndInterval();
							}
						}
					}
				}
				bool flag2 = false;
				if (base.Area3DStyle.Enable3D || !this.IsBorderOnTopSeries())
				{
					flag2 = true;
					this.PaintAreaBack(graph, backgroundPosition2, true);
				}
				if (!base.Area3DStyle.Enable3D || base.chartAreaIsCurcular)
				{
					ArrayList chartTypesAndSeriesToDraw = this.GetChartTypesAndSeriesToDraw();
					foreach (ChartTypeAndSeriesInfo item in chartTypesAndSeriesToDraw)
					{
						this.IterationCounter = 0;
						IChartType chartType2 = base.Common.ChartTypeRegistry.GetChartType(item.ChartType);
						chartType2.Paint(graph, base.Common, this, item.Series);
					}
				}
				else
				{
					base.PaintChartSeries3D(graph);
				}
				if (!flag2)
				{
					this.PaintAreaBack(graph, backgroundPosition2, true);
				}
				Axis[] array7 = array2;
				foreach (Axis axis6 in array7)
				{
					if (axis6.ScaleSegments.Count <= 0)
					{
						axis6.Paint(graph);
					}
					else
					{
						foreach (AxisScaleSegment scaleSegment5 in axis6.ScaleSegments)
						{
							scaleSegment5.SetTempAxisScaleAndInterval();
							axis6.PaintOnSegmentedScalePassOne(graph);
							scaleSegment5.RestoreAxisScaleAndInterval();
						}
						axis6.PaintOnSegmentedScalePassTwo(graph);
					}
				}
				base.Common.EventsManager.OnPaint(this, new ChartPaintEventArgs(graph, base.Common, base.PlotAreaPosition));
				array2 = new Axis[2]
				{
					base.axisY,
					base.axisY2
				};
				Axis[] array8 = array2;
				foreach (Axis axis7 in array8)
				{
					for (int num3 = 0; num3 < axis7.ScaleSegments.Count - 1; num3++)
					{
						axis7.ScaleSegments[num3].PaintBreakLine(graph, axis7.ScaleSegments[num3 + 1]);
					}
				}
				Axis[] array9 = this.axisArray;
				foreach (Axis axis8 in array9)
				{
					axis8.optimizedGetPosition = false;
					axis8.prefferedNumberofIntervals = 5;
					axis8.scaleSegmentsUsed = false;
				}
			}
		}

		private bool IsBorderOnTopSeries()
		{
			bool result = true;
			foreach (Series item in base.Common.Chart.Series)
			{
				if (item.ChartArea == this.Name && (item.ChartType == SeriesChartType.Bubble || item.ChartType == SeriesChartType.Point))
				{
					return false;
				}
			}
			return result;
		}

		internal void PaintCursors(ChartGraphics graph, bool cursorOnly)
		{
			if (!base.Area3DStyle.Enable3D && base.requireAxes && !base.chartAreaIsCurcular && (base.Common == null || base.Common.ChartPicture == null || !base.Common.ChartPicture.isPrinting) && this.Position.Width != 0.0 && this.Position.Height != 0.0 && double.IsNaN(this.cursorX.SelectionStart) && double.IsNaN(this.cursorX.SelectionEnd) && double.IsNaN(this.cursorX.Position) && double.IsNaN(this.cursorY.SelectionStart) && double.IsNaN(this.cursorY.SelectionEnd))
			{
				double.IsNaN(this.cursorY.Position);
			}
		}

		internal ICircularChartType GetCircularChartType()
		{
			foreach (Series item in base.Common.DataManager.Series)
			{
				if (item.IsVisible() && item.ChartArea == this.Name)
				{
					ICircularChartType circularChartType = base.Common.ChartTypeRegistry.GetChartType(item.ChartTypeName) as ICircularChartType;
					if (circularChartType != null)
					{
						return circularChartType;
					}
				}
			}
			return null;
		}

		internal void FitCircularLabels(ChartGraphics chartGraph, ElementPosition chartAreaPosition, ref RectangleF plotArea, float xTitleSize, float yTitleSize)
		{
			if (this.AxisX.LabelStyle.Enabled)
			{
				SizeF absoluteSize = chartGraph.GetAbsoluteSize(new SizeF(xTitleSize, yTitleSize));
				RectangleF absoluteRectangle = chartGraph.GetAbsoluteRectangle(plotArea);
				RectangleF absoluteRectangle2 = chartGraph.GetAbsoluteRectangle(chartAreaPosition.ToRectangleF());
				float y = chartGraph.GetAbsolutePoint(new PointF(0f, (float)(this.AxisX.markSize + 1.0))).Y;
				ArrayList axisList = this.GetCircularAxisList();
				CircularAxisLabelsStyle circularAxisLabelsStyle = this.GetCircularAxisLabelsStyle();
				if (this.AxisX.LabelStyle.Enabled && this.AxisX.LabelsAutoFit)
				{
					this.AxisX.autoLabelFont = new Font(this.AxisX.LabelStyle.Font.FontFamily, 14f, this.AxisX.LabelStyle.Font.Style, GraphicsUnit.Pixel);
					float circularLabelsSize = this.GetCircularLabelsSize(chartGraph, absoluteRectangle2, absoluteRectangle, absoluteSize);
					circularLabelsSize = Math.Min((float)(circularLabelsSize * 1.1000000238418579), (float)(absoluteRectangle.Width / 5.0));
					circularLabelsSize += y;
					this.AxisX.GetCircularAxisLabelsAutoFitFont(chartGraph, axisList, circularAxisLabelsStyle, absoluteRectangle, absoluteRectangle2, circularLabelsSize);
				}
				float circularLabelsSize2 = this.GetCircularLabelsSize(chartGraph, absoluteRectangle2, absoluteRectangle, absoluteSize);
				circularLabelsSize2 = Math.Min(circularLabelsSize2, (float)(absoluteRectangle.Width / 2.5));
				circularLabelsSize2 += y;
				absoluteRectangle.X += circularLabelsSize2;
				absoluteRectangle.Width -= (float)(2.0 * circularLabelsSize2);
				absoluteRectangle.Y += circularLabelsSize2;
				absoluteRectangle.Height -= (float)(2.0 * circularLabelsSize2);
				if (absoluteRectangle.Width < 1.0)
				{
					absoluteRectangle.Width = 1f;
				}
				if (absoluteRectangle.Height < 1.0)
				{
					absoluteRectangle.Height = 1f;
				}
				plotArea = chartGraph.GetRelativeRectangle(absoluteRectangle);
				SizeF relativeSize = chartGraph.GetRelativeSize(new SizeF(circularLabelsSize2, circularLabelsSize2));
				this.AxisX.labelSize = relativeSize.Height;
				this.AxisX2.labelSize = relativeSize.Height;
				this.AxisY.labelSize = relativeSize.Width;
				this.AxisY2.labelSize = relativeSize.Width;
			}
		}

		internal float GetCircularLabelsSize(ChartGraphics chartGraph, RectangleF areaRectAbs, RectangleF plotAreaRectAbs, SizeF titleSize)
		{
			SizeF sizeF = new SizeF(plotAreaRectAbs.X - areaRectAbs.X, plotAreaRectAbs.Y - areaRectAbs.Y);
			sizeF.Width -= titleSize.Width;
			sizeF.Height -= titleSize.Height;
			PointF absolutePoint = chartGraph.GetAbsolutePoint(this.circularCenter);
			ArrayList arrayList = this.GetCircularAxisList();
			CircularAxisLabelsStyle circularAxisLabelsStyle = this.GetCircularAxisLabelsStyle();
			float num = 0f;
			foreach (CircularChartAreaAxis item in arrayList)
			{
				SizeF sizeF2 = chartGraph.MeasureString(item.Title.Replace("\\n", "\n"), (this.AxisX.autoLabelFont == null) ? this.AxisX.LabelStyle.font : this.AxisX.autoLabelFont);
				sizeF2.Width = (float)Math.Ceiling(sizeF2.Width * 1.1000000238418579);
				sizeF2.Height = (float)Math.Ceiling(sizeF2.Height * 1.1000000238418579);
				switch (circularAxisLabelsStyle)
				{
				case CircularAxisLabelsStyle.Circular:
					num = Math.Max(num, sizeF2.Height);
					break;
				case CircularAxisLabelsStyle.Radial:
				{
					float num3 = (float)(item.AxisPosition + 90.0);
					float num4 = (float)Math.Cos(num3 / 180.0 * 3.1415926535897931) * sizeF2.Width;
					float num5 = (float)Math.Sin(num3 / 180.0 * 3.1415926535897931) * sizeF2.Width;
					num4 = (float)Math.Abs(Math.Ceiling((double)num4));
					num5 = (float)Math.Abs(Math.Ceiling((double)num5));
					num4 -= sizeF.Width;
					num5 -= sizeF.Height;
					if (num4 < 0.0)
					{
						num4 = 0f;
					}
					if (num5 < 0.0)
					{
						num5 = 0f;
					}
					num = Math.Max(num, Math.Max(num4, num5));
					break;
				}
				case CircularAxisLabelsStyle.Horizontal:
				{
					float num2 = item.AxisPosition;
					if (num2 > 180.0)
					{
						num2 = (float)(num2 - 180.0);
					}
					PointF[] array = new PointF[1]
					{
						new PointF(absolutePoint.X, plotAreaRectAbs.Y)
					};
					Matrix matrix = new Matrix();
					matrix.RotateAt(num2, absolutePoint);
					matrix.TransformPoints(array);
					float width = sizeF2.Width;
					width -= areaRectAbs.Right - array[0].X;
					if (width < 0.0)
					{
						width = 0f;
					}
					num = Math.Max(num, Math.Max(width, sizeF2.Height));
					break;
				}
				}
			}
			return num;
		}

		internal CircularAxisLabelsStyle GetCircularAxisLabelsStyle()
		{
			CircularAxisLabelsStyle circularAxisLabelsStyle = CircularAxisLabelsStyle.Auto;
			foreach (Series item in base.Common.DataManager.Series)
			{
				if (item.IsVisible() && item.ChartArea == this.Name && item.IsAttributeSet("CircularLabelsStyle"))
				{
					string text = ((DataPointAttributes)item)["CircularLabelsStyle"];
					if (string.Compare(text, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
					{
						circularAxisLabelsStyle = CircularAxisLabelsStyle.Auto;
						continue;
					}
					if (string.Compare(text, "Circular", StringComparison.OrdinalIgnoreCase) == 0)
					{
						circularAxisLabelsStyle = CircularAxisLabelsStyle.Circular;
						continue;
					}
					if (string.Compare(text, "Radial", StringComparison.OrdinalIgnoreCase) == 0)
					{
						circularAxisLabelsStyle = CircularAxisLabelsStyle.Radial;
						continue;
					}
					if (string.Compare(text, "Horizontal", StringComparison.OrdinalIgnoreCase) == 0)
					{
						circularAxisLabelsStyle = CircularAxisLabelsStyle.Horizontal;
						continue;
					}
					throw new InvalidOperationException(SR.ExceptionCustomAttributeValueInvalid(text, "CircularLabelsStyle"));
				}
			}
			if (circularAxisLabelsStyle == CircularAxisLabelsStyle.Auto)
			{
				int circularSectorsNumber = this.CircularSectorsNumber;
				circularAxisLabelsStyle = CircularAxisLabelsStyle.Horizontal;
				if (circularSectorsNumber > 30)
				{
					circularAxisLabelsStyle = CircularAxisLabelsStyle.Radial;
				}
			}
			return circularAxisLabelsStyle;
		}

		private int GetCircularSectorNumber()
		{
			ICircularChartType circularChartType = this.GetCircularChartType();
			if (circularChartType != null)
			{
				return circularChartType.GetNumerOfSectors(this, base.Common.DataManager.Series);
			}
			return 0;
		}

		internal ArrayList GetCircularAxisList()
		{
			if (this.circularAxisList == null)
			{
				this.circularAxisList = new ArrayList();
				int num = this.GetCircularSectorNumber();
				for (int i = 0; i < num; i++)
				{
					CircularChartAreaAxis circularChartAreaAxis = new CircularChartAreaAxis((float)((float)i * 360.0 / (float)num), (float)(360.0 / (float)num));
					if (this.AxisX.CustomLabels.Count > 0)
					{
						if (i < this.AxisX.CustomLabels.Count)
						{
							circularChartAreaAxis.Title = this.AxisX.CustomLabels[i].Text;
							circularChartAreaAxis.TitleColor = this.AxisX.CustomLabels[i].TextColor;
						}
					}
					else
					{
						foreach (Series item in base.Common.DataManager.Series)
						{
							if (item.IsVisible() && item.ChartArea == this.Name && i < item.Points.Count && item.Points[i].AxisLabel.Length > 0)
							{
								circularChartAreaAxis.Title = item.Points[i].AxisLabel;
								break;
							}
						}
					}
					this.circularAxisList.Add(circularChartAreaAxis);
				}
			}
			return this.circularAxisList;
		}

		internal float CircularPositionToAngle(double position)
		{
			double num = 360.0 / Math.Abs(this.AxisX.Maximum - this.AxisX.Minimum);
			return (float)(position * num + this.AxisX.Crossing);
		}

		private ArrayList GetChartTypesAndSeriesToDraw()
		{
			ArrayList arrayList = new ArrayList();
			if (base.ChartTypes.Count > 1 && (base.ChartTypes.Contains("Area") || base.ChartTypes.Contains("SplineArea")))
			{
				ArrayList arrayList2 = new ArrayList();
				ArrayList arrayList3 = new ArrayList();
				ChartAreaCollection chartAreas = base.Common.Chart.ChartAreas;
				bool flag = chartAreas.Count > 0 && chartAreas[0] == this && chartAreas.GetIndex("Default") == -1;
				int num = 0;
				{
					foreach (Series item in base.Common.DataManager.Series)
					{
						if (item.IsVisible() && item.Points.Count > 0 && (this.Name == item.ChartArea || (flag && item.ChartArea == "Default")) && !arrayList2.Contains(item.ChartTypeName))
						{
							bool flag2 = false;
							if (item.ChartType == SeriesChartType.Point || item.ChartType == SeriesChartType.Line || item.ChartType == SeriesChartType.Spline || item.ChartType == SeriesChartType.StepLine)
							{
								flag2 = true;
							}
							if (!flag2)
							{
								arrayList.Add(new ChartTypeAndSeriesInfo(item.ChartTypeName));
								arrayList2.Add(item.ChartTypeName);
							}
							else
							{
								bool flag3 = false;
								if (arrayList3.Contains(item.ChartTypeName))
								{
									flag3 = true;
								}
								else
								{
									bool flag4 = false;
									for (int i = num + 1; i < base.Common.DataManager.Series.Count; i++)
									{
										if (item.ChartTypeName == base.Common.DataManager.Series[i].ChartTypeName)
										{
											if (flag4)
											{
												flag3 = true;
												arrayList3.Add(item.ChartTypeName);
											}
										}
										else if (base.Common.DataManager.Series[i].ChartType == SeriesChartType.Area || base.Common.DataManager.Series[i].ChartType == SeriesChartType.SplineArea)
										{
											flag4 = true;
										}
									}
								}
								if (flag3)
								{
									arrayList.Add(new ChartTypeAndSeriesInfo(item));
								}
								else
								{
									arrayList.Add(new ChartTypeAndSeriesInfo(item.ChartTypeName));
									arrayList2.Add(item.ChartTypeName);
								}
							}
						}
						num++;
					}
					return arrayList;
				}
			}
			foreach (string chartType in base.ChartTypes)
			{
				arrayList.Add(new ChartTypeAndSeriesInfo(chartType));
			}
			return arrayList;
		}
	}
}
