using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal abstract class AggregateBucket<T> : IPersistable where T : IPersistable
	{
		private List<T> m_aggregates;

		private int m_level;

		public List<T> Aggregates
		{
			get
			{
				return this.m_aggregates;
			}
			set
			{
				this.m_aggregates = value;
			}
		}

		public int Level
		{
			get
			{
				return this.m_level;
			}
			set
			{
				this.m_level = value;
			}
		}

		internal AggregateBucket()
		{
		}

		internal AggregateBucket(int level)
		{
			this.m_aggregates = new List<T>();
			this.m_level = level;
		}

		protected abstract Declaration GetSpecificDeclaration();

		public abstract AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(this.GetSpecificDeclaration());
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Aggregates:
					writer.Write<T>(this.m_aggregates);
					break;
				case MemberName.Level:
					writer.Write(this.m_level);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(this.GetSpecificDeclaration());
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Aggregates:
					this.m_aggregates = reader.ReadGenericListOfRIFObjects<T>();
					break;
				case MemberName.Level:
					this.m_level = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "No references to resolve.");
		}
	}
}
