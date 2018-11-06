using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_BlipFillProperties : OoxmlComplexType
	{
		internal enum ChoiceBucket_0
		{
			tile,
			stretch
		}

		private uint _dpi_attr;

		private bool _dpi_attr_is_specified;

		private OoxmlBool _rotWithShape_attr;

		private bool _rotWithShape_attr_is_specified;

		private CT_Blip _blip;

		private CT_RelativeRect _srcRect;

		private CT_StretchInfoProperties _stretch;

		private ChoiceBucket_0 _choice_0;

		public uint Dpi_Attr
		{
			get
			{
				return this._dpi_attr;
			}
			set
			{
				this._dpi_attr = value;
				this._dpi_attr_is_specified = true;
			}
		}

		public bool Dpi_Attr_Is_Specified
		{
			get
			{
				return this._dpi_attr_is_specified;
			}
			set
			{
				this._dpi_attr_is_specified = value;
			}
		}

		public OoxmlBool RotWithShape_Attr
		{
			get
			{
				return this._rotWithShape_attr;
			}
			set
			{
				this._rotWithShape_attr = value;
				this._rotWithShape_attr_is_specified = true;
			}
		}

		public bool RotWithShape_Attr_Is_Specified
		{
			get
			{
				return this._rotWithShape_attr_is_specified;
			}
			set
			{
				this._rotWithShape_attr_is_specified = value;
			}
		}

		public CT_Blip Blip
		{
			get
			{
				return this._blip;
			}
			set
			{
				this._blip = value;
			}
		}

		public CT_RelativeRect SrcRect
		{
			get
			{
				return this._srcRect;
			}
			set
			{
				this._srcRect = value;
			}
		}

		public CT_StretchInfoProperties Stretch
		{
			get
			{
				return this._stretch;
			}
			set
			{
				this._stretch = value;
			}
		}

		public ChoiceBucket_0 Choice_0
		{
			get
			{
				return this._choice_0;
			}
			set
			{
				this._choice_0 = value;
			}
		}

		public static string BlipElementName
		{
			get
			{
				return "blip";
			}
		}

		public static string SrcRectElementName
		{
			get
			{
				return "srcRect";
			}
		}

		public static string StretchElementName
		{
			get
			{
				return "stretch";
			}
		}

		protected override void InitAttributes()
		{
			this._dpi_attr_is_specified = false;
			this._rotWithShape_attr_is_specified = false;
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
			if (this._dpi_attr_is_specified)
			{
				s.Write(" dpi=\"");
				OoxmlComplexType.WriteData(s, this._dpi_attr);
				s.Write("\"");
			}
			if (this._rotWithShape_attr_is_specified)
			{
				s.Write(" rotWithShape=\"");
				OoxmlComplexType.WriteData(s, this._rotWithShape_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_blip(s, depth, namespaces);
			this.Write_srcRect(s, depth, namespaces);
			this.Write_stretch(s, depth, namespaces);
		}

		public void Write_blip(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._blip != null)
			{
				this._blip.Write(s, "blip", depth + 1, namespaces);
			}
		}

		public void Write_srcRect(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._srcRect != null)
			{
				this._srcRect.Write(s, "srcRect", depth + 1, namespaces);
			}
		}

		public void Write_stretch(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.stretch && this._stretch != null)
			{
				this._stretch.Write(s, "stretch", depth + 1, namespaces);
			}
		}
	}
}
