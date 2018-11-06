using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Lvl : OoxmlComplexType, IOoxmlComplexType
	{
		private int _ilvl_attr;

		private CT_DecimalNumber _start;

		private CT_NumFmt _numFmt;

		private CT_DecimalNumber _lvlRestart;

		private CT_String _pStyle;

		private CT_OnOff _isLgl;

		private CT_LevelText _lvlText;

		private CT_DecimalNumber _lvlPicBulletId;

		private CT_Jc _lvlJc;

		private CT_RPr _rPr;

		public int Ilvl_Attr
		{
			get
			{
				return this._ilvl_attr;
			}
			set
			{
				this._ilvl_attr = value;
			}
		}

		public CT_DecimalNumber Start
		{
			get
			{
				return this._start;
			}
			set
			{
				this._start = value;
			}
		}

		public CT_NumFmt NumFmt
		{
			get
			{
				return this._numFmt;
			}
			set
			{
				this._numFmt = value;
			}
		}

		public CT_DecimalNumber LvlRestart
		{
			get
			{
				return this._lvlRestart;
			}
			set
			{
				this._lvlRestart = value;
			}
		}

		public CT_String PStyle
		{
			get
			{
				return this._pStyle;
			}
			set
			{
				this._pStyle = value;
			}
		}

		public CT_OnOff IsLgl
		{
			get
			{
				return this._isLgl;
			}
			set
			{
				this._isLgl = value;
			}
		}

		public CT_LevelText LvlText
		{
			get
			{
				return this._lvlText;
			}
			set
			{
				this._lvlText = value;
			}
		}

		public CT_DecimalNumber LvlPicBulletId
		{
			get
			{
				return this._lvlPicBulletId;
			}
			set
			{
				this._lvlPicBulletId = value;
			}
		}

		public CT_Jc LvlJc
		{
			get
			{
				return this._lvlJc;
			}
			set
			{
				this._lvlJc = value;
			}
		}

		public CT_RPr RPr
		{
			get
			{
				return this._rPr;
			}
			set
			{
				this._rPr = value;
			}
		}

		public static string StartElementName
		{
			get
			{
				return "start";
			}
		}

		public static string NumFmtElementName
		{
			get
			{
				return "numFmt";
			}
		}

		public static string LvlRestartElementName
		{
			get
			{
				return "lvlRestart";
			}
		}

		public static string PStyleElementName
		{
			get
			{
				return "pStyle";
			}
		}

		public static string IsLglElementName
		{
			get
			{
				return "isLgl";
			}
		}

		public static string LvlTextElementName
		{
			get
			{
				return "lvlText";
			}
		}

		public static string LvlPicBulletIdElementName
		{
			get
			{
				return "lvlPicBulletId";
			}
		}

		public static string LvlJcElementName
		{
			get
			{
				return "lvlJc";
			}
		}

		public static string RPrElementName
		{
			get
			{
				return "rPr";
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
			s.Write(" w:ilvl=\"");
			OoxmlComplexType.WriteData(s, this._ilvl_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_start(s);
			this.Write_numFmt(s);
			this.Write_lvlRestart(s);
			this.Write_pStyle(s);
			this.Write_isLgl(s);
			this.Write_lvlText(s);
			this.Write_lvlPicBulletId(s);
			this.Write_lvlJc(s);
			this.Write_rPr(s);
		}

		public void Write_start(TextWriter s)
		{
			if (this._start != null)
			{
				this._start.Write(s, "start");
			}
		}

		public void Write_numFmt(TextWriter s)
		{
			if (this._numFmt != null)
			{
				this._numFmt.Write(s, "numFmt");
			}
		}

		public void Write_lvlRestart(TextWriter s)
		{
			if (this._lvlRestart != null)
			{
				this._lvlRestart.Write(s, "lvlRestart");
			}
		}

		public void Write_pStyle(TextWriter s)
		{
			if (this._pStyle != null)
			{
				this._pStyle.Write(s, "pStyle");
			}
		}

		public void Write_isLgl(TextWriter s)
		{
			if (this._isLgl != null)
			{
				this._isLgl.Write(s, "isLgl");
			}
		}

		public void Write_lvlText(TextWriter s)
		{
			if (this._lvlText != null)
			{
				this._lvlText.Write(s, "lvlText");
			}
		}

		public void Write_lvlPicBulletId(TextWriter s)
		{
			if (this._lvlPicBulletId != null)
			{
				this._lvlPicBulletId.Write(s, "lvlPicBulletId");
			}
		}

		public void Write_lvlJc(TextWriter s)
		{
			if (this._lvlJc != null)
			{
				this._lvlJc.Write(s, "lvlJc");
			}
		}

		public void Write_rPr(TextWriter s)
		{
			if (this._rPr != null)
			{
				this._rPr.Write(s, "rPr");
			}
		}
	}
}
