using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Styles : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_DocDefaults _docDefaults;

		private List<CT_Style> _style;

		public CT_DocDefaults DocDefaults
		{
			get
			{
				return this._docDefaults;
			}
			set
			{
				this._docDefaults = value;
			}
		}

		public List<CT_Style> Style
		{
			get
			{
				return this._style;
			}
			set
			{
				this._style = value;
			}
		}

		public static string DocDefaultsElementName
		{
			get
			{
				return "docDefaults";
			}
		}

		public static string StyleElementName
		{
			get
			{
				return "style";
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
			this._style = new List<CT_Style>();
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
			this.Write_docDefaults(s);
			this.Write_style(s);
		}

		public void Write_docDefaults(TextWriter s)
		{
			if (this._docDefaults != null)
			{
				this._docDefaults.Write(s, "docDefaults");
			}
		}

		public void Write_style(TextWriter s)
		{
			if (this._style != null)
			{
				foreach (CT_Style item in this._style)
				{
					if (item != null)
					{
						item.Write(s, "style");
					}
				}
			}
		}
	}
}
