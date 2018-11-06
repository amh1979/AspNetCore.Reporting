using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class SubReport : ReportItem
	{
		private Report m_report;

		private bool m_processedWithError;

		public Report Report
		{
			get
			{
				return this.m_report;
			}
		}

		public bool ProcessedWithError
		{
			get
			{
				return this.m_processedWithError;
			}
		}

		public bool NoRows
		{
			get
			{
				if (this.m_processedWithError)
				{
					return false;
				}
				Global.Tracer.Assert(null != this.m_report);
				if (this.m_report.ReportInstance != null)
				{
					return this.m_report.InstanceInfo.NoRows;
				}
				return true;
			}
		}

		public string NoRowMessage
		{
			get
			{
				ExpressionInfo noRows = ((AspNetCore.ReportingServices.ReportProcessing.SubReport)base.ReportItemDef).NoRows;
				if (noRows != null)
				{
					if (ExpressionInfo.Types.Constant == noRows.Type)
					{
						return noRows.Value;
					}
					if (base.InstanceInfo != null)
					{
						return ((SubReportInstanceInfo)base.InstanceInfo).NoRows;
					}
				}
				return null;
			}
		}

		internal SubReport(int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.SubReport reportItemDef, SubReportInstance reportItemInstance, RenderingContext renderingContext, Report innerReport, bool processedWithError)
			: base(null, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
			this.m_report = innerReport;
			this.m_processedWithError = processedWithError;
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (!base.SkipSearch && !this.NoRows && !this.ProcessedWithError)
			{
				Report report = this.Report;
				if (report != null)
				{
					return report.Body.Search(searchContext);
				}
				return false;
			}
			return false;
		}
	}
}
