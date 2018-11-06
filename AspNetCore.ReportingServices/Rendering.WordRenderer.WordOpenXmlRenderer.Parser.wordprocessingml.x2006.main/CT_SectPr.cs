using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_SectPr : OoxmlComplexType, IOoxmlComplexType
	{
		private string _rsidRPr_attr;

		private string _rsidDel_attr;

		private string _rsidR_attr;

		private string _rsidSect_attr;

		private CT_SectType _type;

		private CT_PageSz _pgSz;

		private CT_PageMar _pgMar;

		private CT_OnOff _formProt;

		private CT_OnOff _noEndnote;

		private CT_OnOff _titlePg;

		private CT_OnOff _bidi;

		private CT_OnOff _rtlGutter;

		private CT_Rel _printerSettings;

		private List<IEG_HdrFtrReferences> _EG_HdrFtrReferencess;

		public string RsidRPr_Attr
		{
			get
			{
				return this._rsidRPr_attr;
			}
			set
			{
				this._rsidRPr_attr = value;
			}
		}

		public string RsidDel_Attr
		{
			get
			{
				return this._rsidDel_attr;
			}
			set
			{
				this._rsidDel_attr = value;
			}
		}

		public string RsidR_Attr
		{
			get
			{
				return this._rsidR_attr;
			}
			set
			{
				this._rsidR_attr = value;
			}
		}

		public string RsidSect_Attr
		{
			get
			{
				return this._rsidSect_attr;
			}
			set
			{
				this._rsidSect_attr = value;
			}
		}

		public CT_SectType Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}

		public CT_PageSz PgSz
		{
			get
			{
				return this._pgSz;
			}
			set
			{
				this._pgSz = value;
			}
		}

		public CT_PageMar PgMar
		{
			get
			{
				return this._pgMar;
			}
			set
			{
				this._pgMar = value;
			}
		}

		public CT_OnOff FormProt
		{
			get
			{
				return this._formProt;
			}
			set
			{
				this._formProt = value;
			}
		}

		public CT_OnOff NoEndnote
		{
			get
			{
				return this._noEndnote;
			}
			set
			{
				this._noEndnote = value;
			}
		}

		public CT_OnOff TitlePg
		{
			get
			{
				return this._titlePg;
			}
			set
			{
				this._titlePg = value;
			}
		}

		public CT_OnOff Bidi
		{
			get
			{
				return this._bidi;
			}
			set
			{
				this._bidi = value;
			}
		}

		public CT_OnOff RtlGutter
		{
			get
			{
				return this._rtlGutter;
			}
			set
			{
				this._rtlGutter = value;
			}
		}

		public CT_Rel PrinterSettings
		{
			get
			{
				return this._printerSettings;
			}
			set
			{
				this._printerSettings = value;
			}
		}

		public List<IEG_HdrFtrReferences> EG_HdrFtrReferencess
		{
			get
			{
				return this._EG_HdrFtrReferencess;
			}
			set
			{
				this._EG_HdrFtrReferencess = value;
			}
		}

		public static string TypeElementName
		{
			get
			{
				return "type";
			}
		}

		public static string PgSzElementName
		{
			get
			{
				return "pgSz";
			}
		}

		public static string PgMarElementName
		{
			get
			{
				return "pgMar";
			}
		}

		public static string FormProtElementName
		{
			get
			{
				return "formProt";
			}
		}

		public static string NoEndnoteElementName
		{
			get
			{
				return "noEndnote";
			}
		}

		public static string TitlePgElementName
		{
			get
			{
				return "titlePg";
			}
		}

		public static string BidiElementName
		{
			get
			{
				return "bidi";
			}
		}

		public static string RtlGutterElementName
		{
			get
			{
				return "rtlGutter";
			}
		}

		public static string PrinterSettingsElementName
		{
			get
			{
				return "printerSettings";
			}
		}

		public static string HeaderReferenceElementName
		{
			get
			{
				return "headerReference";
			}
		}

		public static string FooterReferenceElementName
		{
			get
			{
				return "footerReference";
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
			this._EG_HdrFtrReferencess = new List<IEG_HdrFtrReferences>();
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
			s.Write(" w:rsidRPr=\"");
			OoxmlComplexType.WriteData(s, this._rsidRPr_attr);
			s.Write("\"");
			s.Write(" w:rsidDel=\"");
			OoxmlComplexType.WriteData(s, this._rsidDel_attr);
			s.Write("\"");
			s.Write(" w:rsidR=\"");
			OoxmlComplexType.WriteData(s, this._rsidR_attr);
			s.Write("\"");
			s.Write(" w:rsidSect=\"");
			OoxmlComplexType.WriteData(s, this._rsidSect_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_EG_HdrFtrReferencess(s);
			this.Write_type(s);
			this.Write_pgSz(s);
			this.Write_pgMar(s);
			this.Write_formProt(s);
			this.Write_noEndnote(s);
			this.Write_titlePg(s);
			this.Write_bidi(s);
			this.Write_rtlGutter(s);
			this.Write_printerSettings(s);
		}

		public void Write_type(TextWriter s)
		{
			if (this._type != null)
			{
				this._type.Write(s, "type");
			}
		}

		public void Write_pgSz(TextWriter s)
		{
			if (this._pgSz != null)
			{
				this._pgSz.Write(s, "pgSz");
			}
		}

		public void Write_pgMar(TextWriter s)
		{
			if (this._pgMar != null)
			{
				this._pgMar.Write(s, "pgMar");
			}
		}

		public void Write_formProt(TextWriter s)
		{
			if (this._formProt != null)
			{
				this._formProt.Write(s, "formProt");
			}
		}

		public void Write_noEndnote(TextWriter s)
		{
			if (this._noEndnote != null)
			{
				this._noEndnote.Write(s, "noEndnote");
			}
		}

		public void Write_titlePg(TextWriter s)
		{
			if (this._titlePg != null)
			{
				this._titlePg.Write(s, "titlePg");
			}
		}

		public void Write_bidi(TextWriter s)
		{
			if (this._bidi != null)
			{
				this._bidi.Write(s, "bidi");
			}
		}

		public void Write_rtlGutter(TextWriter s)
		{
			if (this._rtlGutter != null)
			{
				this._rtlGutter.Write(s, "rtlGutter");
			}
		}

		public void Write_printerSettings(TextWriter s)
		{
			if (this._printerSettings != null)
			{
				this._printerSettings.Write(s, "printerSettings");
			}
		}

		public void Write_EG_HdrFtrReferencess(TextWriter s)
		{
			for (int i = 0; i < this._EG_HdrFtrReferencess.Count; i++)
			{
				string tagName = null;
				switch (this._EG_HdrFtrReferencess[i].GroupInterfaceType)
				{
				case GeneratedType.CT_HdrRef:
					tagName = "headerReference";
					break;
				case GeneratedType.CT_FtrRef:
					tagName = "footerReference";
					break;
				}
				this._EG_HdrFtrReferencess[i].Write(s, tagName);
			}
		}
	}
}
