using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeTablixCellReference : RuntimeCellReference, IReference<RuntimeTablixCell>, IReference, IStorable, IPersistable
	{
		internal RuntimeTablixCellReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeTablixCellReference;
		}

		[DebuggerStepThrough]
		public new RuntimeTablixCell Value()
		{
			return (RuntimeTablixCell)base.InternalValue();
		}
	}
}
