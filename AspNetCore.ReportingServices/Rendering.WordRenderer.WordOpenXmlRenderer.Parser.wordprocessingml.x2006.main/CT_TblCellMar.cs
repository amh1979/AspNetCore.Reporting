using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_TblCellMar : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_TblWidth _top;

		private CT_TblWidth _left;

		private CT_TblWidth _bottom;

		private CT_TblWidth _right;

		public CT_TblWidth Top
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

		public CT_TblWidth Left
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

		public CT_TblWidth Bottom
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

		public CT_TblWidth Right
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

		public static string TopElementName
		{
			get
			{
				return "top";
			}
		}

		public static string LeftElementName
		{
			get
			{
				return "left";
			}
		}

		public static string BottomElementName
		{
			get
			{
				return "bottom";
			}
		}

		public static string RightElementName
		{
			get
			{
				return "right";
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
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_top(s);
			this.Write_left(s);
			this.Write_bottom(s);
			this.Write_right(s);
		}

		public void Write_top(TextWriter s)
		{
			if (this._top != null)
			{
				this._top.Write(s, "top");
			}
		}

		public void Write_left(TextWriter s)
		{
			if (this._left != null)
			{
				this._left.Write(s, "left");
			}
		}

		public void Write_bottom(TextWriter s)
		{
			if (this._bottom != null)
			{
				this._bottom.Write(s, "bottom");
			}
		}

		public void Write_right(TextWriter s)
		{
			if (this._right != null)
			{
				this._right.Write(s, "right");
			}
		}
	}
}
