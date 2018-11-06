using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal interface IChartMapper : IDVMappingLayer, IDisposable
	{
		void RenderChart();
	}
}
