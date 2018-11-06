using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class OnDemandObjectModel : ObjectModel
	{
		public abstract Variables Variables
		{
			get;
		}

		public abstract Lookups Lookups
		{
			get;
		}

		public abstract object MinValue(params object[] arguments);

		public abstract object MaxValue(params object[] arguments);
	}
}
