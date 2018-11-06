using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_PositiveSize2D : OoxmlComplexType, IOoxmlComplexType
	{
		private long _cx_attr;

		private long _cy_attr;

		public long Cx_Attr
		{
			get
			{
				return this._cx_attr;
			}
			set
			{
				this._cx_attr = value;
			}
		}

		public long Cy_Attr
		{
			get
			{
				return this._cy_attr;
			}
			set
			{
				this._cy_attr = value;
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
			s.Write(" cx=\"");
			OoxmlComplexType.WriteData(s, this._cx_attr);
			s.Write("\"");
			s.Write(" cy=\"");
			OoxmlComplexType.WriteData(s, this._cy_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
