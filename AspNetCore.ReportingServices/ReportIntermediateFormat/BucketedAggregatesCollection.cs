using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal abstract class BucketedAggregatesCollection<T> : IEnumerable<T>, IEnumerable, IPersistable where T : IPersistable
	{
		private List<AggregateBucket<T>> m_buckets;

		public List<AggregateBucket<T>> Buckets
		{
			get
			{
				return this.m_buckets;
			}
			set
			{
				this.m_buckets = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.m_buckets.Count == 0;
			}
		}

		public BucketedAggregatesCollection()
		{
			this.m_buckets = new List<AggregateBucket<T>>();
		}

		public AggregateBucket<T> GetOrCreateBucket(int level)
		{
			AggregateBucket<T> aggregateBucket = null;
			for (int i = 0; i < this.m_buckets.Count; i++)
			{
				if (aggregateBucket != null)
				{
					break;
				}
				AggregateBucket<T> aggregateBucket2 = this.m_buckets[i];
				if (aggregateBucket2.Level == level)
				{
					aggregateBucket = aggregateBucket2;
				}
				else if (aggregateBucket2.Level > level)
				{
					aggregateBucket = this.CreateBucket(level);
					this.m_buckets.Insert(i, aggregateBucket);
				}
			}
			if (aggregateBucket == null)
			{
				aggregateBucket = this.CreateBucket(level);
				this.m_buckets.Add(aggregateBucket);
			}
			return aggregateBucket;
		}

		public void MergeFrom(BucketedAggregatesCollection<T> otherCol)
		{
			if (otherCol != null)
			{
				foreach (AggregateBucket<T> bucket in otherCol.Buckets)
				{
					AggregateBucket<T> orCreateBucket = this.GetOrCreateBucket(bucket.Level);
					orCreateBucket.Aggregates.AddRange(bucket.Aggregates);
				}
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (AggregateBucket<T> bucket in this.m_buckets)
			{
				foreach (T aggregate in bucket.Aggregates)
				{
					yield return aggregate;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		protected abstract AggregateBucket<T> CreateBucket(int level);

		protected abstract Declaration GetSpecificDeclaration();

		public abstract AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType();

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(this.GetSpecificDeclaration());
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Buckets)
				{
					writer.Write<AggregateBucket<T>>(this.m_buckets);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(this.GetSpecificDeclaration());
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Buckets)
				{
					this.m_buckets = reader.ReadGenericListOfRIFObjects<AggregateBucket<T>>();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false, "No references to resolve.");
		}
	}
}
