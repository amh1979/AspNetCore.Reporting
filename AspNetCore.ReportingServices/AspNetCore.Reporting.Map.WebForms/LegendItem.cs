using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace AspNetCore.Reporting.Map.WebForms
{
	[DefaultProperty("Name")]
	[Description("Map legend item.")]
	internal class LegendItem : NamedElement
	{
		private Color color = Color.Empty;

		private string image = "";

		private string seriesName = "";

		private int seriesPointIndex = -1;

		private string toolTip = "";

		private string href = "";

		private string attributes = "";

		internal LegendItemStyle itemStyle;

		internal Color borderColor = Color.DarkGray;

		internal int borderWidth = 1;

		internal int pathWidth = 3;

		internal MapDashStyle pathLineStyle = MapDashStyle.Solid;

		internal MapDashStyle borderStyle = MapDashStyle.Solid;

		internal int shadowOffset;

		internal Color shadowColor = Color.FromArgb(128, 0, 0, 0);

		internal MarkerStyle markerStyle = MarkerStyle.Circle;

		internal string markerImage = "";

		internal Color markerImageTranspColor = Color.Empty;

		internal Color markerColor = Color.Empty;

		internal Color markerBorderColor = Color.Empty;

		[Bindable(false)]
		[Browsable(false)]
		public Legend Legend;

		private int markerBorderWidth = 1;

		private LegendCellCollection cells;

		private LegendSeparatorType separator;

		private Color separatorColor = Color.Black;

		internal bool clearTempCells;

		private string text = "";

		private MapHatchStyle hatchStyle;

		internal Color imageTranspColor = Color.Empty;

		private GradientType gradientType;

		private Color secondaryColor = Color.Empty;

		private MapImageAlign imageAlign;

		private MapImageWrapMode imageWrapMode;

		private bool visible = true;

		private float markerWidth = 7f;

		private float markerHeight = 7f;

		private MapDashStyle markerBorderStyle = MapDashStyle.Solid;

		private GradientType markerGradientType;

		private Color markerSecondaryColor = Color.Empty;

		private MapHatchStyle markerHatchStyle;

		internal bool automaticallyAdded;

		[SRDescription("DescriptionAttributeLegendItem_Text")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		public string Text
		{
			get
			{
				if (string.IsNullOrEmpty(this.text))
				{
					return base.Name;
				}
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		[Browsable(true)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeLegendItem_Name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[SRCategory("CategoryAttribute_Background")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_Color")]
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Background")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_Image")]
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		public string Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegendItem_ItemStyle")]
		[DefaultValue(typeof(LegendItemStyle), "Shape")]
		public LegendItemStyle ItemStyle
		{
			get
			{
				return this.itemStyle;
			}
			set
			{
				this.itemStyle = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeLegendItem_BorderColor")]
		[Bindable(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "DarkGray")]
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

		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Background")]
		[DefaultValue(MapHatchStyle.None)]
		[SRDescription("DescriptionAttributeLegendItem_HatchStyle")]
		public MapHatchStyle HatchStyle
		{
			get
			{
				return this.hatchStyle;
			}
			set
			{
				this.hatchStyle = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_ImageTranspColor")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Background")]
		public Color ImageTranspColor
		{
			get
			{
				return this.imageTranspColor;
			}
			set
			{
				this.imageTranspColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(GradientType.None)]
		[SRCategory("CategoryAttribute_Background")]
		[SRDescription("DescriptionAttributeLegendItem_GradientType")]
		[NotifyParentProperty(true)]
		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[Bindable(true)]
		public GradientType GradientType
		{
			get
			{
				return this.gradientType;
			}
			set
			{
				this.gradientType = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Background")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_SecondaryColor")]
		[NotifyParentProperty(true)]
		public Color SecondaryColor
		{
			get
			{
				return this.secondaryColor;
			}
			set
			{
				if (value != Color.Empty && (value.A != 255 || value == Color.Transparent))
				{
					throw new ArgumentException(SR.ExceptionCollorCannotBeTransparent);
				}
				this.secondaryColor = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttribute_Background")]
		[DefaultValue(MapImageAlign.TopLeft)]
		[SRDescription("DescriptionAttributeLegendItem_ImageAlign")]
		[NotifyParentProperty(true)]
		public MapImageAlign ImageAlign
		{
			get
			{
				return this.imageAlign;
			}
			set
			{
				this.imageAlign = value;
				this.Invalidate();
			}
		}

		[DefaultValue(MapImageWrapMode.Tile)]
		[SRDescription("DescriptionAttributeLegendItem_ImageWrapMode")]
		[SRCategory("CategoryAttribute_Background")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		public MapImageWrapMode ImageWrapMode
		{
			get
			{
				return this.imageWrapMode;
			}
			set
			{
				this.imageWrapMode = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
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
					throw new ArgumentOutOfRangeException("BorderWidth", SR.ExceptionBorderWidthMustBeGreaterThanZero);
				}
				this.borderWidth = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(3)]
		[SRDescription("DescriptionAttributeLegendItem_PathWidth")]
		[NotifyParentProperty(true)]
		public int PathWidth
		{
			get
			{
				return this.pathWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("PathWidth", SR.ExceptionPathWidthMustBeGreaterThanZero);
				}
				this.pathWidth = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[DefaultValue(MapDashStyle.Solid)]
		[SRDescription("DescriptionAttributeLegendItem_PathLineStyle")]
		[NotifyParentProperty(true)]
		public MapDashStyle PathLineStyle
		{
			get
			{
				return this.pathLineStyle;
			}
			set
			{
				this.pathLineStyle = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLegendItem_Visible")]
		[NotifyParentProperty(true)]
		public bool Visible
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

		[SRCategory("CategoryAttribute_Marker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerBorderWidth")]
		[DefaultValue(1)]
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
					throw new ArgumentOutOfRangeException("MarkerBorderWidth", SR.ExceptionMarkerBorderMustBeGreaterThanZero);
				}
				this.markerBorderWidth = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(MapDashStyle.Solid)]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_BorderStyle")]
		public MapDashStyle BorderStyle
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
		[SRDescription("DescriptionAttributeLegendItem_ShadowOffset")]
		[NotifyParentProperty(true)]
		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_Appearance")]
		public int ShadowOffset
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

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "128,0,0,0")]
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
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(MarkerStyle.Circle)]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeLegendItem_MarkerStyle")]
		[SRCategory("CategoryAttribute_Marker")]
		[Bindable(true)]
		public MarkerStyle MarkerStyle
		{
			get
			{
				return this.markerStyle;
			}
			set
			{
				this.markerStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLegendItem_MarkerWidth")]
		[SRCategory("CategoryAttribute_Marker")]
		[DefaultValue(7f)]
		public float MarkerWidth
		{
			get
			{
				return this.markerWidth;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					this.markerWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
			}
		}

		[SRDescription("DescriptionAttributeLegendItem_MarkerHeight")]
		[SRCategory("CategoryAttribute_Marker")]
		[DefaultValue(7f)]
		public float MarkerHeight
		{
			get
			{
				return this.markerHeight;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					this.markerHeight = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.must_in_range(0.0, 1000.0));
			}
		}

		[SRDescription("DescriptionAttributeLegendItem_MarkerImage")]
		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Marker")]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		public string MarkerImage
		{
			get
			{
				return this.markerImage;
			}
			set
			{
				this.markerImage = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerImageTranspColor")]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		public Color MarkerImageTranspColor
		{
			get
			{
				return this.markerImageTranspColor;
			}
			set
			{
				this.markerImageTranspColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLegendItem_MarkerColor")]
		[DefaultValue(typeof(Color), "")]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		[SRCategory("CategoryAttribute_Marker")]
		public Color MarkerColor
		{
			get
			{
				return this.markerColor;
			}
			set
			{
				this.markerColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "")]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute_Marker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerBorderColor")]
		[Bindable(true)]
		public Color MarkerBorderColor
		{
			get
			{
				return this.markerBorderColor;
			}
			set
			{
				this.markerBorderColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerBorderStyle")]
		[DefaultValue(MapDashStyle.Solid)]
		public MapDashStyle MarkerBorderStyle
		{
			get
			{
				return this.markerBorderStyle;
			}
			set
			{
				this.markerBorderStyle = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Marker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerGradientType")]
		//[Editor(typeof(GradientEditor), typeof(UITypeEditor))]
		[DefaultValue(GradientType.None)]
		public GradientType MarkerGradientType
		{
			get
			{
				return this.markerGradientType;
			}
			set
			{
				this.markerGradientType = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLegendItem_MarkerSecondaryColor")]
		[SRCategory("CategoryAttribute_Marker")]
		[DefaultValue(typeof(Color), "")]
		public Color MarkerSecondaryColor
		{
			get
			{
				return this.markerSecondaryColor;
			}
			set
			{
				this.markerSecondaryColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(MapHatchStyle.None)]
		[SRCategory("CategoryAttribute_Marker")]
		[SRDescription("DescriptionAttributeLegendItem_MarkerHatchStyle")]
		//[Editor(typeof(HatchStyleEditor), typeof(UITypeEditor))]
		public MapHatchStyle MarkerHatchStyle
		{
			get
			{
				return this.markerHatchStyle;
			}
			set
			{
				this.markerHatchStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLegendItem_Separator")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(LegendSeparatorType), "None")]
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
					this.Invalidate();
				}
			}
		}

		[SRDescription("DescriptionAttributeLegendItem_SeparatorColor")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "Black")]
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
					this.Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLegendItem_Cells")]
		public LegendCellCollection Cells
		{
			get
			{
				return this.cells;
			}
		}

		[SRDescription("DescriptionAttributeLegendItem_AutomaticallyAdded")]
		[DefaultValue(false)]
		[SRCategory("CategoryAttribute_Misc")]
		public bool AutomaticallyAdded
		{
			get
			{
				return this.automaticallyAdded;
			}
			set
			{
				this.automaticallyAdded = value;
			}
		}

		[Browsable(false)]
		[SRDescription("DescriptionAttributeLegendItem_SeriesName")]
		[DefaultValue("")]
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

		[DefaultValue(-1)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeLegendItem_SeriesPointIndex")]
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

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_Behavior")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_ToolTip")]
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

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_MapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegendItem_Href")]
		[NotifyParentProperty(true)]
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

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_MapArea")]
		[SRDescription("DescriptionAttributeLegendItem_MapAreaAttributes")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
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

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				if (this.Cells != null)
				{
					this.Cells.Common = value;
				}
			}
		}

		public LegendItem()
			: base(null)
		{
			this.cells = new LegendCellCollection(this, this, this.Common);
		}

		public LegendItem(string name, Color color, string image)
			: this()
		{
			this.Name = name;
			this.color = color;
			this.image = image;
		}

		protected bool ShouldSerializeText()
		{
			return !string.IsNullOrEmpty(this.text);
		}

		internal void AddAutomaticCells(Legend legend, SizeF singleWCharacterSize)
		{
			if (this.Cells.Count == 0)
			{
				this.clearTempCells = true;
				int num = this.Cells.Add(LegendCellType.Symbol, string.Empty, ContentAlignment.MiddleCenter);
				LegendCell legendCell = this.Cells[num];
				if (this.ItemStyle == LegendItemStyle.Symbol)
				{
					if (Symbol.IsXamlMarker(this.MarkerStyle))
					{
						RectangleF rectangleF = Symbol.CalculateXamlMarkerBounds(this.MarkerStyle, PointF.Empty, this.MarkerWidth, this.MarkerHeight);
						Size empty = Size.Empty;
						empty.Width = (int)Math.Round(100.0 * rectangleF.Width / singleWCharacterSize.Width);
						empty.Height = (int)Math.Round(100.0 * rectangleF.Height / singleWCharacterSize.Height);
						legendCell.SymbolSize = empty;
					}
					else
					{
						int width = (int)Math.Round(100.0 * this.MarkerWidth / singleWCharacterSize.Width);
						int height = (int)Math.Round(100.0 * this.MarkerHeight / singleWCharacterSize.Height);
						legendCell.SymbolSize = new Size(width, height);
					}
				}
				else if (this.ItemStyle == LegendItemStyle.Path)
				{
					float num2 = (float)(100.0 * (float)this.PathWidth / singleWCharacterSize.Height);
					float num3 = (float)(100.0 * (float)this.BorderWidth / singleWCharacterSize.Height);
					float num4 = 9.1f;
					if (this.PathLineStyle == MapDashStyle.Dash)
					{
						num4 = 8.9f;
					}
					else if (this.PathLineStyle == MapDashStyle.DashDot)
					{
						num4 = 11.5f;
					}
					else if (this.PathLineStyle == MapDashStyle.DashDotDot)
					{
						num4 = 9.1f;
					}
					else if (this.PathLineStyle == MapDashStyle.Dot)
					{
						num4 = 9.1f;
					}
					if (this.PathWidth == 1)
					{
						num4 = (float)(num4 * 3.0);
					}
					else if (this.PathWidth == 2)
					{
						num4 = (float)(num4 * 2.0);
					}
					else if (this.PathWidth == 3)
					{
						num4 = (float)(num4 * 1.5);
					}
					int width2 = (int)Math.Round((double)(num2 * num4));
					legendCell.SymbolSize = new Size(width2, (int)Math.Round(num2 + num3 * 2.0));
				}
				if (num < legend.CellColumns.Count)
				{
					LegendCellColumn legendCellColumn = legend.CellColumns[num];
					legendCell.ToolTip = legendCellColumn.ToolTip;
					legendCell.Href = legendCellColumn.Href;
					legendCell.CellAttributes = legendCellColumn.CellColumnAttributes;
				}
				num = this.Cells.Add(LegendCellType.Text, "#LEGENDTEXT", ContentAlignment.MiddleLeft);
				legendCell = this.Cells[num];
				if (num < legend.CellColumns.Count)
				{
					LegendCellColumn legendCellColumn2 = legend.CellColumns[num];
					legendCell.ToolTip = legendCellColumn2.ToolTip;
					legendCell.Href = legendCellColumn2.Href;
					legendCell.CellAttributes = legendCellColumn2.CellColumnAttributes;
				}
			}
		}

		internal SizeF MeasureLegendItem(MapGraphics graph, int fontSizeReducedBy)
		{
			SizeF empty = SizeF.Empty;
			int num = 0;
			foreach (LegendCell cell in this.Cells)
			{
				if (num > 0)
				{
					num--;
				}
				else
				{
					SizeF sizeF = cell.MeasureCell(graph, fontSizeReducedBy, this.Legend.autofitFont, this.Legend.singleWCharacterSize);
					if (cell.CellSpan > 1)
					{
						num = cell.CellSpan - 1;
					}
					empty.Width += sizeF.Width;
					empty.Height = Math.Max(empty.Height, sizeF.Height);
				}
			}
			return empty;
		}

		internal string ReplaceKeywords(string strOriginal)
		{
			if (strOriginal.Length == 0)
			{
				return strOriginal;
			}
			return strOriginal.Replace("#LEGENDTEXT", this.Text);
		}

		internal override void Invalidate()
		{
			if (this.Legend != null)
			{
				this.Legend.Invalidate();
			}
		}
	}
}
