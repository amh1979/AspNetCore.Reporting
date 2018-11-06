using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TablixInstance : DataRegionInstance
	{
		private Tablix m_owner;

		private ReportSize m_topMargin;

		private ReportSize m_bottomMargin;

		private ReportSize m_leftMargin;

		private ReportSize m_rightMargin;

		public ReportSize TopMargin
		{
			get
			{
				return this.GetOrEvaluateMarginProperty(ref this.m_topMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.TopMargin);
			}
		}

		public ReportSize BottomMargin
		{
			get
			{
				return this.GetOrEvaluateMarginProperty(ref this.m_bottomMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.BottomMargin);
			}
		}

		public ReportSize LeftMargin
		{
			get
			{
				return this.GetOrEvaluateMarginProperty(ref this.m_leftMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.LeftMargin);
			}
		}

		public ReportSize RightMargin
		{
			get
			{
				return this.GetOrEvaluateMarginProperty(ref this.m_rightMargin, AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition.RightMargin);
			}
		}

		internal TablixInstance(Tablix reportItemDef)
			: base(reportItemDef)
		{
			this.m_owner = reportItemDef;
		}

		private ReportSize GetOrEvaluateMarginProperty(ref ReportSize property, AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix.MarginPosition marginPosition)
		{
			if (this.m_owner.IsOldSnapshot)
			{
				return null;
			}
			if (property == null)
			{
				property = new ReportSize(this.m_owner.TablixDef.EvaluateTablixMargin(this.ReportScopeInstance, marginPosition, base.m_reportElementDef.RenderingContext.OdpContext));
			}
			return property;
		}

		internal override void SetNewContext()
		{
			this.m_topMargin = (this.m_bottomMargin = (this.m_leftMargin = (this.m_rightMargin = null)));
			base.SetNewContext();
		}
	}
}
