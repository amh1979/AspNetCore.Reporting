using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class EdgePageItem : PageItem
	{
		internal EdgePageItem(double top, double left, string id, PageContext pageContext)
			: base(null)
		{
			if (pageContext != null)
			{
				base.m_itemPageSizes = pageContext.GetSharedEdgeItemSizesElement(top, left, id);
			}
			if (base.m_itemPageSizes == null)
			{
				base.m_itemPageSizes = new ItemSizes(top, left, id);
			}
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			return true;
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)3);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)255);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper(3);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}
	}
}
