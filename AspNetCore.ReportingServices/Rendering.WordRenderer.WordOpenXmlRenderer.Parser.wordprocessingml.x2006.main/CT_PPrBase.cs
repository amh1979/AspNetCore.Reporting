using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_PPrBase : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_String _pStyle;

		private CT_OnOff _keepNext;

		private CT_OnOff _keepLines;

		private CT_OnOff _pageBreakBefore;

		private CT_OnOff _widowControl;

		private CT_OnOff _suppressLineNumbers;

		private CT_OnOff _suppressAutoHyphens;

		private CT_OnOff _kinsoku;

		private CT_OnOff _wordWrap;

		private CT_OnOff _overflowPunct;

		private CT_OnOff _topLinePunct;

		private CT_OnOff _autoSpaceDE;

		private CT_OnOff _autoSpaceDN;

		private CT_OnOff _bidi;

		private CT_OnOff _adjustRightInd;

		private CT_OnOff _snapToGrid;

		private CT_Spacing _spacing;

		private CT_OnOff _contextualSpacing;

		private CT_OnOff _mirrorIndents;

		private CT_OnOff _suppressOverlap;

		private CT_Jc _jc;

		private CT_DecimalNumber _outlineLvl;

		private CT_DecimalNumber _divId;

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

		public CT_OnOff KeepNext
		{
			get
			{
				return this._keepNext;
			}
			set
			{
				this._keepNext = value;
			}
		}

		public CT_OnOff KeepLines
		{
			get
			{
				return this._keepLines;
			}
			set
			{
				this._keepLines = value;
			}
		}

		public CT_OnOff PageBreakBefore
		{
			get
			{
				return this._pageBreakBefore;
			}
			set
			{
				this._pageBreakBefore = value;
			}
		}

		public CT_OnOff WidowControl
		{
			get
			{
				return this._widowControl;
			}
			set
			{
				this._widowControl = value;
			}
		}

		public CT_OnOff SuppressLineNumbers
		{
			get
			{
				return this._suppressLineNumbers;
			}
			set
			{
				this._suppressLineNumbers = value;
			}
		}

		public CT_OnOff SuppressAutoHyphens
		{
			get
			{
				return this._suppressAutoHyphens;
			}
			set
			{
				this._suppressAutoHyphens = value;
			}
		}

		public CT_OnOff Kinsoku
		{
			get
			{
				return this._kinsoku;
			}
			set
			{
				this._kinsoku = value;
			}
		}

		public CT_OnOff WordWrap
		{
			get
			{
				return this._wordWrap;
			}
			set
			{
				this._wordWrap = value;
			}
		}

		public CT_OnOff OverflowPunct
		{
			get
			{
				return this._overflowPunct;
			}
			set
			{
				this._overflowPunct = value;
			}
		}

		public CT_OnOff TopLinePunct
		{
			get
			{
				return this._topLinePunct;
			}
			set
			{
				this._topLinePunct = value;
			}
		}

		public CT_OnOff AutoSpaceDE
		{
			get
			{
				return this._autoSpaceDE;
			}
			set
			{
				this._autoSpaceDE = value;
			}
		}

		public CT_OnOff AutoSpaceDN
		{
			get
			{
				return this._autoSpaceDN;
			}
			set
			{
				this._autoSpaceDN = value;
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

		public CT_OnOff AdjustRightInd
		{
			get
			{
				return this._adjustRightInd;
			}
			set
			{
				this._adjustRightInd = value;
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

		public CT_Spacing Spacing
		{
			get
			{
				return this._spacing;
			}
			set
			{
				this._spacing = value;
			}
		}

		public CT_OnOff ContextualSpacing
		{
			get
			{
				return this._contextualSpacing;
			}
			set
			{
				this._contextualSpacing = value;
			}
		}

		public CT_OnOff MirrorIndents
		{
			get
			{
				return this._mirrorIndents;
			}
			set
			{
				this._mirrorIndents = value;
			}
		}

		public CT_OnOff SuppressOverlap
		{
			get
			{
				return this._suppressOverlap;
			}
			set
			{
				this._suppressOverlap = value;
			}
		}

		public CT_Jc Jc
		{
			get
			{
				return this._jc;
			}
			set
			{
				this._jc = value;
			}
		}

		public CT_DecimalNumber OutlineLvl
		{
			get
			{
				return this._outlineLvl;
			}
			set
			{
				this._outlineLvl = value;
			}
		}

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

		public static string PStyleElementName
		{
			get
			{
				return "pStyle";
			}
		}

		public static string KeepNextElementName
		{
			get
			{
				return "keepNext";
			}
		}

		public static string KeepLinesElementName
		{
			get
			{
				return "keepLines";
			}
		}

		public static string PageBreakBeforeElementName
		{
			get
			{
				return "pageBreakBefore";
			}
		}

		public static string WidowControlElementName
		{
			get
			{
				return "widowControl";
			}
		}

		public static string SuppressLineNumbersElementName
		{
			get
			{
				return "suppressLineNumbers";
			}
		}

		public static string SuppressAutoHyphensElementName
		{
			get
			{
				return "suppressAutoHyphens";
			}
		}

		public static string KinsokuElementName
		{
			get
			{
				return "kinsoku";
			}
		}

		public static string WordWrapElementName
		{
			get
			{
				return "wordWrap";
			}
		}

		public static string OverflowPunctElementName
		{
			get
			{
				return "overflowPunct";
			}
		}

		public static string TopLinePunctElementName
		{
			get
			{
				return "topLinePunct";
			}
		}

		public static string AutoSpaceDEElementName
		{
			get
			{
				return "autoSpaceDE";
			}
		}

		public static string AutoSpaceDNElementName
		{
			get
			{
				return "autoSpaceDN";
			}
		}

		public static string BidiElementName
		{
			get
			{
				return "bidi";
			}
		}

		public static string AdjustRightIndElementName
		{
			get
			{
				return "adjustRightInd";
			}
		}

		public static string SnapToGridElementName
		{
			get
			{
				return "snapToGrid";
			}
		}

		public static string SpacingElementName
		{
			get
			{
				return "spacing";
			}
		}

		public static string ContextualSpacingElementName
		{
			get
			{
				return "contextualSpacing";
			}
		}

		public static string MirrorIndentsElementName
		{
			get
			{
				return "mirrorIndents";
			}
		}

		public static string SuppressOverlapElementName
		{
			get
			{
				return "suppressOverlap";
			}
		}

		public static string JcElementName
		{
			get
			{
				return "jc";
			}
		}

		public static string OutlineLvlElementName
		{
			get
			{
				return "outlineLvl";
			}
		}

		public static string DivIdElementName
		{
			get
			{
				return "divId";
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
			this.Write_pStyle(s);
			this.Write_keepNext(s);
			this.Write_keepLines(s);
			this.Write_pageBreakBefore(s);
			this.Write_widowControl(s);
			this.Write_suppressLineNumbers(s);
			this.Write_suppressAutoHyphens(s);
			this.Write_kinsoku(s);
			this.Write_wordWrap(s);
			this.Write_overflowPunct(s);
			this.Write_topLinePunct(s);
			this.Write_autoSpaceDE(s);
			this.Write_autoSpaceDN(s);
			this.Write_bidi(s);
			this.Write_adjustRightInd(s);
			this.Write_snapToGrid(s);
			this.Write_spacing(s);
			this.Write_contextualSpacing(s);
			this.Write_mirrorIndents(s);
			this.Write_suppressOverlap(s);
			this.Write_jc(s);
			this.Write_outlineLvl(s);
			this.Write_divId(s);
		}

		public void Write_pStyle(TextWriter s)
		{
			if (this._pStyle != null)
			{
				this._pStyle.Write(s, "pStyle");
			}
		}

		public void Write_keepNext(TextWriter s)
		{
			if (this._keepNext != null)
			{
				this._keepNext.Write(s, "keepNext");
			}
		}

		public void Write_keepLines(TextWriter s)
		{
			if (this._keepLines != null)
			{
				this._keepLines.Write(s, "keepLines");
			}
		}

		public void Write_pageBreakBefore(TextWriter s)
		{
			if (this._pageBreakBefore != null)
			{
				this._pageBreakBefore.Write(s, "pageBreakBefore");
			}
		}

		public void Write_widowControl(TextWriter s)
		{
			if (this._widowControl != null)
			{
				this._widowControl.Write(s, "widowControl");
			}
		}

		public void Write_suppressLineNumbers(TextWriter s)
		{
			if (this._suppressLineNumbers != null)
			{
				this._suppressLineNumbers.Write(s, "suppressLineNumbers");
			}
		}

		public void Write_suppressAutoHyphens(TextWriter s)
		{
			if (this._suppressAutoHyphens != null)
			{
				this._suppressAutoHyphens.Write(s, "suppressAutoHyphens");
			}
		}

		public void Write_kinsoku(TextWriter s)
		{
			if (this._kinsoku != null)
			{
				this._kinsoku.Write(s, "kinsoku");
			}
		}

		public void Write_wordWrap(TextWriter s)
		{
			if (this._wordWrap != null)
			{
				this._wordWrap.Write(s, "wordWrap");
			}
		}

		public void Write_overflowPunct(TextWriter s)
		{
			if (this._overflowPunct != null)
			{
				this._overflowPunct.Write(s, "overflowPunct");
			}
		}

		public void Write_topLinePunct(TextWriter s)
		{
			if (this._topLinePunct != null)
			{
				this._topLinePunct.Write(s, "topLinePunct");
			}
		}

		public void Write_autoSpaceDE(TextWriter s)
		{
			if (this._autoSpaceDE != null)
			{
				this._autoSpaceDE.Write(s, "autoSpaceDE");
			}
		}

		public void Write_autoSpaceDN(TextWriter s)
		{
			if (this._autoSpaceDN != null)
			{
				this._autoSpaceDN.Write(s, "autoSpaceDN");
			}
		}

		public void Write_bidi(TextWriter s)
		{
			if (this._bidi != null)
			{
				this._bidi.Write(s, "bidi");
			}
		}

		public void Write_adjustRightInd(TextWriter s)
		{
			if (this._adjustRightInd != null)
			{
				this._adjustRightInd.Write(s, "adjustRightInd");
			}
		}

		public void Write_snapToGrid(TextWriter s)
		{
			if (this._snapToGrid != null)
			{
				this._snapToGrid.Write(s, "snapToGrid");
			}
		}

		public void Write_spacing(TextWriter s)
		{
			if (this._spacing != null)
			{
				this._spacing.Write(s, "spacing");
			}
		}

		public void Write_contextualSpacing(TextWriter s)
		{
			if (this._contextualSpacing != null)
			{
				this._contextualSpacing.Write(s, "contextualSpacing");
			}
		}

		public void Write_mirrorIndents(TextWriter s)
		{
			if (this._mirrorIndents != null)
			{
				this._mirrorIndents.Write(s, "mirrorIndents");
			}
		}

		public void Write_suppressOverlap(TextWriter s)
		{
			if (this._suppressOverlap != null)
			{
				this._suppressOverlap.Write(s, "suppressOverlap");
			}
		}

		public void Write_jc(TextWriter s)
		{
			if (this._jc != null)
			{
				this._jc.Write(s, "jc");
			}
		}

		public void Write_outlineLvl(TextWriter s)
		{
			if (this._outlineLvl != null)
			{
				this._outlineLvl.Write(s, "outlineLvl");
			}
		}

		public void Write_divId(TextWriter s)
		{
			if (this._divId != null)
			{
				this._divId.Write(s, "divId");
			}
		}
	}
}
