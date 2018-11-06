using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class ToPart : OoxmlPart
	{
		private CT_Marker _root;

		protected static readonly string _tag = "to";

		private Dictionary<string, string> _namespaces;

		public override OoxmlComplexType Root
		{
			get
			{
				return this._root;
			}
		}

		public override string Tag
		{
			get
			{
				return ToPart._tag;
			}
		}

		public override Dictionary<string, string> Namespaces
		{
			get
			{
				return this._namespaces;
			}
		}

		public ToPart()
		{
			this.InitNamespaces();
			this._root = new CT_Marker();
		}

		private void InitNamespaces()
		{
			this._namespaces = new Dictionary<string, string>();
			this._namespaces["http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing"] = "";
			this._namespaces["http://schemas.openxmlformats.org/officeDocument/2006/relationships"] = "r";
			this._namespaces["http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing"] = "xdr";
			this._namespaces["http://schemas.openxmlformats.org/officeDocument/2006/sharedTypes"] = "s";
			this._namespaces["http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes"] = "vt";
			this._namespaces["http://schemas.openxmlformats.org/drawingml/2006/main"] = "a";
			this._namespaces["http://schemas.openxmlformats.org/drawingml/2006/chartDrawing"] = "cdr";
		}
	}
}
