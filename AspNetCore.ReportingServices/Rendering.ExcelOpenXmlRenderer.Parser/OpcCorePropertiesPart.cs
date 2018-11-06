using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser
{
	internal class OpcCorePropertiesPart : OoxmlPart
	{
		private OpcCoreProperties _root;

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
				return "coreProperties";
			}
		}

		public override Dictionary<string, string> Namespaces
		{
			get
			{
				return this._namespaces;
			}
		}

		public OpcCorePropertiesPart()
		{
			this._root = new OpcCoreProperties();
			this.InitNamespaces();
		}

		private void InitNamespaces()
		{
			this._namespaces = new Dictionary<string, string>();
			this._namespaces["cp"] = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
			this._namespaces["dc"] = "http://purl.org/dc/elements/1.1/";
			this._namespaces["dcterms"] = "http://purl.org/dc/terms/";
			this._namespaces["dcmitype"] = "http://purl.org/dc/dcmitype/";
			this._namespaces["xsi"] = "http://www.w3.org/2001/XMLSchema-instance";
		}
	}
}
