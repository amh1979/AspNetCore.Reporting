using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_PictureNonVisual : OoxmlComplexType
	{
		private CT_NonVisualDrawingProps _cNvPr;

		private CT_NonVisualPictureProperties _cNvPicPr;

		public CT_NonVisualDrawingProps CNvPr
		{
			get
			{
				return this._cNvPr;
			}
			set
			{
				this._cNvPr = value;
			}
		}

		public CT_NonVisualPictureProperties CNvPicPr
		{
			get
			{
				return this._cNvPicPr;
			}
			set
			{
				this._cNvPicPr = value;
			}
		}

		public static string CNvPrElementName
		{
			get
			{
				return "cNvPr";
			}
		}

		public static string CNvPicPrElementName
		{
			get
			{
				return "cNvPicPr";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			this._cNvPr = new CT_NonVisualDrawingProps();
			this._cNvPicPr = new CT_NonVisualPictureProperties();
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
			this.Write_cNvPr(s, depth, namespaces);
			this.Write_cNvPicPr(s, depth, namespaces);
		}

		public void Write_cNvPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._cNvPr != null)
			{
				this._cNvPr.Write(s, "cNvPr", depth + 1, namespaces);
			}
		}

		public void Write_cNvPicPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._cNvPicPr != null)
			{
				this._cNvPicPr.Write(s, "cNvPicPr", depth + 1, namespaces);
			}
		}
	}
}
