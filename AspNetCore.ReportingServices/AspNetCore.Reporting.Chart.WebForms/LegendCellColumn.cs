using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendCellColumn_LegendCellColumn")]
	internal class LegendCellColumn
	{
		private Legend legend;

		private string name = string.Empty;

		private LegendCellColumnType columnType;

		private string text = "#LEGENDTEXT";

		private Color textColor = Color.Empty;

		private Color backColor = Color.Empty;

		private Font font;

		private Size seriesSymbolSize = new Size(200, 70);

		private ContentAlignment alignment = ContentAlignment.MiddleCenter;

		private string toolTip = string.Empty;

		private Margins margins = new Margins(0, 0, 15, 15);

		private string href = string.Empty;

		private string mapAreaAttribute = string.Empty;

		private string headerText = string.Empty;

		private StringAlignment headerTextAlignment = StringAlignment.Center;

		private Color headerColor = Color.Black;

		private Color headerBackColor = Color.Empty;

		private Font headerFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f, FontStyle.Bold);

		private int minimumCellWidth = -1;

		private int maximumCellWidth = -1;

		[SRCategory("CategoryAttributeMisc")]
		[SRDescription("DescriptionAttributeLegendCellColumn_Name")]
		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (!(this.name != value))
				{
					return;
				}
				if (this.legend != null)
				{
					foreach (LegendCellColumn cellColumn in this.legend.CellColumns)
					{
						if (cellColumn.Name == value)
						{
							throw new ArgumentException(SR.ExceptionLegendColumnAlreadyExistsInCollection(value));
						}
					}
				}
				if (value != null && value.Length != 0)
				{
					this.name = value;
					return;
				}
				throw new ArgumentException(SR.ExceptionLegendColumnIsEmpty);
			}
		}

		[ParenthesizePropertyName(true)]
		[DefaultValue(LegendCellColumnType.Text)]
		[SRDescription("DescriptionAttributeLegendCellColumn_ColumnType")]
		[SRCategory("CategoryAttributeSeriesItems")]
		public virtual LegendCellColumnType ColumnType
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

		[DefaultValue("#LEGENDTEXT")]
		[SRCategory("CategoryAttributeSeriesItems")]
		[SRDescription("DescriptionAttributeLegendCellColumn_Text")]
		public virtual string Text
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeLegendCellColumn_TextColor")]
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

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeSeriesItems")]
		[SRDescription("DescriptionAttributeLegendCellColumn_BackColor")]
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributeLegendCellColumn_Font")]
		public virtual Font Font
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue(typeof(Size), "200, 70")]
		[SRDescription("DescriptionAttributeLegendCellColumn_SeriesSymbolSize")]
		public virtual Size SeriesSymbolSize
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
				throw new ArgumentException(SR.ExceptionSeriesSymbolSizeIsNegative, "value");
			}
		}

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue(ContentAlignment.MiddleCenter)]
		[SRDescription("DescriptionAttributeLegendCellColumn_Alignment")]
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Margins), "0,0,15,15")]
		[SRDescription("DescriptionAttributeLegendCellColumn_Margins")]
		[SerializationVisibility(SerializationVisibility.Attribute)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual Margins Margins
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

		[DefaultValue("")]
		[SRCategory("CategoryAttributeSeriesItems")]
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
		[SRCategory("CategoryAttributeSeriesItems")]
		[SRDescription("DescriptionAttributeLegendCellColumn_Href")]
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

		[SRCategory("CategoryAttributeSeriesItems")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLegendCellColumn_MapAreaAttributes")]
		public virtual string MapAreaAttributes
		{
			get
			{
				return this.mapAreaAttribute;
			}
			set
			{
				this.mapAreaAttribute = value;
			}
		}

		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderText")]
		[SRCategory("CategoryAttributeHeader")]
		[DefaultValue("")]
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

		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttributeHeader")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderColor")]
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
		[SRCategory("CategoryAttributeHeader")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderBackColor")]
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

		[SRCategory("CategoryAttributeHeader")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt, style=Bold")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderFont")]
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

		[DefaultValue(typeof(StringAlignment), "Center")]
		[SRCategory("CategoryAttributeHeader")]
		[SRDescription("DescriptionAttributeLegendCellColumn_HeaderTextAlignment")]
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

		[TypeConverter(typeof(IntNanValueConverter))]
		[SRCategory("CategoryAttributeSize")]
		[DefaultValue(-1)]
		[SRDescription("DescriptionAttributeLegendCellColumn_MinimumWidth")]
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
					throw new ArgumentException(SR.ExceptionMinimumCellWidthIsWrong, "value");
				}
				this.minimumCellWidth = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeSize")]
		[TypeConverter(typeof(IntNanValueConverter))]
		[SRDescription("DescriptionAttributeLegendCellColumn_MaximumWidth")]
		[DefaultValue(-1)]
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
					throw new ArgumentException(SR.ExceptionMaximumCellWidthIsWrong, "value");
				}
				this.maximumCellWidth = value;
				this.Invalidate();
			}
		}

		public LegendCellColumn()
			: this(string.Empty, LegendCellColumnType.Text, "#LEGENDTEXT", ContentAlignment.MiddleCenter)
		{
		}

		public LegendCellColumn(string headerText, LegendCellColumnType columnType, string text)
			: this(headerText, columnType, text, ContentAlignment.MiddleCenter)
		{
		}

		public LegendCellColumn(string headerText, LegendCellColumnType columnType, string text, ContentAlignment alignment)
		{
			this.headerText = headerText;
			this.columnType = columnType;
			this.text = text;
			this.alignment = alignment;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeMargins()
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
			legendCell.CellType = (LegendCellType)((this.ColumnType == LegendCellColumnType.SeriesSymbol) ? 1 : 0);
			legendCell.Text = this.Text;
			legendCell.ToolTip = this.ToolTip;
			legendCell.Href = this.Href;
			legendCell.MapAreaAttributes = this.MapAreaAttributes;
			legendCell.SeriesSymbolSize = this.SeriesSymbolSize;
			legendCell.Alignment = this.Alignment;
			legendCell.Margins = new Margins(this.Margins.Top, this.Margins.Bottom, this.Margins.Left, this.Margins.Right);
			return legendCell;
		}

		protected void Invalidate()
		{
			if (this.legend != null)
			{
				this.legend.Invalidate(false);
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
	}
}
