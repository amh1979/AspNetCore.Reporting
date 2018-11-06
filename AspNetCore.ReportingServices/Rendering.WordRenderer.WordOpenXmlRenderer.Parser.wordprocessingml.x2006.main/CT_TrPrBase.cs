using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_TrPrBase : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_DecimalNumber _divId;

		private CT_DecimalNumber _gridBefore;

		private CT_DecimalNumber _gridAfter;

		private CT_TblWidth _wBefore;

		private CT_TblWidth _wAfter;

		private CT_OnOff _cantSplit;

		private CT_Height _trHeight;

		private CT_OnOff _tblHeader;

		private CT_TblWidth _tblCellSpacing;

		private CT_OnOff _hidden;

		public CT_DecimalNumber DivId
		{
			get
			{
				return this._divId;
			}
			set
			{
				this._divId = value;
			}
		}

		public CT_DecimalNumber GridBefore
		{
			get
			{
				return this._gridBefore;
			}
			set
			{
				this._gridBefore = value;
			}
		}

		public CT_DecimalNumber GridAfter
		{
			get
			{
				return this._gridAfter;
			}
			set
			{
				this._gridAfter = value;
			}
		}

		public CT_TblWidth WBefore
		{
			get
			{
				return this._wBefore;
			}
			set
			{
				this._wBefore = value;
			}
		}

		public CT_TblWidth WAfter
		{
			get
			{
				return this._wAfter;
			}
			set
			{
				this._wAfter = value;
			}
		}

		public CT_OnOff CantSplit
		{
			get
			{
				return this._cantSplit;
			}
			set
			{
				this._cantSplit = value;
			}
		}

		public CT_Height TrHeight
		{
			get
			{
				return this._trHeight;
			}
			set
			{
				this._trHeight = value;
			}
		}

		public CT_OnOff TblHeader
		{
			get
			{
				return this._tblHeader;
			}
			set
			{
				this._tblHeader = value;
			}
		}

		public CT_TblWidth TblCellSpacing
		{
			get
			{
				return this._tblCellSpacing;
			}
			set
			{
				this._tblCellSpacing = value;
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

		public static string DivIdElementName
		{
			get
			{
				return "divId";
			}
		}

		public static string GridBeforeElementName
		{
			get
			{
				return "gridBefore";
			}
		}

		public static string GridAfterElementName
		{
			get
			{
				return "gridAfter";
			}
		}

		public static string WBeforeElementName
		{
			get
			{
				return "wBefore";
			}
		}

		public static string WAfterElementName
		{
			get
			{
				return "wAfter";
			}
		}

		public static string CantSplitElementName
		{
			get
			{
				return "cantSplit";
			}
		}

		public static string TrHeightElementName
		{
			get
			{
				return "trHeight";
			}
		}

		public static string TblHeaderElementName
		{
			get
			{
				return "tblHeader";
			}
		}

		public static string TblCellSpacingElementName
		{
			get
			{
				return "tblCellSpacing";
			}
		}

		public static string HiddenElementName
		{
			get
			{
				return "hidden";
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
			this.Write_divId(s);
			this.Write_gridBefore(s);
			this.Write_gridAfter(s);
			this.Write_wBefore(s);
			this.Write_wAfter(s);
			this.Write_cantSplit(s);
			this.Write_trHeight(s);
			this.Write_tblHeader(s);
			this.Write_tblCellSpacing(s);
			this.Write_hidden(s);
		}

		public void Write_divId(TextWriter s)
		{
			if (this._divId != null)
			{
				this._divId.Write(s, "divId");
			}
		}

		public void Write_gridBefore(TextWriter s)
		{
			if (this._gridBefore != null)
			{
				this._gridBefore.Write(s, "gridBefore");
			}
		}

		public void Write_gridAfter(TextWriter s)
		{
			if (this._gridAfter != null)
			{
				this._gridAfter.Write(s, "gridAfter");
			}
		}

		public void Write_wBefore(TextWriter s)
		{
			if (this._wBefore != null)
			{
				this._wBefore.Write(s, "wBefore");
			}
		}

		public void Write_wAfter(TextWriter s)
		{
			if (this._wAfter != null)
			{
				this._wAfter.Write(s, "wAfter");
			}
		}

		public void Write_cantSplit(TextWriter s)
		{
			if (this._cantSplit != null)
			{
				this._cantSplit.Write(s, "cantSplit");
			}
		}

		public void Write_trHeight(TextWriter s)
		{
			if (this._trHeight != null)
			{
				this._trHeight.Write(s, "trHeight");
			}
		}

		public void Write_tblHeader(TextWriter s)
		{
			if (this._tblHeader != null)
			{
				this._tblHeader.Write(s, "tblHeader");
			}
		}

		public void Write_tblCellSpacing(TextWriter s)
		{
			if (this._tblCellSpacing != null)
			{
				this._tblCellSpacing.Write(s, "tblCellSpacing");
			}
		}

		public void Write_hidden(TextWriter s)
		{
			if (this._hidden != null)
			{
				this._hidden.Write(s, "hidden");
			}
		}
	}
}
