using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class SelectiveRendering
	{
		private sealed class ItemContext
		{
			internal RPLWriter RPLWriter
			{
				get;
				private set;
			}

			internal PageContext PageContext
			{
				get;
				private set;
			}

			internal AspNetCore.ReportingServices.OnDemandReportRendering.Report Report
			{
				get;
				private set;
			}

			internal AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection ReportSection
			{
				get;
				private set;
			}

			internal ItemContext(RPLWriter rplWriter, PageContext pageContext, AspNetCore.ReportingServices.OnDemandReportRendering.Report report, AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection reportSection)
			{
				this.RPLWriter = rplWriter;
				this.PageContext = pageContext;
				this.Report = report;
				this.ReportSection = reportSection;
			}
		}

		private abstract class ReportToRplWriterBase
		{
			protected readonly PageItem m_pageItem;

			private readonly ItemContext m_itemContext;

			protected PageContext PageContext
			{
				get
				{
					return this.m_itemContext.PageContext;
				}
			}

			protected RPLWriter RplWriter
			{
				get
				{
					return this.m_itemContext.RPLWriter;
				}
			}

			protected AspNetCore.ReportingServices.OnDemandReportRendering.Report Report
			{
				get
				{
					return this.m_itemContext.Report;
				}
			}

			protected AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection ReportSection
			{
				get
				{
					return this.m_itemContext.ReportSection;
				}
			}

			protected float Width
			{
				get
				{
					return (float)this.m_pageItem.ItemPageSizes.Width;
				}
			}

			protected float Height
			{
				get
				{
					return (float)this.m_pageItem.ItemPageSizes.Height;
				}
			}

			protected ReportToRplWriterBase(PageItem pageItem, ItemContext itemContext)
			{
				this.m_pageItem = pageItem;
				this.m_itemContext = itemContext;
			}
		}

		private sealed class ReportToRplStreamWriter : ReportToRplWriterBase
		{
			private BinaryWriter m_spbifWriter;

			private ReportToRplStreamWriter(PageItem item, ItemContext itemContext)
				: base(item, itemContext)
			{
				this.m_spbifWriter = base.RplWriter.BinaryWriter;
			}

			internal static void Write(PageItem item, ItemContext itemContext)
			{
				ReportToRplStreamWriter reportToRplStreamWriter = new ReportToRplStreamWriter(item, itemContext);
				reportToRplStreamWriter.WriteImpl();
			}

			private void WriteImpl()
			{
				BinaryWriter binaryWriter = base.RplWriter.BinaryWriter;
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write((byte)0);
				binaryWriter.Write((byte)2);
				this.WritePropertyToRplStream(15, base.Report.Name);
				this.WritePropertyToRplStream(9, base.Report.Description);
				this.WritePropertyToRplStream(13, base.Report.Author);
				this.WritePropertyToRplStream(11, AspNetCore.ReportingServices.Rendering.HPBProcessing.Report.GetReportLanguage(base.Report));
				if (base.Report.AutoRefresh > 0)
				{
					this.WritePropertyToRplStream(14, base.Report.AutoRefresh);
				}
				binaryWriter.Write((byte)12);
				binaryWriter.Write(base.Report.ExecutionTime.ToBinary());
				if (base.Report.Location != null)
				{
					this.WritePropertyToRplStream(10, base.Report.Location.ToString());
				}
				binaryWriter.Write((byte)255);
				long value = this.WritePage();
				long position2 = baseStream.Position;
				binaryWriter.Write((byte)18);
				binaryWriter.Write(position);
				binaryWriter.Write(1);
				binaryWriter.Write(value);
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position2);
				binaryWriter.Write((byte)255);
			}

			private long WritePage()
			{
				Stream baseStream = this.m_spbifWriter.BaseStream;
				long position = baseStream.Position;
				this.m_spbifWriter.Write((byte)19);
				this.m_spbifWriter.Write((byte)3);
				this.WritePropertyToRplStream(17, base.Width);
				this.WritePropertyToRplStream(16, base.Height);
				this.m_spbifWriter.Write((byte)6);
				this.m_spbifWriter.Write((byte)0);
				this.m_spbifWriter.Write((byte)5);
				this.m_spbifWriter.Write((byte)0);
				this.m_spbifWriter.Write((byte)255);
				this.m_spbifWriter.Write((byte)255);
				long grandParentEndOffset = this.WriteReportSection();
				long position2 = baseStream.Position;
				this.WriteSingleMeasurement(position, grandParentEndOffset, base.Width, base.Height);
				long position3 = baseStream.Position;
				this.m_spbifWriter.Write((byte)254);
				this.m_spbifWriter.Write(position2);
				this.m_spbifWriter.Write((byte)255);
				return position3;
			}

			private long WriteReportSection()
			{
				Stream baseStream = this.m_spbifWriter.BaseStream;
				long position = baseStream.Position;
				this.m_spbifWriter.Write((byte)21);
				this.m_spbifWriter.Write((byte)22);
				this.WritePropertyToRplStream(0, base.ReportSection.ID);
				this.WritePropertyToRplStream(1, 1);
				this.m_spbifWriter.Write((byte)255);
				long grandParentEndOffset = this.WriteSingleColumn();
				long position2 = baseStream.Position;
				this.WriteSingleMeasurement(position, grandParentEndOffset, base.Width, base.Height);
				long position3 = baseStream.Position;
				this.m_spbifWriter.Write((byte)254);
				this.m_spbifWriter.Write(position2);
				this.m_spbifWriter.Write((byte)255);
				return position3;
			}

			private long WriteSingleColumn()
			{
				Stream baseStream = this.m_spbifWriter.BaseStream;
				long position = baseStream.Position;
				this.m_spbifWriter.Write((byte)20);
				long grandParentEndOffset = this.WriteReportBody();
				long position2 = baseStream.Position;
				this.WriteSingleMeasurement(position, grandParentEndOffset, base.Width, base.Height);
				long position3 = baseStream.Position;
				this.m_spbifWriter.Write((byte)254);
				this.m_spbifWriter.Write(position2);
				this.m_spbifWriter.Write((byte)255);
				return position3;
			}

			private long WriteReportBody()
			{
				long position = this.m_spbifWriter.BaseStream.Position;
				this.m_spbifWriter.Write((byte)6);
				this.m_spbifWriter.Write((byte)15);
				this.m_spbifWriter.Write((byte)0);
				this.WritePropertyToRplStream(1, base.ReportSection.Body.ID);
				this.WritePropertyToRplStream(0, base.ReportSection.Body.InstanceUniqueName);
				this.m_spbifWriter.Write((byte)255);
				this.m_spbifWriter.Write((byte)255);
				base.m_pageItem.WriteStartItemToStream(base.RplWriter, base.PageContext);
				long position2 = this.m_spbifWriter.BaseStream.Position;
				this.m_spbifWriter.Write((byte)16);
				this.m_spbifWriter.Write(position);
				this.m_spbifWriter.Write(1);
				base.m_pageItem.WritePageItemSizes(this.m_spbifWriter);
				long position3 = this.m_spbifWriter.BaseStream.Position;
				this.m_spbifWriter.Write((byte)254);
				this.m_spbifWriter.Write(position2);
				this.m_spbifWriter.Write((byte)255);
				return position3;
			}

			private void WritePropertyToRplStream(byte itemNameToken, string value)
			{
				if (value != null)
				{
					this.m_spbifWriter.Write(itemNameToken);
					this.m_spbifWriter.Write(value);
				}
			}

			private void WritePropertyToRplStream(byte itemNameToken, float value)
			{
				this.m_spbifWriter.Write(itemNameToken);
				this.m_spbifWriter.Write(value);
			}

			private void WritePropertyToRplStream(byte itemNameToken, int value)
			{
				this.m_spbifWriter.Write(itemNameToken);
				this.m_spbifWriter.Write(value);
			}

			private void WriteSingleMeasurement(long parentStartOffset, long grandParentEndOffset, float width, float height)
			{
				this.m_spbifWriter.Write((byte)16);
				this.m_spbifWriter.Write(parentStartOffset);
				this.m_spbifWriter.Write(1);
				this.m_spbifWriter.Write(0f);
				this.m_spbifWriter.Write(0f);
				this.m_spbifWriter.Write(width);
				this.m_spbifWriter.Write(height);
				this.m_spbifWriter.Write(0);
				this.m_spbifWriter.Write((byte)0);
				this.m_spbifWriter.Write(grandParentEndOffset);
			}
		}

		private sealed class ReportToRplOmWriter : ReportToRplWriterBase
		{
			private ReportToRplOmWriter(PageItem item, ItemContext itemContext)
				: base(item, itemContext)
			{
			}

			internal static void Write(PageItem item, ItemContext itemContext)
			{
				ReportToRplOmWriter reportToRplOmWriter = new ReportToRplOmWriter(item, itemContext);
				reportToRplOmWriter.WriteImpl();
			}

			private void WriteImpl()
			{
				Version rPLVersion = new Version(10, 6, 0);
				RPLReport rPLReport = new RPLReport();
				rPLReport.ReportName = base.Report.Name;
				rPLReport.Description = base.Report.Description;
				rPLReport.Author = base.Report.Author;
				rPLReport.AutoRefresh = base.Report.AutoRefresh;
				rPLReport.ExecutionTime = base.Report.ExecutionTime;
				rPLReport.Location = base.Report.Location.ToString();
				rPLReport.Language = AspNetCore.ReportingServices.Rendering.HPBProcessing.Report.GetReportLanguage(base.Report);
				rPLReport.RPLVersion = rPLVersion;
				rPLReport.RPLPaginatedPages = new RPLPageContent[1];
				base.RplWriter.Report = rPLReport;
				RPLReportSection rPLReportSection = new RPLReportSection(1);
				rPLReportSection.ID = base.ReportSection.ID;
				rPLReportSection.ColumnCount = 1;
				RPLBody rPLBody = new RPLBody();
				RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
				rPLItemMeasurement.Left = 0f;
				rPLItemMeasurement.Top = 0f;
				rPLItemMeasurement.Width = base.Width;
				rPLItemMeasurement.Height = base.Height;
				rPLItemMeasurement.ZIndex = 0;
				rPLItemMeasurement.State = 0;
				rPLItemMeasurement.Element = rPLBody;
				rPLReportSection.Columns[0] = rPLItemMeasurement;
				rPLReportSection.BodyArea = new RPLMeasurement();
				rPLReportSection.BodyArea.Top = 0f;
				rPLReportSection.BodyArea.Height = base.Height;
				base.m_pageItem.WriteStartItemToStream(base.RplWriter, base.PageContext);
				RPLItemMeasurement[] array2 = rPLBody.Children = new RPLItemMeasurement[1];
				array2[0] = base.m_pageItem.WritePageItemSizes();
				RPLPageLayout rPLPageLayout = new RPLPageLayout();
				rPLPageLayout.PageHeight = base.Height;
				rPLPageLayout.PageWidth = base.Width;
				rPLPageLayout.Style = new RPLElementStyle(null, null);
				RPLPageContent rPLPageContent = new RPLPageContent(1, rPLPageLayout);
				RPLMeasurement rPLMeasurement = new RPLMeasurement();
				rPLMeasurement.Left = 0f;
				rPLMeasurement.Top = 0f;
				rPLMeasurement.Width = base.Width;
				rPLMeasurement.Height = base.Height;
				rPLPageContent.ReportSectionSizes[0] = rPLMeasurement;
				rPLPageContent.AddReportSection(rPLReportSection);
				base.RplWriter.Report.RPLPaginatedPages[0] = rPLPageContent;
			}
		}

		private const char ReportItemPathSeparator = '/';

		private AspNetCore.ReportingServices.OnDemandReportRendering.Report m_report;

		private PageContext m_pageContext;

		private PaginationSettings m_paginationSettings;

		internal bool Done
		{
			get;
			private set;
		}

		internal SelectiveRendering(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, PageContext pageContext, PaginationSettings paginationSettings)
		{
			this.m_report = report;
			this.m_pageContext = pageContext;
			this.m_paginationSettings = paginationSettings;
		}

		internal void RenderReportItem(RPLWriter rplWriter, string reportItemName)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection reportSection = null;
			ReportItem reportItem = SelectiveRendering.FindReportItem(this.m_report, SelectiveRendering.SplitReportItemPath(reportItemName), out reportSection);
			if (reportItem == null)
			{
				throw new SelectiveRenderingCannotFindReportItemException(reportItemName);
			}
			CustomReportItem criOwner = reportItem.CriOwner;
			if (criOwner != null)
			{
				criOwner.DynamicWidth = ReportSize.FromMillimeters(this.m_paginationSettings.PhysicalPageWidth);
				criOwner.DynamicHeight = ReportSize.FromMillimeters(this.m_paginationSettings.PhysicalPageHeight);
			}
			PageItem pageItem = PageItem.Create(reportItem, false, this.m_pageContext);
			pageItem.ItemPageSizes.Top = 0.0;
			pageItem.ItemPageSizes.Left = 0.0;
			pageItem.ItemPageSizes.Width = this.m_paginationSettings.PhysicalPageWidth;
			pageItem.ItemPageSizes.Height = this.m_paginationSettings.PhysicalPageHeight;
			ItemContext itemContext = new ItemContext(rplWriter, this.m_pageContext, this.m_report, reportSection);
			if (rplWriter.BinaryWriter != null)
			{
				ReportToRplStreamWriter.Write(pageItem, itemContext);
			}
			else
			{
				ReportToRplOmWriter.Write(pageItem, itemContext);
			}
			this.Done = true;
		}

		private static IEnumerable<string> SplitReportItemPath(string reportItemPath)
		{
			if (reportItemPath == null)
			{
				return Enumerable.Empty<string>();
			}
			return reportItemPath.Split('/');
		}

		private static ReportItem FindReportItem(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, IEnumerable<string> reportItemPathSteps, out AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection reportSection)
		{
			reportSection = null;
			int num = reportItemPathSteps.Count();
			if (num == 0)
			{
				return null;
			}
			bool flag = num > 1;
			string text = reportItemPathSteps.FirstOrDefault();
			ReportItem reportItem = null;
			foreach (AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection reportSection2 in report.ReportSections)
			{
				foreach (ReportItem item in reportSection2.Body.ReportItemCollection)
				{
					if (flag)
					{
						AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = item as AspNetCore.ReportingServices.OnDemandReportRendering.SubReport;
						if (subReport != null && subReport.Report != null && string.CompareOrdinal(item.Name, text) == 0)
						{
							reportItem = SelectiveRendering.FindReportItem(subReport.Report, reportItemPathSteps.Skip(1), out reportSection);
						}
					}
					else
					{
						AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle rectangle = item as AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle;
						if (rectangle != null)
						{
							reportItem = SelectiveRendering.FindReportItem(rectangle, text);
						}
						else if (string.CompareOrdinal(item.Name, text) == 0)
						{
							reportItem = item;
						}
					}
					if (reportItem != null)
					{
						reportSection = reportSection2;
						return reportItem;
					}
				}
			}
			return null;
		}

		private static ReportItem FindReportItem(AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle container, string reportItemName)
		{
			foreach (ReportItem item in container.ReportItemCollection)
			{
				if (string.CompareOrdinal(item.Name, reportItemName) == 0)
				{
					return item;
				}
			}
			return null;
		}
	}
}
