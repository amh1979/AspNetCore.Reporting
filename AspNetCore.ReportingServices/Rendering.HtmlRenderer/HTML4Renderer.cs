using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Web;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal abstract class HTML4Renderer : IHtmlReportWriter, IHtmlWriter, IHtmlRenderer
	{
		internal enum RequestType
		{
			Render,
			Search,
			Bookmark
		}

		internal enum Border
		{
			All,
			Left,
			Top,
			Right,
			Bottom
		}

		internal enum BorderAttribute
		{
			BorderWidth,
			BorderStyle,
			BorderColor
		}

		internal enum Direction
		{
			Row,
			Column
		}

		internal enum PageSection
		{
			Body,
			PageHeader,
			PageFooter
		}

		internal enum FontAttributes
		{
			None,
			Partial,
			All
		}

		private const float MaxWordSize = 558.8f;

		private const string FixedRowMarker = "r";

		private const string FixedColMarker = "c";

		private const string EmptyColMarker = "e";

		private const string EmptyHeightColMarker = "h";

		internal const string FixedRowGroupHeaderPrefix = "frgh";

		internal const string FixedCornerHeaderPrefix = "fch";

		internal const string FixedColGroupHeaderPrefix = "fcgh";

		internal const string FixedRGHArrayPrefix = "frhArr";

		internal const string FixedCGHArrayPrefix = "fcghArr";

		internal const string FixedCHArrayPrefix = "fchArr";

		internal const string ReportDiv = "oReportDiv";

		private const char Space = ' ';

		private const char Comma = ',';

		private const string MSuffix = "_m";

		private const string SSuffix = "_s";

		private const string ASuffix = "_a";

		private const string PSuffix = "_p";

		private const string FitVertTextSuffix = "_fvt";

		private const string GrowRectanglesSuffix = "_gr";

		private const string ImageConImageSuffix = "_ici";

		private const string ImageFitDivSuffix = "_ifd";

		private const long FitProptionalDefaultSize = 5L;

		protected const int SecondaryStreamBufferSize = 4096;

		internal const string SortAction = "Sort";

		internal const string ToggleAction = "Toggle";

		internal const string DrillthroughAction = "Drillthrough";

		internal const string BookmarkAction = "Bookmark";

		internal const string GetImageKey = "GetImage";

		internal const string SectionKey = "Section";

		internal const string PrefixIdKey = "PrefixId";

		internal const int IgnoreLeft = 1;

		internal const int IgnoreRight = 2;

		internal const int IgnoreTop = 4;

		internal const int IgnoreBottom = 8;

		internal const int IgnoreAll = 15;

		internal const char StreamNameSeparator = '_';

		internal const string PageStyleName = "p";

		internal const string MHTMLPrefix = "cid:";

		internal const string CSSSuffix = "style";

		protected const string m_resourceNamespace = "AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources";

		internal static byte[] m_overflowXHidden;

		internal static byte[] m_percentWidthOverflow;

		internal static byte[] m_layoutFixed;

		internal static byte[] m_layoutBorder;

		internal static byte[] m_ignoreBorder;

		internal static byte[] m_ignoreBorderL;

		internal static byte[] m_ignoreBorderR;

		internal static byte[] m_ignoreBorderT;

		internal static byte[] m_ignoreBorderB;

		internal static byte[] m_percentHeight;

		internal static byte[] m_percentSizesOverflow;

		internal static byte[] m_percentSizes;

		internal static byte[] m_space;

		internal static byte[] m_closeBracket;

		internal static byte[] m_semiColon;

		internal static byte[] m_border;

		internal static byte[] m_borderBottom;

		internal static byte[] m_borderLeft;

		internal static byte[] m_borderRight;

		internal static byte[] m_borderTop;

		internal static byte[] m_marginBottom;

		internal static byte[] m_marginLeft;

		internal static byte[] m_marginRight;

		internal static byte[] m_marginTop;

		internal static byte[] m_textIndent;

		internal static byte[] m_mm;

		internal static byte[] m_styleWidth;

		internal static byte[] m_styleHeight;

		internal static byte[] m_percent;

		internal static byte[] m_ninetyninepercent;

		internal static byte[] m_degree90;

		internal static byte[] m_newLine;

		internal static byte[] m_closeAccol;

		internal static byte[] m_backgroundRepeat;

		internal static byte[] m_closeBrace;

		internal static byte[] m_backgroundColor;

		internal static byte[] m_backgroundImage;

		internal static byte[] m_overflowHidden;

		internal static byte[] m_wordWrap;

		internal static byte[] m_whiteSpacePreWrap;

		internal static byte[] m_leftValue;

		internal static byte[] m_rightValue;

		internal static byte[] m_centerValue;

		internal static byte[] m_textAlign;

		internal static byte[] m_verticalAlign;

		internal static byte[] m_lineHeight;

		internal static byte[] m_color;

		internal static byte[] m_writingMode;

		internal static byte[] m_tbrl;

		internal static byte[] m_btrl;

		internal static byte[] m_lrtb;

		internal static byte[] m_rltb;

		internal static byte[] m_layoutFlow;

		internal static byte[] m_verticalIdeographic;

		internal static byte[] m_horizontal;

		internal static byte[] m_unicodeBiDi;

		internal static byte[] m_direction;

		internal static byte[] m_textDecoration;

		internal static byte[] m_fontWeight;

		internal static byte[] m_fontSize;

		internal static byte[] m_fontFamily;

		internal static byte[] m_fontStyle;

		internal static byte[] m_openAccol;

		internal static byte[] m_borderColor;

		internal static byte[] m_borderStyle;

		internal static byte[] m_borderWidth;

		internal static byte[] m_borderBottomColor;

		internal static byte[] m_borderBottomStyle;

		internal static byte[] m_borderBottomWidth;

		internal static byte[] m_borderLeftColor;

		internal static byte[] m_borderLeftStyle;

		internal static byte[] m_borderLeftWidth;

		internal static byte[] m_borderRightColor;

		internal static byte[] m_borderRightStyle;

		internal static byte[] m_borderRightWidth;

		internal static byte[] m_borderTopColor;

		internal static byte[] m_borderTopStyle;

		internal static byte[] m_borderTopWidth;

		internal static byte[] m_paddingBottom;

		internal static byte[] m_paddingLeft;

		internal static byte[] m_paddingRight;

		internal static byte[] m_paddingTop;

		protected static byte[] m_classAction;

		internal static byte[] m_styleAction;

		internal static byte[] m_emptyTextBox;

		internal static byte[] m_percentSizeInlineTable;

		internal static byte[] m_classPercentSizeInlineTable;

		internal static byte[] m_percentHeightInlineTable;

		internal static byte[] m_classPercentHeightInlineTable;

		internal static byte[] m_dot;

		internal static byte[] m_popupAction;

		internal static byte[] m_tableLayoutFixed;

		internal static byte[] m_borderCollapse;

		internal static byte[] m_none;

		internal static byte[] m_displayNone;

		internal static byte[] m_rtlEmbed;

		internal static byte[] m_classRtlEmbed;

		internal static byte[] m_noVerticalMarginClassName;

		internal static byte[] m_zeroPoint;

		internal static byte[] m_smallPoint;

		internal static byte[] m_filter;

		internal static byte[] m_basicImageRotation180;

		internal static byte[] m_msoRotation;

		internal static byte[] m_styleMinWidth;

		internal static byte[] m_styleMinHeight;

		private static byte[] m_styleDisplayInlineBlock;

		internal static byte[] m_reportItemCustomAttr;

		protected static byte[] m_br;

		protected static byte[] m_tabIndex;

		protected static byte[] m_closeTable;

		protected static byte[] m_openTable;

		protected static byte[] m_closeDiv;

		protected static byte[] m_openDiv;

		protected static byte[] m_zeroBorder;

		protected static byte[] m_cols;

		protected static byte[] m_colSpan;

		protected static byte[] m_rowSpan;

		protected static byte[] m_headers;

		protected static byte[] m_closeTD;

		protected static byte[] m_closeTR;

		protected static byte[] m_firstTD;

		protected static byte[] m_lastTD;

		protected static byte[] m_openTD;

		protected static byte[] m_openTR;

		protected static byte[] m_valign;

		protected static byte[] m_closeQuote;

		internal static string m_closeQuoteString;

		protected static byte[] m_closeSpan;

		protected static byte[] m_openSpan;

		protected static byte[] m_quote;

		internal static string m_quoteString;

		protected static byte[] m_closeTag;

		protected static byte[] m_id;

		protected static byte[] m_px;

		protected static byte[] m_zeroWidth;

		protected static byte[] m_zeroHeight;

		protected static byte[] m_openHtml;

		protected static byte[] m_closeHtml;

		protected static byte[] m_openBody;

		protected static byte[] m_closeBody;

		protected static byte[] m_openHead;

		protected static byte[] m_closeHead;

		protected static byte[] m_openTitle;

		protected static byte[] m_closeTitle;

		protected static byte[] m_openA;

		protected static byte[] m_target;

		protected static byte[] m_closeA;

		protected static string m_hrefString;

		protected static byte[] m_href;

		protected static byte[] m_nohref;

		protected static byte[] m_inlineHeight;

		protected static byte[] m_inlineWidth;

		protected static byte[] m_img;

		protected static byte[] m_imgOnError;

		protected static byte[] m_src;

		protected static byte[] m_topValue;

		protected static byte[] m_alt;

		protected static byte[] m_title;

		protected static byte[] m_classID;

		protected static byte[] m_codeBase;

		protected static byte[] m_valueObject;

		protected static byte[] m_paramObject;

		protected static byte[] m_openObject;

		protected static byte[] m_closeObject;

		protected static byte[] m_equal;

		protected static byte[] m_encodedAmp;

		protected static byte[] m_nbsp;

		protected static byte[] m_questionMark;

		protected static byte[] m_checked;

		protected static byte[] m_checkForEnterKey;

		protected static byte[] m_unchecked;

		protected static byte[] m_showHideOnClick;

		protected static byte[] m_cursorHand;

		protected static byte[] m_rtlDir;

		protected static byte[] m_ltrDir;

		protected static byte[] m_classStyle;

		protected static byte[] m_openStyle;

		protected static byte[] m_underscore;

		protected static byte[] m_lineBreak;

		protected static byte[] m_ssClassID;

		protected static byte[] m_ptClassID;

		protected static byte[] m_xmlData;

		protected static byte[] m_useMap;

		protected static byte[] m_openMap;

		protected static byte[] m_closeMap;

		protected static byte[] m_mapArea;

		protected static byte[] m_mapCoords;

		protected static byte[] m_mapShape;

		protected static byte[] m_name;

		protected static byte[] m_dataName;

		protected static byte[] m_circleShape;

		protected static byte[] m_polyShape;

		protected static byte[] m_rectShape;

		protected static byte[] m_comma;

		private static string m_mapPrefixString;

		protected static byte[] m_mapPrefix;

		protected static byte[] m_classPopupAction;

		protected static byte[] m_closeLi;

		protected static byte[] m_openLi;

		protected static byte[] m_firstNonHeaderPostfix;

		protected static byte[] m_fixedMatrixCornerPostfix;

		protected static byte[] m_fixedRowGroupingHeaderPostfix;

		protected static byte[] m_fixedColumnGroupingHeaderPostfix;

		protected static byte[] m_fixedRowHeaderPostfix;

		protected static byte[] m_fixedColumnHeaderPostfix;

		protected static byte[] m_fixedTableCornerPostfix;

		internal static byte[] m_language;

		private static byte[] m_zeroBorderWidth;

		internal static byte[] m_onLoadFitProportionalPv;

		private static byte[] m_normalWordWrap;

		private static byte[] m_classPercentSizes;

		private static byte[] m_classPercentSizesOverflow;

		private static byte[] m_classPercentWidthOverflow;

		private static byte[] m_classPercentHeight;

		private static byte[] m_classLayoutBorder;

		private static byte[] m_classLayoutFixed;

		private static byte[] m_strokeColor;

		private static byte[] m_strokeWeight;

		private static byte[] m_slineStyle;

		private static byte[] m_dashStyle;

		private static byte[] m_closeVGroup;

		private static byte[] m_openVGroup;

		private static byte[] m_openVLine;

		private static byte[] m_leftSlant;

		private static byte[] m_rightSlant;

		private static byte[] m_pageBreakDelimiter;

		private static byte[] m_nogrowAttribute;

		private static byte[] m_stylePositionAbsolute;

		private static byte[] m_stylePositionRelative;

		private static byte[] m_styleClipRectOpenBrace;

		private static byte[] m_styleTop;

		private static byte[] m_styleLeft;

		private static byte[] m_pxSpace;

		internal static char[] m_cssDelimiters;

		protected bool m_hasOnePage = true;

		protected Stream m_mainStream;

		internal Encoding m_encoding;

		protected RPLReport m_rplReport;

		protected RPLPageContent m_pageContent;

		protected RPLReportSection m_rplReportSection;

		protected IReportWrapper m_report;

		protected ISPBProcessing m_spbProcessing;

		protected Hashtable m_usedStyles;

		protected NameValueCollection m_serverParams;

		protected DeviceInfo m_deviceInfo;

		protected NameValueCollection m_rawDeviceInfo;

		protected Dictionary<string, string> m_images;

		protected byte[] m_stylePrefixIdBytes;

		protected int m_pageNum;

		protected AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream m_createAndRegisterStreamCallback;

		protected bool m_fitPropImages;

		protected bool m_browserIE = true;

		protected RequestType m_requestType;

		protected bool m_htmlFragment;

		protected Stream m_styleStream;

		protected Stream m_growRectangleIdsStream;

		protected Stream m_fitVertTextIdsStream;

		protected Stream m_imgFitDivIdsStream;

		protected Stream m_imgConImageIdsStream;

		protected bool m_useInlineStyle;

		protected bool m_pageWithBookmarkLinks;

		protected bool m_pageWithSortClicks;

		protected bool m_allPages;

		protected int m_outputLineLength;

		protected bool m_onlyVisibleStyles;

		private SecondaryStreams m_createSecondaryStreams = SecondaryStreams.Server;

		protected int m_tabIndexNum;

		protected int m_currentHitCount;

		protected Hashtable m_duplicateItems;

		protected string m_searchText;

		protected bool m_emitImageConsolidationScaling;

		protected bool m_needsCanGrowFalseScript;

		protected bool m_needsGrowRectangleScript;

		protected bool m_needsFitVertTextScript;

		internal static string m_searchHitIdPrefix;

		internal static string m_standardLineBreak;

		protected Stack m_linkToChildStack;

		protected PageSection m_pageSection;

		protected bool m_pageHasStyle;

		protected bool m_isBody;

		protected bool m_usePercentWidth;

		protected bool m_hasSlantedLines;

		internal bool m_expandItem;

		protected ArrayList m_fixedHeaders;

		private bool m_isStyleOpen;

		private bool m_renderTableHeight;

		private string m_contextLanguage;

		private bool m_allowBandTable = true;

		protected byte[] m_styleClassPrefix;

		internal string SearchText
		{
			set
			{
				this.m_searchText = value;
			}
		}

		internal bool NeedResizeImages
		{
			get
			{
				return this.m_fitPropImages;
			}
		}

		protected bool IsFragment
		{
			get
			{
				if (this.m_htmlFragment)
				{
					return !this.m_deviceInfo.HasActionScript;
				}
				return false;
			}
		}

		public bool IsBrowserIE
		{
			get
			{
				return this.m_deviceInfo.IsBrowserIE;
			}
		}

		protected virtual bool FillPageHeight
		{
			get
			{
				return this.m_deviceInfo.IsBrowserIE;
			}
		}

		static HTML4Renderer()
		{
			HTML4Renderer.m_overflowXHidden = null;
			HTML4Renderer.m_percentWidthOverflow = null;
			HTML4Renderer.m_layoutFixed = null;
			HTML4Renderer.m_layoutBorder = null;
			HTML4Renderer.m_ignoreBorder = null;
			HTML4Renderer.m_ignoreBorderL = null;
			HTML4Renderer.m_ignoreBorderR = null;
			HTML4Renderer.m_ignoreBorderT = null;
			HTML4Renderer.m_ignoreBorderB = null;
			HTML4Renderer.m_percentHeight = null;
			HTML4Renderer.m_percentSizesOverflow = null;
			HTML4Renderer.m_percentSizes = null;
			HTML4Renderer.m_space = null;
			HTML4Renderer.m_closeBracket = null;
			HTML4Renderer.m_semiColon = null;
			HTML4Renderer.m_border = null;
			HTML4Renderer.m_borderBottom = null;
			HTML4Renderer.m_borderLeft = null;
			HTML4Renderer.m_borderRight = null;
			HTML4Renderer.m_borderTop = null;
			HTML4Renderer.m_marginBottom = null;
			HTML4Renderer.m_marginLeft = null;
			HTML4Renderer.m_marginRight = null;
			HTML4Renderer.m_marginTop = null;
			HTML4Renderer.m_textIndent = null;
			HTML4Renderer.m_mm = null;
			HTML4Renderer.m_styleWidth = null;
			HTML4Renderer.m_styleHeight = null;
			HTML4Renderer.m_percent = null;
			HTML4Renderer.m_ninetyninepercent = null;
			HTML4Renderer.m_degree90 = null;
			HTML4Renderer.m_newLine = null;
			HTML4Renderer.m_closeAccol = null;
			HTML4Renderer.m_backgroundRepeat = null;
			HTML4Renderer.m_closeBrace = null;
			HTML4Renderer.m_backgroundColor = null;
			HTML4Renderer.m_backgroundImage = null;
			HTML4Renderer.m_overflowHidden = null;
			HTML4Renderer.m_wordWrap = null;
			HTML4Renderer.m_whiteSpacePreWrap = null;
			HTML4Renderer.m_leftValue = null;
			HTML4Renderer.m_rightValue = null;
			HTML4Renderer.m_centerValue = null;
			HTML4Renderer.m_textAlign = null;
			HTML4Renderer.m_verticalAlign = null;
			HTML4Renderer.m_lineHeight = null;
			HTML4Renderer.m_color = null;
			HTML4Renderer.m_writingMode = null;
			HTML4Renderer.m_tbrl = null;
			HTML4Renderer.m_btrl = null;
			HTML4Renderer.m_lrtb = null;
			HTML4Renderer.m_rltb = null;
			HTML4Renderer.m_layoutFlow = null;
			HTML4Renderer.m_verticalIdeographic = null;
			HTML4Renderer.m_horizontal = null;
			HTML4Renderer.m_unicodeBiDi = null;
			HTML4Renderer.m_direction = null;
			HTML4Renderer.m_textDecoration = null;
			HTML4Renderer.m_fontWeight = null;
			HTML4Renderer.m_fontSize = null;
			HTML4Renderer.m_fontFamily = null;
			HTML4Renderer.m_fontStyle = null;
			HTML4Renderer.m_openAccol = null;
			HTML4Renderer.m_borderColor = null;
			HTML4Renderer.m_borderStyle = null;
			HTML4Renderer.m_borderWidth = null;
			HTML4Renderer.m_borderBottomColor = null;
			HTML4Renderer.m_borderBottomStyle = null;
			HTML4Renderer.m_borderBottomWidth = null;
			HTML4Renderer.m_borderLeftColor = null;
			HTML4Renderer.m_borderLeftStyle = null;
			HTML4Renderer.m_borderLeftWidth = null;
			HTML4Renderer.m_borderRightColor = null;
			HTML4Renderer.m_borderRightStyle = null;
			HTML4Renderer.m_borderRightWidth = null;
			HTML4Renderer.m_borderTopColor = null;
			HTML4Renderer.m_borderTopStyle = null;
			HTML4Renderer.m_borderTopWidth = null;
			HTML4Renderer.m_paddingBottom = null;
			HTML4Renderer.m_paddingLeft = null;
			HTML4Renderer.m_paddingRight = null;
			HTML4Renderer.m_paddingTop = null;
			HTML4Renderer.m_classAction = null;
			HTML4Renderer.m_styleAction = null;
			HTML4Renderer.m_emptyTextBox = null;
			HTML4Renderer.m_percentSizeInlineTable = null;
			HTML4Renderer.m_classPercentSizeInlineTable = null;
			HTML4Renderer.m_percentHeightInlineTable = null;
			HTML4Renderer.m_classPercentHeightInlineTable = null;
			HTML4Renderer.m_dot = null;
			HTML4Renderer.m_popupAction = null;
			HTML4Renderer.m_tableLayoutFixed = null;
			HTML4Renderer.m_borderCollapse = null;
			HTML4Renderer.m_none = null;
			HTML4Renderer.m_displayNone = null;
			HTML4Renderer.m_rtlEmbed = null;
			HTML4Renderer.m_classRtlEmbed = null;
			HTML4Renderer.m_noVerticalMarginClassName = null;
			HTML4Renderer.m_zeroPoint = null;
			HTML4Renderer.m_smallPoint = null;
			HTML4Renderer.m_filter = null;
			HTML4Renderer.m_basicImageRotation180 = null;
			HTML4Renderer.m_msoRotation = null;
			HTML4Renderer.m_styleMinWidth = null;
			HTML4Renderer.m_styleMinHeight = null;
			HTML4Renderer.m_styleDisplayInlineBlock = null;
			HTML4Renderer.m_reportItemCustomAttr = null;
			HTML4Renderer.m_br = null;
			HTML4Renderer.m_tabIndex = null;
			HTML4Renderer.m_closeTable = null;
			HTML4Renderer.m_openTable = null;
			HTML4Renderer.m_closeDiv = null;
			HTML4Renderer.m_openDiv = null;
			HTML4Renderer.m_zeroBorder = null;
			HTML4Renderer.m_cols = null;
			HTML4Renderer.m_colSpan = null;
			HTML4Renderer.m_rowSpan = null;
			HTML4Renderer.m_headers = null;
			HTML4Renderer.m_closeTD = null;
			HTML4Renderer.m_closeTR = null;
			HTML4Renderer.m_firstTD = null;
			HTML4Renderer.m_lastTD = null;
			HTML4Renderer.m_openTD = null;
			HTML4Renderer.m_openTR = null;
			HTML4Renderer.m_valign = null;
			HTML4Renderer.m_closeQuote = null;
			HTML4Renderer.m_closeQuoteString = "\">";
			HTML4Renderer.m_closeSpan = null;
			HTML4Renderer.m_openSpan = null;
			HTML4Renderer.m_quote = null;
			HTML4Renderer.m_quoteString = "\"";
			HTML4Renderer.m_closeTag = null;
			HTML4Renderer.m_id = null;
			HTML4Renderer.m_px = null;
			HTML4Renderer.m_zeroWidth = null;
			HTML4Renderer.m_zeroHeight = null;
			HTML4Renderer.m_openHtml = null;
			HTML4Renderer.m_closeHtml = null;
			HTML4Renderer.m_openBody = null;
			HTML4Renderer.m_closeBody = null;
			HTML4Renderer.m_openHead = null;
			HTML4Renderer.m_closeHead = null;
			HTML4Renderer.m_openTitle = null;
			HTML4Renderer.m_closeTitle = null;
			HTML4Renderer.m_openA = null;
			HTML4Renderer.m_target = null;
			HTML4Renderer.m_closeA = null;
			HTML4Renderer.m_hrefString = " href=\"";
			HTML4Renderer.m_href = null;
			HTML4Renderer.m_nohref = null;
			HTML4Renderer.m_inlineHeight = null;
			HTML4Renderer.m_inlineWidth = null;
			HTML4Renderer.m_img = null;
			HTML4Renderer.m_imgOnError = null;
			HTML4Renderer.m_src = null;
			HTML4Renderer.m_topValue = null;
			HTML4Renderer.m_alt = null;
			HTML4Renderer.m_title = null;
			HTML4Renderer.m_classID = null;
			HTML4Renderer.m_codeBase = null;
			HTML4Renderer.m_valueObject = null;
			HTML4Renderer.m_paramObject = null;
			HTML4Renderer.m_openObject = null;
			HTML4Renderer.m_closeObject = null;
			HTML4Renderer.m_equal = null;
			HTML4Renderer.m_encodedAmp = null;
			HTML4Renderer.m_nbsp = null;
			HTML4Renderer.m_questionMark = null;
			HTML4Renderer.m_checked = null;
			HTML4Renderer.m_checkForEnterKey = null;
			HTML4Renderer.m_unchecked = null;
			HTML4Renderer.m_showHideOnClick = null;
			HTML4Renderer.m_cursorHand = null;
			HTML4Renderer.m_rtlDir = null;
			HTML4Renderer.m_ltrDir = null;
			HTML4Renderer.m_classStyle = null;
			HTML4Renderer.m_openStyle = null;
			HTML4Renderer.m_underscore = null;
			HTML4Renderer.m_lineBreak = null;
			HTML4Renderer.m_ssClassID = null;
			HTML4Renderer.m_ptClassID = null;
			HTML4Renderer.m_xmlData = null;
			HTML4Renderer.m_useMap = null;
			HTML4Renderer.m_openMap = null;
			HTML4Renderer.m_closeMap = null;
			HTML4Renderer.m_mapArea = null;
			HTML4Renderer.m_mapCoords = null;
			HTML4Renderer.m_mapShape = null;
			HTML4Renderer.m_name = null;
			HTML4Renderer.m_dataName = null;
			HTML4Renderer.m_circleShape = null;
			HTML4Renderer.m_polyShape = null;
			HTML4Renderer.m_rectShape = null;
			HTML4Renderer.m_comma = null;
			HTML4Renderer.m_mapPrefixString = "Map";
			HTML4Renderer.m_mapPrefix = null;
			HTML4Renderer.m_classPopupAction = null;
			HTML4Renderer.m_closeLi = null;
			HTML4Renderer.m_openLi = null;
			HTML4Renderer.m_firstNonHeaderPostfix = null;
			HTML4Renderer.m_fixedMatrixCornerPostfix = null;
			HTML4Renderer.m_fixedRowGroupingHeaderPostfix = null;
			HTML4Renderer.m_fixedColumnGroupingHeaderPostfix = null;
			HTML4Renderer.m_fixedRowHeaderPostfix = null;
			HTML4Renderer.m_fixedColumnHeaderPostfix = null;
			HTML4Renderer.m_fixedTableCornerPostfix = null;
			HTML4Renderer.m_language = null;
			HTML4Renderer.m_zeroBorderWidth = null;
			HTML4Renderer.m_onLoadFitProportionalPv = null;
			HTML4Renderer.m_normalWordWrap = null;
			HTML4Renderer.m_classPercentSizes = null;
			HTML4Renderer.m_classPercentSizesOverflow = null;
			HTML4Renderer.m_classPercentWidthOverflow = null;
			HTML4Renderer.m_classPercentHeight = null;
			HTML4Renderer.m_classLayoutBorder = null;
			HTML4Renderer.m_classLayoutFixed = null;
			HTML4Renderer.m_strokeColor = null;
			HTML4Renderer.m_strokeWeight = null;
			HTML4Renderer.m_slineStyle = null;
			HTML4Renderer.m_dashStyle = null;
			HTML4Renderer.m_closeVGroup = null;
			HTML4Renderer.m_openVGroup = null;
			HTML4Renderer.m_openVLine = null;
			HTML4Renderer.m_leftSlant = null;
			HTML4Renderer.m_rightSlant = null;
			HTML4Renderer.m_pageBreakDelimiter = null;
			HTML4Renderer.m_nogrowAttribute = null;
			HTML4Renderer.m_stylePositionAbsolute = null;
			HTML4Renderer.m_stylePositionRelative = null;
			HTML4Renderer.m_styleClipRectOpenBrace = null;
			HTML4Renderer.m_styleTop = null;
			HTML4Renderer.m_styleLeft = null;
			HTML4Renderer.m_pxSpace = null;
			HTML4Renderer.m_cssDelimiters = new char[13]
			{
				'[',
				']',
				'"',
				'\'',
				'<',
				'>',
				'{',
				'}',
				'(',
				')',
				'/',
				'%',
				' '
			};
			HTML4Renderer.m_searchHitIdPrefix = "oHit";
			HTML4Renderer.m_standardLineBreak = "\n";
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			HTML4Renderer.m_newLine = uTF8Encoding.GetBytes("\r\n");
			HTML4Renderer.m_openTable = uTF8Encoding.GetBytes("<TABLE CELLSPACING=\"0\" CELLPADDING=\"0\"");
			HTML4Renderer.m_zeroBorder = uTF8Encoding.GetBytes(" BORDER=\"0\"");
			HTML4Renderer.m_zeroPoint = uTF8Encoding.GetBytes("0pt");
			HTML4Renderer.m_smallPoint = uTF8Encoding.GetBytes("1px");
			HTML4Renderer.m_cols = uTF8Encoding.GetBytes(" COLS=\"");
			HTML4Renderer.m_colSpan = uTF8Encoding.GetBytes(" COLSPAN=\"");
			HTML4Renderer.m_rowSpan = uTF8Encoding.GetBytes(" ROWSPAN=\"");
			HTML4Renderer.m_headers = uTF8Encoding.GetBytes(" HEADERS=\"");
			HTML4Renderer.m_space = uTF8Encoding.GetBytes(" ");
			HTML4Renderer.m_closeBracket = uTF8Encoding.GetBytes(">");
			HTML4Renderer.m_closeTable = uTF8Encoding.GetBytes("</TABLE>");
			HTML4Renderer.m_openDiv = uTF8Encoding.GetBytes("<DIV");
			HTML4Renderer.m_closeDiv = uTF8Encoding.GetBytes("</DIV>");
			HTML4Renderer.m_openBody = uTF8Encoding.GetBytes("<body");
			HTML4Renderer.m_closeBody = uTF8Encoding.GetBytes("</body>");
			HTML4Renderer.m_openHtml = uTF8Encoding.GetBytes("<html>");
			HTML4Renderer.m_closeHtml = uTF8Encoding.GetBytes("</html>");
			HTML4Renderer.m_openHead = uTF8Encoding.GetBytes("<head>");
			HTML4Renderer.m_closeHead = uTF8Encoding.GetBytes("</head>");
			HTML4Renderer.m_openTitle = uTF8Encoding.GetBytes("<title>");
			HTML4Renderer.m_closeTitle = uTF8Encoding.GetBytes("</title>");
			HTML4Renderer.m_firstTD = uTF8Encoding.GetBytes("<TR><TD");
			HTML4Renderer.m_lastTD = uTF8Encoding.GetBytes("</TD></TR>");
			HTML4Renderer.m_openTD = uTF8Encoding.GetBytes("<TD");
			HTML4Renderer.m_closeTD = uTF8Encoding.GetBytes("</TD>");
			HTML4Renderer.m_closeTR = uTF8Encoding.GetBytes("</TR>");
			HTML4Renderer.m_openTR = uTF8Encoding.GetBytes("<TR");
			HTML4Renderer.m_valign = uTF8Encoding.GetBytes(" VALIGN=\"");
			HTML4Renderer.m_openSpan = uTF8Encoding.GetBytes("<span");
			HTML4Renderer.m_closeSpan = uTF8Encoding.GetBytes("</span>");
			HTML4Renderer.m_quote = uTF8Encoding.GetBytes(HTML4Renderer.m_quoteString);
			HTML4Renderer.m_closeQuote = uTF8Encoding.GetBytes(HTML4Renderer.m_closeQuoteString);
			HTML4Renderer.m_id = uTF8Encoding.GetBytes(" ID=\"");
			HTML4Renderer.m_mm = uTF8Encoding.GetBytes("mm");
			HTML4Renderer.m_px = uTF8Encoding.GetBytes("px");
			HTML4Renderer.m_zeroWidth = uTF8Encoding.GetBytes(" WIDTH=\"0\"");
			HTML4Renderer.m_zeroHeight = uTF8Encoding.GetBytes(" HEIGHT=\"0\"");
			HTML4Renderer.m_closeTag = uTF8Encoding.GetBytes("\"/>");
			HTML4Renderer.m_openA = uTF8Encoding.GetBytes("<a");
			HTML4Renderer.m_target = uTF8Encoding.GetBytes(" TARGET=\"");
			HTML4Renderer.m_closeA = uTF8Encoding.GetBytes("</a>");
			HTML4Renderer.m_href = uTF8Encoding.GetBytes(HTML4Renderer.m_hrefString);
			HTML4Renderer.m_nohref = uTF8Encoding.GetBytes(" nohref=\"true\"");
			HTML4Renderer.m_inlineHeight = uTF8Encoding.GetBytes(" HEIGHT=\"");
			HTML4Renderer.m_inlineWidth = uTF8Encoding.GetBytes(" WIDTH=\"");
			HTML4Renderer.m_img = uTF8Encoding.GetBytes("<IMG");
			HTML4Renderer.m_imgOnError = uTF8Encoding.GetBytes(" onerror=\"this.errored=true;\"");
			HTML4Renderer.m_src = uTF8Encoding.GetBytes(" SRC=\"");
			HTML4Renderer.m_topValue = uTF8Encoding.GetBytes("top");
			HTML4Renderer.m_leftValue = uTF8Encoding.GetBytes("left");
			HTML4Renderer.m_rightValue = uTF8Encoding.GetBytes("right");
			HTML4Renderer.m_centerValue = uTF8Encoding.GetBytes("center");
			HTML4Renderer.m_classID = uTF8Encoding.GetBytes(" CLASSID=\"CLSID:");
			HTML4Renderer.m_codeBase = uTF8Encoding.GetBytes(" CODEBASE=\"");
			HTML4Renderer.m_title = uTF8Encoding.GetBytes(" TITLE=\"");
			HTML4Renderer.m_alt = uTF8Encoding.GetBytes(" ALT=\"");
			HTML4Renderer.m_openObject = uTF8Encoding.GetBytes("<OBJECT");
			HTML4Renderer.m_closeObject = uTF8Encoding.GetBytes("</OBJECT>");
			HTML4Renderer.m_paramObject = uTF8Encoding.GetBytes("<PARAM NAME=\"");
			HTML4Renderer.m_valueObject = uTF8Encoding.GetBytes(" VALUE=\"");
			HTML4Renderer.m_equal = uTF8Encoding.GetBytes("=");
			HTML4Renderer.m_encodedAmp = uTF8Encoding.GetBytes("&amp;");
			HTML4Renderer.m_nbsp = uTF8Encoding.GetBytes("&nbsp;");
			HTML4Renderer.m_questionMark = uTF8Encoding.GetBytes("?");
			HTML4Renderer.m_none = uTF8Encoding.GetBytes("none");
			HTML4Renderer.m_displayNone = uTF8Encoding.GetBytes("display: none;");
			HTML4Renderer.m_checkForEnterKey = uTF8Encoding.GetBytes("if(event.keyCode == 13 || event.which == 13){");
			HTML4Renderer.m_percent = uTF8Encoding.GetBytes("100%");
			HTML4Renderer.m_ninetyninepercent = uTF8Encoding.GetBytes("99%");
			HTML4Renderer.m_degree90 = uTF8Encoding.GetBytes("90");
			HTML4Renderer.m_lineBreak = uTF8Encoding.GetBytes(HTML4Renderer.m_standardLineBreak);
			HTML4Renderer.m_closeBrace = uTF8Encoding.GetBytes(")");
			HTML4Renderer.m_rtlDir = uTF8Encoding.GetBytes(" dir=\"RTL\"");
			HTML4Renderer.m_ltrDir = uTF8Encoding.GetBytes(" dir=\"LTR\"");
			HTML4Renderer.m_br = uTF8Encoding.GetBytes("<br/>");
			HTML4Renderer.m_tabIndex = uTF8Encoding.GetBytes(" tabindex=\"");
			HTML4Renderer.m_useMap = uTF8Encoding.GetBytes(" USEMAP=\"");
			HTML4Renderer.m_openMap = uTF8Encoding.GetBytes("<MAP ");
			HTML4Renderer.m_closeMap = uTF8Encoding.GetBytes("</MAP>");
			HTML4Renderer.m_mapArea = uTF8Encoding.GetBytes("<AREA ");
			HTML4Renderer.m_mapCoords = uTF8Encoding.GetBytes(" COORDS=\"");
			HTML4Renderer.m_mapShape = uTF8Encoding.GetBytes(" SHAPE=\"");
			HTML4Renderer.m_name = uTF8Encoding.GetBytes(" NAME=\"");
			HTML4Renderer.m_dataName = uTF8Encoding.GetBytes(" data-name=\"");
			HTML4Renderer.m_circleShape = uTF8Encoding.GetBytes("circle");
			HTML4Renderer.m_polyShape = uTF8Encoding.GetBytes("poly");
			HTML4Renderer.m_rectShape = uTF8Encoding.GetBytes("rect");
			HTML4Renderer.m_comma = uTF8Encoding.GetBytes(",");
			HTML4Renderer.m_mapPrefix = uTF8Encoding.GetBytes(HTML4Renderer.m_mapPrefixString);
			HTML4Renderer.m_openLi = uTF8Encoding.GetBytes("<li");
			HTML4Renderer.m_closeLi = uTF8Encoding.GetBytes("</li>");
			HTML4Renderer.m_firstNonHeaderPostfix = uTF8Encoding.GetBytes("_FNHR");
			HTML4Renderer.m_fixedMatrixCornerPostfix = uTF8Encoding.GetBytes("_MCC");
			HTML4Renderer.m_fixedRowGroupingHeaderPostfix = uTF8Encoding.GetBytes("_FRGH");
			HTML4Renderer.m_fixedColumnGroupingHeaderPostfix = uTF8Encoding.GetBytes("_FCGH");
			HTML4Renderer.m_fixedRowHeaderPostfix = uTF8Encoding.GetBytes("_FRH");
			HTML4Renderer.m_fixedColumnHeaderPostfix = uTF8Encoding.GetBytes("_FCH");
			HTML4Renderer.m_fixedTableCornerPostfix = uTF8Encoding.GetBytes("_FCC");
			HTML4Renderer.m_dot = uTF8Encoding.GetBytes(".");
			HTML4Renderer.m_percentSizes = uTF8Encoding.GetBytes("r1");
			HTML4Renderer.m_percentSizesOverflow = uTF8Encoding.GetBytes("r2");
			HTML4Renderer.m_percentHeight = uTF8Encoding.GetBytes("r3");
			HTML4Renderer.m_ignoreBorder = uTF8Encoding.GetBytes("r4");
			HTML4Renderer.m_ignoreBorderL = uTF8Encoding.GetBytes("r5");
			HTML4Renderer.m_ignoreBorderR = uTF8Encoding.GetBytes("r6");
			HTML4Renderer.m_ignoreBorderT = uTF8Encoding.GetBytes("r7");
			HTML4Renderer.m_ignoreBorderB = uTF8Encoding.GetBytes("r8");
			HTML4Renderer.m_layoutFixed = uTF8Encoding.GetBytes("r9");
			HTML4Renderer.m_layoutBorder = uTF8Encoding.GetBytes("r10");
			HTML4Renderer.m_percentWidthOverflow = uTF8Encoding.GetBytes("r11");
			HTML4Renderer.m_popupAction = uTF8Encoding.GetBytes("r12");
			HTML4Renderer.m_styleAction = uTF8Encoding.GetBytes("r13");
			HTML4Renderer.m_emptyTextBox = uTF8Encoding.GetBytes("r14");
			HTML4Renderer.m_classPercentSizes = uTF8Encoding.GetBytes(" class=\"r1\"");
			HTML4Renderer.m_classPercentSizesOverflow = uTF8Encoding.GetBytes(" class=\"r2\"");
			HTML4Renderer.m_classPercentHeight = uTF8Encoding.GetBytes(" class=\"r3\"");
			HTML4Renderer.m_classLayoutFixed = uTF8Encoding.GetBytes(" class=\"r9");
			HTML4Renderer.m_classLayoutBorder = uTF8Encoding.GetBytes(" class=\"r10");
			HTML4Renderer.m_classPercentWidthOverflow = uTF8Encoding.GetBytes(" class=\"r11\"");
			HTML4Renderer.m_classPopupAction = uTF8Encoding.GetBytes(" class=\"r12\"");
			HTML4Renderer.m_classAction = uTF8Encoding.GetBytes(" class=\"r13\"");
			HTML4Renderer.m_rtlEmbed = uTF8Encoding.GetBytes("r15");
			HTML4Renderer.m_classRtlEmbed = uTF8Encoding.GetBytes(" class=\"r15\"");
			HTML4Renderer.m_noVerticalMarginClassName = uTF8Encoding.GetBytes("r16");
			HTML4Renderer.m_percentSizeInlineTable = uTF8Encoding.GetBytes("r17");
			HTML4Renderer.m_classPercentSizeInlineTable = uTF8Encoding.GetBytes(" class=\"r17\"");
			HTML4Renderer.m_percentHeightInlineTable = uTF8Encoding.GetBytes("r18");
			HTML4Renderer.m_classPercentHeightInlineTable = uTF8Encoding.GetBytes(" class=\"r18\"");
			HTML4Renderer.m_underscore = uTF8Encoding.GetBytes("_");
			HTML4Renderer.m_openAccol = uTF8Encoding.GetBytes("{");
			HTML4Renderer.m_closeAccol = uTF8Encoding.GetBytes("}");
			HTML4Renderer.m_classStyle = uTF8Encoding.GetBytes(" class=\"");
			HTML4Renderer.m_openStyle = uTF8Encoding.GetBytes(" style=\"");
			HTML4Renderer.m_styleHeight = uTF8Encoding.GetBytes("HEIGHT:");
			HTML4Renderer.m_styleMinHeight = uTF8Encoding.GetBytes("min-height:");
			HTML4Renderer.m_styleWidth = uTF8Encoding.GetBytes("WIDTH:");
			HTML4Renderer.m_styleMinWidth = uTF8Encoding.GetBytes("min-width:");
			HTML4Renderer.m_zeroBorderWidth = uTF8Encoding.GetBytes("border-width:0px");
			HTML4Renderer.m_border = uTF8Encoding.GetBytes("border:");
			HTML4Renderer.m_borderLeft = uTF8Encoding.GetBytes("border-left:");
			HTML4Renderer.m_borderTop = uTF8Encoding.GetBytes("border-top:");
			HTML4Renderer.m_borderBottom = uTF8Encoding.GetBytes("border-bottom:");
			HTML4Renderer.m_borderRight = uTF8Encoding.GetBytes("border-right:");
			HTML4Renderer.m_borderColor = uTF8Encoding.GetBytes("border-color:");
			HTML4Renderer.m_borderStyle = uTF8Encoding.GetBytes("border-style:");
			HTML4Renderer.m_borderWidth = uTF8Encoding.GetBytes("border-width:");
			HTML4Renderer.m_borderBottomColor = uTF8Encoding.GetBytes("border-bottom-color:");
			HTML4Renderer.m_borderBottomStyle = uTF8Encoding.GetBytes("border-bottom-style:");
			HTML4Renderer.m_borderBottomWidth = uTF8Encoding.GetBytes("border-bottom-width:");
			HTML4Renderer.m_borderLeftColor = uTF8Encoding.GetBytes("border-left-color:");
			HTML4Renderer.m_borderLeftStyle = uTF8Encoding.GetBytes("border-left-style:");
			HTML4Renderer.m_borderLeftWidth = uTF8Encoding.GetBytes("border-left-width:");
			HTML4Renderer.m_borderRightColor = uTF8Encoding.GetBytes("border-right-color:");
			HTML4Renderer.m_borderRightStyle = uTF8Encoding.GetBytes("border-right-style:");
			HTML4Renderer.m_borderRightWidth = uTF8Encoding.GetBytes("border-right-width:");
			HTML4Renderer.m_borderTopColor = uTF8Encoding.GetBytes("border-top-color:");
			HTML4Renderer.m_borderTopStyle = uTF8Encoding.GetBytes("border-top-style:");
			HTML4Renderer.m_borderTopWidth = uTF8Encoding.GetBytes("border-top-width:");
			HTML4Renderer.m_semiColon = uTF8Encoding.GetBytes(";");
			HTML4Renderer.m_wordWrap = uTF8Encoding.GetBytes("word-wrap:break-word");
			HTML4Renderer.m_whiteSpacePreWrap = uTF8Encoding.GetBytes("white-space:pre-wrap");
			HTML4Renderer.m_normalWordWrap = uTF8Encoding.GetBytes("word-wrap:normal");
			HTML4Renderer.m_overflowHidden = uTF8Encoding.GetBytes("overflow:hidden");
			HTML4Renderer.m_overflowXHidden = uTF8Encoding.GetBytes("overflow-x:hidden");
			HTML4Renderer.m_borderCollapse = uTF8Encoding.GetBytes("border-collapse:collapse");
			HTML4Renderer.m_tableLayoutFixed = uTF8Encoding.GetBytes("table-layout:fixed");
			HTML4Renderer.m_paddingLeft = uTF8Encoding.GetBytes("padding-left:");
			HTML4Renderer.m_paddingRight = uTF8Encoding.GetBytes("padding-right:");
			HTML4Renderer.m_paddingTop = uTF8Encoding.GetBytes("padding-top:");
			HTML4Renderer.m_paddingBottom = uTF8Encoding.GetBytes("padding-bottom:");
			HTML4Renderer.m_backgroundColor = uTF8Encoding.GetBytes("background-color:");
			HTML4Renderer.m_backgroundImage = uTF8Encoding.GetBytes("background-image:url(");
			HTML4Renderer.m_backgroundRepeat = uTF8Encoding.GetBytes("background-repeat:");
			HTML4Renderer.m_fontStyle = uTF8Encoding.GetBytes("font-style:");
			HTML4Renderer.m_fontFamily = uTF8Encoding.GetBytes("font-family:");
			HTML4Renderer.m_fontSize = uTF8Encoding.GetBytes("font-size:");
			HTML4Renderer.m_fontWeight = uTF8Encoding.GetBytes("font-weight:");
			HTML4Renderer.m_textDecoration = uTF8Encoding.GetBytes("text-decoration:");
			HTML4Renderer.m_textAlign = uTF8Encoding.GetBytes("text-align:");
			HTML4Renderer.m_verticalAlign = uTF8Encoding.GetBytes("vertical-align:");
			HTML4Renderer.m_color = uTF8Encoding.GetBytes("color:");
			HTML4Renderer.m_lineHeight = uTF8Encoding.GetBytes("line-height:");
			HTML4Renderer.m_direction = uTF8Encoding.GetBytes("direction:");
			HTML4Renderer.m_unicodeBiDi = uTF8Encoding.GetBytes("unicode-bidi:");
			HTML4Renderer.m_writingMode = uTF8Encoding.GetBytes("writing-mode:");
			HTML4Renderer.m_msoRotation = uTF8Encoding.GetBytes("mso-rotate:");
			HTML4Renderer.m_tbrl = uTF8Encoding.GetBytes("tb-rl");
			HTML4Renderer.m_btrl = uTF8Encoding.GetBytes("bt-rl");
			HTML4Renderer.m_lrtb = uTF8Encoding.GetBytes("lr-tb");
			HTML4Renderer.m_rltb = uTF8Encoding.GetBytes("rl-tb");
			HTML4Renderer.m_layoutFlow = uTF8Encoding.GetBytes("layout-flow:");
			HTML4Renderer.m_verticalIdeographic = uTF8Encoding.GetBytes("vertical-ideographic");
			HTML4Renderer.m_horizontal = uTF8Encoding.GetBytes("horizontal");
			HTML4Renderer.m_cursorHand = uTF8Encoding.GetBytes("cursor:pointer");
			HTML4Renderer.m_filter = uTF8Encoding.GetBytes("filter:");
			HTML4Renderer.m_language = uTF8Encoding.GetBytes(" LANG=\"");
			HTML4Renderer.m_marginLeft = uTF8Encoding.GetBytes("margin-left:");
			HTML4Renderer.m_marginTop = uTF8Encoding.GetBytes("margin-top:");
			HTML4Renderer.m_marginBottom = uTF8Encoding.GetBytes("margin-bottom:");
			HTML4Renderer.m_marginRight = uTF8Encoding.GetBytes("margin-right:");
			HTML4Renderer.m_textIndent = uTF8Encoding.GetBytes("text-indent:");
			HTML4Renderer.m_onLoadFitProportionalPv = uTF8Encoding.GetBytes(" onload=\"this.fitproportional=true;this.pv=");
			HTML4Renderer.m_basicImageRotation180 = uTF8Encoding.GetBytes("progid:DXImageTransform.Microsoft.BasicImage(rotation=2)");
			HTML4Renderer.m_openVGroup = uTF8Encoding.GetBytes("<v:group coordsize=\"100,100\" coordorigin=\"0,0\"");
			HTML4Renderer.m_openVLine = uTF8Encoding.GetBytes("<v:line from=\"0,");
			HTML4Renderer.m_strokeColor = uTF8Encoding.GetBytes(" strokecolor=\"");
			HTML4Renderer.m_strokeWeight = uTF8Encoding.GetBytes(" strokeWeight=\"");
			HTML4Renderer.m_dashStyle = uTF8Encoding.GetBytes("<v:stroke dashstyle=\"");
			HTML4Renderer.m_slineStyle = uTF8Encoding.GetBytes(" slineStyle=\"");
			HTML4Renderer.m_closeVGroup = uTF8Encoding.GetBytes("</v:line></v:group>");
			HTML4Renderer.m_rightSlant = uTF8Encoding.GetBytes("100\" to=\"100,0\"");
			HTML4Renderer.m_leftSlant = uTF8Encoding.GetBytes("0\" to=\"100,100\"");
			HTML4Renderer.m_pageBreakDelimiter = uTF8Encoding.GetBytes("<div style=\"page-break-after:always\"><hr/></div>");
			HTML4Renderer.m_stylePositionAbsolute = uTF8Encoding.GetBytes("position:absolute;");
			HTML4Renderer.m_stylePositionRelative = uTF8Encoding.GetBytes("position:relative;");
			HTML4Renderer.m_styleClipRectOpenBrace = uTF8Encoding.GetBytes("clip:rect(");
			HTML4Renderer.m_styleTop = uTF8Encoding.GetBytes("top:");
			HTML4Renderer.m_styleLeft = uTF8Encoding.GetBytes("left:");
			HTML4Renderer.m_pxSpace = uTF8Encoding.GetBytes("px ");
			HTML4Renderer.m_nogrowAttribute = uTF8Encoding.GetBytes(" nogrow=\"true\"");
			HTML4Renderer.m_styleMinWidth = uTF8Encoding.GetBytes("min-width: ");
			HTML4Renderer.m_styleMinHeight = uTF8Encoding.GetBytes("min-height: ");
			HTML4Renderer.m_styleDisplayInlineBlock = uTF8Encoding.GetBytes("display: inline-block;");
			HTML4Renderer.m_reportItemCustomAttr = uTF8Encoding.GetBytes(" data-reportitem=\"");
		}

		public HTML4Renderer(IReportWrapper report, ISPBProcessing spbProcessing, NameValueCollection reportServerParams, DeviceInfo deviceInfo, NameValueCollection rawDeviceInfo, NameValueCollection browserCaps, AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream createAndRegisterStreamCallback, SecondaryStreams secondaryStreams)
		{
			this.SearchText = deviceInfo.FindString;
			this.m_report = report;
			this.m_spbProcessing = spbProcessing;
			this.m_createSecondaryStreams = secondaryStreams;
			this.m_usedStyles = new Hashtable();
			this.m_images = new Dictionary<string, string>();
			this.m_browserIE = deviceInfo.IsBrowserIE;
			this.m_deviceInfo = deviceInfo;
			this.m_rawDeviceInfo = rawDeviceInfo;
			this.m_serverParams = reportServerParams;
			this.m_createAndRegisterStreamCallback = createAndRegisterStreamCallback;
			this.m_htmlFragment = deviceInfo.HTMLFragment;
			this.m_onlyVisibleStyles = deviceInfo.OnlyVisibleStyles;
			this.m_pageNum = deviceInfo.Section;
			rawDeviceInfo.Remove("Section");
			rawDeviceInfo.Remove("FindString");
			rawDeviceInfo.Remove("BookmarkId");
			SPBContext context = new SPBContext
			{
				StartPage = this.m_pageNum,
				EndPage = this.m_pageNum,
				SecondaryStreams = this.m_createSecondaryStreams,
				AddSecondaryStreamNames = true,
				UseImageConsolidation = this.m_deviceInfo.ImageConsolidation
			};
			this.m_spbProcessing.SetContext(context);
			this.m_linkToChildStack = new Stack(1);
			this.m_stylePrefixIdBytes = Encoding.UTF8.GetBytes(this.m_deviceInfo.StylePrefixId);
			if (!this.m_deviceInfo.StyleStream)
			{
				this.m_useInlineStyle = this.m_htmlFragment;
			}
		}

		internal void InitializeReport()
		{
			this.m_rplReport = this.GetNextPage();
			if (this.m_rplReport == null)
			{
				throw new InvalidSectionException();
			}
			this.m_pageContent = this.m_rplReport.RPLPaginatedPages[0];
			this.m_rplReportSection = this.m_pageContent.GetNextReportSection();
			this.CheckBodyStyle();
			this.m_contextLanguage = this.m_rplReport.Language;
			this.m_expandItem = false;
		}

		protected static string GetStyleStreamName(string aReportName, int aPageNumber)
		{
			return HTML4Renderer.GetStreamName(aReportName, aPageNumber, "style");
		}

		internal static string GetStreamName(string aReportName, int aPageNumber, string suffix)
		{
			if (aPageNumber > 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{1}{3}", aReportName, '_', suffix, aPageNumber);
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", aReportName, '_', suffix);
		}

		internal static string HandleSpecialFontCharacters(string fontName)
		{
			if (fontName.IndexOfAny(HTML4Renderer.m_cssDelimiters) != -1)
			{
				fontName = fontName.Trim();
				if (fontName.StartsWith("'", StringComparison.Ordinal))
				{
					fontName = fontName.Substring(1);
				}
				if (fontName.EndsWith("'", StringComparison.Ordinal))
				{
					fontName = fontName.Substring(0, fontName.Length - 1);
				}
				return "'" + fontName.Replace("'", "&quot;") + "'";
			}
			return fontName;
		}

		protected abstract void RenderSortAction(RPLTextBoxProps textBoxProps, RPLFormat.SortOptions sortState);

		protected abstract void RenderInternalImageSrc();

		protected abstract void RenderToggleImage(RPLTextBoxProps textBoxProps);

		public abstract void Render(TextWriter outputWriter);

		internal void RenderStylesOnly(string streamName)
		{
			this.m_encoding = Encoding.UTF8;
			Stream stream = this.CreateStream(streamName, "css", Encoding.UTF8, "text/css", false, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateAndRegister);
			StyleContext styleContext = new StyleContext();
			int num = 0;
			this.m_styleStream = new BufferedStream(stream);
			while (this.m_rplReportSection != null)
			{
				num = 0;
				RPLItemMeasurement header = this.m_rplReportSection.Header;
				if (header != null)
				{
					RPLHeaderFooter rPLHeaderFooter = (RPLHeaderFooter)header.Element;
					RPLElementProps elementProps = rPLHeaderFooter.ElementProps;
					RPLElementPropsDef definition = elementProps.Definition;
					styleContext.StyleOnCell = true;
					this.RenderSharedStyle(rPLHeaderFooter, elementProps, definition, definition.SharedStyle, header, definition.ID + "c", styleContext, ref num);
					styleContext.StyleOnCell = false;
					this.RenderSharedStyle(rPLHeaderFooter, elementProps, definition, definition.SharedStyle, header, definition.ID, styleContext, ref num);
					RPLItemMeasurement[] children = rPLHeaderFooter.Children;
					if (children != null)
					{
						for (int i = 0; i < children.Length; i++)
						{
							this.RenderStylesOnlyRecursive(children[i], new StyleContext());
						}
					}
					header.Element = null;
				}
				RPLItemMeasurement footer = this.m_rplReportSection.Footer;
				if (footer != null)
				{
					RPLHeaderFooter rPLHeaderFooter2 = (RPLHeaderFooter)footer.Element;
					RPLElementProps elementProps2 = rPLHeaderFooter2.ElementProps;
					RPLElementPropsDef definition2 = elementProps2.Definition;
					styleContext.StyleOnCell = true;
					this.RenderSharedStyle(rPLHeaderFooter2, elementProps2, definition2, definition2.SharedStyle, footer, definition2.ID + "c", styleContext, ref num);
					styleContext.StyleOnCell = false;
					this.RenderSharedStyle(rPLHeaderFooter2, elementProps2, definition2, definition2.SharedStyle, footer, definition2.ID, styleContext, ref num);
					RPLItemMeasurement[] children2 = rPLHeaderFooter2.Children;
					if (children2 != null)
					{
						for (int j = 0; j < children2.Length; j++)
						{
							this.RenderStylesOnlyRecursive(children2[j], new StyleContext());
						}
					}
					footer.Element = null;
				}
				RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
				rPLItemMeasurement.Width = this.m_pageContent.MaxSectionWidth;
				rPLItemMeasurement.Height = this.m_rplReportSection.BodyArea.Height;
				RPLItemMeasurement rPLItemMeasurement2 = this.m_rplReportSection.Columns[0];
				RPLBody rPLBody = (RPLBody)this.m_rplReportSection.Columns[0].Element;
				RPLElementProps elementProps3 = rPLBody.ElementProps;
				RPLElementPropsDef definition3 = elementProps3.Definition;
				this.RenderSharedStyle(rPLBody, elementProps3, definition3, definition3.SharedStyle, rPLItemMeasurement, definition3.ID, styleContext, ref num);
				RPLItemMeasurement[] children3 = rPLBody.Children;
				if (children3 != null && children3.Length > 0)
				{
					for (int k = 0; k < children3.Length; k++)
					{
						this.RenderStylesOnlyRecursive(children3[k], new StyleContext());
					}
				}
				rPLItemMeasurement2.Element = null;
				this.m_rplReportSection = this.m_pageContent.GetNextReportSection();
			}
			this.m_styleStream.Flush();
		}

		internal void RenderStylesOnlyRecursive(RPLItemMeasurement measurement, StyleContext styleContext)
		{
			int num = 0;
			RPLElement element = measurement.Element;
			RPLElementProps elementProps = element.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLStyleProps sharedStyle = definition.SharedStyle;
			string iD = definition.ID;
			object obj = elementProps.Style[26];
			if (element is RPLTextBox)
			{
				RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)definition;
				bool ignoreVerticalAlign = styleContext.IgnoreVerticalAlign;
				if (rPLTextBoxPropsDef.CanSort && !this.m_usedStyles.ContainsKey(iD + "p"))
				{
					if (rPLTextBoxPropsDef.CanGrow || rPLTextBoxPropsDef.CanShrink)
					{
						styleContext.StyleOnCell = true;
					}
					if (!rPLTextBoxPropsDef.CanGrow && rPLTextBoxPropsDef.CanShrink)
					{
						styleContext.IgnoreVerticalAlign = true;
					}
					this.RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "p", styleContext, ref num);
					styleContext.StyleOnCell = false;
				}
				if (!this.m_deviceInfo.IsBrowserIE || this.m_deviceInfo.BrowserMode == BrowserMode.Standards || this.m_deviceInfo.OutlookCompat || (obj != null && (RPLFormat.VerticalAlignments)obj != 0))
				{
					styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
				}
				if (rPLTextBoxPropsDef.CanShrink && !this.m_usedStyles.ContainsKey(iD + "s"))
				{
					styleContext.NoBorders = true;
					this.RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "s", styleContext, ref num);
					if (!rPLTextBoxPropsDef.CanGrow)
					{
						styleContext.IgnoreVerticalAlign = true;
					}
				}
				if (rPLTextBoxPropsDef.CanSort && !rPLTextBoxPropsDef.IsSimple && !this.IsFragment && rPLTextBoxPropsDef.IsToggleParent)
				{
					styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
				}
				styleContext.RenderMeasurements = false;
				if (!this.m_usedStyles.ContainsKey(iD))
				{
					int num2 = num;
					this.RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD, styleContext, ref num2);
					styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
					num2 = num;
					this.RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "l", styleContext, ref num2);
					this.RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD + "r", styleContext, ref num);
				}
				RPLTextBoxProps rPLTextBoxProps = elementProps as RPLTextBoxProps;
				if (!this.m_usedStyles.ContainsKey(iD + "a") && this.HasAction(rPLTextBoxProps.ActionInfo))
				{
					TextRunStyleWriter textRunStyleWriter = new TextRunStyleWriter(this);
					this.RenderSharedStyle(textRunStyleWriter, definition.SharedStyle, styleContext, iD + "a");
					textRunStyleWriter.WriteStyles(StyleWriterMode.Shared, definition.SharedStyle);
				}
				if (!rPLTextBoxPropsDef.IsSimple)
				{
					RPLTextBox rPLTextBox = element as RPLTextBox;
					ParagraphStyleWriter paragraphStyleWriter = new ParagraphStyleWriter(this, rPLTextBox);
					TextRunStyleWriter styleWriter = new TextRunStyleWriter(this);
					for (RPLParagraph nextParagraph = rPLTextBox.GetNextParagraph(); nextParagraph != null; nextParagraph = rPLTextBox.GetNextParagraph())
					{
						paragraphStyleWriter.Paragraph = nextParagraph;
						string iD2 = nextParagraph.ElementProps.Definition.ID;
						paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.All;
						this.RenderSharedStyle(paragraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2);
						paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ListOnly;
						this.RenderSharedStyle(paragraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2 + "l");
						paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ParagraphOnly;
						this.RenderSharedStyle(paragraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2 + "p");
						for (RPLTextRun nextTextRun = nextParagraph.GetNextTextRun(); nextTextRun != null; nextTextRun = nextParagraph.GetNextTextRun())
						{
							this.RenderSharedStyle(styleWriter, nextTextRun.ElementProps.Definition.SharedStyle, styleContext, nextTextRun.ElementProps.Definition.ID);
						}
					}
				}
			}
			else
			{
				if (!this.m_usedStyles.ContainsKey(iD))
				{
					this.RenderSharedStyle(element, elementProps, definition, sharedStyle, measurement, iD, styleContext, ref num);
				}
				if (element is RPLSubReport)
				{
					RPLItemMeasurement[] children = ((RPLSubReport)element).Children;
					if (children != null)
					{
						for (int i = 0; i < children.Length; i++)
						{
							RPLContainer rPLContainer = children[i].Element as RPLContainer;
							if (rPLContainer != null && rPLContainer.Children != null && rPLContainer.Children.Length > 0)
							{
								for (int j = 0; j < rPLContainer.Children.Length; j++)
								{
									this.RenderStylesOnlyRecursive(rPLContainer.Children[j], styleContext);
									rPLContainer.Children[j] = null;
								}
							}
							children[i] = null;
						}
						measurement.Element = null;
					}
				}
				else if (element is RPLContainer)
				{
					styleContext.InTablix = false;
					RPLItemMeasurement[] children2 = ((RPLContainer)element).Children;
					if (children2 != null && children2.Length > 0)
					{
						for (int k = 0; k < children2.Length; k++)
						{
							this.RenderStylesOnlyRecursive(children2[k], styleContext);
							children2[k] = null;
						}
					}
				}
				else if (element is RPLTablix)
				{
					RPLTablix rPLTablix = (RPLTablix)element;
					RPLTablixRow nextRow = rPLTablix.GetNextRow();
					bool inTablix = styleContext.InTablix;
					while (nextRow != null)
					{
						for (int l = 0; l < nextRow.NumCells; l++)
						{
							RPLTablixCell rPLTablixCell = nextRow[l];
							RPLElement element2 = rPLTablixCell.Element;
							RPLElementProps elementProps2 = element2.ElementProps;
							RPLElementPropsDef definition2 = elementProps2.Definition;
							RPLStyleProps sharedStyle2 = definition2.SharedStyle;
							bool zeroWidth = styleContext.ZeroWidth;
							float columnWidth = rPLTablix.GetColumnWidth(rPLTablixCell.ColIndex, rPLTablixCell.ColSpan);
							styleContext.ZeroWidth = (columnWidth == 0.0);
							if (element2 != null)
							{
								string iD3 = definition2.ID;
								if (!(element2 is RPLLine) && !this.m_usedStyles.ContainsKey(iD3 + "c"))
								{
									styleContext.StyleOnCell = true;
									num = HTML4Renderer.GetNewContext(num, rPLTablixCell.ColIndex == 0, rPLTablixCell.ColIndex + rPLTablixCell.ColSpan == rPLTablix.ColumnWidths.Length, rPLTablixCell.RowIndex == 0, rPLTablixCell.RowIndex + rPLTablixCell.RowSpan == rPLTablix.RowHeights.Length);
									int num3 = num;
									RPLTextBox rPLTextBox2 = (RPLTextBox)element2;
									bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
									if (rPLTextBox2 != null && HTML4Renderer.IsWritingModeVertical(sharedStyle2) && this.m_deviceInfo.IsBrowserIE && this.m_deviceInfo.BrowserMode == BrowserMode.Standards)
									{
										styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
									}
									this.RenderSharedStyle(element2, elementProps2, definition2, sharedStyle2, null, iD3 + "c", styleContext, ref num3);
									num3 = num;
									this.RenderSharedStyle(element2, elementProps2, definition2, sharedStyle2, null, iD3 + "cl", styleContext, ref num3);
									this.RenderSharedStyle(element2, elementProps2, definition2, sharedStyle2, null, iD3 + "cr", styleContext, ref num);
									styleContext.StyleOnCell = false;
									styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
								}
								styleContext.InTablix = true;
								if (element2 is RPLContainer)
								{
									RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
									rPLItemMeasurement.Width = rPLTablix.GetColumnWidth(rPLTablixCell.ColIndex, rPLTablixCell.ColSpan);
									rPLItemMeasurement.Height = rPLTablix.GetRowHeight(rPLTablixCell.RowIndex, rPLTablixCell.RowSpan);
									rPLItemMeasurement.Element = (element2 as RPLItem);
									this.RenderStylesOnlyRecursive(rPLItemMeasurement, styleContext);
								}
								else if (!this.m_usedStyles.ContainsKey(iD3))
								{
									if (element2 is RPLTextBox)
									{
										object obj2 = element2.ElementProps.Style[26];
										RPLTextBoxPropsDef rPLTextBoxPropsDef2 = (RPLTextBoxPropsDef)element2.ElementProps.Definition;
										bool flag = obj2 != null && (RPLFormat.VerticalAlignments)obj2 != 0 && !rPLTextBoxPropsDef2.CanGrow;
										if (rPLTextBoxPropsDef2.CanSort || flag)
										{
											styleContext.RenderMeasurements = false;
										}
									}
									this.RenderSharedStyle(element2, elementProps2, definition2, sharedStyle2, null, element2.ElementProps.Definition.ID, styleContext, ref num);
								}
								styleContext.InTablix = inTablix;
								nextRow[l] = null;
								styleContext.ZeroWidth = zeroWidth;
							}
						}
						nextRow = rPLTablix.GetNextRow();
					}
				}
				measurement.Element = null;
			}
		}

		internal void RenderEmptyTopTablixRow(RPLTablix tablix, List<RPLTablixOmittedRow> omittedRows, string tablixID, bool emptyCol, TablixFixedHeaderStorage headerStorage)
		{
			bool flag = headerStorage.RowHeaders != null || headerStorage.ColumnHeaders != null;
			this.WriteStream(HTML4Renderer.m_openTR);
			if (flag)
			{
				string text = tablixID + "r";
				this.RenderReportItemId(text);
				if (headerStorage.RowHeaders != null)
				{
					headerStorage.RowHeaders.Add(text);
				}
				if (headerStorage.ColumnHeaders != null)
				{
					headerStorage.ColumnHeaders.Add(text);
				}
				if (headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			this.WriteStream(HTML4Renderer.m_zeroHeight);
			this.WriteStream(HTML4Renderer.m_closeBracket);
			if (emptyCol)
			{
				headerStorage.HasEmptyCol = true;
				this.WriteStream(HTML4Renderer.m_openTD);
				if (headerStorage.RowHeaders != null)
				{
					string text2 = tablixID + "e";
					this.RenderReportItemId(text2);
					headerStorage.RowHeaders.Add(text2);
					if (headerStorage.CornerHeaders != null)
					{
						headerStorage.CornerHeaders.Add(text2);
					}
				}
				this.WriteStream(HTML4Renderer.m_openStyle);
				this.WriteStream(HTML4Renderer.m_styleWidth);
				this.WriteStream("0");
				this.WriteStream(HTML4Renderer.m_px);
				this.WriteStream(HTML4Renderer.m_closeQuote);
				this.WriteStream(HTML4Renderer.m_closeTD);
			}
			int[] array = new int[omittedRows.Count];
			for (int i = 0; i < tablix.ColumnWidths.Length; i++)
			{
				this.WriteStream(HTML4Renderer.m_openTD);
				if (tablix.FixedColumns[i] && headerStorage.RowHeaders != null)
				{
					string text3 = tablixID + "e" + i;
					this.RenderReportItemId(text3);
					headerStorage.RowHeaders.Add(text3);
					if (i == tablix.ColumnWidths.Length - 1 || !tablix.FixedColumns[i + 1])
					{
						headerStorage.LastRowGroupCol = text3;
					}
					if (headerStorage.CornerHeaders != null)
					{
						headerStorage.CornerHeaders.Add(text3);
					}
				}
				this.WriteStream(HTML4Renderer.m_openStyle);
				if (tablix.ColumnWidths[i] == 0.0)
				{
					this.WriteStream(HTML4Renderer.m_displayNone);
				}
				this.WriteStream(HTML4Renderer.m_styleWidth);
				this.WriteDStream(tablix.ColumnWidths[i]);
				this.WriteStream(HTML4Renderer.m_mm);
				this.WriteStream(HTML4Renderer.m_semiColon);
				this.WriteStream(HTML4Renderer.m_styleMinWidth);
				this.WriteDStream(tablix.ColumnWidths[i]);
				this.WriteStream(HTML4Renderer.m_mm);
				this.WriteStream(HTML4Renderer.m_closeQuote);
				for (int j = 0; j < omittedRows.Count; j++)
				{
					List<RPLTablixMemberCell> omittedHeaders = omittedRows[j].OmittedHeaders;
					this.RenderTablixOmittedHeaderCells(omittedHeaders, i, false, ref array[j]);
				}
				this.WriteStream(HTML4Renderer.m_closeTD);
			}
			this.WriteStream(HTML4Renderer.m_closeTR);
		}

		internal void RenderEmptyHeightCell(float height, string tablixID, bool fixedRow, int row, TablixFixedHeaderStorage headerStorage)
		{
			this.WriteStream(HTML4Renderer.m_openTD);
			if (headerStorage.RowHeaders != null)
			{
				string text = tablixID + "h" + row;
				this.RenderReportItemId(text);
				headerStorage.RowHeaders.Add(text);
				if (fixedRow && headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			this.WriteStream(HTML4Renderer.m_openStyle);
			this.WriteStream(HTML4Renderer.m_styleHeight);
			this.WriteDStream(height);
			this.WriteStream(HTML4Renderer.m_mm);
			this.WriteStream(HTML4Renderer.m_closeQuote);
			this.WriteStream(HTML4Renderer.m_closeTD);
		}

		protected static int GetNewContext(int borderContext, bool left, bool right, bool top, bool bottom)
		{
			int num = 0;
			if (borderContext > 0)
			{
				if (top)
				{
					num |= (borderContext & 4);
				}
				if (bottom)
				{
					num |= (borderContext & 8);
				}
				if (left)
				{
					num |= (borderContext & 1);
				}
				if (right)
				{
					num |= (borderContext & 2);
				}
			}
			return num;
		}

		protected static int GetNewContext(int borderContext, int x, int y, int xMax, int yMax)
		{
			int num = 0;
			if (borderContext > 0)
			{
				if (x == 1)
				{
					num |= (borderContext & 4);
				}
				if (x == xMax)
				{
					num |= (borderContext & 8);
				}
				if (y == 1)
				{
					num |= (borderContext & 1);
				}
				if (y == yMax)
				{
					num |= (borderContext & 2);
				}
			}
			return num;
		}

		protected System.Drawing.Rectangle RenderDynamicImage(RPLItemMeasurement measurement, RPLDynamicImageProps dynamicImageProps)
		{
			if (this.m_createSecondaryStreams != 0)
			{
				return dynamicImageProps.ImageConsolidationOffsets;
			}
			Stream stream = null;
			stream = this.CreateStream(dynamicImageProps.StreamName, "png", null, "image/png", false, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateAndRegister);
			if (dynamicImageProps.DynamicImageContentOffset >= 0)
			{
				this.m_rplReport.GetImage(dynamicImageProps.DynamicImageContentOffset, stream);
			}
			else if (dynamicImageProps.DynamicImageContent != null)
			{
				byte[] array = new byte[4096];
				dynamicImageProps.DynamicImageContent.Position = 0L;
				int num2;
				for (int num = (int)dynamicImageProps.DynamicImageContent.Length; num > 0; num -= num2)
				{
					num2 = dynamicImageProps.DynamicImageContent.Read(array, 0, Math.Min(array.Length, num));
					stream.Write(array, 0, num2);
				}
			}
			return System.Drawing.Rectangle.Empty;
		}

		protected bool IsCollectionWithoutContent(RPLContainer container, ref bool empty)
		{
			bool result = false;
			if (container != null)
			{
				result = true;
				if (container.Children == null)
				{
					empty = true;
				}
			}
			return result;
		}

		private void RenderOpenStyle(string id)
		{
			this.WriteStreamLineBreak();
			if (this.m_styleClassPrefix != null)
			{
				this.WriteStream(this.m_styleClassPrefix);
			}
			this.WriteStream(HTML4Renderer.m_dot);
			this.WriteStream(this.m_stylePrefixIdBytes);
			this.WriteStream(id);
			this.WriteStream(HTML4Renderer.m_openAccol);
		}

		protected virtual RPLReport GetNextPage()
		{
			RPLReport result = default(RPLReport);
			this.m_spbProcessing.GetNextPage(out result);
			return result;
		}

		protected virtual bool NeedSharedToggleParent(RPLTextBoxProps textBoxProps)
		{
			if (!this.IsFragment)
			{
				return textBoxProps.IsToggleParent;
			}
			return false;
		}

		protected virtual bool CanSort(RPLTextBoxPropsDef textBoxDef)
		{
			if (!this.IsFragment)
			{
				return textBoxDef.CanSort;
			}
			return false;
		}

		protected void RenderSortImage(RPLTextBoxProps textBoxProps)
		{
			if (this.m_deviceInfo.BrowserMode == BrowserMode.Quirks || this.m_deviceInfo.IsBrowserIE)
			{
				this.WriteStream(HTML4Renderer.m_nbsp);
			}
			this.WriteStream(HTML4Renderer.m_openA);
			this.WriteStream(HTML4Renderer.m_tabIndex);
			this.WriteStream(++this.m_tabIndexNum);
			this.WriteStream(HTML4Renderer.m_quote);
			RPLFormat.SortOptions sortState = textBoxProps.SortState;
			this.RenderSortAction(textBoxProps, sortState);
			this.WriteStream(HTML4Renderer.m_img);
			this.WriteStream(HTML4Renderer.m_alt);
			switch (sortState)
			{
			case RPLFormat.SortOptions.Ascending:
				this.WriteAttrEncoded(RenderRes.SortAscAltText);
				break;
			case RPLFormat.SortOptions.Descending:
				this.WriteAttrEncoded(RenderRes.SortDescAltText);
				break;
			default:
				this.WriteAttrEncoded(RenderRes.UnsortedAltText);
				break;
			}
			this.WriteStream(HTML4Renderer.m_quote);
			if (this.m_browserIE)
			{
				this.WriteStream(HTML4Renderer.m_imgOnError);
			}
			this.WriteStream(HTML4Renderer.m_zeroBorder);
			this.WriteStream(HTML4Renderer.m_src);
			this.RenderSortImageText(sortState);
			this.WriteStream(HTML4Renderer.m_closeTag);
			this.WriteStream(HTML4Renderer.m_closeA);
		}

		protected virtual void RenderSortImageText(RPLFormat.SortOptions sortState)
		{
			this.RenderInternalImageSrc();
			switch (sortState)
			{
			case RPLFormat.SortOptions.Ascending:
				this.WriteStream(this.m_report.GetImageName("sortAsc.gif"));
				break;
			case RPLFormat.SortOptions.Descending:
				this.WriteStream(this.m_report.GetImageName("sortDesc.gif"));
				break;
			default:
				this.WriteStream(this.m_report.GetImageName("unsorted.gif"));
				break;
			}
		}

		internal void RenderOnClickActionScript(string actionType, string actionArg)
		{
			this.WriteStream(" onclick=\"");
			this.WriteStream(this.m_deviceInfo.ActionScript);
			this.WriteStream("('");
			this.WriteStream(actionType);
			this.WriteStream("','");
			this.WriteStream(actionArg);
			this.WriteStream("');return false;\"");
			this.WriteStream(" onkeypress=\"");
			this.WriteStream(HTML4Renderer.m_checkForEnterKey);
			this.WriteStream(this.m_deviceInfo.ActionScript);
			this.WriteStream("('");
			this.WriteStream(actionType);
			this.WriteStream("','");
			this.WriteStream(actionArg);
			this.WriteStream("');return false;}\"");
		}

		protected PaddingSharedInfo GetPaddings(RPLElementStyle style, PaddingSharedInfo paddingInfo)
		{
			int num = 0;
			RPLReportSize rPLReportSize = null;
			double num2 = 0.0;
			double num3 = 0.0;
			bool flag = false;
			PaddingSharedInfo result = paddingInfo;
			if (paddingInfo != null)
			{
				num = paddingInfo.PaddingContext;
				num2 = paddingInfo.PadH;
				num3 = paddingInfo.PadV;
			}
			if ((num & 4) == 0)
			{
				string text = (string)style[17];
				if (text != null)
				{
					rPLReportSize = new RPLReportSize(text);
					flag = true;
					num |= 4;
					num3 += rPLReportSize.ToMillimeters();
				}
			}
			if ((num & 8) == 0)
			{
				flag = true;
				string text2 = (string)style[18];
				if (text2 != null)
				{
					rPLReportSize = new RPLReportSize(text2);
					num |= 8;
					num3 += rPLReportSize.ToMillimeters();
				}
			}
			if ((num & 1) == 0)
			{
				flag = true;
				string text3 = (string)style[15];
				if (text3 != null)
				{
					rPLReportSize = new RPLReportSize(text3);
					num |= 1;
					num2 += rPLReportSize.ToMillimeters();
				}
			}
			if ((num & 2) == 0)
			{
				flag = true;
				string text4 = (string)style[16];
				if (text4 != null)
				{
					rPLReportSize = new RPLReportSize(text4);
					num |= 2;
					num2 += rPLReportSize.ToMillimeters();
				}
			}
			if (flag)
			{
				result = new PaddingSharedInfo(num, num2, num3);
			}
			return result;
		}

		protected bool NeedReportItemId(RPLElement repItem, RPLElementProps props)
		{
			if (this.m_pageSection != 0)
			{
				return false;
			}
			bool flag = this.m_linkToChildStack.Count > 0 && props.Definition.ID.Equals(this.m_linkToChildStack.Peek());
			if (flag)
			{
				this.m_linkToChildStack.Pop();
			}
			RPLItemProps rPLItemProps = props as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			string bookmark = rPLItemProps.Bookmark;
			if (bookmark == null)
			{
				bookmark = rPLItemPropsDef.Bookmark;
			}
			string label = rPLItemProps.Label;
			if (label == null)
			{
				label = rPLItemPropsDef.Label;
			}
			if (bookmark == null && label == null)
			{
				return flag;
			}
			return true;
		}

		protected void RenderHtmlBody()
		{
			int num = 0;
			this.m_isBody = true;
			this.m_hasOnePage = (this.m_spbProcessing.Done || this.m_pageNum != 0);
			this.RenderPageStart(true, this.m_spbProcessing.Done, this.m_pageContent.PageLayout.Style);
			this.m_pageSection = PageSection.Body;
			bool flag = this.m_rplReport != null;
			while (flag)
			{
				bool flag2 = this.m_pageContent.ReportSectionSizes.Length > 1 || this.m_rplReportSection.Header != null || this.m_rplReportSection.Footer != null;
				if (flag2)
				{
					this.WriteStream(HTML4Renderer.m_openTable);
					this.WriteStream(HTML4Renderer.m_closeBracket);
				}
				while (this.m_rplReportSection != null)
				{
					num = 0;
					RPLItemMeasurement header = this.m_rplReportSection.Header;
					RPLItemMeasurement footer = this.m_rplReportSection.Footer;
					StyleContext styleContext = new StyleContext();
					RPLItemMeasurement rPLItemMeasurement = this.m_rplReportSection.Columns[0];
					RPLBody rPLBody = rPLItemMeasurement.Element as RPLBody;
					RPLItemProps rPLItemProps = rPLBody.ElementProps as RPLItemProps;
					RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
					if (flag2)
					{
						if (header != null)
						{
							this.m_pageSection = PageSection.PageHeader;
							this.m_isBody = false;
							this.RenderPageHeaderFooter(header);
							this.m_isBody = true;
						}
						this.WriteStream(HTML4Renderer.m_firstTD);
						styleContext.StyleOnCell = true;
						this.RenderReportItemStyle(rPLBody, rPLItemProps, rPLItemPropsDef, null, styleContext, ref num, rPLItemPropsDef.ID + "c");
						styleContext.StyleOnCell = false;
						this.WriteStream(HTML4Renderer.m_closeBracket);
					}
					this.m_pageSection = PageSection.Body;
					this.m_isBody = true;
					RPLItemMeasurement rPLItemMeasurement2 = new RPLItemMeasurement();
					rPLItemMeasurement2.Width = this.m_pageContent.MaxSectionWidth;
					rPLItemMeasurement2.Height = this.m_rplReportSection.BodyArea.Height;
					this.RenderRectangle(rPLBody, rPLItemProps, rPLItemPropsDef, rPLItemMeasurement2, ref num, false, styleContext);
					if (flag2)
					{
						this.WriteStream(HTML4Renderer.m_closeTD);
						this.WriteStream(HTML4Renderer.m_closeTR);
						if (footer != null)
						{
							this.m_pageSection = PageSection.PageFooter;
							this.m_isBody = false;
							this.RenderPageHeaderFooter(footer);
							this.m_isBody = true;
						}
					}
					this.m_rplReportSection = this.m_pageContent.GetNextReportSection();
					rPLItemMeasurement.Element = null;
				}
				if (flag2)
				{
					this.WriteStream(HTML4Renderer.m_closeTable);
				}
				this.RenderPageEnd();
				if (this.m_pageNum == 0)
				{
					if (!this.m_spbProcessing.Done)
					{
						if (this.m_rplReport != null)
						{
							this.m_rplReport.Release();
						}
						RPLReport rPLReport = null;
						rPLReport = this.GetNextPage();
						this.m_pageContent = rPLReport.RPLPaginatedPages[0];
						this.m_rplReportSection = this.m_pageContent.GetNextReportSection();
						this.m_rplReport = rPLReport;
						this.WriteStream(HTML4Renderer.m_pageBreakDelimiter);
						this.RenderPageStart(false, this.m_spbProcessing.Done, this.m_pageContent.PageLayout.Style);
						num = 0;
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
			}
			if (this.m_rplReport != null)
			{
				this.m_rplReport.Release();
			}
		}

		protected abstract void WriteScrollbars();

		protected abstract void WriteFixedHeaderOnScrollScript();

		protected abstract void WriteFixedHeaderPropertyChangeScript();

		protected virtual void RenderPageStart(bool firstPage, bool lastPage, RPLElementStyle pageStyle)
		{
			this.WriteStream(HTML4Renderer.m_openDiv);
			this.WriteStream(HTML4Renderer.m_ltrDir);
			this.RenderPageStartDimensionStyles(lastPage);
			if (firstPage)
			{
				this.RenderReportItemId("oReportDiv");
			}
			bool flag = this.m_hasOnePage && this.m_deviceInfo.AllowScript && this.m_deviceInfo.HTMLFragment;
			if (flag)
			{
				this.WriteFixedHeaderOnScrollScript();
			}
			if (this.m_pageHasStyle)
			{
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_openDiv);
				this.OpenStyle();
				if (this.FillPageHeight)
				{
					this.WriteStream(HTML4Renderer.m_styleHeight);
					this.WriteStream(HTML4Renderer.m_percent);
					this.WriteStream(HTML4Renderer.m_semiColon);
				}
				this.WriteStream(HTML4Renderer.m_styleWidth);
				this.WriteStream(HTML4Renderer.m_percent);
				this.WriteStream(HTML4Renderer.m_semiColon);
				this.RenderPageStyle(pageStyle);
				this.CloseStyle(true);
			}
			this.WriteStream(HTML4Renderer.m_closeBracket);
			this.WriteStream(HTML4Renderer.m_openTable);
			this.WriteStream(HTML4Renderer.m_closeBracket);
			this.WriteStream(HTML4Renderer.m_firstTD);
			if (firstPage)
			{
				this.RenderReportItemId("oReportCell");
			}
			this.RenderZoom();
			if (flag)
			{
				this.WriteFixedHeaderPropertyChangeScript();
			}
			this.WriteStream(HTML4Renderer.m_closeBracket);
		}

		protected virtual void RenderPageStartDimensionStyles(bool lastPage)
		{
			if (this.m_pageNum != 0 || lastPage)
			{
				this.WriteStream(HTML4Renderer.m_openStyle);
				this.WriteScrollbars();
				if (this.m_deviceInfo.IsBrowserIE)
				{
					this.WriteStream(HTML4Renderer.m_styleHeight);
					this.WriteStream(HTML4Renderer.m_percent);
					this.WriteStream(HTML4Renderer.m_semiColon);
				}
				this.WriteStream(HTML4Renderer.m_styleWidth);
				this.WriteStream(HTML4Renderer.m_percent);
				this.WriteStream(HTML4Renderer.m_semiColon);
				this.WriteStream("direction:ltr");
				this.WriteStream(HTML4Renderer.m_quote);
			}
			else
			{
				this.OpenStyle();
				this.WriteStream("direction:ltr");
				this.CloseStyle(true);
			}
		}

		private void RenderPageStyle(RPLElementStyle style)
		{
			int num = 0;
			if (this.m_useInlineStyle)
			{
				this.OpenStyle();
				this.RenderBackgroundStyleProps(style);
				this.RenderHtmlBorders(style, ref num, 0, true, true, null);
				this.CloseStyle(true);
			}
			else
			{
				RPLStyleProps sharedProperties = style.SharedProperties;
				RPLStyleProps nonSharedProperties = style.NonSharedProperties;
				if (sharedProperties != null && sharedProperties.Count > 0)
				{
					this.CloseStyle(true);
					string text = "p";
					byte[] array = (byte[])this.m_usedStyles[text];
					if (array == null)
					{
						array = this.m_encoding.GetBytes(text);
						this.m_usedStyles.Add(text, array);
						if (this.m_onlyVisibleStyles)
						{
							Stream mainStream = this.m_mainStream;
							this.m_mainStream = this.m_styleStream;
							this.RenderOpenStyle(text);
							this.RenderBackgroundStyleProps(sharedProperties);
							this.RenderHtmlBorders(sharedProperties, ref num, 0, true, true, null);
							this.WriteStream(HTML4Renderer.m_closeAccol);
							this.m_mainStream = mainStream;
						}
					}
					this.WriteClassStyle(array, true);
				}
				if (nonSharedProperties != null && nonSharedProperties.Count > 0)
				{
					this.OpenStyle();
					num = 0;
					this.RenderHtmlBorders(nonSharedProperties, ref num, 0, true, true, sharedProperties);
					this.RenderBackgroundStyleProps(nonSharedProperties);
					this.CloseStyle(true);
				}
			}
		}

		protected void OpenStyle()
		{
			if (!this.m_isStyleOpen)
			{
				this.m_isStyleOpen = true;
				this.WriteStream(HTML4Renderer.m_openStyle);
			}
		}

		protected void CloseStyle(bool renderQuote)
		{
			if (this.m_isStyleOpen && renderQuote)
			{
				this.WriteStream(HTML4Renderer.m_quote);
			}
			this.m_isStyleOpen = false;
		}

		public void WriteClassName(byte[] className, byte[] classNameIfNoPrefix)
		{
			if (this.m_deviceInfo.HtmlPrefixId.Length > 0 || classNameIfNoPrefix == null)
			{
				this.WriteStream(HTML4Renderer.m_classStyle);
				this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
				this.WriteStream(className);
				this.WriteStream(HTML4Renderer.m_quote);
			}
			else
			{
				this.WriteStream(classNameIfNoPrefix);
			}
		}

		protected virtual void WriteClassStyle(byte[] styleBytes, bool close)
		{
			this.WriteStream(HTML4Renderer.m_classStyle);
			this.WriteStream(this.m_stylePrefixIdBytes);
			this.WriteStream(styleBytes);
			if (close)
			{
				this.WriteStream(HTML4Renderer.m_quote);
			}
		}

		protected void RenderBackgroundStyleProps(IRPLStyle style)
		{
			object obj = style[34];
			if (obj != null)
			{
				this.WriteStream(HTML4Renderer.m_backgroundColor);
				this.WriteStream(obj);
				this.WriteStream(HTML4Renderer.m_semiColon);
			}
			obj = style[33];
			if (obj != null)
			{
				this.WriteStream(HTML4Renderer.m_backgroundImage);
				this.RenderImageUrl(true, (RPLImageData)obj);
				this.WriteStream(HTML4Renderer.m_closeBrace);
				this.WriteStream(HTML4Renderer.m_semiColon);
			}
			obj = style[35];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.BackgroundRepeatTypes)obj);
				this.WriteStream(HTML4Renderer.m_backgroundRepeat);
				this.WriteStream(obj);
				this.WriteStream(HTML4Renderer.m_semiColon);
			}
		}

		protected virtual void RenderPageEnd()
		{
			if (this.m_deviceInfo.ExpandContent)
			{
				this.WriteStream(HTML4Renderer.m_lastTD);
				this.WriteStream(HTML4Renderer.m_closeTable);
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_closeTD);
				this.WriteStream(HTML4Renderer.m_openTD);
				this.WriteStream(HTML4Renderer.m_inlineWidth);
				this.WriteStream(HTML4Renderer.m_percent);
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_inlineHeight);
				this.WriteStream("0");
				this.WriteStream(HTML4Renderer.m_closeQuote);
				this.WriteStream(HTML4Renderer.m_lastTD);
				this.WriteStream(HTML4Renderer.m_firstTD);
				this.WriteStream(HTML4Renderer.m_inlineWidth);
				if (this.m_deviceInfo.IsBrowserGeckoEngine)
				{
					this.WriteStream(HTML4Renderer.m_percent);
				}
				else
				{
					this.WriteStream("0");
				}
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_inlineHeight);
				this.WriteStream(HTML4Renderer.m_percent);
				this.WriteStream(HTML4Renderer.m_closeQuote);
				this.WriteStream(HTML4Renderer.m_lastTD);
				this.WriteStream(HTML4Renderer.m_closeTable);
			}
			if (this.m_pageHasStyle)
			{
				this.WriteStream(HTML4Renderer.m_closeDiv);
			}
			this.WriteStream(HTML4Renderer.m_closeDiv);
		}

		public virtual void WriteStream(string theString)
		{
			if (theString.Length != 0)
			{
				byte[] array = null;
				array = this.m_encoding.GetBytes(theString);
				this.m_mainStream.Write(array, 0, array.Length);
			}
		}

		internal void WriteStream(object theString)
		{
			if (theString != null)
			{
				this.WriteStream(theString.ToString());
			}
		}

		public virtual void WriteStream(byte[] theBytes)
		{
			this.m_mainStream.Write(theBytes, 0, theBytes.Length);
		}

		protected void WriteStreamCR(string theString)
		{
			this.WriteStream(theString);
		}

		protected void WriteStreamCR(byte[] theBytes)
		{
			this.WriteStream(theBytes);
		}

		protected void WriteStreamEncoded(string theString)
		{
			this.WriteStream(HttpUtility.HtmlEncode(theString));
		}

		protected void WriteAttrEncoded(byte[] attributeName, string theString)
		{
			this.WriteAttribute(attributeName, this.m_encoding.GetBytes(HttpUtility.HtmlAttributeEncode(theString)));
		}

		protected virtual void WriteAttribute(byte[] attributeName, byte[] value)
		{
			this.WriteStream(attributeName);
			this.WriteStream(value);
			this.WriteStream(HTML4Renderer.m_quote);
		}

		protected void WriteAttrEncoded(string theString)
		{
			this.WriteStream(HttpUtility.HtmlAttributeEncode(theString));
		}

		protected void WriteStreamCREncoded(string theString)
		{
			this.WriteStream(HttpUtility.HtmlEncode(theString));
		}

		protected virtual void WriteStreamLineBreak()
		{
		}

		protected void WriteRSStream(float size)
		{
			this.WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
			this.WriteStream(HTML4Renderer.m_mm);
		}

		protected void WriteRSStreamCR(float size)
		{
			this.WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
			this.WriteStreamCR(HTML4Renderer.m_mm);
		}

		protected void WriteDStream(float size)
		{
			this.WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
		}

		private void WriteIdToSecondaryStream(Stream secondaryStream, string tagId)
		{
			Stream mainStream = this.m_mainStream;
			this.m_mainStream = secondaryStream;
			this.WriteReportItemId(tagId);
			this.WriteStream(',');
			this.m_mainStream = mainStream;
		}

		internal static void QuoteString(StringBuilder output, string input)
		{
			if (output != null && input != null && input.Length != 0)
			{
				int i = output.Length;
				output.Append(input);
				for (; i < output.Length; i++)
				{
					if (output[i] == '\\' || output[i] == '"' || output[i] == '\'')
					{
						output.Insert(i, '\\');
						i++;
					}
				}
			}
		}

		protected byte[] RenderSharedStyle(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLStyleProps sharedStyle, RPLItemMeasurement measurement, string id, StyleContext styleContext, ref int borderContext)
		{
			return this.RenderSharedStyle(reportItem, props, definition, sharedStyle, (RPLStyleProps)null, measurement, id, styleContext, ref borderContext);
		}

		protected byte[] RenderSharedStyle(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLStyleProps sharedStyle, RPLStyleProps nonSharedStyle, RPLItemMeasurement measurement, string id, StyleContext styleContext, ref int borderContext)
		{
			Stream mainStream = this.m_mainStream;
			this.m_mainStream = this.m_styleStream;
			this.RenderOpenStyle(id);
			byte omitBordersState = styleContext.OmitBordersState;
			styleContext.OmitBordersState = 0;
			this.RenderStyleProps(reportItem, props, definition, measurement, (IRPLStyle)sharedStyle, (IRPLStyle)nonSharedStyle, styleContext, ref borderContext, false);
			styleContext.OmitBordersState = omitBordersState;
			this.WriteStream(HTML4Renderer.m_closeAccol);
			this.m_mainStream = mainStream;
			byte[] bytes = this.m_encoding.GetBytes(id);
			this.m_usedStyles.Add(id, bytes);
			return bytes;
		}

		protected byte[] RenderSharedStyle(ElementStyleWriter styleWriter, RPLStyleProps sharedStyle, StyleContext styleContext, string id)
		{
			if (sharedStyle != null && id != null)
			{
				Stream mainStream = this.m_mainStream;
				this.m_mainStream = this.m_styleStream;
				this.RenderOpenStyle(id);
				byte omitBordersState = styleContext.OmitBordersState;
				styleContext.OmitBordersState = 0;
				styleWriter.WriteStyles(StyleWriterMode.Shared, sharedStyle);
				styleContext.OmitBordersState = omitBordersState;
				this.WriteStream(HTML4Renderer.m_closeAccol);
				this.m_mainStream = mainStream;
				byte[] bytes = this.m_encoding.GetBytes(id);
				this.m_usedStyles.Add(id, bytes);
				return bytes;
			}
			return null;
		}

		protected void RenderMeasurementStyle(float height, float width)
		{
			this.RenderMeasurementStyle(height, width, false);
		}

		protected void RenderMeasurementStyle(float height, float width, bool renderMin)
		{
			this.RenderMeasurementHeight(height, renderMin);
			this.RenderMeasurementWidth(width, true);
		}

		protected void RenderMeasurementHeight(float height, bool renderMin)
		{
			if (renderMin)
			{
				this.WriteStream(HTML4Renderer.m_styleMinHeight);
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_styleHeight);
			}
			this.WriteRSStream(height);
			this.WriteStream(HTML4Renderer.m_semiColon);
		}

		protected void RenderMeasurementMinHeight(float height)
		{
			this.WriteStream(HTML4Renderer.m_styleMinHeight);
			this.WriteRSStream(height);
			this.WriteStream(HTML4Renderer.m_semiColon);
		}

		protected void RenderMeasurementWidth(float width, bool renderMinWidth)
		{
			this.WriteStream(HTML4Renderer.m_styleWidth);
			this.WriteRSStream(width);
			this.WriteStream(HTML4Renderer.m_semiColon);
			if (renderMinWidth)
			{
				this.RenderMeasurementMinWidth(width);
			}
		}

		protected void RenderMeasurementMinWidth(float minWidth)
		{
			this.WriteStream(HTML4Renderer.m_styleMinWidth);
			this.WriteRSStream(minWidth);
			this.WriteStream(HTML4Renderer.m_semiColon);
		}

		protected void RenderMeasurementHeight(float height)
		{
			this.RenderMeasurementHeight(height, false);
		}

		protected void RenderMeasurementWidth(float width)
		{
			this.RenderMeasurementWidth(width, false);
		}

		private bool ReportPageHasBorder(IRPLStyle style, string backgroundColor)
		{
			bool flag = this.ReportPageBorder(style, Border.All, backgroundColor);
			if (!flag)
			{
				flag = this.ReportPageBorder(style, Border.Left, backgroundColor);
				if (!flag)
				{
					flag = this.ReportPageBorder(style, Border.Right, backgroundColor);
					if (!flag)
					{
						flag = this.ReportPageBorder(style, Border.Bottom, backgroundColor);
						if (!flag)
						{
							flag = this.ReportPageBorder(style, Border.Top, backgroundColor);
						}
					}
				}
			}
			return flag;
		}

		protected virtual void RenderDynamicImageSrc(RPLDynamicImageProps dynamicImageProps)
		{
			string text = null;
			string streamName = dynamicImageProps.StreamName;
			if (streamName != null)
			{
				text = this.m_report.GetStreamUrl(true, streamName);
			}
			if (text != null)
			{
				this.WriteStream(text);
			}
		}

		protected void RenderHtmlBorders(IRPLStyle styleProps, ref int borderContext, byte omitBordersState, bool renderPadding, bool isNonShared, IRPLStyle sharedStyleProps)
		{
			if (renderPadding)
			{
				this.RenderPaddingStyle(styleProps);
			}
			if (styleProps != null && borderContext != 15)
			{
				object obj = styleProps[10];
				object obj2 = styleProps[5];
				object obj3 = styleProps[0];
				IRPLStyle iRPLStyle = styleProps;
				if (isNonShared && sharedStyleProps != null && !this.OnlyGeneralBorder(sharedStyleProps) && !this.OnlyGeneralBorder(styleProps))
				{
					iRPLStyle = new RPLElementStyle(styleProps as RPLStyleProps, sharedStyleProps as RPLStyleProps);
				}
				if (borderContext != 0 || omitBordersState != 0 || !this.OnlyGeneralBorder(iRPLStyle))
				{
					if (obj2 == null || (RPLFormat.BorderStyles)obj2 == RPLFormat.BorderStyles.None)
					{
						this.RenderBorderStyle(obj, obj2, obj3, Border.All);
					}
					if ((borderContext & 8) == 0 && (omitBordersState & 2) == 0 && this.RenderBorderInstance(iRPLStyle, obj, obj2, obj3, Border.Bottom))
					{
						borderContext |= 8;
					}
					if ((borderContext & 1) == 0 && (omitBordersState & 4) == 0 && this.RenderBorderInstance(iRPLStyle, obj, obj2, obj3, Border.Left))
					{
						borderContext |= 1;
					}
					if ((borderContext & 2) == 0 && (omitBordersState & 8) == 0 && this.RenderBorderInstance(iRPLStyle, obj, obj2, obj3, Border.Right))
					{
						borderContext |= 2;
					}
					if ((borderContext & 4) == 0 && (omitBordersState & 1) == 0 && this.RenderBorderInstance(iRPLStyle, obj, obj2, obj3, Border.Top))
					{
						borderContext |= 4;
					}
				}
				else
				{
					if (obj2 != null && (RPLFormat.BorderStyles)obj2 != 0)
					{
						borderContext = 15;
					}
					this.RenderBorderStyle(obj, obj2, obj3, Border.All);
				}
			}
		}

		protected void RenderPaddingStyle(IRPLStyle styleProps)
		{
			if (styleProps != null)
			{
				object obj = styleProps[15];
				if (obj != null)
				{
					this.WriteStream(HTML4Renderer.m_paddingLeft);
					this.WriteStream(obj);
					this.WriteStream(HTML4Renderer.m_semiColon);
				}
				obj = styleProps[17];
				if (obj != null)
				{
					this.WriteStream(HTML4Renderer.m_paddingTop);
					this.WriteStream(obj);
					this.WriteStream(HTML4Renderer.m_semiColon);
				}
				obj = styleProps[16];
				if (obj != null)
				{
					this.WriteStream(HTML4Renderer.m_paddingRight);
					this.WriteStream(obj);
					this.WriteStream(HTML4Renderer.m_semiColon);
				}
				obj = styleProps[18];
				if (obj != null)
				{
					this.WriteStream(HTML4Renderer.m_paddingBottom);
					this.WriteStream(obj);
					this.WriteStream(HTML4Renderer.m_semiColon);
				}
			}
		}

		protected void RenderMultiLineText(string text)
		{
			if (text != null)
			{
				int num = 0;
				int num2 = 0;
				int length = text.Length;
				string text2 = null;
				for (int i = 0; i < length; i++)
				{
					switch (text[i])
					{
					case '\r':
						text2 = text.Substring(num2, num - num2);
						this.WriteStreamEncoded(text2);
						num2 = num + 1;
						break;
					case '\n':
						text2 = text.Substring(num2, num - num2);
						if (!string.IsNullOrEmpty(text2.Trim()))
						{
							this.WriteStreamEncoded(text2);
						}
						this.WriteStreamCR(HTML4Renderer.m_br);
						num2 = num + 1;
						break;
					}
					num++;
				}
				if (num2 == 0)
				{
					this.WriteStreamEncoded(text);
				}
				else
				{
					this.WriteStreamEncoded(text.Substring(num2, num - num2));
				}
			}
		}

		protected bool IsLineSlanted(RPLItemMeasurement measurement)
		{
			if (measurement == null)
			{
				return false;
			}
			if (measurement.Width != 0.0 && measurement.Height != 0.0)
			{
				return true;
			}
			return false;
		}

		protected void RenderCellItem(PageTableCell currCell, int borderContext, bool layoutExpand)
		{
			bool flag = false;
			RPLItemMeasurement rPLItemMeasurement = null;
			rPLItemMeasurement = currCell.Measurement;
			RPLItem element = rPLItemMeasurement.Element;
			if (element != null)
			{
				RPLItemProps rPLItemProps = element.ElementProps as RPLItemProps;
				RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
				flag = this.NeedReportItemId(rPLItemMeasurement.Element, rPLItemProps);
				bool flag2 = false;
				if (rPLItemProps is RPLImageProps)
				{
					RPLImagePropsDef rPLImagePropsDef = (RPLImagePropsDef)rPLItemPropsDef;
					if (rPLImagePropsDef.Sizing == RPLFormat.Sizings.FitProportional)
					{
						flag2 = true;
					}
				}
				if (!flag2 && currCell.ConsumedByEmptyWhiteSpace)
				{
					if (rPLItemProps is RPLImageProps)
					{
						RPLImageProps rPLImageProps = (RPLImageProps)rPLItemProps;
						RPLImagePropsDef rPLImagePropsDef2 = (RPLImagePropsDef)rPLItemProps.Definition;
						if (rPLImageProps != null && !rPLImageProps.Image.ImageConsolidationOffsets.IsEmpty)
						{
							flag2 = true;
						}
					}
					if (!flag2 && rPLItemProps is RPLDynamicImageProps)
					{
						RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)rPLItemProps;
						if (rPLDynamicImageProps != null && !rPLDynamicImageProps.ImageConsolidationOffsets.IsEmpty)
						{
							flag2 = true;
						}
					}
				}
				if (flag2)
				{
					this.WriteStream(HTML4Renderer.m_openDiv);
					this.OpenStyle();
					if (currCell.DXValue > rPLItemMeasurement.Width)
					{
						this.RenderMeasurementWidth(rPLItemMeasurement.Width);
					}
					if (currCell.DYValue > rPLItemMeasurement.Height)
					{
						this.RenderMeasurementHeight(rPLItemMeasurement.Height);
					}
					this.CloseStyle(true);
					this.WriteStream(HTML4Renderer.m_closeBracket);
				}
				this.RenderReportItem(element, rPLItemProps, rPLItemPropsDef, rPLItemMeasurement, new StyleContext(), borderContext, flag);
				if (flag2)
				{
					this.WriteStream(HTML4Renderer.m_closeDiv);
				}
				rPLItemMeasurement.Element = null;
			}
		}

		protected virtual void RenderBlankImage()
		{
			this.WriteStream(HTML4Renderer.m_img);
			if (this.m_browserIE)
			{
				this.WriteStream(HTML4Renderer.m_imgOnError);
			}
			this.WriteStream(HTML4Renderer.m_src);
			this.RenderInternalImageSrc();
			this.WriteStream(this.m_report.GetImageName("Blank.gif"));
			this.WriteStream(HTML4Renderer.m_closeTag);
		}

		protected virtual void RenderImageUrl(bool useSessionId, RPLImageData image)
		{
			string text = this.CreateImageStream(image);
			string text2 = null;
			if (text != null)
			{
				text2 = this.m_report.GetStreamUrl(useSessionId, text);
			}
			if (text2 != null)
			{
				this.WriteStream(text2);
			}
		}

		protected virtual void RenderReportItemId(string repItemId)
		{
			this.WriteStream(HTML4Renderer.m_id);
			this.WriteReportItemId(repItemId);
			this.WriteStream(HTML4Renderer.m_quote);
		}

		private void WriteReportItemId(string repItemId)
		{
			this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
			this.WriteStream(repItemId);
		}

		protected void RenderTextBox(RPLTextBox textBox, RPLTextBoxProps textBoxProps, RPLTextBoxPropsDef textBoxPropsDef, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId)
		{
			string text = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			RPLStyleProps actionStyle = null;
			RPLActionInfo actionInfo = textBoxProps.ActionInfo;
			RPLElementStyle style = textBoxProps.Style;
			bool flag4 = this.CanSort(textBoxPropsDef);
			bool flag5 = this.NeedSharedToggleParent(textBoxProps);
			bool flag6 = false;
			bool isSimple = textBoxPropsDef.IsSimple;
			bool flag7 = !isSimple && flag5;
			bool flag8 = flag4 || flag7;
			bool flag9 = HTML4Renderer.IsDirectionRTL(style);
			RPLStyleProps nonSharedStyle = textBoxProps.NonSharedStyle;
			RPLStyleProps sharedStyle = textBoxPropsDef.SharedStyle;
			bool flag10 = HTML4Renderer.IsWritingModeVertical(style);
			bool flag11 = flag10 && this.m_deviceInfo.IsBrowserIE;
			bool ignoreVerticalAlign = styleContext.IgnoreVerticalAlign;
			if (isSimple)
			{
				text = textBoxProps.Value;
				if (string.IsNullOrEmpty(text))
				{
					text = textBoxPropsDef.Value;
				}
				if (string.IsNullOrEmpty(text) && !flag4 && !flag5)
				{
					flag = true;
				}
			}
			if (textBoxProps.UniqueName == null)
			{
				flag4 = false;
				flag5 = false;
				renderId = false;
			}
			float adjustedWidth = this.GetAdjustedWidth(measurement, textBoxProps.Style);
			float adjustedHeight = this.GetAdjustedHeight(measurement, textBoxProps.Style);
			if (flag)
			{
				styleContext.EmptyTextBox = true;
				this.WriteStream(HTML4Renderer.m_openTable);
				this.RenderReportLanguage();
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_firstTD);
				if (this.m_deviceInfo.IsBrowserGeckoEngine)
				{
					this.WriteStream(HTML4Renderer.m_openDiv);
				}
				this.OpenStyle();
				float width = measurement.Width;
				float height = measurement.Height;
				if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
				{
					width = adjustedWidth;
					height = adjustedHeight;
				}
				this.RenderMeasurementWidth(width, false);
				this.RenderMeasurementMinWidth(adjustedWidth);
				if (!textBoxPropsDef.CanShrink)
				{
					this.RenderMeasurementHeight(height);
				}
			}
			else
			{
				if (flag11)
				{
					this.WriteStream(HTML4Renderer.m_openDiv);
					this.OpenStyle();
					this.RenderDirectionStyles(textBox, textBoxProps, textBoxPropsDef, null, textBoxProps.Style, nonSharedStyle, false, styleContext);
					if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
					{
						this.RenderMeasurementHeight(adjustedHeight);
						this.RenderHtmlBorders((IRPLStyle)textBoxProps.Style, ref borderContext, styleContext.OmitBordersState, true, true, (IRPLStyle)null);
						styleContext.NoBorders = true;
					}
					this.WriteStream("display: inline;");
					bool flag12 = false;
					if (this.m_deviceInfo.BrowserMode == BrowserMode.Standards)
					{
						this.RenderMeasurementHeight(measurement.Height);
						flag12 = true;
					}
					this.CloseStyle(true);
					if (flag12 && this.m_deviceInfo.AllowScript)
					{
						if (!this.m_needsFitVertTextScript)
						{
							this.CreateFitVertTextIdsStream();
						}
						this.WriteIdToSecondaryStream(this.m_fitVertTextIdsStream, textBoxProps.UniqueName + "_fvt");
						this.RenderReportItemId(textBoxProps.UniqueName + "_fvt");
					}
					this.WriteStream(HTML4Renderer.m_closeBracket);
				}
				object obj = style[26];
				if (textBoxPropsDef.CanGrow)
				{
					this.WriteStream(HTML4Renderer.m_openTable);
					this.RenderReportLanguage();
					this.OpenStyle();
					if (flag11)
					{
						if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
						{
							this.RenderMeasurementWidth(adjustedWidth, false);
							if (!textBoxPropsDef.CanShrink)
							{
								this.RenderMeasurementHeight(adjustedHeight);
							}
						}
						else
						{
							this.RenderMeasurementWidth(measurement.Width, true);
						}
					}
					if (isSimple && (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text.Trim())))
					{
						this.WriteStream(HTML4Renderer.m_borderCollapse);
					}
					this.CloseStyle(true);
					this.WriteStream(HTML4Renderer.m_closeBracket);
					this.WriteStream(HTML4Renderer.m_firstTD);
					this.OpenStyle();
					if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
					{
						this.RenderMeasurementWidth(adjustedWidth, false);
					}
					else
					{
						this.RenderMeasurementWidth(measurement.Width, false);
					}
					this.RenderMeasurementMinWidth(adjustedWidth);
					if (!textBoxPropsDef.CanShrink)
					{
						if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode || (this.m_deviceInfo.IsBrowserSafari && this.m_deviceInfo.BrowserMode != BrowserMode.Quirks))
						{
							if (!flag11)
							{
								this.RenderMeasurementHeight(adjustedHeight);
							}
						}
						else
						{
							this.RenderMeasurementHeight(measurement.Height);
						}
					}
					styleContext.RenderMeasurements = false;
					if (flag8)
					{
						styleContext.StyleOnCell = true;
						this.RenderReportItemStyle((RPLElement)textBox, (RPLElementProps)textBoxProps, (RPLElementPropsDef)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxPropsDef.ID + "p");
						styleContext.StyleOnCell = false;
						styleContext.NoBorders = true;
					}
					if (textBoxPropsDef.CanShrink)
					{
						if (flag10 || (flag5 && flag9))
						{
							flag2 = true;
						}
						if (!flag2 && obj != null && !styleContext.IgnoreVerticalAlign)
						{
							obj = EnumStrings.GetValue((RPLFormat.VerticalAlignments)obj);
							this.WriteStream(HTML4Renderer.m_verticalAlign);
							this.WriteStream(obj);
							this.WriteStream(HTML4Renderer.m_semiColon);
						}
						this.CloseStyle(true);
						this.WriteStreamCR(HTML4Renderer.m_closeBracket);
						if (flag2)
						{
							this.WriteStream(HTML4Renderer.m_openTable);
							this.WriteStream(HTML4Renderer.m_inlineWidth);
							this.WriteStream(HTML4Renderer.m_percent);
							this.WriteStream(HTML4Renderer.m_quote);
							this.WriteStream(HTML4Renderer.m_closeBracket);
							this.WriteStream(HTML4Renderer.m_firstTD);
						}
						else
						{
							this.WriteStream(HTML4Renderer.m_openDiv);
							if (!flag8)
							{
								styleContext.IgnoreVerticalAlign = true;
							}
						}
					}
				}
				else
				{
					this.WriteStream(HTML4Renderer.m_openDiv);
					styleContext.IgnoreVerticalAlign = true;
					if (!this.m_deviceInfo.IsBrowserIE || this.m_deviceInfo.BrowserMode == BrowserMode.Standards || (obj != null && (RPLFormat.VerticalAlignments)obj != 0) || this.m_deviceInfo.OutlookCompat)
					{
						if (!flag8)
						{
							bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
							bool noBorders = styleContext.NoBorders;
							styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
							int num = 0;
							if (textBoxPropsDef.CanShrink)
							{
								styleContext.NoBorders = true;
							}
							this.RenderReportItemStyle(textBox, textBoxProps, textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref num, textBoxPropsDef.ID + "v");
							styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
							measurement = null;
							if (textBoxPropsDef.CanShrink)
							{
								styleContext.NoBorders = noBorders;
							}
							else
							{
								styleContext.NoBorders = true;
							}
						}
						this.WriteStreamCR(HTML4Renderer.m_closeBracket);
						styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
						if (obj != null && (RPLFormat.VerticalAlignments)obj != 0)
						{
							this.WriteStream(HTML4Renderer.m_openTable);
							if (!flag4 || flag10)
							{
								this.WriteStream(HTML4Renderer.m_inlineWidth);
								this.WriteStream(HTML4Renderer.m_percent);
								this.WriteStream(HTML4Renderer.m_quote);
							}
							if (!textBoxPropsDef.CanShrink)
							{
								this.WriteStream(HTML4Renderer.m_inlineHeight);
								this.WriteStream(HTML4Renderer.m_percent);
								this.WriteStream(HTML4Renderer.m_quote);
							}
							this.WriteStream(HTML4Renderer.m_zeroBorder);
							this.WriteStream(HTML4Renderer.m_closeBracket);
							this.WriteStream(HTML4Renderer.m_firstTD);
							flag2 = true;
						}
						else
						{
							this.WriteStream(HTML4Renderer.m_openDiv);
							flag3 = true;
						}
					}
					if (flag8)
					{
						this.OpenStyle();
						if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && !textBoxPropsDef.CanShrink)
						{
							this.RenderMeasurementWidth(adjustedWidth, false);
						}
						else
						{
							this.RenderMeasurementWidth(measurement.Width, false);
						}
						this.RenderMeasurementMinWidth(adjustedWidth);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					if (textBoxPropsDef.CanShrink)
					{
						bool noBorders2 = styleContext.NoBorders;
						styleContext.NoBorders = true;
						this.RenderReportItemStyle((RPLElement)textBox, (RPLElementProps)textBoxProps, (RPLElementPropsDef)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxPropsDef.ID + "s");
						this.CloseStyle(true);
						this.WriteStreamCR(HTML4Renderer.m_closeBracket);
						this.WriteStream(HTML4Renderer.m_openDiv);
						styleContext.IgnoreVerticalAlign = true;
						styleContext.NoBorders = noBorders2;
						styleContext.StyleOnCell = true;
					}
					if (flag8)
					{
						this.RenderReportItemStyle((RPLElement)textBox, (RPLElementProps)textBoxProps, (RPLElementPropsDef)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxPropsDef.ID + "p");
						styleContext.StyleOnCell = false;
					}
				}
			}
			if (flag8)
			{
				styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
				this.CloseStyle(true);
				this.WriteStreamCR(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_openTable);
				this.WriteStream(HTML4Renderer.m_zeroBorder);
				this.RenderReportLanguage();
				styleContext.RenderMeasurements = false;
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_firstTD);
				if (flag10)
				{
					this.WriteStream(" ROWS='2'");
				}
				this.RenderAtStart(textBoxProps, style, flag4 && flag9, flag7 && !flag9);
				styleContext.InTablix = true;
			}
			string textBoxClass = this.GetTextBoxClass(textBoxPropsDef, textBoxProps, nonSharedStyle, textBoxPropsDef.ID);
			this.RenderReportItemStyle((RPLElement)textBox, (RPLElementProps)textBoxProps, (RPLElementPropsDef)textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref borderContext, textBoxClass);
			this.CloseStyle(true);
			styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
			if (renderId || flag5 || flag4)
			{
				this.RenderReportItemId(textBoxProps.UniqueName);
			}
			this.WriteToolTip(textBoxProps);
			if (!flag)
			{
				string language = (string)style[32];
				this.RenderLanguage(language);
			}
			this.WriteStreamCR(HTML4Renderer.m_closeBracket);
			if (renderId)
			{
				this.WriteStream(HTML4Renderer.m_openA);
				this.WriteStream(HTML4Renderer.m_name);
				this.WriteStream(textBoxProps.UniqueName);
				this.WriteStream(HTML4Renderer.m_closeTag);
			}
			if ((!this.m_deviceInfo.IsBrowserIE || (this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && !flag10)) && isSimple && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
			{
				this.WriteStream(HTML4Renderer.m_openDiv);
				if (measurement != null)
				{
					this.OpenStyle();
					float num2 = this.GetInnerContainerWidth(measurement, textBoxProps.Style);
					if (flag4 && !flag9)
					{
						num2 = (float)(num2 - 4.2333331108093262);
					}
					if (num2 > 0.0)
					{
						this.WriteStream(HTML4Renderer.m_styleWidth);
						this.WriteRSStream(num2);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					this.CloseStyle(true);
				}
				this.WriteStream(HTML4Renderer.m_closeBracket);
			}
			if (flag5 && isSimple)
			{
				this.RenderToggleImage(textBoxProps);
			}
			RPLAction rPLAction = null;
			if (this.HasAction(actionInfo))
			{
				rPLAction = actionInfo.Actions[0];
				this.RenderElementHyperlinkAllTextStyles(textBoxProps.Style, rPLAction, textBoxPropsDef.ID + "a");
				flag6 = true;
				if (flag)
				{
					this.WriteStream(HTML4Renderer.m_openDiv);
					this.OpenStyle();
					float num3 = 0f;
					if (measurement != null)
					{
						num3 = measurement.Height;
					}
					if (num3 > 0.0)
					{
						num3 = this.GetInnerContainerHeightSubtractBorders(measurement, textBoxProps.Style);
						if (this.m_deviceInfo.IsBrowserIE && this.m_deviceInfo.BrowserMode == BrowserMode.Quirks)
						{
							this.RenderMeasurementHeight(num3);
						}
						else
						{
							this.RenderMeasurementMinHeight(num3);
						}
					}
					this.WriteStream(HTML4Renderer.m_cursorHand);
					this.WriteStream(HTML4Renderer.m_semiColon);
					this.CloseStyle(true);
					this.WriteStream(HTML4Renderer.m_closeBracket);
				}
			}
			this.RenderTextBoxContent(textBox, textBoxProps, textBoxPropsDef, text, actionStyle, flag5 || flag4, measurement, rPLAction);
			if (flag6)
			{
				if (flag)
				{
					this.WriteStream(HTML4Renderer.m_closeDiv);
				}
				this.WriteStream(HTML4Renderer.m_closeA);
			}
			if ((!this.m_deviceInfo.IsBrowserIE || (this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && !flag10)) && isSimple && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
			{
				this.WriteStream(HTML4Renderer.m_closeDiv);
			}
			if (flag8)
			{
				this.RenderAtEnd(textBoxProps, style, flag4 && !flag9, flag7 && flag9);
				this.WriteStream(HTML4Renderer.m_lastTD);
				this.WriteStream(HTML4Renderer.m_closeTable);
			}
			if (flag)
			{
				if (this.m_deviceInfo.IsBrowserGeckoEngine)
				{
					this.WriteStream(HTML4Renderer.m_closeDiv);
				}
				this.WriteStream(HTML4Renderer.m_lastTD);
				this.WriteStream(HTML4Renderer.m_closeTable);
			}
			else
			{
				if (textBoxPropsDef.CanGrow)
				{
					if (textBoxPropsDef.CanShrink)
					{
						if (flag2)
						{
							this.WriteStream(HTML4Renderer.m_lastTD);
							this.WriteStream(HTML4Renderer.m_closeTable);
						}
						else
						{
							this.WriteStream(HTML4Renderer.m_closeDiv);
						}
					}
					this.WriteStream(HTML4Renderer.m_lastTD);
					this.WriteStreamCR(HTML4Renderer.m_closeTable);
				}
				else
				{
					if (flag2)
					{
						this.WriteStream(HTML4Renderer.m_lastTD);
						this.WriteStream(HTML4Renderer.m_closeTable);
					}
					if (flag3)
					{
						this.WriteStream(HTML4Renderer.m_closeDiv);
					}
					this.WriteStreamCR(HTML4Renderer.m_closeDiv);
				}
				if (flag11)
				{
					this.WriteStream(HTML4Renderer.m_closeDiv);
				}
			}
		}

		private string GetTextBoxClass(RPLTextBoxPropsDef textBoxPropsDef, RPLTextBoxProps textBoxProps, RPLStyleProps nonSharedStyle, string defaultClass)
		{
			if (textBoxPropsDef.SharedTypeCode == TypeCode.Object && (nonSharedStyle == null || nonSharedStyle.Count == 0 || nonSharedStyle[25] == null))
			{
				object obj = textBoxProps.Style[25];
				if (obj != null && (RPLFormat.TextAlignments)obj == RPLFormat.TextAlignments.General)
				{
					if (HTML4Renderer.GetTextAlignForType(textBoxProps))
					{
						return defaultClass + "r";
					}
					return defaultClass + "l";
				}
			}
			return defaultClass;
		}

		private void WriteToolTip(RPLElementProps props)
		{
			RPLItemProps rPLItemProps = props as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			string toolTip = rPLItemProps.ToolTip;
			if (toolTip == null)
			{
				toolTip = rPLItemPropsDef.ToolTip;
			}
			if (toolTip != null)
			{
				this.WriteToolTipAttribute(toolTip);
			}
		}

		private void WriteToolTipAttribute(string tooltip)
		{
			this.WriteAttrEncoded(HTML4Renderer.m_alt, tooltip);
			this.WriteAttrEncoded(HTML4Renderer.m_title, tooltip);
		}

		private void WriteOuterConsolidation(System.Drawing.Rectangle consolidationOffsets, RPLFormat.Sizings sizing, string propsUniqueName)
		{
			bool flag = false;
			switch (sizing)
			{
			case RPLFormat.Sizings.Fit:
				this.WriteStream(" imgConDiv=\"true\"");
				this.m_emitImageConsolidationScaling = true;
				flag = true;
				break;
			case RPLFormat.Sizings.FitProportional:
				this.WriteStream(" imgConFitProp=\"true\"");
				break;
			}
			if (this.m_deviceInfo.AllowScript)
			{
				if (this.m_imgConImageIdsStream == null)
				{
					this.CreateImgConImageIdsStream();
				}
				this.WriteIdToSecondaryStream(this.m_imgConImageIdsStream, propsUniqueName + "_ici");
				this.RenderReportItemId(propsUniqueName + "_ici");
			}
			this.WriteStream(" imgConImage=\"" + sizing.ToString() + "\"");
			if (flag)
			{
				this.WriteStream(" imgConWidth=\"" + consolidationOffsets.Width + "\"");
				this.WriteStream(" imgConHeight=\"" + consolidationOffsets.Height + "\"");
			}
			this.OpenStyle();
			this.WriteStream(HTML4Renderer.m_styleWidth);
			if (flag)
			{
				this.WriteStream("1");
			}
			else
			{
				this.WriteStream(consolidationOffsets.Width);
			}
			this.WriteStream(HTML4Renderer.m_px);
			this.WriteStream(HTML4Renderer.m_semiColon);
			this.WriteStream(HTML4Renderer.m_styleHeight);
			if (flag)
			{
				this.WriteStream("1");
			}
			else
			{
				this.WriteStream(consolidationOffsets.Height);
			}
			this.WriteStream(HTML4Renderer.m_px);
			this.WriteStream(HTML4Renderer.m_semiColon);
			this.WriteStream(HTML4Renderer.m_overflowHidden);
			this.WriteStream(HTML4Renderer.m_semiColon);
			if (this.m_deviceInfo.BrowserMode == BrowserMode.Standards)
			{
				this.WriteStream(HTML4Renderer.m_stylePositionAbsolute);
			}
		}

		private void WriteClippedDiv(System.Drawing.Rectangle clipCoordinates)
		{
			this.OpenStyle();
			this.WriteStream(HTML4Renderer.m_styleTop);
			if (clipCoordinates.Top > 0)
			{
				this.WriteStream("-");
			}
			this.WriteStream(clipCoordinates.Top);
			this.WriteStream(HTML4Renderer.m_px);
			this.WriteStream(HTML4Renderer.m_semiColon);
			this.WriteStream(HTML4Renderer.m_styleLeft);
			if (clipCoordinates.Left > 0)
			{
				this.WriteStream("-");
			}
			this.WriteStream(clipCoordinates.Left);
			this.WriteStream(HTML4Renderer.m_px);
			this.WriteStream(HTML4Renderer.m_semiColon);
			this.WriteStream(HTML4Renderer.m_stylePositionRelative);
			this.CloseStyle(true);
		}

		protected void RenderNavigationId(string navigationId)
		{
			if (!this.IsFragment)
			{
				this.WriteStream(HTML4Renderer.m_openSpan);
				this.WriteStream(HTML4Renderer.m_id);
				this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
				this.WriteStream(navigationId);
				this.WriteStream(HTML4Renderer.m_closeTag);
			}
		}

		protected void RenderTablix(RPLTablix tablix, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId)
		{
			string uniqueName = props.UniqueName;
			TablixFixedHeaderStorage tablixFixedHeaderStorage = new TablixFixedHeaderStorage();
			if (tablix.ColumnWidths == null)
			{
				tablix.ColumnWidths = new float[0];
			}
			if (tablix.RowHeights == null)
			{
				tablix.RowHeights = new float[0];
			}
			bool flag = this.InitFixedColumnHeaders(tablix, uniqueName, tablixFixedHeaderStorage);
			bool flag2 = this.InitFixedRowHeaders(tablix, uniqueName, tablixFixedHeaderStorage);
			bool flag3 = tablix.ColumnHeaderRows == 0 && tablix.RowHeaderColumns == 0 && !this.m_deviceInfo.AccessibleTablix && this.m_deviceInfo.BrowserMode != BrowserMode.Standards;
			if (flag && flag2)
			{
				tablixFixedHeaderStorage.CornerHeaders = new List<string>();
			}
			this.WriteStream(HTML4Renderer.m_openTable);
			int columns = (tablix.ColumnHeaderRows > 0 || tablix.RowHeaderColumns > 0 || !flag3) ? (tablix.ColumnWidths.Length + 1) : tablix.ColumnWidths.Length;
			this.WriteStream(HTML4Renderer.m_cols);
			this.WriteStream(columns.ToString(CultureInfo.InvariantCulture));
			this.WriteStream(HTML4Renderer.m_quote);
			if (renderId || flag || flag2)
			{
				this.RenderReportItemId(uniqueName);
			}
			this.WriteToolTip(tablix.ElementProps);
			this.WriteStream(HTML4Renderer.m_zeroBorder);
			this.OpenStyle();
			this.WriteStream(HTML4Renderer.m_borderCollapse);
			this.WriteStream(HTML4Renderer.m_semiColon);
			if (this.m_deviceInfo.OutlookCompat && measurement != null)
			{
				this.RenderMeasurementWidth(measurement.Width, true);
			}
			this.RenderReportItemStyle((RPLElement)tablix, props, def, measurement, styleContext, ref borderContext, def.ID);
			this.CloseStyle(true);
			this.WriteStream(HTML4Renderer.m_closeBracket);
			int colsBeforeRowHeaders = tablix.ColsBeforeRowHeaders;
			RPLTablixRow nextRow = tablix.GetNextRow();
			List<RPLTablixOmittedRow> list = new List<RPLTablixOmittedRow>();
			while (nextRow != null && nextRow is RPLTablixOmittedRow)
			{
				list.Add((RPLTablixOmittedRow)nextRow);
				nextRow = tablix.GetNextRow();
			}
			if (flag3)
			{
				this.RenderEmptyTopTablixRow(tablix, list, uniqueName, false, tablixFixedHeaderStorage);
				this.RenderSimpleTablixRows(tablix, uniqueName, nextRow, borderContext, tablixFixedHeaderStorage);
			}
			else
			{
				styleContext = new StyleContext();
				float[] columnWidths = tablix.ColumnWidths;
				float[] rowHeights = tablix.RowHeights;
				int num = columnWidths.Length;
				int numRows = rowHeights.Length;
				this.RenderEmptyTopTablixRow(tablix, list, uniqueName, true, tablixFixedHeaderStorage);
				bool flag4 = flag;
				int num2 = 0;
				list = new List<RPLTablixOmittedRow>();
				HTMLHeader[] array = null;
				string[] array2 = null;
				OmittedHeaderStack omittedHeaders = null;
				if (this.m_deviceInfo.AccessibleTablix)
				{
					array = new HTMLHeader[tablix.RowHeaderColumns];
					array2 = new string[num];
					omittedHeaders = new OmittedHeaderStack();
				}
				while (nextRow != null)
				{
					if (nextRow is RPLTablixOmittedRow)
					{
						list.Add((RPLTablixOmittedRow)nextRow);
						nextRow = tablix.GetNextRow();
						continue;
					}
					if (rowHeights[num2] == 0.0 && num2 > 1 && nextRow.NumCells == 1 && nextRow[0].Element is RPLRectangle)
					{
						RPLRectangle rPLRectangle = (RPLRectangle)nextRow[0].Element;
						if (rPLRectangle.Children != null && rPLRectangle.Children.Length != 0)
						{
							goto IL_02da;
						}
						nextRow = tablix.GetNextRow();
						num2++;
						continue;
					}
					goto IL_02da;
					IL_02da:
					this.WriteStream(HTML4Renderer.m_openTR);
					if (tablix.FixedRow(num2) || flag2 || flag4)
					{
						string text = uniqueName + "r" + num2;
						this.RenderReportItemId(text);
						if (tablix.FixedRow(num2))
						{
							tablixFixedHeaderStorage.ColumnHeaders.Add(text);
							if (tablixFixedHeaderStorage.CornerHeaders != null)
							{
								tablixFixedHeaderStorage.CornerHeaders.Add(text);
							}
						}
						else if (flag4)
						{
							tablixFixedHeaderStorage.BodyID = text;
							flag4 = false;
						}
						if (flag2)
						{
							tablixFixedHeaderStorage.RowHeaders.Add(text);
						}
					}
					this.WriteStream(HTML4Renderer.m_valign);
					this.WriteStream(HTML4Renderer.m_topValue);
					this.WriteStream(HTML4Renderer.m_quote);
					this.WriteStream(HTML4Renderer.m_closeBracket);
					this.RenderEmptyHeightCell(rowHeights[num2], uniqueName, tablix.FixedRow(num2), num2, tablixFixedHeaderStorage);
					int num3 = 0;
					int numCells = nextRow.NumCells;
					int num4 = numCells;
					if (nextRow.BodyStart == -1)
					{
						int[] omittedIndices = new int[list.Count];
						for (int i = num3; i < num4; i++)
						{
							RPLTablixCell rPLTablixCell = nextRow[i];
							this.RenderColumnHeaderTablixCell(tablix, uniqueName, num, rPLTablixCell.ColIndex, rPLTablixCell.ColSpan, num2, borderContext, rPLTablixCell, styleContext, tablixFixedHeaderStorage, list, omittedIndices);
							if (array2 != null && num2 < tablix.ColumnHeaderRows)
							{
								string text2 = null;
								if (rPLTablixCell is RPLTablixMemberCell)
								{
									text2 = ((RPLTablixMemberCell)rPLTablixCell).UniqueName;
									if (text2 == null && rPLTablixCell.Element != null)
									{
										text2 = rPLTablixCell.Element.ElementProps.UniqueName;
										((RPLTablixMemberCell)rPLTablixCell).UniqueName = text2;
									}
									if (text2 != null)
									{
										for (int j = 0; j < rPLTablixCell.ColSpan; j++)
										{
											string text3 = array2[rPLTablixCell.ColIndex + j];
											text3 = ((text3 != null) ? (text3 + " " + HttpUtility.HtmlAttributeEncode(this.m_deviceInfo.HtmlPrefixId) + text2) : (HttpUtility.HtmlAttributeEncode(this.m_deviceInfo.HtmlPrefixId) + text2));
											array2[rPLTablixCell.ColIndex + j] = text3;
										}
										goto IL_04dd;
									}
									continue;
								}
							}
							goto IL_04dd;
							IL_04dd:
							nextRow[i] = null;
						}
						list = new List<RPLTablixOmittedRow>();
					}
					else
					{
						if (array != null)
						{
							int headerStart = nextRow.HeaderStart;
							int num5 = 0;
							for (int k = 0; k < array.Length; k++)
							{
								HTMLHeader hTMLHeader = array[k];
								if (array[k] != null)
								{
									if (array[k].Span > 1)
									{
										array[k].Span--;
										continue;
									}
								}
								else
								{
									hTMLHeader = (array[k] = new HTMLHeader());
								}
								RPLTablixCell rPLTablixCell2 = nextRow[num5 + headerStart];
								hTMLHeader.ID = this.CalculateRowHeaderId(rPLTablixCell2, tablix.FixedColumns[rPLTablixCell2.ColIndex], uniqueName, num2, k + tablix.ColsBeforeRowHeaders, null, this.m_deviceInfo.AccessibleTablix, false);
								hTMLHeader.Span = rPLTablixCell2.RowSpan;
								num5++;
							}
						}
						if (list != null && list.Count > 0)
						{
							for (int l = 0; l < list.Count; l++)
							{
								this.RenderTablixOmittedRow(columns, list[l]);
							}
							list = null;
						}
						List<RPLTablixMemberCell> omittedHeaders2 = nextRow.OmittedHeaders;
						if (colsBeforeRowHeaders > 0)
						{
							int num6 = 0;
							int headerStart2 = nextRow.HeaderStart;
							int bodyStart = nextRow.BodyStart;
							int m = headerStart2;
							int n = bodyStart;
							int num7 = 0;
							for (; n < num4; n++)
							{
								if (num7 >= colsBeforeRowHeaders)
								{
									break;
								}
								RPLTablixCell rPLTablixCell3 = nextRow[n];
								int colSpan = rPLTablixCell3.ColSpan;
								this.RenderTablixCell(tablix, false, uniqueName, num, numRows, num7, colSpan, num2, borderContext, rPLTablixCell3, omittedHeaders2, ref num6, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								num7 += colSpan;
								nextRow[n] = null;
							}
							num4 = ((bodyStart > headerStart2) ? bodyStart : num4);
							if (m >= 0)
							{
								for (; m < num4; m++)
								{
									RPLTablixCell rPLTablixCell4 = nextRow[m];
									int colSpan2 = rPLTablixCell4.ColSpan;
									this.RenderTablixCell(tablix, flag2, uniqueName, num, numRows, num7, colSpan2, num2, borderContext, rPLTablixCell4, omittedHeaders2, ref num6, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
									num7 += colSpan2;
									nextRow[m] = null;
								}
							}
							num3 = n;
							num4 = ((bodyStart < headerStart2) ? headerStart2 : numCells);
							for (int num8 = num3; num8 < num4; num8++)
							{
								RPLTablixCell rPLTablixCell5 = nextRow[num8];
								this.RenderTablixCell(tablix, false, uniqueName, num, numRows, rPLTablixCell5.ColIndex, rPLTablixCell5.ColSpan, num2, borderContext, rPLTablixCell5, omittedHeaders2, ref num6, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								nextRow[num8] = null;
							}
						}
						else
						{
							int num9 = 0;
							for (int num10 = num3; num10 < num4; num10++)
							{
								RPLTablixCell rPLTablixCell6 = nextRow[num10];
								int colIndex = rPLTablixCell6.ColIndex;
								this.RenderTablixCell(tablix, tablix.FixedColumns[colIndex], uniqueName, num, numRows, colIndex, rPLTablixCell6.ColSpan, num2, borderContext, rPLTablixCell6, omittedHeaders2, ref num9, styleContext, tablixFixedHeaderStorage, array, array2, omittedHeaders);
								nextRow[num10] = null;
							}
						}
					}
					this.WriteStream(HTML4Renderer.m_closeTR);
					nextRow = tablix.GetNextRow();
					num2++;
				}
			}
			this.WriteStream(HTML4Renderer.m_closeTable);
			if (!flag && !flag2)
			{
				return;
			}
			if (this.m_fixedHeaders == null)
			{
				this.m_fixedHeaders = new ArrayList();
			}
			this.m_fixedHeaders.Add(tablixFixedHeaderStorage);
		}

		private void RenderTablixOmittedRow(int columns, RPLTablixRow currentRow)
		{
			int i = 0;
			List<RPLTablixMemberCell> omittedHeaders;
			for (omittedHeaders = currentRow.OmittedHeaders; i < omittedHeaders.Count && omittedHeaders[i].GroupLabel == null; i++)
			{
			}
			if (i < omittedHeaders.Count)
			{
				int num = omittedHeaders[i].ColIndex;
				this.WriteStream(HTML4Renderer.m_openTR);
				this.WriteStream(HTML4Renderer.m_zeroHeight);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_openTD);
				this.WriteStream(HTML4Renderer.m_colSpan);
				this.WriteStream(num.ToString(CultureInfo.InvariantCulture));
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_closeTD);
				for (; i < omittedHeaders.Count; i++)
				{
					if (omittedHeaders[i].GroupLabel != null)
					{
						this.WriteStream(HTML4Renderer.m_openTD);
						int colIndex = omittedHeaders[i].ColIndex;
						int num2 = colIndex - num;
						if (num2 > 1)
						{
							this.WriteStream(HTML4Renderer.m_colSpan);
							this.WriteStream(num2.ToString(CultureInfo.InvariantCulture));
							this.WriteStream(HTML4Renderer.m_quote);
							this.WriteStream(HTML4Renderer.m_closeBracket);
							this.WriteStream(HTML4Renderer.m_closeTD);
							this.WriteStream(HTML4Renderer.m_openTD);
						}
						int colSpan = omittedHeaders[i].ColSpan;
						if (colSpan > 1)
						{
							this.WriteStream(HTML4Renderer.m_colSpan);
							this.WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
							this.WriteStream(HTML4Renderer.m_quote);
						}
						this.RenderReportItemId(omittedHeaders[i].UniqueName);
						this.WriteStream(HTML4Renderer.m_closeBracket);
						this.WriteStream(HTML4Renderer.m_closeTD);
						num = colIndex + colSpan;
					}
				}
				if (num < columns)
				{
					this.WriteStream(HTML4Renderer.m_openTD);
					this.WriteStream(HTML4Renderer.m_colSpan);
					this.WriteStream((columns - num).ToString(CultureInfo.InvariantCulture));
					this.WriteStream(HTML4Renderer.m_quote);
					this.WriteStream(HTML4Renderer.m_closeBracket);
					this.WriteStream(HTML4Renderer.m_closeTD);
				}
				this.WriteStream(HTML4Renderer.m_closeTR);
			}
		}

		protected void RenderSimpleTablixRows(RPLTablix tablix, string tablixID, RPLTablixRow currentRow, int borderContext, TablixFixedHeaderStorage headerStorage)
		{
			int num = 0;
			StyleContext styleContext = new StyleContext();
			float[] rowHeights = tablix.RowHeights;
			int num2 = tablix.ColumnWidths.Length;
			int num3 = rowHeights.Length;
			bool flag = headerStorage.ColumnHeaders != null;
			SharedListLayoutState sharedListLayoutState = SharedListLayoutState.None;
			while (currentRow != null)
			{
				List<RPLTablixMemberCell> omittedHeaders = currentRow.OmittedHeaders;
				int num4 = 0;
				if (num2 == 1)
				{
					sharedListLayoutState = SharedListLayoutState.None;
					bool flag2 = tablix.SharedLayoutRow(num);
					bool flag3 = tablix.UseSharedLayoutRow(num);
					bool flag4 = tablix.RowsState.Length > num + 1 && tablix.UseSharedLayoutRow(num + 1);
					if (flag2 && flag4)
					{
						sharedListLayoutState = SharedListLayoutState.Start;
					}
					else if (flag3)
					{
						sharedListLayoutState = (SharedListLayoutState)((!flag4) ? 3 : 2);
					}
				}
				if (sharedListLayoutState != 0 && sharedListLayoutState != SharedListLayoutState.Start)
				{
					goto IL_01bd;
				}
				if (rowHeights[num] == 0.0 && num > 1 && currentRow.NumCells == 1 && currentRow[0].Element is RPLRectangle)
				{
					RPLRectangle rPLRectangle = (RPLRectangle)currentRow[0].Element;
					if (rPLRectangle.Children != null && rPLRectangle.Children.Length != 0)
					{
						goto IL_00fe;
					}
					currentRow = tablix.GetNextRow();
					num++;
					continue;
				}
				goto IL_00fe;
				IL_00fe:
				this.WriteStream(HTML4Renderer.m_openTR);
				if (tablix.FixedRow(num) || headerStorage.RowHeaders != null || flag)
				{
					string text = tablixID + "tr" + num;
					this.RenderReportItemId(text);
					if (tablix.FixedRow(num))
					{
						headerStorage.ColumnHeaders.Add(text);
						if (headerStorage.CornerHeaders != null)
						{
							headerStorage.CornerHeaders.Add(text);
						}
					}
					else if (flag)
					{
						headerStorage.BodyID = text;
						flag = false;
					}
					if (headerStorage.RowHeaders != null)
					{
						headerStorage.RowHeaders.Add(text);
					}
				}
				this.WriteStream(HTML4Renderer.m_valign);
				this.WriteStream(HTML4Renderer.m_topValue);
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				goto IL_01bd;
				IL_01bd:
				int numCells = currentRow.NumCells;
				bool firstRow = num == 0;
				bool lastRow = num == num3 - 1;
				RPLTablixCell rPLTablixCell = currentRow[0];
				currentRow[0] = null;
				if (sharedListLayoutState != 0)
				{
					this.RenderListReportItem(tablix, rPLTablixCell, omittedHeaders, borderContext, styleContext, firstRow, lastRow, sharedListLayoutState, rPLTablixCell.Element);
				}
				else
				{
					this.RenderSimpleTablixCellWithHeight(rowHeights[num], tablix, tablixID, num2, num, borderContext, rPLTablixCell, omittedHeaders, ref num4, styleContext, firstRow, lastRow, headerStorage);
				}
				int i;
				for (i = 1; i < numCells - 1; i++)
				{
					rPLTablixCell = currentRow[i];
					this.RenderSimpleTablixCell(tablix, tablixID, rPLTablixCell.ColSpan, num, borderContext, rPLTablixCell, omittedHeaders, ref num4, false, firstRow, lastRow, headerStorage);
					currentRow[i] = null;
				}
				if (numCells > 1)
				{
					rPLTablixCell = currentRow[i];
					this.RenderSimpleTablixCell(tablix, tablixID, rPLTablixCell.ColSpan, num, borderContext, rPLTablixCell, omittedHeaders, ref num4, true, firstRow, lastRow, headerStorage);
					currentRow[i] = null;
				}
				if (sharedListLayoutState == SharedListLayoutState.None || sharedListLayoutState == SharedListLayoutState.End)
				{
					this.WriteStream(HTML4Renderer.m_closeTR);
				}
				currentRow = tablix.GetNextRow();
				num++;
			}
		}

		private void RenderSimpleTablixCellWithHeight(float height, RPLTablix tablix, string tablixID, int numCols, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, StyleContext styleContext, bool firstRow, bool lastRow, TablixFixedHeaderStorage headerStorage)
		{
			int colIndex = cell.ColIndex;
			int num = cell.ColSpan;
			bool lastCol = colIndex + num == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(colIndex, num);
			styleContext.ZeroWidth = (columnWidth == 0.0);
			int startIndex = this.RenderZeroWidthTDsForTablix(colIndex, num, tablix);
			num = this.GetColSpanMinusZeroWidthColumns(colIndex, num, tablix);
			this.WriteStream(HTML4Renderer.m_openTD);
			this.RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
			if (num > 1)
			{
				this.WriteStream(HTML4Renderer.m_colSpan);
				this.WriteStream(num.ToString(CultureInfo.InvariantCulture));
				this.WriteStream(HTML4Renderer.m_quote);
			}
			this.OpenStyle();
			this.WriteStream(HTML4Renderer.m_styleHeight);
			this.WriteDStream(height);
			this.WriteStream(HTML4Renderer.m_mm);
			RPLElement element = cell.Element;
			if (element != null)
			{
				this.WriteStream(HTML4Renderer.m_semiColon);
				int num2 = 0;
				this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, true, lastCol, firstRow, lastRow, element, ref num2);
				this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				this.RenderTablixReportItem(tablix, tablixContext, cell, styleContext, true, lastCol, firstRow, lastRow, element, ref num2);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					this.WriteStream(HTML4Renderer.m_displayNone);
				}
				this.CloseStyle(true);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				this.WriteStream(HTML4Renderer.m_nbsp);
			}
			this.WriteStream(HTML4Renderer.m_closeTD);
			this.RenderZeroWidthTDsForTablix(startIndex, num, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private void RenderTablixReportItemStyle(RPLTablix tablix, int tablixContext, RPLTablixCell cell, StyleContext styleContext, bool firstCol, bool lastCol, bool firstRow, bool lastRow, RPLElement cellItem, ref int borderContext)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLTextBox rPLTextBox = cellItem as RPLTextBox;
			RPLTextBoxProps rPLTextBoxProps = (rPLTextBox != null) ? (rPLTextBox.ElementProps as RPLTextBoxProps) : null;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (rPLTextBox != null) ? (elementProps.Definition as RPLTextBoxPropsDef) : null;
			styleContext.OmitBordersState = cell.ElementState;
			if (!(cellItem is RPLLine))
			{
				styleContext.StyleOnCell = true;
				borderContext = HTML4Renderer.GetNewContext(tablixContext, firstCol, lastCol, firstRow, lastRow);
				if (rPLTextBox != null)
				{
					bool ignorePadding = styleContext.IgnorePadding;
					styleContext.IgnorePadding = true;
					RPLItemMeasurement rPLItemMeasurement = null;
					if (this.m_deviceInfo.OutlookCompat || !this.m_deviceInfo.IsBrowserIE)
					{
						rPLItemMeasurement = new RPLItemMeasurement();
						rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					}
					styleContext.EmptyTextBox = (rPLTextBoxPropsDef.IsSimple && string.IsNullOrEmpty(rPLTextBoxProps.Value) && string.IsNullOrEmpty(rPLTextBoxPropsDef.Value) && !this.NeedSharedToggleParent(rPLTextBoxProps) && !this.CanSort(rPLTextBoxPropsDef));
					string textBoxClass = this.GetTextBoxClass(rPLTextBoxPropsDef, rPLTextBoxProps, rPLTextBoxProps.NonSharedStyle, definition.ID + "c");
					bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
					if (HTML4Renderer.IsWritingModeVertical(rPLTextBoxProps.Style) && this.m_deviceInfo.IsBrowserIE && (rPLTextBoxPropsDef.CanGrow || (this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)))
					{
						styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
					}
					this.RenderReportItemStyle(cellItem, elementProps, definition, rPLTextBoxProps.NonSharedStyle, rPLTextBoxPropsDef.SharedStyle, rPLItemMeasurement, styleContext, ref borderContext, textBoxClass);
					styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
					styleContext.IgnorePadding = ignorePadding;
				}
				else
				{
					this.RenderReportItemStyle(cellItem, elementProps, definition, (RPLItemMeasurement)null, styleContext, ref borderContext, definition.ID + "c");
				}
				styleContext.StyleOnCell = false;
			}
			else if (styleContext.ZeroWidth)
			{
				this.WriteStream(HTML4Renderer.m_displayNone);
			}
			this.CloseStyle(true);
			if (styleContext.EmptyTextBox && rPLTextBox != null && elementProps != null)
			{
				this.WriteToolTip(elementProps);
			}
			this.WriteStream(HTML4Renderer.m_closeBracket);
		}

		private void RenderTablixReportItem(RPLTablix tablix, int tablixContext, RPLTablixCell cell, StyleContext styleContext, bool firstCol, bool lastCol, bool firstRow, bool lastRow, RPLElement cellItem, ref int borderContext)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLTextBox rPLTextBox = cellItem as RPLTextBox;
			RPLTextBoxProps rPLTextBoxProps = (rPLTextBox != null) ? (rPLTextBox.ElementProps as RPLTextBoxProps) : null;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (rPLTextBox != null) ? (elementProps.Definition as RPLTextBoxPropsDef) : null;
			RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
			styleContext.OmitBordersState = cell.ElementState;
			if (styleContext.EmptyTextBox)
			{
				bool flag = false;
				RPLActionInfo actionInfo = rPLTextBoxProps.ActionInfo;
				if (this.HasAction(actionInfo))
				{
					this.RenderElementHyperlinkAllTextStyles(rPLTextBoxProps.Style, actionInfo.Actions[0], rPLTextBoxPropsDef.ID + "a");
					this.WriteStream(HTML4Renderer.m_openDiv);
					this.OpenStyle();
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					rPLItemMeasurement.Height = this.GetInnerContainerHeightSubtractBorders(rPLItemMeasurement, rPLTextBoxProps.Style);
					if (this.m_deviceInfo.BrowserMode == BrowserMode.Quirks && this.m_deviceInfo.IsBrowserIE)
					{
						this.RenderMeasurementHeight(rPLItemMeasurement.Height);
					}
					else
					{
						this.RenderMeasurementMinHeight(rPLItemMeasurement.Height);
					}
					this.WriteStream(HTML4Renderer.m_semiColon);
					this.WriteStream(HTML4Renderer.m_cursorHand);
					this.WriteStream(HTML4Renderer.m_semiColon);
					this.CloseStyle(true);
					this.WriteStream(HTML4Renderer.m_closeBracket);
					flag = true;
				}
				this.WriteStream(HTML4Renderer.m_nbsp);
				if (flag)
				{
					this.WriteStream(HTML4Renderer.m_closeDiv);
					this.WriteStream(HTML4Renderer.m_closeA);
				}
			}
			else
			{
				styleContext.InTablix = true;
				bool renderId = this.NeedReportItemId(cellItem, elementProps);
				if (rPLTextBox != null)
				{
					styleContext.RenderMeasurements = false;
					rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					this.RenderTextBoxPercent(rPLTextBox, rPLTextBoxProps, rPLTextBoxPropsDef, rPLItemMeasurement, styleContext, renderId);
				}
				else
				{
					rPLItemMeasurement.Width = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
					rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
					if (cellItem is RPLRectangle || cellItem is RPLSubReport || cellItem is RPLLine)
					{
						styleContext.RenderMeasurements = false;
					}
					this.RenderReportItem(cellItem, elementProps, definition, rPLItemMeasurement, styleContext, borderContext, renderId);
				}
			}
			styleContext.Reset();
		}

		private void RenderListReportItem(RPLTablix tablix, RPLTablixCell cell, List<RPLTablixMemberCell> omittedHeaders, int tablixContext, StyleContext styleContext, bool firstRow, bool lastRow, SharedListLayoutState layoutState, RPLElement cellItem)
		{
			RPLElementProps elementProps = cellItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLItemMeasurement rPLItemMeasurement = null;
			rPLItemMeasurement = new RPLItemMeasurement();
			rPLItemMeasurement.Width = tablix.ColumnWidths[0];
			rPLItemMeasurement.Height = tablix.GetRowHeight(cell.RowIndex, cell.RowSpan);
			rPLItemMeasurement.State = cell.ElementState;
			bool zeroWidth = styleContext.ZeroWidth;
			styleContext.ZeroWidth = (rPLItemMeasurement.Width == 0.0);
			if (layoutState == SharedListLayoutState.Start)
			{
				this.WriteStream(HTML4Renderer.m_openTD);
				if (styleContext.ZeroWidth)
				{
					this.OpenStyle();
					this.WriteStream(HTML4Renderer.m_displayNone);
					this.CloseStyle(true);
				}
				this.WriteStream(HTML4Renderer.m_closeBracket);
			}
			if (cellItem is RPLRectangle)
			{
				int num = tablix.ColumnWidths.Length;
				int colIndex = cell.ColIndex;
				int colSpan = cell.ColSpan;
				bool right = colIndex + colSpan == num;
				int newContext = HTML4Renderer.GetNewContext(tablixContext, true, right, firstRow, lastRow);
				this.RenderListRectangle((RPLRectangle)cellItem, omittedHeaders, rPLItemMeasurement, elementProps, definition, layoutState, newContext);
				if (layoutState == SharedListLayoutState.End)
				{
					this.WriteStream(HTML4Renderer.m_closeTD);
				}
			}
			else
			{
				int num2 = 0;
				this.RenderTablixOmittedHeaderCells(omittedHeaders, 0, true, ref num2);
				this.RenderReportItem(cellItem, elementProps, definition, rPLItemMeasurement, styleContext, 0, this.NeedReportItemId(cellItem, elementProps));
				styleContext.Reset();
				if (layoutState == SharedListLayoutState.End)
				{
					this.WriteStream(HTML4Renderer.m_closeTD);
				}
			}
			styleContext.ZeroWidth = zeroWidth;
		}

		protected void RenderListRectangle(RPLContainer rectangle, List<RPLTablixMemberCell> omittedHeaders, RPLItemMeasurement measurement, RPLElementProps props, RPLElementPropsDef def, SharedListLayoutState layoutState, int borderContext)
		{
			RPLItemMeasurement[] children = rectangle.Children;
			this.GenerateHTMLTable(children, measurement.Top, measurement.Left, measurement.Width, measurement.Height, borderContext, false, layoutState, omittedHeaders, props.Style);
		}

		private void RenderSimpleTablixCell(RPLTablix tablix, string tablixID, int colSpan, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, bool lastCol, bool firstRow, bool lastRow, TablixFixedHeaderStorage headerStorage)
		{
			StyleContext styleContext = new StyleContext();
			int colIndex = cell.ColIndex;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(colIndex, cell.ColSpan);
			styleContext.ZeroWidth = (columnWidth == 0.0);
			int startIndex = this.RenderZeroWidthTDsForTablix(colIndex, colSpan, tablix);
			colSpan = this.GetColSpanMinusZeroWidthColumns(colIndex, colSpan, tablix);
			this.WriteStream(HTML4Renderer.m_openTD);
			this.RenderSimpleTablixCellID(tablix, tablixID, row, headerStorage, colIndex);
			if (colSpan > 1)
			{
				this.WriteStream(HTML4Renderer.m_colSpan);
				this.WriteStream(colSpan.ToString(CultureInfo.InvariantCulture));
				this.WriteStream(HTML4Renderer.m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int num = 0;
				this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, false, lastCol, firstRow, lastRow, element, ref num);
				this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
				this.RenderTablixReportItem(tablix, tablixContext, cell, styleContext, false, lastCol, firstRow, lastRow, element, ref num);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					this.OpenStyle();
					this.WriteStream(HTML4Renderer.m_displayNone);
					this.CloseStyle(true);
				}
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_nbsp);
				this.RenderTablixOmittedHeaderCells(omittedCells, colIndex, lastCol, ref omittedIndex);
			}
			this.WriteStream(HTML4Renderer.m_closeTD);
			this.RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private int GetColSpanMinusZeroWidthColumns(int startColIndex, int colSpan, RPLTablix tablix)
		{
			int num = colSpan;
			for (int i = startColIndex; i < startColIndex + colSpan; i++)
			{
				if (tablix.ColumnWidths[i] == 0.0)
				{
					num--;
				}
			}
			return num;
		}

		private int RenderZeroWidthTDsForTablix(int startIndex, int colSpan, RPLTablix tablix)
		{
			int i;
			for (i = startIndex; i < startIndex + colSpan && tablix.ColumnWidths[i] == 0.0; i++)
			{
				this.WriteStream(HTML4Renderer.m_openTD);
				this.OpenStyle();
				this.WriteStream(HTML4Renderer.m_displayNone);
				this.CloseStyle(true);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_closeTD);
			}
			return i;
		}

		private void RenderSimpleTablixCellID(RPLTablix tablix, string tablixID, int row, TablixFixedHeaderStorage headerStorage, int col)
		{
			if (tablix.FixedColumns[col])
			{
				string text = tablixID + "r" + row + "c" + col;
				this.RenderReportItemId(text);
				headerStorage.RowHeaders.Add(text);
				if (headerStorage.CornerHeaders != null && tablix.FixedRow(row))
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
		}

		protected void RenderMultiLineTextWithHits(string text, List<int> hits)
		{
			if (text != null)
			{
				int num = 0;
				int startPos = 0;
				int num2 = 0;
				int length = text.Length;
				for (int i = 0; i < length; i++)
				{
					switch (text[i])
					{
					case '\r':
						this.RenderTextWithHits(text, startPos, num, hits, ref num2);
						startPos = num + 1;
						break;
					case '\n':
						this.RenderTextWithHits(text, startPos, num, hits, ref num2);
						this.WriteStreamCR(HTML4Renderer.m_br);
						startPos = num + 1;
						break;
					}
					num++;
				}
				this.RenderTextWithHits(text, startPos, num, hits, ref num2);
			}
		}

		protected void RenderTextWithHits(string text, int startPos, int endPos, List<int> hitIndices, ref int currentHitIndex)
		{
			int length = this.m_searchText.Length;
			while (currentHitIndex < hitIndices.Count && hitIndices[currentHitIndex] < endPos)
			{
				int num = hitIndices[currentHitIndex];
				string theString = text.Substring(startPos, num - startPos);
				this.WriteStreamEncoded(theString);
				theString = text.Substring(num, length);
				this.OutputFindString(theString, 0);
				startPos = num + length;
				currentHitIndex++;
				this.m_currentHitCount++;
			}
			if (startPos <= endPos)
			{
				string theString = text.Substring(startPos, endPos - startPos);
				this.WriteStreamEncoded(theString);
			}
		}

		private void OutputFindString(string findString, int offset)
		{
			this.WriteStream(HTML4Renderer.m_openSpan);
			this.WriteStream(HTML4Renderer.m_id);
			this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
			this.WriteStream(HTML4Renderer.m_searchHitIdPrefix);
			this.WriteStream(this.m_currentHitCount.ToString(CultureInfo.InvariantCulture));
			if (offset > 0)
			{
				this.WriteStream("_");
				this.WriteStream(offset.ToString(CultureInfo.InvariantCulture));
			}
			this.WriteStream(HTML4Renderer.m_quote);
			if (this.m_currentHitCount == 0)
			{
				if (this.m_deviceInfo.IsBrowserSafari)
				{
					this.WriteStream(" style=\"COLOR:black;BACKGROUND-COLOR:#B5D4FE;\">");
				}
				else
				{
					this.WriteStream(" style=\"COLOR:highlighttext;BACKGROUND-COLOR:highlight;\">");
				}
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_closeBracket);
			}
			this.WriteStreamEncoded(findString);
			this.WriteStream(HTML4Renderer.m_closeSpan);
		}

		private bool IsImageNotFitProportional(RPLElement reportItem, RPLElementPropsDef definition)
		{
			RPLImagePropsDef rPLImagePropsDef = null;
			if (definition is RPLImagePropsDef)
			{
				rPLImagePropsDef = (RPLImagePropsDef)definition;
			}
			if (reportItem is RPLImage && rPLImagePropsDef != null)
			{
				return rPLImagePropsDef.Sizing != RPLFormat.Sizings.FitProportional;
			}
			return false;
		}

		protected void RenderImage(RPLImage image, RPLImageProps imageProps, RPLImagePropsDef imagePropsDef, RPLItemMeasurement measurement, ref int borderContext, bool renderId)
		{
			bool flag = false;
			bool flag2 = false;
			RPLImageData image2 = imageProps.Image;
			RPLActionInfo actionInfo = imageProps.ActionInfo;
			StyleContext styleContext = new StyleContext();
			RPLFormat.Sizings sizing = imagePropsDef.Sizing;
			bool flag3 = false;
			if (sizing == RPLFormat.Sizings.AutoSize)
			{
				flag3 = true;
				this.WriteStream(HTML4Renderer.m_openTable);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_firstTD);
				this.WriteStream(HTML4Renderer.m_closeBracket);
			}
			this.WriteStream(HTML4Renderer.m_openDiv);
			int xOffset = 0;
			int yOffset = 0;
			System.Drawing.Rectangle imageConsolidationOffsets = imageProps.Image.ImageConsolidationOffsets;
			bool flag4 = !imageConsolidationOffsets.IsEmpty;
			if (flag4)
			{
				if (sizing == RPLFormat.Sizings.Clip || sizing == RPLFormat.Sizings.FitProportional || sizing == RPLFormat.Sizings.Fit)
				{
					styleContext.RenderMeasurements = (styleContext.InTablix || sizing != RPLFormat.Sizings.AutoSize);
					this.RenderReportItemStyle((RPLElement)image, (RPLElementProps)imageProps, (RPLElementPropsDef)imagePropsDef, measurement, styleContext, ref borderContext, imagePropsDef.ID);
					this.WriteStream(HTML4Renderer.m_closeBracket);
					this.WriteStream(HTML4Renderer.m_openDiv);
				}
				this.WriteOuterConsolidation(imageConsolidationOffsets, sizing, imageProps.UniqueName);
				this.RenderReportItemStyle((RPLElement)image, (RPLElementProps)imageProps, (RPLElementPropsDef)imagePropsDef, (RPLItemMeasurement)null, styleContext, ref borderContext, imagePropsDef.ID);
				xOffset = imageConsolidationOffsets.Left;
				yOffset = imageConsolidationOffsets.Top;
			}
			else
			{
				styleContext.RenderMeasurements = (styleContext.InTablix || sizing != RPLFormat.Sizings.AutoSize);
				this.RenderReportItemStyle((RPLElement)image, (RPLElementProps)imageProps, (RPLElementPropsDef)imagePropsDef, measurement, styleContext, ref borderContext, imagePropsDef.ID);
			}
			this.WriteStream(HTML4Renderer.m_closeBracket);
			if (this.HasAction(actionInfo))
			{
				flag2 = this.RenderElementHyperlink(imageProps.Style, actionInfo.Actions[0]);
			}
			this.WriteStream(HTML4Renderer.m_img);
			if (this.m_browserIE)
			{
				this.WriteStream(HTML4Renderer.m_imgOnError);
			}
			if (renderId || flag)
			{
				this.RenderReportItemId(imageProps.UniqueName);
			}
			if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
			{
				this.WriteAttrEncoded(HTML4Renderer.m_useMap, "#" + this.m_deviceInfo.HtmlPrefixId + HTML4Renderer.m_mapPrefixString + imageProps.UniqueName);
				this.WriteStream(HTML4Renderer.m_zeroBorder);
			}
			else if (flag2)
			{
				this.WriteStream(HTML4Renderer.m_zeroBorder);
			}
			switch (sizing)
			{
			case RPLFormat.Sizings.FitProportional:
			{
				PaddingSharedInfo paddings = this.GetPaddings(image.ElementProps.Style, null);
				bool writeSmallSize = !flag4 && this.m_deviceInfo.BrowserMode == BrowserMode.Standards;
				this.RenderImageFitProportional(image, measurement, paddings, writeSmallSize);
				break;
			}
			case RPLFormat.Sizings.Fit:
				if (!flag4)
				{
					if (this.m_useInlineStyle)
					{
						this.PercentSizes();
					}
					else
					{
						this.ClassPercentSizes();
					}
				}
				break;
			}
			if (flag4)
			{
				this.WriteClippedDiv(imageConsolidationOffsets);
			}
			this.WriteToolTip(imageProps);
			this.WriteStream(HTML4Renderer.m_src);
			this.RenderImageUrl(true, image2);
			this.WriteStreamCR(HTML4Renderer.m_closeTag);
			if (flag2)
			{
				this.WriteStream(HTML4Renderer.m_closeA);
			}
			if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
			{
				this.RenderImageMapAreas(imageProps.ActionImageMapAreas, (double)measurement.Width, (double)measurement.Height, imageProps.UniqueName, xOffset, yOffset);
			}
			if (flag4 && (sizing == RPLFormat.Sizings.Clip || sizing == RPLFormat.Sizings.FitProportional || sizing == RPLFormat.Sizings.Fit))
			{
				this.WriteStream(HTML4Renderer.m_closeDiv);
			}
			this.WriteStreamCR(HTML4Renderer.m_closeDiv);
			if (flag3)
			{
				this.WriteStreamCR(HTML4Renderer.m_lastTD);
				this.WriteStreamCR(HTML4Renderer.m_closeTable);
			}
		}

		protected int RenderReportItem(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, StyleContext styleContext, int borderContext, bool renderId)
		{
			int num = borderContext;
			if (reportItem == null)
			{
				return num;
			}
			if (measurement != null)
			{
				styleContext.OmitBordersState = measurement.State;
			}
			RPLTextBox rPLTextBox = reportItem as RPLTextBox;
			if (rPLTextBox != null)
			{
				if (styleContext.InTablix)
				{
					this.RenderTextBoxPercent(rPLTextBox, rPLTextBox.ElementProps as RPLTextBoxProps, rPLTextBox.ElementProps.Definition as RPLTextBoxPropsDef, measurement, styleContext, renderId);
				}
				else
				{
					this.RenderTextBox(rPLTextBox, rPLTextBox.ElementProps as RPLTextBoxProps, rPLTextBox.ElementProps.Definition as RPLTextBoxPropsDef, measurement, styleContext, ref num, renderId);
				}
			}
			else if (reportItem is RPLTablix)
			{
				this.RenderTablix((RPLTablix)reportItem, props, def, measurement, styleContext, ref num, renderId);
			}
			else if (reportItem is RPLRectangle)
			{
				this.RenderRectangle((RPLContainer)reportItem, props, (RPLRectanglePropsDef)def, measurement, ref num, renderId, styleContext);
			}
			else if (reportItem is RPLChart || reportItem is RPLGaugePanel || reportItem is RPLMap)
			{
				this.RenderServerDynamicImage(reportItem, (RPLDynamicImageProps)props, def, measurement, num, renderId, styleContext);
			}
			else if (reportItem is RPLSubReport)
			{
				this.RenderSubReport((RPLSubReport)reportItem, props, def, measurement, ref num, renderId, styleContext);
			}
			else if (reportItem is RPLImage)
			{
				if (styleContext.InTablix)
				{
					this.RenderImagePercent((RPLImage)reportItem, (RPLImageProps)props, (RPLImagePropsDef)def, measurement);
				}
				else
				{
					this.RenderImage((RPLImage)reportItem, (RPLImageProps)props, (RPLImagePropsDef)def, measurement, ref num, renderId);
				}
			}
			else if (reportItem is RPLLine)
			{
				this.RenderLine((RPLLine)reportItem, props, (RPLLinePropsDef)def, measurement, renderId, styleContext);
			}
			return num;
		}

		protected void RenderSubReport(RPLSubReport subReport, RPLElementProps subReportProps, RPLElementPropsDef subReportDef, RPLItemMeasurement measurement, ref int borderContext, bool renderId, StyleContext styleContext)
		{
			if (!styleContext.InTablix || renderId)
			{
				styleContext.RenderMeasurements = false;
				this.WriteStream(HTML4Renderer.m_openDiv);
				this.RenderReportItemStyle((RPLElement)subReport, subReportProps, subReportDef, measurement, styleContext, ref borderContext, subReportDef.ID);
				if (renderId)
				{
					this.RenderReportItemId(subReportProps.UniqueName);
				}
				this.WriteStreamCR(HTML4Renderer.m_closeBracket);
			}
			RPLItemMeasurement[] children = subReport.Children;
			int num = 0;
			int num2 = borderContext;
			bool usePercentWidth = children.Length > 0;
			int num3 = children.Length;
			for (int i = 0; i < num3; i++)
			{
				if (i == 0 && num3 > 1 && (borderContext & 8) > 0)
				{
					num2 &= -9;
				}
				else if (i == 1 && (borderContext & 4) > 0)
				{
					num2 &= -5;
				}
				if (i > 0 && i == num3 - 1 && (borderContext & 8) > 0)
				{
					num2 |= 8;
				}
				num = num2;
				RPLItemMeasurement rPLItemMeasurement = children[i];
				RPLContainer rPLContainer = (RPLContainer)rPLItemMeasurement.Element;
				RPLElementProps elementProps = rPLContainer.ElementProps;
				RPLElementPropsDef definition = elementProps.Definition;
				this.m_isBody = true;
				this.m_usePercentWidth = usePercentWidth;
				this.RenderRectangle(rPLContainer, elementProps, definition, rPLItemMeasurement, ref num, false, new StyleContext());
			}
			if (styleContext.InTablix && !renderId)
			{
				return;
			}
			this.WriteStreamCR(HTML4Renderer.m_closeDiv);
		}

		protected void RenderRectangleMeasurements(RPLItemMeasurement measurement, IRPLStyle style)
		{
			float adjustedWidth = this.GetAdjustedWidth(measurement, style);
			float adjustedHeight = this.GetAdjustedHeight(measurement, style);
			this.RenderMeasurementWidth(adjustedWidth, true);
			if (this.m_deviceInfo.IsBrowserIE && this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6)
			{
				this.RenderMeasurementMinHeight(adjustedHeight);
			}
			else
			{
				this.RenderMeasurementHeight(adjustedHeight);
			}
		}

		private void WriteFontSizeSmallPoint()
		{
			if (this.m_deviceInfo.IsBrowserGeckoEngine)
			{
				this.WriteStream(HTML4Renderer.m_smallPoint);
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_zeroPoint);
			}
		}

		protected void RenderRectangle(RPLContainer rectangle, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, ref int borderContext, bool renderId, StyleContext styleContext)
		{
			RPLItemMeasurement[] children = rectangle.Children;
			RPLRectanglePropsDef rPLRectanglePropsDef = def as RPLRectanglePropsDef;
			if (rPLRectanglePropsDef != null && rPLRectanglePropsDef.LinkToChildId != null)
			{
				this.m_linkToChildStack.Push(rPLRectanglePropsDef.LinkToChildId);
			}
			bool expandItem = this.m_expandItem;
			bool flag = renderId;
			string text = props.UniqueName;
			bool flag2 = children == null || children.Length == 0;
			if (flag2 && styleContext.InTablix)
			{
				return;
			}
			bool flag3 = this.m_deviceInfo.OutlookCompat || !this.m_browserIE || (flag2 && this.m_usePercentWidth);
			if (!styleContext.InTablix || renderId)
			{
				if (flag3)
				{
					this.WriteStream(HTML4Renderer.m_openTable);
					this.WriteStream(HTML4Renderer.m_zeroBorder);
				}
				else
				{
					this.WriteStream(HTML4Renderer.m_openDiv);
					if (this.m_deviceInfo.IsBrowserIE && this.m_deviceInfo.AllowScript)
					{
						if (!this.m_needsGrowRectangleScript)
						{
							this.CreateGrowRectIdsStream();
						}
						flag = true;
						if (!renderId)
						{
							text = props.UniqueName + "_gr";
						}
						this.WriteIdToSecondaryStream(this.m_growRectangleIdsStream, text);
					}
				}
				if (flag)
				{
					this.RenderReportItemId(text);
				}
				if (this.m_isBody)
				{
					this.m_isBody = false;
					styleContext.RenderMeasurements = false;
					if (flag2)
					{
						this.OpenStyle();
						if (this.m_usePercentWidth)
						{
							this.RenderMeasurementHeight(measurement.Height);
							this.WriteStream(HTML4Renderer.m_styleWidth);
							this.WriteStream(HTML4Renderer.m_percent);
							this.WriteStream(HTML4Renderer.m_semiColon);
						}
						else
						{
							this.RenderRectangleMeasurements(measurement, props.Style);
						}
					}
					else if (flag3 && this.m_usePercentWidth)
					{
						this.OpenStyle();
						this.WriteStream(HTML4Renderer.m_styleWidth);
						this.WriteStream(HTML4Renderer.m_percent);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					this.m_usePercentWidth = false;
				}
				if (!styleContext.InTablix)
				{
					if (styleContext.RenderMeasurements)
					{
						this.OpenStyle();
						this.RenderRectangleMeasurements(measurement, props.Style);
					}
					this.RenderReportItemStyle((RPLElement)rectangle, props, def, measurement, styleContext, ref borderContext, def.ID);
				}
				this.CloseStyle(true);
				this.WriteToolTip(props);
				this.WriteStreamCR(HTML4Renderer.m_closeBracket);
				if (flag3)
				{
					this.WriteStream(HTML4Renderer.m_firstTD);
					this.OpenStyle();
					if (flag2)
					{
						this.RenderMeasurementStyle(measurement.Height, measurement.Width);
						this.WriteStream(HTML4Renderer.m_fontSize);
						this.WriteStream("1pt");
					}
					else
					{
						this.WriteStream(HTML4Renderer.m_verticalAlign);
						this.WriteStream(HTML4Renderer.m_topValue);
					}
					this.CloseStyle(true);
					this.WriteStream(HTML4Renderer.m_closeBracket);
				}
			}
			if (flag2)
			{
				this.WriteStream(HTML4Renderer.m_nbsp);
			}
			else
			{
				bool inTablix = styleContext.InTablix;
				styleContext.InTablix = false;
				flag2 = this.GenerateHTMLTable(children, measurement.Top, measurement.Left, measurement.Width, measurement.Height, borderContext, expandItem, SharedListLayoutState.None, null, props.Style);
				if (inTablix)
				{
					styleContext.InTablix = true;
				}
			}
			if (!styleContext.InTablix || renderId)
			{
				if (flag3)
				{
					this.WriteStream(HTML4Renderer.m_lastTD);
					this.WriteStream(HTML4Renderer.m_closeTable);
				}
				else
				{
					this.WriteStreamCR(HTML4Renderer.m_closeDiv);
				}
			}
			if (this.m_linkToChildStack.Count > 0 && rPLRectanglePropsDef != null && rPLRectanglePropsDef.LinkToChildId != null && rPLRectanglePropsDef.LinkToChildId.Equals(this.m_linkToChildStack.Peek()))
			{
				this.m_linkToChildStack.Pop();
			}
		}

		private void RenderElementHyperlinkAllTextStyles(RPLElementStyle style, RPLAction action, string id)
		{
			this.WriteStream(HTML4Renderer.m_openA);
			this.RenderTabIndex();
			bool flag = false;
			if (action.Hyperlink != null)
			{
				this.WriteStream(HTML4Renderer.m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + HTML4Renderer.m_quoteString);
				flag = true;
			}
			else
			{
				this.RenderInteractionAction(action, ref flag);
			}
			TextRunStyleWriter styleWriter = new TextRunStyleWriter(this);
			this.WriteStyles(id, style.NonSharedProperties, style.SharedProperties, styleWriter);
			if (this.m_deviceInfo.LinkTarget != null)
			{
				this.WriteStream(HTML4Renderer.m_target);
				this.WriteStream(this.m_deviceInfo.LinkTarget);
				this.WriteStream(HTML4Renderer.m_quote);
			}
			this.WriteStream(HTML4Renderer.m_closeBracket);
		}

		private bool RenderElementHyperlink(IRPLStyle style, RPLAction action)
		{
			object obj = style[24];
			obj = ((obj != null) ? obj : ((object)RPLFormat.TextDecorations.None));
			string color = (string)style[27];
			return this.RenderHyperlink(action, (RPLFormat.TextDecorations)obj, color);
		}

		protected void RenderTextBoxPercent(RPLTextBox textBox, RPLTextBoxProps textBoxProps, RPLTextBoxPropsDef textBoxPropsDef, RPLItemMeasurement measurement, StyleContext styleContext, bool renderId)
		{
			RPLStyleProps actionStyle = null;
			RPLActionInfo actionInfo = textBoxProps.ActionInfo;
			RPLStyleProps nonSharedStyle = textBoxProps.NonSharedStyle;
			RPLStyleProps sharedStyle = textBoxPropsDef.SharedStyle;
			RPLElementStyle style = textBoxProps.Style;
			bool flag = this.CanSort(textBoxPropsDef);
			bool flag2 = this.NeedSharedToggleParent(textBoxProps);
			bool flag3 = false;
			bool isSimple = textBoxPropsDef.IsSimple;
			bool flag4 = HTML4Renderer.IsDirectionRTL(style);
			bool flag5 = HTML4Renderer.IsWritingModeVertical(style);
			bool flag6 = flag5 && this.m_deviceInfo.IsBrowserIE;
			if (flag6)
			{
				if (textBoxPropsDef.CanGrow)
				{
					this.WriteStream(HTML4Renderer.m_openDiv);
					this.OpenStyle();
					this.RenderDirectionStyles(textBox, textBoxProps, textBoxPropsDef, null, textBoxProps.Style, nonSharedStyle, false, styleContext);
					this.WriteStream("display: inline;");
					this.CloseStyle(true);
					this.ClassPercentHeight();
					if (this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && this.m_deviceInfo.AllowScript)
					{
						if (!this.m_needsFitVertTextScript)
						{
							this.CreateFitVertTextIdsStream();
						}
						this.WriteIdToSecondaryStream(this.m_fitVertTextIdsStream, textBoxProps.UniqueName + "_fvt");
						this.RenderReportItemId(textBoxProps.UniqueName + "_fvt");
					}
					this.WriteStreamCR(HTML4Renderer.m_closeBracket);
					this.WriteStream(HTML4Renderer.m_openTable);
					this.ClassPercentHeight();
					this.WriteStreamCR(HTML4Renderer.m_closeBracket);
					this.WriteStream(HTML4Renderer.m_firstTD);
				}
				else
				{
					this.WriteStream(HTML4Renderer.m_openDiv);
				}
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_openDiv);
			}
			if (renderId || flag2 || flag)
			{
				this.RenderReportItemId(textBoxProps.UniqueName);
			}
			bool flag7 = flag2 && !isSimple;
			bool flag8 = flag || flag7;
			if (!textBoxPropsDef.CanGrow)
			{
				if ((!this.m_browserIE || this.m_deviceInfo.BrowserMode == BrowserMode.Standards || flag6) && measurement != null)
				{
					styleContext.RenderMeasurements = false;
					float innerContainerHeight = this.GetInnerContainerHeight(measurement, style);
					this.OpenStyle();
					this.RenderMeasurementHeight(innerContainerHeight);
					this.WriteStream(HTML4Renderer.m_overflowHidden);
					this.WriteStream(HTML4Renderer.m_semiColon);
					goto IL_0201;
				}
				styleContext.RenderMeasurements = true;
				goto IL_0201;
			}
			goto IL_0232;
			IL_0201:
			if (!flag8)
			{
				object obj = style[26];
				bool flag9 = obj != null && (RPLFormat.VerticalAlignments)obj != 0 && !textBoxPropsDef.CanGrow;
				flag8 = flag9;
			}
			measurement = null;
			goto IL_0232;
			IL_0232:
			if (flag8)
			{
				this.CloseStyle(true);
				styleContext.RenderMeasurements = false;
				this.WriteStreamCR(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_openTable);
				this.WriteStream(HTML4Renderer.m_zeroBorder);
				if (isSimple && (flag || flag7))
				{
					this.WriteClassName(HTML4Renderer.m_percentHeightInlineTable, HTML4Renderer.m_classPercentHeightInlineTable);
				}
				else
				{
					this.WriteClassName(HTML4Renderer.m_percentSizeInlineTable, HTML4Renderer.m_classPercentSizeInlineTable);
				}
				this.RenderReportLanguage();
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_firstTD);
				if (flag || flag7)
				{
					if (flag5)
					{
						this.WriteStream(" ROWS='2'");
					}
					this.RenderAtStart(textBoxProps, style, flag && flag4, flag7 && !flag4);
				}
			}
			int num = 0;
			this.RenderReportItemStyle(textBox, textBoxProps, textBoxPropsDef, nonSharedStyle, sharedStyle, measurement, styleContext, ref num, textBoxPropsDef.ID);
			this.WriteToolTip(textBoxProps);
			this.WriteStreamCR(HTML4Renderer.m_closeBracket);
			if (flag2 && isSimple)
			{
				this.RenderToggleImage(textBoxProps);
			}
			RPLAction rPLAction = null;
			if (this.HasAction(actionInfo))
			{
				rPLAction = actionInfo.Actions[0];
				this.RenderElementHyperlinkAllTextStyles(style, rPLAction, textBoxPropsDef.ID + "a");
				flag3 = true;
			}
			string text = null;
			if (textBoxPropsDef.IsSimple)
			{
				text = textBoxProps.Value;
				if (string.IsNullOrEmpty(text))
				{
					text = textBoxPropsDef.Value;
				}
			}
			this.RenderTextBoxContent(textBox, textBoxProps, textBoxPropsDef, text, actionStyle, flag2 || flag, measurement, rPLAction);
			if (flag3)
			{
				this.WriteStream(HTML4Renderer.m_closeA);
			}
			if (flag8)
			{
				this.RenderAtEnd(textBoxProps, style, flag && !flag4, flag7 && flag4);
				this.WriteStream(HTML4Renderer.m_lastTD);
				this.WriteStream(HTML4Renderer.m_closeTable);
			}
			if (flag6)
			{
				if (textBoxPropsDef.CanGrow)
				{
					this.WriteStreamCR(HTML4Renderer.m_lastTD);
					this.WriteStreamCR(HTML4Renderer.m_closeTable);
					this.WriteStreamCR(HTML4Renderer.m_closeDiv);
				}
				else
				{
					this.WriteStream(HTML4Renderer.m_closeDiv);
				}
			}
			else
			{
				this.WriteStreamCR(HTML4Renderer.m_closeDiv);
			}
		}

		protected void RenderPageHeaderFooter(RPLItemMeasurement hfMeasurement)
		{
			if (hfMeasurement.Height != 0.0)
			{
				RPLHeaderFooter rPLHeaderFooter = (RPLHeaderFooter)hfMeasurement.Element;
				int borderContext = 0;
				StyleContext styleContext = new StyleContext();
				this.WriteStream(HTML4Renderer.m_openTR);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_openTD);
				styleContext.StyleOnCell = true;
				this.RenderReportItemStyle(rPLHeaderFooter, rPLHeaderFooter.ElementProps, rPLHeaderFooter.ElementProps.Definition, null, styleContext, ref borderContext, rPLHeaderFooter.ElementProps.Definition.ID + "c");
				styleContext.StyleOnCell = false;
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_openDiv);
				if (!this.m_deviceInfo.IsBrowserIE)
				{
					styleContext.RenderMeasurements = false;
					styleContext.RenderMinMeasurements = true;
				}
				this.RenderReportItemStyle(rPLHeaderFooter, hfMeasurement, ref borderContext, styleContext);
				this.WriteStreamCR(HTML4Renderer.m_closeBracket);
				RPLItemMeasurement[] children = rPLHeaderFooter.Children;
				if (children != null && children.Length > 0)
				{
					this.m_renderTableHeight = true;
					this.GenerateHTMLTable(children, 0f, 0f, this.m_pageContent.MaxSectionWidth, hfMeasurement.Height, borderContext, false, SharedListLayoutState.None, null, rPLHeaderFooter.ElementProps.Style);
				}
				else
				{
					this.WriteStream(HTML4Renderer.m_nbsp);
				}
				this.m_renderTableHeight = false;
				this.WriteStreamCR(HTML4Renderer.m_closeDiv);
				this.WriteStream(HTML4Renderer.m_closeTD);
				this.WriteStream(HTML4Renderer.m_closeTR);
			}
		}

		protected void RenderStyleProps(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLItemMeasurement measurement, IRPLStyle sharedStyleProps, IRPLStyle nonSharedStyleProps, StyleContext styleContext, ref int borderContext, bool isNonSharedStyles)
		{
			if (styleContext.ZeroWidth)
			{
				this.WriteStream(HTML4Renderer.m_displayNone);
			}
			IRPLStyle iRPLStyle = isNonSharedStyles ? nonSharedStyleProps : sharedStyleProps;
			float num;
			if (iRPLStyle != null)
			{
				object obj = null;
				if (styleContext.StyleOnCell)
				{
					bool renderPadding = true;
					if ((HTML4Renderer.IsWritingModeVertical(sharedStyleProps) || HTML4Renderer.IsWritingModeVertical(nonSharedStyleProps)) && styleContext.IgnorePadding && this.m_deviceInfo.IsBrowserIE)
					{
						renderPadding = false;
					}
					if (!styleContext.NoBorders)
					{
						this.RenderHtmlBorders(iRPLStyle, ref borderContext, styleContext.OmitBordersState, renderPadding, isNonSharedStyles, sharedStyleProps);
						this.RenderBackgroundStyleProps(iRPLStyle);
					}
					if (!styleContext.OnlyRenderMeasurementsBackgroundBorders)
					{
						obj = iRPLStyle[26];
						if (obj != null && !styleContext.IgnoreVerticalAlign)
						{
							obj = EnumStrings.GetValue((RPLFormat.VerticalAlignments)obj);
							this.WriteStream(HTML4Renderer.m_verticalAlign);
							this.WriteStream(obj);
							this.WriteStream(HTML4Renderer.m_semiColon);
						}
						obj = iRPLStyle[25];
						if (obj != null)
						{
							if ((RPLFormat.TextAlignments)obj != 0)
							{
								obj = EnumStrings.GetValue((RPLFormat.TextAlignments)obj);
								this.WriteStream(HTML4Renderer.m_textAlign);
								this.WriteStream(obj);
								this.WriteStream(HTML4Renderer.m_semiColon);
							}
							else
							{
								this.RenderTextAlign(props as RPLTextBoxProps, props.Style);
							}
						}
						this.RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
					}
					if (measurement == null)
					{
						return;
					}
					if (!this.m_deviceInfo.OutlookCompat && this.m_deviceInfo.IsBrowserIE)
					{
						return;
					}
					num = measurement.Width;
					if ((reportItem is RPLTextBox || this.IsImageNotFitProportional(reportItem, definition)) && !styleContext.InTablix)
					{
						float adjustedWidth = this.GetAdjustedWidth(measurement, props.Style);
						if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
						{
							num = adjustedWidth;
						}
						this.RenderMeasurementMinWidth(adjustedWidth);
						goto IL_01a2;
					}
					this.RenderMeasurementMinWidth(num);
					goto IL_01a2;
				}
				if (reportItem is RPLTextBox)
				{
					this.WriteStream(HTML4Renderer.m_wordWrap);
					this.WriteStream(HTML4Renderer.m_semiColon);
					this.WriteStream(HTML4Renderer.m_whiteSpacePreWrap);
					this.WriteStream(HTML4Renderer.m_semiColon);
				}
				if (styleContext.RenderMeasurements || styleContext.RenderMinMeasurements)
				{
					bool flag = false;
					this.IsCollectionWithoutContent(reportItem as RPLContainer, ref flag);
					if (measurement == null || (styleContext.InTablix && !flag && (reportItem is RPLChart || reportItem is RPLGaugePanel || reportItem is RPLMap)))
					{
						if (reportItem is RPLTextBox)
						{
							RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)definition;
							if (styleContext.RenderMeasurements)
							{
								this.WriteStream(HTML4Renderer.m_styleWidth);
							}
							else if (styleContext.RenderMinMeasurements)
							{
								this.WriteStream(HTML4Renderer.m_styleMinWidth);
							}
							if (styleContext.InTablix && this.m_deviceInfo.BrowserMode == BrowserMode.Quirks)
							{
								this.WriteStream(HTML4Renderer.m_ninetyninepercent);
							}
							else
							{
								this.WriteStream(HTML4Renderer.m_percent);
							}
							this.WriteStream(HTML4Renderer.m_semiColon);
							if (rPLTextBoxPropsDef.CanGrow)
							{
								this.WriteStream(HTML4Renderer.m_overflowXHidden);
							}
							else
							{
								if (styleContext.RenderMeasurements)
								{
									this.WriteStream(HTML4Renderer.m_styleHeight);
								}
								else if (styleContext.RenderMinMeasurements)
								{
									this.WriteStream(HTML4Renderer.m_styleMinHeight);
								}
								this.WriteStream(HTML4Renderer.m_percent);
								this.WriteStream(HTML4Renderer.m_semiColon);
								this.WriteStream(HTML4Renderer.m_overflowHidden);
							}
							this.WriteStream(HTML4Renderer.m_semiColon);
						}
						else if (!(reportItem is RPLTablix))
						{
							this.RenderPercentSizes();
						}
					}
					else if (reportItem is RPLTextBox)
					{
						float num2 = measurement.Width;
						float height = measurement.Height;
						if (!styleContext.NoBorders && !styleContext.InTablix)
						{
							float adjustedWidth2 = this.GetAdjustedWidth(measurement, props.Style);
							if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
							{
								num2 = adjustedWidth2;
								height = this.GetAdjustedHeight(measurement, props.Style);
							}
							this.RenderMeasurementMinWidth(adjustedWidth2);
						}
						else
						{
							this.RenderMeasurementMinWidth(num2);
						}
						RPLTextBoxPropsDef rPLTextBoxPropsDef2 = (RPLTextBoxPropsDef)definition;
						if (rPLTextBoxPropsDef2.CanGrow && rPLTextBoxPropsDef2.CanShrink)
						{
							this.RenderMeasurementWidth(num2, false);
						}
						else
						{
							this.WriteStream(HTML4Renderer.m_overflowHidden);
							this.WriteStream(HTML4Renderer.m_semiColon);
							this.RenderMeasurementWidth(num2, false);
							this.RenderMeasurementHeight(height);
						}
					}
					else if (!(reportItem is RPLTablix))
					{
						if (!(reportItem is RPLRectangle))
						{
							float height2 = measurement.Height;
							float num3 = measurement.Width;
							if (!styleContext.InTablix && this.IsImageNotFitProportional(reportItem, definition) && !styleContext.NoBorders)
							{
								float adjustedWidth3 = this.GetAdjustedWidth(measurement, props.Style);
								if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
								{
									num3 = adjustedWidth3;
									height2 = this.GetAdjustedHeight(measurement, props.Style);
								}
								this.RenderMeasurementMinWidth(adjustedWidth3);
							}
							else
							{
								this.RenderMeasurementMinWidth(num3);
							}
							if (reportItem is RPLHeaderFooter && (!this.m_deviceInfo.IsBrowserIE || (this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6)))
							{
								this.RenderMeasurementMinHeight(height2);
							}
							else
							{
								this.RenderMeasurementHeight(height2);
							}
							this.RenderMeasurementWidth(num3, false);
						}
						if (flag || reportItem is RPLImage)
						{
							this.WriteStream(HTML4Renderer.m_overflowHidden);
							this.WriteStream(HTML4Renderer.m_semiColon);
						}
					}
				}
				if (!styleContext.InTablix && !styleContext.NoBorders)
				{
					this.RenderHtmlBorders(iRPLStyle, ref borderContext, styleContext.OmitBordersState, !styleContext.EmptyTextBox || this.m_deviceInfo.IsBrowserIE6Or7StandardsMode, isNonSharedStyles, sharedStyleProps);
					this.RenderBackgroundStyleProps(iRPLStyle);
				}
				if (!styleContext.OnlyRenderMeasurementsBackgroundBorders && (!styleContext.EmptyTextBox || !isNonSharedStyles))
				{
					obj = iRPLStyle[19];
					if (obj != null)
					{
						obj = EnumStrings.GetValue((RPLFormat.FontStyles)obj);
						this.WriteStream(HTML4Renderer.m_fontStyle);
						this.WriteStream(obj);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					obj = iRPLStyle[20];
					if (obj != null)
					{
						this.WriteStream(HTML4Renderer.m_fontFamily);
						this.WriteStream(HTML4Renderer.HandleSpecialFontCharacters(obj.ToString()));
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					obj = iRPLStyle[21];
					if (obj != null)
					{
						this.WriteStream(HTML4Renderer.m_fontSize);
						if (string.Compare(obj.ToString(), "0pt", StringComparison.OrdinalIgnoreCase) != 0)
						{
							this.WriteStream(obj);
						}
						else
						{
							this.WriteFontSizeSmallPoint();
						}
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					else
					{
						RPLTextBoxPropsDef rPLTextBoxPropsDef3 = definition as RPLTextBoxPropsDef;
						RPLStyleProps sharedStyle = reportItem.ElementPropsDef.SharedStyle;
						if ((!isNonSharedStyles || sharedStyle == null || sharedStyle.Count == 0) && rPLTextBoxPropsDef3 != null && !rPLTextBoxPropsDef3.IsSimple)
						{
							this.WriteStream(HTML4Renderer.m_fontSize);
							this.WriteFontSizeSmallPoint();
							this.WriteStream(HTML4Renderer.m_semiColon);
						}
					}
					obj = iRPLStyle[22];
					if (obj != null)
					{
						obj = EnumStrings.GetValue((RPLFormat.FontWeights)obj);
						this.WriteStream(HTML4Renderer.m_fontWeight);
						this.WriteStream(obj);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					obj = iRPLStyle[24];
					if (obj != null)
					{
						obj = EnumStrings.GetValue((RPLFormat.TextDecorations)obj);
						this.WriteStream(HTML4Renderer.m_textDecoration);
						this.WriteStream(obj);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					obj = iRPLStyle[31];
					if (obj != null)
					{
						obj = EnumStrings.GetValue((RPLFormat.UnicodeBiDiTypes)obj);
						this.WriteStream(HTML4Renderer.m_unicodeBiDi);
						this.WriteStream(obj);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					obj = iRPLStyle[27];
					if (obj != null)
					{
						this.WriteStream(HTML4Renderer.m_color);
						this.WriteStream(obj);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					obj = iRPLStyle[28];
					if (obj != null)
					{
						this.WriteStream(HTML4Renderer.m_lineHeight);
						this.WriteStream(obj);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					if ((HTML4Renderer.IsWritingModeVertical(sharedStyleProps) || HTML4Renderer.IsWritingModeVertical(nonSharedStyleProps)) && reportItem is RPLTextBox && styleContext.InTablix && this.m_deviceInfo.IsBrowserIE && !styleContext.IgnorePadding)
					{
						this.RenderPaddingStyle(iRPLStyle);
					}
					this.RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
					obj = iRPLStyle[26];
					if (obj != null && !styleContext.IgnoreVerticalAlign)
					{
						obj = EnumStrings.GetValue((RPLFormat.VerticalAlignments)obj);
						this.WriteStream(HTML4Renderer.m_verticalAlign);
						this.WriteStream(obj);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					obj = iRPLStyle[25];
					if (obj != null)
					{
						if ((RPLFormat.TextAlignments)obj != 0)
						{
							this.WriteStream(HTML4Renderer.m_textAlign);
							this.WriteStream(EnumStrings.GetValue((RPLFormat.TextAlignments)obj));
							this.WriteStream(HTML4Renderer.m_semiColon);
						}
						else
						{
							this.RenderTextAlign(props as RPLTextBoxProps, props.Style);
						}
					}
				}
			}
			return;
			IL_01a2:
			this.RenderMeasurementWidth(num, false);
		}

		protected void RenderLine(RPLLine reportItem, RPLElementProps rplProps, RPLLinePropsDef rplPropsDef, RPLItemMeasurement measurement, bool renderId, StyleContext styleContext)
		{
			if (this.IsLineSlanted(measurement))
			{
				if (renderId)
				{
					this.RenderNavigationId(rplProps.UniqueName);
				}
				if (this.m_deviceInfo.BrowserMode == BrowserMode.Quirks)
				{
					this.RenderVMLLine(reportItem, measurement, styleContext);
				}
			}
			else
			{
				bool flag = measurement.Height == 0.0;
				this.WriteStream(HTML4Renderer.m_openSpan);
				if (renderId)
				{
					this.RenderReportItemId(rplProps.UniqueName);
				}
				int num = 0;
				object obj = rplProps.Style[10];
				if (obj != null)
				{
					this.OpenStyle();
					if (flag)
					{
						this.WriteStream(HTML4Renderer.m_styleHeight);
					}
					else
					{
						this.WriteStream(HTML4Renderer.m_styleWidth);
					}
					this.WriteStream(obj);
					this.WriteStream(HTML4Renderer.m_semiColon);
				}
				obj = rplProps.Style[0];
				if (obj != null)
				{
					this.OpenStyle();
					this.WriteStream(HTML4Renderer.m_backgroundColor);
					this.WriteStream(obj);
				}
				this.RenderReportItemStyle(reportItem, measurement, ref num);
				this.CloseStyle(true);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_closeSpan);
			}
		}

		protected bool GenerateHTMLTable(RPLItemMeasurement[] repItemCol, float ownerTop, float ownerLeft, float dxParent, float dyParent, int borderContext, bool expandLayout, SharedListLayoutState layoutState, List<RPLTablixMemberCell> omittedHeaders, IRPLStyle style)
		{
			int num = 0;
			bool result = false;
			object defaultBorderStyle = null;
			object specificBorderStyle = null;
			object specificBorderStyle2 = null;
			object defaultBorderWidth = null;
			object specificBorderWidth = null;
			object specificBorderWidth2 = null;
			if (style != null)
			{
				defaultBorderStyle = style[5];
				specificBorderStyle = style[6];
				specificBorderStyle2 = style[7];
				defaultBorderWidth = style[10];
				specificBorderWidth = style[11];
				specificBorderWidth2 = style[12];
			}
			if (repItemCol != null && repItemCol.Length != 0)
			{
				PageTableLayout pageTableLayout = null;
				PageTableLayout.GenerateTableLayout(repItemCol, dxParent, dyParent, 0f, out pageTableLayout, expandLayout, this.m_rplReport.ConsumeContainerWhitespace);
				if (pageTableLayout == null)
				{
					return result;
				}
				if (pageTableLayout.BandTable && this.m_allowBandTable && layoutState == SharedListLayoutState.None && (!this.m_renderTableHeight || pageTableLayout.NrRows == 1))
				{
					if (omittedHeaders != null)
					{
						for (int i = 0; i < omittedHeaders.Count; i++)
						{
							if (omittedHeaders[i].GroupLabel != null)
							{
								this.RenderNavigationId(omittedHeaders[i].UniqueName);
							}
						}
					}
					int borderContext2 = 0;
					int j;
					for (j = 0; j < pageTableLayout.NrRows - 1; j++)
					{
						if (borderContext > 0)
						{
							borderContext2 = HTML4Renderer.GetNewContext(borderContext, j + 1, 1, pageTableLayout.NrRows, 1);
						}
						this.RenderCellItem(pageTableLayout.GetCell(j), borderContext2, false);
					}
					if (borderContext > 0)
					{
						borderContext2 = HTML4Renderer.GetNewContext(borderContext, j + 1, 1, pageTableLayout.NrRows, 1);
					}
					this.RenderCellItem(pageTableLayout.GetCell(j), borderContext2, false);
					return result;
				}
				this.m_allowBandTable = true;
				bool flag = false;
				bool renderHeight = true;
				bool flag2 = expandLayout;
				int num2 = pageTableLayout.NrCols;
				if (!flag2)
				{
					flag2 = pageTableLayout.AreSpansInColOne();
				}
				if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.Start)
				{
					this.WriteStream(HTML4Renderer.m_openTable);
					this.WriteStream(HTML4Renderer.m_zeroBorder);
					if (flag2)
					{
						num2++;
					}
					if (!this.m_deviceInfo.IsBrowserGeckoEngine)
					{
						this.WriteStream(HTML4Renderer.m_cols);
						this.WriteStream(num2.ToString(CultureInfo.InvariantCulture));
						this.WriteStream(HTML4Renderer.m_quote);
					}
					this.RenderReportLanguage();
					if (this.m_useInlineStyle)
					{
						this.OpenStyle();
						this.WriteStream(HTML4Renderer.m_borderCollapse);
						if (expandLayout)
						{
							this.WriteStream(HTML4Renderer.m_semiColon);
							this.WriteStream(HTML4Renderer.m_styleHeight);
							this.WriteStream(HTML4Renderer.m_percent);
						}
					}
					else
					{
						this.ClassLayoutBorder();
						if (expandLayout)
						{
							this.WriteStream(HTML4Renderer.m_space);
							this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
							this.WriteStream(HTML4Renderer.m_percentHeight);
						}
						this.WriteStream(HTML4Renderer.m_quote);
					}
					if (this.m_renderTableHeight)
					{
						if (this.m_isStyleOpen)
						{
							this.WriteStream(HTML4Renderer.m_semiColon);
						}
						else
						{
							this.OpenStyle();
						}
						this.WriteStream(HTML4Renderer.m_styleHeight);
						this.WriteDStream(dyParent);
						this.WriteStream(HTML4Renderer.m_mm);
						this.m_renderTableHeight = false;
					}
					if (this.m_deviceInfo.OutlookCompat || this.m_deviceInfo.IsBrowserSafari)
					{
						if (this.m_isStyleOpen)
						{
							this.WriteStream(HTML4Renderer.m_semiColon);
						}
						else
						{
							this.OpenStyle();
						}
						this.WriteStream(HTML4Renderer.m_styleWidth);
						float num3 = dxParent;
						if (num3 > 0.0)
						{
							num3 = this.SubtractBorderStyles(num3, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
							num3 = this.SubtractBorderStyles(num3, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
							if (num3 < 0.0)
							{
								num3 = 1f;
							}
						}
						this.WriteStream(num3);
						this.WriteStream(HTML4Renderer.m_mm);
					}
					this.CloseStyle(true);
					this.WriteStream(HTML4Renderer.m_closeBracket);
					if (pageTableLayout.NrCols > 1)
					{
						flag = pageTableLayout.NeedExtraRow();
						if (flag)
						{
							this.WriteStream(HTML4Renderer.m_openTR);
							this.WriteStream(HTML4Renderer.m_zeroHeight);
							this.WriteStream(HTML4Renderer.m_closeBracket);
							if (flag2)
							{
								this.WriteStream(HTML4Renderer.m_openTD);
								this.WriteStream(HTML4Renderer.m_openStyle);
								this.WriteStream(HTML4Renderer.m_styleWidth);
								this.WriteStream("0");
								this.WriteStream(HTML4Renderer.m_px);
								this.WriteStream(HTML4Renderer.m_closeQuote);
								this.WriteStream(HTML4Renderer.m_closeTD);
							}
							for (num = 0; num < pageTableLayout.NrCols; num++)
							{
								this.WriteStream(HTML4Renderer.m_openTD);
								this.WriteStream(HTML4Renderer.m_openStyle);
								this.WriteStream(HTML4Renderer.m_styleWidth);
								float num4 = pageTableLayout.GetCell(num).DXValue.Value;
								if (num4 > 0.0)
								{
									if (num == 0)
									{
										num4 = this.SubtractBorderStyles(num4, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
									}
									if (num == pageTableLayout.NrCols - 1)
									{
										num4 = this.SubtractBorderStyles(num4, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
									}
									if (num4 <= 0.0)
									{
										num4 = (float)((this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE) ? 1.0 : pageTableLayout.GetCell(num).DXValue.Value);
									}
								}
								this.WriteDStream(num4);
								this.WriteStream(HTML4Renderer.m_mm);
								this.WriteStream(HTML4Renderer.m_semiColon);
								this.WriteStream(HTML4Renderer.m_styleMinWidth);
								this.WriteDStream(num4);
								this.WriteStream(HTML4Renderer.m_mm);
								this.WriteStream(HTML4Renderer.m_closeQuote);
								this.WriteStream(HTML4Renderer.m_closeTD);
							}
							this.WriteStream(HTML4Renderer.m_closeTR);
						}
					}
				}
				this.GenerateTableLayoutContent(pageTableLayout, repItemCol, flag, flag2, renderHeight, borderContext, expandLayout, layoutState, omittedHeaders, style);
				if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.End)
				{
					if (expandLayout)
					{
						this.WriteStream(HTML4Renderer.m_firstTD);
						this.ClassPercentHeight();
						this.WriteStream(HTML4Renderer.m_cols);
						this.WriteStream(num2.ToString(CultureInfo.InvariantCulture));
						this.WriteStream(HTML4Renderer.m_closeQuote);
						this.WriteStream(HTML4Renderer.m_lastTD);
					}
					this.WriteStreamCR(HTML4Renderer.m_closeTable);
				}
				return result;
			}
			if (omittedHeaders != null)
			{
				for (int k = 0; k < omittedHeaders.Count; k++)
				{
					if (omittedHeaders[k].GroupLabel != null)
					{
						this.RenderNavigationId(omittedHeaders[k].UniqueName);
					}
				}
			}
			return result;
		}

		protected void RenderZoom()
		{
			if (this.m_deviceInfo.Zoom != 100)
			{
				this.WriteStream(HTML4Renderer.m_openStyle);
				this.WriteStream("zoom:");
				this.WriteStream(this.m_deviceInfo.Zoom.ToString(CultureInfo.InvariantCulture));
				this.WriteStream("%\"");
			}
		}

		protected void PredefinedStyles()
		{
			HTML4Renderer.PredefinedStyles(this.m_deviceInfo, this, this.m_styleClassPrefix);
		}

		internal static void PredefinedStyles(DeviceInfo m_deviceInfo, IHtmlRenderer writer)
		{
			HTML4Renderer.PredefinedStyles(m_deviceInfo, writer, null);
		}

		internal static void PredefinedStyles(DeviceInfo deviceInfo, IHtmlRenderer writer, byte[] classStylePrefix)
		{
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentSizes);
			writer.WriteStream(HTML4Renderer.m_styleHeight);
			writer.WriteStream(HTML4Renderer.m_percent);
			writer.WriteStream(HTML4Renderer.m_semiColon);
			writer.WriteStream(HTML4Renderer.m_styleWidth);
			writer.WriteStream(HTML4Renderer.m_percent);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentSizesOverflow);
			writer.WriteStream(HTML4Renderer.m_styleHeight);
			writer.WriteStream(HTML4Renderer.m_percent);
			writer.WriteStream(HTML4Renderer.m_semiColon);
			writer.WriteStream(HTML4Renderer.m_styleWidth);
			writer.WriteStream(HTML4Renderer.m_percent);
			writer.WriteStream(HTML4Renderer.m_semiColon);
			writer.WriteStream(HTML4Renderer.m_overflowHidden);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentHeight);
			writer.WriteStream(HTML4Renderer.m_styleHeight);
			writer.WriteStream(HTML4Renderer.m_percent);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_ignoreBorder);
			writer.WriteStream(HTML4Renderer.m_borderStyle);
			writer.WriteStream(HTML4Renderer.m_none);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_ignoreBorderL);
			writer.WriteStream(HTML4Renderer.m_borderLeftStyle);
			writer.WriteStream(HTML4Renderer.m_none);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_ignoreBorderR);
			writer.WriteStream(HTML4Renderer.m_borderRightStyle);
			writer.WriteStream(HTML4Renderer.m_none);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_ignoreBorderT);
			writer.WriteStream(HTML4Renderer.m_borderTopStyle);
			writer.WriteStream(HTML4Renderer.m_none);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_ignoreBorderB);
			writer.WriteStream(HTML4Renderer.m_borderBottomStyle);
			writer.WriteStream(HTML4Renderer.m_none);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_layoutBorder);
			writer.WriteStream(HTML4Renderer.m_borderCollapse);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_layoutFixed);
			writer.WriteStream(HTML4Renderer.m_borderCollapse);
			writer.WriteStream(HTML4Renderer.m_semiColon);
			writer.WriteStream(HTML4Renderer.m_tableLayoutFixed);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentWidthOverflow);
			writer.WriteStream(HTML4Renderer.m_styleWidth);
			writer.WriteStream(HTML4Renderer.m_percent);
			writer.WriteStream(HTML4Renderer.m_semiColon);
			writer.WriteStream(HTML4Renderer.m_overflowXHidden);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_popupAction);
			writer.WriteStream("position:absolute;display:none;background-color:white;border:1px solid black;");
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_styleAction);
			writer.WriteStream("text-decoration:none;color:black;cursor:pointer;");
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_emptyTextBox);
			writer.WriteStream(HTML4Renderer.m_fontSize);
			writer.WriteStream(deviceInfo.IsBrowserGeckoEngine ? HTML4Renderer.m_smallPoint : HTML4Renderer.m_zeroPoint);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_rtlEmbed);
			writer.WriteStream(HTML4Renderer.m_direction);
			writer.WriteStream("RTL;");
			writer.WriteStream(HTML4Renderer.m_unicodeBiDi);
			writer.WriteStream(EnumStrings.GetValue(RPLFormat.UnicodeBiDiTypes.Embed));
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_noVerticalMarginClassName);
			writer.WriteStream(HTML4Renderer.m_marginTop);
			writer.WriteStream(HTML4Renderer.m_zeroPoint);
			writer.WriteStream(HTML4Renderer.m_semiColon);
			writer.WriteStream(HTML4Renderer.m_marginBottom);
			writer.WriteStream(HTML4Renderer.m_zeroPoint);
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentSizeInlineTable);
			writer.WriteStream(HTML4Renderer.m_styleHeight);
			writer.WriteStream(HTML4Renderer.m_percent);
			writer.WriteStream(HTML4Renderer.m_semiColon);
			writer.WriteStream(HTML4Renderer.m_styleWidth);
			writer.WriteStream(HTML4Renderer.m_percent);
			writer.WriteStream(HTML4Renderer.m_semiColon);
			writer.WriteStream("display:inline-table");
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			HTML4Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTML4Renderer.m_percentHeightInlineTable);
			writer.WriteStream(HTML4Renderer.m_styleHeight);
			writer.WriteStream(HTML4Renderer.m_percent);
			writer.WriteStream(HTML4Renderer.m_semiColon);
			writer.WriteStream("display:inline-table");
			writer.WriteStream(HTML4Renderer.m_closeAccol);
			if (classStylePrefix != null)
			{
				writer.WriteStream(classStylePrefix);
			}
			writer.WriteStream(" * { ");
			string value = null;
			if (deviceInfo.IsBrowserSafari)
			{
				value = "-webkit-";
			}
			else if (deviceInfo.IsBrowserGeckoEngine)
			{
				value = "-moz-";
			}
			if (!string.IsNullOrEmpty(value))
			{
				writer.WriteStream(value);
				writer.WriteStream("box-sizing: border-box; ");
			}
			writer.WriteStream("box-sizing: border-box }");
		}

		private static void StartPredefinedStyleClass(DeviceInfo deviceInfo, IHtmlRenderer writer, byte[] classStylePrefix, byte[] className)
		{
			if (classStylePrefix != null)
			{
				writer.WriteStream(classStylePrefix);
			}
			writer.WriteStream(HTML4Renderer.m_dot);
			writer.WriteStream(deviceInfo.HtmlPrefixId);
			writer.WriteStream(className);
			writer.WriteStream(HTML4Renderer.m_openAccol);
		}

		private void CheckBodyStyle()
		{
			RPLElementStyle style = this.m_pageContent.PageLayout.Style;
			string text = (string)style[34];
			this.m_pageHasStyle = (text != null || style[33] != null || this.ReportPageHasBorder(style, text));
		}

		private bool ReportPageBorder(IRPLStyle pageStyle, Border border, string backgroundColor)
		{
			byte b = 0;
			byte b2 = 0;
			byte b3 = 0;
			bool result = false;
			string text = null;
			string text2 = null;
			switch (border)
			{
			case Border.All:
				b = 10;
				b2 = 5;
				b3 = 0;
				break;
			case Border.Bottom:
				b = 14;
				b2 = 9;
				b3 = 4;
				break;
			case Border.Left:
				b = 11;
				b2 = 6;
				b3 = 1;
				break;
			case Border.Right:
				b = 12;
				b2 = 7;
				b3 = 2;
				break;
			default:
				b = 13;
				b2 = 8;
				b3 = 3;
				break;
			}
			object obj = pageStyle[b2];
			if (obj != null && (RPLFormat.BorderStyles)obj != 0)
			{
				text = (string)pageStyle[b];
				if (text != null && new RPLReportSize(text).ToMillimeters() > 0.0)
				{
					text2 = (string)pageStyle[b3];
					if (text2 != backgroundColor)
					{
						result = true;
					}
				}
			}
			return result;
		}

		private void BorderBottomAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				this.WriteStream(HTML4Renderer.m_borderBottomColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				this.WriteStream(HTML4Renderer.m_borderBottomStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				this.WriteStream(HTML4Renderer.m_borderBottomWidth);
			}
		}

		private void BorderLeftAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				this.WriteStream(HTML4Renderer.m_borderLeftColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				this.WriteStream(HTML4Renderer.m_borderLeftStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				this.WriteStream(HTML4Renderer.m_borderLeftWidth);
			}
		}

		private void BorderRightAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				this.WriteStream(HTML4Renderer.m_borderRightColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				this.WriteStream(HTML4Renderer.m_borderRightStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				this.WriteStream(HTML4Renderer.m_borderRightWidth);
			}
		}

		private void BorderTopAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				this.WriteStream(HTML4Renderer.m_borderTopColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				this.WriteStream(HTML4Renderer.m_borderTopStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				this.WriteStream(HTML4Renderer.m_borderTopWidth);
			}
		}

		private void BorderAllAtribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				this.WriteStream(HTML4Renderer.m_borderColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				this.WriteStream(HTML4Renderer.m_borderStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				this.WriteStream(HTML4Renderer.m_borderWidth);
			}
		}

		private void RenderBorder(object styleAttribute, Border border, BorderAttribute borderAttribute)
		{
			if (styleAttribute != null)
			{
				switch (border)
				{
				case Border.All:
					this.BorderAllAtribute(borderAttribute);
					break;
				case Border.Bottom:
					this.BorderBottomAttribute(borderAttribute);
					break;
				case Border.Right:
					this.BorderRightAttribute(borderAttribute);
					break;
				case Border.Top:
					this.BorderTopAttribute(borderAttribute);
					break;
				default:
					this.BorderLeftAttribute(borderAttribute);
					break;
				}
				this.WriteStream(styleAttribute);
				this.WriteStream(HTML4Renderer.m_semiColon);
			}
		}

		private void RenderBorderStyle(object width, object style, object color, Border border)
		{
			if (width == null && color == null && style == null)
			{
				return;
			}
			if (width != null && color != null && style != null)
			{
				string value = EnumStrings.GetValue((RPLFormat.BorderStyles)style);
				switch (border)
				{
				case Border.All:
					this.WriteStream(HTML4Renderer.m_border);
					break;
				case Border.Bottom:
					this.WriteStream(HTML4Renderer.m_borderBottom);
					break;
				case Border.Left:
					this.WriteStream(HTML4Renderer.m_borderLeft);
					break;
				case Border.Right:
					this.WriteStream(HTML4Renderer.m_borderRight);
					break;
				default:
					this.WriteStream(HTML4Renderer.m_borderTop);
					break;
				}
				this.WriteStream(width);
				this.WriteStream(HTML4Renderer.m_space);
				this.WriteStream(value);
				this.WriteStream(HTML4Renderer.m_space);
				this.WriteStream(color);
				this.WriteStream(HTML4Renderer.m_semiColon);
			}
			else
			{
				this.RenderBorder(color, border, BorderAttribute.BorderColor);
				if (style != null)
				{
					string value2 = EnumStrings.GetValue((RPLFormat.BorderStyles)style);
					this.RenderBorder(value2, border, BorderAttribute.BorderStyle);
				}
				this.RenderBorder(width, border, BorderAttribute.BorderWidth);
			}
		}

		protected bool BorderInstance(IRPLStyle reportItemStyle, object defWidth, object defStyle, object defColor, ref object borderWidth, ref object borderStyle, ref object borderColor, Border border)
		{
			byte styleName = 0;
			byte styleName2 = 0;
			byte styleName3 = 0;
			switch (border)
			{
			case Border.Bottom:
				styleName = 14;
				styleName2 = 9;
				styleName3 = 4;
				break;
			case Border.Left:
				styleName = 11;
				styleName2 = 6;
				styleName3 = 1;
				break;
			case Border.Right:
				styleName = 12;
				styleName2 = 7;
				styleName3 = 2;
				break;
			case Border.Top:
				styleName = 13;
				styleName2 = 8;
				styleName3 = 3;
				break;
			}
			if (reportItemStyle != null)
			{
				borderStyle = reportItemStyle[styleName2];
			}
			if (borderStyle == null)
			{
				borderStyle = defStyle;
			}
			if (borderStyle != null && (RPLFormat.BorderStyles)borderStyle == RPLFormat.BorderStyles.None)
			{
				return false;
			}
			object obj = reportItemStyle[styleName];
			if (obj == null)
			{
				borderWidth = defWidth;
			}
			else
			{
				borderWidth = obj;
			}
			object obj2 = reportItemStyle[styleName3];
			if (obj2 == null)
			{
				borderColor = defColor;
			}
			else
			{
				borderColor = obj2;
			}
			if (borderStyle == null && obj == null)
			{
				return obj2 != null;
			}
			return true;
		}

		private bool RenderBorderInstance(IRPLStyle reportItemStyle, object defWidth, object defStyle, object defColor, Border border)
		{
			object width = null;
			object color = null;
			object style = null;
			bool flag = this.BorderInstance(reportItemStyle, defWidth, defStyle, defColor, ref width, ref style, ref color, border);
			if (flag)
			{
				this.RenderBorderStyle(width, style, color, border);
			}
			return flag;
		}

		private bool OnlyGeneralBorder(IRPLStyle style)
		{
			bool result = true;
			if (style[1] != null || style[11] != null || style[6] != null || style[3] != null || style[13] != null || style[8] != null || style[2] != null || style[12] != null || style[7] != null || style[4] != null || style[14] != null || style[9] != null)
			{
				result = false;
			}
			return result;
		}

		protected string CreateImageStream(RPLImageData image)
		{
			if (image.ImageName == null)
			{
				return null;
			}
			if (image.IsShared && this.m_images.ContainsKey(image.ImageName))
			{
				return image.ImageName;
			}
			if (this.m_createSecondaryStreams == SecondaryStreams.Embedded)
			{
				Stream stream = this.CreateStream(image.ImageName, string.Empty, null, image.ImageMimeType, false, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateAndRegister);
				long imageDataOffset = image.ImageDataOffset;
				if (imageDataOffset >= 0)
				{
					this.m_rplReport.GetImage(imageDataOffset, stream);
				}
				else if (image.ImageData != null)
				{
					stream.Write(image.ImageData, 0, image.ImageData.Length);
				}
			}
			if (image.IsShared)
			{
				this.m_images.Add(image.ImageName, null);
			}
			return image.ImageName;
		}

		private void RenderAtStart(RPLTextBoxProps textBoxProps, IRPLStyle style, bool renderSort, bool renderToggle)
		{
			if (!renderSort && !renderToggle)
			{
				return;
			}
			object obj = style[26];
			RPLFormat.VerticalAlignments verticalAlignments = RPLFormat.VerticalAlignments.Top;
			if (obj != null)
			{
				verticalAlignments = (RPLFormat.VerticalAlignments)obj;
			}
			if (HTML4Renderer.IsWritingModeVertical(style) && this.m_deviceInfo.IsBrowserIE)
			{
				this.WriteStream(HTML4Renderer.m_openStyle);
				this.WriteStream(HTML4Renderer.m_textAlign);
				switch (verticalAlignments)
				{
				case RPLFormat.VerticalAlignments.Top:
					this.WriteStream(HTML4Renderer.m_rightValue);
					break;
				case RPLFormat.VerticalAlignments.Middle:
					this.WriteStream(HTML4Renderer.m_centerValue);
					break;
				case RPLFormat.VerticalAlignments.Bottom:
					this.WriteStream(HTML4Renderer.m_leftValue);
					break;
				}
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				if (renderSort)
				{
					this.RenderSortImage(textBoxProps);
				}
				if (renderToggle)
				{
					this.RenderToggleImage(textBoxProps);
				}
				this.WriteStream(HTML4Renderer.m_closeTD);
				this.WriteStream(HTML4Renderer.m_closeTR);
				this.WriteStream(HTML4Renderer.m_firstTD);
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_openStyle);
				this.WriteStream(HTML4Renderer.m_verticalAlign);
				this.WriteStream(EnumStrings.GetValue(verticalAlignments));
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				if (renderSort)
				{
					this.RenderSortImage(textBoxProps);
				}
				if (renderToggle)
				{
					this.RenderToggleImage(textBoxProps);
				}
				this.WriteStream(HTML4Renderer.m_closeTD);
				this.WriteStream(HTML4Renderer.m_openTD);
			}
		}

		private void RenderAtEnd(RPLTextBoxProps textBoxProps, IRPLStyle style, bool renderSort, bool renderToggle)
		{
			if (!renderSort && !renderToggle)
			{
				return;
			}
			object obj = style[26];
			RPLFormat.VerticalAlignments verticalAlignments = RPLFormat.VerticalAlignments.Top;
			if (obj != null)
			{
				verticalAlignments = (RPLFormat.VerticalAlignments)obj;
			}
			this.WriteStream(HTML4Renderer.m_closeTD);
			if (HTML4Renderer.IsWritingModeVertical(style) && this.m_deviceInfo.IsBrowserIE)
			{
				this.WriteStream(HTML4Renderer.m_closeTR);
				this.WriteStream(HTML4Renderer.m_firstTD);
				this.WriteStream(HTML4Renderer.m_openStyle);
				this.WriteStream(HTML4Renderer.m_textAlign);
				switch (verticalAlignments)
				{
				case RPLFormat.VerticalAlignments.Top:
					this.WriteStream(HTML4Renderer.m_rightValue);
					break;
				case RPLFormat.VerticalAlignments.Middle:
					this.WriteStream(HTML4Renderer.m_centerValue);
					break;
				case RPLFormat.VerticalAlignments.Bottom:
					this.WriteStream(HTML4Renderer.m_leftValue);
					break;
				}
				this.WriteStream(HTML4Renderer.m_quote);
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_openTD);
				this.WriteStream(HTML4Renderer.m_openStyle);
				this.WriteStream(HTML4Renderer.m_verticalAlign);
				this.WriteStream(EnumStrings.GetValue(verticalAlignments));
				this.WriteStream(HTML4Renderer.m_quote);
			}
			this.WriteStream(HTML4Renderer.m_closeBracket);
			if (renderSort)
			{
				this.RenderSortImage(textBoxProps);
			}
			if (renderToggle)
			{
				this.RenderToggleImage(textBoxProps);
			}
		}

		private bool RenderHyperlink(RPLAction action, RPLFormat.TextDecorations textDec, string color)
		{
			this.WriteStream(HTML4Renderer.m_openA);
			this.RenderTabIndex();
			this.RenderActionHref(action, textDec, color);
			this.WriteStream(HTML4Renderer.m_closeBracket);
			return true;
		}

		private void RenderTabIndex()
		{
			this.WriteStream(HTML4Renderer.m_tabIndex);
			this.WriteStream(++this.m_tabIndexNum);
			this.WriteStream(HTML4Renderer.m_quote);
		}

		private bool HasAction(RPLAction action)
		{
			if (action.BookmarkLink == null && action.DrillthroughId == null && action.DrillthroughUrl == null)
			{
				return action.Hyperlink != null;
			}
			return true;
		}

		private bool HasAction(RPLActionInfo actionInfo)
		{
			if (actionInfo != null && actionInfo.Actions != null)
			{
				return this.HasAction(actionInfo.Actions[0]);
			}
			return false;
		}

		protected abstract void RenderInteractionAction(RPLAction action, ref bool hasHref);

		private bool RenderActionHref(RPLAction action, RPLFormat.TextDecorations textDec, string color)
		{
			bool result = false;
			if (action.Hyperlink != null)
			{
				this.WriteStream(HTML4Renderer.m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + HTML4Renderer.m_quoteString);
				result = true;
			}
			else
			{
				this.RenderInteractionAction(action, ref result);
			}
			if (textDec != RPLFormat.TextDecorations.Underline)
			{
				this.OpenStyle();
				this.WriteStream(HTML4Renderer.m_textDecoration);
				this.WriteStream(HTML4Renderer.m_none);
				this.WriteStream(HTML4Renderer.m_semiColon);
			}
			if (color != null)
			{
				this.OpenStyle();
				this.WriteStream(HTML4Renderer.m_color);
				this.WriteStream(color);
			}
			this.CloseStyle(true);
			if (this.m_deviceInfo.LinkTarget != null)
			{
				this.WriteStream(HTML4Renderer.m_target);
				this.WriteStream(this.m_deviceInfo.LinkTarget);
				this.WriteStream(HTML4Renderer.m_quote);
			}
			return result;
		}

		protected void RenderControlActionScript(RPLAction action)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = null;
			if (action.DrillthroughId != null)
			{
				HTML4Renderer.QuoteString(stringBuilder, action.DrillthroughId);
				text = "Drillthrough";
			}
			else
			{
				HTML4Renderer.QuoteString(stringBuilder, action.BookmarkLink);
				text = "Bookmark";
			}
			this.RenderOnClickActionScript(text, stringBuilder.ToString());
		}

		internal static bool IsDirectionRTL(IRPLStyle style)
		{
			object obj = style[29];
			if (obj != null)
			{
				return (RPLFormat.Directions)obj == RPLFormat.Directions.RTL;
			}
			return false;
		}

		internal static bool IsWritingModeVertical(IRPLStyle style)
		{
			if (style == null)
			{
				return false;
			}
			object obj = style[30];
			if (obj != null)
			{
				return HTML4Renderer.IsWritingModeVertical((RPLFormat.WritingModes)obj);
			}
			return false;
		}

		internal static bool IsWritingModeVertical(RPLFormat.WritingModes writingMode)
		{
			if (writingMode != RPLFormat.WritingModes.Vertical)
			{
				return writingMode == RPLFormat.WritingModes.Rotate270;
			}
			return true;
		}

		internal static bool HasHorizontalPaddingStyles(IRPLStyle style)
		{
			if (style != null)
			{
				if (style[15] == null)
				{
					return style[16] != null;
				}
				return true;
			}
			return false;
		}

		private void PercentSizes()
		{
			this.WriteStream(HTML4Renderer.m_openStyle);
			this.WriteStream(HTML4Renderer.m_styleHeight);
			this.WriteStream(HTML4Renderer.m_percent);
			this.WriteStream(HTML4Renderer.m_semiColon);
			this.WriteStream(HTML4Renderer.m_styleWidth);
			this.WriteStream(HTML4Renderer.m_percent);
			this.WriteStream(HTML4Renderer.m_quote);
		}

		private void PercentSizesOverflow()
		{
			this.WriteStream(HTML4Renderer.m_openStyle);
			this.WriteStream(HTML4Renderer.m_styleHeight);
			this.WriteStream(HTML4Renderer.m_percent);
			this.WriteStream(HTML4Renderer.m_semiColon);
			this.WriteStream(HTML4Renderer.m_styleWidth);
			this.WriteStream(HTML4Renderer.m_percent);
			this.WriteStream(HTML4Renderer.m_semiColon);
			this.WriteStream(HTML4Renderer.m_overflowHidden);
			this.WriteStream(HTML4Renderer.m_quote);
		}

		private void ClassLayoutBorder()
		{
			this.WriteClassName(HTML4Renderer.m_layoutBorder, HTML4Renderer.m_classLayoutBorder);
		}

		private void ClassPercentSizes()
		{
			this.WriteClassName(HTML4Renderer.m_percentSizes, HTML4Renderer.m_classPercentSizes);
		}

		private void ClassPercentSizesOverflow()
		{
			this.WriteClassName(HTML4Renderer.m_percentSizesOverflow, HTML4Renderer.m_classPercentSizesOverflow);
		}

		private void ClassPercentHeight()
		{
			this.WriteClassName(HTML4Renderer.m_percentHeight, HTML4Renderer.m_classPercentHeight);
		}

		private void RenderLanguage(string language)
		{
			if (!string.IsNullOrEmpty(language))
			{
				this.WriteStream(HTML4Renderer.m_language);
				this.WriteAttrEncoded(language);
				this.WriteStream(HTML4Renderer.m_quote);
			}
		}

		private void RenderReportLanguage()
		{
			this.RenderLanguage(this.m_contextLanguage);
		}

		private bool InitFixedColumnHeaders(RPLTablix tablix, string tablixID, TablixFixedHeaderStorage storage)
		{
			for (int i = 0; i < tablix.RowHeights.Length; i++)
			{
				if (tablix.FixedRow(i))
				{
					storage.HtmlId = tablixID;
					storage.ColumnHeaders = new List<string>();
					return true;
				}
			}
			return false;
		}

		private bool InitFixedRowHeaders(RPLTablix tablix, string tablixID, TablixFixedHeaderStorage storage)
		{
			for (int i = 0; i < tablix.ColumnWidths.Length; i++)
			{
				if (tablix.FixedColumns[i])
				{
					storage.HtmlId = tablixID;
					storage.RowHeaders = new List<string>();
					return true;
				}
			}
			return false;
		}

		private void RenderVMLLine(RPLLine line, RPLItemMeasurement measurement, StyleContext styleContext)
		{
			if (!this.m_hasSlantedLines)
			{
				this.WriteStream("<?XML:NAMESPACE PREFIX=v /><?IMPORT NAMESPACE=\"v\" IMPLEMENTATION=\"#default#VML\" />");
				this.m_hasSlantedLines = true;
			}
			this.WriteStream(HTML4Renderer.m_openVGroup);
			this.WriteStream(HTML4Renderer.m_openStyle);
			this.WriteStream(HTML4Renderer.m_styleWidth);
			if (styleContext.InTablix)
			{
				this.WriteStream(HTML4Renderer.m_percent);
				this.WriteStream(HTML4Renderer.m_semiColon);
				this.WriteStream(HTML4Renderer.m_styleHeight);
				this.WriteStream(HTML4Renderer.m_percent);
			}
			else
			{
				this.WriteRSStream(measurement.Width);
				this.WriteStream(HTML4Renderer.m_semiColon);
				this.WriteStream(HTML4Renderer.m_styleHeight);
				this.WriteRSStream(measurement.Height);
			}
			this.WriteStream(HTML4Renderer.m_closeQuote);
			this.WriteStream(HTML4Renderer.m_openVLine);
			if (((RPLLinePropsDef)line.ElementProps.Definition).Slant)
			{
				this.WriteStream(HTML4Renderer.m_rightSlant);
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_leftSlant);
			}
			IRPLStyle style = line.ElementProps.Style;
			string text = (string)style[0];
			string text2 = (string)style[10];
			if (text != null && text2 != null)
			{
				int value = new RPLReportColor(text).ToColor().ToArgb() & 0xFFFFFF;
				this.WriteStream(HTML4Renderer.m_strokeColor);
				this.WriteStream("#");
				this.WriteStream(Convert.ToString(value, 16));
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_strokeWeight);
				this.WriteStream(text2);
				this.WriteStream(HTML4Renderer.m_closeQuote);
			}
			string theString = "solid";
			string text3 = null;
			object obj = style[5];
			if (obj != null)
			{
				string value2 = EnumStrings.GetValue((RPLFormat.BorderStyles)obj);
				if (string.CompareOrdinal(value2, "dashed") == 0)
				{
					theString = "dash";
				}
				else if (string.CompareOrdinal(value2, "dotted") == 0)
				{
					theString = "dot";
				}
				if (string.CompareOrdinal(value2, "double") == 0)
				{
					text3 = "thinthin";
				}
			}
			this.WriteStream(HTML4Renderer.m_dashStyle);
			this.WriteStream(theString);
			if (text3 != null)
			{
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_slineStyle);
				this.WriteStream(text3);
			}
			this.WriteStream(HTML4Renderer.m_quote);
			this.WriteStream(HTML4Renderer.m_closeTag);
			this.WriteStreamCR(HTML4Renderer.m_closeVGroup);
		}

		private List<string> RenderTableCellBorder(PageTableCell currCell, Hashtable renderedLines)
		{
			RPLLine rPLLine = null;
			List<string> list = new List<string>(4);
			if (this.m_isStyleOpen)
			{
				this.WriteStream(HTML4Renderer.m_semiColon);
			}
			else
			{
				this.OpenStyle();
			}
			this.WriteStream(HTML4Renderer.m_zeroBorderWidth);
			rPLLine = currCell.BorderLeft;
			if (rPLLine != null)
			{
				this.WriteStream(HTML4Renderer.m_semiColon);
				this.WriteStream(HTML4Renderer.m_borderLeft);
				this.RenderBorderLine(rPLLine);
				this.CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderRight;
			if (rPLLine != null)
			{
				this.WriteStream(HTML4Renderer.m_semiColon);
				this.WriteStream(HTML4Renderer.m_borderRight);
				this.RenderBorderLine(rPLLine);
				this.CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderTop;
			if (rPLLine != null)
			{
				this.WriteStream(HTML4Renderer.m_semiColon);
				this.WriteStream(HTML4Renderer.m_borderTop);
				this.RenderBorderLine(rPLLine);
				this.CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderBottom;
			if (rPLLine != null)
			{
				this.WriteStream(HTML4Renderer.m_semiColon);
				this.WriteStream(HTML4Renderer.m_borderBottom);
				this.RenderBorderLine(rPLLine);
				this.CheckForLineID(rPLLine, list, renderedLines);
			}
			return list;
		}

		private void CheckForLineID(RPLLine line, List<string> lineIDs, Hashtable renderedLines)
		{
			RPLElementProps elementProps = line.ElementProps;
			string uniqueName = elementProps.UniqueName;
			if (!renderedLines.ContainsKey(uniqueName))
			{
				if (this.NeedReportItemId(line, elementProps))
				{
					lineIDs.Add(elementProps.UniqueName);
				}
				renderedLines.Add(uniqueName, uniqueName);
			}
		}

		private int GenerateTableLayoutContent(PageTableLayout rgTableGrid, RPLItemMeasurement[] repItemCol, bool bfZeroRowReq, bool bfZeroColReq, bool renderHeight, int borderContext, bool layoutExpand, SharedListLayoutState layoutState, List<RPLTablixMemberCell> omittedHeaders, IRPLStyle style)
		{
			int num = 0;
			int i = 0;
			int num2 = 1;
			int num3 = 1;
			int num4 = 0;
			int num5 = 0;
			bool flag = false;
			bool flag2 = true;
			PageTableCell pageTableCell = null;
			PageTableCell pageTableCell2 = null;
			Hashtable renderedLines = new Hashtable();
			int nrRows = rgTableGrid.NrRows;
			int nrCols = rgTableGrid.NrCols;
			int num6 = 0;
			int result = 0;
			bool flag3 = true;
			object defaultBorderStyle = null;
			object specificBorderStyle = null;
			object specificBorderStyle2 = null;
			object specificBorderStyle3 = null;
			object specificBorderStyle4 = null;
			object defaultBorderWidth = null;
			object specificBorderWidth = null;
			object specificBorderWidth2 = null;
			object specificBorderWidth3 = null;
			object specificBorderWidth4 = null;
			if (style != null)
			{
				defaultBorderStyle = style[5];
				specificBorderStyle = style[6];
				specificBorderStyle2 = style[7];
				specificBorderStyle3 = style[8];
				specificBorderStyle4 = style[9];
				defaultBorderWidth = style[10];
				specificBorderWidth = style[11];
				specificBorderWidth2 = style[12];
				specificBorderWidth3 = style[13];
				specificBorderWidth4 = style[14];
			}
			for (; i < nrRows; i++)
			{
				num4 = nrCols * i;
				pageTableCell = rgTableGrid.GetCell(num4);
				flag = rgTableGrid.EmptyRow(repItemCol, false, num4, renderHeight, ref num5);
				this.WriteStream(HTML4Renderer.m_openTR);
				if (!flag)
				{
					this.WriteStream(HTML4Renderer.m_valign);
					this.WriteStream(HTML4Renderer.m_topValue);
					this.WriteStream(HTML4Renderer.m_quote);
				}
				this.WriteStream(HTML4Renderer.m_closeBracket);
				flag3 = true;
				for (num = 0; num < nrCols; num++)
				{
					int num7 = num + num4;
					bool flag4 = num == 0;
					if (flag4 && bfZeroColReq)
					{
						this.WriteStream(HTML4Renderer.m_openTD);
						if (renderHeight || num5 <= 0)
						{
							this.WriteStream(HTML4Renderer.m_openStyle);
							if (this.m_deviceInfo.OutlookCompat)
							{
								for (int j = 0; j < nrCols; j++)
								{
									pageTableCell2 = rgTableGrid.GetCell(num4 + j);
									if (!pageTableCell2.NeedsRowHeight)
									{
										flag3 = false;
										break;
									}
								}
							}
							if (flag3)
							{
								this.WriteStream(HTML4Renderer.m_styleHeight);
								float num8 = pageTableCell.DYValue.Value;
								if (num8 > 0.0)
								{
									if (i == 0)
									{
										num8 = this.SubtractBorderStyles(num8, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
									}
									if (i == rgTableGrid.NrRows - num2)
									{
										num8 = this.SubtractBorderStyles(num8, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
									}
									if (num8 <= 0.0)
									{
										num8 = (float)((this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE) ? 1.0 : pageTableCell.DYValue.Value);
									}
								}
								this.WriteDStream(num8);
								this.WriteStream(HTML4Renderer.m_mm);
								this.WriteStream(HTML4Renderer.m_semiColon);
							}
							this.WriteStream(HTML4Renderer.m_styleWidth);
							this.WriteDStream(0f);
							this.WriteStream(HTML4Renderer.m_mm);
							this.WriteStream(HTML4Renderer.m_quote);
						}
						else
						{
							this.WriteStream(HTML4Renderer.m_openStyle);
							this.WriteStream(HTML4Renderer.m_styleWidth);
							this.WriteDStream(0f);
							this.WriteStream(HTML4Renderer.m_mm);
							this.WriteStream(HTML4Renderer.m_quote);
						}
						this.WriteStream(HTML4Renderer.m_closeBracket);
						if (omittedHeaders != null)
						{
							for (int k = 0; k < omittedHeaders.Count; k++)
							{
								if (omittedHeaders[k].GroupLabel != null)
								{
									this.RenderNavigationId(omittedHeaders[k].UniqueName);
								}
							}
						}
						this.WriteStream(HTML4Renderer.m_closeTD);
					}
					pageTableCell2 = rgTableGrid.GetCell(num7);
					if (!pageTableCell2.Eaten)
					{
						if (!pageTableCell2.InUse)
						{
							HTML4Renderer.MergeEmptyCells(rgTableGrid, num, i, num4, flag2, pageTableCell2, nrRows, nrCols, num7);
						}
						this.WriteStream(HTML4Renderer.m_openTD);
						num2 = pageTableCell2.RowSpan;
						if (num2 != 1)
						{
							this.WriteStream(HTML4Renderer.m_rowSpan);
							this.WriteStream(num2.ToString(CultureInfo.InvariantCulture));
							this.WriteStream(HTML4Renderer.m_quote);
						}
						if (!flag2 || bfZeroRowReq || layoutState == SharedListLayoutState.Continue || layoutState == SharedListLayoutState.End)
						{
							num3 = pageTableCell2.ColSpan;
							if (num3 != 1)
							{
								this.WriteStream(HTML4Renderer.m_colSpan);
								this.WriteStream(num3.ToString(CultureInfo.InvariantCulture));
								this.WriteStream(HTML4Renderer.m_quote);
							}
						}
						if (flag4 && !bfZeroColReq && (renderHeight || num5 <= 0))
						{
							float num9 = pageTableCell.DYValue.Value;
							if (num9 >= 0.0 && flag3 && (i != nrRows - 1 || !flag || layoutState != 0) && (!this.m_deviceInfo.OutlookCompat || pageTableCell2.NeedsRowHeight))
							{
								this.OpenStyle();
								this.WriteStream(HTML4Renderer.m_styleHeight);
								if (i == 0)
								{
									num9 = this.SubtractBorderStyles(num9, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
								}
								if (i == rgTableGrid.NrRows - num2)
								{
									num9 = this.SubtractBorderStyles(num9, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
								}
								if (num9 <= 0.0)
								{
									num9 = (float)((this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE) ? 1.0 : pageTableCell.DYValue.Value);
								}
								this.WriteDStream(num9);
								this.WriteStream(HTML4Renderer.m_mm);
							}
						}
						if (this.m_deviceInfo.OutlookCompat || (flag2 && !bfZeroRowReq && (layoutState == SharedListLayoutState.Start || layoutState == SharedListLayoutState.None)))
						{
							float num10 = 0f;
							for (int l = 0; l < num3; l++)
							{
								num10 += rgTableGrid.GetCell(num + l).DXValue.Value;
							}
							float num11 = num10;
							if (this.m_isStyleOpen)
							{
								this.WriteStream(HTML4Renderer.m_semiColon);
							}
							else
							{
								this.OpenStyle();
							}
							this.WriteStream(HTML4Renderer.m_styleWidth);
							if (num11 > 0.0)
							{
								if (num == 0)
								{
									num11 = this.SubtractBorderStyles(num11, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
								}
								if (num == rgTableGrid.NrCols - num3)
								{
									num11 = this.SubtractBorderStyles(num11, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
								}
								if (num11 <= 0.0)
								{
									num11 = (float)((this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE) ? 1.0 : num10);
								}
							}
							this.WriteDStream(num11);
							this.WriteStream(HTML4Renderer.m_mm);
							this.WriteStream(HTML4Renderer.m_semiColon);
							this.WriteStream(HTML4Renderer.m_styleMinWidth);
							this.WriteDStream(num11);
							this.WriteStream(HTML4Renderer.m_mm);
							this.WriteStream(HTML4Renderer.m_semiColon);
							if (flag3 && !pageTableCell2.InUse && this.m_deviceInfo.OutlookCompat)
							{
								float num12 = pageTableCell2.DYValue.Value;
								if (num12 < 558.79998779296875)
								{
									this.WriteStream(HTML4Renderer.m_styleHeight);
									if (num12 > 0.0)
									{
										if (i == 0)
										{
											num12 = this.SubtractBorderStyles(num12, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
										}
										if (i == rgTableGrid.NrRows - num2)
										{
											num12 = this.SubtractBorderStyles(num12, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
										}
										if (num12 <= 0.0)
										{
											num12 = (float)((this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE) ? 1.0 : pageTableCell2.DYValue.Value);
										}
									}
									this.WriteDStream(num12);
									this.WriteStream(HTML4Renderer.m_mm);
									this.WriteStream(HTML4Renderer.m_semiColon);
								}
							}
						}
						List<string> list = null;
						if (pageTableCell2.HasBorder)
						{
							list = this.RenderTableCellBorder(pageTableCell2, renderedLines);
						}
						if (this.m_isStyleOpen)
						{
							this.CloseStyle(false);
							this.WriteStream(HTML4Renderer.m_closeQuote);
						}
						else
						{
							this.WriteStream(HTML4Renderer.m_closeBracket);
						}
						if (flag4 && !bfZeroColReq && omittedHeaders != null)
						{
							for (int m = 0; m < omittedHeaders.Count; m++)
							{
								if (omittedHeaders[m].GroupLabel != null)
								{
									this.RenderNavigationId(omittedHeaders[m].UniqueName);
								}
							}
						}
						if (list != null && list.Count > 0)
						{
							for (int n = 0; n < list.Count; n++)
							{
								this.RenderNavigationId(list[n]);
							}
						}
						if (pageTableCell2.InUse)
						{
							int num13 = nrRows - pageTableCell2.RowSpan + 1;
							if (num13 == i + 1 && pageTableCell2.KeepBottomBorder)
							{
								num13++;
							}
							int num14 = nrCols - pageTableCell2.ColSpan + 1;
							if (num14 == num + 1 && pageTableCell2.KeepRightBorder)
							{
								num14++;
							}
							num6 = HTML4Renderer.GetNewContext(borderContext, i + 1, num + 1, num13, num14);
							if ((num6 & 8) > 0 && pageTableCell2.Measurement != null)
							{
								float height = pageTableCell2.Measurement.Height;
								float num15 = pageTableCell2.DYValue.Value;
								for (int num16 = 1; num16 < pageTableCell2.RowSpan; num16++)
								{
									num15 += rgTableGrid.GetCell(num7 + num16 * rgTableGrid.NrCols).DYValue.Value;
								}
								if (height < num15)
								{
									num6 &= -9;
								}
							}
							if ((num6 & 2) > 0 && pageTableCell2.Measurement != null)
							{
								float width = pageTableCell2.Measurement.Width;
								float num17 = pageTableCell2.DXValue.Value;
								for (int num18 = 1; num18 < pageTableCell2.ColSpan; num18++)
								{
									num17 += rgTableGrid.GetCell(num7 + num18).DXValue.Value;
								}
								if (width < num17)
								{
									num6 &= -3;
								}
							}
							this.RenderCellItem(pageTableCell2, num6, layoutExpand);
						}
						else if (!this.m_browserIE && pageTableCell2.HasBorder)
						{
							this.RenderBlankImage();
						}
						this.WriteStream(HTML4Renderer.m_closeTD);
					}
				}
				this.WriteStream(HTML4Renderer.m_closeTR);
				flag2 = false;
				num5--;
			}
			return result;
		}

		private static void MergeEmptyCells(PageTableLayout rgTableGrid, int x, int y, int currRow, bool firstRow, PageTableCell currCell, int numRows, int numCols, int index)
		{
			int num = index + 1;
			int num2 = currRow + numCols;
			if (currCell.BorderLeft == null && !firstRow)
			{
				while (num < num2)
				{
					PageTableCell cell = rgTableGrid.GetCell(num++);
					if (cell.Eaten)
					{
						break;
					}
					if (cell.InUse)
					{
						break;
					}
					if (cell.BorderTop != currCell.BorderTop)
					{
						break;
					}
					if (cell.BorderBottom != currCell.BorderBottom)
					{
						break;
					}
					if (cell.BorderLeft != null)
					{
						break;
					}
					cell.Eaten = true;
					currCell.ColSpan++;
					currCell.BorderRight = cell.BorderRight;
				}
			}
			int num4 = index;
			int num5 = y + 1;
			num = numCols * num5 + x;
			num2 = numCols * numRows;
			while (num < num2)
			{
				PageTableCell cell2 = rgTableGrid.GetCell(num);
				if (cell2.Eaten)
				{
					break;
				}
				if (cell2.InUse)
				{
					break;
				}
				if (cell2.BorderLeft != currCell.BorderLeft)
				{
					break;
				}
				if (cell2.BorderRight != currCell.BorderRight)
				{
					break;
				}
				if (cell2.BorderTop != null)
				{
					break;
				}
				if (currCell.ColSpan == 1 && currCell.BorderLeft == null && currCell.BorderRight == null)
				{
					break;
				}
				int i = 1;
				PageTableCell pageTableCell = cell2;
				for (; i < currCell.ColSpan; i++)
				{
					PageTableCell cell3 = rgTableGrid.GetCell(num4 + i);
					PageTableCell cell4 = rgTableGrid.GetCell(num + i);
					if (cell4.InUse)
					{
						break;
					}
					if (cell4.Eaten)
					{
						break;
					}
					if (cell4.BorderLeft != null)
					{
						break;
					}
					if (cell4.BorderRight != cell3.BorderRight)
					{
						break;
					}
					if (cell4.BorderTop != null)
					{
						break;
					}
					if (cell4.BorderBottom != pageTableCell.BorderBottom)
					{
						break;
					}
					pageTableCell = cell4;
				}
				if (i != currCell.ColSpan)
				{
					break;
				}
				currCell.RowSpan++;
				currCell.BorderBottom = cell2.BorderBottom;
				for (i = 0; i < currCell.ColSpan; i++)
				{
					PageTableCell cell5 = rgTableGrid.GetCell(num + i);
					cell5.Eaten = true;
				}
				num4 = num;
				num5++;
				num = numCols * num5 + x;
			}
		}

		private void RenderIE7WritingMode(RPLFormat.WritingModes writingMode, RPLFormat.Directions direction, StyleContext styleContext)
		{
			this.WriteStream(HTML4Renderer.m_writingMode);
			if (HTML4Renderer.IsWritingModeVertical(writingMode))
			{
				if (direction == RPLFormat.Directions.RTL)
				{
					this.WriteStream(HTML4Renderer.m_btrl);
				}
				else
				{
					this.WriteStream(HTML4Renderer.m_tbrl);
				}
				if (writingMode == RPLFormat.WritingModes.Rotate270)
				{
					HTML4Renderer.WriteRotate270(this.m_deviceInfo, styleContext, this.WriteStream);
				}
			}
			else if (direction == RPLFormat.Directions.RTL)
			{
				this.WriteStream(HTML4Renderer.m_rltb);
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_lrtb);
			}
			this.WriteStream(HTML4Renderer.m_semiColon);
		}

		internal static void WriteRotate270(DeviceInfo deviceInfo, StyleContext styleContext, Action<byte[]> WriteStream)
		{
			if (deviceInfo.IsBrowserIE && styleContext != null && !styleContext.StyleOnCell)
			{
				if (!styleContext.RotationApplied)
				{
					WriteStream(HTML4Renderer.m_semiColon);
					WriteStream(HTML4Renderer.m_filter);
					WriteStream(HTML4Renderer.m_basicImageRotation180);
					styleContext.RotationApplied = true;
				}
				if (deviceInfo.OutlookCompat)
				{
					WriteStream(HTML4Renderer.m_semiColon);
					WriteStream(HTML4Renderer.m_msoRotation);
					WriteStream(HTML4Renderer.m_degree90);
				}
			}
		}

		private void RenderDirectionStyles(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLItemMeasurement measurement, IRPLStyle sharedStyleProps, IRPLStyle nonSharedStyleProps, bool isNonSharedStyles, StyleContext styleContext)
		{
			IRPLStyle iRPLStyle = isNonSharedStyles ? nonSharedStyleProps : sharedStyleProps;
			bool flag = HTML4Renderer.HasHorizontalPaddingStyles(sharedStyleProps);
			bool flag2 = HTML4Renderer.HasHorizontalPaddingStyles(nonSharedStyleProps);
			object obj = iRPLStyle[29];
			RPLFormat.Directions? nullable = null;
			if (obj != null)
			{
				nullable = (RPLFormat.Directions)obj;
				obj = EnumStrings.GetValue(nullable.Value);
				this.WriteStream(HTML4Renderer.m_direction);
				this.WriteStream(obj);
				this.WriteStream(HTML4Renderer.m_semiColon);
			}
			obj = iRPLStyle[30];
			RPLFormat.WritingModes? nullable2 = null;
			if (obj != null)
			{
				nullable2 = (RPLFormat.WritingModes)obj;
				this.WriteStream(HTML4Renderer.m_layoutFlow);
				if (HTML4Renderer.IsWritingModeVertical(nullable2.Value))
				{
					this.WriteStream(HTML4Renderer.m_verticalIdeographic);
				}
				else
				{
					this.WriteStream(HTML4Renderer.m_horizontal);
				}
				this.WriteStream(HTML4Renderer.m_semiColon);
				if (this.m_deviceInfo.IsBrowserIE && HTML4Renderer.IsWritingModeVertical(nullable2.Value) && measurement != null && reportItem is RPLTextBox)
				{
					RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)definition;
					float height = measurement.Height;
					float num = measurement.Width;
					float adjustedWidth = this.GetAdjustedWidth(measurement, props.Style);
					if (this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
					{
						num = adjustedWidth;
						height = this.GetAdjustedHeight(measurement, props.Style);
					}
					if (rPLTextBoxPropsDef.CanGrow)
					{
						if (styleContext != null && styleContext.InTablix && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
						{
							obj = null;
							if (flag2)
							{
								obj = nonSharedStyleProps[15];
							}
							if (obj == null && flag)
							{
								obj = sharedStyleProps[15];
							}
							if (obj != null)
							{
								RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
								float num2 = (float)rPLReportSize.ToMillimeters();
								num -= num2;
							}
							obj = null;
							if (flag2)
							{
								obj = nonSharedStyleProps[16];
							}
							if (obj == null && flag)
							{
								obj = sharedStyleProps[16];
							}
							if (obj != null)
							{
								RPLReportSize rPLReportSize2 = new RPLReportSize(obj as string);
								float num3 = (float)rPLReportSize2.ToMillimeters();
								num += num3;
							}
						}
						this.RenderMeasurementWidth((float)((num >= 0.0) ? num : 0.0));
					}
					else
					{
						this.WriteStream(HTML4Renderer.m_overflowHidden);
						this.WriteStream(HTML4Renderer.m_semiColon);
						this.RenderMeasurementWidth(num, false);
						this.RenderMeasurementHeight(height);
					}
					this.RenderMeasurementMinWidth(adjustedWidth);
				}
			}
			if (nullable2.HasValue && nullable.HasValue)
			{
				this.RenderIE7WritingMode(nullable2.Value, nullable.Value, styleContext);
			}
			else
			{
				if (!nullable2.HasValue && !nullable.HasValue)
				{
					return;
				}
				if (isNonSharedStyles)
				{
					if (!nullable2.HasValue)
					{
						obj = definition.SharedStyle[30];
						nullable2 = (RPLFormat.WritingModes)obj;
					}
					else if (!nullable.HasValue)
					{
						obj = definition.SharedStyle[29];
						nullable = (RPLFormat.Directions)obj;
					}
					this.RenderIE7WritingMode(nullable2.Value, nullable.Value, styleContext);
				}
			}
		}

		private void RenderReportItemStyle(RPLElement reportItem, RPLItemMeasurement measurement, ref int borderContext)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			this.RenderReportItemStyle(reportItem, elementProps, definition, measurement, new StyleContext(), ref borderContext, definition.ID);
		}

		private void RenderReportItemStyle(RPLElement reportItem, RPLItemMeasurement measurement, ref int borderContext, StyleContext styleContext)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			this.RenderReportItemStyle(reportItem, elementProps, definition, measurement, styleContext, ref borderContext, definition.ID);
		}

		private void RenderReportItemStyle(RPLElement reportItem, RPLElementProps elementProps, RPLElementPropsDef definition, RPLStyleProps nonSharedStyle, RPLStyleProps sharedStyle, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, string styleID)
		{
			if (this.m_useInlineStyle)
			{
				this.OpenStyle();
				RPLElementStyle sharedStyleProps = new RPLElementStyle(nonSharedStyle, sharedStyle);
				this.RenderStyleProps(reportItem, elementProps, definition, measurement, (IRPLStyle)sharedStyleProps, (IRPLStyle)null, styleContext, ref borderContext, false);
				if (styleContext.EmptyTextBox)
				{
					this.WriteStream(HTML4Renderer.m_fontSize);
					this.WriteFontSizeSmallPoint();
				}
				this.CloseStyle(true);
			}
			else
			{
				int num = borderContext;
				bool flag = sharedStyle != null && sharedStyle.Count > 0;
				if (nonSharedStyle != null && nonSharedStyle.Count > 0)
				{
					bool renderMeasurements = styleContext.RenderMeasurements;
					if (flag)
					{
						styleContext.RenderMeasurements = false;
					}
					this.OpenStyle();
					this.RenderStyleProps(reportItem, elementProps, definition, measurement, sharedStyle, nonSharedStyle, styleContext, ref num, true);
					this.CloseStyle(true);
					styleContext.RenderMeasurements = renderMeasurements;
				}
				if (flag)
				{
					byte[] array = (byte[])this.m_usedStyles[styleID];
					if (array == null)
					{
						if (this.m_onlyVisibleStyles)
						{
							int num2 = 0;
							array = this.RenderSharedStyle(reportItem, elementProps, definition, sharedStyle, nonSharedStyle, measurement, styleID, styleContext, ref num2);
						}
						else
						{
							array = this.m_encoding.GetBytes(styleID);
							this.m_usedStyles.Add(styleID, array);
						}
					}
					this.CloseStyle(true);
					this.WriteClassStyle(array, false);
					byte omitBordersState = styleContext.OmitBordersState;
					if (borderContext != 0 || omitBordersState != 0)
					{
						if (borderContext == 15)
						{
							this.WriteStream(HTML4Renderer.m_space);
							this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
							this.WriteStream(HTML4Renderer.m_ignoreBorder);
						}
						else
						{
							if ((borderContext & 4) != 0 || (omitBordersState & 1) != 0)
							{
								this.WriteStream(HTML4Renderer.m_space);
								this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
								this.WriteStream(HTML4Renderer.m_ignoreBorderT);
							}
							if ((borderContext & 1) != 0 || (omitBordersState & 4) != 0)
							{
								this.WriteStream(HTML4Renderer.m_space);
								this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
								this.WriteStream(HTML4Renderer.m_ignoreBorderL);
							}
							if ((borderContext & 8) != 0 || (omitBordersState & 2) != 0)
							{
								this.WriteStream(HTML4Renderer.m_space);
								this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
								this.WriteStream(HTML4Renderer.m_ignoreBorderB);
							}
							if ((borderContext & 2) != 0 || (omitBordersState & 8) != 0)
							{
								this.WriteStream(HTML4Renderer.m_space);
								this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
								this.WriteStream(HTML4Renderer.m_ignoreBorderR);
							}
						}
					}
					if (styleContext.EmptyTextBox)
					{
						this.WriteStream(HTML4Renderer.m_space);
						this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
						this.WriteStream(HTML4Renderer.m_emptyTextBox);
					}
					this.WriteStream(HTML4Renderer.m_quote);
					if (!styleContext.NoBorders)
					{
						this.GetBorderContext((IRPLStyle)sharedStyle, ref borderContext, omitBordersState);
					}
				}
				borderContext |= num;
			}
		}

		private void GetBorderContext(IRPLStyle styleProps, ref int borderContext, byte omitBordersState)
		{
			object defWidth = styleProps[10];
			object obj = styleProps[5];
			object defColor = styleProps[0];
			object obj2 = null;
			object obj3 = null;
			object obj4 = null;
			if (borderContext != 0 || omitBordersState != 0 || !this.OnlyGeneralBorder(styleProps))
			{
				if ((borderContext & 8) == 0 && (omitBordersState & 2) == 0 && this.BorderInstance(styleProps, defWidth, obj, defColor, ref obj2, ref obj3, ref obj4, Border.Bottom))
				{
					borderContext |= 8;
				}
				if ((borderContext & 1) == 0 && (omitBordersState & 4) == 0 && this.BorderInstance(styleProps, defWidth, obj, defColor, ref obj2, ref obj3, ref obj4, Border.Left))
				{
					borderContext |= 1;
				}
				if ((borderContext & 2) == 0 && (omitBordersState & 8) == 0 && this.BorderInstance(styleProps, defWidth, obj, defColor, ref obj2, ref obj3, ref obj4, Border.Right))
				{
					borderContext |= 2;
				}
				if ((borderContext & 4) == 0 && (omitBordersState & 1) == 0 && this.BorderInstance(styleProps, defWidth, obj, defColor, ref obj2, ref obj3, ref obj4, Border.Top))
				{
					borderContext |= 4;
				}
			}
			else if (obj != null && (RPLFormat.BorderStyles)obj != 0)
			{
				borderContext = 15;
			}
		}

		private void RenderReportItemStyle(RPLElement reportItem, RPLElementProps elementProps, RPLElementPropsDef definition, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, string styleID)
		{
			this.RenderReportItemStyle(reportItem, elementProps, definition, elementProps.NonSharedStyle, definition.SharedStyle, measurement, styleContext, ref borderContext, styleID);
		}

		private void RenderPercentSizes()
		{
			this.WriteStream(HTML4Renderer.m_styleHeight);
			this.WriteStream(HTML4Renderer.m_percent);
			this.WriteStream(HTML4Renderer.m_semiColon);
			this.WriteStream(HTML4Renderer.m_styleWidth);
			this.WriteStream(HTML4Renderer.m_percent);
			this.WriteStream(HTML4Renderer.m_semiColon);
		}

		private void RenderTextAlign(RPLTextBoxProps props, RPLElementStyle style)
		{
			if (props != null)
			{
				this.WriteStream(HTML4Renderer.m_textAlign);
				bool flag = HTML4Renderer.GetTextAlignForType(props);
				if (HTML4Renderer.IsDirectionRTL(style))
				{
					flag = ((byte)((!flag) ? 1 : 0) != 0);
				}
				if (flag)
				{
					this.WriteStream(HTML4Renderer.m_rightValue);
				}
				else
				{
					this.WriteStream(HTML4Renderer.m_leftValue);
				}
				this.WriteStream(HTML4Renderer.m_semiColon);
			}
		}

		internal static bool GetTextAlignForType(RPLTextBoxProps textBoxProps)
		{
			TypeCode typeCode = textBoxProps.TypeCode;
			return HTML4Renderer.GetTextAlignForType(typeCode);
		}

		internal static bool GetTextAlignForType(TypeCode typeCode)
		{
			bool result = false;
			switch (typeCode)
			{
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
			case TypeCode.DateTime:
				result = true;
				break;
			}
			return result;
		}

		private bool HasBorderStyle(object borderStyle)
		{
			if (borderStyle != null)
			{
				return (RPLFormat.BorderStyles)borderStyle != RPLFormat.BorderStyles.None;
			}
			return false;
		}

		private float SubtractBorderStyles(float width, object defaultBorderStyle, object specificBorderStyle, object defaultBorderWidth, object specificBorderWidth)
		{
			object obj = null;
			obj = specificBorderWidth;
			if (obj == null)
			{
				obj = defaultBorderWidth;
			}
			if (obj != null && (this.HasBorderStyle(specificBorderStyle) || this.HasBorderStyle(defaultBorderStyle)))
			{
				RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
				width -= (float)rPLReportSize.ToMillimeters();
			}
			return width;
		}

		private float GetInnerContainerWidth(RPLMeasurement measurement, IRPLStyle containerStyle)
		{
			if (measurement == null)
			{
				return -1f;
			}
			float width = measurement.Width;
			float num = 0f;
			object obj = containerStyle[15];
			if (obj != null)
			{
				RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
				num += (float)rPLReportSize.ToMillimeters();
			}
			obj = containerStyle[16];
			if (obj != null)
			{
				RPLReportSize rPLReportSize2 = new RPLReportSize(obj as string);
				num += (float)rPLReportSize2.ToMillimeters();
			}
			return width - num;
		}

		private float GetInnerContainerWidthSubtractBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			if (measurement == null)
			{
				return -1f;
			}
			float innerContainerWidth = this.GetInnerContainerWidth(measurement, containerStyle);
			object defaultBorderStyle = containerStyle[5];
			object defaultBorderWidth = containerStyle[10];
			object specificBorderWidth = containerStyle[11];
			object specificBorderStyle = containerStyle[6];
			innerContainerWidth = this.SubtractBorderStyles(innerContainerWidth, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			specificBorderWidth = containerStyle[12];
			specificBorderStyle = containerStyle[7];
			innerContainerWidth = this.SubtractBorderStyles(innerContainerWidth, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			if (innerContainerWidth <= 0.0)
			{
				innerContainerWidth = 1f;
			}
			return innerContainerWidth;
		}

		private float GetAdjustedWidth(RPLItemMeasurement measurement, IRPLStyle style)
		{
			float result = measurement.Width;
			if (this.m_deviceInfo.BrowserMode == BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE)
			{
				result = this.GetInnerContainerWidthSubtractBorders(measurement, style);
			}
			return result;
		}

		private float GetAdjustedHeight(RPLItemMeasurement measurement, IRPLStyle style)
		{
			float result = measurement.Height;
			if (this.m_deviceInfo.BrowserMode == BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE)
			{
				result = this.GetInnerContainerHeightSubtractBorders(measurement, style);
			}
			return result;
		}

		private float GetInnerContainerHeight(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			if (measurement == null)
			{
				return -1f;
			}
			float height = measurement.Height;
			float num = 0f;
			object obj = containerStyle[17];
			if (obj != null)
			{
				RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
				num += (float)rPLReportSize.ToMillimeters();
			}
			obj = containerStyle[18];
			if (obj != null)
			{
				RPLReportSize rPLReportSize2 = new RPLReportSize(obj as string);
				num += (float)rPLReportSize2.ToMillimeters();
			}
			return height - num;
		}

		private float GetInnerContainerHeightSubtractBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			if (measurement == null)
			{
				return -1f;
			}
			float innerContainerHeight = this.GetInnerContainerHeight(measurement, containerStyle);
			object defaultBorderStyle = containerStyle[5];
			object defaultBorderWidth = containerStyle[10];
			object specificBorderWidth = containerStyle[13];
			object specificBorderStyle = containerStyle[8];
			innerContainerHeight = this.SubtractBorderStyles(innerContainerHeight, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			specificBorderWidth = containerStyle[14];
			specificBorderStyle = containerStyle[9];
			innerContainerHeight = this.SubtractBorderStyles(innerContainerHeight, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			if (innerContainerHeight <= 0.0)
			{
				innerContainerHeight = 1f;
			}
			return innerContainerHeight;
		}

		private void RenderTextBoxContent(RPLTextBox textBox, RPLTextBoxProps tbProps, RPLTextBoxPropsDef tbDef, string textBoxValue, RPLStyleProps actionStyle, bool renderImages, RPLItemMeasurement measurement, RPLAction textBoxAction)
		{
			if (tbDef.IsSimple)
			{
				bool flag = false;
				object obj = null;
				bool flag2 = string.IsNullOrEmpty(textBoxValue);
				if (!flag2 && renderImages)
				{
					obj = tbProps.Style[24];
					if (obj != null && (RPLFormat.TextDecorations)obj != 0)
					{
						obj = EnumStrings.GetValue((RPLFormat.TextDecorations)obj);
						flag = true;
						this.WriteStream(HTML4Renderer.m_openSpan);
						this.WriteStream(HTML4Renderer.m_openStyle);
						this.WriteStream(HTML4Renderer.m_textDecoration);
						this.WriteStream(obj);
						this.WriteStream(HTML4Renderer.m_closeQuote);
					}
				}
				if (flag2)
				{
					if (!this.NeedSharedToggleParent(tbProps))
					{
						this.WriteStream(HTML4Renderer.m_nbsp);
					}
				}
				else
				{
					List<int> list = null;
					if (!string.IsNullOrEmpty(this.m_searchText))
					{
						int startIndex = 0;
						int length = this.m_searchText.Length;
						string text = textBoxValue;
						string text2 = this.m_searchText;
						if (text2.IndexOf(' ') >= 0)
						{
							text2 = text2.Replace('\u00a0', ' ');
							text = text.Replace('\u00a0', ' ');
						}
						while ((startIndex = text.IndexOf(text2, startIndex, StringComparison.OrdinalIgnoreCase)) != -1)
						{
							if (list == null)
							{
								list = new List<int>(2);
							}
							list.Add(startIndex);
							startIndex += length;
						}
						if (list == null)
						{
							this.RenderMultiLineText(textBoxValue);
						}
						else
						{
							this.RenderMultiLineTextWithHits(textBoxValue, list);
						}
					}
					else
					{
						this.RenderMultiLineText(textBoxValue);
					}
				}
				if (flag)
				{
					this.WriteStream(HTML4Renderer.m_closeSpan);
				}
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_openDiv);
				RPLElementStyle style = tbProps.Style;
				bool flag3 = false;
				bool flag4 = HTML4Renderer.IsWritingModeVertical(style);
				if (!this.m_deviceInfo.IsBrowserIE || !flag4)
				{
					this.OpenStyle();
					double num = 0.0;
					if (this.m_deviceInfo.IsBrowserIE)
					{
						this.WriteStream(HTML4Renderer.m_overflowXHidden);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
					num = 0.0;
					if (measurement != null)
					{
						num = (double)this.GetInnerContainerWidthSubtractBorders(measurement, tbProps.Style);
					}
					if (tbDef.CanSort && !this.IsFragment && !HTML4Renderer.IsDirectionRTL(tbProps.Style))
					{
						num -= 4.2333332697550459;
					}
					if (num > 0.0)
					{
						this.WriteStream(HTML4Renderer.m_styleWidth);
						this.WriteRSStream((float)num);
						this.WriteStream(HTML4Renderer.m_semiColon);
					}
				}
				if (HTML4Renderer.IsDirectionRTL(style))
				{
					this.OpenStyle();
					this.WriteStream(HTML4Renderer.m_direction);
					this.WriteStream("rtl");
					this.CloseStyle(true);
					flag3 = true;
					this.WriteStream(HTML4Renderer.m_classStyle);
					this.WriteStream(HTML4Renderer.m_rtlEmbed);
				}
				else
				{
					this.CloseStyle(true);
				}
				if (textBoxAction != null)
				{
					if (!flag3)
					{
						flag3 = true;
						this.WriteStream(HTML4Renderer.m_classStyle);
					}
					else
					{
						this.WriteStream(HTML4Renderer.m_space);
					}
					this.WriteStream(HTML4Renderer.m_styleAction);
				}
				if (flag3)
				{
					this.WriteStream(HTML4Renderer.m_quote);
				}
				this.WriteStream(HTML4Renderer.m_closeBracket);
				TextRunStyleWriter trsw = new TextRunStyleWriter(this);
				ParagraphStyleWriter paragraphStyleWriter = new ParagraphStyleWriter(this, textBox);
				RPLStyleProps nonSharedStyle = tbProps.NonSharedStyle;
				if (nonSharedStyle != null && (nonSharedStyle[30] != null || nonSharedStyle[29] != null))
				{
					paragraphStyleWriter.OutputSharedInNonShared = true;
				}
				RPLParagraph nextParagraph = textBox.GetNextParagraph();
				ListLevelStack listLevelStack = null;
				while (nextParagraph != null)
				{
					RPLParagraphProps rPLParagraphProps = nextParagraph.ElementProps as RPLParagraphProps;
					RPLParagraphPropsDef rPLParagraphPropsDef = rPLParagraphProps.Definition as RPLParagraphPropsDef;
					int num2 = rPLParagraphProps.ListLevel ?? rPLParagraphPropsDef.ListLevel;
					RPLFormat.ListStyles listStyles = rPLParagraphProps.ListStyle ?? rPLParagraphPropsDef.ListStyle;
					string text3 = null;
					RPLStyleProps nonSharedStyle2 = rPLParagraphProps.NonSharedStyle;
					RPLStyleProps shared = null;
					if (rPLParagraphPropsDef != null)
					{
						if (num2 == 0)
						{
							num2 = rPLParagraphPropsDef.ListLevel;
						}
						if (listStyles == RPLFormat.ListStyles.None)
						{
							listStyles = rPLParagraphPropsDef.ListStyle;
						}
						text3 = rPLParagraphPropsDef.ID;
						if (!paragraphStyleWriter.OutputSharedInNonShared)
						{
							shared = rPLParagraphPropsDef.SharedStyle;
						}
					}
					paragraphStyleWriter.Paragraph = nextParagraph;
					paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.All;
					paragraphStyleWriter.CurrentListLevel = num2;
					byte[] array = null;
					if (num2 > 0)
					{
						if (listLevelStack == null)
						{
							listLevelStack = new ListLevelStack();
						}
						bool writeNoVerticalMargin = !this.m_deviceInfo.IsBrowserIE || !flag4 || (this.m_deviceInfo.BrowserMode == BrowserMode.Standards && !this.m_deviceInfo.IsBrowserIE6Or7StandardsMode);
						listLevelStack.PushTo(this, num2, listStyles, writeNoVerticalMargin);
						if (listStyles != 0)
						{
							if (this.m_deviceInfo.BrowserMode == BrowserMode.Quirks || this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
							{
								this.WriteStream(HTML4Renderer.m_openDiv);
								this.WriteStream(HTML4Renderer.m_closeBracket);
							}
							this.WriteStream(HTML4Renderer.m_openLi);
							paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ListOnly;
							this.WriteStyles(text3 + "l", nonSharedStyle2, shared, paragraphStyleWriter);
							this.WriteStream(HTML4Renderer.m_closeBracket);
							array = HTML4Renderer.m_closeLi;
							paragraphStyleWriter.ParagraphMode = ParagraphStyleWriter.Mode.ParagraphOnly;
							text3 += "p";
						}
					}
					else if (listLevelStack != null)
					{
						listLevelStack.PopAll();
						listLevelStack = null;
					}
					this.WriteStream(HTML4Renderer.m_openDiv);
					this.WriteStyles(text3, nonSharedStyle2, shared, paragraphStyleWriter);
					this.WriteStream(HTML4Renderer.m_closeBracket);
					RPLReportSize hangingIndent = rPLParagraphProps.HangingIndent;
					if (hangingIndent == null)
					{
						hangingIndent = rPLParagraphPropsDef.HangingIndent;
					}
					float num3 = 0f;
					if (hangingIndent != null)
					{
						num3 = (float)hangingIndent.ToMillimeters();
					}
					if (num3 > 0.0)
					{
						this.WriteStream(HTML4Renderer.m_openSpan);
						this.OpenStyle();
						this.RenderMeasurementWidth(num3, true);
						this.WriteStream(HTML4Renderer.m_styleDisplayInlineBlock);
						this.CloseStyle(true);
						this.WriteStream(HTML4Renderer.m_closeBracket);
						if (this.m_deviceInfo.IsBrowserGeckoEngine)
						{
							this.WriteStream(HTML4Renderer.m_nbsp);
						}
						this.WriteStream(HTML4Renderer.m_closeSpan);
					}
					this.RenderTextRuns(nextParagraph, trsw, textBoxAction);
					this.WriteStream(HTML4Renderer.m_closeDiv);
					if (array != null)
					{
						this.WriteStream(array);
						if (this.m_deviceInfo.BrowserMode == BrowserMode.Quirks || this.m_deviceInfo.IsBrowserIE6Or7StandardsMode)
						{
							this.WriteStream(HTML4Renderer.m_closeDiv);
						}
					}
					nextParagraph = textBox.GetNextParagraph();
				}
				if (listLevelStack != null)
				{
					listLevelStack.PopAll();
				}
				this.WriteStream(HTML4Renderer.m_closeDiv);
			}
		}

		private void RenderTextRuns(RPLParagraph paragraph, TextRunStyleWriter trsw, RPLAction textBoxAction)
		{
			int num = 0;
			RPLTextRun rPLTextRun = null;
			if (!string.IsNullOrEmpty(this.m_searchText))
			{
				RPLTextRun nextTextRun = paragraph.GetNextTextRun();
				rPLTextRun = nextTextRun;
				List<RPLTextRun> list = new List<RPLTextRun>();
				StringBuilder stringBuilder = new StringBuilder();
				while (nextTextRun != null)
				{
					list.Add(nextTextRun);
					string value = (nextTextRun.ElementProps as RPLTextRunProps).Value;
					if (string.IsNullOrEmpty(value))
					{
						value = (nextTextRun.ElementPropsDef as RPLTextRunPropsDef).Value;
					}
					stringBuilder.Append(value);
					nextTextRun = paragraph.GetNextTextRun();
				}
				string text = stringBuilder.ToString();
				string text2 = this.m_searchText;
				if (text2.IndexOf(' ') >= 0)
				{
					text2 = text2.Replace('\u00a0', ' ');
					text = text.Replace('\u00a0', ' ');
				}
				int num2 = text.IndexOf(text2, StringComparison.OrdinalIgnoreCase);
				List<int> list2 = new List<int>();
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int length = this.m_searchText.Length;
				for (int i = 0; i < list.Count; i++)
				{
					nextTextRun = list[i];
					string value2 = (nextTextRun.ElementProps as RPLTextRunProps).Value;
					if (string.IsNullOrEmpty(value2))
					{
						value2 = (nextTextRun.ElementPropsDef as RPLTextRunPropsDef).Value;
					}
					if (!string.IsNullOrEmpty(value2))
					{
						while (num2 > -1 && num2 < num3 + value2.Length)
						{
							list2.Add(num2 - num3);
							num2 = text.IndexOf(text2, num2 + length, StringComparison.OrdinalIgnoreCase);
						}
						if (list2.Count > 0 || num4 > 0)
						{
							num += this.RenderTextRunFindString(nextTextRun, list2, num4, ref num5, trsw, textBoxAction);
							if (num4 > 0)
							{
								num4 -= value2.Length;
								if (num4 < 0)
								{
									num4 = 0;
								}
							}
							if (list2.Count > 0)
							{
								int num6 = list2[list2.Count - 1];
								list2.Clear();
								if (value2.Length < num6 + length)
								{
									num4 = length - (value2.Length - num6);
								}
							}
						}
						else
						{
							num += this.RenderTextRun(nextTextRun, trsw, textBoxAction);
						}
						num3 += value2.Length;
					}
				}
			}
			else
			{
				RPLTextRun nextTextRun2 = paragraph.GetNextTextRun();
				rPLTextRun = nextTextRun2;
				while (nextTextRun2 != null)
				{
					num += this.RenderTextRun(nextTextRun2, trsw, textBoxAction);
					nextTextRun2 = paragraph.GetNextTextRun();
				}
			}
			if (num == 0 && rPLTextRun != null)
			{
				RPLTextRunProps rPLTextRunProps = rPLTextRun.ElementProps as RPLTextRunProps;
				RPLElementPropsDef definition = rPLTextRunProps.Definition;
				this.WriteStream(HTML4Renderer.m_openSpan);
				this.WriteStyles(definition.ID, rPLTextRunProps.NonSharedStyle, definition.SharedStyle, trsw);
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStream(HTML4Renderer.m_nbsp);
				this.WriteStream(HTML4Renderer.m_closeSpan);
			}
		}

		private int RenderTextRunFindString(RPLTextRun textRun, List<int> hits, int remainingChars, ref int runOffsetCount, TextRunStyleWriter trsw, RPLAction textBoxAction)
		{
			RPLTextRunProps rPLTextRunProps = textRun.ElementProps as RPLTextRunProps;
			RPLTextRunPropsDef rPLTextRunPropsDef = rPLTextRunProps.Definition as RPLTextRunPropsDef;
			RPLStyleProps shared = null;
			string id = null;
			string value = rPLTextRunProps.Value;
			string toolTip = rPLTextRunProps.ToolTip;
			if (rPLTextRunPropsDef != null)
			{
				shared = rPLTextRunPropsDef.SharedStyle;
				id = rPLTextRunPropsDef.ID;
				if (string.IsNullOrEmpty(value))
				{
					value = rPLTextRunPropsDef.Value;
				}
				if (string.IsNullOrEmpty(toolTip))
				{
					toolTip = rPLTextRunPropsDef.ToolTip;
				}
			}
			if (string.IsNullOrEmpty(value))
			{
				return 0;
			}
			byte[] theBytes = HTML4Renderer.m_closeSpan;
			RPLAction rPLAction = null;
			if (textBoxAction == null && this.HasAction(rPLTextRunProps.ActionInfo))
			{
				rPLAction = rPLTextRunProps.ActionInfo.Actions[0];
			}
			if (rPLAction != null)
			{
				this.WriteStream(HTML4Renderer.m_openA);
				this.RenderTabIndex();
				this.RenderActionHref(rPLAction, RPLFormat.TextDecorations.Underline, null);
				theBytes = HTML4Renderer.m_closeA;
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_openSpan);
			}
			if (toolTip != null)
			{
				this.WriteToolTipAttribute(toolTip);
			}
			this.WriteStyles(id, rPLTextRunProps.NonSharedStyle, shared, trsw);
			this.WriteStream(HTML4Renderer.m_closeBracket);
			int num = 0;
			int num2 = 0;
			int length = value.Length;
			if (remainingChars > 0)
			{
				int num3 = remainingChars;
				if (num3 > length)
				{
					num3 = length;
				}
				if (num3 > 0)
				{
					this.OutputFindString(value.Substring(0, num3), runOffsetCount++);
					num += num3;
					if (num3 >= remainingChars)
					{
						this.m_currentHitCount++;
						runOffsetCount = 0;
					}
				}
			}
			int num4 = hits.Count - 1;
			bool flag = false;
			int length2 = this.m_searchText.Length;
			if (hits.Count > 0)
			{
				num2 = hits[hits.Count - 1];
				if (num2 + length2 > length)
				{
					flag = true;
				}
				else
				{
					num4 = hits.Count;
				}
			}
			for (int i = 0; i < num4; i++)
			{
				num2 = hits[i];
				if (num < num2)
				{
					this.RenderMultiLineText(value.Substring(num, num2 - num));
				}
				this.OutputFindString(value.Substring(num2, length2), 0);
				this.m_currentHitCount++;
				runOffsetCount = 0;
				num = num2 + length2;
			}
			if (flag)
			{
				num2 = hits[hits.Count - 1];
				if (num < num2)
				{
					this.RenderMultiLineText(value.Substring(num, num2 - num));
				}
				this.OutputFindString(value.Substring(num2, length - num2), runOffsetCount++);
			}
			else if (num < length)
			{
				this.RenderMultiLineText(value.Substring(num));
			}
			this.WriteStream(theBytes);
			return length;
		}

		private int RenderTextRun(RPLTextRun textRun, TextRunStyleWriter trsw, RPLAction textBoxAction)
		{
			RPLTextRunProps rPLTextRunProps = textRun.ElementProps as RPLTextRunProps;
			RPLTextRunPropsDef rPLTextRunPropsDef = rPLTextRunProps.Definition as RPLTextRunPropsDef;
			RPLStyleProps shared = null;
			string id = null;
			string value = rPLTextRunProps.Value;
			string toolTip = rPLTextRunProps.ToolTip;
			if (rPLTextRunPropsDef != null)
			{
				shared = rPLTextRunPropsDef.SharedStyle;
				id = rPLTextRunPropsDef.ID;
				if (string.IsNullOrEmpty(value))
				{
					value = rPLTextRunPropsDef.Value;
				}
				if (string.IsNullOrEmpty(toolTip))
				{
					toolTip = rPLTextRunPropsDef.ToolTip;
				}
			}
			if (string.IsNullOrEmpty(value))
			{
				return 0;
			}
			byte[] theBytes = HTML4Renderer.m_closeSpan;
			RPLAction rPLAction = null;
			if (textBoxAction == null)
			{
				rPLAction = textBoxAction;
				if (this.HasAction(rPLTextRunProps.ActionInfo))
				{
					rPLAction = rPLTextRunProps.ActionInfo.Actions[0];
				}
			}
			if (rPLAction != null)
			{
				this.WriteStream(HTML4Renderer.m_openA);
				this.RenderTabIndex();
				this.RenderActionHref(rPLAction, RPLFormat.TextDecorations.Underline, null);
				theBytes = HTML4Renderer.m_closeA;
			}
			else
			{
				this.WriteStream(HTML4Renderer.m_openSpan);
			}
			if (toolTip != null)
			{
				this.WriteToolTipAttribute(toolTip);
			}
			this.WriteStyles(id, rPLTextRunProps.NonSharedStyle, shared, trsw);
			this.RenderLanguage(rPLTextRunProps.Style[32] as string);
			this.WriteStream(HTML4Renderer.m_closeBracket);
			this.RenderMultiLineText(value);
			this.WriteStream(theBytes);
			return value.Length;
		}

		private void WriteStyles(string id, RPLStyleProps nonShared, RPLStyleProps shared, ElementStyleWriter styleWriter)
		{
			bool flag = (shared != null && shared.Count > 0) || styleWriter.NeedsToWriteNullStyle(StyleWriterMode.Shared);
			if (this.m_useInlineStyle || (flag && id == null))
			{
				this.OpenStyle();
				styleWriter.WriteStyles(StyleWriterMode.All, new RPLElementStyle(nonShared, shared));
				this.CloseStyle(true);
			}
			else
			{
				if ((nonShared != null && nonShared.Count > 0) || styleWriter.NeedsToWriteNullStyle(StyleWriterMode.NonShared))
				{
					this.OpenStyle();
					styleWriter.WriteStyles(StyleWriterMode.NonShared, nonShared);
					this.CloseStyle(true);
				}
				if (flag && id != null)
				{
					byte[] array = (byte[])this.m_usedStyles[id];
					if (array == null)
					{
						if (this.m_onlyVisibleStyles)
						{
							Stream mainStream = this.m_mainStream;
							this.m_mainStream = this.m_styleStream;
							this.RenderOpenStyle(id);
							styleWriter.WriteStyles(StyleWriterMode.Shared, shared);
							this.WriteStream(HTML4Renderer.m_closeAccol);
							this.m_mainStream = mainStream;
							array = this.m_encoding.GetBytes(id);
							this.m_usedStyles.Add(id, array);
						}
						else
						{
							array = this.m_encoding.GetBytes(id);
							this.m_usedStyles.Add(id, array);
						}
					}
					this.CloseStyle(true);
					this.WriteClassStyle(array, true);
				}
			}
		}

		protected abstract void WriteFitProportionalScript(double pv, double ph);

		private void RenderImageFitProportional(RPLImage image, RPLItemMeasurement measurement, PaddingSharedInfo padds, bool writeSmallSize)
		{
			if (this.m_deviceInfo.AllowScript)
			{
				this.m_fitPropImages = true;
				double pv = 0.0;
				double ph = 0.0;
				if (padds != null)
				{
					pv = padds.PadV;
					ph = padds.PadH;
				}
				this.WriteFitProportionalScript(pv, ph);
				if (writeSmallSize || !this.m_browserIE)
				{
					long num = 1L;
					this.WriteStream(HTML4Renderer.m_inlineHeight);
					if (this.m_deviceInfo.IsBrowserSafari || this.m_deviceInfo.IsBrowserGeckoEngine)
					{
						num = 5L;
						if (measurement != null)
						{
							double num2 = (double)measurement.Height;
							if ((double)measurement.Width < num2)
							{
								num2 = (double)measurement.Width;
							}
							num = Utility.MMToPx(num2);
							if (num < 5)
							{
								num = 5L;
							}
						}
					}
					this.WriteStream(num.ToString(CultureInfo.InvariantCulture));
					this.WriteStream(HTML4Renderer.m_px);
					this.WriteStream(HTML4Renderer.m_quote);
				}
				if (writeSmallSize)
				{
					this.WriteStream(HTML4Renderer.m_inlineWidth);
					this.WriteStream("1");
					this.WriteStream(HTML4Renderer.m_px);
					this.WriteStream(HTML4Renderer.m_quote);
				}
			}
		}

		private void RenderImagePercent(RPLImage image, RPLImageProps imageProps, RPLImagePropsDef imagePropsDef, RPLItemMeasurement measurement)
		{
			bool flag = false;
			bool flag2 = false;
			RPLImageData image2 = imageProps.Image;
			RPLActionInfo actionInfo = imageProps.ActionInfo;
			RPLFormat.Sizings sizing = imagePropsDef.Sizing;
			if (sizing == RPLFormat.Sizings.FitProportional || sizing == RPLFormat.Sizings.Fit || sizing == RPLFormat.Sizings.Clip)
			{
				flag = true;
				this.WriteStream(HTML4Renderer.m_openDiv);
				if (this.m_useInlineStyle)
				{
					this.PercentSizesOverflow();
				}
				else
				{
					this.ClassPercentSizesOverflow();
				}
				if (measurement != null)
				{
					this.OpenStyle();
					this.RenderMeasurementMinWidth(this.GetInnerContainerWidth(measurement, imageProps.Style));
					this.RenderMeasurementMinHeight(this.GetInnerContainerHeight(measurement, imageProps.Style));
					this.CloseStyle(true);
				}
			}
			int xOffset = 0;
			int yOffset = 0;
			System.Drawing.Rectangle imageConsolidationOffsets = imageProps.Image.ImageConsolidationOffsets;
			bool flag3 = !imageConsolidationOffsets.IsEmpty;
			if (flag3)
			{
				if (!flag)
				{
					flag = true;
					this.WriteStream(HTML4Renderer.m_openDiv);
					if (sizing != 0)
					{
						if (this.m_useInlineStyle)
						{
							this.PercentSizesOverflow();
						}
						else
						{
							this.ClassPercentSizesOverflow();
						}
					}
				}
				if (sizing == RPLFormat.Sizings.Clip || sizing == RPLFormat.Sizings.FitProportional || sizing == RPLFormat.Sizings.Fit)
				{
					this.WriteStream(HTML4Renderer.m_closeBracket);
					this.WriteStream(HTML4Renderer.m_openDiv);
					if (this.m_deviceInfo.IsBrowserIE6 && this.m_deviceInfo.IsBrowserIE6Or7StandardsMode && measurement != null)
					{
						this.WriteStream(" origWidth=\"");
						this.WriteRSStream(measurement.Width);
						this.WriteStream("\" origHeight=\"");
						this.WriteStream("\"");
					}
				}
				this.WriteOuterConsolidation(imageConsolidationOffsets, sizing, imageProps.UniqueName);
				this.CloseStyle(true);
				xOffset = imageConsolidationOffsets.Left;
				yOffset = imageConsolidationOffsets.Top;
			}
			else if (this.m_deviceInfo.AllowScript && sizing == RPLFormat.Sizings.Fit && this.m_deviceInfo.BrowserMode == BrowserMode.Standards)
			{
				flag = true;
				this.WriteStream(HTML4Renderer.m_openDiv);
				if (this.m_imgFitDivIdsStream == null)
				{
					this.CreateImgFitDivImageIdsStream();
				}
				this.WriteIdToSecondaryStream(this.m_imgFitDivIdsStream, imageProps.UniqueName + "_ifd");
				this.RenderReportItemId(imageProps.UniqueName + "_ifd");
			}
			if (flag)
			{
				this.WriteStream(HTML4Renderer.m_closeBracket);
			}
			if (this.HasAction(actionInfo))
			{
				flag2 = this.RenderElementHyperlink(imageProps.Style, actionInfo.Actions[0]);
			}
			this.WriteStream(HTML4Renderer.m_img);
			if (this.m_browserIE)
			{
				this.WriteStream(HTML4Renderer.m_imgOnError);
			}
			if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
			{
				this.WriteAttrEncoded(HTML4Renderer.m_useMap, "#" + this.m_deviceInfo.HtmlPrefixId + HTML4Renderer.m_mapPrefixString + imageProps.UniqueName);
				this.WriteStream(HTML4Renderer.m_zeroBorder);
			}
			else if (flag2)
			{
				this.WriteStream(HTML4Renderer.m_zeroBorder);
			}
			switch (sizing)
			{
			case RPLFormat.Sizings.FitProportional:
			{
				PaddingSharedInfo padds = null;
				if (this.m_deviceInfo.IsBrowserSafari)
				{
					padds = this.GetPaddings(image.ElementProps.Style, null);
				}
				bool writeSmallSize = !flag3 && this.m_deviceInfo.BrowserMode == BrowserMode.Standards;
				this.RenderImageFitProportional(image, null, padds, writeSmallSize);
				break;
			}
			case RPLFormat.Sizings.Fit:
				if (!flag3)
				{
					if (this.m_deviceInfo.AllowScript && this.m_deviceInfo.BrowserMode == BrowserMode.Standards)
					{
						this.WriteStream(" width=\"1px\" height=\"1px\"");
					}
					else if (this.m_useInlineStyle)
					{
						this.PercentSizes();
					}
					else
					{
						this.ClassPercentSizes();
					}
				}
				break;
			}
			if (flag3)
			{
				this.WriteClippedDiv(imageConsolidationOffsets);
			}
			this.WriteToolTip(imageProps);
			this.WriteStream(HTML4Renderer.m_src);
			this.RenderImageUrl(true, image2);
			this.WriteStream(HTML4Renderer.m_closeTag);
			if (flag2)
			{
				this.WriteStream(HTML4Renderer.m_closeA);
			}
			if (imageProps.ActionImageMapAreas != null && imageProps.ActionImageMapAreas.Length > 0)
			{
				this.RenderImageMapAreas(imageProps.ActionImageMapAreas, (double)measurement.Width, (double)measurement.Height, imageProps.UniqueName, xOffset, yOffset);
			}
			if (flag3 && (sizing == RPLFormat.Sizings.Clip || sizing == RPLFormat.Sizings.FitProportional || sizing == RPLFormat.Sizings.Fit))
			{
				this.WriteStream(HTML4Renderer.m_closeDiv);
			}
			if (flag)
			{
				this.WriteStreamCR(HTML4Renderer.m_closeDiv);
			}
		}

		private void RenderImageMapAreas(RPLActionInfoWithImageMap[] actionImageMaps, double width, double height, string uniqueName, int xOffset, int yOffset)
		{			
			double imageWidth = width * 96.0 * 0.03937007874;
			double imageHeight = height * 96.0 * 0.03937007874;
			this.WriteStream(HTML4Renderer.m_openMap);
			this.WriteAttrEncoded(HTML4Renderer.m_name, this.m_deviceInfo.HtmlPrefixId + HTML4Renderer.m_mapPrefixString + uniqueName);
			this.WriteStreamCR(HTML4Renderer.m_closeBracket);
			foreach (RPLActionInfoWithImageMap rPLActionInfoWithImageMap in actionImageMaps)
			{
				if (rPLActionInfoWithImageMap != null)
				{
					this.RenderImageMapArea(rPLActionInfoWithImageMap, imageWidth, imageHeight, uniqueName, xOffset, yOffset);
				}
			}
			this.WriteStream(HTML4Renderer.m_closeMap);
		}

		protected void RenderImageMapArea(RPLActionInfoWithImageMap actionImageMap, double imageWidth, double imageHeight, string uniqueName, int xOffset, int yOffset)
		{
			RPLAction rPLAction = null;
			if (actionImageMap.Actions != null && actionImageMap.Actions.Length > 0)
			{
				rPLAction = actionImageMap.Actions[0];
				if (!this.HasAction(rPLAction))
				{
					rPLAction = null;
				}
			}
			if (actionImageMap.ImageMaps != null && actionImageMap.ImageMaps.Count > 0)
			{
				RPLImageMap rPLImageMap = null;
				for (int i = 0; i < actionImageMap.ImageMaps.Count; i++)
				{
					rPLImageMap = actionImageMap.ImageMaps[i];
					string toolTip = rPLImageMap.ToolTip;
					if (rPLAction != null || toolTip != null)
					{
						this.WriteStream(HTML4Renderer.m_mapArea);
						this.RenderTabIndex();
						if (toolTip != null)
						{
							this.WriteToolTipAttribute(toolTip);
						}
						if (rPLAction != null)
						{
							this.RenderActionHref(rPLAction, RPLFormat.TextDecorations.None, null);
						}
						else
						{
							this.WriteStream(HTML4Renderer.m_nohref);
						}
						this.WriteStream(HTML4Renderer.m_mapShape);
						switch (rPLImageMap.Shape)
						{
						case RPLFormat.ShapeType.Circle:
							this.WriteStream(HTML4Renderer.m_circleShape);
							break;
						case RPLFormat.ShapeType.Polygon:
							this.WriteStream(HTML4Renderer.m_polyShape);
							break;
						default:
							this.WriteStream(HTML4Renderer.m_rectShape);
							break;
						}
						this.WriteStream(HTML4Renderer.m_quote);
						this.WriteStream(HTML4Renderer.m_mapCoords);
						float[] coordinates = rPLImageMap.Coordinates;
						long num = 0L;
						bool flag = true;
						int j = 0;
						if (coordinates != null)
						{
							for (; j < coordinates.Length - 1; j += 2)
							{
								if (!flag)
								{
									this.WriteStream(HTML4Renderer.m_comma);
								}
								num = (long)(coordinates[j] / 100.0 * imageWidth) + xOffset;
								this.WriteStream(num);
								this.WriteStream(HTML4Renderer.m_comma);
								num = (long)(coordinates[j + 1] / 100.0 * imageHeight) + yOffset;
								this.WriteStream(num);
								flag = false;
							}
							if (j < coordinates.Length)
							{
								this.WriteStream(HTML4Renderer.m_comma);
								num = (long)(coordinates[j] / 100.0 * imageWidth);
								this.WriteStream(num);
							}
						}
						this.WriteStream(HTML4Renderer.m_quote);
						this.WriteStreamCR(HTML4Renderer.m_closeBracket);
					}
				}
			}
		}

		protected void RenderCreateFixedHeaderFunction(string prefix, string fixedHeaderObject, StringBuilder function, StringBuilder arrayBuilder, bool createHeadersWithArray)
		{
			int num = 0;
			StringBuilder stringBuilder = function;
			if (createHeadersWithArray)
			{
				stringBuilder = arrayBuilder;
			}
			foreach (TablixFixedHeaderStorage fixedHeader in this.m_fixedHeaders)
			{
				string text = "frgh" + num + '_' + fixedHeader.HtmlId;
				string text2 = "fcgh" + num + '_' + fixedHeader.HtmlId;
				string text3 = "fch" + num + '_' + fixedHeader.HtmlId;
				string value = this.m_deviceInfo.HtmlPrefixId + text;
				string value2 = this.m_deviceInfo.HtmlPrefixId + text2;
				string value3 = this.m_deviceInfo.HtmlPrefixId + text3;
				if (fixedHeader.ColumnHeaders != null)
				{
					string value4 = prefix + "fcghArr" + num;
					arrayBuilder.Append(value4);
					arrayBuilder.Append("=new Array('");
					arrayBuilder.Append(fixedHeader.HtmlId);
					arrayBuilder.Append('\'');
					for (int i = 0; i < fixedHeader.ColumnHeaders.Count; i++)
					{
						arrayBuilder.Append(",'");
						arrayBuilder.Append(fixedHeader.ColumnHeaders[i]);
						arrayBuilder.Append('\'');
					}
					arrayBuilder.Append(");");
					if (!createHeadersWithArray)
					{
						arrayBuilder.Append(value2);
						arrayBuilder.Append("=null;");
						function.Append("if (!");
						function.Append(value2);
						function.Append("){");
						function.Append(value2);
						function.Append("=");
					}
					stringBuilder.Append(fixedHeaderObject);
					stringBuilder.Append(".CreateFixedColumnHeader(");
					stringBuilder.Append(value4);
					stringBuilder.Append(",'");
					stringBuilder.Append(text2);
					stringBuilder.Append("');");
					if (!createHeadersWithArray)
					{
						function.Append("}");
					}
				}
				if (fixedHeader.RowHeaders != null)
				{
					string value5 = prefix + "frhArr" + num;
					arrayBuilder.Append(value5);
					arrayBuilder.Append("=new Array('");
					arrayBuilder.Append(fixedHeader.HtmlId);
					arrayBuilder.Append('\'');
					for (int j = 0; j < fixedHeader.RowHeaders.Count; j++)
					{
						arrayBuilder.Append(",'");
						arrayBuilder.Append(fixedHeader.RowHeaders[j]);
						arrayBuilder.Append('\'');
					}
					arrayBuilder.Append(");");
					if (!createHeadersWithArray)
					{
						arrayBuilder.Append(value);
						arrayBuilder.Append("=null;");
						function.Append("if (!");
						function.Append(value);
						function.Append("){");
						function.Append(value);
						function.Append("=");
					}
					stringBuilder.Append(fixedHeaderObject);
					stringBuilder.Append(".CreateFixedRowHeader(");
					stringBuilder.Append(value5);
					stringBuilder.Append(",'");
					stringBuilder.Append(text);
					stringBuilder.Append("');");
					if (!createHeadersWithArray)
					{
						function.Append("}");
					}
				}
				if (fixedHeader.CornerHeaders != null)
				{
					string value6 = prefix + "fchArr" + num;
					arrayBuilder.Append(value6);
					arrayBuilder.Append("=new Array('");
					arrayBuilder.Append(fixedHeader.HtmlId);
					arrayBuilder.Append('\'');
					for (int k = 0; k < fixedHeader.CornerHeaders.Count; k++)
					{
						arrayBuilder.Append(",'");
						arrayBuilder.Append(fixedHeader.CornerHeaders[k]);
						arrayBuilder.Append('\'');
					}
					arrayBuilder.Append(");");
					if (!createHeadersWithArray)
					{
						arrayBuilder.Append(value3);
						arrayBuilder.Append("=null;");
						function.Append("if (!");
						function.Append(value3);
						function.Append("){");
						function.Append(value3);
						function.Append("=");
					}
					stringBuilder.Append(fixedHeaderObject);
					stringBuilder.Append(".CreateFixedRowHeader(");
					stringBuilder.Append(value6);
					stringBuilder.Append(",'");
					stringBuilder.Append(text3);
					stringBuilder.Append("');");
					if (!createHeadersWithArray)
					{
						function.Append("}");
					}
				}
				function.Append(fixedHeaderObject);
				function.Append(".ShowFixedTablixHeaders('");
				function.Append(fixedHeader.HtmlId);
				function.Append("','");
				function.Append((fixedHeader.BodyID != null) ? fixedHeader.BodyID : fixedHeader.HtmlId);
				function.Append("','");
				function.Append(text);
				function.Append("','");
				function.Append(text2);
				function.Append("','");
				function.Append(text3);
				function.Append("','");
				function.Append(fixedHeader.FirstRowGroupCol);
				function.Append("','");
				function.Append(fixedHeader.LastRowGroupCol);
				function.Append("','");
				function.Append(fixedHeader.LastColGroupRow);
				function.Append("');");
				num++;
			}
		}

		private void RenderServerDynamicImage(RPLElement dynamicImage, RPLDynamicImageProps dynamicImageProps, RPLElementPropsDef def, RPLItemMeasurement measurement, int borderContext, bool renderId, StyleContext styleContext)
		{
			if (dynamicImage != null)
			{
				bool flag = dynamicImageProps.ActionImageMapAreas != null && dynamicImageProps.ActionImageMapAreas.Length > 0;
				System.Drawing.Rectangle rectangle = this.RenderDynamicImage(measurement, dynamicImageProps);
				int xOffset = 0;
				int yOffset = 0;
				bool flag2 = !rectangle.IsEmpty;
				bool flag3 = !this.m_deviceInfo.IsBrowserSafari || this.m_deviceInfo.AllowScript || !styleContext.InTablix;
				if (flag3)
				{
					this.WriteStream(HTML4Renderer.m_openDiv);
				}
				bool flag4 = this.m_deviceInfo.DataVisualizationFitSizing == DataVisualizationFitSizing.Exact && styleContext.InTablix;
				if (flag2)
				{
					RPLFormat.Sizings sizing = (RPLFormat.Sizings)(flag4 ? 1 : 0);
					this.WriteOuterConsolidation(rectangle, sizing, dynamicImageProps.UniqueName);
					this.RenderReportItemStyle(dynamicImage, null, ref borderContext);
					xOffset = rectangle.Left;
					yOffset = rectangle.Top;
				}
				else if (flag4 && this.m_deviceInfo.AllowScript)
				{
					if (this.m_imgFitDivIdsStream == null)
					{
						this.CreateImgFitDivImageIdsStream();
					}
					this.WriteIdToSecondaryStream(this.m_imgFitDivIdsStream, dynamicImageProps.UniqueName + "_ifd");
					this.RenderReportItemId(dynamicImageProps.UniqueName + "_ifd");
				}
				if (flag3)
				{
					this.WriteStream(HTML4Renderer.m_closeBracket);
				}
				this.WriteStream(HTML4Renderer.m_img);
				if (this.m_browserIE)
				{
					this.WriteStream(HTML4Renderer.m_imgOnError);
				}
				if (renderId)
				{
					this.RenderReportItemId(dynamicImageProps.UniqueName);
				}
				this.WriteStream(HTML4Renderer.m_zeroBorder);
				bool flag5 = dynamicImage is RPLChart;
				if (flag)
				{
					this.WriteAttrEncoded(HTML4Renderer.m_useMap, "#" + this.m_deviceInfo.HtmlPrefixId + HTML4Renderer.m_mapPrefixString + dynamicImageProps.UniqueName);
					if (flag4)
					{
						this.OpenStyle();
						if (this.m_useInlineStyle && !flag2)
						{
							this.WriteStream(HTML4Renderer.m_styleHeight);
							this.WriteStream(HTML4Renderer.m_percent);
							this.WriteStream(HTML4Renderer.m_semiColon);
							this.WriteStream(HTML4Renderer.m_styleWidth);
							this.WriteStream(HTML4Renderer.m_percent);
							this.WriteStream(HTML4Renderer.m_semiColon);
							flag5 = false;
						}
						this.WriteStream("border-style:none;");
					}
				}
				else if (flag4 && this.m_useInlineStyle && !flag2)
				{
					this.PercentSizes();
					flag5 = false;
				}
				StyleContext styleContext2 = new StyleContext();
				if (!flag4 && (this.m_deviceInfo.IsBrowserIE7 || this.m_deviceInfo.IsBrowserIE6))
				{
					styleContext2.RenderMeasurements = false;
					styleContext2.RenderMinMeasurements = false;
				}
				if (!flag2)
				{
					if (flag4)
					{
						this.RenderReportItemStyle(dynamicImage, null, ref borderContext, styleContext2);
					}
					else if (flag5)
					{
						RPLElementProps elementProps = dynamicImage.ElementProps;
						StyleContext styleContext3 = new StyleContext();
						styleContext3.RenderMeasurements = false;
						this.OpenStyle();
						this.RenderMeasurementStyle(measurement.Height, measurement.Width);
						this.RenderReportItemStyle(dynamicImage, elementProps, def, measurement, styleContext3, ref borderContext, def.ID);
					}
					else
					{
						this.RenderReportItemStyle(dynamicImage, measurement, ref borderContext, styleContext2);
					}
				}
				else
				{
					this.WriteClippedDiv(rectangle);
				}
				this.WriteToolTip(dynamicImageProps);
				this.WriteStream(HTML4Renderer.m_src);
				this.RenderDynamicImageSrc(dynamicImageProps);
				this.WriteStreamCR(HTML4Renderer.m_closeTag);
				if (flag)
				{
					this.RenderImageMapAreas(dynamicImageProps.ActionImageMapAreas, (double)measurement.Width, (double)measurement.Height, dynamicImageProps.UniqueName, xOffset, yOffset);
				}
				if (flag3)
				{
					this.WriteStream(HTML4Renderer.m_closeDiv);
				}
			}
		}

		private void RenderBorderLine(RPLElement reportItem)
		{
			object obj = null;
			IRPLStyle style = reportItem.ElementProps.Style;
			obj = style[10];
			if (obj != null)
			{
				this.WriteStream(obj.ToString());
				this.WriteStream(HTML4Renderer.m_space);
			}
			obj = style[5];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.BorderStyles)obj);
				this.WriteStream(obj);
				this.WriteStream(HTML4Renderer.m_space);
			}
			obj = style[0];
			if (obj != null)
			{
				this.WriteStream((string)obj);
			}
		}

		private string CalculateRowHeaderId(RPLTablixCell cell, bool fixedHeader, string tablixID, int row, int col, TablixFixedHeaderStorage headerStorage, bool useElementName, bool fixedCornerHeader)
		{
			string text = null;
			if (cell is RPLTablixMemberCell)
			{
				if (((RPLTablixMemberCell)cell).GroupLabel != null)
				{
					text = ((RPLTablixMemberCell)cell).UniqueName;
				}
				else if (!fixedHeader && useElementName && cell.Element != null && cell.Element.ElementProps != null)
				{
					text = cell.Element.ElementProps.UniqueName;
				}
			}
			if (fixedHeader)
			{
				if (text == null)
				{
					text = tablixID + "r" + row + "c" + col;
				}
				if (headerStorage != null)
				{
					headerStorage.RowHeaders.Add(text);
					if (headerStorage.CornerHeaders != null && fixedCornerHeader)
					{
						headerStorage.CornerHeaders.Add(text);
					}
				}
			}
			return text;
		}

		private void RenderAccessibleHeaders(RPLTablix tablix, bool fixedHeader, int numCols, int col, int colSpan, int row, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, HTMLHeader[] rowHeaderIds, string[] colHeaderIds, OmittedHeaderStack omittedHeaders, ref string id)
		{
			int currentLevel = -1;
			if (tablix.RowHeaderColumns == 0 && omittedCells != null && omittedCells.Count > 0)
			{
				foreach (RPLTablixMemberCell omittedCell in omittedCells)
				{
					RPLTablixMemberDef tablixMemberDef = omittedCell.TablixMemberDef;
					if (tablixMemberDef != null && tablixMemberDef.IsStatic && tablixMemberDef.StaticHeadersTree)
					{
						if (id == null && cell.Element != null && cell.Element.ElementProps.UniqueName != null)
						{
							id = cell.Element.ElementProps.UniqueName;
						}
						currentLevel = tablixMemberDef.Level;
						omittedHeaders.Push(tablixMemberDef.Level, col, colSpan, id, numCols);
					}
				}
			}
			if (row >= tablix.ColumnHeaderRows && !fixedHeader && (col < tablix.ColsBeforeRowHeaders || tablix.RowHeaderColumns <= 0 || col >= tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders))
			{
				bool flag = false;
				string text = colHeaderIds[cell.ColIndex];
				if (!string.IsNullOrEmpty(text))
				{
					this.WriteStream(HTML4Renderer.m_headers);
					this.WriteStream(text);
					flag = true;
				}
				foreach (HTMLHeader hTMLHeader in rowHeaderIds)
				{
					string iD = hTMLHeader.ID;
					if (!string.IsNullOrEmpty(iD))
					{
						if (flag)
						{
							this.WriteStream(HTML4Renderer.m_space);
						}
						else
						{
							this.WriteStream(HTML4Renderer.m_headers);
						}
						this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
						this.WriteStream(iD);
						flag = true;
					}
				}
				string headers = omittedHeaders.GetHeaders(col, currentLevel, HttpUtility.HtmlAttributeEncode(this.m_deviceInfo.HtmlPrefixId));
				if (!string.IsNullOrEmpty(headers))
				{
					if (flag)
					{
						this.WriteStream(HTML4Renderer.m_space);
					}
					else
					{
						this.WriteStream(HTML4Renderer.m_headers);
					}
					this.WriteStream(headers);
					flag = true;
				}
				if (flag)
				{
					this.WriteStream(HTML4Renderer.m_quote);
				}
			}
		}

		private void RenderTablixCell(RPLTablix tablix, bool fixedHeader, string tablixID, int numCols, int numRows, int col, int colSpan, int row, int tablixContext, RPLTablixCell cell, List<RPLTablixMemberCell> omittedCells, ref int omittedIndex, StyleContext styleContext, TablixFixedHeaderStorage headerStorage, HTMLHeader[] rowHeaderIds, string[] colHeaderIds, OmittedHeaderStack omittedHeaders)
		{
			bool lastCol = col + colSpan == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(cell.ColIndex, cell.ColSpan);
			styleContext.ZeroWidth = (columnWidth == 0.0);
			int startIndex = this.RenderZeroWidthTDsForTablix(col, colSpan, tablix);
			colSpan = this.GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
			bool useElementName = this.m_deviceInfo.AccessibleTablix && tablix.RowHeaderColumns > 0 && col >= tablix.ColsBeforeRowHeaders && col < tablix.RowHeaderColumns + tablix.ColsBeforeRowHeaders;
			bool fixedCornerHeader = fixedHeader && tablix.FixedColumns[col] && tablix.FixedRow(row);
			string text = this.CalculateRowHeaderId(cell, fixedHeader, tablixID, cell.RowIndex, cell.ColIndex, headerStorage, useElementName, fixedCornerHeader);
			this.WriteStream(HTML4Renderer.m_openTD);
			if (this.m_deviceInfo.AccessibleTablix)
			{
				this.RenderAccessibleHeaders(tablix, fixedHeader, numCols, cell.ColIndex, colSpan, cell.RowIndex, cell, omittedCells, rowHeaderIds, colHeaderIds, omittedHeaders, ref text);
			}
			if (text != null)
			{
				this.RenderReportItemId(text);
			}
			int rowSpan = cell.RowSpan;
			if (cell.RowSpan > 1)
			{
				this.WriteStream(HTML4Renderer.m_rowSpan);
				this.WriteStream(cell.RowSpan.ToString(CultureInfo.InvariantCulture));
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_inlineHeight);
				this.WriteStream(Utility.MmToPxAsString((double)tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
				this.WriteStream(HTML4Renderer.m_quote);
			}
			if (colSpan > 1)
			{
				this.WriteStream(HTML4Renderer.m_colSpan);
				this.WriteStream(cell.ColSpan.ToString(CultureInfo.InvariantCulture));
				this.WriteStream(HTML4Renderer.m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int num = 0;
				this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref num);
				this.RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
				this.RenderTablixReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, row + rowSpan == numRows, element, ref num);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					this.OpenStyle();
					this.WriteStream(HTML4Renderer.m_displayNone);
					this.CloseStyle(true);
				}
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.RenderTablixOmittedHeaderCells(omittedCells, col, lastCol, ref omittedIndex);
				this.WriteStream(HTML4Renderer.m_nbsp);
			}
			this.WriteStream(HTML4Renderer.m_closeTD);
			this.RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		private void RenderTablixOmittedHeaderCells(List<RPLTablixMemberCell> omittedHeaders, int colIndex, bool lastCol, ref int omittedIndex)
		{
			if (omittedHeaders != null)
			{
				while (true)
				{
					if (omittedIndex >= omittedHeaders.Count)
					{
						break;
					}
					if (omittedHeaders[omittedIndex].ColIndex != colIndex)
					{
						if (!lastCol)
						{
							break;
						}
						if (omittedHeaders[omittedIndex].ColIndex <= colIndex)
						{
							break;
						}
					}
					RPLTablixMemberCell rPLTablixMemberCell = omittedHeaders[omittedIndex];
					if (rPLTablixMemberCell.GroupLabel != null)
					{
						this.RenderNavigationId(rPLTablixMemberCell.UniqueName);
					}
					omittedIndex++;
				}
			}
		}

		private void RenderColumnHeaderTablixCell(RPLTablix tablix, string tablixID, int numCols, int col, int colSpan, int row, int tablixContext, RPLTablixCell cell, StyleContext styleContext, TablixFixedHeaderStorage headerStorage, List<RPLTablixOmittedRow> omittedRows, int[] omittedIndices)
		{
			bool lastCol = col + colSpan == numCols;
			bool zeroWidth = styleContext.ZeroWidth;
			float columnWidth = tablix.GetColumnWidth(col, colSpan);
			styleContext.ZeroWidth = (columnWidth == 0.0);
			int startIndex = this.RenderZeroWidthTDsForTablix(col, colSpan, tablix);
			colSpan = this.GetColSpanMinusZeroWidthColumns(col, colSpan, tablix);
			this.WriteStream(HTML4Renderer.m_openTD);
			int rowSpan = cell.RowSpan;
			string text = null;
			if (cell is RPLTablixMemberCell && (((RPLTablixMemberCell)cell).GroupLabel != null || this.m_deviceInfo.AccessibleTablix))
			{
				text = ((RPLTablixMemberCell)cell).UniqueName;
				if (text == null && cell.Element != null && cell.Element.ElementProps != null)
				{
					text = cell.Element.ElementProps.UniqueName;
					((RPLTablixMemberCell)cell).UniqueName = text;
				}
				if (text != null)
				{
					this.RenderReportItemId(text);
				}
			}
			if (tablix.FixedColumns[col])
			{
				if (text == null)
				{
					text = tablixID + "r" + row + "c" + col;
					this.RenderReportItemId(text);
				}
				headerStorage.RowHeaders.Add(text);
				if (headerStorage.CornerHeaders != null)
				{
					headerStorage.CornerHeaders.Add(text);
				}
			}
			if (rowSpan > 1)
			{
				this.WriteStream(HTML4Renderer.m_rowSpan);
				this.WriteStream(cell.RowSpan.ToString(CultureInfo.InvariantCulture));
				this.WriteStream(HTML4Renderer.m_quote);
				this.WriteStream(HTML4Renderer.m_inlineHeight);
				this.WriteStream(Utility.MmToPxAsString((double)tablix.GetRowHeight(cell.RowIndex, cell.RowSpan)));
				this.WriteStream(HTML4Renderer.m_quote);
			}
			if (colSpan > 1)
			{
				this.WriteStream(HTML4Renderer.m_colSpan);
				this.WriteStream(cell.ColSpan.ToString(CultureInfo.InvariantCulture));
				this.WriteStream(HTML4Renderer.m_quote);
			}
			RPLElement element = cell.Element;
			if (element != null)
			{
				int num = 0;
				this.RenderTablixReportItemStyle(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, false, element, ref num);
				for (int i = 0; i < omittedRows.Count; i++)
				{
					this.RenderTablixOmittedHeaderCells(omittedRows[i].OmittedHeaders, col, lastCol, ref omittedIndices[i]);
				}
				this.RenderTablixReportItem(tablix, tablixContext, cell, styleContext, col == 0, lastCol, row == 0, false, element, ref num);
			}
			else
			{
				if (styleContext.ZeroWidth)
				{
					this.OpenStyle();
					this.WriteStream(HTML4Renderer.m_displayNone);
					this.CloseStyle(true);
				}
				this.WriteStream(HTML4Renderer.m_closeBracket);
				for (int j = 0; j < omittedRows.Count; j++)
				{
					this.RenderTablixOmittedHeaderCells(omittedRows[j].OmittedHeaders, col, lastCol, ref omittedIndices[j]);
				}
				this.WriteStream(HTML4Renderer.m_nbsp);
			}
			this.WriteStream(HTML4Renderer.m_closeTD);
			this.RenderZeroWidthTDsForTablix(startIndex, colSpan, tablix);
			styleContext.ZeroWidth = zeroWidth;
		}

		protected void CreateGrowRectIdsStream()
		{
			string streamName = HTML4Renderer.GetStreamName(this.m_rplReport.ReportName, this.m_pageNum, "_gr");
			Stream stream = this.CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", true, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateOnly);
			this.m_growRectangleIdsStream = new BufferedStream(stream);
			this.m_needsGrowRectangleScript = true;
		}

		protected void CreateFitVertTextIdsStream()
		{
			string streamName = HTML4Renderer.GetStreamName(this.m_rplReport.ReportName, this.m_pageNum, "_fvt");
			Stream stream = this.CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", true, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateOnly);
			this.m_fitVertTextIdsStream = new BufferedStream(stream);
			this.m_needsFitVertTextScript = true;
		}

		protected void CreateImgConImageIdsStream()
		{
			string streamName = HTML4Renderer.GetStreamName(this.m_rplReport.ReportName, this.m_pageNum, "_ici");
			Stream stream = this.CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", true, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateOnly);
			this.m_imgConImageIdsStream = new BufferedStream(stream);
		}

		protected void CreateImgFitDivImageIdsStream()
		{
			string streamName = HTML4Renderer.GetStreamName(this.m_rplReport.ReportName, this.m_pageNum, "_ifd");
			Stream stream = this.CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", true, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateOnly);
			this.m_imgFitDivIdsStream = new BufferedStream(stream);
			this.m_emitImageConsolidationScaling = true;
		}

		[SecurityTreatAsSafe]
		[SecurityCritical]
		protected Stream CreateStream(string name, string extension, Encoding encoding, string mimeType, bool willSeek, AspNetCore.ReportingServices.Interfaces.StreamOper operation)
		{
			return this.m_createAndRegisterStreamCallback(name, extension, encoding, mimeType, willSeek, operation);
		}

		protected void RenderSecondaryStreamIdsSpanTag(Stream secondaryStream, string tagId)
		{
			if (secondaryStream != null && secondaryStream.CanSeek)
			{
				this.WriteStream(HTML4Renderer.m_openSpan);
				this.RenderReportItemId(tagId);
				this.WriteStream(" ids=\"");
				secondaryStream.Seek(0L, SeekOrigin.Begin);
				byte[] array = new byte[4096];
				int count;
				while ((count = secondaryStream.Read(array, 0, array.Length)) > 0)
				{
					this.m_mainStream.Write(array, 0, count);
				}
				this.WriteStream("\"");
				this.WriteStream(HTML4Renderer.m_closeBracket);
				this.WriteStreamCR(HTML4Renderer.m_closeSpan);
			}
		}

		protected void RenderSecondaryStreamSpanTagsForJavascriptFunctions()
		{
			this.RenderSecondaryStreamIdsSpanTag(this.m_growRectangleIdsStream, "growRectangleIdsTag");
			this.RenderSecondaryStreamIdsSpanTag(this.m_fitVertTextIdsStream, "fitVertTextIdsTag");
			this.RenderSecondaryStreamIdsSpanTag(this.m_imgFitDivIdsStream, "imgFitDivIdsTag");
			this.RenderSecondaryStreamIdsSpanTag(this.m_imgConImageIdsStream, "imgConImageIdsTag");
		}
	}
}
