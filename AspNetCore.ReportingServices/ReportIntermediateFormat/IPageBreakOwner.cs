using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal interface IPageBreakOwner
	{
		PageBreak PageBreak
		{
			get;
			set;
		}

		ObjectType ObjectType
		{
			get;
		}

		string ObjectName
		{
			get;
		}

		IInstancePath InstancePath
		{
			get;
		}
	}
}
