using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal interface IInterleave : IStorable, IPersistable
	{
		int Index
		{
			get;
		}

		long Location
		{
			get;
		}

		void Write(TextWriter output);
	}
}
