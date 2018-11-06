using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_OnOff : OoxmlComplexType, IOoxmlComplexType
	{
		private bool _val_attr;

		public bool Val_Attr
		{
			get
			{
				return this._val_attr;
			}
			set
			{
				this._val_attr = value;
			}
		}

		protected override void InitAttributes()
		{
			this._val_attr = true;
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void Write(TextWriter s, string tagName)
		{
			base.WriteEmptyTag(s, tagName, "w");
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
			if (!this._val_attr)
			{
				s.Write(" w:val=\"");
				OoxmlComplexType.WriteData(s, this._val_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
