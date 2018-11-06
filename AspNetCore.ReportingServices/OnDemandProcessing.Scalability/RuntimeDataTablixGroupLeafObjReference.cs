using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDataTablixGroupLeafObjReference : RuntimeGroupLeafObjReference, IReference<RuntimeDataTablixGroupLeafObj>, IOnDemandMemberInstanceReference, IOnDemandMemberOwnerInstanceReference, IReference<IOnDemandScopeInstance>, IReference<IOnDemandMemberOwnerInstance>, IReference<IOnDemandMemberInstance>, IReference, IStorable, IPersistable
	{
		internal RuntimeDataTablixGroupLeafObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDataTablixGroupLeafObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeDataTablixGroupLeafObj Value()
		{
			return (RuntimeDataTablixGroupLeafObj)base.InternalValue();
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

		[DebuggerStepThrough]
		IOnDemandMemberInstance IReference<IOnDemandMemberInstance>.Value()
		{
			return (IOnDemandMemberInstance)base.InternalValue();
		}
	}
}
