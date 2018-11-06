using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeChartCriGroupLeafObjReference : RuntimeDataTablixGroupLeafObjReference, IReference<RuntimeChartCriGroupLeafObj>, IReference<ISortDataHolder>, IReference, IStorable, IPersistable
	{
		internal RuntimeChartCriGroupLeafObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeChartCriGroupLeafObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeChartCriGroupLeafObj Value()
		{
			return (RuntimeChartCriGroupLeafObj)base.InternalValue();
		}

		[DebuggerStepThrough]
		ISortDataHolder IReference<ISortDataHolder>.Value()
		{
			return (ISortDataHolder)base.InternalValue();
		}
	}
}
