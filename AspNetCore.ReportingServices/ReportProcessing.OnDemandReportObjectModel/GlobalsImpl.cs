using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class GlobalsImpl : Globals
	{
		private string m_reportName;

		private int m_pageNumber;

		private int m_totalPages;

		private int m_overallPageNumber;

		private int m_overallTotalPages;

		private DateTime m_executionTime;

		private string m_reportServerUrl;

		private string m_reportFolder;

		private RenderFormat m_renderFormat;

		private string m_pageName;

		public override object this[string key]
		{
			get
			{
				switch (key)
				{
				case "ReportName":
					return this.m_reportName;
				case "PageNumber":
					return this.m_pageNumber;
				case "TotalPages":
					return this.m_totalPages;
				case "OverallPageNumber":
					return this.m_overallPageNumber;
				case "OverallTotalPages":
					return this.m_overallTotalPages;
				case "ExecutionTime":
					return this.m_executionTime;
				case "ReportServerUrl":
					return this.m_reportServerUrl;
				case "ReportFolder":
					return this.m_reportFolder;
				case "RenderFormat":
					return this.m_renderFormat;
				case "PageName":
					return this.m_pageName;
				default:
					throw new ReportProcessingException_NonExistingGlobalReference(key);
				}
			}
		}

		public override string ReportName
		{
			get
			{
				return this.m_reportName;
			}
		}

		public override int PageNumber
		{
			get
			{
				return this.m_pageNumber;
			}
		}

		public override int TotalPages
		{
			get
			{
				return this.m_totalPages;
			}
		}

		public override int OverallPageNumber
		{
			get
			{
				return this.m_overallPageNumber;
			}
		}

		public override int OverallTotalPages
		{
			get
			{
				return this.m_overallTotalPages;
			}
		}

		public override DateTime ExecutionTime
		{
			get
			{
				return this.m_executionTime;
			}
		}

		public override string ReportServerUrl
		{
			get
			{
				return this.m_reportServerUrl;
			}
		}

		public override string ReportFolder
		{
			get
			{
				return this.m_reportFolder;
			}
		}

		public override RenderFormat RenderFormat
		{
			get
			{
				return this.m_renderFormat;
			}
		}

		public override string PageName
		{
			get
			{
				return this.m_pageName;
			}
		}

		internal GlobalsImpl(OnDemandProcessingContext odpContext)
		{
			this.m_reportName = odpContext.ReportContext.ItemName;
			this.m_executionTime = odpContext.ExecutionTime;
			this.m_reportServerUrl = odpContext.ReportContext.HostRootUri;
			this.m_reportFolder = odpContext.ReportFolder;
			this.m_pageNumber = 1;
			this.m_totalPages = 1;
			this.m_overallPageNumber = 1;
			this.m_overallTotalPages = 1;
			this.m_pageName = null;
			this.m_renderFormat = new RenderFormat(new RenderFormatImpl(odpContext));
		}

		internal GlobalsImpl(string reportName, int pageNumber, int totalPages, int overallPageNumber, int overallTotalPages, DateTime executionTime, string reportServerUrl, string reportFolder, string pageName, OnDemandProcessingContext odpContext)
		{
			this.m_reportName = reportName;
			this.m_pageNumber = pageNumber;
			this.m_totalPages = totalPages;
			this.m_overallPageNumber = overallPageNumber;
			this.m_overallTotalPages = overallTotalPages;
			this.m_executionTime = executionTime;
			this.m_reportServerUrl = reportServerUrl;
			this.m_reportFolder = reportFolder;
			this.m_pageName = pageName;
			this.m_renderFormat = new RenderFormat(new RenderFormatImpl(odpContext));
		}

		internal void SetPageNumbers(int pageNumber, int totalPages, int overallPageNumber, int overallTotalPages)
		{
			this.m_pageNumber = pageNumber;
			this.m_totalPages = totalPages;
			this.m_overallPageNumber = overallPageNumber;
			this.m_overallTotalPages = overallTotalPages;
		}

		internal void SetPageName(string pageName)
		{
			this.m_pageName = pageName;
		}
	}
}
