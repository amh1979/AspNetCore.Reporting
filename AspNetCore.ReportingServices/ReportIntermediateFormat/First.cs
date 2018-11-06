using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class First : DataAggregate
	{
		private object m_value;

		private bool m_updated;

		private static Declaration m_declaration = First.GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType
		{
			get
			{
				return DataAggregateInfo.AggregateTypes.First;
			}
		}

		public override int Size
		{
			get
			{
				return ItemSizes.SizeOf(this.m_value) + 1;
			}
		}

		internal override void Init()
		{
			this.m_value = null;
			this.m_updated = false;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			if (!this.m_updated)
			{
				this.m_value = expressions[0];
				this.m_updated = true;
			}
		}

		internal override object Result()
		{
			return this.m_value;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new First();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(First.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.Updated:
					writer.Write(this.m_updated);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(First.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Value:
					this.m_value = reader.ReadVariant();
					break;
				case MemberName.Updated:
					this.m_updated = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "First should not resolve references");
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.First;
		}

		public static Declaration GetDeclaration()
		{
			if (First.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Value, Token.Object));
				list.Add(new MemberInfo(MemberName.Updated, Token.Boolean));
				return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.First, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregate, list);
			}
			return First.m_declaration;
		}
	}
}
