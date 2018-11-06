using AspNetCore.ReportingServices.OnDemandReportRendering;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class NoRowsItem : PageItem
	{
		internal NoRowsItem(ReportItem source, PageContext pageContext, bool createForRepeat)
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
			base.m_itemPageSizes.AdjustHeightTo(0.0);
			base.m_itemPageSizes.AdjustWidthTo(0.0);
			base.m_itemState = State.OnPage;
			if (interactivity != null)
			{
				interactivity.RegisterItem(this, pageContext);
			}
			if (rplWriter != null && base.m_itemRenderSizes == null)
			{
				this.CreateItemRenderSizes(null, pageContext, false);
			}
			return true;
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)2);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)255);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper(2);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}
	}
}
