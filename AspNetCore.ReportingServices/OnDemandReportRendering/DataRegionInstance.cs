using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class DataRegionInstance : ReportItemInstance, IReportScopeInstance
	{
		private string m_noRowsMessage;

		private bool m_isNewContext = true;

		private bool? m_noRows;

		private string m_pageName;

		private bool m_pageNameEvaluated;

		private readonly DataRegion m_dataRegionDef;

		string IReportScopeInstance.UniqueName
		{
			get
			{
				return base.UniqueName;
			}
		}

		bool IReportScopeInstance.IsNewContext
		{
			get
			{
				return this.m_isNewContext;
			}
			set
			{
				this.m_isNewContext = value;
			}
		}

		IReportScope IReportScopeInstance.ReportScope
		{
			get
			{
				return base.m_reportScope;
			}
		}

		public string NoRowsMessage
		{
			get
			{
				if (this.m_noRowsMessage == null)
				{
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_noRowsMessage = ((AspNetCore.ReportingServices.ReportRendering.DataRegion)base.m_reportElementDef.RenderReportItem).NoRowMessage;
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)base.m_reportElementDef.ReportItemDef;
						if (dataRegion.NoRowsMessage != null)
						{
							if (!dataRegion.NoRowsMessage.IsExpression)
							{
								this.m_noRowsMessage = dataRegion.NoRowsMessage.StringValue;
							}
							else
							{
								this.m_noRowsMessage = dataRegion.EvaluateNoRowsMessage(this, base.m_reportElementDef.RenderingContext.OdpContext);
							}
						}
					}
				}
				return this.m_noRowsMessage;
			}
		}

		public bool NoRows
		{
			get
			{
				if (!this.m_noRows.HasValue)
				{
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_noRows = ((AspNetCore.ReportingServices.ReportRendering.DataRegion)base.m_reportElementDef.RenderReportItem).NoRows;
					}
					else
					{
						base.m_reportElementDef.RenderingContext.OdpContext.SetupContext(base.m_reportElementDef.ReportItemDef, this);
						this.m_noRows = ((AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)base.m_reportElementDef.ReportItemDef).NoRows;
					}
				}
				return this.m_noRows.Value;
			}
		}

		public override VisibilityInstance Visibility
		{
			get
			{
				if (base.m_reportElementDef.IsOldSnapshot && ((DataRegion)base.m_reportElementDef).DataRegionType == DataRegion.Type.List)
				{
					return null;
				}
				return base.Visibility;
			}
		}

		public string PageName
		{
			get
			{
				if (!this.m_pageNameEvaluated)
				{
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_pageName = null;
					}
					else
					{
						this.m_pageNameEvaluated = true;
						AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)base.m_reportElementDef.ReportItemDef;
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo pageName = dataRegion.PageName;
						if (pageName != null)
						{
							if (pageName.IsExpression)
							{
								this.m_pageName = dataRegion.EvaluatePageName(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext);
							}
							else
							{
								this.m_pageName = pageName.StringValue;
							}
						}
					}
				}
				return this.m_pageName;
			}
		}

		internal DataRegionInstance(DataRegion reportItemDef)
			: base(reportItemDef)
		{
			this.m_dataRegionDef = reportItemDef;
		}

		internal override void SetNewContext()
		{
			this.m_isNewContext = true;
			this.m_noRowsMessage = null;
			this.m_noRows = null;
			this.m_pageNameEvaluated = false;
			this.m_pageName = null;
			base.SetNewContext();
		}

		internal void DoneSettingScopeID()
		{
			if (base.m_reportElementDef.RenderingContext.OdpContext.QueryRestartInfo.QueryRestartPosition.Count > 0)
			{
				base.m_reportElementDef.RenderingContext.OdpContext.QueryRestartInfo.EnableQueryRestart();
				base.m_reportElementDef.RenderingContext.OdpContext.SetupContext(this.m_dataRegionDef.ReportItemDef, (IReportScopeInstance)this.m_dataRegionDef.Instance);
				base.m_reportElementDef.RenderingContext.OdpContext.QueryRestartInfo.RomBasedRestart();
				return;
			}
			throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidScopeIDNotSet, this.m_dataRegionDef.Name);
		}
	}
}
