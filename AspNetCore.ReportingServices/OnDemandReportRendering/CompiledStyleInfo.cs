namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class CompiledStyleInfo
	{
		private HtmlElement.HtmlElementType m_elementType;

		private ReportColor m_color;

		private FontStyles m_fontStyle;

		private string m_fontFamily;

		private ReportSize m_fontSize;

		private TextAlignments m_textAlign;

		private TextDecorations m_textDecoration;

		private FontWeights m_fontWeight;

		private bool m_colorSet;

		private bool m_fontStyleSet;

		private bool m_fontFamilySet;

		private bool m_fontSizeSet;

		private bool m_textAlignSet;

		private bool m_textDecorationSet;

		private bool m_fontWeightSet;

		private CompiledStyleInfo m_parentStyle;

		private CompiledStyleInfo m_childStyle;

		internal HtmlElement.HtmlElementType ElementType
		{
			get
			{
				return this.m_elementType;
			}
			set
			{
				this.m_elementType = value;
			}
		}

		internal ReportColor Color
		{
			get
			{
				if (this.m_colorSet)
				{
					return this.m_color;
				}
				if (this.m_parentStyle != null)
				{
					return this.m_parentStyle.Color;
				}
				return null;
			}
			set
			{
				this.m_colorSet = true;
				this.m_color = value;
			}
		}

		internal FontStyles FontStyle
		{
			get
			{
				if (this.m_fontStyleSet)
				{
					return this.m_fontStyle;
				}
				if (this.m_parentStyle != null)
				{
					return this.m_parentStyle.FontStyle;
				}
				return FontStyles.Default;
			}
			set
			{
				this.m_fontStyleSet = true;
				this.m_fontStyle = value;
			}
		}

		internal string FontFamily
		{
			get
			{
				if (this.m_fontFamilySet)
				{
					return this.m_fontFamily;
				}
				if (this.m_parentStyle != null)
				{
					return this.m_parentStyle.FontFamily;
				}
				return null;
			}
			set
			{
				this.m_fontFamilySet = true;
				this.m_fontFamily = value;
			}
		}

		internal ReportSize FontSize
		{
			get
			{
				if (this.m_fontSizeSet)
				{
					return this.m_fontSize;
				}
				if (this.m_parentStyle != null)
				{
					return this.m_parentStyle.FontSize;
				}
				return null;
			}
			set
			{
				this.m_fontSizeSet = true;
				this.m_fontSize = value;
			}
		}

		internal TextAlignments TextAlign
		{
			get
			{
				if (this.m_textAlignSet)
				{
					return this.m_textAlign;
				}
				if (this.m_parentStyle != null)
				{
					return this.m_parentStyle.TextAlign;
				}
				return TextAlignments.Default;
			}
			set
			{
				this.m_textAlignSet = true;
				this.m_textAlign = value;
			}
		}

		internal FontWeights FontWeight
		{
			get
			{
				if (this.m_fontWeightSet)
				{
					return this.m_fontWeight;
				}
				if (this.m_parentStyle != null)
				{
					return this.m_parentStyle.FontWeight;
				}
				return FontWeights.Default;
			}
			set
			{
				this.m_fontWeightSet = true;
				this.m_fontWeight = value;
			}
		}

		internal TextDecorations TextDecoration
		{
			get
			{
				if (this.m_textDecorationSet)
				{
					return this.m_textDecoration;
				}
				if (this.m_parentStyle != null)
				{
					return this.m_parentStyle.TextDecoration;
				}
				return TextDecorations.Default;
			}
			set
			{
				this.m_textDecorationSet = true;
				this.m_textDecoration = value;
			}
		}

		internal CompiledStyleInfo CreateChildStyle(HtmlElement.HtmlElementType elementType)
		{
			CompiledStyleInfo compiledStyleInfo = new CompiledStyleInfo();
			compiledStyleInfo.m_elementType = elementType;
			compiledStyleInfo.m_parentStyle = this;
			this.m_childStyle = compiledStyleInfo;
			return compiledStyleInfo;
		}

		internal CompiledStyleInfo RemoveStyle(HtmlElement.HtmlElementType elementType)
		{
			if (this.m_elementType == elementType)
			{
				if (this.m_parentStyle != null)
				{
					this.m_parentStyle.m_childStyle = null;
					return this.m_parentStyle;
				}
				this.ResetStyle();
			}
			else if (this.m_parentStyle != null)
			{
				this.m_parentStyle.InternalRemoveStyle(elementType);
			}
			return this;
		}

		internal void InternalRemoveStyle(HtmlElement.HtmlElementType elementType)
		{
			if (this.m_elementType == elementType)
			{
				if (this.m_parentStyle != null)
				{
					this.m_parentStyle.m_childStyle = this.m_childStyle;
					this.m_childStyle.m_parentStyle = this.m_parentStyle;
				}
				else if (this.m_parentStyle == null)
				{
					this.m_childStyle.m_parentStyle = null;
				}
			}
			else if (this.m_parentStyle != null)
			{
				this.m_parentStyle.InternalRemoveStyle(elementType);
			}
		}

		private void ResetStyle()
		{
			this.m_colorSet = false;
			this.m_fontFamilySet = false;
			this.m_fontSizeSet = false;
			this.m_fontStyleSet = false;
			this.m_fontWeightSet = false;
			this.m_textAlignSet = false;
			this.m_textDecorationSet = false;
		}

		internal void PopulateStyleInstance(ICompiledStyleInstance styleInstance, bool isParagraphStyle)
		{
			if (isParagraphStyle)
			{
				TextAlignments textAlign = this.TextAlign;
				if (textAlign != 0)
				{
					styleInstance.TextAlign = textAlign;
				}
			}
			else
			{
				ReportColor color = this.Color;
				if (color != null)
				{
					styleInstance.Color = color;
				}
				string fontFamily = this.FontFamily;
				if (!string.IsNullOrEmpty(fontFamily))
				{
					styleInstance.FontFamily = fontFamily;
				}
				ReportSize fontSize = this.FontSize;
				if (fontSize != null)
				{
					styleInstance.FontSize = fontSize;
				}
				FontStyles fontStyle = this.FontStyle;
				if (fontStyle != 0)
				{
					styleInstance.FontStyle = fontStyle;
				}
				FontWeights fontWeight = this.FontWeight;
				if (fontWeight != 0)
				{
					styleInstance.FontWeight = fontWeight;
				}
				TextDecorations textDecoration = this.TextDecoration;
				if (textDecoration != 0)
				{
					styleInstance.TextDecoration = textDecoration;
				}
			}
		}
	}
}
