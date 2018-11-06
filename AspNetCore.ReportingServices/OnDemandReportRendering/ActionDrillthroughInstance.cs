using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Specialized;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ActionDrillthroughInstance : BaseInstance
	{
		private bool m_isOldSnapshot;

		private AspNetCore.ReportingServices.ReportRendering.Action m_renderAction;

		private string m_reportName;

		private string m_drillthroughID;

		private string m_drillthroughUrl;

		private bool m_drillthroughUrlEvaluated;

		private ActionDrillthrough m_actionDef;

		private int m_index = -1;

		public string ReportName
		{
			get
			{
				if (this.m_reportName == null)
				{
					if (this.m_isOldSnapshot)
					{
						if (this.m_renderAction != null)
						{
							this.m_reportName = this.m_renderAction.DrillthroughPath;
						}
					}
					else
					{
						Global.Tracer.Assert(this.m_actionDef.ReportName != null, "(m_actionDef.ReportName != null)");
						if (!this.m_actionDef.ReportName.IsExpression)
						{
							this.m_reportName = this.m_actionDef.ReportName.Value;
						}
						else if (this.m_actionDef.Owner.ReportElementOwner == null || this.m_actionDef.Owner.ReportElementOwner.CriOwner == null)
						{
							ActionInfo owner = this.m_actionDef.Owner;
							this.m_reportName = this.m_actionDef.ActionItemDef.EvaluateDrillthroughReportName(this.ReportScopeInstance, owner.RenderingContext.OdpContext, owner.InstancePath, owner.ObjectType, owner.ObjectName);
						}
					}
				}
				return this.m_reportName;
			}
			set
			{
				ReportElement reportElementOwner = this.m_actionDef.Owner.ReportElementOwner;
				Global.Tracer.Assert(this.m_actionDef.ReportName != null, "(m_actionDef.ReportName != null)");
				if (!this.m_actionDef.Owner.IsChartConstruction && (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.None || (reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance && !this.m_actionDef.ReportName.IsExpression)))
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
				}
				this.m_reportName = value;
				this.m_drillthroughUrl = null;
			}
		}

		public string DrillthroughID
		{
			get
			{
				if (this.m_drillthroughID == null)
				{
					if (this.m_isOldSnapshot)
					{
						if (this.m_renderAction != null)
						{
							this.m_drillthroughID = this.m_renderAction.DrillthroughID;
						}
					}
					else if (this.ReportName != null)
					{
						this.m_drillthroughID = ((this.m_actionDef.Owner.ROMActionOwner != null) ? this.m_actionDef.Owner.ROMActionOwner.UniqueName : this.m_actionDef.Owner.InstancePath.UniqueName);
						this.m_drillthroughID = this.m_drillthroughID + ":" + this.m_index.ToString(CultureInfo.InvariantCulture);
					}
				}
				return this.m_drillthroughID;
			}
		}

		public string DrillthroughUrl
		{
			get
			{
				if (!this.m_drillthroughUrlEvaluated)
				{
					this.m_drillthroughUrlEvaluated = true;
					if (this.ReportName != null)
					{
						if (this.m_isOldSnapshot)
						{
							if (this.m_renderAction != null)
							{
								try
								{
									ReportUrlBuilder urlBuilder = this.m_renderAction.DrillthroughReport.GetUrlBuilder(null, true);
									urlBuilder.AddParameters(this.m_renderAction.DrillthroughParameters, UrlParameterType.ReportParameter);
									this.m_drillthroughUrl = urlBuilder.ToUri().AbsoluteUri;
								}
								catch (ItemNotFoundException)
								{
									this.m_drillthroughUrl = null;
								}
							}
						}
						else
						{
							try
							{
								NameValueCollection parameters = null;
								if (this.m_actionDef.Parameters != null)
								{
									parameters = this.m_actionDef.Parameters.ToNameValueCollection;
								}
								this.m_drillthroughUrl = ReportUrl.BuildDrillthroughUrl(this.m_actionDef.PathResolutionContext, this.ReportName, parameters);
							}
							catch (ItemNotFoundException)
							{
								this.m_drillthroughUrl = null;
							}
						}
					}
				}
				return this.m_drillthroughUrl;
			}
		}

		internal ActionDrillthroughInstance(IReportScope reportScope, ActionDrillthrough actionDef, int index)
			: base(reportScope)
		{
			this.m_isOldSnapshot = false;
			this.m_actionDef = actionDef;
			this.m_index = index;
		}

		internal ActionDrillthroughInstance(AspNetCore.ReportingServices.ReportRendering.Action renderAction)
			: base(null)
		{
			this.m_isOldSnapshot = true;
			this.m_renderAction = renderAction;
		}

		internal void Update(AspNetCore.ReportingServices.ReportRendering.Action newAction)
		{
			this.m_renderAction = newAction;
			this.ResetInstanceCache();
		}

		protected override void ResetInstanceCache()
		{
			this.m_reportName = null;
			this.m_drillthroughID = null;
			this.m_drillthroughUrl = null;
			this.m_drillthroughUrlEvaluated = false;
		}
	}
}
