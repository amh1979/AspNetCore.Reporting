using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ReportRendererFactory
	{
		private ReportRendererFactory()
		{
		}

		internal static IRenderingExtension GetNewRenderer(string format, IExtensionFactory extFactory)
		{
			IRenderingExtension renderingExtension = null;
			try
			{
				return (IRenderingExtension)extFactory.GetNewRendererExtensionClass(format);
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				throw new ReportProcessingException(ErrorCode.rsRenderingExtensionNotFound);
			}
		}
	}
}
