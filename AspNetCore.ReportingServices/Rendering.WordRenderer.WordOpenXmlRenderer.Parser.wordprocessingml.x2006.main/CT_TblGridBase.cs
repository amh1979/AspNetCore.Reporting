using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_TblGridBase : OoxmlComplexType, IOoxmlComplexType
	{
		private List<CT_TblGridCol> _gridCol;

		public List<CT_TblGridCol> GridCol
		{
			get
			{
				return this._gridCol;
			}
			set
			{
				this._gridCol = value;
			}
		}

		public static string GridColElementName
		{
			get
			{
				return "gridCol";
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
			this._gridCol = new List<CT_TblGridCol>();
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
			this.Write_gridCol(s);
		}

		public void Write_gridCol(TextWriter s)
		{
			if (this._gridCol != null)
			{
				foreach (CT_TblGridCol item in this._gridCol)
				{
					if (item != null)
					{
						item.Write(s, "gridCol");
					}
				}
			}
		}
	}
}
