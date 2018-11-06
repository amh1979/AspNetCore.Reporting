using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal interface IGaugeMapper : IDVMappingLayer, IDisposable
	{
		void RenderGaugePanel();

		void RenderDataGaugePanel();
	}
}
