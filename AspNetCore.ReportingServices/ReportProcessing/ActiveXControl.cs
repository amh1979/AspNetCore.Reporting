using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActiveXControl : ReportItem
	{
		private string m_classID;

		private string m_codeBase;

		private ParameterValueList m_parameters;

		[NonSerialized]
		private ActiveXControlExprHost m_exprHost;

		internal override ObjectType ObjectType
		{
			get
			{
				return ObjectType.ActiveXControl;
			}
		}

		internal string ClassID
		{
			get
			{
				return this.m_classID;
			}
			set
			{
				this.m_classID = value;
			}
		}

		internal string CodeBase
		{
			get
			{
				return this.m_codeBase;
			}
			set
			{
				this.m_codeBase = value;
			}
		}

		internal ParameterValueList Parameters
		{
			get
			{
				return this.m_parameters;
			}
			set
			{
				this.m_parameters = value;
			}
		}

		internal ActiveXControl(ReportItem parent)
			: base(parent)
		{
		}

		internal ActiveXControl(int id, ReportItem parent)
			: base(id, parent)
		{
			this.m_parameters = new ParameterValueList();
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			context.ExprHostBuilder.ActiveXControlStart(base.m_name);
			base.Initialize(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context, false, false);
			}
			if (this.m_parameters != null)
			{
				for (int i = 0; i < this.m_parameters.Count; i++)
				{
					ParameterValue parameterValue = this.m_parameters[i];
					context.ExprHostBuilder.ActiveXControlParameterStart();
					parameterValue.Initialize(context, false);
					parameterValue.ExprHostID = context.ExprHostBuilder.ActiveXControlParameterEnd();
				}
			}
			base.ExprHostID = context.ExprHostBuilder.ActiveXControlEnd();
			return true;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.ActiveXControlHostsRemotable[base.ExprHostID];
				base.ReportItemSetExprHost(this.m_exprHost, reportObjectModel);
				if (this.m_exprHost.ParameterHostsRemotable != null)
				{
					Global.Tracer.Assert(this.m_parameters != null);
					for (int num = this.m_parameters.Count - 1; num >= 0; num--)
					{
						this.m_parameters[num].SetExprHost(this.m_exprHost.ParameterHostsRemotable, reportObjectModel);
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ClassID, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.CodeBase, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Parameters, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterValueList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
