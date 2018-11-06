using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class ExcelApplication
	{
		public virtual Workbook CreateStreaming(Stream outputStream)
		{
			return new Workbook(new XMLStreambookModel(outputStream));
		}

		public virtual void Save(Workbook workbook)
		{
			IStreambookModel model = workbook.Model;
			model.Save();
		}
	}
}
