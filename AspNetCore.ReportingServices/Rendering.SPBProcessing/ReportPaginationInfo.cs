using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class ReportPaginationInfo
	{
		internal const string PaginationInfoChunk = "PaginationInfo";

		private Version m_version = new Version(0, 0, 0);

		private int m_paginatedPages = -1;

		private Stream m_stream;

		private BinaryReader m_reader;

		private BinaryWriter m_writer;

		private List<long> m_metadataPages;

		private long m_offsetLastPage = -1L;

		private long m_regionPageTotalInfoOffset = -1L;

		private long m_offsetHeader = -1L;

		private bool m_reportDone;

		private double m_pageHeight;

		private bool m_newPagesMetadata;

		internal Version Version
		{
			get
			{
				return this.m_version;
			}
		}

		internal int PaginatedPages
		{
			get
			{
				return this.m_paginatedPages;
			}
		}

		internal long OffsetLastPage
		{
			get
			{
				return this.m_offsetLastPage;
			}
		}

		internal BinaryWriter BinaryWriter
		{
			get
			{
				if (this.m_stream != null)
				{
					if (this.m_writer == null)
					{
						this.m_writer = new BinaryWriter(this.m_stream, Encoding.Unicode);
					}
					return this.m_writer;
				}
				return null;
			}
		}

		internal BinaryReader BinaryReader
		{
			get
			{
				if (this.m_stream != null)
				{
					if (this.m_reader == null)
					{
						this.m_reader = new BinaryReader(this.m_stream, Encoding.Unicode);
					}
					return this.m_reader;
				}
				return null;
			}
		}

		internal bool IsDone
		{
			get
			{
				return this.m_reportDone;
			}
		}

		internal ReportPaginationInfo()
		{
			this.m_stream = null;
			this.m_version = new Version(0, 0, 0);
			this.m_paginatedPages = -1;
			this.m_offsetLastPage = -1L;
			this.m_offsetHeader = -1L;
			this.m_pageHeight = 0.0;
			this.m_metadataPages = null;
			this.m_reportDone = false;
		}

		internal ReportPaginationInfo(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, Version serverRPLVersion, double pageHeight)
		{
			bool flag = false;
			this.m_stream = report.GetOrCreateChunk(AspNetCore.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Pagination, "PaginationInfo", out flag);
			if (this.m_stream != null)
			{
				if (flag)
				{
					this.m_version = serverRPLVersion;
					this.m_pageHeight = pageHeight;
					this.SavePaginationInfo();
					this.m_paginatedPages = 0;
					this.m_metadataPages = new List<long>();
					this.m_reportDone = false;
				}
				else if (this.ExtractPaginationInfo(serverRPLVersion))
				{
					this.m_stream.Close();
					this.m_stream = report.CreateChunk(AspNetCore.ReportingServices.OnDemandReportRendering.Report.ChunkTypes.Pagination, "PaginationInfo");
					this.m_version = serverRPLVersion;
					this.m_pageHeight = pageHeight;
					this.SavePaginationInfo();
					this.m_paginatedPages = 0;
					this.m_metadataPages = new List<long>();
					this.m_reportDone = false;
				}
				else if (pageHeight != this.m_pageHeight)
				{
					this.m_stream = null;
					this.m_version = new Version(0, 0, 0);
					this.m_paginatedPages = -1;
					this.m_offsetLastPage = -1L;
					this.m_offsetHeader = -1L;
					this.m_pageHeight = 0.0;
					this.m_metadataPages = null;
					this.m_reportDone = false;
				}
			}
		}

		internal bool HasPaginationInfoStream()
		{
			Version value = new Version(0, 0, 0);
			if (this.m_version.CompareTo(value) == 0)
			{
				return false;
			}
			return true;
		}

		internal void ReadPageInfo(int page, out ReportSectionHelper pageInfo)
		{
			RSTrace.RenderingTracer.Assert(this.m_stream != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(this.m_paginatedPages >= 0, "The number of paginated pages is negative.");
			RSTrace.RenderingTracer.Assert(page >= 0 && page <= this.m_paginatedPages, "The number of the solicited page is outside of the interval: 0 - " + this.m_paginatedPages.ToString(CultureInfo.InvariantCulture) + ".");
			pageInfo = null;
			switch (page)
			{
			case 0:
				break;
			case 1:
			{
				this.BinaryReader.BaseStream.Seek(this.m_offsetHeader, SeekOrigin.Begin);
				long offsetEndPage2 = this.m_metadataPages[0];
				if (RSTrace.RenderingTracer.TraceVerbose)
				{
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We extract from stream the page: " + page.ToString(CultureInfo.InvariantCulture));
				}
				pageInfo = ReportSectionHelper.ReadReportSection(this.BinaryReader, offsetEndPage2);
				break;
			}
			default:
			{
				long offset = this.m_metadataPages[page - 2];
				long offsetEndPage = this.m_metadataPages[page - 1];
				this.BinaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
				if (RSTrace.RenderingTracer.TraceVerbose)
				{
					RSTrace.RenderingTracer.Trace(TraceLevel.Verbose, "We extract from stream the page: " + page.ToString(CultureInfo.InvariantCulture));
				}
				pageInfo = ReportSectionHelper.ReadReportSection(this.BinaryReader, offsetEndPage);
				break;
			}
			}
		}

		internal void ReadRegionPageTotalInfo(PageTotalInfo pageTotalInfo)
		{
			if (this.m_regionPageTotalInfoOffset > 0)
			{
				this.BinaryReader.BaseStream.Seek(this.m_regionPageTotalInfoOffset, SeekOrigin.Begin);
				if (this.BinaryReader.ReadBoolean())
				{
					bool isCalculationDone = this.BinaryReader.ReadBoolean();
					bool isCounting = this.BinaryReader.ReadBoolean();
					int num = this.BinaryReader.ReadInt32();
					List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>(num);
					for (int i = 0; i < num; i++)
					{
						int key = this.BinaryReader.ReadInt32();
						string value = this.BinaryReader.ReadString();
						list.Add(new KeyValuePair<int, string>(key, value));
					}
					num = this.BinaryReader.ReadInt32();
					List<KeyValuePair<int, int>> list2 = new List<KeyValuePair<int, int>>(num);
					for (int j = 0; j < num; j++)
					{
						int key2 = this.BinaryReader.ReadInt32();
						int value2 = this.BinaryReader.ReadInt32();
						list2.Add(new KeyValuePair<int, int>(key2, value2));
					}
					pageTotalInfo.SetupPageTotalInfo(isCalculationDone, isCounting, list2, list);
				}
			}
		}

		internal void SavePaginationMetadata(bool reportDone, PageTotalInfo pageTotalInfo)
		{
			if (this.m_stream != null && !this.m_reportDone && this.m_newPagesMetadata)
			{
				this.m_reportDone = reportDone;
				this.BinaryWriter.BaseStream.Seek(this.m_offsetLastPage, SeekOrigin.Begin);
				this.BinaryWriter.Write(this.m_reportDone);
				this.BinaryWriter.Write(this.m_metadataPages.Count);
				for (int i = 0; i < this.m_metadataPages.Count; i++)
				{
					this.BinaryWriter.Write(this.m_metadataPages[i]);
				}
				this.m_regionPageTotalInfoOffset = this.BinaryWriter.BaseStream.Position;
				if (pageTotalInfo == null)
				{
					this.BinaryWriter.Write(false);
				}
				else
				{
					this.BinaryWriter.Write(true);
					this.BinaryWriter.Write(pageTotalInfo.CalculationDone);
					this.BinaryWriter.Write(pageTotalInfo.IsCounting);
					List<KeyValuePair<int, string>> pageNameList = pageTotalInfo.GetPageNameList();
					this.BinaryWriter.Write(pageNameList.Count);
					foreach (KeyValuePair<int, string> item in pageNameList)
					{
						this.BinaryWriter.Write(item.Key);
						this.BinaryWriter.Write(item.Value);
					}
					List<KeyValuePair<int, int>> pageNumberList = pageTotalInfo.GetPageNumberList();
					this.BinaryWriter.Write(pageNumberList.Count);
					foreach (KeyValuePair<int, int> item2 in pageNumberList)
					{
						this.BinaryWriter.Write(item2.Key);
						this.BinaryWriter.Write(item2.Value);
					}
				}
				this.BinaryWriter.Write(this.m_regionPageTotalInfoOffset);
				this.BinaryWriter.Flush();
				this.m_newPagesMetadata = false;
			}
		}

		internal void UpdateReportInfo()
		{
			if (this.m_stream != null)
			{
				this.m_metadataPages.Add(this.BinaryWriter.BaseStream.Position);
				this.m_paginatedPages++;
				this.m_offsetLastPage = this.BinaryWriter.BaseStream.Position;
				this.m_newPagesMetadata = true;
			}
		}

		private void SavePaginationInfo()
		{
			RSTrace.RenderingTracer.Assert(this.m_stream != null, "The pagination stream is null.");
			this.BinaryWriter.BaseStream.Seek(0L, SeekOrigin.Begin);
			this.BinaryWriter.Write("RPLIF");
			this.BinaryWriter.Write((byte)this.m_version.Major);
			this.BinaryWriter.Write((byte)this.m_version.Minor);
			this.BinaryWriter.Write(this.m_version.Build);
			this.BinaryWriter.Write(this.m_pageHeight);
			this.m_offsetHeader = this.BinaryWriter.BaseStream.Position;
			this.m_offsetLastPage = this.BinaryWriter.BaseStream.Position;
		}

		private bool ExtractPaginationInfo(Version serverRPLVersion)
		{
			RSTrace.RenderingTracer.Assert(this.m_stream != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(serverRPLVersion != (Version)null, "The version of the server shouldn't be null");
			this.BinaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
			this.BinaryReader.ReadString();
			this.m_version = new Version(this.BinaryReader.BaseStream.ReadByte(), this.BinaryReader.BaseStream.ReadByte(), this.BinaryReader.ReadInt32());
			if (this.NeedsNewPaginationInfoStream(serverRPLVersion))
			{
				return true;
			}
			this.m_pageHeight = this.BinaryReader.ReadDouble();
			this.m_offsetHeader = this.BinaryReader.BaseStream.Position;
			this.BinaryReader.BaseStream.Seek(-8L, SeekOrigin.End);
			this.m_regionPageTotalInfoOffset = this.BinaryReader.ReadInt64();
			this.BinaryReader.BaseStream.Seek(this.m_regionPageTotalInfoOffset - 8, SeekOrigin.Begin);
			this.m_offsetLastPage = this.BinaryReader.ReadInt64();
			if (this.m_offsetLastPage > 0)
			{
				this.BinaryReader.BaseStream.Seek(this.m_offsetLastPage, SeekOrigin.Begin);
				this.m_reportDone = this.BinaryReader.ReadBoolean();
				this.m_paginatedPages = this.BinaryReader.ReadInt32();
				this.m_metadataPages = new List<long>(this.m_paginatedPages);
				for (int i = 0; i < this.m_paginatedPages; i++)
				{
					long item = this.BinaryReader.ReadInt64();
					this.m_metadataPages.Add(item);
					if (i == 0 && this.m_metadataPages[i] <= this.m_offsetHeader)
					{
						throw new InvalidDataException(SPBRes.InvalidPaginationStream);
					}
				}
				this.BinaryReader.BaseStream.Seek(this.m_offsetLastPage, SeekOrigin.Begin);
				return false;
			}
			throw new InvalidDataException(SPBRes.InvalidPaginationStream);
		}

		private bool NeedsNewPaginationInfoStream(Version serverRPLVersion)
		{
			RSTrace.RenderingTracer.Assert(serverRPLVersion != (Version)null, "The version of the server shouldn't be null");
			int major = serverRPLVersion.Major;
			int minor = serverRPLVersion.Minor;
			int major2 = this.m_version.Major;
			int minor2 = this.m_version.Minor;
			if (major2 == major && minor2 == minor)
			{
				return false;
			}
			return true;
		}
	}
}
