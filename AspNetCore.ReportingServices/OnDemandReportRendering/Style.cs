using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class Style : StyleBase
	{
		internal sealed class StyleDefaults
		{
			private Hashtable m_nameMap;

			private string[] m_keyCollection;

			private object[] m_valueCollection;

			internal object this[int index]
			{
				get
				{
					return this.m_valueCollection[index];
				}
			}

			internal object this[string styleName]
			{
				get
				{
					return this.m_valueCollection[(int)this.m_nameMap[styleName]];
				}
			}

			internal StyleDefaults(bool isLine, string defaultFontFamily)
			{
				this.m_nameMap = new Hashtable(51);
				this.m_keyCollection = new string[51];
				this.m_valueCollection = new object[51];
				int num = 0;
				this.m_nameMap["BorderColor"] = num;
				this.m_keyCollection[num] = "BorderColor";
				this.m_valueCollection[num++] = new ReportColor("Black", System.Drawing.Color.Empty, false);
				this.m_nameMap["BorderColorTop"] = num;
				this.m_keyCollection[num] = "BorderColorTop";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BorderColorLeft"] = num;
				this.m_keyCollection[num] = "BorderColorLeft";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BorderColorRight"] = num;
				this.m_keyCollection[num] = "BorderColorRight";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BorderColorBottom"] = num;
				this.m_keyCollection[num] = "BorderColorBottom";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BorderStyle"] = num;
				this.m_keyCollection[num] = "BorderStyle";
				if (!isLine)
				{
					this.m_valueCollection[num++] = "None";
				}
				else
				{
					this.m_valueCollection[num++] = "Solid";
				}
				this.m_nameMap["BorderStyleTop"] = num;
				this.m_keyCollection[num] = "BorderStyleTop";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BorderStyleLeft"] = num;
				this.m_keyCollection[num] = "BorderStyleLeft";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BorderStyleRight"] = num;
				this.m_keyCollection[num] = "BorderStyleRight";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BorderStyleBottom"] = num;
				this.m_keyCollection[num] = "BorderStyleBottom";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BorderWidth"] = num;
				this.m_keyCollection[num] = "BorderWidth";
				this.m_valueCollection[num++] = new ReportSize("1pt");
				this.m_nameMap["BorderWidthTop"] = num;
				this.m_keyCollection[num] = "BorderWidthTop";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BorderWidthLeft"] = num;
				this.m_keyCollection[num] = "BorderWidthLeft";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BorderWidthRight"] = num;
				this.m_keyCollection[num] = "BorderWidthRight";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BorderWidthBottom"] = num;
				this.m_keyCollection[num] = "BorderWidthBottom";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BackgroundColor"] = num;
				this.m_keyCollection[num] = "BackgroundColor";
				this.m_valueCollection[num++] = new ReportColor("Transparent", System.Drawing.Color.Empty, true);
				this.m_nameMap["BackgroundGradientType"] = num;
				this.m_keyCollection[num] = "BackgroundGradientType";
				this.m_valueCollection[num++] = BackgroundGradients.None;
				this.m_nameMap["BackgroundGradientEndColor"] = num;
				this.m_keyCollection[num] = "BackgroundGradientEndColor";
				this.m_valueCollection[num++] = new ReportColor("Transparent", System.Drawing.Color.Empty, true);
				this.m_nameMap["BackgroundImage"] = num;
				this.m_keyCollection[num] = "BackgroundImage";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BackgroundRepeat"] = num;
				this.m_keyCollection[num] = "BackgroundRepeat";
				this.m_valueCollection[num++] = "Repeat";
				this.m_nameMap["FontStyle"] = num;
				this.m_keyCollection[num] = "FontStyle";
				this.m_valueCollection[num++] = "Normal";
				this.m_nameMap["FontFamily"] = num;
				this.m_keyCollection[num] = "FontFamily";
				this.m_valueCollection[num++] = (defaultFontFamily ?? "Arial");
				this.m_nameMap["FontSize"] = num;
				this.m_keyCollection[num] = "FontSize";
				this.m_valueCollection[num++] = new ReportSize("10pt");
				this.m_nameMap["FontWeight"] = num;
				this.m_keyCollection[num] = "FontWeight";
				this.m_valueCollection[num++] = "Normal";
				this.m_nameMap["Format"] = num;
				this.m_keyCollection[num] = "Format";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["TextDecoration"] = num;
				this.m_keyCollection[num] = "TextDecoration";
				this.m_valueCollection[num++] = "None";
				this.m_nameMap["TextAlign"] = num;
				this.m_keyCollection[num] = "TextAlign";
				this.m_valueCollection[num++] = "General";
				this.m_nameMap["VerticalAlign"] = num;
				this.m_keyCollection[num] = "VerticalAlign";
				this.m_valueCollection[num++] = "Top";
				this.m_nameMap["Color"] = num;
				this.m_keyCollection[num] = "Color";
				this.m_valueCollection[num++] = new ReportColor("Black", System.Drawing.Color.Empty, false);
				this.m_nameMap["PaddingLeft"] = num;
				this.m_keyCollection[num] = "PaddingLeft";
				this.m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				this.m_nameMap["PaddingRight"] = num;
				this.m_keyCollection[num] = "PaddingRight";
				this.m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				this.m_nameMap["PaddingTop"] = num;
				this.m_keyCollection[num] = "PaddingTop";
				this.m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				this.m_nameMap["PaddingBottom"] = num;
				this.m_keyCollection[num] = "PaddingBottom";
				this.m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				this.m_nameMap["LineHeight"] = num;
				this.m_keyCollection[num] = "LineHeight";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["Direction"] = num;
				this.m_keyCollection[num] = "Direction";
				this.m_valueCollection[num++] = "LTR";
				this.m_nameMap["WritingMode"] = num;
				this.m_keyCollection[num] = "WritingMode";
				this.m_valueCollection[num++] = "lr-tb";
				this.m_nameMap["Language"] = num;
				this.m_keyCollection[num] = "Language";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["UnicodeBiDi"] = num;
				this.m_keyCollection[num] = "UnicodeBiDi";
				this.m_valueCollection[num++] = "Normal";
				this.m_nameMap["Calendar"] = num;
				this.m_keyCollection[num] = "Calendar";
				this.m_valueCollection[num++] = "Gregorian";
				this.m_nameMap["NumeralLanguage"] = num;
				this.m_keyCollection[num] = "NumeralLanguage";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["NumeralVariant"] = num;
				this.m_keyCollection[num] = "NumeralVariant";
				this.m_valueCollection[num++] = 1;
				this.m_nameMap["TextEffect"] = num;
				this.m_keyCollection[num] = "TextEffect";
				this.m_valueCollection[num++] = "None";
				this.m_nameMap["BackgroundHatchType"] = num;
				this.m_keyCollection[num] = "BackgroundHatchType";
				this.m_valueCollection[num++] = "None";
				this.m_nameMap["ShadowColor"] = num;
				this.m_keyCollection[num] = "ShadowColor";
				this.m_valueCollection[num++] = new ReportColor("Transparent", System.Drawing.Color.Empty, true);
				this.m_nameMap["ShadowOffset"] = num;
				this.m_keyCollection[num] = "ShadowOffset";
				this.m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				this.m_nameMap["Position"] = num;
				this.m_keyCollection[num] = "Position";
				this.m_valueCollection[num++] = "Center";
				this.m_nameMap["TransparentColor"] = num;
				this.m_keyCollection[num] = "TransparentColor";
				this.m_valueCollection[num++] = new ReportColor("Transparent", System.Drawing.Color.Empty, true);
				this.m_nameMap["BackgroundImageSource"] = num;
				this.m_keyCollection[num] = "BackgroundImageSource";
				this.m_valueCollection[num++] = AspNetCore.ReportingServices.ReportRendering.Image.SourceType.External;
				this.m_nameMap["BackgroundImageValue"] = num;
				this.m_keyCollection[num] = "BackgroundImageValue";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["BackgroundImageMIMEType"] = num;
				this.m_keyCollection[num] = "BackgroundImageMIMEType";
				this.m_valueCollection[num++] = null;
				this.m_nameMap["CurrencyLanguage"] = num;
				this.m_keyCollection[num] = "CurrencyLanguage";
				this.m_valueCollection[num++] = null;
				Global.Tracer.Assert(51 == num, "(Style.StyleAttributeCount == index)");
			}

			internal string GetName(int index)
			{
				return this.m_keyCollection[index];
			}
		}

		private bool m_isOldSnapshot;

		private IStyleContainer m_iStyleContainer;

		private AspNetCore.ReportingServices.ReportRendering.ReportItem m_renderReportItem;

		private AspNetCore.ReportingServices.ReportRendering.Style m_cachedRenderStyle;

		private bool m_isLineBorderStyle;

		private StyleDefaults m_styleDefaults;

		private BackgroundImage m_backgroundImage;

		private Border m_border;

		private Border m_topBorder;

		private Border m_rightBorder;

		private Border m_bottomBorder;

		private Border m_leftBorder;

		private StyleDefaults m_normalStyleDefaults;

		private StyleDefaults m_lineStyleDefaults;

		private IReportScope m_reportScope;

		private ReportElement m_reportElement;

		private AspNetCore.ReportingServices.ReportProcessing.Style m_styleDef;

		private bool m_isDynamicImageStyle;

		private object[] m_styleValues;

		private ReportProperty[] m_cachedReportProperties = new ReportProperty[51];

		private bool m_disallowBorderTransparencyOnDynamicImage;

		internal static FontStyles DefaultEnumFontStyle = FontStyles.Normal;

		internal static FontWeights DefaultEnumFontWeight = FontWeights.Normal;

		internal static TextDecorations DefaultEnumTextDecoration = TextDecorations.None;

		internal static TextAlignments DefaultEnumTextAlignment = TextAlignments.General;

		internal static VerticalAlignments DefaultEnumVerticalAlignment = VerticalAlignments.Top;

		internal static Directions DefaultEnumDirection = Directions.LTR;

		internal static WritingModes DefaultEnumWritingMode = WritingModes.Horizontal;

		internal static UnicodeBiDiTypes DefaultEnumUnicodeBiDiType = UnicodeBiDiTypes.Normal;

		internal static Calendars DefaultEnumCalendar = Calendars.Default;

		internal static BackgroundGradients DefaultEnumBackgroundGradient = BackgroundGradients.None;

		internal static BackgroundRepeatTypes DefaultEnumBackgroundRepeatType = BackgroundRepeatTypes.Repeat;

		internal IReportScope ReportScope
		{
			get
			{
				return this.m_reportScope;
			}
		}

		internal ReportElement ReportElement
		{
			get
			{
				return this.m_reportElement;
			}
		}

		public override ReportProperty this[StyleAttributeNames style]
		{
			get
			{
				return this.GetReportProperty(style);
			}
		}

		public override List<StyleAttributeNames> SharedStyleAttributes
		{
			get
			{
				if (base.m_sharedStyles == null)
				{
					this.PopulateCollections();
				}
				return base.m_sharedStyles;
			}
		}

		public override List<StyleAttributeNames> NonSharedStyleAttributes
		{
			get
			{
				if (base.m_nonSharedStyles == null)
				{
					this.PopulateCollections();
				}
				return base.m_nonSharedStyles;
			}
		}

		public override BackgroundImage BackgroundImage
		{
			get
			{
				if (this.m_backgroundImage == null)
				{
					this.m_backgroundImage = (this.GetReportProperty(StyleAttributeNames.BackgroundImage) as BackgroundImage);
				}
				return this.m_backgroundImage;
			}
		}

		public override Border Border
		{
			get
			{
				if (this.m_border == null)
				{
					this.m_border = new Border(this, Border.Position.Default, this.m_isLineBorderStyle);
				}
				return this.m_border;
			}
		}

		public override Border TopBorder
		{
			get
			{
				if (this.m_topBorder == null && this.HasBorderProperties(Border.Position.Top))
				{
					this.m_topBorder = new Border(this, Border.Position.Top, false);
				}
				return this.m_topBorder;
			}
		}

		public override Border RightBorder
		{
			get
			{
				if (this.m_rightBorder == null && this.HasBorderProperties(Border.Position.Right))
				{
					this.m_rightBorder = new Border(this, Border.Position.Right, false);
				}
				return this.m_rightBorder;
			}
		}

		public override Border BottomBorder
		{
			get
			{
				if (this.m_bottomBorder == null && this.HasBorderProperties(Border.Position.Bottom))
				{
					this.m_bottomBorder = new Border(this, Border.Position.Bottom, false);
				}
				return this.m_bottomBorder;
			}
		}

		public override Border LeftBorder
		{
			get
			{
				if (this.m_leftBorder == null && this.HasBorderProperties(Border.Position.Left))
				{
					this.m_leftBorder = new Border(this, Border.Position.Left, false);
				}
				return this.m_leftBorder;
			}
		}

		public override ReportColorProperty BackgroundGradientEndColor
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.BackgroundGradientEndColor) as ReportColorProperty;
			}
		}

		public override ReportColorProperty BackgroundColor
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.BackgroundColor) as ReportColorProperty;
			}
		}

		public override ReportColorProperty Color
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.Color) as ReportColorProperty;
			}
		}

		public override ReportEnumProperty<FontStyles> FontStyle
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.FontStyle) as ReportEnumProperty<FontStyles>;
			}
		}

		public override ReportStringProperty FontFamily
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.FontFamily) as ReportStringProperty;
			}
		}

		public override ReportEnumProperty<FontWeights> FontWeight
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.FontWeight) as ReportEnumProperty<FontWeights>;
			}
		}

		public override ReportStringProperty Format
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.Format) as ReportStringProperty;
			}
		}

		public override ReportEnumProperty<TextDecorations> TextDecoration
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.TextDecoration) as ReportEnumProperty<TextDecorations>;
			}
		}

		public override ReportEnumProperty<TextAlignments> TextAlign
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.TextAlign) as ReportEnumProperty<TextAlignments>;
			}
		}

		public override ReportEnumProperty<VerticalAlignments> VerticalAlign
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.VerticalAlign) as ReportEnumProperty<VerticalAlignments>;
			}
		}

		public override ReportEnumProperty<Directions> Direction
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.Direction) as ReportEnumProperty<Directions>;
			}
		}

		public override ReportEnumProperty<WritingModes> WritingMode
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.WritingMode) as ReportEnumProperty<WritingModes>;
			}
		}

		public override ReportStringProperty Language
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.Language) as ReportStringProperty;
			}
		}

		public override ReportEnumProperty<UnicodeBiDiTypes> UnicodeBiDi
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.UnicodeBiDi) as ReportEnumProperty<UnicodeBiDiTypes>;
			}
		}

		public override ReportEnumProperty<Calendars> Calendar
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.Calendar) as ReportEnumProperty<Calendars>;
			}
		}

		public override ReportStringProperty CurrencyLanguage
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.CurrencyLanguage) as ReportStringProperty;
			}
		}

		public override ReportStringProperty NumeralLanguage
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.NumeralLanguage) as ReportStringProperty;
			}
		}

		public override ReportEnumProperty<BackgroundGradients> BackgroundGradientType
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.BackgroundGradientType) as ReportEnumProperty<BackgroundGradients>;
			}
		}

		public override ReportSizeProperty FontSize
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.FontSize) as ReportSizeProperty;
			}
		}

		public override ReportSizeProperty PaddingLeft
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.PaddingLeft) as ReportSizeProperty;
			}
		}

		public override ReportSizeProperty PaddingRight
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.PaddingRight) as ReportSizeProperty;
			}
		}

		public override ReportSizeProperty PaddingTop
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.PaddingTop) as ReportSizeProperty;
			}
		}

		public override ReportSizeProperty PaddingBottom
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.PaddingBottom) as ReportSizeProperty;
			}
		}

		public override ReportSizeProperty LineHeight
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.LineHeight) as ReportSizeProperty;
			}
		}

		public override ReportIntProperty NumeralVariant
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.NumeralVariant) as ReportIntProperty;
			}
		}

		public override ReportEnumProperty<TextEffects> TextEffect
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.TextEffect) as ReportEnumProperty<TextEffects>;
			}
		}

		public override ReportEnumProperty<BackgroundHatchTypes> BackgroundHatchType
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.BackgroundHatchType) as ReportEnumProperty<BackgroundHatchTypes>;
			}
		}

		public override ReportColorProperty ShadowColor
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.ShadowColor) as ReportColorProperty;
			}
		}

		public override ReportSizeProperty ShadowOffset
		{
			get
			{
				return this.GetReportProperty(StyleAttributeNames.ShadowOffset) as ReportSizeProperty;
			}
		}

		internal bool IsOldSnapshot
		{
			get
			{
				return this.m_isOldSnapshot;
			}
		}

		internal bool IsDynamicImageStyle
		{
			get
			{
				return this.m_isDynamicImageStyle;
			}
		}

		internal AspNetCore.ReportingServices.ReportRendering.Style CachedRenderStyle
		{
			get
			{
				return this.m_cachedRenderStyle;
			}
		}

		internal IStyleContainer StyleContainer
		{
			get
			{
				return this.m_iStyleContainer;
			}
		}

		internal StyleDefaults NormalStyleDefaults
		{
			get
			{
				return this.m_normalStyleDefaults;
			}
		}

		internal StyleDefaults LineStyleDefaults
		{
			get
			{
				return this.m_lineStyleDefaults;
			}
		}

		internal Style(ReportElement reportElement, IReportScope reportScope, IStyleContainer styleContainer, RenderingContext renderingContext)
			: base(renderingContext)
		{
			this.m_reportElement = reportElement;
			this.m_lineStyleDefaults = new StyleDefaults(true, Style.GetDefaultFontFamily(renderingContext));
			this.m_normalStyleDefaults = new StyleDefaults(false, Style.GetDefaultFontFamily(renderingContext));
			this.m_reportScope = reportScope;
			this.m_iStyleContainer = styleContainer;
			this.m_isOldSnapshot = false;
			switch (styleContainer.ObjectType)
			{
			case ObjectType.Line:
				this.m_isLineBorderStyle = true;
				this.m_styleDefaults = this.LineStyleDefaults;
				break;
			case ObjectType.Chart:
			{
				Chart chart = reportElement as Chart;
				this.m_disallowBorderTransparencyOnDynamicImage = (chart != null);
				this.m_isDynamicImageStyle = true;
				this.m_styleDefaults = this.NormalStyleDefaults;
				break;
			}
			case ObjectType.GaugePanel:
			{
				GaugePanel gaugePanel = reportElement as GaugePanel;
				this.m_disallowBorderTransparencyOnDynamicImage = (gaugePanel != null);
				this.m_isDynamicImageStyle = true;
				this.m_styleDefaults = this.NormalStyleDefaults;
				break;
			}
			case ObjectType.Map:
			{
				Map map = reportElement as Map;
				this.m_disallowBorderTransparencyOnDynamicImage = (map != null);
				this.m_isDynamicImageStyle = true;
				this.m_styleDefaults = this.NormalStyleDefaults;
				break;
			}
			default:
				this.m_styleDefaults = this.NormalStyleDefaults;
				break;
			}
		}

		internal Style(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem, RenderingContext renderingContext, bool useRenderStyle)
			: base(renderingContext)
		{
			this.m_isOldSnapshot = true;
			this.m_renderReportItem = renderReportItem;
			this.m_lineStyleDefaults = new StyleDefaults(true, null);
			this.m_normalStyleDefaults = new StyleDefaults(false, null);
			if (useRenderStyle)
			{
				this.m_cachedRenderStyle = renderReportItem.Style;
			}
			if (renderReportItem is AspNetCore.ReportingServices.ReportRendering.Line)
			{
				this.m_isLineBorderStyle = true;
				this.m_styleDefaults = this.LineStyleDefaults;
			}
			else
			{
				this.m_styleDefaults = this.NormalStyleDefaults;
			}
		}

		internal Style(AspNetCore.ReportingServices.ReportProcessing.Style styleDefinition, object[] styleValues, RenderingContext renderingContext)
			: base(renderingContext)
		{
			this.m_isOldSnapshot = true;
			this.m_isDynamicImageStyle = true;
			this.m_lineStyleDefaults = new StyleDefaults(true, Style.GetDefaultFontFamily(renderingContext));
			this.m_normalStyleDefaults = new StyleDefaults(false, Style.GetDefaultFontFamily(renderingContext));
			this.m_styleDef = styleDefinition;
			this.m_styleValues = styleValues;
			this.m_styleDefaults = this.NormalStyleDefaults;
		}

		internal Style(ReportElement reportElement, RenderingContext renderingContext)
			: base(renderingContext)
		{
			this.m_isOldSnapshot = true;
			this.m_reportElement = reportElement;
			this.m_lineStyleDefaults = new StyleDefaults(true, Style.GetDefaultFontFamily(renderingContext));
			this.m_normalStyleDefaults = new StyleDefaults(false, Style.GetDefaultFontFamily(renderingContext));
			this.m_styleDefaults = this.NormalStyleDefaults;
			this.m_reportScope = reportElement.ReportScope;
		}

		internal void UpdateStyleCache(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			if (renderReportItem != null)
			{
				this.m_renderReportItem = renderReportItem;
				this.m_cachedRenderStyle = renderReportItem.Style;
			}
		}

		internal void UpdateStyleCache(object[] styleValues)
		{
			this.m_styleValues = styleValues;
		}

		internal void SetNewContext()
		{
			if (this.m_backgroundImage != null && this.m_backgroundImage.Instance != null)
			{
				this.m_backgroundImage.Instance.SetNewContext();
			}
			if (this.m_border != null && this.m_border.GetInstance() != null)
			{
				this.m_border.GetInstance().SetNewContext();
			}
			if (this.m_topBorder != null && this.m_topBorder.GetInstance() != null)
			{
				this.m_topBorder.GetInstance().SetNewContext();
			}
			if (this.m_rightBorder != null && this.m_rightBorder.GetInstance() != null)
			{
				this.m_rightBorder.GetInstance().SetNewContext();
			}
			if (this.m_bottomBorder != null && this.m_bottomBorder.GetInstance() != null)
			{
				this.m_bottomBorder.GetInstance().SetNewContext();
			}
			if (this.m_leftBorder != null && this.m_leftBorder.GetInstance() != null)
			{
				this.m_leftBorder.GetInstance().SetNewContext();
			}
		}

		internal ReportColor EvaluateInstanceReportColor(StyleAttributeNames style)
		{
			ReportColor result = null;
			if (this.m_isOldSnapshot)
			{
				if (this.m_isDynamicImageStyle)
				{
					if (this.m_styleDef != null)
					{
						string styleStringFromEnum = this.GetStyleStringFromEnum(style);
						AspNetCore.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = this.m_styleDef.StyleAttributes[styleStringFromEnum];
						if (attributeInfo != null)
						{
							string text = null;
							text = ((!attributeInfo.IsExpression) ? attributeInfo.Value : (this.m_styleValues[attributeInfo.IntValue] as string));
							if (text != null)
							{
								result = new ReportColor(text, false);
							}
						}
					}
				}
				else if (this.IsAvailableStyle(style))
				{
					AspNetCore.ReportingServices.ReportRendering.ReportColor reportColor = null;
					if (this.m_cachedRenderStyle != null)
					{
						reportColor = (((AspNetCore.ReportingServices.ReportRendering.StyleBase)this.m_cachedRenderStyle)[this.GetStyleStringFromEnum(style)] as AspNetCore.ReportingServices.ReportRendering.ReportColor);
					}
					if (reportColor != null)
					{
						result = new ReportColor(reportColor);
					}
				}
			}
			else if (this.m_iStyleContainer.StyleClass != null)
			{
				base.m_renderingContext.OdpContext.SetupContext(this.m_iStyleContainer.InstancePath, this.m_reportScope.ReportScopeInstance);
				string text2 = this.m_iStyleContainer.StyleClass.EvaluateStyle(this.m_iStyleContainer.ObjectType, this.m_iStyleContainer.Name, (AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId)style, base.m_renderingContext.OdpContext) as string;
				if (text2 != null)
				{
					if (this.m_disallowBorderTransparencyOnDynamicImage)
					{
						switch (style)
						{
						case StyleAttributeNames.BorderColor:
						case StyleAttributeNames.BorderColorTop:
						case StyleAttributeNames.BorderColorLeft:
						case StyleAttributeNames.BorderColorRight:
						case StyleAttributeNames.BorderColorBottom:
							if (!ReportColor.TryParse(text2, out result))
							{
								base.m_renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsInvalidColor, Severity.Warning, this.m_iStyleContainer.ObjectType, this.m_iStyleContainer.Name, this.GetStyleStringFromEnum(style), text2);
							}
							break;
						default:
							result = new ReportColor(text2, this.m_isDynamicImageStyle);
							break;
						}
					}
					else
					{
						result = new ReportColor(text2, this.m_isDynamicImageStyle);
					}
				}
			}
			return result;
		}

		internal ReportSize EvaluateInstanceReportSize(StyleAttributeNames style)
		{
			ReportSize result = null;
			if (this.m_isOldSnapshot)
			{
				if (this.m_isDynamicImageStyle)
				{
					if (this.m_styleDef != null)
					{
						string styleStringFromEnum = this.GetStyleStringFromEnum(style);
						AspNetCore.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = this.m_styleDef.StyleAttributes[styleStringFromEnum];
						if (attributeInfo != null)
						{
							object obj = null;
							obj = ((!attributeInfo.IsExpression) ? attributeInfo.Value : this.m_styleValues[attributeInfo.IntValue]);
							if (obj != null)
							{
								result = new ReportSize(obj as string);
							}
						}
					}
				}
				else if (this.IsAvailableStyle(style))
				{
					AspNetCore.ReportingServices.ReportRendering.ReportSize reportSize = null;
					if (this.m_cachedRenderStyle != null)
					{
						reportSize = (((AspNetCore.ReportingServices.ReportRendering.StyleBase)this.m_cachedRenderStyle)[this.GetStyleStringFromEnum(style)] as AspNetCore.ReportingServices.ReportRendering.ReportSize);
					}
					if (reportSize != null)
					{
						result = new ReportSize(reportSize);
					}
				}
			}
			else if (this.m_iStyleContainer.StyleClass != null)
			{
				base.m_renderingContext.OdpContext.SetupContext(this.m_iStyleContainer.InstancePath, this.m_reportScope.ReportScopeInstance);
				string text = this.m_iStyleContainer.StyleClass.EvaluateStyle(this.m_iStyleContainer.ObjectType, this.m_iStyleContainer.Name, (AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId)style, base.m_renderingContext.OdpContext) as string;
				if (text != null)
				{
					result = new ReportSize(text);
				}
			}
			return result;
		}

		internal string EvaluateInstanceStyleString(StyleAttributeNames style)
		{
			string result = null;
			if (this.m_isOldSnapshot)
			{
				if (this.m_isDynamicImageStyle)
				{
					if (this.m_styleDef != null)
					{
						string styleStringFromEnum = this.GetStyleStringFromEnum(style);
						AspNetCore.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = this.m_styleDef.StyleAttributes[styleStringFromEnum];
						if (attributeInfo != null)
						{
							result = ((!attributeInfo.IsExpression) ? attributeInfo.Value : (this.m_styleValues[attributeInfo.IntValue] as string));
						}
					}
				}
				else if (this.IsAvailableStyle(style) && this.m_cachedRenderStyle != null)
				{
					result = (((AspNetCore.ReportingServices.ReportRendering.StyleBase)this.m_cachedRenderStyle)[this.GetStyleStringFromEnum(style)] as string);
				}
			}
			else if (this.m_iStyleContainer.StyleClass != null)
			{
				result = this.EvaluateInstanceStyleString((AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId)style);
			}
			return result;
		}

		internal string EvaluateInstanceStyleString(AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId style)
		{
			string text = null;
			base.m_renderingContext.OdpContext.SetupContext(this.m_iStyleContainer.InstancePath, this.m_reportScope.ReportScopeInstance);
			return this.m_iStyleContainer.StyleClass.EvaluateStyle(this.m_iStyleContainer.ObjectType, this.m_iStyleContainer.Name, style, base.m_renderingContext.OdpContext) as string;
		}

		internal int EvaluateInstanceStyleInt(StyleAttributeNames style, int defaultValue)
		{
			object obj = null;
			if (this.m_isOldSnapshot)
			{
				if (this.m_isDynamicImageStyle)
				{
					if (this.m_styleDef != null)
					{
						string styleStringFromEnum = this.GetStyleStringFromEnum(style);
						AspNetCore.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = this.m_styleDef.StyleAttributes[styleStringFromEnum];
						if (attributeInfo != null)
						{
							obj = ((!attributeInfo.IsExpression) ? ((object)attributeInfo.IntValue) : this.m_styleValues[attributeInfo.IntValue]);
						}
					}
				}
				else if (this.IsAvailableStyle(style) && this.m_cachedRenderStyle != null)
				{
					obj = ((AspNetCore.ReportingServices.ReportRendering.StyleBase)this.m_cachedRenderStyle)[this.GetStyleStringFromEnum(style)];
				}
			}
			else if (this.m_iStyleContainer.StyleClass != null)
			{
				obj = this.EvaluateInstanceStyleInt((AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId)style);
			}
			if (obj != null && obj is int)
			{
				return (int)obj;
			}
			return defaultValue;
		}

		private object EvaluateInstanceStyleInt(AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId style)
		{
			object obj = null;
			base.m_renderingContext.OdpContext.SetupContext(this.m_iStyleContainer.InstancePath, this.m_reportScope.ReportScopeInstance);
			return this.m_iStyleContainer.StyleClass.EvaluateStyle(this.m_iStyleContainer.ObjectType, this.m_iStyleContainer.Name, style, base.m_renderingContext.OdpContext);
		}

		internal int EvaluateInstanceStyleEnum(StyleAttributeNames style)
		{
			return this.EvaluateInstanceStyleEnum(style, 1);
		}

		internal int EvaluateInstanceStyleEnum(StyleAttributeNames style, int styleDefaultValueIfNull)
		{
			int? nullable = null;
			if (this.m_isOldSnapshot)
			{
				if (this.m_isDynamicImageStyle)
				{
					if (this.m_styleDef != null)
					{
						string styleStringFromEnum = this.GetStyleStringFromEnum(style);
						AspNetCore.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = this.m_styleDef.StyleAttributes[styleStringFromEnum];
						if (attributeInfo != null)
						{
							string text = null;
							text = ((!attributeInfo.IsExpression) ? attributeInfo.Value : (this.m_styleValues[attributeInfo.IntValue] as string));
							if (text != null)
							{
								nullable = StyleTranslator.TranslateStyle(style, text, null, this.m_isDynamicImageStyle);
							}
						}
					}
				}
				else if (this.IsAvailableStyle(style) && this.m_cachedRenderStyle != null)
				{
					string text2 = ((AspNetCore.ReportingServices.ReportRendering.StyleBase)this.m_cachedRenderStyle)[this.GetStyleStringFromEnum(style)] as string;
					if (text2 != null)
					{
						nullable = StyleTranslator.TranslateStyle(style, text2, null, this.m_isDynamicImageStyle);
					}
				}
			}
			else if (this.m_iStyleContainer.StyleClass != null)
			{
				nullable = this.EvaluateInstanceStyleEnum((AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId)style);
			}
			if (!nullable.HasValue)
			{
				return styleDefaultValueIfNull;
			}
			return nullable.Value;
		}

		internal int? EvaluateInstanceStyleEnum(AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId style)
		{
			int? result = null;
			base.m_renderingContext.OdpContext.SetupContext(this.m_iStyleContainer.InstancePath, this.m_reportScope.ReportScopeInstance);
			object obj = this.m_iStyleContainer.StyleClass.EvaluateStyle(this.m_iStyleContainer.ObjectType, this.m_iStyleContainer.Name, style, base.m_renderingContext.OdpContext);
			if (obj != null)
			{
				string text = obj as string;
				result = ((text == null) ? new int?((int)obj) : new int?(StyleTranslator.TranslateStyle((StyleAttributeNames)style, text, null, this.m_isDynamicImageStyle)));
				if (AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageReportItem(this.m_iStyleContainer.ObjectType) && !AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageSubElement(this.m_iStyleContainer) && (result.Value == 6 || result.Value == 7))
				{
					result = 3;
				}
			}
			return result;
		}

		internal object EvaluateInstanceStyleVariant(AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId style)
		{
			object obj = null;
			base.m_renderingContext.OdpContext.SetupContext(this.m_iStyleContainer.InstancePath, this.m_reportScope.ReportScopeInstance);
			return this.m_iStyleContainer.StyleClass.EvaluateStyle(this.m_iStyleContainer.ObjectType, this.m_iStyleContainer.Name, style, base.m_renderingContext.OdpContext);
		}

		internal void ConstructStyleDefinition()
		{
			Global.Tracer.Assert(this.ReportElement != null, "(ReportElement != null)");
			Global.Tracer.Assert(this.ReportElement is ReportItem, "(ReportElement is ReportItem)");
			Global.Tracer.Assert(this.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition, "(ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition)");
			Global.Tracer.Assert(this.ReportElement.ReportItemDef.StyleClass == null, "(ReportElement.ReportItemDef.StyleClass == null)");
			this.ReportElement.ReportItemDef.StyleClass = new AspNetCore.ReportingServices.ReportIntermediateFormat.Style(AspNetCore.ReportingServices.ReportIntermediateFormat.ConstructionPhase.Publishing);
			this.ReportElement.ReportItemDef.StyleClass.InitializeForCRIGeneratedReportItem();
			this.Border.ConstructBorderDefinition();
			this.TopBorder.ConstructBorderDefinition();
			this.BottomBorder.ConstructBorderDefinition();
			this.LeftBorder.ConstructBorderDefinition();
			this.RightBorder.ConstructBorderDefinition();
			StyleInstance style = ((ReportItem)this.ReportElement).Instance.Style;
			Global.Tracer.Assert(!this.BackgroundColor.IsExpression, "(!this.BackgroundColor.IsExpression)");
			if (style.IsBackgroundColorAssigned)
			{
				string value = (style.BackgroundColor != null) ? style.BackgroundColor.ToString() : null;
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.BackgroundColor), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.BackgroundColor), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.BackgroundGradientEndColor.IsExpression, "(!this.BackgroundGradientEndColor.IsExpression)");
			if (style.IsBackgroundGradientEndColorAssigned)
			{
				string value2 = (style.BackgroundGradientEndColor != null) ? style.BackgroundGradientEndColor.ToString() : null;
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.BackgroundGradientEndColor), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value2));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.BackgroundGradientEndColor), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.Color.IsExpression, "(!this.Color.IsExpression)");
			if (style.IsColorAssigned)
			{
				string value3 = (style.Color != null) ? style.Color.ToString() : null;
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.Color), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value3));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.Color), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.FontStyle.IsExpression, "(!this.FontStyle.IsExpression)");
			if (style.IsFontStyleAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.FontStyle), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.FontStyle.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.FontStyle), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.FontFamily.IsExpression, "(!this.FontFamily.IsExpression)");
			if (style.IsFontFamilyAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.FontFamily), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.FontFamily));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.FontFamily), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.FontWeight.IsExpression, "(!this.FontWeight.IsExpression)");
			if (style.IsFontWeightAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.FontWeight), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.FontWeight.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.FontWeight), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.Format.IsExpression, "(!this.Format.IsExpression)");
			if (style.IsFormatAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.Format), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.Format));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.Format), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.TextDecoration.IsExpression, "(!this.TextDecoration.IsExpression)");
			if (style.IsTextDecorationAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.TextDecoration), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.TextDecoration.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.TextDecoration), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.TextAlign.IsExpression, "(!this.TextAlign.IsExpression)");
			if (style.IsTextAlignAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.TextAlign), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.TextAlign.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.TextAlign), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.VerticalAlign.IsExpression, "(!this.VerticalAlign.IsExpression)");
			if (style.IsVerticalAlignAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.VerticalAlign), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.VerticalAlign.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.VerticalAlign), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.Direction.IsExpression, "(!this.Direction.IsExpression)");
			if (style.IsDirectionAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.Direction), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.Direction.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.Direction), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.WritingMode.IsExpression, "(!this.WritingMode.IsExpression)");
			if (style.IsWritingModeAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.WritingMode), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.WritingMode.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.WritingMode), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.Language.IsExpression, "(!this.Language.IsExpression)");
			if (style.IsLanguageAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.Language), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.Language));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.Language), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.UnicodeBiDi.IsExpression, "(!this.UnicodeBiDi.IsExpression)");
			if (style.IsUnicodeBiDiAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.UnicodeBiDi), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.UnicodeBiDi.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.UnicodeBiDi), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.Calendar.IsExpression, "(!this.Calendar.IsExpression)");
			if (style.IsCalendarAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.Calendar), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.Calendar.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.Calendar), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.CurrencyLanguage.IsExpression, "(!this.CurrencyLanguage.IsExpression)");
			if (style.IsCurrencyLanguageAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.CurrencyLanguage), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.CurrencyLanguage));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.CurrencyLanguage), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.NumeralLanguage.IsExpression, "(!this.NumeralLanguage.IsExpression)");
			if (style.IsNumeralLanguageAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.NumeralLanguage), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.NumeralLanguage));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.NumeralLanguage), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.BackgroundGradientType.IsExpression, "(!this.BackgroundGradientType.IsExpression)");
			if (style.IsBackgroundGradientTypeAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.BackgroundGradientType), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.BackgroundGradientType.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.BackgroundGradientType), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.FontSize.IsExpression, "(!this.FontSize.IsExpression)");
			if (style.IsFontSizeAssigned)
			{
				string value4 = (style.FontSize != null) ? style.FontSize.ToString() : null;
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.FontSize), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value4));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.FontSize), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.PaddingLeft.IsExpression, "(!this.PaddingLeft.IsExpression)");
			if (style.IsPaddingLeftAssigned)
			{
				string value5 = (style.PaddingLeft != null) ? style.PaddingLeft.ToString() : null;
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.PaddingLeft), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value5));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.PaddingLeft), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.PaddingRight.IsExpression, "(!this.PaddingRight.IsExpression)");
			if (style.IsPaddingRightAssigned)
			{
				string value6 = (style.PaddingRight != null) ? style.PaddingRight.ToString() : null;
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.PaddingRight), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value6));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.PaddingRight), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.PaddingTop.IsExpression, "(!this.PaddingTop.IsExpression)");
			if (style.IsPaddingTopAssigned)
			{
				string value7 = (style.PaddingTop != null) ? style.PaddingTop.ToString() : null;
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.PaddingTop), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value7));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.PaddingTop), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.PaddingBottom.IsExpression, "(!this.PaddingBottom.IsExpression)");
			if (style.IsPaddingBottomAssigned)
			{
				string value8 = (style.PaddingBottom != null) ? style.PaddingBottom.ToString() : null;
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.PaddingBottom), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value8));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.PaddingBottom), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.LineHeight.IsExpression, "(!this.LineHeight.IsExpression)");
			if (style.IsLineHeightAssigned)
			{
				string value9 = (style.LineHeight != null) ? style.LineHeight.ToString() : null;
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.LineHeight), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value9));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.LineHeight), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.NumeralVariant.IsExpression, "(!this.NumeralVariant.IsExpression)");
			if (style.IsNumeralVariantAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.NumeralVariant), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.NumeralVariant));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.NumeralVariant), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.TextEffect.IsExpression, "(!this.TextEffect.IsExpression)");
			if (style.IsTextEffectAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.TextEffect), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.TextEffect.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.TextEffect), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.BackgroundHatchType.IsExpression, "(!this.BackgroundHatchType.IsExpression)");
			if (style.IsBackgroundHatchTypeAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.BackgroundHatchType), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.BackgroundHatchType.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.BackgroundHatchType), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.ShadowColor.IsExpression, "(!this.ShadowColor.IsExpression)");
			if (style.IsShadowColorAssigned)
			{
				string value10 = (style.Color != null) ? style.ShadowColor.ToString() : null;
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.ShadowColor), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value10));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.ShadowColor), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(!this.ShadowOffset.IsExpression, "(!this.ShadowOffset.IsExpression)");
			if (style.IsShadowOffsetAssigned)
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.ShadowOffset), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(style.ShadowOffset.ToString()));
			}
			else
			{
				this.m_iStyleContainer.StyleClass.AddAttribute(this.GetStyleStringFromEnum(StyleAttributeNames.ShadowOffset), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			foreach (StyleAttributeNames styleName in StyleBase.StyleNames)
			{
				string styleStringFromEnum = this.GetStyleStringFromEnum(styleName);
				AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = default(AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo);
				if (!this.m_iStyleContainer.StyleClass.GetAttributeInfo(styleStringFromEnum, out attributeInfo))
				{
					this.m_iStyleContainer.StyleClass.AddAttribute(styleStringFromEnum, AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
				}
				else if (!attributeInfo.IsExpression && attributeInfo.Value == null)
				{
					this.m_iStyleContainer.StyleClass.StyleAttributes.Remove(styleStringFromEnum);
				}
			}
			base.m_sharedStyles = null;
			base.m_nonSharedStyles = null;
		}

		private bool HasBorderProperties(Border.Position position)
		{
			if (position == Border.Position.Default)
			{
				return true;
			}
			if (this.m_isOldSnapshot)
			{
				if (!this.IsAvailableStyle(StyleAttributeNames.BorderColor))
				{
					return false;
				}
				if (this.m_cachedRenderStyle == null && this.m_styleDef == null)
				{
					return false;
				}
				string text = null;
				switch (position)
				{
				case Border.Position.Top:
					text = "BorderStyleTop";
					break;
				case Border.Position.Right:
					text = "BorderStyleRight";
					break;
				case Border.Position.Bottom:
					text = "BorderStyleBottom";
					break;
				case Border.Position.Left:
					text = "BorderStyleLeft";
					break;
				}
				if (this.m_isDynamicImageStyle)
				{
					if (this.m_styleDef.StyleAttributes[text] != null)
					{
						return true;
					}
				}
				else if (this.m_cachedRenderStyle.GetStyleDefinition(text) != null)
				{
					return true;
				}
				switch (position)
				{
				case Border.Position.Top:
					text = "BorderColorTop";
					break;
				case Border.Position.Right:
					text = "BorderColorRight";
					break;
				case Border.Position.Bottom:
					text = "BorderColorBottom";
					break;
				case Border.Position.Left:
					text = "BorderColorLeft";
					break;
				}
				if (this.m_isDynamicImageStyle)
				{
					if (this.m_styleDef.StyleAttributes[text] != null)
					{
						return true;
					}
				}
				else if (this.m_cachedRenderStyle.GetStyleDefinition(text) != null)
				{
					return true;
				}
				switch (position)
				{
				case Border.Position.Top:
					text = "BorderWidthTop";
					break;
				case Border.Position.Right:
					text = "BorderWidthRight";
					break;
				case Border.Position.Bottom:
					text = "BorderWidthBottom";
					break;
				case Border.Position.Left:
					text = "BorderWidthLeft";
					break;
				}
				if (this.m_isDynamicImageStyle)
				{
					if (this.m_styleDef.StyleAttributes[text] != null)
					{
						return true;
					}
				}
				else if (this.m_cachedRenderStyle.GetStyleDefinition(text) != null)
				{
					return true;
				}
				return false;
			}
			return true;
		}

		private static string GetDefaultFontFamily(RenderingContext renderingContext)
		{
			if (renderingContext.OdpContext != null && renderingContext.OdpContext.ReportDefinition != null)
			{
				return renderingContext.OdpContext.ReportDefinition.DefaultFontFamily;
			}
			return null;
		}

		private void PopulateCollections()
		{
			if (this.m_isOldSnapshot)
			{
				if (this.m_cachedRenderStyle == null && this.m_styleDef == null)
				{
					return;
				}
				base.m_sharedStyles = new List<StyleAttributeNames>();
				base.m_nonSharedStyles = new List<StyleAttributeNames>();
				foreach (StyleAttributeNames styleName in StyleBase.StyleNames)
				{
					bool flag = default(bool);
					if (StyleAttributeNames.BackgroundImage != styleName)
					{
						AspNetCore.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = null;
						if (this.IsAvailableStyle(styleName))
						{
							attributeInfo = ((!this.m_isDynamicImageStyle) ? this.m_cachedRenderStyle.GetStyleDefinition(this.GetStyleStringFromEnum(styleName)) : this.m_styleDef.StyleAttributes[this.GetStyleStringFromEnum(styleName)]);
						}
						if (attributeInfo != null)
						{
							if (attributeInfo.IsExpression)
							{
								base.m_nonSharedStyles.Add(styleName);
							}
							else
							{
								base.m_sharedStyles.Add(styleName);
							}
						}
					}
					else if (!this.m_isDynamicImageStyle && this.m_cachedRenderStyle.HasBackgroundImage(out flag))
					{
						if (flag)
						{
							base.m_nonSharedStyles.Add(styleName);
						}
						else
						{
							base.m_sharedStyles.Add(styleName);
						}
					}
				}
			}
			else
			{
				base.m_sharedStyles = new List<StyleAttributeNames>();
				base.m_nonSharedStyles = new List<StyleAttributeNames>();
				if (this.m_iStyleContainer != null && this.m_iStyleContainer.StyleClass != null && this.m_iStyleContainer.StyleClass.StyleAttributes != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo2 = null;
					foreach (StyleAttributeNames styleName2 in StyleBase.StyleNames)
					{
						string styleAttributeName = (StyleAttributeNames.BackgroundImage == styleName2) ? "BackgroundImageValue" : this.GetStyleStringFromEnum(styleName2);
						if (this.m_iStyleContainer.StyleClass.GetAttributeInfo(styleAttributeName, out attributeInfo2))
						{
							if (attributeInfo2.IsExpression)
							{
								base.m_nonSharedStyles.Add(styleName2);
							}
							else
							{
								base.m_sharedStyles.Add(styleName2);
							}
						}
					}
				}
			}
		}

		internal string GetStyleStringFromEnum(StyleAttributeNames style)
		{
			switch (style)
			{
			case StyleAttributeNames.BackgroundColor:
				return "BackgroundColor";
			case StyleAttributeNames.Color:
				return "Color";
			case StyleAttributeNames.FontStyle:
				return "FontStyle";
			case StyleAttributeNames.FontFamily:
				return "FontFamily";
			case StyleAttributeNames.FontWeight:
				return "FontWeight";
			case StyleAttributeNames.FontSize:
				return "FontSize";
			case StyleAttributeNames.Format:
				return "Format";
			case StyleAttributeNames.BackgroundImage:
				return "BackgroundImage";
			case StyleAttributeNames.BorderColor:
				return "BorderColor";
			case StyleAttributeNames.BorderColorBottom:
				return "BorderColorBottom";
			case StyleAttributeNames.BorderColorLeft:
				return "BorderColorLeft";
			case StyleAttributeNames.BorderColorRight:
				return "BorderColorRight";
			case StyleAttributeNames.BorderColorTop:
				return "BorderColorTop";
			case StyleAttributeNames.BackgroundGradientEndColor:
				return "BackgroundGradientEndColor";
			case StyleAttributeNames.BorderStyle:
				return "BorderStyle";
			case StyleAttributeNames.BorderStyleTop:
				return "BorderStyleTop";
			case StyleAttributeNames.BorderStyleLeft:
				return "BorderStyleLeft";
			case StyleAttributeNames.BorderStyleRight:
				return "BorderStyleRight";
			case StyleAttributeNames.BorderStyleBottom:
				return "BorderStyleBottom";
			case StyleAttributeNames.TextDecoration:
				return "TextDecoration";
			case StyleAttributeNames.TextAlign:
				return "TextAlign";
			case StyleAttributeNames.VerticalAlign:
				return "VerticalAlign";
			case StyleAttributeNames.Direction:
				return "Direction";
			case StyleAttributeNames.WritingMode:
				return "WritingMode";
			case StyleAttributeNames.Language:
				return "Language";
			case StyleAttributeNames.UnicodeBiDi:
				return "UnicodeBiDi";
			case StyleAttributeNames.Calendar:
				return "Calendar";
			case StyleAttributeNames.CurrencyLanguage:
				return "CurrencyLanguage";
			case StyleAttributeNames.NumeralLanguage:
				return "NumeralLanguage";
			case StyleAttributeNames.BackgroundGradientType:
				return "BackgroundGradientType";
			case StyleAttributeNames.BorderWidth:
				return "BorderWidth";
			case StyleAttributeNames.BorderWidthTop:
				return "BorderWidthTop";
			case StyleAttributeNames.BorderWidthLeft:
				return "BorderWidthLeft";
			case StyleAttributeNames.BorderWidthRight:
				return "BorderWidthRight";
			case StyleAttributeNames.BorderWidthBottom:
				return "BorderWidthBottom";
			case StyleAttributeNames.PaddingLeft:
				return "PaddingLeft";
			case StyleAttributeNames.PaddingRight:
				return "PaddingRight";
			case StyleAttributeNames.PaddingTop:
				return "PaddingTop";
			case StyleAttributeNames.PaddingBottom:
				return "PaddingBottom";
			case StyleAttributeNames.LineHeight:
				return "LineHeight";
			case StyleAttributeNames.NumeralVariant:
				return "NumeralVariant";
			case StyleAttributeNames.TextEffect:
				return "TextEffect";
			case StyleAttributeNames.BackgroundHatchType:
				return "BackgroundHatchType";
			case StyleAttributeNames.ShadowColor:
				return "ShadowColor";
			case StyleAttributeNames.ShadowOffset:
				return "ShadowOffset";
			case StyleAttributeNames.BackgroundImageRepeat:
				return "BackgroundRepeat";
			case StyleAttributeNames.BackgroundImageSource:
				return "BackgroundImageSource";
			case StyleAttributeNames.BackgroundImageValue:
				return "BackgroundImageValue";
			case StyleAttributeNames.BackgroundImageMimeType:
				return "BackgroundImageMIMEType";
			case StyleAttributeNames.Position:
				return "Position";
			case StyleAttributeNames.TransparentColor:
				return "TransparentColor";
			default:
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
		}

		private ReportProperty GetReportProperty(StyleAttributeNames styleName)
		{
			ReportProperty reportProperty = null;
			if (styleName >= StyleAttributeNames.Count)
			{
				return null;
			}
			if (this.m_cachedReportProperties[(int)styleName] != null)
			{
				reportProperty = this.m_cachedReportProperties[(int)styleName];
			}
			else
			{
				reportProperty = ((!this.m_isOldSnapshot) ? this.GetOdpReportProperty(styleName) : ((!this.m_isDynamicImageStyle) ? this.GetOldSnapshotReportProperty(styleName, this.m_cachedRenderStyle) : this.GetOldSnapshotReportProperty(styleName, this.m_styleDef)));
				if (this.ReportElement == null || this.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Definition)
				{
					this.m_cachedReportProperties[(int)styleName] = reportProperty;
				}
			}
			return reportProperty;
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo GetAttributeInfo(string styleNameString, out string expressionString)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = null;
			expressionString = null;
			if (this.m_iStyleContainer.StyleClass != null && this.m_iStyleContainer.StyleClass.GetAttributeInfo(styleNameString, out attributeInfo))
			{
				if (attributeInfo.IsExpression)
				{
					expressionString = this.m_iStyleContainer.StyleClass.ExpressionList[attributeInfo.IntValue].OriginalText;
				}
				else
				{
					expressionString = attributeInfo.Value;
				}
			}
			return attributeInfo;
		}

		private ReportProperty GetOdpReportProperty(StyleAttributeNames styleName)
		{
			string text = null;
			text = ((styleName != StyleAttributeNames.BackgroundImage) ? this.GetStyleStringFromEnum(styleName) : "BackgroundImageValue");
			string text2 = null;
			string expressionString = null;
			AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = this.GetAttributeInfo(text, out expressionString);
			switch (styleName)
			{
			case StyleAttributeNames.BackgroundImage:
				if (attributeInfo == null)
				{
					break;
				}
				return new BackgroundImage(attributeInfo.IsExpression, expressionString, this);
			case StyleAttributeNames.BorderColor:
			case StyleAttributeNames.BorderColorTop:
			case StyleAttributeNames.BorderColorLeft:
			case StyleAttributeNames.BorderColorRight:
			case StyleAttributeNames.BorderColorBottom:
			case StyleAttributeNames.BackgroundColor:
			case StyleAttributeNames.Color:
			case StyleAttributeNames.BackgroundGradientEndColor:
			case StyleAttributeNames.ShadowColor:
			{
				ReportColor reportColor = null;
				if (!this.m_isDynamicImageStyle || styleName != StyleAttributeNames.Color)
				{
					reportColor = (this.m_styleDefaults[text] as ReportColor);
				}
				if (attributeInfo == null)
				{
					return new ReportColorProperty(false, null, reportColor, reportColor);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportColorProperty(attributeInfo.IsExpression, expressionString, attributeInfo.IsExpression ? null : new ReportColor(text2, this.m_isDynamicImageStyle), reportColor);
			}
			case StyleAttributeNames.FontFamily:
			case StyleAttributeNames.Format:
			case StyleAttributeNames.Language:
			case StyleAttributeNames.NumeralLanguage:
			case StyleAttributeNames.CurrencyLanguage:
				if (attributeInfo == null)
				{
					return new ReportStringProperty(false, null, this.m_styleDefaults[text] as string);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportStringProperty(attributeInfo.IsExpression, expressionString, text2, attributeInfo.IsExpression ? (this.m_styleDefaults[text] as string) : null);
			case StyleAttributeNames.BorderStyle:
			{
				BorderStyles borderStyles = (BorderStyles)((!this.m_isLineBorderStyle) ? 1 : 4);
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<BorderStyles>(false, null, borderStyles, borderStyles);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<BorderStyles>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateBorderStyle(text2, null), borderStyles);
			}
			case StyleAttributeNames.BorderStyleTop:
			case StyleAttributeNames.BorderStyleLeft:
			case StyleAttributeNames.BorderStyleRight:
			case StyleAttributeNames.BorderStyleBottom:
				if (attributeInfo == null)
				{
					return null;
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<BorderStyles>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateBorderStyle(text2, null), BorderStyles.None);
			case StyleAttributeNames.FontStyle:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<FontStyles>(Style.DefaultEnumFontStyle);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<FontStyles>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateFontStyle(text2, null), Style.DefaultEnumFontStyle);
			case StyleAttributeNames.FontWeight:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<FontWeights>(Style.DefaultEnumFontWeight);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<FontWeights>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateFontWeight(text2, null), Style.DefaultEnumFontWeight);
			case StyleAttributeNames.TextDecoration:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<TextDecorations>(Style.DefaultEnumTextDecoration);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<TextDecorations>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateTextDecoration(text2, null), Style.DefaultEnumTextDecoration);
			case StyleAttributeNames.TextAlign:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<TextAlignments>(Style.DefaultEnumTextAlignment);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<TextAlignments>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateTextAlign(text2, null), Style.DefaultEnumTextAlignment);
			case StyleAttributeNames.VerticalAlign:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<VerticalAlignments>(Style.DefaultEnumVerticalAlignment);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<VerticalAlignments>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateVerticalAlign(text2, null), Style.DefaultEnumVerticalAlignment);
			case StyleAttributeNames.Direction:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<Directions>(Style.DefaultEnumDirection);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<Directions>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateDirection(text2, null), Style.DefaultEnumDirection);
			case StyleAttributeNames.WritingMode:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<WritingModes>(Style.DefaultEnumWritingMode);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<WritingModes>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateWritingMode(text2, null), Style.DefaultEnumWritingMode);
			case StyleAttributeNames.UnicodeBiDi:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<UnicodeBiDiTypes>(Style.DefaultEnumUnicodeBiDiType);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<UnicodeBiDiTypes>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateUnicodeBiDi(text2, null), Style.DefaultEnumUnicodeBiDiType);
			case StyleAttributeNames.Calendar:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<Calendars>(Style.DefaultEnumCalendar);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<Calendars>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateCalendar(text2, null), Style.DefaultEnumCalendar);
			case StyleAttributeNames.BackgroundGradientType:
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<BackgroundGradients>(Style.DefaultEnumBackgroundGradient);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<BackgroundGradients>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateBackgroundGradientType(text2, null), Style.DefaultEnumBackgroundGradient);
			case StyleAttributeNames.BorderWidth:
			case StyleAttributeNames.FontSize:
			case StyleAttributeNames.PaddingLeft:
			case StyleAttributeNames.PaddingRight:
			case StyleAttributeNames.PaddingTop:
			case StyleAttributeNames.PaddingBottom:
			case StyleAttributeNames.LineHeight:
			case StyleAttributeNames.ShadowOffset:
				if (attributeInfo == null)
				{
					return new ReportSizeProperty(false, null, this.m_styleDefaults[text] as ReportSize);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportSizeProperty(attributeInfo.IsExpression, expressionString, attributeInfo.IsExpression ? null : new ReportSize(text2, false), attributeInfo.IsExpression ? (this.m_styleDefaults[text] as ReportSize) : null);
			case StyleAttributeNames.BorderWidthTop:
			case StyleAttributeNames.BorderWidthLeft:
			case StyleAttributeNames.BorderWidthRight:
			case StyleAttributeNames.BorderWidthBottom:
				if (attributeInfo == null)
				{
					return null;
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportSizeProperty(attributeInfo.IsExpression, expressionString, attributeInfo.IsExpression ? null : new ReportSize(text2, false), null);
			case StyleAttributeNames.NumeralVariant:
			{
				int num = (int)this.m_styleDefaults[text];
				if (attributeInfo == null)
				{
					return new ReportIntProperty(false, null, num, num);
				}
				if (!attributeInfo.IsExpression)
				{
					num = attributeInfo.IntValue;
				}
				return new ReportIntProperty(attributeInfo.IsExpression, expressionString, num, num);
			}
			case StyleAttributeNames.TextEffect:
			{
				TextEffects defaultValue2 = StyleTranslator.TranslateTextEffect(null, null, this.m_isDynamicImageStyle);
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<TextEffects>(false, null, TextEffects.Default, defaultValue2);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<TextEffects>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateTextEffect(text2, null, this.m_isDynamicImageStyle), defaultValue2);
			}
			case StyleAttributeNames.BackgroundHatchType:
			{
				BackgroundHatchTypes defaultValue = StyleTranslator.TranslateBackgroundHatchType(null, null, this.m_isDynamicImageStyle);
				if (attributeInfo == null)
				{
					return new ReportEnumProperty<BackgroundHatchTypes>(false, null, BackgroundHatchTypes.Default, defaultValue);
				}
				if (!attributeInfo.IsExpression)
				{
					text2 = attributeInfo.Value;
				}
				return new ReportEnumProperty<BackgroundHatchTypes>(attributeInfo.IsExpression, expressionString, StyleTranslator.TranslateBackgroundHatchType(text2, null, this.m_isDynamicImageStyle), defaultValue);
			}
			}
			return null;
		}

		private ReportProperty GetOldSnapshotReportProperty(StyleAttributeNames styleName, AspNetCore.ReportingServices.ReportRendering.Style style)
		{
			AspNetCore.ReportingServices.ReportProcessing.AttributeInfo styleDefinition = null;
			string styleStringFromEnum = this.GetStyleStringFromEnum(styleName);
			string expressionString = null;
			if (style != null && styleName != StyleAttributeNames.BackgroundImage)
			{
				styleDefinition = style.GetStyleDefinition(styleStringFromEnum, out expressionString);
			}
			return this.GetOldSnapshotReportProperty(styleDefinition, expressionString, styleName, styleStringFromEnum, style);
		}

		private ReportProperty GetOldSnapshotReportProperty(StyleAttributeNames styleName, AspNetCore.ReportingServices.ReportProcessing.Style style)
		{
			AspNetCore.ReportingServices.ReportProcessing.AttributeInfo attributeInfo = null;
			string expressionString = null;
			string styleStringFromEnum = this.GetStyleStringFromEnum(styleName);
			if (style != null)
			{
				attributeInfo = this.m_styleDef.StyleAttributes[styleStringFromEnum];
				if (attributeInfo.IsExpression)
				{
					expressionString = this.m_styleDef.ExpressionList[attributeInfo.IntValue].OriginalText;
				}
			}
			return this.GetOldSnapshotReportProperty(attributeInfo, expressionString, styleName, styleStringFromEnum, null);
		}

		private ReportProperty GetOldSnapshotReportProperty(AspNetCore.ReportingServices.ReportProcessing.AttributeInfo styleDefinition, string expressionString, StyleAttributeNames styleName, string styleNameString, AspNetCore.ReportingServices.ReportRendering.Style style)
		{
			if (!this.IsAvailableStyle(styleName))
			{
				styleDefinition = null;
			}
			switch (styleName)
			{
			case StyleAttributeNames.BackgroundImage:
			{
				if (!this.IsAvailableStyle(styleName))
				{
					break;
				}
				AspNetCore.ReportingServices.ReportRendering.BackgroundImage backgroundImage = null;
				if (style != null)
				{
					backgroundImage = (((AspNetCore.ReportingServices.ReportRendering.StyleBase)style)[styleNameString] as AspNetCore.ReportingServices.ReportRendering.BackgroundImage);
				}
				if (backgroundImage == null)
				{
					break;
				}
				return new BackgroundImage(true, expressionString, style, this);
			}
			case StyleAttributeNames.BorderColor:
			case StyleAttributeNames.BorderColorTop:
			case StyleAttributeNames.BorderColorLeft:
			case StyleAttributeNames.BorderColorRight:
			case StyleAttributeNames.BorderColorBottom:
			case StyleAttributeNames.BackgroundColor:
			case StyleAttributeNames.Color:
			case StyleAttributeNames.BackgroundGradientEndColor:
			case StyleAttributeNames.ShadowColor:
			{
				ReportColor reportColor = null;
				if (!this.m_isDynamicImageStyle || styleName != StyleAttributeNames.Color)
				{
					reportColor = (this.m_styleDefaults[styleNameString] as ReportColor);
				}
				if (styleDefinition == null)
				{
					return new ReportColorProperty(false, null, reportColor, reportColor);
				}
				return new ReportColorProperty(styleDefinition.IsExpression, expressionString, styleDefinition.IsExpression ? null : new ReportColor(styleDefinition.Value, this.m_isDynamicImageStyle), reportColor);
			}
			case StyleAttributeNames.FontFamily:
			case StyleAttributeNames.Format:
			case StyleAttributeNames.Language:
			case StyleAttributeNames.NumeralLanguage:
			case StyleAttributeNames.CurrencyLanguage:
				if (styleDefinition == null)
				{
					return new ReportStringProperty(false, null, this.m_styleDefaults[styleNameString] as string);
				}
				return new ReportStringProperty(styleDefinition.IsExpression, expressionString, styleDefinition.Value, styleDefinition.IsExpression ? (this.m_styleDefaults[styleNameString] as string) : null);
			case StyleAttributeNames.BorderStyle:
			{
				BorderStyles borderStyles = (BorderStyles)((!this.m_isLineBorderStyle) ? 1 : 4);
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<BorderStyles>(false, null, borderStyles, borderStyles);
				}
				return new ReportEnumProperty<BorderStyles>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateBorderStyle(styleDefinition.Value, null), borderStyles);
			}
			case StyleAttributeNames.BorderStyleTop:
			case StyleAttributeNames.BorderStyleLeft:
			case StyleAttributeNames.BorderStyleRight:
			case StyleAttributeNames.BorderStyleBottom:
				if (styleDefinition == null)
				{
					return null;
				}
				return new ReportEnumProperty<BorderStyles>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateBorderStyle(styleDefinition.Value, null), BorderStyles.None);
			case StyleAttributeNames.FontStyle:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<FontStyles>(Style.DefaultEnumFontStyle);
				}
				return new ReportEnumProperty<FontStyles>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateFontStyle(styleDefinition.Value, null), Style.DefaultEnumFontStyle);
			case StyleAttributeNames.FontWeight:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<FontWeights>(Style.DefaultEnumFontWeight);
				}
				return new ReportEnumProperty<FontWeights>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateFontWeight(styleDefinition.Value, null), Style.DefaultEnumFontWeight);
			case StyleAttributeNames.TextDecoration:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<TextDecorations>(Style.DefaultEnumTextDecoration);
				}
				return new ReportEnumProperty<TextDecorations>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateTextDecoration(styleDefinition.Value, null), Style.DefaultEnumTextDecoration);
			case StyleAttributeNames.TextAlign:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<TextAlignments>(Style.DefaultEnumTextAlignment);
				}
				return new ReportEnumProperty<TextAlignments>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateTextAlign(styleDefinition.Value, null), Style.DefaultEnumTextAlignment);
			case StyleAttributeNames.VerticalAlign:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<VerticalAlignments>(Style.DefaultEnumVerticalAlignment);
				}
				return new ReportEnumProperty<VerticalAlignments>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateVerticalAlign(styleDefinition.Value, null), Style.DefaultEnumVerticalAlignment);
			case StyleAttributeNames.Direction:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<Directions>(Style.DefaultEnumDirection);
				}
				return new ReportEnumProperty<Directions>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateDirection(styleDefinition.Value, null), Style.DefaultEnumDirection);
			case StyleAttributeNames.WritingMode:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<WritingModes>(Style.DefaultEnumWritingMode);
				}
				return new ReportEnumProperty<WritingModes>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateWritingMode(styleDefinition.Value, null), Style.DefaultEnumWritingMode);
			case StyleAttributeNames.UnicodeBiDi:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<UnicodeBiDiTypes>(Style.DefaultEnumUnicodeBiDiType);
				}
				return new ReportEnumProperty<UnicodeBiDiTypes>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateUnicodeBiDi(styleDefinition.Value, null), Style.DefaultEnumUnicodeBiDiType);
			case StyleAttributeNames.Calendar:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<Calendars>(Style.DefaultEnumCalendar);
				}
				return new ReportEnumProperty<Calendars>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateCalendar(styleDefinition.Value, null), Style.DefaultEnumCalendar);
			case StyleAttributeNames.BackgroundGradientType:
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<BackgroundGradients>(Style.DefaultEnumBackgroundGradient);
				}
				return new ReportEnumProperty<BackgroundGradients>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateBackgroundGradientType(styleDefinition.Value, null), Style.DefaultEnumBackgroundGradient);
			case StyleAttributeNames.BorderWidth:
			case StyleAttributeNames.FontSize:
			case StyleAttributeNames.PaddingLeft:
			case StyleAttributeNames.PaddingRight:
			case StyleAttributeNames.PaddingTop:
			case StyleAttributeNames.PaddingBottom:
			case StyleAttributeNames.LineHeight:
			case StyleAttributeNames.ShadowOffset:
				if (styleDefinition == null)
				{
					return new ReportSizeProperty(false, null, this.m_styleDefaults[styleNameString] as ReportSize);
				}
				return new ReportSizeProperty(styleDefinition.IsExpression, expressionString, styleDefinition.IsExpression ? null : new ReportSize(styleDefinition.Value, false), styleDefinition.IsExpression ? (this.m_styleDefaults[styleNameString] as ReportSize) : null);
			case StyleAttributeNames.BorderWidthTop:
			case StyleAttributeNames.BorderWidthLeft:
			case StyleAttributeNames.BorderWidthRight:
			case StyleAttributeNames.BorderWidthBottom:
				if (styleDefinition == null)
				{
					return null;
				}
				return new ReportSizeProperty(styleDefinition.IsExpression, expressionString, styleDefinition.IsExpression ? null : new ReportSize(styleDefinition.Value, false), null);
			case StyleAttributeNames.NumeralVariant:
			{
				int num = (int)this.m_styleDefaults[styleNameString];
				if (styleDefinition == null)
				{
					return new ReportIntProperty(false, null, num, num);
				}
				return new ReportIntProperty(styleDefinition.IsExpression, expressionString, styleDefinition.IntValue, num);
			}
			case StyleAttributeNames.TextEffect:
			{
				TextEffects defaultValue2 = StyleTranslator.TranslateTextEffect(null, null, this.m_isDynamicImageStyle);
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<TextEffects>(false, null, TextEffects.Default, defaultValue2);
				}
				return new ReportEnumProperty<TextEffects>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateTextEffect(styleDefinition.Value, null, this.m_isDynamicImageStyle), defaultValue2);
			}
			case StyleAttributeNames.BackgroundHatchType:
			{
				BackgroundHatchTypes defaultValue = StyleTranslator.TranslateBackgroundHatchType(null, null, this.m_isDynamicImageStyle);
				if (styleDefinition == null)
				{
					return new ReportEnumProperty<BackgroundHatchTypes>(false, null, BackgroundHatchTypes.Default, defaultValue);
				}
				return new ReportEnumProperty<BackgroundHatchTypes>(styleDefinition.IsExpression, expressionString, StyleTranslator.TranslateBackgroundHatchType(styleDefinition.Value, null, this.m_isDynamicImageStyle), defaultValue);
			}
			}
			return null;
		}

		protected virtual bool IsAvailableStyle(StyleAttributeNames styleName)
		{
			return true;
		}
	}
}
