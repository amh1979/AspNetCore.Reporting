using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class SharedDataSetQuery : IPersistable
	{
		private List<ParameterValue> m_queryParameters;

		private string m_originalSharedDataSetReference;

		[NonSerialized]
		private IndexedExprHost m_queryParamsExprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = SharedDataSetQuery.GetDeclaration();

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

		internal string SharedDataSetReference
		{
			get
			{
				return this.m_originalSharedDataSetReference;
			}
			set
			{
				this.m_originalSharedDataSetReference = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
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
			list.Add(new MemberInfo(MemberName.QueryParameters, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue));
			list.Add(new MemberInfo(MemberName.OriginalSharedDataSetReference, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SharedDataSetQuery, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(SharedDataSetQuery.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.QueryParameters:
					writer.Write(this.m_queryParameters);
					break;
				case MemberName.OriginalSharedDataSetReference:
					writer.Write(this.m_originalSharedDataSetReference);
					break;
				default:
					Global.Tracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(SharedDataSetQuery.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.QueryParameters:
					this.m_queryParameters = reader.ReadGenericListOfRIFObjects<ParameterValue>();
					break;
				case MemberName.OriginalSharedDataSetReference:
					this.m_originalSharedDataSetReference = reader.ReadString();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SharedDataSetQuery;
		}
	}
}
