using AspNetCore.ReportingServices.OnDemandProcessing;
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
	internal class ParameterValue : IPersistable
	{
		private string m_name;

		private string m_uniqueName;

		private ExpressionInfo m_value;

		private DataType m_constantDataType = DataType.String;

		private int m_exprHostID = -1;

		private ExpressionInfo m_omit;

		[NonSerialized]
		private ParamExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ParameterValue.GetDeclaration();

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

		internal DataType ConstantDataType
		{
			get
			{
				return this.m_constantDataType;
			}
			set
			{
				this.m_constantDataType = value;
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

		internal string UniqueName
		{
			get
			{
				return this.m_uniqueName ?? this.m_name;
			}
			set
			{
				this.m_uniqueName = value;
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

		internal void Initialize(string containerPropertyName, InitializationContext context, bool queryParam)
		{
			string str = (containerPropertyName == null) ? "" : (containerPropertyName + ".");
			if (this.m_value != null)
			{
				this.m_value.Initialize(str + "Value", context);
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
				this.m_omit.Initialize(str + "Omit", context);
				context.ExprHostBuilder.ParameterOmit(this.m_omit);
			}
		}

		internal void SetExprHost(IList<ParamExprHost> paramExprHosts, ObjectModelImpl reportObjectModel)
		{
			if (this.m_exprHostID >= 0)
			{
				Global.Tracer.Assert(paramExprHosts != null && reportObjectModel != null, "(paramExprHosts != null && reportObjectModel != null)");
				this.m_exprHost = paramExprHosts[this.m_exprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal object EvaluateQueryParameterValue(OnDemandProcessingContext odpContext, DataSetExprHost dataSetExprHost)
		{
			return odpContext.ReportRuntime.EvaluateQueryParamValue(this.m_value, (dataSetExprHost != null) ? dataSetExprHost.QueryParametersHost : null, AspNetCore.ReportingServices.ReportProcessing.ObjectType.QueryParameter, this.m_name);
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ParameterValue parameterValue = (ParameterValue)base.MemberwiseClone();
			if (this.m_name != null)
			{
				parameterValue.m_name = (string)this.m_name.Clone();
			}
			if (this.m_value != null)
			{
				parameterValue.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			parameterValue.m_constantDataType = this.m_constantDataType;
			if (this.m_omit != null)
			{
				parameterValue.m_omit = (ExpressionInfo)this.m_omit.PublishClone(context);
			}
			return parameterValue;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.Omit, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UniqueName, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ParameterValue.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.DataType:
					writer.WriteEnum((int)this.m_constantDataType);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.Omit:
					writer.Write(this.m_omit);
					break;
				case MemberName.UniqueName:
					writer.Write(this.m_uniqueName);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ParameterValue.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DataType:
					this.m_constantDataType = (DataType)reader.ReadEnum();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.Omit:
					this.m_omit = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UniqueName:
					this.m_uniqueName = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public virtual AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ParameterValue;
		}
	}
}
