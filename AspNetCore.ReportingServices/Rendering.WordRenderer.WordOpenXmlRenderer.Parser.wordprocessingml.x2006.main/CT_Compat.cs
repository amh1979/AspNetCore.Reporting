using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Compat : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_OnOff _useSingleBorderforContiguousCells;

		private CT_OnOff _wpJustification;

		private CT_OnOff _noTabHangInd;

		private CT_OnOff _noLeading;

		private CT_OnOff _spaceForUL;

		private CT_OnOff _noColumnBalance;

		private CT_OnOff _balanceSingleByteDoubleByteWidth;

		private CT_OnOff _noExtraLineSpacing;

		private CT_OnOff _doNotLeaveBackslashAlone;

		private CT_OnOff _ulTrailSpace;

		private CT_OnOff _doNotExpandShiftReturn;

		private CT_OnOff _spacingInWholePoints;

		private CT_OnOff _lineWrapLikeWord6;

		private CT_OnOff _printBodyTextBeforeHeader;

		private CT_OnOff _printColBlack;

		private CT_OnOff _wpSpaceWidth;

		private CT_OnOff _showBreaksInFrames;

		private CT_OnOff _subFontBySize;

		private CT_OnOff _suppressBottomSpacing;

		private CT_OnOff _suppressTopSpacing;

		private CT_OnOff _suppressSpacingAtTopOfPage;

		private CT_OnOff _suppressTopSpacingWP;

		private CT_OnOff _suppressSpBfAfterPgBrk;

		private CT_OnOff _swapBordersFacingPages;

		private CT_OnOff _convMailMergeEsc;

		private CT_OnOff _truncateFontHeightsLikeWP6;

		private CT_OnOff _mwSmallCaps;

		private CT_OnOff _usePrinterMetrics;

		private CT_OnOff _doNotSuppressParagraphBorders;

		private CT_OnOff _wrapTrailSpaces;

		private CT_OnOff _footnoteLayoutLikeWW8;

		private CT_OnOff _shapeLayoutLikeWW8;

		private CT_OnOff _alignTablesRowByRow;

		private CT_OnOff _forgetLastTabAlignment;

		private CT_OnOff _adjustLineHeightInTable;

		private CT_OnOff _autoSpaceLikeWord95;

		private CT_OnOff _noSpaceRaiseLower;

		private CT_OnOff _doNotUseHTMLParagraphAutoSpacing;

		private CT_OnOff _layoutRawTableWidth;

		private CT_OnOff _layoutTableRowsApart;

		private CT_OnOff _useWord97LineBreakRules;

		private CT_OnOff _doNotBreakWrappedTables;

		private CT_OnOff _doNotSnapToGridInCell;

		private CT_OnOff _selectFldWithFirstOrLastChar;

		private CT_OnOff _applyBreakingRules;

		private CT_OnOff _doNotWrapTextWithPunct;

		private CT_OnOff _doNotUseEastAsianBreakRules;

		private CT_OnOff _useWord2002TableStyleRules;

		private CT_OnOff _growAutofit;

		private CT_OnOff _useFELayout;

		private CT_OnOff _useNormalStyleForList;

		private CT_OnOff _doNotUseIndentAsNumberingTabStop;

		private CT_OnOff _useAltKinsokuLineBreakRules;

		private CT_OnOff _allowSpaceOfSameStyleInTable;

		private CT_OnOff _doNotSuppressIndentation;

		private CT_OnOff _doNotAutofitConstrainedTables;

		private CT_OnOff _autofitToFirstFixedWidthCell;

		private CT_OnOff _underlineTabInNumList;

		private CT_OnOff _displayHangulFixedWidth;

		private CT_OnOff _splitPgBreakAndParaMark;

		private CT_OnOff _doNotVertAlignCellWithSp;

		private CT_OnOff _doNotBreakConstrainedForcedTable;

		private CT_OnOff _doNotVertAlignInTxbx;

		private CT_OnOff _useAnsiKerningPairs;

		private CT_OnOff _cachedColBalance;

		public CT_OnOff UseSingleBorderforContiguousCells
		{
			get
			{
				return this._useSingleBorderforContiguousCells;
			}
			set
			{
				this._useSingleBorderforContiguousCells = value;
			}
		}

		public CT_OnOff WpJustification
		{
			get
			{
				return this._wpJustification;
			}
			set
			{
				this._wpJustification = value;
			}
		}

		public CT_OnOff NoTabHangInd
		{
			get
			{
				return this._noTabHangInd;
			}
			set
			{
				this._noTabHangInd = value;
			}
		}

		public CT_OnOff NoLeading
		{
			get
			{
				return this._noLeading;
			}
			set
			{
				this._noLeading = value;
			}
		}

		public CT_OnOff SpaceForUL
		{
			get
			{
				return this._spaceForUL;
			}
			set
			{
				this._spaceForUL = value;
			}
		}

		public CT_OnOff NoColumnBalance
		{
			get
			{
				return this._noColumnBalance;
			}
			set
			{
				this._noColumnBalance = value;
			}
		}

		public CT_OnOff BalanceSingleByteDoubleByteWidth
		{
			get
			{
				return this._balanceSingleByteDoubleByteWidth;
			}
			set
			{
				this._balanceSingleByteDoubleByteWidth = value;
			}
		}

		public CT_OnOff NoExtraLineSpacing
		{
			get
			{
				return this._noExtraLineSpacing;
			}
			set
			{
				this._noExtraLineSpacing = value;
			}
		}

		public CT_OnOff DoNotLeaveBackslashAlone
		{
			get
			{
				return this._doNotLeaveBackslashAlone;
			}
			set
			{
				this._doNotLeaveBackslashAlone = value;
			}
		}

		public CT_OnOff UlTrailSpace
		{
			get
			{
				return this._ulTrailSpace;
			}
			set
			{
				this._ulTrailSpace = value;
			}
		}

		public CT_OnOff DoNotExpandShiftReturn
		{
			get
			{
				return this._doNotExpandShiftReturn;
			}
			set
			{
				this._doNotExpandShiftReturn = value;
			}
		}

		public CT_OnOff SpacingInWholePoints
		{
			get
			{
				return this._spacingInWholePoints;
			}
			set
			{
				this._spacingInWholePoints = value;
			}
		}

		public CT_OnOff LineWrapLikeWord6
		{
			get
			{
				return this._lineWrapLikeWord6;
			}
			set
			{
				this._lineWrapLikeWord6 = value;
			}
		}

		public CT_OnOff PrintBodyTextBeforeHeader
		{
			get
			{
				return this._printBodyTextBeforeHeader;
			}
			set
			{
				this._printBodyTextBeforeHeader = value;
			}
		}

		public CT_OnOff PrintColBlack
		{
			get
			{
				return this._printColBlack;
			}
			set
			{
				this._printColBlack = value;
			}
		}

		public CT_OnOff WpSpaceWidth
		{
			get
			{
				return this._wpSpaceWidth;
			}
			set
			{
				this._wpSpaceWidth = value;
			}
		}

		public CT_OnOff ShowBreaksInFrames
		{
			get
			{
				return this._showBreaksInFrames;
			}
			set
			{
				this._showBreaksInFrames = value;
			}
		}

		public CT_OnOff SubFontBySize
		{
			get
			{
				return this._subFontBySize;
			}
			set
			{
				this._subFontBySize = value;
			}
		}

		public CT_OnOff SuppressBottomSpacing
		{
			get
			{
				return this._suppressBottomSpacing;
			}
			set
			{
				this._suppressBottomSpacing = value;
			}
		}

		public CT_OnOff SuppressTopSpacing
		{
			get
			{
				return this._suppressTopSpacing;
			}
			set
			{
				this._suppressTopSpacing = value;
			}
		}

		public CT_OnOff SuppressSpacingAtTopOfPage
		{
			get
			{
				return this._suppressSpacingAtTopOfPage;
			}
			set
			{
				this._suppressSpacingAtTopOfPage = value;
			}
		}

		public CT_OnOff SuppressTopSpacingWP
		{
			get
			{
				return this._suppressTopSpacingWP;
			}
			set
			{
				this._suppressTopSpacingWP = value;
			}
		}

		public CT_OnOff SuppressSpBfAfterPgBrk
		{
			get
			{
				return this._suppressSpBfAfterPgBrk;
			}
			set
			{
				this._suppressSpBfAfterPgBrk = value;
			}
		}

		public CT_OnOff SwapBordersFacingPages
		{
			get
			{
				return this._swapBordersFacingPages;
			}
			set
			{
				this._swapBordersFacingPages = value;
			}
		}

		public CT_OnOff ConvMailMergeEsc
		{
			get
			{
				return this._convMailMergeEsc;
			}
			set
			{
				this._convMailMergeEsc = value;
			}
		}

		public CT_OnOff TruncateFontHeightsLikeWP6
		{
			get
			{
				return this._truncateFontHeightsLikeWP6;
			}
			set
			{
				this._truncateFontHeightsLikeWP6 = value;
			}
		}

		public CT_OnOff MwSmallCaps
		{
			get
			{
				return this._mwSmallCaps;
			}
			set
			{
				this._mwSmallCaps = value;
			}
		}

		public CT_OnOff UsePrinterMetrics
		{
			get
			{
				return this._usePrinterMetrics;
			}
			set
			{
				this._usePrinterMetrics = value;
			}
		}

		public CT_OnOff DoNotSuppressParagraphBorders
		{
			get
			{
				return this._doNotSuppressParagraphBorders;
			}
			set
			{
				this._doNotSuppressParagraphBorders = value;
			}
		}

		public CT_OnOff WrapTrailSpaces
		{
			get
			{
				return this._wrapTrailSpaces;
			}
			set
			{
				this._wrapTrailSpaces = value;
			}
		}

		public CT_OnOff FootnoteLayoutLikeWW8
		{
			get
			{
				return this._footnoteLayoutLikeWW8;
			}
			set
			{
				this._footnoteLayoutLikeWW8 = value;
			}
		}

		public CT_OnOff ShapeLayoutLikeWW8
		{
			get
			{
				return this._shapeLayoutLikeWW8;
			}
			set
			{
				this._shapeLayoutLikeWW8 = value;
			}
		}

		public CT_OnOff AlignTablesRowByRow
		{
			get
			{
				return this._alignTablesRowByRow;
			}
			set
			{
				this._alignTablesRowByRow = value;
			}
		}

		public CT_OnOff ForgetLastTabAlignment
		{
			get
			{
				return this._forgetLastTabAlignment;
			}
			set
			{
				this._forgetLastTabAlignment = value;
			}
		}

		public CT_OnOff AdjustLineHeightInTable
		{
			get
			{
				return this._adjustLineHeightInTable;
			}
			set
			{
				this._adjustLineHeightInTable = value;
			}
		}

		public CT_OnOff AutoSpaceLikeWord95
		{
			get
			{
				return this._autoSpaceLikeWord95;
			}
			set
			{
				this._autoSpaceLikeWord95 = value;
			}
		}

		public CT_OnOff NoSpaceRaiseLower
		{
			get
			{
				return this._noSpaceRaiseLower;
			}
			set
			{
				this._noSpaceRaiseLower = value;
			}
		}

		public CT_OnOff DoNotUseHTMLParagraphAutoSpacing
		{
			get
			{
				return this._doNotUseHTMLParagraphAutoSpacing;
			}
			set
			{
				this._doNotUseHTMLParagraphAutoSpacing = value;
			}
		}

		public CT_OnOff LayoutRawTableWidth
		{
			get
			{
				return this._layoutRawTableWidth;
			}
			set
			{
				this._layoutRawTableWidth = value;
			}
		}

		public CT_OnOff LayoutTableRowsApart
		{
			get
			{
				return this._layoutTableRowsApart;
			}
			set
			{
				this._layoutTableRowsApart = value;
			}
		}

		public CT_OnOff UseWord97LineBreakRules
		{
			get
			{
				return this._useWord97LineBreakRules;
			}
			set
			{
				this._useWord97LineBreakRules = value;
			}
		}

		public CT_OnOff DoNotBreakWrappedTables
		{
			get
			{
				return this._doNotBreakWrappedTables;
			}
			set
			{
				this._doNotBreakWrappedTables = value;
			}
		}

		public CT_OnOff DoNotSnapToGridInCell
		{
			get
			{
				return this._doNotSnapToGridInCell;
			}
			set
			{
				this._doNotSnapToGridInCell = value;
			}
		}

		public CT_OnOff SelectFldWithFirstOrLastChar
		{
			get
			{
				return this._selectFldWithFirstOrLastChar;
			}
			set
			{
				this._selectFldWithFirstOrLastChar = value;
			}
		}

		public CT_OnOff ApplyBreakingRules
		{
			get
			{
				return this._applyBreakingRules;
			}
			set
			{
				this._applyBreakingRules = value;
			}
		}

		public CT_OnOff DoNotWrapTextWithPunct
		{
			get
			{
				return this._doNotWrapTextWithPunct;
			}
			set
			{
				this._doNotWrapTextWithPunct = value;
			}
		}

		public CT_OnOff DoNotUseEastAsianBreakRules
		{
			get
			{
				return this._doNotUseEastAsianBreakRules;
			}
			set
			{
				this._doNotUseEastAsianBreakRules = value;
			}
		}

		public CT_OnOff UseWord2002TableStyleRules
		{
			get
			{
				return this._useWord2002TableStyleRules;
			}
			set
			{
				this._useWord2002TableStyleRules = value;
			}
		}

		public CT_OnOff GrowAutofit
		{
			get
			{
				return this._growAutofit;
			}
			set
			{
				this._growAutofit = value;
			}
		}

		public CT_OnOff UseFELayout
		{
			get
			{
				return this._useFELayout;
			}
			set
			{
				this._useFELayout = value;
			}
		}

		public CT_OnOff UseNormalStyleForList
		{
			get
			{
				return this._useNormalStyleForList;
			}
			set
			{
				this._useNormalStyleForList = value;
			}
		}

		public CT_OnOff DoNotUseIndentAsNumberingTabStop
		{
			get
			{
				return this._doNotUseIndentAsNumberingTabStop;
			}
			set
			{
				this._doNotUseIndentAsNumberingTabStop = value;
			}
		}

		public CT_OnOff UseAltKinsokuLineBreakRules
		{
			get
			{
				return this._useAltKinsokuLineBreakRules;
			}
			set
			{
				this._useAltKinsokuLineBreakRules = value;
			}
		}

		public CT_OnOff AllowSpaceOfSameStyleInTable
		{
			get
			{
				return this._allowSpaceOfSameStyleInTable;
			}
			set
			{
				this._allowSpaceOfSameStyleInTable = value;
			}
		}

		public CT_OnOff DoNotSuppressIndentation
		{
			get
			{
				return this._doNotSuppressIndentation;
			}
			set
			{
				this._doNotSuppressIndentation = value;
			}
		}

		public CT_OnOff DoNotAutofitConstrainedTables
		{
			get
			{
				return this._doNotAutofitConstrainedTables;
			}
			set
			{
				this._doNotAutofitConstrainedTables = value;
			}
		}

		public CT_OnOff AutofitToFirstFixedWidthCell
		{
			get
			{
				return this._autofitToFirstFixedWidthCell;
			}
			set
			{
				this._autofitToFirstFixedWidthCell = value;
			}
		}

		public CT_OnOff UnderlineTabInNumList
		{
			get
			{
				return this._underlineTabInNumList;
			}
			set
			{
				this._underlineTabInNumList = value;
			}
		}

		public CT_OnOff DisplayHangulFixedWidth
		{
			get
			{
				return this._displayHangulFixedWidth;
			}
			set
			{
				this._displayHangulFixedWidth = value;
			}
		}

		public CT_OnOff SplitPgBreakAndParaMark
		{
			get
			{
				return this._splitPgBreakAndParaMark;
			}
			set
			{
				this._splitPgBreakAndParaMark = value;
			}
		}

		public CT_OnOff DoNotVertAlignCellWithSp
		{
			get
			{
				return this._doNotVertAlignCellWithSp;
			}
			set
			{
				this._doNotVertAlignCellWithSp = value;
			}
		}

		public CT_OnOff DoNotBreakConstrainedForcedTable
		{
			get
			{
				return this._doNotBreakConstrainedForcedTable;
			}
			set
			{
				this._doNotBreakConstrainedForcedTable = value;
			}
		}

		public CT_OnOff DoNotVertAlignInTxbx
		{
			get
			{
				return this._doNotVertAlignInTxbx;
			}
			set
			{
				this._doNotVertAlignInTxbx = value;
			}
		}

		public CT_OnOff UseAnsiKerningPairs
		{
			get
			{
				return this._useAnsiKerningPairs;
			}
			set
			{
				this._useAnsiKerningPairs = value;
			}
		}

		public CT_OnOff CachedColBalance
		{
			get
			{
				return this._cachedColBalance;
			}
			set
			{
				this._cachedColBalance = value;
			}
		}

		public static string UseSingleBorderforContiguousCellsElementName
		{
			get
			{
				return "useSingleBorderforContiguousCells";
			}
		}

		public static string WpJustificationElementName
		{
			get
			{
				return "wpJustification";
			}
		}

		public static string NoTabHangIndElementName
		{
			get
			{
				return "noTabHangInd";
			}
		}

		public static string NoLeadingElementName
		{
			get
			{
				return "noLeading";
			}
		}

		public static string SpaceForULElementName
		{
			get
			{
				return "spaceForUL";
			}
		}

		public static string NoColumnBalanceElementName
		{
			get
			{
				return "noColumnBalance";
			}
		}

		public static string BalanceSingleByteDoubleByteWidthElementName
		{
			get
			{
				return "balanceSingleByteDoubleByteWidth";
			}
		}

		public static string NoExtraLineSpacingElementName
		{
			get
			{
				return "noExtraLineSpacing";
			}
		}

		public static string DoNotLeaveBackslashAloneElementName
		{
			get
			{
				return "doNotLeaveBackslashAlone";
			}
		}

		public static string UlTrailSpaceElementName
		{
			get
			{
				return "ulTrailSpace";
			}
		}

		public static string DoNotExpandShiftReturnElementName
		{
			get
			{
				return "doNotExpandShiftReturn";
			}
		}

		public static string SpacingInWholePointsElementName
		{
			get
			{
				return "spacingInWholePoints";
			}
		}

		public static string LineWrapLikeWord6ElementName
		{
			get
			{
				return "lineWrapLikeWord6";
			}
		}

		public static string PrintBodyTextBeforeHeaderElementName
		{
			get
			{
				return "printBodyTextBeforeHeader";
			}
		}

		public static string PrintColBlackElementName
		{
			get
			{
				return "printColBlack";
			}
		}

		public static string WpSpaceWidthElementName
		{
			get
			{
				return "wpSpaceWidth";
			}
		}

		public static string ShowBreaksInFramesElementName
		{
			get
			{
				return "showBreaksInFrames";
			}
		}

		public static string SubFontBySizeElementName
		{
			get
			{
				return "subFontBySize";
			}
		}

		public static string SuppressBottomSpacingElementName
		{
			get
			{
				return "suppressBottomSpacing";
			}
		}

		public static string SuppressTopSpacingElementName
		{
			get
			{
				return "suppressTopSpacing";
			}
		}

		public static string SuppressSpacingAtTopOfPageElementName
		{
			get
			{
				return "suppressSpacingAtTopOfPage";
			}
		}

		public static string SuppressTopSpacingWPElementName
		{
			get
			{
				return "suppressTopSpacingWP";
			}
		}

		public static string SuppressSpBfAfterPgBrkElementName
		{
			get
			{
				return "suppressSpBfAfterPgBrk";
			}
		}

		public static string SwapBordersFacingPagesElementName
		{
			get
			{
				return "swapBordersFacingPages";
			}
		}

		public static string ConvMailMergeEscElementName
		{
			get
			{
				return "convMailMergeEsc";
			}
		}

		public static string TruncateFontHeightsLikeWP6ElementName
		{
			get
			{
				return "truncateFontHeightsLikeWP6";
			}
		}

		public static string MwSmallCapsElementName
		{
			get
			{
				return "mwSmallCaps";
			}
		}

		public static string UsePrinterMetricsElementName
		{
			get
			{
				return "usePrinterMetrics";
			}
		}

		public static string DoNotSuppressParagraphBordersElementName
		{
			get
			{
				return "doNotSuppressParagraphBorders";
			}
		}

		public static string WrapTrailSpacesElementName
		{
			get
			{
				return "wrapTrailSpaces";
			}
		}

		public static string FootnoteLayoutLikeWW8ElementName
		{
			get
			{
				return "footnoteLayoutLikeWW8";
			}
		}

		public static string ShapeLayoutLikeWW8ElementName
		{
			get
			{
				return "shapeLayoutLikeWW8";
			}
		}

		public static string AlignTablesRowByRowElementName
		{
			get
			{
				return "alignTablesRowByRow";
			}
		}

		public static string ForgetLastTabAlignmentElementName
		{
			get
			{
				return "forgetLastTabAlignment";
			}
		}

		public static string AdjustLineHeightInTableElementName
		{
			get
			{
				return "adjustLineHeightInTable";
			}
		}

		public static string AutoSpaceLikeWord95ElementName
		{
			get
			{
				return "autoSpaceLikeWord95";
			}
		}

		public static string NoSpaceRaiseLowerElementName
		{
			get
			{
				return "noSpaceRaiseLower";
			}
		}

		public static string DoNotUseHTMLParagraphAutoSpacingElementName
		{
			get
			{
				return "doNotUseHTMLParagraphAutoSpacing";
			}
		}

		public static string LayoutRawTableWidthElementName
		{
			get
			{
				return "layoutRawTableWidth";
			}
		}

		public static string LayoutTableRowsApartElementName
		{
			get
			{
				return "layoutTableRowsApart";
			}
		}

		public static string UseWord97LineBreakRulesElementName
		{
			get
			{
				return "useWord97LineBreakRules";
			}
		}

		public static string DoNotBreakWrappedTablesElementName
		{
			get
			{
				return "doNotBreakWrappedTables";
			}
		}

		public static string DoNotSnapToGridInCellElementName
		{
			get
			{
				return "doNotSnapToGridInCell";
			}
		}

		public static string SelectFldWithFirstOrLastCharElementName
		{
			get
			{
				return "selectFldWithFirstOrLastChar";
			}
		}

		public static string ApplyBreakingRulesElementName
		{
			get
			{
				return "applyBreakingRules";
			}
		}

		public static string DoNotWrapTextWithPunctElementName
		{
			get
			{
				return "doNotWrapTextWithPunct";
			}
		}

		public static string DoNotUseEastAsianBreakRulesElementName
		{
			get
			{
				return "doNotUseEastAsianBreakRules";
			}
		}

		public static string UseWord2002TableStyleRulesElementName
		{
			get
			{
				return "useWord2002TableStyleRules";
			}
		}

		public static string GrowAutofitElementName
		{
			get
			{
				return "growAutofit";
			}
		}

		public static string UseFELayoutElementName
		{
			get
			{
				return "useFELayout";
			}
		}

		public static string UseNormalStyleForListElementName
		{
			get
			{
				return "useNormalStyleForList";
			}
		}

		public static string DoNotUseIndentAsNumberingTabStopElementName
		{
			get
			{
				return "doNotUseIndentAsNumberingTabStop";
			}
		}

		public static string UseAltKinsokuLineBreakRulesElementName
		{
			get
			{
				return "useAltKinsokuLineBreakRules";
			}
		}

		public static string AllowSpaceOfSameStyleInTableElementName
		{
			get
			{
				return "allowSpaceOfSameStyleInTable";
			}
		}

		public static string DoNotSuppressIndentationElementName
		{
			get
			{
				return "doNotSuppressIndentation";
			}
		}

		public static string DoNotAutofitConstrainedTablesElementName
		{
			get
			{
				return "doNotAutofitConstrainedTables";
			}
		}

		public static string AutofitToFirstFixedWidthCellElementName
		{
			get
			{
				return "autofitToFirstFixedWidthCell";
			}
		}

		public static string UnderlineTabInNumListElementName
		{
			get
			{
				return "underlineTabInNumList";
			}
		}

		public static string DisplayHangulFixedWidthElementName
		{
			get
			{
				return "displayHangulFixedWidth";
			}
		}

		public static string SplitPgBreakAndParaMarkElementName
		{
			get
			{
				return "splitPgBreakAndParaMark";
			}
		}

		public static string DoNotVertAlignCellWithSpElementName
		{
			get
			{
				return "doNotVertAlignCellWithSp";
			}
		}

		public static string DoNotBreakConstrainedForcedTableElementName
		{
			get
			{
				return "doNotBreakConstrainedForcedTable";
			}
		}

		public static string DoNotVertAlignInTxbxElementName
		{
			get
			{
				return "doNotVertAlignInTxbx";
			}
		}

		public static string UseAnsiKerningPairsElementName
		{
			get
			{
				return "useAnsiKerningPairs";
			}
		}

		public static string CachedColBalanceElementName
		{
			get
			{
				return "cachedColBalance";
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
			this.Write_useSingleBorderforContiguousCells(s);
			this.Write_wpJustification(s);
			this.Write_noTabHangInd(s);
			this.Write_noLeading(s);
			this.Write_spaceForUL(s);
			this.Write_noColumnBalance(s);
			this.Write_balanceSingleByteDoubleByteWidth(s);
			this.Write_noExtraLineSpacing(s);
			this.Write_doNotLeaveBackslashAlone(s);
			this.Write_ulTrailSpace(s);
			this.Write_doNotExpandShiftReturn(s);
			this.Write_spacingInWholePoints(s);
			this.Write_lineWrapLikeWord6(s);
			this.Write_printBodyTextBeforeHeader(s);
			this.Write_printColBlack(s);
			this.Write_wpSpaceWidth(s);
			this.Write_showBreaksInFrames(s);
			this.Write_subFontBySize(s);
			this.Write_suppressBottomSpacing(s);
			this.Write_suppressTopSpacing(s);
			this.Write_suppressSpacingAtTopOfPage(s);
			this.Write_suppressTopSpacingWP(s);
			this.Write_suppressSpBfAfterPgBrk(s);
			this.Write_swapBordersFacingPages(s);
			this.Write_convMailMergeEsc(s);
			this.Write_truncateFontHeightsLikeWP6(s);
			this.Write_mwSmallCaps(s);
			this.Write_usePrinterMetrics(s);
			this.Write_doNotSuppressParagraphBorders(s);
			this.Write_wrapTrailSpaces(s);
			this.Write_footnoteLayoutLikeWW8(s);
			this.Write_shapeLayoutLikeWW8(s);
			this.Write_alignTablesRowByRow(s);
			this.Write_forgetLastTabAlignment(s);
			this.Write_adjustLineHeightInTable(s);
			this.Write_autoSpaceLikeWord95(s);
			this.Write_noSpaceRaiseLower(s);
			this.Write_doNotUseHTMLParagraphAutoSpacing(s);
			this.Write_layoutRawTableWidth(s);
			this.Write_layoutTableRowsApart(s);
			this.Write_useWord97LineBreakRules(s);
			this.Write_doNotBreakWrappedTables(s);
			this.Write_doNotSnapToGridInCell(s);
			this.Write_selectFldWithFirstOrLastChar(s);
			this.Write_applyBreakingRules(s);
			this.Write_doNotWrapTextWithPunct(s);
			this.Write_doNotUseEastAsianBreakRules(s);
			this.Write_useWord2002TableStyleRules(s);
			this.Write_growAutofit(s);
			this.Write_useFELayout(s);
			this.Write_useNormalStyleForList(s);
			this.Write_doNotUseIndentAsNumberingTabStop(s);
			this.Write_useAltKinsokuLineBreakRules(s);
			this.Write_allowSpaceOfSameStyleInTable(s);
			this.Write_doNotSuppressIndentation(s);
			this.Write_doNotAutofitConstrainedTables(s);
			this.Write_autofitToFirstFixedWidthCell(s);
			this.Write_underlineTabInNumList(s);
			this.Write_displayHangulFixedWidth(s);
			this.Write_splitPgBreakAndParaMark(s);
			this.Write_doNotVertAlignCellWithSp(s);
			this.Write_doNotBreakConstrainedForcedTable(s);
			this.Write_doNotVertAlignInTxbx(s);
			this.Write_useAnsiKerningPairs(s);
			this.Write_cachedColBalance(s);
		}

		public void Write_useSingleBorderforContiguousCells(TextWriter s)
		{
			if (this._useSingleBorderforContiguousCells != null)
			{
				this._useSingleBorderforContiguousCells.Write(s, "useSingleBorderforContiguousCells");
			}
		}

		public void Write_wpJustification(TextWriter s)
		{
			if (this._wpJustification != null)
			{
				this._wpJustification.Write(s, "wpJustification");
			}
		}

		public void Write_noTabHangInd(TextWriter s)
		{
			if (this._noTabHangInd != null)
			{
				this._noTabHangInd.Write(s, "noTabHangInd");
			}
		}

		public void Write_noLeading(TextWriter s)
		{
			if (this._noLeading != null)
			{
				this._noLeading.Write(s, "noLeading");
			}
		}

		public void Write_spaceForUL(TextWriter s)
		{
			if (this._spaceForUL != null)
			{
				this._spaceForUL.Write(s, "spaceForUL");
			}
		}

		public void Write_noColumnBalance(TextWriter s)
		{
			if (this._noColumnBalance != null)
			{
				this._noColumnBalance.Write(s, "noColumnBalance");
			}
		}

		public void Write_balanceSingleByteDoubleByteWidth(TextWriter s)
		{
			if (this._balanceSingleByteDoubleByteWidth != null)
			{
				this._balanceSingleByteDoubleByteWidth.Write(s, "balanceSingleByteDoubleByteWidth");
			}
		}

		public void Write_noExtraLineSpacing(TextWriter s)
		{
			if (this._noExtraLineSpacing != null)
			{
				this._noExtraLineSpacing.Write(s, "noExtraLineSpacing");
			}
		}

		public void Write_doNotLeaveBackslashAlone(TextWriter s)
		{
			if (this._doNotLeaveBackslashAlone != null)
			{
				this._doNotLeaveBackslashAlone.Write(s, "doNotLeaveBackslashAlone");
			}
		}

		public void Write_ulTrailSpace(TextWriter s)
		{
			if (this._ulTrailSpace != null)
			{
				this._ulTrailSpace.Write(s, "ulTrailSpace");
			}
		}

		public void Write_doNotExpandShiftReturn(TextWriter s)
		{
			if (this._doNotExpandShiftReturn != null)
			{
				this._doNotExpandShiftReturn.Write(s, "doNotExpandShiftReturn");
			}
		}

		public void Write_spacingInWholePoints(TextWriter s)
		{
			if (this._spacingInWholePoints != null)
			{
				this._spacingInWholePoints.Write(s, "spacingInWholePoints");
			}
		}

		public void Write_lineWrapLikeWord6(TextWriter s)
		{
			if (this._lineWrapLikeWord6 != null)
			{
				this._lineWrapLikeWord6.Write(s, "lineWrapLikeWord6");
			}
		}

		public void Write_printBodyTextBeforeHeader(TextWriter s)
		{
			if (this._printBodyTextBeforeHeader != null)
			{
				this._printBodyTextBeforeHeader.Write(s, "printBodyTextBeforeHeader");
			}
		}

		public void Write_printColBlack(TextWriter s)
		{
			if (this._printColBlack != null)
			{
				this._printColBlack.Write(s, "printColBlack");
			}
		}

		public void Write_wpSpaceWidth(TextWriter s)
		{
			if (this._wpSpaceWidth != null)
			{
				this._wpSpaceWidth.Write(s, "wpSpaceWidth");
			}
		}

		public void Write_showBreaksInFrames(TextWriter s)
		{
			if (this._showBreaksInFrames != null)
			{
				this._showBreaksInFrames.Write(s, "showBreaksInFrames");
			}
		}

		public void Write_subFontBySize(TextWriter s)
		{
			if (this._subFontBySize != null)
			{
				this._subFontBySize.Write(s, "subFontBySize");
			}
		}

		public void Write_suppressBottomSpacing(TextWriter s)
		{
			if (this._suppressBottomSpacing != null)
			{
				this._suppressBottomSpacing.Write(s, "suppressBottomSpacing");
			}
		}

		public void Write_suppressTopSpacing(TextWriter s)
		{
			if (this._suppressTopSpacing != null)
			{
				this._suppressTopSpacing.Write(s, "suppressTopSpacing");
			}
		}

		public void Write_suppressSpacingAtTopOfPage(TextWriter s)
		{
			if (this._suppressSpacingAtTopOfPage != null)
			{
				this._suppressSpacingAtTopOfPage.Write(s, "suppressSpacingAtTopOfPage");
			}
		}

		public void Write_suppressTopSpacingWP(TextWriter s)
		{
			if (this._suppressTopSpacingWP != null)
			{
				this._suppressTopSpacingWP.Write(s, "suppressTopSpacingWP");
			}
		}

		public void Write_suppressSpBfAfterPgBrk(TextWriter s)
		{
			if (this._suppressSpBfAfterPgBrk != null)
			{
				this._suppressSpBfAfterPgBrk.Write(s, "suppressSpBfAfterPgBrk");
			}
		}

		public void Write_swapBordersFacingPages(TextWriter s)
		{
			if (this._swapBordersFacingPages != null)
			{
				this._swapBordersFacingPages.Write(s, "swapBordersFacingPages");
			}
		}

		public void Write_convMailMergeEsc(TextWriter s)
		{
			if (this._convMailMergeEsc != null)
			{
				this._convMailMergeEsc.Write(s, "convMailMergeEsc");
			}
		}

		public void Write_truncateFontHeightsLikeWP6(TextWriter s)
		{
			if (this._truncateFontHeightsLikeWP6 != null)
			{
				this._truncateFontHeightsLikeWP6.Write(s, "truncateFontHeightsLikeWP6");
			}
		}

		public void Write_mwSmallCaps(TextWriter s)
		{
			if (this._mwSmallCaps != null)
			{
				this._mwSmallCaps.Write(s, "mwSmallCaps");
			}
		}

		public void Write_usePrinterMetrics(TextWriter s)
		{
			if (this._usePrinterMetrics != null)
			{
				this._usePrinterMetrics.Write(s, "usePrinterMetrics");
			}
		}

		public void Write_doNotSuppressParagraphBorders(TextWriter s)
		{
			if (this._doNotSuppressParagraphBorders != null)
			{
				this._doNotSuppressParagraphBorders.Write(s, "doNotSuppressParagraphBorders");
			}
		}

		public void Write_wrapTrailSpaces(TextWriter s)
		{
			if (this._wrapTrailSpaces != null)
			{
				this._wrapTrailSpaces.Write(s, "wrapTrailSpaces");
			}
		}

		public void Write_footnoteLayoutLikeWW8(TextWriter s)
		{
			if (this._footnoteLayoutLikeWW8 != null)
			{
				this._footnoteLayoutLikeWW8.Write(s, "footnoteLayoutLikeWW8");
			}
		}

		public void Write_shapeLayoutLikeWW8(TextWriter s)
		{
			if (this._shapeLayoutLikeWW8 != null)
			{
				this._shapeLayoutLikeWW8.Write(s, "shapeLayoutLikeWW8");
			}
		}

		public void Write_alignTablesRowByRow(TextWriter s)
		{
			if (this._alignTablesRowByRow != null)
			{
				this._alignTablesRowByRow.Write(s, "alignTablesRowByRow");
			}
		}

		public void Write_forgetLastTabAlignment(TextWriter s)
		{
			if (this._forgetLastTabAlignment != null)
			{
				this._forgetLastTabAlignment.Write(s, "forgetLastTabAlignment");
			}
		}

		public void Write_adjustLineHeightInTable(TextWriter s)
		{
			if (this._adjustLineHeightInTable != null)
			{
				this._adjustLineHeightInTable.Write(s, "adjustLineHeightInTable");
			}
		}

		public void Write_autoSpaceLikeWord95(TextWriter s)
		{
			if (this._autoSpaceLikeWord95 != null)
			{
				this._autoSpaceLikeWord95.Write(s, "autoSpaceLikeWord95");
			}
		}

		public void Write_noSpaceRaiseLower(TextWriter s)
		{
			if (this._noSpaceRaiseLower != null)
			{
				this._noSpaceRaiseLower.Write(s, "noSpaceRaiseLower");
			}
		}

		public void Write_doNotUseHTMLParagraphAutoSpacing(TextWriter s)
		{
			if (this._doNotUseHTMLParagraphAutoSpacing != null)
			{
				this._doNotUseHTMLParagraphAutoSpacing.Write(s, "doNotUseHTMLParagraphAutoSpacing");
			}
		}

		public void Write_layoutRawTableWidth(TextWriter s)
		{
			if (this._layoutRawTableWidth != null)
			{
				this._layoutRawTableWidth.Write(s, "layoutRawTableWidth");
			}
		}

		public void Write_layoutTableRowsApart(TextWriter s)
		{
			if (this._layoutTableRowsApart != null)
			{
				this._layoutTableRowsApart.Write(s, "layoutTableRowsApart");
			}
		}

		public void Write_useWord97LineBreakRules(TextWriter s)
		{
			if (this._useWord97LineBreakRules != null)
			{
				this._useWord97LineBreakRules.Write(s, "useWord97LineBreakRules");
			}
		}

		public void Write_doNotBreakWrappedTables(TextWriter s)
		{
			if (this._doNotBreakWrappedTables != null)
			{
				this._doNotBreakWrappedTables.Write(s, "doNotBreakWrappedTables");
			}
		}

		public void Write_doNotSnapToGridInCell(TextWriter s)
		{
			if (this._doNotSnapToGridInCell != null)
			{
				this._doNotSnapToGridInCell.Write(s, "doNotSnapToGridInCell");
			}
		}

		public void Write_selectFldWithFirstOrLastChar(TextWriter s)
		{
			if (this._selectFldWithFirstOrLastChar != null)
			{
				this._selectFldWithFirstOrLastChar.Write(s, "selectFldWithFirstOrLastChar");
			}
		}

		public void Write_applyBreakingRules(TextWriter s)
		{
			if (this._applyBreakingRules != null)
			{
				this._applyBreakingRules.Write(s, "applyBreakingRules");
			}
		}

		public void Write_doNotWrapTextWithPunct(TextWriter s)
		{
			if (this._doNotWrapTextWithPunct != null)
			{
				this._doNotWrapTextWithPunct.Write(s, "doNotWrapTextWithPunct");
			}
		}

		public void Write_doNotUseEastAsianBreakRules(TextWriter s)
		{
			if (this._doNotUseEastAsianBreakRules != null)
			{
				this._doNotUseEastAsianBreakRules.Write(s, "doNotUseEastAsianBreakRules");
			}
		}

		public void Write_useWord2002TableStyleRules(TextWriter s)
		{
			if (this._useWord2002TableStyleRules != null)
			{
				this._useWord2002TableStyleRules.Write(s, "useWord2002TableStyleRules");
			}
		}

		public void Write_growAutofit(TextWriter s)
		{
			if (this._growAutofit != null)
			{
				this._growAutofit.Write(s, "growAutofit");
			}
		}

		public void Write_useFELayout(TextWriter s)
		{
			if (this._useFELayout != null)
			{
				this._useFELayout.Write(s, "useFELayout");
			}
		}

		public void Write_useNormalStyleForList(TextWriter s)
		{
			if (this._useNormalStyleForList != null)
			{
				this._useNormalStyleForList.Write(s, "useNormalStyleForList");
			}
		}

		public void Write_doNotUseIndentAsNumberingTabStop(TextWriter s)
		{
			if (this._doNotUseIndentAsNumberingTabStop != null)
			{
				this._doNotUseIndentAsNumberingTabStop.Write(s, "doNotUseIndentAsNumberingTabStop");
			}
		}

		public void Write_useAltKinsokuLineBreakRules(TextWriter s)
		{
			if (this._useAltKinsokuLineBreakRules != null)
			{
				this._useAltKinsokuLineBreakRules.Write(s, "useAltKinsokuLineBreakRules");
			}
		}

		public void Write_allowSpaceOfSameStyleInTable(TextWriter s)
		{
			if (this._allowSpaceOfSameStyleInTable != null)
			{
				this._allowSpaceOfSameStyleInTable.Write(s, "allowSpaceOfSameStyleInTable");
			}
		}

		public void Write_doNotSuppressIndentation(TextWriter s)
		{
			if (this._doNotSuppressIndentation != null)
			{
				this._doNotSuppressIndentation.Write(s, "doNotSuppressIndentation");
			}
		}

		public void Write_doNotAutofitConstrainedTables(TextWriter s)
		{
			if (this._doNotAutofitConstrainedTables != null)
			{
				this._doNotAutofitConstrainedTables.Write(s, "doNotAutofitConstrainedTables");
			}
		}

		public void Write_autofitToFirstFixedWidthCell(TextWriter s)
		{
			if (this._autofitToFirstFixedWidthCell != null)
			{
				this._autofitToFirstFixedWidthCell.Write(s, "autofitToFirstFixedWidthCell");
			}
		}

		public void Write_underlineTabInNumList(TextWriter s)
		{
			if (this._underlineTabInNumList != null)
			{
				this._underlineTabInNumList.Write(s, "underlineTabInNumList");
			}
		}

		public void Write_displayHangulFixedWidth(TextWriter s)
		{
			if (this._displayHangulFixedWidth != null)
			{
				this._displayHangulFixedWidth.Write(s, "displayHangulFixedWidth");
			}
		}

		public void Write_splitPgBreakAndParaMark(TextWriter s)
		{
			if (this._splitPgBreakAndParaMark != null)
			{
				this._splitPgBreakAndParaMark.Write(s, "splitPgBreakAndParaMark");
			}
		}

		public void Write_doNotVertAlignCellWithSp(TextWriter s)
		{
			if (this._doNotVertAlignCellWithSp != null)
			{
				this._doNotVertAlignCellWithSp.Write(s, "doNotVertAlignCellWithSp");
			}
		}

		public void Write_doNotBreakConstrainedForcedTable(TextWriter s)
		{
			if (this._doNotBreakConstrainedForcedTable != null)
			{
				this._doNotBreakConstrainedForcedTable.Write(s, "doNotBreakConstrainedForcedTable");
			}
		}

		public void Write_doNotVertAlignInTxbx(TextWriter s)
		{
			if (this._doNotVertAlignInTxbx != null)
			{
				this._doNotVertAlignInTxbx.Write(s, "doNotVertAlignInTxbx");
			}
		}

		public void Write_useAnsiKerningPairs(TextWriter s)
		{
			if (this._useAnsiKerningPairs != null)
			{
				this._useAnsiKerningPairs.Write(s, "useAnsiKerningPairs");
			}
		}

		public void Write_cachedColBalance(TextWriter s)
		{
			if (this._cachedColBalance != null)
			{
				this._cachedColBalance.Write(s, "cachedColBalance");
			}
		}
	}
}
