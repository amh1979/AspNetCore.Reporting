using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeTablixGroupLeafObjReference : RuntimeDataTablixGroupLeafObjReference, IReference<RuntimeTablixGroupLeafObj>, IReference<ISortDataHolder>, IReference, IStorable, IPersistable
	{
		internal RuntimeTablixGroupLeafObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeTablixGroupLeafObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeTablixGroupLeafObj Value()
		{
			return (RuntimeTablixGroupLeafObj)base.InternalValue();
		}

		[DebuggerStepThrough]
		ISortDataHolder IReference<ISortDataHolder>.Value()
		{
			return (ISortDataHolder)base.InternalValue();
		}
	}
}
