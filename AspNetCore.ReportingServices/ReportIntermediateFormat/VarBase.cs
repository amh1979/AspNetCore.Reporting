using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal abstract class VarBase : DataAggregate
	{
		private AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode m_expressionType;

		protected uint m_currentCount;

		protected AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode m_sumOfXType;

		protected object m_sumOfX;

		protected object m_sumOfXSquared;

		private static Declaration m_declaration = VarBase.GetDeclaration();

		public override int Size
		{
			get
			{
				return 12 + ItemSizes.SizeOf(this.m_sumOfX) + ItemSizes.SizeOf(this.m_sumOfXSquared);
			}
		}

		internal override void Init()
		{
			this.m_currentCount = 0u;
			this.m_sumOfXType = AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null;
			this.m_sumOfX = null;
			this.m_sumOfXSquared = null;
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
				object obj2 = DataAggregate.Square(typeCode, obj);
				if (this.m_sumOfX == null)
				{
					this.m_sumOfXType = typeCode;
					this.m_sumOfX = obj;
					this.m_sumOfXSquared = obj2;
				}
				else
				{
					this.m_sumOfX = DataAggregate.Add(this.m_sumOfXType, this.m_sumOfX, typeCode, obj);
					this.m_sumOfXSquared = DataAggregate.Add(this.m_sumOfXType, this.m_sumOfXSquared, typeCode, obj2);
				}
				this.m_currentCount += 1u;
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(VarBase.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					writer.WriteEnum((int)this.m_expressionType);
					break;
				case MemberName.CurrentCount:
					writer.Write(this.m_currentCount);
					break;
				case MemberName.SumOfXType:
					writer.WriteEnum((int)this.m_sumOfXType);
					break;
				case MemberName.SumOfX:
					writer.Write(this.m_sumOfX);
					break;
				case MemberName.SumOfXSquared:
					writer.Write(this.m_sumOfXSquared);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(VarBase.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ExpressionType:
					this.m_expressionType = (AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode)reader.ReadEnum();
					break;
				case MemberName.CurrentCount:
					this.m_currentCount = reader.ReadUInt32();
					break;
				case MemberName.SumOfXType:
					this.m_sumOfXType = (AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode)reader.ReadEnum();
					break;
				case MemberName.SumOfX:
					this.m_sumOfX = reader.ReadVariant();
					break;
				case MemberName.SumOfXSquared:
					this.m_sumOfXSquared = reader.ReadVariant();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "VarBase should not resolve references");
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VarBase;
		}

		public static Declaration GetDeclaration()
		{
			if (VarBase.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ExpressionType, Token.Enum));
				list.Add(new MemberInfo(MemberName.CurrentCount, Token.UInt32));
				list.Add(new MemberInfo(MemberName.SumOfXType, Token.Enum));
				list.Add(new MemberInfo(MemberName.SumOfX, Token.Object));
				list.Add(new MemberInfo(MemberName.SumOfXSquared, Token.Object));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VarBase, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return VarBase.m_declaration;
		}
	}
}
