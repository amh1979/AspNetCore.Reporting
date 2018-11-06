using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ActionDrillthrough
	{
		private ReportStringProperty m_reportName;

		private ParameterCollection m_parameters;

		private ActionDrillthroughInstance m_instance;

		private AspNetCore.ReportingServices.ReportRendering.Action m_renderAction;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem m_actionItemDef;

		private ActionInfo m_owner;

		private int m_index = -1;

		public ReportStringProperty ReportName
		{
			get
			{
				if (this.m_reportName == null)
				{
					if (this.IsOldSnapshot)
					{
						this.m_reportName = new ReportStringProperty(this.m_renderAction.ActionDefinition.DrillthroughReportName);
					}
					else
					{
						this.m_reportName = new ReportStringProperty(this.m_actionItemDef.DrillthroughReportName);
					}
				}
				return this.m_reportName;
			}
		}

		public ParameterCollection Parameters
		{
			get
			{
				if (this.m_parameters == null)
				{
					if (this.IsOldSnapshot)
					{
						NameValueCollection drillthroughParameters = this.m_renderAction.DrillthroughParameters;
						if (drillthroughParameters != null)
						{
							this.m_parameters = new ParameterCollection(this, drillthroughParameters, this.m_renderAction.DrillthroughParameterNameObjectCollection, this.m_renderAction.DrillthroughParameterValueList, this.m_renderAction.ActionInstance);
						}
					}
					else if (this.m_actionItemDef.DrillthroughParameters != null)
					{
						this.m_parameters = new ParameterCollection(this, this.m_actionItemDef.DrillthroughParameters);
					}
				}
				return this.m_parameters;
			}
		}

		private bool IsOldSnapshot
		{
			get
			{
				return this.m_owner.IsOldSnapshot;
			}
		}

		public ActionDrillthroughInstance Instance
		{
			get
			{
				if (this.m_owner.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					if (this.IsOldSnapshot)
					{
						this.m_instance = new ActionDrillthroughInstance(this.m_renderAction);
					}
					else
					{
						this.m_instance = new ActionDrillthroughInstance(this.m_owner.ReportScope, this, this.m_index);
					}
				}
				ReportItem reportItem = this.m_owner.ReportElementOwner as ReportItem;
				if (reportItem != null)
				{
					reportItem.CriEvaluateInstance();
				}
				return this.m_instance;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem ActionItemDef
		{
			get
			{
				return this.m_actionItemDef;
			}
		}

		internal ActionInfo Owner
		{
			get
			{
				return this.m_owner;
			}
		}

		internal ICatalogItemContext PathResolutionContext
		{
			get
			{
				return this.m_owner.RenderingContext.OdpContext.TopLevelContext.ReportContext;
			}
		}

		internal ActionDrillthrough(ActionInfo owner, AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItemDef, int index)
		{
			this.m_owner = owner;
			this.m_actionItemDef = actionItemDef;
			this.m_index = index;
		}

		internal ActionDrillthrough(ActionInfo owner, AspNetCore.ReportingServices.ReportRendering.Action renderAction)
		{
			this.m_owner = owner;
			this.m_renderAction = renderAction;
		}

		public void RegisterDrillthroughAction()
		{
			string drillthroughID = this.Instance.DrillthroughID;
			if (drillthroughID != null)
			{
				this.m_owner.RenderingContext.AddDrillthroughAction(drillthroughID, this.Instance.ReportName, (this.Parameters != null) ? this.Parameters.ParametersNameObjectCollection : null);
			}
		}

		public Parameter CreateParameter(string name)
		{
			if (!this.m_owner.IsChartConstruction)
			{
				if (this.m_owner.IsDynamic && this.m_owner.ReportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance)
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackDynamicAction);
				}
				if (!this.m_owner.IsDynamic && this.m_owner.ReportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Definition)
				{
					throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackNonDynamicAction);
				}
			}
			if (this.Parameters == null)
			{
				this.m_actionItemDef.DrillthroughParameters = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue>();
				Global.Tracer.Assert(this.Parameters != null, "(Parameters != null)");
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue parameterValue = new AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterValue();
			parameterValue.Name = name;
			this.m_actionItemDef.DrillthroughParameters.Add(parameterValue);
			if (!this.m_owner.IsChartConstruction && this.m_owner.ReportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Instance)
			{
				parameterValue.Value = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			return this.Parameters.Add(this, parameterValue);
		}

		internal void Update(AspNetCore.ReportingServices.ReportRendering.Action newAction)
		{
			if (this.m_instance != null)
			{
				this.m_instance.Update(newAction);
			}
			if (newAction != null)
			{
				this.m_renderAction = newAction;
			}
			if (this.m_parameters != null)
			{
				this.m_parameters.Update(this.m_renderAction.DrillthroughParameters, this.m_renderAction.DrillthroughParameterNameObjectCollection, this.m_renderAction.ActionInstance);
			}
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_parameters != null)
			{
				this.m_parameters.SetNewContext();
			}
		}

		internal void ConstructDrillthoughDefinition()
		{
			ActionDrillthroughInstance instance = this.Instance;
			Global.Tracer.Assert(instance != null, "(instance != null)");
			if (instance.ReportName != null)
			{
				this.m_actionItemDef.DrillthroughReportName = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.ReportName);
			}
			else
			{
				this.m_actionItemDef.DrillthroughReportName = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			this.m_reportName = null;
			if (this.m_parameters != null)
			{
				this.m_parameters.ConstructParameterDefinitions();
			}
		}
	}
}
