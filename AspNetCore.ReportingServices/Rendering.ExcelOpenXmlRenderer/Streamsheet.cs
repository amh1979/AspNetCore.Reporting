using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Streamsheet
	{
		private readonly IStreamsheetModel _model;

		public string Name
		{
			get
			{
				return this._model.Name;
			}
			set
			{
				this._model.Name = value;
			}
		}

		public Pictures Pictures
		{
			get
			{
				return this._model.Pictures.Interface;
			}
		}

		public PageSetup PageSetup
		{
			get
			{
				return this._model.PageSetup.Interface;
			}
		}

		public bool ShowGridlines
		{
			set
			{
				this._model.ShowGridlines = value;
			}
		}

		public int MaxRowIndex
		{
			get
			{
				return this._model.MaxRowIndex;
			}
		}

		public int MaxColIndex
		{
			get
			{
				return this._model.MaxColIndex;
			}
		}

		public Streamsheet(IStreamsheetModel model)
		{
			this._model = model;
		}

		public Row CreateRow()
		{
			return this._model.CreateRow().RowInterface;
		}

		public Row CreateRow(int index)
		{
			return this._model.CreateRow(index).RowInterface;
		}

		public ColumnProperties GetColumnProperties(int columnIndex)
		{
			return this._model.getColumn(columnIndex).Interface;
		}

		public void CreateHyperlink(string areaFormula, string href, string label)
		{
			this._model.CreateHyperlink(areaFormula, href, label);
		}

		public Anchor CreateAnchor(int rowNumber, int columnNumber, double offsetX, double offsetY)
		{
			return this._model.createAnchor(rowNumber, columnNumber, offsetX, offsetY).Interface;
		}

		public void MergeCells(int firstRow, int firstCol, int rowCount, int colCount)
		{
			this._model.MergeCells(firstRow, firstCol, rowCount, colCount);
		}

		public void SetFreezePanes(int row, int col)
		{
			this._model.SetFreezePanes(row, col);
		}

		public void SetBackgroundPicture(string uniqueId, string extension, Stream pictureStream)
		{
			this._model.SetBackgroundPicture(uniqueId, extension, pictureStream);
		}

		public void Cleanup()
		{
			this._model.Cleanup();
		}
	}
}
