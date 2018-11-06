using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpInitial : RenderReportOdp
	{
		private readonly DateTime m_executionTimeStamp;

		protected DateTime ExecutionTimeStamp
		{
			get
			{
				return this.m_executionTimeStamp;
			}
		}

		public RenderReportOdpInitial(ProcessingContext pc, RenderingContext rc, DateTime executionTimeStamp, IConfiguration configuration)
			: base(pc, rc, configuration)
		{
			this.m_executionTimeStamp = executionTimeStamp;
		}

		protected override void PrepareForExecution()
		{
			base.ValidateReportParameters();
			ReportProcessing.CheckReportCredentialsAndConnectionUserDependency(base.PublicProcessingContext);
		}

		protected override void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState)
		{
			GlobalIDOwnerCollection globalIDOwnerCollection = default(GlobalIDOwnerCollection);
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report reportDefinition = this.GetReportDefinition(out globalIDOwnerCollection);
			ProcessReportOdpInitial processReportOdpInitial = new ProcessReportOdpInitial(base.Configuration, base.PublicProcessingContext, reportDefinition, errorContext, base.PublicRenderingContext.StoreServerParametersCallback, globalIDOwnerCollection, executionLogContext, this.m_executionTimeStamp);
			base.m_odpReportSnapshot = ((ProcessReportOdp)processReportOdpInitial).Execute(out base.m_odpContext);
		}

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.Report GetReportDefinition(out GlobalIDOwnerCollection globalIDOwnerCollection)
		{
			globalIDOwnerCollection = new GlobalIDOwnerCollection();
			return ReportProcessing.DeserializeKatmaiReport(base.PublicProcessingContext.ChunkFactory, false, globalIDOwnerCollection);
		}
	}
}
