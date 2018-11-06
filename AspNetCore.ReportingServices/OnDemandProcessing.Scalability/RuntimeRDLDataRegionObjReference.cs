using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeRDLDataRegionObjReference : RuntimeDataRegionObjReference, IReference<IHierarchyObj>, IReference<RuntimeRDLDataRegionObj>, IReference<IDataRowSortOwner>, IReference<AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>, IReference<IDataCorrelation>, IReference, IStorable, IPersistable
	{
		internal RuntimeRDLDataRegionObjReference()
		{
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObjReference;
		}

		[DebuggerStepThrough]
		IHierarchyObj IReference<IHierarchyObj>.Value()
		{
			return (IHierarchyObj)base.InternalValue();
		}

		[DebuggerStepThrough]
		public new RuntimeRDLDataRegionObj Value()
		{
			return (RuntimeRDLDataRegionObj)base.InternalValue();
		}

		[DebuggerStepThrough]
		IDataRowSortOwner IReference<IDataRowSortOwner>.Value()
		{
			return (IDataRowSortOwner)base.InternalValue();
		}

		[DebuggerStepThrough]
		AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner IReference<AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>.Value()
		{
			return (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner)base.InternalValue();
		}

		[DebuggerStepThrough]
		IDataCorrelation IReference<IDataCorrelation>.Value()
		{
			return (IDataCorrelation)base.InternalValue();
		}
	}
}
