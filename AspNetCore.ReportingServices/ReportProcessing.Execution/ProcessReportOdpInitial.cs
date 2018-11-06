using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportOdpInitial : ProcessReportOdp
	{
		private readonly DateTime m_executionTime;

		protected override bool SnapshotProcessing
		{
			get
			{
				return false;
			}
		}

		protected override bool ReprocessSnapshot
		{
			get
			{
				return false;
			}
		}

		protected override bool ProcessWithCachedData
		{
			get
			{
				return false;
			}
		}

		protected override OnDemandProcessingContext.Mode OnDemandProcessingMode
		{
			get
			{
				return OnDemandProcessingContext.Mode.Full;
			}
		}

		public ProcessReportOdpInitial(IConfiguration configuration, ProcessingContext pc, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, DateTime executionTime)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext)
		{
			this.m_executionTime = executionTime;
		}

		protected override OnDemandMetadata PrepareMetadata()
		{
			OnDemandMetadata onDemandMetadata = new OnDemandMetadata(base.ReportDefinition);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot2 = onDemandMetadata.ReportSnapshot = new AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot(base.ReportDefinition, base.PublicProcessingContext.ReportContext.ItemName, base.PublicProcessingContext.Parameters, base.PublicProcessingContext.RequestUserName, this.m_executionTime, base.PublicProcessingContext.ReportContext.HostRootUri, base.PublicProcessingContext.ReportContext.ParentPath, base.PublicProcessingContext.UserLanguage.Name);
			return onDemandMetadata;
		}

		protected override void SetupReportLanguage(Merge odpMerge, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			odpMerge.EvaluateReportLanguage(reportInstance, null);
		}

		protected override void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			if (base.ReportDefinition.HasSubReports)
			{
				ReportProcessing.FetchSubReports(base.ReportDefinition, odpContext.ChunkFactory, odpContext.ErrorContext, odpContext.OdpMetadata, odpContext.ReportContext, odpContext.SubReportCallback, 0, odpContext.SnapshotProcessing, odpContext.ProcessWithCachedData, base.GlobalIDOwnerCollection, base.PublicProcessingContext.QueryParameters);
				SubReportInitializer.InitializeSubReportOdpContext(base.ReportDefinition, odpContext);
			}
			odpMerge.FetchData(reportInstance, false);
			reportInstance.CalculateAndStoreReportVariables(odpContext);
			if (base.ReportDefinition.HasSubReports)
			{
				SubReportInitializer.InitializeSubReports(base.ReportDefinition, reportInstance, odpContext, false, false);
			}
			base.SetupInitialOdpState(odpContext, reportInstance, reportSnapshot);
			if (!base.ReportDefinition.HasSubReports)
			{
				if (base.ReportDefinition.DeferVariableEvaluation)
				{
					return;
				}
				if (!base.ReportDefinition.HasVariables)
				{
					return;
				}
			}
			Merge.PreProcessTablixes(base.ReportDefinition, odpContext, true);
		}
	}
}
