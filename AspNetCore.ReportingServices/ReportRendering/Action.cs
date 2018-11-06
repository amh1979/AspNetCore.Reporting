using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class Action
	{
		internal ActionType m_actionType;

		internal MemberBase m_members;

		internal NameValueCollection m_parameters;

		internal DrillthroughParameters m_parameterNameObjectCollection;

		public ReportUrl HyperLinkURL
		{
			get
			{
				if (this.m_actionType != ActionType.DrillThrough && this.m_actionType != ActionType.BookmarkLink)
				{
					if (this.IsCustomControl)
					{
						return null;
					}
					ReportUrl reportUrl = this.Rendering.m_actionURL;
					if (this.Rendering.m_actionURL == null && this.Rendering.m_actionDef.HyperLinkURL != null)
					{
						string hyperLinkUrlValue = null;
						this.m_actionType = ActionType.HyperLink;
						if (this.Rendering.m_actionDef.HyperLinkURL.Type == ExpressionInfo.Types.Constant)
						{
							hyperLinkUrlValue = this.Rendering.m_actionDef.HyperLinkURL.Value;
						}
						else if (this.Rendering.m_actionInstance == null)
						{
							reportUrl = null;
						}
						else
						{
							hyperLinkUrlValue = this.Rendering.m_actionInstance.HyperLinkURL;
						}
						reportUrl = ReportUrl.BuildHyperLinkURL(hyperLinkUrlValue, this.Rendering.m_renderingContext);
						if (this.Rendering.m_renderingContext.CacheState)
						{
							this.Rendering.m_actionURL = reportUrl;
						}
					}
					return reportUrl;
				}
				return null;
			}
		}

		public ReportUrl DrillthroughReport
		{
			get
			{
				if (this.m_actionType != ActionType.HyperLink && this.m_actionType != ActionType.BookmarkLink)
				{
					if (this.IsCustomControl)
					{
						return null;
					}
					ReportUrl reportUrl = this.Rendering.m_actionURL;
					if (this.Rendering.m_actionURL == null && this.Rendering.m_actionDef.DrillthroughReportName != null)
					{
						string drillthroughPath = this.DrillthroughPath;
						this.m_actionType = ActionType.DrillThrough;
						if (drillthroughPath != null)
						{
							try
							{
								reportUrl = new ReportUrl(this.Rendering.m_renderingContext, drillthroughPath, true, null, true);
							}
							catch (ItemNotFoundException)
							{
								return null;
							}
						}
						if (this.Rendering.m_renderingContext.CacheState)
						{
							this.Rendering.m_actionURL = reportUrl;
						}
					}
					return reportUrl;
				}
				return null;
			}
		}

		internal DrillthroughParameters DrillthroughParameterNameObjectCollection
		{
			get
			{
				if (this.m_actionType != ActionType.HyperLink && this.m_actionType != ActionType.BookmarkLink)
				{
					DrillthroughParameters drillthroughParameters = this.m_parameterNameObjectCollection;
					if (!this.IsCustomControl && this.m_parameters == null)
					{
						ParameterValueList drillthroughParameters2 = this.Rendering.m_actionDef.DrillthroughParameters;
						if (drillthroughParameters2 != null && drillthroughParameters2.Count > 0)
						{
							this.m_actionType = ActionType.DrillThrough;
							drillthroughParameters = new DrillthroughParameters();
							for (int i = 0; i < drillthroughParameters2.Count; i++)
							{
								ParameterValue parameterValue = drillthroughParameters2[i];
								if (parameterValue.Omit == null || (this.Rendering.m_actionInstance == null && parameterValue.Omit.Type == ExpressionInfo.Types.Constant && !parameterValue.Omit.BoolValue) || !this.Rendering.m_actionInstance.DrillthroughParametersOmits[i])
								{
									object value = (parameterValue.Value.Type != ExpressionInfo.Types.Constant) ? ((this.Rendering.m_actionInstance != null) ? this.Rendering.m_actionInstance.DrillthroughParametersValues[i] : null) : parameterValue.Value.Value;
									drillthroughParameters.Add(parameterValue.Name, value);
								}
							}
							this.m_parameterNameObjectCollection = drillthroughParameters;
						}
					}
					return drillthroughParameters;
				}
				return null;
			}
		}

		public NameValueCollection DrillthroughParameters
		{
			get
			{
				if (this.m_actionType != ActionType.HyperLink && this.m_actionType != ActionType.BookmarkLink)
				{
					NameValueCollection nameValueCollection = this.m_parameters;
					if (!this.IsCustomControl && this.m_parameters == null)
					{
						ParameterValueList drillthroughParameters = this.Rendering.m_actionDef.DrillthroughParameters;
						if (drillthroughParameters != null && drillthroughParameters.Count > 0)
						{
							this.m_actionType = ActionType.DrillThrough;
							nameValueCollection = new NameValueCollection();
							bool[] array = new bool[drillthroughParameters.Count];
							for (int i = 0; i < drillthroughParameters.Count; i++)
							{
								ParameterValue parameterValue = drillthroughParameters[i];
								if (parameterValue.Value != null && parameterValue.Value.Type == ExpressionInfo.Types.Token)
								{
									array[i] = true;
								}
								else
								{
									array[i] = false;
								}
								if (parameterValue.Omit == null || (this.Rendering.m_actionInstance == null && parameterValue.Omit.Type == ExpressionInfo.Types.Constant && !parameterValue.Omit.BoolValue) || !this.Rendering.m_actionInstance.DrillthroughParametersOmits[i])
								{
									object obj = (parameterValue.Value.Type != ExpressionInfo.Types.Constant) ? ((this.Rendering.m_actionInstance != null) ? this.Rendering.m_actionInstance.DrillthroughParametersValues[i] : null) : parameterValue.Value.Value;
									if (obj == null)
									{
										nameValueCollection.Add(parameterValue.Name, null);
									}
									else
									{
										object[] array2 = obj as object[];
										if (array2 != null)
										{
											for (int j = 0; j < array2.Length; j++)
											{
												nameValueCollection.Add(parameterValue.Name, array2[j].ToString());
											}
										}
										else
										{
											nameValueCollection.Add(parameterValue.Name, obj.ToString());
										}
									}
								}
							}
							bool flag = false;
							if (this.Rendering.m_renderingContext.StoreServerParameters != null && this.DrillthroughPath != null)
							{
								string drillthroughPath = this.DrillthroughPath;
								ICatalogItemContext subreportContext = this.Rendering.m_renderingContext.TopLevelReportContext.GetSubreportContext(drillthroughPath);
								nameValueCollection = this.Rendering.m_renderingContext.StoreServerParameters(subreportContext, nameValueCollection, array, out flag);
							}
							if (this.Rendering.m_renderingContext.CacheState)
							{
								this.m_parameters = nameValueCollection;
							}
						}
					}
					return nameValueCollection;
				}
				return null;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.m_parameters = value;
			}
		}

		public string DrillthroughID
		{
			get
			{
				if (this.IsCustomControl)
				{
					return null;
				}
				if (this.m_actionType != ActionType.HyperLink && this.m_actionType != ActionType.BookmarkLink)
				{
					if (this.DrillthroughReport != null)
					{
						this.m_actionType = ActionType.DrillThrough;
						return this.Rendering.m_drillthroughId;
					}
					return null;
				}
				return null;
			}
		}

		public string BookmarkLink
		{
			get
			{
				if (this.m_actionType != ActionType.HyperLink && this.m_actionType != ActionType.DrillThrough)
				{
					string result = null;
					if (this.IsCustomControl)
					{
						result = this.Processing.m_action;
					}
					else if (this.Rendering.m_actionDef.BookmarkLink != null)
					{
						this.m_actionType = ActionType.BookmarkLink;
						result = ((this.Rendering.m_actionDef.BookmarkLink.Type != ExpressionInfo.Types.Constant) ? ((this.Rendering.m_actionInstance != null) ? this.Rendering.m_actionInstance.BookmarkLink : null) : this.Rendering.m_actionDef.BookmarkLink.Value);
					}
					return result;
				}
				return null;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.m_actionType = ActionType.BookmarkLink;
				this.Processing.m_action = value;
			}
		}

		public string Label
		{
			get
			{
				string result = null;
				if (this.IsCustomControl)
				{
					result = this.Processing.m_label;
				}
				else if (this.Rendering.m_actionDef.Label != null)
				{
					result = ((this.Rendering.m_actionDef.Label.Type != ExpressionInfo.Types.Constant) ? ((this.Rendering.m_actionInstance != null) ? this.Rendering.m_actionInstance.Label : null) : this.Rendering.m_actionDef.Label.Value);
				}
				return result;
			}
			set
			{
				if (!this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.m_label = value;
			}
		}

		internal ActionItem ActionDefinition
		{
			get
			{
				if (this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return this.Rendering.m_actionDef;
			}
		}

		internal ActionItemInstance ActionInstance
		{
			get
			{
				if (this.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return this.Rendering.m_actionInstance;
			}
		}

		internal ParameterValueList DrillthroughParameterValueList
		{
			get
			{
				if (this.m_actionType != ActionType.HyperLink && this.m_actionType != ActionType.BookmarkLink)
				{
					return this.Rendering.m_actionDef.DrillthroughParameters;
				}
				return null;
			}
		}

		internal string DrillthroughPath
		{
			get
			{
				string text = null;
				if (this.Rendering.m_actionDef.DrillthroughReportName.Type == ExpressionInfo.Types.Constant)
				{
					return this.Rendering.m_actionDef.DrillthroughReportName.Value;
				}
				if (this.Rendering.m_actionInstance == null)
				{
					return null;
				}
				return this.Rendering.m_actionInstance.DrillthroughReportName;
			}
		}

		private bool IsCustomControl
		{
			get
			{
				return this.m_members.IsCustomControl;
			}
		}

		private ActionRendering Rendering
		{
			get
			{
				Global.Tracer.Assert(!this.m_members.IsCustomControl);
				ActionRendering actionRendering = this.m_members as ActionRendering;
				if (actionRendering == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionRendering;
			}
		}

		internal ActionProcessing Processing
		{
			get
			{
				Global.Tracer.Assert(this.m_members.IsCustomControl);
				ActionProcessing actionProcessing = this.m_members as ActionProcessing;
				if (actionProcessing == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return actionProcessing;
			}
		}

		public Action()
		{
			this.m_members = new ActionProcessing();
			Global.Tracer.Assert(this.IsCustomControl);
		}

		internal Action(ActionItem actionItemDef, ActionItemInstance actionItemInstance, string drillthroughId, RenderingContext renderingContext)
		{
			this.m_members = new ActionRendering();
			Global.Tracer.Assert(!this.IsCustomControl);
			this.Rendering.m_actionDef = actionItemDef;
			this.Rendering.m_actionInstance = actionItemInstance;
			this.Rendering.m_renderingContext = renderingContext;
			this.Rendering.m_drillthroughId = drillthroughId;
		}

		public void SetHyperlinkAction(string hyperlink)
		{
			this.SetHyperlinkAction(hyperlink, null);
		}

		public void SetHyperlinkAction(string hyperlink, string label)
		{
			if (!this.IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			this.m_actionType = ActionType.HyperLink;
			this.Processing.m_action = hyperlink;
			this.Processing.m_label = label;
		}

		public void SetDrillthroughAction(string reportName)
		{
			this.SetDrillthroughAction(reportName, null, null);
		}

		public void SetDrillthroughAction(string reportName, NameValueCollection parameters)
		{
			this.SetDrillthroughAction(reportName, parameters, null);
		}

		public void SetDrillthroughAction(string reportName, NameValueCollection parameters, string label)
		{
			if (!this.IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (parameters != null && reportName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "reportName");
			}
			this.m_actionType = ActionType.DrillThrough;
			this.Processing.m_action = reportName;
			this.Processing.m_label = label;
			this.m_parameters = parameters;
		}

		public void SetBookmarkAction(string bookmark)
		{
			this.SetBookmarkAction(bookmark, null);
		}

		public void SetBookmarkAction(string bookmark, string label)
		{
			this.m_actionType = ActionType.BookmarkLink;
			this.Processing.m_action = bookmark;
			this.Processing.m_label = label;
		}

		internal Action DeepClone()
		{
			if (!this.IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			Global.Tracer.Assert(this.m_members != null && this.m_members is ActionProcessing);
			Action action = new Action();
			action.m_actionType = this.m_actionType;
			action.m_members = this.Processing.DeepClone();
			if (ActionType.DrillThrough == this.m_actionType && this.m_parameters != null)
			{
				action.m_parameters = new NameValueCollection(this.m_parameters);
			}
			return action;
		}
	}
}
