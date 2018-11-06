using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class Avg : Sum
	{
		private uint m_currentCount;

		private static Declaration m_declaration = Avg.GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType
		{
			get
			{
				return DataAggregateInfo.AggregateTypes.Avg;
			}
		}

		public override int Size
		{
			get
			{
				return base.Size + 4;
			}
		}

		internal override void Init()
		{
			base.Init();
			this.m_currentCount = 0u;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			object o = expressions[0];
			AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode = DataAggregate.GetTypeCode(o);
			if (!DataAggregate.IsNull(typeCode))
			{
				base.Update(expressions, iErrorContext);
				this.m_currentCount += 1u;
			}
		}

		internal override object Result()
		{
			switch (base.m_currentTotalType)
			{
			case AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null:
				return null;
			case AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double:
				return (double)base.m_currentTotal / (double)this.m_currentCount;
			case AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal:
				return (decimal)base.m_currentTotal / (decimal)this.m_currentCount;
			default:
				Global.Tracer.Assert(false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new Avg();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Avg.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.CurrentCount)
				{
					writer.Write(this.m_currentCount);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(Avg.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.CurrentCount)
				{
					this.m_currentCount = reader.ReadUInt32();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "Avg should not resolve references");
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Avg;
		}

		public new static Declaration GetDeclaration()
		{
			if (Avg.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.CurrentCount, Token.UInt32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Avg, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sum, list);
			}
			return Avg.m_declaration;
		}
	}
}
