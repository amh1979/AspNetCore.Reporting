using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	[Description("Represents a column in the map legend.")]
	internal class LegendCellColumn : NamedElement, IToolTipProvider, IImageMapProvider
	{
		private Legend legend;

		private LegendCellColumnType columnType;

		private string text = "#LEGENDTEXT";

		private Color textColor = Color.Empty;

		private Color backColor = Color.Empty;

		private Font font;

		private Size seriesSymbolSize = new Size(200, 70);

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private Margins margins = new Margins(0, 0, 15, 15);

		private string headerText = string.Empty;

		private StringAlignment headerTextAlignment = StringAlignment.Center;

		private Color headerColor = Color.Black;

		private Color headerBackColor = Color.Empty;

		private int minimumCellWidth = -1;

		private int maximumCellWidth = -1;

		private string toolTip = string.Empty;

		private string href = string.Empty;

		private string cellColumnAttributes = string.Empty;

		private Font headerFont = new Font("Microsoft Sans Serif", 8f);

		private object mapAreaTag;

		[ParenthesizePropertyName(true)]
		[Description("Legend column type of the items automatically generated.")]
		[Category("Series Items")]
		[DefaultValue(LegendCellColumnType.Text)]
		internal virtual LegendCellColumnType ColumnType
		{
			get
			{
				return this.columnType;
			}
			set
			{
				this.columnType = value;
				this.Invalidate();
			}
		}

		[Category("Series Items")]
		[DefaultValue("#LEGENDTEXT")]
		[Description("Legend column text of the items automatically generated. Set ColumnType to Text to use this property.")]
		internal virtual string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				this.Invalidate();
			}
		}

		[Description("Legend column text color.")]
		[Category("Series Items")]
		[DefaultValue(typeof(Color), "")]
		internal virtual Color TextColor
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

		[Category("Series Items")]
		[DefaultValue(typeof(Color), "")]
		[Description("Legend column back color.")]
		internal virtual Color BackColor
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

		[Description("Legend column text font.")]
		[Category("Series Items")]
		[DefaultValue(null)]
		internal virtual Font Font
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

		[DefaultValue(typeof(Size), "200, 70")]
		[Category("Series Items")]
		[Description("Legend column symbol size (as a percentage of legend font size). This is only applicable to items that are automatically generated.")]
		internal virtual Size SeriesSymbolSize
		{
			get
			{
				return this.seriesSymbolSize;
			}
			set
			{
				if (value.Width >= 0 && value.Height >= 0)
				{
					this.seriesSymbolSize = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException("Column symbol width and height cannot be a negative number.", "SeriesSymbolSize");
			}
		}

		[Description("Legend column content alignment of the items automatically generated.")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[Category("Series Items")]
		internal virtual ContentAlignment Alignment
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

		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Margins), "0,0,15,15")]
		[Description("Legend column margins (as a percentage of legend font size).  This is only applicable to items that are automatically generated.")]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Series Items")]
		internal virtual Margins Margins
		{
			get
			{
				return this.margins;
			}
			set
			{
				this.margins = value;
				this.Invalidate();
				if (this.GetLegend() != null)
				{
					this.margins.Common = this.GetLegend().Common;
				}
			}
		}

		[Browsable(false)]
		[DefaultValue("")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLegendCellColumn_ToolTip")]
		public virtual string ToolTip
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
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLegendCellColumn_Href")]
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

		[SRDescription("DescriptionAttributeLegendCellColumn_CellColumnAttributes")]
		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		public virtual string CellColumnAttributes
		{
			get
			{
				return this.cellColumnAttributes;
			}
			set
			{
				this.cellColumnAttributes = value;
			}
		}

		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderText")]
		[SRCategory("CategoryAttribute_Header")]
		public virtual string HeaderText
		{
			get
			{
				return this.headerText;
			}
			set
			{
				this.headerText = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Header")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderColor")]
		[DefaultValue(typeof(Color), "Black")]
		public virtual Color HeaderColor
		{
			get
			{
				return this.headerColor;
			}
			set
			{
				this.headerColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderBackColor")]
		[SRCategory("CategoryAttribute_Header")]
		public virtual Color HeaderBackColor
		{
			get
			{
				return this.headerBackColor;
			}
			set
			{
				this.headerBackColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderFont")]
		[SRCategory("CategoryAttribute_Header")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt, style=Bold")]
		public virtual Font HeaderFont
		{
			get
			{
				return this.headerFont;
			}
			set
			{
				this.headerFont = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderTextAlignment")]
		[DefaultValue(typeof(StringAlignment), "Center")]
		[SRCategory("CategoryAttribute_Header")]
		public StringAlignment HeaderTextAlignment
		{
			get
			{
				return this.headerTextAlignment;
			}
			set
			{
				if (value != this.headerTextAlignment)
				{
					this.headerTextAlignment = value;
					this.Invalidate();
				}
			}
		}

		[SRDescription("DescriptionAttributeLegendCellColumn_MinimumWidth")]
		[DefaultValue(-1)]
		[SRCategory("CategoryAttribute_Size")]
		[TypeConverter(typeof(IntNanValueConverter))]
		public virtual int MinimumWidth
		{
			get
			{
				return this.minimumCellWidth;
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentException("Column minimum width cannot be less than -1.", "MinimumCellWidth");
				}
				this.minimumCellWidth = value;
				this.Invalidate();
			}
		}

		[DefaultValue(-1)]
		[TypeConverter(typeof(IntNanValueConverter))]
		[SRDescription("DescriptionAttributeLegendCellColumn_MaximumWidth")]
		[SRCategory("CategoryAttribute_Size")]
		public virtual int MaximumWidth
		{
			get
			{
				return this.maximumCellWidth;
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentException("Column maximum width cannot be less than -1.", "MaximumWidth");
				}
				this.maximumCellWidth = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeLegendCellColumn_Name")]
		[Browsable(true)]
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

		object IImageMapProvider.Tag
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

		internal override string DefaultName
		{
			get
			{
				return "Column";
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
				if (this.margins != null)
				{
					this.margins.Common = value;
				}
			}
		}

		public LegendCellColumn()
			: base(null)
		{
			this.Intitialize(string.Empty, LegendCellColumnType.Text, "#LEGENDTEXT", ContentAlignment.MiddleCenter);
		}

		public LegendCellColumn(string headerText)
			: base(null)
		{
			this.Intitialize(headerText, LegendCellColumnType.Text, "#LEGENDTEXT", ContentAlignment.MiddleCenter);
		}

		public LegendCellColumn(string name, string headerText)
			: base(null)
		{
			this.Intitialize(headerText, LegendCellColumnType.Text, "#LEGENDTEXT", ContentAlignment.MiddleCenter);
			this.Name = name;
		}

		public LegendCellColumn(string name, string headerText, ContentAlignment alignment)
			: base(null)
		{
			this.Intitialize(headerText, LegendCellColumnType.Text, "#LEGENDTEXT", alignment);
			this.Name = name;
		}

		private void Intitialize(string headerText, LegendCellColumnType columnType, string text, ContentAlignment alignment)
		{
			this.headerText = headerText;
			this.ColumnType = columnType;
			this.Text = text;
			this.Alignment = alignment;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected bool ShouldSerializeMargins()
		{
			if (this.margins.Top == 0 && this.margins.Bottom == 0 && this.margins.Left == 15 && this.margins.Right == 15)
			{
				return false;
			}
			return true;
		}

		internal LegendCell CreateNewCell()
		{
			LegendCell legendCell = new LegendCell();
			legendCell.CellType = (LegendCellType)((this.ColumnType == LegendCellColumnType.Symbol) ? 1 : 0);
			legendCell.Text = this.Text;
			legendCell.ToolTip = this.ToolTip;
			legendCell.Href = this.Href;
			legendCell.CellAttributes = this.CellColumnAttributes;
			legendCell.SymbolSize = this.SeriesSymbolSize;
			legendCell.Alignment = this.Alignment;
			legendCell.Margins = new Margins(this.Margins.Top, this.Margins.Bottom, this.Margins.Left, this.Margins.Right);
			return legendCell;
		}

		internal override void Invalidate()
		{
			if (this.legend != null)
			{
				this.legend.Invalidate();
			}
		}

		public virtual Legend GetLegend()
		{
			return this.legend;
		}

		internal void SetContainingLegend(Legend legend)
		{
			this.legend = legend;
			if (this.legend != null)
			{
				this.margins.Common = this.legend.Common;
			}
		}

		string IToolTipProvider.GetToolTip()
		{
			return this.ToolTip;
		}

		string IImageMapProvider.GetToolTip()
		{
			return ((IToolTipProvider)this).GetToolTip();
		}

		string IImageMapProvider.GetHref()
		{
			return this.Href;
		}

		string IImageMapProvider.GetMapAreaAttributes()
		{
			return this.CellColumnAttributes;
		}
	}
}
