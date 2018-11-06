using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_PresetGeometry2D : OoxmlComplexType
	{
		private ST_ShapeType _prst_attr;

		private CT_GeomGuideList _avLst;

		public ST_ShapeType Prst_Attr
		{
			get
			{
				return this._prst_attr;
			}
			set
			{
				this._prst_attr = value;
			}
		}

		public CT_GeomGuideList AvLst
		{
			get
			{
				return this._avLst;
			}
			set
			{
				this._avLst = value;
			}
		}

		public static string AvLstElementName
		{
			get
			{
				return "avLst";
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" prst=\"");
			OoxmlComplexType.WriteData(s, this._prst_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_avLst(s, depth, namespaces);
		}

		public void Write_avLst(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._avLst != null)
			{
				this._avLst.Write(s, "avLst", depth + 1, namespaces);
			}
		}
	}
}
