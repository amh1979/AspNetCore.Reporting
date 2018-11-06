using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
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
	internal sealed class ActionItem : IPersistable
	{
		private ExpressionInfo m_hyperLinkURL;

		private ExpressionInfo m_drillthroughReportName;

		private List<ParameterValue> m_drillthroughParameters;

		private ExpressionInfo m_drillthroughBookmarkLink;

		private ExpressionInfo m_bookmarkLink;

		private ExpressionInfo m_label;

		private int m_exprHostID = -1;

		private int m_computedIndex = -1;

		[NonSerialized]
		private ActionExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ActionItem.GetDeclaration();

		internal ExpressionInfo HyperLinkURL
		{
			get
			{
				return this.m_hyperLinkURL;
			}
			set
			{
				this.m_hyperLinkURL = value;
			}
		}

		internal ExpressionInfo DrillthroughReportName
		{
			get
			{
				return this.m_drillthroughReportName;
			}
			set
			{
				this.m_drillthroughReportName = value;
			}
		}

		internal List<ParameterValue> DrillthroughParameters
		{
			get
			{
				return this.m_drillthroughParameters;
			}
			set
			{
				this.m_drillthroughParameters = value;
			}
		}

		internal ExpressionInfo DrillthroughBookmarkLink
		{
			get
			{
				return this.m_drillthroughBookmarkLink;
			}
			set
			{
				this.m_drillthroughBookmarkLink = value;
			}
		}

		internal ExpressionInfo BookmarkLink
		{
			get
			{
				return this.m_bookmarkLink;
			}
			set
			{
				this.m_bookmarkLink = value;
			}
		}

		internal ExpressionInfo Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}

		internal int ComputedIndex
		{
			get
			{
				return this.m_computedIndex;
			}
			set
			{
				this.m_computedIndex = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return this.m_exprHostID;
			}
			set
			{
				this.m_exprHostID = value;
			}
		}

		internal ActionExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ActionStart();
			if (this.m_hyperLinkURL != null)
			{
				this.m_hyperLinkURL.Initialize("Hyperlink", context);
				context.ExprHostBuilder.ActionHyperlink(this.m_hyperLinkURL);
			}
			if (this.m_drillthroughReportName != null)
			{
				this.m_drillthroughReportName.Initialize("DrillthroughReportName", context);
				context.ExprHostBuilder.ActionDrillThroughReportName(this.m_drillthroughReportName);
			}
			if (this.m_drillthroughParameters != null)
			{
				for (int i = 0; i < this.m_drillthroughParameters.Count; i++)
				{
					ParameterValue parameterValue = this.m_drillthroughParameters[i];
					context.ExprHostBuilder.ActionDrillThroughParameterStart();
					parameterValue.Initialize("DrillthroughParameters", context, false);
					parameterValue.ExprHostID = context.ExprHostBuilder.ActionDrillThroughParameterEnd();
				}
			}
			if (this.m_drillthroughBookmarkLink != null)
			{
				this.m_drillthroughBookmarkLink.Initialize("BookmarkLink", context);
				context.ExprHostBuilder.ActionDrillThroughBookmarkLink(this.m_drillthroughBookmarkLink);
			}
			if (this.m_bookmarkLink != null)
			{
				this.m_bookmarkLink.Initialize("BookmarkLink", context);
				context.ExprHostBuilder.ActionBookmarkLink(this.m_bookmarkLink);
			}
			if (this.m_label != null)
			{
				this.m_label.Initialize("Label", context);
				context.ExprHostBuilder.GenericLabel(this.m_label);
			}
			this.m_exprHostID = context.ExprHostBuilder.ActionEnd();
		}

		internal void SetExprHost(IList<ActionExprHost> actionItemExprHosts, ObjectModelImpl reportObjectModel)
		{
			if (this.m_exprHostID >= 0)
			{
				Global.Tracer.Assert(actionItemExprHosts != null && reportObjectModel != null, "(actionItemExprHosts != null && reportObjectModel != null)");
				this.m_exprHost = actionItemExprHosts[this.m_exprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				if (this.m_exprHost.DrillThroughParameterHostsRemotable != null)
				{
					Global.Tracer.Assert(this.m_drillthroughParameters != null, "(m_drillthroughParameters != null)");
					for (int num = this.m_drillthroughParameters.Count - 1; num >= 0; num--)
					{
						this.m_drillthroughParameters[num].SetExprHost(this.m_exprHost.DrillThroughParameterHostsRemotable, reportObjectModel);
					}
				}
			}
		}

		internal string EvaluateHyperLinkURL(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			return context.ReportRuntime.EvaluateReportItemHyperlinkURLExpression(this, this.m_hyperLinkURL, objectType, objectName);
		}

		internal string EvaluateDrillthroughReportName(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			return context.ReportRuntime.EvaluateReportItemDrillthroughReportName(this, this.m_drillthroughReportName, objectType, objectName);
		}

		internal string EvaluateBookmarkLink(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			return context.ReportRuntime.EvaluateReportItemBookmarkLinkExpression(this, this.m_bookmarkLink, objectType, objectName);
		}

		internal string EvaluateLabel(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			return context.ReportRuntime.EvaluateActionLabelExpression(this, this.m_label, objectType, objectName);
		}

		internal object EvaluateDrillthroughParamValue(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, List<string> fieldsUsedInOwnerValue, ParameterValue paramValue, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			AspNetCore.ReportingServices.RdlExpressions.ReportRuntime reportRuntime = context.ReportRuntime;
			reportRuntime.FieldsUsedInCurrentActionOwnerValue = fieldsUsedInOwnerValue;
			AspNetCore.ReportingServices.RdlExpressions.ParameterValueResult parameterValueResult = reportRuntime.EvaluateParameterValueExpression(paramValue, objectType, objectName, "DrillthroughParameterValue");
			reportRuntime.FieldsUsedInCurrentActionOwnerValue = null;
			return parameterValueResult.Value;
		}

		internal bool EvaluateDrillthroughParamOmit(IReportScopeInstance romInstance, OnDemandProcessingContext context, IInstancePath ownerItem, ParameterValue paramValue, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			context.SetupContext(ownerItem, romInstance);
			return context.ReportRuntime.EvaluateParamValueOmitExpression(paramValue, objectType, objectName);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.HyperLinkURL, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DrillthroughReportName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DrillthroughParameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue));
			list.Add(new MemberInfo(MemberName.DrillthroughBookmarkLink, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BookmarkLink, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Label, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Index, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionItem, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ActionItem.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HyperLinkURL:
					writer.Write(this.m_hyperLinkURL);
					break;
				case MemberName.DrillthroughReportName:
					writer.Write(this.m_drillthroughReportName);
					break;
				case MemberName.DrillthroughParameters:
					writer.Write(this.m_drillthroughParameters);
					break;
				case MemberName.DrillthroughBookmarkLink:
					writer.Write(this.m_drillthroughBookmarkLink);
					break;
				case MemberName.BookmarkLink:
					writer.Write(this.m_bookmarkLink);
					break;
				case MemberName.Label:
					writer.Write(this.m_label);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.Index:
					writer.Write(this.m_computedIndex);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ActionItem.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.HyperLinkURL:
					this.m_hyperLinkURL = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DrillthroughReportName:
					this.m_drillthroughReportName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DrillthroughParameters:
					this.m_drillthroughParameters = reader.ReadGenericListOfRIFObjects<ParameterValue>();
					break;
				case MemberName.DrillthroughBookmarkLink:
					this.m_drillthroughBookmarkLink = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BookmarkLink:
					this.m_bookmarkLink = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Label:
					this.m_label = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Index:
					this.m_computedIndex = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ActionItem;
		}

		public object PublishClone(AutomaticSubtotalContext context)
		{
			ActionItem actionItem = (ActionItem)base.MemberwiseClone();
			if (this.m_hyperLinkURL != null)
			{
				actionItem.m_hyperLinkURL = (ExpressionInfo)this.m_hyperLinkURL.PublishClone(context);
			}
			if (this.m_drillthroughReportName != null)
			{
				actionItem.m_drillthroughReportName = (ExpressionInfo)this.m_drillthroughReportName.PublishClone(context);
			}
			if (this.m_drillthroughParameters != null)
			{
				actionItem.m_drillthroughParameters = new List<ParameterValue>(this.m_drillthroughParameters.Count);
				foreach (ParameterValue drillthroughParameter in this.m_drillthroughParameters)
				{
					actionItem.m_drillthroughParameters.Add((ParameterValue)drillthroughParameter.PublishClone(context));
				}
			}
			if (this.m_drillthroughBookmarkLink != null)
			{
				actionItem.m_drillthroughBookmarkLink = (ExpressionInfo)this.m_drillthroughBookmarkLink.PublishClone(context);
			}
			if (this.m_bookmarkLink != null)
			{
				actionItem.m_bookmarkLink = (ExpressionInfo)this.m_bookmarkLink.PublishClone(context);
			}
			if (this.m_label != null)
			{
				actionItem.m_label = (ExpressionInfo)this.m_label.PublishClone(context);
			}
			return actionItem;
		}
	}
}
