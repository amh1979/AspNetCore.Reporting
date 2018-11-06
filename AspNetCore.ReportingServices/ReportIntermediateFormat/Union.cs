using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
//
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class Union : DataAggregate
	{
		protected AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode m_expressionType;

		protected object m_currentUnion;

		private static Declaration m_declaration = Union.GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType
		{
			get
			{
				return DataAggregateInfo.AggregateTypes.Union;
			}
		}

		public override int Size
		{
			get
			{
				return 4 + ItemSizes.SizeOf(this.m_currentUnion);
			}
		}

		internal override void Init()
		{
			this.m_expressionType = AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null;
			this.m_currentUnion = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			object obj = expressions[0];
			AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode = DataAggregate.GetTypeCode(obj);
			if (!DataAggregate.IsNull(typeCode))
			{
				if (!DataTypeUtility.IsSpatial(typeCode))
				{
					iErrorContext.Register(ProcessingErrorCode.rsUnionOfNonSpatialData, Severity.Warning);
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				if (this.m_expressionType == AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null)
				{
					this.m_expressionType = typeCode;
				}
				else if (typeCode != this.m_expressionType)
				{
					iErrorContext.Register(ProcessingErrorCode.rsUnionOfMixedSpatialTypes, Severity.Warning);
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				if (this.m_currentUnion == null)
				{
					this.m_expressionType = typeCode;
					this.m_currentUnion = obj;
				}
				//else if (this.m_expressionType == AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.SqlGeometry)
				//{
				//	this.m_currentUnion = ((SqlGeometry)this.m_currentUnion).STUnion((SqlGeometry)obj);
				//}
				//else
				//{
				//	this.m_currentUnion = ((SqlGeography)this.m_currentUnion).STUnion((SqlGeography)obj);
				//}
			}
		}

		internal override object Result()
		{
			return this.m_currentUnion;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new Union();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Union.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					writer.WriteEnum((int)this.m_expressionType);
					break;
				case MemberName.CurrentUnion:
					writer.Write(this.m_currentUnion);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Union.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					this.m_expressionType = (AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode)reader.ReadEnum();
					break;
				case MemberName.CurrentUnion:
					this.m_currentUnion = reader.ReadVariant();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "Union should not resolve references");
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Union;
		}

		public static Declaration GetDeclaration()
		{
			if (Union.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ExpressionType, Token.Enum));
				list.Add(new MemberInfo(MemberName.CurrentUnion, Token.Object));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Union, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregate, list);
			}
			return Union.m_declaration;
		}
	}
}
