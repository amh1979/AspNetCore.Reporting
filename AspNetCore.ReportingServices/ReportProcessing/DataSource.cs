using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataSource : IProcessingDataSource
	{
		private string m_name;

		private bool m_transaction;

		private string m_type;

		private ExpressionInfo m_connectString;

		private bool m_integratedSecurity;

		private string m_prompt;

		private string m_dataSourceReference;

		private DataSetList m_dataSets;

		private Guid m_ID = Guid.Empty;

		private int m_exprHostID = -1;

		private string m_sharedDataSourceReferencePath;

		[NonSerialized]
		private DataSourceExprHost m_exprHost;

		[NonSerialized]
		private bool m_isComplex;

		[NonSerialized]
		private StringList m_parameterNames;

		public string Name
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

		public bool Transaction
		{
			get
			{
				return this.m_transaction;
			}
			set
			{
				this.m_transaction = value;
			}
		}

		public string Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
			}
		}

		internal ExpressionInfo ConnectStringExpression
		{
			get
			{
				return this.m_connectString;
			}
			set
			{
				this.m_connectString = value;
			}
		}

		public bool IntegratedSecurity
		{
			get
			{
				return this.m_integratedSecurity;
			}
			set
			{
				this.m_integratedSecurity = value;
			}
		}

		public string Prompt
		{
			get
			{
				return this.m_prompt;
			}
			set
			{
				this.m_prompt = value;
			}
		}

		public string DataSourceReference
		{
			get
			{
				return this.m_dataSourceReference;
			}
			set
			{
				this.m_dataSourceReference = value;
			}
		}

		internal DataSetList DataSets
		{
			get
			{
				return this.m_dataSets;
			}
			set
			{
				this.m_dataSets = value;
			}
		}

		public Guid ID
		{
			get
			{
				return this.m_ID;
			}
			set
			{
				this.m_ID = value;
			}
		}

		internal DataSourceExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
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

		internal bool IsComplex
		{
			get
			{
				return this.m_isComplex;
			}
			set
			{
				this.m_isComplex = value;
			}
		}

		internal StringList ParameterNames
		{
			get
			{
				return this.m_parameterNames;
			}
			set
			{
				this.m_parameterNames = value;
			}
		}

		public string SharedDataSourceReferencePath
		{
			get
			{
				return this.m_sharedDataSourceReferencePath;
			}
			set
			{
				this.m_sharedDataSourceReferencePath = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType.DataSource;
			context.ObjectName = this.m_name;
			this.InternalInitialize(context);
			if (this.m_dataSets != null)
			{
				for (int i = 0; i < this.m_dataSets.Count; i++)
				{
					Global.Tracer.Assert(null != this.m_dataSets[i]);
					this.m_dataSets[i].Initialize(context);
				}
			}
		}

		internal string ResolveConnectionString(ReportProcessing.ReportProcessingContext pc, out DataSourceInfo dataSourceInfo)
		{
			dataSourceInfo = null;
			string text = null;
			if (pc.DataSourceInfos != null)
			{
				if (Guid.Empty != this.ID)
				{
					dataSourceInfo = pc.DataSourceInfos.GetByID(this.ID);
				}
				if (dataSourceInfo == null)
				{
					dataSourceInfo = pc.DataSourceInfos.GetByName(this.Name, pc.ReportContext);
				}
				if (dataSourceInfo == null)
				{
					throw new DataSourceNotFoundException(this.Name);
				}
				text = dataSourceInfo.GetConnectionString(pc.DataProtection);
				if (!dataSourceInfo.IsReference && text == null)
				{
					text = this.EvaluateConnectStringExpression(pc);
				}
			}
			else
			{
				if (this.DataSourceReference != null)
				{
					throw new DataSourceNotFoundException(this.Name);
				}
				text = this.EvaluateConnectStringExpression(pc);
			}
			return text;
		}

		private void InternalInitialize(InitializationContext context)
		{
			context.ExprHostBuilder.DataSourceStart(this.m_name);
			if (this.m_connectString != null)
			{
				this.m_connectString.Initialize("ConnectString", context);
				context.ExprHostBuilder.DataSourceConnectString(this.m_connectString);
			}
			this.m_exprHostID = context.ExprHostBuilder.DataSourceEnd();
		}

		private void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (this.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.DataSourceHostsRemotable[this.ExprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		private string EvaluateConnectStringExpression(ReportProcessing.ProcessingContext processingContext)
		{
			if (this.m_connectString == null)
			{
				return null;
			}
			if (ExpressionInfo.Types.Constant == this.m_connectString.Type)
			{
				return this.m_connectString.Value;
			}
			Global.Tracer.Assert(null != processingContext.ReportRuntime);
			if (processingContext.ReportRuntime.ReportExprHost != null)
			{
				this.SetExprHost(processingContext.ReportRuntime.ReportExprHost, processingContext.ReportObjectModel);
			}
			StringResult stringResult = processingContext.ReportRuntime.EvaluateConnectString(this);
			if (stringResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsDataSourceConnectStringProcessingError, this.m_name);
			}
			return stringResult.Value;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Transaction, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Type, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.ConnectString, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.IntegratedSecurity, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Prompt, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataSourceReference, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataSets, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataSetList));
			memberInfoList.Add(new MemberInfo(MemberName.ID, Token.Guid));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.SharedDataSourceReferencePath, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
