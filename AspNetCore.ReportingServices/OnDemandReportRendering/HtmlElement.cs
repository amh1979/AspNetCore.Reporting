using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class HtmlElement
	{
		internal enum HtmlNodeType
		{
			Element,
			EndElement,
			Text,
			Comment,
			ScriptText,
			StyleText
		}

		internal enum HtmlElementType
		{
			None,
			Unsupported,
			SCRIPT,
			STYLE,
			P,
			DIV,
			BR,
			UL,
			OL,
			LI,
			SPAN,
			FONT,
			A,
			STRONG,
			STRIKE,
			B,
			I,
			U,
			S,
			EM,
			H1,
			H2,
			H3,
			H4,
			H5,
			H6,
			DD,
			DT,
			BLOCKQUOTE,
			TITLE
		}

		private string m_value;

		private bool m_isEmptyElement;

		private HtmlNodeType m_nodeType;

		private HtmlElementType m_elementType;

		private string m_attributesAsString;

		private Dictionary<string, string> m_parsedAttributes;

		private Dictionary<string, string> m_parsedCssStyleValues;

		private int m_characterPosition;

		private static Regex m_AttributeRegEx = new Regex("((?<name>\\w+)(\\s*=\\s*((\"(?<quotedvalue>[^\"]*)\")|('(?<singlequotedvalue>[^']*)')|(?<value>[^ =]+))?)?)*", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		internal bool IsEmptyElement
		{
			get
			{
				return this.m_isEmptyElement;
			}
		}

		internal HtmlNodeType NodeType
		{
			get
			{
				return this.m_nodeType;
			}
		}

		internal HtmlElementType ElementType
		{
			get
			{
				return this.m_elementType;
			}
		}

		internal Dictionary<string, string> Attributes
		{
			get
			{
				this.ParseAttributes();
				return this.m_parsedAttributes;
			}
		}

		internal Dictionary<string, string> CssStyle
		{
			get
			{
				if (this.HasAttributes)
				{
					this.ParseAttributes();
					string text = default(string);
					if (this.m_parsedAttributes.TryGetValue("style", out text) && !string.IsNullOrEmpty(text))
					{
						this.ParseCssStyle(text);
					}
				}
				return this.m_parsedCssStyleValues;
			}
		}

		internal bool HasAttributes
		{
			get
			{
				if (this.m_attributesAsString == null)
				{
					return this.m_parsedAttributes != null;
				}
				return true;
			}
		}

		internal string Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal int CharacterPosition
		{
			get
			{
				return this.m_characterPosition;
			}
		}

		internal HtmlElement(HtmlNodeType nodeType, HtmlElementType elemntType, int characterPosition)
			: this(nodeType, elemntType, null, true, characterPosition)
		{
		}

		internal HtmlElement(HtmlNodeType nodeType, HtmlElementType elemntType, string value, int characterPosition)
			: this(nodeType, elemntType, null, false, characterPosition)
		{
			this.m_value = value;
		}

		internal HtmlElement(HtmlNodeType nodeType, HtmlElementType type, string attributesAsString, bool isEmpty, int characterPosition)
		{
			this.m_nodeType = nodeType;
			this.m_elementType = type;
			this.m_isEmptyElement = isEmpty;
			this.m_characterPosition = characterPosition;
			if (!string.IsNullOrEmpty(attributesAsString))
			{
				this.m_attributesAsString = attributesAsString;
			}
		}

		private void ParseCssStyle(string cssStyles)
		{
			string[] array = cssStyles.Split(new char[1]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			this.m_parsedCssStyleValues = new Dictionary<string, string>(array.Length, StringEqualityComparer.Instance);
			foreach (string text in array)
			{
				string text2 = string.Empty;
				string html = string.Empty;
				int num = text.IndexOf(':');
				if (num == -1)
				{
					text2 = text.Trim();
				}
				else if (num > 0)
				{
					text2 = text.Substring(0, num).Trim();
					if (num + 1 < text.Length)
					{
						html = text.Substring(num + 1).Trim();
					}
				}
				if (!string.IsNullOrEmpty(text2))
				{
					this.m_parsedCssStyleValues[text2.ToLowerInvariant()] = HtmlEntityResolver.ResolveEntities(html).ToLowerInvariant();
				}
			}
		}

		private void ParseAttributes()
		{
			if (this.m_attributesAsString != null)
			{
				MatchCollection matchCollection = HtmlElement.m_AttributeRegEx.Matches(this.m_attributesAsString.Trim());
				if (matchCollection.Count > 0)
				{
					this.m_parsedAttributes = new Dictionary<string, string>(matchCollection.Count, StringEqualityComparer.Instance);
					for (int i = 0; i < matchCollection.Count; i++)
					{
						Match match = matchCollection[i];
						string text = null;
						string html = null;
						System.Text.RegularExpressions.Group group = match.Groups["name"];
						if (group.Length > 0)
						{
							text = group.Value;
							group = match.Groups["quotedvalue"];
							if (group.Length > 0)
							{
								html = group.Value;
							}
							else
							{
								group = match.Groups["singlequotedvalue"];
								if (group.Length > 0)
								{
									html = group.Value;
								}
								else
								{
									group = match.Groups["value"];
									if (group.Length > 0)
									{
										html = group.Value;
									}
								}
							}
							this.m_parsedAttributes[text.ToLowerInvariant()] = HtmlEntityResolver.ResolveEntities(html);
						}
					}
				}
			}
			this.m_attributesAsString = null;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.m_nodeType.ToString());
			stringBuilder.Append(" Type = ");
			stringBuilder.Append(this.m_elementType.ToString());
			if (!this.m_isEmptyElement)
			{
				if (this.HasAttributes)
				{
					this.GetDictionaryAsString("Attributes", this.Attributes, stringBuilder);
					this.GetDictionaryAsString("CssStyle", this.CssStyle, stringBuilder);
				}
				if (this.m_value != null)
				{
					stringBuilder.Append("; Value = \"");
					stringBuilder.Append(this.m_value);
					stringBuilder.Append("\"");
				}
			}
			return stringBuilder.ToString();
		}

		private void GetDictionaryAsString(string name, Dictionary<string, string> dict, StringBuilder sb)
		{
			if (dict != null)
			{
				sb.Append("; ");
				sb.Append(name);
				sb.Append(" = { ");
				int num = 0;
				foreach (KeyValuePair<string, string> item in dict)
				{
					if (num > 0)
					{
						sb.Append(", ");
					}
					num++;
					sb.Append(item.Key);
					sb.Append("=\"");
					sb.Append(item.Value);
					sb.Append("\"");
				}
				sb.Append(" }");
			}
		}
	}
}
