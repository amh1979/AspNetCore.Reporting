using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ReportQuery : IPersistable
	{
		private CommandType m_commandType = CommandType.Text;

		private ExpressionInfo m_commandText;

		private List<ParameterValue> m_queryParameters;

		private int m_timeOut;

		[NonSerialized]
		private string m_dataSourceName;

		[NonSerialized]
		private IndexedExprHost m_queryParamsExprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ReportQuery.GetDeclaration();

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

		internal List<ParameterValue> Parameters
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
				AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType = context.ObjectType;
				string objectName = context.ObjectName;
				context.ObjectType = AspNetCore.ReportingServices.ReportProcessing.ObjectType.QueryParameter;
				context.ExprHostBuilder.QueryParametersStart();
				for (int i = 0; i < this.m_queryParameters.Count; i++)
				{
					ParameterValue parameterValue = this.m_queryParameters[i];
					context.ObjectName = parameterValue.Name;
					parameterValue.Initialize(null, context, true);
				}
				context.ExprHostBuilder.QueryParametersEnd();
				context.ObjectType = objectType;
				context.ObjectName = objectName;
			}
		}

		internal void SetExprHost(IndexedExprHost queryParamsExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(queryParamsExprHost != null && reportObjectModel != null, "(queryParamsExprHost != null && reportObjectModel != null)");
			this.m_queryParamsExprHost = queryParamsExprHost;
			this.m_queryParamsExprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.CommandType, Token.Enum));
			list.Add(new MemberInfo(MemberName.CommandText, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.QueryParameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue));
			list.Add(new MemberInfo(MemberName.Timeout, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportQuery, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ReportQuery.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CommandType:
					writer.WriteEnum((int)this.m_commandType);
					break;
				case MemberName.CommandText:
					writer.Write(this.m_commandText);
					break;
				case MemberName.QueryParameters:
					writer.Write(this.m_queryParameters);
					break;
				case MemberName.Timeout:
					writer.Write(this.m_timeOut);
					break;
				default:
					Global.Tracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ReportQuery.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.CommandType:
					this.m_commandType = (CommandType)reader.ReadEnum();
					break;
				case MemberName.CommandText:
					this.m_commandText = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.QueryParameters:
					this.m_queryParameters = reader.ReadGenericListOfRIFObjects<ParameterValue>();
					break;
				case MemberName.Timeout:
					this.m_timeOut = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, string.Empty);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportQuery;
		}
	}
}
