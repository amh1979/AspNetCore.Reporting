using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class PageBreakInfo
	{
		private PageBreakLocation m_breakLocation;

		private bool m_disabled;

		private bool m_resetPageNumber;

		private string m_reportItemName;

		internal PageBreakLocation BreakLocation
		{
			get
			{
				return this.m_breakLocation;
			}
		}

		internal bool Disabled
		{
			get
			{
				return this.m_disabled;
			}
		}

		internal bool ResetPageNumber
		{
			get
			{
				return this.m_resetPageNumber;
			}
		}

		internal string ReportItemName
		{
			get
			{
				return this.m_reportItemName;
			}
		}

		internal PageBreakInfo(PageBreak pageBreak, string reportItemName)
		{
			if (pageBreak != null)
			{
				this.m_breakLocation = pageBreak.BreakLocation;
				this.m_disabled = pageBreak.Instance.Disabled;
				this.m_resetPageNumber = pageBreak.Instance.ResetPageNumber;
				if (RenderingDiagnostics.Enabled)
				{
					this.m_reportItemName = reportItemName;
				}
			}
		}
	}
}
