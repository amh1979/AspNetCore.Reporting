using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class StyleSheetPart : OoxmlPart
	{
		private CT_Stylesheet _root;

		protected static readonly string _tag = "styleSheet";

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
				return StyleSheetPart._tag;
			}
		}

		public override Dictionary<string, string> Namespaces
		{
			get
			{
				return this._namespaces;
			}
		}

		public StyleSheetPart()
		{
			this.InitNamespaces();
			this._root = new CT_Stylesheet();
		}

		private void InitNamespaces()
		{
			this._namespaces = new Dictionary<string, string>();
			this._namespaces["http://schemas.openxmlformats.org/spreadsheetml/2006/main"] = "";
			this._namespaces["http://schemas.openxmlformats.org/officeDocument/2006/relationships"] = "r";
			this._namespaces["http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing"] = "xdr";
			this._namespaces["http://schemas.openxmlformats.org/officeDocument/2006/sharedTypes"] = "s";
			this._namespaces["http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes"] = "vt";
			this._namespaces["http://schemas.openxmlformats.org/drawingml/2006/main"] = "a";
			this._namespaces["http://schemas.openxmlformats.org/drawingml/2006/chartDrawing"] = "cdr";
		}
	}
}
