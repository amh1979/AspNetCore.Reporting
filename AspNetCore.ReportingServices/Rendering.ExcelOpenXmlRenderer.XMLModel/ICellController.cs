using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal interface ICellController
	{
		object Value
		{
			get;
			set;
		}

		Cell.CellValueType ValueType
		{
			get;
		}

		IStyleModel Style
		{
			get;
			set;
		}

		XMLCharacterRunManager CharManager
		{
			get;
		}

		void Cleanup();
	}
}
