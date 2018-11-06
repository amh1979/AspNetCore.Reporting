using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_RelativeRect : OoxmlComplexType, IOoxmlComplexType
	{
		private string _r_attr;

		private string _b_attr;

		public string R_Attr
		{
			get
			{
				return this._r_attr;
			}
			set
			{
				this._r_attr = value;
			}
		}

		public string B_Attr
		{
			get
			{
				return this._b_attr;
			}
			set
			{
				this._b_attr = value;
			}
		}

		protected override void InitAttributes()
		{
			this._r_attr = "0%";
			this._b_attr = "0%";
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void Write(TextWriter s, string tagName)
		{
			base.WriteEmptyTag(s, tagName, "a");
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			base.WriteOpenTag(s, tagName, "a", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</a:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (this._r_attr != "0%")
			{
				s.Write(" r=\"");
				OoxmlComplexType.WriteData(s, this._r_attr);
				s.Write("\"");
			}
			if (this._b_attr != "0%")
			{
				s.Write(" b=\"");
				OoxmlComplexType.WriteData(s, this._b_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
