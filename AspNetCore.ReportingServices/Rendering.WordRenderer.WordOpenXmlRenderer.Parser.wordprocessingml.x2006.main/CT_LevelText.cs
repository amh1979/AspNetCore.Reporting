using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_LevelText : OoxmlComplexType, IOoxmlComplexType
	{
		private string _val_attr;

		private bool _val_attr_is_specified;

		private bool __null_attr;

		private bool __null_attr_is_specified;

		public bool _null_Attr
		{
			get
			{
				return this.__null_attr;
			}
			set
			{
				this.__null_attr = value;
				this.__null_attr_is_specified = true;
			}
		}

		public string Val_Attr
		{
			get
			{
				return this._val_attr;
			}
			set
			{
				this._val_attr = value;
				this._val_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			this._val_attr_is_specified = false;
			this.__null_attr_is_specified = false;
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void Write(TextWriter s, string tagName)
		{
			base.WriteEmptyTag(s, tagName, "w");
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
			if (this._val_attr_is_specified)
			{
				s.Write(" w:val=\"");
				OoxmlComplexType.WriteData(s, this._val_attr);
				s.Write("\"");
			}
			if (this.__null_attr_is_specified)
			{
				s.Write(" w:null=\"");
				OoxmlComplexType.WriteData(s, this.__null_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
