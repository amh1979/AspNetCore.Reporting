using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.Rendering.HtmlRenderer;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.Rendering.SPBProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class Word97Renderer : WordRenderer
	{
		protected List<SectionEntry> m_sections = new List<SectionEntry>();

		internal Word97Renderer(CreateAndRegisterStream createAndRegisterStream, AspNetCore.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, DeviceInfo deviceInfo, string reportName)
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
			bool flag2 = false;
			List<RPLReport> rplReportCache = new List<RPLReport>();
			while (!base.m_spbProcessing.Done)
			{
				if (!flag)
				{
					base.m_writer.WritePageBreak();
				}
				base.m_spbProcessing.GetNextPage(out base.m_rplReport);
				RPLPageContent rPLPageContent = base.m_rplReport.RPLPaginatedPages[0];
				RPLReportSection rPLReportSection = rPLPageContent.GetNextReportSection();
				bool flag3 = false;
				bool flag4 = true;
				while (rPLReportSection != null)
				{
					rPLItemMeasurement = rPLReportSection.Columns[0];
					float width = rPLReportSection.BodyArea.Width;
					RPLHeaderFooter footer = null;
					SectionEntry sectionEntry = null;
					if (!flag4 || this.m_sections.Count == 0 || string.CompareOrdinal(this.m_sections[this.m_sections.Count - 1].SectionId, rPLReportSection.ID) != 0)
					{
						if (RSTrace.RenderingTracer.TraceVerbose)
						{
							RSTrace.RenderingTracer.Trace("The left or right margin is either <0 or the sum exceeds the page width.");
						}
						sectionEntry = new SectionEntry(rPLReportSection);
						if (rPLReportSection.Footer != null)
						{
							footer = (rPLReportSection.Footer.Element as RPLHeaderFooter);
						}
						if (sectionEntry.HeaderMeasurement != null || sectionEntry.FooterMeasurement != null)
						{
							flag2 = true;
						}
						this.m_sections.Add(sectionEntry);
						base.CachePage(ref flag3, rplReportCache);
					}
					flag = base.SetFirstPageDimensions(flag, rPLPageContent, ref rPLPageLayout, ref leftMargin, ref rightMargin, ref num, ref title, ref author, ref description);
					num = base.RevisePageDimensions(leftMargin, rightMargin, num, width, autoFit);
					base.RenderHeaderBetweenSections(rPLReportSection, flag4);
					base.RenderBodyContent(width, rPLItemMeasurement);
					rPLReportSection = base.AdvanceToNextSection(rPLPageContent, rPLReportSection, ref flag4, this.m_sections[this.m_sections.Count - 1], footer, sectionEntry);
				}
				if (!base.m_spbProcessing.Done && !flag3)
				{
					base.m_rplReport.Release();
				}
			}
			base.m_writer.WriteParagraphEnd();
			base.m_writer.SetPageDimensions(base.m_pageHeight, num, leftMargin, rightMargin, rPLPageLayout.MarginTop, rPLPageLayout.MarginBottom);
			if (flag2)
			{
				base.m_inHeaderFooter = true;
				base.m_writer.InitHeaderFooter();
				bool flag5 = false;
				base.m_needsToResetTextboxes = true;
				for (int i = 0; i < this.m_sections.Count; i++)
				{
					RPLItemMeasurement headerMeasurement = this.m_sections[i].HeaderMeasurement;
					if (headerMeasurement != null)
					{
						base.RenderRectangle((RPLContainer)headerMeasurement.Element, 0f, true, headerMeasurement, new BorderContext(), false, true);
					}
					base.m_writer.FinishHeader(i);
					RPLItemMeasurement footerMeasurement = this.m_sections[i].FooterMeasurement;
					if (footerMeasurement != null)
					{
						base.RenderRectangle((RPLContainer)footerMeasurement.Element, 0f, true, footerMeasurement, new BorderContext(), false, true);
					}
					base.m_writer.FinishFooter(i);
					if (i == 0)
					{
						bool flag6 = headerMeasurement != null;
						bool flag7 = flag6 && !(headerMeasurement.Element.ElementPropsDef as RPLHeaderFooterPropsDef).PrintOnFirstPage;
						bool flag8 = footerMeasurement != null;
						bool flag9 = flag8 && !(footerMeasurement.Element.ElementPropsDef as RPLHeaderFooterPropsDef).PrintOnFirstPage;
						flag5 = ((flag7 || flag9) && (flag6 || flag8));
						if (flag5)
						{
							if (flag6 && !flag7)
							{
								base.RenderRectangle((RPLContainer)headerMeasurement.Element, 0f, true, headerMeasurement, new BorderContext(), false, true);
							}
							base.m_writer.FinishHeader(i, Word97Writer.HeaderFooterLocation.First);
							if (flag8 && !flag9)
							{
								base.RenderRectangle((RPLContainer)footerMeasurement.Element, 0f, true, footerMeasurement, new BorderContext(), false, true);
							}
							base.m_writer.FinishFooter(i, Word97Writer.HeaderFooterLocation.First);
						}
						base.m_needsToResetTextboxes = false;
					}
				}
				base.m_writer.FinishHeadersFooters(flag5);
				base.m_inHeaderFooter = false;
			}
			base.FinishRendering(rplReportCache, title, author, description);
			return true;
		}

		protected override bool RenderRectangleItemAndLines(RPLContainer rectangle, BorderContext borderContext, int y, PageTableCell cell, string linkToChildId, float runningLeft, bool rowUsed)
		{
			rowUsed = base.RenderRectangleItem(y, cell, borderContext, linkToChildId, rectangle, runningLeft, rowUsed);
			base.RenderLines(y, cell, borderContext);
			return rowUsed;
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
			base.RenderTextBox(textBox, inTablix, cellIndex, needsTable, style, measurement, notCanGrow, textBoxPropsDef, textBoxProperties, isSimple, textBoxValue, borderContext, oldCellIndex);
			base.RenderTextBoxProperties(inTablix, cellIndex, needsTable, style);
		}

		protected override void RenderTablixCell(RPLTablix tablix, float left, float[] widths, TablixGhostCell[] ghostCells, BorderContext borderContext, int nextCell, RPLTablixCell cell, List<RPLTablixMemberCell>.Enumerator omittedCells, bool lastCell)
		{
			RPLItemMeasurement tablixCellMeasurement = base.GetTablixCellMeasurement(cell, nextCell, widths, ghostCells, omittedCells, lastCell, tablix);
			base.RenderTablixCellItem(cell, widths, tablixCellMeasurement, left, borderContext);
			base.ClearTablixCellBorders(cell);
			base.FinishRenderingTablixCell(cell, widths, ghostCells, borderContext);
		}

		protected override void RenderRPLContainer(RPLElement element, bool inTablix, RPLItemMeasurement measurement, int cellIndex, BorderContext borderContext, bool hasBorder)
		{
			base.RenderRPLContainerContents(element, measurement, borderContext, inTablix, hasBorder);
			base.RenderRPLContainerProperties(element, inTablix, cellIndex);
		}
	}
}
