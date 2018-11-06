using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class ProcessPageBreaks
	{
		private bool m_pbAtStart;

		private double m_minAtStart;

		private bool m_spanItems;

		private double m_maxSpanItems;

		private List<double> m_pbsAtEnd;

		private double m_maxAtEnd;

		internal void ProcessItemPageBreaks(PageItem pageItem)
		{
			if (pageItem.ItemState != PageItem.State.OnPage && pageItem.ItemState != PageItem.State.OnPageHidden)
			{
				if (pageItem.ItemState == PageItem.State.OnPagePBEnd)
				{
					if (this.m_pbsAtEnd != null)
					{
						this.m_maxAtEnd = Math.Max(this.m_maxAtEnd, pageItem.ItemPageSizes.Bottom);
					}
					else
					{
						this.m_pbsAtEnd = new List<double>();
						this.m_maxAtEnd = pageItem.ItemPageSizes.Bottom;
					}
					this.m_pbsAtEnd.Add(pageItem.ItemPageSizes.Bottom);
				}
				else if (pageItem.ItemState == PageItem.State.TopNextPage)
				{
					if (this.m_pbAtStart)
					{
						this.m_minAtStart = Math.Min(this.m_minAtStart, pageItem.ItemPageSizes.Top);
					}
					else
					{
						this.m_pbAtStart = true;
						this.m_minAtStart = pageItem.ItemPageSizes.Top;
					}
				}
				else if (pageItem.ItemState == PageItem.State.SpanPages)
				{
					this.m_spanItems = true;
					this.m_maxSpanItems = Math.Max(this.m_maxSpanItems, pageItem.ItemPageSizes.Top);
					this.ResolveItemPosition(this.m_maxSpanItems, false);
				}
			}
			else
			{
				this.ResolveItemPosition(pageItem.ItemPageSizes.Bottom, true);
			}
		}

		internal bool HasPageBreaks(ref double breakPosition, ref double pageItemHeight)
		{
			if (this.m_pbAtStart)
			{
				breakPosition = this.m_minAtStart;
				pageItemHeight = this.m_minAtStart;
				return true;
			}
			if (this.m_pbsAtEnd != null)
			{
				breakPosition = this.m_maxAtEnd;
				return true;
			}
			if (this.m_spanItems)
			{
				breakPosition = this.m_maxSpanItems;
				return true;
			}
			return false;
		}

		private void ResolveItemPosition(double itemPosition, bool checkSpanItems)
		{
			if (this.m_pbAtStart && (RoundedDouble)itemPosition >= this.m_minAtStart)
			{
				this.m_pbAtStart = false;
				this.m_minAtStart = 0.0;
			}
			if (checkSpanItems && this.m_spanItems && (RoundedDouble)itemPosition >= this.m_maxSpanItems)
			{
				this.m_spanItems = false;
				this.m_maxSpanItems = 0.0;
			}
			if (this.m_pbsAtEnd != null && (RoundedDouble)itemPosition < this.m_maxAtEnd)
			{
				this.m_maxAtEnd = 0.0;
				int num = 0;
				while (num < this.m_pbsAtEnd.Count)
				{
					if ((RoundedDouble)itemPosition < this.m_pbsAtEnd[num])
					{
						this.m_pbsAtEnd.RemoveAt(num);
					}
					else
					{
						num++;
						this.m_maxAtEnd = Math.Max(this.m_maxAtEnd, this.m_pbsAtEnd[num]);
					}
				}
				if (this.m_pbsAtEnd.Count == 0)
				{
					this.m_pbsAtEnd = null;
				}
			}
		}
	}
}
