using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Models;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using System.IO.Packaging;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLHyperlinkModel : IOoxmlCtWrapperModel
	{
		private readonly CT_Hyperlink _link;

		private bool _isInternal;

		private readonly PartManager _manager;

		private readonly CT_Sheet _worksheetEntry;

		public OoxmlComplexType OoxmlTag
		{
			get
			{
				return this._link;
			}
		}

		public XMLHyperlinkModel(string area, string href, string text, PartManager manager, CT_Sheet entry)
		{
			this._link = new CT_Hyperlink();
			this._link._ref_Attr = area;
			this._link.Location_Attr = href;
			this._link.Display_Attr = text;
			this._isInternal = !href.Contains("://");
			this._manager = manager;
			this._worksheetEntry = entry;
		}

		public void Cleanup()
		{
			if (!this._isInternal)
			{
				Relationship relationship = this._manager.AddExternalPartToTree("http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink", this._link.Location_Attr, this._manager.GetWorksheetXmlPart(this._worksheetEntry), TargetMode.External);
				this._link.Location_Attr = null;
				this._link.Id_Attr = relationship.RelationshipId;
			}
		}
	}
}
