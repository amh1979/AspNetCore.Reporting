using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportInstance : BaseInstance, IReportScopeInstance
	{
		private Report m_reportDef;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance m_reportInstance;

		private string m_language;

		private bool m_isNewContext = true;

		private string m_initialPageName;

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

		public string UniqueName
		{
			get
			{
				if (this.m_reportDef.IsOldSnapshot)
				{
					return this.m_reportDef.RenderReport.UniqueName;
				}
				return InstancePathItem.GenerateInstancePathString(this.m_reportDef.ReportDef.InstancePath) + "xA";
			}
		}

		public string Language
		{
			get
			{
				if (this.m_language == null)
				{
					if (this.m_reportDef.IsOldSnapshot)
					{
						return this.m_reportDef.RenderReport.ReportLanguage;
					}
					this.m_language = this.m_reportInstance.Language;
				}
				return this.m_language;
			}
		}

		public int AutoRefresh
		{
			get
			{
				if (this.m_reportDef.IsOldSnapshot)
				{
					return this.m_reportDef.RenderReport.AutoRefresh;
				}
				return this.m_reportDef.ReportDef.EvaluateAutoRefresh(this, this.m_reportDef.RenderingContext.OdpContext);
			}
		}

		public string InitialPageName
		{
			get
			{
				if (this.m_reportDef.IsOldSnapshot)
				{
					return null;
				}
				ExpressionInfo initialPageName = this.m_reportDef.ReportDef.InitialPageName;
				if (initialPageName != null)
				{
					if (!initialPageName.IsExpression)
					{
						this.m_initialPageName = initialPageName.StringValue;
					}
					else
					{
						this.m_initialPageName = this.m_reportDef.ReportDef.EvaluateInitialPageName(this, this.m_reportDef.RenderingContext.OdpContext);
					}
				}
				return this.m_initialPageName;
			}
		}

		internal Report ReportDef
		{
			get
			{
				return this.m_reportDef;
			}
		}

		internal ReportInstance(Report reportDef, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
			: base(null)
		{
			this.m_reportDef = reportDef;
			this.m_reportInstance = reportInstance;
		}

		internal ReportInstance(Report reportDef)
			: base(null)
		{
			this.m_reportDef = reportDef;
			this.m_reportInstance = null;
		}

		protected override void ResetInstanceCache()
		{
		}

		public void ResetContext()
		{
			this.m_reportDef.SetNewContext();
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			this.m_isNewContext = true;
		}
	}
}
