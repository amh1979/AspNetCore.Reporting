using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeDataSourceFullDataProcessing : RuntimeDataSourceDataProcessing
	{
		protected override bool NeedsExecutionLogging
		{
			get
			{
				return false;
			}
		}

		internal RuntimeDataSourceFullDataProcessing(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, OnDemandProcessingContext processingContext)
			: base(dataSet, processingContext)
		{
		}

		protected override RuntimeOnDemandDataSet CreateRuntimeDataSet()
		{
			OnDemandProcessingContext odpContext = base.OdpContext;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance currentReportInstance = odpContext.CurrentReportInstance;
			DataSetInstance dataSetInstance = currentReportInstance.GetDataSetInstance(base.m_dataSet, odpContext);
			if (odpContext.IsTablixProcessingComplete(base.m_dataSet.IndexInCollection))
			{
				Global.Tracer.Trace(TraceLevel.Warning, "Tablix processing is being attempted multiple times on DataSet '{0}'.", base.m_dataSet.Name.MarkAsPrivate());
			}
			return new RuntimeOnDemandDataSet(base.DataSourceDefinition, base.m_dataSet, dataSetInstance, odpContext, false, true, true);
		}

		protected override void OpenInitialConnectionAndTransaction()
		{
		}
	}
}
