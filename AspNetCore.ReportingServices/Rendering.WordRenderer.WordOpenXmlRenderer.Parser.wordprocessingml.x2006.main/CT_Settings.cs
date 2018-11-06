using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Settings : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_OnOff _removePersonalInformation;

		private CT_OnOff _removeDateAndTime;

		private CT_OnOff _doNotDisplayPageBoundaries;

		private CT_OnOff _displayBackgroundShape;

		private CT_OnOff _printPostScriptOverText;

		private CT_OnOff _printFractionalCharacterWidth;

		private CT_OnOff _printFormsData;

		private CT_OnOff _embedTrueTypeFonts;

		private CT_OnOff _embedSystemFonts;

		private CT_OnOff _saveSubsetFonts;

		private CT_OnOff _saveFormsData;

		private CT_OnOff _mirrorMargins;

		private CT_OnOff _alignBordersAndEdges;

		private CT_OnOff _bordersDoNotSurroundHeader;

		private CT_OnOff _bordersDoNotSurroundFooter;

		private CT_OnOff _gutterAtTop;

		private CT_OnOff _hideSpellingErrors;

		private CT_OnOff _hideGrammaticalErrors;

		private CT_OnOff _formsDesign;

		private CT_Rel _attachedTemplate;

		private CT_OnOff _linkStyles;

		private CT_OnOff _trackRevisions;

		private CT_OnOff _doNotTrackMoves;

		private CT_OnOff _doNotTrackFormatting;

		private CT_OnOff _autoFormatOverride;

		private CT_OnOff _styleLockTheme;

		private CT_OnOff _styleLockQFSet;

		private CT_OnOff _autoHyphenation;

		private CT_DecimalNumber _consecutiveHyphenLimit;

		private CT_OnOff _doNotHyphenateCaps;

		private CT_OnOff _showEnvelope;

		private CT_String _clickAndTypeStyle;

		private CT_String _defaultTableStyle;

		private CT_OnOff _evenAndOddHeaders;

		private CT_OnOff _bookFoldRevPrinting;

		private CT_OnOff _bookFoldPrinting;

		private CT_DecimalNumber _bookFoldPrintingSheets;

		private CT_DecimalNumber _displayHorizontalDrawingGridEvery;

		private CT_DecimalNumber _displayVerticalDrawingGridEvery;

		private CT_OnOff _doNotUseMarginsForDrawingGridOrigin;

		private CT_OnOff _doNotShadeFormData;

		private CT_OnOff _noPunctuationKerning;

		private CT_OnOff _printTwoOnOne;

		private CT_OnOff _strictFirstAndLastChars;

		private CT_OnOff _savePreviewPicture;

		private CT_OnOff _doNotValidateAgainstSchema;

		private CT_OnOff _saveInvalidXml;

		private CT_OnOff _ignoreMixedContent;

		private CT_OnOff _alwaysShowPlaceholderText;

		private CT_OnOff _doNotDemarcateInvalidXml;

		private CT_OnOff _saveXmlDataOnly;

		private CT_OnOff _useXSLTWhenSaving;

		private CT_OnOff _showXMLTags;

		private CT_OnOff _alwaysMergeEmptyNamespace;

		private CT_OnOff _updateFields;

		private CT_Compat _compat;

		private CT_OnOff _doNotIncludeSubdocsInStats;

		private CT_OnOff _doNotAutoCompressPictures;

		private CT_OnOff _doNotEmbedSmartTags;

		private CT_String _decimalSymbol;

		private CT_String _listSeparator;

		private List<CT_String> _attachedSchema;

		public CT_OnOff RemovePersonalInformation
		{
			get
			{
				return this._removePersonalInformation;
			}
			set
			{
				this._removePersonalInformation = value;
			}
		}

		public CT_OnOff RemoveDateAndTime
		{
			get
			{
				return this._removeDateAndTime;
			}
			set
			{
				this._removeDateAndTime = value;
			}
		}

		public CT_OnOff DoNotDisplayPageBoundaries
		{
			get
			{
				return this._doNotDisplayPageBoundaries;
			}
			set
			{
				this._doNotDisplayPageBoundaries = value;
			}
		}

		public CT_OnOff DisplayBackgroundShape
		{
			get
			{
				return this._displayBackgroundShape;
			}
			set
			{
				this._displayBackgroundShape = value;
			}
		}

		public CT_OnOff PrintPostScriptOverText
		{
			get
			{
				return this._printPostScriptOverText;
			}
			set
			{
				this._printPostScriptOverText = value;
			}
		}

		public CT_OnOff PrintFractionalCharacterWidth
		{
			get
			{
				return this._printFractionalCharacterWidth;
			}
			set
			{
				this._printFractionalCharacterWidth = value;
			}
		}

		public CT_OnOff PrintFormsData
		{
			get
			{
				return this._printFormsData;
			}
			set
			{
				this._printFormsData = value;
			}
		}

		public CT_OnOff EmbedTrueTypeFonts
		{
			get
			{
				return this._embedTrueTypeFonts;
			}
			set
			{
				this._embedTrueTypeFonts = value;
			}
		}

		public CT_OnOff EmbedSystemFonts
		{
			get
			{
				return this._embedSystemFonts;
			}
			set
			{
				this._embedSystemFonts = value;
			}
		}

		public CT_OnOff SaveSubsetFonts
		{
			get
			{
				return this._saveSubsetFonts;
			}
			set
			{
				this._saveSubsetFonts = value;
			}
		}

		public CT_OnOff SaveFormsData
		{
			get
			{
				return this._saveFormsData;
			}
			set
			{
				this._saveFormsData = value;
			}
		}

		public CT_OnOff MirrorMargins
		{
			get
			{
				return this._mirrorMargins;
			}
			set
			{
				this._mirrorMargins = value;
			}
		}

		public CT_OnOff AlignBordersAndEdges
		{
			get
			{
				return this._alignBordersAndEdges;
			}
			set
			{
				this._alignBordersAndEdges = value;
			}
		}

		public CT_OnOff BordersDoNotSurroundHeader
		{
			get
			{
				return this._bordersDoNotSurroundHeader;
			}
			set
			{
				this._bordersDoNotSurroundHeader = value;
			}
		}

		public CT_OnOff BordersDoNotSurroundFooter
		{
			get
			{
				return this._bordersDoNotSurroundFooter;
			}
			set
			{
				this._bordersDoNotSurroundFooter = value;
			}
		}

		public CT_OnOff GutterAtTop
		{
			get
			{
				return this._gutterAtTop;
			}
			set
			{
				this._gutterAtTop = value;
			}
		}

		public CT_OnOff HideSpellingErrors
		{
			get
			{
				return this._hideSpellingErrors;
			}
			set
			{
				this._hideSpellingErrors = value;
			}
		}

		public CT_OnOff HideGrammaticalErrors
		{
			get
			{
				return this._hideGrammaticalErrors;
			}
			set
			{
				this._hideGrammaticalErrors = value;
			}
		}

		public CT_OnOff FormsDesign
		{
			get
			{
				return this._formsDesign;
			}
			set
			{
				this._formsDesign = value;
			}
		}

		public CT_Rel AttachedTemplate
		{
			get
			{
				return this._attachedTemplate;
			}
			set
			{
				this._attachedTemplate = value;
			}
		}

		public CT_OnOff LinkStyles
		{
			get
			{
				return this._linkStyles;
			}
			set
			{
				this._linkStyles = value;
			}
		}

		public CT_OnOff TrackRevisions
		{
			get
			{
				return this._trackRevisions;
			}
			set
			{
				this._trackRevisions = value;
			}
		}

		public CT_OnOff DoNotTrackMoves
		{
			get
			{
				return this._doNotTrackMoves;
			}
			set
			{
				this._doNotTrackMoves = value;
			}
		}

		public CT_OnOff DoNotTrackFormatting
		{
			get
			{
				return this._doNotTrackFormatting;
			}
			set
			{
				this._doNotTrackFormatting = value;
			}
		}

		public CT_OnOff AutoFormatOverride
		{
			get
			{
				return this._autoFormatOverride;
			}
			set
			{
				this._autoFormatOverride = value;
			}
		}

		public CT_OnOff StyleLockTheme
		{
			get
			{
				return this._styleLockTheme;
			}
			set
			{
				this._styleLockTheme = value;
			}
		}

		public CT_OnOff StyleLockQFSet
		{
			get
			{
				return this._styleLockQFSet;
			}
			set
			{
				this._styleLockQFSet = value;
			}
		}

		public CT_OnOff AutoHyphenation
		{
			get
			{
				return this._autoHyphenation;
			}
			set
			{
				this._autoHyphenation = value;
			}
		}

		public CT_DecimalNumber ConsecutiveHyphenLimit
		{
			get
			{
				return this._consecutiveHyphenLimit;
			}
			set
			{
				this._consecutiveHyphenLimit = value;
			}
		}

		public CT_OnOff DoNotHyphenateCaps
		{
			get
			{
				return this._doNotHyphenateCaps;
			}
			set
			{
				this._doNotHyphenateCaps = value;
			}
		}

		public CT_OnOff ShowEnvelope
		{
			get
			{
				return this._showEnvelope;
			}
			set
			{
				this._showEnvelope = value;
			}
		}

		public CT_String ClickAndTypeStyle
		{
			get
			{
				return this._clickAndTypeStyle;
			}
			set
			{
				this._clickAndTypeStyle = value;
			}
		}

		public CT_String DefaultTableStyle
		{
			get
			{
				return this._defaultTableStyle;
			}
			set
			{
				this._defaultTableStyle = value;
			}
		}

		public CT_OnOff EvenAndOddHeaders
		{
			get
			{
				return this._evenAndOddHeaders;
			}
			set
			{
				this._evenAndOddHeaders = value;
			}
		}

		public CT_OnOff BookFoldRevPrinting
		{
			get
			{
				return this._bookFoldRevPrinting;
			}
			set
			{
				this._bookFoldRevPrinting = value;
			}
		}

		public CT_OnOff BookFoldPrinting
		{
			get
			{
				return this._bookFoldPrinting;
			}
			set
			{
				this._bookFoldPrinting = value;
			}
		}

		public CT_DecimalNumber BookFoldPrintingSheets
		{
			get
			{
				return this._bookFoldPrintingSheets;
			}
			set
			{
				this._bookFoldPrintingSheets = value;
			}
		}

		public CT_DecimalNumber DisplayHorizontalDrawingGridEvery
		{
			get
			{
				return this._displayHorizontalDrawingGridEvery;
			}
			set
			{
				this._displayHorizontalDrawingGridEvery = value;
			}
		}

		public CT_DecimalNumber DisplayVerticalDrawingGridEvery
		{
			get
			{
				return this._displayVerticalDrawingGridEvery;
			}
			set
			{
				this._displayVerticalDrawingGridEvery = value;
			}
		}

		public CT_OnOff DoNotUseMarginsForDrawingGridOrigin
		{
			get
			{
				return this._doNotUseMarginsForDrawingGridOrigin;
			}
			set
			{
				this._doNotUseMarginsForDrawingGridOrigin = value;
			}
		}

		public CT_OnOff DoNotShadeFormData
		{
			get
			{
				return this._doNotShadeFormData;
			}
			set
			{
				this._doNotShadeFormData = value;
			}
		}

		public CT_OnOff NoPunctuationKerning
		{
			get
			{
				return this._noPunctuationKerning;
			}
			set
			{
				this._noPunctuationKerning = value;
			}
		}

		public CT_OnOff PrintTwoOnOne
		{
			get
			{
				return this._printTwoOnOne;
			}
			set
			{
				this._printTwoOnOne = value;
			}
		}

		public CT_OnOff StrictFirstAndLastChars
		{
			get
			{
				return this._strictFirstAndLastChars;
			}
			set
			{
				this._strictFirstAndLastChars = value;
			}
		}

		public CT_OnOff SavePreviewPicture
		{
			get
			{
				return this._savePreviewPicture;
			}
			set
			{
				this._savePreviewPicture = value;
			}
		}

		public CT_OnOff DoNotValidateAgainstSchema
		{
			get
			{
				return this._doNotValidateAgainstSchema;
			}
			set
			{
				this._doNotValidateAgainstSchema = value;
			}
		}

		public CT_OnOff SaveInvalidXml
		{
			get
			{
				return this._saveInvalidXml;
			}
			set
			{
				this._saveInvalidXml = value;
			}
		}

		public CT_OnOff IgnoreMixedContent
		{
			get
			{
				return this._ignoreMixedContent;
			}
			set
			{
				this._ignoreMixedContent = value;
			}
		}

		public CT_OnOff AlwaysShowPlaceholderText
		{
			get
			{
				return this._alwaysShowPlaceholderText;
			}
			set
			{
				this._alwaysShowPlaceholderText = value;
			}
		}

		public CT_OnOff DoNotDemarcateInvalidXml
		{
			get
			{
				return this._doNotDemarcateInvalidXml;
			}
			set
			{
				this._doNotDemarcateInvalidXml = value;
			}
		}

		public CT_OnOff SaveXmlDataOnly
		{
			get
			{
				return this._saveXmlDataOnly;
			}
			set
			{
				this._saveXmlDataOnly = value;
			}
		}

		public CT_OnOff UseXSLTWhenSaving
		{
			get
			{
				return this._useXSLTWhenSaving;
			}
			set
			{
				this._useXSLTWhenSaving = value;
			}
		}

		public CT_OnOff ShowXMLTags
		{
			get
			{
				return this._showXMLTags;
			}
			set
			{
				this._showXMLTags = value;
			}
		}

		public CT_OnOff AlwaysMergeEmptyNamespace
		{
			get
			{
				return this._alwaysMergeEmptyNamespace;
			}
			set
			{
				this._alwaysMergeEmptyNamespace = value;
			}
		}

		public CT_OnOff UpdateFields
		{
			get
			{
				return this._updateFields;
			}
			set
			{
				this._updateFields = value;
			}
		}

		public CT_Compat Compat
		{
			get
			{
				return this._compat;
			}
			set
			{
				this._compat = value;
			}
		}

		public CT_OnOff DoNotIncludeSubdocsInStats
		{
			get
			{
				return this._doNotIncludeSubdocsInStats;
			}
			set
			{
				this._doNotIncludeSubdocsInStats = value;
			}
		}

		public CT_OnOff DoNotAutoCompressPictures
		{
			get
			{
				return this._doNotAutoCompressPictures;
			}
			set
			{
				this._doNotAutoCompressPictures = value;
			}
		}

		public CT_OnOff DoNotEmbedSmartTags
		{
			get
			{
				return this._doNotEmbedSmartTags;
			}
			set
			{
				this._doNotEmbedSmartTags = value;
			}
		}

		public CT_String DecimalSymbol
		{
			get
			{
				return this._decimalSymbol;
			}
			set
			{
				this._decimalSymbol = value;
			}
		}

		public CT_String ListSeparator
		{
			get
			{
				return this._listSeparator;
			}
			set
			{
				this._listSeparator = value;
			}
		}

		public List<CT_String> AttachedSchema
		{
			get
			{
				return this._attachedSchema;
			}
			set
			{
				this._attachedSchema = value;
			}
		}

		public static string RemovePersonalInformationElementName
		{
			get
			{
				return "removePersonalInformation";
			}
		}

		public static string RemoveDateAndTimeElementName
		{
			get
			{
				return "removeDateAndTime";
			}
		}

		public static string DoNotDisplayPageBoundariesElementName
		{
			get
			{
				return "doNotDisplayPageBoundaries";
			}
		}

		public static string DisplayBackgroundShapeElementName
		{
			get
			{
				return "displayBackgroundShape";
			}
		}

		public static string PrintPostScriptOverTextElementName
		{
			get
			{
				return "printPostScriptOverText";
			}
		}

		public static string PrintFractionalCharacterWidthElementName
		{
			get
			{
				return "printFractionalCharacterWidth";
			}
		}

		public static string PrintFormsDataElementName
		{
			get
			{
				return "printFormsData";
			}
		}

		public static string EmbedTrueTypeFontsElementName
		{
			get
			{
				return "embedTrueTypeFonts";
			}
		}

		public static string EmbedSystemFontsElementName
		{
			get
			{
				return "embedSystemFonts";
			}
		}

		public static string SaveSubsetFontsElementName
		{
			get
			{
				return "saveSubsetFonts";
			}
		}

		public static string SaveFormsDataElementName
		{
			get
			{
				return "saveFormsData";
			}
		}

		public static string MirrorMarginsElementName
		{
			get
			{
				return "mirrorMargins";
			}
		}

		public static string AlignBordersAndEdgesElementName
		{
			get
			{
				return "alignBordersAndEdges";
			}
		}

		public static string BordersDoNotSurroundHeaderElementName
		{
			get
			{
				return "bordersDoNotSurroundHeader";
			}
		}

		public static string BordersDoNotSurroundFooterElementName
		{
			get
			{
				return "bordersDoNotSurroundFooter";
			}
		}

		public static string GutterAtTopElementName
		{
			get
			{
				return "gutterAtTop";
			}
		}

		public static string HideSpellingErrorsElementName
		{
			get
			{
				return "hideSpellingErrors";
			}
		}

		public static string HideGrammaticalErrorsElementName
		{
			get
			{
				return "hideGrammaticalErrors";
			}
		}

		public static string FormsDesignElementName
		{
			get
			{
				return "formsDesign";
			}
		}

		public static string AttachedTemplateElementName
		{
			get
			{
				return "attachedTemplate";
			}
		}

		public static string LinkStylesElementName
		{
			get
			{
				return "linkStyles";
			}
		}

		public static string TrackRevisionsElementName
		{
			get
			{
				return "trackRevisions";
			}
		}

		public static string DoNotTrackMovesElementName
		{
			get
			{
				return "doNotTrackMoves";
			}
		}

		public static string DoNotTrackFormattingElementName
		{
			get
			{
				return "doNotTrackFormatting";
			}
		}

		public static string AutoFormatOverrideElementName
		{
			get
			{
				return "autoFormatOverride";
			}
		}

		public static string StyleLockThemeElementName
		{
			get
			{
				return "styleLockTheme";
			}
		}

		public static string StyleLockQFSetElementName
		{
			get
			{
				return "styleLockQFSet";
			}
		}

		public static string AutoHyphenationElementName
		{
			get
			{
				return "autoHyphenation";
			}
		}

		public static string ConsecutiveHyphenLimitElementName
		{
			get
			{
				return "consecutiveHyphenLimit";
			}
		}

		public static string DoNotHyphenateCapsElementName
		{
			get
			{
				return "doNotHyphenateCaps";
			}
		}

		public static string ShowEnvelopeElementName
		{
			get
			{
				return "showEnvelope";
			}
		}

		public static string ClickAndTypeStyleElementName
		{
			get
			{
				return "clickAndTypeStyle";
			}
		}

		public static string DefaultTableStyleElementName
		{
			get
			{
				return "defaultTableStyle";
			}
		}

		public static string EvenAndOddHeadersElementName
		{
			get
			{
				return "evenAndOddHeaders";
			}
		}

		public static string BookFoldRevPrintingElementName
		{
			get
			{
				return "bookFoldRevPrinting";
			}
		}

		public static string BookFoldPrintingElementName
		{
			get
			{
				return "bookFoldPrinting";
			}
		}

		public static string BookFoldPrintingSheetsElementName
		{
			get
			{
				return "bookFoldPrintingSheets";
			}
		}

		public static string DisplayHorizontalDrawingGridEveryElementName
		{
			get
			{
				return "displayHorizontalDrawingGridEvery";
			}
		}

		public static string DisplayVerticalDrawingGridEveryElementName
		{
			get
			{
				return "displayVerticalDrawingGridEvery";
			}
		}

		public static string DoNotUseMarginsForDrawingGridOriginElementName
		{
			get
			{
				return "doNotUseMarginsForDrawingGridOrigin";
			}
		}

		public static string DoNotShadeFormDataElementName
		{
			get
			{
				return "doNotShadeFormData";
			}
		}

		public static string NoPunctuationKerningElementName
		{
			get
			{
				return "noPunctuationKerning";
			}
		}

		public static string PrintTwoOnOneElementName
		{
			get
			{
				return "printTwoOnOne";
			}
		}

		public static string StrictFirstAndLastCharsElementName
		{
			get
			{
				return "strictFirstAndLastChars";
			}
		}

		public static string SavePreviewPictureElementName
		{
			get
			{
				return "savePreviewPicture";
			}
		}

		public static string DoNotValidateAgainstSchemaElementName
		{
			get
			{
				return "doNotValidateAgainstSchema";
			}
		}

		public static string SaveInvalidXmlElementName
		{
			get
			{
				return "saveInvalidXml";
			}
		}

		public static string IgnoreMixedContentElementName
		{
			get
			{
				return "ignoreMixedContent";
			}
		}

		public static string AlwaysShowPlaceholderTextElementName
		{
			get
			{
				return "alwaysShowPlaceholderText";
			}
		}

		public static string DoNotDemarcateInvalidXmlElementName
		{
			get
			{
				return "doNotDemarcateInvalidXml";
			}
		}

		public static string SaveXmlDataOnlyElementName
		{
			get
			{
				return "saveXmlDataOnly";
			}
		}

		public static string UseXSLTWhenSavingElementName
		{
			get
			{
				return "useXSLTWhenSaving";
			}
		}

		public static string ShowXMLTagsElementName
		{
			get
			{
				return "showXMLTags";
			}
		}

		public static string AlwaysMergeEmptyNamespaceElementName
		{
			get
			{
				return "alwaysMergeEmptyNamespace";
			}
		}

		public static string UpdateFieldsElementName
		{
			get
			{
				return "updateFields";
			}
		}

		public static string CompatElementName
		{
			get
			{
				return "compat";
			}
		}

		public static string DoNotIncludeSubdocsInStatsElementName
		{
			get
			{
				return "doNotIncludeSubdocsInStats";
			}
		}

		public static string DoNotAutoCompressPicturesElementName
		{
			get
			{
				return "doNotAutoCompressPictures";
			}
		}

		public static string DoNotEmbedSmartTagsElementName
		{
			get
			{
				return "doNotEmbedSmartTags";
			}
		}

		public static string DecimalSymbolElementName
		{
			get
			{
				return "decimalSymbol";
			}
		}

		public static string ListSeparatorElementName
		{
			get
			{
				return "listSeparator";
			}
		}

		public static string AttachedSchemaElementName
		{
			get
			{
				return "attachedSchema";
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
			this._attachedSchema = new List<CT_String>();
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
			this.Write_removePersonalInformation(s);
			this.Write_removeDateAndTime(s);
			this.Write_doNotDisplayPageBoundaries(s);
			this.Write_displayBackgroundShape(s);
			this.Write_printPostScriptOverText(s);
			this.Write_printFractionalCharacterWidth(s);
			this.Write_printFormsData(s);
			this.Write_embedTrueTypeFonts(s);
			this.Write_embedSystemFonts(s);
			this.Write_saveSubsetFonts(s);
			this.Write_saveFormsData(s);
			this.Write_mirrorMargins(s);
			this.Write_alignBordersAndEdges(s);
			this.Write_bordersDoNotSurroundHeader(s);
			this.Write_bordersDoNotSurroundFooter(s);
			this.Write_gutterAtTop(s);
			this.Write_hideSpellingErrors(s);
			this.Write_hideGrammaticalErrors(s);
			this.Write_formsDesign(s);
			this.Write_attachedTemplate(s);
			this.Write_linkStyles(s);
			this.Write_trackRevisions(s);
			this.Write_doNotTrackMoves(s);
			this.Write_doNotTrackFormatting(s);
			this.Write_autoFormatOverride(s);
			this.Write_styleLockTheme(s);
			this.Write_styleLockQFSet(s);
			this.Write_autoHyphenation(s);
			this.Write_consecutiveHyphenLimit(s);
			this.Write_doNotHyphenateCaps(s);
			this.Write_showEnvelope(s);
			this.Write_clickAndTypeStyle(s);
			this.Write_defaultTableStyle(s);
			this.Write_evenAndOddHeaders(s);
			this.Write_bookFoldRevPrinting(s);
			this.Write_bookFoldPrinting(s);
			this.Write_bookFoldPrintingSheets(s);
			this.Write_displayHorizontalDrawingGridEvery(s);
			this.Write_displayVerticalDrawingGridEvery(s);
			this.Write_doNotUseMarginsForDrawingGridOrigin(s);
			this.Write_doNotShadeFormData(s);
			this.Write_noPunctuationKerning(s);
			this.Write_printTwoOnOne(s);
			this.Write_strictFirstAndLastChars(s);
			this.Write_savePreviewPicture(s);
			this.Write_doNotValidateAgainstSchema(s);
			this.Write_saveInvalidXml(s);
			this.Write_ignoreMixedContent(s);
			this.Write_alwaysShowPlaceholderText(s);
			this.Write_doNotDemarcateInvalidXml(s);
			this.Write_saveXmlDataOnly(s);
			this.Write_useXSLTWhenSaving(s);
			this.Write_showXMLTags(s);
			this.Write_alwaysMergeEmptyNamespace(s);
			this.Write_updateFields(s);
			this.Write_compat(s);
			this.Write_attachedSchema(s);
			this.Write_doNotIncludeSubdocsInStats(s);
			this.Write_doNotAutoCompressPictures(s);
			this.Write_doNotEmbedSmartTags(s);
			this.Write_decimalSymbol(s);
			this.Write_listSeparator(s);
		}

		public void Write_removePersonalInformation(TextWriter s)
		{
			if (this._removePersonalInformation != null)
			{
				this._removePersonalInformation.Write(s, "removePersonalInformation");
			}
		}

		public void Write_removeDateAndTime(TextWriter s)
		{
			if (this._removeDateAndTime != null)
			{
				this._removeDateAndTime.Write(s, "removeDateAndTime");
			}
		}

		public void Write_doNotDisplayPageBoundaries(TextWriter s)
		{
			if (this._doNotDisplayPageBoundaries != null)
			{
				this._doNotDisplayPageBoundaries.Write(s, "doNotDisplayPageBoundaries");
			}
		}

		public void Write_displayBackgroundShape(TextWriter s)
		{
			if (this._displayBackgroundShape != null)
			{
				this._displayBackgroundShape.Write(s, "displayBackgroundShape");
			}
		}

		public void Write_printPostScriptOverText(TextWriter s)
		{
			if (this._printPostScriptOverText != null)
			{
				this._printPostScriptOverText.Write(s, "printPostScriptOverText");
			}
		}

		public void Write_printFractionalCharacterWidth(TextWriter s)
		{
			if (this._printFractionalCharacterWidth != null)
			{
				this._printFractionalCharacterWidth.Write(s, "printFractionalCharacterWidth");
			}
		}

		public void Write_printFormsData(TextWriter s)
		{
			if (this._printFormsData != null)
			{
				this._printFormsData.Write(s, "printFormsData");
			}
		}

		public void Write_embedTrueTypeFonts(TextWriter s)
		{
			if (this._embedTrueTypeFonts != null)
			{
				this._embedTrueTypeFonts.Write(s, "embedTrueTypeFonts");
			}
		}

		public void Write_embedSystemFonts(TextWriter s)
		{
			if (this._embedSystemFonts != null)
			{
				this._embedSystemFonts.Write(s, "embedSystemFonts");
			}
		}

		public void Write_saveSubsetFonts(TextWriter s)
		{
			if (this._saveSubsetFonts != null)
			{
				this._saveSubsetFonts.Write(s, "saveSubsetFonts");
			}
		}

		public void Write_saveFormsData(TextWriter s)
		{
			if (this._saveFormsData != null)
			{
				this._saveFormsData.Write(s, "saveFormsData");
			}
		}

		public void Write_mirrorMargins(TextWriter s)
		{
			if (this._mirrorMargins != null)
			{
				this._mirrorMargins.Write(s, "mirrorMargins");
			}
		}

		public void Write_alignBordersAndEdges(TextWriter s)
		{
			if (this._alignBordersAndEdges != null)
			{
				this._alignBordersAndEdges.Write(s, "alignBordersAndEdges");
			}
		}

		public void Write_bordersDoNotSurroundHeader(TextWriter s)
		{
			if (this._bordersDoNotSurroundHeader != null)
			{
				this._bordersDoNotSurroundHeader.Write(s, "bordersDoNotSurroundHeader");
			}
		}

		public void Write_bordersDoNotSurroundFooter(TextWriter s)
		{
			if (this._bordersDoNotSurroundFooter != null)
			{
				this._bordersDoNotSurroundFooter.Write(s, "bordersDoNotSurroundFooter");
			}
		}

		public void Write_gutterAtTop(TextWriter s)
		{
			if (this._gutterAtTop != null)
			{
				this._gutterAtTop.Write(s, "gutterAtTop");
			}
		}

		public void Write_hideSpellingErrors(TextWriter s)
		{
			if (this._hideSpellingErrors != null)
			{
				this._hideSpellingErrors.Write(s, "hideSpellingErrors");
			}
		}

		public void Write_hideGrammaticalErrors(TextWriter s)
		{
			if (this._hideGrammaticalErrors != null)
			{
				this._hideGrammaticalErrors.Write(s, "hideGrammaticalErrors");
			}
		}

		public void Write_formsDesign(TextWriter s)
		{
			if (this._formsDesign != null)
			{
				this._formsDesign.Write(s, "formsDesign");
			}
		}

		public void Write_attachedTemplate(TextWriter s)
		{
			if (this._attachedTemplate != null)
			{
				this._attachedTemplate.Write(s, "attachedTemplate");
			}
		}

		public void Write_linkStyles(TextWriter s)
		{
			if (this._linkStyles != null)
			{
				this._linkStyles.Write(s, "linkStyles");
			}
		}

		public void Write_trackRevisions(TextWriter s)
		{
			if (this._trackRevisions != null)
			{
				this._trackRevisions.Write(s, "trackRevisions");
			}
		}

		public void Write_doNotTrackMoves(TextWriter s)
		{
			if (this._doNotTrackMoves != null)
			{
				this._doNotTrackMoves.Write(s, "doNotTrackMoves");
			}
		}

		public void Write_doNotTrackFormatting(TextWriter s)
		{
			if (this._doNotTrackFormatting != null)
			{
				this._doNotTrackFormatting.Write(s, "doNotTrackFormatting");
			}
		}

		public void Write_autoFormatOverride(TextWriter s)
		{
			if (this._autoFormatOverride != null)
			{
				this._autoFormatOverride.Write(s, "autoFormatOverride");
			}
		}

		public void Write_styleLockTheme(TextWriter s)
		{
			if (this._styleLockTheme != null)
			{
				this._styleLockTheme.Write(s, "styleLockTheme");
			}
		}

		public void Write_styleLockQFSet(TextWriter s)
		{
			if (this._styleLockQFSet != null)
			{
				this._styleLockQFSet.Write(s, "styleLockQFSet");
			}
		}

		public void Write_autoHyphenation(TextWriter s)
		{
			if (this._autoHyphenation != null)
			{
				this._autoHyphenation.Write(s, "autoHyphenation");
			}
		}

		public void Write_consecutiveHyphenLimit(TextWriter s)
		{
			if (this._consecutiveHyphenLimit != null)
			{
				this._consecutiveHyphenLimit.Write(s, "consecutiveHyphenLimit");
			}
		}

		public void Write_doNotHyphenateCaps(TextWriter s)
		{
			if (this._doNotHyphenateCaps != null)
			{
				this._doNotHyphenateCaps.Write(s, "doNotHyphenateCaps");
			}
		}

		public void Write_showEnvelope(TextWriter s)
		{
			if (this._showEnvelope != null)
			{
				this._showEnvelope.Write(s, "showEnvelope");
			}
		}

		public void Write_clickAndTypeStyle(TextWriter s)
		{
			if (this._clickAndTypeStyle != null)
			{
				this._clickAndTypeStyle.Write(s, "clickAndTypeStyle");
			}
		}

		public void Write_defaultTableStyle(TextWriter s)
		{
			if (this._defaultTableStyle != null)
			{
				this._defaultTableStyle.Write(s, "defaultTableStyle");
			}
		}

		public void Write_evenAndOddHeaders(TextWriter s)
		{
			if (this._evenAndOddHeaders != null)
			{
				this._evenAndOddHeaders.Write(s, "evenAndOddHeaders");
			}
		}

		public void Write_bookFoldRevPrinting(TextWriter s)
		{
			if (this._bookFoldRevPrinting != null)
			{
				this._bookFoldRevPrinting.Write(s, "bookFoldRevPrinting");
			}
		}

		public void Write_bookFoldPrinting(TextWriter s)
		{
			if (this._bookFoldPrinting != null)
			{
				this._bookFoldPrinting.Write(s, "bookFoldPrinting");
			}
		}

		public void Write_bookFoldPrintingSheets(TextWriter s)
		{
			if (this._bookFoldPrintingSheets != null)
			{
				this._bookFoldPrintingSheets.Write(s, "bookFoldPrintingSheets");
			}
		}

		public void Write_displayHorizontalDrawingGridEvery(TextWriter s)
		{
			if (this._displayHorizontalDrawingGridEvery != null)
			{
				this._displayHorizontalDrawingGridEvery.Write(s, "displayHorizontalDrawingGridEvery");
			}
		}

		public void Write_displayVerticalDrawingGridEvery(TextWriter s)
		{
			if (this._displayVerticalDrawingGridEvery != null)
			{
				this._displayVerticalDrawingGridEvery.Write(s, "displayVerticalDrawingGridEvery");
			}
		}

		public void Write_doNotUseMarginsForDrawingGridOrigin(TextWriter s)
		{
			if (this._doNotUseMarginsForDrawingGridOrigin != null)
			{
				this._doNotUseMarginsForDrawingGridOrigin.Write(s, "doNotUseMarginsForDrawingGridOrigin");
			}
		}

		public void Write_doNotShadeFormData(TextWriter s)
		{
			if (this._doNotShadeFormData != null)
			{
				this._doNotShadeFormData.Write(s, "doNotShadeFormData");
			}
		}

		public void Write_noPunctuationKerning(TextWriter s)
		{
			if (this._noPunctuationKerning != null)
			{
				this._noPunctuationKerning.Write(s, "noPunctuationKerning");
			}
		}

		public void Write_printTwoOnOne(TextWriter s)
		{
			if (this._printTwoOnOne != null)
			{
				this._printTwoOnOne.Write(s, "printTwoOnOne");
			}
		}

		public void Write_strictFirstAndLastChars(TextWriter s)
		{
			if (this._strictFirstAndLastChars != null)
			{
				this._strictFirstAndLastChars.Write(s, "strictFirstAndLastChars");
			}
		}

		public void Write_savePreviewPicture(TextWriter s)
		{
			if (this._savePreviewPicture != null)
			{
				this._savePreviewPicture.Write(s, "savePreviewPicture");
			}
		}

		public void Write_doNotValidateAgainstSchema(TextWriter s)
		{
			if (this._doNotValidateAgainstSchema != null)
			{
				this._doNotValidateAgainstSchema.Write(s, "doNotValidateAgainstSchema");
			}
		}

		public void Write_saveInvalidXml(TextWriter s)
		{
			if (this._saveInvalidXml != null)
			{
				this._saveInvalidXml.Write(s, "saveInvalidXml");
			}
		}

		public void Write_ignoreMixedContent(TextWriter s)
		{
			if (this._ignoreMixedContent != null)
			{
				this._ignoreMixedContent.Write(s, "ignoreMixedContent");
			}
		}

		public void Write_alwaysShowPlaceholderText(TextWriter s)
		{
			if (this._alwaysShowPlaceholderText != null)
			{
				this._alwaysShowPlaceholderText.Write(s, "alwaysShowPlaceholderText");
			}
		}

		public void Write_doNotDemarcateInvalidXml(TextWriter s)
		{
			if (this._doNotDemarcateInvalidXml != null)
			{
				this._doNotDemarcateInvalidXml.Write(s, "doNotDemarcateInvalidXml");
			}
		}

		public void Write_saveXmlDataOnly(TextWriter s)
		{
			if (this._saveXmlDataOnly != null)
			{
				this._saveXmlDataOnly.Write(s, "saveXmlDataOnly");
			}
		}

		public void Write_useXSLTWhenSaving(TextWriter s)
		{
			if (this._useXSLTWhenSaving != null)
			{
				this._useXSLTWhenSaving.Write(s, "useXSLTWhenSaving");
			}
		}

		public void Write_showXMLTags(TextWriter s)
		{
			if (this._showXMLTags != null)
			{
				this._showXMLTags.Write(s, "showXMLTags");
			}
		}

		public void Write_alwaysMergeEmptyNamespace(TextWriter s)
		{
			if (this._alwaysMergeEmptyNamespace != null)
			{
				this._alwaysMergeEmptyNamespace.Write(s, "alwaysMergeEmptyNamespace");
			}
		}

		public void Write_updateFields(TextWriter s)
		{
			if (this._updateFields != null)
			{
				this._updateFields.Write(s, "updateFields");
			}
		}

		public void Write_compat(TextWriter s)
		{
			if (this._compat != null)
			{
				this._compat.Write(s, "compat");
			}
		}

		public void Write_doNotIncludeSubdocsInStats(TextWriter s)
		{
			if (this._doNotIncludeSubdocsInStats != null)
			{
				this._doNotIncludeSubdocsInStats.Write(s, "doNotIncludeSubdocsInStats");
			}
		}

		public void Write_doNotAutoCompressPictures(TextWriter s)
		{
			if (this._doNotAutoCompressPictures != null)
			{
				this._doNotAutoCompressPictures.Write(s, "doNotAutoCompressPictures");
			}
		}

		public void Write_doNotEmbedSmartTags(TextWriter s)
		{
			if (this._doNotEmbedSmartTags != null)
			{
				this._doNotEmbedSmartTags.Write(s, "doNotEmbedSmartTags");
			}
		}

		public void Write_decimalSymbol(TextWriter s)
		{
			if (this._decimalSymbol != null)
			{
				this._decimalSymbol.Write(s, "decimalSymbol");
			}
		}

		public void Write_listSeparator(TextWriter s)
		{
			if (this._listSeparator != null)
			{
				this._listSeparator.Write(s, "listSeparator");
			}
		}

		public void Write_attachedSchema(TextWriter s)
		{
			if (this._attachedSchema != null)
			{
				foreach (CT_String item in this._attachedSchema)
				{
					if (item != null)
					{
						item.Write(s, "attachedSchema");
					}
				}
			}
		}
	}
}
