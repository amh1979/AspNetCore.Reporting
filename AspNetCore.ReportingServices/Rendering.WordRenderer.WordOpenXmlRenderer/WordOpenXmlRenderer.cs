using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.HtmlRenderer;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.SPBProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer
{
	internal sealed class WordOpenXmlRenderer : WordRenderer
	{
		internal WordOpenXmlRenderer(CreateAndRegisterStream createAndRegisterStream, AspNetCore.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, DeviceInfo deviceInfo, string reportName)
			: base(createAndRegisterStream, spbProcessing, writer, deviceInfo, reportName)
		{
		}

		internal override bool Render()
		{
			RPLItemMeasurement rPLItemMeasurement = null;
			bool flag = true;
			string author = "";
			string title = "";
			string description = "";
			AutoFit autoFit = base.m_writer.AutoFit;
			float num = 0f;
			float leftMargin = 0f;
			float rightMargin = 0f;
			RPLPageLayout rPLPageLayout = null;
			List<RPLReport> rplReportCache = new List<RPLReport>();
			bool flag2 = false;
			bool flag3 = false;
			SectionEntry sectionEntry = null;
			while (!base.m_spbProcessing.Done)
			{
				if (!flag)
				{
					base.m_writer.WritePageBreak();
				}
				base.m_spbProcessing.GetNextPage(out base.m_rplReport);
				RPLPageContent rPLPageContent = base.m_rplReport.RPLPaginatedPages[0];
				RPLReportSection rPLReportSection = rPLPageContent.GetNextReportSection();
				bool flag4 = false;
				bool flag5 = true;
				while (rPLReportSection != null)
				{
					rPLItemMeasurement = rPLReportSection.Columns[0];
					float width = rPLReportSection.BodyArea.Width;
					RPLHeaderFooter footer = null;
					SectionEntry se = null;
					if (!flag5 || sectionEntry == null || string.CompareOrdinal(sectionEntry.SectionId, rPLReportSection.ID) != 0)
					{
						if (RSTrace.RenderingTracer.TraceVerbose)
						{
							RSTrace.RenderingTracer.Trace("The left or right margin is either <0 or the sum exceeds the page width.");
						}
						sectionEntry = (se = this.RenderHeaderFooters(rPLReportSection, flag5, ref flag4, rplReportCache, ref footer, ref flag2, ref flag3));
					}
					flag = base.SetFirstPageDimensions(flag, rPLPageContent, ref rPLPageLayout, ref leftMargin, ref rightMargin, ref num, ref title, ref author, ref description);
					num = base.RevisePageDimensions(leftMargin, rightMargin, num, width, autoFit);
					base.RenderHeaderBetweenSections(rPLReportSection, flag5);
					base.RenderBodyContent(width, rPLItemMeasurement);
					rPLReportSection = base.AdvanceToNextSection(rPLPageContent, rPLReportSection, ref flag5, sectionEntry, footer, se);
				}
				if (!base.m_spbProcessing.Done && !flag4)
				{
					base.m_rplReport.Release();
				}
			}
			base.m_writer.WriteParagraphEnd();
			base.m_writer.SetPageDimensions(base.m_pageHeight, num, leftMargin, rightMargin, rPLPageLayout.MarginTop, rPLPageLayout.MarginBottom);
			base.FinishRendering(rplReportCache, title, author, description);
			return true;
		}

		protected override void RenderTablixCell(RPLTablix tablix, float left, float[] widths, TablixGhostCell[] ghostCells, BorderContext borderContext, int nextCell, RPLTablixCell cell, List<RPLTablixMemberCell>.Enumerator omittedCells, bool lastCell)
		{
			RPLItemMeasurement tablixCellMeasurement = base.GetTablixCellMeasurement(cell, nextCell, widths, ghostCells, omittedCells, lastCell, tablix);
			base.ClearTablixCellBorders(cell);
			base.m_writer.ApplyCellBorderContext(borderContext);
			base.RenderTablixCellItem(cell, widths, tablixCellMeasurement, left, borderContext);
			base.FinishRenderingTablixCell(cell, widths, ghostCells, borderContext);
		}

		protected override void RenderTextBox(RPLTextBox textBox, RPLItemMeasurement measurement, int cellIndex, float left, BorderContext borderContext, bool inTablix, bool hasBorder)
		{
			RPLTextBoxPropsDef textBoxPropsDef = default(RPLTextBoxPropsDef);
			bool isSimple = default(bool);
			string textBoxValue = default(string);
			bool notCanGrow = default(bool);
			bool needsTable = default(bool);
			RPLElementStyle style = default(RPLElementStyle);
			int oldCellIndex = default(int);
			RPLTextBoxProps textBoxProperties = base.GetTextBoxProperties(textBox, out textBoxPropsDef, out isSimple, out textBoxValue, inTablix, out notCanGrow, hasBorder, cellIndex, out needsTable, out style, out oldCellIndex);
			base.RenderTextBoxProperties(inTablix, cellIndex, needsTable, style);
			base.RenderTextBox(textBox, inTablix, cellIndex, needsTable, style, measurement, notCanGrow, textBoxPropsDef, textBoxProperties, isSimple, textBoxValue, borderContext, oldCellIndex);
		}

		protected override bool RenderRectangleItemAndLines(RPLContainer rectangle, BorderContext borderContext, int y, PageTableCell cell, string linkToChildId, float runningLeft, bool rowUsed)
		{
			base.RenderLines(y, cell, borderContext);
			rowUsed = base.RenderRectangleItem(y, cell, borderContext, linkToChildId, rectangle, runningLeft, rowUsed);
			return rowUsed;
		}

		private SectionEntry RenderHeaderFooters(RPLReportSection section, bool firstSection, ref bool pageCached, List<RPLReport> rplReportCache, ref RPLHeaderFooter footer, ref bool hasHeaderSoFar, ref bool hasFooterSoFar)
		{
			SectionEntry result = new SectionEntry(section);
			if (section.Footer != null)
			{
				footer = (section.Footer.Element as RPLHeaderFooter);
				if (footer.Children != null && footer.Children.Length != 0)
				{
					hasFooterSoFar = true;
				}
			}
			if (section.Header != null)
			{
				RPLHeaderFooter rPLHeaderFooter = section.Header.Element as RPLHeaderFooter;
				if (rPLHeaderFooter.Children != null && rPLHeaderFooter.Children.Length != 0)
				{
					hasHeaderSoFar = true;
				}
			}
			base.CachePage(ref pageCached, rplReportCache);
			base.m_inHeaderFooter = true;
			if (firstSection)
			{
				base.m_needsToResetTextboxes = true;
			}
			RPLItemMeasurement header = section.Header;
			if (hasHeaderSoFar)
			{
				base.m_writer.StartHeader();
				if (header != null)
				{
					base.RenderRectangle((RPLContainer)header.Element, 0f, true, header, new BorderContext(), false, true);
				}
				base.m_writer.FinishHeader();
			}
			RPLItemMeasurement footer2 = section.Footer;
			if (hasFooterSoFar)
			{
				base.m_writer.StartFooter();
				if (footer2 != null)
				{
					base.RenderRectangle((RPLContainer)footer2.Element, 0f, true, footer2, new BorderContext(), false, true);
				}
				base.m_writer.FinishFooter();
			}
			bool flag = false;
			if (firstSection)
			{
				bool flag2 = header != null;
				bool flag3 = flag2 && !(header.Element.ElementPropsDef as RPLHeaderFooterPropsDef).PrintOnFirstPage;
				bool flag4 = footer2 != null;
				bool flag5 = flag4 && !(footer2.Element.ElementPropsDef as RPLHeaderFooterPropsDef).PrintOnFirstPage;
				if ((flag3 || flag5) && (flag2 || flag4))
				{
					if (hasHeaderSoFar)
					{
						base.m_writer.StartHeader(true);
						if (flag2 && !flag3)
						{
							base.RenderRectangle((RPLContainer)header.Element, 0f, true, header, new BorderContext(), false, true);
						}
						base.m_writer.FinishHeader();
					}
					if (hasFooterSoFar)
					{
						base.m_writer.StartFooter(true);
						if (flag4 && !flag5)
						{
							base.RenderRectangle((RPLContainer)footer2.Element, 0f, true, footer2, new BorderContext(), false, true);
						}
						base.m_writer.FinishFooter();
					}
					base.m_writer.HasTitlePage = true;
				}
				base.m_needsToResetTextboxes = false;
			}
			base.m_inHeaderFooter = false;
			return result;
		}

		protected override void RenderRPLContainer(RPLElement element, bool inTablix, RPLItemMeasurement measurement, int cellIndex, BorderContext borderContext, bool hasBorder)
		{
			base.RenderRPLContainerProperties(element, inTablix, cellIndex);
			base.RenderRPLContainerContents(element, measurement, borderContext, inTablix, hasBorder);
		}
	}
}
