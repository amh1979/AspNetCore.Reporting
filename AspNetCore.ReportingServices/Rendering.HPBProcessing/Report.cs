using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Report
	{
		private AspNetCore.ReportingServices.OnDemandReportRendering.Report m_report;

		private PageContext m_pageContext;

		private PaginationSettings m_pageSettings;

		private long m_offset;

		private long m_pageOffset;

		private List<ReportSection> m_sections = new List<ReportSection>();

		private Version m_rplVersion = new Version(10, 6, 0);

		internal bool Done
		{
			get
			{
				if (this.m_sections.Count == 0)
				{
					return true;
				}
				return false;
			}
		}

		internal IJobContext JobContext
		{
			get
			{
				return this.m_report.JobContext;
			}
		}

		internal string InitialPageName
		{
			get
			{
				return this.m_report.Instance.InitialPageName;
			}
		}

		internal Report(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, PageContext pageContext, PaginationSettings aPagination)
		{
			this.m_report = report;
			this.m_pageContext = pageContext;
			this.m_pageSettings = aPagination;
		}

		internal static string GetReportLanguage(AspNetCore.ReportingServices.OnDemandReportRendering.Report report)
		{
			string result = null;
			ReportStringProperty language = report.Language;
			if (language != null)
			{
				if (language.IsExpression)
				{
					ReportInstance instance = report.Instance;
					result = instance.Language;
				}
				else
				{
					result = language.Value;
				}
			}
			return result;
		}

		internal void SetContext()
		{
			ReportSectionCollection reportSections = this.m_report.ReportSections;
			for (int i = 0; i < reportSections.Count; i++)
			{
				AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection reportSection = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)reportSections)[i];
				ReportSection reportSection2 = new ReportSection(reportSection, this.m_pageContext, this.m_pageSettings, this.m_pageSettings.SectionPaginationSettings[i]);
				reportSection2.SetContext();
				this.m_sections.Add(reportSection2);
			}
		}

		internal void NextPage(RPLWriter rplWriter, int totalPages)
		{
			this.WriteStartItemToStream(rplWriter);
			double num = 0.0;
			ReportSection reportSection = this.m_sections[0];
			int count = this.m_sections.Count;
			double num2 = 0.0;
			List<ReportSection> list = new List<ReportSection>();
			List<ReportSection> list2 = new List<ReportSection>();
			bool isFirstSectionOnPage = true;
			RoundedDouble roundedDouble = new RoundedDouble(this.m_pageContext.UsablePageHeight);
			for (int i = 0; i < this.m_sections.Count; i++)
			{
				ReportSection reportSection2 = this.m_sections[i];
				ReportSection nextSection = null;
				if (i < this.m_sections.Count - 1)
				{
					nextSection = this.m_sections[i + 1];
				}
				double num3 = reportSection2.NextPage(rplWriter, this.m_pageContext.PageNumber, totalPages, num, roundedDouble.Value - num, nextSection, isFirstSectionOnPage);
				if (num3 > 0.0)
				{
					num += num3;
					list2.Add(reportSection2);
				}
				num2 = Math.Max(num2, reportSection2.LeftEdge);
				if (reportSection2.Done)
				{
					list.Add(reportSection2);
				}
				if (roundedDouble == num)
				{
					break;
				}
				isFirstSectionOnPage = false;
			}
			if (num2 == 0.0)
			{
				foreach (ReportSection item in list)
				{
					this.m_sections.Remove(item);
				}
			}
			if (rplWriter != null)
			{
				reportSection.WriteDelayedSectionHeader(rplWriter, this.Done);
				this.WriteEndItemToStream(rplWriter, list2);
			}
			if (this.m_pageContext.PropertyCacheState == PageContext.CacheState.RPLStream)
			{
				this.m_pageContext.ItemPropsStart = null;
				this.m_pageContext.SharedImages = null;
			}
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				string reportLanguage = Report.GetReportLanguage(this.m_report);
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					this.m_offset = baseStream.Position;
					binaryWriter.Write((byte)0);
					binaryWriter.Write((byte)2);
					if (this.m_report.Name != null)
					{
						binaryWriter.Write((byte)15);
						binaryWriter.Write(this.m_report.Name);
					}
					if (this.m_report.Description != null)
					{
						binaryWriter.Write((byte)9);
						binaryWriter.Write(this.m_report.Description);
					}
					if (this.m_report.Author != null)
					{
						binaryWriter.Write((byte)13);
						binaryWriter.Write(this.m_report.Author);
					}
					if (this.m_report.AutoRefresh > 0)
					{
						binaryWriter.Write((byte)14);
						binaryWriter.Write(this.m_report.AutoRefresh);
					}
					DateTime executionTime = this.m_report.ExecutionTime;
					binaryWriter.Write((byte)12);
					binaryWriter.Write(executionTime.ToBinary());
					ReportUrl location = this.m_report.Location;
					if (location != null)
					{
						binaryWriter.Write((byte)10);
						binaryWriter.Write(location.ToString());
					}
					if (reportLanguage != null)
					{
						binaryWriter.Write((byte)11);
						binaryWriter.Write(reportLanguage);
					}
					binaryWriter.Write((byte)255);
					this.m_pageOffset = baseStream.Position;
					binaryWriter.Write((byte)19);
					binaryWriter.Write((byte)3);
					binaryWriter.Write((byte)16);
					binaryWriter.Write((float)this.m_pageSettings.PhysicalPageHeight);
					binaryWriter.Write((byte)17);
					binaryWriter.Write((float)this.m_pageSettings.PhysicalPageWidth);
					binaryWriter.Write((byte)20);
					binaryWriter.Write((float)this.m_pageSettings.MarginBottom);
					binaryWriter.Write((byte)19);
					binaryWriter.Write((float)this.m_pageSettings.MarginLeft);
					binaryWriter.Write((byte)21);
					binaryWriter.Write((float)this.m_pageSettings.MarginRight);
					binaryWriter.Write((byte)18);
					binaryWriter.Write((float)this.m_pageSettings.MarginTop);
					ReportSectionPage reportSectionPage = new ReportSectionPage(((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)this.m_report.ReportSections)[0].Page);
					reportSectionPage.WriteItemStyle(rplWriter, this.m_pageContext);
					binaryWriter.Write((byte)255);
				}
				else
				{
					RPLReport rPLReport = new RPLReport();
					rPLReport.ReportName = this.m_report.Name;
					rPLReport.Description = this.m_report.Description;
					rPLReport.Author = this.m_report.Author;
					rPLReport.AutoRefresh = this.m_report.AutoRefresh;
					rPLReport.ExecutionTime = this.m_report.ExecutionTime;
					rPLReport.Location = this.m_report.Location.ToString();
					rPLReport.Language = reportLanguage;
					rPLReport.RPLVersion = this.m_rplVersion;
					rPLReport.RPLPaginatedPages = new RPLPageContent[1];
					rplWriter.Report = rPLReport;
				}
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter, List<ReportSection> sectionsOnPage)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				rplWriter.BinaryWriter.Write((byte)255);
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write((byte)16);
				binaryWriter.Write(this.m_pageOffset);
				int count = sectionsOnPage.Count;
				binaryWriter.Write(count);
				for (int i = 0; i < count; i++)
				{
					sectionsOnPage[i].WriteMeasurement(binaryWriter);
				}
				this.m_pageOffset = baseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write((byte)255);
				position = baseStream.Position;
				binaryWriter.Write((byte)18);
				binaryWriter.Write(this.m_offset);
				binaryWriter.Write(1);
				binaryWriter.Write(this.m_pageOffset);
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write((byte)255);
			}
			else
			{
				int count2 = sectionsOnPage.Count;
				ReportSection reportSection = sectionsOnPage[0];
				RPLPageLayout pageLayout = reportSection.WritePageLayout(rplWriter, this.m_pageContext);
				RPLPageContent rPLPageContent = new RPLPageContent(count2, pageLayout);
				for (int j = 0; j < count2; j++)
				{
					RPLMeasurement rPLMeasurement = null;
					sectionsOnPage[j].AddToPage(rPLPageContent, out rPLMeasurement);
					rPLPageContent.ReportSectionSizes[j] = rPLMeasurement;
				}
				rplWriter.Report.RPLPaginatedPages[0] = rPLPageContent;
			}
		}
	}
}
