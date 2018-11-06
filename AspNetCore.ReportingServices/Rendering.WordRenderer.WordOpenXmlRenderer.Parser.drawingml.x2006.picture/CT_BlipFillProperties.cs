using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.picture
{
	internal class CT_BlipFillProperties : OoxmlComplexType, IOoxmlComplexType
	{
		internal enum ChoiceBucket_0
		{
			tile,
			stretch
		}

		private CT_Blip _blip;

		private CT_RelativeRect _srcRect;

		private CT_StretchInfoProperties _stretch;

		private ChoiceBucket_0 _choice_0;

		public CT_Blip Blip
		{
			get
			{
				return this._blip;
			}
			set
			{
				this._blip = value;
			}
		}

		public CT_RelativeRect SrcRect
		{
			get
			{
				return this._srcRect;
			}
			set
			{
				this._srcRect = value;
			}
		}

		public CT_StretchInfoProperties Stretch
		{
			get
			{
				return this._stretch;
			}
			set
			{
				this._stretch = value;
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

		public static string BlipElementName
		{
			get
			{
				return "blip";
			}
		}

		public static string SrcRectElementName
		{
			get
			{
				return "srcRect";
			}
		}

		public static string StretchElementName
		{
			get
			{
				return "stretch";
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
			this.Write_blip(s);
			this.Write_srcRect(s);
			this.Write_stretch(s);
		}

		public void Write_blip(TextWriter s)
		{
			if (this._blip != null)
			{
				this._blip.Write(s, "blip");
			}
		}

		public void Write_srcRect(TextWriter s)
		{
			if (this._srcRect != null)
			{
				this._srcRect.Write(s, "srcRect");
			}
		}

		public void Write_stretch(TextWriter s)
		{
			if (this._choice_0 == ChoiceBucket_0.stretch && this._stretch != null)
			{
				this._stretch.Write(s, "stretch");
			}
		}
	}
}
