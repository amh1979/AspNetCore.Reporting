using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.docPropsVTypes
{
	internal class CT_Vector : OoxmlComplexType
	{
		internal enum ChoiceBucket_0
		{
			variant,
			i1,
			i2,
			i4,
			i8,
			ui1,
			ui2,
			ui4,
			ui8,
			r4,
			r8,
			lpstr,
			lpwstr,
			bstr,
			date,
			filetime,
			_bool,
			cy,
			error,
			clsid,
			cf
		}

		private ST_VectorBaseType _baseType_attr;

		private uint _size_attr;

		private List<CT_Variant> _variant;

		private ChoiceBucket_0 _choice_0;

		public ST_VectorBaseType BaseType_Attr
		{
			get
			{
				return this._baseType_attr;
			}
			set
			{
				this._baseType_attr = value;
			}
		}

		public uint Size_Attr
		{
			get
			{
				return this._size_attr;
			}
			set
			{
				this._size_attr = value;
			}
		}

		public List<CT_Variant> Variant
		{
			get
			{
				return this._variant;
			}
			set
			{
				this._variant = value;
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

		public static string VariantElementName
		{
			get
			{
				return "variant";
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
			this._variant = new List<CT_Variant>();
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" baseType=\"");
			OoxmlComplexType.WriteData(s, this._baseType_attr);
			s.Write("\"");
			s.Write(" size=\"");
			OoxmlComplexType.WriteData(s, this._size_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			this.Write_variant(s, depth, namespaces);
		}

		public void Write_variant(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (this._choice_0 == ChoiceBucket_0.variant && this._variant != null)
			{
				foreach (CT_Variant item in this._variant)
				{
					if (item != null)
					{
						item.Write(s, "variant", depth + 1, namespaces);
					}
				}
			}
		}
	}
}
