using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Field : IPersistable, IStaticReferenceable
	{
		[NonSerialized]
		private const int AggregateIndicatorFieldNotSpecified = -1;

		private string m_name;

		private string m_dataField;

		private ExpressionInfo m_value;

		private int m_exprHostID = -1;

		private DataType m_constantDataType = DataType.String;

		private int m_aggregateIndicatorFieldIndex = -1;

		[NonSerialized]
		private CalcFieldExprHost m_exprHost;

		[NonSerialized]
		private ObjectModelImpl m_lastReportOm;

		[NonSerialized]
		private int m_id = -2147483648;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Field.GetDeclaration();

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

		internal string DataField
		{
			get
			{
				return this.m_dataField;
			}
			set
			{
				this.m_dataField = value;
			}
		}

		internal int AggregateIndicatorFieldIndex
		{
			get
			{
				return this.m_aggregateIndicatorFieldIndex;
			}
			set
			{
				this.m_aggregateIndicatorFieldIndex = value;
			}
		}

		internal bool HasAggregateIndicatorField
		{
			get
			{
				return this.m_aggregateIndicatorFieldIndex != -1;
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

		internal bool IsCalculatedField
		{
			get
			{
				return this.m_dataField == null;
			}
		}

		internal DataType DataType
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

		internal CalcFieldExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		public int ID
		{
			get
			{
				return this.m_id;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			if (this.Value != null)
			{
				context.ExprHostBuilder.CalcFieldStart(this.m_name);
				this.m_value.Initialize("Field", context);
				context.ExprHostBuilder.GenericValue(this.m_value);
				this.m_exprHostID = context.ExprHostBuilder.CalcFieldEnd();
			}
		}

		internal void SetExprHost(DataSetExprHost dataSetExprHost, ObjectModelImpl reportObjectModel)
		{
			if (this.ExprHostID >= 0)
			{
				Global.Tracer.Assert(dataSetExprHost != null && reportObjectModel != null, "(dataSetExprHost != null && reportObjectModel != null)");
				this.m_exprHost = dataSetExprHost.FieldHostsRemotable[this.ExprHostID];
				this.EnsureExprHostReportObjectModelBinding(reportObjectModel);
			}
		}

		internal void EnsureExprHostReportObjectModelBinding(ObjectModelImpl reportObjectModel)
		{
			if (this.m_exprHost != null && this.m_lastReportOm != reportObjectModel)
			{
				this.m_lastReportOm = reportObjectModel;
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.DataField, Token.String));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.AggregateIndicatorFieldIndex, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Field, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Field.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.DataField:
					writer.Write(this.m_dataField);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				case MemberName.DataType:
					writer.WriteEnum((int)this.m_constantDataType);
					break;
				case MemberName.AggregateIndicatorFieldIndex:
					writer.Write(this.m_aggregateIndicatorFieldIndex);
					break;
				default:
					Global.Tracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Field.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = reader.ReadString();
					break;
				case MemberName.DataField:
					this.m_dataField = reader.ReadString();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.DataType:
					this.m_constantDataType = (DataType)reader.ReadEnum();
					break;
				case MemberName.AggregateIndicatorFieldIndex:
					this.m_aggregateIndicatorFieldIndex = reader.ReadInt32();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Field;
		}

		public void SetID(int id)
		{
			this.m_id = id;
		}
	}
}
