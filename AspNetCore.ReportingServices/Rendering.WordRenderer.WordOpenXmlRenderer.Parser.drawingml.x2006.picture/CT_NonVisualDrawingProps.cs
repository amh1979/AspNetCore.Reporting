using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.picture
{
	internal class CT_NonVisualDrawingProps : OoxmlComplexType, IOoxmlComplexType
	{
		private uint _id_attr;

		private string _name_attr;

		public uint Id_Attr
		{
			get
			{
				return this._id_attr;
			}
			set
			{
				this._id_attr = value;
			}
		}

		public string Name_Attr
		{
			get
			{
				return this._name_attr;
			}
			set
			{
				this._name_attr = value;
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
			base.WriteEmptyTag(s, tagName, "pic");
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			base.WriteOpenTag(s, tagName, "pic", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</pic:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" id=\"");
			OoxmlComplexType.WriteData(s, this._id_attr);
			s.Write("\"");
			s.Write(" name=\"");
			OoxmlComplexType.WriteData(s, this._name_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
