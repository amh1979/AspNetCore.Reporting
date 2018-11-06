using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Worksheets
	{
		private readonly IWorksheetsModel _model;

		public int Count
		{
			get
			{
				return this._model.Count;
			}
		}

		internal Worksheets(IWorksheetsModel model)
		{
			this._model = model;
		}

		public Streamsheet CreateStreamsheet(string name, ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			return this._model.CreateStreamsheet(name, createTempStream).Interface;
		}

		public override bool Equals(object obj)
		{
			Worksheets worksheets = obj as Worksheets;
			if (obj == null)
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return worksheets._model.Equals(this._model);
		}

		public override int GetHashCode()
		{
			return this._model.GetHashCode();
		}
	}
}
