using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(LegendConverter))]
	[DefaultProperty("Enabled")]
	[Description("Legend style, position, custom elements and other properties.")]
	internal class Legend : AutoSizePanel, IToolTipProvider
	{
		internal class LegendConverter : AutoSizePanelConverter
		{
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof(InstanceDescriptor))
				{
					return true;
				}
				return base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType == typeof(InstanceDescriptor))
				{
					ConstructorInfo constructor = typeof(Legend).GetConstructor(Type.EmptyTypes);
					return new InstanceDescriptor(constructor, null, false);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}

		private const int DummyItemsCount = 5;

		private LegendTableStyle legendTableStyle;

		private bool autoFitText = true;

		private TempLegendItemsCollection legendItems;

		private SizeF sizeLargestItemText = SizeF.Empty;

		private SizeF sizeAverageItemText = SizeF.Empty;

		private SizeF sizeItemImage = SizeF.Empty;

		private int itemColumns;

		private SizeF itemCellSize = SizeF.Empty;

		internal Font autofitFont;

		private bool interlacedRows;

		private Color interlacedRowsColor = Color.Empty;

		private Size offset = System.Drawing.Size.Empty;

		private int textWrapThreshold = 25;

		private int autoFitFontSizeAdjustment;

		private LegendCellColumnCollection cellColumns;

		private AutoBool reversed;

		private string title = "Legend Title";

		private Color titleColor = Color.Black;

		private Color titleBackColor = Color.Empty;

		private StringAlignment titleAlignment = StringAlignment.Center;

		private LegendSeparatorType titleSeparator = LegendSeparatorType.GradientLine;

		private Color titleSeparatorColor = Color.Gray;

		private LegendSeparatorType headerSeparator;

		private Color headerSeparatorColor = Color.Black;

		private LegendSeparatorType itemColumnSeparator;

		private Color itemColumnSeparatorColor = Color.DarkGray;

		private int itemColumnSpacing = 50;

		private int itemColumnSpacingRel;

		private Rectangle titlePosition = Rectangle.Empty;

		private Rectangle headerPosition = Rectangle.Empty;

		private int autoFitMinFontSize = 7;

		private int horizontalSpaceLeft;

		private int verticalSpaceLeft;

		private int[,] subColumnSizes;

		private int[,] cellHeights;

		private int[] numberOfRowsPerColumn;

		private int numberOfLegendItemsToProcess = -1;

		private Rectangle legendItemsAreaPosition = Rectangle.Empty;

		private bool legendItemsTruncated;

		private int truncatedDotsSize = 3;

		private int numberOfCells = -1;

		internal Size singleWCharacterSize = System.Drawing.Size.Empty;

		private bool showSelectedTitle;

		private bool equallySpacedItems;

		private LegendStyle legendStyle = LegendStyle.Table;

		private Font font = new Font("Microsoft Sans Serif", 8f);

		private bool disposeFont = true;

		private Color textColor = Color.Black;

		private LegendItemsCollection customLegends;

		private Font titleFont = new Font("Microsoft Sans Serif", 9.75f);

		[DefaultValue(false)]
		[NotifyParentProperty(true)]
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
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[Browsable(false)]
		internal RectangleF TitleSelectionRectangle
		{
			get
			{
				RectangleF result = this.titlePosition;
				result.Offset(base.GetAbsoluteLocation());
				result.Inflate((float)(-this.GetBorderSize()), (float)(-this.GetBorderSize()));
				result.Inflate((float)(-this.offset.Width), (float)(-this.offset.Height));
				return result;
			}
		}

		[SerializationVisibility(SerializationVisibility.Attribute)]
		[NotifyParentProperty(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeLegend_Name")]
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

		[DefaultValue(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegend_EquallySpacedItems")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		public bool EquallySpacedItems
		{
			get
			{
				return this.equallySpacedItems;
			}
			set
			{
				this.equallySpacedItems = value;
				this.Invalidate();
			}
		}

		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeLegend_AutoFitText")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		public bool AutoFitText
		{
			get
			{
				return this.autoFitText;
			}
			set
			{
				this.autoFitText = value;
				if (this.autoFitText)
				{
					if (this.font != null)
					{
						Font font = new Font(this.font.FontFamily, 8f, this.font.Style);
						if (this.disposeFont)
						{
							this.font.Dispose();
						}
						this.font = font;
						this.disposeFont = true;
					}
					else
					{
						this.font = new Font("Microsoft Sans Serif", 8f);
						this.disposeFont = true;
					}
				}
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[Bindable(true)]
		[DefaultValue(LegendStyle.Table)]
		[SRDescription("DescriptionAttributeLegend_LegendStyle")]
		[NotifyParentProperty(true)]
		public LegendStyle LegendStyle
		{
			get
			{
				return this.legendStyle;
			}
			set
			{
				this.legendStyle = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeLegend_TableStyle")]
		[Bindable(true)]
		[DefaultValue(LegendTableStyle.Auto)]
		[SRCategory("CategoryAttribute_Behavior")]
		[NotifyParentProperty(true)]
		public LegendTableStyle TableStyle
		{
			get
			{
				return this.legendTableStyle;
			}
			set
			{
				this.legendTableStyle = value;
				this.Invalidate();
			}
		}

		[DefaultValue(7)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeLegend_AutoFitMinFontSize")]
		public int AutoFitMinFontSize
		{
			get
			{
				return this.autoFitMinFontSize;
			}
			set
			{
				if (value < 5)
				{
					throw new InvalidOperationException(SR.ExceptionAutoFitMinFontSizeMinValue);
				}
				this.autoFitMinFontSize = value;
				this.Invalidate();
			}
		}

		[DefaultValue(true)]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
			}
		}

		[SRCategory("CategoryAttribute_CellColumns")]
		[SRDescription("DescriptionAttributeLegend_CellColumns")]
		public LegendCellColumnCollection CellColumns
		{
			get
			{
				return this.cellColumns;
			}
		}

		[SRDescription("DescriptionAttributeLegend_HeaderSeparator")]
		[DefaultValue(typeof(LegendSeparatorType), "None")]
		[SRCategory("CategoryAttribute_CellColumns")]
		public LegendSeparatorType HeaderSeparator
		{
			get
			{
				return this.headerSeparator;
			}
			set
			{
				if (value != this.headerSeparator)
				{
					this.headerSeparator = value;
					this.Invalidate();
				}
			}
		}

		[SRDescription("DescriptionAttributeLegend_HeaderSeparatorColor")]
		[SRCategory("CategoryAttribute_CellColumns")]
		[DefaultValue(typeof(Color), "Black")]
		public Color HeaderSeparatorColor
		{
			get
			{
				return this.headerSeparatorColor;
			}
			set
			{
				if (value != this.headerSeparatorColor)
				{
					this.headerSeparatorColor = value;
					this.Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_CellColumns")]
		[SRDescription("DescriptionAttributeLegend_ItemColumnSeparator")]
		[DefaultValue(typeof(LegendSeparatorType), "None")]
		public LegendSeparatorType ItemColumnSeparator
		{
			get
			{
				return this.itemColumnSeparator;
			}
			set
			{
				if (value != this.itemColumnSeparator)
				{
					this.itemColumnSeparator = value;
					this.Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_CellColumns")]
		[SRDescription("DescriptionAttributeLegend_ItemColumnSeparatorColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color ItemColumnSeparatorColor
		{
			get
			{
				return this.itemColumnSeparatorColor;
			}
			set
			{
				if (value != this.itemColumnSeparatorColor)
				{
					this.itemColumnSeparatorColor = value;
					this.Invalidate();
				}
			}
		}

		[SRDescription("DescriptionAttributeLegend_ItemColumnSpacing")]
		[SRCategory("CategoryAttribute_CellColumns")]
		[DefaultValue(50)]
		public int ItemColumnSpacing
		{
			get
			{
				return this.itemColumnSpacing;
			}
			set
			{
				if (value != this.itemColumnSpacing)
				{
					if (value < 0)
					{
						throw new ArgumentOutOfRangeException("TableColumnSpacing", SR.ExceptionLegendTableColumnSpacingTooSmall);
					}
					this.itemColumnSpacing = value;
					this.Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[Browsable(false)]
		[Bindable(true)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeLegend_Font")]
		public Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.AutoFitText = false;
				if (this.disposeFont)
				{
					this.font.Dispose();
				}
				this.font = value;
				this.disposeFont = false;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttribute_Appearance")]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLegend_TextColor")]
		[NotifyParentProperty(true)]
		public Color TextColor
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

		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeLegend_Items")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		public LegendItemsCollection Items
		{
			get
			{
				return this.customLegends;
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(25)]
		[SRDescription("DescriptionAttributeLegend_TextWrapThreshold")]
		public int TextWrapThreshold
		{
			get
			{
				return this.textWrapThreshold;
			}
			set
			{
				if (value != this.textWrapThreshold)
				{
					if (value < 0)
					{
						throw new ArgumentException(SR.ExceptionTextThresholdValueTooSmall, "TextWrapThreshold");
					}
					this.textWrapThreshold = value;
					this.Invalidate();
				}
			}
		}

		[Description("Indicates that all legend items are shown in reversed order. This property only affects legend items automatically added and has no effect on custom legend items.")]
		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(AutoBool.Auto)]
		private AutoBool Reversed
		{
			get
			{
				return this.reversed;
			}
			set
			{
				if (value != this.reversed)
				{
					this.reversed = value;
					this.Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Behavior")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeLegend_InterlacedRows")]
		public bool InterlacedRows
		{
			get
			{
				return this.interlacedRows;
			}
			set
			{
				if (value != this.interlacedRows)
				{
					this.interlacedRows = value;
					this.Invalidate();
				}
			}
		}

		[SRDescription("DescriptionAttributeLegend_InterlacedRowsColor")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttribute_Behavior")]
		public Color InterlacedRowsColor
		{
			get
			{
				return this.interlacedRowsColor;
			}
			set
			{
				if (value != this.interlacedRowsColor)
				{
					this.interlacedRowsColor = value;
					this.Invalidate();
				}
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
				return this.Items.Count == 0;
			}
		}

		[SRDescription("DescriptionAttributeLegend_Title")]
		[DefaultValue("Legend Title")]
		[SRCategory("CategoryAttribute_Title")]
		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				if (value != this.title)
				{
					this.title = value;
					this.Invalidate();
				}
			}
		}

		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeLegend_TitleColor")]
		[SRCategory("CategoryAttribute_Title")]
		public Color TitleColor
		{
			get
			{
				return this.titleColor;
			}
			set
			{
				if (value != this.titleColor)
				{
					this.titleColor = value;
					this.Invalidate();
				}
			}
		}

		[SRDescription("DescriptionAttributeLegend_TitleBackColor")]
		[SRCategory("CategoryAttribute_Title")]
		[DefaultValue(typeof(Color), "")]
		public Color TitleBackColor
		{
			get
			{
				return this.titleBackColor;
			}
			set
			{
				if (value != this.titleBackColor)
				{
					this.titleBackColor = value;
					this.Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Title")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 9.75pt")]
		[SRDescription("DescriptionAttributeLegend_TitleFont")]
		public Font TitleFont
		{
			get
			{
				return this.titleFont;
			}
			set
			{
				if (value != this.titleFont)
				{
					this.titleFont = value;
					this.Invalidate();
				}
			}
		}

		[DefaultValue(typeof(StringAlignment), "Center")]
		[SRDescription("DescriptionAttributeLegend_TitleAlignment")]
		[SRCategory("CategoryAttribute_Title")]
		public StringAlignment TitleAlignment
		{
			get
			{
				return this.titleAlignment;
			}
			set
			{
				if (value != this.titleAlignment)
				{
					this.titleAlignment = value;
					this.Invalidate();
				}
			}
		}

		[SRDescription("DescriptionAttributeLegend_TitleSeparator")]
		[SRCategory("CategoryAttribute_Title")]
		[DefaultValue(typeof(LegendSeparatorType), "GradientLine")]
		public LegendSeparatorType TitleSeparator
		{
			get
			{
				return this.titleSeparator;
			}
			set
			{
				if (value != this.titleSeparator)
				{
					this.titleSeparator = value;
					this.Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Title")]
		[DefaultValue(typeof(Color), "Gray")]
		[SRDescription("DescriptionAttributeLegend_TitleSeparatorColor")]
		public Color TitleSeparatorColor
		{
			get
			{
				return this.titleSeparatorColor;
			}
			set
			{
				if (value != this.titleSeparatorColor)
				{
					this.titleSeparatorColor = value;
					this.Invalidate();
				}
			}
		}

		internal override string DefaultName
		{
			get
			{
				return "Legend";
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
				if (this.Items != null)
				{
					this.Items.Common = value;
				}
				if (this.CellColumns != null)
				{
					this.CellColumns.Common = value;
				}
			}
		}

		public Legend()
			: this((CommonElements)null)
		{
		}

		internal Legend(CommonElements common)
			: base(common)
		{
			this.customLegends = new LegendItemsCollection(this, common);
			this.customLegends.Legend = this;
			this.legendItems = new TempLegendItemsCollection();
			this.cellColumns = new LegendCellColumnCollection(this, this, common);
			this.Visible = true;
		}

		public Legend(string name)
			: this((CommonElements)null)
		{
			this.Name = name;
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

		private void RecalcLegendInfo(MapGraphics g)
		{
			RectangleF relative = new RectangleF(0f, 0f, 100f, 100f);
			Rectangle rectangle = Rectangle.Round(g.GetAbsoluteRectangle(relative));
			this.offset.Width = (int)Math.Ceiling((float)this.singleWCharacterSize.Width / 2.0);
			this.offset.Height = (int)Math.Ceiling((float)this.singleWCharacterSize.Width / 3.0);
			this.itemColumnSpacingRel = (int)((float)this.singleWCharacterSize.Width * ((float)this.itemColumnSpacing / 100.0));
			if (this.itemColumnSpacingRel % 2 == 1)
			{
				this.itemColumnSpacingRel++;
			}
			this.titlePosition = Rectangle.Empty;
			if (!string.IsNullOrEmpty(this.Title))
			{
				Size titleSize = this.GetTitleSize(g, rectangle.Size);
				this.titlePosition = new Rectangle(rectangle.Location.X, rectangle.Location.Y, rectangle.Width, Math.Min(rectangle.Height, titleSize.Height));
				rectangle.Height -= this.titlePosition.Height;
				this.titlePosition.Y += this.GetBorderSize();
			}
			this.headerPosition = Rectangle.Empty;
			Size empty = System.Drawing.Size.Empty;
			foreach (LegendCellColumn cellColumn in this.CellColumns)
			{
				if (!string.IsNullOrEmpty(cellColumn.HeaderText))
				{
					Size headerSize = this.GetHeaderSize(g, cellColumn);
					empty.Height = Math.Max(empty.Height, headerSize.Height);
				}
			}
			if (!empty.IsEmpty)
			{
				this.headerPosition = new Rectangle(rectangle.Location.X + this.GetBorderSize() + this.offset.Width, rectangle.Location.Y + this.titlePosition.Height, rectangle.Width - (this.GetBorderSize() + this.offset.Width) * 2, Math.Min(rectangle.Height - this.titlePosition.Height, empty.Height));
				this.headerPosition.Height = Math.Max(this.headerPosition.Height, 0);
				rectangle.Height -= this.headerPosition.Height;
				this.headerPosition.Y += this.GetBorderSize();
			}
			this.legendItemsAreaPosition = new Rectangle(rectangle.X + this.offset.Width + this.GetBorderSize(), rectangle.Y + this.offset.Height + this.GetBorderSize() + this.titlePosition.Height + this.headerPosition.Height, rectangle.Width - 2 * (this.offset.Width + this.GetBorderSize()), rectangle.Height - 2 * (this.offset.Height + this.GetBorderSize()));
			this.GetNumberOfRowsAndColumns(g, this.legendItemsAreaPosition.Size, -1, out this.numberOfRowsPerColumn, out this.itemColumns, out this.horizontalSpaceLeft, out this.verticalSpaceLeft);
			this.autoFitFontSizeAdjustment = 0;
			this.legendItemsTruncated = false;
			bool flag = this.horizontalSpaceLeft >= 0 && this.verticalSpaceLeft >= 0;
			this.numberOfLegendItemsToProcess = this.legendItems.Count;
			int num = 0;
			for (int i = 0; i < this.itemColumns; i++)
			{
				num += this.numberOfRowsPerColumn[i];
			}
			if (num < this.numberOfLegendItemsToProcess)
			{
				flag = false;
			}
			this.autofitFont = new Font(this.Font, this.Font.Style);
			while (!flag && this.AutoFitText && this.Font.Size - (float)this.autoFitFontSizeAdjustment > (float)this.autoFitMinFontSize)
			{
				this.autoFitFontSizeAdjustment++;
				int num2 = (int)Math.Round((double)(this.Font.Size - (float)this.autoFitFontSizeAdjustment));
				if (num2 < 1)
				{
					num2 = 1;
				}
				if (this.autofitFont != null)
				{
					this.autofitFont.Dispose();
					this.autofitFont = null;
				}
				this.autofitFont = new Font(this.Font.FontFamily, (float)num2, this.Font.Style, this.Font.Unit);
				this.GetNumberOfRowsAndColumns(g, this.legendItemsAreaPosition.Size, -1, out this.numberOfRowsPerColumn, out this.itemColumns, out this.horizontalSpaceLeft, out this.verticalSpaceLeft);
				flag = (this.horizontalSpaceLeft >= 0 && this.verticalSpaceLeft >= 0);
				num = 0;
				for (int j = 0; j < this.itemColumns; j++)
				{
					num += this.numberOfRowsPerColumn[j];
				}
				if (num < this.numberOfLegendItemsToProcess)
				{
					flag = false;
				}
			}
			int num3 = 0;
			int num4 = this.numberOfLegendItemsToProcess;
			while (!flag)
			{
				if (this.numberOfLegendItemsToProcess > 2)
				{
					if (num4 - num3 <= 1)
					{
						flag = true;
						if (this.numberOfLegendItemsToProcess != num3)
						{
							this.numberOfLegendItemsToProcess = num3;
							this.TryLayoutLegendItems(g);
						}
						break;
					}
					if (this.itemColumns == 1 && this.horizontalSpaceLeft < 0 && this.verticalSpaceLeft >= 0)
					{
						flag = true;
						this.numberOfLegendItemsToProcess = Math.Min(this.numberOfLegendItemsToProcess, this.numberOfRowsPerColumn[0]);
						break;
					}
					if (this.GetMaximumNumberOfRows() == 1 && this.verticalSpaceLeft < 0 && this.horizontalSpaceLeft >= 0)
					{
						flag = true;
						this.numberOfLegendItemsToProcess = Math.Min(this.numberOfLegendItemsToProcess, this.itemColumns);
						break;
					}
					if (!this.legendItemsTruncated)
					{
						this.legendItemsTruncated = true;
						this.legendItemsAreaPosition.Height -= this.truncatedDotsSize;
					}
					this.numberOfLegendItemsToProcess = (num4 + num3) / 2;
					if (this.TryLayoutLegendItems(g))
					{
						num3 = this.numberOfLegendItemsToProcess;
					}
					else
					{
						num4 = this.numberOfLegendItemsToProcess;
					}
					continue;
				}
				flag = true;
				break;
			}
			Size empty2 = System.Drawing.Size.Empty;
			if (this.verticalSpaceLeft > 0)
			{
				empty2.Height = this.verticalSpaceLeft / this.GetMaximumNumberOfRows() / 2;
			}
			if (this.horizontalSpaceLeft > 0)
			{
				empty2.Width = this.horizontalSpaceLeft / 2;
			}
			int num5 = 0;
			int num6 = 0;
			if (this.numberOfLegendItemsToProcess < 0)
			{
				this.numberOfLegendItemsToProcess = this.legendItems.Count;
			}
			for (int k = 0; k < this.numberOfLegendItemsToProcess; k++)
			{
				LegendItem legendItem = this.legendItems[k];
				int num7;
				for (num7 = 0; num7 < legendItem.Cells.Count; num7++)
				{
					LegendCell legendCell = legendItem.Cells[num7];
					Rectangle cellPosition = this.GetCellPosition(g, num5, num6, num7, empty2);
					int num8 = 0;
					if (legendCell.CellSpan > 1)
					{
						for (int l = 1; l < legendCell.CellSpan && num7 + l < legendItem.Cells.Count; l++)
						{
							Rectangle cellPosition2 = this.GetCellPosition(g, num5, num6, num7 + l, empty2);
							if (cellPosition.Right < cellPosition2.Right)
							{
								cellPosition.Width += cellPosition2.Right - cellPosition.Right;
							}
							num8++;
							LegendCell legendCell2 = legendItem.Cells[num7 + l];
							legendCell2.SetCellPosition(g, num5, num6, Rectangle.Empty, this.autoFitFontSizeAdjustment, (this.autofitFont == null) ? this.Font : this.autofitFont, this.singleWCharacterSize);
						}
					}
					cellPosition.Intersect(this.legendItemsAreaPosition);
					legendCell.SetCellPosition(g, num5, num6, cellPosition, this.autoFitFontSizeAdjustment, (this.autofitFont == null) ? this.Font : this.autofitFont, this.singleWCharacterSize);
					num7 += num8;
				}
				num6++;
				if (num6 >= this.numberOfRowsPerColumn[num5])
				{
					num5++;
					num6 = 0;
					if (num5 >= this.itemColumns)
					{
						break;
					}
				}
			}
		}

		private bool TryLayoutLegendItems(MapGraphics g)
		{
			bool result = false;
			for (int i = 0; i < 2; i++)
			{
				result = this.GetNumberOfRowsAndColumns(g, this.legendItemsAreaPosition.Size, this.numberOfLegendItemsToProcess, out this.numberOfRowsPerColumn, out this.itemColumns, out this.horizontalSpaceLeft, out this.verticalSpaceLeft);
			}
			return result;
		}

		protected override SizeF CalculateUndockedAutoSize(SizeF size)
		{
			switch (this.LegendStyle)
			{
			case LegendStyle.Column:
				size.Width = (float)(size.Width * this.MaxAutoSize / 100.0);
				break;
			case LegendStyle.Row:
				size.Height = (float)(size.Height * this.MaxAutoSize / 100.0);
				break;
			case LegendStyle.Table:
				switch (this.TableStyle)
				{
				case LegendTableStyle.Tall:
					size.Width = (float)(size.Width * this.MaxAutoSize / 100.0);
					break;
				case LegendTableStyle.Wide:
					size.Height = (float)(size.Height * this.MaxAutoSize / 100.0);
					break;
				case LegendTableStyle.Auto:
					size = base.CalculateUndockedAutoSize(size);
					break;
				}
				break;
			}
			return size;
		}

		private Rectangle GetCellPosition(MapGraphics g, int columnIndex, int rowIndex, int cellIndex, Size itemHalfSpacing)
		{
			Rectangle result = this.legendItemsAreaPosition;
			for (int i = 0; i < rowIndex; i++)
			{
				result.Y += this.cellHeights[columnIndex, i];
			}
			if (itemHalfSpacing.Height > 0)
			{
				result.Y += itemHalfSpacing.Height * rowIndex * 2 + itemHalfSpacing.Height;
			}
			if (this.horizontalSpaceLeft > 0)
			{
				result.X += itemHalfSpacing.Width;
			}
			int num = this.GetNumberOfCells();
			for (int j = 0; j < columnIndex; j++)
			{
				for (int k = 0; k < num; k++)
				{
					result.X += this.subColumnSizes[j, k];
				}
				result.X += this.GetSeparatorSize(g, this.ItemColumnSeparator).Width;
			}
			for (int l = 0; l < cellIndex; l++)
			{
				result.X += this.subColumnSizes[columnIndex, l];
			}
			result.Height = this.cellHeights[columnIndex, rowIndex];
			result.Width = this.subColumnSizes[columnIndex, cellIndex];
			return result;
		}

		internal override SizeF GetOptimalSize(MapGraphics g, SizeF maxSizeAbs)
		{
			this.offset = System.Drawing.Size.Empty;
			this.itemColumns = 0;
			this.horizontalSpaceLeft = 0;
			this.verticalSpaceLeft = 0;
			this.subColumnSizes = null;
			this.numberOfRowsPerColumn = null;
			this.cellHeights = null;
			this.autofitFont = null;
			this.autoFitFontSizeAdjustment = 0;
			this.numberOfCells = -1;
			this.numberOfLegendItemsToProcess = -1;
			Size empty = System.Drawing.Size.Empty;
			Size size = new Size((int)maxSizeAbs.Width, (int)maxSizeAbs.Height);
			foreach (LegendItem legendItem in this.legendItems)
			{
				foreach (LegendCell cell in legendItem.Cells)
				{
					cell.ResetCache();
				}
			}
			if (this.IsVisible())
			{
				this.singleWCharacterSize = g.MeasureStringAbs("W", this.Font);
				Size size2 = g.MeasureStringAbs("WW", this.Font);
				this.singleWCharacterSize.Width = size2.Width - this.singleWCharacterSize.Width;
				this.FillLegendItemsCollection(this.singleWCharacterSize);
				if (this.legendItems.Count > 0)
				{
					this.offset.Width = (int)Math.Ceiling((float)this.singleWCharacterSize.Width / 2.0);
					this.offset.Height = (int)Math.Ceiling((float)this.singleWCharacterSize.Width / 3.0);
					this.itemColumnSpacingRel = (int)((float)this.singleWCharacterSize.Width * ((float)this.itemColumnSpacing / 100.0));
					if (this.itemColumnSpacingRel % 2 == 1)
					{
						this.itemColumnSpacingRel++;
					}
					Size size3 = System.Drawing.Size.Empty;
					if (!string.IsNullOrEmpty(this.Title))
					{
						size3 = this.GetTitleSize(g, size);
					}
					Size empty2 = System.Drawing.Size.Empty;
					foreach (LegendCellColumn cellColumn in this.CellColumns)
					{
						if (!string.IsNullOrEmpty(cellColumn.HeaderText))
						{
							Size headerSize = this.GetHeaderSize(g, cellColumn);
							empty2.Height = Math.Max(empty2.Height, headerSize.Height);
						}
					}
					Size legendSize = size;
					legendSize.Width -= 2 * (this.offset.Width + this.GetBorderSize());
					legendSize.Height -= 2 * (this.offset.Height + this.GetBorderSize());
					legendSize.Height -= size3.Height;
					legendSize.Height -= empty2.Height;
					this.autoFitFontSizeAdjustment = 0;
					this.autofitFont = new Font(this.Font, this.Font.Style);
					int num = 0;
					int num2 = 0;
					bool flag = this.AutoFitText;
					bool flag2 = false;
					do
					{
						this.GetNumberOfRowsAndColumns(g, legendSize, -1, out this.numberOfRowsPerColumn, out this.itemColumns, out num2, out num);
						int num3 = 0;
						for (int i = 0; i < this.itemColumns; i++)
						{
							num3 += this.numberOfRowsPerColumn[i];
						}
						flag2 = (num2 >= 0 && num >= 0 && num3 >= this.legendItems.Count);
						if (flag && !flag2)
						{
							if (this.Font.Size - (float)this.autoFitFontSizeAdjustment > (float)this.autoFitMinFontSize)
							{
								this.autoFitFontSizeAdjustment++;
								int num4 = (int)Math.Round((double)(this.Font.Size - (float)this.autoFitFontSizeAdjustment));
								if (num4 < 1)
								{
									num4 = 1;
								}
								if (this.autofitFont != null)
								{
									this.autofitFont.Dispose();
									this.autofitFont = null;
								}
								this.autofitFont = new Font(this.Font.FontFamily, (float)num4, this.Font.Style, this.Font.Unit);
							}
							else
							{
								flag = false;
							}
						}
					}
					while (flag && !flag2);
					num2 -= Math.Min(4, num2);
					num -= Math.Min(2, num);
					empty.Width = legendSize.Width - num2;
					empty.Width = Math.Max(empty.Width, size3.Width);
					empty.Width += 2 * (this.offset.Width + this.GetBorderSize());
					empty.Height = legendSize.Height - num + size3.Height + empty2.Height;
					empty.Height += 2 * (this.offset.Height + this.GetBorderSize());
					if (num2 < 0 || num < 0)
					{
						empty.Height += this.truncatedDotsSize;
					}
					empty.Width += 2 * this.BorderWidth;
					empty.Height += 2 * this.BorderWidth;
					if (empty.Width > size.Width)
					{
						empty.Width = size.Width;
					}
					if (empty.Height > size.Height)
					{
						empty.Height = size.Height;
					}
					if (empty.Width < 2)
					{
						empty.Width = 2;
					}
					if (empty.Height < 2)
					{
						empty.Height = 2;
					}
				}
			}
			return empty;
		}

		private bool GetNumberOfRowsAndColumns(MapGraphics g, Size legendSize, int numberOfItemsToCheck, out int[] numberOfRowsPerColumn, out int columnNumber)
		{
			int num = 0;
			int num2 = 0;
			return this.GetNumberOfRowsAndColumns(g, legendSize, numberOfItemsToCheck, out numberOfRowsPerColumn, out columnNumber, out num, out num2);
		}

		private bool GetNumberOfRowsAndColumns(MapGraphics g, Size legendSize, int numberOfItemsToCheck, out int[] numberOfRowsPerColumn, out int columnNumber, out int horSpaceLeft, out int vertSpaceLeft)
		{
			numberOfRowsPerColumn = null;
			columnNumber = 1;
			horSpaceLeft = 0;
			vertSpaceLeft = 0;
			if (numberOfItemsToCheck < 0)
			{
				numberOfItemsToCheck = this.legendItems.Count;
			}
			if (this.LegendStyle == LegendStyle.Column || numberOfItemsToCheck <= 1)
			{
				columnNumber = 1;
				numberOfRowsPerColumn = new int[1]
				{
					numberOfItemsToCheck
				};
			}
			else if (this.LegendStyle == LegendStyle.Row)
			{
				columnNumber = numberOfItemsToCheck;
				numberOfRowsPerColumn = new int[columnNumber];
				for (int i = 0; i < columnNumber; i++)
				{
					numberOfRowsPerColumn[i] = 1;
				}
			}
			else if (this.LegendStyle == LegendStyle.Table)
			{
				columnNumber = 1;
				numberOfRowsPerColumn = new int[1]
				{
					1
				};
				switch (this.GetLegendTableStyle(g))
				{
				case LegendTableStyle.Tall:
				{
					bool flag4 = false;
					int num6 = 1;
					for (num6 = 1; !flag4 && num6 < numberOfItemsToCheck; num6++)
					{
						numberOfRowsPerColumn[columnNumber - 1]++;
						if (!this.CheckLegendItemsFit(g, legendSize, num6 + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft))
						{
							if (columnNumber != 1 && horSpaceLeft >= 0)
							{
								goto IL_0114;
							}
							if (vertSpaceLeft <= 0)
							{
								goto IL_0114;
							}
						}
						continue;
						IL_0114:
						if (numberOfRowsPerColumn[columnNumber - 1] > 1)
						{
							numberOfRowsPerColumn[columnNumber - 1]--;
						}
						int num7 = 0;
						if (horSpaceLeft > 0)
						{
							num7 = (int)Math.Round((double)(legendSize.Width - horSpaceLeft) / (double)columnNumber) / 2;
						}
						if (columnNumber < 50 && horSpaceLeft >= num7)
						{
							columnNumber++;
							int[] array2 = numberOfRowsPerColumn;
							numberOfRowsPerColumn = new int[columnNumber];
							for (int num8 = 0; num8 < array2.Length; num8++)
							{
								numberOfRowsPerColumn[num8] = array2[num8];
							}
							numberOfRowsPerColumn[columnNumber - 1] = 1;
							if (num6 == numberOfItemsToCheck - 1)
							{
								this.CheckLegendItemsFit(g, legendSize, num6 + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
							}
						}
						else
						{
							flag4 = true;
						}
					}
					if (columnNumber > 1)
					{
						bool flag5 = false;
						while (!flag5)
						{
							flag5 = true;
							int num9 = -1;
							for (int num10 = 0; num10 < columnNumber; num10++)
							{
								int num11 = 0;
								for (int num12 = 0; num12 < this.numberOfRowsPerColumn[num10] - 1; num12++)
								{
									num11 += this.cellHeights[num10, num12];
								}
								num9 = Math.Max(num9, num11);
							}
							int num13 = 0;
							for (int num14 = 0; num14 < columnNumber - 1; num14++)
							{
								if (this.numberOfRowsPerColumn[num14] > 1)
								{
									num13 += this.cellHeights[num14, this.numberOfRowsPerColumn[num14] - 1];
								}
							}
							if (num13 > 0)
							{
								int columnHeight2 = this.GetColumnHeight(columnNumber - 1);
								if (columnHeight2 + num13 <= num9)
								{
									int num15 = 0;
									for (int num16 = 0; num16 < columnNumber - 1; num16++)
									{
										if (this.numberOfRowsPerColumn[num16] > 1)
										{
											this.numberOfRowsPerColumn[num16]--;
											num15++;
										}
									}
									if (num15 > 0)
									{
										this.numberOfRowsPerColumn[columnNumber - 1] += num15;
										this.CheckLegendItemsFit(g, legendSize, num6 + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
										flag5 = false;
									}
								}
							}
						}
					}
					break;
				}
				case LegendTableStyle.Wide:
				{
					bool flag = false;
					int num = 1;
					for (num = 1; !flag && num < numberOfItemsToCheck; num++)
					{
						columnNumber++;
						int[] array = numberOfRowsPerColumn;
						numberOfRowsPerColumn = new int[columnNumber];
						for (int j = 0; j < array.Length; j++)
						{
							numberOfRowsPerColumn[j] = array[j];
						}
						numberOfRowsPerColumn[columnNumber - 1] = 1;
						bool flag2 = this.CheckLegendItemsFit(g, legendSize, num + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
						if (!flag2)
						{
							if (this.GetMaximumNumberOfRows(numberOfRowsPerColumn) != 1 && vertSpaceLeft >= 0)
							{
								goto IL_03ee;
							}
							if (horSpaceLeft <= 0)
							{
								goto IL_03ee;
							}
						}
						continue;
						IL_03ee:
						bool flag3 = true;
						while (flag3)
						{
							flag3 = false;
							int num2 = 0;
							if (columnNumber > 1)
							{
								num2 = numberOfRowsPerColumn[columnNumber - 1];
								columnNumber--;
								array = numberOfRowsPerColumn;
								numberOfRowsPerColumn = new int[columnNumber];
								for (int k = 0; k < columnNumber; k++)
								{
									numberOfRowsPerColumn[k] = array[k];
								}
							}
							for (int l = 0; l < num2; l++)
							{
								int num3 = -1;
								int num4 = 2147483647;
								for (int m = 0; m < columnNumber; m++)
								{
									int columnHeight = this.GetColumnHeight(m);
									int num5 = 0;
									if (m < columnNumber - 1)
									{
										num5 = this.cellHeights[m + 1, 0];
									}
									if (columnHeight < num4 && columnHeight + num5 < legendSize.Height)
									{
										num4 = columnHeight;
										num3 = m;
									}
								}
								if (num3 < 0)
								{
									flag2 = this.CheckLegendItemsFit(g, legendSize, num + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
									flag = true;
									break;
								}
								numberOfRowsPerColumn[num3]++;
								if (num3 < columnNumber - 1 && numberOfRowsPerColumn[num3 + 1] == 1)
								{
									array = numberOfRowsPerColumn;
									for (int n = num3 + 1; n < array.Length - 1; n++)
									{
										numberOfRowsPerColumn[n] = array[n + 1];
									}
									numberOfRowsPerColumn[columnNumber - 1] = 1;
								}
								flag2 = this.CheckLegendItemsFit(g, legendSize, num + 1, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
							}
							if (!flag2 && (float)horSpaceLeft < 0.0 && columnNumber > 1)
							{
								flag3 = true;
							}
						}
					}
					break;
				}
				}
			}
			return this.CheckLegendItemsFit(g, legendSize, numberOfItemsToCheck, this.autoFitFontSizeAdjustment, columnNumber, numberOfRowsPerColumn, out this.subColumnSizes, out this.cellHeights, out horSpaceLeft, out vertSpaceLeft);
		}

		private int GetHighestColumnIndex()
		{
			int result = 0;
			int num = -1;
			for (int i = 0; i < this.itemColumns; i++)
			{
				int num2 = 0;
				for (int j = 0; j < this.numberOfRowsPerColumn[i]; j++)
				{
					num2 += this.cellHeights[i, j];
				}
				if (num2 > num)
				{
					num = num2;
					result = i;
				}
			}
			return result;
		}

		private int GetColumnHeight(int columnIndex)
		{
			int num = 0;
			for (int i = 0; i < this.numberOfRowsPerColumn[columnIndex]; i++)
			{
				num += this.cellHeights[columnIndex, i];
			}
			return num;
		}

		private int GetMaximumNumberOfRows()
		{
			return this.GetMaximumNumberOfRows(this.numberOfRowsPerColumn);
		}

		private int GetMaximumNumberOfRows(int[] rowsPerColumn)
		{
			int num = 0;
			if (rowsPerColumn != null)
			{
				for (int i = 0; i < rowsPerColumn.Length; i++)
				{
					num = Math.Max(num, rowsPerColumn[i]);
				}
			}
			return num;
		}

		private bool CheckLegendItemsFit(MapGraphics graph, Size legendItemsAreaSize, int numberOfItemsToCheck, int fontSizeReducedBy, int numberOfColumns, int[] numberOfRowsPerColumn, out int[,] subColumnSizes, out int[,] cellHeights, out int horizontalSpaceLeft, out int verticalSpaceLeft)
		{
			bool result = true;
			horizontalSpaceLeft = 0;
			verticalSpaceLeft = 0;
			if (numberOfItemsToCheck < 0)
			{
				numberOfItemsToCheck = this.legendItems.Count;
			}
			int num = this.GetNumberOfCells();
			int maximumNumberOfRows = this.GetMaximumNumberOfRows(numberOfRowsPerColumn);
			int[,,] array = new int[numberOfColumns, maximumNumberOfRows, num];
			cellHeights = new int[numberOfColumns, maximumNumberOfRows];
			this.singleWCharacterSize = graph.MeasureStringAbs("W", (this.autofitFont == null) ? this.Font : this.autofitFont);
			Size size = graph.MeasureStringAbs("WW", (this.autofitFont == null) ? this.Font : this.autofitFont);
			this.singleWCharacterSize.Width = size.Width - this.singleWCharacterSize.Width;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < numberOfItemsToCheck; i++)
			{
				LegendItem legendItem = this.legendItems[i];
				int num4 = 0;
				for (int j = 0; j < legendItem.Cells.Count; j++)
				{
					LegendCell legendCell = legendItem.Cells[j];
					LegendCellColumn legendCellColumn = null;
					if (j < this.CellColumns.Count)
					{
						legendCellColumn = this.CellColumns[j];
					}
					if (num4 > 0)
					{
						array[num2, num3, j] = -1;
						num4--;
					}
					else
					{
						if (legendCell.CellSpan > 1)
						{
							num4 = legendCell.CellSpan - 1;
						}
						Size size2 = legendCell.MeasureCell(graph, fontSizeReducedBy, (this.autofitFont == null) ? this.Font : this.autofitFont, this.singleWCharacterSize);
						if (legendCellColumn != null)
						{
							if (legendCellColumn.MinimumWidth >= 0)
							{
								size2.Width = (int)Math.Max((float)size2.Width, (float)((float)(legendCellColumn.MinimumWidth * this.singleWCharacterSize.Width) / 100.0));
							}
							if (legendCellColumn.MaximumWidth >= 0)
							{
								size2.Width = (int)Math.Min((float)size2.Width, (float)((float)(legendCellColumn.MaximumWidth * this.singleWCharacterSize.Width) / 100.0));
							}
						}
						int[,,] array2 = array;
						int num5 = num2;
						int num6 = num3;
						int num7 = j;
						int width = size2.Width;
						array2[num5, num6, num7] = width;
						if (j == 0)
						{
							int[,] obj = cellHeights;
							int num8 = num2;
							int num9 = num3;
							int height = size2.Height;
							obj[num8, num9] = height;
						}
						else
						{
							int[,] obj2 = cellHeights;
							int num10 = num2;
							int num11 = num3;
							int num12 = Math.Max(cellHeights[num2, num3], size2.Height);
							obj2[num10, num11] = num12;
						}
					}
				}
				num3++;
				if (num3 >= numberOfRowsPerColumn[num2])
				{
					num2++;
					num3 = 0;
					if (num2 >= numberOfColumns)
					{
						if (i < numberOfItemsToCheck - 1)
						{
							result = false;
						}
						break;
					}
				}
			}
			subColumnSizes = new int[numberOfColumns, num];
			bool flag = false;
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				for (int k = 0; k < num; k++)
				{
					int num13 = 0;
					for (num3 = 0; num3 < numberOfRowsPerColumn[num2]; num3++)
					{
						int num14 = array[num2, num3, k];
						if (num14 < 0)
						{
							flag = true;
							continue;
						}
						if (k + 1 < num)
						{
							int num15 = array[num2, num3, k + 1];
							if (num15 >= 0)
							{
								goto IL_02cf;
							}
							continue;
						}
						goto IL_02cf;
						IL_02cf:
						num13 = Math.Max(num13, num14);
					}
					subColumnSizes[num2, k] = num13;
				}
			}
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				for (int l = 0; l < num; l++)
				{
					if (l < this.CellColumns.Count)
					{
						LegendCellColumn legendCellColumn2 = this.CellColumns[l];
						if (!string.IsNullOrEmpty(legendCellColumn2.HeaderText))
						{
							Size size3 = graph.MeasureStringAbs(legendCellColumn2.HeaderText + "I", legendCellColumn2.HeaderFont);
							if (size3.Width > subColumnSizes[num2, l])
							{
								int[,] obj3 = subColumnSizes;
								int num16 = num2;
								int num17 = l;
								int width2 = size3.Width;
								obj3[num16, num17] = width2;
								if (legendCellColumn2.MinimumWidth >= 0)
								{
									int[,] obj4 = subColumnSizes;
									int num18 = num2;
									int num19 = l;
									int num20 = (int)Math.Max((float)subColumnSizes[num2, l], (float)((float)(legendCellColumn2.MinimumWidth * this.singleWCharacterSize.Width) / 100.0));
									obj4[num18, num19] = num20;
								}
								if (legendCellColumn2.MaximumWidth >= 0)
								{
									int[,] obj5 = subColumnSizes;
									int num21 = num2;
									int num22 = l;
									int num23 = (int)Math.Min((float)subColumnSizes[num2, l], (float)((float)(legendCellColumn2.MaximumWidth * this.singleWCharacterSize.Width) / 100.0));
									obj5[num21, num22] = num23;
								}
							}
						}
					}
				}
			}
			if (flag)
			{
				for (num2 = 0; num2 < numberOfColumns; num2++)
				{
					for (int m = 0; m < num; m++)
					{
						for (num3 = 0; num3 < numberOfRowsPerColumn[num2]; num3++)
						{
							int num24 = array[num2, num3, m];
							int n;
							for (n = 0; m + n + 1 < num; n++)
							{
								int num25 = array[num2, num3, m + n + 1];
								if (num25 >= 0)
								{
									break;
								}
							}
							if (n > 0)
							{
								int num26 = 0;
								for (int num27 = 0; num27 <= n; num27++)
								{
									num26 += subColumnSizes[num2, m + num27];
								}
								if (num24 > num26)
								{
									subColumnSizes[num2, m + n] += num24 - num26;
								}
							}
						}
					}
				}
			}
			if (this.EquallySpacedItems)
			{
				for (int num28 = 0; num28 < num; num28++)
				{
					int num29 = 0;
					for (num2 = 0; num2 < numberOfColumns; num2++)
					{
						num29 = Math.Max(num29, subColumnSizes[num2, num28]);
					}
					for (num2 = 0; num2 < numberOfColumns; num2++)
					{
						subColumnSizes[num2, num28] = num29;
					}
				}
			}
			int num30 = 0;
			int num31 = 0;
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				for (int num32 = 0; num32 < num; num32++)
				{
					num30 += subColumnSizes[num2, num32];
				}
				if (num2 < numberOfColumns - 1)
				{
					num31 += this.GetSeparatorSize(graph, this.ItemColumnSeparator).Width;
				}
			}
			int num33 = 0;
			for (num2 = 0; num2 < numberOfColumns; num2++)
			{
				int num34 = 0;
				for (num3 = 0; num3 < numberOfRowsPerColumn[num2]; num3++)
				{
					num34 += cellHeights[num2, num3];
				}
				num33 = Math.Max(num33, num34);
			}
			horizontalSpaceLeft = legendItemsAreaSize.Width - num30 - num31;
			if (horizontalSpaceLeft < 0)
			{
				result = false;
			}
			verticalSpaceLeft = legendItemsAreaSize.Height - num33;
			if (verticalSpaceLeft < 0)
			{
				result = false;
			}
			return result;
		}

		private int GetNumberOfCells()
		{
			if (this.numberOfCells < 0)
			{
				this.numberOfCells = this.CellColumns.Count;
				foreach (LegendItem legendItem in this.legendItems)
				{
					this.numberOfCells = Math.Max(this.numberOfCells, legendItem.Cells.Count);
				}
			}
			return this.numberOfCells;
		}

		private void FillLegendItemsCollection(SizeF singleWCharacterSize)
		{
			this.legendItems.Clear();
			foreach (LegendItem customLegend in this.customLegends)
			{
				if (customLegend.Visible)
				{
					this.legendItems.Add(customLegend);
				}
			}
			if (this.legendItems.Count == 0 && this.Common != null && this.Common.MapControl != null && this.Common.MapControl.IsDesignMode())
			{
				ColorGenerator colorGenerator = new ColorGenerator();
				Color[] array = colorGenerator.GenerateColors(MapColorPalette.Light, 5);
				for (int i = 0; i < array.Length; i++)
				{
					LegendItem legendItem2 = new LegendItem(string.Format(CultureInfo.CurrentCulture, "LegendItem{0}", i + 1), array[i], "");
					legendItem2.Legend = this;
					legendItem2.Common = this.Common;
					legendItem2.ItemStyle = LegendItemStyle.Shape;
					this.legendItems.Add(legendItem2);
				}
			}
			foreach (LegendItem legendItem3 in this.legendItems)
			{
				legendItem3.AddAutomaticCells(this, singleWCharacterSize);
			}
		}

		internal override void Render(MapGraphics g)
		{
			base.Render(g);
			this.RenderLegend(g);
		}

		private void RenderLegend(MapGraphics g)
		{
			this.offset = System.Drawing.Size.Empty;
			this.itemColumns = 0;
			this.horizontalSpaceLeft = 0;
			this.verticalSpaceLeft = 0;
			this.subColumnSizes = null;
			this.numberOfRowsPerColumn = null;
			this.cellHeights = null;
			this.autofitFont = null;
			this.autoFitFontSizeAdjustment = 0;
			this.numberOfCells = -1;
			this.numberOfLegendItemsToProcess = -1;
			RectangleF relative = new RectangleF(0f, 0f, 100f, 100f);
			this.singleWCharacterSize = g.MeasureStringAbs("W", this.Font);
			Size size = g.MeasureStringAbs("WW", this.Font);
			this.singleWCharacterSize.Width = size.Width - this.singleWCharacterSize.Width;
			this.FillLegendItemsCollection(this.singleWCharacterSize);
			foreach (LegendItem legendItem4 in this.legendItems)
			{
				foreach (LegendCell cell in legendItem4.Cells)
				{
					cell.ResetCache();
				}
			}
			if (this.legendItems.Count != 0)
			{
				this.RecalcLegendInfo(g);
				this.DrawLegendHeader(g);
				this.DrawLegendTitle(g);
				if (this.numberOfLegendItemsToProcess < 0)
				{
					this.numberOfLegendItemsToProcess = this.legendItems.Count;
				}
				for (int i = 0; i < this.numberOfLegendItemsToProcess; i++)
				{
					LegendItem legendItem2 = this.legendItems[i];
					for (int j = 0; j < legendItem2.Cells.Count; j++)
					{
						LegendCell legendCell2 = legendItem2.Cells[j];
						legendCell2.Paint(g, this.autoFitFontSizeAdjustment, this.autofitFont, this.singleWCharacterSize);
					}
					if (legendItem2.Separator != 0 && legendItem2.Cells.Count > 0)
					{
						Rectangle empty = Rectangle.Empty;
						empty.X = legendItem2.Cells[0].cellPosition.Left;
						int num = 0;
						for (int num2 = legendItem2.Cells.Count - 1; num2 >= 0; num2--)
						{
							num = legendItem2.Cells[num2].cellPosition.Right;
							if (num > 0)
							{
								break;
							}
						}
						empty.Width = num - empty.X;
						empty.Y = legendItem2.Cells[0].cellPosition.Bottom;
						empty.Height = this.GetSeparatorSize(g, legendItem2.Separator).Height;
						empty.Intersect(this.legendItemsAreaPosition);
						this.DrawSeparator(g, legendItem2.Separator, legendItem2.SeparatorColor, true, empty);
					}
				}
				if (this.ItemColumnSeparator != 0)
				{
					Rectangle position = Rectangle.Round(g.GetAbsoluteRectangle(relative));
					position.Y += this.GetBorderSize() + this.titlePosition.Height;
					position.Height -= 2 * this.GetBorderSize() + this.titlePosition.Height;
					position.X += this.GetBorderSize() + this.offset.Width;
					position.Width = this.GetSeparatorSize(g, this.ItemColumnSeparator).Width;
					if (this.horizontalSpaceLeft > 0)
					{
						position.X += this.horizontalSpaceLeft / 2;
					}
					if (position.Width > 0 && position.Height > 0)
					{
						for (int k = 0; k < this.itemColumns; k++)
						{
							int num3 = this.GetNumberOfCells();
							for (int l = 0; l < num3; l++)
							{
								position.X += this.subColumnSizes[k, l];
							}
							if (k < this.itemColumns - 1)
							{
								this.DrawSeparator(g, this.ItemColumnSeparator, this.ItemColumnSeparatorColor, false, position);
							}
							position.X += position.Width;
						}
					}
				}
				if (this.legendItemsTruncated && this.legendItemsAreaPosition.Height > this.truncatedDotsSize / 2)
				{
					int num4 = 3;
					int val = this.legendItemsAreaPosition.Width / 3 / num4;
					val = Math.Min(val, 10);
					PointF absolute = new PointF((float)(this.legendItemsAreaPosition.X + this.legendItemsAreaPosition.Width / 2) - (float)val * (float)Math.Floor((float)num4 / 2.0), (float)(this.legendItemsAreaPosition.Bottom + (this.truncatedDotsSize + this.offset.Height) / 2));
					for (int m = 0; m < num4; m++)
					{
						g.DrawMarkerRel(g.GetRelativePoint(absolute), MarkerStyle.Circle, this.truncatedDotsSize, this.TextColor, GradientType.None, MapHatchStyle.None, Color.Empty, MapDashStyle.Solid, Color.Empty, 0, string.Empty, Color.Empty, 0, Color.Empty, RectangleF.Empty);
						absolute.X += (float)val;
					}
				}
				bool processModePaint = this.Common.ProcessModePaint;
				foreach (LegendItem legendItem5 in this.legendItems)
				{
					if (legendItem5.clearTempCells)
					{
						legendItem5.clearTempCells = false;
						legendItem5.Cells.Clear();
					}
				}
			}
		}

		private Size GetTitleSize(MapGraphics chartGraph, Size titleMaxSize)
		{
			Size result = System.Drawing.Size.Empty;
			if (!string.IsNullOrEmpty(this.Title))
			{
				titleMaxSize.Width -= this.GetBorderSize() * 2 + this.offset.Width;
				result = chartGraph.MeasureStringAbs(this.Title.Replace("\\n", "\n"), this.TitleFont, titleMaxSize, new StringFormat());
				result.Height += this.offset.Height;
				result.Width += this.offset.Width;
				result.Height += this.GetSeparatorSize(chartGraph, this.TitleSeparator).Height;
			}
			return result;
		}

		private Size GetHeaderSize(MapGraphics chartGraph, LegendCellColumn legendColumn)
		{
			Size result = System.Drawing.Size.Empty;
			if (!string.IsNullOrEmpty(legendColumn.HeaderText))
			{
				result = chartGraph.MeasureStringAbs(legendColumn.HeaderText.Replace("\\n", "\n") + "I", legendColumn.HeaderFont);
				result.Height += this.offset.Height;
				result.Width += this.offset.Width;
				result.Height += this.GetSeparatorSize(chartGraph, this.HeaderSeparator).Height;
			}
			return result;
		}

		private void DrawLegendHeader(MapGraphics g)
		{
			if (!this.headerPosition.IsEmpty && this.headerPosition.Width > 0 && this.headerPosition.Height > 0)
			{
				int num = -1;
				RectangleF relative = new RectangleF(0f, 0f, 100f, 100f);
				Rectangle rect = Rectangle.Round(g.GetAbsoluteRectangle(relative));
				rect.Y += this.GetBorderSize();
				rect.Height -= 2 * (this.offset.Height + this.GetBorderSize());
				rect.X += this.GetBorderSize();
				rect.Width -= 2 * this.GetBorderSize();
				if (this.GetBorderSize() > 0)
				{
					rect.Height++;
					rect.Width++;
				}
				bool flag = false;
				for (int i = 0; i < this.CellColumns.Count; i++)
				{
					LegendCellColumn legendCellColumn = this.CellColumns[i];
					if (!legendCellColumn.HeaderBackColor.IsEmpty)
					{
						flag = true;
					}
				}
				for (int j = 0; j < this.itemColumns; j++)
				{
					int num2 = 0;
					int num3 = 0;
					int length = this.subColumnSizes.GetLength(1);
					for (int k = 0; k < length; k++)
					{
						Rectangle rectangle = this.headerPosition;
						if (this.horizontalSpaceLeft > 0)
						{
							rectangle.X += (int)((float)this.horizontalSpaceLeft / 2.0);
						}
						if (num != -1)
						{
							rectangle.X = num;
						}
						rectangle.Width = this.subColumnSizes[j, k];
						num = rectangle.Right;
						if (k == 0)
						{
							num2 = rectangle.Left;
						}
						num3 += rectangle.Width;
						rectangle.Intersect(rect);
						Rectangle r;
						if (rectangle.Width > 0 && rectangle.Height > 0)
						{
							r = rectangle;
							if (this.titlePosition.Height <= 0)
							{
								r.Y -= this.offset.Height;
								r.Height += this.offset.Height;
							}
							if (this.itemColumns == 1 && flag)
							{
								goto IL_0238;
							}
							if (this.ItemColumnSeparator != 0)
							{
								goto IL_0238;
							}
							goto IL_034b;
						}
						continue;
						IL_034b:
						if (k < this.CellColumns.Count)
						{
							LegendCellColumn legendCellColumn2 = this.CellColumns[k];
							if (!legendCellColumn2.HeaderBackColor.IsEmpty)
							{
								if (r.Right > rect.Right)
								{
									r.Width -= rect.Right - r.Right;
								}
								if (r.X < rect.X)
								{
									r.X += rect.X - r.X;
									r.Width -= rect.X - r.X;
								}
								r.Intersect(rect);
								g.FillRectangleRel(g.GetRelativeRectangle(r), legendCellColumn2.HeaderBackColor, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, MapDashStyle.None, Color.Empty, 0, PenAlignment.Inset);
							}
							using (SolidBrush brush = new SolidBrush(legendCellColumn2.HeaderColor))
							{
								StringFormat stringFormat = new StringFormat();
								stringFormat.Alignment = legendCellColumn2.HeaderTextAlignment;
								stringFormat.LineAlignment = StringAlignment.Center;
								stringFormat.FormatFlags = StringFormatFlags.LineLimit;
								stringFormat.Trimming = StringTrimming.EllipsisCharacter;
								g.DrawStringRel(legendCellColumn2.HeaderText, legendCellColumn2.HeaderFont, brush, g.GetRelativeRectangle(rectangle), stringFormat);
							}
						}
						continue;
						IL_0238:
						if (j == 0 && k == 0)
						{
							int x = rect.X;
							num3 += num2 - x;
							num2 = x;
							r.Width += r.X - rect.X;
							r.X = x;
						}
						if (j == this.itemColumns - 1 && k == length - 1)
						{
							num3 += rect.Right - r.Right + 1;
							r.Width += rect.Right - r.Right + 1;
						}
						if (j != 0 && k == 0)
						{
							num3 += this.itemColumnSpacingRel / 2;
							num2 -= this.itemColumnSpacingRel / 2;
							r.Width += this.itemColumnSpacingRel / 2;
							r.X -= this.itemColumnSpacingRel / 2;
						}
						if (j != this.itemColumns - 1 && k == length - 1)
						{
							num3 += this.itemColumnSpacingRel / 2;
							r.Width += this.itemColumnSpacingRel / 2;
						}
						goto IL_034b;
					}
					Rectangle position = this.headerPosition;
					position.X = num2;
					position.Width = num3;
					if (this.HeaderSeparator == LegendSeparatorType.Line || this.HeaderSeparator == LegendSeparatorType.DoubleLine)
					{
						rect.Width--;
					}
					position.Intersect(rect);
					this.DrawSeparator(g, this.HeaderSeparator, this.HeaderSeparatorColor, true, position);
					num += this.GetSeparatorSize(g, this.ItemColumnSeparator).Width;
				}
			}
		}

		private void DrawLegendTitle(MapGraphics g)
		{
			if (!string.IsNullOrEmpty(this.Title) && !this.titlePosition.IsEmpty)
			{
				RectangleF relative = new RectangleF(0f, 0f, 100f, 100f);
				Rectangle rect = Rectangle.Round(g.GetAbsoluteRectangle(relative));
				rect.Y += this.GetBorderSize();
				rect.Height -= 2 * this.GetBorderSize();
				rect.X += this.GetBorderSize();
				rect.Width -= 2 * this.GetBorderSize();
				if (this.GetBorderSize() > 0)
				{
					rect.Height++;
					rect.Width++;
				}
				if (!this.TitleBackColor.IsEmpty)
				{
					Rectangle r = this.titlePosition;
					r.Intersect(rect);
					g.FillRectangleRel(g.GetRelativeRectangle(r), this.TitleBackColor, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.None, Color.Empty, Color.Empty, 0, MapDashStyle.None, Color.Empty, 0, PenAlignment.Inset);
				}
				using (SolidBrush brush = new SolidBrush(this.TitleColor))
				{
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = this.TitleAlignment;
					Rectangle r2 = this.titlePosition;
					r2.Y += this.offset.Height;
					r2.X += this.offset.Width;
					r2.X += this.GetBorderSize();
					r2.Width -= this.GetBorderSize() * 2 + this.offset.Width;
					r2.Intersect(rect);
					g.DrawStringRel(this.Title.Replace("\\n", "\n"), this.TitleFont, brush, g.GetRelativeRectangle(r2), stringFormat);
				}
				Rectangle position = this.titlePosition;
				if (this.TitleSeparator == LegendSeparatorType.Line || this.TitleSeparator == LegendSeparatorType.DoubleLine)
				{
					rect.Width--;
				}
				position.Intersect(rect);
				this.DrawSeparator(g, this.TitleSeparator, this.TitleSeparatorColor, true, position);
			}
		}

		internal Size GetSeparatorSize(MapGraphics chartGraph, LegendSeparatorType separatorType)
		{
			Size result = System.Drawing.Size.Empty;
			switch (separatorType)
			{
			case LegendSeparatorType.None:
				result = System.Drawing.Size.Empty;
				break;
			case LegendSeparatorType.Line:
				result = new Size(1, 1);
				break;
			case LegendSeparatorType.DashLine:
				result = new Size(1, 1);
				break;
			case LegendSeparatorType.DotLine:
				result = new Size(1, 1);
				break;
			case LegendSeparatorType.ThickLine:
				result = new Size(2, 2);
				break;
			case LegendSeparatorType.DoubleLine:
				result = new Size(3, 3);
				break;
			case LegendSeparatorType.GradientLine:
				result = new Size(1, 1);
				break;
			case LegendSeparatorType.ThickGradientLine:
				result = new Size(2, 2);
				break;
			default:
				throw new InvalidOperationException("Unknown legend separator type '" + ((Enum)(object)separatorType).ToString((IFormatProvider)CultureInfo.CurrentCulture) + "'.");
			}
			result.Width += this.itemColumnSpacingRel;
			return result;
		}

		private void DrawSeparator(MapGraphics chartGraph, LegendSeparatorType separatorType, Color color, bool horizontal, Rectangle position)
		{
			SmoothingMode smoothingMode = chartGraph.SmoothingMode;
			chartGraph.SmoothingMode = SmoothingMode.None;
			RectangleF rectangleF = position;
			if (!horizontal)
			{
				rectangleF.X += (float)(int)((float)this.itemColumnSpacingRel / 2.0);
				rectangleF.Width -= (float)this.itemColumnSpacingRel;
			}
			switch (separatorType)
			{
			case LegendSeparatorType.Line:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 1.0)));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF((float)(rectangleF.Right - 1.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 1.0), rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DashLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Dash, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 1.0)));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Dash, new PointF((float)(rectangleF.Right - 1.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 1.0), rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DotLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Dot, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 1.0)));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Dot, new PointF((float)(rectangleF.Right - 1.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 1.0), rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.ThickLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 2, MapDashStyle.Solid, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 1.0)));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 2, MapDashStyle.Solid, new PointF((float)(rectangleF.Right - 1.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 1.0), rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.DoubleLine:
				if (horizontal)
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 3.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 3.0)));
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0)), new PointF(rectangleF.Right, (float)(rectangleF.Bottom - 1.0)));
				}
				else
				{
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF((float)(rectangleF.Right - 3.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 3.0), rectangleF.Bottom));
					chartGraph.DrawLineAbs(color, 1, MapDashStyle.Solid, new PointF((float)(rectangleF.Right - 1.0), rectangleF.Top), new PointF((float)(rectangleF.Right - 1.0), rectangleF.Bottom));
				}
				break;
			case LegendSeparatorType.GradientLine:
				if (horizontal)
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Left, (float)(rectangleF.Bottom - 1.0), rectangleF.Width, 0f), Color.Transparent, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.VerticalCenter, color, Color.Empty, 0, MapDashStyle.None, PenAlignment.Inset);
				}
				else
				{
					chartGraph.FillRectangleAbs(new RectangleF((float)(rectangleF.Right - 1.0), rectangleF.Top, 0f, rectangleF.Height), Color.Transparent, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.HorizontalCenter, color, Color.Empty, 0, MapDashStyle.None, PenAlignment.Inset);
				}
				break;
			case LegendSeparatorType.ThickGradientLine:
				if (horizontal)
				{
					chartGraph.FillRectangleAbs(new RectangleF(rectangleF.Left, (float)(rectangleF.Bottom - 2.0), rectangleF.Width, 1f), Color.Transparent, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.VerticalCenter, color, Color.Empty, 0, MapDashStyle.None, PenAlignment.Inset);
				}
				else
				{
					chartGraph.FillRectangleAbs(new RectangleF((float)(rectangleF.Right - 2.0), rectangleF.Top, 1f, rectangleF.Height), Color.Transparent, MapHatchStyle.None, string.Empty, MapImageWrapMode.Tile, Color.Empty, MapImageAlign.Center, GradientType.HorizontalCenter, color, Color.Empty, 0, MapDashStyle.None, PenAlignment.Inset);
				}
				break;
			}
			chartGraph.SmoothingMode = smoothingMode;
		}

		private int GetBorderSize()
		{
			return 0;
		}

		private LegendTableStyle GetLegendTableStyle(MapGraphics g)
		{
			LegendTableStyle tableStyle = this.TableStyle;
			if (this.TableStyle == LegendTableStyle.Auto)
			{
				if (this.Dock != 0)
				{
					if (this.Dock != PanelDockStyle.Left && this.Dock != PanelDockStyle.Right)
					{
						return LegendTableStyle.Wide;
					}
					return LegendTableStyle.Tall;
				}
				SizeF size = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)).Size;
				if (size.Width < size.Height)
				{
					return LegendTableStyle.Tall;
				}
				return LegendTableStyle.Wide;
			}
			return tableStyle;
		}

		internal override bool IsRenderVisible(MapGraphics g, RectangleF clipRect)
		{
			if (base.AutoSize && this.MaxAutoSize == 0.0)
			{
				return false;
			}
			return base.IsRenderVisible(g, clipRect);
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Dock":
				return PanelDockStyle.Right;
			case "DockAlignment":
				return DockAlignment.Near;
			case "SizeUnit":
				return CoordinateUnit.Percent;
			case "BackColor":
				return Color.White;
			case "BorderColor":
				return Color.DarkGray;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		internal override void Invalidate()
		{
			this.InvalidateAndLayout();
		}
	}
}
