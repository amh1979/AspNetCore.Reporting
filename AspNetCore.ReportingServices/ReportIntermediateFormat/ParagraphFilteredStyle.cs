namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class ParagraphFilteredStyle : Style
	{
		internal ParagraphFilteredStyle(Style style)
			: base(ConstructionPhase.Deserializing)
		{
			base.m_styleAttributes = style.StyleAttributes;
			base.m_expressionList = style.ExpressionList;
		}

		internal override bool GetAttributeInfo(string styleAttributeName, out AttributeInfo styleAttribute)
		{
			switch (styleAttributeName)
			{
			case "TextAlign":
			case "LineHeight":
				return base.GetAttributeInfo(styleAttributeName, out styleAttribute);
			default:
				styleAttribute = null;
				return false;
			}
		}
	}
}
