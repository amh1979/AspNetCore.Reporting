using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class Navigation
	{
		internal readonly AspNetCore.ReportingServices.ReportIntermediateFormat.Navigation m_navigation;

		internal Navigation(AspNetCore.ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayout)
		{
			this.m_navigation = bandLayout.Navigation;
		}
	}
}
