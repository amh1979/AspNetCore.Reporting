using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_Picture : OoxmlComplexType
	{
		private string _macro_attr;

		private OoxmlBool _fPublished_attr;

		private CT_PictureNonVisual _nvPicPr;

		private CT_BlipFillProperties _blipFill;

		private CT_ShapeProperties _spPr;

		public string Macro_Attr
		{
			get
			{
				return this._macro_attr;
			}
			set
			{
				this._macro_attr = value;
			}
		}

		public OoxmlBool FPublished_Attr
		{
			get
			{
				return this._fPublished_attr;
			}
			set
			{
				this._fPublished_attr = value;
			}
		}

		public CT_PictureNonVisual NvPicPr
		{
			get
			{
				return this._nvPicPr;
			}
			set
			{
				this._nvPicPr = value;
			}
		}

		public CT_BlipFillProperties BlipFill
		{
			get
			{
				return this._blipFill;
			}
			set
			{
				this._blipFill = value;
			}
		}

		public CT_ShapeProperties SpPr
		{
			get
			{
				return this._spPr;
			}
			set
			{
				this._spPr = value;
			}
		}

		public static string NvPicPrElementName
		{
			get
			{
				return "nvPicPr";
			}
		}

		public static string BlipFillElementName
		{
			get
			{
				return "blipFill";
			}
		}

		public static string SpPrElementName
		{
			get
			{
				return "spPr";
			}
		}

		protected override void InitAttributes()
		{
			this._macro_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			this._fPublished_attr = OoxmlBool.OoxmlFalse;
		}

		protected override void InitElements()
		{
			this._nvPicPr = new CT_PictureNonVisual();
			this._blipFill = new CT_BlipFillProperties();
			this._spPr = new CT_ShapeProperties();
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
			if (this._macro_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" macro=\"");
				OoxmlComplexType.WriteData(s, this._macro_attr);
				s.Write("\"");
			}
			if ((bool)(this._fPublished_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" fPublished=\"");
				OoxmlComplexType.WriteData(s, this._fPublished_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_nvPicPr(s, depth, namespaces);
			this.Write_blipFill(s, depth, namespaces);
			this.Write_spPr(s, depth, namespaces);
		}

		public void Write_nvPicPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._nvPicPr != null)
			{
				this._nvPicPr.Write(s, "nvPicPr", depth + 1, namespaces);
			}
		}

		public void Write_blipFill(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._blipFill != null)
			{
				this._blipFill.Write(s, "blipFill", depth + 1, namespaces);
			}
		}

		public void Write_spPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._spPr != null)
			{
				this._spPr.Write(s, "spPr", depth + 1, namespaces);
			}
		}
	}
}
