using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Models;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLRowModel : IRowModel, IOoxmlCtWrapperModel
	{
		private Row _rowInterface;

		private readonly XMLWorksheetModel _worksheetModel;

		private readonly CT_Row _row;

		private readonly PartManager _manager;

		private IDictionary<int, ICellModel> _cells;

		public Row RowInterface
		{
			get
			{
				if (this._rowInterface == null)
				{
					this._rowInterface = new Row(this);
				}
				return this._rowInterface;
			}
		}

		public IWorksheetModel WorksheetModel
		{
			get
			{
				if (this._worksheetModel == null)
				{
					throw new FatalException();
				}
				return this._worksheetModel;
			}
		}

		public IDictionary<int, ICellModel> CellsMap
		{
			get
			{
				return this._cells;
			}
		}

		public int RowNumber
		{
			get
			{
				return (int)(this._row.R_Attr - 1);
			}
			set
			{
				this._row.R_Attr = (uint)(value + 1);
			}
		}

		public double Height
		{
			set
			{
				this._row.Ht_Attr = value;
				this._row.CustomHeight_Attr = true;
			}
		}

		public bool CustomHeight
		{
			set
			{
				this._row.CustomHeight_Attr = value;
			}
		}

		public int OutlineLevel
		{
			set
			{
				if (value >= 0 && value <= 7)
				{
					this._row.OutlineLevel_Attr = (byte)value;
					return;
				}
				throw new FatalException();
			}
		}

		public bool Hidden
		{
			set
			{
				this._row.Hidden_Attr = value;
			}
		}

		public bool OutlineCollapsed
		{
			set
			{
				this._row.Collapsed_Attr = value;
			}
		}

		public OoxmlComplexType OoxmlTag
		{
			get
			{
				return this._row;
			}
		}

		public XMLRowModel(XMLStreamsheetModel sheet, PartManager manager, int rowNumber)
		{
			this._worksheetModel = sheet;
			this._manager = manager;
			this._row = new CT_Row();
			this._row.C = new List<CT_Cell>();
			this.RowNumber = rowNumber;
			this.Init();
		}

		public ICellModel getCell(int col)
		{
			if (col >= 0 && col <= 16383)
			{
				ICellModel cellModel = default(ICellModel);
				if (this.CellsMap.TryGetValue(col, out cellModel))
				{
					return cellModel;
				}
				CT_Cell cT_Cell = new CT_Cell();
				cT_Cell.R_Attr = CellPair.Name(this.RowNumber, col);
				cellModel = new XMLCellModel(this._worksheetModel, this._manager, cT_Cell);
				this.CellsMap.Add(col, cellModel);
				return cellModel;
			}
			throw new FatalException();
		}

		public void ClearCell(int column)
		{
			if (this.CellsMap.ContainsKey(column))
			{
				this.CellsMap.Remove(column);
			}
		}

		private void Init()
		{
			this._cells = new Dictionary<int, ICellModel>();
			this._row.C.Clear();
		}

		public void Cleanup()
		{
			List<int> list = new List<int>(this.CellsMap.Keys);
			list.Sort();
			foreach (int item in list)
			{
				XMLCellModel xMLCellModel = (XMLCellModel)this.CellsMap[item];
				xMLCellModel.Cleanup();
				this._row.C.Add(xMLCellModel.Data);
			}
			this.CellsMap.Clear();
		}
	}
}
