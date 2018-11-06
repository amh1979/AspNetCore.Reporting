using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_Blip : OoxmlComplexType, IOoxmlComplexType
	{
		internal enum ChoiceBucket_0
		{
			alphaBiLevel,
			alphaCeiling,
			alphaFloor,
			alphaInv,
			alphaMod,
			alphaModFix,
			alphaRepl,
			biLevel,
			blur,
			clrChange,
			clrRepl,
			duotone,
			fillOverlay,
			grayscl,
			hsl,
			lum,
			tint
		}

		private string _embed_attr;

		private string _cstate_attr;

		private ChoiceBucket_0 _choice_0;

		public string Embed_Attr
		{
			get
			{
				return this._embed_attr;
			}
			set
			{
				this._embed_attr = value;
			}
		}

		public string Cstate_Attr
		{
			get
			{
				return this._cstate_attr;
			}
			set
			{
				this._cstate_attr = value;
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

		protected override void InitAttributes()
		{
			this._embed_attr = "";
			this._cstate_attr = "none";
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void Write(TextWriter s, string tagName)
		{
			base.WriteEmptyTag(s, tagName, "a");
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
			if (this._embed_attr != "")
			{
				s.Write(" r:embed=\"");
				OoxmlComplexType.WriteData(s, this._embed_attr);
				s.Write("\"");
			}
			if (this._cstate_attr != "none")
			{
				s.Write(" cstate=\"");
				OoxmlComplexType.WriteData(s, this._cstate_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
