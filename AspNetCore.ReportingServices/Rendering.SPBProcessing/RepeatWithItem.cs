using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class RepeatWithItem
	{
		internal const string RepeatSuffix = "_REPEAT";

		private double m_relativeTop;

		private double m_relativeBottom;

		private double m_relativeTopToBottom;

		private int m_dataRegionIndex;

		private PageItem m_pageItem;

		private ItemSizes m_renderItemSize;

		internal int DataRegionIndex
		{
			get
			{
				return this.m_dataRegionIndex;
			}
		}

		internal PageItem SourcePageItem
		{
			get
			{
				return this.m_pageItem;
			}
		}

		internal double RelativeTop
		{
			get
			{
				return this.m_relativeTop;
			}
		}

		internal double RelativeBottom
		{
			get
			{
				return this.m_relativeBottom;
			}
		}

		internal RepeatWithItem(PageItem pageItem, PageContext pageContext)
		{
			this.m_pageItem = pageItem;
			this.m_pageItem.ItemState = PageItem.State.OnPage;
		}

		internal void UpdateCreateState(PageItem dataRegion, int dataRegionIndex, List<int> pageItemsAbove, PageContext pageContext)
		{
			this.m_dataRegionIndex = dataRegionIndex;
			this.m_relativeTop = this.m_pageItem.ItemPageSizes.Top - dataRegion.ItemPageSizes.Top;
			this.m_relativeBottom = this.m_pageItem.ItemPageSizes.Bottom - dataRegion.ItemPageSizes.Bottom;
			this.m_relativeTopToBottom = this.m_pageItem.ItemPageSizes.Top - dataRegion.ItemPageSizes.Bottom;
			if (pageItemsAbove != null)
			{
				this.m_pageItem.PageItemsAbove = new List<int>(pageItemsAbove);
			}
			PaddItemSizes paddItemSizes = this.m_pageItem.ItemRenderSizes as PaddItemSizes;
			if (paddItemSizes != null)
			{
				if (pageContext != null)
				{
					this.m_renderItemSize = pageContext.GetSharedRenderRepeatItemSizesElement(paddItemSizes, true, true);
				}
				else
				{
					this.m_renderItemSize = new PaddItemSizes(paddItemSizes);
				}
			}
			else if (pageContext != null)
			{
				this.m_renderItemSize = pageContext.GetSharedRenderRepeatItemSizesElement(this.m_pageItem.ItemRenderSizes, false, false);
			}
			else
			{
				this.m_renderItemSize = new ItemSizes(this.m_pageItem.ItemRenderSizes);
			}
		}

		internal void UpdateSizes(PageContext pageContext)
		{
			PaddItemSizes paddItemSizes = this.m_renderItemSize as PaddItemSizes;
			if (paddItemSizes != null)
			{
				if (pageContext != null)
				{
					this.m_pageItem.ItemRenderSizes = pageContext.GetSharedRenderItemSizesElement(paddItemSizes, true, true);
				}
				else
				{
					this.m_pageItem.ItemRenderSizes = new PaddItemSizes(paddItemSizes);
				}
			}
			else if (pageContext != null)
			{
				this.m_pageItem.ItemRenderSizes = pageContext.GetSharedRenderItemSizesElement(this.m_renderItemSize, false, false);
			}
			else
			{
				this.m_pageItem.ItemRenderSizes = new ItemSizes(this.m_renderItemSize);
			}
		}

		internal bool AddOnPage(ItemSizes dataRegionSizes, PageItem[] siblings, int itemIndex, ref List<int> parentOverlappedItems, ref double header)
		{
			if (siblings == null)
			{
				return true;
			}
			double num = dataRegionSizes.Top + this.m_relativeTop;
			num = ((!(this.m_relativeTopToBottom < 0.0)) ? (dataRegionSizes.Bottom - dataRegionSizes.DeltaY + this.m_relativeTopToBottom) : (dataRegionSizes.Top + this.m_relativeTop));
			double x = num + this.m_pageItem.ItemRenderSizes.Height - this.m_pageItem.ItemRenderSizes.DeltaY;
			PageItem pageItem = null;
			List<int> list = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			for (int i = 0; i < siblings.Length; i++)
			{
				pageItem = siblings[i];
				if (pageItem != null && pageItem.ItemState != PageItem.State.Below && pageItem.ItemState != PageItem.State.TopNextPage)
				{
					roundedDouble.Value = pageItem.ItemRenderSizes.Left;
					roundedDouble2.Value = pageItem.ItemRenderSizes.Right - pageItem.ItemRenderSizes.DeltaX;
					if (!(roundedDouble2 <= this.m_pageItem.ItemRenderSizes.Left) && !(roundedDouble >= this.m_pageItem.ItemRenderSizes.Right - this.m_pageItem.ItemRenderSizes.DeltaX))
					{
						roundedDouble.Value = pageItem.ItemRenderSizes.Top;
						roundedDouble2.Value = pageItem.ItemRenderSizes.Bottom - pageItem.ItemRenderSizes.DeltaY;
						if (!(roundedDouble2 <= num) && !(roundedDouble >= x))
						{
							goto IL_01b2;
						}
						roundedDouble2.Value = pageItem.ItemRenderSizes.Bottom;
						x = num + this.m_pageItem.ItemRenderSizes.Height;
						if (!(roundedDouble2 <= num) && !(roundedDouble >= x))
						{
							goto IL_01b2;
						}
					}
				}
				continue;
				IL_01b2:
				if (roundedDouble >= num)
				{
					if (pageItem.PageItemsAbove == null)
					{
						return false;
					}
					if (pageItem.PageItemsAbove.BinarySearch(itemIndex) < 0)
					{
						return false;
					}
					if (list == null)
					{
						list = new List<int>();
					}
					list.Add(i);
				}
				else
				{
					if (this.m_pageItem.PageItemsAbove == null)
					{
						return false;
					}
					if (this.m_pageItem.PageItemsAbove.BinarySearch(i) < 0)
					{
						return false;
					}
					if (list == null)
					{
						list = new List<int>();
					}
					list.Add(itemIndex);
				}
			}
			this.m_pageItem.ItemRenderSizes.Top = num;
			header = Math.Min(header, num);
			if (parentOverlappedItems == null)
			{
				parentOverlappedItems = list;
			}
			else if (list != null)
			{
				int j = 0;
				for (int k = 0; k < list.Count; k++)
				{
					for (; parentOverlappedItems[j] < list[k]; j++)
					{
					}
					if (j < parentOverlappedItems.Count)
					{
						if (parentOverlappedItems[j] > list[k])
						{
							parentOverlappedItems.Insert(j, list[k]);
						}
					}
					else
					{
						parentOverlappedItems.Add(list[k]);
					}
					j++;
				}
			}
			return true;
		}

		internal void WriteRepeatWithToPage(RPLWriter rplWriter, PageContext pageContext)
		{
			this.m_pageItem.WriteRepeatWithToPage(rplWriter, pageContext);
		}

		internal void UpdateItem(PageItemHelper itemHelper, RPLWriter rplWriter, PageContext pageContext)
		{
			if (itemHelper != null)
			{
				PageItemRepeatWithHelper pageItemRepeatWithHelper = itemHelper as PageItemRepeatWithHelper;
				RSTrace.RenderingTracer.Assert(pageItemRepeatWithHelper != null, "This should be a RepeatWith");
				this.m_relativeTop = pageItemRepeatWithHelper.RelativeTop;
				this.m_relativeBottom = pageItemRepeatWithHelper.RelativeBottom;
				this.m_relativeTopToBottom = pageItemRepeatWithHelper.RelativeTopToBottom;
				this.m_dataRegionIndex = pageItemRepeatWithHelper.DataRegionIndex;
				if (pageItemRepeatWithHelper.RenderItemSize != null)
				{
					this.m_renderItemSize = pageItemRepeatWithHelper.RenderItemSize.GetNewItem();
				}
				if (this.m_pageItem != null)
				{
					PageContext pageContext2 = new PageContext(pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.Repeated);
					this.m_pageItem.CalculateRepeatWithPage(rplWriter, pageContext2, null);
				}
			}
		}

		internal void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)13);
				reportPageInfo.Write((byte)12);
				reportPageInfo.Write(this.m_relativeTop);
				reportPageInfo.Write((byte)13);
				reportPageInfo.Write(this.m_relativeBottom);
				reportPageInfo.Write((byte)14);
				reportPageInfo.Write(this.m_relativeTopToBottom);
				reportPageInfo.Write((byte)15);
				reportPageInfo.Write(this.m_dataRegionIndex);
				if (this.m_renderItemSize != null)
				{
					this.m_renderItemSize.WritePaginationInfo(reportPageInfo);
				}
				reportPageInfo.Write((byte)255);
			}
		}

		internal PageItemHelper WritePaginationInfo()
		{
			PageItemRepeatWithHelper pageItemRepeatWithHelper = new PageItemRepeatWithHelper(13);
			pageItemRepeatWithHelper.RelativeTop = this.m_relativeTop;
			pageItemRepeatWithHelper.RelativeBottom = this.m_relativeBottom;
			pageItemRepeatWithHelper.RelativeTopToBottom = this.m_relativeTopToBottom;
			pageItemRepeatWithHelper.DataRegionIndex = this.m_dataRegionIndex;
			if (this.m_renderItemSize != null)
			{
				pageItemRepeatWithHelper.RenderItemSize = this.m_renderItemSize.WritePaginationInfo();
			}
			return pageItemRepeatWithHelper;
		}
	}
}
