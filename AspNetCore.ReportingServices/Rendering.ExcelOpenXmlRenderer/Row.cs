using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Row
	{
		private readonly IRowModel _model;

		public Cell this[int index]
		{
			get
			{
				return this._model.getCell(index).Interface;
			}
		}

		public double Height
		{
			set
			{
				this._model.Height = value;
			}
		}

		public bool Hidden
		{
			set
			{
				this._model.Hidden = value;
			}
		}

		public bool OutlineCollapsed
		{
			set
			{
				this._model.OutlineCollapsed = value;
			}
		}

		public int OutlineLevel
		{
			set
			{
				this._model.OutlineLevel = value;
			}
		}

		public int RowNumber
		{
			get
			{
				return this._model.RowNumber;
			}
		}

		public bool CustomHeight
		{
			set
			{
				this._model.CustomHeight = value;
			}
		}

		internal Row(IRowModel model)
		{
			this._model = model;
		}

		public void ClearCell(int column)
		{
			this._model.ClearCell(column);
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Row)
			{
				if (obj == this)
				{
					return true;
				}
				Row row = (Row)obj;
				return row._model.Equals(this._model);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this._model.GetHashCode();
		}
	}
}
