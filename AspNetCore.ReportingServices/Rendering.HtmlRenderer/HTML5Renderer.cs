using AspNetCore.ReportingServices.Diagnostics;
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
	internal abstract class HTML5Renderer : IHtmlReportWriter, IHtmlWriter, IHtmlRenderer
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

		internal enum FontAttributes
		{
			None,
			Partial,
			All
		}

		internal enum PageSection
		{
			Body,
			PageHeader,
			PageFooter
		}

		internal const string DrillthroughAction = "Drillthrough";

		internal const string BookmarkAction = "Bookmark";

		internal const string GetImageKey = "GetImage";

		internal const int IgnoreLeft = 1;

		internal const int IgnoreRight = 2;

		internal const int IgnoreTop = 4;

		internal const int IgnoreBottom = 8;

		internal const int IgnoreAll = 15;

		internal const float MaxWordSize = 558.8f;

		internal const string FixedRowGroupHeaderPrefix = "frgh";

		internal const string FixedCornerHeaderPrefix = "fch";

		internal const string FixedColGroupHeaderPrefix = "fcgh";

		internal const string FixedRGHArrayPrefix = "frhArr";

		internal const string FixedCGHArrayPrefix = "fcghArr";

		internal const string FixedCHArrayPrefix = "fchArr";

		internal const string ReportDiv = "oReportDiv";

		internal const string ReportCell = "oReportCell";

		private const char PathSeparator = '/';

		protected const string ImageConImageSuffix = "_ici";

		protected internal const string ImageFitDivSuffix = "_ifd";

		protected internal const string NavigationAnchorSuffix = "_na";

		protected internal const long FitProptionalDefaultSize = 5L;

		protected const int SecondaryStreamBufferSize = 4096;

		internal const string SortAction = "Sort";

		internal const string ToggleAction = "Toggle";

		internal const char StreamNameSeparator = '_';

		internal const string PageStyleName = "p";

		internal const string MHTMLPrefix = "cid:";

		internal const string CSSSuffix = "style";

		protected const string m_resourceNamespace = "AspNetCore.ReportingServices.Rendering.HtmlRenderer.RendererResources";

		protected internal DeviceInfo m_deviceInfo;

		protected Stream m_mainStream;

		protected bool m_isStyleOpen;

		protected internal ArrayList m_fixedHeaders;

		protected PageSection m_pageSection;

		protected internal Stack m_linkToChildStack;

		protected bool m_htmlFragment;

		protected int m_tabIndexNum;

		internal int m_currentHitCount;

		internal Encoding m_encoding;

		internal static char[] m_cssDelimiters = new char[13]
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

		protected bool m_hasOnePage = true;

		protected internal RPLReport m_rplReport;

		protected RPLPageContent m_pageContent;

		protected RPLReportSection m_rplReportSection;

		protected IReportWrapper m_report;

		protected ISPBProcessing m_spbProcessing;

		protected internal IElementExtender m_elementExtender;

		protected Hashtable m_usedStyles;

		protected NameValueCollection m_serverParams;

		protected NameValueCollection m_rawDeviceInfo;

		protected Dictionary<string, string> m_images;

		protected byte[] m_stylePrefixIdBytes;

		protected internal int m_pageNum;

		protected AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream m_createAndRegisterStreamCallback;

		protected internal bool m_fitPropImages;

		protected internal bool m_browserIE = true;

		protected RequestType m_requestType;

		protected Stream m_styleStream;

		protected internal Stream m_growRectangleIdsStream;

		protected internal Stream m_fitVertTextIdsStream;

		protected internal Stream m_imgFitDivIdsStream;

		protected Stream m_imgConImageIdsStream;

		protected internal bool m_useInlineStyle;

		protected bool m_pageWithBookmarkLinks;

		protected bool m_pageWithSortClicks;

		protected bool m_allPages;

		protected int m_outputLineLength;

		protected bool m_onlyVisibleStyles;

		protected internal SecondaryStreams m_createSecondaryStreams = SecondaryStreams.Server;

		protected Hashtable m_duplicateItems;

		protected internal string m_searchText;

		protected bool m_emitImageConsolidationScaling;

		protected bool m_needsCanGrowFalseScript;

		protected internal bool m_needsFitVertTextScript;

		internal static string m_searchHitIdPrefix = "oHit";

		internal static string m_standardLineBreak = "\n";

		protected bool m_pageHasStyle;

		protected internal bool m_isBody;

		protected internal bool m_usePercentWidth;

		internal bool m_expandItem;

		protected internal ReportContext m_reportContext;

		private bool m_renderTableHeight;

		private string m_contextLanguage;

		private bool m_allowBandTable = true;

		protected byte[] m_styleClassPrefix;

		protected internal bool IsFragment
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

		private string GetCurrentHtmlAsText()
		{
			MemoryStream memoryStream = new MemoryStream();
			this.m_mainStream.Seek(0L, SeekOrigin.Begin);
			StreamSupport.CopyStreamUsingBuffer(this.m_mainStream, memoryStream, 1024);
			this.m_mainStream.Seek(0L, SeekOrigin.End);
			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}

		public HTML5Renderer(IReportWrapper report, ISPBProcessing spbProcessing, NameValueCollection reportServerParams, DeviceInfo deviceInfo, NameValueCollection rawDeviceInfo, NameValueCollection browserCaps, AspNetCore.ReportingServices.Interfaces.CreateAndRegisterStream createAndRegisterStreamCallback, SecondaryStreams secondaryStreams, IElementExtender elementExtender = null)
		{
			this.SearchText = deviceInfo.FindString;
			this.m_report = report;
			this.m_spbProcessing = spbProcessing;
			this.m_elementExtender = (elementExtender ?? new NoOpElementExtender());
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
			this.m_reportContext = new ReportContext();
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

		private static bool HasBorderStyle(object borderStyle)
		{
			if (borderStyle != null)
			{
				return (RPLFormat.BorderStyles)borderStyle != RPLFormat.BorderStyles.None;
			}
			return false;
		}

		protected virtual void RenderInteractionAction(RPLAction action, ref bool hasHref)
		{
			this.RenderControlActionScript(action);
			this.WriteStream(HTMLElements.m_href);
			this.WriteStream(HTMLElements.m_quote);
			this.OpenStyle();
			this.WriteStream(HTMLElements.m_cursorHand);
			this.WriteStream(HTMLElements.m_semiColon);
			hasHref = true;
		}

		protected void RenderControlActionScript(RPLAction action)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = null;
			string actionUrl = null;
			if (action.DrillthroughId != null)
			{
				HTML5Renderer.QuoteString(stringBuilder, action.DrillthroughId);
				text = "Drillthrough";
				actionUrl = action.DrillthroughUrl;
			}
			else
			{
				HTML5Renderer.QuoteString(stringBuilder, action.BookmarkLink);
				text = "Bookmark";
			}
			this.RenderOnClickActionScript(text, stringBuilder.ToString(), actionUrl);
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

		public virtual void WriteStream(byte[] theBytes)
		{
			this.m_mainStream.Write(theBytes, 0, theBytes.Length);
		}

		internal void WriteAccesibilityTags(string nameToUseIfNoTooltip, RPLElementProps itemProperties, bool reportItemIsTopLevel)
		{
			if (reportItemIsTopLevel)
			{
				string tooltip = this.GetTooltip(itemProperties);
				tooltip = (string.IsNullOrEmpty(tooltip) ? nameToUseIfNoTooltip : tooltip);
				this.WriteAriaAccessibleTags(tooltip);
			}
		}

		internal void WriteAriaPresentationRole()
		{
			this.WriteStream(HTMLElements.m_space);
			this.WriteStream(HTMLElements.m_role);
			this.WriteStream(HTMLElements.m_equal);
			this.WriteStream(HTMLElements.m_quote);
			this.WriteStream(HTMLElements.m_presentationRole);
			this.WriteStream(HTMLElements.m_quote);
		}

		internal void WriteAriaAccessibleTags(string accessibleAriaName)
		{
			this.WriteStream(HTMLElements.m_space);
			this.WriteStream(HTMLElements.m_role);
			this.WriteStream(HTMLElements.m_equal);
			this.WriteStream(HTMLElements.m_quote);
			this.WriteStream(HTMLElements.m_navigationRole);
			this.WriteStream(HTMLElements.m_quote);
			if (!string.IsNullOrEmpty(accessibleAriaName))
			{
				this.WriteStream(HTMLElements.m_space);
				this.WriteStream(HTMLElements.m_ariaLabel);
				this.WriteStream(HTMLElements.m_equal);
				this.WriteStream(HTMLElements.m_quote);
				this.WriteStream(accessibleAriaName);
				this.WriteStream(HTMLElements.m_quote);
			}
		}

		protected internal virtual void RenderReportItemId(string repItemId)
		{
			this.WriteStream(HTMLElements.m_id);
			this.WriteReportItemId(repItemId);
			this.WriteStream(HTMLElements.m_quote);
		}

		internal string GetTooltip(RPLElementProps props)
		{
			RPLItemProps rPLItemProps = props as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			string toolTip = rPLItemProps.ToolTip;
			if (toolTip == null)
			{
				toolTip = rPLItemPropsDef.ToolTip;
			}
			return toolTip;
		}

		protected internal void WriteToolTip(RPLElementProps props)
		{
			string tooltip = this.GetTooltip(props);
			if (tooltip != null)
			{
				this.WriteToolTipAttribute(tooltip);
			}
		}

		protected internal void WriteToolTipAttribute(string tooltip)
		{
			this.WriteAttrEncoded(HTMLElements.m_alt, tooltip);
			this.WriteAttrEncoded(HTMLElements.m_title, tooltip);
		}

		internal void OpenStyle()
		{
			if (!this.m_isStyleOpen)
			{
				this.m_isStyleOpen = true;
				this.WriteStream(HTMLElements.m_openStyle);
			}
		}

		internal void CloseStyle(bool renderQuote)
		{
			if (this.m_isStyleOpen && renderQuote)
			{
				this.WriteStream(HTMLElements.m_quote);
			}
			this.m_isStyleOpen = false;
		}

		protected internal void RenderMeasurementHeight(float height, bool renderMin)
		{
			if (renderMin)
			{
				this.WriteStream(HTMLElements.m_styleMinHeight);
			}
			else
			{
				this.WriteStream(HTMLElements.m_styleHeight);
			}
			this.WriteRSStream(height);
			this.WriteStream(HTMLElements.m_semiColon);
		}

		internal void RenderMeasurementMinHeight(float height)
		{
			this.WriteStream(HTMLElements.m_styleMinHeight);
			this.WriteRSStream(height);
			this.WriteStream(HTMLElements.m_semiColon);
		}

		protected internal void RenderMeasurementWidth(float width, bool renderMinWidth)
		{
			this.WriteStream(HTMLElements.m_styleWidth);
			this.WriteRSStream(width);
			this.WriteStream(HTMLElements.m_semiColon);
			if (renderMinWidth)
			{
				this.RenderMeasurementMinWidth(width);
			}
		}

		protected internal void RenderMeasurementMaxHeight(float maxHeight)
		{
			this.WriteStream(HTMLElements.m_styleMaxHeight);
			this.WriteRSStream(maxHeight);
			this.WriteStream(HTMLElements.m_semiColon);
		}

		protected internal void RenderMeasurementMinWidth(float minWidth)
		{
			this.WriteStream(HTMLElements.m_styleMinWidth);
			this.WriteRSStream(minWidth);
			this.WriteStream(HTMLElements.m_semiColon);
		}

		protected internal void RenderMeasurementMaxWidth(float maxWidth)
		{
			this.WriteStream(HTMLElements.m_styleMaxWidth);
			this.WriteRSStream(maxWidth);
			this.WriteStream(HTMLElements.m_semiColon);
		}

		protected internal void RenderMeasurementLeft(float left)
		{
			this.WriteStream(HTMLElements.m_styleLeft);
			this.WriteRSStream(left);
			this.WriteStream(HTMLElements.m_semiColon);
		}

		protected internal void RenderMeasurementHeight(float height)
		{
			this.RenderMeasurementHeight(height, false);
		}

		protected void RenderMeasurementWidth(float width)
		{
			this.RenderMeasurementWidth(width, false);
		}

		internal void WriteReportItemId(string repItemId)
		{
			this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
			this.WriteStream(repItemId);
		}

		internal void WriteAttrEncoded(byte[] attributeName, string theString)
		{
			this.WriteAttribute(attributeName, this.m_encoding.GetBytes(HttpUtility.HtmlAttributeEncode(theString)));
		}

		internal void WriteRSStream(float size)
		{
			this.WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
			this.WriteStream(HTMLElements.m_mm);
		}

		internal void WriteAttrEncoded(string theString)
		{
			this.WriteStream(HttpUtility.HtmlAttributeEncode(theString));
		}

		protected virtual void WriteAttribute(byte[] attributeName, byte[] value)
		{
			this.WriteStream(attributeName);
			this.WriteStream(value);
			this.WriteStream(HTMLElements.m_quote);
		}

		internal void RenderNavigationId(string navigationId)
		{
			if (!this.IsFragment)
			{
				this.WriteStream(HTMLElements.m_openSpan);
				this.WriteStream(HTMLElements.m_id);
				this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
				this.WriteStream(navigationId);
				this.WriteStream(HTMLElements.m_closeTag);
			}
		}

		protected internal void WriteDStream(float size)
		{
			this.WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
		}

		protected internal bool NeedReportItemId(RPLElement repItem, RPLElementProps props)
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

		protected internal static float GetInnerContainerWidthSubtractHalfBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			return HTML5Renderer.GetInnerContainerMeasurementSubtractingHalfBorders(measurement, containerStyle, 6, 11, 7, 12, HTML5Renderer.GetInnerContainerWidth(measurement, containerStyle));
		}

		internal static float GetInnerContainerHeightSubtractHalfBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			return HTML5Renderer.GetInnerContainerMeasurementSubtractingHalfBorders(measurement, containerStyle, 8, 13, 9, 14, HTML5Renderer.GetInnerContainerHeight(measurement, containerStyle));
		}

		private static float GetInnerContainerMeasurementSubtractingHalfBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle, byte border1StyleProps, byte border1WidthProps, byte border2StyleProps, byte border2WidthProps, float length)
		{
			if (measurement == null)
			{
				return -1f;
			}
			object defaultBorderStyle = containerStyle[5];
			object defaultBorderWidth = containerStyle[10];
			object specificBorderStyle = containerStyle[border1StyleProps];
			object specificBorderWidth = containerStyle[border1WidthProps];
			object specificBorderStyle2 = containerStyle[border2StyleProps];
			object specificBorderWidth2 = containerStyle[border2WidthProps];
			float width = 0f;
			width = HTML5Renderer.SubtractBorderStyles(width, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			width = HTML5Renderer.SubtractBorderStyles(width, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
			length = (float)(length + 0.5 * width);
			return Math.Max(length, 1f);
		}

		internal float GetInnerContainerHeightSubtractBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			if (measurement == null)
			{
				return -1f;
			}
			float innerContainerHeight = HTML5Renderer.GetInnerContainerHeight(measurement, containerStyle);
			object defaultBorderStyle = containerStyle[5];
			object defaultBorderWidth = containerStyle[10];
			object specificBorderWidth = containerStyle[13];
			object specificBorderStyle = containerStyle[8];
			innerContainerHeight = HTML5Renderer.SubtractBorderStyles(innerContainerHeight, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			specificBorderWidth = containerStyle[14];
			specificBorderStyle = containerStyle[9];
			innerContainerHeight = HTML5Renderer.SubtractBorderStyles(innerContainerHeight, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			if (innerContainerHeight <= 0.0)
			{
				innerContainerHeight = 1f;
			}
			return innerContainerHeight;
		}

		protected internal void RenderElementHyperlinkAllTextStyles(RPLElementStyle style, RPLAction action, string id)
		{
			this.WriteStream(HTMLElements.m_openA);
			this.RenderTabIndex();
			bool flag = false;
			if (action.Hyperlink != null)
			{
				this.WriteStream(HTMLElements.m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + HTMLElements.m_quoteString);
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
				this.WriteStream(HTMLElements.m_target);
				this.WriteStream(this.m_deviceInfo.LinkTarget);
				this.WriteStream(HTMLElements.m_quote);
			}
			this.WriteStream(HTMLElements.m_closeBracket);
		}

		protected internal static int GetNewContext(int borderContext, bool left, bool right, bool top, bool bottom)
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

		protected internal virtual bool NeedSharedToggleParent(RPLTextBoxProps textBoxProps)
		{
			if (!this.IsFragment)
			{
				return textBoxProps.IsToggleParent;
			}
			return false;
		}

		protected internal virtual bool CanSort(RPLTextBoxPropsDef textBoxDef)
		{
			if (!this.IsFragment)
			{
				return textBoxDef.CanSort;
			}
			return false;
		}

		protected internal string GetTextBoxClass(RPLTextBoxPropsDef textBoxPropsDef, RPLTextBoxProps textBoxProps, RPLStyleProps nonSharedStyle, string defaultClass)
		{
			if (textBoxPropsDef.SharedTypeCode == TypeCode.Object && (nonSharedStyle == null || nonSharedStyle.Count == 0 || nonSharedStyle[25] == null))
			{
				object obj = textBoxProps.Style[25];
				if (obj != null && (RPLFormat.TextAlignments)obj == RPLFormat.TextAlignments.General)
				{
					if (HTML5Renderer.GetTextAlignForType(textBoxProps))
					{
						return defaultClass + "r";
					}
					return defaultClass + "l";
				}
			}
			return defaultClass;
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
				return HTML5Renderer.IsWritingModeVertical((RPLFormat.WritingModes)obj);
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

		internal bool HasAction(RPLActionInfo actionInfo)
		{
			if (actionInfo != null && actionInfo.Actions != null)
			{
				return this.HasAction(actionInfo.Actions[0]);
			}
			return false;
		}

		internal static float GetInnerContainerHeight(RPLItemMeasurement measurement, IRPLStyle containerStyle)
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

		protected static float SubtractBorderStyles(float width, object defaultBorderStyle, object specificBorderStyle, object defaultBorderWidth, object specificBorderWidth)
		{
			object obj = specificBorderWidth ?? defaultBorderWidth;
			if (obj != null && (HTML5Renderer.HasBorderStyle(specificBorderStyle) || (specificBorderStyle == null && HTML5Renderer.HasBorderStyle(defaultBorderStyle))))
			{
				RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
				width -= (float)rPLReportSize.ToMillimeters();
			}
			return width;
		}

		internal void RenderTabIndex()
		{
			this.WriteStream(HTMLElements.m_tabIndex);
			this.WriteStream(++this.m_tabIndexNum);
			this.WriteStream(HTMLElements.m_quote);
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

		internal void RenderOnClickActionScript(string actionType, string actionArg, string actionUrl = null)
		{
			this.WriteStream(" onclick=\"");
			this.WriteStream(this.m_deviceInfo.ActionScript);
			this.WriteStream("('");
			this.WriteStream(actionType);
			this.WriteStream("','");
			this.WriteStream(actionArg);
			this.WriteStream("',event);return false;\"");
			this.WriteStream(" onkeypress=\"");
			this.WriteStream(HTMLElements.m_checkForEnterKey);
			this.WriteStream(this.m_deviceInfo.ActionScript);
			this.WriteStream("('");
			this.WriteStream(actionType);
			this.WriteStream("','");
			this.WriteStream(actionArg);
			this.WriteStream("',event);return false;}\"");
			if (!string.IsNullOrEmpty(actionUrl))
			{
				this.WriteStream(" data-drillThroughUrl=\"");
				this.WriteStream(actionUrl);
				this.WriteStream(HTMLElements.m_quote);
			}
		}

		protected static string GetStyleStreamName(string aReportName, int aPageNumber)
		{
			return HTML5Renderer.GetStreamName(aReportName, aPageNumber, "style");
		}

		internal static string GetStreamName(string aReportName, int aPageNumber, string suffix)
		{
			if (aPageNumber > 0)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{1}{3}", aReportName, '_', suffix, aPageNumber);
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", aReportName, '_', suffix);
		}

		internal string GetReportItemPath(string reportItemName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in this.m_reportContext.GetPath())
			{
				stringBuilder.Append(item).Append('/');
			}
			stringBuilder.Append(reportItemName);
			return stringBuilder.ToString();
		}

		internal static string HandleSpecialFontCharacters(string fontName)
		{
			if (fontName.IndexOfAny(HTML5Renderer.m_cssDelimiters) != -1)
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

		protected internal abstract void RenderSortAction(RPLTextBoxProps textBoxProps, RPLFormat.SortOptions sortState);

		protected abstract void RenderInternalImageSrc();

		protected internal abstract void RenderToggleImage(RPLTextBoxProps textBoxProps);

		public abstract void Render(TextWriter outputWriter);

		protected abstract void WriteScrollbars();

		protected abstract void WriteFixedHeaderOnScrollScript();

		protected abstract void WriteFixedHeaderPropertyChangeScript();

		protected internal abstract void WriteFitProportionalScript(double pv, double ph);

		internal void RenderStylesOnly(string streamName)
		{
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
			object obj2 = elementProps.Style[26];
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
				styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
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
					HTML5ParagraphStyleWriter hTML5ParagraphStyleWriter = new HTML5ParagraphStyleWriter(this, rPLTextBox);
					TextRunStyleWriter styleWriter = new TextRunStyleWriter(this);
					for (RPLParagraph nextParagraph = rPLTextBox.GetNextParagraph(); nextParagraph != null; nextParagraph = rPLTextBox.GetNextParagraph())
					{
						hTML5ParagraphStyleWriter.Paragraph = nextParagraph;
						string iD2 = nextParagraph.ElementProps.Definition.ID;
						hTML5ParagraphStyleWriter.ParagraphMode = HTML5ParagraphStyleWriter.Mode.All;
						this.RenderSharedStyle(hTML5ParagraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2);
						hTML5ParagraphStyleWriter.ParagraphMode = HTML5ParagraphStyleWriter.Mode.ListOnly;
						this.RenderSharedStyle(hTML5ParagraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2 + "l");
						hTML5ParagraphStyleWriter.ParagraphMode = HTML5ParagraphStyleWriter.Mode.ParagraphOnly;
						this.RenderSharedStyle(hTML5ParagraphStyleWriter, nextParagraph.ElementProps.Definition.SharedStyle, styleContext, iD2 + "p");
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
									num = HTML5Renderer.GetNewContext(num, rPLTablixCell.ColIndex == 0, rPLTablixCell.ColIndex + rPLTablixCell.ColSpan == rPLTablix.ColumnWidths.Length, rPLTablixCell.RowIndex == 0, rPLTablixCell.RowIndex + rPLTablixCell.RowSpan == rPLTablix.RowHeights.Length);
									int num3 = num;
									RPLTextBox rPLTextBox2 = (RPLTextBox)element2;
									bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
									if (rPLTextBox2 != null && HTML5Renderer.IsWritingModeVertical(sharedStyle2))
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
										object obj = element2.ElementProps.Style[26];
										RPLTextBoxPropsDef rPLTextBoxPropsDef2 = (RPLTextBoxPropsDef)element2.ElementProps.Definition;
										bool flag = obj != null && (RPLFormat.VerticalAlignments)obj != 0 && !rPLTextBoxPropsDef2.CanGrow;
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
			this.WriteStream(HTMLElements.m_dot);
			this.WriteStream(this.m_stylePrefixIdBytes);
			this.WriteStream(id);
			this.WriteStream(HTMLElements.m_openAccol);
		}

		protected virtual RPLReport GetNextPage()
		{
			RPLReport result = default(RPLReport);
			this.m_spbProcessing.GetNextPage(out result);
			return result;
		}

		protected internal void RenderSortImage(RPLTextBoxProps textBoxProps)
		{
			this.WriteStream(HTMLElements.m_openA);
			this.WriteStream(HTMLElements.m_tabIndex);
			this.WriteStream(++this.m_tabIndexNum);
			this.WriteStream(HTMLElements.m_quote);
			RPLFormat.SortOptions sortState = textBoxProps.SortState;
			this.RenderSortAction(textBoxProps, sortState);
			this.WriteStream(HTMLElements.m_img);
			this.WriteStream(HTMLElements.m_alt);
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
			this.WriteStream(HTMLElements.m_quote);
			if (this.m_browserIE)
			{
				this.WriteStream(HTMLElements.m_imgOnError);
			}
			this.WriteStream(HTMLElements.m_zeroBorder);
			this.WriteStream(HTMLElements.m_src);
			this.RenderSortImageText(sortState);
			this.WriteStream(HTMLElements.m_closeTag);
			this.WriteStream(HTMLElements.m_closeA);
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

		protected internal PaddingSharedInfo GetPaddings(RPLElementStyle style, PaddingSharedInfo paddingInfo)
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
					this.WriteStream(HTMLElements.m_openTable);
					this.WriteStream(HTMLElements.m_closeBracket);
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
						this.WriteStream(HTMLElements.m_firstTD);
						styleContext.StyleOnCell = true;
						this.RenderReportItemStyle(rPLBody, rPLItemProps, rPLItemPropsDef, null, styleContext, ref num, rPLItemPropsDef.ID + "c");
						styleContext.StyleOnCell = false;
						this.WriteStream(HTMLElements.m_closeBracket);
					}
					this.m_pageSection = PageSection.Body;
					this.m_isBody = true;
					RPLItemMeasurement rPLItemMeasurement2 = new RPLItemMeasurement();
					rPLItemMeasurement2.Width = this.m_pageContent.MaxSectionWidth;
					rPLItemMeasurement2.Height = this.m_rplReportSection.BodyArea.Height;
					new RectangleRenderer(this).RenderReportItem(rPLBody, rPLItemMeasurement2, styleContext, ref num, false, true);
					if (flag2)
					{
						this.WriteStream(HTMLElements.m_closeTD);
						this.WriteStream(HTMLElements.m_closeTR);
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
					this.WriteStream(HTMLElements.m_closeTable);
				}
				if (this.m_elementExtender.HasSetupRequirements())
				{
					this.WriteStream(this.m_elementExtender.SetupRequirements());
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
						this.WriteStream(HTMLElements.m_pageBreakDelimiter);
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

		protected virtual void RenderPageStart(bool firstPage, bool lastPage, RPLElementStyle pageStyle)
		{
			this.WriteStream(HTMLElements.m_openDiv);
			this.WriteStream(HTMLElements.m_ltrDir);
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
				this.WriteStream(HTMLElements.m_closeBracket);
				this.WriteStream(HTMLElements.m_openDiv);
				this.OpenStyle();
				if (this.FillPageHeight)
				{
					this.WriteStream(HTMLElements.m_styleHeight);
					this.WriteStream(HTMLElements.m_percent);
					this.WriteStream(HTMLElements.m_semiColon);
				}
				this.WriteStream(HTMLElements.m_styleWidth);
				this.WriteStream(HTMLElements.m_percent);
				this.WriteStream(HTMLElements.m_semiColon);
				this.RenderPageStyle(pageStyle);
				this.CloseStyle(true);
			}
			this.WriteStream(HTMLElements.m_closeBracket);
			this.WriteStream(HTMLElements.m_openTable);
			this.WriteStream(HTMLElements.m_closeBracket);
			this.WriteStream(HTMLElements.m_firstTD);
			if (firstPage)
			{
				this.RenderReportItemId("oReportCell");
			}
			if (flag)
			{
				this.WriteFixedHeaderPropertyChangeScript();
			}
			this.WriteStream(HTMLElements.m_closeBracket);
		}

		protected virtual void RenderPageStartDimensionStyles(bool lastPage)
		{
			if (this.m_pageNum != 0 || lastPage)
			{
				this.WriteStream(HTMLElements.m_openStyle);
				this.WriteScrollbars();
				if (this.m_deviceInfo.IsBrowserIE)
				{
					this.WriteStream(HTMLElements.m_styleHeight);
					this.WriteStream(HTMLElements.m_percent);
					this.WriteStream(HTMLElements.m_semiColon);
				}
				this.WriteStream(HTMLElements.m_styleWidth);
				this.WriteStream(HTMLElements.m_percent);
				this.WriteStream(HTMLElements.m_semiColon);
				this.WriteStream("direction:ltr");
				this.WriteStream(HTMLElements.m_quote);
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
							this.WriteStream(HTMLElements.m_closeAccol);
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

		public void WriteClassName(byte[] className, byte[] classNameIfNoPrefix)
		{
			if (this.m_deviceInfo.HtmlPrefixId.Length > 0 || classNameIfNoPrefix == null)
			{
				this.WriteStream(HTMLElements.m_classStyle);
				this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
				this.WriteStream(className);
				this.WriteStream(HTMLElements.m_quote);
			}
			else
			{
				this.WriteStream(classNameIfNoPrefix);
			}
		}

		protected virtual void WriteClassStyle(byte[] styleBytes, bool close)
		{
			this.WriteStream(HTMLElements.m_classStyle);
			this.WriteStream(this.m_stylePrefixIdBytes);
			this.WriteStream(styleBytes);
			if (close)
			{
				this.WriteStream(HTMLElements.m_quote);
			}
		}

		protected void RenderBackgroundStyleProps(IRPLStyle style)
		{
			object obj = style[34];
			if (obj != null)
			{
				this.WriteStream(HTMLElements.m_backgroundColor);
				this.WriteStream(obj);
				this.WriteStream(HTMLElements.m_semiColon);
			}
			obj = style[33];
			if (obj != null)
			{
				this.WriteStream(HTMLElements.m_backgroundImage);
				this.RenderImageUrl(true, (RPLImageData)obj);
				this.WriteStream(HTMLElements.m_closeBrace);
				this.WriteStream(HTMLElements.m_semiColon);
			}
			obj = style[35];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.BackgroundRepeatTypes)obj);
				this.WriteStream(HTMLElements.m_backgroundRepeat);
				this.WriteStream(obj);
				this.WriteStream(HTMLElements.m_semiColon);
			}
		}

		protected virtual void RenderPageEnd()
		{
			if (this.m_deviceInfo.ExpandContent)
			{
				this.WriteStream(HTMLElements.m_lastTD);
				this.WriteStream(HTMLElements.m_closeTable);
			}
			else
			{
				this.WriteStream(HTMLElements.m_closeTD);
				this.WriteStream(HTMLElements.m_openTD);
				this.WriteStream(HTMLElements.m_inlineWidth);
				this.WriteStream(HTMLElements.m_percent);
				this.WriteStream(HTMLElements.m_quote);
				this.WriteStream(HTMLElements.m_inlineHeight);
				this.WriteStream("0");
				this.WriteStream(HTMLElements.m_closeQuote);
				this.WriteStream(HTMLElements.m_lastTD);
				this.WriteStream(HTMLElements.m_firstTD);
				this.WriteStream(HTMLElements.m_inlineWidth);
				if (this.m_deviceInfo.IsBrowserGeckoEngine)
				{
					this.WriteStream(HTMLElements.m_percent);
				}
				else
				{
					this.WriteStream("0");
				}
				this.WriteStream(HTMLElements.m_quote);
				this.WriteStream(HTMLElements.m_inlineHeight);
				this.WriteStream(HTMLElements.m_percent);
				this.WriteStream(HTMLElements.m_closeQuote);
				this.WriteStream(HTMLElements.m_lastTD);
				this.WriteStream(HTMLElements.m_closeTable);
			}
			if (this.m_pageHasStyle)
			{
				this.WriteStream(HTMLElements.m_closeDiv);
			}
			this.WriteStream(HTMLElements.m_closeDiv);
		}

		internal void WriteStream(object theString)
		{
			if (theString != null)
			{
				this.WriteStream(theString.ToString());
			}
		}

		public void WriteStreamCR(string theString)
		{
			this.WriteStream(theString);
		}

		public void WriteStreamCR(byte[] theBytes)
		{
			this.WriteStream(theBytes);
		}

		protected internal void WriteStreamEncoded(string theString)
		{
			this.WriteStream(HttpUtility.HtmlEncode(theString));
		}

		protected void WriteStreamCREncoded(string theString)
		{
			this.WriteStream(HttpUtility.HtmlEncode(theString));
		}

		protected virtual void WriteStreamLineBreak()
		{
		}

		protected internal void WriteRSStreamCR(float size)
		{
			this.WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
			this.WriteStreamCR(HTMLElements.m_mm);
		}

		public void WriteIdToSecondaryStream(Stream secondaryStream, string tagId)
		{
			Stream mainStream = this.m_mainStream;
			this.m_mainStream = secondaryStream;
			this.WriteReportItemId(tagId);
			this.WriteStream(HTMLElements.m_comma);
			this.m_mainStream = mainStream;
		}

		protected byte[] RenderSharedStyle(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLStyleProps sharedStyle, RPLItemMeasurement measurement, string id, StyleContext styleContext, ref int borderContext)
		{
			return this.RenderSharedStyle(reportItem, props, definition, sharedStyle, (RPLStyleProps)null, measurement, id, styleContext, ref borderContext, false);
		}

		protected byte[] RenderSharedStyle(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLStyleProps sharedStyle, RPLStyleProps nonSharedStyle, RPLItemMeasurement measurement, string id, StyleContext styleContext, ref int borderContext, bool renderDirectionStyles = false)
		{
			Stream mainStream = this.m_mainStream;
			this.m_mainStream = this.m_styleStream;
			this.RenderOpenStyle(id);
			byte omitBordersState = styleContext.OmitBordersState;
			styleContext.OmitBordersState = 0;
			this.RenderStyleProps(reportItem, props, definition, measurement, (IRPLStyle)sharedStyle, (IRPLStyle)nonSharedStyle, styleContext, ref borderContext, false, renderDirectionStyles);
			styleContext.OmitBordersState = omitBordersState;
			this.WriteStream(HTMLElements.m_closeAccol);
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
				this.WriteStream(HTMLElements.m_closeAccol);
				this.m_mainStream = mainStream;
				byte[] bytes = this.m_encoding.GetBytes(id);
				this.m_usedStyles.Add(id, bytes);
				return bytes;
			}
			return null;
		}

		protected internal void RenderMeasurementStyle(float height, float width)
		{
			this.RenderMeasurementStyle(height, width, false);
		}

		protected void RenderMeasurementStyle(float height, float width, bool renderMin)
		{
			this.RenderMeasurementHeight(height, renderMin);
			this.RenderMeasurementWidth(width, true);
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

		protected internal virtual void RenderDynamicImageSrc(RPLDynamicImageProps dynamicImageProps)
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

		protected internal void RenderHtmlBorders(IRPLStyle styleProps, ref int borderContext, byte omitBordersState, bool renderPadding, bool isNonShared, IRPLStyle sharedStyleProps)
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
					this.WriteStream(HTMLElements.m_paddingLeft);
					this.WriteStream(obj);
					this.WriteStream(HTMLElements.m_semiColon);
				}
				obj = styleProps[17];
				if (obj != null)
				{
					this.WriteStream(HTMLElements.m_paddingTop);
					this.WriteStream(obj);
					this.WriteStream(HTMLElements.m_semiColon);
				}
				obj = styleProps[16];
				if (obj != null)
				{
					this.WriteStream(HTMLElements.m_paddingRight);
					this.WriteStream(obj);
					this.WriteStream(HTMLElements.m_semiColon);
				}
				obj = styleProps[18];
				if (obj != null)
				{
					this.WriteStream(HTMLElements.m_paddingBottom);
					this.WriteStream(obj);
					this.WriteStream(HTMLElements.m_semiColon);
				}
			}
		}

		internal bool IsLineSlanted(RPLItemMeasurement measurement)
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

		protected void RenderCellItem(PageTableCell currCell, int borderContext, bool layoutExpand, bool treatAsTopLevel)
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
					this.WriteStream(HTMLElements.m_openDiv);
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
					this.WriteStream(HTMLElements.m_closeBracket);
				}
				this.RenderReportItem(element, rPLItemProps, rPLItemPropsDef, rPLItemMeasurement, new StyleContext(), borderContext, flag, treatAsTopLevel);
				if (flag2)
				{
					this.WriteStream(HTMLElements.m_closeDiv);
				}
				rPLItemMeasurement.Element = null;
			}
		}

		protected virtual void RenderBlankImage()
		{
			this.WriteStream(HTMLElements.m_img);
			if (this.m_browserIE)
			{
				this.WriteStream(HTMLElements.m_imgOnError);
			}
			this.WriteStream(HTMLElements.m_src);
			this.RenderInternalImageSrc();
			this.WriteStream(this.m_report.GetImageName("Blank.gif"));
			this.WriteStream(HTMLElements.m_closeTag);
		}

		internal virtual string GetImageUrl(bool useSessionId, RPLImageData image)
		{
			string text = this.CreateImageStream(image);
			string result = null;
			if (text != null)
			{
				result = this.m_report.GetStreamUrl(useSessionId, text);
			}
			return result;
		}

		internal void RenderImageUrl(bool useSessionId, RPLImageData image)
		{
			string imageUrl = this.GetImageUrl(useSessionId, image);
			if (imageUrl != null)
			{
				this.WriteStream(imageUrl);
			}
		}

		internal void WriteOuterConsolidation(System.Drawing.Rectangle consolidationOffsets, RPLFormat.Sizings sizing, string propsUniqueName)
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
			this.WriteStream(HTMLElements.m_styleWidth);
			if (flag)
			{
				this.WriteStream("1");
			}
			else
			{
				this.WriteStream(consolidationOffsets.Width);
			}
			this.WriteStream(HTMLElements.m_px);
			this.WriteStream(HTMLElements.m_semiColon);
			this.WriteStream(HTMLElements.m_styleHeight);
			if (flag)
			{
				this.WriteStream("1");
			}
			else
			{
				this.WriteStream(consolidationOffsets.Height);
			}
			this.WriteStream(HTMLElements.m_px);
			this.WriteStream(HTMLElements.m_semiColon);
			this.WriteStream(HTMLElements.m_overflowHidden);
			this.WriteStream(HTMLElements.m_semiColon);
			if (this.m_deviceInfo.BrowserMode == BrowserMode.Standards)
			{
				this.WriteStream(HTMLElements.m_stylePositionAbsolute);
			}
		}

		protected internal void WriteClippedDiv(System.Drawing.Rectangle clipCoordinates)
		{
			this.OpenStyle();
			this.WriteStream(HTMLElements.m_styleTop);
			if (clipCoordinates.Top > 0)
			{
				this.WriteStream("-");
			}
			this.WriteStream(clipCoordinates.Top);
			this.WriteStream(HTMLElements.m_px);
			this.WriteStream(HTMLElements.m_semiColon);
			this.WriteStream(HTMLElements.m_styleLeft);
			if (clipCoordinates.Left > 0)
			{
				this.WriteStream("-");
			}
			this.WriteStream(clipCoordinates.Left);
			this.WriteStream(HTMLElements.m_px);
			this.WriteStream(HTMLElements.m_semiColon);
			this.WriteStream(HTMLElements.m_stylePositionRelative);
			this.CloseStyle(true);
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

		protected internal int RenderReportItem(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, StyleContext styleContext, int borderContext, bool renderId, bool treatAsTopLevel)
		{
			int result = borderContext;
			if (reportItem == null)
			{
				return result;
			}
			if (measurement != null)
			{
				styleContext.OmitBordersState = measurement.State;
			}
			RPLTextBox rPLTextBox = reportItem as RPLTextBox;
			if (rPLTextBox != null)
			{
				new TextBoxRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref result, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLTablix)
			{
				new TablixRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref result, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLRectangle)
			{
				new RectangleRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref result, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLChart || reportItem is RPLGaugePanel || reportItem is RPLMap)
			{
				new ServerDynamicImageRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref result, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLSubReport)
			{
				new SubReportRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref result, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLImage)
			{
				new ImageRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref result, renderId, treatAsTopLevel);
			}
			else if (reportItem is RPLLine)
			{
				new LineRenderer(this).RenderReportItem(reportItem, measurement, styleContext, ref result, renderId, treatAsTopLevel);
			}
			return result;
		}

		private void WriteFontSizeSmallPoint()
		{
			if (this.m_deviceInfo.IsBrowserGeckoEngine)
			{
				this.WriteStream(HTMLElements.m_smallPoint);
			}
			else
			{
				this.WriteStream(HTMLElements.m_zeroPoint);
			}
		}

		protected void RenderPageHeaderFooter(RPLItemMeasurement hfMeasurement)
		{
			if (hfMeasurement.Height != 0.0)
			{
				RPLHeaderFooter rPLHeaderFooter = (RPLHeaderFooter)hfMeasurement.Element;
				int borderContext = 0;
				StyleContext styleContext = new StyleContext();
				this.WriteStream(HTMLElements.m_openTR);
				this.WriteStream(HTMLElements.m_closeBracket);
				this.WriteStream(HTMLElements.m_openTD);
				styleContext.StyleOnCell = true;
				this.RenderReportItemStyle(rPLHeaderFooter, rPLHeaderFooter.ElementProps, rPLHeaderFooter.ElementProps.Definition, null, styleContext, ref borderContext, rPLHeaderFooter.ElementProps.Definition.ID + "c");
				styleContext.StyleOnCell = false;
				this.WriteStream(HTMLElements.m_closeBracket);
				this.WriteStream(HTMLElements.m_openDiv);
				if (!this.m_deviceInfo.IsBrowserIE)
				{
					styleContext.RenderMeasurements = false;
					styleContext.RenderMinMeasurements = true;
				}
				this.RenderReportItemStyle(rPLHeaderFooter, hfMeasurement, ref borderContext, styleContext);
				this.WriteStreamCR(HTMLElements.m_closeBracket);
				RPLItemMeasurement[] children = rPLHeaderFooter.Children;
				if (children != null && children.Length > 0)
				{
					this.m_renderTableHeight = true;
					this.GenerateHTMLTable(children, 0f, 0f, this.m_pageContent.MaxSectionWidth, hfMeasurement.Height, borderContext, false, SharedListLayoutState.None, null, rPLHeaderFooter.ElementProps.Style, false);
				}
				else
				{
					this.WriteStream(HTMLElements.m_nbsp);
				}
				this.m_renderTableHeight = false;
				this.WriteStreamCR(HTMLElements.m_closeDiv);
				this.WriteStream(HTMLElements.m_closeTD);
				this.WriteStream(HTMLElements.m_closeTR);
			}
		}

		protected void RenderStyleProps(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLItemMeasurement measurement, IRPLStyle sharedStyleProps, IRPLStyle nonSharedStyleProps, StyleContext styleContext, ref int borderContext, bool isNonSharedStyles, bool renderDirectionStyles)
		{
			if (styleContext.ZeroWidth)
			{
				this.WriteStream(HTMLElements.m_displayNone);
			}
			IRPLStyle iRPLStyle = isNonSharedStyles ? nonSharedStyleProps : sharedStyleProps;
			float width;
			if (iRPLStyle != null)
			{
				object obj = null;
				if (styleContext.StyleOnCell)
				{
					bool renderPadding = true;
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
							this.WriteStream(HTMLElements.m_verticalAlign);
							this.WriteStream(obj);
							this.WriteStream(HTMLElements.m_semiColon);
						}
						obj = iRPLStyle[25];
						if (obj != null)
						{
							if ((RPLFormat.TextAlignments)obj != 0)
							{
								obj = EnumStrings.GetValue((RPLFormat.TextAlignments)obj);
								this.WriteStream(HTMLElements.m_textAlign);
								this.WriteStream(obj);
								this.WriteStream(HTMLElements.m_semiColon);
							}
							else
							{
								this.RenderTextAlign(props as RPLTextBoxProps, props.Style);
							}
						}
						if (renderDirectionStyles)
						{
							this.RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
						}
					}
					if (measurement != null && this.m_deviceInfo.OutlookCompat)
					{
						width = measurement.Width;
						if (!(reportItem is RPLTextBox) && !this.IsImageNotFitProportional(reportItem, definition))
						{
							goto IL_013e;
						}
						if (styleContext.InTablix)
						{
							goto IL_013e;
						}
						goto IL_0145;
					}
					goto IL_014d;
				}
				this.RenderTextWrapping(reportItem, renderDirectionStyles);
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
								this.WriteStream(HTMLElements.m_styleWidth);
							}
							else if (styleContext.RenderMinMeasurements)
							{
								this.WriteStream(HTMLElements.m_styleMinWidth);
							}
							this.WriteStream(HTMLElements.m_percent);
							this.WriteStream(HTMLElements.m_semiColon);
							if (rPLTextBoxPropsDef.CanGrow)
							{
								this.WriteStream(HTMLElements.m_overflowXHidden);
							}
							else
							{
								if (styleContext.RenderMeasurements)
								{
									this.WriteStream(HTMLElements.m_styleHeight);
								}
								else if (styleContext.RenderMinMeasurements)
								{
									this.WriteStream(HTMLElements.m_styleMinHeight);
								}
								this.WriteStream(HTMLElements.m_percent);
								this.WriteStream(HTMLElements.m_semiColon);
								this.WriteStream(HTMLElements.m_overflowHidden);
							}
							this.WriteStream(HTMLElements.m_semiColon);
						}
						else if (!(reportItem is RPLTablix))
						{
							this.RenderPercentSizes();
						}
					}
					else if (!renderDirectionStyles)
					{
						if (reportItem is RPLTextBox)
						{
							float width2 = measurement.Width;
							float height = measurement.Height;
							this.RenderMeasurementMinWidth(width2);
							RPLTextBoxPropsDef rPLTextBoxPropsDef2 = (RPLTextBoxPropsDef)definition;
							if (rPLTextBoxPropsDef2.CanGrow && rPLTextBoxPropsDef2.CanShrink)
							{
								this.RenderMeasurementWidth(width2, false);
							}
							else
							{
								this.WriteStream(HTMLElements.m_overflowHidden);
								this.WriteStream(HTMLElements.m_semiColon);
								this.RenderMeasurementWidth(width2, false);
								if (rPLTextBoxPropsDef2.CanShrink)
								{
									this.RenderMeasurementMaxHeight(height);
								}
								else if (!rPLTextBoxPropsDef2.CanGrow)
								{
									this.RenderMeasurementHeight(height);
								}
							}
						}
						else if (!(reportItem is RPLTablix))
						{
							if (!(reportItem is RPLRectangle))
							{
								float height2 = measurement.Height;
								float width3 = measurement.Width;
								this.RenderMeasurementMinWidth(width3);
								if (reportItem is RPLHeaderFooter)
								{
									this.RenderMeasurementMinHeight(height2);
								}
								else
								{
									this.RenderMeasurementHeight(height2);
								}
								this.RenderMeasurementWidth(width3, false);
							}
							if (flag || reportItem is RPLImage)
							{
								this.WriteStream(HTMLElements.m_overflowHidden);
								this.WriteStream(HTMLElements.m_semiColon);
							}
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
						this.WriteStream(HTMLElements.m_fontStyle);
						this.WriteStream(obj);
						this.WriteStream(HTMLElements.m_semiColon);
					}
					obj = iRPLStyle[20];
					if (obj != null)
					{
						this.WriteStream(HTMLElements.m_fontFamily);
						this.WriteStream(HTML5Renderer.HandleSpecialFontCharacters(obj.ToString()));
						this.WriteStream(HTMLElements.m_semiColon);
					}
					obj = iRPLStyle[21];
					if (obj != null)
					{
						this.WriteStream(HTMLElements.m_fontSize);
						if (string.Compare(obj.ToString(), "0pt", StringComparison.OrdinalIgnoreCase) != 0)
						{
							this.WriteStream(obj);
						}
						else
						{
							this.WriteFontSizeSmallPoint();
						}
						this.WriteStream(HTMLElements.m_semiColon);
					}
					else
					{
						RPLTextBoxPropsDef rPLTextBoxPropsDef3 = definition as RPLTextBoxPropsDef;
						RPLStyleProps sharedStyle = reportItem.ElementPropsDef.SharedStyle;
						if ((!isNonSharedStyles || sharedStyle == null || sharedStyle.Count == 0) && rPLTextBoxPropsDef3 != null && !rPLTextBoxPropsDef3.IsSimple)
						{
							this.WriteStream(HTMLElements.m_fontSize);
							this.WriteFontSizeSmallPoint();
							this.WriteStream(HTMLElements.m_semiColon);
						}
					}
					obj = iRPLStyle[22];
					if (obj != null)
					{
						obj = EnumStrings.GetValue((RPLFormat.FontWeights)obj);
						this.WriteStream(HTMLElements.m_fontWeight);
						this.WriteStream(obj);
						this.WriteStream(HTMLElements.m_semiColon);
					}
					obj = iRPLStyle[24];
					if (obj != null)
					{
						obj = EnumStrings.GetValue((RPLFormat.TextDecorations)obj);
						this.WriteStream(HTMLElements.m_textDecoration);
						this.WriteStream(obj);
						this.WriteStream(HTMLElements.m_semiColon);
					}
					obj = iRPLStyle[31];
					if (obj != null)
					{
						obj = EnumStrings.GetValue((RPLFormat.UnicodeBiDiTypes)obj);
						this.WriteStream(HTMLElements.m_unicodeBiDi);
						this.WriteStream(obj);
						this.WriteStream(HTMLElements.m_semiColon);
					}
					obj = iRPLStyle[27];
					if (obj != null)
					{
						this.WriteStream(HTMLElements.m_color);
						this.WriteStream(obj);
						this.WriteStream(HTMLElements.m_semiColon);
					}
					obj = iRPLStyle[28];
					if (obj != null)
					{
						this.WriteStream(HTMLElements.m_lineHeight);
						this.WriteStream(obj);
						this.WriteStream(HTMLElements.m_semiColon);
					}
					if ((HTML5Renderer.IsWritingModeVertical(sharedStyleProps) || HTML5Renderer.IsWritingModeVertical(nonSharedStyleProps)) && reportItem is RPLTextBox && styleContext.InTablix && this.m_deviceInfo.IsBrowserIE && !styleContext.IgnorePadding)
					{
						this.RenderPaddingStyle(iRPLStyle);
					}
					if (renderDirectionStyles)
					{
						this.RenderDirectionStyles(reportItem, props, definition, measurement, sharedStyleProps, nonSharedStyleProps, isNonSharedStyles, styleContext);
					}
					obj = iRPLStyle[26];
					if (obj != null && !styleContext.IgnoreVerticalAlign)
					{
						obj = EnumStrings.GetValue((RPLFormat.VerticalAlignments)obj);
						this.WriteStream(HTMLElements.m_verticalAlign);
						this.WriteStream(obj);
						this.WriteStream(HTMLElements.m_semiColon);
					}
					obj = iRPLStyle[25];
					if (obj != null)
					{
						if ((RPLFormat.TextAlignments)obj != 0)
						{
							this.WriteStream(HTMLElements.m_textAlign);
							this.WriteStream(EnumStrings.GetValue((RPLFormat.TextAlignments)obj));
							this.WriteStream(HTMLElements.m_semiColon);
						}
						else
						{
							this.RenderTextAlign(props as RPLTextBoxProps, props.Style);
						}
					}
				}
			}
			return;
			IL_014d:
			this.RenderTextWrapping(reportItem, renderDirectionStyles);
			return;
			IL_013e:
			this.RenderMeasurementMinWidth(width);
			goto IL_0145;
			IL_0145:
			this.RenderMeasurementWidth(width, false);
			goto IL_014d;
		}

		protected internal bool GenerateHTMLTable(RPLItemMeasurement[] repItemCol, float ownerTop, float ownerLeft, float dxParent, float dyParent, int borderContext, bool expandLayout, SharedListLayoutState layoutState, List<RPLTablixMemberCell> omittedHeaders, IRPLStyle style, bool treatAsTopLevel)
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
							borderContext2 = HTML5Renderer.GetNewContext(borderContext, j + 1, 1, pageTableLayout.NrRows, 1);
						}
						this.RenderCellItem(pageTableLayout.GetCell(j), borderContext2, false, treatAsTopLevel);
					}
					if (borderContext > 0)
					{
						borderContext2 = HTML5Renderer.GetNewContext(borderContext, j + 1, 1, pageTableLayout.NrRows, 1);
					}
					this.RenderCellItem(pageTableLayout.GetCell(j), borderContext2, false, treatAsTopLevel);
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
					this.WriteStream(HTMLElements.m_openTable);
					this.WriteStream(HTMLElements.m_zeroBorder);
					if (flag2)
					{
						num2++;
					}
					if (!this.m_deviceInfo.IsBrowserGeckoEngine)
					{
						this.WriteStream(HTMLElements.m_cols);
						this.WriteStream(num2.ToString(CultureInfo.InvariantCulture));
						this.WriteStream(HTMLElements.m_quote);
					}
					this.RenderReportLanguage();
					this.WriteAriaPresentationRole();
					if (this.m_useInlineStyle)
					{
						this.OpenStyle();
						this.WriteStream(HTMLElements.m_borderCollapse);
						if (expandLayout)
						{
							this.WriteStream(HTMLElements.m_semiColon);
							this.WriteStream(HTMLElements.m_styleHeight);
							this.WriteStream(HTMLElements.m_percent);
						}
					}
					else
					{
						this.ClassLayoutBorder();
						if (expandLayout)
						{
							this.WriteStream(HTMLElements.m_space);
							this.WriteAttrEncoded(this.m_deviceInfo.HtmlPrefixId);
							this.WriteStream(HTMLElements.m_percentHeight);
						}
						this.WriteStream(HTMLElements.m_quote);
					}
					if (this.m_renderTableHeight)
					{
						if (this.m_isStyleOpen)
						{
							this.WriteStream(HTMLElements.m_semiColon);
						}
						else
						{
							this.OpenStyle();
						}
						this.WriteStream(HTMLElements.m_styleHeight);
						this.WriteDStream(dyParent);
						this.WriteStream(HTMLElements.m_mm);
						this.m_renderTableHeight = false;
					}
					if (this.m_deviceInfo.OutlookCompat || this.m_deviceInfo.IsBrowserSafari)
					{
						if (this.m_isStyleOpen)
						{
							this.WriteStream(HTMLElements.m_semiColon);
						}
						else
						{
							this.OpenStyle();
						}
						this.WriteStream(HTMLElements.m_styleWidth);
						float num3 = dxParent;
						if (num3 > 0.0)
						{
							num3 = HTML5Renderer.SubtractBorderStyles(num3, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
							num3 = HTML5Renderer.SubtractBorderStyles(num3, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
							if (num3 < 0.0)
							{
								num3 = 1f;
							}
						}
						this.WriteStream(num3);
						this.WriteStream(HTMLElements.m_mm);
					}
					this.CloseStyle(true);
					this.WriteStream(HTMLElements.m_closeBracket);
					if (pageTableLayout.NrCols > 1)
					{
						flag = pageTableLayout.NeedExtraRow();
						if (flag)
						{
							this.WriteStream(HTMLElements.m_openTR);
							this.WriteStream(HTMLElements.m_zeroHeight);
							this.WriteStream(HTMLElements.m_closeBracket);
							if (flag2)
							{
								this.WriteStream(HTMLElements.m_openTD);
								this.WriteStream(HTMLElements.m_openStyle);
								this.WriteStream(HTMLElements.m_styleWidth);
								this.WriteStream("0");
								this.WriteStream(HTMLElements.m_px);
								this.WriteStream(HTMLElements.m_closeQuote);
								this.WriteStream(HTMLElements.m_closeTD);
							}
							for (num = 0; num < pageTableLayout.NrCols; num++)
							{
								this.WriteStream(HTMLElements.m_openTD);
								this.WriteStream(HTMLElements.m_openStyle);
								this.WriteStream(HTMLElements.m_styleWidth);
								float num4 = pageTableLayout.GetCell(num).DXValue.Value;
								if (num4 > 0.0)
								{
									if (num == 0)
									{
										num4 = HTML5Renderer.SubtractBorderStyles(num4, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
									}
									if (num == pageTableLayout.NrCols - 1)
									{
										num4 = HTML5Renderer.SubtractBorderStyles(num4, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
									}
									if (num4 <= 0.0)
									{
										num4 = (float)((this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE) ? 1.0 : pageTableLayout.GetCell(num).DXValue.Value);
									}
								}
								this.WriteDStream(num4);
								this.WriteStream(HTMLElements.m_mm);
								this.WriteStream(HTMLElements.m_semiColon);
								this.WriteStream(HTMLElements.m_styleMinWidth);
								this.WriteDStream(num4);
								this.WriteStream(HTMLElements.m_mm);
								this.WriteStream(HTMLElements.m_closeQuote);
								this.WriteStream(HTMLElements.m_closeTD);
							}
							this.WriteStream(HTMLElements.m_closeTR);
						}
					}
				}
				this.GenerateTableLayoutContent(pageTableLayout, repItemCol, flag, flag2, renderHeight, borderContext, expandLayout, layoutState, omittedHeaders, style, treatAsTopLevel);
				if (layoutState == SharedListLayoutState.None || layoutState == SharedListLayoutState.End)
				{
					if (expandLayout)
					{
						this.WriteStream(HTMLElements.m_firstTD);
						this.ClassPercentHeight();
						this.WriteStream(HTMLElements.m_cols);
						this.WriteStream(num2.ToString(CultureInfo.InvariantCulture));
						this.WriteStream(HTMLElements.m_closeQuote);
						this.WriteStream(HTMLElements.m_lastTD);
					}
					this.WriteStreamCR(HTMLElements.m_closeTable);
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

		protected void PredefinedStyles()
		{
			HTML5Renderer.PredefinedStyles(this.m_deviceInfo, this, this.m_styleClassPrefix);
		}

		internal static void PredefinedStyles(DeviceInfo m_deviceInfo, IHtmlRenderer writer)
		{
			HTML5Renderer.PredefinedStyles(m_deviceInfo, writer, null);
		}

		internal static void PredefinedStyles(DeviceInfo deviceInfo, IHtmlRenderer writer, byte[] classStylePrefix)
		{
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentSizes);
			writer.WriteStream(HTMLElements.m_styleHeight);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_styleWidth);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentSizesOverflow);
			writer.WriteStream(HTMLElements.m_styleHeight);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_styleWidth);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_overflowHidden);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentHeight);
			writer.WriteStream(HTMLElements.m_styleHeight);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_ignoreBorder);
			writer.WriteStream(HTMLElements.m_borderStyle);
			writer.WriteStream(HTMLElements.m_none);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_ignoreBorderL);
			writer.WriteStream(HTMLElements.m_borderLeftStyle);
			writer.WriteStream(HTMLElements.m_none);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_ignoreBorderR);
			writer.WriteStream(HTMLElements.m_borderRightStyle);
			writer.WriteStream(HTMLElements.m_none);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_ignoreBorderT);
			writer.WriteStream(HTMLElements.m_borderTopStyle);
			writer.WriteStream(HTMLElements.m_none);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_ignoreBorderB);
			writer.WriteStream(HTMLElements.m_borderBottomStyle);
			writer.WriteStream(HTMLElements.m_none);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_layoutBorder);
			writer.WriteStream(HTMLElements.m_borderCollapse);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_layoutFixed);
			writer.WriteStream(HTMLElements.m_borderCollapse);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_tableLayoutFixed);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentWidthOverflow);
			writer.WriteStream(HTMLElements.m_styleWidth);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_overflowXHidden);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_popupAction);
			writer.WriteStream("position:absolute;display:none;background-color:white;border:1px solid black;");
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_styleAction);
			writer.WriteStream("text-decoration:none;color:black;cursor:pointer;");
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_emptyTextBox);
			writer.WriteStream(HTMLElements.m_fontSize);
			writer.WriteStream(deviceInfo.IsBrowserGeckoEngine ? HTMLElements.m_smallPoint : HTMLElements.m_zeroPoint);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_rtlEmbed);
			writer.WriteStream(HTMLElements.m_direction);
			writer.WriteStream("RTL;");
			writer.WriteStream(HTMLElements.m_unicodeBiDi);
			writer.WriteStream(EnumStrings.GetValue(RPLFormat.UnicodeBiDiTypes.Embed));
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_noVerticalMarginClassName);
			writer.WriteStream(HTMLElements.m_marginTop);
			writer.WriteStream(HTMLElements.m_zeroPoint);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_marginBottom);
			writer.WriteStream(HTMLElements.m_zeroPoint);
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentSizeInlineTable);
			writer.WriteStream(HTMLElements.m_styleHeight);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream(HTMLElements.m_styleWidth);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream("display:inline-table");
			writer.WriteStream(HTMLElements.m_closeAccol);
			HTML5Renderer.StartPredefinedStyleClass(deviceInfo, writer, classStylePrefix, HTMLElements.m_percentHeightInlineTable);
			writer.WriteStream(HTMLElements.m_styleHeight);
			writer.WriteStream(HTMLElements.m_percent);
			writer.WriteStream(HTMLElements.m_semiColon);
			writer.WriteStream("display:inline-table");
			writer.WriteStream(HTMLElements.m_closeAccol);
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
				writer.WriteStream(HTMLElements.m_boxSizingBorderBox);
			}
			writer.WriteStream(HTMLElements.m_boxSizingBorderBox);
			writer.WriteStream(HTMLElements.m_closeAccol);
		}

		private static void StartPredefinedStyleClass(DeviceInfo deviceInfo, IHtmlRenderer writer, byte[] classStylePrefix, byte[] className)
		{
			if (classStylePrefix != null)
			{
				writer.WriteStream(classStylePrefix);
			}
			writer.WriteStream(HTMLElements.m_dot);
			writer.WriteStream(deviceInfo.HtmlPrefixId);
			writer.WriteStream(className);
			writer.WriteStream(HTMLElements.m_openAccol);
		}

		private void CheckBodyStyle()
		{
			RPLElementStyle style = this.m_pageContent.PageLayout.Style;
			string text = (string)style[34];
			this.m_pageHasStyle = (text != null || style[33] != null || this.ReportPageHasBorder(style, text));
		}

		private void RenderTextWrapping(RPLElement reportItem, bool renderDirectionStyles)
		{
			if (reportItem is RPLTextBox)
			{
				if (!renderDirectionStyles)
				{
					this.WriteStream(HTMLElements.m_wordWrap);
					this.WriteStream(HTMLElements.m_semiColon);
					this.WriteStream(HTMLElements.m_wordBreak);
					this.WriteStream(HTMLElements.m_semiColon);
				}
				this.WriteStream(HTMLElements.m_whiteSpacePreWrap);
				this.WriteStream(HTMLElements.m_semiColon);
			}
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
				this.WriteStream(HTMLElements.m_borderBottomColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				this.WriteStream(HTMLElements.m_borderBottomStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				this.WriteStream(HTMLElements.m_borderBottomWidth);
			}
		}

		private void BorderLeftAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				this.WriteStream(HTMLElements.m_borderLeftColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				this.WriteStream(HTMLElements.m_borderLeftStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				this.WriteStream(HTMLElements.m_borderLeftWidth);
			}
		}

		private void BorderRightAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				this.WriteStream(HTMLElements.m_borderRightColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				this.WriteStream(HTMLElements.m_borderRightStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				this.WriteStream(HTMLElements.m_borderRightWidth);
			}
		}

		private void BorderTopAttribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				this.WriteStream(HTMLElements.m_borderTopColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				this.WriteStream(HTMLElements.m_borderTopStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				this.WriteStream(HTMLElements.m_borderTopWidth);
			}
		}

		private void BorderAllAtribute(BorderAttribute attribute)
		{
			if (attribute == BorderAttribute.BorderColor)
			{
				this.WriteStream(HTMLElements.m_borderColor);
			}
			if (attribute == BorderAttribute.BorderStyle)
			{
				this.WriteStream(HTMLElements.m_borderStyle);
			}
			if (attribute == BorderAttribute.BorderWidth)
			{
				this.WriteStream(HTMLElements.m_borderWidth);
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
				this.WriteStream(HTMLElements.m_semiColon);
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
					this.WriteStream(HTMLElements.m_border);
					break;
				case Border.Bottom:
					this.WriteStream(HTMLElements.m_borderBottom);
					break;
				case Border.Left:
					this.WriteStream(HTMLElements.m_borderLeft);
					break;
				case Border.Right:
					this.WriteStream(HTMLElements.m_borderRight);
					break;
				default:
					this.WriteStream(HTMLElements.m_borderTop);
					break;
				}
				this.WriteStream(width);
				this.WriteStream(HTMLElements.m_space);
				this.WriteStream(value);
				this.WriteStream(HTMLElements.m_space);
				this.WriteStream(color);
				this.WriteStream(HTMLElements.m_semiColon);
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

		internal string CreateImageStream(RPLImageData image)
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

		private bool HasAction(RPLAction action)
		{
			if (action.BookmarkLink == null && action.DrillthroughId == null && action.DrillthroughUrl == null)
			{
				return action.Hyperlink != null;
			}
			return true;
		}

		internal bool RenderActionHref(RPLAction action, RPLFormat.TextDecorations textDec, string color)
		{
			bool result = false;
			if (action.Hyperlink != null)
			{
				this.WriteStream(HTMLElements.m_hrefString + HttpUtility.HtmlAttributeEncode(action.Hyperlink) + HTMLElements.m_quoteString);
				result = true;
			}
			else
			{
				this.RenderInteractionAction(action, ref result);
			}
			if (textDec != RPLFormat.TextDecorations.Underline)
			{
				this.OpenStyle();
				this.WriteStream(HTMLElements.m_textDecoration);
				this.WriteStream(HTMLElements.m_none);
				this.WriteStream(HTMLElements.m_semiColon);
			}
			if (color != null)
			{
				this.OpenStyle();
				this.WriteStream(HTMLElements.m_color);
				this.WriteStream(color);
			}
			this.CloseStyle(true);
			if (this.m_deviceInfo.LinkTarget != null)
			{
				this.WriteStream(HTMLElements.m_target);
				this.WriteStream(this.m_deviceInfo.LinkTarget);
				this.WriteStream(HTMLElements.m_quote);
			}
			return result;
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

		internal void PercentSizes()
		{
			this.WriteStream(HTMLElements.m_openStyle);
			this.WriteStream(HTMLElements.m_styleHeight);
			this.WriteStream(HTMLElements.m_percent);
			this.WriteStream(HTMLElements.m_semiColon);
			this.WriteStream(HTMLElements.m_styleWidth);
			this.WriteStream(HTMLElements.m_percent);
			this.WriteStream(HTMLElements.m_quote);
		}

		private void ClassLayoutBorder()
		{
			this.WriteClassName(HTMLElements.m_layoutBorder, HTMLElements.m_classLayoutBorder);
		}

		protected internal void ClassPercentHeight()
		{
			this.WriteClassName(HTMLElements.m_percentHeight, HTMLElements.m_classPercentHeight);
		}

		internal void RenderLanguage(string language)
		{
			if (!string.IsNullOrEmpty(language))
			{
				this.WriteStream(HTMLElements.m_language);
				this.WriteAttrEncoded(language);
				this.WriteStream(HTMLElements.m_quote);
			}
		}

		internal void RenderReportLanguage()
		{
			this.RenderLanguage(this.m_contextLanguage);
		}

		private List<string> RenderTableCellBorder(PageTableCell currCell, Hashtable renderedLines)
		{
			RPLLine rPLLine = null;
			List<string> list = new List<string>(4);
			if (this.m_isStyleOpen)
			{
				this.WriteStream(HTMLElements.m_semiColon);
			}
			else
			{
				this.OpenStyle();
			}
			this.WriteStream(HTMLElements.m_zeroBorderWidth);
			rPLLine = currCell.BorderLeft;
			if (rPLLine != null)
			{
				this.WriteStream(HTMLElements.m_semiColon);
				this.WriteStream(HTMLElements.m_borderLeft);
				this.RenderBorderLine(rPLLine);
				this.CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderRight;
			if (rPLLine != null)
			{
				this.WriteStream(HTMLElements.m_semiColon);
				this.WriteStream(HTMLElements.m_borderRight);
				this.RenderBorderLine(rPLLine);
				this.CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderTop;
			if (rPLLine != null)
			{
				this.WriteStream(HTMLElements.m_semiColon);
				this.WriteStream(HTMLElements.m_borderTop);
				this.RenderBorderLine(rPLLine);
				this.CheckForLineID(rPLLine, list, renderedLines);
			}
			rPLLine = currCell.BorderBottom;
			if (rPLLine != null)
			{
				this.WriteStream(HTMLElements.m_semiColon);
				this.WriteStream(HTMLElements.m_borderBottom);
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

		private int GenerateTableLayoutContent(PageTableLayout rgTableGrid, RPLItemMeasurement[] repItemCol, bool bfZeroRowReq, bool bfZeroColReq, bool renderHeight, int borderContext, bool layoutExpand, SharedListLayoutState layoutState, List<RPLTablixMemberCell> omittedHeaders, IRPLStyle style, bool treatAsTopLevel)
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
				this.WriteStream(HTMLElements.m_openTR);
				if (!flag)
				{
					this.WriteStream(HTMLElements.m_valign);
					this.WriteStream(HTMLElements.m_topValue);
					this.WriteStream(HTMLElements.m_quote);
				}
				this.WriteStream(HTMLElements.m_closeBracket);
				flag3 = true;
				for (num = 0; num < nrCols; num++)
				{
					int num7 = num + num4;
					bool flag4 = num == 0;
					if (flag4 && bfZeroColReq)
					{
						this.WriteStream(HTMLElements.m_openTD);
						if (renderHeight || num5 <= 0)
						{
							this.WriteStream(HTMLElements.m_openStyle);
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
								this.WriteStream(HTMLElements.m_styleHeight);
								float num8 = pageTableCell.DYValue.Value;
								if (num8 > 0.0)
								{
									if (i == 0)
									{
										num8 = HTML5Renderer.SubtractBorderStyles(num8, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
									}
									if (i == rgTableGrid.NrRows - num2)
									{
										num8 = HTML5Renderer.SubtractBorderStyles(num8, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
									}
									if (num8 <= 0.0)
									{
										num8 = (float)((this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE) ? 1.0 : pageTableCell.DYValue.Value);
									}
								}
								this.WriteDStream(num8);
								this.WriteStream(HTMLElements.m_mm);
								this.WriteStream(HTMLElements.m_semiColon);
							}
							this.WriteStream(HTMLElements.m_styleWidth);
							this.WriteDStream(0f);
							this.WriteStream(HTMLElements.m_mm);
							this.WriteStream(HTMLElements.m_quote);
						}
						else
						{
							this.WriteStream(HTMLElements.m_openStyle);
							this.WriteStream(HTMLElements.m_styleWidth);
							this.WriteDStream(0f);
							this.WriteStream(HTMLElements.m_mm);
							this.WriteStream(HTMLElements.m_quote);
						}
						this.WriteStream(HTMLElements.m_closeBracket);
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
						this.WriteStream(HTMLElements.m_closeTD);
					}
					pageTableCell2 = rgTableGrid.GetCell(num7);
					if (!pageTableCell2.Eaten)
					{
						if (!pageTableCell2.InUse)
						{
							HTML5Renderer.MergeEmptyCells(rgTableGrid, num, i, num4, flag2, pageTableCell2, nrRows, nrCols, num7);
						}
						this.WriteStream(HTMLElements.m_openTD);
						num2 = pageTableCell2.RowSpan;
						if (num2 != 1)
						{
							this.WriteStream(HTMLElements.m_rowSpan);
							this.WriteStream(num2.ToString(CultureInfo.InvariantCulture));
							this.WriteStream(HTMLElements.m_quote);
						}
						if (!flag2 || bfZeroRowReq || layoutState == SharedListLayoutState.Continue || layoutState == SharedListLayoutState.End)
						{
							num3 = pageTableCell2.ColSpan;
							if (num3 != 1)
							{
								this.WriteStream(HTMLElements.m_colSpan);
								this.WriteStream(num3.ToString(CultureInfo.InvariantCulture));
								this.WriteStream(HTMLElements.m_quote);
							}
						}
						if (flag4 && !bfZeroColReq && (renderHeight || num5 <= 0))
						{
							float num9 = pageTableCell.DYValue.Value;
							if (num9 >= 0.0 && flag3 && (i != nrRows - 1 || !flag || layoutState != 0) && (!this.m_deviceInfo.OutlookCompat || pageTableCell2.NeedsRowHeight))
							{
								this.OpenStyle();
								this.WriteStream(HTMLElements.m_styleHeight);
								if (i == 0)
								{
									num9 = HTML5Renderer.SubtractBorderStyles(num9, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
								}
								if (i == rgTableGrid.NrRows - num2)
								{
									num9 = HTML5Renderer.SubtractBorderStyles(num9, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
								}
								if (num9 <= 0.0)
								{
									num9 = (float)((this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE) ? 1.0 : pageTableCell.DYValue.Value);
								}
								this.WriteDStream(num9);
								this.WriteStream(HTMLElements.m_mm);
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
								this.WriteStream(HTMLElements.m_semiColon);
							}
							else
							{
								this.OpenStyle();
							}
							this.WriteStream(HTMLElements.m_styleWidth);
							if (num11 > 0.0)
							{
								if (num == 0)
								{
									num11 = HTML5Renderer.SubtractBorderStyles(num11, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
								}
								if (num == rgTableGrid.NrCols - num3)
								{
									num11 = HTML5Renderer.SubtractBorderStyles(num11, defaultBorderStyle, specificBorderStyle2, defaultBorderWidth, specificBorderWidth2);
								}
								if (num11 <= 0.0)
								{
									num11 = (float)((this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE) ? 1.0 : num10);
								}
							}
							this.WriteDStream(num11);
							this.WriteStream(HTMLElements.m_mm);
							this.WriteStream(HTMLElements.m_semiColon);
							this.WriteStream(HTMLElements.m_styleMinWidth);
							this.WriteDStream(num11);
							this.WriteStream(HTMLElements.m_mm);
							this.WriteStream(HTMLElements.m_semiColon);
							if (flag3 && !pageTableCell2.InUse && this.m_deviceInfo.OutlookCompat)
							{
								float num12 = pageTableCell2.DYValue.Value;
								if (num12 < 558.79998779296875)
								{
									this.WriteStream(HTMLElements.m_styleHeight);
									if (num12 > 0.0)
									{
										if (i == 0)
										{
											num12 = HTML5Renderer.SubtractBorderStyles(num12, defaultBorderStyle, specificBorderStyle3, defaultBorderWidth, specificBorderWidth3);
										}
										if (i == rgTableGrid.NrRows - num2)
										{
											num12 = HTML5Renderer.SubtractBorderStyles(num12, defaultBorderStyle, specificBorderStyle4, defaultBorderWidth, specificBorderWidth4);
										}
										if (num12 <= 0.0)
										{
											num12 = (float)((this.m_deviceInfo.BrowserMode != BrowserMode.Standards || !this.m_deviceInfo.IsBrowserIE) ? 1.0 : pageTableCell2.DYValue.Value);
										}
									}
									this.WriteDStream(num12);
									this.WriteStream(HTMLElements.m_mm);
									this.WriteStream(HTMLElements.m_semiColon);
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
							this.WriteStream(HTMLElements.m_closeQuote);
						}
						else
						{
							this.WriteStream(HTMLElements.m_closeBracket);
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
							num6 = HTML5Renderer.GetNewContext(borderContext, i + 1, num + 1, num13, num14);
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
							this.RenderCellItem(pageTableCell2, num6, layoutExpand, treatAsTopLevel);
						}
						else if (!this.m_browserIE && pageTableCell2.HasBorder)
						{
							this.RenderBlankImage();
						}
						this.WriteStream(HTMLElements.m_closeTD);
					}
				}
				this.WriteStream(HTMLElements.m_closeTR);
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

		private void RenderWritingMode(RPLFormat.WritingModes writingMode, RPLFormat.Directions direction, StyleContext styleContext, bool isRTL)
		{
			if (HTML5Renderer.IsWritingModeVertical(writingMode))
			{
				if (isRTL)
				{
					this.WriteStream(HTMLElements.m_ms_verticalRTL);
				}
				else
				{
					this.WriteStream(HTMLElements.m_ms_vertical);
				}
				this.WriteStream(HTMLElements.m_ff_vertical);
				this.WriteStream(HTMLElements.m_webkit_vertical);
				if (writingMode == RPLFormat.WritingModes.Rotate270)
				{
					this.WriteStream(HTMLElements.m_rotate180deg);
				}
			}
		}

		protected internal void RenderDirectionStyles(RPLElement reportItem, RPLElementProps props, RPLElementPropsDef definition, RPLItemMeasurement measurement, IRPLStyle sharedStyleProps, IRPLStyle nonSharedStyleProps, bool isNonSharedStyles, StyleContext styleContext)
		{
			IRPLStyle iRPLStyle = isNonSharedStyles ? nonSharedStyleProps : sharedStyleProps;
			bool flag = HTML5Renderer.HasHorizontalPaddingStyles(sharedStyleProps);
			bool flag2 = HTML5Renderer.HasHorizontalPaddingStyles(nonSharedStyleProps);
			object obj = iRPLStyle[29];
			RPLFormat.Directions? nullable = null;
			if (obj != null)
			{
				nullable = (RPLFormat.Directions)obj;
				obj = EnumStrings.GetValue(nullable.Value);
				this.WriteStream(HTMLElements.m_direction);
				this.WriteStream(obj);
				this.WriteStream(HTMLElements.m_semiColon);
			}
			obj = iRPLStyle[30];
			RPLFormat.WritingModes? nullable2 = null;
			if (obj != null)
			{
				nullable2 = (RPLFormat.WritingModes)obj;
				this.WriteStream(HTMLElements.m_layoutFlow);
				if (!HTML5Renderer.IsWritingModeVertical(nullable2.Value))
				{
					this.WriteStream(HTMLElements.m_horizontal);
				}
				this.WriteStream(HTMLElements.m_semiColon);
				if (HTML5Renderer.IsWritingModeVertical(nullable2.Value) && measurement != null && reportItem is RPLTextBox)
				{
					RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)definition;
					float height = measurement.Height;
					float num = measurement.Width;
					if (!rPLTextBoxPropsDef.CanGrow && !rPLTextBoxPropsDef.CanShrink && styleContext.InTablix)
					{
						this.RenderMeasurementWidth(num);
					}
					if (rPLTextBoxPropsDef.CanGrow)
					{
						if (styleContext != null && styleContext.InTablix)
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
					}
					else
					{
						this.WriteStream(HTMLElements.m_overflowHidden);
						this.WriteStream(HTMLElements.m_semiColon);
					}
					if (styleContext != null && !styleContext.InTablix)
					{
						this.WriteStream(HTMLElements.m_styleHeight);
						this.WriteStream(HTMLElements.m_percent);
						this.WriteStream(HTMLElements.m_semiColon);
						this.WriteStream(HTMLElements.m_overflowHidden);
						this.WriteStream(HTMLElements.m_semiColon);
						if (!rPLTextBoxPropsDef.CanGrow && !rPLTextBoxPropsDef.CanShrink)
						{
							this.WriteStream(HTMLElements.m_wordWrap);
							this.WriteStream(HTMLElements.m_semiColon);
							this.WriteStream(HTMLElements.m_styleWidth);
							this.WriteStream(HTMLElements.m_percent);
							this.WriteStream(HTMLElements.m_semiColon);
						}
					}
					else
					{
						this.WriteStream(HTMLElements.m_wordWrap);
						this.WriteStream(HTMLElements.m_semiColon);
						this.WriteStream(HTMLElements.m_styleHeight);
						this.WriteStream(HTMLElements.m_percent);
						this.WriteStream(HTMLElements.m_semiColon);
					}
				}
			}
			if (nullable2.HasValue && nullable.HasValue)
			{
				RPLTextBoxProps rPLTextBoxProps = (RPLTextBoxProps)props;
				this.RenderWritingMode(nullable2.Value, nullable.Value, styleContext, HTML5Renderer.IsDirectionRTL(rPLTextBoxProps.Style));
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
					RPLTextBoxProps rPLTextBoxProps2 = (RPLTextBoxProps)props;
					this.RenderWritingMode(nullable2.Value, nullable.Value, styleContext, HTML5Renderer.IsDirectionRTL(rPLTextBoxProps2.Style));
				}
			}
		}

		internal void RenderReportItemStyle(RPLElement reportItem, RPLItemMeasurement measurement, ref int borderContext)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			this.RenderReportItemStyle(reportItem, elementProps, definition, measurement, new StyleContext(), ref borderContext, definition.ID);
		}

		public void RenderReportItemStyle(RPLElement reportItem, RPLItemMeasurement measurement, ref int borderContext, StyleContext styleContext)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			this.RenderReportItemStyle(reportItem, elementProps, definition, measurement, styleContext, ref borderContext, definition.ID);
		}

		internal void RenderReportItemStyle(RPLElement reportItem, RPLElementProps elementProps, RPLElementPropsDef definition, RPLStyleProps nonSharedStyle, RPLStyleProps sharedStyle, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, string styleID, bool renderDirectionStyles = false)
		{
			if (this.m_useInlineStyle)
			{
				this.OpenStyle();
				RPLElementStyle sharedStyleProps = new RPLElementStyle(nonSharedStyle, sharedStyle);
				this.RenderStyleProps(reportItem, elementProps, definition, measurement, (IRPLStyle)sharedStyleProps, (IRPLStyle)null, styleContext, ref borderContext, false, renderDirectionStyles);
				if (styleContext.EmptyTextBox)
				{
					this.WriteStream(HTMLElements.m_fontSize);
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
					this.RenderStyleProps(reportItem, elementProps, definition, measurement, sharedStyle, nonSharedStyle, styleContext, ref num, true, renderDirectionStyles);
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
							array = this.RenderSharedStyle(reportItem, elementProps, definition, sharedStyle, nonSharedStyle, measurement, styleID, styleContext, ref num2, renderDirectionStyles);
						}
						else
						{
							array = this.m_encoding.GetBytes(styleID);
							this.m_usedStyles.Add(styleID, array);
						}
					}
					this.CloseStyle(true);
					this.WriteClassStyle(array, false);
					if (styleContext.InTablix && reportItem is RPLTextBox)
					{
						if (!((RPLTextBoxPropsDef)definition).CanGrow)
						{
							this.WriteStream(HTMLElements.m_classCannotGrowTextBoxInTablix);
						}
						else
						{
							this.WriteStream(HTMLElements.m_classCanGrowTextBoxInTablix);
						}
						if (((RPLTextBoxPropsDef)definition).CanShrink)
						{
							this.WriteStream(HTMLElements.m_classCanShrinkTextBoxInTablix);
						}
						else
						{
							this.WriteStream(HTMLElements.m_classCannotShrinkTextBoxInTablix);
						}
					}
					byte omitBordersState = styleContext.OmitBordersState;
					if (borderContext != 0 || omitBordersState != 0)
					{
						if (borderContext == 15)
						{
							this.WriteStream(HTMLElements.m_space);
							this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
							this.WriteStream(HTMLElements.m_ignoreBorder);
						}
						else
						{
							if ((borderContext & 4) != 0 || (omitBordersState & 1) != 0)
							{
								this.WriteStream(HTMLElements.m_space);
								this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
								this.WriteStream(HTMLElements.m_ignoreBorderT);
							}
							if ((borderContext & 1) != 0 || (omitBordersState & 4) != 0)
							{
								this.WriteStream(HTMLElements.m_space);
								this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
								this.WriteStream(HTMLElements.m_ignoreBorderL);
							}
							if ((borderContext & 8) != 0 || (omitBordersState & 2) != 0)
							{
								this.WriteStream(HTMLElements.m_space);
								this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
								this.WriteStream(HTMLElements.m_ignoreBorderB);
							}
							if ((borderContext & 2) != 0 || (omitBordersState & 8) != 0)
							{
								this.WriteStream(HTMLElements.m_space);
								this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
								this.WriteStream(HTMLElements.m_ignoreBorderR);
							}
						}
					}
					if (styleContext.EmptyTextBox)
					{
						this.WriteStream(HTMLElements.m_space);
						this.WriteStream(this.m_deviceInfo.HtmlPrefixId);
						this.WriteStream(HTMLElements.m_emptyTextBox);
					}
					this.WriteStream(HTMLElements.m_quote);
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

		internal void RenderReportItemStyle(RPLElement reportItem, RPLElementProps elementProps, RPLElementPropsDef definition, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, string styleID)
		{
			this.RenderReportItemStyle(reportItem, elementProps, definition, elementProps.NonSharedStyle, definition.SharedStyle, measurement, styleContext, ref borderContext, styleID, false);
		}

		private void RenderPercentSizes()
		{
			this.WriteStream(HTMLElements.m_styleHeight);
			this.WriteStream(HTMLElements.m_percent);
			this.WriteStream(HTMLElements.m_semiColon);
			this.WriteStream(HTMLElements.m_styleWidth);
			this.WriteStream(HTMLElements.m_percent);
			this.WriteStream(HTMLElements.m_semiColon);
		}

		private void RenderTextAlign(RPLTextBoxProps props, RPLElementStyle style)
		{
			if (props != null)
			{
				this.WriteStream(HTMLElements.m_textAlign);
				bool flag = HTML5Renderer.GetTextAlignForType(props);
				if (HTML5Renderer.IsDirectionRTL(style))
				{
					flag = ((byte)((!flag) ? 1 : 0) != 0);
				}
				if (flag)
				{
					this.WriteStream(HTMLElements.m_rightValue);
				}
				else
				{
					this.WriteStream(HTMLElements.m_leftValue);
				}
				this.WriteStream(HTMLElements.m_semiColon);
			}
		}

		internal static bool GetTextAlignForType(RPLTextBoxProps textBoxProps)
		{
			TypeCode typeCode = textBoxProps.TypeCode;
			return HTML5Renderer.GetTextAlignForType(typeCode);
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

		protected internal static float GetInnerContainerWidth(RPLMeasurement measurement, IRPLStyle containerStyle)
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

		protected internal float GetInnerContainerWidthSubtractBorders(RPLItemMeasurement measurement, IRPLStyle containerStyle)
		{
			if (measurement == null)
			{
				return -1f;
			}
			float innerContainerWidth = HTML5Renderer.GetInnerContainerWidth(measurement, containerStyle);
			object defaultBorderStyle = containerStyle[5];
			object defaultBorderWidth = containerStyle[10];
			object specificBorderWidth = containerStyle[11];
			object specificBorderStyle = containerStyle[6];
			innerContainerWidth = HTML5Renderer.SubtractBorderStyles(innerContainerWidth, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			specificBorderWidth = containerStyle[12];
			specificBorderStyle = containerStyle[7];
			innerContainerWidth = HTML5Renderer.SubtractBorderStyles(innerContainerWidth, defaultBorderStyle, specificBorderStyle, defaultBorderWidth, specificBorderWidth);
			if (innerContainerWidth <= 0.0)
			{
				innerContainerWidth = 1f;
			}
			return innerContainerWidth;
		}

		protected internal void WriteStyles(string id, RPLStyleProps nonShared, RPLStyleProps shared, ElementStyleWriter styleWriter)
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
							this.WriteStream(HTMLElements.m_closeAccol);
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

		public void RenderImageMapAreas(RPLActionInfoWithImageMap[] actionImageMaps, double width, double height, string uniqueName, int xOffset, int yOffset)
		{

			double imageWidth = width * 96.0 * 0.03937007874;
			double imageHeight = height * 96.0 * 0.03937007874;
			this.WriteStream(HTMLElements.m_openMap);
			this.WriteAttrEncoded(HTMLElements.m_name, this.m_deviceInfo.HtmlPrefixId + HTMLElements.m_mapPrefixString + uniqueName);
			this.WriteStreamCR(HTMLElements.m_closeBracket);
			foreach (RPLActionInfoWithImageMap rPLActionInfoWithImageMap in actionImageMaps)
			{
				if (rPLActionInfoWithImageMap != null)
				{
					this.RenderImageMapArea(rPLActionInfoWithImageMap, imageWidth, imageHeight, uniqueName, xOffset, yOffset);
				}
			}
			this.WriteStream(HTMLElements.m_closeMap);
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
						this.WriteStream(HTMLElements.m_mapArea);
						if (rPLAction != null)
						{
							this.RenderTabIndex();
						}
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
							this.WriteStream(HTMLElements.m_nohref);
						}
						this.WriteStream(HTMLElements.m_mapShape);
						switch (rPLImageMap.Shape)
						{
						case RPLFormat.ShapeType.Circle:
							this.WriteStream(HTMLElements.m_circleShape);
							break;
						case RPLFormat.ShapeType.Polygon:
							this.WriteStream(HTMLElements.m_polyShape);
							break;
						default:
							this.WriteStream(HTMLElements.m_rectShape);
							break;
						}
						this.WriteStream(HTMLElements.m_quote);
						this.WriteStream(HTMLElements.m_mapCoords);
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
									this.WriteStream(HTMLElements.m_comma);
								}
								num = (long)(coordinates[j] / 100.0 * imageWidth) + xOffset;
								this.WriteStream(num);
								this.WriteStream(HTMLElements.m_comma);
								num = (long)(coordinates[j + 1] / 100.0 * imageHeight) + yOffset;
								this.WriteStream(num);
								flag = false;
							}
							if (j < coordinates.Length)
							{
								this.WriteStream(HTMLElements.m_comma);
								num = (long)(coordinates[j] / 100.0 * imageWidth);
								this.WriteStream(num);
							}
						}
						this.WriteStream(HTMLElements.m_quote);
						this.WriteStreamCR(HTMLElements.m_closeBracket);
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

		private void RenderBorderLine(RPLElement reportItem)
		{
			object obj = null;
			IRPLStyle style = reportItem.ElementProps.Style;
			obj = style[10];
			if (obj != null)
			{
				this.WriteStream(obj.ToString());
				this.WriteStream(HTMLElements.m_space);
			}
			obj = style[5];
			if (obj != null)
			{
				obj = EnumStrings.GetValue((RPLFormat.BorderStyles)obj);
				this.WriteStream(obj);
				this.WriteStream(HTMLElements.m_space);
			}
			obj = style[0];
			if (obj != null)
			{
				this.WriteStream((string)obj);
			}
		}

		protected void CreateImgConImageIdsStream()
		{
			string streamName = HTML5Renderer.GetStreamName(this.m_rplReport.ReportName, this.m_pageNum, "_ici");
			Stream stream = this.CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", true, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateOnly);
			this.m_imgConImageIdsStream = new BufferedStream(stream);
		}

		internal void CreateImgFitDivImageIdsStream()
		{
			string streamName = HTML5Renderer.GetStreamName(this.m_rplReport.ReportName, this.m_pageNum, "_ifd");
			Stream stream = this.CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", true, AspNetCore.ReportingServices.Interfaces.StreamOper.CreateOnly);
			this.m_imgFitDivIdsStream = new BufferedStream(stream);
			this.m_emitImageConsolidationScaling = true;
		}

		[SecurityTreatAsSafe]
		[SecurityCritical]
		protected internal Stream CreateStream(string name, string extension, Encoding encoding, string mimeType, bool willSeek, AspNetCore.ReportingServices.Interfaces.StreamOper operation)
		{
			return this.m_createAndRegisterStreamCallback(name, extension, encoding, mimeType, willSeek, operation);
		}

		protected void RenderSecondaryStreamIdsSpanTag(Stream secondaryStream, string tagId)
		{
			if (secondaryStream != null && secondaryStream.CanSeek)
			{
				this.WriteStream(HTMLElements.m_openSpan);
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
				this.WriteStream(HTMLElements.m_closeBracket);
				this.WriteStreamCR(HTMLElements.m_closeSpan);
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
