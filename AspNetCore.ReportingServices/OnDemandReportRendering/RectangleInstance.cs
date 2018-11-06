using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RectangleInstance : ReportItemInstance
	{
		private bool m_pageNameEvaluated;

		private string m_pageName;

		public override VisibilityInstance Visibility
		{
			get
			{
				if (((Rectangle)base.m_reportElementDef).IsListContentsRectangle)
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
						AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle rectangle = (AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)base.m_reportElementDef.ReportItemDef;
						ExpressionInfo pageName = rectangle.PageName;
						if (pageName != null)
						{
							if (pageName.IsExpression)
							{
								this.m_pageName = rectangle.EvaluatePageName(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext);
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

		internal RectangleInstance(Rectangle reportItemDef)
			: base(reportItemDef)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_pageNameEvaluated = false;
			this.m_pageName = null;
		}
	}
}
