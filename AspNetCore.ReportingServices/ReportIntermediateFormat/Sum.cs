using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class Sum : DataAggregate
	{
		private AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode m_expressionType;

		protected AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode m_currentTotalType;

		protected object m_currentTotal;

		private static Declaration m_declaration = Sum.GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType
		{
			get
			{
				return DataAggregateInfo.AggregateTypes.Sum;
			}
		}

		public override int Size
		{
			get
			{
				return 8 + ItemSizes.SizeOf(this.m_currentTotal);
			}
		}

		internal override void Init()
		{
			this.m_currentTotalType = AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null;
			this.m_currentTotal = null;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			object obj = expressions[0];
			AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode = DataAggregate.GetTypeCode(obj);
			if (!DataAggregate.IsNull(typeCode))
			{
				if (!DataTypeUtility.IsNumeric(typeCode))
				{
					iErrorContext.Register(ProcessingErrorCode.rsAggregateOfNonNumericData, Severity.Warning);
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				DataAggregate.ConvertToDoubleOrDecimal(typeCode, obj, out typeCode, out obj);
				if (this.m_expressionType == AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null)
				{
					this.m_expressionType = typeCode;
				}
				else if (typeCode != this.m_expressionType)
				{
					iErrorContext.Register(ProcessingErrorCode.rsAggregateOfMixedDataTypes, Severity.Warning);
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				if (this.m_currentTotal == null)
				{
					this.m_currentTotalType = typeCode;
					this.m_currentTotal = obj;
				}
				else
				{
					this.m_currentTotal = DataAggregate.Add(this.m_currentTotalType, this.m_currentTotal, typeCode, obj);
				}
			}
		}

		internal override object Result()
		{
			return this.m_currentTotal;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new Sum();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(Sum.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					writer.WriteEnum((int)this.m_expressionType);
					break;
				case MemberName.CurrentTotalType:
					writer.WriteEnum((int)this.m_currentTotalType);
					break;
				case MemberName.CurrentTotal:
					writer.Write(this.m_currentTotal);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(Sum.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					this.m_expressionType = (AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode)reader.ReadEnum();
					break;
				case MemberName.CurrentTotalType:
					this.m_currentTotalType = (AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode)reader.ReadEnum();
					break;
				case MemberName.CurrentTotal:
					this.m_currentTotal = reader.ReadVariant();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "Sum should not resolve references");
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sum;
		}

		public static Declaration GetDeclaration()
		{
			if (Sum.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ExpressionType, Token.Enum));
				list.Add(new MemberInfo(MemberName.CurrentTotalType, Token.Enum));
				list.Add(new MemberInfo(MemberName.CurrentTotal, Token.Object));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sum, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregate, list);
			}
			return Sum.m_declaration;
		}
	}
}
