using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class ReportBody : PageItemContainer
	{
		private new Body m_source;

		internal override string SourceUniqueName
		{
			get
			{
				return this.m_source.InstanceUniqueName;
			}
		}

		internal override string SourceID
		{
			get
			{
				return this.m_source.ID;
			}
		}

		internal override ReportElement OriginalSource
		{
			get
			{
				return this.m_source;
			}
		}

		internal ReportBody(Body source, ReportSize width, PageContext pageContext)
			: base(null, false)
		{
			if (pageContext != null)
			{
				base.m_itemPageSizes = pageContext.GetSharedItemSizesElement(width, source.Height, source.ID, true);
			}
			else
			{
				base.m_itemPageSizes = new PaddItemSizes(width, source.Height, source.ID);
			}
			this.m_source = source;
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			base.AdjustOriginFromItemsAbove(siblings, repeatWithItems);
			if (!this.HitsCurrentPage(pageContext, parentTopInPage))
			{
				return false;
			}
			this.WriteStartItemToStream(rplWriter, pageContext);
			PageItemHelper[] array = null;
			if (base.m_itemsCreated)
			{
				PageItemContainerHelper pageItemContainerHelper = lastPageInfo as PageItemContainerHelper;
				RSTrace.RenderingTracer.Assert(pageItemContainerHelper != null || lastPageInfo == null, "This should be a container");
				base.CreateChildrenFromPaginationState(this.m_source.ReportItemCollection, pageContext, pageItemContainerHelper, false);
				base.ResolveRepeatWithFromPaginationState(pageItemContainerHelper, rplWriter, pageContext);
				base.UpdateItemPageState(pageContext, false);
				if (pageItemContainerHelper != null)
				{
					array = pageItemContainerHelper.Children;
				}
			}
			else
			{
				base.CreateChildren(this.m_source.ReportItemCollection, pageContext, base.m_itemPageSizes.Width, base.m_itemPageSizes.Height);
				base.ResolveRepeatWith(this.m_source.ReportItemCollection, pageContext);
				base.m_itemsCreated = true;
			}
			double num = parentTopInPage + base.m_itemPageSizes.Top;
			double num2 = 0.0;
			int num3 = 0;
			PageItem[] childrenOnPage = null;
			bool flag = true;
			bool flag2 = true;
			ProcessPageBreaks processPageBreaks = null;
			List<int> repeatedSiblings = null;
			double num4 = 0.0;
			if (base.m_children != null)
			{
				double num5 = base.m_itemPageSizes.PaddingBottom;
				PageItem pageItem = null;
				processPageBreaks = new ProcessPageBreaks();
				for (int i = 0; i < base.m_children.Length; i++)
				{
					pageItem = base.m_children[i];
					if (pageItem != null)
					{
						num4 = pageItem.ReserveSpaceForRepeatWith(base.m_repeatWithItems, pageContext);
						if (array != null)
						{
							pageItem.CalculatePage(rplWriter, array[i], pageContext, base.m_children, base.m_repeatWithItems, num + num4, ref num2, interactivity);
						}
						else
						{
							pageItem.CalculatePage(rplWriter, null, pageContext, base.m_children, base.m_repeatWithItems, num + num4, ref num2, interactivity);
						}
						if (!pageContext.FullOnPage)
						{
							processPageBreaks.ProcessItemPageBreaks(pageItem);
							if (pageItem.ItemState != State.OnPage && pageItem.ItemState != State.OnPageHidden)
							{
								if (pageItem.ItemState != State.OnPagePBEnd)
								{
									flag = false;
								}
								if (pageItem.ItemState != State.Below)
								{
									flag2 = false;
								}
							}
							else
							{
								base.m_prevPageEnd = num2;
								flag2 = false;
							}
							if (rplWriter != null)
							{
								pageItem.MergeRepeatSiblings(ref repeatedSiblings);
							}
						}
						num5 = Math.Max(num5, pageItem.ItemPageSizes.Bottom + base.m_itemPageSizes.PaddingBottom);
					}
				}
				base.ConsumeWhitespaceVertical(base.m_itemPageSizes, num5, pageContext);
			}
			if (pageContext.CancelPage)
			{
				base.m_itemState = State.Below;
				base.m_children = null;
				base.m_rplElement = null;
				return false;
			}
			bool flag3 = false;
			if (processPageBreaks != null && processPageBreaks.HasPageBreaks(ref base.m_prevPageEnd, ref num2))
			{
				if (flag)
				{
					if (num2 - base.m_itemPageSizes.Height != 0.0)
					{
						flag = false;
					}
					else
					{
						flag3 = true;
					}
				}
			}
			else if (!pageContext.FullOnPage)
			{
				if (flag)
				{
					double num6 = num + base.m_itemPageSizes.Height;
					if ((RoundedDouble)num6 > pageContext.PageHeight && (RoundedDouble)(num6 - base.m_itemPageSizes.PaddingBottom) <= pageContext.PageHeight)
					{
						double val = pageContext.PageHeight - num;
						base.m_prevPageEnd = Math.Max(num2, val);
						num2 = base.m_prevPageEnd;
						flag = false;
					}
					else
					{
						num2 = base.m_itemPageSizes.Height;
					}
				}
				else if (flag2)
				{
					double num7 = num + base.m_itemPageSizes.Height;
					if ((RoundedDouble)num7 > pageContext.PageHeight)
					{
						base.m_prevPageEnd = pageContext.PageHeight - num;
						num2 = base.m_prevPageEnd;
					}
				}
			}
			else
			{
				num2 = base.m_itemPageSizes.Height;
			}
			if (pageContext.FullOnPage || flag)
			{
				base.m_itemState = State.OnPage;
				if (flag && flag3)
				{
					base.m_itemState = State.OnPagePBEnd;
				}
				parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Top + num2);
				if (rplWriter != null)
				{
					this.CreateItemRenderSizes(null, pageContext, false);
					num3 = base.CalculateRenderSizes(rplWriter, pageContext, interactivity, repeatedSiblings, out childrenOnPage);
					this.WriteEndItemToStream(rplWriter, num3, childrenOnPage);
				}
				base.m_indexesLeftToRight = null;
				base.m_children = null;
			}
			else
			{
				base.m_itemState = State.SpanPages;
				parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Top + num2);
				if (rplWriter != null)
				{
					this.CreateItemRenderSizes(null, pageContext, false);
					base.m_itemRenderSizes.PaddingBottom = 0.0;
					base.m_itemRenderSizes.AdjustHeightTo(num2);
					num3 = base.CalculateRenderSizes(rplWriter, pageContext, interactivity, repeatedSiblings, out childrenOnPage);
					this.WriteEndItemToStream(rplWriter, num3, childrenOnPage);
				}
				else
				{
					base.ReleaseChildrenOnPage();
				}
			}
			return true;
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					base.m_offset = baseStream.Position;
					if (pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
					{
						binaryWriter.Write((byte)10);
					}
					else
					{
						binaryWriter.Write((byte)6);
					}
					binaryWriter.Write((byte)15);
					binaryWriter.Write((byte)0);
					binaryWriter.Write((byte)1);
					binaryWriter.Write(this.SourceID);
					Style style = this.m_source.Style;
					if (style != null)
					{
						this.WriteSharedStyle(binaryWriter, style, pageContext, 6);
						binaryWriter.Write((byte)255);
						binaryWriter.Write((byte)1);
						binaryWriter.Write((byte)0);
						binaryWriter.Write(this.SourceUniqueName);
						StyleInstance styleInstance = base.GetStyleInstance(this.m_source, null);
						if (styleInstance != null)
						{
							this.WriteNonSharedStyle(binaryWriter, style, styleInstance, pageContext, 6, null);
						}
					}
					else
					{
						binaryWriter.Write((byte)255);
						binaryWriter.Write((byte)1);
						binaryWriter.Write((byte)0);
						binaryWriter.Write(this.SourceUniqueName);
					}
					binaryWriter.Write((byte)255);
					binaryWriter.Write((byte)255);
				}
				else
				{
					base.m_rplElement = new RPLBody();
					base.m_rplElement.ElementProps.Definition.ID = this.SourceID;
					base.m_rplElement.ElementProps.UniqueName = this.SourceUniqueName;
					Style style2 = this.m_source.Style;
					if (style2 != null)
					{
						base.m_rplElement.ElementProps.Definition.SharedStyle = this.WriteSharedStyle(style2, pageContext);
						StyleInstance styleInstance2 = base.GetStyleInstance(this.m_source, null);
						if (styleInstance2 != null)
						{
							base.m_rplElement.ElementProps.NonSharedStyle = this.WriteNonSharedStyle(style2, styleInstance2, pageContext, null);
						}
					}
				}
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			this.WriteBackgroundImage(spbifWriter, style, true, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			this.WriteBackgroundImage(rplStyleProps, style, true, pageContext);
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
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)7);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)255);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemContainerHelper(7);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}
	}
}
