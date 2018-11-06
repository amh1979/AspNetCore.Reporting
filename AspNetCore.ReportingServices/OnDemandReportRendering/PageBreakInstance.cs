using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class PageBreakInstance : BaseInstance
	{
		private bool? m_resetPageNumber;

		private bool? m_disabled;

		private PageBreak m_pageBreakDef;

		public bool Disabled
		{
			get
			{
				if (!this.m_disabled.HasValue)
				{
					if (this.m_pageBreakDef.IsOldSnapshot)
					{
						this.m_disabled = false;
					}
					else
					{
						ExpressionInfo disabled = this.m_pageBreakDef.PageBreakDef.Disabled;
						if (disabled != null)
						{
							if (disabled.IsExpression)
							{
								this.m_disabled = this.m_pageBreakDef.PageBreakDef.EvaluateDisabled(this.ReportScopeInstance, this.m_pageBreakDef.RenderingContext.OdpContext, this.m_pageBreakDef.PageBreakOwner);
							}
							else
							{
								this.m_disabled = disabled.BoolValue;
							}
						}
						else
						{
							this.m_disabled = false;
						}
					}
				}
				return this.m_disabled.Value;
			}
		}

		public bool ResetPageNumber
		{
			get
			{
				if (!this.m_resetPageNumber.HasValue)
				{
					if (this.m_pageBreakDef.IsOldSnapshot)
					{
						this.m_resetPageNumber = false;
					}
					else
					{
						ExpressionInfo resetPageNumber = this.m_pageBreakDef.PageBreakDef.ResetPageNumber;
						if (resetPageNumber != null)
						{
							if (resetPageNumber.IsExpression)
							{
								this.m_resetPageNumber = this.m_pageBreakDef.PageBreakDef.EvaluateResetPageNumber(this.ReportScopeInstance, this.m_pageBreakDef.RenderingContext.OdpContext, this.m_pageBreakDef.PageBreakOwner);
							}
							else
							{
								this.m_resetPageNumber = resetPageNumber.BoolValue;
							}
						}
						else
						{
							this.m_resetPageNumber = false;
						}
					}
				}
				return this.m_resetPageNumber.Value;
			}
		}

		internal PageBreakInstance(IReportScope reportScope, PageBreak pageBreakDef)
			: base(reportScope)
		{
			this.m_pageBreakDef = pageBreakDef;
		}

		protected override void ResetInstanceCache()
		{
			this.m_resetPageNumber = null;
			this.m_disabled = null;
		}
	}
}
