using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDataTablixObjReference : RuntimeRDLDataRegionObjReference, IReference<RuntimeDataTablixObj>, IOnDemandMemberOwnerInstanceReference, IReference<IOnDemandScopeInstance>, IReference<IOnDemandMemberOwnerInstance>, IReference, IStorable, IPersistable
	{
		internal RuntimeDataTablixObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDataTablixObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeDataTablixObj Value()
		{
			return (RuntimeDataTablixObj)base.InternalValue();
		}

		[DebuggerStepThrough]
		IOnDemandScopeInstance IReference<IOnDemandScopeInstance>.Value()
		{
			return (IOnDemandScopeInstance)base.InternalValue();
		}

		[DebuggerStepThrough]
		IOnDemandMemberOwnerInstance IReference<IOnDemandMemberOwnerInstance>.Value()
		{
			return (IOnDemandMemberOwnerInstance)base.InternalValue();
		}
	}
}
