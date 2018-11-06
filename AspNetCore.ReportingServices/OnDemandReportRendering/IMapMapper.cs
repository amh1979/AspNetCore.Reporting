using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal interface IMapMapper : IDVMappingLayer, IDisposable
	{
		void RenderMap();
	}
}
