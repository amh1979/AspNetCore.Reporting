using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class Style : IPersistable
	{
		internal enum StyleId
		{
			BorderColor,
			BorderColorTop,
			BorderColorLeft,
			BorderColorRight,
			BorderColorBottom,
			BorderStyle,
			BorderStyleTop,
			BorderStyleLeft,
			BorderStyleRight,
			BorderStyleBottom,
			BorderWidth,
			BorderWidthTop,
			BorderWidthLeft,
			BorderWidthRight,
			BorderWidthBottom,
			BackgroundColor,
			FontStyle,
			FontFamily,
			FontSize,
			FontWeight,
			Format,
			TextDecoration,
			TextAlign,
			VerticalAlign,
			Color,
			PaddingLeft,
			PaddingRight,
			PaddingTop,
			PaddingBottom,
			LineHeight,
			Direction,
			WritingMode,
			Language,
			UnicodeBiDi,
			Calendar,
			NumeralLanguage,
			NumeralVariant,
			BackgroundGradientType,
			BackgroundGradientEndColor,
			BackgroundHatchType,
			TransparentColor,
			ShadowColor,
			ShadowOffset,
			Position,
			TextEffect,
			BackgroundImage,
			BackgroundImageRepeat,
			BackgroundImageSource,
			BackgroundImageValue,
			BackgroundImageMimeType,
			CurrencyLanguage
		}

		private sealed class EmptyStyleExprHost : StyleExprHost
		{
			internal static EmptyStyleExprHost Instance = new EmptyStyleExprHost();

			private EmptyStyleExprHost()
			{
			}
		}

		protected Dictionary<string, AttributeInfo> m_styleAttributes;

		protected List<ExpressionInfo> m_expressionList;

		[NonSerialized]
		private StyleExprHost m_exprHost;

		[NonSerialized]
		private int m_customSharedStyleCount = -1;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Style.GetDeclaration();

		internal Dictionary<string, AttributeInfo> StyleAttributes
		{
			get
			{
				return this.m_styleAttributes;
			}
			set
			{
				this.m_styleAttributes = value;
			}
		}

		internal List<ExpressionInfo> ExpressionList
		{
			get
			{
				return this.m_expressionList;
			}
			set
			{
				this.m_expressionList = value;
			}
		}

		internal StyleExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int CustomSharedStyleCount
		{
			get
			{
				return this.m_customSharedStyleCount;
			}
			set
			{
				this.m_customSharedStyleCount = value;
			}
		}

		internal static string GetStyleString(StyleId styleId)
		{
			switch (styleId)
			{
			case StyleId.BorderColor:
				return "BorderColor";
			case StyleId.BorderColorTop:
				return "BorderColorTop";
			case StyleId.BorderColorLeft:
				return "BorderColorLeft";
			case StyleId.BorderColorRight:
				return "BorderColorRight";
			case StyleId.BorderColorBottom:
				return "BorderColorBottom";
			case StyleId.BorderStyle:
				return "BorderStyle";
			case StyleId.BorderStyleTop:
				return "BorderStyleTop";
			case StyleId.BorderStyleLeft:
				return "BorderStyleLeft";
			case StyleId.BorderStyleRight:
				return "BorderStyleRight";
			case StyleId.BorderStyleBottom:
				return "BorderStyleBottom";
			case StyleId.BorderWidth:
				return "BorderWidth";
			case StyleId.BorderWidthTop:
				return "BorderWidthTop";
			case StyleId.BorderWidthLeft:
				return "BorderWidthLeft";
			case StyleId.BorderWidthRight:
				return "BorderWidthRight";
			case StyleId.BorderWidthBottom:
				return "BorderWidthBottom";
			case StyleId.BackgroundColor:
				return "BackgroundColor";
			case StyleId.FontStyle:
				return "FontStyle";
			case StyleId.FontFamily:
				return "FontFamily";
			case StyleId.FontSize:
				return "FontSize";
			case StyleId.FontWeight:
				return "FontWeight";
			case StyleId.Format:
				return "Format";
			case StyleId.TextDecoration:
				return "TextDecoration";
			case StyleId.TextAlign:
				return "TextAlign";
			case StyleId.VerticalAlign:
				return "VerticalAlign";
			case StyleId.Color:
				return "Color";
			case StyleId.PaddingLeft:
				return "PaddingLeft";
			case StyleId.PaddingRight:
				return "PaddingRight";
			case StyleId.PaddingTop:
				return "PaddingTop";
			case StyleId.PaddingBottom:
				return "PaddingBottom";
			case StyleId.LineHeight:
				return "LineHeight";
			case StyleId.Direction:
				return "Direction";
			case StyleId.WritingMode:
				return "WritingMode";
			case StyleId.Language:
				return "Language";
			case StyleId.UnicodeBiDi:
				return "UnicodeBiDi";
			case StyleId.Calendar:
				return "Calendar";
			case StyleId.CurrencyLanguage:
				return "CurrencyLanguage";
			case StyleId.NumeralLanguage:
				return "NumeralLanguage";
			case StyleId.NumeralVariant:
				return "NumeralVariant";
			case StyleId.BackgroundGradientType:
				return "BackgroundGradientType";
			case StyleId.BackgroundGradientEndColor:
				return "BackgroundGradientEndColor";
			case StyleId.BackgroundImage:
				return "BackgroundImage";
			case StyleId.BackgroundImageRepeat:
				return "BackgroundRepeat";
			case StyleId.BackgroundImageSource:
				return "BackgroundImageSource";
			case StyleId.BackgroundImageValue:
				return "BackgroundImageValue";
			case StyleId.BackgroundImageMimeType:
				return "BackgroundImageMIMEType";
			case StyleId.Position:
				return "Position";
			case StyleId.TransparentColor:
				return "TransparentColor";
			case StyleId.TextEffect:
				return "TextEffect";
			case StyleId.ShadowColor:
				return "ShadowColor";
			case StyleId.ShadowOffset:
				return "ShadowOffset";
			case StyleId.BackgroundHatchType:
				return "BackgroundHatchType";
			default:
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal static StyleId GetStyleId(string styleString)
		{
			switch (styleString)
			{
			case "BorderColor":
				return StyleId.BorderColor;
			case "BorderColorTop":
				return StyleId.BorderColorTop;
			case "BorderColorLeft":
				return StyleId.BorderColorLeft;
			case "BorderColorRight":
				return StyleId.BorderColorRight;
			case "BorderColorBottom":
				return StyleId.BorderColorBottom;
			case "BorderStyle":
				return StyleId.BorderStyle;
			case "BorderStyleTop":
				return StyleId.BorderStyleTop;
			case "BorderStyleLeft":
				return StyleId.BorderStyleLeft;
			case "BorderStyleRight":
				return StyleId.BorderStyleRight;
			case "BorderStyleBottom":
				return StyleId.BorderStyleBottom;
			case "BorderWidth":
				return StyleId.BorderWidth;
			case "BorderWidthTop":
				return StyleId.BorderWidthTop;
			case "BorderWidthLeft":
				return StyleId.BorderWidthLeft;
			case "BorderWidthRight":
				return StyleId.BorderWidthRight;
			case "BorderWidthBottom":
				return StyleId.BorderWidthBottom;
			case "BackgroundColor":
				return StyleId.BackgroundColor;
			case "FontStyle":
				return StyleId.FontStyle;
			case "FontFamily":
				return StyleId.FontFamily;
			case "FontSize":
				return StyleId.FontSize;
			case "FontWeight":
				return StyleId.FontWeight;
			case "Format":
				return StyleId.Format;
			case "TextDecoration":
				return StyleId.TextDecoration;
			case "TextAlign":
				return StyleId.TextAlign;
			case "VerticalAlign":
				return StyleId.VerticalAlign;
			case "Color":
				return StyleId.Color;
			case "PaddingLeft":
				return StyleId.PaddingLeft;
			case "PaddingRight":
				return StyleId.PaddingRight;
			case "PaddingTop":
				return StyleId.PaddingTop;
			case "PaddingBottom":
				return StyleId.PaddingBottom;
			case "LineHeight":
				return StyleId.LineHeight;
			case "Direction":
				return StyleId.Direction;
			case "WritingMode":
				return StyleId.WritingMode;
			case "Language":
				return StyleId.Language;
			case "UnicodeBiDi":
				return StyleId.UnicodeBiDi;
			case "Calendar":
				return StyleId.Calendar;
			case "CurrencyLanguage":
				return StyleId.CurrencyLanguage;
			case "NumeralLanguage":
				return StyleId.NumeralLanguage;
			case "NumeralVariant":
				return StyleId.NumeralVariant;
			case "BackgroundGradientType":
				return StyleId.BackgroundGradientType;
			case "BackgroundGradientEndColor":
				return StyleId.BackgroundGradientEndColor;
			case "BackgroundImage":
				return StyleId.BackgroundImage;
			case "BackgroundRepeat":
				return StyleId.BackgroundImageRepeat;
			case "BackgroundImageSource":
				return StyleId.BackgroundImageSource;
			case "BackgroundImageValue":
				return StyleId.BackgroundImageValue;
			case "BackgroundImageMIMEType":
				return StyleId.BackgroundImageMimeType;
			case "TextEffect":
				return StyleId.TextEffect;
			case "ShadowColor":
				return StyleId.ShadowColor;
			case "ShadowOffset":
				return StyleId.ShadowOffset;
			case "BackgroundHatchType":
				return StyleId.BackgroundHatchType;
			default:
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal Style(ConstructionPhase phase)
		{
			if (phase == ConstructionPhase.Publishing)
			{
				this.m_styleAttributes = new Dictionary<string, AttributeInfo>();
			}
		}

		internal void SetStyleExprHost(StyleExprHost exprHost)
		{
			Global.Tracer.Assert(null != exprHost, "(null != exprHost)");
			this.m_exprHost = exprHost;
		}

		internal int GetStyleAttribute(AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string styleAttributeName, OnDemandProcessingContext context, ref bool sharedFormatSettings, out string styleStringValue)
		{
			styleStringValue = null;
			int result = 0;
			object obj = null;
			AttributeInfo attributeInfo = null;
			if (this.GetAttributeInfo(styleAttributeName, out attributeInfo))
			{
				if (attributeInfo.IsExpression)
				{
					result = 1;
					sharedFormatSettings = false;
					obj = this.EvaluateStyle(objectType, objectName, styleAttributeName, context);
				}
				else
				{
					result = 2;
					obj = attributeInfo.Value;
				}
			}
			if (obj != null)
			{
				styleStringValue = (string)obj;
			}
			return result;
		}

		internal void GetStyleAttribute(AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string styleAttributeName, OnDemandProcessingContext context, ref bool sharedFormatSettings, out int styleIntValue)
		{
			styleIntValue = 0;
			AttributeInfo attributeInfo = null;
			if (this.GetAttributeInfo(styleAttributeName, out attributeInfo))
			{
				if (attributeInfo.IsExpression)
				{
					sharedFormatSettings = false;
					object obj = this.EvaluateStyle(objectType, objectName, styleAttributeName, context);
					if (obj != null)
					{
						styleIntValue = (int)obj;
					}
				}
				else
				{
					styleIntValue = attributeInfo.IntValue;
				}
			}
		}

		internal virtual bool GetAttributeInfo(string styleAttributeName, out AttributeInfo styleAttribute)
		{
			if (this.m_styleAttributes.TryGetValue(styleAttributeName, out styleAttribute) && styleAttribute != null)
			{
				return true;
			}
			return false;
		}

		internal object EvaluateStyle(AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, StyleId styleId, OnDemandProcessingContext context)
		{
			AttributeInfo attribute = null;
			if (this.GetAttributeInfo(Style.GetStyleString(styleId), out attribute))
			{
				return this.EvaluateStyle(objectType, objectName, attribute, styleId, context);
			}
			return null;
		}

		internal object EvaluateStyle(AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string styleAttributeName, OnDemandProcessingContext context)
		{
			AttributeInfo attribute = null;
			if (this.GetAttributeInfo(styleAttributeName, out attribute))
			{
				StyleId styleId = Style.GetStyleId(styleAttributeName);
				return this.EvaluateStyle(objectType, objectName, attribute, styleId, context);
			}
			return null;
		}

		internal object EvaluateStyle(AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, AttributeInfo attribute, StyleId styleId, OnDemandProcessingContext context)
		{
			if (attribute != null)
			{
				if (!attribute.IsExpression)
				{
					if (StyleId.NumeralLanguage != styleId && StyleId.BackgroundImageSource != styleId)
					{
						if (attribute.Value != null && attribute.Value.Length != 0)
						{
							return attribute.Value;
						}
						return null;
					}
					return attribute.IntValue;
				}
				switch (styleId)
				{
				case StyleId.BackgroundImageSource:
					return null;
				case StyleId.BackgroundImageValue:
				{
					AttributeInfo attributeInfo = this.m_styleAttributes["BackgroundImageSource"];
					if (attributeInfo == null)
					{
						return null;
					}
					switch (attributeInfo.IntValue)
					{
					case 2:
						return context.ReportRuntime.EvaluateStyleBackgroundDatabaseImageValue(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
					case 1:
						return context.ReportRuntime.EvaluateStyleBackgroundEmbeddedImageValue(this, this.m_expressionList[attribute.IntValue], context.EmbeddedImages, objectType, objectName);
					case 0:
						return context.ReportRuntime.EvaluateStyleBackgroundUrlImageValue(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
					}
					break;
				}
				case StyleId.BackgroundImageMimeType:
					return context.ReportRuntime.EvaluateStyleBackgroundImageMIMEType(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BackgroundImageRepeat:
					return context.ReportRuntime.EvaluateStyleBackgroundRepeat(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BackgroundColor:
					return context.ReportRuntime.EvaluateStyleBackgroundColor(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BackgroundGradientType:
					return context.ReportRuntime.EvaluateStyleBackgroundGradientType(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BackgroundGradientEndColor:
					return context.ReportRuntime.EvaluateStyleBackgroundGradientEndColor(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Color:
					return context.ReportRuntime.EvaluateStyleColor(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.FontSize:
					return context.ReportRuntime.EvaluateStyleFontSize(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.FontStyle:
					return context.ReportRuntime.EvaluateStyleFontStyle(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.FontFamily:
					return context.ReportRuntime.EvaluateStyleFontFamily(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.FontWeight:
					return context.ReportRuntime.EvaluateStyleFontWeight(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Format:
					return context.ReportRuntime.EvaluateStyleFormat(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.TextDecoration:
					return context.ReportRuntime.EvaluateStyleTextDecoration(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.TextAlign:
					return context.ReportRuntime.EvaluateStyleTextAlign(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.VerticalAlign:
					return context.ReportRuntime.EvaluateStyleVerticalAlign(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Direction:
					return context.ReportRuntime.EvaluateStyleDirection(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.WritingMode:
					return context.ReportRuntime.EvaluateStyleWritingMode(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Language:
					return context.ReportRuntime.EvaluateStyleLanguage(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.UnicodeBiDi:
					return context.ReportRuntime.EvaluateStyleUnicodeBiDi(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Calendar:
					return context.ReportRuntime.EvaluateStyleCalendar(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.CurrencyLanguage:
					return context.ReportRuntime.EvaluateStyleCurrencyLanguage(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.NumeralLanguage:
					return context.ReportRuntime.EvaluateStyleNumeralLanguage(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.LineHeight:
					return context.ReportRuntime.EvaluateStyleLineHeight(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.NumeralVariant:
					return context.ReportRuntime.EvaluateStyleNumeralVariant(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderColor:
					return context.ReportRuntime.EvaluateStyleBorderColor(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderColorBottom:
					return context.ReportRuntime.EvaluateStyleBorderColorBottom(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderColorLeft:
					return context.ReportRuntime.EvaluateStyleBorderColorLeft(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderColorRight:
					return context.ReportRuntime.EvaluateStyleBorderColorRight(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderColorTop:
					return context.ReportRuntime.EvaluateStyleBorderColorTop(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderStyle:
					return context.ReportRuntime.EvaluateStyleBorderStyle(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderStyleTop:
					return context.ReportRuntime.EvaluateStyleBorderStyleTop(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderStyleLeft:
					return context.ReportRuntime.EvaluateStyleBorderStyleLeft(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderStyleRight:
					return context.ReportRuntime.EvaluateStyleBorderStyleRight(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderStyleBottom:
					return context.ReportRuntime.EvaluateStyleBorderStyleBottom(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderWidth:
					return context.ReportRuntime.EvaluateStyleBorderWidth(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderWidthTop:
					return context.ReportRuntime.EvaluateStyleBorderWidthTop(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderWidthLeft:
					return context.ReportRuntime.EvaluateStyleBorderWidthLeft(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderWidthRight:
					return context.ReportRuntime.EvaluateStyleBorderWidthRight(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderWidthBottom:
					return context.ReportRuntime.EvaluateStyleBorderWidthBottom(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.PaddingLeft:
					return context.ReportRuntime.EvaluateStylePaddingLeft(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.PaddingRight:
					return context.ReportRuntime.EvaluateStylePaddingRight(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.PaddingTop:
					return context.ReportRuntime.EvaluateStylePaddingTop(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.PaddingBottom:
					return context.ReportRuntime.EvaluateStylePaddingBottom(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.TextEffect:
					return context.ReportRuntime.EvaluateStyleTextEffect(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.ShadowColor:
					return context.ReportRuntime.EvaluateStyleShadowColor(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.ShadowOffset:
					return context.ReportRuntime.EvaluateStyleShadowOffset(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BackgroundHatchType:
					return context.ReportRuntime.EvaluateStyleBackgroundHatchType(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Position:
					return context.ReportRuntime.EvaluatePosition(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.TransparentColor:
					return context.ReportRuntime.EvaluateTransparentColor(this, this.m_expressionList[attribute.IntValue], objectType, objectName);
				}
			}
			return null;
		}

		internal void AddAttribute(string name, ExpressionInfo expressionInfo)
		{
			this.AddAttribute(name, expressionInfo, ValueType.Constant);
		}

		internal void AddAttribute(StyleInformation.StyleInformationAttribute attribute)
		{
			this.AddAttribute(attribute.Name, attribute.Value, attribute.ValueType);
		}

		internal void AddAttribute(string name, ExpressionInfo expressionInfo, ValueType valueType)
		{
			AttributeInfo attributeInfo = new AttributeInfo();
			attributeInfo.ValueType = valueType;
			attributeInfo.IsExpression = (ExpressionInfo.Types.Constant != expressionInfo.Type);
			if (attributeInfo.IsExpression)
			{
				if (this.m_expressionList == null)
				{
					this.m_expressionList = new List<ExpressionInfo>();
				}
				this.m_expressionList.Add(expressionInfo);
				attributeInfo.IntValue = this.m_expressionList.Count - 1;
			}
			else
			{
				attributeInfo.Value = expressionInfo.StringValue;
				attributeInfo.BoolValue = expressionInfo.BoolValue;
				attributeInfo.IntValue = expressionInfo.IntValue;
				attributeInfo.FloatValue = expressionInfo.FloatValue;
			}
			Global.Tracer.Assert(null != this.m_styleAttributes, "(null != m_styleAttributes)");
			this.m_styleAttributes.Add(name, attributeInfo);
		}

		internal void Initialize(InitializationContext context)
		{
			Global.Tracer.Assert(null != this.m_styleAttributes, "(null != m_styleAttributes)");
			IDictionaryEnumerator dictionaryEnumerator = (IDictionaryEnumerator)(object)this.m_styleAttributes.GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				string text = (string)dictionaryEnumerator.Key;
				AttributeInfo attributeInfo = (AttributeInfo)dictionaryEnumerator.Value;
				Global.Tracer.Assert(null != text, "(null != name)");
				Global.Tracer.Assert(null != attributeInfo, "(null != attribute)");
				if (attributeInfo.IsExpression)
				{
					string name = text;
					switch (text)
					{
					case "BorderColorLeft":
					case "BorderColorRight":
					case "BorderColorTop":
					case "BorderColorBottom":
						text = "BorderColor";
						break;
					case "BorderStyleLeft":
					case "BorderStyleRight":
					case "BorderStyleTop":
					case "BorderStyleBottom":
						text = "BorderStyle";
						break;
					case "BorderWidthLeft":
					case "BorderWidthRight":
					case "BorderWidthTop":
					case "BorderWidthBottom":
						text = "BorderWidth";
						break;
					}
					Global.Tracer.Assert(null != this.m_expressionList, "(null != m_expressionList)");
					ExpressionInfo expressionInfo = this.m_expressionList[attributeInfo.IntValue];
					expressionInfo.Initialize(text, context);
					context.ExprHostBuilder.StyleAttribute(name, expressionInfo);
				}
			}
			AttributeInfo attributeInfo2 = default(AttributeInfo);
			this.m_styleAttributes.TryGetValue("BackgroundImageSource", out attributeInfo2);
			if (attributeInfo2 != null)
			{
				Global.Tracer.Assert(!attributeInfo2.IsExpression, "(!source.IsExpression)");
				AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType intValue = (AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType)attributeInfo2.IntValue;
				AttributeInfo attributeInfo3 = default(AttributeInfo);
				if (AspNetCore.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded == intValue && (!this.m_styleAttributes.TryGetValue("EmbeddingMode", out attributeInfo3) || attributeInfo3.IntValue != 1))
				{
					AttributeInfo attributeInfo4 = this.m_styleAttributes["BackgroundImageValue"];
					Global.Tracer.Assert(null != attributeInfo4, "(null != embeddedImageName)");
					AspNetCore.ReportingServices.ReportPublishing.PublishingValidator.ValidateEmbeddedImageName(attributeInfo4, context.EmbeddedImages, context.ObjectType, context.ObjectName, "BackgroundImageValue", context.ErrorContext);
				}
			}
			context.CheckInternationalSettings(this.m_styleAttributes);
		}

		public object PublishClone(AutomaticSubtotalContext context)
		{
			Style style = (Style)base.MemberwiseClone();
			if (this.m_styleAttributes != null)
			{
				style.m_styleAttributes = new Dictionary<string, AttributeInfo>(this.m_styleAttributes.Count);
				foreach (KeyValuePair<string, AttributeInfo> styleAttribute in this.m_styleAttributes)
				{
					style.m_styleAttributes.Add(styleAttribute.Key, styleAttribute.Value.PublishClone(context));
				}
			}
			if (this.m_expressionList != null)
			{
				style.m_expressionList = new List<ExpressionInfo>(this.m_expressionList.Count);
				{
					foreach (ExpressionInfo expression in this.m_expressionList)
					{
						style.m_expressionList.Add((ExpressionInfo)expression.PublishClone(context));
					}
					return style;
				}
			}
			return style;
		}

		internal void InitializeForCRIGeneratedReportItem()
		{
			this.SetStyleExprHost(EmptyStyleExprHost.Instance);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StyleAttributes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AttributeInfo));
			list.Add(new MemberInfo(MemberName.ExpressionList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Style.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StyleAttributes:
					writer.WriteStringRIFObjectDictionary(this.m_styleAttributes);
					break;
				case MemberName.ExpressionList:
					writer.Write(this.m_expressionList);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Style.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.StyleAttributes:
					this.m_styleAttributes = reader.ReadStringRIFObjectDictionary<AttributeInfo>();
					break;
				case MemberName.ExpressionList:
					this.m_expressionList = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style;
		}
	}
}
