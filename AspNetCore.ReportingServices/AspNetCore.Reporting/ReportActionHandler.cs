using AspNetCore.ReportingServices.Rendering.HtmlRenderer;
using System;
using System.ComponentModel;

namespace AspNetCore.Reporting
{
	internal sealed class ReportActionHandler
	{
		private Report m_report;

		private PageCountMode m_pageCountMode;

		private int m_currentPage;

		private object m_eventSender;

		//private PageNavigationEventHandler m_pageNav;

		private CancelEventHandler m_toggle;

		//private BookmarkNavigationEventHandler m_bookmarkNavigation;

		private DocumentMapNavigationEventHandler m_documentMapNavigation;

		//private DrillthroughEventHandler m_drillthrough;

		private SortEventHandler m_sort;

		//private SearchEventHandler m_search;

		private CancelEventHandler m_refresh;

		public ReportActionHandler(Report report, object eventSender, int currentPage, PageCountMode pageCountMode, CancelEventHandler toggle, DocumentMapNavigationEventHandler documentMapNavigation,
            SortEventHandler sort, CancelEventHandler refresh)
		{
			this.m_report = report;
			this.m_pageCountMode = pageCountMode;
			this.m_currentPage = currentPage;
			this.m_eventSender = eventSender;
			//this.m_pageNav = pageNav;
			this.m_toggle = toggle;
			//this.m_bookmarkNavigation = bookmarkNavigation;
			this.m_documentMapNavigation = documentMapNavigation;
			//this.m_drillthrough = drillthrough;
			this.m_sort = sort;
			//this.m_search = search;
			this.m_refresh = refresh;
		}

		public bool HandleToggle(string toggleID)
		{
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			if (this.m_toggle != null)
			{
				this.m_toggle(this.m_eventSender, cancelEventArgs);
			}
			if (!cancelEventArgs.Cancel)
			{
				this.m_report.PerformToggle(toggleID);
				//scrollTarget = new ScrollTarget(toggleID, ActionScrollStyle.MaintainPosition);
				return true;
			}
			//scrollTarget = null;
			return false;
		}






		public bool HandleRefresh()
		{
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			if (this.m_refresh != null)
			{
				this.m_refresh(this.m_eventSender, cancelEventArgs);
			}
			if (!cancelEventArgs.Cancel)
			{
				this.m_report.Refresh();
				return true;
			}
			return false;
		}

		public bool HandlePageNavigation(int targetPage)
		{
			if (targetPage <= 0)
			{
				throw new ArgumentOutOfRangeException("targetPage");
			}
			PageCountMode pageCountMode = default(PageCountMode);
			int totalPages = this.m_report.GetTotalPages(out pageCountMode);
			if (totalPages == 0)
			{
				pageCountMode = PageCountMode.Estimate;
			}
			if (targetPage == 2147483647 && pageCountMode != PageCountMode.Estimate)
			{
				targetPage = totalPages;
			}
			if (targetPage > totalPages && pageCountMode != PageCountMode.Estimate)
			{
				throw new InvalidOperationException(Errors.InvalidPageNav);
			}
			return this.FirePageNavigationEvent(targetPage);
		}

		private bool FirePageNavigationEvent(int targetPage)
		{

			return true;
		}

		private void ActionParamToSortParams(string actionParam, out string sortID, out SortOrder sortDirection, out bool clearSort)
		{
			string[] array = actionParam.Split('_');
			if (array.Length < 3)
			{
				throw new ArgumentOutOfRangeException("actionParam");
			}
			sortID = array[0];
			if (string.Compare(array[1], "A", StringComparison.Ordinal) == 0)
			{
				sortDirection = SortOrder.Ascending;
			}
			else
			{
				sortDirection = SortOrder.Descending;
			}
			clearSort = (string.Compare(array[2], "T", StringComparison.Ordinal) != 0);
		}

		private int GetSearchEndPage(int startPage)
		{
			if (startPage == 1)
			{
				PageCountMode pageCountMode = default(PageCountMode);
				int totalPages = this.m_report.GetTotalPages(out pageCountMode);
				if (pageCountMode != 0)
				{
					return 2147483647;
				}
				return totalPages;
			}
			return startPage - 1;
		}
	}
}
