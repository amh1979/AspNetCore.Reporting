using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActionItem
	{
		private ExpressionInfo m_hyperLinkURL;

		private ExpressionInfo m_drillthroughReportName;

		private ParameterValueList m_drillthroughParameters;

		private ExpressionInfo m_drillthroughBookmarkLink;

		private ExpressionInfo m_bookmarkLink;

		private ExpressionInfo m_label;

		private int m_exprHostID = -1;

		private int m_computedIndex = -1;

		[NonSerialized]
		private ActionExprHost m_exprHost;

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

		internal ParameterValueList DrillthroughParameters
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
					parameterValue.Initialize(context, false);
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
				Global.Tracer.Assert(actionItemExprHosts != null && reportObjectModel != null);
				this.m_exprHost = actionItemExprHosts[this.m_exprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
				if (this.m_exprHost.DrillThroughParameterHostsRemotable != null)
				{
					Global.Tracer.Assert(this.m_drillthroughParameters != null);
					for (int num = this.m_drillthroughParameters.Count - 1; num >= 0; num--)
					{
						this.m_drillthroughParameters[num].SetExprHost(this.m_exprHost.DrillThroughParameterHostsRemotable, reportObjectModel);
					}
				}
			}
		}

		internal void SetExprHost(ActionExprHost actionExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(actionExprHost != null && reportObjectModel != null);
			this.m_exprHost = actionExprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_exprHost.DrillThroughParameterHostsRemotable != null)
			{
				Global.Tracer.Assert(this.m_drillthroughParameters != null);
				for (int num = this.m_drillthroughParameters.Count - 1; num >= 0; num--)
				{
					this.m_drillthroughParameters[num].SetExprHost(this.m_exprHost.DrillThroughParameterHostsRemotable, reportObjectModel);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.HyperLinkURL, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughReportName, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughParameters, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterValueList));
			memberInfoList.Add(new MemberInfo(MemberName.DrillthroughBookmarkLink, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.BookmarkLink, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Label, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Index, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, int ownerUniqueName, int index)
		{
			if (this.m_drillthroughReportName != null)
			{
				Global.Tracer.Assert(this.m_drillthroughReportName.Type == ExpressionInfo.Types.Constant);
				if (this.m_drillthroughReportName.Value != null)
				{
					DrillthroughParameters drillthroughParameters = null;
					if (this.m_drillthroughParameters != null)
					{
						ParameterValue parameterValue = null;
						for (int i = 0; i < this.m_drillthroughParameters.Count; i++)
						{
							parameterValue = this.m_drillthroughParameters[i];
							if (parameterValue.Omit != null)
							{
								Global.Tracer.Assert(parameterValue.Omit.Type == ExpressionInfo.Types.Constant);
								if (!parameterValue.Omit.BoolValue)
								{
									goto IL_007c;
								}
								continue;
							}
							goto IL_007c;
							IL_007c:
							Global.Tracer.Assert(parameterValue.Value.Type == ExpressionInfo.Types.Constant);
							if (drillthroughParameters == null)
							{
								drillthroughParameters = new DrillthroughParameters();
							}
							drillthroughParameters.Add(parameterValue.Name, parameterValue.Value.Value);
						}
					}
					DrillthroughInformation drillthroughInfo = new DrillthroughInformation(this.m_drillthroughReportName.Value, drillthroughParameters, null);
					string drillthroughId = ownerUniqueName.ToString(CultureInfo.InvariantCulture) + ":" + index.ToString(CultureInfo.InvariantCulture);
					processingContext.DrillthroughInfo.AddDrillthrough(drillthroughId, drillthroughInfo);
				}
			}
		}
	}
}
