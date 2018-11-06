using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class SubReportInstance : ReportItemInstance
	{
		private string m_noRowsMessageExpressionResult;

		public bool ProcessedWithError
		{
			get
			{
				return this.SubReportDefinition.ProcessedWithError;
			}
		}

		public SubReportErrorCodes ErrorCode
		{
			get
			{
				return this.SubReportDefinition.ErrorCode;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.SubReportDefinition.ErrorMessage;
			}
		}

		public string NoRowsMessage
		{
			get
			{
				if (this.m_noRowsMessageExpressionResult == null)
				{
					if (this.SubReportDefinition.IsOldSnapshot)
					{
						this.m_noRowsMessageExpressionResult = ((AspNetCore.ReportingServices.ReportRendering.SubReport)this.SubReportDefinition.RenderReportItem).NoRowMessage;
					}
					else if (!this.SubReportDefinition.ProcessedWithError)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport = (AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport)this.SubReportDefinition.ReportItemDef;
						this.m_noRowsMessageExpressionResult = subReport.EvaulateNoRowMessage(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext);
					}
				}
				return this.m_noRowsMessageExpressionResult;
			}
		}

		public bool NoRows
		{
			get
			{
				return this.SubReportDefinition.NoRows;
			}
		}

		private SubReport SubReportDefinition
		{
			get
			{
				return base.m_reportElementDef as SubReport;
			}
		}

		internal SubReportInstance(SubReport reportItemDef)
			: base(reportItemDef)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_noRowsMessageExpressionResult = null;
		}
	}
}
