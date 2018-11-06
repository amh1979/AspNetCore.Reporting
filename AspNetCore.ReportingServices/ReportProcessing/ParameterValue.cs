using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ParameterValue
	{
		private string m_name;

		private ExpressionInfo m_value;

		private ExpressionInfo m_omit;

		private int m_exprHostID = -1;

		[NonSerialized]
		private ParamExprHost m_exprHost;

		internal string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		internal ExpressionInfo Omit
		{
			get
			{
				return this.m_omit;
			}
			set
			{
				this.m_omit = value;
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

		internal ParamExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
			set
			{
				this.m_exprHost = value;
			}
		}

		internal void Initialize(InitializationContext context, bool queryParam)
		{
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
				if (!queryParam)
				{
					context.ExprHostBuilder.GenericValue(this.m_value);
				}
				else
				{
					context.ExprHostBuilder.QueryParameterValue(this.m_value);
				}
			}
			if (this.m_omit != null)
			{
				this.m_omit.Initialize("Omit", context);
				context.ExprHostBuilder.ParameterOmit(this.m_omit);
			}
		}

		internal void SetExprHost(IList<ParamExprHost> paramExprHosts, ObjectModelImpl reportObjectModel)
		{
			if (this.m_exprHostID >= 0)
			{
				Global.Tracer.Assert(paramExprHosts != null && reportObjectModel != null);
				this.m_exprHost = paramExprHosts[this.m_exprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Omit, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
