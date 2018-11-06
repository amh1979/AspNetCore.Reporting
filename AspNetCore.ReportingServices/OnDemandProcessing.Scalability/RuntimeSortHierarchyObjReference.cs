using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeSortHierarchyObjReference : Reference<IHierarchyObj>, IReference<RuntimeSortHierarchyObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeSortHierarchyObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeSortHierarchyObjReference;
		}

		[DebuggerStepThrough]
		public RuntimeSortHierarchyObj Value()
		{
			return (RuntimeSortHierarchyObj)base.InternalValue();
		}
	}
}
