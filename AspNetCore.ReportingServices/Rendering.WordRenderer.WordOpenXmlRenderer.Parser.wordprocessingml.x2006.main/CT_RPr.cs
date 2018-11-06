using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_RPr : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_String _rStyle;

		private CT_Fonts _rFonts;

		private CT_OnOff _b;

		private CT_OnOff _bCs;

		private CT_OnOff _i;

		private CT_OnOff _iCs;

		private CT_OnOff _caps;

		private CT_OnOff _smallCaps;

		private CT_OnOff _strike;

		private CT_OnOff _dstrike;

		private CT_OnOff _outline;

		private CT_OnOff _shadow;

		private CT_OnOff _emboss;

		private CT_OnOff _imprint;

		private CT_OnOff _noProof;

		private CT_OnOff _snapToGrid;

		private CT_OnOff _vanish;

		private CT_OnOff _webHidden;

		private CT_HpsMeasure _kern;

		private CT_HpsMeasure _sz;

		private CT_HpsMeasure _szCs;

		private CT_OnOff _rtl;

		private CT_OnOff _cs;

		private CT_OnOff _specVanish;

		private CT_OnOff _oMath;

		public CT_String RStyle
		{
			get
			{
				return this._rStyle;
			}
			set
			{
				this._rStyle = value;
			}
		}

		public CT_Fonts RFonts
		{
			get
			{
				return this._rFonts;
			}
			set
			{
				this._rFonts = value;
			}
		}

		public CT_OnOff B
		{
			get
			{
				return this._b;
			}
			set
			{
				this._b = value;
			}
		}

		public CT_OnOff BCs
		{
			get
			{
				return this._bCs;
			}
			set
			{
				this._bCs = value;
			}
		}

		public CT_OnOff I
		{
			get
			{
				return this._i;
			}
			set
			{
				this._i = value;
			}
		}

		public CT_OnOff ICs
		{
			get
			{
				return this._iCs;
			}
			set
			{
				this._iCs = value;
			}
		}

		public CT_OnOff Caps
		{
			get
			{
				return this._caps;
			}
			set
			{
				this._caps = value;
			}
		}

		public CT_OnOff SmallCaps
		{
			get
			{
				return this._smallCaps;
			}
			set
			{
				this._smallCaps = value;
			}
		}

		public CT_OnOff Strike
		{
			get
			{
				return this._strike;
			}
			set
			{
				this._strike = value;
			}
		}

		public CT_OnOff Dstrike
		{
			get
			{
				return this._dstrike;
			}
			set
			{
				this._dstrike = value;
			}
		}

		public CT_OnOff Outline
		{
			get
			{
				return this._outline;
			}
			set
			{
				this._outline = value;
			}
		}

		public CT_OnOff Shadow
		{
			get
			{
				return this._shadow;
			}
			set
			{
				this._shadow = value;
			}
		}

		public CT_OnOff Emboss
		{
			get
			{
				return this._emboss;
			}
			set
			{
				this._emboss = value;
			}
		}

		public CT_OnOff Imprint
		{
			get
			{
				return this._imprint;
			}
			set
			{
				this._imprint = value;
			}
		}

		public CT_OnOff NoProof
		{
			get
			{
				return this._noProof;
			}
			set
			{
				this._noProof = value;
			}
		}

		public CT_OnOff SnapToGrid
		{
			get
			{
				return this._snapToGrid;
			}
			set
			{
				this._snapToGrid = value;
			}
		}

		public CT_OnOff Vanish
		{
			get
			{
				return this._vanish;
			}
			set
			{
				this._vanish = value;
			}
		}

		public CT_OnOff WebHidden
		{
			get
			{
				return this._webHidden;
			}
			set
			{
				this._webHidden = value;
			}
		}

		public CT_HpsMeasure Kern
		{
			get
			{
				return this._kern;
			}
			set
			{
				this._kern = value;
			}
		}

		public CT_HpsMeasure Sz
		{
			get
			{
				return this._sz;
			}
			set
			{
				this._sz = value;
			}
		}

		public CT_HpsMeasure SzCs
		{
			get
			{
				return this._szCs;
			}
			set
			{
				this._szCs = value;
			}
		}

		public CT_OnOff Rtl
		{
			get
			{
				return this._rtl;
			}
			set
			{
				this._rtl = value;
			}
		}

		public CT_OnOff Cs
		{
			get
			{
				return this._cs;
			}
			set
			{
				this._cs = value;
			}
		}

		public CT_OnOff SpecVanish
		{
			get
			{
				return this._specVanish;
			}
			set
			{
				this._specVanish = value;
			}
		}

		public CT_OnOff OMath
		{
			get
			{
				return this._oMath;
			}
			set
			{
				this._oMath = value;
			}
		}

		public static string RStyleElementName
		{
			get
			{
				return "rStyle";
			}
		}

		public static string RFontsElementName
		{
			get
			{
				return "rFonts";
			}
		}

		public static string BElementName
		{
			get
			{
				return "b";
			}
		}

		public static string BCsElementName
		{
			get
			{
				return "bCs";
			}
		}

		public static string IElementName
		{
			get
			{
				return "i";
			}
		}

		public static string ICsElementName
		{
			get
			{
				return "iCs";
			}
		}

		public static string CapsElementName
		{
			get
			{
				return "caps";
			}
		}

		public static string SmallCapsElementName
		{
			get
			{
				return "smallCaps";
			}
		}

		public static string StrikeElementName
		{
			get
			{
				return "strike";
			}
		}

		public static string DstrikeElementName
		{
			get
			{
				return "dstrike";
			}
		}

		public static string OutlineElementName
		{
			get
			{
				return "outline";
			}
		}

		public static string ShadowElementName
		{
			get
			{
				return "shadow";
			}
		}

		public static string EmbossElementName
		{
			get
			{
				return "emboss";
			}
		}

		public static string ImprintElementName
		{
			get
			{
				return "imprint";
			}
		}

		public static string NoProofElementName
		{
			get
			{
				return "noProof";
			}
		}

		public static string SnapToGridElementName
		{
			get
			{
				return "snapToGrid";
			}
		}

		public static string VanishElementName
		{
			get
			{
				return "vanish";
			}
		}

		public static string WebHiddenElementName
		{
			get
			{
				return "webHidden";
			}
		}

		public static string KernElementName
		{
			get
			{
				return "kern";
			}
		}

		public static string SzElementName
		{
			get
			{
				return "sz";
			}
		}

		public static string SzCsElementName
		{
			get
			{
				return "szCs";
			}
		}

		public static string RtlElementName
		{
			get
			{
				return "rtl";
			}
		}

		public static string CsElementName
		{
			get
			{
				return "cs";
			}
		}

		public static string SpecVanishElementName
		{
			get
			{
				return "specVanish";
			}
		}

		public static string OMathElementName
		{
			get
			{
				return "oMath";
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
			this.Write_rStyle(s);
			this.Write_rFonts(s);
			this.Write_b(s);
			this.Write_bCs(s);
			this.Write_i(s);
			this.Write_iCs(s);
			this.Write_caps(s);
			this.Write_smallCaps(s);
			this.Write_strike(s);
			this.Write_dstrike(s);
			this.Write_outline(s);
			this.Write_shadow(s);
			this.Write_emboss(s);
			this.Write_imprint(s);
			this.Write_noProof(s);
			this.Write_snapToGrid(s);
			this.Write_vanish(s);
			this.Write_webHidden(s);
			this.Write_kern(s);
			this.Write_sz(s);
			this.Write_szCs(s);
			this.Write_rtl(s);
			this.Write_cs(s);
			this.Write_specVanish(s);
			this.Write_oMath(s);
		}

		public void Write_rStyle(TextWriter s)
		{
			if (this._rStyle != null)
			{
				this._rStyle.Write(s, "rStyle");
			}
		}

		public void Write_rFonts(TextWriter s)
		{
			if (this._rFonts != null)
			{
				this._rFonts.Write(s, "rFonts");
			}
		}

		public void Write_b(TextWriter s)
		{
			if (this._b != null)
			{
				this._b.Write(s, "b");
			}
		}

		public void Write_bCs(TextWriter s)
		{
			if (this._bCs != null)
			{
				this._bCs.Write(s, "bCs");
			}
		}

		public void Write_i(TextWriter s)
		{
			if (this._i != null)
			{
				this._i.Write(s, "i");
			}
		}

		public void Write_iCs(TextWriter s)
		{
			if (this._iCs != null)
			{
				this._iCs.Write(s, "iCs");
			}
		}

		public void Write_caps(TextWriter s)
		{
			if (this._caps != null)
			{
				this._caps.Write(s, "caps");
			}
		}

		public void Write_smallCaps(TextWriter s)
		{
			if (this._smallCaps != null)
			{
				this._smallCaps.Write(s, "smallCaps");
			}
		}

		public void Write_strike(TextWriter s)
		{
			if (this._strike != null)
			{
				this._strike.Write(s, "strike");
			}
		}

		public void Write_dstrike(TextWriter s)
		{
			if (this._dstrike != null)
			{
				this._dstrike.Write(s, "dstrike");
			}
		}

		public void Write_outline(TextWriter s)
		{
			if (this._outline != null)
			{
				this._outline.Write(s, "outline");
			}
		}

		public void Write_shadow(TextWriter s)
		{
			if (this._shadow != null)
			{
				this._shadow.Write(s, "shadow");
			}
		}

		public void Write_emboss(TextWriter s)
		{
			if (this._emboss != null)
			{
				this._emboss.Write(s, "emboss");
			}
		}

		public void Write_imprint(TextWriter s)
		{
			if (this._imprint != null)
			{
				this._imprint.Write(s, "imprint");
			}
		}

		public void Write_noProof(TextWriter s)
		{
			if (this._noProof != null)
			{
				this._noProof.Write(s, "noProof");
			}
		}

		public void Write_snapToGrid(TextWriter s)
		{
			if (this._snapToGrid != null)
			{
				this._snapToGrid.Write(s, "snapToGrid");
			}
		}

		public void Write_vanish(TextWriter s)
		{
			if (this._vanish != null)
			{
				this._vanish.Write(s, "vanish");
			}
		}

		public void Write_webHidden(TextWriter s)
		{
			if (this._webHidden != null)
			{
				this._webHidden.Write(s, "webHidden");
			}
		}

		public void Write_kern(TextWriter s)
		{
			if (this._kern != null)
			{
				this._kern.Write(s, "kern");
			}
		}

		public void Write_sz(TextWriter s)
		{
			if (this._sz != null)
			{
				this._sz.Write(s, "sz");
			}
		}

		public void Write_szCs(TextWriter s)
		{
			if (this._szCs != null)
			{
				this._szCs.Write(s, "szCs");
			}
		}

		public void Write_rtl(TextWriter s)
		{
			if (this._rtl != null)
			{
				this._rtl.Write(s, "rtl");
			}
		}

		public void Write_cs(TextWriter s)
		{
			if (this._cs != null)
			{
				this._cs.Write(s, "cs");
			}
		}

		public void Write_specVanish(TextWriter s)
		{
			if (this._specVanish != null)
			{
				this._specVanish.Write(s, "specVanish");
			}
		}

		public void Write_oMath(TextWriter s)
		{
			if (this._oMath != null)
			{
				this._oMath.Write(s, "oMath");
			}
		}
	}
}
