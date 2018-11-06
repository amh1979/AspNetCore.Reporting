using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_RPrDefault : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_RPr _rPr;

		public CT_RPr RPr
		{
			get
			{
				return this._rPr;
			}
			set
			{
				this._rPr = value;
			}
		}

		public static string RPrElementName
		{
			get
			{
				return "rPr";
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
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_rPr(s);
		}

		public void Write_rPr(TextWriter s)
		{
			if (this._rPr != null)
			{
				this._rPr.Write(s, "rPr");
			}
		}
	}
}
