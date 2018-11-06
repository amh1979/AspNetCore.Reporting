namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportUrlProperty : ReportProperty
	{
		private ReportUrl m_reportUrl;

		public ReportUrl Value
		{
			get
			{
				return this.m_reportUrl;
			}
		}

		internal ReportUrlProperty(bool isExpression, string expressionString, ReportUrl reportUrl)
			: base(isExpression, expressionString)
		{
			if (!isExpression)
			{
				this.m_reportUrl = reportUrl;
			}
		}
	}
}
