using System.IO;

namespace AspNetCore.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IStreamHandler
	{
		Stream OpenStream();
	}
}
