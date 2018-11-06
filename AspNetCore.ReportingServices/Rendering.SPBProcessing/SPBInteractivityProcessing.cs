using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class SPBInteractivityProcessing : IInteractivityPaginationModule
	{
		public int ProcessFindStringEvent(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, int totalPages, int startPage, int endPage, string findValue)
		{
			if (findValue != null && startPage > 0 && endPage > 0)
			{
				int num = 0;
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, totalPages, true))
				{
					sPBProcessing.CanTracePagination = true;
					sPBProcessing.SetContext(new SPBContext(0, 0, false));
					return sPBProcessing.FindString(startPage, endPage, findValue);
				}
			}
			return 0;
		}

		public int ProcessUserSortEvent(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, string textbox, ref int numberOfPages, ref PaginationMode paginationMode)
		{
			if (textbox == null)
			{
				return 0;
			}
			int num = 0;
			using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, false))
			{
				sPBProcessing.CanTracePagination = true;
				sPBProcessing.SetContext(new SPBContext(0, 0, true));
				return sPBProcessing.FindUserSort(textbox, ref numberOfPages, ref paginationMode);
			}
		}

		public int ProcessBookmarkNavigationEvent(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, int totalPages, string bookmarkId, out string uniqueName)
		{
			uniqueName = null;
			if (!report.HasBookmarks)
			{
				return 0;
			}
			if (bookmarkId == null)
			{
				return 0;
			}
			int lastPageCollected = 0;
			bool flag = false;
			int num = InteractivityChunks.FindBoomark(report, bookmarkId, ref uniqueName, ref lastPageCollected, ref flag);
			if (!flag && num == 0)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, totalPages, true))
				{
					sPBProcessing.CanTracePagination = true;
					sPBProcessing.SetContext(new SPBContext(0, 0, true));
					return sPBProcessing.FindBookmark(bookmarkId, lastPageCollected, ref uniqueName);
				}
			}
			return num;
		}

		public string ProcessDrillthroughEvent(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, int totalPages, string drillthroughId, out NameValueCollection parameters)
		{
			parameters = null;
			if (drillthroughId == null)
			{
				return null;
			}
			int lastPageCollected = 0;
			string text = null;
			using (SPBProcessing sPBProcessing = new SPBProcessing(report, totalPages, true))
			{
				sPBProcessing.CanTracePagination = true;
				sPBProcessing.SetContext(new SPBContext(0, 0, true));
				return sPBProcessing.FindDrillthrough(drillthroughId, lastPageCollected, out parameters);
			}
		}

		public int ProcessDocumentMapNavigationEvent(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, string documentMapId)
		{
			if (!report.HasDocumentMap)
			{
				return 0;
			}
			if (documentMapId == null)
			{
				return 0;
			}
			int lastPageCollected = 0;
			bool flag = false;
			int num = InteractivityChunks.FindDocumentMapLabel(report, documentMapId, ref lastPageCollected, ref flag);
			if (!flag && num == 0)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, false))
				{
					sPBProcessing.CanTracePagination = true;
					sPBProcessing.SetContext(new SPBContext(0, 0, true));
					return sPBProcessing.FindDocumentMap(documentMapId, lastPageCollected);
				}
			}
			return num;
		}
	}
}
