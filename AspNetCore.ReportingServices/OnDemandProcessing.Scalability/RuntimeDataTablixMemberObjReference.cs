using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDataTablixMemberObjReference : RuntimeMemberObjReference, IReference<RuntimeDataTablixMemberObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeDataTablixMemberObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDataTablixMemberObjReference;
		}

		[DebuggerStepThrough]
		public RuntimeDataTablixMemberObj Value()
		{
			return (RuntimeDataTablixMemberObj)base.InternalValue();
		}
	}
}
