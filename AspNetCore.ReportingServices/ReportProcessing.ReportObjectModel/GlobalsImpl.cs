using System;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class GlobalsImpl : Globals
	{
		internal const string Name = "Globals";

		internal const string FullName = "AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.Globals";

		private string m_reportName;

		private int m_pageNumber;

		private int m_totalPages;

		private DateTime m_executionTime;

		private string m_reportServerUrl;

		private string m_reportFolder;

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
				case "ExecutionTime":
					return this.m_executionTime;
				case "ReportServerUrl":
					return this.m_reportServerUrl;
				case "ReportFolder":
					return this.m_reportFolder;
				case "RenderFormat":
					return new NotSupportedException();
				default:
					throw new ArgumentOutOfRangeException("key");
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
				return this.m_pageNumber;
			}
		}

		public override int OverallTotalPages
		{
			get
			{
				return this.m_totalPages;
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

		public override string PageName
		{
			get
			{
				return null;
			}
		}

		public override RenderFormat RenderFormat
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		internal GlobalsImpl(string reportName, DateTime executionTime, string reportServerUrl, string reportFolder)
		{
			this.m_reportName = reportName;
			this.m_pageNumber = 1;
			this.m_totalPages = 1;
			this.m_executionTime = executionTime;
			this.m_reportServerUrl = reportServerUrl;
			this.m_reportFolder = reportFolder;
		}

		internal GlobalsImpl(string reportName, int pageNumber, int totalPages, DateTime executionTime, string reportServerUrl, string reportFolder)
		{
			this.m_reportName = reportName;
			this.m_pageNumber = pageNumber;
			this.m_totalPages = totalPages;
			this.m_executionTime = executionTime;
			this.m_reportServerUrl = reportServerUrl;
			this.m_reportFolder = reportFolder;
		}

		internal void SetPageNumber(int pageNumber)
		{
			this.m_pageNumber = pageNumber;
		}

		internal void SetPageNumbers(int pageNumber, int totalPages)
		{
			this.m_pageNumber = pageNumber;
			this.m_totalPages = totalPages;
		}
	}
}
