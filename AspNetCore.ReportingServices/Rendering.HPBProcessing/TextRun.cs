using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class TextRun : IStorable, IPersistable, ITextRunProps
	{
		[StaticReference]
		private static Declaration m_declaration = TextRun.GetDeclaration();

		[StaticReference]
		private AspNetCore.ReportingServices.OnDemandReportRendering.TextRun m_source;

		private Dictionary<byte, object> m_styles;

		private string m_text;

		private string m_toolTip;

		private byte? m_markup = null;

		private List<string> m_hyperlinks;

		private string m_uniqueName;

		private List<int> m_splitIndices = new List<int>();

		private int m_startCharacterOffset;

		private string m_fontKey;

		public string Text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				this.m_text = value;
			}
		}

		public string DefinitionText
		{
			get
			{
				if (this.m_source != null && this.m_source.Value != null)
				{
					if (!this.m_source.FormattedValueExpressionBased && this.m_source.SharedTypeCode != TypeCode.String)
					{
						return this.m_source.Instance.Value;
					}
					return this.m_source.Value.Value;
				}
				return null;
			}
		}

		public List<int> SplitIndices
		{
			get
			{
				return this.m_splitIndices;
			}
		}

		public string FontFamily
		{
			get
			{
				string text = null;
				if (this.m_source != null)
				{
					text = Utility.GetStringProp(20, StyleAttributeNames.FontFamily, this.m_source.Style, this.m_styles);
				}
				if (text == null)
				{
					text = "Arial";
				}
				return text;
			}
		}

		public float FontSize
		{
			get
			{
				if (this.m_source == null)
				{
					return 10f;
				}
				return (float)Utility.GetSizeProp(21, StyleAttributeNames.FontSize, 10f, this.m_source.Style, this.m_styles);
			}
		}

		public Color Color
		{
			get
			{
				if (this.m_source == null)
				{
					return Color.Black;
				}
				return Utility.GetColorProp(27, StyleAttributeNames.Color, Color.Black, this.m_source.Style, this.m_styles);
			}
		}

		public bool Bold
		{
			get
			{
				bool result = false;
				if (this.m_source != null)
				{
					byte? enumProp = Utility.GetEnumProp(22, StyleAttributeNames.FontWeight, this.m_source.Style, this.m_styles);
					int? nullable = enumProp;
					if (nullable.HasValue)
					{
						result = Utility.IsBold((RPLFormat.FontWeights)enumProp.Value);
					}
				}
				return result;
			}
		}

		public bool Italic
		{
			get
			{
				if (this.m_source == null)
				{
					return false;
				}
				byte? enumProp = Utility.GetEnumProp(19, StyleAttributeNames.FontStyle, this.m_source.Style, this.m_styles);
				return ((int?)enumProp).HasValue && enumProp.Value == 1;
			}
		}

		public RPLFormat.TextDecorations TextDecoration
		{
			get
			{
				if (this.m_source == null)
				{
					return RPLFormat.TextDecorations.None;
				}
				byte? enumProp = Utility.GetEnumProp(24, StyleAttributeNames.TextDecoration, this.m_source.Style, this.m_styles);
				if (!enumProp.HasValue)
				{
					return RPLFormat.TextDecorations.None;
				}
				return (RPLFormat.TextDecorations)enumProp.Value;
			}
		}

		public int IndexInParagraph
		{
			get
			{
				return -1;
			}
		}

		public string FontKey
		{
			get
			{
				return this.m_fontKey;
			}
			set
			{
				this.m_fontKey = value;
			}
		}

		public int Size
		{
			get
			{
				return AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_text) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_styles) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_toolTip) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_markup) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_hyperlinks) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_uniqueName) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_splitIndices) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_startCharacterOffset) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_fontKey) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize;
			}
		}

		internal TextRun()
		{
		}

		internal TextRun(TextRunInstance instance, bool hideDuplicates)
		{
			this.m_source = instance.Definition;
			Utility.AddInstanceStyles(instance.Style, ref this.m_styles);
			if (instance.IsCompiled)
			{
				this.m_text = instance.Value;
				this.m_toolTip = instance.ToolTip;
			}
			else
			{
				this.m_text = ((this.m_source.Value != null && (this.m_source.FormattedValueExpressionBased || hideDuplicates)) ? instance.Value : null);
				this.m_toolTip = ((this.m_source.ToolTip != null && this.m_source.ToolTip.IsExpression) ? instance.ToolTip : null);
			}
			ActionInfo actionInfo = this.m_source.ActionInfo;
			if (instance.IsCompiled)
			{
				CompiledTextRunInstance compiledTextRunInstance = instance as CompiledTextRunInstance;
				ActionInstance actionInstance = compiledTextRunInstance.ActionInstance;
				if (actionInstance != null)
				{
					this.m_hyperlinks = new List<string>(1);
					ReportUrl hyperlink = actionInstance.Hyperlink;
					string item = null;
					if (hyperlink != null)
					{
						item = hyperlink.ToString();
					}
					this.m_hyperlinks.Add(item);
				}
			}
			else if (actionInfo != null)
			{
				ActionCollection actions = actionInfo.Actions;
				this.m_hyperlinks = new List<string>(actions.Count);
				foreach (AspNetCore.ReportingServices.OnDemandReportRendering.Action item3 in actions)
				{
					ReportUrl hyperlink2 = item3.Instance.Hyperlink;
					string item2 = null;
					if (hyperlink2 != null)
					{
						item2 = hyperlink2.ToString();
					}
					this.m_hyperlinks.Add(item2);
				}
			}
			this.m_uniqueName = instance.UniqueName;
			if (this.m_source.MarkupType != null && this.m_source.MarkupType.IsExpression)
			{
				this.m_markup = StyleEnumConverter.Translate(instance.MarkupType);
			}
		}

		public void AddSplitIndex(int index)
		{
			this.m_splitIndices.Add(index);
		}

		public void ClearTo(int splitIndex)
		{
			int startCharacterOffset = this.m_startCharacterOffset;
			if (splitIndex < this.m_splitIndices.Count)
			{
				startCharacterOffset = this.SplitIndices[splitIndex];
				this.SplitIndices.RemoveRange(0, splitIndex + 1);
			}
			this.m_startCharacterOffset = startCharacterOffset;
		}

		internal AspNetCore.ReportingServices.Rendering.RichText.TextRun GetRichTextRun()
		{
			string text = this.m_text;
			if (text == null && this.m_source != null && this.m_source.Value != null)
			{
				text = this.m_source.Value.Value;
			}
			return new AspNetCore.ReportingServices.Rendering.RichText.TextRun(text, this);
		}

		public RPLTextRun GetRPLTextRun(PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, int previousCount, List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			RPLTextRun rPLTextRun = new RPLTextRun();
			RPLTextRunProps props = rPLTextRun.ElementProps as RPLTextRunProps;
			this.WriteElementProps(props, pageContext, hideDuplicates, startPosition, endPosition, previousCount, richTextRuns);
			return rPLTextRun;
		}

		internal string GetStringValue(string fullValue, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, int previousRunCount, List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextRuns, out TextRunItemizedData glyphData)
		{
			glyphData = null;
			if (string.IsNullOrEmpty(fullValue))
			{
				return null;
			}
			int num = this.m_startCharacterOffset;
			int num2 = fullValue.Length;
			if (endPosition == null && startPosition == null)
			{
				if (this.m_startCharacterOffset == 0)
				{
					glyphData = this.CreateGlyphData(previousRunCount, num, num2, fullValue, richTextRuns);
					return fullValue;
				}
				glyphData = this.CreateGlyphData(previousRunCount, num, num2, fullValue, richTextRuns);
				return fullValue.Substring(this.m_startCharacterOffset);
			}
			if (startPosition != null)
			{
				int num3 = startPosition.TextRunIndex - previousRunCount;
				if (num3 == 0)
				{
					num += startPosition.CharacterIndex;
				}
				else if (num3 > 0)
				{
					num = this.m_splitIndices[num3 - 1];
					num += startPosition.CharacterIndex;
				}
			}
			if (endPosition != null)
			{
				int num4 = endPosition.TextRunIndex - previousRunCount;
				RSTrace.RenderingTracer.Assert(num4 >= 0, string.Empty);
				if (num4 == 0)
				{
					num2 = endPosition.CharacterIndex + this.m_startCharacterOffset;
				}
				else if (num4 <= this.m_splitIndices.Count)
				{
					num2 = this.m_splitIndices[num4 - 1];
					num2 += endPosition.CharacterIndex;
				}
				RSTrace.RenderingTracer.Assert(num2 <= fullValue.Length, string.Empty);
			}
			glyphData = this.CreateGlyphData(previousRunCount, num, num2, fullValue, richTextRuns);
			int length = num2 - num;
			return fullValue.Substring(num, length);
		}

		internal TextRunItemizedData GetGlyphValue(string fullValue, int previousRunCount, List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			if (string.IsNullOrEmpty(fullValue))
			{
				return null;
			}
			return this.CreateGlyphData(previousRunCount, this.m_startCharacterOffset, fullValue.Length, fullValue, richTextRuns);
		}

		private TextRunItemizedData CreateGlyphData(int startIndex, int start, int end, string fullValue, List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			if (richTextRuns != null && richTextRuns.Count != 0)
			{
				List<int> list = new List<int>();
				List<TexRunShapeData> list2 = new List<TexRunShapeData>();
				if (start == 0 && end == fullValue.Length)
				{
					for (int i = 0; i < this.m_splitIndices.Count; i++)
					{
						list2.Add(new TexRunShapeData(richTextRuns[startIndex], true));
						startIndex++;
					}
					list2.Add(new TexRunShapeData(richTextRuns[startIndex], true));
					return new TextRunItemizedData(this.m_splitIndices, list2);
				}
				int startIndex2 = start - this.m_startCharacterOffset;
				if (this.m_splitIndices.Count > 0)
				{
					bool flag = true;
					bool flag2 = false;
					int num = 0;
					while (num < this.m_splitIndices.Count)
					{
						if (this.m_splitIndices[num] < end)
						{
							if (this.m_splitIndices[num] == start)
							{
								flag = false;
								startIndex++;
							}
							else if (this.m_splitIndices[num] < start)
							{
								startIndex++;
								flag = true;
							}
							else
							{
								list.Add(this.m_splitIndices[num] - start);
								if (flag)
								{
									if (start == 0)
									{
										list2.Add(new TexRunShapeData(richTextRuns[startIndex], true));
									}
									else
									{
										list2.Add(new TexRunShapeData(richTextRuns[startIndex], false, startIndex2));
									}
									flag = false;
									startIndex2 = 0;
								}
								else
								{
									list2.Add(new TexRunShapeData(richTextRuns[startIndex], true));
								}
								startIndex++;
							}
							num++;
							continue;
						}
						flag2 = true;
						if (this.m_splitIndices[num] > end)
						{
							list2.Add(new TexRunShapeData(richTextRuns[startIndex], false, startIndex2));
						}
						else
						{
							list2.Add(new TexRunShapeData(richTextRuns[startIndex], true));
						}
						break;
					}
					if (!flag2)
					{
						if (end < fullValue.Length)
						{
							list2.Add(new TexRunShapeData(richTextRuns[startIndex], false, startIndex2));
						}
						else
						{
							list2.Add(new TexRunShapeData(richTextRuns[startIndex], true));
						}
					}
				}
				else
				{
					list2.Add(new TexRunShapeData(richTextRuns[startIndex], false, startIndex2));
				}
				if (list.Count <= 0 && list2.Count <= 0)
				{
					return null;
				}
				return new TextRunItemizedData(list, list2);
			}
			return null;
		}

		private void WriteElementProps(RPLTextRunProps props, PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, int previousRunCount, List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			Hashtable itemPropsStart = pageContext.ItemPropsStart;
			string text = this.m_source.ID;
			bool flag = true;
			ReportStringProperty value = this.m_source.Value;
			if ((this.m_startCharacterOffset > 0 || endPosition != null || startPosition != null) && value != null && !this.m_source.FormattedValueExpressionBased)
			{
				text += "_NV";
				flag = false;
			}
			RPLTextRunPropsDef rPLTextRunPropsDef = pageContext.Common.GetFromCache<RPLTextRunPropsDef>(text, out itemPropsStart);
			TextRunItemizedData textRunItemizedData = null;
			if (rPLTextRunPropsDef == null)
			{
				rPLTextRunPropsDef = new RPLTextRunPropsDef();
				rPLTextRunPropsDef.SharedStyle = new RPLStyleProps();
				this.WriteSharedStyles(new StyleWriterOM(rPLTextRunPropsDef.SharedStyle), this.m_source.Style);
				if (this.m_source.Label != null)
				{
					rPLTextRunPropsDef.Label = this.m_source.Label;
				}
				if (this.m_source.MarkupType != null && !this.m_source.MarkupType.IsExpression)
				{
					rPLTextRunPropsDef.Markup = (RPLFormat.MarkupStyles)StyleEnumConverter.Translate(this.m_source.MarkupType.Value);
				}
				if (this.m_source.ToolTip != null && !this.m_source.ToolTip.IsExpression)
				{
					rPLTextRunPropsDef.ToolTip = this.m_source.ToolTip.Value;
				}
				if (flag && value != null && !this.m_source.FormattedValueExpressionBased && !hideDuplicates)
				{
					if (this.m_source.SharedTypeCode == TypeCode.String)
					{
						rPLTextRunPropsDef.Value = this.GetStringValue(value.Value, (TextBox.TextBoxOffset)null, (TextBox.TextBoxOffset)null, previousRunCount, richTextRuns, out textRunItemizedData);
					}
					else
					{
						rPLTextRunPropsDef.Value = this.GetStringValue(this.m_source.Instance.Value, (TextBox.TextBoxOffset)null, (TextBox.TextBoxOffset)null, previousRunCount, richTextRuns, out textRunItemizedData);
					}
				}
				rPLTextRunPropsDef.ID = text;
				itemPropsStart[text] = rPLTextRunPropsDef;
			}
			else if (richTextRuns != null && flag && !hideDuplicates && value != null && !this.m_source.FormattedValueExpressionBased)
			{
				textRunItemizedData = this.GetGlyphValue(value.Value, previousRunCount, richTextRuns);
			}
			props.Definition = rPLTextRunPropsDef;
			props.UniqueName = this.m_uniqueName;
			if (((int?)this.m_markup).HasValue)
			{
				props.Markup = (RPLFormat.MarkupStyles)this.m_markup.Value;
			}
			props.ToolTip = this.m_toolTip;
			TextRunItemizedData textRunItemizedData2 = null;
			if (!flag)
			{
				if (value != null && !hideDuplicates)
				{
					if (this.m_source.SharedTypeCode == TypeCode.String)
					{
						props.Value = this.GetStringValue(value.Value, startPosition, endPosition, previousRunCount, richTextRuns, out textRunItemizedData2);
					}
					else
					{
						props.Value = this.GetStringValue(this.m_source.Instance.Value, startPosition, endPosition, previousRunCount, richTextRuns, out textRunItemizedData2);
					}
				}
			}
			else
			{
				props.Value = this.GetStringValue(this.m_text, startPosition, endPosition, previousRunCount, richTextRuns, out textRunItemizedData2);
			}
			if (textRunItemizedData2 == null)
			{
				textRunItemizedData2 = textRunItemizedData;
			}
			pageContext.RegisterTextRunData(textRunItemizedData2);
			if (this.m_hyperlinks != null)
			{
				int count = this.m_hyperlinks.Count;
				props.ActionInfo = new RPLActionInfo(count);
				for (int i = 0; i < count; i++)
				{
					string text2 = this.m_hyperlinks[i];
					RPLAction rPLAction = new RPLAction();
					if (text2 != null)
					{
						rPLAction.Hyperlink = text2;
					}
					props.ActionInfo.Actions[i] = rPLAction;
				}
			}
			RPLStyleProps rPLStyleProps = null;
			if (this.m_styles != null)
			{
				rPLStyleProps = new RPLStyleProps();
				StyleWriterOM styleWriterOM = new StyleWriterOM(rPLStyleProps);
				styleWriterOM.WriteAll(this.m_styles);
			}
			props.NonSharedStyle = rPLStyleProps;
		}

		internal long WriteToStream(BinaryWriter writer, PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, int previousRunCount, List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			long position = writer.BaseStream.Position;
			writer.Write((byte)20);
			this.WriteElementProps(writer, pageContext, hideDuplicates, startPosition, endPosition, previousRunCount, richTextRuns);
			writer.Write((byte)255);
			return position;
		}

		private void WriteElementProps(BinaryWriter spbifWriter, PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, int previousRunCount, List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextRuns)
		{
			StyleWriterStream styleWriterStream = new StyleWriterStream(spbifWriter);
			string text = this.m_source.ID;
			bool flag = true;
			ReportStringProperty value = this.m_source.Value;
			if ((this.m_startCharacterOffset > 0 || endPosition != null || startPosition != null) && value != null && !this.m_source.FormattedValueExpressionBased)
			{
				text += "_NV";
				flag = false;
			}
			TextRunItemizedData textRunItemizedData = null;
			string value2 = null;
			Hashtable itemPropsStart = pageContext.ItemPropsStart;
			long primitiveFromCache = pageContext.Common.GetPrimitiveFromCache<long>(text, out itemPropsStart);
			if (primitiveFromCache <= 0)
			{
				primitiveFromCache = spbifWriter.BaseStream.Position;
				itemPropsStart[text] = primitiveFromCache;
				spbifWriter.Write((byte)15);
				spbifWriter.Write((byte)0);
				spbifWriter.Write((byte)6);
				spbifWriter.Write((byte)0);
				this.WriteSharedStyles(styleWriterStream, this.m_source.Style);
				spbifWriter.Write((byte)255);
				styleWriterStream.WriteNotNull(5, text);
				styleWriterStream.WriteNotNull(8, this.m_source.Label);
				if (this.m_source.MarkupType != null)
				{
					styleWriterStream.Write(7, StyleEnumConverter.Translate(this.m_source.MarkupType.Value));
				}
				styleWriterStream.WriteSharedProperty(9, this.m_source.ToolTip);
				if (flag && !hideDuplicates && value != null && !this.m_source.FormattedValueExpressionBased)
				{
					if (this.m_source.SharedTypeCode == TypeCode.String)
					{
						styleWriterStream.WriteNotNull(10, this.GetStringValue(value.Value, (TextBox.TextBoxOffset)null, (TextBox.TextBoxOffset)null, previousRunCount, richTextRuns, out textRunItemizedData));
					}
					else
					{
						styleWriterStream.WriteNotNull(10, this.GetStringValue(this.m_source.Instance.Value, (TextBox.TextBoxOffset)null, (TextBox.TextBoxOffset)null, previousRunCount, richTextRuns, out textRunItemizedData));
					}
				}
				spbifWriter.Write((byte)255);
			}
			else
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write((byte)2);
				spbifWriter.Write(primitiveFromCache);
				if (richTextRuns != null && flag && !hideDuplicates && value != null && !this.m_source.FormattedValueExpressionBased)
				{
					textRunItemizedData = this.GetGlyphValue(value.Value, previousRunCount, richTextRuns);
				}
			}
			spbifWriter.Write((byte)1);
			TextRunItemizedData textRunItemizedData2 = null;
			if (!flag)
			{
				if (!hideDuplicates && value != null)
				{
					value2 = ((this.m_source.SharedTypeCode != TypeCode.String) ? this.GetStringValue(this.m_source.Instance.Value, startPosition, endPosition, previousRunCount, richTextRuns, out textRunItemizedData2) : this.GetStringValue(value.Value, startPosition, endPosition, previousRunCount, richTextRuns, out textRunItemizedData2));
				}
			}
			else
			{
				value2 = this.GetStringValue(this.m_text, startPosition, endPosition, previousRunCount, richTextRuns, out textRunItemizedData2);
			}
			if (textRunItemizedData2 == null)
			{
				textRunItemizedData2 = textRunItemizedData;
			}
			pageContext.RegisterTextRunData(textRunItemizedData2);
			styleWriterStream.WriteNotNull(10, value2);
			styleWriterStream.WriteNotNull(9, this.m_toolTip);
			styleWriterStream.WriteNotNull(4, this.m_uniqueName);
			styleWriterStream.WriteNotNull(7, this.m_markup);
			spbifWriter.Write((byte)6);
			spbifWriter.Write((byte)1);
			if (this.m_styles != null)
			{
				styleWriterStream.WriteAll(this.m_styles);
			}
			spbifWriter.Write((byte)255);
			this.WriteActions(spbifWriter);
			spbifWriter.Write((byte)255);
			spbifWriter.Write((byte)255);
		}

		public void WriteNonSharedStyles(StyleWriter writer)
		{
			if (this.m_styles != null)
			{
				writer.WriteAll(this.m_styles);
			}
		}

		internal void WriteActions(BinaryWriter spbifWriter)
		{
			if (this.m_hyperlinks != null)
			{
				spbifWriter.Write((byte)11);
				spbifWriter.Write((byte)2);
				spbifWriter.Write(this.m_hyperlinks.Count);
				foreach (string hyperlink in this.m_hyperlinks)
				{
					spbifWriter.Write((byte)3);
					if (hyperlink != null)
					{
						spbifWriter.Write((byte)6);
						spbifWriter.Write(hyperlink);
					}
					spbifWriter.Write((byte)255);
				}
				spbifWriter.Write((byte)255);
			}
		}

		internal void WriteSharedStyles(StyleWriter writer, Style style)
		{
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.FontFamily);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.FontSize);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.FontStyle);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.FontWeight);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.TextDecoration);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.Color);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.Direction);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.Calendar);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.NumeralLanguage);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.NumeralVariant);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(TextRun.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Text:
					writer.Write(this.m_text);
					break;
				case MemberName.Style:
					writer.WriteByteVariantHashtable(this.m_styles);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.MarkupType:
					writer.Write(this.m_markup);
					break;
				case MemberName.Actions:
					writer.WriteListOfPrimitives(this.m_hyperlinks);
					break;
				case MemberName.UniqueName:
					writer.Write(this.m_uniqueName);
					break;
				case MemberName.Source:
					writer.Write(scalabilityCache.StoreStaticReference(this.m_source));
					break;
				case MemberName.Indexes:
					writer.WriteListOfPrimitives(this.m_splitIndices);
					break;
				case MemberName.Offset:
					writer.Write(this.m_startCharacterOffset);
					break;
				case MemberName.Key:
					writer.Write(this.m_fontKey);
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(TextRun.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Text:
					this.m_text = reader.ReadString();
					break;
				case MemberName.Style:
					this.m_styles = reader.ReadByteVariantHashtable<Dictionary<byte, object>>();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = reader.ReadString();
					break;
				case MemberName.MarkupType:
					this.m_markup = (byte?)(object)(reader.ReadVariant() as byte?);
					break;
				case MemberName.Actions:
					this.m_hyperlinks = reader.ReadListOfPrimitives<string>();
					break;
				case MemberName.UniqueName:
					this.m_uniqueName = reader.ReadString();
					break;
				case MemberName.Source:
					this.m_source = (AspNetCore.ReportingServices.OnDemandReportRendering.TextRun)scalabilityCache.FetchStaticReference(reader.ReadInt32());
					break;
				case MemberName.Indexes:
					this.m_splitIndices = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.Offset:
					this.m_startCharacterOffset = reader.ReadInt32();
					break;
				case MemberName.Key:
					this.m_fontKey = reader.ReadString();
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.TextRun;
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		internal static Declaration GetDeclaration()
		{
			if (TextRun.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Text, Token.String));
				list.Add(new MemberInfo(MemberName.Style, ObjectType.ByteVariantHashtable, Token.Object));
				list.Add(new MemberInfo(MemberName.ToolTip, Token.String));
				list.Add(new MemberInfo(MemberName.MarkupType, ObjectType.RIFObject));
				list.Add(new MemberInfo(MemberName.Actions, ObjectType.PrimitiveList, Token.String));
				list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
				list.Add(new MemberInfo(MemberName.Source, Token.Int32));
				list.Add(new MemberInfo(MemberName.Indexes, ObjectType.PrimitiveList, Token.Int32));
				list.Add(new MemberInfo(MemberName.Offset, Token.Int32));
				list.Add(new MemberInfo(MemberName.Key, Token.String));
				return new Declaration(ObjectType.TextRun, ObjectType.None, list);
			}
			return TextRun.m_declaration;
		}
	}
}
