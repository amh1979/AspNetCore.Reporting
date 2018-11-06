using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class CountDistinct : DataAggregate
	{
		private Hashtable m_distinctValues = new Hashtable();

		private static Declaration m_declaration = CountDistinct.GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType
		{
			get
			{
				return DataAggregateInfo.AggregateTypes.CountDistinct;
			}
		}

		public override int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_distinctValues);
			}
		}

		internal override void Init()
		{
			this.m_distinctValues.Clear();
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			object obj = expressions[0];
			AspNetCore.ReportingServices.ReportProcessing.DataAggregate.DataTypeCode typeCode = DataAggregate.GetTypeCode(obj);
			if (!DataAggregate.IsNull(typeCode))
			{
				if (!DataAggregate.IsVariant(typeCode) || DataTypeUtility.IsSpatial(typeCode))
				{
					iErrorContext.Register(ProcessingErrorCode.rsInvalidExpressionDataType, Severity.Warning);
					throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
				}
				if (!this.m_distinctValues.ContainsKey(obj))
				{
					this.m_distinctValues.Add(obj, null);
				}
			}
		}

		internal override object Result()
		{
			return this.m_distinctValues.Count;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new CountDistinct();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(CountDistinct.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.DistinctValues)
				{
					writer.WriteVariantVariantHashtable(this.m_distinctValues);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(CountDistinct.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.DistinctValues)
				{
					this.m_distinctValues = reader.ReadVariantVariantHashtable();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "CountDistinct should not resolve references");
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CountDistinct;
		}

		public static Declaration GetDeclaration()
		{
			if (CountDistinct.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.DistinctValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VariantVariantHashtable));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CountDistinct, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return CountDistinct.m_declaration;
		}
	}
}
