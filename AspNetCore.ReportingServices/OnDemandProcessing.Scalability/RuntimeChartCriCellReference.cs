using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeChartCriCellReference : RuntimeCellReference, IReference<RuntimeChartCriCell>, IReference, IStorable, IPersistable
	{
		internal RuntimeChartCriCellReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeChartCriCellReference;
		}

		[DebuggerStepThrough]
		public new RuntimeChartCriCell Value()
		{
			return (RuntimeChartCriCell)base.InternalValue();
		}
	}
}
