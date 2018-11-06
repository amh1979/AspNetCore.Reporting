namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class TextRunFilteredStyle : Style
	{
		internal TextRunFilteredStyle(Style style)
			: base(ConstructionPhase.Deserializing)
		{
			base.m_styleAttributes = style.StyleAttributes;
			base.m_expressionList = style.ExpressionList;
		}

		internal override bool GetAttributeInfo(string styleAttributeName, out AttributeInfo styleAttribute)
		{
			switch (styleAttributeName)
			{
			case "FontStyle":
			case "FontFamily":
			case "FontSize":
			case "FontWeight":
			case "Format":
			case "TextDecoration":
			case "Color":
			case "Language":
			case "Calendar":
			case "CurrencyLanguage":
			case "NumeralLanguage":
			case "NumeralVariant":
				return base.GetAttributeInfo(styleAttributeName, out styleAttribute);
			default:
				styleAttribute = null;
				return false;
			}
		}
	}
}
