using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_Drawing : OoxmlComplexType
	{
		private List<CT_TwoCellAnchor> _twoCellAnchor;

		public List<CT_TwoCellAnchor> TwoCellAnchor
		{
			get
			{
				return this._twoCellAnchor;
			}
			set
			{
				this._twoCellAnchor = value;
			}
		}

		public static string TwoCellAnchorElementName
		{
			get
			{
				return "twoCellAnchor";
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
			this._twoCellAnchor = new List<CT_TwoCellAnchor>();
		}

		public override void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			this.WriteOpenTag(s, tagName, depth, namespaces, true);
			this.WriteElements(s, depth, namespaces);
			this.WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			this.WriteOpenTag(s, tagName, depth, namespaces, false);
			this.WriteElements(s, depth, namespaces);
			this.WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
			s.Write(tagName);
			this.WriteAttributes(s);
			if (root)
			{
				foreach (string key in namespaces.Keys)
				{
					s.Write(" xmlns");
					if (namespaces[key] != "")
					{
						s.Write(":");
						s.Write(namespaces[key]);
					}
					s.Write("=\"");
					s.Write(key);
					s.Write("\"");
				}
			}
			s.Write(">");
		}

		public override void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_twoCellAnchor(s, depth, namespaces);
		}

		public void Write_twoCellAnchor(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._twoCellAnchor != null)
			{
				foreach (CT_TwoCellAnchor item in this._twoCellAnchor)
				{
					if (item != null)
					{
						item.Write(s, "twoCellAnchor", depth + 1, namespaces);
					}
				}
			}
		}
	}
}
