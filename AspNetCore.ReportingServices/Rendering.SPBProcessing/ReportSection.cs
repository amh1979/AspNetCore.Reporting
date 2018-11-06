using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class ReportSection
	{
		private AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection m_reportSectionDef;

		private PageContext m_pageContext;

		private PageItem m_body;

		private PageHeadFoot m_header;

		private PageHeadFoot m_footer;

		private ItemSizes m_itemRenderSizes;

		private int m_sectionIndex = -1;

		private PageItemHelper m_bodyHelper;

		private long m_offset;

		private long m_bodyOffset;

		private RPLReportSection m_rplReportSection;

		private int m_itemsOnPage;

		internal bool Done
		{
			get
			{
				if (this.m_body == null)
				{
					return true;
				}
				if (this.m_body.ItemState != PageItem.State.OnPage)
				{
					return this.m_body.ItemState == PageItem.State.OnPagePBEnd;
				}
				return true;
			}
		}

		internal bool SpanPages
		{
			get
			{
				if (this.m_body == null)
				{
					return true;
				}
				return this.m_body.ItemState == PageItem.State.SpanPages;
			}
		}

		internal bool OnPagePBEnd
		{
			get
			{
				if (this.m_body == null)
				{
					return true;
				}
				return this.m_body.ItemState == PageItem.State.OnPagePBEnd;
			}
		}

		internal PageItem Body
		{
			get
			{
				return this.m_body;
			}
		}

		internal PageHeadFoot Header
		{
			get
			{
				return this.m_header;
			}
		}

		internal PageHeadFoot Footer
		{
			get
			{
				return this.m_footer;
			}
		}

		internal ItemSizes ItemRenderSizes
		{
			get
			{
				return this.m_itemRenderSizes;
			}
		}

		internal int SectionIndex
		{
			get
			{
				return this.m_sectionIndex;
			}
			set
			{
				this.m_sectionIndex = value;
			}
		}

		internal long Offset
		{
			get
			{
				return this.m_offset;
			}
		}

		internal RPLReportSection RPLReportSection
		{
			get
			{
				return this.m_rplReportSection;
			}
		}

		internal ReportSection(AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection section, PageContext pageContext)
		{
			this.m_reportSectionDef = section;
			this.m_pageContext = pageContext;
		}

		internal void SetContext()
		{
			this.m_body = new ReportBody(this.m_reportSectionDef.Body, this.m_reportSectionDef.Width, this.m_pageContext);
		}

		internal bool CalculatePage(RPLWriter rplWriter, int page, int totalPages, int regionPageNumber, int regionTotalPages, bool firstSectionOnPage, bool lastSection, Interactivity interactivity, double heightToBeUsed, ref PageItemHelper lastBodyInfo, ref bool delayedHeader, ref bool delayedFooter, ref bool lastSectionOnPage)
		{
			this.m_pageContext.EvaluatePageHeaderFooter = false;
			PageSection pageHeader = this.m_reportSectionDef.Page.PageHeader;
			PageSection pageFooter = this.m_reportSectionDef.Page.PageFooter;
			bool flag = false;
			bool flag2 = false;
			if (pageHeader != null || pageFooter != null)
			{
				this.m_reportSectionDef.SetPage(regionPageNumber, regionTotalPages, page, totalPages);
			}
			if ((rplWriter != null || (interactivity != null && !interactivity.Done && interactivity.NeedPageHeaderFooter)) && this.m_reportSectionDef.NeedsReportItemsOnPage)
			{
				this.InitialCheckForHeader(pageHeader, page, totalPages, firstSectionOnPage, ref flag);
				this.InitialCheckForFooter(pageFooter, page, totalPages, lastSection, ref flag2);
				if (flag || flag2)
				{
					this.m_pageContext.EvaluatePageHeaderFooter = true;
				}
			}
			this.WriteStartItemToStream(rplWriter);
			double num = 0.0;
			this.m_body.UpdateItem(lastBodyInfo);
			this.m_body.CalculatePage(rplWriter, lastBodyInfo, this.m_pageContext, null, null, 0.0, ref num, interactivity);
			this.m_pageContext.ApplyPageName(page);
			this.m_itemsOnPage++;
			if (this.m_pageContext.CancelPage)
			{
				this.m_body = null;
				return false;
			}
			this.WriteBodyColumnsToStream(rplWriter);
			this.CreateReportSectionSizes(rplWriter);
			this.CheckForLastSectionOnPage(heightToBeUsed, lastSection, ref lastSectionOnPage);
			if (rplWriter != null || (interactivity != null && !interactivity.Done && interactivity.NeedPageHeaderFooter))
			{
				if (!this.m_reportSectionDef.NeedsReportItemsOnPage)
				{
					this.InitialCheckForHeader(pageHeader, page, totalPages, firstSectionOnPage, ref flag);
					this.InitialCheckForFooter(pageFooter, page, totalPages, lastSection, ref flag2);
				}
				this.FinalCheckForHeader(pageHeader, page, lastSection && this.Done, firstSectionOnPage, ref flag);
				this.FinalCheckForFooter(pageFooter, page, lastSection && this.Done, lastSectionOnPage, ref flag2);
				if (pageHeader != null || pageFooter != null)
				{
					string pageName = this.m_pageContext.PageTotalInfo.GetPageName(page);
					this.m_reportSectionDef.SetPageName(pageName);
					this.m_reportSectionDef.GetPageSections();
				}
				PageContext pageContext = new PageContext(this.m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.HeaderFooter);
				if (flag2)
				{
					if ((this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation) && lastSectionOnPage)
					{
						delayedFooter = true;
					}
					if (!delayedFooter)
					{
						pageContext.RPLSectionArea = PageContext.RPLReportSectionArea.Footer;
						this.m_footer = new PageHeadFoot(pageFooter, this.m_reportSectionDef.Width, pageContext);
						if (this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
						{
							this.m_footer.CalculateItem(rplWriter, pageContext, false, interactivity, false);
						}
						else
						{
							this.m_footer.CalculateItem(rplWriter, pageContext, false, interactivity, true);
						}
						this.m_itemsOnPage++;
					}
					if (this.m_pageContext.CancelPage)
					{
						this.m_body = null;
						this.m_footer = null;
						return false;
					}
				}
				if (flag)
				{
					if (this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
					{
						if (firstSectionOnPage)
						{
							delayedHeader = true;
						}
					}
					else if (page > 1 && firstSectionOnPage && !pageHeader.PrintOnLastPage && !this.m_pageContext.AddFirstPageHeaderFooter)
					{
						delayedHeader = true;
					}
					if (!delayedHeader)
					{
						pageContext.RPLSectionArea = PageContext.RPLReportSectionArea.Header;
						this.m_header = new PageHeadFoot(pageHeader, this.m_reportSectionDef.Width, pageContext);
						if (this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
						{
							this.m_header.CalculateItem(rplWriter, pageContext, true, interactivity, false);
						}
						else
						{
							this.m_header.CalculateItem(rplWriter, pageContext, true, interactivity, true);
						}
						this.m_itemsOnPage++;
					}
					if (this.m_pageContext.CancelPage)
					{
						this.m_body = null;
						this.m_footer = null;
						this.m_header = null;
						return false;
					}
				}
			}
			if (!delayedHeader || this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
			{
				this.UpdateReportSectionSizes(rplWriter);
				this.WriteEndItemToStream(rplWriter);
			}
			lastBodyInfo = null;
			return true;
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					if (this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
					{
						this.WriteStartRectangleItemToRPLStream2008(binaryWriter);
					}
					else
					{
						this.WriteStartSectionItemToRPLStream(binaryWriter);
					}
				}
				else
				{
					this.WriteStartSectionItemToRPLOM(rplWriter);
				}
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					if (this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008 || this.m_pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
					{
						this.WriteEndRectangleItemToRPLStream2008(binaryWriter);
					}
					else
					{
						this.WriteEndSectionItemToRPLStream(binaryWriter);
					}
				}
				else
				{
					this.WriteEndSectionItemToRPLOM(rplWriter);
				}
			}
		}

		internal void WriteBodyColumnsToStream(RPLWriter rplWriter)
		{
			if (this.m_pageContext.VersionPicker != 0 && this.m_pageContext.VersionPicker != RPLVersionEnum.RPL2008WithImageConsolidation && rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					long position = baseStream.Position;
					binaryWriter.Write((byte)16);
					binaryWriter.Write(this.m_bodyOffset);
					binaryWriter.Write(1);
					this.m_body.WritePageItemRenderSizes(binaryWriter);
					this.m_bodyOffset = baseStream.Position;
					binaryWriter.Write((byte)254);
					binaryWriter.Write(position);
					binaryWriter.Write((byte)255);
				}
				else
				{
					this.m_rplReportSection.Columns[0] = this.m_body.WritePageItemRenderSizes();
				}
			}
		}

		internal void CalculateDelayedHeader(RPLWriter rplWriter, Interactivity interactivity)
		{
			PageSection pageHeader = this.m_reportSectionDef.Page.PageHeader;
			PageContext pageContext = new PageContext(this.m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.HeaderFooter);
			pageContext.RPLSectionArea = PageContext.RPLReportSectionArea.Header;
			this.m_header = new PageHeadFoot(pageHeader, this.m_reportSectionDef.Width, pageContext);
			this.m_header.CalculateItem(rplWriter, pageContext, true, interactivity, true);
			this.m_itemsOnPage++;
		}

		internal void CalculateDelayedFooter(RPLWriter rplWriter, Interactivity interactivity)
		{
			PageSection pageFooter = this.m_reportSectionDef.Page.PageFooter;
			PageContext pageContext = new PageContext(this.m_pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.HeaderFooter);
			pageContext.RPLSectionArea = PageContext.RPLReportSectionArea.Footer;
			this.m_footer = new PageHeadFoot(pageFooter, this.m_reportSectionDef.Width, pageContext);
			this.m_footer.CalculateItem(rplWriter, pageContext, false, interactivity, true);
			this.m_itemsOnPage++;
		}

		internal void UpdateItem(ReportSectionHelper sectionHelper)
		{
			if (sectionHelper != null)
			{
				this.m_sectionIndex = sectionHelper.SectionIndex;
				this.m_bodyHelper = sectionHelper.BodyHelper;
			}
		}

		internal void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)16);
				this.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)255);
			}
		}

		internal ReportSectionHelper WritePaginationInfo()
		{
			ReportSectionHelper reportSectionHelper = new ReportSectionHelper();
			this.WritePaginationInfoProperties(reportSectionHelper);
			return reportSectionHelper;
		}

		internal void WritePaginationInfoProperties(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)23);
				reportPageInfo.Write(this.m_sectionIndex);
				if (this.m_body != null && !this.Done)
				{
					this.m_body.WritePaginationInfo(reportPageInfo);
				}
			}
		}

		internal void WritePaginationInfoProperties(ReportSectionHelper sectionHelper)
		{
			if (sectionHelper != null)
			{
				sectionHelper.SectionIndex = this.m_sectionIndex;
				if (this.m_body != null && !this.Done)
				{
					sectionHelper.BodyHelper = this.m_body.WritePaginationInfo();
				}
			}
		}

		internal void UpdateReportSectionSizes(RPLWriter rplWriter)
		{
			if (this.m_itemRenderSizes != null && rplWriter != null)
			{
				double num = this.m_itemRenderSizes.Width;
				if (this.m_header != null)
				{
					num = Math.Max(num, this.m_header.ItemRenderSizes.Width);
					this.m_itemRenderSizes.Height += this.m_header.ItemRenderSizes.Height;
				}
				if (this.m_footer != null)
				{
					num = Math.Max(num, this.m_footer.ItemRenderSizes.Width);
					this.m_itemRenderSizes.Height += this.m_footer.ItemRenderSizes.Height;
				}
				this.m_itemRenderSizes.Width = num;
				this.NormalizeSectionAreasWidths(rplWriter, num);
			}
		}

		internal void Reset()
		{
			this.m_header = null;
			this.m_footer = null;
			this.m_rplReportSection = null;
			this.m_bodyOffset = 0L;
			this.m_offset = 0L;
			this.m_itemRenderSizes = null;
			this.m_sectionIndex = -1;
			this.m_bodyHelper = null;
			this.m_itemsOnPage = 0;
			if (this.Done)
			{
				this.m_body = null;
			}
		}

		internal bool IsHeaderPrintOnLastPage()
		{
			PageSection pageHeader = this.m_reportSectionDef.Page.PageHeader;
			if (pageHeader == null)
			{
				return false;
			}
			if (pageHeader.PrintOnLastPage)
			{
				return true;
			}
			return false;
		}

		private void NormalizeSectionAreasWidths(RPLWriter rplWriter, double width)
		{
			if (this.m_itemRenderSizes != null && rplWriter != null)
			{
				this.m_body.ItemRenderSizes.Width = width;
				if (this.m_header != null)
				{
					this.m_header.ItemRenderSizes.Width = width;
				}
				if (this.m_footer != null)
				{
					this.m_footer.ItemRenderSizes.Width = width;
				}
			}
		}

		private void CreateReportSectionSizes(RPLWriter rplWriter)
		{
			if (rplWriter != null)
			{
				this.m_itemRenderSizes = new ItemSizes(this.m_body.ItemRenderSizes);
			}
			else
			{
				this.m_itemRenderSizes = new ItemSizes(this.m_body.ItemPageSizes);
			}
		}

		private void InitialCheckForHeader(PageSection header, int page, int totalPages, bool firstSectionOnPage, ref bool renderHeader)
		{
			if (header != null)
			{
				if (this.m_pageContext.AddFirstPageHeaderFooter)
				{
					renderHeader = true;
				}
				else if (totalPages > 0)
				{
					if (page == 1)
					{
						if (firstSectionOnPage)
						{
							if (header.PrintOnFirstPage)
							{
								renderHeader = true;
							}
						}
						else if (header.PrintBetweenSections)
						{
							renderHeader = true;
						}
					}
					else if (page > 1 && page < totalPages)
					{
						if (firstSectionOnPage)
						{
							renderHeader = true;
						}
						else if (header.PrintBetweenSections)
						{
							renderHeader = true;
						}
					}
					else if (firstSectionOnPage)
					{
						if (header.PrintOnLastPage)
						{
							renderHeader = true;
						}
					}
					else if (header.PrintBetweenSections)
					{
						renderHeader = true;
					}
				}
				else if (page == 1)
				{
					if (firstSectionOnPage)
					{
						if (header.PrintOnFirstPage)
						{
							renderHeader = true;
						}
					}
					else if (header.PrintBetweenSections)
					{
						renderHeader = true;
					}
				}
				else if (firstSectionOnPage)
				{
					renderHeader = true;
				}
				else if (header.PrintBetweenSections)
				{
					renderHeader = true;
				}
			}
		}

		private void InitialCheckForFooter(PageSection footer, int page, int totalPages, bool lastReportSection, ref bool renderFooter)
		{
			if (footer != null)
			{
				if (this.m_pageContext.AddFirstPageHeaderFooter)
				{
					renderFooter = true;
				}
				else if (totalPages > 0)
				{
					if (page >= 1 && page < totalPages)
					{
						renderFooter = true;
					}
					else if (lastReportSection)
					{
						if (footer.PrintOnLastPage)
						{
							renderFooter = true;
						}
					}
					else if (footer.PrintBetweenSections)
					{
						renderFooter = true;
					}
				}
				else
				{
					renderFooter = true;
				}
			}
		}

		private void FinalCheckForHeader(PageSection header, int page, bool lastPage, bool firstSectionOnPage, ref bool renderHeader)
		{
			if (header != null && renderHeader && !this.m_pageContext.AddFirstPageHeaderFooter && page > 1 && firstSectionOnPage && lastPage && !header.PrintOnLastPage)
			{
				renderHeader = false;
			}
		}

		private void FinalCheckForFooter(PageSection footer, int page, bool lastPage, bool lastSectionOnPage, ref bool renderFooter)
		{
			if (footer != null && renderFooter && !this.m_pageContext.AddFirstPageHeaderFooter)
			{
				bool flag = false;
				if (!lastPage)
				{
					if (page == 1)
					{
						if (lastSectionOnPage)
						{
							if (footer.PrintOnFirstPage)
							{
								flag = true;
							}
						}
						else if (footer.PrintBetweenSections)
						{
							flag = true;
						}
					}
					else if (lastSectionOnPage)
					{
						flag = true;
					}
					else if (footer.PrintBetweenSections)
					{
						flag = true;
					}
				}
				else if (lastSectionOnPage)
				{
					if (footer.PrintOnLastPage)
					{
						flag = true;
					}
				}
				else if (footer.PrintBetweenSections)
				{
					renderFooter = true;
				}
				renderFooter = flag;
			}
		}

		private void CheckForLastSectionOnPage(double heightToBeUsed, bool lastSection, ref bool lastSectionOnPage)
		{
			if (!lastSectionOnPage)
			{
				lastSectionOnPage = (this.SpanPages || this.OnPagePBEnd || (this.Done && lastSection));
				if (!lastSectionOnPage && heightToBeUsed != 1.7976931348623157E+308 && heightToBeUsed - this.Body.ItemPageSizes.Height < 0.01)
				{
					lastSectionOnPage = true;
				}
			}
		}

		private void WriteStartSectionItemToRPLStream(BinaryWriter spbifWriter)
		{
			Stream baseStream = spbifWriter.BaseStream;
			this.m_offset = baseStream.Position;
			spbifWriter.Write((byte)21);
			spbifWriter.Write((byte)22);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(this.m_reportSectionDef.ID);
			spbifWriter.Write((byte)2);
			spbifWriter.Write((float)this.m_reportSectionDef.Page.ColumnSpacing.ToMillimeters());
			spbifWriter.Write((byte)1);
			spbifWriter.Write(this.m_reportSectionDef.Page.Columns);
			spbifWriter.Write((byte)255);
			this.m_bodyOffset = baseStream.Position;
			spbifWriter.Write((byte)20);
		}

		private void WriteStartRectangleItemToRPLStream2008(BinaryWriter spbifWriter)
		{
			Stream baseStream = spbifWriter.BaseStream;
			this.m_offset = baseStream.Position;
			spbifWriter.Write((byte)10);
			spbifWriter.Write((byte)15);
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)255);
			spbifWriter.Write((byte)1);
			spbifWriter.Write((byte)255);
			spbifWriter.Write((byte)255);
		}

		private void WriteStartSectionItemToRPLOM(RPLWriter rplWriter)
		{
			this.m_rplReportSection = new RPLReportSection(1);
			this.m_rplReportSection.ID = this.m_reportSectionDef.ID;
			this.m_rplReportSection.ColumnSpacing = (float)this.m_reportSectionDef.Page.ColumnSpacing.ToMillimeters();
			this.m_rplReportSection.ColumnCount = this.m_reportSectionDef.Page.Columns;
		}

		private void WriteEndSectionItemToRPLStream(BinaryWriter spbifWriter)
		{
			spbifWriter.Write((byte)255);
			Stream baseStream = spbifWriter.BaseStream;
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(this.m_offset);
			spbifWriter.Write(this.m_itemsOnPage);
			this.m_body.WritePageItemRenderSizes(spbifWriter, this.m_bodyOffset);
			this.m_body.ItemRenderSizes = null;
			if (this.m_header != null)
			{
				this.m_header.WritePageItemRenderSizes(spbifWriter);
				this.m_header.ItemRenderSizes = null;
			}
			if (this.m_footer != null)
			{
				this.m_footer.WritePageItemRenderSizes(spbifWriter);
				this.m_footer.ItemRenderSizes = null;
			}
			this.m_offset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write((byte)255);
		}

		private void WriteEndRectangleItemToRPLStream2008(BinaryWriter spbifWriter)
		{
			Stream baseStream = spbifWriter.BaseStream;
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(this.m_offset);
			spbifWriter.Write(this.m_itemsOnPage);
			double num = 0.0;
			if (this.m_header != null)
			{
				this.m_header.WritePageItemRenderSizes(spbifWriter);
				num += this.m_header.ItemRenderSizes.Height;
				this.m_header.ItemRenderSizes = null;
			}
			this.m_body.ItemRenderSizes.Top += num;
			this.m_body.WritePageItemRenderSizes(spbifWriter);
			num += this.m_body.ItemRenderSizes.Height;
			this.m_body.ItemRenderSizes = null;
			if (this.m_footer != null)
			{
				this.m_footer.ItemRenderSizes.Top += num;
				this.m_footer.WritePageItemRenderSizes(spbifWriter);
				this.m_footer.ItemRenderSizes = null;
			}
			this.m_offset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write((byte)255);
		}

		private void WriteEndSectionItemToRPLOM(RPLWriter rplWriter)
		{
			RPLItemMeasurement rPLItemMeasurement = this.m_body.WritePageItemRenderSizes();
			this.m_body.ItemRenderSizes = null;
			this.m_rplReportSection.BodyArea = new RPLMeasurement();
			this.m_rplReportSection.BodyArea.Height = rPLItemMeasurement.Height;
			this.m_rplReportSection.BodyArea.Width = rPLItemMeasurement.Width;
			this.m_rplReportSection.BodyArea.Top = rPLItemMeasurement.Top;
			this.m_rplReportSection.BodyArea.Left = rPLItemMeasurement.Left;
			if (this.m_header != null)
			{
				this.m_rplReportSection.Header = this.m_header.WritePageItemRenderSizes();
				this.m_header.ItemRenderSizes = null;
			}
			if (this.m_footer != null)
			{
				this.m_rplReportSection.Footer = this.m_footer.WritePageItemRenderSizes();
				this.m_footer.ItemRenderSizes = null;
			}
		}
	}
}
