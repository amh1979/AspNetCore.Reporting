using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_GraphicalObject : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_GraphicalObjectData _graphicData;

		public CT_GraphicalObjectData GraphicData
		{
			get
			{
				return this._graphicData;
			}
			set
			{
				this._graphicData = value;
			}
		}

		public static string GraphicDataElementName
		{
			get
			{
				return "graphicData";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			this._graphicData = new CT_GraphicalObjectData();
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
			this.Write_graphicData(s);
		}

		public void Write_graphicData(TextWriter s)
		{
			if (this._graphicData != null)
			{
				this._graphicData.Write(s, "graphicData");
			}
		}
	}
}
