using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal abstract class RenderReport
	{
		private readonly ProcessingContext m_publicProcessingContext;

		private readonly RenderingContext m_publicRenderingContext;

		protected abstract ProcessingEngine RunningProcessingEngine
		{
			get;
		}

		protected virtual bool IsSnapshotReprocessing
		{
			get
			{
				return false;
			}
		}

		internal ProcessingContext PublicProcessingContext
		{
			get
			{
				return this.m_publicProcessingContext;
			}
		}

		internal RenderingContext PublicRenderingContext
		{
			get
			{
				return this.m_publicRenderingContext;
			}
		}

		protected string ReportName
		{
			get
			{
				return this.PublicProcessingContext.ReportContext.ItemName;
			}
		}

		protected NameValueCollection RenderingParameters
		{
			get
			{
				return this.PublicRenderingContext.ReportContext.RSRequestParameters.RenderingParameters;
			}
		}

		public RenderReport(ProcessingContext pc, RenderingContext rc)
		{
			this.m_publicProcessingContext = pc;
			this.m_publicRenderingContext = rc;
		}

		public OnDemandProcessingResult Execute(IRenderingExtension newRenderer)
		{
			ExecutionLogContext executionLogContext = new ExecutionLogContext(this.m_publicProcessingContext.JobContext);
			executionLogContext.StartProcessingTimer();
			ProcessingErrorContext processingErrorContext = new ProcessingErrorContext();
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				bool eventInfoChanged = false;
				bool renderingInfoChanged = false;
				UserProfileState userProfileState = UserProfileState.None;
				Hashtable renderProperties = this.PublicRenderingContext.GetRenderProperties(this.IsSnapshotReprocessing);
				NameValueCollection reportServerParameters = this.FormServerParameterCollection(this.PublicRenderingContext.ReportContext.RSRequestParameters.CatalogParameters);
				this.PrepareForExecution();
				this.ProcessReport(processingErrorContext, executionLogContext, ref userProfileState);
				AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext = null;
				try
				{
					AspNetCore.ReportingServices.OnDemandReportRendering.Report report = this.PrepareROM(out odpRenderingContext);
					executionLogContext.StartRenderingTimer();
					renderingInfoChanged = this.InvokeRenderer(newRenderer, report, reportServerParameters, this.RenderingParameters, this.PublicRenderingContext.ReportContext.RSRequestParameters.BrowserCapabilities, ref renderProperties, this.PublicProcessingContext.CreateStreamCallback);
					this.UpdateServerTotalPages(newRenderer, ref renderProperties);
					this.UpdateEventInfo(odpRenderingContext, this.PublicRenderingContext, ref eventInfoChanged);
				}
				catch (ReportProcessing.DataCacheUnavailableException)
				{
					throw;
				}
				catch (ReportRenderingException rex)
				{
					ReportProcessing.HandleRenderingException(rex);
				}
				catch (RSException ex)
				{
					throw;
				}
				catch (Exception ex3)
				{
					if (AsynchronousExceptionDetection.IsStoppingException(ex3))
					{
						throw;
					}
					throw new UnhandledReportRenderingException(ex3);
				}
				finally
				{
					this.FinallyBlockForProcessingAndRendering(odpRenderingContext, executionLogContext);
				}
				this.CleanupSuccessfulProcessing(processingErrorContext);
				return this.ConstructProcessingResult(eventInfoChanged, renderProperties, processingErrorContext, userProfileState, renderingInfoChanged, executionLogContext);
			}
			catch (ReportProcessing.DataCacheUnavailableException ex)
			{
				throw;
			}
			catch (RSException ex)
			{
				this.CleanupForException();
				throw;
			}
			catch (Exception innerException)
			{
				this.CleanupForException();
				throw new ReportProcessingException(innerException, processingErrorContext.Messages);
			}
			finally
			{
				this.FinalCleanup();
				ReportProcessing.UpdateHostingEnvironment(processingErrorContext, this.PublicProcessingContext.ReportContext, executionLogContext, this.RunningProcessingEngine, this.PublicProcessingContext.JobContext);
				if (currentCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
				}
			}
		}

		protected virtual void CleanupSuccessfulProcessing(ProcessingErrorContext errorContext)
		{
		}

		protected virtual bool InvokeRenderer(IRenderingExtension renderer, AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			return renderer.Render(report, reportServerParameters, deviceInfo, clientCapabilities, ref renderProperties, createAndRegisterStream);
		}

		protected abstract void ProcessReport(ProcessingErrorContext errorContext, ExecutionLogContext executionLogContext, ref UserProfileState userProfileState);

		protected abstract void PrepareForExecution();

		protected abstract AspNetCore.ReportingServices.OnDemandReportRendering.Report PrepareROM(out AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext);

		protected abstract OnDemandProcessingResult ConstructProcessingResult(bool eventInfoChanged, Hashtable renderProperties, ProcessingErrorContext errorContext, UserProfileState userProfileState, bool renderingInfoChanged, ExecutionLogContext executionLogContext);

		protected abstract void FinalCleanup();

		protected virtual void CleanupForException()
		{
		}

		private void UpdateEventInfo(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, RenderingContext rc, ref bool eventInfoChanged)
		{
			this.UpdateEventInfoInSnapshot();
			eventInfoChanged |= odpRenderingContext.EventInfoChanged;
			if (eventInfoChanged)
			{
				rc.EventInfo = odpRenderingContext.EventInfo;
			}
		}

		protected virtual void UpdateEventInfoInSnapshot()
		{
		}

		private void FinallyBlockForProcessingAndRendering(AspNetCore.ReportingServices.OnDemandReportRendering.RenderingContext odpRenderingContext, ExecutionLogContext executionLogContext)
		{
			if (odpRenderingContext != null)
			{
				odpRenderingContext.CloseRenderingChunkManager();
			}
			executionLogContext.StopRenderingTimer();
		}

		protected int GetNumberOfPages(Hashtable renderProperties)
		{
			int result = 0;
			if (renderProperties != null)
			{
				object obj = renderProperties["TotalPages"];
				if (obj != null && obj is int)
				{
					result = (int)obj;
				}
			}
			return result;
		}

		protected PaginationMode GetUpdatedPaginationMode(Hashtable renderProperties, PaginationMode clientPaginationMode)
		{
			try
			{
				if (renderProperties != null)
				{
					object obj = renderProperties["UpdatedPaginationMode"];
					if (obj != null)
					{
						return (PaginationMode)obj;
					}
				}
			}
			catch (InvalidCastException)
			{
			}
			return PaginationMode.Estimate;
		}

		protected void UpdateServerTotalPages(IRenderingExtension renderer, ref Hashtable renderProperties)
		{
			if (renderProperties != null && renderer != null && !(renderer is ITotalPages))
			{
				renderProperties["TotalPages"] = 0;
				renderProperties["UpdatedPaginationMode"] = PaginationMode.Estimate;
			}
		}

		private NameValueCollection FormServerParameterCollection(NameValueCollection serverParams)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			if (serverParams == null)
			{
				return nameValueCollection;
			}
			this.CheckAndAddServerParam(serverParams, "Command", nameValueCollection);
			this.CheckAndAddServerParam(serverParams, "Format", nameValueCollection);
			this.CheckAndAddServerParam(serverParams, "SessionID", nameValueCollection);
			this.CheckAndAddServerParam(serverParams, "ShowHideToggle", nameValueCollection);
			this.CheckAndAddServerParam(serverParams, "ImageID", nameValueCollection);
			this.CheckAndAddServerParam(serverParams, "Snapshot", nameValueCollection);
			return nameValueCollection;
		}

		private void CheckAndAddServerParam(NameValueCollection src, string paramName, NameValueCollection dest)
		{
			string[] values = src.GetValues(paramName);
			if (values != null && values.Length > 0)
			{
				dest.Add(paramName, values[0]);
			}
		}

		protected void ValidateReportParameters()
		{
			bool flag = true;
			if (this.PublicProcessingContext.Parameters.ValuesAreValid(out flag, true))
			{
				return;
			}
			throw new ReportProcessingException(ErrorCode.rsParametersNotSpecified);
		}
	}
}
