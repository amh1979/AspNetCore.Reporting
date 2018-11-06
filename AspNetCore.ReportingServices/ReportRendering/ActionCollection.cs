using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ActionCollection
	{
		private MemberBase m_members;

		public Action this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					Action action = null;
					if (this.IsCustomControl)
					{
						action = (this.Processing.m_actions[index] as Action);
					}
					else
					{
						if (this.Rendering.m_actions != null)
						{
							action = this.Rendering.m_actions[index];
						}
						if (action == null)
						{
							ActionItem actionItem = this.Rendering.m_actionList[index];
							ActionItemInstance actionItemInstance = null;
							if (this.Rendering.m_actionInstanceList != null && actionItem.ComputedIndex >= 0)
							{
								actionItemInstance = this.Rendering.m_actionInstanceList[actionItem.ComputedIndex];
							}
							string drillthroughId = this.Rendering.m_ownerUniqueName + ":" + index.ToString(CultureInfo.InvariantCulture);
							action = new Action(actionItem, actionItemInstance, drillthroughId, this.Rendering.m_renderingContext);
							if (this.Rendering.m_renderingContext.CacheState)
							{
								if (this.Rendering.m_actions == null)
								{
									this.Rendering.m_actions = new Action[this.Count];
								}
								this.Rendering.m_actions[index] = action;
							}
						}
					}
					return action;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.m_actions.Count;
				}
				return this.Rendering.m_actionList.Count;
			}
		}

		private bool IsCustomControl
		{
			get
			{
				return this.m_members.IsCustomControl;
			}
		}

		private ActionCollectionRendering Rendering
		{
			get
			{
				Global.Tracer.Assert(!this.m_members.IsCustomControl);
				ActionCollectionRendering actionCollectionRendering = this.m_members as ActionCollectionRendering;
				if (actionCollectionRendering == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionCollectionRendering;
			}
		}

		private ActionCollectionProcessing Processing
		{
			get
			{
				Global.Tracer.Assert(this.m_members.IsCustomControl);
				ActionCollectionProcessing actionCollectionProcessing = this.m_members as ActionCollectionProcessing;
				if (actionCollectionProcessing == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionCollectionProcessing;
			}
		}

		public ActionCollection()
		{
			this.m_members = new ActionCollectionProcessing();
			Global.Tracer.Assert(this.IsCustomControl);
			this.Processing.m_actions = new ArrayList();
		}

		internal ActionCollection(ActionItemList actionItemList, ActionItemInstanceList actionItemInstanceList, string ownerUniqueName, RenderingContext renderingContext)
		{
			this.m_members = new ActionCollectionRendering();
			Global.Tracer.Assert(!this.IsCustomControl);
			this.Rendering.m_actionList = actionItemList;
			this.Rendering.m_actionInstanceList = actionItemInstanceList;
			this.Rendering.m_renderingContext = renderingContext;
			this.Rendering.m_ownerUniqueName = ownerUniqueName;
		}

		public void Add(Action action)
		{
			if (!this.IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (action == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "action");
			}
			int count = this.Processing.m_actions.Count;
			if (2 <= count)
			{
				if (action.Label == null)
				{
					throw new ReportRenderingException(ErrorCode.rrInvalidActionLabel);
				}
			}
			else if (1 == count)
			{
				if (action.Label == null)
				{
					throw new ReportRenderingException(ErrorCode.rrInvalidActionLabel);
				}
				if (((Action)this.Processing.m_actions[0]).Label == null)
				{
					throw new ReportRenderingException(ErrorCode.rrInvalidActionLabel);
				}
			}
			this.Processing.m_actions.Add(action);
		}

		internal ActionCollection DeepClone()
		{
			if (!this.IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			ActionCollection actionCollection = new ActionCollection();
			Global.Tracer.Assert(this.m_members != null && this.m_members is ActionCollectionProcessing);
			actionCollection.m_members = this.Processing.DeepClone();
			return actionCollection;
		}
	}
}
