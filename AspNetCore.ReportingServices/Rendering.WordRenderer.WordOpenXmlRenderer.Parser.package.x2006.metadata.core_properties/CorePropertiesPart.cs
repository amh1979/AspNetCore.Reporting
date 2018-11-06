using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.package.x2006.metadata.core_properties
{
	internal class CorePropertiesPart : OoxmlPart
	{
		private CT_CoreProperties _root;

		protected static readonly string _tag = "coreProperties";

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
				return CorePropertiesPart._tag;
			}
		}

		public override Dictionary<string, string> Namespaces
		{
			get
			{
				return this._namespaces;
			}
		}

		public CorePropertiesPart()
		{
			this.InitNamespaces();
			this._root = new CT_CoreProperties();
		}

		private void InitNamespaces()
		{
			this._namespaces = new Dictionary<string, string>();
			this._namespaces["http://schemas.openxmlformats.org/markup-compatibility/2006"] = "ve";
			this._namespaces["urn:schemas-microsoft-com:office:office"] = "o";
			this._namespaces["http://schemas.openxmlformats.org/officeDocument/2006/math"] = "m";
			this._namespaces["urn:schemas-microsoft-com:vml"] = "v";
			this._namespaces["http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing"] = "wp";
			this._namespaces["urn:schemas-microsoft-com:office:word"] = "w10";
			this._namespaces["http://schemas.openxmlformats.org/wordprocessingml/2006/main"] = "w";
			this._namespaces["http://schemas.microsoft.com/office/word/2006/wordml"] = "wne";
			this._namespaces["http://schemas.openxmlformats.org/officeDocument/2006/relationships"] = "r";
			this._namespaces["http://schemas.openxmlformats.org/package/2006/metadata/core-properties"] = "cp";
			this._namespaces["http://purl.org/dc/elements/1.1/"] = "dc";
			this._namespaces["http://schemas.openxmlformats.org/drawingml/2006/main"] = "a";
			this._namespaces["http://schemas.openxmlformats.org/drawingml/2006/picture"] = "pic";
			this._namespaces["http://www.w3.org/2001/XMLSchema"] = "";
		}
	}
}
