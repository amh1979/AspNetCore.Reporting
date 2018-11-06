using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Internal;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal class HPBProcessing : IDisposable
	{
		protected static ResourceManager HPBResManager;

		private PageContext m_pageContext;

		private PaginationSettings m_paginationSettings;

		private List<SectionItemizedData> m_glyphCache;

		private int m_startPage = 1;

		private int m_endPage = 1;

		private int m_totalPages;

		private bool m_createStream = true;

		private CreateAndRegisterStream m_createAndRegisterStream;

		private Report m_report;

		private SelectiveRendering m_selectiveRendering;

		private static Version m_rplVersion;

		internal static ResourceManager HPBResourceManager
		{
			get
			{
				return HPBProcessing.HPBResManager;
			}
		}

		public PaginationSettings PaginationSettings
		{
			get
			{
				return this.m_paginationSettings;
			}
		}

		internal FontCache SharedFontCache
		{
			get
			{
				return this.m_pageContext.Common.FontCache;
			}
		}

		internal List<SectionItemizedData> GlyphCache
		{
			get
			{
				return this.m_glyphCache;
			}
		}

		static HPBProcessing()
		{
			HPBProcessing.HPBResManager = null;
			HPBProcessing.m_rplVersion = new Version(10, 6, 0);
			HPBProcessing.HPBResManager = new ResourceManager("AspNetCore.ReportingServices.Rendering.HPBProcessing.Images", Assembly.GetExecutingAssembly());
		}

		public HPBProcessing(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection deviceInfo, CreateAndRegisterStream createAndRegisterStream, ref Hashtable renderProperties)
		{
			this.Init(report, new PaginationSettings(report, deviceInfo), createAndRegisterStream, ref renderProperties);
		}

		public HPBProcessing(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, PaginationSettings pagination, CreateAndRegisterStream createAndRegisterStream, ref Hashtable renderProperties)
		{
			this.Init(report, pagination, createAndRegisterStream, ref renderProperties);
		}

		private void Init(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, PaginationSettings pagination, CreateAndRegisterStream createAndRegisterStream, ref Hashtable renderProperties)
		{
			this.m_pageContext = new PageContext(pagination, report.AddToCurrentPage, report.ConsumeContainerWhitespace, createAndRegisterStream);
			this.m_paginationSettings = pagination;
			this.m_report = new Report(report, this.m_pageContext, pagination);
			this.m_createAndRegisterStream = createAndRegisterStream;
			if (report.SnapshotPageSizeInfo != AspNetCore.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Large)
			{
				this.m_createStream = false;
			}
			if (!string.IsNullOrEmpty(pagination.ReportItemPath))
			{
				this.m_pageContext.Common.IsInSelectiveRendering = true;
				this.m_selectiveRendering = new SelectiveRendering(report, this.m_pageContext, pagination);
			}
			else if (this.m_totalPages <= 0)
			{
				this.m_totalPages = 0;
				if (report.NeedsOverallTotalPages | report.NeedsPageBreakTotalPages)
				{
					this.m_pageContext.Common.PauseDiagnostics();
					this.SetContext(0, 0);
					this.m_pageContext.PropertyCacheState = PageContext.CacheState.CountPages;
					while (this.NextPage())
					{
					}
					this.m_totalPages = this.m_pageContext.PageNumber;
					this.m_pageContext.Common.UpdateTotalPagesRegionMapping();
					this.m_pageContext.Common.ResumeDiagnostics();
					this.m_pageContext.TextBoxDuplicates = null;
				}
			}
		}

		public void Dispose()
		{
			this.m_glyphCache = null;
			this.m_pageContext.DisposeGraphics();
			if (this.m_report.JobContext != null)
			{
				IJobContext jobContext = this.m_report.JobContext;
				lock (jobContext.SyncRoot)
				{
					if (jobContext.AdditionalInfo.ScalabilityTime == null)
					{
						jobContext.AdditionalInfo.ScalabilityTime = new ScaleTimeCategory();
					}
					jobContext.AdditionalInfo.ScalabilityTime.Pagination = this.m_pageContext.TotalScaleTimeMs;
					if (jobContext.AdditionalInfo.EstimatedMemoryUsageKB == null)
					{
						jobContext.AdditionalInfo.EstimatedMemoryUsageKB = new EstimatedMemoryUsageKBCategory();
					}
					jobContext.AdditionalInfo.EstimatedMemoryUsageKB.Pagination = this.m_pageContext.PeakMemoryUsageKB;
				}
			}
			GC.SuppressFinalize(this);
		}

		private bool NextPage()
		{
			if (this.m_report.Done)
			{
				return false;
			}
			this.IncrementPageNumber();
			this.m_report.NextPage(null, this.m_totalPages);
			return true;
		}

		private void CreateCacheStream()
		{
			Stream stream = this.m_pageContext.PropertyCache;
			if (stream == null)
			{
				stream = this.m_createAndRegisterStream("NonSharedCache", "rpl", null, null, true, StreamOper.CreateOnly);
				this.m_pageContext.PropertyCache = stream;
			}
			stream.Position = 0L;
		}

		public void SetContext()
		{
			this.SetContext(this.m_paginationSettings.StartPage, this.m_paginationSettings.EndPage);
		}

		public void SetContext(int startPage, int endPage)
		{
			if (startPage <= endPage && endPage >= 0)
			{
				this.m_startPage = startPage;
				this.m_endPage = endPage;
				if (startPage == 0)
				{
					this.m_endPage = this.m_startPage;
				}
				this.CreateCacheStream();
				if (this.m_createStream)
				{
					this.m_pageContext.PropertyCacheState = PageContext.CacheState.RPLStream;
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "RPL stream in use");
				}
				else
				{
					this.m_pageContext.PropertyCacheState = PageContext.CacheState.RPLObjectModel;
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "RPL object model in use");
				}
				this.m_report.SetContext();
				this.ResetPageNumber();
				while (this.m_pageContext.PageNumber < this.m_startPage - 1 && !this.m_report.Done)
				{
					this.IncrementPageNumber();
					this.m_report.NextPage(null, this.m_totalPages);
				}
			}
		}

		public void SetContext(int startPage, int endPage, bool createStream)
		{
			this.m_createStream = createStream;
			this.SetContext(startPage, endPage);
		}

		public Stream GetNextPage()
		{
			RPLReport rPLReport = null;
			this.m_createStream = true;
			return this.GetNextPage(out rPLReport);
		}

		public Stream GetNextPage(out RPLReport rplReport)
		{
			rplReport = null;
			this.m_glyphCache = null;
			if (this.m_report.Done)
			{
				return null;
			}
			if (this.m_selectiveRendering != null && this.m_selectiveRendering.Done)
			{
				return null;
			}
			Stream stream = null;
			RPLWriter rPLWriter = null;
			this.IncrementPageNumber();
			if (this.m_startPage >= 0 && this.m_endPage >= 0 && (this.m_endPage == 0 || (this.m_pageContext.PageNumber >= this.m_startPage && this.m_pageContext.PageNumber <= this.m_endPage)))
			{
				rPLWriter = new RPLWriter();
				if (this.m_createStream)
				{
					string name = this.m_pageContext.PageNumber.ToString(CultureInfo.InvariantCulture);
					stream = this.m_createAndRegisterStream(name, "rpl", null, null, true, StreamOper.CreateOnly);
					BufferedStream output = new BufferedStream(stream);
					BinaryWriter binaryWriter2 = rPLWriter.BinaryWriter = new BinaryWriter(output, Encoding.Unicode);
					binaryWriter2.Write("RPLIF");
					this.WriteVersionStamp(binaryWriter2);
				}
			}
			if (this.m_selectiveRendering != null)
			{
				this.m_selectiveRendering.RenderReportItem(rPLWriter, this.m_paginationSettings.ReportItemPath);
			}
			else
			{
				this.m_report.NextPage(rPLWriter, this.m_totalPages);
			}
			if (rPLWriter != null)
			{
				this.m_glyphCache = rPLWriter.GlyphCache;
				if (this.m_createStream)
				{
					this.WriteVersionStamp(rPLWriter.BinaryWriter);
					rPLWriter.BinaryWriter.Flush();
					rPLWriter = null;
					BufferedStream input = new BufferedStream(stream);
					BinaryReader reader = new BinaryReader(input, Encoding.Unicode);
					rplReport = new RPLReport(reader);
				}
				else
				{
					rplReport = rPLWriter.Report;
				}
			}
			return stream;
		}

		private void ResetPageNumber()
		{
			this.m_pageContext.PageNumber = 0;
			this.m_pageContext.PageNumberRegion = 0;
			this.m_pageContext.Common.ResetPageBreakProcessing();
			this.m_pageContext.Common.ResetPageNameProcessing();
			this.m_pageContext.Common.ResetPageNameTracing();
		}

		private void IncrementPageNumber()
		{
			this.m_pageContext.Common.ProcessPageBreakProperties();
			if (!this.m_pageContext.Common.PaginatingHorizontally)
			{
				this.m_pageContext.Common.ResetPageNameProcessing();
			}
			if (this.m_pageContext.PageNumber == 0)
			{
				this.m_pageContext.Common.OverwritePageName(this.m_report.InitialPageName);
			}
			this.m_pageContext.PageNumber++;
			this.m_pageContext.PageNumberRegion++;
		}

		private void WriteVersionStamp(BinaryWriter spbifWriter)
		{
			spbifWriter.Write((byte)HPBProcessing.m_rplVersion.Major);
			spbifWriter.Write((byte)HPBProcessing.m_rplVersion.Minor);
			spbifWriter.Write(HPBProcessing.m_rplVersion.Build);
		}
	}
}
