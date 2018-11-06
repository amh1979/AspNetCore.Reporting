using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;
using System.Xml;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataSource : IPersistable, IReferenceable, IProcessingDataSource
	{
		private int m_referenceID = -1;

		private string m_name;

		private bool m_transaction;

		private string m_type;

		private ExpressionInfo m_connectString;

		private bool m_integratedSecurity;

		private string m_prompt;

		private string m_dataSourceReference;

		private List<DataSet> m_dataSets;

		private Guid m_ID = Guid.Empty;

		private int m_exprHostID = -1;

		private string m_sharedDataSourceReferencePath;

		private bool m_isArtificialDataSource;

		[NonSerialized]
		private DataSourceExprHost m_exprHost;

		[NonSerialized]
		private bool m_isComplex;

		[NonSerialized]
		private Dictionary<string, bool> m_parameterNames;

		[NonSerialized]
		private string m_connectionCategory;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataSource.GetDeclaration();

		internal bool IsArtificialForSharedDataSets
		{
			get
			{
				return this.m_isArtificialDataSource;
			}
		}

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

		internal List<DataSet> DataSets
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

		internal Dictionary<string, bool> ParameterNames
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

		internal string ConnectionCategory
		{
			get
			{
				return this.m_connectionCategory;
			}
			set
			{
				this.m_connectionCategory = value;
			}
		}

		int IReferenceable.ID
		{
			get
			{
				return this.m_referenceID;
			}
		}

		internal DataSource()
		{
		}

		internal DataSource(int id)
		{
			this.m_referenceID = id;
		}

		internal DataSource(int id, Guid sharedDataSourceReferenceId)
		{
			this.m_referenceID = id;
			this.m_ID = sharedDataSourceReferenceId;
			this.m_isArtificialDataSource = true;
			this.m_name = " Data source for shared dataset";
		}

		internal DataSource(int id, Guid sharedDataSourceReferenceId, DataSetCore dataSetCore)
			: this(id, sharedDataSourceReferenceId)
		{
			DataSet item = new DataSet(dataSetCore);
			this.m_dataSets = new List<DataSet>(1);
			this.m_dataSets.Add(item);
		}

		internal void Initialize(InitializationContext context)
		{
			context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.DataSource;
			context.ObjectName = this.m_name;
			this.InternalInitialize(context);
			if (this.m_dataSets != null)
			{
				for (int i = 0; i < this.m_dataSets.Count; i++)
				{
					Global.Tracer.Assert(null != this.m_dataSets[i], "(null != m_dataSets[i])");
					this.m_dataSets[i].Initialize(context);
				}
				for (int j = 0; j < this.m_dataSets.Count; j++)
				{
					this.m_dataSets[j].CheckCircularDefaultRelationshipReference(context);
				}
			}
		}

		internal void DetermineDecomposability(InitializationContext context)
		{
			if (this.m_dataSets != null)
			{
				foreach (DataSet dataSet in this.m_dataSets)
				{
					dataSet.DetermineDecomposability(context);
				}
			}
		}

		internal string ResolveConnectionString(OnDemandProcessingContext pc, out DataSourceInfo dataSourceInfo)
		{
			dataSourceInfo = this.GetDataSourceInfo(pc);
			string text = null;
			if (dataSourceInfo != null)
			{
				text = dataSourceInfo.GetConnectionString(pc.DataProtection);
				if (!dataSourceInfo.IsReference && text == null)
				{
					text = this.EvaluateConnectStringExpression(pc);
				}
			}
			else
			{
				text = this.EvaluateConnectStringExpression(pc);
			}
			if (DataSourceInfo.HasUseridReference(text))
			{
				pc.ReportObjectModel.UserImpl.SetConnectionStringUserProfileDependencyOrThrow();
			}
			return text;
		}

		internal DataSourceInfo GetDataSourceInfo(OnDemandProcessingContext pc)
		{
			DataSourceInfo dataSourceInfo = null;
			if (pc.DataSourceInfos != null)
			{
				if (pc.IsSharedDataSetExecutionOnly)
				{
					dataSourceInfo = pc.DataSourceInfos.GetForSharedDataSetExecution();
				}
				else
				{
					if (Guid.Empty != this.ID)
					{
						dataSourceInfo = pc.DataSourceInfos.GetByID(this.ID);
					}
					if (dataSourceInfo == null)
					{
						dataSourceInfo = pc.DataSourceInfos.GetByName(this.Name, pc.ReportContext);
					}
				}
				if (dataSourceInfo == null)
				{                    
					throw new DataSourceNotFoundException(this.Name);
				}
			}
			//else if (this.DataSourceReference != null)
			//{                
   //             throw new DataSourceNotFoundException(this.Name);
			//}
			return dataSourceInfo;
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
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null, "(reportExprHost != null && reportObjectModel != null)");
				this.m_exprHost = reportExprHost.DataSourceHostsRemotable[this.ExprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		private string EvaluateConnectStringExpression(OnDemandProcessingContext processingContext)
		{
			if (this.m_connectString == null)
			{
				return null;
			}
			if (ExpressionInfo.Types.Constant == this.m_connectString.Type)
			{
				return this.m_connectString.StringValue;
			}
			Global.Tracer.Assert(null != processingContext.ReportRuntime, "(null != processingContext.ReportRuntime)");
			if (processingContext.ReportRuntime.ReportExprHost != null)
			{
				this.SetExprHost(processingContext.ReportRuntime.ReportExprHost, processingContext.ReportObjectModel);
			}
			AspNetCore.ReportingServices.RdlExpressions.StringResult stringResult = processingContext.ReportRuntime.EvaluateConnectString(this);
			if (stringResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsDataSourceConnectStringProcessingError, this.m_name);
			}
			return stringResult.Value;
		}

		internal bool AnyActiveDataSetNeedsAutoDetectCollation()
		{
			foreach (DataSet dataSet in this.m_dataSets)
			{
				if (!dataSet.UsedOnlyInParameters && dataSet.NeedAutoDetectCollation())
				{
					return true;
				}
			}
			return false;
		}

		internal void MergeCollationSettingsForAllDataSets(ErrorContext errorContext, string cultureName, bool caseSensitive, bool accentSensitive, bool kanatypeSensitive, bool widthSensitive)
		{
			for (int i = 0; i < this.m_dataSets.Count; i++)
			{
				DataSet dataSet = this.m_dataSets[i];
				dataSet.MergeCollationSettings(errorContext, this.m_type, cultureName, caseSensitive, accentSensitive, kanatypeSensitive, widthSensitive);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Transaction, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Type, Token.String));
			list.Add(new MemberInfo(MemberName.ConnectString, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntegratedSecurity, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Prompt, Token.String));
			list.Add(new MemberInfo(MemberName.DataSourceReference, Token.String));
			list.Add(new MemberInfo(MemberName.DataSets, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet));
			list.Add(new MemberInfo(MemberName.ID, Token.Guid));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.SharedDataSourceReferencePath, Token.String));
			list.Add(new MemberInfo(MemberName.ReferenceID, Token.Int32));
			list.Add(new MemberInfo(MemberName.IsArtificialDataSource, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(DataSource.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Transaction:
					writer.Write(this.m_transaction);
					break;
				case MemberName.Type:
					writer.Write(this.m_type);
					break;
				case MemberName.ConnectString:
					writer.Write(this.m_connectString);
					break;
				case MemberName.IntegratedSecurity:
					writer.Write(this.m_integratedSecurity);
					break;
				case MemberName.Prompt:
					writer.Write(this.m_prompt);
					break;
				case MemberName.DataSourceReference:
					writer.Write(this.m_dataSourceReference);
					break;
				case MemberName.DataSets:
					writer.Write(this.m_dataSets);
					break;
				case MemberName.ID:
					writer.Write(this.m_ID);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.SharedDataSourceReferencePath:
					writer.Write(this.m_sharedDataSourceReferencePath);
					break;
				case MemberName.ReferenceID:
					writer.Write(this.m_referenceID);
					break;
				case MemberName.IsArtificialDataSource:
					writer.Write(this.m_isArtificialDataSource);
					break;
				default:
					Global.Tracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(DataSource.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Transaction:
					this.m_transaction = reader.ReadBoolean();
					break;
				case MemberName.Type:
					this.m_type = reader.ReadString();
					break;
				case MemberName.ConnectString:
					this.m_connectString = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntegratedSecurity:
					this.m_integratedSecurity = reader.ReadBoolean();
					break;
				case MemberName.Prompt:
					this.m_prompt = reader.ReadString();
					break;
				case MemberName.DataSourceReference:
					this.m_dataSourceReference = reader.ReadString();
					break;
				case MemberName.DataSets:
					this.m_dataSets = reader.ReadGenericListOfRIFObjects<DataSet>();
					break;
				case MemberName.ID:
					this.m_ID = reader.ReadGuid();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.SharedDataSourceReferencePath:
					this.m_sharedDataSourceReferencePath = reader.ReadString();
					break;
				case MemberName.ReferenceID:
					this.m_referenceID = reader.ReadInt32();
					break;
				case MemberName.IsArtificialDataSource:
					this.m_isArtificialDataSource = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, string.Empty);
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSource;
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IReferenceable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSource;
		}
	}
}
