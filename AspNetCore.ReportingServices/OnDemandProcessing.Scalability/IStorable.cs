using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IStorable : IPersistable
	{
		int Size
		{
			get;
		}
	}
}
