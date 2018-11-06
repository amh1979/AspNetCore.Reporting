using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Style : OoxmlComplexType, IOoxmlComplexType
	{
		private bool __default_attr;

		private bool __default_attr_is_specified;

		private CT_String _name;

		private CT_String _aliases;

		private CT_String _basedOn;

		private CT_String _next;

		private CT_String _link;

		private CT_OnOff _autoRedefine;

		private CT_OnOff _hidden;

		private CT_DecimalNumber _uiPriority;

		private CT_OnOff _semiHidden;

		private CT_OnOff _unhideWhenUsed;

		private CT_OnOff _qFormat;

		private CT_OnOff _locked;

		private CT_OnOff _personal;

		private CT_OnOff _personalCompose;

		private CT_OnOff _personalReply;

		private CT_LongHexNumber _rsid;

		private CT_RPr _rPr;

		private CT_TblPrBase _tblPr;

		private CT_TrPr _trPr;

		private CT_TcPr _tcPr;

		public bool _default_Attr
		{
			get
			{
				return this.__default_attr;
			}
			set
			{
				this.__default_attr = value;
				this.__default_attr_is_specified = true;
			}
		}

		public CT_String Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public CT_String Aliases
		{
			get
			{
				return this._aliases;
			}
			set
			{
				this._aliases = value;
			}
		}

		public CT_String BasedOn
		{
			get
			{
				return this._basedOn;
			}
			set
			{
				this._basedOn = value;
			}
		}

		public CT_String Next
		{
			get
			{
				return this._next;
			}
			set
			{
				this._next = value;
			}
		}

		public CT_String Link
		{
			get
			{
				return this._link;
			}
			set
			{
				this._link = value;
			}
		}

		public CT_OnOff AutoRedefine
		{
			get
			{
				return this._autoRedefine;
			}
			set
			{
				this._autoRedefine = value;
			}
		}

		public CT_OnOff Hidden
		{
			get
			{
				return this._hidden;
			}
			set
			{
				this._hidden = value;
			}
		}

		public CT_DecimalNumber UiPriority
		{
			get
			{
				return this._uiPriority;
			}
			set
			{
				this._uiPriority = value;
			}
		}

		public CT_OnOff SemiHidden
		{
			get
			{
				return this._semiHidden;
			}
			set
			{
				this._semiHidden = value;
			}
		}

		public CT_OnOff UnhideWhenUsed
		{
			get
			{
				return this._unhideWhenUsed;
			}
			set
			{
				this._unhideWhenUsed = value;
			}
		}

		public CT_OnOff QFormat
		{
			get
			{
				return this._qFormat;
			}
			set
			{
				this._qFormat = value;
			}
		}

		public CT_OnOff Locked
		{
			get
			{
				return this._locked;
			}
			set
			{
				this._locked = value;
			}
		}

		public CT_OnOff Personal
		{
			get
			{
				return this._personal;
			}
			set
			{
				this._personal = value;
			}
		}

		public CT_OnOff PersonalCompose
		{
			get
			{
				return this._personalCompose;
			}
			set
			{
				this._personalCompose = value;
			}
		}

		public CT_OnOff PersonalReply
		{
			get
			{
				return this._personalReply;
			}
			set
			{
				this._personalReply = value;
			}
		}

		public CT_LongHexNumber Rsid
		{
			get
			{
				return this._rsid;
			}
			set
			{
				this._rsid = value;
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

		public CT_TblPrBase TblPr
		{
			get
			{
				return this._tblPr;
			}
			set
			{
				this._tblPr = value;
			}
		}

		public CT_TrPr TrPr
		{
			get
			{
				return this._trPr;
			}
			set
			{
				this._trPr = value;
			}
		}

		public CT_TcPr TcPr
		{
			get
			{
				return this._tcPr;
			}
			set
			{
				this._tcPr = value;
			}
		}

		public static string NameElementName
		{
			get
			{
				return "name";
			}
		}

		public static string AliasesElementName
		{
			get
			{
				return "aliases";
			}
		}

		public static string BasedOnElementName
		{
			get
			{
				return "basedOn";
			}
		}

		public static string NextElementName
		{
			get
			{
				return "next";
			}
		}

		public static string LinkElementName
		{
			get
			{
				return "link";
			}
		}

		public static string AutoRedefineElementName
		{
			get
			{
				return "autoRedefine";
			}
		}

		public static string HiddenElementName
		{
			get
			{
				return "hidden";
			}
		}

		public static string UiPriorityElementName
		{
			get
			{
				return "uiPriority";
			}
		}

		public static string SemiHiddenElementName
		{
			get
			{
				return "semiHidden";
			}
		}

		public static string UnhideWhenUsedElementName
		{
			get
			{
				return "unhideWhenUsed";
			}
		}

		public static string QFormatElementName
		{
			get
			{
				return "qFormat";
			}
		}

		public static string LockedElementName
		{
			get
			{
				return "locked";
			}
		}

		public static string PersonalElementName
		{
			get
			{
				return "personal";
			}
		}

		public static string PersonalComposeElementName
		{
			get
			{
				return "personalCompose";
			}
		}

		public static string PersonalReplyElementName
		{
			get
			{
				return "personalReply";
			}
		}

		public static string RsidElementName
		{
			get
			{
				return "rsid";
			}
		}

		public static string RPrElementName
		{
			get
			{
				return "rPr";
			}
		}

		public static string TblPrElementName
		{
			get
			{
				return "tblPr";
			}
		}

		public static string TrPrElementName
		{
			get
			{
				return "trPr";
			}
		}

		public static string TcPrElementName
		{
			get
			{
				return "tcPr";
			}
		}

		protected override void InitAttributes()
		{
			this.__default_attr_is_specified = false;
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
			if (this.__default_attr_is_specified)
			{
				s.Write(" w:default=\"");
				OoxmlComplexType.WriteData(s, this.__default_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
			this.Write_name(s);
			this.Write_aliases(s);
			this.Write_basedOn(s);
			this.Write_next(s);
			this.Write_link(s);
			this.Write_autoRedefine(s);
			this.Write_hidden(s);
			this.Write_uiPriority(s);
			this.Write_semiHidden(s);
			this.Write_unhideWhenUsed(s);
			this.Write_qFormat(s);
			this.Write_locked(s);
			this.Write_personal(s);
			this.Write_personalCompose(s);
			this.Write_personalReply(s);
			this.Write_rsid(s);
			this.Write_rPr(s);
			this.Write_tblPr(s);
			this.Write_trPr(s);
			this.Write_tcPr(s);
		}

		public void Write_name(TextWriter s)
		{
			if (this._name != null)
			{
				this._name.Write(s, "name");
			}
		}

		public void Write_aliases(TextWriter s)
		{
			if (this._aliases != null)
			{
				this._aliases.Write(s, "aliases");
			}
		}

		public void Write_basedOn(TextWriter s)
		{
			if (this._basedOn != null)
			{
				this._basedOn.Write(s, "basedOn");
			}
		}

		public void Write_next(TextWriter s)
		{
			if (this._next != null)
			{
				this._next.Write(s, "next");
			}
		}

		public void Write_link(TextWriter s)
		{
			if (this._link != null)
			{
				this._link.Write(s, "link");
			}
		}

		public void Write_autoRedefine(TextWriter s)
		{
			if (this._autoRedefine != null)
			{
				this._autoRedefine.Write(s, "autoRedefine");
			}
		}

		public void Write_hidden(TextWriter s)
		{
			if (this._hidden != null)
			{
				this._hidden.Write(s, "hidden");
			}
		}

		public void Write_uiPriority(TextWriter s)
		{
			if (this._uiPriority != null)
			{
				this._uiPriority.Write(s, "uiPriority");
			}
		}

		public void Write_semiHidden(TextWriter s)
		{
			if (this._semiHidden != null)
			{
				this._semiHidden.Write(s, "semiHidden");
			}
		}

		public void Write_unhideWhenUsed(TextWriter s)
		{
			if (this._unhideWhenUsed != null)
			{
				this._unhideWhenUsed.Write(s, "unhideWhenUsed");
			}
		}

		public void Write_qFormat(TextWriter s)
		{
			if (this._qFormat != null)
			{
				this._qFormat.Write(s, "qFormat");
			}
		}

		public void Write_locked(TextWriter s)
		{
			if (this._locked != null)
			{
				this._locked.Write(s, "locked");
			}
		}

		public void Write_personal(TextWriter s)
		{
			if (this._personal != null)
			{
				this._personal.Write(s, "personal");
			}
		}

		public void Write_personalCompose(TextWriter s)
		{
			if (this._personalCompose != null)
			{
				this._personalCompose.Write(s, "personalCompose");
			}
		}

		public void Write_personalReply(TextWriter s)
		{
			if (this._personalReply != null)
			{
				this._personalReply.Write(s, "personalReply");
			}
		}

		public void Write_rsid(TextWriter s)
		{
			if (this._rsid != null)
			{
				this._rsid.Write(s, "rsid");
			}
		}

		public void Write_rPr(TextWriter s)
		{
			if (this._rPr != null)
			{
				this._rPr.Write(s, "rPr");
			}
		}

		public void Write_tblPr(TextWriter s)
		{
			if (this._tblPr != null)
			{
				this._tblPr.Write(s, "tblPr");
			}
		}

		public void Write_trPr(TextWriter s)
		{
			if (this._trPr != null)
			{
				this._trPr.Write(s, "trPr");
			}
		}

		public void Write_tcPr(TextWriter s)
		{
			if (this._tcPr != null)
			{
				this._tcPr.Write(s, "tcPr");
			}
		}
	}
}
