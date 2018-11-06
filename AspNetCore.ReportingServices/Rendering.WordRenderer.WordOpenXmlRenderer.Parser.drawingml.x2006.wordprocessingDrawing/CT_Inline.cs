using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.wordprocessingDrawing
{
	internal class CT_Inline : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_PositiveSize2D _extent;

		private CT_NonVisualDrawingProps _docPr;

		private CT_GraphicalObject _graphic;

		public CT_PositiveSize2D Extent
		{
			get
			{
				return this._extent;
			}
			set
			{
				this._extent = value;
			}
		}

		public CT_NonVisualDrawingProps DocPr
		{
			get
			{
				return this._docPr;
			}
			set
			{
				this._docPr = value;
			}
		}

		public CT_GraphicalObject Graphic
		{
			get
			{
				return this._graphic;
			}
			set
			{
				this._graphic = value;
			}
		}

		public static string ExtentElementName
		{
			get
			{
				return "extent";
			}
		}

		public static string DocPrElementName
		{
			get
			{
				return "docPr";
			}
		}

		public static string GraphicElementName
		{
			get
			{
				return "graphic";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			this._extent = new CT_PositiveSize2D();
			this._docPr = new CT_NonVisualDrawingProps();
			this._graphic = new CT_GraphicalObject();
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
			base.WriteOpenTag(s, tagName, "wp", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</wp:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_extent(s);
			this.Write_docPr(s);
			this.Write_graphic(s);
		}

		public void Write_extent(TextWriter s)
		{
			if (this._extent != null)
			{
				this._extent.Write(s, "extent");
			}
		}

		public void Write_docPr(TextWriter s)
		{
			if (this._docPr != null)
			{
				this._docPr.Write(s, "docPr");
			}
		}

		public void Write_graphic(TextWriter s)
		{
			if (this._graphic != null)
			{
				this._graphic.Write(s, "graphic");
			}
		}
	}
}
