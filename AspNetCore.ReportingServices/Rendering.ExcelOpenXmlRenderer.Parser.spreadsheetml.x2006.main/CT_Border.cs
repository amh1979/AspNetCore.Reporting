using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Border : OoxmlComplexType
	{
		private OoxmlBool _outline_attr;

		private OoxmlBool _diagonalUp_attr;

		private bool _diagonalUp_attr_is_specified;

		private OoxmlBool _diagonalDown_attr;

		private bool _diagonalDown_attr_is_specified;

		private CT_BorderPr _left;

		private CT_BorderPr _right;

		private CT_BorderPr _top;

		private CT_BorderPr _bottom;

		private CT_BorderPr _diagonal;

		private CT_BorderPr _vertical;

		private CT_BorderPr _horizontal;

		public OoxmlBool Outline_Attr
		{
			get
			{
				return this._outline_attr;
			}
			set
			{
				this._outline_attr = value;
			}
		}

		public OoxmlBool DiagonalUp_Attr
		{
			get
			{
				return this._diagonalUp_attr;
			}
			set
			{
				this._diagonalUp_attr = value;
				this._diagonalUp_attr_is_specified = true;
			}
		}

		public bool DiagonalUp_Attr_Is_Specified
		{
			get
			{
				return this._diagonalUp_attr_is_specified;
			}
			set
			{
				this._diagonalUp_attr_is_specified = value;
			}
		}

		public OoxmlBool DiagonalDown_Attr
		{
			get
			{
				return this._diagonalDown_attr;
			}
			set
			{
				this._diagonalDown_attr = value;
				this._diagonalDown_attr_is_specified = true;
			}
		}

		public bool DiagonalDown_Attr_Is_Specified
		{
			get
			{
				return this._diagonalDown_attr_is_specified;
			}
			set
			{
				this._diagonalDown_attr_is_specified = value;
			}
		}

		public CT_BorderPr Left
		{
			get
			{
				return this._left;
			}
			set
			{
				this._left = value;
			}
		}

		public CT_BorderPr Right
		{
			get
			{
				return this._right;
			}
			set
			{
				this._right = value;
			}
		}

		public CT_BorderPr Top
		{
			get
			{
				return this._top;
			}
			set
			{
				this._top = value;
			}
		}

		public CT_BorderPr Bottom
		{
			get
			{
				return this._bottom;
			}
			set
			{
				this._bottom = value;
			}
		}

		public CT_BorderPr Diagonal
		{
			get
			{
				return this._diagonal;
			}
			set
			{
				this._diagonal = value;
			}
		}

		public CT_BorderPr Vertical
		{
			get
			{
				return this._vertical;
			}
			set
			{
				this._vertical = value;
			}
		}

		public CT_BorderPr Horizontal
		{
			get
			{
				return this._horizontal;
			}
			set
			{
				this._horizontal = value;
			}
		}

		public static string LeftElementName
		{
			get
			{
				return "left";
			}
		}

		public static string RightElementName
		{
			get
			{
				return "right";
			}
		}

		public static string TopElementName
		{
			get
			{
				return "top";
			}
		}

		public static string BottomElementName
		{
			get
			{
				return "bottom";
			}
		}

		public static string DiagonalElementName
		{
			get
			{
				return "diagonal";
			}
		}

		public static string VerticalElementName
		{
			get
			{
				return "vertical";
			}
		}

		public static string HorizontalElementName
		{
			get
			{
				return "horizontal";
			}
		}

		protected override void InitAttributes()
		{
			this._outline_attr = OoxmlBool.OoxmlTrue;
			this._diagonalUp_attr_is_specified = false;
			this._diagonalDown_attr_is_specified = false;
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if ((bool)(this._outline_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" outline=\"");
				OoxmlComplexType.WriteData(s, this._outline_attr);
				s.Write("\"");
			}
			if (this._diagonalUp_attr_is_specified)
			{
				s.Write(" diagonalUp=\"");
				OoxmlComplexType.WriteData(s, this._diagonalUp_attr);
				s.Write("\"");
			}
			if (this._diagonalDown_attr_is_specified)
			{
				s.Write(" diagonalDown=\"");
				OoxmlComplexType.WriteData(s, this._diagonalDown_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_left(s, depth, namespaces);
			this.Write_right(s, depth, namespaces);
			this.Write_top(s, depth, namespaces);
			this.Write_bottom(s, depth, namespaces);
			this.Write_diagonal(s, depth, namespaces);
			this.Write_vertical(s, depth, namespaces);
			this.Write_horizontal(s, depth, namespaces);
		}

		public void Write_left(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._left != null)
			{
				this._left.Write(s, "left", depth + 1, namespaces);
			}
		}

		public void Write_right(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._right != null)
			{
				this._right.Write(s, "right", depth + 1, namespaces);
			}
		}

		public void Write_top(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._top != null)
			{
				this._top.Write(s, "top", depth + 1, namespaces);
			}
		}

		public void Write_bottom(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._bottom != null)
			{
				this._bottom.Write(s, "bottom", depth + 1, namespaces);
			}
		}

		public void Write_diagonal(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._diagonal != null)
			{
				this._diagonal.Write(s, "diagonal", depth + 1, namespaces);
			}
		}

		public void Write_vertical(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._vertical != null)
			{
				this._vertical.Write(s, "vertical", depth + 1, namespaces);
			}
		}

		public void Write_horizontal(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._horizontal != null)
			{
				this._horizontal.Write(s, "horizontal", depth + 1, namespaces);
			}
		}
	}
}
