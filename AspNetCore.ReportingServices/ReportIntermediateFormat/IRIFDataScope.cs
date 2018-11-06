using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IRIFDataScope
	{
		string Name
		{
			get;
		}

		DataScopeInfo DataScopeInfo
		{
			get;
		}

		ObjectType DataScopeObjectType
		{
			get;
		}
	}
}
