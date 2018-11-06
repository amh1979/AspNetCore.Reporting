using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal interface ISortDataHolder : IStorable, IPersistable
	{
		void NextRow();

		void Traverse(ProcessingStages operation, ITraversalContext traversalContext);
	}
}
