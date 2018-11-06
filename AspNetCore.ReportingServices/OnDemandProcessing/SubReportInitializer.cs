using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal class SubReportInitializer
	{
		internal static void InitializeSubReportOdpContext(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, OnDemandProcessingContext parentOdpContext)
		{
			foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport in report.SubReports)
			{
				if (!subReport.ExceededMaxLevel)
				{
					OnDemandProcessingContext parentOdpContext2 = subReport.OdpContext = new OnDemandProcessingContext(parentOdpContext, subReport.ReportContext, subReport);
					if (subReport.RetrievalStatus != AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed && subReport.Report.HasSubReports)
					{
						SubReportInitializer.InitializeSubReportOdpContext(subReport.Report, parentOdpContext2);
					}
				}
			}
		}

		internal static bool InitializeSubReports(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, OnDemandProcessingContext odpContext, bool inDataRegion, bool fromCreateSubReportInstance)
		{
			try
			{
				odpContext.IsTopLevelSubReportProcessing = true;
				bool flag = true;
				OnDemandProcessingContext onDemandProcessingContext = odpContext;
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport in report.SubReports)
				{
					if (subReport.ExceededMaxLevel)
					{
						return flag;
					}
					IReference<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance> reference = null;
					try
					{
						bool flag2 = false;
						if (subReport.RetrievalStatus != AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed)
						{
							onDemandProcessingContext = SubReportInitializer.InitializeSubReport(odpContext, subReport, reportInstance, inDataRegion || subReport.InDataRegion, fromCreateSubReportInstance, out flag2);
							if (!inDataRegion && !subReport.InDataRegion && (!odpContext.SnapshotProcessing || odpContext.ReprocessSnapshot))
							{
								reference = subReport.CurrentSubReportInstance;
							}
						}
						if (flag2 && subReport.Report.HasSubReports)
						{
							flag &= SubReportInitializer.InitializeSubReports(subReport.Report, (subReport.CurrentSubReportInstance != null) ? subReport.CurrentSubReportInstance.Value().ReportInstance.Value() : null, onDemandProcessingContext, inDataRegion || subReport.InDataRegion, fromCreateSubReportInstance);
						}
						if (onDemandProcessingContext.ErrorContext.Messages != null && 0 < onDemandProcessingContext.ErrorContext.Messages.Count)
						{
							OnDemandProcessingContext topLevelContext = odpContext.TopLevelContext;
							topLevelContext.ErrorContext.Register(ProcessingErrorCode.rsWarningExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, null, onDemandProcessingContext.ErrorContext.Messages);
						}
						flag &= flag2;
					}
					catch (Exception e)
					{
						flag = false;
						AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.HandleSubReportProcessingError(onDemandProcessingContext.TopLevelContext.ErrorContext, subReport, InstancePathItem.GenerateInstancePathString(subReport.InstancePath), onDemandProcessingContext.ErrorContext, e);
					}
					finally
					{
						if (reference != null)
						{
							reference.Value().InstanceComplete();
						}
					}
				}
				return flag;
			}
			finally
			{
				odpContext.IsTopLevelSubReportProcessing = false;
			}
		}

		internal static bool InitializeSubReport(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			bool result = false;
			OnDemandProcessingContext onDemandProcessingContext = null;
			try
			{
				onDemandProcessingContext = subReport.OdpContext;
				Merge merge = new Merge(subReport.Report, onDemandProcessingContext);
				result = merge.InitAndSetupSubReport(subReport);
				if (onDemandProcessingContext.ErrorContext.Messages != null)
				{
					if (0 < onDemandProcessingContext.ErrorContext.Messages.Count)
					{
						OnDemandProcessingContext topLevelContext = onDemandProcessingContext.TopLevelContext;
						topLevelContext.ErrorContext.Register(ProcessingErrorCode.rsWarningExecutingSubreport, Severity.Warning, subReport.ObjectType, subReport.Name, null, onDemandProcessingContext.ErrorContext.Messages);
						return result;
					}
					return result;
				}
				return result;
			}
			catch (Exception e)
			{
				AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.HandleSubReportProcessingError(onDemandProcessingContext.TopLevelContext.ErrorContext, subReport, InstancePathItem.GenerateInstancePathString(subReport.InstancePath), onDemandProcessingContext.ErrorContext, e);
				return result;
			}
		}

		private static OnDemandProcessingContext InitializeSubReport(OnDemandProcessingContext parentOdpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, bool inDataRegion, bool fromCreateSubReportInstance, out bool prefetchSuccess)
		{
			Global.Tracer.Assert(null != subReport.OdpContext, "(null != subReport.OdpContext)");
			prefetchSuccess = true;
			if (!inDataRegion)
			{
				IReference<AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance> reference2 = subReport.CurrentSubReportInstance = ((subReport.OdpContext.SnapshotProcessing && !subReport.OdpContext.ReprocessSnapshot) ? reportInstance.SubreportInstances[subReport.IndexInCollection] : AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance.CreateInstance(reportInstance, subReport, parentOdpContext.OdpMetadata));
				if (!fromCreateSubReportInstance)
				{
					ReportSection containingSection = subReport.GetContainingSection(parentOdpContext);
					parentOdpContext.SetupContext(containingSection, null);
				}
				Merge merge = new Merge(subReport.Report, subReport.OdpContext);
				prefetchSuccess = merge.InitAndSetupSubReport(subReport);
			}
			return subReport.OdpContext;
		}
	}
}
