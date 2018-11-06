using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.picture
{
	internal class CT_PictureNonVisual : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_NonVisualDrawingProps _cNvPr;

		private CT_NonVisualPictureProperties _cNvPicPr;

		public CT_NonVisualDrawingProps CNvPr
		{
			get
			{
				return this._cNvPr;
			}
			set
			{
				this._cNvPr = value;
			}
		}

		public CT_NonVisualPictureProperties CNvPicPr
		{
			get
			{
				return this._cNvPicPr;
			}
			set
			{
				this._cNvPicPr = value;
			}
		}

		public static string CNvPrElementName
		{
			get
			{
				return "cNvPr";
			}
		}

		public static string CNvPicPrElementName
		{
			get
			{
				return "cNvPicPr";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			this._cNvPr = new CT_NonVisualDrawingProps();
			this._cNvPicPr = new CT_NonVisualPictureProperties();
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
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_cNvPr(s);
			this.Write_cNvPicPr(s);
		}

		public void Write_cNvPr(TextWriter s)
		{
			if (this._cNvPr != null)
			{
				this._cNvPr.Write(s, "cNvPr");
			}
		}

		public void Write_cNvPicPr(TextWriter s)
		{
			if (this._cNvPicPr != null)
			{
				this._cNvPicPr.Write(s, "cNvPicPr");
			}
		}
	}
}
