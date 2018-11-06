using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class HtmlLexer
	{
		internal sealed class Constants
		{
			internal class AttributeNames
			{
				internal const string Align = "align";

				internal const string Padding = "padding";

				internal const string PaddingTop = "padding-top";

				internal const string PaddingBottom = "padding-bottom";

				internal const string PaddingLeft = "padding-left";

				internal const string PaddingRight = "padding-right";

				internal const string Href = "href";

				internal const string Size = "size";

				internal const string Face = "face";

				internal const string Color = "color";

				internal const string Style = "style";

				internal const string FontFamily = "font-family";

				internal const string FontSize = "font-size";

				internal const string FontWeight = "font-weight";

				internal const string TextAlign = "text-align";

				internal const string TextIndent = "text-indent";
			}

			internal class ElementNames
			{
				internal const string SCRIPT = "SCRIPT";

				internal const string STYLE = "STYLE";

				internal const string P = "P";

				internal const string DIV = "DIV";

				internal const string BR = "BR";

				internal const string UL = "UL";

				internal const string OL = "OL";

				internal const string LI = "LI";

				internal const string SPAN = "SPAN";

				internal const string FONT = "FONT";

				internal const string A = "A";

				internal const string STRONG = "STRONG";

				internal const string STRIKE = "STRIKE";

				internal const string B = "B";

				internal const string I = "I";

				internal const string U = "U";

				internal const string S = "S";

				internal const string EM = "EM";

				internal const string H1 = "H1";

				internal const string H2 = "H2";

				internal const string H3 = "H3";

				internal const string H4 = "H4";

				internal const string H5 = "H5";

				internal const string H6 = "H6";

				internal const string DD = "DD";

				internal const string DT = "DT";

				internal const string BLOCKQUOTE = "BLOCKQUOTE";

				internal const string TITLE = "TITLE";
			}
		}

		private enum AttributeEscapeState
		{
			None,
			SingleQuote,
			DoubleQuote,
			NoQuote,
			RawEquals
		}

		private sealed class HtmlStringReader
		{
			private int m_markedIndex;

			private int m_currentIndex;

			private string m_html;

			internal int Position
			{
				get
				{
					return this.m_currentIndex;
				}
			}

			internal HtmlStringReader(string html)
			{
				this.m_html = html;
			}

			internal bool Read(out char c)
			{
				if (this.Peek(out c))
				{
					this.m_currentIndex++;
					return true;
				}
				return false;
			}

			internal bool Peek(out char c)
			{
				if (this.m_currentIndex < this.m_html.Length)
				{
					c = this.m_html[this.m_currentIndex];
					switch (c)
					{
					case '\r':
						this.m_currentIndex++;
						return this.Peek(out c);
					case '\t':
					case '\n':
					case '\v':
					case '\f':
						c = ' ';
						break;
					}
					return true;
				}
				c = '\0';
				return false;
			}

			internal bool Peek(int lookAhead, out char c)
			{
				int currentIndex = this.m_currentIndex;
				this.m_currentIndex += lookAhead;
				bool result = this.Peek(out c);
				this.m_currentIndex = currentIndex;
				return result;
			}

			internal void Advance()
			{
				this.m_currentIndex++;
			}

			internal void Advance(int amount)
			{
				this.m_currentIndex += amount;
			}

			internal void Mark()
			{
				this.m_markedIndex = this.m_currentIndex;
			}

			internal void Reset()
			{
				this.m_currentIndex = this.m_markedIndex;
			}
		}

		private StringBuilder m_sb = new StringBuilder(16);

		private HtmlElement m_currentElement;

		private bool m_readWhiteSpace;

		private HtmlStringReader m_htmlReader;

		private Stack<HtmlElement> m_elementStack;

		internal HtmlElement CurrentElement
		{
			get
			{
				return this.m_currentElement;
			}
		}

		internal HtmlLexer(string html)
		{
			this.m_htmlReader = new HtmlStringReader(html);
			this.m_elementStack = new Stack<HtmlElement>();
		}

		private static HtmlElement.HtmlElementType GetElementType(string elementName)
		{
			switch (elementName.ToUpperInvariant())
			{
			case "A":
				return HtmlElement.HtmlElementType.A;
			case "B":
				return HtmlElement.HtmlElementType.B;
			case "BLOCKQUOTE":
				return HtmlElement.HtmlElementType.BLOCKQUOTE;
			case "BR":
				return HtmlElement.HtmlElementType.BR;
			case "DD":
				return HtmlElement.HtmlElementType.DD;
			case "DIV":
				return HtmlElement.HtmlElementType.DIV;
			case "DT":
				return HtmlElement.HtmlElementType.DT;
			case "EM":
				return HtmlElement.HtmlElementType.EM;
			case "FONT":
				return HtmlElement.HtmlElementType.FONT;
			case "H1":
				return HtmlElement.HtmlElementType.H1;
			case "H2":
				return HtmlElement.HtmlElementType.H2;
			case "H3":
				return HtmlElement.HtmlElementType.H3;
			case "H4":
				return HtmlElement.HtmlElementType.H4;
			case "H5":
				return HtmlElement.HtmlElementType.H5;
			case "H6":
				return HtmlElement.HtmlElementType.H6;
			case "I":
				return HtmlElement.HtmlElementType.I;
			case "LI":
				return HtmlElement.HtmlElementType.LI;
			case "OL":
				return HtmlElement.HtmlElementType.OL;
			case "P":
				return HtmlElement.HtmlElementType.P;
			case "S":
				return HtmlElement.HtmlElementType.S;
			case "SPAN":
				return HtmlElement.HtmlElementType.SPAN;
			case "STRIKE":
				return HtmlElement.HtmlElementType.STRIKE;
			case "STRONG":
				return HtmlElement.HtmlElementType.STRONG;
			case "U":
				return HtmlElement.HtmlElementType.U;
			case "UL":
				return HtmlElement.HtmlElementType.UL;
			case "STYLE":
				return HtmlElement.HtmlElementType.STYLE;
			case "SCRIPT":
				return HtmlElement.HtmlElementType.SCRIPT;
			case "TITLE":
				return HtmlElement.HtmlElementType.TITLE;
			default:
				return HtmlElement.HtmlElementType.Unsupported;
			}
		}

		internal bool Read()
		{
			char c = default(char);
			if (this.m_htmlReader.Peek(out c))
			{
				char c2 = c;
				if (c2 == '<')
				{
					if (this.m_htmlReader.Peek(1, out c))
					{
						switch (c)
						{
						case '/':
							this.m_htmlReader.Advance();
							this.ReadEndElement();
							break;
						case '!':
							this.m_htmlReader.Advance();
							this.ReadBangElement();
							break;
						default:
							if (char.IsLetter(c))
							{
								this.m_htmlReader.Advance();
								this.ReadStartElement();
								break;
							}
							this.ReadTextElement();
							if (!string.IsNullOrEmpty(this.m_currentElement.Value))
							{
								break;
							}
							return this.Read();
						}
					}
					else
					{
						this.ReadTextElement();
					}
					return true;
				}
				HtmlElement.HtmlElementType htmlElementType = HtmlElement.HtmlElementType.None;
				if (this.m_elementStack.Count > 0)
				{
					htmlElementType = this.m_elementStack.Peek().ElementType;
					if (htmlElementType != HtmlElement.HtmlElementType.STYLE && htmlElementType != HtmlElement.HtmlElementType.SCRIPT)
					{
						goto IL_00c5;
					}
					this.ReadScriptOrStyleContents(htmlElementType);
					return true;
				}
				goto IL_00c5;
			}
			return false;
			IL_00c5:
			this.ReadTextElement();
			if (string.IsNullOrEmpty(this.m_currentElement.Value))
			{
				return this.Read();
			}
			return true;
		}

		private void ReadStartElement()
		{
			int position = this.m_htmlReader.Position;
			HtmlElement.HtmlElementType type = this.ReadElementType(false);
			bool isEmpty = default(bool);
			this.m_currentElement = new HtmlElement(HtmlElement.HtmlNodeType.Element, type, this.GetAttributesAsString(out isEmpty), isEmpty, position);
			this.m_elementStack.Push(this.m_currentElement);
			this.AdvanceToEndOfElement();
		}

		private void ReadScriptOrStyleContents(HtmlElement.HtmlElementType aElementType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			int position = this.m_htmlReader.Position;
			char c = default(char);
			while (this.m_htmlReader.Peek(out c) && flag)
			{
				char c2 = c;
				if (c2 == '<')
				{
					this.m_htmlReader.Mark();
					this.m_htmlReader.Advance();
					char c3 = default(char);
					if (this.m_htmlReader.Peek(out c3))
					{
						if (c3 == '!' && this.m_htmlReader.Peek(1, out c3) && c3 == '-' && this.m_htmlReader.Peek(2, out c3) && c3 == '-')
						{
							flag = false;
							this.m_htmlReader.Reset();
						}
						else if (c3 == '/')
						{
							HtmlElement.HtmlElementType htmlElementType = this.ReadElementType(true);
							this.m_htmlReader.Reset();
							if (htmlElementType == aElementType)
							{
								flag = false;
							}
							else
							{
								this.m_htmlReader.Advance();
								stringBuilder.Append(c);
							}
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
				}
				else
				{
					this.m_htmlReader.Advance();
					stringBuilder.Append(c);
				}
			}
			this.m_currentElement = new HtmlElement((HtmlElement.HtmlNodeType)((aElementType == HtmlElement.HtmlElementType.SCRIPT) ? 4 : 5), HtmlElement.HtmlElementType.None, stringBuilder.ToString(), position);
		}

		private void ReadBangElement()
		{
			bool flag = false;
			this.m_sb.Length = 0;
			this.m_htmlReader.Advance();
			char c2 = default(char);
			char c = default(char);
			if (this.m_htmlReader.Peek(out c) && c == '-' && this.m_htmlReader.Peek(1, out c2) && c2 == '-')
			{
				int position = this.m_htmlReader.Position;
				this.m_htmlReader.Advance(2);
				this.m_htmlReader.Mark();
				while (this.m_htmlReader.Read(out c))
				{
					if (c != '-' || !this.m_htmlReader.Peek(out c2) || c2 != '-' || !this.m_htmlReader.Peek(1, out c2) || c2 != '>')
					{
						this.m_sb.Append(c);
						continue;
					}
					this.m_htmlReader.Advance(2);
					flag = true;
					break;
				}
				if (!flag)
				{
					this.m_htmlReader.Reset();
					bool flag2 = default(bool);
					this.m_sb = this.ReadTextContent(true, out flag2);
				}
				this.m_currentElement = new HtmlElement(HtmlElement.HtmlNodeType.Comment, HtmlElement.HtmlElementType.None, this.m_sb.ToString(), position);
			}
			else
			{
				this.ReadStartElement();
			}
		}

		private void AdvanceToEndOfElement()
		{
			char c = default(char);
			while (this.m_htmlReader.Peek(out c))
			{
				switch (c)
				{
				case '<':
					return;
				case '>':
					this.m_htmlReader.Advance();
					return;
				}
				this.m_htmlReader.Advance();
			}
		}

		private HtmlElement.HtmlElementType ReadElementType(bool isEndElement)
		{
			this.m_sb.Length = 0;
			bool flag = true;
			char c = default(char);
			while (flag && this.m_htmlReader.Peek(out c))
			{
				switch (c)
				{
				case ' ':
					if (isEndElement && this.m_sb.Length == 0)
					{
						this.m_htmlReader.Advance();
						break;
					}
					goto case '>';
				case '/':
					if (isEndElement)
					{
						this.m_htmlReader.Advance();
						break;
					}
					goto case '>';
				case '>':
					if (this.m_sb.Length == 0)
					{
						return HtmlElement.HtmlElementType.Unsupported;
					}
					return HtmlLexer.GetElementType(this.m_sb.ToString());
				default:
					this.m_htmlReader.Advance();
					this.m_sb.Append(c);
					break;
				}
			}
			return HtmlElement.HtmlElementType.Unsupported;
		}

		private string GetAttributesAsString(out bool isEmpty)
		{
			this.m_sb.Length = 0;
			isEmpty = false;
			AttributeEscapeState attributeEscapeState = AttributeEscapeState.None;
			char c = default(char);
			while (this.m_htmlReader.Peek(out c))
			{
				switch (attributeEscapeState)
				{
				case AttributeEscapeState.None:
					switch (c)
					{
					case '=':
						this.ConsumeAndAppend(c);
						attributeEscapeState = AttributeEscapeState.RawEquals;
						break;
					case '<':
					case '>':
						return this.m_sb.ToString();
					case '/':
					{
						char c4 = default(char);
						if (this.m_htmlReader.Peek(1, out c4) && c4 == '>')
						{
							isEmpty = true;
							return this.m_sb.ToString();
						}
						this.ConsumeAndAppend(c);
						break;
					}
					default:
						this.ConsumeAndAppend(c);
						break;
					}
					break;
				case AttributeEscapeState.RawEquals:
					switch (c)
					{
					case ' ':
						this.ConsumeAndAppend(c);
						break;
					case '"':
						this.ConsumeAndAppend(c);
						attributeEscapeState = AttributeEscapeState.DoubleQuote;
						break;
					case '\'':
						attributeEscapeState = AttributeEscapeState.SingleQuote;
						this.ConsumeAndAppend(c);
						break;
					case '>':
						return this.m_sb.ToString();
					default:
						attributeEscapeState = AttributeEscapeState.NoQuote;
						this.ConsumeAndAppend(c);
						break;
					}
					break;
				case AttributeEscapeState.DoubleQuote:
				{
					this.ConsumeAndAppend(c);
					char c3 = c;
					if (c3 == '"')
					{
						attributeEscapeState = AttributeEscapeState.None;
					}
					break;
				}
				case AttributeEscapeState.SingleQuote:
				{
					this.ConsumeAndAppend(c);
					char c2 = c;
					if (c2 == '\'')
					{
						attributeEscapeState = AttributeEscapeState.None;
					}
					break;
				}
				case AttributeEscapeState.NoQuote:
					switch (c)
					{
					case '>':
						return this.m_sb.ToString();
					case ' ':
						this.ConsumeAndAppend(c);
						attributeEscapeState = AttributeEscapeState.None;
						break;
					default:
						this.ConsumeAndAppend(c);
						break;
					}
					break;
				}
			}
			return this.m_sb.ToString();
		}

		private void ConsumeAndAppend(char c)
		{
			this.m_htmlReader.Advance();
			this.m_sb.Append(c);
		}

		private StringBuilder ReadTextContent(bool inComment, out bool hasEntity)
		{
			hasEntity = false;
			this.m_sb.Length = 0;
			bool flag = true;
			char c = default(char);
			while (flag && this.m_htmlReader.Peek(out c))
			{
				switch (c)
				{
				case '>':
					if (inComment)
					{
						flag = false;
						break;
					}
					goto default;
				case '<':
				{
					char c2 = default(char);
					this.m_htmlReader.Peek(1, out c2);
					char c3 = c2;
					if (c3 == '!' || c3 == '/')
					{
						flag = false;
					}
					else if (char.IsLetter(c2))
					{
						flag = false;
					}
					if (!flag)
					{
						break;
					}
					goto default;
				}
				case ' ':
					if (!this.m_readWhiteSpace)
					{
						this.m_sb.Append(c);
						this.m_readWhiteSpace = true;
					}
					break;
				case '&':
					hasEntity = true;
					goto default;
				default:
					this.m_sb.Append(c);
					if (this.m_readWhiteSpace)
					{
						this.m_readWhiteSpace = false;
					}
					break;
				}
				if (flag)
				{
					this.m_htmlReader.Advance();
				}
			}
			return this.m_sb;
		}

		private void ReadTextElement()
		{
			int position = this.m_htmlReader.Position;
			bool flag = default(bool);
			this.m_sb = this.ReadTextContent(false, out flag);
			if (flag)
			{
				HtmlEntityResolver.ResolveEntities(this.m_sb);
			}
			this.m_currentElement = new HtmlElement(HtmlElement.HtmlNodeType.Text, HtmlElement.HtmlElementType.None, this.m_sb.ToString(), position);
		}

		private void ReadEndElement()
		{
			int position = this.m_htmlReader.Position;
			HtmlElement.HtmlElementType elemntType = this.ReadElementType(true);
			this.m_currentElement = new HtmlElement(HtmlElement.HtmlNodeType.EndElement, elemntType, position);
			this.AdvanceToEndOfElement();
			if (this.m_elementStack.Count > 0)
			{
				HtmlElement htmlElement = this.m_elementStack.Pop();
				HtmlElement.HtmlElementType elementType = htmlElement.ElementType;
			}
		}
	}
}
