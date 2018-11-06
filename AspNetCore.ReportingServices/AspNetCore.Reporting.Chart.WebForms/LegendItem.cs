using AspNetCore.Reporting.Chart.WebForms.ChartTypes;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendItem_LegendItem")]
	[DefaultProperty("Name")]
	internal class LegendItem : IMapAreaAttributes
	{
		private string name = "Legend Item";

		private Color color = Color.Empty;

		private string image = "";

		private string seriesName = "";

		private int seriesPointIndex = -1;

		private string toolTip = "";

		private string href = "";

		private object mapAreaTag;

		private string attributes = "";

		internal LegendImageStyle style;

		internal GradientType backGradientType;

		internal Color backGradientEndColor = Color.Empty;

		internal Color backImageTranspColor = Color.Empty;

		internal Color borderColor = Color.Black;

		internal int borderWidth = 1;

		internal ChartDashStyle borderStyle = ChartDashStyle.Solid;

		internal ChartHatchStyle backHatchStyle;

		internal int shadowOffset;

		internal Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		internal string backImage = "";

		internal ChartImageWrapMode backImageMode;

		internal ChartImageAlign backImageAlign;

		internal MarkerStyle markerStyle;

		internal int markerSize = 5;

		internal string markerImage = "";

		internal Color markerImageTranspColor = Color.Empty;

		internal Color markerColor = Color.Empty;

		internal Color markerBorderColor = Color.Empty;

		internal CommonElements common;

		private Legend m_legend;

		private bool enabled = true;

		private int markerBorderWidth = 1;

		private LegendCellCollection cells;

		private LegendSeparatorType separator;

		private Color separatorColor = Color.Black;

		private object tag;

		internal bool clearTempCells;

		[Browsable(false)]
		[Bindable(false)]
		public Legend Legend
		{
			get
			{
				return this.m_legend;
			}
			set
			{
				this.m_legend = value;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegendItem_Name")]
		[NotifyParentProperty(true)]
		[ParenthesizePropertyName(true)]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				this.Invalidate(false);
			}
		}

		[NotifyParentProperty(true)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_Color")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeAppearance")]
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				this.Invalidate(true);
			}
		}

		[NotifyParentProperty(true)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_Image")]
		[DefaultValue("")]
		[SRCategory("CategoryAttributeAppearance")]
		public string Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
				this.Invalidate(false);
			}
		}

		[DefaultValue(typeof(LegendImageStyle), "Rectangle")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegendItem_Style")]
		[ParenthesizePropertyName(true)]
		public LegendImageStyle Style
		{
			get
			{
				return this.style;
			}
			set
			{
				this.style = value;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegendItem_BorderColor")]
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

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartHatchStyle.None)]
		[SRDescription("DescriptionAttributeLegendItem_BackHatchStyle")]
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
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegendItem_BackImageTransparentColor")]
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

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeLegendItem_BackGradientType")]
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

		[DefaultValue(typeof(Color), "")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegendItem_BackGradientEndColor")]
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

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLegendItem_BorderWidth")]
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
					throw new ArgumentOutOfRangeException("value", SR.ExceptionBorderWidthIsZero);
				}
				this.borderWidth = value;
				this.Invalidate(false);
			}
		}

		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLegendItem_Enabled")]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeLegendItem_MarkerBorderWidth")]
		public int MarkerBorderWidth
		{
			get
			{
				return this.markerBorderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionLegendMarkerBorderWidthIsNegative);
				}
				this.markerBorderWidth = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLegendItem_BorderStyle")]
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

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_ShadowOffset")]
		[DefaultValue(0)]
		public int ShadowOffset
		{
			get
			{
				return this.shadowOffset;
			}
			set
			{
				this.shadowOffset = value;
				this.Invalidate(false);
			}
		}

		[DefaultValue(typeof(Color), "128,0,0,0")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegendItem_ShadowColor")]
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

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[DefaultValue(MarkerStyle.None)]
		[SRDescription("DescriptionAttributeLegendItem_MarkerStyle")]
		[RefreshProperties(RefreshProperties.All)]
		public MarkerStyle MarkerStyle
		{
			get
			{
				return this.markerStyle;
			}
			set
			{
				this.markerStyle = value;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[DefaultValue(5)]
		[SRDescription("DescriptionAttributeLegendItem_MarkerSize")]
		[RefreshProperties(RefreshProperties.All)]
		public int MarkerSize
		{
			get
			{
				return this.markerSize;
			}
			set
			{
				this.markerSize = value;
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerImage")]
		[RefreshProperties(RefreshProperties.All)]
		public string MarkerImage
		{
			get
			{
				return this.markerImage;
			}
			set
			{
				this.markerImage = value;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerImageTransparentColor")]
		[RefreshProperties(RefreshProperties.All)]
		public Color MarkerImageTransparentColor
		{
			get
			{
				return this.markerImageTranspColor;
			}
			set
			{
				this.markerImageTranspColor = value;
				this.Invalidate(true);
			}
		}

		[DefaultValue(typeof(Color), "")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeMarker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerColor")]
		[RefreshProperties(RefreshProperties.All)]
		public Color MarkerColor
		{
			get
			{
				return this.markerColor;
			}
			set
			{
				this.markerColor = value;
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerBorderColor")]
		[RefreshProperties(RefreshProperties.All)]
		public Color MarkerBorderColor
		{
			get
			{
				return this.markerBorderColor;
			}
			set
			{
				this.markerBorderColor = value;
				this.Invalidate(true);
			}
		}

		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegendItem_SeriesName")]
		[Browsable(false)]
		public string SeriesName
		{
			get
			{
				return this.seriesName;
			}
			set
			{
				this.seriesName = value;
			}
		}

		[SRDescription("DescriptionAttributeLegendItem_SeriesPointIndex")]
		[Browsable(false)]
		[DefaultValue(-1)]
		public int SeriesPointIndex
		{
			get
			{
				return this.seriesPointIndex;
			}
			set
			{
				this.seriesPointIndex = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(LegendSeparatorType), "None")]
		[SRDescription("DescriptionAttributeLegendItem_Separator")]
		public LegendSeparatorType Separator
		{
			get
			{
				return this.separator;
			}
			set
			{
				if (value != this.separator)
				{
					this.separator = value;
					this.Invalidate(false);
				}
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegendItem_SeparatorColor")]
		public Color SeparatorColor
		{
			get
			{
				return this.separatorColor;
			}
			set
			{
				if (value != this.separatorColor)
				{
					this.separatorColor = value;
					this.Invalidate(false);
				}
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeLegendItem_Cells")]
		public LegendCellCollection Cells
		{
			get
			{
				return this.cells;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Browsable(false)]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeLegendItem_Tag")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public object Tag
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

		[Bindable(true)]
		[SRCategory("CategoryAttributeMapArea")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeToolTip7")]
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
		[Bindable(true)]
		[SRCategory("CategoryAttributeMapArea")]
		[DefaultValue("")]
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

		[Bindable(true)]
		[SRCategory("CategoryAttributeMapArea")]
		[SRDescription("DescriptionAttributeMapAreaAttributes9")]
		[DefaultValue("")]
		public string MapAreaAttributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = value;
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

		public LegendItem()
		{
			this.cells = new LegendCellCollection(this);
		}

		public LegendItem(string name, Color color, string image)
		{
			this.name = name;
			this.color = color;
			this.image = image;
			this.cells = new LegendCellCollection(this);
		}

		internal void AddAutomaticCells(Legend legend)
		{
			if (this.Cells.Count == 0)
			{
				if (this.SeriesName.Length > 0)
				{
					if (legend.CellColumns.Count == 0)
					{
						if (legend.Common != null && legend.Common.ChartPicture.IsRightToLeft())
						{
							this.Cells.Add(LegendCellType.Text, "#LEGENDTEXT", ContentAlignment.MiddleLeft);
							this.Cells.Add(LegendCellType.SeriesSymbol, string.Empty, ContentAlignment.MiddleCenter);
						}
						else
						{
							this.Cells.Add(LegendCellType.SeriesSymbol, string.Empty, ContentAlignment.MiddleCenter);
							this.Cells.Add(LegendCellType.Text, "#LEGENDTEXT", ContentAlignment.MiddleLeft);
						}
					}
					else
					{
						foreach (LegendCellColumn cellColumn in legend.CellColumns)
						{
							this.Cells.Add(cellColumn.CreateNewCell());
						}
					}
				}
				else
				{
					this.clearTempCells = true;
					this.Cells.Add(LegendCellType.SeriesSymbol, string.Empty, ContentAlignment.MiddleCenter);
					this.Cells.Add(LegendCellType.Text, "#LEGENDTEXT", ContentAlignment.MiddleLeft);
				}
			}
		}

		internal void SetAttributes(CommonElements common, Series series)
		{
			IChartType chartType = common.ChartTypeRegistry.GetChartType(series.ChartTypeName);
			this.style = chartType.GetLegendImageStyle(series);
			this.seriesName = series.Name;
			this.shadowOffset = series.ShadowOffset;
			this.shadowColor = series.ShadowColor;
			bool enable3D = common.Chart.ChartAreas[series.ChartArea].Area3DStyle.Enable3D;
			this.SetAttributes(series, enable3D);
		}

		internal void SetAttributes(DataPointAttributes attrib, bool area3D)
		{
			this.borderColor = attrib.BorderColor;
			this.borderWidth = attrib.BorderWidth;
			this.borderStyle = attrib.BorderStyle;
			this.markerStyle = attrib.MarkerStyle;
			this.markerSize = attrib.MarkerSize;
			this.markerImage = attrib.MarkerImage;
			this.markerImageTranspColor = attrib.MarkerImageTransparentColor;
			this.markerColor = attrib.MarkerColor;
			this.markerBorderColor = attrib.MarkerBorderColor;
			this.markerBorderWidth = attrib.MarkerBorderWidth;
			float num = 96f;
			if (this.common != null)
			{
				num = this.common.graph.Graphics.DpiX;
			}
			int num2 = (int)Math.Round(2.0 * num / 96.0);
			if (this.markerBorderWidth > num2)
			{
				this.markerBorderWidth = num2;
			}
			if (attrib.MarkerBorderWidth <= 0)
			{
				this.markerBorderColor = Color.Transparent;
			}
			if (this.style == LegendImageStyle.Line && this.borderWidth <= (int)Math.Round(num / 96.0))
			{
				this.borderWidth = num2;
			}
			if (!area3D)
			{
				this.backGradientType = attrib.BackGradientType;
				this.backGradientEndColor = attrib.BackGradientEndColor;
				this.backImageTranspColor = attrib.BackImageTransparentColor;
				this.backImage = attrib.BackImage;
				this.backImageMode = attrib.BackImageMode;
				this.backImageAlign = attrib.BackImageAlign;
				this.backHatchStyle = attrib.BackHatchStyle;
			}
		}

		private void Invalidate(bool invalidateLegendOnly)
		{
		}
	}
}
