using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Document : CT_DocumentBase, IOoxmlComplexType
	{
		private CT_Body _body;

		public CT_Body Body
		{
			get
			{
				return this._body;
			}
			set
			{
				this._body = value;
			}
		}

		public static string BodyElementName
		{
			get
			{
				return "body";
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
		}

		public override void WriteElements(TextWriter s)
		{
			base.WriteElements(s);
			this.Write_body(s);
		}

		public void Write_body(TextWriter s)
		{
			if (this._body != null)
			{
				this._body.Write(s, "body");
			}
		}
	}
}
