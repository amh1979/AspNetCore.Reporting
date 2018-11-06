using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class ReportSection
	{
		internal class ColumnDetail
		{
			private double m_left;

			private double m_top;

			private double m_width;

			private double m_height;

			private long m_offset;

			private RPLBody m_element;

			internal double Left
			{
				get
				{
					return this.m_left;
				}
			}

			internal double Top
			{
				get
				{
					return this.m_top;
				}
			}

			internal double Width
			{
				get
				{
					return this.m_width;
				}
			}

			internal RPLBody Element
			{
				get
				{
					return this.m_element;
				}
			}

			internal double Height
			{
				get
				{
					return this.m_height;
				}
			}

			internal ColumnDetail(double leftEdge, double topEdge, ReportBody reportBody)
			{
				this.m_left = leftEdge;
				this.m_top = topEdge;
				this.m_width = reportBody.ItemPageSizes.Width;
				this.m_height = reportBody.ItemPageSizes.Height;
				this.m_offset = reportBody.Offset;
				this.m_element = (RPLBody)reportBody.RPLElement;
			}

			internal void WriteMeasurement(BinaryWriter spbifWriter)
			{
				spbifWriter.Write((float)this.Left);
				spbifWriter.Write((float)this.Top);
				spbifWriter.Write((float)this.Width);
				spbifWriter.Write((float)this.Height);
				spbifWriter.Write(0);
				spbifWriter.Write((byte)0);
				spbifWriter.Write(this.m_offset);
			}

			internal RPLItemMeasurement WriteMeasurement()
			{
				RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
				rPLItemMeasurement.Left = (float)this.Left;
				rPLItemMeasurement.Top = (float)this.Top;
				rPLItemMeasurement.Width = (float)this.Width;
				rPLItemMeasurement.Height = (float)this.Height;
				rPLItemMeasurement.ZIndex = 0;
				rPLItemMeasurement.State = 0;
				rPLItemMeasurement.Element = this.Element;
				return rPLItemMeasurement;
			}
		}

		private AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection m_reportSection;

		private ReportBody m_reportBody;

		private PageContext m_pageContext;

		private double m_leftEdge;

		private double m_topEdge;

		private long m_offset;

		private long m_pageOffset;

		private long m_columnsOffset;

		private List<ColumnDetail> m_columns;

		private PaginationSettings m_pageSettings;

		private SectionPaginationSettings m_sectionPageSettings;

		private RPLReportSection m_rplElement;

		private double m_columnHeight;

		private ItemSizes m_itemPageSizes = new ItemSizes();

		private PageHeadFoot m_header;

		private PageHeadFoot m_footer;

		private bool m_needsHeaderHeight;

		private int m_verticalPageNumber;

		private bool m_wroteEndToStream;

		internal double LeftEdge
		{
			get
			{
				return this.m_leftEdge;
			}
		}

		internal bool Done
		{
			get
			{
				if (this.m_reportBody == null)
				{
					return true;
				}
				return false;
			}
		}

		public SectionPaginationSettings SectionPaginationSettings
		{
			get
			{
				return this.m_sectionPageSettings;
			}
		}

		public AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection ROMReportSection
		{
			get
			{
				return this.m_reportSection;
			}
		}

		internal ReportSection(AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection reportSection, PageContext pageContext, PaginationSettings paginationSettings, SectionPaginationSettings sectionPaginationSettings)
		{
			this.m_reportSection = reportSection;
			this.m_pageContext = pageContext;
			this.m_pageSettings = paginationSettings;
			this.m_sectionPageSettings = sectionPaginationSettings;
		}

		internal void SetContext()
		{
			this.m_reportBody = new ReportBody(this.m_reportSection.Body, this.m_reportSection.Width);
		}

		internal double NextPage(RPLWriter rplWriter, int pageNumber, int totalPages, double top, double availableHeight, ReportSection nextSection, bool isFirstSectionOnPage)
		{
			double num = 0.0;
			bool flag = nextSection == null;
			if (!flag)
			{
				SectionPaginationSettings sectionPaginationSettings = nextSection.SectionPaginationSettings;
				num = 12.699999809265137 + sectionPaginationSettings.FooterHeight;
				PageSection pageHeader = nextSection.ROMReportSection.Page.PageHeader;
				if (pageHeader != null && pageHeader.PrintBetweenSections)
				{
					num += sectionPaginationSettings.HeaderHeight;
				}
			}
			double num2 = 0.0;
			PageSection pageHeader2 = this.m_reportSection.Page.PageHeader;
			PageSection pageFooter = this.m_reportSection.Page.PageFooter;
			bool flag2 = pageHeader2 != null && pageHeader2.PrintBetweenSections;
			bool flag3 = pageFooter != null && pageFooter.PrintBetweenSections;
			this.m_needsHeaderHeight = (isFirstSectionOnPage ? (pageHeader2 != null) : flag2);
			if (this.m_needsHeaderHeight)
			{
				num2 += this.m_sectionPageSettings.HeaderHeight;
				availableHeight -= num2;
			}
			if (pageFooter != null)
			{
				availableHeight -= this.m_sectionPageSettings.FooterHeight;
			}
			bool flag4 = false;
			bool flag5 = false;
			this.m_reportSection.SetPage(this.m_pageContext.PageNumberRegion, this.m_pageContext.Common.GetTotalPagesRegion(pageNumber), pageNumber, totalPages);
			bool needsReportItemsOnPage = this.m_reportSection.NeedsReportItemsOnPage;
			bool flag6 = this.m_pageContext.PropertyCacheState != PageContext.CacheState.CountPages;
			this.m_pageContext.EvaluatePageHeaderFooter = false;
			if (needsReportItemsOnPage && flag6)
			{
				if (flag2 && !isFirstSectionOnPage)
				{
					goto IL_015f;
				}
				if (this.HasHeaderOnPage(pageNumber, totalPages))
				{
					goto IL_015f;
				}
				goto IL_0162;
			}
			goto IL_018a;
			IL_018a:
			if (rplWriter != null)
			{
				this.m_columns = new List<ColumnDetail>(this.m_sectionPageSettings.Columns);
				this.WriteStartItemToStream(rplWriter);
			}
			long num3 = 0L;
			if (!this.Done)
			{
				double columnWidth = this.m_sectionPageSettings.ColumnWidth;
				this.m_pageContext.Common.Pagination.CurrentColumnWidth = columnWidth;
				this.m_columnHeight = availableHeight;
				this.m_pageContext.Common.Pagination.CurrentColumnHeight = this.m_columnHeight;
				int num4 = 0;
				int columns = this.m_sectionPageSettings.Columns;
				RoundedDouble roundedDouble = new RoundedDouble(this.m_topEdge);
				RoundedDouble roundedDouble2 = new RoundedDouble(this.m_leftEdge);
				this.m_pageContext.VerticalPageNumber = this.m_verticalPageNumber;
				while (num4 < columns)
				{
					bool flag7 = false;
					if (this.m_leftEdge == 0.0)
					{
						if (this.m_pageContext.TextBoxDuplicates != null)
						{
							this.m_pageContext.TextBoxDuplicates = null;
							this.m_reportBody.ResolveDuplicates(this.m_pageContext, this.m_topEdge, null, false);
						}
						this.m_reportBody.CalculateVertical(this.m_pageContext, this.m_topEdge, this.m_topEdge + availableHeight, null, new List<PageItem>(), ref flag7, false);
						this.m_verticalPageNumber++;
						this.m_pageContext.VerticalPageNumber = this.m_verticalPageNumber;
					}
					flag7 = false;
					this.m_reportBody.CalculateHorizontal(this.m_pageContext, this.m_leftEdge, this.m_leftEdge + columnWidth, null, new List<PageItem>(), ref flag7, false);
					num3 = this.m_pageContext.PropertyCacheWriter.BaseStream.Position;
					this.m_reportBody.AddToPage(rplWriter, this.m_pageContext, this.m_leftEdge, this.m_topEdge, this.m_leftEdge + columnWidth, this.m_topEdge + availableHeight, PageItem.RepeatState.None);
					if (rplWriter != null)
					{
						rplWriter.RegisterSectionItemizedData();
					}
					this.m_pageContext.PropertyCacheWriter.BaseStream.Seek(num3, SeekOrigin.Begin);
					if (rplWriter != null)
					{
						this.m_columns.Add(new ColumnDetail(0.0 - this.m_leftEdge, 0.0 - this.m_topEdge, this.m_reportBody));
					}
					this.m_leftEdge += columnWidth;
					roundedDouble2.Value = this.m_leftEdge;
					if (num4 == 0 && this.m_reportBody.ItemPageSizes.Bottom - this.m_topEdge < availableHeight)
					{
						this.m_columnHeight = this.m_reportBody.ItemPageSizes.Bottom - this.m_topEdge;
					}
					if (roundedDouble2 >= this.m_reportBody.ItemPageSizes.Width)
					{
						this.m_leftEdge = 0.0;
						this.m_topEdge += availableHeight;
						roundedDouble.Value = this.m_topEdge;
						this.m_reportBody.ResetHorizontal(true, null);
						this.m_pageContext.Common.PaginatingHorizontally = false;
					}
					else
					{
						this.m_pageContext.Common.PaginatingHorizontally = true;
					}
					num4++;
					if (roundedDouble >= this.m_reportBody.ItemPageSizes.Bottom)
					{
						this.m_reportBody = null;
						this.m_topEdge = 0.0;
						this.m_leftEdge = 0.0;
						break;
					}
				}
			}
			double num5 = availableHeight - this.m_columnHeight;
			if (this.Done && !flag && pageFooter != null && !flag3)
			{
				num5 += this.m_sectionPageSettings.FooterHeight;
			}
			bool flag8 = false;
			if (num5 < num || flag)
			{
				this.m_columnHeight = availableHeight;
				flag8 = true;
				num2 += this.m_sectionPageSettings.FooterHeight;
			}
			else if (flag3)
			{
				num2 += this.m_sectionPageSettings.FooterHeight;
			}
			this.m_itemPageSizes.Height = num2 + this.m_columnHeight;
			this.m_itemPageSizes.Top = top;
			this.WriteColumns(rplWriter);
			this.m_columns = null;
			if (this.Done && flag && totalPages == 0)
			{
				totalPages = pageNumber;
			}
			if (flag6)
			{
				flag4 = ((!isFirstSectionOnPage) ? flag2 : this.HasHeaderOnPage(pageNumber, totalPages));
				flag5 = ((!flag8) ? flag3 : this.HasFooterOnPage(pageNumber, totalPages));
			}
			if (flag4 || flag5)
			{
				this.m_pageContext.Common.CheckPageNameChanged();
				this.m_reportSection.SetPageName(this.m_pageContext.PageName);
				this.m_reportSection.GetPageSections();
				if (flag4 && !this.IsHeaderUnknown(isFirstSectionOnPage, pageNumber, totalPages))
				{
					this.RenderHeader(rplWriter);
				}
				else
				{
					this.m_header = null;
					flag4 = false;
				}
				if (flag5)
				{
					this.RenderFooter(rplWriter);
				}
				else
				{
					this.m_footer = null;
				}
				if (rplWriter != null && (flag4 || flag5))
				{
					rplWriter.RegisterSectionHeaderFooter();
				}
			}
			if (!this.IsHeaderUnknown(isFirstSectionOnPage, pageNumber, totalPages))
			{
				this.WriteEndItemToStream(rplWriter);
				this.m_wroteEndToStream = true;
			}
			else
			{
				this.m_wroteEndToStream = false;
			}
			if (rplWriter != null)
			{
				rplWriter.RegisterPageItemizedData();
			}
			return this.m_itemPageSizes.Height;
			IL_015f:
			flag4 = true;
			goto IL_0162;
			IL_0162:
			if (this.HasFooterOnPage(pageNumber, totalPages) || (flag3 && !flag))
			{
				flag5 = true;
			}
			if (flag4 || flag5)
			{
				this.m_pageContext.EvaluatePageHeaderFooter = true;
			}
			goto IL_018a;
		}

		private bool IsHeaderUnknown(bool isFirstSection, int page, int totalPages)
		{
			PageSection pageHeader = this.m_reportSection.Page.PageHeader;
			if (isFirstSection && page > 1 && totalPages == 0 && pageHeader != null)
			{
				return !pageHeader.PrintOnLastPage;
			}
			return false;
		}

		internal void WriteDelayedSectionHeader(RPLWriter rplWriter, bool isLastPage)
		{
			if (!this.m_wroteEndToStream)
			{
				if (!isLastPage)
				{
					List<SectionItemizedData> glyphCache = rplWriter.GlyphCache;
					if (this.m_footer != null && glyphCache != null)
					{
						SectionItemizedData sectionItemizedData = glyphCache[0];
						if (sectionItemizedData != null)
						{
							rplWriter.PageParagraphsItemizedData = sectionItemizedData.HeaderFooter;
						}
					}
					this.RenderHeader(rplWriter);
					if (this.m_footer == null && glyphCache != null)
					{
						Dictionary<string, List<TextRunItemizedData>> dictionary = rplWriter.PageParagraphsItemizedData;
						if (dictionary != null && dictionary.Count == 0)
						{
							dictionary = null;
						}
						glyphCache[0].HeaderFooter = dictionary;
					}
				}
				this.WriteEndItemToStream(rplWriter);
			}
		}

		private void RenderHeader(RPLWriter rplWriter)
		{
			double columnWidth = this.m_pageContext.ColumnWidth;
			double columnHeight = this.m_pageContext.ColumnHeight;
			PageContext pageContext = new PageContext(this.m_pageContext);
			pageContext.IgnorePageBreaks = true;
			pageContext.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideHeaderFooter;
			pageContext.EvaluatePageHeaderFooter = false;
			pageContext.Common.Pagination.CurrentColumnWidth = this.m_pageContext.Common.Pagination.UsablePageWidth;
			pageContext.Common.InHeaderFooter = true;
			double headerHeight = this.m_sectionPageSettings.HeaderHeight;
			pageContext.Common.Pagination.CurrentColumnHeight = headerHeight;
			PageSection pageHeader = this.m_reportSection.Page.PageHeader;
			PageHeadFoot pageHeadFoot = new PageHeadFoot(pageHeader, this.m_pageSettings.UsablePageWidth, true);
			bool flag = false;
			pageHeadFoot.CalculateVertical(pageContext, 0.0, headerHeight, null, new List<PageItem>(), ref flag, true);
			pageHeadFoot.CalculateHorizontal(pageContext, 0.0, this.m_pageSettings.UsablePageWidth, null, new List<PageItem>(), ref flag, true);
			pageHeadFoot.AddToPage(rplWriter, pageContext, 0.0, 0.0, this.m_pageSettings.UsablePageWidth, headerHeight, PageItem.RepeatState.None);
			this.m_header = pageHeadFoot;
			pageContext.Common.Pagination.CurrentColumnWidth = columnWidth;
			pageContext.Common.Pagination.CurrentColumnHeight = columnHeight;
			pageContext.Common.InHeaderFooter = false;
		}

		private void RenderFooter(RPLWriter rplWriter)
		{
			double columnWidth = this.m_pageContext.ColumnWidth;
			double columnHeight = this.m_pageContext.ColumnHeight;
			PageContext pageContext = new PageContext(this.m_pageContext);
			pageContext.IgnorePageBreaks = true;
			pageContext.IgnorePageBreaksReason = PageContext.IgnorePageBreakReason.InsideHeaderFooter;
			pageContext.EvaluatePageHeaderFooter = false;
			pageContext.Common.Pagination.CurrentColumnWidth = this.m_pageContext.Common.Pagination.UsablePageWidth;
			pageContext.Common.InHeaderFooter = true;
			double footerHeight = this.m_sectionPageSettings.FooterHeight;
			pageContext.Common.Pagination.CurrentColumnHeight = footerHeight;
			PageSection pageFooter = this.m_reportSection.Page.PageFooter;
			this.m_footer = new PageHeadFoot(pageFooter, this.m_pageSettings.UsablePageWidth, false);
			bool flag = false;
			this.m_footer.CalculateVertical(pageContext, 0.0, footerHeight, null, new List<PageItem>(), ref flag, true);
			this.m_footer.CalculateHorizontal(pageContext, 0.0, this.m_pageSettings.UsablePageWidth, null, new List<PageItem>(), ref flag, true);
			this.m_footer.AddToPage(rplWriter, pageContext, 0.0, 0.0, this.m_pageSettings.UsablePageWidth, footerHeight, PageItem.RepeatState.None);
			pageContext.Common.Pagination.CurrentColumnWidth = columnWidth;
			pageContext.Common.Pagination.CurrentColumnHeight = columnHeight;
			pageContext.Common.InHeaderFooter = false;
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					this.m_offset = baseStream.Position;
					this.m_pageOffset = baseStream.Position;
					binaryWriter.Write((byte)21);
					binaryWriter.Write((byte)22);
					binaryWriter.Write((byte)0);
					binaryWriter.Write(this.m_reportSection.ID);
					binaryWriter.Write((byte)1);
					binaryWriter.Write(this.m_sectionPageSettings.Columns);
					binaryWriter.Write((byte)2);
					binaryWriter.Write((float)this.m_sectionPageSettings.ColumnSpacing);
					binaryWriter.Write((byte)255);
					this.m_columnsOffset = baseStream.Position;
					binaryWriter.Write((byte)20);
				}
			}
		}

		internal void WriteColumns(RPLWriter rplWriter)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)16);
					binaryWriter.Write(this.m_columnsOffset);
					binaryWriter.Write(this.m_columns.Count);
					foreach (ColumnDetail column in this.m_columns)
					{
						column.WriteMeasurement(binaryWriter);
					}
					this.m_columnsOffset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write((byte)255);
				}
				else
				{
					RPLReportSection rPLReportSection = new RPLReportSection(this.m_columns.Count);
					rPLReportSection.ID = this.m_reportSection.ID;
					rPLReportSection.ColumnCount = this.m_sectionPageSettings.Columns;
					rPLReportSection.ColumnSpacing = (float)this.m_sectionPageSettings.ColumnSpacing;
					for (int i = 0; i < this.m_columns.Count; i++)
					{
						rPLReportSection.Columns[i] = this.m_columns[i].WriteMeasurement();
					}
					this.m_rplElement = rPLReportSection;
				}
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter != null)
			{
				PageHeadFoot header = this.m_header;
				this.m_header = null;
				PageHeadFoot footer = this.m_footer;
				this.m_footer = null;
				bool needsHeaderHeight = this.m_needsHeaderHeight;
				this.m_needsHeaderHeight = false;
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					rplWriter.BinaryWriter.Write((byte)255);
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)16);
					binaryWriter.Write(this.m_pageOffset);
					binaryWriter.Write(1 + ((header != null) ? 1 : 0) + ((footer != null) ? 1 : 0));
					binaryWriter.Write((float)this.m_pageSettings.MarginLeft);
					if (needsHeaderHeight)
					{
						binaryWriter.Write((float)this.m_sectionPageSettings.HeaderHeight);
					}
					else
					{
						binaryWriter.Write(0f);
					}
					binaryWriter.Write((float)(this.m_pageSettings.PhysicalPageWidth - this.m_pageSettings.MarginLeft - this.m_pageSettings.MarginRight));
					binaryWriter.Write((float)this.m_columnHeight);
					binaryWriter.Write(0);
					binaryWriter.Write((byte)0);
					binaryWriter.Write(this.m_columnsOffset);
					if (header != null)
					{
						header.WritePageItemSizes(binaryWriter);
					}
					if (footer != null)
					{
						footer.WritePageItemSizes(binaryWriter);
					}
					this.m_offset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write((byte)255);
				}
				else
				{
					this.m_rplElement.BodyArea = new RPLMeasurement();
					float top = 0f;
					if (needsHeaderHeight)
					{
						top = (float)this.m_sectionPageSettings.HeaderHeight;
					}
					this.m_rplElement.BodyArea.Top = top;
					this.m_rplElement.BodyArea.Height = (float)this.m_columnHeight;
					if (header != null)
					{
						this.m_rplElement.Header = header.WritePageItemSizes();
					}
					if (footer != null)
					{
						this.m_rplElement.Footer = footer.WritePageItemSizes();
					}
				}
			}
		}

		internal void WriteMeasurement(BinaryWriter spbifWriter)
		{
			spbifWriter.Write((float)this.m_itemPageSizes.Left);
			spbifWriter.Write((float)this.m_itemPageSizes.Top);
			spbifWriter.Write((float)this.m_itemPageSizes.Width);
			spbifWriter.Write((float)this.m_itemPageSizes.Height);
			spbifWriter.Write(0);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(this.m_offset);
		}

		internal void AddToPage(RPLPageContent rplPageContent, out RPLMeasurement measurement)
		{
			measurement = new RPLMeasurement();
			measurement.Left = 0f;
			measurement.Top = (float)this.m_itemPageSizes.Top;
			measurement.Width = (float)this.m_pageSettings.UsablePageWidth;
			measurement.Height = (float)this.m_itemPageSizes.Height;
			rplPageContent.AddReportSection(this.m_rplElement);
		}

		internal RPLPageLayout WritePageLayout(RPLWriter rplWriter, PageContext pageContext)
		{
			RPLPageLayout rPLPageLayout = new RPLPageLayout();
			ReportSectionPage reportSectionPage = new ReportSectionPage(this.m_reportSection.Page);
			reportSectionPage.PageLayout = rPLPageLayout;
			rPLPageLayout.PageHeight = (float)pageContext.Common.Pagination.PhysicalPageHeight;
			rPLPageLayout.PageWidth = (float)pageContext.Common.Pagination.PhysicalPageWidth;
			rPLPageLayout.MarginBottom = (float)pageContext.Common.Pagination.MarginBottom;
			rPLPageLayout.MarginLeft = (float)pageContext.Common.Pagination.MarginLeft;
			rPLPageLayout.MarginRight = (float)pageContext.Common.Pagination.MarginRight;
			rPLPageLayout.MarginTop = (float)pageContext.Common.Pagination.MarginTop;
			reportSectionPage.WriteItemStyle(rplWriter, pageContext);
			return rPLPageLayout;
		}

		public bool HasHeaderOnPage(int page, int totalPages)
		{
			PageSection pageHeader = this.m_reportSection.Page.PageHeader;
			if (pageHeader != null)
			{
				if (page > 1 && (totalPages == 0 || page < totalPages))
				{
					goto IL_003b;
				}
				if (pageHeader.PrintOnFirstPage && page == 1)
				{
					goto IL_003b;
				}
				if (pageHeader.PrintOnLastPage && page == totalPages && page != 1)
				{
					goto IL_003b;
				}
			}
			return false;
			IL_003b:
			return true;
		}

		public bool HasFooterOnPage(int page, int totalPages)
		{
			PageSection pageFooter = this.m_reportSection.Page.PageFooter;
			if (pageFooter != null)
			{
				if (page > 1 && (totalPages == 0 || page < totalPages))
				{
					goto IL_003b;
				}
				if (pageFooter.PrintOnFirstPage && page == 1 && page != totalPages)
				{
					goto IL_003b;
				}
				if (pageFooter.PrintOnLastPage && page == totalPages)
				{
					goto IL_003b;
				}
			}
			return false;
			IL_003b:
			return true;
		}
	}
}
