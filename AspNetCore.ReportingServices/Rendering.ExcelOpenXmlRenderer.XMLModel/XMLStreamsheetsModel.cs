using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLStreamsheetsModel : XMLWorksheetsModel
	{
		private XMLStreamsheetModel _currentSheet;

		public XMLStreamsheetsModel(XMLWorkbookModel workbook, PartManager manager)
			: base(workbook, manager)
		{
		}

		public override IStreamsheetModel CreateStreamsheet(string sheetName, ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			return this.CreateSheet(sheetName, createTempStream);
		}

		private XMLStreamsheetModel CreateSheet(string sheetName, ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			XMLStreamsheetModel xMLStreamsheetModel = new XMLStreamsheetModel((XMLStreambookModel)base.Workbook, this, base.Manager, sheetName, createTempStream);
			base.Worksheets.Add(xMLStreamsheetModel);
			this._currentSheet = xMLStreamsheetModel;
			return xMLStreamsheetModel;
		}
	}
}
