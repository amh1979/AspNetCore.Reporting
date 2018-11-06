using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Paragraph : IStorable, IPersistable, IParagraphProps
	{
		[StaticReference]
		public static Declaration m_declaration = Paragraph.GetDeclaration();

		[StaticReference]
		private AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph m_source;

		private Dictionary<byte, object> m_styles;

		private RPLFormat.ListStyles? m_listStyle;

		private int? m_listLevel;

		private int m_paragraphNumber;

		private ReportSize m_spaceBefore;

		private ReportSize m_spaceAfter;

		private ReportSize m_leftIndent;

		private ReportSize m_rightIndent;

		private ReportSize m_hangingIndent;

		private string m_uniqueName;

		private List<TextRun> m_textRuns = new List<TextRun>();

		private bool m_firstLine = true;

		public List<TextRun> TextRuns
		{
			get
			{
				return this.m_textRuns;
			}
			set
			{
				this.m_textRuns = value;
			}
		}

		public int ParagraphNumber
		{
			get
			{
				return this.m_paragraphNumber;
			}
			set
			{
				this.m_paragraphNumber = value;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
		}

		public bool FirstLine
		{
			get
			{
				return this.m_firstLine;
			}
			set
			{
				this.m_firstLine = value;
			}
		}

		public RPLFormat.TextAlignments Alignment
		{
			get
			{
				byte? nullable = null;
				if (this.m_source != null)
				{
					nullable = Utility.GetEnumProp(25, StyleAttributeNames.TextAlign, this.m_source.Style, this.m_styles);
				}
				RPLFormat.TextAlignments result = RPLFormat.TextAlignments.General;
				if (((int?)nullable).HasValue)
				{
					result = (RPLFormat.TextAlignments)nullable.Value;
				}
				return result;
			}
		}

		public float SpaceBefore
		{
			get
			{
				if (this.m_source == null)
				{
					return 0f;
				}
				return (float)Utility.GetSizePropertyValue(this.m_source.SpaceBefore, this.m_spaceBefore);
			}
		}

		public float SpaceAfter
		{
			get
			{
				if (this.m_source == null)
				{
					return 0f;
				}
				return (float)Utility.GetSizePropertyValue(this.m_source.SpaceAfter, this.m_spaceAfter);
			}
		}

		public float LeftIndent
		{
			get
			{
				if (this.m_source == null)
				{
					return 0f;
				}
				return (float)Utility.GetSizePropertyValue(this.m_source.LeftIndent, this.m_leftIndent);
			}
		}

		public float RightIndent
		{
			get
			{
				if (this.m_source == null)
				{
					return 0f;
				}
				return (float)Utility.GetSizePropertyValue(this.m_source.RightIndent, this.m_rightIndent);
			}
		}

		public float HangingIndent
		{
			get
			{
				if (this.m_source == null)
				{
					return 0f;
				}
				return (float)Utility.GetSizePropertyValue(this.m_source.HangingIndent, this.m_hangingIndent);
			}
		}

		public int ListLevel
		{
			get
			{
				if (this.m_source == null)
				{
					return 0;
				}
				return Utility.GetIntPropertyValue(this.m_source.ListLevel, this.m_listLevel);
			}
		}

		public RPLFormat.ListStyles ListStyle
		{
			get
			{
				RPLFormat.ListStyles result = RPLFormat.ListStyles.None;
				if (this.m_listStyle.HasValue)
				{
					result = this.m_listStyle.Value;
				}
				else if (this.m_source != null && this.m_source.ListStyle != null)
				{
					result = (RPLFormat.ListStyles)StyleEnumConverter.Translate(this.m_source.ListStyle.Value);
				}
				return result;
			}
		}

		public int Size
		{
			get
			{
				return AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_textRuns) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_styles) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.NullableByteSize + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.NullableInt32Size + 4 + Utility.ReportSizeItemSize(this.m_spaceBefore) + Utility.ReportSizeItemSize(this.m_spaceAfter) + Utility.ReportSizeItemSize(this.m_leftIndent) + Utility.ReportSizeItemSize(this.m_rightIndent) + Utility.ReportSizeItemSize(this.m_hangingIndent) + 1 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_uniqueName) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize;
			}
		}

		internal Paragraph()
		{
		}

		internal Paragraph(string uniqueName)
		{
			this.m_uniqueName = uniqueName;
		}

		internal Paragraph(ParagraphInstance romParagraphInstance, string uniqueName, bool hideDuplicates)
		{
			this.m_source = romParagraphInstance.Definition;
			AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph source = this.m_source;
			Utility.AddInstanceStyles(romParagraphInstance.Style, ref this.m_styles);
			if (romParagraphInstance.IsCompiled)
			{
				this.m_spaceAfter = romParagraphInstance.SpaceAfter;
				this.m_spaceBefore = romParagraphInstance.SpaceBefore;
				this.m_leftIndent = romParagraphInstance.LeftIndent;
				this.m_rightIndent = romParagraphInstance.RightIndent;
				this.m_hangingIndent = romParagraphInstance.HangingIndent;
				this.m_listLevel = romParagraphInstance.ListLevel;
				this.m_listStyle = (RPLFormat.ListStyles)StyleEnumConverter.Translate(romParagraphInstance.ListStyle);
			}
			else
			{
				if (source.SpaceAfter != null && source.SpaceAfter.IsExpression)
				{
					this.m_spaceAfter = this.m_source.Instance.SpaceAfter;
				}
				if (source.SpaceBefore != null && source.SpaceBefore.IsExpression)
				{
					this.m_spaceBefore = this.m_source.Instance.SpaceBefore;
				}
				if (source.LeftIndent != null && source.LeftIndent.IsExpression)
				{
					this.m_leftIndent = this.m_source.Instance.LeftIndent;
				}
				if (source.RightIndent != null && source.RightIndent.IsExpression)
				{
					this.m_rightIndent = this.m_source.Instance.RightIndent;
				}
				if (source.HangingIndent != null && source.HangingIndent.IsExpression)
				{
					this.m_hangingIndent = this.m_source.Instance.HangingIndent;
				}
				if (source.ListLevel != null && source.ListLevel.IsExpression)
				{
					this.m_listLevel = this.m_source.Instance.ListLevel;
				}
				if (source.ListStyle != null && source.ListStyle.IsExpression)
				{
					this.m_listStyle = (RPLFormat.ListStyles)StyleEnumConverter.Translate(this.m_source.Instance.ListStyle);
				}
			}
			this.m_uniqueName = uniqueName;
			if (string.IsNullOrEmpty(uniqueName))
			{
				this.m_uniqueName = romParagraphInstance.UniqueName;
			}
			TextRunInstanceCollection textRunInstances = romParagraphInstance.TextRunInstances;
			foreach (TextRunInstance item in textRunInstances)
			{
				this.m_textRuns.Add(new TextRun(item, hideDuplicates));
			}
		}

		internal AspNetCore.ReportingServices.Rendering.RichText.Paragraph GetRichTextParagraph()
		{
			AspNetCore.ReportingServices.Rendering.RichText.Paragraph paragraph = new AspNetCore.ReportingServices.Rendering.RichText.Paragraph(this, this.m_textRuns.Count);
			foreach (TextRun textRun in this.m_textRuns)
			{
				AspNetCore.ReportingServices.Rendering.RichText.TextRun richTextRun = textRun.GetRichTextRun();
				if (richTextRun != null)
				{
					paragraph.Runs.Add(richTextRun);
				}
			}
			return paragraph;
		}

		internal RPLParagraph GetRPLParagraph(PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextParaRuns)
		{
			RPLParagraph rPLParagraph = new RPLParagraph();
			this.WriteElementProps(rPLParagraph.ElementProps as RPLParagraphProps, pageContext);
			int num = 0;
			int num2 = 0;
			if (endPosition == null && startPosition == null)
			{
				{
					foreach (TextRun textRun2 in this.m_textRuns)
					{
						num = num2;
						num2 += textRun2.SplitIndices.Count + 1;
						RPLTextRun rPLTextRun = textRun2.GetRPLTextRun(pageContext, hideDuplicates, null, null, num, richTextParaRuns);
						rPLParagraph.AddTextRun(rPLTextRun);
					}
					return rPLParagraph;
				}
			}
			int count = this.m_textRuns.Count;
			int num3 = -1;
			if (startPosition != null)
			{
				num3 = startPosition.TextRunIndex;
			}
			int num4 = -1;
			if (endPosition != null)
			{
				num4 = endPosition.TextRunIndex;
			}
			for (int i = 0; i < count; i++)
			{
				TextRun textRun = this.m_textRuns[i];
				num = num2;
				num2 += textRun.SplitIndices.Count + 1;
				if (num3 < num2)
				{
					if (endPosition != null)
					{
						if (num2 < num4)
						{
							rPLParagraph.AddTextRun(textRun.GetRPLTextRun(pageContext, hideDuplicates, startPosition, null, num, richTextParaRuns));
							continue;
						}
						if (num2 == num4)
						{
							rPLParagraph.AddTextRun(textRun.GetRPLTextRun(pageContext, hideDuplicates, startPosition, null, num, richTextParaRuns));
							if (endPosition.CharacterIndex <= 0)
							{
								break;
							}
							continue;
						}
						rPLParagraph.AddTextRun(textRun.GetRPLTextRun(pageContext, hideDuplicates, startPosition, endPosition, num, richTextParaRuns));
						break;
					}
					rPLParagraph.AddTextRun(textRun.GetRPLTextRun(pageContext, hideDuplicates, startPosition, null, num, richTextParaRuns));
				}
			}
			return rPLParagraph;
		}

		internal void WriteElementProps(RPLParagraphProps elemProps, PageContext pageContext)
		{
			Hashtable itemPropsStart = pageContext.ItemPropsStart;
			RPLParagraphPropsDef rPLParagraphPropsDef = pageContext.Common.GetFromCache<RPLParagraphPropsDef>(this.m_source.ID, out itemPropsStart);
			if (rPLParagraphPropsDef == null)
			{
				rPLParagraphPropsDef = new RPLParagraphPropsDef();
				rPLParagraphPropsDef.SharedStyle = new RPLStyleProps();
				this.WriteSharedStyles(new StyleWriterOM(rPLParagraphPropsDef.SharedStyle), this.m_source.Style);
				if (this.m_source.ListLevel != null && !this.m_source.ListLevel.IsExpression)
				{
					rPLParagraphPropsDef.ListLevel = this.m_source.ListLevel.Value;
				}
				if (this.m_source.ListStyle != null && !this.m_source.ListStyle.IsExpression)
				{
					rPLParagraphPropsDef.ListStyle = (RPLFormat.ListStyles)StyleEnumConverter.Translate(this.m_source.ListStyle.Value);
				}
				if (this.m_source.LeftIndent != null && !this.m_source.LeftIndent.IsExpression)
				{
					rPLParagraphPropsDef.LeftIndent = new RPLReportSize(this.m_source.LeftIndent.Value.ToString());
				}
				if (this.m_source.RightIndent != null && !this.m_source.RightIndent.IsExpression)
				{
					rPLParagraphPropsDef.RightIndent = new RPLReportSize(this.m_source.RightIndent.Value.ToString());
				}
				if (this.m_source.HangingIndent != null && !this.m_source.HangingIndent.IsExpression)
				{
					rPLParagraphPropsDef.HangingIndent = new RPLReportSize(this.m_source.HangingIndent.Value.ToString());
				}
				if (this.m_source.SpaceBefore != null && !this.m_source.SpaceBefore.IsExpression)
				{
					rPLParagraphPropsDef.SpaceBefore = new RPLReportSize(this.m_source.SpaceBefore.Value.ToString());
				}
				if (this.m_source.SpaceAfter != null && !this.m_source.SpaceAfter.IsExpression)
				{
					rPLParagraphPropsDef.SpaceAfter = new RPLReportSize(this.m_source.SpaceAfter.Value.ToString());
				}
				rPLParagraphPropsDef.ID = this.m_source.ID;
				itemPropsStart[this.m_source.ID] = rPLParagraphPropsDef;
			}
			elemProps.Definition = rPLParagraphPropsDef;
			if (this.m_leftIndent != null)
			{
				elemProps.LeftIndent = new RPLReportSize(this.m_leftIndent.ToString());
			}
			if (this.m_rightIndent != null)
			{
				elemProps.RightIndent = new RPLReportSize(this.m_rightIndent.ToString());
			}
			if (this.m_hangingIndent != null)
			{
				elemProps.HangingIndent = new RPLReportSize(this.m_hangingIndent.ToString());
			}
			if (this.m_listStyle.HasValue)
			{
				elemProps.ListStyle = this.m_listStyle.Value;
			}
			if (this.m_listLevel.HasValue)
			{
				elemProps.ListLevel = this.m_listLevel.Value;
			}
			if (this.m_spaceBefore != null)
			{
				elemProps.SpaceBefore = new RPLReportSize(this.m_spaceBefore.ToString());
			}
			if (this.m_spaceAfter != null)
			{
				elemProps.SpaceAfter = new RPLReportSize(this.m_spaceAfter.ToString());
			}
			elemProps.ParagraphNumber = this.m_paragraphNumber;
			elemProps.FirstLine = this.m_firstLine;
			elemProps.UniqueName = this.m_uniqueName;
			RPLStyleProps rPLStyleProps = null;
			if (this.m_styles != null)
			{
				rPLStyleProps = new RPLStyleProps();
				StyleWriter styleWriter = new StyleWriterOM(rPLStyleProps);
				styleWriter.WriteAll(this.m_styles);
			}
			elemProps.NonSharedStyle = rPLStyleProps;
		}

		internal void WriteSharedStyles(StyleWriter writer, Style style)
		{
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.TextAlign);
			Utility.WriteStyleProperty(writer, style, StyleAttributeNames.LineHeight);
		}

		internal long WriteToStream(BinaryWriter writer, PageContext pageContext, bool hideDuplicates, TextBox.TextBoxOffset startPosition, TextBox.TextBoxOffset endPosition, List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextParaRuns)
		{
			int num = 0;
			int num2 = 0;
			List<long> list = new List<long>();
			if (endPosition == null && startPosition == null)
			{
				foreach (TextRun textRun in this.m_textRuns)
				{
					num2 = num;
					num += textRun.SplitIndices.Count + 1;
					long item = textRun.WriteToStream(writer, pageContext, hideDuplicates, null, null, num2, richTextParaRuns);
					list.Add(item);
				}
			}
			else
			{
				int num3 = -1;
				if (startPosition != null)
				{
					num3 = startPosition.TextRunIndex;
				}
				int num4 = -1;
				if (endPosition != null)
				{
					num4 = endPosition.TextRunIndex;
				}
				long num5 = 0L;
				foreach (TextRun textRun2 in this.m_textRuns)
				{
					num2 = num;
					num += textRun2.SplitIndices.Count + 1;
					if (num3 < num)
					{
						if (endPosition != null)
						{
							if (num < num4)
							{
								num5 = textRun2.WriteToStream(writer, pageContext, hideDuplicates, startPosition, null, num2, richTextParaRuns);
								list.Add(num5);
								continue;
							}
							if (num == num4)
							{
								num5 = textRun2.WriteToStream(writer, pageContext, hideDuplicates, startPosition, null, num2, richTextParaRuns);
								list.Add(num5);
								if (endPosition.CharacterIndex <= 0)
								{
									break;
								}
								continue;
							}
							num5 = textRun2.WriteToStream(writer, pageContext, hideDuplicates, startPosition, endPosition, num2, richTextParaRuns);
							list.Add(num5);
							break;
						}
						num5 = textRun2.WriteToStream(writer, pageContext, hideDuplicates, startPosition, null, num2, richTextParaRuns);
						list.Add(num5);
					}
				}
			}
			long position = writer.BaseStream.Position;
			writer.Write((byte)19);
			if (this.m_source != null)
			{
				this.WriteElementProps(writer, pageContext);
			}
			else
			{
				writer.Write((byte)15);
				writer.Write((byte)0);
				writer.Write((byte)255);
				writer.Write((byte)1);
				writer.Write((byte)255);
				writer.Write((byte)255);
			}
			writer.Write(list.Count);
			foreach (long item2 in list)
			{
				writer.Write(item2);
			}
			writer.Write((byte)255);
			return position;
		}

		internal void WriteElementProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
			StyleWriterStream styleWriterStream = new StyleWriterStream(spbifWriter);
			Hashtable itemPropsStart = pageContext.ItemPropsStart;
			long primitiveFromCache = pageContext.Common.GetPrimitiveFromCache<long>(this.m_source.ID, out itemPropsStart);
			if (primitiveFromCache <= 0)
			{
				primitiveFromCache = spbifWriter.BaseStream.Position;
				itemPropsStart[this.m_source.ID] = primitiveFromCache;
				spbifWriter.Write((byte)15);
				spbifWriter.Write((byte)0);
				spbifWriter.Write((byte)6);
				spbifWriter.Write((byte)0);
				this.WriteSharedStyles(styleWriterStream, this.m_source.Style);
				spbifWriter.Write((byte)255);
				styleWriterStream.WriteSharedProperty(8, this.m_source.ListLevel);
				if (this.m_source.ListStyle != null)
				{
					styleWriterStream.Write(7, StyleEnumConverter.Translate(this.m_source.ListStyle.Value));
				}
				styleWriterStream.WriteSharedProperty(9, this.m_source.LeftIndent);
				styleWriterStream.WriteSharedProperty(10, this.m_source.RightIndent);
				styleWriterStream.WriteSharedProperty(11, this.m_source.HangingIndent);
				styleWriterStream.WriteSharedProperty(12, this.m_source.SpaceBefore);
				styleWriterStream.WriteSharedProperty(13, this.m_source.SpaceAfter);
				styleWriterStream.WriteNotNull(5, this.m_source.ID);
				spbifWriter.Write((byte)255);
			}
			else
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write((byte)2);
				spbifWriter.Write(primitiveFromCache);
			}
			spbifWriter.Write((byte)1);
			Utility.WriteReportSize(spbifWriter, 9, this.m_leftIndent);
			Utility.WriteReportSize(spbifWriter, 10, this.m_rightIndent);
			Utility.WriteReportSize(spbifWriter, 11, this.m_hangingIndent);
			if (this.m_listStyle.HasValue)
			{
				styleWriterStream.Write(7, (byte)this.m_listStyle.Value);
			}
			if (this.m_listLevel.HasValue)
			{
				styleWriterStream.Write(8, this.m_listLevel.Value);
			}
			if (!this.m_firstLine)
			{
				styleWriterStream.Write(15, this.m_firstLine);
			}
			Utility.WriteReportSize(spbifWriter, 12, this.m_spaceBefore);
			Utility.WriteReportSize(spbifWriter, 13, this.m_spaceAfter);
			styleWriterStream.Write(14, this.m_paragraphNumber);
			spbifWriter.Write((byte)6);
			spbifWriter.Write((byte)1);
			this.WriteNonSharedStyles(styleWriterStream);
			spbifWriter.Write((byte)255);
			spbifWriter.Write((byte)4);
			spbifWriter.Write(this.m_uniqueName);
			spbifWriter.Write((byte)255);
			spbifWriter.Write((byte)255);
		}

		internal void WriteNonSharedStyles(StyleWriter writer)
		{
			if (this.m_styles != null)
			{
				writer.WriteAll(this.m_styles);
			}
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Paragraph.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.TextRuns:
					writer.Write(this.m_textRuns);
					break;
				case MemberName.Style:
					writer.WriteByteVariantHashtable(this.m_styles);
					break;
				case MemberName.ListStyle:
					writer.Write((byte?)this.m_listStyle);
					break;
				case MemberName.ListLevel:
					writer.Write(this.m_listLevel);
					break;
				case MemberName.ParagraphNumber:
					writer.Write(this.m_paragraphNumber);
					break;
				case MemberName.SpaceBefore:
					Utility.WriteReportSize(writer, this.m_spaceBefore);
					break;
				case MemberName.SpaceAfter:
					Utility.WriteReportSize(writer, this.m_spaceAfter);
					break;
				case MemberName.LeftIndent:
					Utility.WriteReportSize(writer, this.m_leftIndent);
					break;
				case MemberName.RightIndent:
					Utility.WriteReportSize(writer, this.m_rightIndent);
					break;
				case MemberName.HangingIndent:
					Utility.WriteReportSize(writer, this.m_hangingIndent);
					break;
				case MemberName.FirstLine:
					writer.Write(this.m_firstLine);
					break;
				case MemberName.UniqueName:
					writer.Write(this.m_uniqueName);
					break;
				case MemberName.Source:
					writer.Write(scalabilityCache.StoreStaticReference(this.m_source));
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Paragraph.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.TextRuns:
					this.m_textRuns = reader.ReadGenericListOfRIFObjects<TextRun>();
					break;
				case MemberName.Style:
					this.m_styles = reader.ReadByteVariantHashtable<Dictionary<byte, object>>();
					break;
				case MemberName.ListStyle:
					this.m_listStyle = (RPLFormat.ListStyles?)reader.ReadNullable<byte>();
					break;
				case MemberName.ListLevel:
					this.m_listLevel = reader.ReadNullable<int>();
					break;
				case MemberName.ParagraphNumber:
					this.m_paragraphNumber = reader.ReadInt32();
					break;
				case MemberName.SpaceBefore:
					this.m_spaceBefore = Utility.ReadReportSize(reader);
					break;
				case MemberName.SpaceAfter:
					this.m_spaceAfter = Utility.ReadReportSize(reader);
					break;
				case MemberName.LeftIndent:
					this.m_leftIndent = Utility.ReadReportSize(reader);
					break;
				case MemberName.RightIndent:
					this.m_rightIndent = Utility.ReadReportSize(reader);
					break;
				case MemberName.HangingIndent:
					this.m_hangingIndent = Utility.ReadReportSize(reader);
					break;
				case MemberName.FirstLine:
					this.m_firstLine = reader.ReadBoolean();
					break;
				case MemberName.UniqueName:
					this.m_uniqueName = reader.ReadString();
					break;
				case MemberName.Source:
					this.m_source = (AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph)scalabilityCache.FetchStaticReference(reader.ReadInt32());
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public ObjectType GetObjectType()
		{
			return ObjectType.Paragraph;
		}

		public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		internal static Declaration GetDeclaration()
		{
			if (Paragraph.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.TextRuns, ObjectType.RIFObjectList, ObjectType.TextRun));
				list.Add(new MemberInfo(MemberName.Style, ObjectType.ByteVariantHashtable, Token.Object));
				list.Add(new MemberInfo(MemberName.ListStyle, ObjectType.Nullable, Token.Byte));
				list.Add(new MemberInfo(MemberName.ListLevel, ObjectType.Nullable, Token.Int32));
				list.Add(new MemberInfo(MemberName.ParagraphNumber, Token.Int32));
				list.Add(new MemberInfo(MemberName.SpaceBefore, Token.String));
				list.Add(new MemberInfo(MemberName.SpaceAfter, Token.String));
				list.Add(new MemberInfo(MemberName.LeftIndent, Token.String));
				list.Add(new MemberInfo(MemberName.RightIndent, Token.String));
				list.Add(new MemberInfo(MemberName.HangingIndent, Token.String));
				list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
				list.Add(new MemberInfo(MemberName.Source, Token.Int32));
				list.Add(new MemberInfo(MemberName.FirstLine, Token.Boolean));
				return new Declaration(ObjectType.Paragraph, ObjectType.None, list);
			}
			return Paragraph.m_declaration;
		}
	}
}
