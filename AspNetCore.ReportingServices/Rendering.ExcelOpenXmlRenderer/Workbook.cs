using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Workbook
	{
		private readonly IStreambookModel _model;

		public Streamsheet this[int sheetOffset]
		{
			get
			{
				return this._model.getWorksheet(sheetOffset).Interface;
			}
		}

		internal IStreambookModel Model
		{
			get
			{
				return this._model;
			}
		}

		public Palette Palette
		{
			get
			{
				return this._model.Palette.Interface;
			}
		}

		public Worksheets Worksheets
		{
			get
			{
				return this._model.Worksheets.Interface;
			}
		}

		internal Workbook(IStreambookModel model)
		{
			this._model = model;
		}

		public GlobalStyle CreateStyle()
		{
			return this._model.createGlobalStyle().GlobalInterface;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Workbook)
			{
				if (obj == this)
				{
					return true;
				}
				Workbook workbook = (Workbook)obj;
				return workbook._model.Equals(this._model);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this._model.GetHashCode();
		}
	}
}
