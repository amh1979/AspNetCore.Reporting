using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportSectionCollection : ReportElementCollectionBase<ReportSection>
	{
		private ReportSection[] m_sections;

		public override ReportSection this[int index]
		{
			get
			{
				if (0 <= index && index < this.Count)
				{
					return this.m_sections[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_sections.Length;
			}
		}

		internal ReportSectionCollection(Report reportDef)
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection> reportSections = reportDef.ReportDef.ReportSections;
			this.m_sections = new ReportSection[reportSections.Count];
			for (int i = 0; i < this.m_sections.Length; i++)
			{
				this.m_sections[i] = new ReportSection(reportDef, reportSections[i], i);
			}
		}

		internal ReportSectionCollection(Report reportDef, AspNetCore.ReportingServices.ReportRendering.Report renderReport)
		{
			this.m_sections = new ReportSection[1];
			this.m_sections[0] = new ReportSection(reportDef, renderReport, 0);
		}

		internal void SetNewContext()
		{
			for (int i = 0; i < this.m_sections.Length; i++)
			{
				this.m_sections[i].SetNewContext();
			}
		}
	}
}
