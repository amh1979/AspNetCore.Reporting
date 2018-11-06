using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class TextBox : PageItem, ITextBoxProps
	{
		internal enum CalcSize : byte
		{
			None,
			Done,
			Delay
		}

		private const int MAX_GDI_TEXT_SIZE = 32000;

		private CalcSize m_calcSizeState;

		private bool m_isSimple;

		private List<Paragraph> m_paragraphs;

		private AspNetCore.ReportingServices.Rendering.RichText.TextBox m_richTextBox;

		private float m_contentHeight;

		private RPLFormat.WritingModes m_writingMode;

		private long m_contentHeightPosition = -1L;

		private long m_startTextBoxOffset;

		internal override string SourceID
		{
			get
			{
				if (base.m_source is AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)
				{
					return base.m_source.ID;
				}
				return base.m_source.ID + "_NR";
			}
		}

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

		internal bool IsSimple
		{
			get
			{
				return this.m_isSimple;
			}
		}

		public RPLFormat.TextAlignments DefaultAlignment
		{
			get
			{
				if (this.m_isSimple)
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
					TypeCode typeCode = textBox.SharedTypeCode;
					if (typeCode == TypeCode.Object)
					{
						TextBoxInstance textBoxInstance = base.m_source.Instance as TextBoxInstance;
						if (textBoxInstance != null)
						{
							typeCode = textBoxInstance.TypeCode;
						}
					}
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
						return RPLFormat.TextAlignments.Right;
					}
				}
				return RPLFormat.TextAlignments.Left;
			}
		}

		public RPLFormat.Directions Direction
		{
			get
			{
				Directions aValue = (Directions)base.GetRichTextStyleValue(StyleAttributeNames.Direction, null);
				return (RPLFormat.Directions)StyleEnumConverter.Translate(aValue);
			}
		}

		public RPLFormat.WritingModes WritingMode
		{
			get
			{
				return this.m_writingMode;
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
					return textBox.CanGrow;
				}
				return false;
			}
		}

		internal TextBox(AspNetCore.ReportingServices.OnDemandReportRendering.TextBox source, PageContext pageContext, bool createForRepeat)
			: base(source)
		{
			if (pageContext != null)
			{
				if (createForRepeat)
				{
					base.m_itemPageSizes = pageContext.GetSharedFromRepeatItemSizesElement(source, false);
				}
				else
				{
					base.m_itemPageSizes = pageContext.GetSharedItemSizesElement(source, false);
				}
			}
			else
			{
				base.m_itemPageSizes = new ItemSizes(source);
			}
			this.m_isSimple = source.IsSimple;
			WritingModes aValue = (WritingModes)base.GetRichTextStyleValue(StyleAttributeNames.WritingMode, null);
			this.m_writingMode = (RPLFormat.WritingModes)StyleEnumConverter.Translate(aValue, pageContext.VersionPicker);
			if (source.CanGrow && base.m_itemPageSizes.Width > 0.0 && base.m_itemPageSizes.Height == 0.0)
			{
				bool flag = false;
				if (this.m_isSimple)
				{
					string value = ((TextBoxInstance)source.Instance).Value;
					if (!string.IsNullOrEmpty(value))
					{
						flag = true;
					}
				}
				else if (source.Paragraphs.Count > 1)
				{
					flag = true;
				}
				else
				{
					for (int i = 0; i < ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)source.Paragraphs)[0].TextRuns.Count; i++)
					{
						if (flag)
						{
							break;
						}
						if (!string.IsNullOrEmpty(((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)source.Paragraphs)[0].TextRuns)[i].Value.Value))
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					base.m_itemPageSizes.Height = 0.3528;
				}
			}
			if (this.WritingMode == RPLFormat.WritingModes.Horizontal)
			{
				this.m_contentHeight = (float)base.m_itemPageSizes.Height;
			}
			else
			{
				this.m_contentHeight = (float)base.m_itemPageSizes.Width;
			}
		}

		internal TextBox(DataRegion source, PageContext pageContext, bool createForRepeat)
			: base(source)
		{
			if (pageContext != null)
			{
				if (createForRepeat)
				{
					base.m_itemPageSizes = pageContext.GetSharedFromRepeatItemSizesElement(source, false);
				}
				else
				{
					base.m_itemPageSizes = pageContext.GetSharedItemSizesElement(source, false);
				}
			}
			else
			{
				base.m_itemPageSizes = new ItemSizes(source);
			}
			this.m_isSimple = true;
			WritingModes aValue = (WritingModes)base.GetRichTextStyleValue(StyleAttributeNames.WritingMode, null);
			this.m_writingMode = (RPLFormat.WritingModes)StyleEnumConverter.Translate(aValue, pageContext.VersionPicker);
			if (this.WritingMode == RPLFormat.WritingModes.Horizontal)
			{
				this.m_contentHeight = (float)base.m_itemPageSizes.Height;
			}
			else
			{
				this.m_contentHeight = (float)base.m_itemPageSizes.Width;
			}
		}

		internal TextBox(AspNetCore.ReportingServices.OnDemandReportRendering.SubReport source, PageContext pageContext, bool createForRepeat)
			: base(source)
		{
			if (pageContext != null)
			{
				if (createForRepeat)
				{
					base.m_itemPageSizes = pageContext.GetSharedFromRepeatItemSizesElement(source, false);
				}
				else
				{
					base.m_itemPageSizes = pageContext.GetSharedItemSizesElement(source, false);
				}
			}
			else
			{
				base.m_itemPageSizes = new ItemSizes(source);
			}
			this.m_isSimple = true;
			WritingModes aValue = (WritingModes)base.GetRichTextStyleValue(StyleAttributeNames.WritingMode, null);
			this.m_writingMode = (RPLFormat.WritingModes)StyleEnumConverter.Translate(aValue, pageContext.VersionPicker);
			if (this.WritingMode == RPLFormat.WritingModes.Horizontal)
			{
				this.m_contentHeight = (float)base.m_itemPageSizes.Height;
			}
			else
			{
				this.m_contentHeight = (float)base.m_itemPageSizes.Width;
			}
		}

		public void DrawTextRun(AspNetCore.ReportingServices.Rendering.RichText.TextRun run, AspNetCore.ReportingServices.Rendering.RichText.Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, System.Drawing.Rectangle layoutRectangle)
		{
		}

		public void DrawClippedTextRun(AspNetCore.ReportingServices.Rendering.RichText.TextRun run, AspNetCore.ReportingServices.Rendering.RichText.Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, System.Drawing.Rectangle layoutRectangle, uint fontColorOverride, System.Drawing.Rectangle clipRect)
		{
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			base.AdjustOriginFromItemsAbove(siblings, repeatWithItems);
			if (!this.HitsCurrentPage(pageContext, parentTopInPage))
			{
				return false;
			}
			ItemSizes contentSize = null;
			bool flag = base.ResolveItemHiddenState(rplWriter, interactivity, pageContext, false, ref contentSize);
			parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Bottom);
			if (rplWriter != null)
			{
				if (!flag)
				{
					this.CalculateRichTextElements(pageContext);
					this.MeasureTextBox(pageContext, contentSize, false);
					this.WriteItemToStream(rplWriter, pageContext);
				}
				else
				{
					this.m_calcSizeState = CalcSize.Done;
				}
				if (base.m_itemRenderSizes == null)
				{
					this.CreateItemRenderSizes(contentSize, pageContext, false);
				}
			}
			else
			{
				this.m_calcSizeState = CalcSize.Done;
			}
			return true;
		}

		internal override void CalculateRepeatWithPage(RPLWriter rplWriter, PageContext pageContext, PageItem[] siblings)
		{
			base.AdjustOriginFromItemsAbove(siblings, null);
			ItemSizes contentSize = null;
			if (!base.ResolveItemHiddenState(rplWriter, null, pageContext, true, ref contentSize))
			{
				this.CalculateRichTextElements(pageContext);
				this.MeasureTextBox(pageContext, contentSize, true);
			}
			if (base.m_itemRenderSizes == null)
			{
				this.CreateItemRenderSizes(contentSize, pageContext, true);
			}
		}

		internal override int WriteRepeatWithToPage(RPLWriter rplWriter, PageContext pageContext)
		{
			if (base.ItemState == State.OnPageHidden)
			{
				return 0;
			}
			RegisterItem.RegisterPageItem(this, pageContext, pageContext.EvaluatePageHeaderFooter, null);
			this.WriteItemToStream(rplWriter, pageContext);
			return 1;
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				if (textBox.CanGrow)
				{
					spbifWriter.Write((byte)25);
					spbifWriter.Write(textBox.CanGrow);
				}
				if (textBox.CanShrink)
				{
					spbifWriter.Write((byte)26);
					spbifWriter.Write(textBox.CanShrink);
				}
				if (textBox.CanSort)
				{
					spbifWriter.Write((byte)29);
					spbifWriter.Write(textBox.CanSort);
				}
				ReportStringProperty reportStringProperty = null;
				bool isSimple = textBox.IsSimple;
				AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = null;
				if (isSimple)
				{
					textRun = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].TextRuns)[0];
					reportStringProperty = textRun.Value;
				}
				if (reportStringProperty != null && reportStringProperty.IsExpression)
				{
					spbifWriter.Write((byte)31);
					spbifWriter.Write(reportStringProperty.ExpressionString);
				}
				if (textBox.IsToggleParent)
				{
					spbifWriter.Write((byte)32);
					spbifWriter.Write(textBox.IsToggleParent);
				}
				if (textBox.SharedTypeCode != TypeCode.String)
				{
					spbifWriter.Write((byte)33);
					spbifWriter.Write((byte)textBox.SharedTypeCode);
				}
				if (textBox.FormattedValueExpressionBased)
				{
					spbifWriter.Write((byte)45);
					spbifWriter.Write(textBox.FormattedValueExpressionBased);
				}
				if (!textBox.HideDuplicates && reportStringProperty != null && reportStringProperty.Value != null && !textBox.FormattedValueExpressionBased)
				{
					spbifWriter.Write((byte)27);
					if (textRun.SharedTypeCode == TypeCode.String)
					{
						spbifWriter.Write(reportStringProperty.Value);
					}
					else
					{
						spbifWriter.Write(textRun.Instance.Value);
					}
				}
				if (!isSimple)
				{
					spbifWriter.Write((byte)35);
					spbifWriter.Write(isSimple);
				}
				pageContext.HideDuplicates = textBox.HideDuplicates;
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)sharedProps;
				rPLTextBoxPropsDef.CanGrow = textBox.CanGrow;
				rPLTextBoxPropsDef.CanShrink = textBox.CanShrink;
				rPLTextBoxPropsDef.CanSort = textBox.CanSort;
				ReportStringProperty reportStringProperty = null;
				bool isSimple = textBox.IsSimple;
				AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = null;
				if (isSimple)
				{
					textRun = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].TextRuns)[0];
					reportStringProperty = textRun.Value;
				}
				if (reportStringProperty != null && reportStringProperty.IsExpression)
				{
					rPLTextBoxPropsDef.Formula = reportStringProperty.ExpressionString;
				}
				rPLTextBoxPropsDef.IsToggleParent = textBox.IsToggleParent;
				rPLTextBoxPropsDef.SharedTypeCode = textBox.SharedTypeCode;
				rPLTextBoxPropsDef.FormattedValueExpressionBased = textBox.FormattedValueExpressionBased;
				if (!textBox.HideDuplicates && reportStringProperty != null && reportStringProperty.Value != null && !textBox.FormattedValueExpressionBased)
				{
					if (textRun.SharedTypeCode == TypeCode.String)
					{
						rPLTextBoxPropsDef.Value = reportStringProperty.Value;
					}
					else
					{
						rPLTextBoxPropsDef.Value = textRun.Instance.Value;
					}
				}
				rPLTextBoxPropsDef.IsSimple = isSimple;
				pageContext.HideDuplicates = textBox.HideDuplicates;
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			TextBoxInstance textBoxInstance = base.m_source.Instance as TextBoxInstance;
			if (textBoxInstance == null)
			{
				DataRegion dataRegion = base.m_source as DataRegion;
				if (dataRegion != null)
				{
					DataRegionInstance dataRegionInstance = (DataRegionInstance)dataRegion.Instance;
					if (dataRegionInstance.NoRowsMessage != null)
					{
						spbifWriter.Write((byte)27);
						spbifWriter.Write(dataRegionInstance.NoRowsMessage);
					}
				}
				else
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.SubReport;
					SubReportInstance subReportInstance = (SubReportInstance)subReport.Instance;
					if (subReportInstance.ProcessedWithError)
					{
						spbifWriter.Write((byte)27);
						spbifWriter.Write(subReportInstance.ErrorMessage);
					}
					else if (subReportInstance.NoRowsMessage != null)
					{
						spbifWriter.Write((byte)27);
						spbifWriter.Write(subReportInstance.NoRowsMessage);
					}
				}
			}
			else
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = (AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)base.m_source;
				pageContext.HideDuplicates = textBox.HideDuplicates;
				if (textBox.IsToggleParent && textBoxInstance.IsToggleParent)
				{
					spbifWriter.Write((byte)32);
					spbifWriter.Write(textBoxInstance.IsToggleParent);
					if (textBoxInstance.ToggleState)
					{
						spbifWriter.Write((byte)28);
						spbifWriter.Write(textBoxInstance.ToggleState);
					}
					if (pageContext.RegisterEvents)
					{
						textBoxInstance.RegisterToggleSender();
					}
				}
				if (textBox.CanSort)
				{
					switch (textBoxInstance.SortState)
					{
					case SortOptions.Ascending:
						spbifWriter.Write((byte)30);
						spbifWriter.Write((byte)1);
						break;
					case SortOptions.Descending:
						spbifWriter.Write((byte)30);
						spbifWriter.Write((byte)2);
						break;
					}
				}
				spbifWriter.Write((byte)36);
				this.m_contentHeightPosition = spbifWriter.BaseStream.Position;
				spbifWriter.Write(this.m_contentHeight);
				if (this.m_isSimple && !this.HideDuplicate(textBox, textBoxInstance, pageContext))
				{
					ReportElement source = this.m_paragraphs[0].TextRuns[0].Source;
					if (textBoxInstance.ProcessedWithError)
					{
						spbifWriter.Write((byte)46);
						spbifWriter.Write(true);
					}
					if (pageContext.HideDuplicates || textBox.FormattedValueExpressionBased)
					{
						if (pageContext.AddOriginalValue)
						{
							object originalValue = textBoxInstance.OriginalValue;
							TypeCode typeCode = textBoxInstance.TypeCode;
							TypeCode sharedTypeCode = textBox.SharedTypeCode;
							if ((byte)typeCode != (byte)sharedTypeCode)
							{
								spbifWriter.Write((byte)33);
								spbifWriter.Write((byte)typeCode);
							}
							pageContext.TypeCodeNonString = this.WriteOriginalValue(spbifWriter, typeCode, originalValue);
							if (!pageContext.TypeCodeNonString && textBoxInstance.Value != null)
							{
								if (pageContext.AddToggledItems)
								{
									object richTextStyleValue = this.m_paragraphs[0].TextRuns[0].GetRichTextStyleValue(StyleAttributeNames.Format, null);
									if (richTextStyleValue != null)
									{
										spbifWriter.Write((byte)27);
										spbifWriter.Write(textBoxInstance.Value);
									}
								}
								else
								{
									spbifWriter.Write((byte)27);
									spbifWriter.Write(textBoxInstance.Value);
								}
							}
						}
						else
						{
							TypeCode typeCode = textBoxInstance.TypeCode;
							TypeCode sharedTypeCode = textBox.SharedTypeCode;
							if ((byte)typeCode != (byte)sharedTypeCode)
							{
								spbifWriter.Write((byte)33);
								spbifWriter.Write((byte)typeCode);
							}
							if (textBoxInstance.Value != null)
							{
								spbifWriter.Write((byte)27);
								spbifWriter.Write(textBoxInstance.Value);
							}
						}
					}
					else if (pageContext.AddOriginalValue && textBox.SharedTypeCode != TypeCode.String)
					{
						pageContext.TypeCodeNonString = this.WriteOriginalValue(spbifWriter, textBox.SharedTypeCode, textBoxInstance.OriginalValue);
						if (textBoxInstance.Value != null)
						{
							if (pageContext.AddToggledItems)
							{
								object richTextStyleValue2 = this.m_paragraphs[0].TextRuns[0].GetRichTextStyleValue(StyleAttributeNames.Format, null);
								if (richTextStyleValue2 != null)
								{
									spbifWriter.Write((byte)27);
									spbifWriter.Write(textBoxInstance.Value);
								}
							}
							else
							{
								spbifWriter.Write((byte)27);
								spbifWriter.Write(textBoxInstance.Value);
							}
						}
					}
				}
				base.WriteActionInfo(textBox.ActionInfo, spbifWriter, pageContext, 7);
			}
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			RPLTextBoxProps rPLTextBoxProps = (RPLTextBoxProps)nonSharedProps;
			TextBoxInstance textBoxInstance = base.m_source.Instance as TextBoxInstance;
			if (textBoxInstance == null)
			{
				DataRegion dataRegion = base.m_source as DataRegion;
				if (dataRegion != null)
				{
					DataRegionInstance dataRegionInstance = (DataRegionInstance)dataRegion.Instance;
					rPLTextBoxProps.Value = dataRegionInstance.NoRowsMessage;
				}
				else
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.SubReport;
					SubReportInstance subReportInstance = (SubReportInstance)subReport.Instance;
					if (subReportInstance.ProcessedWithError)
					{
						rPLTextBoxProps.Value = subReportInstance.ErrorMessage;
					}
					else
					{
						rPLTextBoxProps.Value = subReportInstance.NoRowsMessage;
					}
				}
			}
			else
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = (AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)base.m_source;
				pageContext.HideDuplicates = textBox.HideDuplicates;
				if (textBox.IsToggleParent && textBoxInstance.IsToggleParent)
				{
					rPLTextBoxProps.IsToggleParent = textBoxInstance.IsToggleParent;
					rPLTextBoxProps.ToggleState = textBoxInstance.ToggleState;
					if (pageContext.RegisterEvents)
					{
						textBoxInstance.RegisterToggleSender();
					}
				}
				switch (textBoxInstance.SortState)
				{
				case SortOptions.Ascending:
					rPLTextBoxProps.SortState = RPLFormat.SortOptions.Ascending;
					break;
				case SortOptions.Descending:
					rPLTextBoxProps.SortState = RPLFormat.SortOptions.Descending;
					break;
				case SortOptions.None:
					rPLTextBoxProps.SortState = RPLFormat.SortOptions.None;
					break;
				}
				rPLTextBoxProps.ContentHeight = this.m_contentHeight;
				rPLTextBoxProps.TypeCode = textBox.SharedTypeCode;
				if (this.m_isSimple && !this.HideDuplicate(textBox, textBoxInstance, pageContext))
				{
					ReportElement source = this.m_paragraphs[0].TextRuns[0].Source;
					rPLTextBoxProps.ProcessedWithError = textBoxInstance.ProcessedWithError;
					if (pageContext.HideDuplicates || textBox.FormattedValueExpressionBased)
					{
						if (pageContext.AddOriginalValue)
						{
							object originalValue = textBoxInstance.OriginalValue;
							TypeCode typeCode = textBoxInstance.TypeCode;
							TypeCode sharedTypeCode = textBox.SharedTypeCode;
							if ((byte)typeCode != (byte)sharedTypeCode)
							{
								rPLTextBoxProps.TypeCode = typeCode;
							}
							pageContext.TypeCodeNonString = this.WriteOriginalValue(rPLTextBoxProps, typeCode, originalValue);
							if (!pageContext.TypeCodeNonString && textBoxInstance.Value != null)
							{
								if (pageContext.AddToggledItems)
								{
									object richTextStyleValue = this.m_paragraphs[0].TextRuns[0].GetRichTextStyleValue(StyleAttributeNames.Format, null);
									if (richTextStyleValue != null)
									{
										rPLTextBoxProps.Value = textBoxInstance.Value;
									}
								}
								else
								{
									rPLTextBoxProps.Value = textBoxInstance.Value;
								}
							}
						}
						else
						{
							TypeCode typeCode = textBoxInstance.TypeCode;
							TypeCode sharedTypeCode = textBox.SharedTypeCode;
							if ((byte)typeCode != (byte)sharedTypeCode)
							{
								rPLTextBoxProps.TypeCode = typeCode;
							}
							rPLTextBoxProps.Value = textBoxInstance.Value;
						}
					}
					else if (pageContext.AddOriginalValue && textBox.SharedTypeCode != TypeCode.String)
					{
						pageContext.TypeCodeNonString = this.WriteOriginalValue(rPLTextBoxProps, textBox.SharedTypeCode, textBoxInstance.OriginalValue);
						if (textBoxInstance.Value != null)
						{
							if (pageContext.AddToggledItems)
							{
								object richTextStyleValue2 = this.m_paragraphs[0].TextRuns[0].GetRichTextStyleValue(StyleAttributeNames.Format, null);
								if (richTextStyleValue2 != null)
								{
									rPLTextBoxProps.Value = textBoxInstance.Value;
								}
							}
							else
							{
								rPLTextBoxProps.Value = textBoxInstance.Value;
							}
						}
					}
				}
				rPLTextBoxProps.ActionInfo = base.WriteActionInfo(textBox.ActionInfo, pageContext);
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			this.WriteBackgroundImage(spbifWriter, style, true, pageContext);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.UnicodeBiDi, 31);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.VerticalAlign, 26);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.WritingMode, 30, pageContext.VersionPicker);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Direction, 29);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingBottom, 18);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingLeft, 15);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingRight, 16);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingTop, 17);
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Color, 27);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontFamily, 20);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontSize, 21);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontStyle, 19);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontWeight, 22);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Format, 23);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Language, 32);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.LineHeight, 28);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.NumeralLanguage, 36);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.NumeralVariant, 37);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Calendar, 38);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.TextAlign, 25);
				base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
			}
			else if (this.m_isSimple)
			{
				Style style2 = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].Style;
				this.m_paragraphs[0].WriteItemSharedStyleProps(spbifWriter, style2, pageContext);
				Style style3 = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].TextRuns)[0].Style;
				this.m_paragraphs[0].TextRuns[0].WriteItemSharedStyleProps(spbifWriter, style3, pageContext);
			}
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			this.WriteBackgroundImage(rplStyleProps, style, true, pageContext);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.UnicodeBiDi, 31);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.VerticalAlign, 26);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.WritingMode, 30);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Direction, 29);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Color, 27);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontSize, 21);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Format, 23);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Language, 32);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.LineHeight, 28);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.NumeralLanguage, 36);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.NumeralVariant, 37);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Calendar, 38);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.TextAlign, 25);
				base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
			}
			else if (this.m_isSimple)
			{
				Style style2 = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].Style;
				this.m_paragraphs[0].WriteItemSharedStyleProps(rplStyleProps, style2, pageContext);
				Style style3 = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0].TextRuns)[0].Style;
				this.m_paragraphs[0].TextRuns[0].WriteItemSharedStyleProps(rplStyleProps, style3, pageContext);
			}
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				this.WriteBackgroundImage(spbifWriter, styleDef, false, pageContext);
				break;
			case StyleAttributeNames.UnicodeBiDi:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.UnicodeBiDi, 31);
				break;
			case StyleAttributeNames.VerticalAlign:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.VerticalAlign, 26);
				break;
			case StyleAttributeNames.WritingMode:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.WritingMode, 30, pageContext.VersionPicker);
				break;
			case StyleAttributeNames.Direction:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Direction, 29);
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
			default:
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
				if (textBox == null)
				{
					switch (styleAtt)
					{
					case StyleAttributeNames.VerticalAlign:
					case StyleAttributeNames.PaddingLeft:
					case StyleAttributeNames.PaddingRight:
					case StyleAttributeNames.PaddingTop:
					case StyleAttributeNames.PaddingBottom:
					case StyleAttributeNames.Direction:
					case StyleAttributeNames.WritingMode:
					case StyleAttributeNames.UnicodeBiDi:
						break;
					case StyleAttributeNames.TextAlign:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.TextAlign, 25);
						break;
					case StyleAttributeNames.LineHeight:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.LineHeight, 28);
						break;
					case StyleAttributeNames.Color:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Color, 27);
						break;
					case StyleAttributeNames.FontFamily:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontFamily, 20);
						break;
					case StyleAttributeNames.FontSize:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontSize, 21);
						break;
					case StyleAttributeNames.FontStyle:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontStyle, 19);
						break;
					case StyleAttributeNames.FontWeight:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontWeight, 22);
						break;
					case StyleAttributeNames.Format:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Format, 23);
						break;
					case StyleAttributeNames.Language:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Language, 32);
						break;
					case StyleAttributeNames.NumeralLanguage:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.NumeralLanguage, 36);
						break;
					case StyleAttributeNames.NumeralVariant:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.NumeralVariant, 37);
						break;
					case StyleAttributeNames.Calendar:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Calendar, 38);
						break;
					case StyleAttributeNames.TextDecoration:
						base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
						break;
					}
				}
				break;
			}
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				this.WriteBackgroundImage(rplStyleProps, styleDef, false, pageContext);
				break;
			case StyleAttributeNames.UnicodeBiDi:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.UnicodeBiDi, 31);
				break;
			case StyleAttributeNames.VerticalAlign:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.VerticalAlign, 26);
				break;
			case StyleAttributeNames.WritingMode:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.WritingMode, 30);
				break;
			case StyleAttributeNames.Direction:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Direction, 29);
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
			default:
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
				if (textBox == null)
				{
					switch (styleAtt)
					{
					case StyleAttributeNames.VerticalAlign:
					case StyleAttributeNames.PaddingLeft:
					case StyleAttributeNames.PaddingRight:
					case StyleAttributeNames.PaddingTop:
					case StyleAttributeNames.PaddingBottom:
					case StyleAttributeNames.Direction:
					case StyleAttributeNames.WritingMode:
					case StyleAttributeNames.UnicodeBiDi:
						break;
					case StyleAttributeNames.Color:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Color, 27);
						break;
					case StyleAttributeNames.FontFamily:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
						break;
					case StyleAttributeNames.FontSize:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontSize, 21);
						break;
					case StyleAttributeNames.FontStyle:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
						break;
					case StyleAttributeNames.FontWeight:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
						break;
					case StyleAttributeNames.Format:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Format, 23);
						break;
					case StyleAttributeNames.Language:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Language, 32);
						break;
					case StyleAttributeNames.LineHeight:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.LineHeight, 28);
						break;
					case StyleAttributeNames.NumeralLanguage:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.NumeralLanguage, 36);
						break;
					case StyleAttributeNames.NumeralVariant:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.NumeralVariant, 37);
						break;
					case StyleAttributeNames.Calendar:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Calendar, 38);
						break;
					case StyleAttributeNames.TextAlign:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.TextAlign, 25);
						break;
					case StyleAttributeNames.TextDecoration:
						base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
						break;
					}
				}
				break;
			}
			}
		}

		internal override void WriteNonSharedStyle(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, PageContext pageContext, byte? rplTag, ReportElementInstance compiledSource)
		{
			if (base.m_source != null)
			{
				bool flag = base.WriteCommonNonSharedStyle(spbifWriter, styleDef, style, pageContext, rplTag, compiledSource);
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
				if (this.m_isSimple && textBox != null)
				{
					if (!flag)
					{
						int? nullable = rplTag;
						if (nullable.HasValue)
						{
							spbifWriter.Write(rplTag.Value);
						}
						spbifWriter.Write((byte)1);
						flag = true;
					}
					Paragraph paragraph = this.m_paragraphs[0];
					AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph paragraph2 = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0];
					Style style2 = paragraph2.Style;
					List<StyleAttributeNames> nonSharedStyleAttributes = style2.NonSharedStyleAttributes;
					if (nonSharedStyleAttributes != null && nonSharedStyleAttributes.Count > 0)
					{
						StyleInstance style3 = paragraph2.Instance.Style;
						for (int i = 0; i < nonSharedStyleAttributes.Count; i++)
						{
							paragraph.WriteNonSharedStyleProp(spbifWriter, style2, style3, nonSharedStyleAttributes[i], pageContext);
						}
					}
					AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)paragraph2.TextRuns)[0];
					Style style4 = textRun.Style;
					nonSharedStyleAttributes = style4.NonSharedStyleAttributes;
					if (nonSharedStyleAttributes != null && nonSharedStyleAttributes.Count > 0)
					{
						TextRun textRun2 = paragraph.TextRuns[0];
						StyleInstance style5 = textRun.Instance.Style;
						for (int j = 0; j < nonSharedStyleAttributes.Count; j++)
						{
							textRun2.WriteNonSharedStyleProp(spbifWriter, style4, style5, nonSharedStyleAttributes[j], pageContext);
						}
					}
				}
				if (flag)
				{
					spbifWriter.Write((byte)255);
				}
			}
		}

		internal override RPLStyleProps WriteNonSharedStyle(Style styleDef, StyleInstance style, PageContext pageContext, ReportElementInstance compiledSource)
		{
			RPLStyleProps rPLStyleProps = base.WriteNonSharedStyle(styleDef, style, pageContext, compiledSource);
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (this.m_isSimple && textBox != null)
			{
				if (rPLStyleProps == null)
				{
					rPLStyleProps = new RPLStyleProps();
				}
				Paragraph paragraph = this.m_paragraphs[0];
				AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph paragraph2 = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.Paragraph>)textBox.Paragraphs)[0];
				Style style2 = paragraph2.Style;
				List<StyleAttributeNames> nonSharedStyleAttributes = style2.NonSharedStyleAttributes;
				if (nonSharedStyleAttributes != null && nonSharedStyleAttributes.Count > 0)
				{
					StyleInstance style3 = paragraph2.Instance.Style;
					for (int i = 0; i < nonSharedStyleAttributes.Count; i++)
					{
						paragraph.WriteNonSharedStyleProp(rPLStyleProps, style2, style3, nonSharedStyleAttributes[i], pageContext);
					}
				}
				AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.TextRun>)paragraph2.TextRuns)[0];
				Style style4 = textRun.Style;
				nonSharedStyleAttributes = style4.NonSharedStyleAttributes;
				if (nonSharedStyleAttributes != null && nonSharedStyleAttributes.Count > 0)
				{
					TextRun textRun2 = paragraph.TextRuns[0];
					StyleInstance style5 = textRun.Instance.Style;
					for (int j = 0; j < nonSharedStyleAttributes.Count; j++)
					{
						textRun2.WriteNonSharedStyleProp(rPLStyleProps, style4, style5, nonSharedStyleAttributes[j], pageContext);
					}
				}
			}
			return rPLStyleProps;
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)12);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)255);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper(12);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}

		internal void MeasureTextBox(PageContext pageContext, ItemSizes contentSize, bool createForRepeat)
		{
			double num3;
			if (this.m_calcSizeState != CalcSize.Done)
			{
				if (pageContext != null && pageContext.MeasureItems)
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
					if (textBox != null && this.m_richTextBox == null && (textBox.CanGrow || textBox.CanShrink))
					{
						if (this.m_calcSizeState == CalcSize.Delay)
						{
							return;
						}
						if (!pageContext.Common.EmSquare)
						{
							this.m_richTextBox = new AspNetCore.ReportingServices.Rendering.RichText.TextBox(this);
						}
						if (!this.HideDuplicate(textBox, (TextBoxInstance)textBox.Instance, pageContext))
						{
							double num = 0.0;
							float num2 = (!pageContext.Common.EmSquare) ? this.MeasureTextBox_Uniscribe(pageContext, contentSize, textBox, ref num) : this.MeasureTextBox_GDI(pageContext, contentSize, textBox, ref num);
							num3 = 0.0;
							num3 = ((contentSize != null) ? ((double)num2 - (contentSize.Height - num)) : ((double)num2 - (base.m_itemPageSizes.Height - num)));
							if (num3 > 0.0 && textBox.CanGrow)
							{
								goto IL_0112;
							}
							if (num3 < 0.0 && textBox.CanShrink)
							{
								goto IL_0112;
							}
						}
						else if (textBox.CanShrink)
						{
							if (base.m_itemRenderSizes == null)
							{
								this.CreateItemRenderSizes(contentSize, pageContext, createForRepeat);
							}
							base.m_itemRenderSizes.AdjustHeightTo(0.0);
						}
					}
				}
				goto IL_0175;
			}
			return;
			IL_0112:
			if (base.m_itemRenderSizes == null)
			{
				this.CreateItemRenderSizes(contentSize, pageContext, createForRepeat);
			}
			base.m_itemRenderSizes.AdjustHeightTo(base.m_itemRenderSizes.Height + num3 + 0.0001);
			goto IL_0175;
			IL_0175:
			this.m_calcSizeState = CalcSize.Done;
		}

		private float MeasureTextBox_Uniscribe(PageContext pageContext, ItemSizes contentSize, AspNetCore.ReportingServices.OnDemandReportRendering.TextBox tbDef, ref double padVertical)
		{
			double padHorizontal = 0.0;
			this.CalculateTotalPaddings(pageContext, ref padHorizontal, ref padVertical);
			float num = 0f;
			float num2 = 0f;
			float contentHeight = 0f;
			FlowContext flowContext = new FlowContext(this.TextBoxWidth(tbDef, pageContext, padHorizontal), 3.40282347E+38f);
			if (tbDef.CanGrow && !tbDef.CanShrink)
			{
				flowContext.Height = (float)(base.m_itemPageSizes.Height - padVertical);
			}
			if (this.WritingMode == RPLFormat.WritingModes.Horizontal)
			{
				this.m_richTextBox.Paragraphs = new List<AspNetCore.ReportingServices.Rendering.RichText.Paragraph>(1);
				for (int i = 0; i < this.m_paragraphs.Count; i++)
				{
					AspNetCore.ReportingServices.Rendering.RichText.Paragraph richTextParagraph = this.m_paragraphs[i].GetRichTextParagraph();
					this.m_richTextBox.Paragraphs.Add(richTextParagraph);
					this.m_richTextBox.ScriptItemize();
					float num3 = 0f;
					float num4 = pageContext.MeasureFullTextBoxHeight(this.m_richTextBox, flowContext, out num3);
					num += num4;
					num2 += num4;
					this.m_richTextBox.Paragraphs.RemoveAt(0);
				}
				this.m_contentHeight = num2;
			}
			else
			{
				this.m_richTextBox.Paragraphs = new List<AspNetCore.ReportingServices.Rendering.RichText.Paragraph>(this.m_paragraphs.Count);
				for (int j = 0; j < this.m_paragraphs.Count; j++)
				{
					AspNetCore.ReportingServices.Rendering.RichText.Paragraph richTextParagraph2 = this.m_paragraphs[j].GetRichTextParagraph();
					this.m_richTextBox.Paragraphs.Add(richTextParagraph2);
				}
				this.m_richTextBox.ScriptItemize();
				num = pageContext.MeasureFullTextBoxHeight(this.m_richTextBox, flowContext, out contentHeight);
				this.m_contentHeight = contentHeight;
			}
			return num;
		}

		private float MeasureTextBox_GDI(PageContext pageContext, ItemSizes contentSize, AspNetCore.ReportingServices.OnDemandReportRendering.TextBox tbDef, ref double padVertical)
		{
			float result = 0f;
			double padHorizontal = 0.0;
			int num = 0;
			int num2 = 0;
			CanvasFont canvasFont = default(CanvasFont);
			bool flag = default(bool);
			bool flag2 = default(bool);
			string text = this.AggregateTextBoxStyle(pageContext, ref padHorizontal, ref padVertical, out canvasFont, out flag, out flag2);
			if (!string.IsNullOrEmpty(text))
			{
				result = (this.m_contentHeight = pageContext.MeasureStringGDI(text, canvasFont, new SizeF(this.TextBoxWidth(tbDef, pageContext, padHorizontal), 3.40282347E+38f), out num, out num2).Height);
			}
			if (canvasFont != null)
			{
				if (flag && flag2)
				{
					canvasFont.Dispose();
					canvasFont = null;
				}
				else if (flag)
				{
					if (canvasFont.GDIFont != null)
					{
						canvasFont.GDIFont.Dispose();
					}
				}
				else if (flag2 && canvasFont.TrimStringFormat != null)
				{
					canvasFont.TrimStringFormat.Dispose();
				}
			}
			return result;
		}

		private string AggregateTextBoxStyle(PageContext pageContext, ref double padHorizontal, ref double padVertical, out CanvasFont font, out bool newFont, out bool newStringFormat)
		{
			this.CalculateTotalPaddings(pageContext, ref padHorizontal, ref padVertical);
			StringBuilder stringBuilder = new StringBuilder();
			font = null;
			bool flag = true;
			newFont = false;
			newStringFormat = false;
			if (this.m_paragraphs != null)
			{
				foreach (Paragraph paragraph2 in this.m_paragraphs)
				{
					if (paragraph2.TextRuns != null)
					{
						if (!flag)
						{
							stringBuilder.AppendLine();
						}
						flag = false;
						StringBuilder stringBuilder2 = new StringBuilder();
						foreach (TextRun textRun in paragraph2.TextRuns)
						{
							string value = textRun.ComputeValue();
							if (!string.IsNullOrEmpty(value))
							{
								stringBuilder2.Append(value);
								if (font == null)
								{
									font = this.CreateFontPens(pageContext, paragraph2, textRun, out newFont, out newStringFormat);
								}
							}
						}
						if (stringBuilder2.Length > 0)
						{
							string value2 = stringBuilder2.ToString();
							TextBox.AddNewLinesAtGdiLimits(ref value2);
							stringBuilder.Append(value2);
						}
					}
				}
			}
			if (font == null && stringBuilder.Length > 0)
			{
				Paragraph paragraph = this.m_paragraphs[0];
				font = this.CreateFontPens(pageContext, paragraph, paragraph.TextRuns[0], out newFont, out newStringFormat);
			}
			return stringBuilder.ToString();
		}

		internal bool SearchTextBox(string findValue, PageContext pageContext)
		{
			string text = null;
			TextBoxInstance textBoxInstance = base.m_source.Instance as TextBoxInstance;
			if (textBoxInstance == null)
			{
				DataRegion dataRegion = base.m_source as DataRegion;
				if (dataRegion != null)
				{
					DataRegionInstance dataRegionInstance = (DataRegionInstance)dataRegion.Instance;
					text = dataRegionInstance.NoRowsMessage;
				}
				else
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.SubReport;
					SubReportInstance subReportInstance = (SubReportInstance)subReport.Instance;
					text = ((!subReportInstance.ProcessedWithError) ? subReportInstance.NoRowsMessage : SPBRes.RenderSubreportError);
				}
			}
			else
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.TextBox tbDef = (AspNetCore.ReportingServices.OnDemandReportRendering.TextBox)base.m_source;
				if (!this.HideDuplicate(tbDef, textBoxInstance, pageContext))
				{
					text = textBoxInstance.Value;
				}
			}
			if (text != null)
			{
				string text2 = findValue;
				if (text2.IndexOf(' ') >= 0)
				{
					text2 = text2.Replace('\u00a0', ' ');
					text = text.Replace('\u00a0', ' ');
				}
				int num = text.IndexOf(text2, 0, StringComparison.OrdinalIgnoreCase);
				if (num >= 0)
				{
					return true;
				}
			}
			return false;
		}

		private float TextBoxWidth(AspNetCore.ReportingServices.OnDemandReportRendering.TextBox tbDef, PageContext pageContext, double padHorizontal)
		{
			double num = base.m_itemPageSizes.Width - padHorizontal;
			double num2 = pageContext.ConvertToMillimeters(11, pageContext.DpiX) + 2.2;
			if (num > num2 && tbDef.IsToggleParent && ((TextBoxInstance)tbDef.Instance).IsToggleParent)
			{
				num -= num2;
			}
			num2 = pageContext.ConvertToMillimeters(16, pageContext.DpiY) + 2.2;
			if (num > num2 && tbDef.CanSort)
			{
				num -= num2;
			}
			return (float)num;
		}

		private bool HideDuplicate(AspNetCore.ReportingServices.OnDemandReportRendering.TextBox tbDef, TextBoxInstance tbInst, PageContext pageContext)
		{
			if (!tbDef.HideDuplicates)
			{
				return false;
			}
			TextBoxSharedInfo textBoxSharedInfo = null;
			if (pageContext.TextBoxSharedInfo != null)
			{
				textBoxSharedInfo = (TextBoxSharedInfo)pageContext.TextBoxSharedInfo[tbDef.ID];
			}
			if (!tbInst.Duplicate)
			{
				if (textBoxSharedInfo == null)
				{
					if (pageContext.TextBoxSharedInfo == null)
					{
						pageContext.TextBoxSharedInfo = new Hashtable();
					}
					pageContext.TextBoxSharedInfo.Add(tbDef.ID, new TextBoxSharedInfo(pageContext.PageNumber));
				}
				else
				{
					textBoxSharedInfo.PageNumber = pageContext.PageNumber;
				}
			}
			else
			{
				if (textBoxSharedInfo != null && textBoxSharedInfo.PageNumber >= pageContext.PageNumber)
				{
					return true;
				}
				if (textBoxSharedInfo == null)
				{
					if (pageContext.TextBoxSharedInfo == null)
					{
						pageContext.TextBoxSharedInfo = new Hashtable();
					}
					pageContext.TextBoxSharedInfo.Add(tbDef.ID, new TextBoxSharedInfo(pageContext.PageNumber));
				}
				else
				{
					textBoxSharedInfo.PageNumber = pageContext.PageNumber;
				}
			}
			return false;
		}

		private void CalculateTotalPaddings(PageContext pageContext, ref double padHorizontal, ref double padVertical)
		{
			ReportSize reportSize = (ReportSize)base.GetRichTextStyleValue(StyleAttributeNames.PaddingTop, null);
			if (reportSize != null)
			{
				padVertical += reportSize.ToMillimeters();
			}
			reportSize = (ReportSize)base.GetRichTextStyleValue(StyleAttributeNames.PaddingBottom, null);
			if (reportSize != null)
			{
				padVertical += reportSize.ToMillimeters();
			}
			reportSize = (ReportSize)base.GetRichTextStyleValue(StyleAttributeNames.PaddingLeft, null);
			if (reportSize != null)
			{
				padHorizontal += reportSize.ToMillimeters();
			}
			reportSize = (ReportSize)base.GetRichTextStyleValue(StyleAttributeNames.PaddingRight, null);
			if (reportSize != null)
			{
				padHorizontal += reportSize.ToMillimeters();
			}
		}

		private void CalculateRichTextElements(PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				TextBoxInstance textBoxInstance = textBox.Instance as TextBoxInstance;
				if (textBoxInstance != null)
				{
					IEnumerator<ParagraphInstance> enumerator = textBoxInstance.ParagraphInstances.GetEnumerator();
					ParagraphNumberCalculator paragraphNumberCalculator = new ParagraphNumberCalculator();
					while (enumerator.MoveNext())
					{
						ParagraphInstance current = enumerator.Current;
						if (this.m_paragraphs == null)
						{
							this.m_paragraphs = new List<Paragraph>();
						}
						Paragraph paragraph = null;
						paragraph = ((!current.IsCompiled) ? new Paragraph(current.Definition, pageContext) : new Paragraph(current.Definition, current as CompiledParagraphInstance, pageContext));
						paragraphNumberCalculator.UpdateParagraph(paragraph);
						this.m_paragraphs.Add(paragraph);
					}
				}
			}
		}

		private bool WriteOriginalValue(BinaryWriter spbifWriter, TypeCode typeCode, object value)
		{
			if (value == null)
			{
				return false;
			}
			return this.WriteObjectValue(spbifWriter, 34, typeCode, value);
		}

		private bool WriteOriginalValue(RPLTextBoxProps textBoxProps, TypeCode typeCode, object value)
		{
			if (value == null)
			{
				return false;
			}
			return this.WriteObjectValue(textBoxProps, typeCode, value);
		}

		private bool WriteObjectValue(BinaryWriter spbifWriter, byte name, TypeCode typeCode, object value)
		{
			bool result = false;
			spbifWriter.Write(name);
			switch (typeCode)
			{
			case TypeCode.Byte:
				spbifWriter.Write((byte)value);
				break;
			case TypeCode.Decimal:
				spbifWriter.Write((decimal)value);
				break;
			case TypeCode.Double:
				spbifWriter.Write((double)value);
				break;
			case TypeCode.Int16:
				spbifWriter.Write((short)value);
				break;
			case TypeCode.Int32:
				spbifWriter.Write((int)value);
				break;
			case TypeCode.Int64:
				spbifWriter.Write((long)value);
				break;
			case TypeCode.SByte:
				spbifWriter.Write((sbyte)value);
				break;
			case TypeCode.Single:
				spbifWriter.Write((float)value);
				break;
			case TypeCode.UInt16:
				spbifWriter.Write((ushort)value);
				break;
			case TypeCode.UInt32:
				spbifWriter.Write((uint)value);
				break;
			case TypeCode.UInt64:
				spbifWriter.Write((ulong)value);
				break;
			case TypeCode.DateTime:
				spbifWriter.Write(((DateTime)value).ToBinary());
				break;
			case TypeCode.Boolean:
				spbifWriter.Write((bool)value);
				break;
			case TypeCode.Char:
				spbifWriter.Write((char)value);
				break;
			default:
				spbifWriter.Write(value.ToString());
				result = true;
				break;
			}
			return result;
		}

		private bool WriteObjectValue(RPLTextBoxProps textBoxProps, TypeCode typeCode, object value)
		{
			bool result = false;
			textBoxProps.OriginalValue = value;
			switch (typeCode)
			{
			default:
				result = true;
				break;
			case TypeCode.Boolean:
			case TypeCode.Char:
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
				break;
			}
			return result;
		}

		internal void DelayWriteContent(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				if (this.m_contentHeightPosition != -1)
				{
					BinaryWriter propertyCacheWriter = pageContext.PropertyCacheWriter;
					long position = propertyCacheWriter.BaseStream.Position;
					propertyCacheWriter.BaseStream.Seek(this.m_contentHeightPosition - position, SeekOrigin.Current);
					propertyCacheWriter.Write(this.m_contentHeight);
					BinaryReader propertyCacheReader = pageContext.PropertyCacheReader;
					propertyCacheReader.BaseStream.Seek(this.m_startTextBoxOffset, SeekOrigin.Begin);
					this.CopyData(propertyCacheReader, rplWriter, position - this.m_startTextBoxOffset);
				}
			}
			else
			{
				RPLTextBoxProps rPLTextBoxProps = (RPLTextBoxProps)base.m_rplElement.ElementProps;
				rPLTextBoxProps.ContentHeight = this.m_contentHeight;
			}
		}

		internal void CopyData(BinaryReader cacheReader, RPLWriter rplWriter, long length)
		{
			byte[] array = new byte[1024];
			while (length >= array.Length)
			{
				cacheReader.Read(array, 0, array.Length);
				rplWriter.BinaryWriter.Write(array, 0, array.Length);
				length -= array.Length;
			}
			if (length > 0)
			{
				cacheReader.Read(array, 0, (int)length);
				rplWriter.BinaryWriter.Write(array, 0, (int)length);
			}
		}

		private void WriteItemToStream(BinaryWriter spbifWriter, RPLWriter rplWriter, long offsetStart, PageContext pageContext)
		{
			spbifWriter.Write((byte)7);
			this.WriteElementProps(spbifWriter, rplWriter, pageContext, offsetStart + 1);
			List<long> list = null;
			if (!this.m_isSimple)
			{
				list = this.WriteRichTextElementProps(spbifWriter, rplWriter, pageContext);
			}
			offsetStart = spbifWriter.BaseStream.Position;
			spbifWriter.Write((byte)18);
			spbifWriter.Write(base.m_offset);
			if (list != null)
			{
				spbifWriter.Write(list.Count);
				foreach (long item in list)
				{
					spbifWriter.Write(item);
				}
			}
			else
			{
				spbifWriter.Write(0);
			}
			spbifWriter.Write((byte)255);
			base.m_offset = spbifWriter.BaseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(offsetStart);
			spbifWriter.Write((byte)255);
		}

		private void WriteItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				long num = this.m_startTextBoxOffset = (base.m_offset = baseStream.Position);
				if (this.m_calcSizeState == CalcSize.Delay)
				{
					pageContext.CreateCacheStream(num);
					RPLWriter rplWriter2 = new RPLWriter(pageContext.PropertyCacheWriter, rplWriter.Report, rplWriter.TablixRow);
					this.WriteItemToStream(pageContext.PropertyCacheWriter, rplWriter2, num, pageContext);
				}
				else
				{
					this.WriteItemToStream(binaryWriter, rplWriter, num, pageContext);
				}
			}
			else
			{
				base.m_rplElement = new RPLTextBox();
				this.WriteElementProps(base.m_rplElement.ElementProps, rplWriter, pageContext);
				if (!this.m_isSimple)
				{
					this.WriteRichTextElementProps(rplWriter, pageContext);
				}
			}
		}

		private List<long> WriteRichTextElementProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return null;
			}
			TextBoxInstance textBoxInstance = base.m_source.Instance as TextBoxInstance;
			if (textBoxInstance == null)
			{
				return null;
			}
			List<long> result = null;
			bool flag = this.HideDuplicate(textBox, textBoxInstance, pageContext);
			if (this.m_paragraphs != null && !flag)
			{
				result = new List<long>();
				{
					foreach (Paragraph paragraph in this.m_paragraphs)
					{
						paragraph.WriteItemToStream(rplWriter, pageContext);
						result.Add(paragraph.Offset);
					}
					return result;
				}
			}
			return result;
		}

		private void WriteRichTextElementProps(RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox != null)
			{
				TextBoxInstance textBoxInstance = base.m_source.Instance as TextBoxInstance;
				if (textBoxInstance != null)
				{
					RPLTextBox rPLTextBox = base.m_rplElement as RPLTextBox;
					bool flag = this.HideDuplicate(textBox, textBoxInstance, pageContext);
					if (this.m_paragraphs != null && !flag)
					{
						foreach (Paragraph paragraph in this.m_paragraphs)
						{
							paragraph.WriteItemToStream(rplWriter, pageContext);
							rPLTextBox.AddParagraph(paragraph.RPLElement);
						}
					}
				}
			}
		}

		private TextAlignments GetTextAlignmentValue(Paragraph paragraph, ref bool shared)
		{
			TextAlignments textAlignments = (TextAlignments)paragraph.GetRichTextStyleValue(StyleAttributeNames.TextAlign, ref shared);
			if (textAlignments == TextAlignments.General)
			{
				textAlignments = TextAlignments.Left;
				if (this.GetAlignmentRight(ref shared))
				{
					textAlignments = TextAlignments.Right;
				}
			}
			return textAlignments;
		}

		private bool GetAlignmentRight(ref bool shared)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextBox textBox = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return false;
			}
			TypeCode typeCode = textBox.SharedTypeCode;
			if (typeCode == TypeCode.Object)
			{
				shared = false;
				TextBoxInstance textBoxInstance = (TextBoxInstance)textBox.Instance;
				if (textBoxInstance.OriginalValue == null)
				{
					return false;
				}
				Type type = textBoxInstance.OriginalValue.GetType();
				typeCode = Type.GetTypeCode(type);
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

		private CanvasFont CreateFontPens(PageContext pageContext, Paragraph paragraph, TextRun textRun, out bool newFont, out bool newFormatString)
		{
			CanvasFont canvasFont = null;
			TextBoxSharedInfo textBoxSharedInfo = null;
			if (pageContext.TextBoxSharedInfo != null)
			{
				textBoxSharedInfo = (TextBoxSharedInfo)pageContext.TextBoxSharedInfo[base.m_source.ID];
			}
			if (textBoxSharedInfo != null && textBoxSharedInfo.SharedFont != null)
			{
				canvasFont = this.CreatePartialSharedFont(textBoxSharedInfo.SharedFont, textBoxSharedInfo.SharedState, paragraph, textRun, pageContext, out newFont, out newFormatString);
			}
			else
			{
				int num = 0;
				bool flag = true;
				TextAlignments textAlignmentValue = this.GetTextAlignmentValue(paragraph, ref flag);
				if (flag)
				{
					num = 2;
				}
				flag = true;
				string family = (string)textRun.GetRichTextStyleValue(StyleAttributeNames.FontFamily, ref flag);
				ReportSize size = (ReportSize)textRun.GetRichTextStyleValue(StyleAttributeNames.FontSize, ref flag);
				FontStyles style = (FontStyles)textRun.GetRichTextStyleValue(StyleAttributeNames.FontStyle, ref flag);
				FontWeights weight = (FontWeights)textRun.GetRichTextStyleValue(StyleAttributeNames.FontWeight, ref flag);
				TextDecorations decoration = (TextDecorations)textRun.GetRichTextStyleValue(StyleAttributeNames.TextDecoration, ref flag);
				if (flag)
				{
					num |= 1;
				}
				flag = true;
				WritingModes writingMode = (WritingModes)base.GetRichTextStyleValue(StyleAttributeNames.WritingMode, null, ref flag, pageContext.VersionPicker);
				if (flag)
				{
					num |= 8;
				}
				bool flag2 = flag;
				bool flag3 = flag;
				flag = true;
				VerticalAlignments verticalAlignment = (VerticalAlignments)base.GetRichTextStyleValue(StyleAttributeNames.VerticalAlign, null, ref flag);
				if (!flag)
				{
					flag2 = false;
				}
				flag = true;
				Directions direction = (Directions)base.GetRichTextStyleValue(StyleAttributeNames.Direction, null, ref flag);
				if (!flag)
				{
					flag3 = false;
				}
				if (flag3)
				{
					num |= 0x10;
				}
				if (flag2)
				{
					num |= 4;
				}
				canvasFont = new CanvasFont(family, size, style, weight, decoration, textAlignmentValue, verticalAlignment, direction, writingMode);
				if (num > 0)
				{
					if (textBoxSharedInfo == null)
					{
						if (pageContext.TextBoxSharedInfo == null)
						{
							pageContext.TextBoxSharedInfo = new Hashtable();
						}
						pageContext.TextBoxSharedInfo.Add(base.m_source.ID, new TextBoxSharedInfo(canvasFont, num));
					}
					else
					{
						if (textBoxSharedInfo.SharedFont != null)
						{
							textBoxSharedInfo.SharedFont.Dispose();
							textBoxSharedInfo.SharedFont = null;
						}
						textBoxSharedInfo.SharedFont = canvasFont;
						textBoxSharedInfo.SharedState = num;
					}
					newFont = false;
					newFormatString = false;
				}
				else
				{
					newFont = true;
					newFormatString = true;
				}
			}
			return canvasFont;
		}

		private CanvasFont CreatePartialSharedFont(CanvasFont shareFont, int fontSharedState, Paragraph paragraph, TextRun textRun, PageContext pageContext, out bool newFont, out bool newFormatString)
		{
			newFont = false;
			newFormatString = false;
			if (fontSharedState == 31)
			{
				return shareFont;
			}
			CanvasFont canvasFont = new CanvasFont(shareFont);
			bool flag = true;
			bool setWritingMode = false;
			if ((fontSharedState & 8) == 0)
			{
				setWritingMode = true;
				WritingModes writingMode = (WritingModes)base.GetRichTextStyleValue(StyleAttributeNames.WritingMode, null, pageContext.VersionPicker);
				canvasFont.SetWritingMode(writingMode);
			}
			if ((fontSharedState & 1) == 0)
			{
				canvasFont.CreateGDIFont((string)textRun.GetRichTextStyleValue(StyleAttributeNames.FontFamily), (ReportSize)textRun.GetRichTextStyleValue(StyleAttributeNames.FontSize), (FontStyles)textRun.GetRichTextStyleValue(StyleAttributeNames.FontStyle), (FontWeights)textRun.GetRichTextStyleValue(StyleAttributeNames.FontWeight), (TextDecorations)textRun.GetRichTextStyleValue(StyleAttributeNames.TextDecoration));
				newFont = true;
			}
			if ((fontSharedState & 2) == 0)
			{
				TextAlignments alignment = (TextAlignments)paragraph.GetRichTextStyleValue(StyleAttributeNames.TextAlign);
				canvasFont.SetTextStringAlignment(alignment, flag);
				flag = false;
			}
			if ((fontSharedState & 4) == 0)
			{
				VerticalAlignments verticalAlignment = (VerticalAlignments)base.GetRichTextStyleValue(StyleAttributeNames.VerticalAlign, null);
				canvasFont.SetLineStringAlignment(verticalAlignment, flag);
				flag = false;
			}
			if ((fontSharedState & 0x10) == 0)
			{
				Directions direction = (Directions)base.GetRichTextStyleValue(StyleAttributeNames.Direction, null);
				canvasFont.SetFormatFlags(direction, setWritingMode, flag);
				flag = false;
			}
			newFormatString = !flag;
			return canvasFont;
		}

		internal static void AddNewLinesAtGdiLimits(ref string text)
		{
			if (text != null && text.Length > 32000)
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				int num = 0;
				int num2 = text.IndexOf('\n', num);
				while (num2 >= 0)
				{
					flag = true;
					string text2 = text.Substring(num, num2 - num);
					if (text2.Length > 32000)
					{
						TextBox.InsertNewLines(stringBuilder, text2);
					}
					else
					{
						stringBuilder.Append(text2);
					}
					stringBuilder.Append('\n');
					num = num2 + 1;
					num2 = -1;
					if (num < text.Length)
					{
						num2 = text.IndexOf('\n', num);
					}
				}
				if (num < text.Length)
				{
					string text3 = text;
					if (flag)
					{
						text3 = text.Substring(num, text.Length - num);
					}
					if (text3.Length > 32000)
					{
						TextBox.InsertNewLines(stringBuilder, text3);
					}
					else
					{
						stringBuilder.Append(text3);
					}
				}
				text = stringBuilder.ToString();
			}
		}

		private static void InsertNewLines(StringBuilder text, string value)
		{
			if (text != null && !string.IsNullOrEmpty(value))
			{
				int num = 0;
				int num2 = 0;
				while (num < value.Length)
				{
					num2 = value.Length - num;
					if (num2 > 32000)
					{
						num2 = 32000;
						char c = value[num + num2 - 1];
						if (char.IsHighSurrogate(c))
						{
							num2--;
						}
						text.Append(value, num, num2);
						text.AppendLine();
						num += num2;
					}
					else
					{
						text.Append(value, num, num2);
						num += num2;
					}
				}
			}
		}
	}
}
