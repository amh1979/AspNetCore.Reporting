using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Layout;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIF;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer
{
	internal sealed class MainEngine : IDisposable
	{
		private IExcelGenerator m_excel;

		private CreateAndRegisterStream m_streamDelegate;

		private long m_totalScaleTimeMs;

		private long m_peakMemoryUsageKB;

		private Dictionary<string, BorderInfo> m_sharedBorderCache = new Dictionary<string, BorderInfo>();

		private Dictionary<string, ImageInformation> m_sharedImageCache = new Dictionary<string, ImageInformation>();

		private Stream m_backgroundImage;

		private ushort m_backgroundImageWidth;

		private ushort m_backgroundImageHeight;

		internal long TotalScaleTimeMs
		{
			get
			{
				return this.m_totalScaleTimeMs;
			}
		}

		internal long PeakMemoryUsageKB
		{
			get
			{
				return this.m_peakMemoryUsageKB;
			}
		}

		internal MainEngine(CreateAndRegisterStream createStream, ExcelRenderer excelRenderer)
		{
			this.m_streamDelegate = createStream;
			this.m_excel = excelRenderer.CreateExcelGenerator(this.CreateTempStream);
		}

		public void Dispose()
		{
			foreach (KeyValuePair<string, ImageInformation> item in this.m_sharedImageCache)
			{
				ImageInformation value = item.Value;
				if (value != null && value.ImageData != null)
				{
					value.ImageData.Close();
					value.ImageData = null;
				}
			}
			if (this.m_backgroundImage != null)
			{
				this.m_backgroundImage.Close();
				this.m_backgroundImage = null;
			}
		}

		private Stream CreateTempStream(string name)
		{
			return this.m_streamDelegate(name, ".bin", null, "", true, StreamOper.CreateOnly);
		}

		internal void AddBackgroundImage(RPLPageContent pageContent, RPLReportSection reportSection)
		{
			RPLBody rPLBody = (RPLBody)reportSection.Columns[0].Element;
			RPLImageData rPLImageData = (RPLImageData)rPLBody.ElementProps.Style[33];
			if (rPLImageData == null)
			{
				rPLImageData = (RPLImageData)pageContent.PageLayout.Style[33];
			}
			if (rPLImageData != null && rPLImageData.ImageData != null && rPLImageData.ImageData.Length > 0)
			{
				this.m_excel.AddBackgroundImage(rPLImageData.ImageData, rPLImageData.ImageName, ref this.m_backgroundImage, ref this.m_backgroundImageWidth, ref this.m_backgroundImageHeight);
			}
		}

		internal bool AddDocumentMap(DocumentMap docMap)
		{
			if (docMap == null)
			{
				return false;
			}
			this.m_excel.SetCurrentSheetName(ExcelRenderRes.DocumentMap);
			int num = 0;
			this.m_excel.SetColumnExtents(0, this.m_excel.MaxColumns - 1);
			this.m_excel.SetSummaryRowAfter(false);
			this.m_excel.DefineCachedStyle("DocumentMapHyperlinkStyle");
			this.m_excel.GetCellStyle().Color = this.m_excel.AddColor("Blue");
			this.m_excel.GetCellStyle().Underline = Underline.Single;
			this.m_excel.GetCellStyle().Name = "Arial";
			this.m_excel.GetCellStyle().Size = 10.0;
			this.m_excel.EndCachedStyle();
			while (docMap.MoveNext() && num <= this.m_excel.MaxRows)
			{
				if (num % this.m_excel.RowBlockSize == 0)
				{
					for (int i = 0; i < this.m_excel.RowBlockSize; i++)
					{
						this.m_excel.AddRow(i + num);
					}
				}
				DocumentMapNode current = docMap.Current;
				this.m_excel.SetRowContext(num);
				this.m_excel.SetColumnContext(current.Level - 1);
				this.m_excel.SetCellValue(current.Label, TypeCode.String);
				if (num == 0)
				{
					this.m_excel.GetCellStyle().Bold = 700;
					this.m_excel.GetCellStyle().Name = "Arial";
					this.m_excel.GetCellStyle().Size = 10.0;
					this.m_excel.GetCellStyle().Color = this.m_excel.AddColor("Black");
				}
				else
				{
					this.m_excel.AddBookmarkLink(current.Label, current.Id);
					this.m_excel.UseCachedStyle("DocumentMapHyperlinkStyle");
				}
				this.m_excel.SetRowProperties(num, 240, (byte)(current.Level - 1), current.Level > 2, false);
				this.m_excel.AddMergeCell(num, current.Level - 1, num, 32);
				num++;
			}
			for (int j = 0; j < this.m_excel.MaxColumns; j++)
			{
				this.m_excel.SetColumnProperties(j, 20.0, 0, false);
			}
			return true;
		}

		internal void AdjustFirstWorksheetName(string reportName, bool addedDocMap)
		{
			this.m_excel.AdjustFirstWorksheetName(reportName, addedDocMap);
		}

		internal void NextPage()
		{
			this.m_excel.NextWorksheet();
		}

		internal void RenderRPLPage(RPLReport report, bool headerInBody, bool suppressOutlines)
		{
			string key = null;
			LayoutEngine layoutEngine = new LayoutEngine(report, headerInBody, this.m_streamDelegate);
			RPLPageContent rPLPageContent = report.RPLPaginatedPages[0];
			this.m_excel.GenerateWorksheetName(rPLPageContent.PageLayout.PageName);
			RPLReportSection nextReportSection = rPLPageContent.GetNextReportSection();
			this.AddBackgroundImage(rPLPageContent, nextReportSection);
			Reader.ReadReportMeasurements(report, layoutEngine, suppressOutlines, nextReportSection);
			try
			{
				layoutEngine.RenderPageToExcel(this.m_excel, key, this.m_sharedBorderCache, this.m_sharedImageCache);
				if (layoutEngine.ScalabilityCache != null)
				{
					this.m_totalScaleTimeMs += layoutEngine.ScalabilityCache.ScalabilityDurationMs;
					this.m_peakMemoryUsageKB = Math.Max(this.m_peakMemoryUsageKB, layoutEngine.ScalabilityCache.PeakMemoryUsageKBytes);
				}
			}
			finally
			{
				layoutEngine.Dispose();
			}
		}

		internal void Save(Stream output)
		{
			this.m_excel.SaveSpreadsheet(output, this.m_backgroundImage, this.m_backgroundImageWidth, this.m_backgroundImageHeight);
		}
	}
}
