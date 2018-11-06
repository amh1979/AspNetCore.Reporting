using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Rectangle : PageItemContainer
	{
		private bool m_staticItem;

		protected override PageBreak PageBreak
		{
			get
			{
				return ((AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)base.m_source).PageBreak;
			}
		}

		protected override string PageName
		{
			get
			{
				return ((RectangleInstance)base.m_source.Instance).PageName;
			}
		}

		internal override bool StaticItem
		{
			get
			{
				return this.m_staticItem;
			}
		}

		internal Rectangle(AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle source, PageContext pageContext, bool createForRepeat)
			: base(source, createForRepeat)
		{
			if (pageContext != null)
			{
				if (createForRepeat)
				{
					base.m_itemPageSizes = pageContext.GetSharedFromRepeatItemSizesElement(source, true);
				}
				else
				{
					base.m_itemPageSizes = pageContext.GetSharedItemSizesElement(source, true);
				}
			}
			else
			{
				base.m_itemPageSizes = new PaddItemSizes(source);
			}
		}

		private void CalculateHiddenItemRenderSize(PageContext pageContext, bool createForRepeat)
		{
			if (base.m_itemRenderSizes == null)
			{
				if (pageContext != null)
				{
					if (createForRepeat)
					{
						base.m_itemRenderSizes = pageContext.GetSharedRenderFromRepeatItemSizesElement(base.m_itemPageSizes, true, false);
					}
					else
					{
						base.m_itemRenderSizes = pageContext.GetSharedRenderItemSizesElement(base.m_itemPageSizes, true, false);
					}
				}
				if (base.m_itemRenderSizes == null)
				{
					base.m_itemRenderSizes = new ItemSizes(base.m_itemPageSizes);
				}
			}
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			base.AdjustOriginFromItemsAbove(siblings, repeatWithItems);
			if (!this.HitsCurrentPage(pageContext, parentTopInPage))
			{
				return false;
			}
			ItemSizes itemSizes = null;
			if (!base.m_itemsCreated && base.ResolveItemHiddenState(rplWriter, interactivity, pageContext, false, ref itemSizes))
			{
				parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Bottom);
				if (rplWriter != null)
				{
					this.CalculateHiddenItemRenderSize(pageContext, false);
				}
				return true;
			}
			AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle rectangle = (AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)base.m_source;
			PageItemHelper[] array = null;
			bool flag = false;
			this.WriteStartItemToStream(rplWriter, pageContext);
			bool overrideChild = !pageContext.IsPageBreakRegistered;
			if (base.m_itemsCreated)
			{
				PageItemContainerHelper pageItemContainerHelper = lastPageInfo as PageItemContainerHelper;
				RSTrace.RenderingTracer.Assert(pageItemContainerHelper != null || lastPageInfo == null, "This should be a container");
				this.m_staticItem = base.CreateChildrenFromPaginationState(rectangle.ReportItemCollection, pageContext, pageItemContainerHelper, rectangle.IsSimple);
				base.ResolveRepeatWithFromPaginationState(pageItemContainerHelper, rplWriter, pageContext);
				base.UpdateItemPageState(pageContext, rectangle.OmitBorderOnPageBreak);
				if (pageItemContainerHelper != null)
				{
					array = pageItemContainerHelper.Children;
				}
			}
			else
			{
				flag = true;
				if (!pageContext.IgnorePageBreaks)
				{
					pageContext.RegisterPageName(this.PageName);
				}
				this.m_staticItem = base.CreateChildren(rectangle.ReportItemCollection, pageContext, rectangle.Width.ToMillimeters(), rectangle.Height.ToMillimeters(), rectangle.IsSimple);
				base.ResolveRepeatWith(rectangle.ReportItemCollection, pageContext);
				base.m_itemsCreated = true;
				if (itemSizes != null)
				{
					itemSizes.SetPaddings(base.m_itemPageSizes.PaddingRight, base.m_itemPageSizes.PaddingBottom);
				}
			}
			PageContext pageContext2 = pageContext;
			if (!pageContext2.FullOnPage)
			{
				if (base.IgnorePageBreaks)
				{
					pageContext2 = new PageContext(pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.Toggled);
				}
				else if (flag && rectangle.KeepTogether && !pageContext2.KeepTogether)
				{
					pageContext2 = new PageContext(pageContext);
					pageContext2.KeepTogether = true;
					if (pageContext.TracingEnabled && parentTopInPage + base.m_itemPageSizes.Height >= pageContext2.OriginalPageHeight)
					{
						base.TracePageGrownOnKeepTogetherItem(pageContext.PageNumber);
					}
				}
			}
			double num = parentTopInPage + base.m_itemPageSizes.Top;
			double num2 = 0.0;
			int num3 = 0;
			PageItem[] childrenOnPage = null;
			bool flag2 = true;
			bool flag3 = true;
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
						num4 = pageItem.ReserveSpaceForRepeatWith(base.m_repeatWithItems, pageContext2);
						if (array != null)
						{
							pageItem.CalculatePage(rplWriter, array[i], pageContext2, base.m_children, base.m_repeatWithItems, num + num4, ref num2, interactivity);
						}
						else
						{
							pageItem.CalculatePage(rplWriter, null, pageContext2, base.m_children, base.m_repeatWithItems, num + num4, ref num2, interactivity);
						}
						if (!pageContext2.FullOnPage)
						{
							processPageBreaks.ProcessItemPageBreaks(pageItem);
							if (pageItem.ItemState != State.OnPage && pageItem.ItemState != State.OnPageHidden)
							{
								if (pageItem.ItemState != State.OnPagePBEnd)
								{
									flag2 = false;
								}
								if (pageItem.ItemState != State.Below)
								{
									flag3 = false;
								}
							}
							else
							{
								base.m_prevPageEnd = num2;
								flag3 = false;
							}
							if (rplWriter != null)
							{
								pageItem.MergeRepeatSiblings(ref repeatedSiblings);
							}
						}
						else if (!pageContext.FullOnPage && rplWriter != null)
						{
							pageItem.MergeRepeatSiblings(ref repeatedSiblings);
						}
						num5 = Math.Max(num5, pageItem.ItemPageSizes.Bottom + base.m_itemPageSizes.PaddingBottom);
					}
				}
				if (itemSizes != null)
				{
					base.ConsumeWhitespaceVertical(itemSizes, num5, pageContext2);
				}
				else
				{
					base.ConsumeWhitespaceVertical(base.m_itemPageSizes, num5, pageContext2);
				}
			}
			if (pageContext2.CancelPage)
			{
				base.m_itemState = State.Below;
				base.m_children = null;
				base.m_rplElement = null;
				return false;
			}
			bool flag4 = false;
			if (processPageBreaks != null && processPageBreaks.HasPageBreaks(ref base.m_prevPageEnd, ref num2))
			{
				if (flag2)
				{
					if (num2 - base.m_itemPageSizes.Height != 0.0)
					{
						flag2 = false;
					}
					else
					{
						flag4 = true;
					}
				}
			}
			else if (!pageContext2.FullOnPage)
			{
				if (flag2)
				{
					double num6 = num + base.m_itemPageSizes.Height;
					if ((RoundedDouble)num6 > pageContext2.PageHeight && (RoundedDouble)(num6 - base.m_itemPageSizes.PaddingBottom) <= pageContext2.PageHeight)
					{
						double val = pageContext2.PageHeight - num;
						base.m_prevPageEnd = Math.Max(num2, val);
						num2 = base.m_prevPageEnd;
						flag2 = false;
					}
					else
					{
						num2 = base.m_itemPageSizes.Height;
					}
				}
				else if (flag3)
				{
					double num7 = num + base.m_itemPageSizes.Height;
					if ((RoundedDouble)num7 > pageContext2.PageHeight)
					{
						base.m_prevPageEnd = pageContext2.PageHeight - num;
						num2 = base.m_prevPageEnd;
					}
				}
			}
			else
			{
				num2 = base.m_itemPageSizes.Height;
			}
			if (pageContext2.FullOnPage || flag2)
			{
				base.m_itemState = State.OnPage;
				if (flag2)
				{
					if (!pageContext2.IgnorePageBreaks && base.PageBreakAtEnd)
					{
						pageContext.RegisterPageBreak(new PageBreakInfo(this.PageBreak, base.ItemName), overrideChild);
						base.m_itemState = State.OnPagePBEnd;
					}
					else if (flag4)
					{
						base.m_itemState = State.OnPagePBEnd;
					}
					if (pageContext2.TracingEnabled && pageContext2.IgnorePageBreaks)
					{
						base.TracePageBreakAtEndIgnored(pageContext2);
					}
				}
				parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Top + num2);
				if (rplWriter != null)
				{
					this.CreateItemRenderSizes(itemSizes, pageContext2, false);
					num3 = base.CalculateRenderSizes(rplWriter, pageContext2, interactivity, repeatedSiblings, out childrenOnPage);
					this.WriteEndItemToStream(rplWriter, num3, childrenOnPage);
				}
				base.m_indexesLeftToRight = null;
				base.m_children = null;
			}
			else
			{
				base.m_itemState = State.SpanPages;
				if (rectangle.OmitBorderOnPageBreak)
				{
					base.m_rplItemState |= 2;
				}
				parentPageHeight = Math.Max(parentPageHeight, base.m_itemPageSizes.Top + num2);
				if (rplWriter != null)
				{
					this.CreateItemRenderSizes(null, pageContext2, false);
					base.m_itemRenderSizes.PaddingBottom = 0.0;
					base.m_itemRenderSizes.AdjustHeightTo(num2);
					num3 = base.CalculateRenderSizes(rplWriter, pageContext2, interactivity, repeatedSiblings, out childrenOnPage);
					this.WriteEndItemToStream(rplWriter, num3, childrenOnPage);
				}
				else
				{
					base.ReleaseChildrenOnPage();
				}
			}
			return true;
		}

		internal override void CalculateRepeatWithPage(RPLWriter rplWriter, PageContext pageContext, PageItem[] siblings)
		{
			base.AdjustOriginFromItemsAbove(siblings, null);
			ItemSizes itemSizes = null;
			if (base.ResolveItemHiddenState(rplWriter, null, pageContext, true, ref itemSizes))
			{
				this.CalculateHiddenItemRenderSize(pageContext, true);
			}
			else
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle rectangle = (AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)base.m_source;
				this.m_staticItem = base.CreateChildren(rectangle.ReportItemCollection, pageContext, rectangle.Width.ToMillimeters(), rectangle.Height.ToMillimeters(), rectangle.IsSimple);
				base.m_itemsCreated = true;
				if (itemSizes != null)
				{
					itemSizes.SetPaddings(base.m_itemPageSizes.PaddingRight, base.m_itemPageSizes.PaddingBottom);
				}
				if (base.m_children != null)
				{
					double num = base.m_itemPageSizes.PaddingBottom;
					PageItem pageItem = null;
					for (int i = 0; i < base.m_children.Length; i++)
					{
						pageItem = base.m_children[i];
						if (pageItem != null)
						{
							pageItem.CalculateRepeatWithPage(rplWriter, pageContext, base.m_children);
							num = Math.Max(num, pageItem.ItemPageSizes.Bottom + base.m_itemPageSizes.PaddingBottom);
						}
					}
					if (itemSizes != null)
					{
						base.ConsumeWhitespaceVertical(itemSizes, num, pageContext);
					}
					else
					{
						base.ConsumeWhitespaceVertical(base.m_itemPageSizes, num, pageContext);
					}
				}
				base.m_itemState = State.OnPage;
				this.CreateItemRenderSizes(itemSizes, pageContext, true);
				base.CalculateRepeatWithRenderSizes(pageContext);
			}
		}

		internal override int WriteRepeatWithToPage(RPLWriter rplWriter, PageContext pageContext)
		{
			if (base.ItemState == State.OnPageHidden)
			{
				return 0;
			}
			this.WriteStartItemToStream(rplWriter, pageContext);
			int num = 0;
			if (base.m_children != null)
			{
				for (int i = 0; i < base.m_children.Length; i++)
				{
					if (base.m_children[i] != null)
					{
						num += base.m_children[i].WriteRepeatWithToPage(rplWriter, pageContext);
					}
				}
			}
			base.WriteRepeatWithEndItemToStream(rplWriter, num);
			return 1;
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
					binaryWriter.Write((byte)10);
					this.WriteElementProps(binaryWriter, rplWriter, pageContext, base.m_offset + 1);
				}
				else
				{
					base.m_rplElement = new RPLRectangle();
					this.WriteElementProps(base.m_rplElement.ElementProps, rplWriter, pageContext);
				}
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle rectangle = (AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)base.m_source;
			ReportItemCollection reportItemCollection = rectangle.ReportItemCollection;
			if (rectangle.LinkToChild >= 0 && rectangle.LinkToChild < reportItemCollection.Count)
			{
				ReportItem reportItem = ((ReportElementCollectionBase<ReportItem>)reportItemCollection)[rectangle.LinkToChild];
				spbifWriter.Write((byte)43);
				spbifWriter.Write(reportItem.ID);
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle rectangle = (AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)base.m_source;
			ReportItemCollection reportItemCollection = rectangle.ReportItemCollection;
			if (rectangle.LinkToChild >= 0 && rectangle.LinkToChild < reportItemCollection.Count)
			{
				ReportItem reportItem = ((ReportElementCollectionBase<ReportItem>)reportItemCollection)[rectangle.LinkToChild];
				((RPLRectanglePropsDef)sharedProps).LinkToChildId = reportItem.ID;
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)6);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)255);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemContainerHelper(6);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}
	}
}
