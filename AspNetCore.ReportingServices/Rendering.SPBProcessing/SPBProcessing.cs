using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Internal;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportProcessing;
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

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class SPBProcessing : ISPBProcessing, IDisposable
	{
		internal enum RPLState : byte
		{
			RPLObjectModel,
			RPLStream,
			Unknown
		}

		protected static ResourceManager SPBResManager;

		private CreateAndRegisterStream m_createAndRegisterStream;

		private Report m_report;

		private PageContext m_pageContext;

		private RPLState m_rplState = RPLState.Unknown;

		private bool m_useInteractiveHeight = true;

		private int m_startPage = 1;

		private int m_endPage = 1;

		private int m_currentPage;

		private int m_totalPages;

		private ReportPaginationInfo m_reportInfo;

		private ReportSectionHelper m_lastPageInfo;

		private ReportSectionHelper m_lastPageInfoForCancel;

		private Version m_serverRPLVersion = new Version(10, 6, 0);

		internal static ResourceManager SPBResourceManager
		{
			get
			{
				return SPBProcessing.SPBResManager;
			}
		}

		public bool Done
		{
			get
			{
				if (this.m_report == null)
				{
					return false;
				}
				return this.m_report.Done;
			}
		}

		public Dictionary<string, string> PageBookmarks
		{
			get
			{
				if (this.m_pageContext == null)
				{
					return null;
				}
				return this.m_pageContext.PageBookmarks;
			}
		}

		internal FontCache SharedFontCache
		{
			get
			{
				return this.m_pageContext.Common.FontCache;
			}
		}

		internal bool UseEmSquare
		{
			get
			{
				return this.m_pageContext.Common.EmSquare;
			}
			set
			{
				this.m_pageContext.Common.EmSquare = value;
			}
		}

		internal bool CanTracePagination
		{
			get
			{
				return this.m_pageContext.CanTracePagination;
			}
			set
			{
				this.m_pageContext.CanTracePagination = value;
			}
		}

		static SPBProcessing()
		{
			SPBProcessing.SPBResManager = new ResourceManager("AspNetCore.ReportingServices.Rendering.SPBProcessing.Images", Assembly.GetExecutingAssembly());
		}

		public SPBProcessing(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createAndRegisterStream, double pageHeight)
		{
			this.m_pageContext = new PageContext(report.Instance.InitialPageName, pageHeight, false, report.ConsumeContainerWhitespace, createAndRegisterStream);
			this.m_report = new Report(report, this.m_pageContext, null, false);
			this.m_createAndRegisterStream = createAndRegisterStream;
			if (report.SnapshotPageSizeInfo == AspNetCore.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Large)
			{
				this.m_rplState = RPLState.RPLStream;
			}
			else if (report.SnapshotPageSizeInfo == AspNetCore.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Small)
			{
				this.m_rplState = RPLState.RPLObjectModel;
			}
			this.m_reportInfo = new ReportPaginationInfo();
			this.m_useInteractiveHeight = false;
			if (this.m_totalPages <= 0)
			{
				if (this.m_reportInfo.IsDone)
				{
					this.m_totalPages = this.m_reportInfo.PaginatedPages;
				}
				else
				{
					this.m_totalPages = 0;
					if (report.NeedsTotalPages)
					{
						this.SetContext(new SPBContext());
					}
				}
			}
			this.m_pageContext.CanTracePagination = true;
		}

		public SPBProcessing(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createAndRegisterStream, bool registerEvents, string rplVersion, ref Hashtable renderProperties)
		{
			double pageHeight = report.ReportSections[0].Page.InteractiveHeight.ToMillimeters();
			this.m_pageContext = new PageContext(report.Instance.InitialPageName, pageHeight, registerEvents, report.ConsumeContainerWhitespace, createAndRegisterStream);
			this.m_report = new Report(report, this.m_pageContext, rplVersion, true);
			this.m_reportInfo = new ReportPaginationInfo(report, this.m_serverRPLVersion, pageHeight);
			this.m_reportInfo.ReadRegionPageTotalInfo(this.m_pageContext.PageTotalInfo);
			this.InitializeForInteractiveRenderer(report, createAndRegisterStream, registerEvents, ref renderProperties);
			this.m_pageContext.CanTracePagination = true;
		}

		public SPBProcessing(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createAndRegisterStream, bool registerEvents, ref Hashtable renderProperties)
		{
			double pageHeight = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)report.ReportSections)[0].Page.InteractiveHeight.ToMillimeters();
			this.m_pageContext = new PageContext(report.Instance.InitialPageName, pageHeight, registerEvents, report.ConsumeContainerWhitespace, createAndRegisterStream);
			this.m_report = new Report(report, this.m_pageContext, null, false);
			this.m_reportInfo = new ReportPaginationInfo(report, this.m_serverRPLVersion, pageHeight);
			this.m_reportInfo.ReadRegionPageTotalInfo(this.m_pageContext.PageTotalInfo);
			this.InitializeForInteractiveRenderer(report, createAndRegisterStream, registerEvents, ref renderProperties);
			this.m_pageContext.CanTracePagination = true;
		}

		internal SPBProcessing(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, int totalPages, bool needTotalPages)
		{
			double pageHeight = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)report.ReportSections)[0].Page.InteractiveHeight.ToMillimeters();
			this.m_pageContext = new PageContext(report.Instance.InitialPageName, pageHeight, true, report.ConsumeContainerWhitespace, null);
			this.m_report = new Report(report, this.m_pageContext, null, false);
			this.m_reportInfo = new ReportPaginationInfo(report, this.m_serverRPLVersion, pageHeight);
			this.m_reportInfo.ReadRegionPageTotalInfo(this.m_pageContext.PageTotalInfo);
			this.m_totalPages = totalPages;
			if (report.SnapshotPageSizeInfo == AspNetCore.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Large)
			{
				this.m_rplState = RPLState.RPLStream;
			}
			else if (report.SnapshotPageSizeInfo == AspNetCore.ReportingServices.OnDemandReportRendering.Report.SnapshotPageSize.Small)
			{
				this.m_rplState = RPLState.RPLObjectModel;
			}
			if (this.m_reportInfo.IsDone)
			{
				this.m_totalPages = this.m_reportInfo.PaginatedPages;
			}
			else if (needTotalPages)
			{
				if ((!report.NeedsOverallTotalPages || this.m_totalPages > 0) && !report.NeedsPageBreakTotalPages)
				{
					return;
				}
				this.SetContext(new SPBContext());
			}
		}

		public static int TotalNrOfPages(AspNetCore.ReportingServices.OnDemandReportRendering.Report report)
		{
			int num = 0;
			using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, false))
			{
				if (sPBProcessing.m_totalPages <= 0)
				{
					sPBProcessing.m_pageContext.CanTracePagination = true;
					sPBProcessing.SetContext(new SPBContext());
				}
				return sPBProcessing.m_totalPages;
			}
		}

		public static bool RenderSecondaryStream(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createAndRegisterStream, string streamName)
		{
			if (string.IsNullOrEmpty(streamName))
			{
				return false;
			}
			char[] separator = new char[1]
			{
				'_'
			};
			string[] array = streamName.Split(separator);
			if (array.Length < 3)
			{
				return false;
			}
			if (array[0].Equals("C"))
			{
				int pageNumber = SPBProcessing.ParseInt(array[2], 0);
				SPBProcessing.FindChart(report, array[1], pageNumber, streamName, createAndRegisterStream);
				return true;
			}
			if (array[0].Equals("G"))
			{
				int pageNumber2 = SPBProcessing.ParseInt(array[2], 0);
				SPBProcessing.FindGaugePanel(report, array[1], pageNumber2, streamName, createAndRegisterStream);
				return true;
			}
			if (array[0].Equals("M"))
			{
				int pageNumber3 = SPBProcessing.ParseInt(array[2], 0);
				SPBProcessing.FindMap(report, array[1], pageNumber3, streamName, createAndRegisterStream);
				return true;
			}
			if (array[0].Equals("I"))
			{
				int pageNumber4 = SPBProcessing.ParseInt(array[2], 0);
				SPBProcessing.FindImage(report, array[1], pageNumber4, streamName, createAndRegisterStream);
				return true;
			}
			string sTREAMPREFIX = ImageConsolidation.STREAMPREFIX;
			if (streamName.StartsWith(sTREAMPREFIX, StringComparison.OrdinalIgnoreCase))
			{
				string text = streamName.Substring(sTREAMPREFIX.Length);
				string[] array2 = text.Split(separator);
				if (array2.Length == 2)
				{
					int num = SPBProcessing.ParseInt(array2[0], 0);
					int num2 = SPBProcessing.ParseInt(array2[1], 0);
					if (num > -1 && num2 > -1)
					{
						SPBProcessing.FindImageConsolidation(report, num, num2, streamName, createAndRegisterStream);
						return true;
					}
				}
			}
			return false;
		}

		public static void GetDocumentMap(AspNetCore.ReportingServices.OnDemandReportRendering.Report report)
		{
			if (report.HasDocumentMap)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, false))
				{
					sPBProcessing.m_pageContext.CanTracePagination = true;
					sPBProcessing.SetContext(new SPBContext(0, 0, true));
					sPBProcessing.GetDocumentMap();
				}
			}
		}

		internal static int ParseInt(string intValue, int defaultValue)
		{
			int result = default(int);
			if (int.TryParse(intValue, out result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static int CompareWithOrdinalComparison(string x, string y, bool ignoreCase)
		{
			if (ignoreCase)
			{
				return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
			}
			return string.Compare(x, y, StringComparison.Ordinal);
		}

		private static void FindChart(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			if (uniqueName != null)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, false))
				{
					sPBProcessing.SetContext(new SPBContext(0, 0, false));
					sPBProcessing.FindChart(uniqueName, pageNumber, streamName, createAndRegisterStream);
				}
			}
		}

		private static void FindGaugePanel(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			if (uniqueName != null)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, false))
				{
					sPBProcessing.SetContext(new SPBContext(0, 0, false));
					sPBProcessing.FindGaugePanel(uniqueName, pageNumber, streamName, createAndRegisterStream);
				}
			}
		}

		private static void FindMap(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			if (uniqueName != null)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, false))
				{
					sPBProcessing.SetContext(new SPBContext(0, 0, false));
					sPBProcessing.FindMap(uniqueName, pageNumber, streamName, createAndRegisterStream);
				}
			}
		}

		private static void FindImage(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			if (uniqueName != null)
			{
				using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, false))
				{
					sPBProcessing.SetContext(new SPBContext(0, 0, false));
					sPBProcessing.FindImage(uniqueName, pageNumber, streamName, createAndRegisterStream);
				}
			}
		}

		private static void FindImageConsolidation(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, int pageNumber, int offset, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			using (SPBProcessing sPBProcessing = new SPBProcessing(report, 0, false))
			{
				sPBProcessing.SetContext(new SPBContext(0, 0, false));
				sPBProcessing.FindImageConsolidation(report.Name, pageNumber, offset, streamName, createAndRegisterStream);
			}
		}

		public static Dictionary<string, string> CollectBookmarks(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, int totalPages)
		{
			if (!report.HasBookmarks)
			{
				return null;
			}
			Dictionary<string, string> dictionary = null;
			using (SPBProcessing sPBProcessing = new SPBProcessing(report, totalPages, true))
			{
				sPBProcessing.m_pageContext.CanTracePagination = true;
				sPBProcessing.SetContext(new SPBContext(0, 0, false));
				return sPBProcessing.CollectBookmarks();
			}
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.m_pageContext.DisposeResources();
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
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void SetContext(SPBContext context)
		{
			int startPage = context.StartPage;
			int endPage = context.EndPage;
			bool measureItems = context.MeasureItems;
			bool emfDynamicImage = context.EmfDynamicImage;
			SecondaryStreams secondaryStreams = context.SecondaryStreams;
			bool addSecondaryStreamNames = context.AddSecondaryStreamNames;
			bool addToggledItems = context.AddToggledItems;
			bool addOriginalValue = context.AddOriginalValue;
			bool addFirstPageHeaderFooter = context.AddFirstPageHeaderFooter;
			bool flag = context.UseImageConsolidation;
			if (flag && (this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || this.m_pageContext.VersionPicker == RPLVersionEnum.RPLAccess))
			{
				flag = false;
			}
			if (startPage > endPage)
			{
				throw new ArgumentException(SPBRes.InvalidStartPageNumber);
			}
			if (endPage < -1)
			{
				throw new ArgumentException(SPBRes.InvalidEndPageNumber);
			}
			this.m_startPage = startPage;
			this.m_endPage = endPage;
			if (startPage == 0 || startPage == -1)
			{
				this.m_endPage = this.m_startPage;
			}
			if (this.m_rplState == RPLState.RPLStream)
			{
				if (RSTrace.RenderingTracer.TraceVerbose)
				{
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "RPL stream in use");
				}
			}
			else if (RSTrace.RenderingTracer.TraceVerbose)
			{
				RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "RPL object model in use");
			}
			if (secondaryStreams != 0)
			{
				addSecondaryStreamNames = true;
			}
			if (flag)
			{
				this.m_pageContext.ImageConsolidation = new ImageConsolidation(this.m_createAndRegisterStream);
			}
			this.m_pageContext.SetContext(measureItems, emfDynamicImage, secondaryStreams, addSecondaryStreamNames,
                addToggledItems, addOriginalValue, addFirstPageHeaderFooter, context.ConvertImages);
			this.m_report.SetContext(this.m_reportInfo);
			this.m_currentPage = 0;
			this.PaginateReport(startPage, endPage);
		}

		internal void SetContext(SPBContext context, bool createStream)
		{
			if (createStream)
			{
				this.m_rplState = RPLState.RPLStream;
			}
			this.SetContext(context);
		}

		public void UpdateRenderProperties(ref Hashtable renderProperties)
		{
			if (this.m_useInteractiveHeight)
			{
				if (renderProperties == null)
				{
					renderProperties = new Hashtable();
				}
				if (this.m_report.Done)
				{
					renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
					renderProperties["TotalPages"] = this.m_currentPage;
				}
				else if (this.m_totalPages > 0)
				{
					renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
					renderProperties["TotalPages"] = this.m_totalPages;
				}
				else
				{
					PaginationMode paginationMode = PaginationMode.Estimate;
					object obj = renderProperties["ClientPaginationMode"];
					if (obj != null)
					{
						paginationMode = (PaginationMode)obj;
					}
					object obj2 = renderProperties["PreviousTotalPages"];
					if (obj2 != null && obj2 is int)
					{
						this.m_totalPages = (int)obj2;
					}
					if (paginationMode == PaginationMode.TotalPages)
					{
						if (this.m_reportInfo.IsDone)
						{
							this.m_totalPages = this.m_reportInfo.PaginatedPages;
							renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
							renderProperties["TotalPages"] = this.m_totalPages;
						}
						else if (this.m_totalPages <= 0)
						{
							this.PaginateReport(-1, -1);
							renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
							renderProperties["TotalPages"] = this.m_totalPages;
						}
					}
					else if (this.m_totalPages <= 0 && this.m_currentPage + 1 + this.m_totalPages > 0)
					{
						renderProperties["UpdatedPaginationMode"] = PaginationMode.Estimate;
						renderProperties["TotalPages"] = this.m_currentPage + 1;
					}
				}
			}
		}

		public Stream GetNextPage()
		{
			return this.GetNextPage(false);
		}

		public Stream GetNextPage(bool collectPageBookmarks)
		{
			RPLReport rPLReport = null;
			return this.GetNextPage(out rPLReport, collectPageBookmarks);
		}

		public void GetNextPage(Stream outputStream)
		{
			this.GetNextPage(outputStream, false);
		}

		public void GetNextPage(Stream outputStream, bool collectPageBookmarks)
		{
			if (!this.m_report.Done)
			{
				RPLWriter reportNextPage = this.GetReportNextPage(ref outputStream, collectPageBookmarks);
				this.FlushRPLWriter(ref reportNextPage);
			}
		}

		public Stream GetNextPage(out RPLReport rplReport)
		{
			return this.GetNextPage(out rplReport, false);
		}

		public Stream GetNextPage(out RPLReport rplReport, bool collectPageBookmarks)
		{
			rplReport = null;
			if (this.m_report.Done)
			{
				return null;
			}
			Stream stream = null;
			RPLWriter reportNextPage = this.GetReportNextPage(ref stream, collectPageBookmarks);
			if (reportNextPage != null)
			{
				if (reportNextPage.BinaryWriter != null)
				{
					this.FlushRPLWriter(ref reportNextPage);
					BufferedStream input = new BufferedStream(stream);
					BinaryReader reader = new BinaryReader(input, Encoding.Unicode);
					rplReport = new RPLReport(reader);
				}
				else
				{
					rplReport = reportNextPage.Report;
				}
			}
			return stream;
		}

		private RPLWriter GetReportNextPage(ref Stream stream, bool collectPageBookmarks)
		{
			RPLWriter rPLWriter = null;
			Interactivity interactivity = null;
			bool flag = false;
			this.m_currentPage++;
			this.m_pageContext.PageBookmarks = null;
			if (this.m_reportInfo.IsDone && this.m_currentPage > this.m_reportInfo.PaginatedPages)
			{
				return null;
			}
			if (this.m_startPage >= 0 && this.m_endPage >= 0 && (this.m_endPage == 0 || (this.m_currentPage >= this.m_startPage && this.m_currentPage <= this.m_endPage)))
			{
				rPLWriter = new RPLWriter();
				if (this.m_rplState == RPLState.RPLStream && stream == null)
				{
					string name = this.m_currentPage.ToString(CultureInfo.InvariantCulture);
					stream = this.m_createAndRegisterStream(name, "spbif", null, null, true, StreamOper.CreateOnly);
				}
				if (stream != null)
				{
					this.CreateWriter(rPLWriter, stream);
				}
				else if (this.m_rplState == RPLState.Unknown)
				{
					if (this.m_lastPageInfoForCancel == null)
					{
						this.m_lastPageInfoForCancel = this.m_lastPageInfo;
					}
					this.m_pageContext.InitCancelPage(this.m_report.InteractiveHeight);
				}
				if (this.m_useInteractiveHeight)
				{
					this.m_report.LoadInteractiveChunks(this.m_currentPage);
					if (this.m_report.RegisterPageForCollect(this.m_currentPage, collectPageBookmarks))
					{
						interactivity = new Interactivity();
					}
					if (this.m_endPage > 0 && this.m_currentPage == this.m_endPage)
					{
						flag = true;
					}
				}
			}
			bool flag2 = this.m_useInteractiveHeight && this.m_reportInfo.HasPaginationInfoStream();
			this.m_report.NextPage(rPLWriter, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, interactivity, true);
			if (this.m_rplState == RPLState.Unknown)
			{
				if (stream == null)
				{
					bool cancelPage = this.m_pageContext.CancelPage;
					this.m_pageContext.ResetCancelPage();
					if (cancelPage)
					{
						if (RSTrace.RenderingTracer.TraceVerbose)
						{
							RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "Switch to RPL stream ");
						}
						rPLWriter = new RPLWriter();
						string name2 = this.m_currentPage.ToString(CultureInfo.InvariantCulture);
						stream = this.m_createAndRegisterStream(name2, "spbif", null, null, true, StreamOper.CreateOnly);
						this.CreateWriter(rPLWriter, stream);
						this.m_report.SetContext(this.m_reportInfo);
						this.m_report.ResetSectionsOnPage();
						this.m_report.NextPage(rPLWriter, ref this.m_lastPageInfoForCancel, this.m_currentPage, this.m_totalPages, interactivity, true);
					}
				}
				this.m_lastPageInfoForCancel = this.m_report.GetPaginationInfo();
			}
			if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
			{
				this.m_report.UpdatePagination();
			}
			else if (!flag2)
			{
				this.m_report.ResetLastSection();
			}
			if (interactivity != null)
			{
				this.m_report.UnregisterPageForCollect();
			}
			if (this.m_report.Done || flag)
			{
				if (this.m_report.Done)
				{
					this.m_pageContext.PageTotalInfo.CalculationDone = true;
				}
				this.m_report.UnloadInteractiveChunks();
				this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
			}
			this.m_report.DisposeDelayTextBox();
			return rPLWriter;
		}

		private void CreateWriter(RPLWriter rplWriter, Stream stream)
		{
			RSTrace.RenderingTracer.Assert(rplWriter != null, "Writer is null");
			RSTrace.RenderingTracer.Assert(stream != null, "Stream is null");
			BufferedStream output = new BufferedStream(stream);
			BinaryWriter binaryWriter2 = rplWriter.BinaryWriter = new BinaryWriter(output, Encoding.Unicode);
			binaryWriter2.Write("RPLIF");
			this.WriteVersionStamp(binaryWriter2, this.m_report.RPLVersion);
		}

		private RPLWriter FlushRPLWriter(ref RPLWriter rplWriter)
		{
			if (rplWriter == null)
			{
				return null;
			}
			if (rplWriter.BinaryWriter != null)
			{
				this.WriteVersionStamp(rplWriter.BinaryWriter, this.m_report.RPLVersion);
				rplWriter.BinaryWriter.Flush();
			}
			rplWriter = null;
			return rplWriter;
		}

		private void WriteVersionStamp(BinaryWriter sbpWriter, Version rplVersion)
		{
			sbpWriter.Write((byte)rplVersion.Major);
			sbpWriter.Write((byte)rplVersion.Minor);
			sbpWriter.Write(rplVersion.Build);
		}

		private void PaginateReport(int startPage, int endPage)
		{
			if (startPage > endPage || endPage < -1)
			{
				this.m_lastPageInfo = null;
			}
			else
			{
				this.m_startPage = startPage;
				this.m_endPage = endPage;
				bool hasPaginationChunk = this.m_useInteractiveHeight && this.m_reportInfo.HasPaginationInfoStream();
				if (startPage == 0 || startPage == -1)
				{
					this.m_endPage = this.m_startPage;
				}
				if (this.m_startPage > this.m_reportInfo.PaginatedPages)
				{
					if (this.m_reportInfo.PaginatedPages >= 0)
					{
						this.m_currentPage = this.m_reportInfo.PaginatedPages;
						if (!this.m_reportInfo.IsDone)
						{
							this.m_reportInfo.ReadPageInfo(this.m_currentPage, out this.m_lastPageInfo);
							while (!this.m_report.Done && this.m_currentPage < this.m_startPage - 1)
							{
								this.m_currentPage++;
								if (RSTrace.RenderingTracer.TraceVerbose)
								{
									RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We paginate the page: " + this.m_currentPage.ToString(CultureInfo.InvariantCulture));
								}
								this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, (Interactivity)null, hasPaginationChunk);
								if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
								{
									this.m_report.UpdatePagination();
								}
							}
							if (this.m_report.Done)
							{
								this.m_totalPages = this.m_currentPage;
								this.m_pageContext.PageTotalInfo.CalculationDone = true;
								this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
							}
							else
							{
								this.m_reportInfo.ReadPageInfo(this.m_currentPage, out this.m_lastPageInfo);
							}
						}
					}
					else if (-1 == this.m_reportInfo.PaginatedPages && this.m_startPage > 0)
					{
						this.m_currentPage = 0;
						while (!this.m_report.Done && this.m_currentPage < this.m_startPage - 1)
						{
							this.m_currentPage++;
							if (RSTrace.RenderingTracer.TraceVerbose)
							{
								RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We paginate the page: " + this.m_currentPage.ToString(CultureInfo.InvariantCulture));
							}
							this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, (Interactivity)null, hasPaginationChunk);
						}
						if (this.m_report.Done)
						{
							this.m_pageContext.PageTotalInfo.CalculationDone = true;
							this.m_totalPages = this.m_currentPage;
						}
					}
				}
				else if (this.m_endPage > this.m_reportInfo.PaginatedPages)
				{
					this.m_currentPage = this.m_startPage - 1;
					this.m_reportInfo.ReadPageInfo(this.m_currentPage, out this.m_lastPageInfo);
				}
				else if (this.m_startPage == -1)
				{
					if (this.m_reportInfo.PaginatedPages >= 0)
					{
						if (!this.m_reportInfo.IsDone)
						{
							this.m_currentPage = this.m_reportInfo.PaginatedPages;
							this.m_reportInfo.ReadPageInfo(this.m_currentPage, out this.m_lastPageInfo);
							while (!this.m_report.Done)
							{
								this.m_currentPage++;
								if (RSTrace.RenderingTracer.TraceVerbose)
								{
									RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We paginate the page: " + this.m_currentPage.ToString(CultureInfo.InvariantCulture));
								}
								this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, (Interactivity)null, hasPaginationChunk);
								if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
								{
									this.m_report.UpdatePagination();
								}
							}
							this.m_pageContext.PageTotalInfo.CalculationDone = true;
							this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
							this.m_totalPages = this.m_currentPage;
						}
					}
					else if (this.m_reportInfo.PaginatedPages == -1)
					{
						this.m_currentPage = 0;
						while (!this.m_report.Done)
						{
							this.m_currentPage++;
							if (RSTrace.RenderingTracer.TraceVerbose)
							{
								RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We paginate the page: " + this.m_currentPage.ToString(CultureInfo.InvariantCulture));
							}
							this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, (Interactivity)null, hasPaginationChunk);
						}
						this.m_pageContext.PageTotalInfo.CalculationDone = true;
						this.m_totalPages = this.m_currentPage;
					}
				}
				else if (this.m_startPage > 0)
				{
					this.m_currentPage = this.m_startPage - 1;
					this.m_reportInfo.ReadPageInfo(this.m_currentPage, out this.m_lastPageInfo);
				}
			}
		}

		private void InitializeForInteractiveRenderer(OnDemandReportRendering.Report report, CreateAndRegisterStream createAndRegisterStream, bool registerEvents, ref Hashtable renderProperties)
		{
			this.m_createAndRegisterStream = createAndRegisterStream;
			this.m_useInteractiveHeight = true;
			if (report.SnapshotPageSizeInfo == OnDemandReportRendering.Report.SnapshotPageSize.Large)
			{
				this.m_rplState = RPLState.RPLStream;
			}
			else if (report.SnapshotPageSizeInfo == OnDemandReportRendering.Report.SnapshotPageSize.Small)
			{
				this.m_rplState = RPLState.RPLObjectModel;
			}
			if (renderProperties != null)
			{
				object obj = renderProperties["ClientPaginationMode"];
				if (obj != null)
				{
					PaginationMode paginationMode = (PaginationMode)obj;
					if (paginationMode == PaginationMode.TotalPages)
					{
						object obj2 = renderProperties["PreviousTotalPages"];
						if (obj2 != null && obj2 is int)
						{
							this.m_totalPages = (int)obj2;
						}
					}
				}
			}
			if (this.m_totalPages <= 0)
			{
				if (this.m_reportInfo.IsDone)
				{
					this.m_totalPages = this.m_reportInfo.PaginatedPages;
					if (renderProperties == null)
					{
						renderProperties = new Hashtable();
					}
					renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
					renderProperties["TotalPages"] = this.m_totalPages;
				}
				else
				{
					this.m_totalPages = 0;
					if (report.NeedsTotalPages)
					{
						this.SetContext(new SPBContext());
						if (renderProperties == null)
						{
							renderProperties = new Hashtable();
						}
						renderProperties["UpdatedPaginationMode"] = PaginationMode.TotalPages;
						renderProperties["TotalPages"] = this.m_totalPages;
					}
				}
			}
		}

		internal int FindString(int startPage, int endPage, string findValue)
		{
			int result = 0;
			Interactivity interactivity = null;
			bool hasPaginationChunk = this.m_useInteractiveHeight && this.m_reportInfo.HasPaginationInfoStream();
			if (startPage <= endPage)
			{
				while (!this.m_report.Done)
				{
					this.m_currentPage++;
					interactivity = null;
					if (this.m_currentPage >= startPage && this.m_currentPage <= endPage)
					{
						interactivity = new Interactivity(findValue, Interactivity.EventType.FindStringEvent);
					}
					this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, interactivity, hasPaginationChunk);
					if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
					{
						this.m_report.UpdatePagination();
					}
					if (interactivity != null && interactivity.Done)
					{
						if (this.m_report.Done)
						{
							this.m_pageContext.PageTotalInfo.CalculationDone = true;
						}
						this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
						return this.m_currentPage;
					}
				}
			}
			else
			{
				int num = 0;
				bool flag = false;
				while (!this.m_report.Done)
				{
					this.m_currentPage++;
					interactivity = null;
					if (this.m_currentPage <= endPage || this.m_currentPage >= startPage)
					{
						interactivity = new Interactivity(findValue, Interactivity.EventType.FindStringEvent);
					}
					this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, interactivity, hasPaginationChunk);
					if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
					{
						this.m_report.UpdatePagination();
					}
					if (interactivity != null && interactivity.Done)
					{
						if (this.m_currentPage > endPage)
						{
							if (this.m_report.Done)
							{
								this.m_pageContext.PageTotalInfo.CalculationDone = true;
							}
							this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
							return this.m_currentPage;
						}
						if (!flag)
						{
							num = this.m_currentPage;
							flag = true;
						}
					}
				}
				result = num;
			}
			this.m_pageContext.PageTotalInfo.CalculationDone = true;
			this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
			return result;
		}

		internal int FindUserSort(string textbox, ref int numberOfPages, ref PaginationMode paginationMode)
		{
			int result = 0;
			Interactivity interactivity = new Interactivity(textbox, Interactivity.EventType.UserSortEvent);
			bool hasPaginationChunk = this.m_useInteractiveHeight && this.m_reportInfo.HasPaginationInfoStream();
			while (!this.m_report.Done)
			{
				this.m_currentPage++;
				this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, interactivity, hasPaginationChunk);
				if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
				{
					this.m_report.UpdatePagination();
				}
				if (interactivity.Done)
				{
					result = this.m_currentPage;
					break;
				}
			}
			if (paginationMode == PaginationMode.TotalPages)
			{
				while (!this.m_report.Done)
				{
					this.m_currentPage++;
					this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, (Interactivity)null, hasPaginationChunk);
					if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
					{
						this.m_report.UpdatePagination();
					}
				}
			}
			if (this.m_report.Done)
			{
				this.m_pageContext.PageTotalInfo.CalculationDone = true;
				paginationMode = PaginationMode.TotalPages;
				numberOfPages = this.m_reportInfo.PaginatedPages;
			}
			else
			{
				numberOfPages = this.m_currentPage;
			}
			this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
			return result;
		}

		private void FindItem(Interactivity interactivityContext, int pageNumber)
		{
			RSTrace.RenderingTracer.Assert(interactivityContext != null, "The interactivity context is null.");
			bool hasPaginationChunk = this.m_useInteractiveHeight && this.m_reportInfo.HasPaginationInfoStream();
			while (!this.m_report.Done)
			{
				this.m_currentPage++;
				if (this.m_currentPage < pageNumber)
				{
					this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, (Interactivity)null, hasPaginationChunk);
				}
				else
				{
					this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, interactivityContext, hasPaginationChunk);
				}
				if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
				{
					this.m_report.UpdatePagination();
				}
				if (interactivityContext.Done || (this.m_currentPage == pageNumber && interactivityContext.InteractivityEventType == Interactivity.EventType.ImageConsolidation))
				{
					if (this.m_report.Done)
					{
						this.m_pageContext.PageTotalInfo.CalculationDone = true;
					}
					this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
					return;
				}
			}
			this.m_pageContext.PageTotalInfo.CalculationDone = true;
			this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
		}

		internal void FindImageConsolidation(string reportName, int pageNumber, int offset, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			this.m_pageContext.ImageConsolidation = new ImageConsolidation(createAndRegisterStream, offset);
			this.m_pageContext.ImageConsolidation.SetName(reportName, pageNumber);
			Interactivity interactivityContext = new Interactivity(null, Interactivity.EventType.ImageConsolidation, streamName, createAndRegisterStream);
			this.FindItem(interactivityContext, pageNumber);
			this.m_pageContext.ImageConsolidation.RenderToStream();
		}

		internal void FindChart(string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			Interactivity interactivityContext = new Interactivity(uniqueName, Interactivity.EventType.FindChart, streamName, createAndRegisterStream);
			this.FindItem(interactivityContext, pageNumber);
		}

		internal void FindGaugePanel(string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			Interactivity interactivityContext = new Interactivity(uniqueName, Interactivity.EventType.FindGaugePanel, streamName, createAndRegisterStream);
			this.FindItem(interactivityContext, pageNumber);
		}

		internal void FindMap(string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			Interactivity interactivityContext = new Interactivity(uniqueName, Interactivity.EventType.FindMap, streamName, createAndRegisterStream);
			this.FindItem(interactivityContext, pageNumber);
		}

		internal void FindImage(string uniqueName, int pageNumber, string streamName, CreateAndRegisterStream createAndRegisterStream)
		{
			Interactivity interactivityContext = new Interactivity(uniqueName, Interactivity.EventType.FindImage, streamName, createAndRegisterStream);
			this.FindItem(interactivityContext, pageNumber);
		}

		internal string FindDrillthrough(string drillthroughId, int lastPageCollected, out NameValueCollection parameters)
		{
			string result = null;
			parameters = null;
			Interactivity interactivity = new Interactivity(drillthroughId, Interactivity.EventType.DrillthroughEvent);
			bool hasPaginationChunk = this.m_useInteractiveHeight && this.m_reportInfo.HasPaginationInfoStream();
			while (!this.m_report.Done)
			{
				this.m_currentPage++;
				if (this.m_currentPage <= lastPageCollected)
				{
					this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, (Interactivity)null, hasPaginationChunk);
					if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
					{
						this.m_report.UpdatePagination();
					}
				}
				else
				{
					this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, interactivity, hasPaginationChunk);
					if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
					{
						this.m_report.UpdatePagination();
					}
					if (interactivity.Done)
					{
						if (interactivity.DrillthroughResult != null)
						{
							result = interactivity.DrillthroughResult.ReportName;
							parameters = interactivity.DrillthroughResult.Parameters;
						}
						if (this.m_report.Done)
						{
							this.m_pageContext.PageTotalInfo.CalculationDone = true;
						}
						this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
						return result;
					}
				}
			}
			this.m_pageContext.PageTotalInfo.CalculationDone = true;
			this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
			return result;
		}

		internal int FindBookmark(string bookmarkId, int lastPageCollected, ref string uniqueName)
		{
			int result = 0;
			Interactivity interactivity = new Interactivity(bookmarkId);
			bool hasPaginationChunk = this.m_useInteractiveHeight && this.m_reportInfo.HasPaginationInfoStream();
			while (!this.m_report.Done)
			{
				this.m_currentPage++;
				if (this.m_currentPage <= lastPageCollected)
				{
					this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, (Interactivity)null, hasPaginationChunk);
					if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
					{
						this.m_report.UpdatePagination();
					}
				}
				else
				{
					this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, interactivity, hasPaginationChunk);
					if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
					{
						this.m_report.UpdatePagination();
					}
					if (interactivity.Done)
					{
						uniqueName = interactivity.ItemInfo;
						if (this.m_report.Done)
						{
							this.m_pageContext.PageTotalInfo.CalculationDone = true;
						}
						this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
						return this.m_currentPage;
					}
				}
			}
			this.m_pageContext.PageTotalInfo.CalculationDone = true;
			this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
			return result;
		}

		internal Dictionary<string, string> CollectBookmarks()
		{
			Interactivity interactivity = new Interactivity();
			this.m_pageContext.PageBookmarks = new Dictionary<string, string>();
			bool hasPaginationChunk = this.m_useInteractiveHeight && this.m_reportInfo.HasPaginationInfoStream();
			while (!this.m_report.Done)
			{
				this.m_currentPage++;
				this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, interactivity, hasPaginationChunk);
				if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
				{
					this.m_report.UpdatePagination();
				}
			}
			this.m_pageContext.PageTotalInfo.CalculationDone = true;
			this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
			return this.m_pageContext.PageBookmarks;
		}

		internal int FindDocumentMap(string documentMapId, int lastPageCollected)
		{
			int result = 0;
			Interactivity interactivity = new Interactivity(documentMapId, Interactivity.EventType.DocumentMapNavigationEvent);
			bool hasPaginationChunk = this.m_useInteractiveHeight && this.m_reportInfo.HasPaginationInfoStream();
			while (!this.m_report.Done)
			{
				this.m_currentPage++;
				if (this.m_currentPage <= lastPageCollected)
				{
					this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, (Interactivity)null, hasPaginationChunk);
					if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
					{
						this.m_report.UpdatePagination();
					}
				}
				else
				{
					this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, interactivity, hasPaginationChunk);
					if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
					{
						this.m_report.UpdatePagination();
					}
					if (interactivity.Done)
					{
						if (this.m_report.Done)
						{
							this.m_pageContext.PageTotalInfo.CalculationDone = true;
						}
						this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
						return this.m_currentPage;
					}
				}
			}
			this.m_pageContext.PageTotalInfo.CalculationDone = true;
			this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
			return result;
		}

		internal void GetDocumentMap()
		{
			bool hasPaginationChunk = this.m_useInteractiveHeight && this.m_reportInfo.HasPaginationInfoStream();
			Interactivity interactivity = new Interactivity(Interactivity.EventType.GetDocumentMap);
			this.m_report.LoadLabelsChunk();
			while (!this.m_report.Done)
			{
				this.m_currentPage++;
				this.m_report.RegisterPageLabelsForCollect(this.m_currentPage);
				this.m_report.NextPage((RPLWriter)null, ref this.m_lastPageInfo, this.m_currentPage, this.m_totalPages, interactivity, hasPaginationChunk);
				if (this.m_currentPage == this.m_reportInfo.PaginatedPages + 1)
				{
					this.m_report.UpdatePagination();
				}
				this.m_report.UnregisterPageForCollect();
			}
			this.m_report.UnloadInteractiveChunks();
			this.m_pageContext.PageTotalInfo.CalculationDone = true;
			this.m_reportInfo.SavePaginationMetadata(this.m_report.Done, this.m_pageContext.PageTotalInfo);
		}
	}
}
