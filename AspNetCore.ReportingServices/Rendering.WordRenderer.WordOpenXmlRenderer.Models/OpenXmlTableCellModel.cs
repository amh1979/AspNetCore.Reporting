using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableCellModel
	{
		private enum BorderOverride : byte
		{
			UseCellBorder,
			ClearBorder,
			UseTableBorder
		}

		internal interface ICellContent
		{
			void Write(TextWriter writer);
		}

		private readonly OpenXmlTableCellModel _containingCell;

		private readonly OpenXmlTablePropertiesModel _tableProperties;

		private IList<ICellContent> _contents;

		private TextWriter _textWriter;

		private BorderOverride _overrideTopBorder;

		private BorderOverride _overrideBottomBorder;

		private BorderOverride _overrideLeftBorder;

		private BorderOverride _overrideRightBorder;

		private OpenXmlTableCellPropertiesModel _cellProperties;

		public OpenXmlTableCellModel ContainingCell
		{
			get
			{
				return this._containingCell;
			}
		}

		public OpenXmlTableCellPropertiesModel CellProperties
		{
			get
			{
				return this._cellProperties;
			}
		}

		public OpenXmlTableCellModel(OpenXmlTableCellModel containingCell, OpenXmlTablePropertiesModel tableProperties, TextWriter textWriter)
		{
			this._containingCell = containingCell;
			this._tableProperties = tableProperties;
			this._textWriter = textWriter;
			this._contents = new List<ICellContent>();
			this._textWriter.Write("<w:tc>");
			this._cellProperties = new OpenXmlTableCellPropertiesModel();
		}

		public void BlockBorderAt(TableData.Positions side)
		{
			switch (side)
			{
			case TableData.Positions.Top:
				this.ClearBorder(ref this._overrideTopBorder);
				break;
			case TableData.Positions.Bottom:
				this.ClearBorder(ref this._overrideBottomBorder);
				break;
			case TableData.Positions.Left:
				this.ClearBorder(ref this._overrideLeftBorder);
				break;
			case TableData.Positions.Right:
				this.ClearBorder(ref this._overrideRightBorder);
				break;
			}
		}

		public void UseTopTableBorder()
		{
			this._overrideTopBorder = BorderOverride.UseTableBorder;
		}

		public void UseBottomTableBorder()
		{
			this._overrideBottomBorder = BorderOverride.UseTableBorder;
		}

		public void UseLeftTableBorder()
		{
			this._overrideLeftBorder = BorderOverride.UseTableBorder;
		}

		public void UseRightTableBorder()
		{
			this._overrideRightBorder = BorderOverride.UseTableBorder;
		}

		private void ClearBorder(ref BorderOverride borderOverride)
		{
			if (borderOverride == BorderOverride.UseCellBorder)
			{
				borderOverride = BorderOverride.ClearBorder;
			}
		}

		private void Flush()
		{
			if (this._cellProperties != null)
			{
				this.OverrideBorders();
				this.CellProperties.Write(this._textWriter);
				this._cellProperties = null;
			}
			foreach (ICellContent content in this._contents)
			{
				content.Write(this._textWriter);
			}
			this._contents = new List<ICellContent>();
		}

		private void OverrideBorders()
		{
			if (this._overrideTopBorder == BorderOverride.ClearBorder)
			{
				this.CellProperties.ClearBorderTop();
			}
			else if (this._overrideTopBorder == BorderOverride.UseTableBorder)
			{
				this.UpdateBorder(this.CellProperties.BorderTop, this._tableProperties.BorderTop);
			}
			if (this._overrideBottomBorder == BorderOverride.ClearBorder)
			{
				this.CellProperties.ClearBorderBottom();
			}
			else if (this._overrideBottomBorder == BorderOverride.UseTableBorder)
			{
				this.UpdateBorder(this.CellProperties.BorderBottom, this._tableProperties.BorderBottom);
			}
			if (this._overrideLeftBorder == BorderOverride.ClearBorder)
			{
				this.CellProperties.ClearBorderLeft();
			}
			else if (this._overrideLeftBorder == BorderOverride.UseTableBorder)
			{
				this.UpdateBorder(this.CellProperties.BorderLeft, this._tableProperties.BorderLeft);
			}
			if (this._overrideRightBorder == BorderOverride.ClearBorder)
			{
				this.CellProperties.ClearBorderRight();
			}
			else if (this._overrideRightBorder == BorderOverride.UseTableBorder)
			{
				this.UpdateBorder(this.CellProperties.BorderRight, this._tableProperties.BorderRight);
			}
		}

		private void UpdateBorder(OpenXmlBorderPropertiesModel cellBorder, OpenXmlBorderPropertiesModel tableBorder)
		{
			cellBorder.Color = tableBorder.Color;
			cellBorder.Style = tableBorder.Style;
			cellBorder.WidthInEighthPoints = tableBorder.WidthInEighthPoints;
		}

		public void PrepareForNestedTable()
		{
			this.Flush();
		}

		public void AddContent(ICellContent openXmlParagraphModel)
		{
			this._contents.Add(openXmlParagraphModel);
		}

		public void WriteCloseTag(bool emptyLayoutCell)
		{
			bool flag = this._contents.Count > 0;
			this.Flush();
			if (!flag)
			{
				if (emptyLayoutCell)
				{
					OpenXmlParagraphModel.WriteEmptyLayoutCellParagraph(this._textWriter);
				}
				else
				{
					OpenXmlParagraphModel.WriteEmptyParagraph(this._textWriter);
				}
			}
			this._textWriter.Write("</w:tc>");
		}
	}
}
