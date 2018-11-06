using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class Style : StyleBase
	{
		internal enum StyleName
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
			NumeralVariant
		}

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

			internal StyleDefaults(bool isLine)
			{
				this.m_nameMap = new Hashtable(42);
				this.m_keyCollection = new string[42];
				this.m_valueCollection = new object[42];
				int num = 0;
				this.m_nameMap["BorderColor"] = num;
				this.m_keyCollection[num] = "BorderColor";
				this.m_valueCollection[num++] = new ReportColor("Black", false);
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
				this.m_valueCollection[num++] = new ReportSize("1pt", false);
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
				this.m_valueCollection[num++] = new ReportColor("Transparent", false);
				this.m_nameMap["BackgroundGradientType"] = num;
				this.m_keyCollection[num] = "BackgroundGradientType";
				this.m_valueCollection[num++] = "None";
				this.m_nameMap["BackgroundGradientEndColor"] = num;
				this.m_keyCollection[num] = "BackgroundGradientEndColor";
				this.m_valueCollection[num++] = new ReportColor("Transparent", false);
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
				this.m_valueCollection[num++] = "Arial";
				this.m_nameMap["FontSize"] = num;
				this.m_keyCollection[num] = "FontSize";
				this.m_valueCollection[num++] = new ReportSize("10pt", false);
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
				this.m_valueCollection[num++] = new ReportColor("Black", false);
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
				this.m_nameMap["CurrencyLanguage"] = num;
				this.m_keyCollection[num] = "CurrencyLanguage";
				this.m_valueCollection[num++] = null;
				Global.Tracer.Assert(42 == num);
			}

			internal string GetName(int index)
			{
				return this.m_keyCollection[index];
			}
		}

		private ReportItem m_reportItem;

		private AspNetCore.ReportingServices.ReportProcessing.ReportItem m_reportItemDef;

		private StyleDefaults m_styleDefaults;

		private static StyleDefaults NormalStyleDefaults = new StyleDefaults(false);

		private static StyleDefaults LineStyleDefaults = new StyleDefaults(true);

		public override int Count
		{
			get
			{
				if (base.IsCustomControl)
				{
					return base.Count;
				}
				if (this.m_reportItemDef.StyleClass == null)
				{
					return 0;
				}
				return base.Count;
			}
		}

		public override ICollection Keys
		{
			get
			{
				if (base.IsCustomControl)
				{
					return base.Keys;
				}
				if (this.m_reportItemDef.StyleClass == null)
				{
					return null;
				}
				return base.Keys;
			}
		}

		public override object this[string styleName]
		{
			get
			{
				if (base.IsCustomControl)
				{
					object obj = null;
					if (base.m_nonSharedProperties != null)
					{
						obj = base.m_nonSharedProperties[styleName];
					}
					if (obj == null && base.m_sharedProperties != null)
					{
						obj = base.m_sharedProperties[styleName];
					}
					return this.CreatePropertyOrReturnDefault(styleName, obj);
				}
				Global.Tracer.Assert(!base.IsCustomControl);
				if (this.m_reportItem.HeadingInstance == null && this.m_reportItemDef.StyleClass == null)
				{
					return this.m_styleDefaults[styleName];
				}
				StyleAttributeHashtable styleAttributeHashtable = null;
				if (this.m_reportItemDef.StyleClass != null)
				{
					styleAttributeHashtable = this.m_reportItemDef.StyleClass.StyleAttributes;
				}
				StyleAttributeHashtable styleAttributeHashtable2 = null;
				if (this.m_reportItem.HeadingInstance != null)
				{
					Global.Tracer.Assert(null != this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass);
					styleAttributeHashtable2 = this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.StyleAttributes;
				}
				AttributeInfo attributeInfo = null;
				if ("BackgroundImage" == styleName)
				{
					Image.SourceType imageSource = Image.SourceType.External;
					object obj2 = null;
					object obj3 = null;
					bool flag = false;
					bool flag2 = default(bool);
					if (styleAttributeHashtable2 != null)
					{
						base.GetBackgroundImageProperties(styleAttributeHashtable2["BackgroundImageSource"], styleAttributeHashtable2["BackgroundImageValue"], styleAttributeHashtable2["BackgroundImageMIMEType"], out imageSource, out obj2, out flag2, out obj3, out flag);
					}
					if (obj2 == null && styleAttributeHashtable != null)
					{
						base.GetBackgroundImageProperties(styleAttributeHashtable["BackgroundImageSource"], styleAttributeHashtable["BackgroundImageValue"], styleAttributeHashtable["BackgroundImageMIMEType"], out imageSource, out obj2, out flag2, out obj3, out flag);
					}
					if (obj2 != null)
					{
						string mimeType = null;
						if (!flag)
						{
							mimeType = (string)obj3;
						}
						return new BackgroundImage(base.m_renderingContext, imageSource, obj2, mimeType);
					}
				}
				else
				{
					if (styleAttributeHashtable2 != null)
					{
						AttributeInfo attributeInfo2 = styleAttributeHashtable2[styleName];
						if (attributeInfo2 != null)
						{
							return this.CreatePropertyOrReturnDefault(styleName, this.GetStyleAttributeValue(styleName, attributeInfo2));
						}
					}
					if (styleAttributeHashtable != null)
					{
						attributeInfo = styleAttributeHashtable[styleName];
						if (attributeInfo != null)
						{
							return this.CreatePropertyOrReturnDefault(styleName, this.GetStyleAttributeValue(styleName, attributeInfo));
						}
					}
				}
				return this.m_styleDefaults[styleName];
			}
		}

		public override StyleProperties SharedProperties
		{
			get
			{
				if (base.IsCustomControl)
				{
					return base.m_sharedProperties;
				}
				if (this.NeedPopulateSharedProps())
				{
					this.PopulateStyleProperties(false);
					this.m_reportItem.ReportItemDef.SharedStyleProperties = base.m_sharedProperties;
				}
				return base.m_sharedProperties;
			}
		}

		public override StyleProperties NonSharedProperties
		{
			get
			{
				if (base.IsCustomControl)
				{
					return base.m_nonSharedProperties;
				}
				if (this.NeedPopulateNonSharedProps())
				{
					this.PopulateNonSharedStyleProperties();
					if (base.m_nonSharedProperties == null || base.m_nonSharedProperties.Count == 0)
					{
						this.m_reportItemDef.NoNonSharedStyleProps = true;
					}
				}
				return base.m_nonSharedProperties;
			}
		}

		public Style()
		{
			Global.Tracer.Assert(base.IsCustomControl);
			this.m_styleDefaults = Style.NormalStyleDefaults;
		}

		internal Style(ReportItem reportItem, AspNetCore.ReportingServices.ReportProcessing.ReportItem reportItemDef, RenderingContext context)
			: base(context)
		{
			Global.Tracer.Assert(!base.IsCustomControl);
			this.m_reportItem = reportItem;
			this.m_reportItemDef = reportItemDef;
			if (reportItem is Line)
			{
				this.m_styleDefaults = Style.LineStyleDefaults;
			}
			else
			{
				this.m_styleDefaults = Style.NormalStyleDefaults;
			}
		}

		internal bool HasBackgroundImage(out bool isExpressionBased)
		{
			isExpressionBased = false;
			if (this.m_reportItem.HeadingInstance == null && this.m_reportItemDef.StyleClass == null)
			{
				return false;
			}
			if (this.GetStyleDefinition("BackgroundImageValue") == null)
			{
				return false;
			}
			StyleAttributeHashtable styleAttributeHashtable = null;
			if (this.m_reportItemDef.StyleClass != null)
			{
				styleAttributeHashtable = this.m_reportItemDef.StyleClass.StyleAttributes;
			}
			StyleAttributeHashtable styleAttributeHashtable2 = null;
			if (this.m_reportItem.HeadingInstance != null)
			{
				Global.Tracer.Assert(null != this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass);
				styleAttributeHashtable2 = this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.StyleAttributes;
			}
			Image.SourceType sourceType = Image.SourceType.External;
			object obj = null;
			object obj2 = null;
			object obj3 = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (styleAttributeHashtable2 != null)
			{
				base.GetBackgroundImageProperties(styleAttributeHashtable2["BackgroundImageSource"], styleAttributeHashtable2["BackgroundImageValue"], styleAttributeHashtable2["BackgroundImageMIMEType"], styleAttributeHashtable2["BackgroundRepeat"], out sourceType, out obj, out flag, out obj2, out flag2, out obj3, out flag3);
			}
			if (obj == null && styleAttributeHashtable != null)
			{
				base.GetBackgroundImageProperties(styleAttributeHashtable["BackgroundImageSource"], styleAttributeHashtable["BackgroundImageValue"], styleAttributeHashtable["BackgroundImageMIMEType"], styleAttributeHashtable["BackgroundRepeat"], out sourceType, out obj, out flag, out obj2, out flag2, out obj3, out flag3);
			}
			if (obj != null)
			{
				isExpressionBased = (flag || flag2 || flag3);
				return true;
			}
			return false;
		}

		internal AttributeInfo GetStyleDefinition(string styleName)
		{
			string text = null;
			return this.GetStyleDefinition(styleName, out text);
		}

		internal AttributeInfo GetStyleDefinition(string styleName, out string expressionString)
		{
			expressionString = null;
			if (base.IsCustomControl)
			{
				return null;
			}
			if (this.m_reportItem.HeadingInstance == null && this.m_reportItemDef.StyleClass == null)
			{
				return null;
			}
			StyleAttributeHashtable styleAttributeHashtable = null;
			ExpressionInfoList expressionInfoList = null;
			if (this.m_reportItem.HeadingInstance != null)
			{
				Global.Tracer.Assert(null != this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass);
				styleAttributeHashtable = this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.StyleAttributes;
				expressionInfoList = this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.ExpressionList;
			}
			AttributeInfo attributeInfo = null;
			if ((styleAttributeHashtable == null || !styleAttributeHashtable.ContainsKey(styleName)) && this.m_reportItemDef.StyleClass != null)
			{
				styleAttributeHashtable = this.m_reportItemDef.StyleClass.StyleAttributes;
				expressionInfoList = this.m_reportItemDef.StyleClass.ExpressionList;
			}
			if (styleAttributeHashtable != null)
			{
				attributeInfo = styleAttributeHashtable[styleName];
				if (attributeInfo != null && attributeInfo.IsExpression)
				{
					expressionString = expressionInfoList[attributeInfo.IntValue].OriginalText;
				}
			}
			return attributeInfo;
		}

		private bool NeedPopulateSharedProps()
		{
			if (base.IsCustomControl)
			{
				return false;
			}
			if (this.m_reportItem.HeadingInstance != null)
			{
				return true;
			}
			if (base.m_sharedProperties != null)
			{
				return false;
			}
			if (this.m_reportItemDef.SharedStyleProperties != null)
			{
				if (42 != this.m_reportItemDef.SharedStyleProperties.Count + ((base.m_nonSharedProperties != null) ? base.m_nonSharedProperties.Count : 0))
				{
					return true;
				}
				base.m_sharedProperties = this.m_reportItemDef.SharedStyleProperties;
				return false;
			}
			return true;
		}

		private bool NeedPopulateNonSharedProps()
		{
			if (base.IsCustomControl)
			{
				return false;
			}
			if (this.m_reportItem.HeadingInstance != null)
			{
				return true;
			}
			if (base.m_nonSharedProperties == null && !this.m_reportItemDef.NoNonSharedStyleProps)
			{
				return true;
			}
			return false;
		}

		internal static object GetStyleValue(string styleName, AspNetCore.ReportingServices.ReportProcessing.Style styleDef, object[] styleAttributeValues)
		{
			return Style.GetStyleValue(styleName, styleDef, styleAttributeValues, true);
		}

		internal static object GetStyleValue(string styleName, AspNetCore.ReportingServices.ReportProcessing.Style styleDef, object[] styleAttributeValues, bool returnDefaultStyle)
		{
			object styleValueBase = StyleBase.GetStyleValueBase(styleName, styleDef, styleAttributeValues);
			if (styleValueBase != null)
			{
				return styleValueBase;
			}
			if (returnDefaultStyle)
			{
				return Style.NormalStyleDefaults[styleName];
			}
			return null;
		}

		internal override object GetStyleAttributeValue(string styleName, AttributeInfo attribute)
		{
			if (this.m_reportItem.HeadingInstance != null)
			{
				AspNetCore.ReportingServices.ReportProcessing.Style styleClass = this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass;
				Global.Tracer.Assert(null != styleClass);
				AttributeInfo attributeInfo = styleClass.StyleAttributes[styleName];
				if (attributeInfo != null)
				{
					if (attributeInfo.IsExpression)
					{
						MatrixSubtotalHeadingInstanceInfo matrixSubtotalHeadingInstanceInfo = this.m_reportItem.HeadingInstance.GetInstanceInfo(this.m_reportItem.RenderingContext.ChunkManager) as MatrixSubtotalHeadingInstanceInfo;
						Global.Tracer.Assert(null != matrixSubtotalHeadingInstanceInfo);
						Global.Tracer.Assert(null != matrixSubtotalHeadingInstanceInfo.StyleAttributeValues);
						Global.Tracer.Assert(0 <= attributeInfo.IntValue && attributeInfo.IntValue < matrixSubtotalHeadingInstanceInfo.StyleAttributeValues.Length);
						return matrixSubtotalHeadingInstanceInfo.StyleAttributeValues[attributeInfo.IntValue];
					}
					if ("NumeralVariant" == styleName)
					{
						return attributeInfo.IntValue;
					}
					return attributeInfo.Value;
				}
			}
			if (attribute.IsExpression)
			{
				ReportItemInstanceInfo instanceInfo = this.m_reportItem.InstanceInfo;
				if (instanceInfo != null)
				{
					return instanceInfo.GetStyleAttributeValue(attribute.IntValue);
				}
				return null;
			}
			if ("NumeralVariant" == styleName)
			{
				return attribute.IntValue;
			}
			return attribute.Value;
		}

		internal override void PopulateStyleProperties(bool populateAll)
		{
			if (!base.IsCustomControl)
			{
				bool flag = true;
				bool flag2 = false;
				if (populateAll)
				{
					flag = this.NeedPopulateSharedProps();
					flag2 = this.NeedPopulateNonSharedProps();
					if (!flag && !flag2)
					{
						return;
					}
				}
				AspNetCore.ReportingServices.ReportProcessing.Style styleClass = this.m_reportItemDef.StyleClass;
				StyleAttributeHashtable styleAttributeHashtable = null;
				if (styleClass != null)
				{
					styleAttributeHashtable = styleClass.StyleAttributes;
				}
				StyleAttributeHashtable styleAttributeHashtable2 = null;
				if (this.m_reportItem.HeadingInstance != null)
				{
					Global.Tracer.Assert(null != this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass);
					styleAttributeHashtable2 = this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.StyleAttributes;
				}
				for (int i = 0; i < 42; i++)
				{
					string name = this.m_styleDefaults.GetName(i);
					if (styleAttributeHashtable == null && styleAttributeHashtable2 == null)
					{
						base.AddStyleProperty(name, false, flag2, flag, this.m_styleDefaults[i]);
					}
					else if (styleAttributeHashtable2 != null && styleAttributeHashtable2.ContainsKey(name))
					{
						AttributeInfo attribute = styleAttributeHashtable2[name];
						base.AddStyleProperty(name, true, true, false, this.CreatePropertyOrReturnDefault(name, this.GetStyleAttributeValue(name, attribute)));
					}
					else if (styleAttributeHashtable != null && styleAttributeHashtable.ContainsKey(name))
					{
						AttributeInfo attributeInfo = styleAttributeHashtable[name];
						base.AddStyleProperty(name, attributeInfo.IsExpression, flag2, flag, this.CreatePropertyOrReturnDefault(name, this.GetStyleAttributeValue(name, attributeInfo)));
					}
					else if ("BackgroundImage" == name)
					{
						Image.SourceType imageSource = Image.SourceType.External;
						object obj = null;
						object obj2 = null;
						bool flag3 = false;
						bool flag4 = false;
						bool flag5 = false;
						if (styleAttributeHashtable2 != null)
						{
							flag5 = base.GetBackgroundImageProperties(styleAttributeHashtable2["BackgroundImageSource"], styleAttributeHashtable2["BackgroundImageValue"], styleAttributeHashtable2["BackgroundImageMIMEType"], out imageSource, out obj, out flag3, out obj2, out flag4);
						}
						if (!flag5 && styleAttributeHashtable != null)
						{
							flag5 = base.GetBackgroundImageProperties(styleAttributeHashtable["BackgroundImageSource"], styleAttributeHashtable["BackgroundImageValue"], styleAttributeHashtable["BackgroundImageMIMEType"], out imageSource, out obj, out flag3, out obj2, out flag4);
						}
						object styleProperty;
						if (obj != null)
						{
							string mimeType = null;
							if (!flag4)
							{
								mimeType = (string)obj2;
							}
							styleProperty = new BackgroundImage(base.m_renderingContext, imageSource, obj, mimeType);
						}
						else
						{
							styleProperty = this.m_styleDefaults[i];
						}
						base.AddStyleProperty(name, flag3 | flag4, flag2, flag, styleProperty);
					}
					else
					{
						base.AddStyleProperty(name, false, flag2, flag, this.m_styleDefaults[i]);
					}
				}
			}
		}

		private void PopulateNonSharedStyleProperties()
		{
			if (!base.IsCustomControl)
			{
				AspNetCore.ReportingServices.ReportProcessing.Style styleClass = this.m_reportItemDef.StyleClass;
				if (styleClass != null)
				{
					StyleAttributeHashtable styleAttributes = styleClass.StyleAttributes;
					Global.Tracer.Assert(null != styleAttributes);
					this.InternalPopulateNonSharedStyleProperties(styleAttributes, false);
				}
				if (this.m_reportItem.HeadingInstance != null)
				{
					Global.Tracer.Assert(null != this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass);
					StyleAttributeHashtable styleAttributes2 = this.m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.StyleAttributes;
					this.InternalPopulateNonSharedStyleProperties(styleAttributes2, true);
				}
			}
		}

		private void InternalPopulateNonSharedStyleProperties(StyleAttributeHashtable styleAttributes, bool isSubtotal)
		{
			if (!base.IsCustomControl && styleAttributes != null)
			{
				IDictionaryEnumerator enumerator = styleAttributes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AttributeInfo attributeInfo = (AttributeInfo)enumerator.Value;
					string text = (string)enumerator.Key;
					if ("BackgroundImageSource" == text)
					{
						Image.SourceType imageSource = default(Image.SourceType);
						object obj = default(object);
						bool flag = default(bool);
						object obj2 = default(object);
						bool flag2 = default(bool);
						if (base.GetBackgroundImageProperties(attributeInfo, styleAttributes["BackgroundImageValue"], styleAttributes["BackgroundImageMIMEType"], out imageSource, out obj, out flag, out obj2, out flag2) && (flag | flag2))
						{
							object styleProperty;
							if (obj != null)
							{
								string mimeType = null;
								if (!flag2)
								{
									mimeType = (string)obj2;
								}
								styleProperty = new BackgroundImage(base.m_renderingContext, imageSource, obj, mimeType);
							}
							else
							{
								styleProperty = this.m_styleDefaults["BackgroundImage"];
							}
							base.SetStyleProperty("BackgroundImage", true, true, false, styleProperty);
						}
					}
					else if (!("BackgroundImageValue" == text) && !("BackgroundImageMIMEType" == text) && (isSubtotal || attributeInfo.IsExpression))
					{
						base.SetStyleProperty(text, true, true, false, this.CreatePropertyOrReturnDefault(text, this.GetStyleAttributeValue(text, attributeInfo)));
					}
				}
			}
		}

		private object CreatePropertyOrReturnDefault(string styleName, object styleValue)
		{
			if (styleValue == null)
			{
				return this.m_styleDefaults[styleName];
			}
			return StyleBase.CreateStyleProperty(styleName, styleValue);
		}
	}
}
