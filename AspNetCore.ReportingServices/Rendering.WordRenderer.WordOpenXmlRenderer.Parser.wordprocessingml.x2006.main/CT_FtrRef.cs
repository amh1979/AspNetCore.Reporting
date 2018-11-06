using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_FtrRef : CT_Rel, IEG_HdrFtrReferences, IOoxmlComplexType
	{
		private ST_HdrFtr _type_attr;

		public override GeneratedType GroupInterfaceType
		{
			get
			{
				return GeneratedType.CT_FtrRef;
			}
		}

		public ST_HdrFtr Type_Attr
		{
			get
			{
				return this._type_attr;
			}
			set
			{
				this._type_attr = value;
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void Write(TextWriter s, string tagName)
		{
			this.WriteOpenTag(s, tagName, null);
			this.WriteElements(s);
			this.WriteCloseTag(s, tagName);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			base.WriteOpenTag(s, tagName, "w", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</w:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			base.WriteAttributes(s);
			s.Write(" w:type=\"");
			OoxmlComplexType.WriteData(s, this._type_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			base.WriteElements(s);
		}
	}
}
