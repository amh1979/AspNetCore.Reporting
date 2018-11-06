using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_Transform2D : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_Point2D _off;

		private CT_PositiveSize2D _ext;

		public CT_Point2D Off
		{
			get
			{
				return this._off;
			}
			set
			{
				this._off = value;
			}
		}

		public CT_PositiveSize2D Ext
		{
			get
			{
				return this._ext;
			}
			set
			{
				this._ext = value;
			}
		}

		public static string OffElementName
		{
			get
			{
				return "off";
			}
		}

		public static string ExtElementName
		{
			get
			{
				return "ext";
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
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_off(s);
			this.Write_ext(s);
		}

		public void Write_off(TextWriter s)
		{
			if (this._off != null)
			{
				this._off.Write(s, "off");
			}
		}

		public void Write_ext(TextWriter s)
		{
			if (this._ext != null)
			{
				this._ext.Write(s, "ext");
			}
		}
	}
}
