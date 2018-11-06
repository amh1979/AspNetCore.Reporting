using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class CountRows : DataAggregate
	{
		private int m_currentTotal;

		private static Declaration m_declaration = CountRows.GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType
		{
			get
			{
				return DataAggregateInfo.AggregateTypes.CountRows;
			}
		}

		public override int Size
		{
			get
			{
				return 4;
			}
		}

		internal override void Init()
		{
			this.m_currentTotal = 0;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			this.m_currentTotal++;
		}

		internal override object Result()
		{
			return this.m_currentTotal;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new CountRows();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(CountRows.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.CurrentTotal)
				{
					writer.Write(this.m_currentTotal);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(CountRows.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.CurrentTotal)
				{
					this.m_currentTotal = reader.ReadInt32();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "CountRows should not resolve references");
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CountRows;
		}

		public static Declaration GetDeclaration()
		{
			if (CountRows.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.CurrentTotal, Token.Int32));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CountRows, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return CountRows.m_declaration;
		}
	}
}
