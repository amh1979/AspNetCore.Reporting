using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Action : IPersistable
	{
		private List<ActionItem> m_actionItemList;

		private Style m_styleClass;

		private bool m_trackFieldsUsedInValueExpression;

		[NonSerialized]
		private ActionInfoExprHost m_exprHost;

		[NonSerialized]
		private StyleProperties m_sharedStyleProperties;

		[NonSerialized]
		private bool m_noNonSharedStyleProps;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Action.GetDeclaration();

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

		internal List<ActionItem> ActionItems
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

		internal bool TrackFieldsUsedInValueExpression
		{
			get
			{
				return this.m_trackFieldsUsedInValueExpression;
			}
			set
			{
				this.m_trackFieldsUsedInValueExpression = value;
			}
		}

		internal Action()
		{
			this.m_actionItemList = new List<ActionItem>();
		}

		internal Action(ActionItem actionItem, bool computed)
		{
			this.m_actionItemList = new List<ActionItem>();
			this.m_actionItemList.Add(actionItem);
		}

		internal void Initialize(InitializationContext context)
		{
			AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder exprHostBuilder = context.ExprHostBuilder;
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
			Global.Tracer.Assert(exprHost != null && null != reportObjectModel, "(null != exprHost && null != reportObjectModel)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (exprHost.ActionItemHostsRemotable != null)
			{
				Global.Tracer.Assert(this.m_actionItemList != null, "(m_actionItemList != null)");
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
					objectModel.FieldsImpl.ResetFieldsUsedInExpression();
					objectModel.AggregatesImpl.ResetFieldsUsedInExpression();
					return true;
				}
			}
			return false;
		}

		internal void GetSelectedItemsForDrillthroughContext(ObjectModelImpl objectModel, IActionOwner actionOwner)
		{
		}

		public object PublishClone(AutomaticSubtotalContext context)
		{
			Action action = (Action)base.MemberwiseClone();
			if (this.m_actionItemList != null)
			{
				action.m_actionItemList = new List<ActionItem>(this.m_actionItemList.Count);
				foreach (ActionItem actionItem in this.m_actionItemList)
				{
					action.m_actionItemList.Add((ActionItem)actionItem.PublishClone(context));
				}
			}
			if (this.m_styleClass != null)
			{
				action.m_styleClass = (Style)this.m_styleClass.PublishClone(context);
			}
			if (this.m_sharedStyleProperties != null)
			{
				action.m_sharedStyleProperties = (StyleProperties)this.m_sharedStyleProperties.PublishClone(context);
			}
			return action;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ActionItemList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionItem));
			list.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.TrackFieldsUsedInValueExpression, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Action.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ActionItemList:
					writer.Write(this.m_actionItemList);
					break;
				case MemberName.StyleClass:
					writer.Write(this.m_styleClass);
					break;
				case MemberName.TrackFieldsUsedInValueExpression:
					writer.Write(this.m_trackFieldsUsedInValueExpression);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Action.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ActionItemList:
					this.m_actionItemList = reader.ReadGenericListOfRIFObjects<ActionItem>();
					break;
				case MemberName.StyleClass:
					this.m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.TrackFieldsUsedInValueExpression:
					this.m_trackFieldsUsedInValueExpression = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action;
		}
	}
}
