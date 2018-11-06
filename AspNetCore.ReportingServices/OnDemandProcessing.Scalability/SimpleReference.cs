using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class SimpleReference<T> : Reference<T> where T : IStorable
	{
		[NonSerialized]
		private ObjectType m_objectType;

		internal SimpleReference(ObjectType referenceType)
		{
			this.m_objectType = referenceType;
		}

		public override ObjectType GetObjectType()
		{
			return this.m_objectType;
		}
	}
}
