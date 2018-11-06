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
	internal sealed class TextBox : PageItem, ITextBoxProps
	{
		internal enum CalcSize : byte
		{
			None,
			Done,
			Delay,
			LateCalc
		}

		internal sealed class TextBoxOffset : IStorable, IPersistable
		{
			private int m_paragraphIndex;

			private int m_textRunIndex;

			private int m_characterIndex;

			[StaticReference]
			internal static Declaration m_declaration = TextBoxOffset.GetDeclaration();

			public int ParagraphIndex
			{
				get
				{
					return this.m_paragraphIndex;
				}
				set
				{
					this.m_paragraphIndex = value;
				}
			}

			public int TextRunIndex
			{
				get
				{
					return this.m_textRunIndex;
				}
				set
				{
					this.m_textRunIndex = value;
				}
			}

			public int CharacterIndex
			{
				get
				{
					return this.m_characterIndex;
				}
				set
				{
					this.m_characterIndex = value;
				}
			}

			public int Size
			{
				get
				{
					return 12;
				}
			}

			public TextBoxOffset()
			{
			}

			public TextBoxOffset(int defaultValue)
			{
				this.Reset(defaultValue);
			}

			public void Reset(int defaultValue)
			{
				this.ParagraphIndex = defaultValue;
				this.TextRunIndex = defaultValue;
				this.CharacterIndex = defaultValue;
			}

			public void Set(TextBoxContext context)
			{
				this.ParagraphIndex = context.ParagraphIndex;
				this.TextRunIndex = context.TextRunIndex;
				this.CharacterIndex = context.TextRunCharacterIndex;
			}

			public void Serialize(IntermediateFormatWriter writer)
			{
				writer.RegisterDeclaration(TextBoxOffset.m_declaration);
				while (writer.NextMember())
				{
					switch (writer.CurrentMember.MemberName)
					{
					case MemberName.ParagraphIndex:
						writer.Write(this.m_paragraphIndex);
						break;
					case MemberName.TextRunIndex:
						writer.Write(this.m_textRunIndex);
						break;
					case MemberName.CharacterIndex:
						writer.Write(this.m_characterIndex);
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public void Deserialize(IntermediateFormatReader reader)
			{
				reader.RegisterDeclaration(TextBoxOffset.m_declaration);
				while (reader.NextMember())
				{
					switch (reader.CurrentMember.MemberName)
					{
					case MemberName.ParagraphIndex:
						this.m_paragraphIndex = reader.ReadInt32();
						break;
					case MemberName.TextRunIndex:
						this.m_textRunIndex = reader.ReadInt32();
						break;
					case MemberName.CharacterIndex:
						this.m_characterIndex = reader.ReadInt32();
						break;
					default:
						RSTrace.RenderingTracer.Assert(false, string.Empty);
						break;
					}
				}
			}

			public ObjectType GetObjectType()
			{
				return ObjectType.TextBoxOffset;
			}

			public void ResolveReferences(Dictionary<ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
			{
			}

			internal static Declaration GetDeclaration()
			{
				if (TextBoxOffset.m_declaration == null)
				{
					List<MemberInfo> list = new List<MemberInfo>();
					list.Add(new MemberInfo(MemberName.ParagraphIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.TextRunIndex, Token.Int32));
					list.Add(new MemberInfo(MemberName.CharacterIndex, Token.Int32));
					return new Declaration(ObjectType.TextBoxOffset, ObjectType.None, list);
				}
				return TextBoxOffset.m_declaration;
			}
		}

		private CalcSize m_calcSizeState;

		private double m_padHorizontal;

		private double m_padVertical;

		private double m_padTop;

		private List<Paragraph> m_paragraphs = new List<Paragraph>();

		[StaticReference]
		private AspNetCore.ReportingServices.Rendering.RichText.TextBox m_richTextBox;

		private TextBoxState m_textBoxState = new TextBoxState();

		private TextBoxOffset m_pageStartOffset = new TextBoxOffset(0);

		private TextBoxOffset m_pageEndOffset = new TextBoxOffset(-1);

		private TextBoxOffset m_nextPageStartOffset = new TextBoxOffset(-1);

		private double m_contentOffset;

		private double m_contentBottom;

		private float m_contentHeight;

		[StaticReference]
		private static Declaration m_declaration = TextBox.GetDeclaration();

		internal CalcSize CalcSizeState
		{
			get
			{
				return this.m_calcSizeState;
			}
			set
			{
				this.m_calcSizeState = value;
			}
		}

		internal override string SourceID
		{
			get
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
				if (textBox != null)
				{
					if (this.m_textBoxState.SpanPages && textBox.IsSimple && !textBox.FormattedValueExpressionBased)
					{
						return base.m_source.ID + "_NV";
					}
					return base.m_source.ID;
				}
				return base.m_source.ID + "_NR";
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + 1 + 8 + 8 + 8 + 1 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_paragraphs) + 8 + 8 + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_pageStartOffset) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_pageEndOffset) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(this.m_nextPageStartOffset) + AspNetCore.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.ReferenceSize;
			}
		}

		public RPLFormat.TextAlignments DefaultAlignment
		{
			get
			{
				return this.m_textBoxState.DefaultTextAlign;
			}
		}

		public RPLFormat.Directions Direction
		{
			get
			{
				return this.m_textBoxState.Direction;
			}
		}

		public RPLFormat.WritingModes WritingMode
		{
			get
			{
				return this.m_textBoxState.WritingMode;
			}
		}

		public bool VerticalText
		{
			get
			{
				return this.m_textBoxState.VerticalText;
			}
		}

		public bool HorizontalText
		{
			get
			{
				return this.m_textBoxState.HorizontalText;
			}
		}

		public Color BackgroundColor
		{
			get
			{
				return Color.Empty;
			}
		}

		public bool CanGrow
		{
			get
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
				if (textBox != null)
				{
					if (this.m_textBoxState.SpanPages)
					{
						return false;
					}
					return textBox.CanGrow;
				}
				return false;
			}
		}

		internal TextBox()
		{
		}

		private TextBox(ReportItem source, PageContext pageContext)
			: base(source)
		{
			base.m_itemPageSizes = new ItemSizes(source);
			this.InitParagraphs();
		}

		internal TextBox(AspNetCore.ReportingServices.OnDemandReportRendering.TextBox source, PageContext pageContext)
			: this((ReportItem)source, pageContext)
		{
			base.KeepTogetherVertical = source.KeepTogether;
			base.KeepTogetherHorizontal = source.KeepTogether;
			bool unresolvedKTH = base.UnresolvedKTV = source.KeepTogether;
			base.UnresolvedKTH = unresolvedKTH;
			if (source.HideDuplicates)
			{
				base.Duplicate = ((TextBoxInstance)source.Instance).Duplicate;
				if (pageContext.TextBoxDuplicates == null)
				{
					pageContext.TextBoxDuplicates = new Hashtable();
					pageContext.TextBoxDuplicates.Add(source.ID, null);
					base.Duplicate = false;
				}
				else if (!pageContext.TextBoxDuplicates.ContainsKey(source.ID))
				{
					pageContext.TextBoxDuplicates.Add(source.ID, null);
					base.Duplicate = false;
				}
			}
			this.CreateFullStyle(pageContext);
		}

		internal TextBox(DataRegion source, PageContext pageContext)
			: this((ReportItem)source, pageContext)
		{
			if (source is AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.Tablix tablix = (AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)source;
				base.KeepTogetherVertical = tablix.KeepTogether;
				base.KeepTogetherHorizontal = tablix.KeepTogether;
				bool unresolvedKTH = base.UnresolvedKTV = tablix.KeepTogether;
				base.UnresolvedKTH = unresolvedKTH;
			}
			else
			{
				base.KeepTogetherVertical = true;
				base.KeepTogetherHorizontal = true;
				bool unresolvedKTH2 = base.UnresolvedKTV = true;
				base.UnresolvedKTH = unresolvedKTH2;
			}
			this.CreateFullStyle(pageContext);
		}

		internal TextBox(AspNetCore.ReportingServices.OnDemandReportRendering.SubReport source, PageContext pageContext)
			: this((ReportItem)source, pageContext)
		{
			base.KeepTogetherVertical = source.KeepTogether;
			base.KeepTogetherHorizontal = source.KeepTogether;
			bool unresolvedKTH = base.UnresolvedKTV = source.KeepTogether;
			base.UnresolvedKTH = unresolvedKTH;
			this.CreateFullStyle(pageContext);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(TextBox.m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CalcSizeState:
					writer.Write((byte)this.m_calcSizeState);
					break;
				case MemberName.HorizontalPadding:
					writer.Write(this.m_padHorizontal);
					break;
				case MemberName.VerticalPadding:
					writer.Write(this.m_padVertical);
					break;
				case MemberName.TopPadding:
					writer.Write(this.m_padTop);
					break;
				case MemberName.Paragraphs:
					writer.Write(this.m_paragraphs);
					break;
				case MemberName.TextBox:
					writer.Write(scalabilityCache.StoreStaticReference(this.m_richTextBox));
					break;
				case MemberName.State:
					writer.Write(this.m_textBoxState.State);
					break;
				case MemberName.ContentOffset:
					writer.Write(this.m_contentOffset);
					break;
				case MemberName.ContentBottom:
					writer.Write(this.m_contentBottom);
					break;
				case MemberName.ContentHeight:
					writer.Write(this.m_contentHeight);
					break;
				case MemberName.PageStartOffset:
					writer.Write(this.m_pageStartOffset);
					break;
				case MemberName.PageEndOffset:
					writer.Write(this.m_pageEndOffset);
					break;
				case MemberName.NextPageStartOffset:
					writer.Write(this.m_nextPageStartOffset);
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(TextBox.m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CalcSizeState:
					this.m_calcSizeState = (CalcSize)reader.ReadByte();
					break;
				case MemberName.HorizontalPadding:
					this.m_padHorizontal = reader.ReadDouble();
					break;
				case MemberName.VerticalPadding:
					this.m_padVertical = reader.ReadDouble();
					break;
				case MemberName.TopPadding:
					this.m_padTop = reader.ReadDouble();
					break;
				case MemberName.Paragraphs:
					this.m_paragraphs = reader.ReadGenericListOfRIFObjects<Paragraph>();
					break;
				case MemberName.TextBox:
					this.m_richTextBox = (AspNetCore.ReportingServices.Rendering.RichText.TextBox)scalabilityCache.FetchStaticReference(reader.ReadInt32());
					break;
				case MemberName.State:
					this.m_textBoxState.State = reader.ReadByte();
					break;
				case MemberName.ContentBottom:
					this.m_contentBottom = reader.ReadDouble();
					break;
				case MemberName.ContentOffset:
					this.m_contentOffset = reader.ReadDouble();
					break;
				case MemberName.ContentHeight:
					this.m_contentHeight = reader.ReadSingle();
					break;
				case MemberName.PageStartOffset:
					this.m_pageStartOffset = reader.ReadRIFObject<TextBoxOffset>();
					break;
				case MemberName.PageEndOffset:
					this.m_pageEndOffset = reader.ReadRIFObject<TextBoxOffset>();
					break;
				case MemberName.NextPageStartOffset:
					this.m_nextPageStartOffset = reader.ReadRIFObject<TextBoxOffset>();
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.TextBox;
		}

		internal new static Declaration GetDeclaration()
		{
			if (TextBox.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.CalcSizeState, Token.Byte));
				list.Add(new MemberInfo(MemberName.HorizontalPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.VerticalPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.TopPadding, Token.Double));
				list.Add(new MemberInfo(MemberName.Paragraphs, ObjectType.RIFObjectList, ObjectType.Paragraph));
				list.Add(new MemberInfo(MemberName.TextBox, Token.Int32));
				list.Add(new MemberInfo(MemberName.State, Token.Byte));
				list.Add(new MemberInfo(MemberName.ContentOffset, Token.Double));
				list.Add(new MemberInfo(MemberName.ContentBottom, Token.Double));
				list.Add(new MemberInfo(MemberName.ContentHeight, Token.Single));
				list.Add(new MemberInfo(MemberName.PageStartOffset, ObjectType.TextBoxOffset));
				list.Add(new MemberInfo(MemberName.PageEndOffset, ObjectType.TextBoxOffset));
				list.Add(new MemberInfo(MemberName.NextPageStartOffset, ObjectType.TextBoxOffset));
				return new Declaration(ObjectType.TextBox, ObjectType.PageItem, list);
			}
			return TextBox.m_declaration;
		}

		private bool GetAlignmentRight(Style style, StyleInstance styleInstance)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && textBox.IsSimple)
			{
				RPLFormat.TextAlignments textAlignments = RPLFormat.TextAlignments.General;
				AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph paragraph = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0];
				byte? nonCompiledEnumProp = Utility.GetNonCompiledEnumProp(25, StyleAttributeNames.TextAlign, style, styleInstance);
				if (((int?)nonCompiledEnumProp).HasValue)
				{
					textAlignments = (RPLFormat.TextAlignments)nonCompiledEnumProp.Value;
				}
				switch (textAlignments)
				{
				case RPLFormat.TextAlignments.General:
				{
					TypeCode typeCode = textBox.SharedTypeCode;
					if (typeCode == TypeCode.Object)
					{
						TextBoxInstance textBoxInstance = (TextBoxInstance)textBox.Instance;
						typeCode = textBoxInstance.TypeCode;
					}
					bool result = false;
					switch (typeCode)
					{
					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
					case TypeCode.DateTime:
						result = true;
						break;
					}
					return result;
				}
				case RPLFormat.TextAlignments.Right:
					return true;
				default:
					return false;
				}
			}
			return false;
		}

		private void CreateFullStyle(PageContext pageContext)
		{
			PaddingsStyle paddingsStyle = null;
			if (pageContext.ItemPaddingsStyle != null)
			{
				paddingsStyle = (PaddingsStyle)pageContext.ItemPaddingsStyle[base.m_source.ID];
			}
			if (paddingsStyle != null)
			{
				paddingsStyle.GetPaddingValues(base.m_source, out this.m_padVertical, out this.m_padHorizontal, out this.m_padTop);
			}
			else
			{
				PaddingsStyle.CreatePaddingsStyle(pageContext, base.m_source, out this.m_padVertical, out this.m_padHorizontal, out this.m_padTop);
			}
		}

		private void CalculateVerticalSize(PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return;
			}
			bool flag = !textBox.CanShrink;
			float num = 0f;
			double num5;
			if (textBox.CanGrow || textBox.CanShrink)
			{
				if (this.m_richTextBox == null)
				{
					this.m_richTextBox = this.GetRichTextBox();
				}
				if (this.m_richTextBox != null)
				{
					double num2 = base.m_itemPageSizes.Width - this.m_padHorizontal;
					if (num2 > 0.0)
					{
						double num3 = base.m_itemPageSizes.Height - this.m_padVertical;
						if (textBox.CanGrow || num3 > 0.0)
						{
							FlowContext flowContext = new FlowContext((float)num2, 3.40282347E+38f);
							flowContext.Updatable = false;
							if (!textBox.CanShrink)
							{
								flowContext.Height = (float)num3;
							}
							double num4 = (double)pageContext.Common.MeasureFullTextBoxHeight(this.m_richTextBox, flowContext, out num);
							num5 = num4 - (base.m_itemPageSizes.Height - this.m_padVertical);
							if (this.HorizontalText)
							{
								flag = ((byte)((num5 < 0.0 && !textBox.CanShrink) ? 1 : 0) != 0);
							}
							else
							{
								double num6 = (double)num - num2;
								flag = ((byte)((num6 < 0.0) ? 1 : 0) != 0);
							}
							if (num5 > 0.0 && textBox.CanGrow)
							{
								goto IL_0161;
							}
							if (num5 < 0.0 && textBox.CanShrink)
							{
								goto IL_0161;
							}
						}
					}
				}
				else if (textBox.CanShrink)
				{
					base.m_itemPageSizes.AdjustHeightTo(0.0);
				}
			}
			goto IL_01a2;
			IL_01a2:
			if (flag && this.m_textBoxState.VerticalAlignment != 0)
			{
				if (this.m_richTextBox == null)
				{
					this.m_richTextBox = this.GetRichTextBox();
				}
				if (this.m_richTextBox != null)
				{
					double num7 = base.m_itemPageSizes.Width - this.m_padHorizontal;
					if (num == 0.0)
					{
						FlowContext flowContext2 = new FlowContext((float)num7, (float)(base.m_itemPageSizes.Height - this.m_padVertical));
						flowContext2.Updatable = false;
						pageContext.Common.MeasureFullTextBoxHeight(this.m_richTextBox, flowContext2, out num);
					}
				}
			}
			this.m_contentHeight = num;
			return;
			IL_0161:
			base.m_itemPageSizes.AdjustHeightTo(base.m_itemPageSizes.Height + num5 + 0.0001);
			goto IL_01a2;
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (this.m_calcSizeState != CalcSize.Done && this.m_calcSizeState != CalcSize.Delay)
			{
				this.CalculateVerticalSize(pageContext);
				this.m_calcSizeState = CalcSize.Done;
			}
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			if (this.m_textBoxState.SpanPages && this.m_textBoxState.ResetHorizontalState)
			{
				RoundedDouble x = new RoundedDouble(base.ItemPageSizes.Left);
				if (x >= leftInParentSystem)
				{
					this.UpdateState();
					this.m_textBoxState.ResetHorizontalState = false;
				}
			}
		}

		private void CalculateOffsetFromHeight()
		{
			if (this.m_contentHeight > 0.0 && !this.m_textBoxState.SpanPages)
			{
				RPLFormat.VerticalAlignments verticalAlignment = this.m_textBoxState.VerticalAlignment;
				if (verticalAlignment != 0)
				{
					bool horizontalText = this.HorizontalText;
					if (!((double)this.m_contentHeight < base.m_itemPageSizes.Height) || !horizontalText)
					{
						if (!((double)this.m_contentHeight < base.m_itemPageSizes.Width))
						{
							return;
						}
						if (horizontalText)
						{
							return;
						}
					}
					float contentHeight = this.m_contentHeight;
					double num = base.m_itemPageSizes.Width - this.m_padHorizontal;
					double num2 = 0.0;
					num2 = ((!horizontalText) ? num : (base.m_itemPageSizes.Height - this.m_padVertical));
					if (num2 > (double)contentHeight)
					{
						switch (verticalAlignment)
						{
						case RPLFormat.VerticalAlignments.Bottom:
							this.m_contentOffset = num2 - (double)contentHeight;
							break;
						case RPLFormat.VerticalAlignments.Middle:
							this.m_contentOffset = (num2 - (double)contentHeight) / 2.0;
							break;
						}
					}
				}
			}
		}

		internal override bool AddToPage(RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			bool flag;
			RoundedDouble roundedDouble;
			double num4;
			double num5;
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox;
			FlowContext flowContext;
			double num6;
			if (this.HitsCurrentPage(pageLeft, pageTop, pageRight, pageBottom))
			{
				flag = false;
				flowContext = null;
				base.SplitsVerticalPage = false;
				this.CalculateOffsetFromHeight();
				roundedDouble = new RoundedDouble(base.ItemPageSizes.Left);
				if (roundedDouble >= pageLeft)
				{
					double num = base.ItemPageSizes.Bottom - (this.m_padVertical - this.m_padTop);
					double num2 = 0.0;
					bool flag2 = true;
					bool flag3 = false;
					roundedDouble.Value = base.ItemPageSizes.Top;
					if (roundedDouble < pageTop)
					{
						flag3 = true;
						base.SplitsVerticalPage = true;
						roundedDouble.Value = num;
						if (roundedDouble > pageBottom)
						{
							flag2 = false;
							num2 = num - pageBottom;
							this.m_textBoxState.ResetHorizontalState = true;
						}
					}
					else
					{
						roundedDouble.Value = num;
						if (roundedDouble > pageBottom)
						{
							flag3 = true;
							this.m_textBoxState.SpanPages = true;
							this.m_textBoxState.ResetHorizontalState = true;
							flag2 = false;
							num2 = num - pageBottom;
						}
					}
					if (flag3)
					{
						base.m_rplItemState |= 64;
						double num3 = Math.Min(pageBottom, num);
						num4 = base.m_itemPageSizes.Width - this.m_padHorizontal;
						num5 = 0.0;
						num5 = ((!this.VerticalText) ? (num3 - (base.ItemPageSizes.Top + this.m_padTop)) : (num3 - Math.Max(pageTop, base.ItemPageSizes.Top + this.m_padTop)));
						if (!(num5 <= 0.0) && !(num4 <= 0.0))
						{
							if (this.HorizontalText && num5 <= this.m_contentOffset)
							{
								this.m_contentBottom = this.m_contentOffset;
								this.m_pageEndOffset.ParagraphIndex = 0;
								goto IL_0480;
							}
							if (this.VerticalText && num4 <= this.m_contentOffset)
							{
								this.m_contentBottom = this.m_contentOffset - num4;
								this.m_pageEndOffset.ParagraphIndex = 0;
								goto IL_0480;
							}
							textBox = (base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox);
							if (this.m_richTextBox == null)
							{
								this.m_richTextBox = this.GetRichTextBox();
							}
							flowContext = new FlowContext((float)num4, (float)num5, 0, this.m_pageStartOffset.TextRunIndex, this.m_pageStartOffset.CharacterIndex);
							flowContext.Updatable = true;
							flowContext.ContentOffset = (float)this.m_contentOffset;
							if (flag2)
							{
								flowContext.LineLimit = false;
							}
							num6 = 0.0;
							if (this.m_richTextBox != null)
							{
								num6 = (double)pageContext.Common.MeasureTextBoxHeight(this.m_richTextBox, flowContext);
							}
							if (flowContext.OmittedLineHeight > 0.0)
							{
								if (!this.VerticalText)
								{
									if (!((double)flowContext.OmittedLineHeight > pageContext.ColumnHeight))
									{
										if (textBox != null && textBox.CanGrow)
										{
											goto IL_0337;
										}
										if (!((double)flowContext.OmittedLineHeight > num2))
										{
											goto IL_0337;
										}
									}
									goto IL_0344;
								}
								if ((double)flowContext.OmittedLineHeight > num4)
								{
									flowContext = new FlowContext((float)num4, (float)num5, 0, this.m_pageStartOffset.TextRunIndex, this.m_pageStartOffset.CharacterIndex);
									flowContext.Updatable = true;
									flowContext.ContentOffset = (float)this.m_contentOffset;
									flowContext.LineLimit = false;
									num6 = (double)pageContext.Common.MeasureTextBoxHeight(this.m_richTextBox, flowContext);
								}
							}
							goto IL_0397;
						}
						this.m_pageEndOffset.ParagraphIndex = 0;
						this.m_contentBottom = 0.0;
					}
					goto IL_0480;
				}
				goto IL_04bc;
			}
			return false;
			IL_04bc:
			this.WriteStartItemToStream(rplWriter, pageContext);
			if (flag)
			{
				this.m_richTextBox = null;
			}
			this.RegisterTextBoxes(rplWriter, pageContext);
			return true;
			IL_0480:
			if (repeatState == RepeatState.None)
			{
				roundedDouble.Value = base.ItemPageSizes.Bottom;
				if (roundedDouble <= pageBottom)
				{
					roundedDouble.Value = base.ItemPageSizes.Right;
					if (roundedDouble <= pageRight)
					{
						flag = true;
					}
				}
			}
			goto IL_04bc;
			IL_0344:
			flowContext = new FlowContext((float)num4, (float)num5, 0, this.m_pageStartOffset.TextRunIndex, this.m_pageStartOffset.CharacterIndex);
			flowContext.Updatable = true;
			flowContext.ContentOffset = (float)this.m_contentOffset;
			flowContext.LineLimit = false;
			num6 = (double)pageContext.Common.MeasureTextBoxHeight(this.m_richTextBox, flowContext);
			goto IL_0397;
			IL_0337:
			if (pageContext.Common.InHeaderFooter)
			{
				goto IL_0344;
			}
			goto IL_0397;
			IL_0397:
			if (this.HorizontalText)
			{
				this.m_contentBottom = (double)flowContext.ContentOffset;
				if (flowContext.OmittedLineHeight > 0.0)
				{
					double num7 = num5 - (num6 - (double)flowContext.OmittedLineHeight);
					this.m_contentBottom += num7;
					if (!flowContext.AtEndOfTextBox && textBox != null && textBox.CanGrow && num7 > 0.0)
					{
						base.ItemPageSizes.AdjustHeightTo(base.ItemPageSizes.Height + num7);
					}
				}
			}
			else
			{
				this.m_contentBottom = (double)flowContext.ContentOffset - num4;
				if (flowContext.OmittedLineHeight > 0.0)
				{
					this.m_contentBottom += num4 - (num6 - (double)flowContext.OmittedLineHeight);
				}
			}
			this.m_pageEndOffset.Set(flowContext.Context);
			TextBoxContext textBoxContext = flowContext.ClipContext;
			if (textBoxContext == null)
			{
				textBoxContext = flowContext.Context;
			}
			this.m_nextPageStartOffset.Set(textBoxContext);
			goto IL_0480;
		}

		internal override void ResetHorizontal(bool spanPages, double? width)
		{
			base.ResetHorizontal(spanPages, width);
			if (spanPages)
			{
				this.UpdateState();
			}
			else
			{
				this.m_pageStartOffset.Reset(0);
				this.m_nextPageStartOffset.Reset(-1);
				this.m_pageEndOffset.Reset(-1);
			}
			this.m_textBoxState.ResetHorizontalState = false;
		}

		private void UpdateState()
		{
			int paragraphIndex = this.m_nextPageStartOffset.ParagraphIndex;
			if (paragraphIndex > 0)
			{
				this.m_richTextBox.Paragraphs.RemoveRange(0, paragraphIndex);
				this.m_paragraphs.RemoveRange(0, paragraphIndex);
			}
			int textRunIndex = this.m_nextPageStartOffset.TextRunIndex;
			if (textRunIndex > -1)
			{
				this.m_pageStartOffset.TextRunIndex = textRunIndex;
			}
			int characterIndex = this.m_nextPageStartOffset.CharacterIndex;
			if (characterIndex > -1)
			{
				this.m_pageStartOffset.CharacterIndex = characterIndex;
			}
			if (this.m_paragraphs.Count > 0 && paragraphIndex > -1)
			{
				Paragraph paragraph = this.m_paragraphs[0];
				if (textRunIndex > 0 || characterIndex > 0)
				{
					paragraph.FirstLine = false;
					this.m_richTextBox.Paragraphs[0].Updated = true;
				}
				if (textRunIndex > 0)
				{
					int textRunIndex2 = this.m_nextPageStartOffset.TextRunIndex;
					int num;
					for (num = 0; num < textRunIndex2; num++)
					{
						TextRun textRun = paragraph.TextRuns[0];
						int num2 = num;
						num += textRun.SplitIndices.Count;
						if (num < textRunIndex2)
						{
							paragraph.TextRuns.RemoveAt(0);
							this.m_richTextBox.Paragraphs[0].Runs.RemoveRange(0, textRun.SplitIndices.Count + 1);
						}
						else if (textRunIndex2 > num2)
						{
							this.m_richTextBox.Paragraphs[0].Runs.RemoveRange(0, textRunIndex2 - num2);
							textRun.ClearTo(textRunIndex2 - num2 - 1);
						}
						this.m_pageStartOffset.TextRunIndex = 0;
					}
				}
			}
			this.m_nextPageStartOffset.Reset(-1);
			this.m_pageEndOffset.Reset(-1);
			this.m_contentOffset = this.m_contentBottom;
		}

		internal override bool ResolveDuplicates(PageContext pageContext, double topInParentSystem, PageItem[] siblings, bool recalculate)
		{
			if (!base.SplitsVerticalPage && !base.AboveCurrentVerticalPage(topInParentSystem))
			{
				base.ResolveDuplicates(pageContext, topInParentSystem, siblings, recalculate);
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
				if (textBox != null && textBox.HideDuplicates)
				{
					bool flag = false;
					if (pageContext.TextBoxDuplicates == null)
					{
						flag = true;
						pageContext.TextBoxDuplicates = new Hashtable();
					}
					else if (!pageContext.TextBoxDuplicates.ContainsKey(textBox.ID))
					{
						flag = true;
					}
					if (flag)
					{
						pageContext.TextBoxDuplicates.Add(textBox.ID, null);
						base.Duplicate = false;
						if ((textBox.CanGrow || textBox.CanShrink) && this.CalcSizeState == CalcSize.Done)
						{
							this.CalculateVerticalSize(pageContext);
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		private object GetOriginalValue(PageContext pageContext)
		{
			object obj = null;
			if (base.m_nonSharedOffset > -1)
			{
				string text = default(string);
				TypeCode typeCode = default(TypeCode);
				this.ReadNonSharedValuesFromCache(pageContext, out text, out typeCode, out obj);
				if (obj == null)
				{
					obj = text;
				}
			}
			else
			{
				obj = ((TextBoxInstance)base.m_source.Instance).OriginalValue;
			}
			return obj;
		}

		internal override void RegisterTextBoxes(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null && !pageContext.Common.InSubReport && pageContext.EvaluatePageHeaderFooter)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
				if (textBox != null)
				{
					ReportStringProperty reportStringProperty = null;
					if (textBox.IsSimple)
					{
						reportStringProperty = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].TextRuns)[0].Value;
					}
					if (reportStringProperty == null || reportStringProperty.IsExpression)
					{
						if (rplWriter.DelayedTBLevels == 0)
						{
							pageContext.AddTextBox(base.m_source.Name, this.GetOriginalValue(pageContext));
						}
						else
						{
							rplWriter.AddTextBox(base.m_source.Name, this.GetOriginalValue(pageContext));
						}
					}
					else if (rplWriter.DelayedTBLevels == 0)
					{
						if (textBox.SharedTypeCode == TypeCode.String)
						{
							pageContext.AddTextBox(base.m_source.Name, reportStringProperty.Value);
						}
						else
						{
							pageContext.AddTextBox(base.m_source.Name, ((TextBoxInstance)base.m_source.Instance).OriginalValue);
						}
					}
					else if (textBox.SharedTypeCode == TypeCode.String)
					{
						rplWriter.AddTextBox(base.m_source.Name, reportStringProperty.Value);
					}
					else
					{
						rplWriter.AddTextBox(base.m_source.Name, ((TextBoxInstance)base.m_source.Instance).OriginalValue);
					}
				}
			}
		}

		internal override void CacheNonSharedProperties(PageContext pageContext)
		{
			if (pageContext.CacheNonSharedProps)
			{
				base.CacheNonSharedProperties(pageContext);
				BinaryWriter propertyCacheWriter = pageContext.PropertyCacheWriter;
				if (pageContext.PropertyCacheState != 0)
				{
					base.m_nonSharedOffset = propertyCacheWriter.BaseStream.Position;
				}
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
				if (textBox != null && (!textBox.IsSimple || textBox.FormattedValueExpressionBased) && pageContext.EvaluatePageHeaderFooter)
				{
					TextBoxInstance textBoxInstance = (TextBoxInstance)base.m_source.Instance;
					object originalValue = textBoxInstance.OriginalValue;
					if (originalValue != null)
					{
						bool flag = false;
						TypeCode typeCode = this.GetTypeCode(textBox.SharedTypeCode, textBoxInstance, ref flag);
						propertyCacheWriter.Write((byte)33);
						propertyCacheWriter.Write((byte)typeCode);
						this.WriteOriginalValue(propertyCacheWriter, typeCode, originalValue);
					}
				}
				propertyCacheWriter.Write((byte)255);
			}
		}

		internal void ReadNonSharedValuesFromCache(PageContext pageContext, out string value, out TypeCode typeCode, out object originalValue)
		{
			if (pageContext.PropertyCacheState == PageContext.CacheState.RPLStream)
			{
				pageContext.PropertyCacheReader.BaseStream.Seek(base.m_nonSharedOffset + 16, SeekOrigin.Begin);
			}
			else
			{
				pageContext.PropertyCacheReader.BaseStream.Seek(base.m_nonSharedOffset, SeekOrigin.Begin);
			}
			this.ReadNonSharedValuesFromCache(pageContext.PropertyCacheReader, out value, out typeCode, out originalValue);
		}

		internal void ReadNonSharedValuesFromCache(BinaryReader reader, out string value, out TypeCode typeCode, out object originalValue)
		{
			value = null;
			originalValue = null;
			typeCode = TypeCode.String;
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				typeCode = textBox.SharedTypeCode;
			}
			for (byte b = reader.ReadByte(); b != 255; b = reader.ReadByte())
			{
				switch (b)
				{
				case 27:
					value = reader.ReadString();
					break;
				case 33:
					typeCode = (TypeCode)reader.ReadByte();
					break;
				case 34:
					switch (typeCode)
					{
					case TypeCode.Char:
						originalValue = reader.ReadChar();
						break;
					case TypeCode.String:
						originalValue = reader.ReadString();
						break;
					case TypeCode.Boolean:
						originalValue = reader.ReadBoolean();
						break;
					case TypeCode.Byte:
						originalValue = reader.ReadByte();
						break;
					case TypeCode.Int16:
						originalValue = reader.ReadInt16();
						break;
					case TypeCode.Int32:
						originalValue = reader.ReadInt32();
						break;
					case TypeCode.Int64:
						originalValue = reader.ReadInt64();
						break;
					case TypeCode.Single:
						originalValue = reader.ReadSingle();
						break;
					case TypeCode.Decimal:
						originalValue = reader.ReadDecimal();
						break;
					case TypeCode.Double:
						originalValue = reader.ReadDouble();
						break;
					case TypeCode.DateTime:
						originalValue = DateTime.FromBinary(reader.ReadInt64());
						break;
					default:
						typeCode = TypeCode.String;
						originalValue = reader.ReadString();
						break;
					}
					break;
				}
			}
		}

		internal override void CopyCachedData(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryReader propertyCacheReader = pageContext.PropertyCacheReader;
			propertyCacheReader.BaseStream.Seek(base.m_nonSharedOffset, SeekOrigin.Begin);
			long num = propertyCacheReader.ReadInt64();
			long num2 = propertyCacheReader.ReadInt64();
			if (num2 == base.m_nonSharedOffset)
			{
				propertyCacheReader.BaseStream.Seek(num, SeekOrigin.Begin);
				base.CopyData(propertyCacheReader, rplWriter, base.m_nonSharedOffset - num - 1);
			}
			else
			{
				base.CopyDataAndResolve(rplWriter, pageContext, num, num2, true);
			}
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			bool flag = false;
			bool flag2 = false;
			if (textBox != null)
			{
				if (textBox.IsSimple && !base.Duplicate)
				{
					if (textBox.FormattedValueExpressionBased)
					{
						flag = true;
					}
					else if (textBox.HideDuplicates || this.m_textBoxState.SpanPages)
					{
						flag2 = true;
						flag = true;
					}
				}
			}
			else
			{
				flag = true;
			}
			StyleWriterStream styleWriterStream = new StyleWriterStream(rplWriter.BinaryWriter);
			if (flag)
			{
				string value = this.CalculateSimpleValue(!flag2, pageContext);
				styleWriterStream.WriteNotNull(27, value);
			}
			if (this.m_contentOffset > 0.0)
			{
				styleWriterStream.Write(37, (float)this.m_contentOffset);
			}
			rplWriter.BinaryWriter.Write((byte)255);
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				rplWriter.RegisterCacheRichData(null != this.m_richTextBox);
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
				if ((textBox == null || textBox.IsSimple) && rplWriter.PageParagraphsItemizedData != null)
				{
					pageContext.ParagraphItemizedData = new List<TextRunItemizedData>();
				}
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)7);
					base.WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
					List<long> list = this.WriteParagraphs(binaryWriter, pageContext, rplWriter.PageParagraphsItemizedData);
					long position2 = baseStream.Position;
					binaryWriter.Write((byte)18);
					binaryWriter.Write(position);
					binaryWriter.Write(list.Count);
					foreach (long item in list)
					{
						binaryWriter.Write(item);
					}
					binaryWriter.Write((byte)255);
					binaryWriter.Flush();
					base.m_offset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position2);
					binaryWriter.Write((byte)255);
				}
				else if (base.m_rplElement == null)
				{
					base.m_rplElement = new RPLTextBox();
					base.WriteElementProps(base.m_rplElement.ElementProps, pageContext);
					if (base.m_nonSharedOffset < 0)
					{
						this.WriteParagraphs(pageContext, rplWriter.PageParagraphsItemizedData);
					}
				}
				else
				{
					if (base.SplitsVerticalPage)
					{
						RPLItemProps rPLItemProps = base.m_rplElement.ElementProps as RPLItemProps;
						RPLTextBoxProps rplElementProps = (RPLTextBoxProps)((RPLTextBoxProps)rPLItemProps).Clone();
						base.m_rplElement = new RPLTextBox(rplElementProps);
					}
					RPLTextBoxProps rPLTextBoxProps = base.m_rplElement.ElementProps as RPLTextBoxProps;
					bool flag = textBox != null && textBox.IsSimple;
					bool flag2 = true;
					if (this.m_textBoxState.SpanPages && flag && !textBox.FormattedValueExpressionBased)
					{
						base.WriteSharedItemProps(rPLTextBoxProps, pageContext);
						flag2 = false;
					}
					rPLTextBoxProps.ContentOffset = (float)this.m_contentOffset;
					if (flag || textBox == null)
					{
						if (!this.WriteSimpleValue(rPLTextBoxProps, pageContext) && flag2)
						{
							this.CalculateSimpleGlyph(pageContext);
						}
					}
					else
					{
						this.WriteParagraphs(pageContext, rplWriter.PageParagraphsItemizedData);
					}
				}
				if (pageContext.ParagraphItemizedData != null && pageContext.ParagraphItemizedData.Count > 0)
				{
					rplWriter.PageParagraphsItemizedData.Add(this.m_paragraphs[0].UniqueName, pageContext.ParagraphItemizedData);
				}
				pageContext.ParagraphItemizedData = null;
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				StyleWriterStream styleWriterStream = new StyleWriterStream(spbifWriter);
				if (textBox.SharedTypeCode != TypeCode.String)
				{
					styleWriterStream.Write(33, (byte)textBox.SharedTypeCode);
				}
				if (textBox.CanGrow)
				{
					styleWriterStream.Write(25, true);
				}
				if (!textBox.IsSimple)
				{
					styleWriterStream.Write(35, textBox.IsSimple);
				}
				else
				{
					if (textBox.FormattedValueExpressionBased)
					{
						styleWriterStream.Write(45, textBox.FormattedValueExpressionBased);
					}
					if (!textBox.HideDuplicates && this.SimpleValuesIsSharedNonNull(textBox) && !this.m_textBoxState.SpanPages)
					{
						styleWriterStream.WriteNotNull(27, this.CalculateSimpleValue(false, pageContext));
					}
				}
			}
		}

		private bool SimpleValuesIsSharedNonNull(AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBoxDef)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBoxDef.Paragraphs)[0].TextRuns)[0];
			ReportStringProperty value = textRun.Value;
			if (!textRun.FormattedValueExpressionBased)
			{
				return null != value.Value;
			}
			return false;
		}

		private string CalculateSimpleValue(bool isNonShared, PageContext pageContext)
		{
			if (this.m_paragraphs.Count > 0 && (this.m_pageEndOffset.ParagraphIndex != 0 || this.m_pageEndOffset.TextRunIndex > 0 || this.m_pageEndOffset.CharacterIndex > 0))
			{
				Paragraph paragraph = this.m_paragraphs[0];
				if (paragraph.TextRuns.Count > 0)
				{
					TextRun textRun = paragraph.TextRuns[0];
					TextBoxOffset endPosition = this.m_pageEndOffset;
					if (this.m_pageEndOffset.ParagraphIndex != 0)
					{
						endPosition = null;
					}
					string text = null;
					text = ((!isNonShared) ? textRun.DefinitionText : textRun.Text);
					List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextRuns = null;
					if (pageContext.ParagraphItemizedData != null)
					{
						richTextRuns = this.m_richTextBox.Paragraphs[0].Runs;
					}
					TextRunItemizedData textRunItemizedData = null;
					string stringValue = textRun.GetStringValue(text, this.m_pageStartOffset, endPosition, 0, richTextRuns, out textRunItemizedData);
					if (textRunItemizedData != null)
					{
						pageContext.RegisterTextRunData(textRunItemizedData);
					}
					return stringValue;
				}
			}
			return null;
		}

		private void CalculateSimpleGlyph(PageContext pageContext)
		{
			if (pageContext.ParagraphItemizedData != null && this.m_paragraphs.Count > 0)
			{
				if (this.m_pageEndOffset.ParagraphIndex == 0 && this.m_pageEndOffset.TextRunIndex <= 0 && this.m_pageEndOffset.CharacterIndex <= 0)
				{
					return;
				}
				Paragraph paragraph = this.m_paragraphs[0];
				if (paragraph.TextRuns.Count > 0)
				{
					TextRun textRun = paragraph.TextRuns[0];
					int paragraphIndex = this.m_pageEndOffset.ParagraphIndex;
					List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> runs = this.m_richTextBox.Paragraphs[0].Runs;
					TextRunItemizedData glyphValue = textRun.GetGlyphValue(textRun.DefinitionText, 0, runs);
					if (glyphValue != null)
					{
						pageContext.RegisterTextRunData(glyphValue);
					}
				}
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)sharedProps;
				rPLTextBoxPropsDef.SharedTypeCode = textBox.SharedTypeCode;
				rPLTextBoxPropsDef.CanGrow = textBox.CanGrow;
				rPLTextBoxPropsDef.FormattedValueExpressionBased = textBox.FormattedValueExpressionBased;
				if (!textBox.IsSimple)
				{
					rPLTextBoxPropsDef.IsSimple = false;
				}
				else if (!textBox.HideDuplicates && this.SimpleValuesIsSharedNonNull(textBox) && !this.m_textBoxState.SpanPages)
				{
					rPLTextBoxPropsDef.Value = this.CalculateSimpleValue(false, pageContext);
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
			StyleWriterStream styleWriterStream = new StyleWriterStream(spbifWriter);
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				if (base.m_nonSharedOffset == -1)
				{
					styleWriterStream.WriteNotNull(27, this.CalculateSimpleValue(true, pageContext));
				}
			}
			else
			{
				if (textBox.IsSimple)
				{
					bool flag = false;
					TypeCode typeCode = this.GetTypeCode(textBox.SharedTypeCode, base.m_source.Instance as TextBoxInstance, ref flag);
					if (flag)
					{
						styleWriterStream.Write(33, (byte)typeCode);
					}
					if (base.m_nonSharedOffset == -1 && !base.Duplicate)
					{
						bool formattedValueExpressionBased = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].TextRuns)[0].FormattedValueExpressionBased;
						if (formattedValueExpressionBased || textBox.HideDuplicates || this.m_textBoxState.SpanPages)
						{
							styleWriterStream.WriteNotNull(27, this.CalculateSimpleValue(formattedValueExpressionBased, pageContext));
						}
					}
				}
				base.WriteActionInfo(textBox.ActionInfo, spbifWriter);
			}
			if (base.m_nonSharedOffset == -1)
			{
				styleWriterStream.Write(37, (float)this.m_contentOffset);
			}
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, PageContext pageContext)
		{
			RPLTextBoxProps rPLTextBoxProps = (RPLTextBoxProps)nonSharedProps;
			if (base.m_nonSharedOffset < 0)
			{
				this.WriteSimpleValue(rPLTextBoxProps, pageContext);
				rPLTextBoxProps.ContentOffset = (float)this.m_contentOffset;
			}
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				if (textBox.IsSimple)
				{
					bool flag = false;
					TextBoxInstance textBox2 = textBox.Instance as TextBoxInstance;
					rPLTextBoxProps.TypeCode = this.GetTypeCode(textBox.SharedTypeCode, textBox2, ref flag);
				}
				rPLTextBoxProps.ActionInfo = PageItem.WriteActionInfo(textBox.ActionInfo);
			}
		}

		private bool WriteSimpleValue(RPLTextBoxProps textBoxProps, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				textBoxProps.Value = this.CalculateSimpleValue(true, pageContext);
				return true;
			}
			if (textBox.IsSimple && !base.Duplicate)
			{
				bool formattedValueExpressionBased = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].TextRuns)[0].FormattedValueExpressionBased;
				if (!formattedValueExpressionBased && !textBox.HideDuplicates && !this.m_textBoxState.SpanPages)
				{
					goto IL_0074;
				}
				textBoxProps.Value = this.CalculateSimpleValue(formattedValueExpressionBased, pageContext);
				return true;
			}
			goto IL_0074;
			IL_0074:
			return false;
		}

		private TypeCode GetTypeCode(TypeCode sharedTypeCode, TextBoxInstance textBox, ref bool changed)
		{
			TypeCode typeCode = sharedTypeCode;
			if (typeCode == TypeCode.Object)
			{
				changed = true;
				typeCode = textBox.TypeCode;
			}
			return typeCode;
		}

		private void WriteParagraphs(PageContext pageContext, Dictionary<string, List<TextRunItemizedData>> textBoxItemizedData)
		{
			RPLTextBox tb = base.m_rplElement as RPLTextBox;
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && !textBox.IsSimple && !base.Duplicate)
			{
				bool hideDuplicates = textBox.HideDuplicates;
				bool cacheRichData = (byte)((textBoxItemizedData != null) ? 1 : 0) != 0;
				int num = this.m_pageEndOffset.ParagraphIndex;
				if (num < 0)
				{
					num = this.m_paragraphs.Count;
				}
				TextBoxOffset textBoxOffset = this.m_pageStartOffset;
				if (textBoxOffset.TextRunIndex == 0 && textBoxOffset.CharacterIndex == 0)
				{
					textBoxOffset = null;
				}
				if (num > 0)
				{
					this.WriteParagraph(tb, 0, cacheRichData, hideDuplicates, textBoxOffset, null, pageContext, textBoxItemizedData);
					for (int i = 1; i < num; i++)
					{
						this.WriteParagraph(tb, i, cacheRichData, hideDuplicates, null, null, pageContext, textBoxItemizedData);
					}
				}
				if (num < this.m_paragraphs.Count)
				{
					if (this.m_pageEndOffset.TextRunIndex <= 0 && this.m_pageEndOffset.CharacterIndex <= 0)
					{
						return;
					}
					if (num > 0)
					{
						textBoxOffset = null;
					}
					this.WriteParagraph(tb, num, cacheRichData, hideDuplicates, textBoxOffset, this.m_pageEndOffset, pageContext, textBoxItemizedData);
				}
			}
		}

		private void WriteParagraph(RPLTextBox tb, int index, bool cacheRichData, bool hideDuplicates, TextBoxOffset startPosition, TextBoxOffset endPosition, PageContext pageContext, Dictionary<string, List<TextRunItemizedData>> textBoxItemizedData)
		{
			Paragraph paragraph = this.m_paragraphs[index];
			List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextParaRuns = null;
			if (cacheRichData)
			{
				richTextParaRuns = this.m_richTextBox.Paragraphs[index].Runs;
				pageContext.ParagraphItemizedData = new List<TextRunItemizedData>();
			}
			tb.AddParagraph(paragraph.GetRPLParagraph(pageContext, hideDuplicates, startPosition, endPosition, richTextParaRuns));
			if (cacheRichData && pageContext.ParagraphItemizedData.Count > 0)
			{
				textBoxItemizedData.Add(paragraph.UniqueName, pageContext.ParagraphItemizedData);
			}
			pageContext.ParagraphItemizedData = null;
		}

		private List<long> WriteParagraphs(BinaryWriter spbifWriter, PageContext pageContext, Dictionary<string, List<TextRunItemizedData>> textBoxItemizedData)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && !textBox.IsSimple && !base.Duplicate)
			{
				bool cacheRichData = (byte)((textBoxItemizedData != null) ? 1 : 0) != 0;
				bool hideDuplicates = textBox != null && textBox.HideDuplicates;
				TextBoxOffset textBoxOffset = this.m_pageStartOffset;
				if (textBoxOffset.TextRunIndex == 0 && textBoxOffset.CharacterIndex == 0)
				{
					textBoxOffset = null;
				}
				List<long> list = new List<long>();
				int num = this.m_pageEndOffset.ParagraphIndex;
				if (num < 0)
				{
					num = this.m_paragraphs.Count;
				}
				if (num > 0)
				{
					this.WriteParagraph(spbifWriter, 0, cacheRichData, hideDuplicates, textBoxOffset, null, list, pageContext, textBoxItemizedData);
					for (int i = 1; i < num; i++)
					{
						this.WriteParagraph(spbifWriter, i, cacheRichData, hideDuplicates, null, null, list, pageContext, textBoxItemizedData);
					}
				}
				if (num < this.m_paragraphs.Count && (this.m_pageEndOffset.TextRunIndex > 0 || this.m_pageEndOffset.CharacterIndex > 0))
				{
					if (num > 0)
					{
						textBoxOffset = null;
					}
					this.WriteParagraph(spbifWriter, num, cacheRichData, hideDuplicates, textBoxOffset, this.m_pageEndOffset, list, pageContext, textBoxItemizedData);
				}
				return list;
			}
			return new List<long>();
		}

		private void WriteParagraph(BinaryWriter spbifWriter, int index, bool cacheRichData, bool hideDuplicates, TextBoxOffset startPosition, TextBoxOffset endPosition, List<long> offsets, PageContext pageContext, Dictionary<string, List<TextRunItemizedData>> textBoxItemizedData)
		{
			Paragraph paragraph = this.m_paragraphs[index];
			List<AspNetCore.ReportingServices.Rendering.RichText.TextRun> richTextParaRuns = null;
			if (cacheRichData)
			{
				richTextParaRuns = this.m_richTextBox.Paragraphs[index].Runs;
				pageContext.ParagraphItemizedData = new List<TextRunItemizedData>();
			}
			offsets.Add(paragraph.WriteToStream(spbifWriter, pageContext, hideDuplicates, startPosition, endPosition, richTextParaRuns));
			if (cacheRichData && pageContext.ParagraphItemizedData.Count > 0)
			{
				textBoxItemizedData.Add(paragraph.UniqueName, pageContext.ParagraphItemizedData);
			}
			pageContext.ParagraphItemizedData = null;
		}

		private bool WriteOriginalValue(BinaryWriter spbifWriter, TypeCode typeCode, object value)
		{
			if (value == null)
			{
				return false;
			}
			return base.WriteObjectValue(spbifWriter, 34, typeCode, value);
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			base.WriteBackgroundImage(style, true, spbifWriter, pageContext);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.VerticalAlign, 26);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.WritingMode, 30);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingBottom, 18);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingLeft, 15);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingRight, 16);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingTop, 17);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Direction, 29);
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && textBox.IsSimple)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph paragraph = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0];
				AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)paragraph.TextRuns)[0];
				base.WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.Color, 27);
				base.WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.FontFamily, 20);
				base.WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.FontSize, 21);
				base.WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.FontStyle, 19);
				base.WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.FontWeight, 22);
				base.WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.Language, 32);
				base.WriteStyleProp(paragraph.Style, spbifWriter, StyleAttributeNames.TextAlign, 25);
				base.WriteStyleProp(textRun.Style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
			}
			else if (base.m_source is AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)
			{
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Color, 27);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontFamily, 20);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontSize, 21);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontStyle, 19);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontWeight, 22);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Language, 32);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.TextAlign, 25);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
			}
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			base.WriteBackgroundImage(style, true, rplStyleProps, pageContext);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.VerticalAlign, 26);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.WritingMode, 30);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Direction, 29);
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && textBox.IsSimple)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph paragraph = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0];
				AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)paragraph.TextRuns)[0];
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.Color, 27);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.FontSize, 21);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.Language, 32);
				PageItem.WriteStyleProp(paragraph.Style, rplStyleProps, StyleAttributeNames.TextAlign, 25);
				PageItem.WriteStyleProp(textRun.Style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
			}
			else if (base.m_source is AspNetCore.ReportingServices.OnDemandReportRendering.Tablix)
			{
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Color, 27);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontSize, 21);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Language, 32);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.TextAlign, 25);
				PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
			}
		}

		internal override void WriteNonSharedStyle(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, PageContext pageContext)
		{
			if (styleDef == null)
			{
				styleDef = base.m_source.Style;
			}
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			List<StyleAttributeNames> nonSharedStyleAttributes = styleDef.NonSharedStyleAttributes;
			if (!Utility.IsNullOrEmpty(nonSharedStyleAttributes) || (textBox != null && textBox.IsSimple && (!Utility.IsNullOrEmpty(((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].Style.NonSharedStyleAttributes) || !Utility.IsNullOrEmpty(((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].TextRuns)[0].Style.NonSharedStyleAttributes))))
			{
				if (style == null)
				{
					style = base.m_source.Instance.Style;
				}
				bool flag = false;
				spbifWriter.Write((byte)6);
				spbifWriter.Write((byte)1);
				if (!Utility.IsNullOrEmpty(nonSharedStyleAttributes))
				{
					int count = nonSharedStyleAttributes.Count;
					for (int i = 0; i < count; i++)
					{
						switch (nonSharedStyleAttributes[i])
						{
						case StyleAttributeNames.BackgroundImage:
						case StyleAttributeNames.BackgroundImageRepeat:
							if (!flag)
							{
								flag = true;
								this.WriteItemNonSharedStyleProps(spbifWriter, styleDef, style, StyleAttributeNames.BackgroundImage, pageContext);
							}
							break;
						default:
							this.WriteNonSharedStyleProp(spbifWriter, styleDef, style, nonSharedStyleAttributes[i], pageContext);
							break;
						}
					}
				}
				if (textBox != null && textBox.IsSimple)
				{
					StyleWriterStream writer = new StyleWriterStream(spbifWriter);
					Paragraph paragraph = this.m_paragraphs[0];
					paragraph.WriteNonSharedStyles(writer);
					TextRun textRun = paragraph.TextRuns[0];
					textRun.WriteNonSharedStyles(writer);
				}
				spbifWriter.Write((byte)255);
			}
		}

		internal override RPLStyleProps WriteNonSharedStyle(Style styleDef, StyleInstance style, PageContext pageContext)
		{
			RPLStyleProps rPLStyleProps = base.WriteNonSharedStyle(styleDef, style, pageContext);
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null && textBox.IsSimple && this.m_paragraphs.Count > 0)
			{
				if (rPLStyleProps == null)
				{
					rPLStyleProps = new RPLStyleProps();
				}
				StyleWriterOM writer = new StyleWriterOM(rPLStyleProps);
				Paragraph paragraph = this.m_paragraphs[0];
				paragraph.WriteNonSharedStyles(writer);
				TextRun textRun = paragraph.TextRuns[0];
				textRun.WriteNonSharedStyles(writer);
				if (rPLStyleProps.Count == 0)
				{
					rPLStyleProps = null;
				}
			}
			return rPLStyleProps;
		}

		internal override void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				base.WriteBackgroundImage(styleDef, false, spbifWriter, pageContext);
				break;
			case StyleAttributeNames.Direction:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Direction, 29);
				break;
			case StyleAttributeNames.VerticalAlign:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.VerticalAlign, 26);
				break;
			case StyleAttributeNames.WritingMode:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.WritingMode, 30);
				break;
			case StyleAttributeNames.PaddingBottom:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingBottom, 18);
				break;
			case StyleAttributeNames.PaddingLeft:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingLeft, 15);
				break;
			case StyleAttributeNames.PaddingRight:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingRight, 16);
				break;
			case StyleAttributeNames.PaddingTop:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingTop, 17);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProps(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				base.WriteBackgroundImage(styleDef, false, rplStyleProps, pageContext);
				break;
			case StyleAttributeNames.Direction:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Direction, 29);
				break;
			case StyleAttributeNames.VerticalAlign:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.VerticalAlign, 26);
				break;
			case StyleAttributeNames.WritingMode:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.WritingMode, 30);
				break;
			case StyleAttributeNames.PaddingBottom:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
				break;
			case StyleAttributeNames.PaddingLeft:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
				break;
			case StyleAttributeNames.PaddingRight:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
				break;
			case StyleAttributeNames.PaddingTop:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
				break;
			}
		}

		private AspNetCore.ReportingServices.Rendering.RichText.TextBox GetRichTextBox()
		{
			if (base.Duplicate)
			{
				return null;
			}
			if (this.m_paragraphs.Count == 0)
			{
				return null;
			}
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null || textBox.IsSimple)
			{
				string text = this.m_paragraphs[0].TextRuns[0].Text;
				if (text == null && textBox != null)
				{
					text = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].TextRuns)[0].Value.Value;
				}
				if (string.IsNullOrEmpty(text))
				{
					return null;
				}
			}
			AspNetCore.ReportingServices.Rendering.RichText.TextBox textBox2 = new AspNetCore.ReportingServices.Rendering.RichText.TextBox(this);
			textBox2.Paragraphs = new List<AspNetCore.ReportingServices.Rendering.RichText.Paragraph>(this.m_paragraphs.Count);
			foreach (Paragraph paragraph in this.m_paragraphs)
			{
				AspNetCore.ReportingServices.Rendering.RichText.Paragraph richTextParagraph = paragraph.GetRichTextParagraph();
				textBox2.Paragraphs.Add(richTextParagraph);
			}
			textBox2.ScriptItemize();
			return textBox2;
		}

		private void InitParagraphs()
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				Style style = base.m_source.Style;
				StyleInstance style2 = base.m_source.Instance.Style;
				if (this.GetAlignmentRight(style, style2))
				{
					this.m_textBoxState.DefaultTextAlign = RPLFormat.TextAlignments.Right;
				}
				byte? nonCompiledEnumProp = Utility.GetNonCompiledEnumProp(29, StyleAttributeNames.Direction, style, style2);
				if (((int?)nonCompiledEnumProp).HasValue)
				{
					this.m_textBoxState.Direction = (RPLFormat.Directions)nonCompiledEnumProp.Value;
				}
				nonCompiledEnumProp = Utility.GetNonCompiledEnumProp(30, StyleAttributeNames.WritingMode, style, style2);
				if (((int?)nonCompiledEnumProp).HasValue)
				{
					this.m_textBoxState.WritingMode = (RPLFormat.WritingModes)nonCompiledEnumProp.Value;
				}
				nonCompiledEnumProp = Utility.GetNonCompiledEnumProp(26, StyleAttributeNames.VerticalAlign, style, style2);
				if (((int?)nonCompiledEnumProp).HasValue)
				{
					this.m_textBoxState.VerticalAlignment = (RPLFormat.VerticalAlignments)nonCompiledEnumProp.Value;
				}
				string uniqueName = null;
				if (textBox.IsSimple)
				{
					uniqueName = base.m_source.Instance.UniqueName;
				}
				TextBoxInstance textBoxInstance = textBox.Instance as TextBoxInstance;
				ParagraphInstanceCollection paragraphInstances = textBoxInstance.ParagraphInstances;
				ParagraphNumberCalculator paragraphNumberCalculator = new ParagraphNumberCalculator();
				foreach (ParagraphInstance item in paragraphInstances)
				{
					Paragraph paragraph = new Paragraph(item, uniqueName, textBox.HideDuplicates);
					paragraphNumberCalculator.UpdateParagraph(paragraph);
					this.m_paragraphs.Add(paragraph);
				}
			}
			else
			{
				string text = null;
				string uniqueName2 = null;
				DataRegion dataRegion = base.m_source as DataRegion;
				if (dataRegion != null)
				{
					text = ((DataRegionInstance)dataRegion.Instance).NoRowsMessage;
					uniqueName2 = dataRegion.Instance.UniqueName;
				}
				else
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.SubReport;
					if (subReport != null)
					{
						SubReportInstance subReportInstance = subReport.Instance as SubReportInstance;
						text = ((!subReportInstance.ProcessedWithError) ? subReportInstance.NoRowsMessage : subReportInstance.ErrorMessage);
						uniqueName2 = subReportInstance.UniqueName;
					}
				}
				Paragraph paragraph2 = new Paragraph(uniqueName2);
				TextRun textRun = new TextRun();
				textRun.Text = text;
				paragraph2.TextRuns.Add(textRun);
				this.m_paragraphs.Add(paragraph2);
			}
		}

		public void DrawTextRun(AspNetCore.ReportingServices.Rendering.RichText.TextRun run, AspNetCore.ReportingServices.Rendering.RichText.Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, System.Drawing.Rectangle layoutRectangle)
		{
		}

		public void DrawClippedTextRun(AspNetCore.ReportingServices.Rendering.RichText.TextRun run, AspNetCore.ReportingServices.Rendering.RichText.Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, System.Drawing.Rectangle layoutRectangle, uint fontColorOverride, System.Drawing.Rectangle clipRect)
		{
		}
	}
}
