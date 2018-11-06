using AspNetCore.ReportingServices.ReportProcessing;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ActionInfo
	{
		private MemberBase m_members;

		public ActionCollection Actions
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.m_actionCollection;
				}
				ActionCollection actionCollection = this.Rendering.m_actionCollection;
				if (this.Rendering.m_actionCollection == null)
				{
					ActionItemInstanceList actionItemInstanceList = null;
					if (this.Rendering.m_actionInfoInstance != null)
					{
						actionItemInstanceList = this.Rendering.m_actionInfoInstance.ActionItemsValues;
					}
					actionCollection = new ActionCollection(this.Rendering.m_actionInfoDef.ActionItems, actionItemInstanceList, this.Rendering.m_ownerUniqueName, this.Rendering.m_renderingContext);
					if (this.Rendering.m_renderingContext.CacheState)
					{
						this.Rendering.m_actionCollection = actionCollection;
					}
				}
				return actionCollection;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.m_actionCollection = value;
			}
		}

		public ActionStyle Style
		{
			get
			{
				if (this.IsCustomControl)
				{
					return this.Processing.m_style;
				}
				if (this.Rendering.m_actionInfoDef.StyleClass == null)
				{
					return null;
				}
				ActionStyle actionStyle = this.Rendering.m_style;
				if (this.Rendering.m_style == null)
				{
					actionStyle = new ActionStyle(this, this.Rendering.m_renderingContext);
					if (this.Rendering.m_renderingContext.CacheState)
					{
						this.Rendering.m_style = actionStyle;
					}
				}
				return actionStyle;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.m_style = value;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.Action ActionInfoDef
		{
			get
			{
				return this.Rendering.m_actionInfoDef;
			}
		}

		internal ActionInstance ActionInfoInstance
		{
			get
			{
				return this.Rendering.m_actionInfoInstance;
			}
		}

		private bool IsCustomControl
		{
			get
			{
				return this.m_members.IsCustomControl;
			}
		}

		private ActionInfoRendering Rendering
		{
			get
			{
				Global.Tracer.Assert(!this.m_members.IsCustomControl);
				ActionInfoRendering actionInfoRendering = this.m_members as ActionInfoRendering;
				if (actionInfoRendering == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionInfoRendering;
			}
		}

		private ActionInfoProcessing Processing
		{
			get
			{
				Global.Tracer.Assert(this.m_members.IsCustomControl);
				ActionInfoProcessing actionInfoProcessing = this.m_members as ActionInfoProcessing;
				if (actionInfoProcessing == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionInfoProcessing;
			}
		}

		public ActionInfo()
		{
			this.m_members = new ActionInfoProcessing();
			Global.Tracer.Assert(this.m_members.IsCustomControl);
		}

		internal ActionInfo(AspNetCore.ReportingServices.ReportProcessing.Action actionDef, ActionInstance actionInstance, string ownerUniqueName, RenderingContext renderingContext)
		{
			this.m_members = new ActionInfoRendering();
			Global.Tracer.Assert(!this.m_members.IsCustomControl);
			this.Rendering.m_actionInfoDef = actionDef;
			this.Rendering.m_actionInfoInstance = actionInstance;
			this.Rendering.m_renderingContext = renderingContext;
			this.Rendering.m_ownerUniqueName = ownerUniqueName;
		}

		internal ActionInfo DeepClone()
		{
			if (!this.IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			ActionInfo actionInfo = new ActionInfo();
			Global.Tracer.Assert(this.m_members != null && this.m_members is ActionInfoProcessing);
			actionInfo.m_members = this.Processing.DeepClone();
			return actionInfo;
		}

		internal void Deconstruct(int uniqueName, ref AspNetCore.ReportingServices.ReportProcessing.Action action, out ActionInstance actionInstance, AspNetCore.ReportingServices.ReportProcessing.CustomReportItem context)
		{
			Global.Tracer.Assert(this.IsCustomControl && null != context);
			actionInstance = null;
			if (this.Processing.m_actionCollection == null || this.Processing.m_actionCollection.Count == 0)
			{
				if (action != null)
				{
					Global.Tracer.Assert(action.ActionItems != null && 0 < action.ActionItems.Count);
					int count = action.ActionItems.Count;
					actionInstance = new ActionInstance();
					actionInstance.UniqueName = uniqueName;
					actionInstance.ActionItemsValues = new ActionItemInstanceList(count);
					for (int i = 0; i < count; i++)
					{
						ActionItemInstance actionItemInstance = new ActionItemInstance();
						if (action.ActionItems[i].DrillthroughParameters != null)
						{
							int count2 = action.ActionItems[i].DrillthroughParameters.Count;
							actionItemInstance.DrillthroughParametersValues = new object[count2];
							actionItemInstance.DrillthroughParametersOmits = new BoolList(count2);
						}
						actionInstance.ActionItemsValues.Add(actionItemInstance);
					}
				}
			}
			else
			{
				bool flag = null == action;
				int count3 = this.Processing.m_actionCollection.Count;
				Global.Tracer.Assert(1 <= count3);
				if (flag)
				{
					action = new AspNetCore.ReportingServices.ReportProcessing.Action();
					action.ActionItems = new ActionItemList(count3);
					action.ComputedActionItemsCount = count3;
				}
				else if (count3 != action.ComputedActionItemsCount)
				{
					context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, "Actions", action.ComputedActionItemsCount.ToString(CultureInfo.InvariantCulture), count3.ToString(CultureInfo.InvariantCulture));
					throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
				}
				actionInstance = new ActionInstance();
				actionInstance.UniqueName = uniqueName;
				actionInstance.ActionItemsValues = new ActionItemInstanceList(count3);
				for (int j = 0; j < count3; j++)
				{
					Action action2 = this.Processing.m_actionCollection[j];
					ActionItem actionItem = null;
					if (flag)
					{
						actionItem = new ActionItem();
						actionItem.ComputedIndex = j;
						actionItem.Label = new ExpressionInfo(ExpressionInfo.Types.Expression);
						switch (action2.m_actionType)
						{
						case ActionType.HyperLink:
							actionItem.HyperLinkURL = new ExpressionInfo(ExpressionInfo.Types.Expression);
							break;
						case ActionType.DrillThrough:
							actionItem.DrillthroughReportName = new ExpressionInfo(ExpressionInfo.Types.Expression);
							if (action2.m_parameters != null && 0 < action2.m_parameters.Count)
							{
								int count4 = action2.m_parameters.Count;
								actionItem.DrillthroughParameters = new ParameterValueList(count4);
								for (int k = 0; k < count4; k++)
								{
									ParameterValue parameterValue = new ParameterValue();
									parameterValue.Name = action2.m_parameters.GetKey(k);
									parameterValue.Omit = new ExpressionInfo(ExpressionInfo.Types.Constant);
									parameterValue.Omit.BoolValue = false;
									parameterValue.Value = new ExpressionInfo(ExpressionInfo.Types.Expression);
									actionItem.DrillthroughParameters.Add(parameterValue);
								}
							}
							break;
						case ActionType.BookmarkLink:
							actionItem.BookmarkLink = new ExpressionInfo(ExpressionInfo.Types.Expression);
							break;
						}
						action.ActionItems.Add(actionItem);
					}
					else
					{
						actionItem = action.ActionItems[j];
					}
					Global.Tracer.Assert(null != actionItem);
					ActionItemInstance actionItemInstance2 = new ActionItemInstance();
					actionItemInstance2.Label = action2.Processing.m_label;
					switch (action2.m_actionType)
					{
					case ActionType.HyperLink:
						actionItemInstance2.HyperLinkURL = action2.Processing.m_action;
						break;
					case ActionType.DrillThrough:
						actionItemInstance2.DrillthroughReportName = action2.Processing.m_action;
						if (action2.m_parameters != null)
						{
							int count5 = action2.m_parameters.Count;
							if (actionItem.DrillthroughParameters == null && 0 < count5)
							{
								context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, "Action.DrillthroughParameters", "0", count5.ToString(CultureInfo.InvariantCulture));
								throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
							}
							if (count5 != actionItem.DrillthroughParameters.Count)
							{
								context.ProcessingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIRenderItemProperties, Severity.Error, context.CustomObjectType, context.CustomObjectName, context.Type, context.Name, "Action.DrillthroughParameters", actionItem.DrillthroughParameters.Count.ToString(CultureInfo.InvariantCulture), count5.ToString(CultureInfo.InvariantCulture));
								throw new ReportProcessingException(context.ProcessingContext.ErrorContext.Messages);
							}
							Global.Tracer.Assert(0 < count5);
							actionItemInstance2.DrillthroughParametersValues = new object[count5];
							actionItemInstance2.DrillthroughParametersOmits = new BoolList(count5);
							DrillthroughParameters drillthroughParameters = new DrillthroughParameters(count5);
							for (int l = 0; l < count5; l++)
							{
								actionItemInstance2.DrillthroughParametersValues[l] = action2.m_parameters.GetValues(l);
								actionItemInstance2.DrillthroughParametersOmits.Add(false);
								drillthroughParameters.Add(actionItem.DrillthroughParameters[l].Name, actionItemInstance2.DrillthroughParametersValues[l]);
							}
							DrillthroughInformation drillthroughInfo = new DrillthroughInformation(actionItemInstance2.DrillthroughReportName, drillthroughParameters, null);
							string drillthroughId = uniqueName.ToString(CultureInfo.InvariantCulture) + ":" + j.ToString(CultureInfo.InvariantCulture);
							context.ProcessingContext.DrillthroughInfo.AddDrillthrough(drillthroughId, drillthroughInfo);
						}
						break;
					case ActionType.BookmarkLink:
						actionItemInstance2.BookmarkLink = action2.Processing.m_action;
						break;
					}
					actionInstance.ActionItemsValues.Add(actionItemInstance2);
				}
				Global.Tracer.Assert(action != null && actionInstance != null && null != this.Processing.m_actionCollection);
				AspNetCore.ReportingServices.ReportProcessing.Style styleClass = action.StyleClass;
				object[] styleAttributeValues = null;
				AspNetCore.ReportingServices.ReportProcessing.CustomReportItem.DeconstructRenderStyle(flag, this.Processing.m_sharedStyles, this.Processing.m_nonSharedStyles, ref styleClass, out styleAttributeValues, context);
				action.StyleClass = styleClass;
				actionInstance.StyleAttributeValues = styleAttributeValues;
			}
		}
	}
}
