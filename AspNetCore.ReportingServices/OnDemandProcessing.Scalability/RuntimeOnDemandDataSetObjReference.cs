using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeOnDemandDataSetObjReference : IScopeReference, IReference<IHierarchyObj>, IReference<RuntimeOnDemandDataSetObj>, IReference<AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>, IReference, IStorable, IPersistable
	{
		internal RuntimeOnDemandDataSetObjReference()
		{
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference;
		}

		[DebuggerStepThrough]
		IHierarchyObj IReference<IHierarchyObj>.Value()
		{
			return (IHierarchyObj)base.InternalValue();
		}

		[DebuggerStepThrough]
		public new RuntimeOnDemandDataSetObj Value()
		{
			return (RuntimeOnDemandDataSetObj)base.InternalValue();
		}

		[DebuggerStepThrough]
		AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner IReference<AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>.Value()
		{
			return (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner)base.InternalValue();
		}
	}
}
