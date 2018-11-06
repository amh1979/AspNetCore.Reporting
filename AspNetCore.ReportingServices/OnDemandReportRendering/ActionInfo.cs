using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class ActionInfo
	{
		private ActionCollection m_collection;

		private AspNetCore.ReportingServices.ReportRendering.ActionInfo m_renderAction;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Action m_actionDef;

		private bool m_isOldSnapshot;

		private IReportScope m_reportScope;

		private IInstancePath m_instancePath;

		private ReportElement m_reportElementOwner;

		private ObjectType m_objectType;

		private string m_objectName;

		private bool m_dynamic;

		protected bool m_chartConstruction;

		private RenderingContext m_renderingContext;

		private IROMActionOwner m_romActionOwner;

		public ActionCollection Actions
		{
			get
			{
				this.InitActions();
				return this.m_collection;
			}
		}

		internal bool IsOldSnapshot
		{
			get
			{
				return this.m_isOldSnapshot;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				return this.m_reportScope;
			}
		}

		internal RenderingContext RenderingContext
		{
			get
			{
				return this.m_renderingContext;
			}
		}

		internal IInstancePath InstancePath
		{
			get
			{
				return this.m_instancePath;
			}
		}

		internal ReportElement ReportElementOwner
		{
			get
			{
				return this.m_reportElementOwner;
			}
		}

		internal ObjectType ObjectType
		{
			get
			{
				return this.m_objectType;
			}
		}

		internal string ObjectName
		{
			get
			{
				return this.m_objectName;
			}
		}

		internal bool IsDynamic
		{
			get
			{
				return this.m_dynamic;
			}
			set
			{
				this.m_dynamic = value;
			}
		}

		internal bool IsChartConstruction
		{
			get
			{
				return this.m_chartConstruction;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Action ActionDef
		{
			get
			{
				return this.m_actionDef;
			}
			set
			{
				this.m_actionDef = value;
			}
		}

		internal IROMActionOwner ROMActionOwner
		{
			get
			{
				return this.m_romActionOwner;
			}
		}

		internal ActionInfo(RenderingContext renderingContext, IReportScope reportScope, AspNetCore.ReportingServices.ReportIntermediateFormat.Action actionDef, IInstancePath instancePath, ReportElement reportElementOwner, ObjectType objectType, string objectName, IROMActionOwner romActionOwner)
		{
			this.m_renderingContext = renderingContext;
			this.m_reportScope = reportScope;
			this.m_actionDef = actionDef;
			this.m_isOldSnapshot = false;
			this.m_instancePath = instancePath;
			this.m_reportElementOwner = reportElementOwner;
			this.m_objectType = objectType;
			this.m_objectName = objectName;
			this.m_romActionOwner = romActionOwner;
		}

		internal ActionInfo(RenderingContext renderingContext, AspNetCore.ReportingServices.ReportRendering.ActionInfo renderAction)
		{
			this.m_renderingContext = renderingContext;
			this.m_renderAction = renderAction;
			this.m_isOldSnapshot = true;
		}

		public Action CreateHyperlinkAction()
		{
			this.AssertValidCreateActionContext();
			this.InitActions();
			if (this.Actions.Count > 0)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItem = new AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem();
			actionItem.HyperLinkURL = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			this.m_actionDef.ActionItems.Add(actionItem);
			return this.Actions.Add(this, actionItem);
		}

		public Action CreateBookmarkLinkAction()
		{
			this.AssertValidCreateActionContext();
			this.InitActions();
			if (this.Actions.Count > 0)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItem = new AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem();
			actionItem.BookmarkLink = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			this.m_actionDef.ActionItems.Add(actionItem);
			return this.Actions.Add(this, actionItem);
		}

		public Action CreateDrillthroughAction()
		{
			this.AssertValidCreateActionContext();
			this.InitActions();
			if (this.Actions.Count > 0)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItem = new AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem();
			actionItem.DrillthroughReportName = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			this.m_actionDef.ActionItems.Add(actionItem);
			return this.Actions.Add(this, actionItem);
		}

		private void AssertValidCreateActionContext()
		{
			if (this.m_chartConstruction)
			{
				return;
			}
			if (this.m_dynamic && this.m_reportElementOwner.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance)
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackDynamicAction);
			}
			if (this.m_dynamic)
			{
				return;
			}
			if (this.m_reportElementOwner.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition)
			{
				return;
			}
			throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWritebackDynamicAction);
		}

		internal void Update(AspNetCore.ReportingServices.ReportRendering.ActionInfo newActionInfo)
		{
			this.m_collection.Update(newActionInfo);
		}

		internal virtual void SetNewContext()
		{
			if (this.m_collection != null)
			{
				this.m_collection.SetNewContext();
			}
		}

		internal bool ConstructActionDefinition()
		{
			if (this.m_collection != null && this.m_collection.Count != 0)
			{
				this.m_collection.ConstructActionDefinitions();
				return true;
			}
			return false;
		}

		private void InitActions()
		{
			if (this.m_collection == null)
			{
				if (this.IsOldSnapshot)
				{
					this.m_collection = new ActionCollection(this, this.m_renderAction.Actions);
				}
				else
				{
					this.m_collection = new ActionCollection(this, this.m_actionDef.ActionItems);
				}
			}
		}
	}
}
