using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Action
	{
		private ActionItemList m_actionItemList;

		private Style m_styleClass;

		private int m_computedActionItemsCount;

		[NonSerialized]
		private ActionInfoExprHost m_exprHost;

		[NonSerialized]
		private StyleProperties m_sharedStyleProperties;

		[NonSerialized]
		private bool m_noNonSharedStyleProps;

		internal Style StyleClass
		{
			get
			{
				return this.m_styleClass;
			}
			set
			{
				this.m_styleClass = value;
			}
		}

		internal ActionItemList ActionItems
		{
			get
			{
				return this.m_actionItemList;
			}
			set
			{
				this.m_actionItemList = value;
			}
		}

		internal int ComputedActionItemsCount
		{
			get
			{
				return this.m_computedActionItemsCount;
			}
			set
			{
				this.m_computedActionItemsCount = value;
			}
		}

		internal StyleProperties SharedStyleProperties
		{
			get
			{
				return this.m_sharedStyleProperties;
			}
			set
			{
				this.m_sharedStyleProperties = value;
			}
		}

		internal bool NoNonSharedStyleProps
		{
			get
			{
				return this.m_noNonSharedStyleProps;
			}
			set
			{
				this.m_noNonSharedStyleProps = value;
			}
		}

		internal Action(ActionItem actionItem, bool computed)
		{
			this.m_actionItemList = new ActionItemList();
			this.m_actionItemList.Add(actionItem);
			if (computed)
			{
				this.m_computedActionItemsCount = 1;
			}
		}

		internal Action()
		{
			this.m_actionItemList = new ActionItemList();
		}

		internal void Initialize(InitializationContext context)
		{
			ExprHostBuilder exprHostBuilder = context.ExprHostBuilder;
			exprHostBuilder.ActionInfoStart();
			if (this.m_actionItemList != null)
			{
				for (int i = 0; i < this.m_actionItemList.Count; i++)
				{
					this.m_actionItemList[i].Initialize(context);
				}
			}
			if (this.m_styleClass != null)
			{
				this.m_styleClass.Initialize(context);
			}
			exprHostBuilder.ActionInfoEnd();
		}

		internal void SetExprHost(ActionInfoExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && null != reportObjectModel);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (exprHost.ActionItemHostsRemotable != null)
			{
				Global.Tracer.Assert(this.m_actionItemList != null);
				for (int num = this.m_actionItemList.Count - 1; num >= 0; num--)
				{
					this.m_actionItemList[num].SetExprHost(exprHost.ActionItemHostsRemotable, reportObjectModel);
				}
			}
			if (this.m_styleClass != null)
			{
				this.m_styleClass.SetStyleExprHost(this.m_exprHost);
			}
		}

		internal void SetExprHost(ActionExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			if (exprHost != null)
			{
				Global.Tracer.Assert(this.m_actionItemList != null);
				for (int num = this.m_actionItemList.Count - 1; num >= 0; num--)
				{
					this.m_actionItemList[num].SetExprHost(exprHost, reportObjectModel);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ActionItemList, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ActionItemList));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.CoumputedActionsCount, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, int uniqueName)
		{
			if (this.m_actionItemList != null && this.m_actionItemList.Count != 0)
			{
				for (int i = 0; i < this.m_actionItemList.Count; i++)
				{
					this.m_actionItemList[i].ProcessDrillthroughAction(processingContext, uniqueName, i);
				}
			}
		}

		internal bool ResetObjectModelForDrillthroughContext(ObjectModelImpl objectModel, IActionOwner actionOwner)
		{
			if (actionOwner.FieldsUsedInValueExpression == null)
			{
				bool flag = false;
				if (this.m_actionItemList != null)
				{
					int num = 0;
					while (num < this.m_actionItemList.Count)
					{
						if (this.m_actionItemList[num].DrillthroughParameters == null || 0 >= this.m_actionItemList[num].DrillthroughParameters.Count)
						{
							num++;
							continue;
						}
						flag = true;
						break;
					}
				}
				if (flag)
				{
					objectModel.FieldsImpl.ResetUsedInExpression();
					objectModel.AggregatesImpl.ResetUsedInExpression();
					return true;
				}
			}
			return false;
		}

		internal void GetSelectedItemsForDrillthroughContext(ObjectModelImpl objectModel, IActionOwner actionOwner)
		{
			if (actionOwner.FieldsUsedInValueExpression == null)
			{
				actionOwner.FieldsUsedInValueExpression = new List<string>();
				objectModel.FieldsImpl.AddFieldsUsedInExpression(actionOwner.FieldsUsedInValueExpression);
				objectModel.AggregatesImpl.AddFieldsUsedInExpression(actionOwner.FieldsUsedInValueExpression);
			}
		}
	}
}
