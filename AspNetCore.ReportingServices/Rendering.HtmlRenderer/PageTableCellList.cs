using System;
using System.Collections;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	[Serializable]
	internal sealed class PageTableCellList : ArrayList
	{
		internal new PageTableCell this[int index]
		{
			get
			{
				return (PageTableCell)base[index];
			}
		}
	}
}
