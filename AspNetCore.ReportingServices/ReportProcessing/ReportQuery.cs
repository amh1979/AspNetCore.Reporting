using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Data;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportQuery
	{
		private CommandType m_commandType = CommandType.Text;

		private ExpressionInfo m_commandText;

		private ParameterValueList m_queryParameters;

		private int m_timeOut;

		private string m_commandTextValue;

		private string m_writtenCommandText;

		[NonSerialized]
		private string m_dataSourceName;

		[NonSerialized]
		private IndexedExprHost m_queryParamsExprHost;

		internal CommandType CommandType
		{
			get
			{
				return this.m_commandType;
			}
			set
			{
				this.m_commandType = value;
			}
		}

		internal ExpressionInfo CommandText
		{
			get
			{
				return this.m_commandText;
			}
			set
			{
				this.m_commandText = value;
			}
		}

		internal ParameterValueList Parameters
		{
			get
			{
				return this.m_queryParameters;
			}
			set
			{
				this.m_queryParameters = value;
			}
		}

		internal int TimeOut
		{
			get
			{
				return this.m_timeOut;
			}
			set
			{
				this.m_timeOut = value;
			}
		}

		internal string CommandTextValue
		{
			get
			{
				return this.m_commandTextValue;
			}
			set
			{
				this.m_commandTextValue = value;
			}
		}

		internal string RewrittenCommandText
		{
			get
			{
				return this.m_writtenCommandText;
			}
			set
			{
				this.m_writtenCommandText = value;
			}
		}

		internal string DataSourceName
		{
			get
			{
				return this.m_dataSourceName;
			}
			set
			{
				this.m_dataSourceName = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			if (this.m_commandText != null)
			{
				this.m_commandText.Initialize("CommandText", context);
				context.ExprHostBuilder.DataSetQueryCommandText(this.m_commandText);
			}
			if (this.m_queryParameters != null)
			{
				ObjectType objectType = context.ObjectType;
				string objectName = context.ObjectName;
				context.ObjectType = ObjectType.QueryParameter;
				context.ExprHostBuilder.QueryParametersStart();
				for (int i = 0; i < this.m_queryParameters.Count; i++)
				{
					ParameterValue parameterValue = this.m_queryParameters[i];
					context.ObjectName = parameterValue.Name;
					parameterValue.Initialize(context, true);
				}
				context.ExprHostBuilder.QueryParametersEnd();
				context.ObjectType = objectType;
				context.ObjectName = objectName;
			}
		}

		internal void SetExprHost(IndexedExprHost queryParamsExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(queryParamsExprHost != null && reportObjectModel != null);
			this.m_queryParamsExprHost = queryParamsExprHost;
			this.m_queryParamsExprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.CommandType, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.CommandText, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.QueryParameters, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterValueList));
			memberInfoList.Add(new MemberInfo(MemberName.Timeout, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.CommandTextValue, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.RewrittenCommandText, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
