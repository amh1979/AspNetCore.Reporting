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
	[DefaultProperty("Name")]
	[SRDescription("DescriptionAttributeAnnotation_Annotation")]
	internal abstract class Annotation : IMapAreaAttributes
	{
		internal const int selectionMarkerSize = 6;

		private string name = string.Empty;

		private string clipToChartArea = "NotSet";

		private bool selected;

		private bool sizeAlwaysRelative = true;

		private object tag;

		internal Chart chart;

		private double x = double.NaN;

		private double y = double.NaN;

		private double width = double.NaN;

		private double height = double.NaN;

		private string axisXName = string.Empty;

		private string axisYName = string.Empty;

		private Axis axisX;

		private Axis axisY;

		private bool visible = true;

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private Color textColor = Color.Black;

		private Font textFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private TextStyle textStyle;

		internal Color lineColor = Color.Black;

		private int lineWidth = 1;

		private ChartDashStyle lineStyle = ChartDashStyle.Solid;

		private Color backColor = Color.Empty;

		private ChartHatchStyle backHatchStyle;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		private int shadowOffset;

		private string anchorDataPointName = string.Empty;

		private DataPoint anchorDataPoint;

		private DataPoint anchorDataPoint2;

		private double anchorX = double.NaN;

		private double anchorY = double.NaN;

		internal double anchorOffsetX;

		internal double anchorOffsetY;

		internal ContentAlignment anchorAlignment = ContentAlignment.BottomCenter;

		internal RectangleF[] selectionRects;

		internal bool outsideClipRegion;

		private string tooltip = string.Empty;

		internal RectangleF currentPositionRel = new RectangleF(float.NaN, float.NaN, float.NaN, float.NaN);

		internal PointF currentAnchorLocationRel = new PointF(float.NaN, float.NaN);

		private AnnotationSmartLabelsStyle smartLabelsStyle;

		internal int currentPathPointIndex = -1;

		internal AnnotationGroup annotationGroup;

		private bool allowSelecting = true;

		private bool allowMoving = true;

		private bool allowAnchorMoving = true;

		private bool allowResizing = true;

		private bool allowTextEditing = true;

		private bool allowPathEditing = true;

		internal bool positionChanged;

		internal RectangleF startMovePositionRel = RectangleF.Empty;

		internal GraphicsPath startMovePathRel;

		internal PointF startMoveAnchorLocationRel = PointF.Empty;

		internal PointF lastPlacementPosition = PointF.Empty;

		private string href = string.Empty;

		private string mapAreaAttributes = string.Empty;

		private object mapAreaTag;

		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeName4")]
		public virtual string Name
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
					if (this.chart != null && this.chart.Annotations.FindByName(value) != null)
					{
						throw new ArgumentException(SR.ExceptionAnnotationNameIsNotUnique(value));
					}
					this.name = value;
					return;
				}
				throw new ArgumentException(SR.ExceptionAnnotationNameIsEmpty);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotation_AnnotationType")]
		public abstract string AnnotationType
		{
			get;
		}

		[SRCategory("CategoryAttributeMisc")]
		[TypeConverter(typeof(LegendAreaNameConverter))]
		[DefaultValue("NotSet")]
		[SRDescription("DescriptionAttributeAnnotationClipToChartArea")]
		public virtual string ClipToChartArea
		{
			get
			{
				return this.clipToChartArea;
			}
			set
			{
				this.clipToChartArea = value;
				this.Invalidate();
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeMisc")]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeTag5")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeChart")]
		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(null)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		internal virtual Chart Chart
		{
			get
			{
				return this.chart;
			}
			set
			{
				this.chart = value;
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeSmartLabels")]
		[SRCategory("CategoryAttributeMisc")]
		[Browsable(true)]
		public AnnotationSmartLabelsStyle SmartLabels
		{
			get
			{
				if (this.smartLabelsStyle == null)
				{
					this.smartLabelsStyle = new AnnotationSmartLabelsStyle(this);
				}
				return this.smartLabelsStyle;
			}
			set
			{
				value.chartElement = this;
				this.smartLabelsStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeSizeAlwaysRelative")]
		[DefaultValue(true)]
		[SRCategory("CategoryAttributePosition")]
		public virtual bool SizeAlwaysRelative
		{
			get
			{
				return this.sizeAlwaysRelative;
			}
			set
			{
				this.sizeAlwaysRelative = value;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRDescription("DescriptionAttributeAnnotationBaseX")]
		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(double.NaN)]
		public virtual double X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAnnotationBaseY")]
		public virtual double Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRDescription("DescriptionAttributeAnnotationWidth")]
		[SRCategory("CategoryAttributePosition")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(double.NaN)]
		public virtual double Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAnnotationHeight")]
		[SRCategory("CategoryAttributePosition")]
		public virtual double Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeRight3")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[DefaultValue(double.NaN)]
		[SRCategory("CategoryAttributePosition")]
		[RefreshProperties(RefreshProperties.All)]
		public virtual double Right
		{
			get
			{
				return this.x + this.width;
			}
			set
			{
				this.width = value - this.x;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeBottom")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(double.NaN)]
		[RefreshProperties(RefreshProperties.All)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual double Bottom
		{
			get
			{
				return this.y + this.height;
			}
			set
			{
				this.height = value - this.y;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeSelected")]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(false)]
		public virtual bool Selected
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

		[ParenthesizePropertyName(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal virtual SelectionPointsStyle SelectionPointsStyle
		{
			get
			{
				return SelectionPointsStyle.Rectangle;
			}
		}

		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeVisible6")]
		public virtual bool Visible
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

		[SRDescription("DescriptionAttributeAlignment7")]
		[DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
		[SRCategory("CategoryAttributeAppearance")]
		public virtual ContentAlignment Alignment
		{
			get
			{
				return this.alignment;
			}
			set
			{
				this.alignment = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTextColor6")]
		[SRCategory("CategoryAttributeAppearance")]
		public virtual Color TextColor
		{
			get
			{
				return this.textColor;
			}
			set
			{
				this.textColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTextFont")]
		public virtual Font TextFont
		{
			get
			{
				return this.textFont;
			}
			set
			{
				this.textFont = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(TextStyle), "Default")]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeTextStyle3")]
		public virtual TextStyle TextStyle
		{
			get
			{
				return this.textStyle;
			}
			set
			{
				this.textStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLineColor3")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "Black")]
		public virtual Color LineColor
		{
			get
			{
				return this.lineColor;
			}
			set
			{
				this.lineColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLineWidth7")]
		[DefaultValue(1)]
		public virtual int LineWidth
		{
			get
			{
				return this.lineWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionAnnotationLineWidthIsNegative);
				}
				this.lineWidth = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLineStyle6")]
		[DefaultValue(ChartDashStyle.Solid)]
		public virtual ChartDashStyle LineStyle
		{
			get
			{
				return this.lineStyle;
			}
			set
			{
				this.lineStyle = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBackColor9")]
		public virtual Color BackColor
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

		[DefaultValue(ChartHatchStyle.None)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeBackHatchStyle")]
		public virtual ChartHatchStyle BackHatchStyle
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

		[DefaultValue(GradientType.None)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeBackGradientType12")]
		[NotifyParentProperty(true)]
		public virtual GradientType BackGradientType
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
		[SRDescription("DescriptionAttributeBackGradientEndColor13")]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		public virtual Color BackGradientEndColor
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

		[SRDescription("DescriptionAttributeShadowColor4")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "128,0,0,0")]
		public virtual Color ShadowColor
		{
			get
			{
				return this.shadowColor;
			}
			set
			{
				this.shadowColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeShadowOffset7")]
		public virtual int ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				this.shadowOffset = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeAxisXName")]
		[SRCategory("CategoryAttributeAnchorAxes")]
		[DefaultValue("")]
		[Browsable(false)]
		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string AxisXName
		{
			get
			{
				if (this.axisXName.Length == 0 && this.axisX != null)
				{
					this.axisXName = this.GetAxisName(this.axisX);
				}
				return this.axisXName;
			}
			set
			{
				this.axisXName = value;
				this.axisX = null;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[Bindable(false)]
		[SRCategory("CategoryAttributeAnchorAxes")]
		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeAxisYName")]
		public virtual string AxisYName
		{
			get
			{
				return string.Empty;
			}
			set
			{
				this.YAxisName = value;
			}
		}

		[SRCategory("CategoryAttributeAnchorAxes")]
		[SRDescription("DescriptionAttributeAxisYName")]
		[Browsable(false)]
		[Bindable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		public virtual string YAxisName
		{
			get
			{
				if (this.axisYName.Length == 0 && this.axisY != null)
				{
					this.axisYName = this.GetAxisName(this.axisY);
				}
				return this.axisYName;
			}
			set
			{
				this.axisYName = value;
				this.axisY = null;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[TypeConverter(typeof(AnnotationAxisValueConverter))]
		[SRCategory("CategoryAttributeAnchorAxes")]
		[DefaultValue(null)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAxisX")]
		public virtual Axis AxisX
		{
			get
			{
				if (this.axisX == null && this.axisXName.Length > 0)
				{
					this.axisX = this.GetAxisByName(this.axisXName);
				}
				return this.axisX;
			}
			set
			{
				this.axisX = value;
				this.axisXName = string.Empty;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[TypeConverter(typeof(AnnotationAxisValueConverter))]
		[SRCategory("CategoryAttributeAnchorAxes")]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAxisY")]
		public virtual Axis AxisY
		{
			get
			{
				if (this.axisY == null && this.axisYName.Length > 0)
				{
					this.axisY = this.GetAxisByName(this.axisYName);
				}
				return this.axisY;
			}
			set
			{
				this.axisY = value;
				this.axisYName = string.Empty;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[Bindable(false)]
		[SRCategory("CategoryAttributeAnchor")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeAnchorDataPointName")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue("")]
		public virtual string AnchorDataPointName
		{
			get
			{
				if (this.anchorDataPointName.Length == 0 && this.anchorDataPoint != null)
				{
					this.anchorDataPointName = this.GetDataPointName(this.anchorDataPoint);
				}
				return this.anchorDataPointName;
			}
			set
			{
				this.anchorDataPointName = value;
				this.anchorDataPoint = null;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[TypeConverter(typeof(AnchorPointValueConverter))]
		[SRCategory("CategoryAttributeAnchor")]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnchorDataPoint")]
		public virtual DataPoint AnchorDataPoint
		{
			get
			{
				if (this.anchorDataPoint == null && this.anchorDataPointName.Length > 0)
				{
					this.anchorDataPoint = this.GetDataPointByName(this.anchorDataPointName);
				}
				return this.anchorDataPoint;
			}
			set
			{
				this.anchorDataPoint = value;
				this.anchorDataPointName = string.Empty;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAnchor")]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAnchorX")]
		[RefreshProperties(RefreshProperties.All)]
		public virtual double AnchorX
		{
			get
			{
				return this.anchorX;
			}
			set
			{
				this.anchorX = value;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[TypeConverter(typeof(DoubleNanValueConverter))]
		[SRCategory("CategoryAttributeAnchor")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeAnchorY")]
		[RefreshProperties(RefreshProperties.All)]
		public virtual double AnchorY
		{
			get
			{
				return this.anchorY;
			}
			set
			{
				this.anchorY = value;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeAnchorOffsetX3")]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttributeAnchor")]
		[DefaultValue(0.0)]
		public virtual double AnchorOffsetX
		{
			get
			{
				return this.anchorOffsetX;
			}
			set
			{
				if (!(value > 100.0) && !(value < -100.0))
				{
					this.anchorOffsetX = value;
					this.ResetCurrentRelativePosition();
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionAnnotationAnchorOffsetInvalid);
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeAnchorOffsetY3")]
		[SRCategory("CategoryAttributeAnchor")]
		[DefaultValue(0.0)]
		public virtual double AnchorOffsetY
		{
			get
			{
				return this.anchorOffsetY;
			}
			set
			{
				if (!(value > 100.0) && !(value < -100.0))
				{
					this.anchorOffsetY = value;
					this.ResetCurrentRelativePosition();
					this.Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionAnnotationAnchorOffsetInvalid);
			}
		}

		[DefaultValue(typeof(ContentAlignment), "BottomCenter")]
		[SRDescription("DescriptionAttributeAnchorAlignment3")]
		[SRCategory("CategoryAttributeAnchor")]
		public virtual ContentAlignment AnchorAlignment
		{
			get
			{
				return this.anchorAlignment;
			}
			set
			{
				this.anchorAlignment = value;
				this.ResetCurrentRelativePosition();
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeEditing")]
		[SRDescription("DescriptionAttributeAllowSelecting")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(true)]
		public virtual bool AllowSelecting
		{
			get
			{
				return this.allowSelecting;
			}
			set
			{
				this.allowSelecting = value;
			}
		}

		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAllowMoving")]
		[SRCategory("CategoryAttributeEditing")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool AllowMoving
		{
			get
			{
				return this.allowMoving;
			}
			set
			{
				this.allowMoving = value;
			}
		}

		[SRCategory("CategoryAttributeEditing")]
		[Browsable(false)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAllowAnchorMoving3")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool AllowAnchorMoving
		{
			get
			{
				return this.allowAnchorMoving;
			}
			set
			{
				this.allowAnchorMoving = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRDescription("DescriptionAttributeAllowResizing")]
		[SRCategory("CategoryAttributeEditing")]
		[DefaultValue(true)]
		public virtual bool AllowResizing
		{
			get
			{
				return this.allowResizing;
			}
			set
			{
				this.allowResizing = value;
			}
		}

		[SRDescription("DescriptionAttributeAllowTextEditing")]
		[SRCategory("CategoryAttributeEditing")]
		[Browsable(false)]
		[DefaultValue(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool AllowTextEditing
		{
			get
			{
				return this.allowTextEditing;
			}
			set
			{
				this.allowTextEditing = value;
			}
		}

		[SRDescription("DescriptionAttributeAllowPathEditing3")]
		[SRCategory("CategoryAttributeEditing")]
		[Browsable(false)]
		[DefaultValue(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool AllowPathEditing
		{
			get
			{
				return this.allowPathEditing;
			}
			set
			{
				this.allowPathEditing = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeToolTip4")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string ToolTip
		{
			get
			{
				return this.tooltip;
			}
			set
			{
				this.tooltip = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeHref")]
		[DefaultValue("")]
		public virtual string Href
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

		[SRDescription("DescriptionAttributeMapAreaAttributes3")]
		[DefaultValue("")]
		[SRCategory("CategoryAttributeMisc")]
		public virtual string MapAreaAttributes
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

		string IMapAreaAttributes.ToolTip
		{
			get
			{
				return this.ToolTip;
			}
			set
			{
				this.ToolTip = value;
			}
		}

		string IMapAreaAttributes.Href
		{
			get
			{
				return this.Href;
			}
			set
			{
				this.Href = value;
			}
		}

		string IMapAreaAttributes.MapAreaAttributes
		{
			get
			{
				return this.MapAreaAttributes;
			}
			set
			{
				this.MapAreaAttributes = value;
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

		internal abstract void Paint(Chart chart, ChartGraphics graphics);

		internal virtual void PaintSelectionHandles(ChartGraphics chartGraphics, RectangleF rect, GraphicsPath path)
		{
			Color black = Color.Black;
			Color markerColor = Color.FromArgb(200, 255, 255, 255);
			MarkerStyle markerStyle = MarkerStyle.Square;
			int num = 6;
			bool flag = this.Selected;
			SizeF relativeSize = chartGraphics.GetRelativeSize(new SizeF((float)num, (float)num));
			if (this.chart.chartPicture.common.ProcessModePaint && !this.chart.chartPicture.isPrinting)
			{
				this.selectionRects = null;
				if (flag)
				{
					this.selectionRects = new RectangleF[9];
					if (this.SelectionPointsStyle == SelectionPointsStyle.TwoPoints)
					{
						this.selectionRects[0] = new RectangleF((float)(rect.X - relativeSize.Width / 2.0), (float)(rect.Y - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height);
						this.selectionRects[4] = new RectangleF((float)(rect.Right - relativeSize.Width / 2.0), (float)(rect.Bottom - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height);
						chartGraphics.DrawMarkerRel(rect.Location, markerStyle, num, markerColor, black, 1, "", Color.Empty, 0, Color.FromArgb(128, 0, 0, 0), RectangleF.Empty);
						chartGraphics.DrawMarkerRel(new PointF(rect.Right, rect.Bottom), markerStyle, num, markerColor, black, 1, "", Color.Empty, 0, Color.FromArgb(128, 0, 0, 0), RectangleF.Empty);
					}
					else if (this.SelectionPointsStyle == SelectionPointsStyle.Rectangle)
					{
						for (int i = 0; i < 8; i++)
						{
							PointF point = PointF.Empty;
							switch (i)
							{
							case 0:
								point = rect.Location;
								break;
							case 1:
								point = new PointF((float)(rect.X + rect.Width / 2.0), rect.Y);
								break;
							case 2:
								point = new PointF(rect.Right, rect.Y);
								break;
							case 3:
								point = new PointF(rect.Right, (float)(rect.Y + rect.Height / 2.0));
								break;
							case 4:
								point = new PointF(rect.Right, rect.Bottom);
								break;
							case 5:
								point = new PointF((float)(rect.X + rect.Width / 2.0), rect.Bottom);
								break;
							case 6:
								point = new PointF(rect.X, rect.Bottom);
								break;
							case 7:
								point = new PointF(rect.X, (float)(rect.Y + rect.Height / 2.0));
								break;
							}
							this.selectionRects[i] = new RectangleF((float)(point.X - relativeSize.Width / 2.0), (float)(point.Y - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height);
							chartGraphics.DrawMarkerRel(point, markerStyle, num, markerColor, black, 1, "", Color.Empty, 0, Color.FromArgb(128, 0, 0, 0), RectangleF.Empty);
						}
					}
					Axis axis = null;
					Axis axis2 = null;
					this.GetAxes(ref axis, ref axis2);
					double num2 = double.NaN;
					double num3 = double.NaN;
					bool flag2 = false;
					bool flag3 = false;
					this.GetAnchorLocation(ref num2, ref num3, ref flag2, ref flag3);
					if (!double.IsNaN(num2) && !double.IsNaN(num3))
					{
						if (!flag2 && axis2 != null)
						{
							num2 = axis2.ValueToPosition(num2);
						}
						if (!flag3 && axis != null)
						{
							num3 = axis.ValueToPosition(num3);
						}
						ChartArea chartArea = null;
						if (axis2 != null && axis2.chartArea != null)
						{
							chartArea = axis2.chartArea;
						}
						if (axis != null && axis.chartArea != null)
						{
							chartArea = axis.chartArea;
						}
						if (chartArea != null && chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular && chartArea.requireAxes && chartArea.matrix3D.IsInitialized())
						{
							float num4 = chartArea.areaSceneDepth;
							if (this.AnchorDataPoint != null && this.AnchorDataPoint.series != null)
							{
								float num5 = 0f;
								((ChartArea3D)chartArea).GetSeriesZPositionAndDepth(this.AnchorDataPoint.series, out num5, out num4);
								num4 = (float)(num4 + num5 / 2.0);
							}
							Point3D[] array = new Point3D[1]
							{
								new Point3D((float)num2, (float)num3, num4)
							};
							chartArea.matrix3D.TransformPoints(array);
							num2 = (double)array[0].X;
							num3 = (double)array[0].Y;
						}
						this.selectionRects[8] = new RectangleF((float)((float)num2 - relativeSize.Width / 2.0), (float)((float)num3 - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height);
						chartGraphics.DrawMarkerRel(new PointF((float)num2, (float)num3), MarkerStyle.Cross, 9, markerColor, black, 1, "", Color.Empty, 0, Color.FromArgb(128, 0, 0, 0), RectangleF.Empty);
					}
					if (path != null && this.AllowPathEditing)
					{
						PointF[] pathPoints = path.PathPoints;
						RectangleF[] array2 = new RectangleF[pathPoints.Length + 9];
						for (int j = 0; j < this.selectionRects.Length; j++)
						{
							array2[j] = this.selectionRects[j];
						}
						this.selectionRects = array2;
						for (int k = 0; k < pathPoints.Length; k++)
						{
							PointF relativePoint = chartGraphics.GetRelativePoint(pathPoints[k]);
							this.selectionRects[9 + k] = new RectangleF((float)(relativePoint.X - relativeSize.Width / 2.0), (float)(relativePoint.Y - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height);
							chartGraphics.DrawMarkerRel(relativePoint, MarkerStyle.Circle, 7, markerColor, black, 1, "", Color.Empty, 0, Color.FromArgb(128, 0, 0, 0), RectangleF.Empty);
						}
					}
				}
			}
		}

		public virtual void ResizeToContent()
		{
			RectangleF contentPosition = this.GetContentPosition();
			if (!double.IsNaN((double)contentPosition.Width))
			{
				this.Width = (double)contentPosition.Width;
			}
			if (!double.IsNaN((double)contentPosition.Height))
			{
				this.Height = (double)contentPosition.Height;
			}
		}

		internal virtual RectangleF GetContentPosition()
		{
			return new RectangleF(float.NaN, float.NaN, float.NaN, float.NaN);
		}

		private void GetAnchorLocation(ref double anchorX, ref double anchorY, ref bool inRelativeAnchorX, ref bool inRelativeAnchorY)
		{
			anchorX = this.AnchorX;
			anchorY = this.AnchorY;
			if (this.AnchorDataPoint != null && this.AnchorDataPoint.series != null && this.Chart != null && this.Chart.chartPicture != null)
			{
				if (this.GetAnnotationGroup() != null)
				{
					throw new InvalidOperationException(SR.ExceptionAnnotationGroupedAnchorDataPointMustBeEmpty);
				}
				PointF empty = PointF.Empty;
				if (!double.IsNaN(anchorX) && !double.IsNaN(anchorY))
				{
					return;
				}
				if (double.IsNaN(anchorX))
				{
					anchorX = (double)this.AnchorDataPoint.positionRel.X;
					inRelativeAnchorX = true;
				}
				if (double.IsNaN(anchorY))
				{
					anchorY = (double)this.AnchorDataPoint.positionRel.Y;
					inRelativeAnchorY = true;
				}
			}
		}

		internal virtual void GetRelativePosition(out PointF location, out SizeF size, out PointF anchorLocation)
		{
			bool flag = true;
			if (!double.IsNaN((double)this.currentPositionRel.X) && !double.IsNaN((double)this.currentPositionRel.X))
			{
				location = this.currentPositionRel.Location;
				size = this.currentPositionRel.Size;
				anchorLocation = this.currentAnchorLocationRel;
			}
			else
			{
				Axis axis = null;
				Axis axis2 = null;
				this.GetAxes(ref axis, ref axis2);
				if (this.anchorDataPoint != null && this.anchorDataPoint2 != null)
				{
					this.SizeAlwaysRelative = false;
					this.Height = axis.PositionToValue((double)this.anchorDataPoint2.positionRel.Y, false) - axis.PositionToValue((double)this.anchorDataPoint.positionRel.Y, false);
					this.Width = axis2.PositionToValue((double)this.anchorDataPoint2.positionRel.X, false) - axis2.PositionToValue((double)this.anchorDataPoint.positionRel.X, false);
					this.anchorDataPoint2 = null;
				}
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = (byte)(this.sizeAlwaysRelative ? 1 : 0) != 0;
				bool flag5 = (byte)(this.sizeAlwaysRelative ? 1 : 0) != 0;
				bool flag6 = false;
				bool flag7 = false;
				double num = this.AnchorX;
				double num2 = this.AnchorY;
				this.GetAnchorLocation(ref num, ref num2, ref flag6, ref flag7);
				AnnotationGroup annotationGroup = this.GetAnnotationGroup();
				PointF empty = PointF.Empty;
				double num3 = 1.0;
				double num4 = 1.0;
				if (annotationGroup != null)
				{
					flag = false;
					SizeF empty2 = SizeF.Empty;
					PointF empty3 = PointF.Empty;
					((Annotation)annotationGroup).GetRelativePosition(out empty, out empty2, out empty3);
					num3 = (double)empty2.Width / 100.0;
					num4 = (double)empty2.Height / 100.0;
				}
				double num5 = this.width;
				double num6 = this.height;
				RectangleF contentPosition = this.GetContentPosition();
				if (double.IsNaN(num5))
				{
					num5 = (double)contentPosition.Width;
					flag4 = true;
				}
				else
				{
					num5 *= num3;
				}
				if (double.IsNaN(num6))
				{
					num6 = (double)contentPosition.Height;
					flag5 = true;
				}
				else
				{
					num6 *= num4;
				}
				if (this.Chart != null && this.Chart.IsDesignMode() && (this.SizeAlwaysRelative || (axis == null && axis2 == null)))
				{
					if (double.IsNaN(num5))
					{
						num5 = 20.0;
						flag = false;
					}
					if (double.IsNaN(num6))
					{
						num6 = 20.0;
						flag = false;
					}
				}
				double num7 = this.X;
				double num8 = this.Y;
				if (double.IsNaN(num8) && !double.IsNaN(num2))
				{
					flag3 = true;
					double num9 = num2;
					if (!flag7 && axis != null)
					{
						num9 = axis.ValueToPosition(num2);
					}
					if (this.AnchorAlignment == ContentAlignment.TopCenter || this.AnchorAlignment == ContentAlignment.TopLeft || this.AnchorAlignment == ContentAlignment.TopRight)
					{
						num8 = num9 + this.AnchorOffsetY;
						num8 *= num4;
					}
					else if (this.AnchorAlignment == ContentAlignment.BottomCenter || this.AnchorAlignment == ContentAlignment.BottomLeft || this.AnchorAlignment == ContentAlignment.BottomRight)
					{
						num8 = num9 - this.AnchorOffsetY;
						num8 *= num4;
						if (num6 != 0.0 && !double.IsNaN(num6))
						{
							if (flag5)
							{
								num8 -= num6;
							}
							else if (axis != null)
							{
								float num10 = (float)axis.PositionToValue(num8);
								float num11 = (float)axis.ValueToPosition((double)num10 + num6);
								num8 -= (double)num11 - num8;
							}
						}
					}
					else
					{
						num8 = num9 + this.AnchorOffsetY;
						num8 *= num4;
						if (num6 != 0.0 && !double.IsNaN(num6))
						{
							if (flag5)
							{
								num8 -= num6 / 2.0;
							}
							else if (axis != null)
							{
								float num12 = (float)axis.PositionToValue(num8);
								float num13 = (float)axis.ValueToPosition((double)num12 + num6);
								num8 -= ((double)num13 - num8) / 2.0;
							}
						}
					}
				}
				else
				{
					num8 *= num4;
				}
				if (double.IsNaN(num7) && !double.IsNaN(num))
				{
					flag2 = true;
					double num14 = num;
					if (!flag6 && axis2 != null)
					{
						num14 = axis2.ValueToPosition(num);
					}
					if (this.AnchorAlignment == ContentAlignment.BottomLeft || this.AnchorAlignment == ContentAlignment.MiddleLeft || this.AnchorAlignment == ContentAlignment.TopLeft)
					{
						num7 = num14 + this.AnchorOffsetX;
						num7 *= num3;
					}
					else if (this.AnchorAlignment == ContentAlignment.BottomRight || this.AnchorAlignment == ContentAlignment.MiddleRight || this.AnchorAlignment == ContentAlignment.TopRight)
					{
						num7 = num14 - this.AnchorOffsetX;
						num7 *= num3;
						if (num5 != 0.0 && !double.IsNaN(num5))
						{
							if (flag4)
							{
								num7 -= num5;
							}
							else if (axis2 != null)
							{
								float num15 = (float)axis2.PositionToValue(num7);
								num7 -= axis2.ValueToPosition((double)num15 + num5) - num7;
							}
						}
					}
					else
					{
						num7 = num14 + this.AnchorOffsetX;
						num7 *= num3;
						if (num5 != 0.0 && !double.IsNaN(num5))
						{
							if (flag4)
							{
								num7 -= num5 / 2.0;
							}
							else if (axis2 != null)
							{
								float num16 = (float)axis2.PositionToValue(num7);
								num7 -= (axis2.ValueToPosition((double)num16 + num5) - num7) / 2.0;
							}
						}
					}
				}
				else
				{
					num7 *= num3;
				}
				num7 += (double)empty.X;
				num8 += (double)empty.Y;
				if (double.IsNaN(num7))
				{
					num7 = (double)contentPosition.X * num3;
					flag2 = true;
				}
				if (double.IsNaN(num8))
				{
					num8 = (double)contentPosition.Y * num4;
					flag3 = true;
				}
				if (axis2 != null)
				{
					if (!flag2)
					{
						num7 = axis2.ValueToPosition(num7);
					}
					if (!flag6)
					{
						num = axis2.ValueToPosition(num);
					}
					if (!flag4)
					{
						num5 = axis2.ValueToPosition(axis2.PositionToValue(num7, false) + num5) - num7;
					}
				}
				if (axis != null)
				{
					if (!flag3)
					{
						num8 = axis.ValueToPosition(num8);
					}
					if (!flag7)
					{
						num2 = axis.ValueToPosition(num2);
					}
					if (!flag5)
					{
						num6 = axis.ValueToPosition(axis.PositionToValue(num8, false) + num6) - num8;
					}
				}
				ChartArea chartArea = null;
				if (axis2 != null && axis2.chartArea != null)
				{
					chartArea = axis2.chartArea;
				}
				if (axis != null && axis.chartArea != null)
				{
					chartArea = axis.chartArea;
				}
				if (chartArea != null && chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular && chartArea.requireAxes && chartArea.matrix3D.IsInitialized())
				{
					float num17 = chartArea.areaSceneDepth;
					if (this.AnchorDataPoint != null && this.AnchorDataPoint.series != null)
					{
						float num18 = 0f;
						((ChartArea3D)chartArea).GetSeriesZPositionAndDepth(this.AnchorDataPoint.series, out num18, out num17);
						num17 = (float)(num17 + num18 / 2.0);
					}
					Point3D[] array = new Point3D[3]
					{
						new Point3D((float)num7, (float)num8, num17),
						new Point3D((float)(num7 + num5), (float)(num8 + num6), num17),
						new Point3D((float)num, (float)num2, num17)
					};
					chartArea.matrix3D.TransformPoints(array);
					num7 = (double)array[0].X;
					num8 = (double)array[0].Y;
					num = (double)array[2].X;
					num2 = (double)array[2].Y;
					if (!(this is TextAnnotation) || !this.SizeAlwaysRelative)
					{
						num5 = (double)array[1].X - num7;
						num6 = (double)array[1].Y - num8;
					}
				}
				if (this.Chart != null && this.Chart.IsDesignMode())
				{
					if (double.IsNaN(num7))
					{
						num7 = (double)empty.X;
						flag = false;
					}
					if (double.IsNaN(num8))
					{
						num8 = (double)empty.Y;
						flag = false;
					}
					if (double.IsNaN(num5))
					{
						num5 = 20.0 * num3;
						flag = false;
					}
					if (double.IsNaN(num6))
					{
						num6 = 20.0 * num4;
						flag = false;
					}
				}
				location = new PointF((float)num7, (float)num8);
				size = new SizeF((float)num5, (float)num6);
				anchorLocation = new PointF((float)num, (float)num2);
				if (this.SmartLabels.Enabled && annotationGroup == null)
				{
					if (!double.IsNaN(num) && !double.IsNaN(num2) && double.IsNaN(this.X) && double.IsNaN(this.Y))
					{
						if (this.Chart != null && this.Chart.chartPicture != null)
						{
							double minMovingDistance = this.SmartLabels.MinMovingDistance;
							double maxMovingDistance = this.SmartLabels.MaxMovingDistance;
							PointF absolutePoint = this.GetGraphics().GetAbsolutePoint(new PointF((float)this.AnchorOffsetX, (float)this.AnchorOffsetY));
							float num19 = Math.Max(absolutePoint.X, absolutePoint.Y);
							if ((double)num19 > 0.0)
							{
								this.SmartLabels.MinMovingDistance += (double)num19;
								this.SmartLabels.MaxMovingDistance += (double)num19;
							}
							LabelAlignmentTypes labelAlignment = LabelAlignmentTypes.Bottom;
							StringFormat format = new StringFormat();
							SizeF markerSize = new SizeF((float)this.AnchorOffsetX, (float)this.AnchorOffsetY);
							PointF position = this.Chart.chartPicture.annotationSmartLabels.AdjustSmartLabelPosition(this.Chart.chartPicture.common, this.Chart.chartPicture.chartGraph, chartArea, this.SmartLabels, location, size, ref format, anchorLocation, markerSize, labelAlignment, this is CalloutAnnotation);
							this.SmartLabels.MinMovingDistance = minMovingDistance;
							this.SmartLabels.MaxMovingDistance = maxMovingDistance;
							if (position.IsEmpty)
							{
								location = new PointF(float.NaN, float.NaN);
							}
							else
							{
								location = this.Chart.chartPicture.annotationSmartLabels.GetLabelPosition(this.Chart.chartPicture.chartGraph, position, size, format, false).Location;
							}
						}
					}
					else
					{
						StringFormat format2 = new StringFormat();
						this.Chart.chartPicture.annotationSmartLabels.AddSmartLabelPosition(this.Chart.chartPicture.chartGraph, chartArea, location, size, format2);
					}
				}
				if (flag)
				{
					this.currentPositionRel = new RectangleF(location, size);
					this.currentAnchorLocationRel = new PointF(anchorLocation.X, anchorLocation.Y);
				}
			}
		}

		internal void SetPositionRelative(RectangleF position, PointF anchorPoint)
		{
			this.SetPositionRelative(position, anchorPoint, false);
		}

		internal void SetPositionRelative(RectangleF position, PointF anchorPoint, bool userInput)
		{
			double num = (double)position.X;
			double num2 = (double)position.Y;
			double num3 = (double)position.Right;
			double num4 = (double)position.Bottom;
			double num5 = (double)position.Width;
			double num6 = (double)position.Height;
			double num7 = (double)anchorPoint.X;
			double num8 = (double)anchorPoint.Y;
			this.currentPositionRel = new RectangleF(position.Location, position.Size);
			this.currentAnchorLocationRel = new PointF(anchorPoint.X, anchorPoint.Y);
			this.outsideClipRegion = false;
			RectangleF rectangleF = new RectangleF(0f, 0f, 100f, 100f);
			if (this.ClipToChartArea.Length > 0 && this.ClipToChartArea != "NotSet")
			{
				int index = this.chart.ChartAreas.GetIndex(this.ClipToChartArea);
				if (index >= 0)
				{
					rectangleF = this.chart.ChartAreas[index].PlotAreaPosition.ToRectangleF();
				}
			}
			if (position.X > rectangleF.Right || position.Y > rectangleF.Bottom || position.Right < rectangleF.X || position.Bottom < rectangleF.Y)
			{
				this.outsideClipRegion = true;
			}
			Axis axis = null;
			Axis axis2 = null;
			this.GetAxes(ref axis, ref axis2);
			ChartArea chartArea = null;
			if (axis2 != null && axis2.chartArea != null)
			{
				chartArea = axis2.chartArea;
			}
			if (axis != null && axis.chartArea != null)
			{
				chartArea = axis.chartArea;
			}
			if (chartArea != null && chartArea.Area3DStyle.Enable3D)
			{
				if (this.AnchorDataPoint != null)
				{
					bool flag = true;
					bool flag2 = true;
					this.GetAnchorLocation(ref num7, ref num8, ref flag, ref flag2);
					this.currentAnchorLocationRel = new PointF((float)num7, (float)num8);
				}
				this.AnchorDataPoint = null;
				this.AxisX = null;
				this.AxisY = null;
				axis2 = null;
				axis = null;
			}
			if (axis2 != null)
			{
				num = axis2.PositionToValue(num, false);
				if (!double.IsNaN(num7))
				{
					num7 = axis2.PositionToValue(num7, false);
				}
				if (axis2.Logarithmic)
				{
					num = Math.Pow(axis2.logarithmBase, num);
					if (!double.IsNaN(num7))
					{
						num7 = Math.Pow(axis2.logarithmBase, num7);
					}
				}
				if (!this.SizeAlwaysRelative)
				{
					if (float.IsNaN(position.Right) && !float.IsNaN(position.Width) && !float.IsNaN(anchorPoint.X))
					{
						num3 = axis2.PositionToValue((double)(anchorPoint.X + position.Width), false);
						if (axis2.Logarithmic)
						{
							num3 = Math.Pow(axis2.logarithmBase, num3);
						}
						num5 = num3 - num7;
					}
					else
					{
						num3 = axis2.PositionToValue((double)position.Right, false);
						if (axis2.Logarithmic)
						{
							num3 = Math.Pow(axis2.logarithmBase, num3);
						}
						num5 = num3 - num;
					}
				}
			}
			if (axis != null)
			{
				num2 = axis.PositionToValue(num2, false);
				if (!double.IsNaN(num8))
				{
					num8 = axis.PositionToValue(num8, false);
				}
				if (axis.Logarithmic)
				{
					num2 = Math.Pow(axis.logarithmBase, num2);
					if (!double.IsNaN(num8))
					{
						num8 = Math.Pow(axis.logarithmBase, num8);
					}
				}
				if (!this.SizeAlwaysRelative)
				{
					if (float.IsNaN(position.Bottom) && !float.IsNaN(position.Height) && !float.IsNaN(anchorPoint.Y))
					{
						num4 = axis.PositionToValue((double)(anchorPoint.Y + position.Height), false);
						if (axis.Logarithmic)
						{
							num4 = Math.Pow(axis.logarithmBase, num4);
						}
						num6 = num4 - num8;
					}
					else
					{
						num4 = axis.PositionToValue((double)position.Bottom, false);
						if (axis.Logarithmic)
						{
							num4 = Math.Pow(axis.logarithmBase, num4);
						}
						num6 = num4 - num2;
					}
				}
			}
			this.X = num;
			this.Y = num2;
			this.Width = num5;
			this.Height = num6;
			this.AnchorX = num7;
			this.AnchorY = num8;
			this.Invalidate();
		}

		internal virtual void AdjustLocationSize(SizeF movingDistance, ResizingMode resizeMode)
		{
			this.AdjustLocationSize(movingDistance, resizeMode, true);
		}

		internal virtual void AdjustLocationSize(SizeF movingDistance, ResizingMode resizeMode, bool pixelCoord)
		{
			this.AdjustLocationSize(movingDistance, resizeMode, pixelCoord, false);
		}

		internal virtual void AdjustLocationSize(SizeF movingDistance, ResizingMode resizeMode, bool pixelCoord, bool userInput)
		{
			if (!movingDistance.IsEmpty)
			{
				if (pixelCoord)
				{
					movingDistance = this.chart.chartPicture.chartGraph.GetRelativeSize(movingDistance);
				}
				PointF empty = PointF.Empty;
				PointF anchorPoint = PointF.Empty;
				SizeF empty2 = SizeF.Empty;
				if (userInput)
				{
					this.GetRelativePosition(out empty, out empty2, out anchorPoint);
				}
				else
				{
					this.GetRelativePosition(out empty, out empty2, out anchorPoint);
				}
				new PointF(empty.X + empty2.Width, empty.Y + empty2.Height);
				switch (resizeMode)
				{
				case ResizingMode.TopLeftHandle:
					empty.X -= movingDistance.Width;
					empty.Y -= movingDistance.Height;
					empty2.Width += movingDistance.Width;
					empty2.Height += movingDistance.Height;
					break;
				case ResizingMode.TopHandle:
					empty.Y -= movingDistance.Height;
					empty2.Height += movingDistance.Height;
					break;
				case ResizingMode.TopRightHandle:
					empty.Y -= movingDistance.Height;
					empty2.Width -= movingDistance.Width;
					empty2.Height += movingDistance.Height;
					break;
				case ResizingMode.RightHandle:
					empty2.Width -= movingDistance.Width;
					break;
				case ResizingMode.BottomRightHandle:
					empty2.Width -= movingDistance.Width;
					empty2.Height -= movingDistance.Height;
					break;
				case ResizingMode.BottomHandle:
					empty2.Height -= movingDistance.Height;
					break;
				case ResizingMode.BottomLeftHandle:
					empty.X -= movingDistance.Width;
					empty2.Width += movingDistance.Width;
					empty2.Height -= movingDistance.Height;
					break;
				case ResizingMode.LeftHandle:
					empty.X -= movingDistance.Width;
					empty2.Width += movingDistance.Width;
					break;
				case ResizingMode.AnchorHandle:
					anchorPoint.X -= movingDistance.Width;
					anchorPoint.Y -= movingDistance.Height;
					break;
				case ResizingMode.Moving:
					empty.X -= movingDistance.Width;
					empty.Y -= movingDistance.Height;
					break;
				}
				if (resizeMode == ResizingMode.Moving)
				{
					if (double.IsNaN(this.Width))
					{
						empty2.Width = float.NaN;
					}
					if (double.IsNaN(this.Height))
					{
						empty2.Height = float.NaN;
					}
				}
				if (resizeMode == ResizingMode.AnchorHandle)
				{
					if (double.IsNaN(this.X))
					{
						empty.X = float.NaN;
					}
					if (double.IsNaN(this.Y))
					{
						empty.Y = float.NaN;
					}
				}
				else if (double.IsNaN(this.AnchorX) || double.IsNaN(this.AnchorY))
				{
					anchorPoint = new PointF(float.NaN, float.NaN);
				}
				this.SetPositionRelative(new RectangleF(empty, empty2), anchorPoint, userInput);
			}
		}

		internal virtual bool IsAnchorDrawn()
		{
			return false;
		}

		internal DataPoint GetDataPointByName(string dataPointName)
		{
			DataPoint result = null;
			try
			{
				if (this.chart != null)
				{
					if (dataPointName.Length > 0)
					{
						int num = dataPointName.IndexOf("\\r", StringComparison.Ordinal);
						if (num > 0)
						{
							string parameter = dataPointName.Substring(0, num);
							string s = dataPointName.Substring(num + 2);
							result = this.chart.Series[parameter].Points[int.Parse(s, CultureInfo.InvariantCulture)];
							return result;
						}
						return result;
					}
					return result;
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		private Axis GetAxisByName(string axisName)
		{
			Axis result = null;
			try
			{
				if (this.chart != null)
				{
					if (axisName.Length > 0)
					{
						int num = axisName.IndexOf("\\r", StringComparison.Ordinal);
						if (num > 0)
						{
							string parameter = axisName.Substring(0, num);
							string value = axisName.Substring(num + 2);
							switch ((AxisName)Enum.Parse(typeof(AxisName), value))
							{
							case AxisName.X:
								result = this.chart.ChartAreas[parameter].AxisX;
								return result;
							case AxisName.Y:
								result = this.chart.ChartAreas[parameter].AxisY;
								return result;
							case AxisName.X2:
								result = this.chart.ChartAreas[parameter].AxisX2;
								return result;
							case AxisName.Y2:
								result = this.chart.ChartAreas[parameter].AxisY2;
								return result;
							default:
								return result;
							}
						}
						return result;
					}
					return result;
				}
				return result;
			}
			catch
			{
				return result;
			}
		}

		internal string GetDataPointName(DataPoint dataPoint)
		{
			string result = string.Empty;
			if (dataPoint.series != null)
			{
				int num = dataPoint.series.Points.IndexOf(dataPoint);
				if (num >= 0)
				{
					result = dataPoint.series.Name + "\\r" + num.ToString(CultureInfo.InvariantCulture);
				}
			}
			return result;
		}

		private string GetAxisName(Axis axis)
		{
			string result = string.Empty;
			if (axis.chartArea != null)
			{
				result = axis.chartArea.Name + "\\r" + axis.Type.ToString();
			}
			return result;
		}

		public virtual void SendToBack()
		{
			AnnotationCollection annotationCollection = null;
			if (this.chart != null)
			{
				annotationCollection = this.chart.Annotations;
			}
			AnnotationGroup annotationGroup = this.GetAnnotationGroup();
			if (annotationGroup != null)
			{
				annotationCollection = annotationGroup.Annotations;
			}
			if (annotationCollection != null)
			{
				Annotation annotation = annotationCollection.FindByName(this.Name);
				if (annotation != null)
				{
					annotationCollection.Remove(annotation);
					annotationCollection.Insert(0, annotation);
				}
			}
		}

		public virtual void BringToFront()
		{
			AnnotationCollection annotationCollection = null;
			if (this.chart != null)
			{
				annotationCollection = this.chart.Annotations;
			}
			AnnotationGroup annotationGroup = this.GetAnnotationGroup();
			if (annotationGroup != null)
			{
				annotationCollection = annotationGroup.Annotations;
			}
			if (annotationCollection != null)
			{
				Annotation annotation = annotationCollection.FindByName(this.Name);
				if (annotation != null)
				{
					annotationCollection.Remove(annotation);
					annotationCollection.Add(this);
				}
			}
		}

		public AnnotationGroup GetAnnotationGroup()
		{
			return this.annotationGroup;
		}

		internal void AddSmartLabelMarkerPositions(CommonElements common, ArrayList list)
		{
			if (this.Visible && this.IsAnchorDrawn())
			{
				Axis axis = null;
				Axis axis2 = null;
				this.GetAxes(ref axis, ref axis2);
				double num = double.NaN;
				double num2 = double.NaN;
				bool flag = false;
				bool flag2 = false;
				this.GetAnchorLocation(ref num, ref num2, ref flag, ref flag2);
				if (!double.IsNaN(num) && !double.IsNaN(num2))
				{
					if (!flag && axis2 != null)
					{
						num = axis2.ValueToPosition(num);
					}
					if (!flag2 && axis != null)
					{
						num2 = axis.ValueToPosition(num2);
					}
					ChartArea chartArea = null;
					if (axis2 != null && axis2.chartArea != null)
					{
						chartArea = axis2.chartArea;
					}
					if (axis != null && axis.chartArea != null)
					{
						chartArea = axis.chartArea;
					}
					if (chartArea != null && chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular && chartArea.requireAxes && chartArea.matrix3D.IsInitialized())
					{
						float num3 = chartArea.areaSceneDepth;
						if (this.AnchorDataPoint != null && this.AnchorDataPoint.series != null)
						{
							float num4 = 0f;
							((ChartArea3D)chartArea).GetSeriesZPositionAndDepth(this.AnchorDataPoint.series, out num4, out num3);
							num3 = (float)(num3 + num4 / 2.0);
						}
						Point3D[] array = new Point3D[1]
						{
							new Point3D((float)num, (float)num2, num3)
						};
						chartArea.matrix3D.TransformPoints(array);
						num = (double)array[0].X;
						num2 = (double)array[0].Y;
					}
					if (this.GetGraphics() != null)
					{
						SizeF relativeSize = this.GetGraphics().GetRelativeSize(new SizeF(1f, 1f));
						RectangleF rectangleF = new RectangleF((float)((float)num - relativeSize.Width / 2.0), (float)((float)num2 - relativeSize.Height / 2.0), relativeSize.Width, relativeSize.Height);
						list.Add(rectangleF);
					}
				}
			}
		}

		public void Anchor(DataPoint dataPoint)
		{
			this.Anchor(dataPoint, null);
		}

		public void Anchor(DataPoint dataPoint1, DataPoint dataPoint2)
		{
			this.X = double.NaN;
			this.Y = double.NaN;
			this.AnchorX = double.NaN;
			this.AnchorY = double.NaN;
			this.AnchorDataPoint = dataPoint1;
			Axis axis = null;
			Axis axis2 = null;
			this.GetAxes(ref axis, ref axis2);
			if (dataPoint2 != null && dataPoint1 != null)
			{
				this.anchorDataPoint2 = dataPoint2;
			}
			this.Invalidate();
		}

		internal bool IsVisible()
		{
			if (this.Visible)
			{
				if (this.Chart != null)
				{
					ChartArea chartArea = null;
					if (this.AnchorDataPoint != null && this.AnchorDataPoint.series != null && this.Chart.ChartAreas.IndexOf(this.AnchorDataPoint.series.ChartArea) >= 0)
					{
						chartArea = this.Chart.ChartAreas[this.AnchorDataPoint.series.ChartArea];
					}
					if (chartArea == null && this.anchorDataPoint2 != null && this.anchorDataPoint2.series != null && this.Chart.ChartAreas.IndexOf(this.anchorDataPoint2.series.ChartArea) >= 0)
					{
						chartArea = this.Chart.ChartAreas[this.anchorDataPoint2.series.ChartArea];
					}
					if (chartArea == null && this.AxisX != null)
					{
						chartArea = this.AxisX.chartArea;
					}
					if (chartArea == null && this.AxisY != null)
					{
						chartArea = this.AxisY.chartArea;
					}
					if (chartArea != null && !chartArea.Visible)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal void ResetCurrentRelativePosition()
		{
			this.currentPositionRel = new RectangleF(float.NaN, float.NaN, float.NaN, float.NaN);
			this.currentAnchorLocationRel = new PointF(float.NaN, float.NaN);
		}

		public void Delete()
		{
			if (this.Chart != null)
			{
				this.Chart.Annotations.Remove(this);
			}
		}

		internal string ReplaceKeywords(string strOriginal)
		{
			if (this.AnchorDataPoint != null)
			{
				return this.AnchorDataPoint.ReplaceKeywords(strOriginal);
			}
			return strOriginal;
		}

		internal bool IsAnchorVisible()
		{
			Axis axis = null;
			Axis axis2 = null;
			this.GetAxes(ref axis, ref axis2);
			bool flag = false;
			bool flag2 = false;
			double num = this.AnchorX;
			double num2 = this.AnchorY;
			this.GetAnchorLocation(ref num, ref num2, ref flag, ref flag2);
			if (!double.IsNaN(num) && !double.IsNaN(num2) && (this.AnchorDataPoint != null || this.AxisX != null || this.AxisY != null))
			{
				if (!flag && axis2 != null)
				{
					num = axis2.ValueToPosition(num);
				}
				if (!flag2 && axis != null)
				{
					num2 = axis.ValueToPosition(num2);
				}
				ChartArea chartArea = null;
				if (axis2 != null)
				{
					chartArea = axis2.chartArea;
				}
				if (chartArea == null && axis != null)
				{
					chartArea = axis.chartArea;
				}
				if (chartArea != null && chartArea.Area3DStyle.Enable3D && !chartArea.chartAreaIsCurcular && chartArea.requireAxes && chartArea.matrix3D.IsInitialized())
				{
					float num3 = chartArea.areaSceneDepth;
					if (this.AnchorDataPoint != null && this.AnchorDataPoint.series != null)
					{
						float num4 = 0f;
						((ChartArea3D)chartArea).GetSeriesZPositionAndDepth(this.AnchorDataPoint.series, out num4, out num3);
						num3 = (float)(num3 + num4 / 2.0);
					}
					Point3D[] array = new Point3D[1]
					{
						new Point3D((float)num, (float)num2, num3)
					};
					chartArea.matrix3D.TransformPoints(array);
					num = (double)array[0].X;
					num2 = (double)array[0].Y;
				}
				RectangleF rectangleF = chartArea.PlotAreaPosition.ToRectangleF();
				rectangleF.Inflate(1E-05f, 1E-05f);
				if (!rectangleF.Contains((float)num, (float)num2))
				{
					return false;
				}
			}
			return true;
		}

		internal ChartGraphics GetGraphics()
		{
			if (this.chart != null && this.chart.chartPicture != null && this.chart.chartPicture.common != null)
			{
				return this.chart.chartPicture.common.graph;
			}
			return null;
		}

		private Axis GetDataPointAxis(DataPoint dataPoint, AxisName axisName)
		{
			if (dataPoint != null && dataPoint.series != null && this.chart != null)
			{
				try
				{
					ChartArea chartArea = this.chart.ChartAreas[dataPoint.series.ChartArea];
					if ((axisName == AxisName.X || axisName == AxisName.X2) && !chartArea.switchValueAxes)
					{
						return chartArea.GetAxis(axisName, dataPoint.series.XAxisType, dataPoint.series.XSubAxisName);
					}
					return chartArea.GetAxis(axisName, dataPoint.series.YAxisType, dataPoint.series.YSubAxisName);
				}
				catch
				{
				}
			}
			return null;
		}

		internal void GetAxes(ref Axis vertAxis, ref Axis horizAxis)
		{
			vertAxis = null;
			horizAxis = null;
			if (this.AxisX != null && this.AxisX.chartArea != null)
			{
				if (this.AxisX.chartArea.switchValueAxes)
				{
					vertAxis = this.AxisX;
				}
				else
				{
					horizAxis = this.AxisX;
				}
			}
			if (this.AxisY != null && this.AxisY.chartArea != null)
			{
				if (this.AxisY.chartArea.switchValueAxes)
				{
					horizAxis = this.AxisY;
				}
				else
				{
					vertAxis = this.AxisY;
				}
			}
			if (this.AnchorDataPoint != null)
			{
				if (horizAxis == null)
				{
					horizAxis = this.GetDataPointAxis(this.AnchorDataPoint, AxisName.X);
					if (horizAxis != null && horizAxis.chartArea != null && horizAxis.chartArea.switchValueAxes)
					{
						horizAxis = this.GetDataPointAxis(this.AnchorDataPoint, AxisName.Y);
					}
				}
				if (vertAxis == null)
				{
					vertAxis = this.GetDataPointAxis(this.AnchorDataPoint, AxisName.Y);
					if (vertAxis != null && vertAxis.chartArea != null && vertAxis.chartArea.switchValueAxes)
					{
						vertAxis = this.GetDataPointAxis(this.AnchorDataPoint, AxisName.X);
					}
				}
			}
			if (vertAxis == null && horizAxis == null)
			{
				return;
			}
			if (this.GetAnnotationGroup() == null)
			{
				return;
			}
			throw new InvalidOperationException(SR.ExceptionAnnotationGroupedAxisMustBeEmpty);
		}

		internal void Invalidate()
		{
		}
	}
}
