using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.IO;
using System.IO.Packaging;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLStreambookModel : XMLWorkbookModel, IStreambookModel, IWorkbookModel
	{
		private Stream _outputStream;

		private Package _zipPackage;

		public override IWorksheetsModel Worksheets
		{
			get
			{
				if (base._manager == null)
				{
					throw new FatalException();
				}
				if (base._worksheets == null)
				{
					base._worksheets = new XMLStreamsheetsModel(this, base._manager);
					base._worksheets.NextId = (uint)(base._worksheets.Count + 1);
				}
				return base._worksheets;
			}
		}

		public Package ZipPackage
		{
			get
			{
				if (this._zipPackage == null)
				{
					this._zipPackage = Package.Open(this._outputStream, FileMode.Create);
				}
				return this._zipPackage;
			}
		}

		public XMLStreambookModel(Stream outputStream)
		{
			this._outputStream = outputStream;
			base._manager = new PartManager(this);
			base._nameManager = new XMLDefinedNamesManager((CT_Workbook)base._manager.Workbook.Root);
		}

		public void Save()
		{
			base._manager.Write();
		}
	}
}
