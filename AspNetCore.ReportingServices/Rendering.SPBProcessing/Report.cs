using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Report
	{
		private AspNetCore.ReportingServices.OnDemandReportRendering.Report m_report;

		private ReportPaginationInfo m_reportInfo;

		private int m_lastSectionIndex = -1;

		private PageContext m_pageContext;

		private long m_offset;

		private long m_pageContentOffset;

		private long m_pageLayoutOffset;

		private long m_bodyOffset;

		private long m_columnsOffset;

		private List<ReportSection> m_sections;

		private List<ItemSizes> m_sectionSizes;

		private List<long> m_sectionOffsets;

		private DocumentMapLabels m_labels;

		private Bookmarks m_bookmarks;

		private bool m_chunksLoaded;

		private bool m_labelsChunkLoaded;

		private Version m_rplVersion;

		internal bool Done
		{
			get
			{
				if (this.m_sections.Count == 0)
				{
					return true;
				}
				if (this.m_lastSectionIndex == this.m_sections.Count - 1)
				{
					return this.m_sections[this.m_lastSectionIndex].Done;
				}
				return false;
			}
		}

		internal Version RPLVersion
		{
			get
			{
				return this.m_rplVersion;
			}
		}

		internal double InteractiveHeight
		{
			get
			{
				return ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)this.m_report.ReportSections)[0].Page.InteractiveHeight.ToMillimeters();
			}
		}

		internal IJobContext JobContext
		{
			get
			{
				return this.m_report.JobContext;
			}
		}

		internal Report(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, PageContext pageContext, string rplVersion, bool defaultVersion)
		{
			this.m_report = report;
			this.m_pageContext = pageContext;
			if (!string.IsNullOrEmpty(rplVersion))
			{
				char[] separator = new char[1]
				{
					'.'
				};
				string[] array = rplVersion.Split(separator);
				if (array.Length < 2)
				{
					this.m_rplVersion = new Version(10, 3, 0);
				}
				else
				{
					int major = SPBProcessing.ParseInt(array[0], 10);
					int minor = SPBProcessing.ParseInt(array[1], 3);
					int build = 0;
					if (array.Length > 2)
					{
						build = SPBProcessing.ParseInt(array[2], 0);
					}
					this.m_rplVersion = new Version(major, minor, build);
				}
			}
			else if (defaultVersion)
			{
				this.m_rplVersion = new Version(10, 3, 0);
			}
			else
			{
				this.m_rplVersion = new Version(10, 6, 0);
			}
			this.m_pageContext.VersionPicker = RPLReader.CompareRPLVersions(this.m_rplVersion);
		}

		internal void LoadInteractiveChunks(int page)
		{
			if (!this.m_chunksLoaded)
			{
				this.m_chunksLoaded = true;
				if (this.m_report.HasBookmarks)
				{
					this.m_bookmarks = InteractivityChunks.GetBookmarksStream(this.m_report, page);
				}
				if (!this.m_labelsChunkLoaded && this.m_report.HasDocumentMap)
				{
					this.m_labels = InteractivityChunks.GetLabelsStream(this.m_report, page);
				}
			}
		}

		internal void LoadLabelsChunk()
		{
			if (!this.m_chunksLoaded && !this.m_labelsChunkLoaded)
			{
				this.m_labelsChunkLoaded = true;
				if (this.m_report.HasDocumentMap)
				{
					this.m_labels = InteractivityChunks.GetLabelsStream(this.m_report, 1);
				}
			}
		}

		internal void UnloadInteractiveChunks()
		{
			if (this.m_bookmarks != null)
			{
				this.m_bookmarks.Flush(this.Done);
				this.m_bookmarks = null;
			}
			if (this.m_labels != null)
			{
				this.m_labels.Flush(this.Done);
				this.m_labels = null;
			}
			this.m_labelsChunkLoaded = false;
			this.m_chunksLoaded = false;
		}

		internal bool RegisterPageForCollect(int page, bool collectPageBookmarks)
		{
			bool flag = false;
			if (this.m_report.HasBookmarks)
			{
				if (this.m_bookmarks != null && page == this.m_bookmarks.Page + 1)
				{
					flag = true;
					this.m_bookmarks.Page = page;
					this.m_pageContext.Bookmarks = this.m_bookmarks;
				}
				if (collectPageBookmarks)
				{
					flag = true;
					this.m_pageContext.PageBookmarks = new Dictionary<string, string>();
				}
			}
			return flag | this.RegisterPageLabelsForCollect(page);
		}

		internal bool RegisterPageLabelsForCollect(int page)
		{
			bool result = false;
			if (this.m_report.HasDocumentMap && this.m_labels != null && page == this.m_labels.Page + 1)
			{
				result = true;
				this.m_labels.Page = page;
				this.m_pageContext.Labels = this.m_labels;
			}
			return result;
		}

		internal void UnregisterPageForCollect()
		{
			this.m_pageContext.Labels = null;
			this.m_pageContext.Bookmarks = null;
		}

		internal void DisposeDelayTextBox()
		{
			this.m_pageContext.Common.DisposeDelayTextBox();
		}

		internal void ResetSectionsOnPage()
		{
			if (this.m_sections != null)
			{
				this.ResetSectionsOnPage(0, this.m_sections.Count - 1);
			}
		}

		internal void SetContext(ReportPaginationInfo reportInfo)
		{
			if (this.m_sections == null)
			{
				int count = this.m_report.ReportSections.Count;
				this.m_sections = new List<ReportSection>(count);
				this.m_sectionOffsets = new List<long>(count);
				this.m_sectionSizes = new List<ItemSizes>(count);
				foreach (AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection reportSection in this.m_report.ReportSections)
				{
					ReportSection item = new ReportSection(reportSection, this.m_pageContext);
					this.m_sections.Add(item);
				}
			}
			this.m_reportInfo = reportInfo;
			this.m_lastSectionIndex = -1;
		}

		internal void NextPage(RPLWriter rplWriter, ref ReportSectionHelper lastPageInfo, int page, int totalPages, Interactivity interactivity, bool hasPaginationChunk)
		{
			ReportSection reportSection = null;
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = (byte)((this.m_sections.Count <= 1) ? 1 : 0) != 0;
			PageItemHelper pageItemHelper = null;
			int regionPageNumber = page;
			int regionTotalPages = totalPages;
			int num = -1;
			this.CreateFirstSectionBodyFromPaginationState(page, lastPageInfo, ref pageItemHelper, ref num);
			if (this.m_pageContext.ImageConsolidation != null)
			{
				this.m_pageContext.ImageConsolidation.Reset();
				this.m_pageContext.ImageConsolidation.SetName(this.m_report.Name, page);
			}  
			this.m_pageContext.PageNumber = page;
			this.m_pageContext.PageTotalInfo.RegisterPageNumberForStart(page);
			this.m_pageContext.PageTotalInfo.RetrievePageBreakData(page, out regionPageNumber, out regionTotalPages);
			this.WriteStartItemToStream(rplWriter);
			this.WriteReportPageLayoutAtStart(rplWriter);
			for (int i = num; i < this.m_sections.Count; i++)
			{
				ReportSection reportSection2 = this.m_sections[i];
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				if (flag)
				{
					reportSection = reportSection2;
				}
				if (i == this.m_sections.Count - 1)
				{
					flag4 = true;
				}
				if (reportSection2.Body == null)
				{
					reportSection2.SetContext();
				}
				reportSection2.CalculatePage(rplWriter, page, totalPages, regionPageNumber, regionTotalPages, flag, flag4, interactivity, this.m_pageContext.PageHeight, ref pageItemHelper, ref flag5, ref flag6, ref flag7);
				if (this.m_pageContext.CancelPage)
				{
					this.ResetSectionsOnPage(num, i);
					return;
				}
				if (flag)
				{
					flag = false;
					flag2 = flag5;
					lastPageInfo = null;
				}
				if (flag6)
				{
					flag3 = flag6;
				}
				this.m_sectionSizes.Add(reportSection2.ItemRenderSizes);
				this.m_sectionOffsets.Add(reportSection2.Offset);
				if (this.m_pageContext.PageHeight != 1.7976931348623157E+308)
				{
					this.m_pageContext.PageHeight -= reportSection2.Body.ItemPageSizes.Height;
				}
				if (flag7)
				{
					if (i > this.m_lastSectionIndex)
					{
						this.m_lastSectionIndex = i;
					}
					reportSection2.SectionIndex = i;
					break;
				}
			}
			if (this.m_pageContext.TracingEnabled)
			{
				if (this.Done)
				{
					Report.TracePageBreakIgnoredAtBottom(page, this.m_pageContext.Common.PageBreakInfo);
				}
				else if (this.m_pageContext.Common.PageBreakInfo != null)
				{
					Report.TraceLogicalPageBreak(page + 1, this.m_pageContext.Common.PageBreakInfo);
				}
				else
				{
					Report.TraceVerticalPageBreak(page + 1);
				}
			}
			this.m_pageContext.ApplyPageBreak(page);
			int num2 = 1;
			double bodyWidth = 0.0;
			double bodyHeight = 0.0;
			bool flag8 = flag4 && this.m_sections[this.m_lastSectionIndex].Done;
			if (this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
			{
				this.WriteEndReportBodyToRPLStream2008(rplWriter, num, ref bodyWidth, ref bodyHeight);
				if (rplWriter != null || (interactivity != null && !interactivity.Done && interactivity.NeedPageHeaderFooter))
				{
					this.WriteReportPageLayout2008(rplWriter, bodyWidth, bodyHeight);
					if (flag3)
					{
						this.m_sections[this.m_lastSectionIndex].CalculateDelayedFooter(rplWriter, interactivity);
						num2++;
					}
					if (flag2 && (page == 1 || !flag8 || reportSection.IsHeaderPrintOnLastPage()))
					{
						reportSection.CalculateDelayedHeader(rplWriter, interactivity);
						num2++;
					}
					if (rplWriter != null && rplWriter.BinaryWriter != null)
					{
						rplWriter.BinaryWriter.Write((byte)255);
					}
				}
			}
			else if (flag2)
			{
				if (!flag8)
				{
					this.CalculateDelayedHeader(rplWriter, reportSection, interactivity);
				}
				else
				{
					reportSection.WriteEndItemToStream(rplWriter);
					if (rplWriter != null)
					{
						this.m_sectionSizes[0].Height = reportSection.ItemRenderSizes.Height;
						this.m_sectionSizes[0].Width = reportSection.ItemRenderSizes.Width;
						this.m_sectionOffsets[0] = reportSection.Offset;
					}
				}
			}
			if (interactivity != null)
			{
				interactivity.RegisterDocMapRootLabel(this.m_report.Instance.UniqueName, this.m_pageContext);
			}
			if (this.m_pageContext.ImageConsolidation != null)
			{
				this.m_pageContext.ImageConsolidation.RenderToStream();
			}
			string pageName = this.m_pageContext.PageTotalInfo.GetPageName(page);
			this.WriteEndItemToStream(rplWriter, num, num2, reportSection.Header, this.m_sections[this.m_lastSectionIndex].Footer, bodyWidth, bodyHeight, pageName);
			this.m_pageContext.PageHeight = this.m_pageContext.OriginalPageHeight;
			if (hasPaginationChunk)
			{
				this.ReleaseResourcesOnPage(rplWriter, num, false);
			}
			else
			{
				this.ReleaseResourcesOnPage(rplWriter, num, true);
			}
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				string language = null;
				ReportStringProperty language2 = this.m_report.Language;
				if (language2 != null)
				{
					if (language2.IsExpression)
					{
						ReportInstance instance = this.m_report.Instance;
						language = instance.Language;
					}
					else
					{
						language = language2.Value;
					}
				}
				if (binaryWriter != null)
				{
					if (this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
					{
						this.WriteStartItemToRPLStream2008(binaryWriter, language);
					}
					else
					{
						this.WriteStartItemToRPLStream(binaryWriter, language);
					}
				}
				else
				{
					this.WriteStartItemToRPLOM(rplWriter, language);
				}
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter, int sectionStartIndex, int itemsOnPage, PageHeadFoot header, PageHeadFoot footer, double bodyWidth, double bodyHeight, string pageName)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					if (this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
					{
						this.WriteEndItemToRPLStream2008(binaryWriter, itemsOnPage, header, footer, bodyWidth, bodyHeight);
					}
					else
					{
						this.WriteEndItemToRPLStream(binaryWriter, pageName);
					}
				}
				else
				{
					this.WriteEndItemToRPLOM(rplWriter, sectionStartIndex, pageName);
				}
			}
		}

		internal void UpdatePagination()
		{
			if (this.m_sections != null && this.m_sections.Count > 0 && this.m_reportInfo.BinaryWriter != null)
			{
				this.m_reportInfo.BinaryWriter.BaseStream.Seek(this.m_reportInfo.OffsetLastPage, SeekOrigin.Begin);
				ReportSection reportSection = this.m_sections[this.m_lastSectionIndex];
				reportSection.WritePaginationInfo(this.m_reportInfo.BinaryWriter);
				this.m_reportInfo.UpdateReportInfo();
				this.m_sections[this.m_lastSectionIndex].Reset();
			}
		}

		internal void ResetLastSection()
		{
			if (this.m_sections != null && this.m_lastSectionIndex >= 0 && this.m_lastSectionIndex < this.m_sections.Count)
			{
				this.m_sections[this.m_lastSectionIndex].Reset();
			}
		}

		internal ReportSectionHelper GetPaginationInfo()
		{
			if (this.m_sections != null && this.m_sections.Count > 0 && this.m_lastSectionIndex >= 0 && this.m_lastSectionIndex < this.m_sections.Count)
			{
				return this.m_sections[this.m_lastSectionIndex].WritePaginationInfo();
			}
			return null;
		}

		private void CreateFirstSectionBodyFromPaginationState(int page, ReportSectionHelper lastPageInfo, ref PageItemHelper lastBodyInfo, ref int sectionStartIndex)
		{
			if (lastPageInfo != null)
			{
				sectionStartIndex = lastPageInfo.SectionIndex;
				this.m_sections[sectionStartIndex].UpdateItem(lastPageInfo);
				if (lastPageInfo.BodyHelper != null)
				{
					lastBodyInfo = lastPageInfo.BodyHelper;
				}
				else
				{
					sectionStartIndex++;
				}
			}
			else if (page == 1)
			{
				sectionStartIndex = 0;
				this.m_sections[sectionStartIndex].Reset();
			}
			else
			{
				RSTrace.RenderingTracer.Assert(this.m_lastSectionIndex >= 0 && this.m_lastSectionIndex < this.m_sections.Count, "The index of the last section on the previous paginated page should be a valid index");
				if (this.m_sections[this.m_lastSectionIndex].Done && this.m_lastSectionIndex < this.m_sections.Count - 1)
				{
					this.m_sections[this.m_lastSectionIndex].Reset();
					sectionStartIndex = this.m_lastSectionIndex + 1;
				}
				else
				{
					sectionStartIndex = this.m_lastSectionIndex;
					this.m_sections[sectionStartIndex].Reset();
				}
			}
		}

		private void CalculateDelayedHeader(RPLWriter rplWriter, ReportSection firstSection, Interactivity interactivity)
		{
			firstSection.CalculateDelayedHeader(rplWriter, interactivity);
			firstSection.UpdateReportSectionSizes(rplWriter);
			firstSection.WriteEndItemToStream(rplWriter);
			if (rplWriter != null)
			{
				this.m_sectionSizes[0].Height = firstSection.ItemRenderSizes.Height;
				this.m_sectionSizes[0].Width = firstSection.ItemRenderSizes.Width;
				this.m_sectionOffsets[0] = firstSection.Offset;
			}
		}

		private void ReleaseResourcesOnPage(RPLWriter rplWriter, int sectionStartIndex, bool includeLastSection)
		{
			if (includeLastSection)
			{
				this.ResetSectionsOnPage(sectionStartIndex, this.m_lastSectionIndex);
			}
			else
			{
				this.ResetSectionsOnPage(sectionStartIndex, this.m_lastSectionIndex - 1);
			}
			if (rplWriter != null && rplWriter.BinaryWriter != null)
			{
				this.m_pageContext.SharedImages = null;
				this.m_pageContext.ItemPropsStart = null;
			}
		}

		private void ResetSectionsOnPage(int sectionStartIndex, int sectionEndIndex)
		{
			for (int i = sectionStartIndex; i <= sectionEndIndex; i++)
			{
				this.m_sections[i].Reset();
			}
			this.m_sectionOffsets.Clear();
			this.m_sectionSizes.Clear();
		}

		private void WriteStartItemToRPLStream(BinaryWriter spbifWriter, string language)
		{
			Stream baseStream = spbifWriter.BaseStream;
			this.m_offset = baseStream.Position;
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)2);
			if (this.m_report.Name != null)
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write(this.m_report.Name);
			}
			if (this.m_report.Description != null)
			{
				spbifWriter.Write((byte)9);
				spbifWriter.Write(this.m_report.Description);
			}
			if (this.m_report.Author != null)
			{
				spbifWriter.Write((byte)13);
				spbifWriter.Write(this.m_report.Author);
			}
			if (this.m_report.AutoRefresh > 0)
			{
				spbifWriter.Write((byte)14);
				spbifWriter.Write(this.m_report.AutoRefresh);
			}
			DateTime executionTime = this.m_report.ExecutionTime;
			spbifWriter.Write((byte)12);
			spbifWriter.Write(executionTime.ToBinary());
			ReportUrl location = this.m_report.Location;
			if (location != null)
			{
				spbifWriter.Write((byte)10);
				spbifWriter.Write(location.ToString());
			}
			if (language != null)
			{
				spbifWriter.Write((byte)11);
				spbifWriter.Write(language);
			}
			if (this.m_report.ConsumeContainerWhitespace)
			{
				spbifWriter.Write((byte)50);
				spbifWriter.Write(this.m_report.ConsumeContainerWhitespace);
			}
			spbifWriter.Write((byte)255);
			this.m_pageContentOffset = baseStream.Position;
			spbifWriter.Write((byte)19);
		}

		private void WriteStartItemToRPLStream2008(BinaryWriter spbifWriter, string language)
		{
			Stream baseStream = spbifWriter.BaseStream;
			this.m_offset = baseStream.Position;
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)2);
			if (this.m_report.Name != null)
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write(this.m_report.Name);
			}
			if (this.m_report.Description != null)
			{
				spbifWriter.Write((byte)9);
				spbifWriter.Write(this.m_report.Description);
			}
			if (this.m_report.Author != null)
			{
				spbifWriter.Write((byte)13);
				spbifWriter.Write(this.m_report.Author);
			}
			if (this.m_report.AutoRefresh > 0)
			{
				spbifWriter.Write((byte)14);
				spbifWriter.Write(this.m_report.AutoRefresh);
			}
			DateTime executionTime = this.m_report.ExecutionTime;
			spbifWriter.Write((byte)12);
			spbifWriter.Write(executionTime.ToBinary());
			ReportUrl location = this.m_report.Location;
			if (location != null)
			{
				spbifWriter.Write((byte)10);
				spbifWriter.Write(location.ToString());
			}
			if (language != null)
			{
				spbifWriter.Write((byte)11);
				spbifWriter.Write(language);
			}
			spbifWriter.Write((byte)255);
			this.m_pageContentOffset = baseStream.Position;
			spbifWriter.Write((byte)19);
			this.m_columnsOffset = baseStream.Position;
			spbifWriter.Write((byte)20);
			this.WriteStartReportBodyToRPLStream2008(spbifWriter);
		}

		private void WriteStartReportBodyToRPLStream2008(BinaryWriter spbifWriter)
		{
			Stream baseStream = spbifWriter.BaseStream;
			this.m_bodyOffset = baseStream.Position;
			spbifWriter.Write((byte)6);
			spbifWriter.Write((byte)15);
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)255);
			spbifWriter.Write((byte)1);
			spbifWriter.Write((byte)255);
			spbifWriter.Write((byte)255);
		}

		private void WriteStartItemToRPLOM(RPLWriter rplWriter, string language)
		{
			RPLReport rPLReport = new RPLReport();
			rPLReport.ReportName = this.m_report.Name;
			rPLReport.Description = this.m_report.Description;
			rPLReport.Author = this.m_report.Author;
			rPLReport.AutoRefresh = this.m_report.AutoRefresh;
			rPLReport.ExecutionTime = this.m_report.ExecutionTime;
			rPLReport.Location = this.m_report.Location.ToString();
			rPLReport.Language = language;
			rPLReport.RPLVersion = this.m_rplVersion;
			rPLReport.ConsumeContainerWhitespace = this.m_report.ConsumeContainerWhitespace;
			rPLReport.RPLPaginatedPages = new RPLPageContent[1];
			rPLReport.RPLPaginatedPages[0] = new RPLPageContent(1);
			rplWriter.Report = rPLReport;
		}

		private void WriteEndItemToRPLStream(BinaryWriter spbifWriter, string pageName)
		{
			spbifWriter.Write((byte)255);
			Stream baseStream = spbifWriter.BaseStream;
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(this.m_pageContentOffset);
			spbifWriter.Write(this.m_sectionSizes.Count);
			for (int i = 0; i < this.m_sectionSizes.Count; i++)
			{
				spbifWriter.Write((float)this.m_sectionSizes[i].Left);
				spbifWriter.Write((float)this.m_sectionSizes[i].Top);
				spbifWriter.Write((float)this.m_sectionSizes[i].Width);
				spbifWriter.Write((float)this.m_sectionSizes[i].Height);
				spbifWriter.Write(0);
				spbifWriter.Write((byte)0);
				spbifWriter.Write(this.m_sectionOffsets[i]);
			}
			this.WriteReportPageLayoutAtEnd(spbifWriter, pageName);
			this.m_pageContentOffset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write((byte)255);
			long position2 = baseStream.Position;
			spbifWriter.Write((byte)18);
			spbifWriter.Write(this.m_offset);
			spbifWriter.Write(1);
			spbifWriter.Write(this.m_pageContentOffset);
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position2);
			spbifWriter.Write((byte)255);
		}

		private void WriteEndItemToRPLStream2008(BinaryWriter spbifWriter, int itemsOnPage, PageHeadFoot header, PageHeadFoot footer, double reportWidth, double reportHeight)
		{
			Stream baseStream = spbifWriter.BaseStream;
			if (header != null)
			{
				reportWidth = Math.Max(reportWidth, header.ItemRenderSizes.Width);
			}
			if (footer != null)
			{
				reportWidth = Math.Max(reportWidth, footer.ItemRenderSizes.Width);
			}
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(this.m_pageContentOffset);
			spbifWriter.Write(itemsOnPage);
			spbifWriter.Write(0f);
			spbifWriter.Write(0f);
			spbifWriter.Write((float)reportWidth);
			spbifWriter.Write((float)reportHeight);
			spbifWriter.Write(0);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(this.m_columnsOffset);
			if (header != null)
			{
				header.ItemRenderSizes.Width = reportWidth;
				header.WritePageItemRenderSizes(spbifWriter);
			}
			if (footer != null)
			{
				footer.ItemRenderSizes.Width = reportWidth;
				footer.WritePageItemRenderSizes(spbifWriter);
			}
			this.m_pageContentOffset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write((byte)255);
			long position2 = baseStream.Position;
			spbifWriter.Write((byte)18);
			spbifWriter.Write(this.m_offset);
			spbifWriter.Write(1);
			spbifWriter.Write(this.m_pageContentOffset);
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position2);
			spbifWriter.Write((byte)255);
		}

		private void WriteEndItemToRPLOM(RPLWriter rplWriter, int sectionStartIndex, string pageName)
		{
			RPLPageContent rPLPageContent = rplWriter.Report.RPLPaginatedPages[0];
			rPLPageContent.PageLayout.PageName = pageName;
			RPLSizes[] array = new RPLSizes[this.m_sectionSizes.Count];
			for (int i = 0; i < this.m_sectionSizes.Count; i++)
			{
				array[i] = new RPLSizes((float)this.m_sectionSizes[i].Top, (float)this.m_sectionSizes[i].Left, (float)this.m_sectionSizes[i].Height, (float)this.m_sectionSizes[i].Width);
			}
			rPLPageContent.ReportSectionSizes = array;
			for (int j = sectionStartIndex; j <= this.m_lastSectionIndex; j++)
			{
				rPLPageContent.AddReportSection(this.m_sections[j].RPLReportSection);
			}
		}

		private void WriteReportPageLayoutAtEnd(BinaryWriter spbifWriter, string pageName)
		{
			if ((int)this.m_pageContext.VersionPicker > 3 && pageName != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write((byte)48);
				spbifWriter.Write(pageName);
				spbifWriter.Write((byte)255);
			}
		}

		private void WriteReportPageLayoutAtStart(RPLWriter rplWriter)
		{
			if (this.m_pageContext.VersionPicker != 0 && this.m_pageContext.VersionPicker != RPLVersionEnum.RPL2008WithImageConsolidation && rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				Page page = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)this.m_report.ReportSections)[0].Page;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					this.m_pageLayoutOffset = baseStream.Position;
					binaryWriter.Write((byte)3);
					binaryWriter.Write((byte)16);
					binaryWriter.Write((float)page.PageHeight.ToMillimeters());
					binaryWriter.Write((byte)17);
					binaryWriter.Write((float)page.PageWidth.ToMillimeters());
					binaryWriter.Write((byte)20);
					binaryWriter.Write((float)page.BottomMargin.ToMillimeters());
					binaryWriter.Write((byte)19);
					binaryWriter.Write((float)page.LeftMargin.ToMillimeters());
					binaryWriter.Write((byte)21);
					binaryWriter.Write((float)page.RightMargin.ToMillimeters());
					binaryWriter.Write((byte)18);
					binaryWriter.Write((float)page.TopMargin.ToMillimeters());
					ReportPageLayout reportPageLayout = new ReportPageLayout(page);
					reportPageLayout.WriteElementStyle(rplWriter, this.m_pageContext);
					binaryWriter.Write((byte)255);
				}
				else
				{
					RPLPageLayout rPLPageLayout = new RPLPageLayout();
					rplWriter.Report.RPLPaginatedPages[0].PageLayout = rPLPageLayout;
					rPLPageLayout.PageHeight = (float)page.PageHeight.ToMillimeters();
					rPLPageLayout.PageWidth = (float)page.PageWidth.ToMillimeters();
					rPLPageLayout.MarginBottom = (float)page.BottomMargin.ToMillimeters();
					rPLPageLayout.MarginLeft = (float)page.LeftMargin.ToMillimeters();
					rPLPageLayout.MarginRight = (float)page.RightMargin.ToMillimeters();
					rPLPageLayout.MarginTop = (float)page.TopMargin.ToMillimeters();
					ReportPageLayout reportPageLayout2 = new ReportPageLayout(page);
					reportPageLayout2.WriteElementStyle(rplWriter, this.m_pageContext);
				}
			}
		}

		private void WriteReportPageLayout2008(RPLWriter rplWriter, double bodyWidth, double bodyHeight)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection reportSection = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)this.m_report.ReportSections)[0];
				Page page = reportSection.Page;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)16);
					binaryWriter.Write(this.m_columnsOffset);
					binaryWriter.Write(1);
					binaryWriter.Write(0f);
					binaryWriter.Write(0f);
					binaryWriter.Write((float)bodyWidth);
					binaryWriter.Write((float)bodyHeight);
					binaryWriter.Write(0);
					binaryWriter.Write((byte)0);
					binaryWriter.Write(this.m_bodyOffset);
					this.m_columnsOffset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write((byte)255);
					binaryWriter.Write((byte)1);
					binaryWriter.Write((byte)3);
					binaryWriter.Write((byte)1);
					binaryWriter.Write(reportSection.ID);
					binaryWriter.Write((byte)0);
					binaryWriter.Write(page.Instance.UniqueName);
					binaryWriter.Write((byte)16);
					binaryWriter.Write((float)page.PageHeight.ToMillimeters());
					binaryWriter.Write((byte)17);
					binaryWriter.Write((float)page.PageWidth.ToMillimeters());
					binaryWriter.Write((byte)20);
					binaryWriter.Write((float)page.BottomMargin.ToMillimeters());
					binaryWriter.Write((byte)19);
					binaryWriter.Write((float)page.LeftMargin.ToMillimeters());
					binaryWriter.Write((byte)21);
					binaryWriter.Write((float)page.RightMargin.ToMillimeters());
					binaryWriter.Write((byte)18);
					binaryWriter.Write((float)page.TopMargin.ToMillimeters());
					binaryWriter.Write((byte)23);
					binaryWriter.Write(page.Columns);
					binaryWriter.Write((byte)22);
					binaryWriter.Write((float)page.ColumnSpacing.ToMillimeters());
					ReportPageLayout reportPageLayout = new ReportPageLayout(page);
					reportPageLayout.WriteElementStyle(rplWriter, this.m_pageContext);
					binaryWriter.Write((byte)255);
				}
				else
				{
					RPLPageLayout rPLPageLayout = new RPLPageLayout();
					rplWriter.Report.RPLPaginatedPages[0].PageLayout = rPLPageLayout;
					rPLPageLayout.PageHeight = (float)page.PageHeight.ToMillimeters();
					rPLPageLayout.PageWidth = (float)page.PageWidth.ToMillimeters();
					rPLPageLayout.MarginBottom = (float)page.BottomMargin.ToMillimeters();
					rPLPageLayout.MarginLeft = (float)page.LeftMargin.ToMillimeters();
					rPLPageLayout.MarginRight = (float)page.RightMargin.ToMillimeters();
					rPLPageLayout.MarginTop = (float)page.TopMargin.ToMillimeters();
					ReportPageLayout reportPageLayout2 = new ReportPageLayout(page);
					reportPageLayout2.WriteElementStyle(rplWriter, this.m_pageContext);
				}
			}
		}

		private void WriteEndReportBodyToRPLStream2008(RPLWriter rplWriter, int sectionStartIndex, ref double bodyWidth, ref double bodyHeight)
		{
			if (rplWriter != null)
			{
				bodyWidth = 0.0;
				bodyHeight = 0.0;
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)16);
					binaryWriter.Write(this.m_bodyOffset);
					binaryWriter.Write(this.m_sectionSizes.Count);
					for (int i = 0; i < this.m_sectionSizes.Count; i++)
					{
						binaryWriter.Write((float)this.m_sectionSizes[i].Left);
						binaryWriter.Write((float)(this.m_sectionSizes[i].Top + bodyHeight));
						binaryWriter.Write((float)this.m_sectionSizes[i].Width);
						binaryWriter.Write((float)this.m_sectionSizes[i].Height);
						binaryWriter.Write(0);
						binaryWriter.Write((byte)0);
						binaryWriter.Write(this.m_sectionOffsets[i]);
						bodyWidth = Math.Max(bodyWidth, this.m_sectionSizes[i].Width);
						bodyHeight += this.m_sectionSizes[i].Height;
					}
					this.m_bodyOffset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write((byte)255);
				}
			}
		}

		private static void TraceLogicalPageBreak(int pageNumber, PageBreakInfo pageBreak)
		{
			if (pageBreak != null && !pageBreak.Disabled)
			{
				string text = "PR-DIAG [Page {0}] Page created by {1} page break";
				if (pageBreak.ResetPageNumber)
				{
					text += ". Page number reset";
				}
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, text, pageNumber, PageCreationType.Logical.ToString());
			}
		}

		private static void TraceVerticalPageBreak(int pageNumber)
		{
			RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page created by {1} page break", pageNumber, PageCreationType.Vertical.ToString());
		}

		private static void TracePageBreakIgnoredAtBottom(int pageNumber, PageBreakInfo pageBreak)
		{
			if (pageBreak != null && !pageBreak.Disabled)
			{
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page break on '{1}' ignored – bottom of page", pageNumber, pageBreak.ReportItemName);
			}
		}
	}
}
