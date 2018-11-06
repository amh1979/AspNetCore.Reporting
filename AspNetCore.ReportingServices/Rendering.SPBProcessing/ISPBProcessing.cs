using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal interface ISPBProcessing
	{
		bool Done
		{
			get;
		}

		void SetContext(SPBContext context);

		Stream GetNextPage(out RPLReport rplReport);
	}
}
