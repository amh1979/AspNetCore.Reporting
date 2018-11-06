using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeGroupObjReference : RuntimeHierarchyObjReference, IReference<RuntimeGroupObj>, IReference<AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>, IReference, IStorable, IPersistable
	{
		internal RuntimeGroupObjReference()
		{
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeGroupObj Value()
		{
			return (RuntimeGroupObj)base.InternalValue();
		}

		AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner IReference<AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>.Value()
		{
			return (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner)base.InternalValue();
		}
	}
}
