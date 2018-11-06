using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Models;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLCellModel : ICellModel, IOoxmlCtWrapperModel
	{
		private Cell _interface;

		private readonly CT_Cell _cell;

		private ICellController _controller;

		public Cell Interface
		{
			get
			{
				if (this._interface == null)
				{
					this._interface = new Cell(this);
				}
				return this._interface;
			}
		}

		public CT_Cell Data
		{
			get
			{
				return this._cell;
			}
		}

		public Cell.CellValueType ValueType
		{
			get
			{
				return this._controller.ValueType;
			}
		}

		public object Value
		{
			get
			{
				return this._controller.Value;
			}
			set
			{
				this._controller.Value = value;
			}
		}

		public IStyleModel Style
		{
			get
			{
				return this._controller.Style;
			}
			set
			{
				this._controller.Style = value;
			}
		}

		public string Name
		{
			get
			{
				return this._cell.R_Attr;
			}
		}

		public OoxmlComplexType OoxmlTag
		{
			get
			{
				return this._cell;
			}
		}

		public XMLCellModel(XMLWorksheetModel sheet, PartManager manager, CT_Cell cell)
		{
			this._cell = cell;
			this._controller = new XMLCellController(cell, sheet, manager);
			this.FixCellValue();
		}

		private void FixCellValue()
		{
			if (this._cell.T_Attr != ST_CellType.str)
			{
				this._cell.V = null;
				this._cell.T_Attr = ST_CellType.str;
			}
		}

		public ICharacterRunModel getCharacters(int startIndex, int length)
		{
			return this._controller.CharManager.CreateRun(startIndex, length);
		}

		public void Cleanup()
		{
			this._controller.Cleanup();
		}
	}
}
