using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Drawing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal abstract class DynamicImage : PageItem
	{
		protected abstract PaginationInfoItems PaginationInfoEnum
		{
			get;
		}

		protected abstract bool SpecialBorderHandling
		{
			get;
		}

		internal DynamicImage(ReportItem source, PageContext pageContext, bool createForRepeat)
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
			if (!pageContext.IgnorePageBreaks)
			{
				if (base.PageBreakAtEnd)
				{
					base.m_itemState = State.OnPagePBEnd;
					pageContext.RegisterPageBreak(new PageBreakInfo(this.PageBreak, base.ItemName));
				}
				if (!flag)
				{
					pageContext.RegisterPageName(this.PageName);
				}
			}
			if (pageContext.TracingEnabled && pageContext.IgnorePageBreaks)
			{
				base.TracePageBreakAtEndIgnored(pageContext);
			}
			if (rplWriter != null)
			{
				if (base.m_itemRenderSizes == null)
				{
					this.CreateItemRenderSizes(contentSize, pageContext, false);
				}
				if (!flag)
				{
					this.WriteItemToStream(rplWriter, pageContext);
				}
			}
			return true;
		}

		internal Stream LoadDynamicImage(PageContext pageContext, ref string streamName, out ActionInfoWithDynamicImageMapCollection actionImageMaps, out System.Drawing.Rectangle offsets)
		{
			IDynamicImageInstance dynamicImageInstance = (IDynamicImageInstance)base.m_source.Instance;
			Stream result = null;
			if (pageContext.EmfDynamicImage)
			{
				dynamicImageInstance.SetDpi(96, 96);
				result = dynamicImageInstance.GetImage(DynamicImageInstance.ImageType.EMF, out actionImageMaps);
				this.Register(ref result, ref streamName, "emf", "image/emf", pageContext, out offsets);
			}
			else
			{
				result = dynamicImageInstance.GetImage(DynamicImageInstance.ImageType.PNG, out actionImageMaps);
				this.Register(ref result, ref streamName, "png", PageContext.PNG_MIME_TYPE, pageContext, out offsets);
			}
			return result;
		}

		private void Register(ref Stream dynamicImageStream, ref string streamName, string extension, string mimeType, PageContext pageContext, out System.Drawing.Rectangle offsets)
		{
			offsets = System.Drawing.Rectangle.Empty;
			if (dynamicImageStream != null && dynamicImageStream.Length != 0)
			{
				if (pageContext.AddSecondaryStreamNames)
				{
					streamName = this.GenerateStreamName(pageContext);
				}
				if (pageContext.SecondaryStreams != 0)
				{
					string text = streamName;
					if (text == null)
					{
						text = this.GenerateStreamName(pageContext);
					}
					this.RegisterDynamicImage(ref dynamicImageStream, ref text, extension, mimeType, pageContext, out offsets);
					streamName = text;
				}
			}
		}

		internal void WriteItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write(this.GetElementToken(pageContext));
				this.WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
				base.m_offset = baseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write((byte)255);
			}
			else
			{
				base.m_rplElement = this.CreateRPLItem();
				this.WriteElementProps(base.m_rplElement.ElementProps, rplWriter, pageContext);
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)this.PaginationInfoEnum);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)255);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper((byte)this.PaginationInfoEnum);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}

		protected void RegisterDynamicImage(ref Stream dynamicImageStream, ref string streamName, string extension, string mimeType, PageContext pageContext, out System.Drawing.Rectangle offsets)
		{
			offsets = System.Drawing.Rectangle.Empty;
			if (dynamicImageStream != null && dynamicImageStream.Length != 0)
			{
				if (pageContext.ImageConsolidation != null)
				{
					ImageConsolidation imageConsolidation = pageContext.ImageConsolidation;
					offsets = imageConsolidation.AppendImage(dynamicImageStream);
					if (offsets != System.Drawing.Rectangle.Empty)
					{
						dynamicImageStream = null;
						streamName = imageConsolidation.GetStreamName();
					}
				}
				else if (!pageContext.RegisteredStreamNames.Contains(streamName))
				{
					pageContext.RegisteredStreamNames.Add(streamName, null);
					Stream stream = pageContext.CreateAndRegisterStream(streamName, extension, null, mimeType, false, StreamOper.CreateAndRegister);
					if (stream != null)
					{
						dynamicImageStream.Position = 0L;
						int num = (int)dynamicImageStream.Length;
						byte[] array = new byte[4096];
						while (num > 0)
						{
							int num2 = dynamicImageStream.Read(array, 0, Math.Min(array.Length, num));
							stream.Write(array, 0, num2);
							num -= num2;
						}
						dynamicImageStream = null;
					}
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			ActionInfoWithDynamicImageMapCollection actionInfoWithDynamicImageMapCollection = null;
			string text = null;
			System.Drawing.Rectangle empty = System.Drawing.Rectangle.Empty;
			Stream stream = this.LoadDynamicImage(pageContext, ref text, out actionInfoWithDynamicImageMapCollection, out empty);
			if (text != null)
			{
				spbifWriter.Write((byte)40);
				spbifWriter.Write(text);
			}
			if (!empty.IsEmpty)
			{
				if (pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
				{
					spbifWriter.Write((byte)47);
				}
				else
				{
					spbifWriter.Write((byte)49);
				}
				spbifWriter.Write(empty.Left);
				spbifWriter.Write(empty.Top);
				spbifWriter.Write(empty.Width);
				spbifWriter.Write(empty.Height);
			}
			if (stream != null)
			{
				spbifWriter.Write((byte)39);
				spbifWriter.Write((int)stream.Length);
				byte[] array = new byte[4096];
				stream.Position = 0L;
				for (int num = stream.Read(array, 0, array.Length); num != 0; num = stream.Read(array, 0, array.Length))
				{
					spbifWriter.Write(array, 0, num);
				}
			}
			if (actionInfoWithDynamicImageMapCollection != null)
			{
				base.WriteImageMapAreaInstanceCollection(actionInfoWithDynamicImageMapCollection, spbifWriter, pageContext);
			}
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			ActionInfoWithDynamicImageMapCollection actionInfoWithDynamicImageMapCollection = null;
			RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)nonSharedProps;
			string streamName = null;
			System.Drawing.Rectangle empty = System.Drawing.Rectangle.Empty;
			rPLDynamicImageProps.DynamicImageContent = this.LoadDynamicImage(pageContext, ref streamName, out actionInfoWithDynamicImageMapCollection, out empty);
			rPLDynamicImageProps.ImageConsolidationOffsets = empty;
			rPLDynamicImageProps.StreamName = streamName;
			if (actionInfoWithDynamicImageMapCollection != null)
			{
				rPLDynamicImageProps.ActionImageMapAreas = base.WriteImageMapAreaInstanceCollection(actionInfoWithDynamicImageMapCollection, pageContext);
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps styleProps, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, styleProps, StyleAttributeNames.BackgroundColor, 34);
		}

		internal override void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (!this.SpecialBorderHandling)
			{
				base.WriteNonSharedStyleProp(spbifWriter, styleDef, style, styleAttribute, pageContext);
			}
			else
			{
				this.WriteItemNonSharedStyleProp(spbifWriter, styleDef, style, styleAttribute, pageContext);
			}
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (styleAttribute == StyleAttributeNames.BackgroundColor)
			{
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			}
		}

		internal override void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (!this.SpecialBorderHandling)
			{
				base.WriteNonSharedStyleProp(rplStyleProps, styleDef, style, styleAttribute, pageContext);
			}
			else
			{
				this.WriteItemNonSharedStyleProp(rplStyleProps, styleDef, style, styleAttribute, pageContext);
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			if (styleAttribute == StyleAttributeNames.BackgroundColor)
			{
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			}
		}

		internal override void WriteBorderProps(BinaryWriter spbifWriter, Style style)
		{
			if (!this.SpecialBorderHandling)
			{
				base.WriteBorderProps(spbifWriter, style);
			}
		}

		internal override void WriteBorderProps(RPLStyleProps rplStyleProps, Style style)
		{
			if (!this.SpecialBorderHandling)
			{
				base.WriteBorderProps(rplStyleProps, style);
			}
		}

		protected abstract string GenerateStreamName(PageContext pageContext);

		protected abstract RPLItem CreateRPLItem();

		protected abstract byte GetElementToken(PageContext pageContext);
	}
}
