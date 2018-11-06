using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class TextRun : PageElement, ITextRunProps
	{
		private long m_offset;

		private RPLTextRun m_rplElement;

		private CompiledTextRunInstance m_compiledSource;

		private string m_fontKey;

		public override long Offset
		{
			get
			{
				return this.m_offset;
			}
			set
			{
				this.m_offset = value;
			}
		}

		internal override string SourceID
		{
			get
			{
				return base.m_source.ID;
			}
		}

		internal override string SourceUniqueName
		{
			get
			{
				return base.m_source.InstanceUniqueName;
			}
		}

		internal override bool HasBackground
		{
			get
			{
				return false;
			}
		}

		internal RPLTextRun RPLElement
		{
			get
			{
				return this.m_rplElement;
			}
		}

		internal CompiledTextRunInstance CompiledInstance
		{
			get
			{
				return this.m_compiledSource;
			}
		}

		public string FontFamily
		{
			get
			{
				string text = (string)base.GetRichTextStyleValue(StyleAttributeNames.FontFamily, this.m_compiledSource);
				if (string.IsNullOrEmpty(text))
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
				double num = 12.0;
				ReportSize reportSize = (ReportSize)base.GetRichTextStyleValue(StyleAttributeNames.FontSize, this.m_compiledSource);
				if (reportSize != null)
				{
					num = reportSize.ToPoints();
				}
				return (float)num;
			}
		}

		public Color Color
		{
			get
			{
				Color result = Color.Black;
				ReportColor reportColor = (ReportColor)base.GetRichTextStyleValue(StyleAttributeNames.Color, this.m_compiledSource);
				if (reportColor != null)
				{
					result = reportColor.ToColor();
				}
				return result;
			}
		}

		public bool Bold
		{
			get
			{
				FontWeights aValue = (FontWeights)base.GetRichTextStyleValue(StyleAttributeNames.FontWeight, this.m_compiledSource);
				return this.IsBold((RPLFormat.FontWeights)StyleEnumConverter.Translate(aValue));
			}
		}

		public bool Italic
		{
			get
			{
				FontStyles aValue = (FontStyles)base.GetRichTextStyleValue(StyleAttributeNames.FontStyle, this.m_compiledSource);
				return this.IsItalic((RPLFormat.FontStyles)StyleEnumConverter.Translate(aValue));
			}
		}

		public RPLFormat.TextDecorations TextDecoration
		{
			get
			{
				TextDecorations aValue = (TextDecorations)base.GetRichTextStyleValue(StyleAttributeNames.TextDecoration, this.m_compiledSource);
				return (RPLFormat.TextDecorations)StyleEnumConverter.Translate(aValue);
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

		private TextRun()
			: base(null)
		{
		}

		internal TextRun(AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun, PageContext pageContext)
			: this(textRun, null, pageContext)
		{
		}

		internal TextRun(AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun, CompiledTextRunInstance compiledTextRun, PageContext pageContext)
			: base(textRun)
		{
			this.m_compiledSource = compiledTextRun;
		}

		public void AddSplitIndex(int index)
		{
		}

		internal object GetRichTextStyleValue(StyleAttributeNames styleName, ref bool isShared)
		{
			return base.GetRichTextStyleValue(styleName, (ReportElementInstance)this.m_compiledSource, ref isShared);
		}

		internal object GetRichTextStyleValue(StyleAttributeNames styleName)
		{
			return base.GetRichTextStyleValue(styleName, this.m_compiledSource);
		}

		internal string ComputeValue()
		{
			if (this.m_compiledSource == null)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextRun;
				if (textRun.FormattedValueExpressionBased)
				{
					return textRun.Instance.Value;
				}
				return textRun.Value.Value;
			}
			return this.m_compiledSource.Value;
		}

		internal override void WriteSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext, long offset)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextRun;
			RSTrace.RenderingTracer.Assert(textRun != null, "The text run definition cannot be null");
			Hashtable hashtable = pageContext.ItemPropsStart;
			if (hashtable != null)
			{
				object obj = hashtable[this.SourceID];
				if (obj != null)
				{
					spbifWriter.Write((byte)2);
					spbifWriter.Write((long)obj);
					return;
				}
			}
			if (hashtable == null)
			{
				hashtable = (pageContext.ItemPropsStart = new Hashtable());
			}
			hashtable.Add(this.SourceID, offset);
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)5);
			spbifWriter.Write(this.SourceID);
			if (!textRun.MarkupType.IsExpression)
			{
				spbifWriter.Write((byte)7);
				spbifWriter.Write(StyleEnumConverter.Translate(textRun.MarkupType.Value));
			}
			if (textRun.Label != null)
			{
				spbifWriter.Write((byte)8);
				spbifWriter.Write(textRun.Label);
			}
			if (textRun.ToolTip != null && !textRun.ToolTip.IsExpression && textRun.ToolTip.Value != null)
			{
				spbifWriter.Write((byte)9);
				spbifWriter.Write(textRun.ToolTip.Value);
			}
			if (textRun.Value.IsExpression)
			{
				spbifWriter.Write((byte)12);
				spbifWriter.Write(textRun.Value.ExpressionString);
			}
			if (!pageContext.HideDuplicates && !textRun.FormattedValueExpressionBased && textRun.Value.Value != null)
			{
				spbifWriter.Write((byte)10);
				if (textRun.SharedTypeCode == TypeCode.String)
				{
					spbifWriter.Write(textRun.Value.Value);
				}
				else
				{
					spbifWriter.Write(textRun.Instance.Value);
				}
			}
			this.WriteSharedStyle(spbifWriter, null, pageContext, 6);
			spbifWriter.Write((byte)255);
		}

		internal override void WriteSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextRun;
			RSTrace.RenderingTracer.Assert(textRun != null, "The text run definition cannot be null");
			Hashtable hashtable = pageContext.ItemPropsStart;
			if (hashtable != null)
			{
				object obj = hashtable[this.SourceID];
				if (obj != null)
				{
					elemProps.Definition = (RPLTextRunPropsDef)obj;
					return;
				}
			}
			RPLTextRunProps rPLTextRunProps = elemProps as RPLTextRunProps;
			RPLTextRunPropsDef rPLTextRunPropsDef = rPLTextRunProps.Definition as RPLTextRunPropsDef;
			if (hashtable == null)
			{
				hashtable = (pageContext.ItemPropsStart = new Hashtable());
			}
			hashtable.Add(this.SourceID, rPLTextRunPropsDef);
			rPLTextRunPropsDef.ID = this.SourceID;
			if (!textRun.MarkupType.IsExpression)
			{
				rPLTextRunPropsDef.Markup = (RPLFormat.MarkupStyles)StyleEnumConverter.Translate(textRun.MarkupType.Value);
			}
			if (textRun.Label != null)
			{
				rPLTextRunPropsDef.Label = textRun.Label;
			}
			if (textRun.ToolTip != null && !textRun.ToolTip.IsExpression && textRun.ToolTip.Value != null)
			{
				rPLTextRunPropsDef.ToolTip = textRun.ToolTip.Value;
			}
			if (textRun.Value.IsExpression)
			{
				rPLTextRunPropsDef.Formula = textRun.Value.ExpressionString;
			}
			if (!pageContext.HideDuplicates && !textRun.FormattedValueExpressionBased && textRun.Value.Value != null)
			{
				if (textRun.SharedTypeCode == TypeCode.String)
				{
					rPLTextRunPropsDef.Value = textRun.Value.Value;
				}
				else
				{
					rPLTextRunPropsDef.Value = textRun.Instance.Value;
				}
			}
			rPLTextRunPropsDef.SharedStyle = this.WriteSharedStyle(null, pageContext);
		}

		internal override void WriteNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextRun;
			RSTrace.RenderingTracer.Assert(textRun != null, "The text run definition cannot be null");
			TextRunInstance textRunInstance = null;
			bool flag = false;
			if (this.m_compiledSource != null)
			{
				textRunInstance = this.m_compiledSource;
				flag = true;
			}
			else
			{
				textRunInstance = textRun.Instance;
				RSTrace.RenderingTracer.Assert(textRunInstance != null, "The text run instance cannot be null");
			}
			spbifWriter.Write((byte)1);
			spbifWriter.Write((byte)4);
			spbifWriter.Write(textRunInstance.UniqueName);
			if (!flag)
			{
				if (textRunInstance.ProcessedWithError)
				{
					spbifWriter.Write((byte)13);
					spbifWriter.Write(true);
				}
				if (textRun.MarkupType.IsExpression)
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write(StyleEnumConverter.Translate(textRunInstance.MarkupType));
				}
				if (textRun.ToolTip != null && textRun.ToolTip.IsExpression && textRunInstance.ToolTip != null)
				{
					spbifWriter.Write((byte)9);
					spbifWriter.Write(textRunInstance.ToolTip);
				}
				base.WriteActionInfo(textRun.ActionInfo, spbifWriter, pageContext, 11);
			}
			else
			{
				spbifWriter.Write((byte)7);
				spbifWriter.Write(StyleEnumConverter.Translate(textRunInstance.MarkupType));
				if (textRunInstance.ToolTip != null)
				{
					spbifWriter.Write((byte)9);
					spbifWriter.Write(textRunInstance.ToolTip);
				}
				if (this.m_compiledSource.ActionInstance != null)
				{
					this.WriteActionInstance(this.m_compiledSource.ActionInstance, spbifWriter, pageContext);
				}
			}
			if ((pageContext.HideDuplicates || textRun.FormattedValueExpressionBased) && textRunInstance.Value != null)
			{
				spbifWriter.Write((byte)10);
				spbifWriter.Write(textRunInstance.Value);
			}
			pageContext.HideDuplicates = false;
			pageContext.TypeCodeNonString = false;
			this.WriteNonSharedStyle(spbifWriter, null, null, pageContext, 6, this.m_compiledSource);
			spbifWriter.Write((byte)255);
		}

		internal override void WriteNonSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextRun;
			RSTrace.RenderingTracer.Assert(textRun != null, "The text run definition cannot be null");
			TextRunInstance textRunInstance = null;
			bool flag = false;
			if (this.m_compiledSource != null)
			{
				textRunInstance = this.m_compiledSource;
				flag = true;
			}
			else
			{
				textRunInstance = textRun.Instance;
				RSTrace.RenderingTracer.Assert(textRunInstance != null, "The text run instance cannot be null");
			}
			elemProps.UniqueName = textRunInstance.UniqueName;
			RPLTextRunProps rPLTextRunProps = elemProps as RPLTextRunProps;
			if (!flag)
			{
				rPLTextRunProps.ProcessedWithError = textRunInstance.ProcessedWithError;
				if (textRun.MarkupType.IsExpression)
				{
					rPLTextRunProps.Markup = (RPLFormat.MarkupStyles)StyleEnumConverter.Translate(textRunInstance.MarkupType);
				}
				if (textRun.ToolTip != null && textRun.ToolTip.IsExpression && textRunInstance.ToolTip != null)
				{
					rPLTextRunProps.ToolTip = textRunInstance.ToolTip;
				}
				rPLTextRunProps.ActionInfo = base.WriteActionInfo(textRun.ActionInfo, pageContext);
			}
			else
			{
				rPLTextRunProps.Markup = (RPLFormat.MarkupStyles)StyleEnumConverter.Translate(textRunInstance.MarkupType);
				if (textRunInstance.ToolTip != null)
				{
					rPLTextRunProps.ToolTip = textRunInstance.ToolTip;
				}
				if (this.m_compiledSource.ActionInstance != null)
				{
					rPLTextRunProps.ActionInfo = this.WriteActionInstance(this.m_compiledSource.ActionInstance, pageContext);
				}
			}
			if ((pageContext.HideDuplicates || textRun.FormattedValueExpressionBased) && textRunInstance.Value != null)
			{
				rPLTextRunProps.Value = textRunInstance.Value;
			}
			pageContext.HideDuplicates = false;
			pageContext.TypeCodeNonString = false;
			rPLTextRunProps.NonSharedStyle = this.WriteNonSharedStyle(null, null, pageContext, this.m_compiledSource);
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontStyle, 19);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontFamily, 20);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontSize, 21);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontWeight, 22);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Format, 23);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Color, 27);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Language, 32);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.Calendar, 38);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.NumeralLanguage, 36);
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.NumeralVariant, 37);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontSize, 21);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Format, 23);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Color, 27);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Language, 32);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Calendar, 38);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.NumeralLanguage, 36);
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.NumeralVariant, 37);
		}

		internal override void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.TextAlign:
			case StyleAttributeNames.VerticalAlign:
			case StyleAttributeNames.PaddingLeft:
			case StyleAttributeNames.PaddingRight:
			case StyleAttributeNames.PaddingTop:
			case StyleAttributeNames.PaddingBottom:
			case StyleAttributeNames.LineHeight:
			case StyleAttributeNames.Direction:
			case StyleAttributeNames.WritingMode:
			case StyleAttributeNames.UnicodeBiDi:
				break;
			case StyleAttributeNames.FontStyle:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontStyle, 19);
				break;
			case StyleAttributeNames.FontFamily:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontFamily, 20);
				break;
			case StyleAttributeNames.FontSize:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontSize, 21);
				break;
			case StyleAttributeNames.FontWeight:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontWeight, 22);
				break;
			case StyleAttributeNames.Format:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Format, 23);
				break;
			case StyleAttributeNames.TextDecoration:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
				break;
			case StyleAttributeNames.Color:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Color, 27);
				break;
			case StyleAttributeNames.Language:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Language, 32);
				break;
			case StyleAttributeNames.Calendar:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Calendar, 38);
				break;
			case StyleAttributeNames.NumeralLanguage:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.NumeralLanguage, 36);
				break;
			case StyleAttributeNames.NumeralVariant:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.NumeralVariant, 37);
				break;
			}
		}

		internal override void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.TextAlign:
			case StyleAttributeNames.VerticalAlign:
			case StyleAttributeNames.PaddingLeft:
			case StyleAttributeNames.PaddingRight:
			case StyleAttributeNames.PaddingTop:
			case StyleAttributeNames.PaddingBottom:
			case StyleAttributeNames.LineHeight:
			case StyleAttributeNames.Direction:
			case StyleAttributeNames.WritingMode:
			case StyleAttributeNames.UnicodeBiDi:
				break;
			case StyleAttributeNames.FontStyle:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
				break;
			case StyleAttributeNames.FontFamily:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
				break;
			case StyleAttributeNames.FontSize:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontSize, 21);
				break;
			case StyleAttributeNames.FontWeight:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
				break;
			case StyleAttributeNames.Format:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Format, 23);
				break;
			case StyleAttributeNames.TextDecoration:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
				break;
			case StyleAttributeNames.Color:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Color, 27);
				break;
			case StyleAttributeNames.Language:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Language, 32);
				break;
			case StyleAttributeNames.Calendar:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Calendar, 38);
				break;
			case StyleAttributeNames.NumeralLanguage:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.NumeralLanguage, 36);
				break;
			case StyleAttributeNames.NumeralVariant:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.NumeralVariant, 37);
				break;
			}
		}

		internal override void WriteBackgroundImage(BinaryWriter spbifWriter, Style style, bool writeShared, PageContext pageContext)
		{
		}

		internal override void WriteBackgroundImage(RPLStyleProps rplStyleProps, Style style, bool writeShared, PageContext pageContext)
		{
		}

		internal override void WriteBorderProps(BinaryWriter spbifWriter, Style style)
		{
		}

		internal override void WriteBorderProps(RPLStyleProps rplStyleProps, Style style)
		{
		}

		internal void WriteItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				this.m_offset = baseStream.Position;
				binaryWriter.Write((byte)20);
				this.WriteElementProps(binaryWriter, rplWriter, pageContext, this.m_offset + 1);
				binaryWriter.Write((byte)255);
			}
			else
			{
				this.m_rplElement = new RPLTextRun();
				this.WriteElementProps(this.m_rplElement.ElementProps, rplWriter, pageContext);
			}
		}

		internal AspNetCore.ReportingServices.Rendering.RichText.TextRun GetRichTextRun()
		{
			string text = null;
			AspNetCore.ReportingServices.OnDemandReportRendering.TextRun textRun = base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.TextRun;
			RSTrace.RenderingTracer.Assert(textRun != null, "The text run definition cannot be null");
			TextRunInstance textRunInstance = null;
			textRunInstance = ((this.m_compiledSource != null) ? this.m_compiledSource : textRun.Instance);
			text = textRunInstance.Value;
			if (string.IsNullOrEmpty(text) && textRun.Value != null)
			{
				text = textRun.Value.Value;
			}
			return new AspNetCore.ReportingServices.Rendering.RichText.TextRun(text, this);
		}

		private void WriteActionInstance(ActionInstance actionInst, BinaryWriter spbifWriter, PageContext pageContext)
		{
			if (actionInst != null)
			{
				spbifWriter.Write((byte)11);
				spbifWriter.Write((byte)2);
				spbifWriter.Write(1);
				spbifWriter.Write((byte)3);
				if (actionInst.Label != null)
				{
					spbifWriter.Write((byte)4);
					spbifWriter.Write(actionInst.Label);
				}
				if (actionInst.Hyperlink != null)
				{
					ReportUrl hyperlink = actionInst.Hyperlink;
					if (hyperlink != null)
					{
						Uri uri = hyperlink.ToUri();
						if ((Uri)null != uri)
						{
							spbifWriter.Write((byte)6);
							spbifWriter.Write(uri.AbsoluteUri);
						}
					}
				}
				else if (actionInst.BookmarkLink != null)
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write(actionInst.BookmarkLink);
				}
				spbifWriter.Write((byte)255);
				spbifWriter.Write((byte)255);
			}
		}

		private RPLActionInfo WriteActionInstance(ActionInstance actionInst, PageContext pageContext)
		{
			if (actionInst == null)
			{
				return null;
			}
			RPLActionInfo rPLActionInfo = new RPLActionInfo(1);
			RPLAction rPLAction = null;
			rPLAction = ((actionInst.Label == null) ? new RPLAction() : new RPLAction(actionInst.Label));
			if (actionInst.Hyperlink != null)
			{
				ReportUrl hyperlink = actionInst.Hyperlink;
				if (hyperlink != null)
				{
					Uri uri = hyperlink.ToUri();
					if ((Uri)null != uri)
					{
						rPLAction.Hyperlink = uri.AbsoluteUri;
					}
				}
			}
			else if (actionInst.BookmarkLink != null)
			{
				rPLAction.BookmarkLink = actionInst.BookmarkLink;
			}
			rPLActionInfo.Actions[0] = rPLAction;
			return rPLActionInfo;
		}

		private bool IsBold(RPLFormat.FontWeights fontWeight)
		{
			if (fontWeight != RPLFormat.FontWeights.SemiBold && fontWeight != RPLFormat.FontWeights.Bold && fontWeight != RPLFormat.FontWeights.ExtraBold && fontWeight != RPLFormat.FontWeights.Heavy)
			{
				return false;
			}
			return true;
		}

		private bool IsItalic(RPLFormat.FontStyles fontStyle)
		{
			if (fontStyle == RPLFormat.FontStyles.Italic)
			{
				return true;
			}
			return false;
		}
	}
}
