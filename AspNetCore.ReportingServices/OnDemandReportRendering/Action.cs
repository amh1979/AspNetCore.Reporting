using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Action
	{
		private ReportStringProperty m_label;

		private ReportStringProperty m_bookmark;

		private ReportUrlProperty m_hyperlink;

		private ActionInstance m_instance;

		private ActionDrillthrough m_drillthrough;

		private AspNetCore.ReportingServices.ReportRendering.Action m_renderAction;

		private ActionInfo m_owner;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem m_actionItemDef;

		private int m_index = -1;

		public ReportStringProperty Label
		{
			get
			{
				if (this.m_label == null)
				{
					if (this.IsOldSnapshot)
					{
						if (this.m_renderAction.ActionDefinition.Label != null)
						{
							this.m_label = new ReportStringProperty(this.m_renderAction.ActionDefinition.Label);
						}
					}
					else if (this.m_actionItemDef.Label != null)
					{
						this.m_label = new ReportStringProperty(this.m_actionItemDef.Label);
					}
				}
				return this.m_label;
			}
		}

		public ReportStringProperty BookmarkLink
		{
			get
			{
				if (this.m_bookmark == null)
				{
					if (this.IsOldSnapshot)
					{
						if (this.m_renderAction.ActionDefinition.BookmarkLink != null)
						{
							this.m_bookmark = new ReportStringProperty(this.m_renderAction.ActionDefinition.BookmarkLink);
						}
					}
					else if (this.m_actionItemDef.BookmarkLink != null)
					{
						this.m_bookmark = new ReportStringProperty(this.m_actionItemDef.BookmarkLink);
					}
				}
				return this.m_bookmark;
			}
		}

		public ReportUrlProperty Hyperlink
		{
			get
			{
				if (this.m_hyperlink == null)
				{
					if (this.IsOldSnapshot)
					{
						if (this.m_renderAction.ActionDefinition.HyperLinkURL != null)
						{
							this.m_hyperlink = new ReportUrlProperty(this.m_renderAction.ActionDefinition.HyperLinkURL.IsExpression, this.m_renderAction.ActionDefinition.HyperLinkURL.OriginalText, this.m_renderAction.ActionDefinition.HyperLinkURL.IsExpression ? null : new ReportUrl(this.m_renderAction.HyperLinkURL));
						}
					}
					else if (this.m_actionItemDef.HyperLinkURL != null)
					{
						ReportUrl reportUrl = null;
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo hyperLinkURL = this.m_actionItemDef.HyperLinkURL;
						if (!hyperLinkURL.IsExpression)
						{
							reportUrl = ReportUrl.BuildHyperlinkUrl(this.m_owner.RenderingContext, this.m_owner.ObjectType, this.m_owner.ObjectName, "Hyperlink", this.m_owner.RenderingContext.OdpContext.ReportContext, hyperLinkURL.StringValue);
						}
						this.m_hyperlink = new ReportUrlProperty(hyperLinkURL.IsExpression, hyperLinkURL.OriginalText, reportUrl);
					}
				}
				return this.m_hyperlink;
			}
		}

		public ActionDrillthrough Drillthrough
		{
			get
			{
				if (this.m_drillthrough == null)
				{
					if (this.IsOldSnapshot)
					{
						if (this.m_renderAction.ActionDefinition.DrillthroughReportName != null)
						{
							this.m_drillthrough = new ActionDrillthrough(this.m_owner, this.m_renderAction);
						}
					}
					else if (this.m_actionItemDef.DrillthroughReportName != null)
					{
						this.m_drillthrough = new ActionDrillthrough(this.m_owner, this.m_actionItemDef, this.m_index);
					}
				}
				return this.m_drillthrough;
			}
		}

		private bool IsOldSnapshot
		{
			get
			{
				return this.m_owner.IsOldSnapshot;
			}
		}

		public ActionInstance Instance
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
						this.m_instance = new ActionInstance(this.m_renderAction);
					}
					else
					{
						this.m_instance = new ActionInstance(this.m_owner.ReportScope, this);
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

		internal Action(ActionInfo owner, AspNetCore.ReportingServices.ReportIntermediateFormat.ActionItem actionItemDef, int index)
		{
			this.m_owner = owner;
			this.m_actionItemDef = actionItemDef;
			this.m_index = index;
		}

		internal Action(ActionInfo owner, AspNetCore.ReportingServices.ReportRendering.Action renderAction)
		{
			this.m_owner = owner;
			this.m_renderAction = renderAction;
		}

		internal void Update(AspNetCore.ReportingServices.ReportRendering.Action newAction)
		{
			if (this.m_instance != null)
			{
				this.m_instance.Update(newAction);
			}
			if (this.m_drillthrough != null)
			{
				this.m_drillthrough.Update(newAction);
			}
			if (newAction != null)
			{
				this.m_renderAction = newAction;
			}
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_drillthrough != null)
			{
				this.m_drillthrough.SetNewContext();
			}
		}

		internal void ConstructActionDefinition()
		{
			ActionInstance instance = this.Instance;
			Global.Tracer.Assert(instance != null);
			if (instance.Label != null)
			{
				this.m_actionItemDef.Label = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.Label);
			}
			else
			{
				this.m_actionItemDef.Label = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			this.m_label = null;
			if (this.BookmarkLink != null)
			{
				if (instance.BookmarkLink != null)
				{
					this.m_actionItemDef.BookmarkLink = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.BookmarkLink);
				}
				else
				{
					this.m_actionItemDef.BookmarkLink = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
				}
				this.m_bookmark = null;
			}
			if (this.Hyperlink != null)
			{
				if (instance.HyperlinkText != null)
				{
					this.m_actionItemDef.HyperLinkURL = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.HyperlinkText);
				}
				else
				{
					this.m_actionItemDef.HyperLinkURL = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
				}
				this.m_hyperlink = null;
			}
			if (this.Drillthrough != null)
			{
				this.Drillthrough.ConstructDrillthoughDefinition();
			}
		}
	}
}
