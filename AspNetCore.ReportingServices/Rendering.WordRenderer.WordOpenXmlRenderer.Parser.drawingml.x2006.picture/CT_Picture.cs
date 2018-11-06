using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.picture
{
	internal class CT_Picture : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_PictureNonVisual _nvPicPr;

		private CT_BlipFillProperties _blipFill;

		private CT_ShapeProperties _spPr;

		public CT_PictureNonVisual NvPicPr
		{
			get
			{
				return this._nvPicPr;
			}
			set
			{
				this._nvPicPr = value;
			}
		}

		public CT_BlipFillProperties BlipFill
		{
			get
			{
				return this._blipFill;
			}
			set
			{
				this._blipFill = value;
			}
		}

		public CT_ShapeProperties SpPr
		{
			get
			{
				return this._spPr;
			}
			set
			{
				this._spPr = value;
			}
		}

		public static string NvPicPrElementName
		{
			get
			{
				return "nvPicPr";
			}
		}

		public static string BlipFillElementName
		{
			get
			{
				return "blipFill";
			}
		}

		public static string SpPrElementName
		{
			get
			{
				return "spPr";
			}
		}

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			this._nvPicPr = new CT_PictureNonVisual();
			this._blipFill = new CT_BlipFillProperties();
			this._spPr = new CT_ShapeProperties();
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
			this.Write_nvPicPr(s);
			this.Write_blipFill(s);
			this.Write_spPr(s);
		}

		public void Write_nvPicPr(TextWriter s)
		{
			if (this._nvPicPr != null)
			{
				this._nvPicPr.Write(s, "nvPicPr");
			}
		}

		public void Write_blipFill(TextWriter s)
		{
			if (this._blipFill != null)
			{
				this._blipFill.Write(s, "blipFill");
			}
		}

		public void Write_spPr(TextWriter s)
		{
			if (this._spPr != null)
			{
				this._spPr.Write(s, "spPr");
			}
		}
	}
}
