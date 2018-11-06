using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	internal sealed class RSTrace
	{
		internal enum TraceComponents
		{
			Library,
			ConfigManager,
			WebServer,
			NtService,
			Session,
			BufferedResponse,
			RunningRequests,
			DbPolling,
			Notification,
			Provider,
			Schedule,
			Subscription,
			Security,
			ServiceController,
			DbCleanup,
			Cache,
			Chunks,
			ExtensionFactory,
			RunningJobs,
			Processing,
			ReportRendering,
			HtmlViewer,
			DataExtension,
			EmailExtension,
			ImageRenderer,
			ExcelRenderer,
			PreviewServer,
			ResourceUtilities,
			ReportPreview,
			UI,
			Crypto,
			SemanticModelGenerator,
			SemanticQueryEngine,
			AppDomainManager,
			HttpRuntime,
			WcfRuntime,
			AlertingRuntime,
			UndoManager,
			DataManager,
			DataStructureManager,
			Controls,
			PowerView,
			QueryDesign,
			MonitoredScope,
			CloudReportServer,
			ExecutionLog,
			DataShapeQueryTranslation,
			InfoNav,
			ReportServerWebApp,
			Thread
		}

		private class DefaultRSTraceInternal : IRSTraceInternal
		{
			public string TraceDirectory
			{
				get
				{
					return string.Empty;
				}
			}

			public string CurrentTraceFilePath
			{
				get
				{
					return string.Empty;
				}
			}

			public bool BufferOutput
			{
				get
				{
					return false;
				}
				set
				{
				}
			}

			public bool IsTraceInitialized
			{
				get
				{
					return true;
				}
			}

			public void ClearBuffer()
			{
			}

			public void WriteBuffer()
			{
			}

			public string GetDefaultTraceLevel()
			{
				return "0";
			}

			public void Trace(string componentName, string message)
			{
			}

			public void Trace(string componentName, string format, params object[] arg)
			{
			}

			public void Trace(TraceLevel traceLevel, string componentName, string message)
			{
			}

			public void TraceWithDetails(TraceLevel traceLevel, string componentName, string message, string details)
			{
			}

			public void Trace(TraceLevel traceLevel, string componentName, string format, params object[] arg)
			{
			}

			public void TraceException(TraceLevel traceLevel, string componentName, string message)
			{
			}

			public void TraceWithNoEventLog(TraceLevel traceLevel, string componentName, string format, params object[] arg)
			{
			}

			public void Fail(string componentName)
			{
				throw new InvalidOperationException(componentName);
			}

			public void Fail(string componentName, string message)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "({0}): {1}", componentName, message));
			}

			public bool GetTraceLevel(string componentName, out TraceLevel componentTraceLevel)
			{
				componentTraceLevel = TraceLevel.Off;
				return false;
			}
		}

		internal sealed class WriteOnce
		{
			private Hashtable m_traceWriteOnce = new Hashtable();

			private static Hashtable s_traceWriteOnce = new Hashtable();

			public bool TraceWritten(string text)
			{
				if (text != null && !this.m_traceWriteOnce.Contains(text))
				{
					this.m_traceWriteOnce.Add(text, null);
					lock (WriteOnce.s_traceWriteOnce.SyncRoot)
					{
						if (!WriteOnce.s_traceWriteOnce.Contains(text))
						{
							WriteOnce.s_traceWriteOnce.Add(text, null);
							return false;
						}
					}
				}
				return true;
			}
		}

		private const string m_allComponents = "all";

		private static RSTrace m_CryptoTrace;

		private static RSTrace m_ResourceUtilTrace;

		private static RSTrace m_CatalogTrace;

		private static RSTrace m_ConfigManagerTrace;

		private static RSTrace m_WebServerTrace;

		private static RSTrace m_WcfRuntimeTrace;

		private static RSTrace m_AlertingRuntimeTrace;

		private static RSTrace m_NtServiceTrace;

		private static RSTrace m_SessionTrace;

		private static RSTrace m_BufRespTrace;

		private static RSTrace m_RunningRequestsTrace;

		private static RSTrace m_DbPollingTrace;

		private static RSTrace m_NotificationTrace;

		private static RSTrace m_ProviderTrace;

		private static RSTrace m_ScheduleTrace;

		private static RSTrace m_SubscriptionTrace;

		private static RSTrace m_SecurityTrace;

		private static RSTrace m_ServiceControllerTrace;

		private static RSTrace m_CleanupTrace;

		private static RSTrace m_CacheTrace;

		private static RSTrace m_ChunkTrace;

		private static RSTrace m_ExtTrace;

		private static RSTrace m_RunningJobsTrace;

		private static RSTrace m_ProcessingTrace;

		private static RSTrace m_RenderingTrace;

		private static RSTrace m_ViewerTrace;

		private static RSTrace m_DataExtTrace;

		private static RSTrace m_RSWebAppTracer;

		private static RSTrace m_EmailExtensionTrace;

		private static RSTrace m_ImageRendererTrace;

		private static RSTrace m_ExcelRendererTrace;

		private static RSTrace m_PreviewServerTrace;

		private static RSTrace m_ReportPreviewTrace;

		private static RSTrace m_UITrace;

		private static RSTrace m_SMGTrace;

		private static RSTrace m_SQETrace;

		private static RSTrace m_AppDomainManagerTrace;

		private static RSTrace m_HttpRuntimeTrace;

		private static RSTrace m_UndoManagerTrace;

		private static RSTrace m_DataManagerTrace;

		private static RSTrace m_DataStructureManagerTrace;

		private static RSTrace m_QueryDesignTrace;

		private static RSTrace m_ControlsTrace;

		private static RSTrace m_ClientEventTracer;

		private static RSTrace m_ThreadTracer;

		private static RSTrace m_MonitoredScope;

		private static RSTrace m_dsqtTracer;

		private static RSTrace m_infoNavTracer;

		private readonly string m_ComponentName;

		private TraceLevel m_componentTraceLevel;

		private static IRSTraceInternal m_traceInternal = new DefaultRSTraceInternal();

		private static IRSTraceInternalWithDynamicLevel m_alternateTraceInternal;

		private static TraceSwitch m_rsTraceSwitch;

		public static RSTrace CryptoTrace
		{
			get
			{
				RSTrace cryptoTrace = RSTrace.m_CryptoTrace;
				if (cryptoTrace == null)
				{
					cryptoTrace = new RSTrace(TraceComponents.Crypto.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_CryptoTrace, cryptoTrace, (RSTrace)null);
				}
				return RSTrace.m_CryptoTrace;
			}
		}

		public static RSTrace ResourceUtilTrace
		{
			get
			{
				RSTrace resourceUtilTrace = RSTrace.m_ResourceUtilTrace;
				if (resourceUtilTrace == null)
				{
					resourceUtilTrace = new RSTrace(TraceComponents.ResourceUtilities.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ResourceUtilTrace, resourceUtilTrace, (RSTrace)null);
				}
				return RSTrace.m_ResourceUtilTrace;
			}
		}

		public static RSTrace CatalogTrace
		{
			get
			{
				RSTrace catalogTrace = RSTrace.m_CatalogTrace;
				if (catalogTrace == null)
				{
					catalogTrace = new RSTrace(TraceComponents.Library.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_CatalogTrace, catalogTrace, (RSTrace)null);
				}
				return RSTrace.m_CatalogTrace;
			}
		}

		public static RSTrace ConfigManagerTracer
		{
			get
			{
				RSTrace configManagerTrace = RSTrace.m_ConfigManagerTrace;
				if (configManagerTrace == null)
				{
					configManagerTrace = new RSTrace(TraceComponents.ConfigManager.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ConfigManagerTrace, configManagerTrace, (RSTrace)null);
				}
				return RSTrace.m_ConfigManagerTrace;
			}
		}

		public static RSTrace WebServerTracer
		{
			get
			{
				RSTrace webServerTrace = RSTrace.m_WebServerTrace;
				if (webServerTrace == null)
				{
					webServerTrace = new RSTrace(TraceComponents.WebServer.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_WebServerTrace, webServerTrace, (RSTrace)null);
				}
				return RSTrace.m_WebServerTrace;
			}
		}

		public static RSTrace WcfRuntimeTracer
		{
			get
			{
				RSTrace wcfRuntimeTrace = RSTrace.m_WcfRuntimeTrace;
				if (wcfRuntimeTrace == null)
				{
					wcfRuntimeTrace = new RSTrace(TraceComponents.WcfRuntime.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_WcfRuntimeTrace, wcfRuntimeTrace, (RSTrace)null);
				}
				return RSTrace.m_WcfRuntimeTrace;
			}
		}

		public static RSTrace AlertingRuntimeTracer
		{
			get
			{
				RSTrace alertingRuntimeTrace = RSTrace.m_AlertingRuntimeTrace;
				if (alertingRuntimeTrace == null)
				{
					alertingRuntimeTrace = new RSTrace(TraceComponents.AlertingRuntime.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_AlertingRuntimeTrace, alertingRuntimeTrace, (RSTrace)null);
				}
				return RSTrace.m_AlertingRuntimeTrace;
			}
		}

		public static RSTrace NtServiceTracer
		{
			get
			{
				RSTrace ntServiceTrace = RSTrace.m_NtServiceTrace;
				if (ntServiceTrace == null)
				{
					ntServiceTrace = new RSTrace(TraceComponents.NtService.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_NtServiceTrace, ntServiceTrace, (RSTrace)null);
				}
				return RSTrace.m_NtServiceTrace;
			}
		}

		public static RSTrace SessionTrace
		{
			get
			{
				RSTrace sessionTrace = RSTrace.m_SessionTrace;
				if (sessionTrace == null)
				{
					sessionTrace = new RSTrace(TraceComponents.Session.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_SessionTrace, sessionTrace, (RSTrace)null);
				}
				return RSTrace.m_SessionTrace;
			}
		}

		public static RSTrace BufferedResponseTracer
		{
			get
			{
				RSTrace bufRespTrace = RSTrace.m_BufRespTrace;
				if (bufRespTrace == null)
				{
					bufRespTrace = new RSTrace(TraceComponents.BufferedResponse.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_BufRespTrace, bufRespTrace, (RSTrace)null);
				}
				return RSTrace.m_BufRespTrace;
			}
		}

		public static RSTrace RunningRequestsTracer
		{
			get
			{
				RSTrace runningRequestsTrace = RSTrace.m_RunningRequestsTrace;
				if (runningRequestsTrace == null)
				{
					runningRequestsTrace = new RSTrace(TraceComponents.RunningRequests.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_RunningRequestsTrace, runningRequestsTrace, (RSTrace)null);
				}
				return RSTrace.m_RunningRequestsTrace;
			}
		}

		public static RSTrace DbPollingTracer
		{
			get
			{
				RSTrace dbPollingTrace = RSTrace.m_DbPollingTrace;
				if (dbPollingTrace == null)
				{
					dbPollingTrace = new RSTrace(TraceComponents.DbPolling.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_DbPollingTrace, dbPollingTrace, (RSTrace)null);
				}
				return RSTrace.m_DbPollingTrace;
			}
		}

		public static RSTrace NotificationTracer
		{
			get
			{
				RSTrace notificationTrace = RSTrace.m_NotificationTrace;
				if (notificationTrace == null)
				{
					notificationTrace = new RSTrace(TraceComponents.Notification.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_NotificationTrace, notificationTrace, (RSTrace)null);
				}
				return RSTrace.m_NotificationTrace;
			}
		}

		public static RSTrace ProviderTracer
		{
			get
			{
				RSTrace providerTrace = RSTrace.m_ProviderTrace;
				if (providerTrace == null)
				{
					providerTrace = new RSTrace(TraceComponents.Provider.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ProviderTrace, providerTrace, (RSTrace)null);
				}
				return RSTrace.m_ProviderTrace;
			}
		}

		public static RSTrace ScheduleTracer
		{
			get
			{
				RSTrace scheduleTrace = RSTrace.m_ScheduleTrace;
				if (scheduleTrace == null)
				{
					scheduleTrace = new RSTrace(TraceComponents.Schedule.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ScheduleTrace, scheduleTrace, (RSTrace)null);
				}
				return RSTrace.m_ScheduleTrace;
			}
		}

		public static RSTrace SubscriptionTracer
		{
			get
			{
				RSTrace subscriptionTrace = RSTrace.m_SubscriptionTrace;
				if (subscriptionTrace == null)
				{
					subscriptionTrace = new RSTrace(TraceComponents.Subscription.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_SubscriptionTrace, subscriptionTrace, (RSTrace)null);
				}
				return RSTrace.m_SubscriptionTrace;
			}
		}

		public static RSTrace SecurityTracer
		{
			get
			{
				RSTrace securityTrace = RSTrace.m_SecurityTrace;
				if (securityTrace == null)
				{
					securityTrace = new RSTrace(TraceComponents.Security.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_SecurityTrace, securityTrace, (RSTrace)null);
				}
				return RSTrace.m_SecurityTrace;
			}
		}

		public static RSTrace ServiceControllerTracer
		{
			get
			{
				RSTrace serviceControllerTrace = RSTrace.m_ServiceControllerTrace;
				if (serviceControllerTrace == null)
				{
					serviceControllerTrace = new RSTrace(TraceComponents.ServiceController.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ServiceControllerTrace, serviceControllerTrace, (RSTrace)null);
				}
				return RSTrace.m_ServiceControllerTrace;
			}
		}

		public static RSTrace CleanupTracer
		{
			get
			{
				RSTrace cleanupTrace = RSTrace.m_CleanupTrace;
				if (cleanupTrace == null)
				{
					cleanupTrace = new RSTrace(TraceComponents.DbCleanup.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_CleanupTrace, cleanupTrace, (RSTrace)null);
				}
				return RSTrace.m_CleanupTrace;
			}
		}

		public static RSTrace CacheTracer
		{
			get
			{
				RSTrace cacheTrace = RSTrace.m_CacheTrace;
				if (cacheTrace == null)
				{
					cacheTrace = new RSTrace(TraceComponents.Cache.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_CacheTrace, cacheTrace, (RSTrace)null);
				}
				return RSTrace.m_CacheTrace;
			}
		}

		public static RSTrace ChunkTracer
		{
			get
			{
				RSTrace chunkTrace = RSTrace.m_ChunkTrace;
				if (chunkTrace == null)
				{
					chunkTrace = new RSTrace(TraceComponents.Chunks.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ChunkTrace, chunkTrace, (RSTrace)null);
				}
				return RSTrace.m_ChunkTrace;
			}
		}

		public static RSTrace ExtensionFactoryTracer
		{
			get
			{
				RSTrace extTrace = RSTrace.m_ExtTrace;
				if (extTrace == null)
				{
					extTrace = new RSTrace(TraceComponents.ExtensionFactory.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ExtTrace, extTrace, (RSTrace)null);
				}
				return RSTrace.m_ExtTrace;
			}
		}

		public static RSTrace RunningJobsTrace
		{
			get
			{
				RSTrace runningJobsTrace = RSTrace.m_RunningJobsTrace;
				if (runningJobsTrace == null)
				{
					runningJobsTrace = new RSTrace(TraceComponents.RunningJobs.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_RunningJobsTrace, runningJobsTrace, (RSTrace)null);
				}
				return RSTrace.m_RunningJobsTrace;
			}
		}

		public static RSTrace ProcessingTracer
		{
			get
			{
				RSTrace processingTrace = RSTrace.m_ProcessingTrace;
				if (processingTrace == null)
				{
					processingTrace = new RSTrace(TraceComponents.Processing.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ProcessingTrace, processingTrace, (RSTrace)null);
				}
				return RSTrace.m_ProcessingTrace;
			}
		}

		public static RSTrace RenderingTracer
		{
			get
			{
				RSTrace renderingTrace = RSTrace.m_RenderingTrace;
				if (renderingTrace == null)
				{
					renderingTrace = new RSTrace(TraceComponents.ReportRendering.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_RenderingTrace, renderingTrace, (RSTrace)null);
				}
				return RSTrace.m_RenderingTrace;
			}
		}

		public static RSTrace ViewerTracer
		{
			get
			{
				RSTrace viewerTrace = RSTrace.m_ViewerTrace;
				if (viewerTrace == null)
				{
					viewerTrace = new RSTrace(TraceComponents.ConfigManager.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ViewerTrace, viewerTrace, (RSTrace)null);
				}
				return RSTrace.m_ViewerTrace;
			}
		}

		public static RSTrace DataExtensionTracer
		{
			get
			{
				RSTrace dataExtTrace = RSTrace.m_DataExtTrace;
				if (dataExtTrace == null)
				{
					dataExtTrace = new RSTrace(TraceComponents.DataExtension.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_DataExtTrace, dataExtTrace, (RSTrace)null);
				}
				return RSTrace.m_DataExtTrace;
			}
		}

		public static RSTrace RSWebAppTracer
		{
			get
			{
				RSTrace rSWebAppTracer = RSTrace.m_RSWebAppTracer;
				if (rSWebAppTracer == null)
				{
					rSWebAppTracer = new RSTrace(TraceComponents.ReportServerWebApp.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_RSWebAppTracer, rSWebAppTracer, (RSTrace)null);
				}
				return RSTrace.m_RSWebAppTracer;
			}
		}

		public static RSTrace EmailExtensionTracer
		{
			get
			{
				RSTrace emailExtensionTrace = RSTrace.m_EmailExtensionTrace;
				if (emailExtensionTrace == null)
				{
					emailExtensionTrace = new RSTrace(TraceComponents.EmailExtension.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_EmailExtensionTrace, emailExtensionTrace, (RSTrace)null);
				}
				return RSTrace.m_EmailExtensionTrace;
			}
		}

		public static RSTrace ImageRendererTracer
		{
			get
			{
				RSTrace imageRendererTrace = RSTrace.m_ImageRendererTrace;
				if (imageRendererTrace == null)
				{
					imageRendererTrace = new RSTrace(TraceComponents.ImageRenderer.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ImageRendererTrace, imageRendererTrace, (RSTrace)null);
				}
				return RSTrace.m_ImageRendererTrace;
			}
		}

		public static RSTrace ExcelRendererTracer
		{
			get
			{
				RSTrace excelRendererTrace = RSTrace.m_ExcelRendererTrace;
				if (excelRendererTrace == null)
				{
					excelRendererTrace = new RSTrace(TraceComponents.ExcelRenderer.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ExcelRendererTrace, excelRendererTrace, (RSTrace)null);
				}
				return RSTrace.m_ExcelRendererTrace;
			}
		}

		public static RSTrace PreviewServerTracer
		{
			get
			{
				RSTrace previewServerTrace = RSTrace.m_PreviewServerTrace;
				if (previewServerTrace == null)
				{
					previewServerTrace = new RSTrace(TraceComponents.PreviewServer.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_PreviewServerTrace, previewServerTrace, (RSTrace)null);
				}
				return RSTrace.m_PreviewServerTrace;
			}
		}

		public static RSTrace ReportPreviewTracer
		{
			get
			{
				RSTrace reportPreviewTrace = RSTrace.m_ReportPreviewTrace;
				if (reportPreviewTrace == null)
				{
					reportPreviewTrace = new RSTrace(TraceComponents.ReportPreview.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ReportPreviewTrace, reportPreviewTrace, (RSTrace)null);
				}
				return RSTrace.m_ReportPreviewTrace;
			}
		}

		public static RSTrace UITracer
		{
			get
			{
				RSTrace uITrace = RSTrace.m_UITrace;
				if (uITrace == null)
				{
					uITrace = new RSTrace(TraceComponents.UI.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_UITrace, uITrace, (RSTrace)null);
				}
				return RSTrace.m_UITrace;
			}
		}

		public static RSTrace SMGTracer
		{
			get
			{
				RSTrace sMGTrace = RSTrace.m_SMGTrace;
				if (sMGTrace == null)
				{
					sMGTrace = new RSTrace(TraceComponents.SemanticModelGenerator.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_SMGTrace, sMGTrace, (RSTrace)null);
				}
				return RSTrace.m_SMGTrace;
			}
		}

		public static RSTrace SQETracer
		{
			get
			{
				RSTrace sQETrace = RSTrace.m_SQETrace;
				if (sQETrace == null)
				{
					sQETrace = new RSTrace(TraceComponents.SemanticQueryEngine.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_SQETrace, sQETrace, (RSTrace)null);
				}
				return RSTrace.m_SQETrace;
			}
		}

		public static RSTrace AppDomainManagerTracer
		{
			get
			{
				RSTrace appDomainManagerTrace = RSTrace.m_AppDomainManagerTrace;
				if (appDomainManagerTrace == null)
				{
					appDomainManagerTrace = new RSTrace(TraceComponents.AppDomainManager.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_AppDomainManagerTrace, appDomainManagerTrace, (RSTrace)null);
				}
				return RSTrace.m_AppDomainManagerTrace;
			}
		}

		public static RSTrace HttpRuntimeTracer
		{
			get
			{
				RSTrace httpRuntimeTrace = RSTrace.m_HttpRuntimeTrace;
				if (httpRuntimeTrace == null)
				{
					httpRuntimeTrace = new RSTrace(TraceComponents.HttpRuntime.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_HttpRuntimeTrace, httpRuntimeTrace, (RSTrace)null);
				}
				return RSTrace.m_HttpRuntimeTrace;
			}
		}

		public static RSTrace UndoManager
		{
			get
			{
				RSTrace undoManagerTrace = RSTrace.m_UndoManagerTrace;
				if (undoManagerTrace == null)
				{
					undoManagerTrace = new RSTrace(TraceComponents.UndoManager.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_UndoManagerTrace, undoManagerTrace, (RSTrace)null);
				}
				return RSTrace.m_UndoManagerTrace;
			}
		}

		public static RSTrace DataManager
		{
			get
			{
				RSTrace dataManagerTrace = RSTrace.m_DataManagerTrace;
				if (dataManagerTrace == null)
				{
					dataManagerTrace = new RSTrace(TraceComponents.DataManager.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_DataManagerTrace, dataManagerTrace, (RSTrace)null);
				}
				return RSTrace.m_DataManagerTrace;
			}
		}

		public static RSTrace DataStructureManager
		{
			get
			{
				RSTrace dataStructureManagerTrace = RSTrace.m_DataStructureManagerTrace;
				if (dataStructureManagerTrace == null)
				{
					dataStructureManagerTrace = new RSTrace(TraceComponents.DataStructureManager.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_DataStructureManagerTrace, dataStructureManagerTrace, (RSTrace)null);
				}
				return RSTrace.m_DataStructureManagerTrace;
			}
		}

		public static RSTrace QueryDesign
		{
			get
			{
				RSTrace queryDesignTrace = RSTrace.m_QueryDesignTrace;
				if (queryDesignTrace == null)
				{
					queryDesignTrace = new RSTrace(TraceComponents.QueryDesign.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_QueryDesignTrace, queryDesignTrace, (RSTrace)null);
				}
				return RSTrace.m_QueryDesignTrace;
			}
		}

		public static RSTrace Controls
		{
			get
			{
				RSTrace controlsTrace = RSTrace.m_ControlsTrace;
				if (controlsTrace == null)
				{
					controlsTrace = new RSTrace(TraceComponents.Controls.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ControlsTrace, controlsTrace, (RSTrace)null);
				}
				return RSTrace.m_ControlsTrace;
			}
		}

		public static RSTrace ClientEventTracer
		{
			get
			{
				RSTrace clientEventTracer = RSTrace.m_ClientEventTracer;
				if (clientEventTracer == null)
				{
					clientEventTracer = new RSTrace(TraceComponents.PowerView.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ClientEventTracer, clientEventTracer, (RSTrace)null);
				}
				return RSTrace.m_ClientEventTracer;
			}
		}

		public static RSTrace ThreadTracer
		{
			get
			{
				RSTrace threadTracer = RSTrace.m_ThreadTracer;
				if (threadTracer == null)
				{
					threadTracer = new RSTrace(TraceComponents.Thread.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_ThreadTracer, threadTracer, (RSTrace)null);
				}
				return RSTrace.m_ThreadTracer;
			}
		}

		public static RSTrace MonitoredScope
		{
			get
			{
				RSTrace monitoredScope = RSTrace.m_MonitoredScope;
				if (monitoredScope == null)
				{
					monitoredScope = new RSTrace(TraceComponents.MonitoredScope.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_MonitoredScope, monitoredScope, (RSTrace)null);
				}
				return RSTrace.m_MonitoredScope;
			}
		}

		public static RSTrace DsqtTracer
		{
			get
			{
				RSTrace dsqtTracer = RSTrace.m_dsqtTracer;
				if (dsqtTracer == null)
				{
					dsqtTracer = new RSTrace(TraceComponents.DataShapeQueryTranslation.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_dsqtTracer, dsqtTracer, (RSTrace)null);
				}
				return RSTrace.m_dsqtTracer;
			}
		}

		public static RSTrace InfoNavTracer
		{
			get
			{
				RSTrace infoNavTracer = RSTrace.m_infoNavTracer;
				if (infoNavTracer == null)
				{
					infoNavTracer = new RSTrace(TraceComponents.InfoNav.ToString());
					Interlocked.CompareExchange<RSTrace>(ref RSTrace.m_infoNavTracer, infoNavTracer, (RSTrace)null);
				}
				return RSTrace.m_infoNavTracer;
			}
		}

		public bool TraceInfo
		{
			get
			{
				return this.IsTraceLevelEnabled(TraceLevel.Info);
			}
		}

		public bool TraceError
		{
			get
			{
				return this.IsTraceLevelEnabled(TraceLevel.Error);
			}
		}

		public bool TraceWarning
		{
			get
			{
				return this.IsTraceLevelEnabled(TraceLevel.Warning);
			}
		}

		public bool TraceVerbose
		{
			get
			{
				return this.IsTraceLevelEnabled(TraceLevel.Verbose);
			}
		}

		internal static bool IsTraceInitialized
		{
			get
			{
				return RSTrace.m_traceInternal.IsTraceInitialized;
			}
		}

		internal string TraceFileName
		{
			get
			{
				return RSTrace.m_traceInternal.CurrentTraceFilePath;
			}
		}

		public string TraceDirectory
		{
			get
			{
				return RSTrace.m_traceInternal.TraceDirectory;
			}
		}

		public bool BufferOutput
		{
			get
			{
				return RSTrace.m_traceInternal.BufferOutput;
			}
			set
			{
				RSTrace.m_traceInternal.BufferOutput = value;
			}
		}

		public TraceSwitch RSTraceSwitch
		{
			get
			{
				if (RSTrace.m_rsTraceSwitch == null)
				{
					lock (this)
					{
						string defaultTraceLevel = RSTrace.m_traceInternal.GetDefaultTraceLevel();
						RSTrace.m_rsTraceSwitch = new TraceSwitch("DefaultTraceSwitch", "Default Trace Switch", defaultTraceLevel);
					}
				}
				return RSTrace.m_rsTraceSwitch;
			}
			set
			{
				lock (this)
				{
					RSTrace.m_rsTraceSwitch = value;
				}
			}
		}

		internal RSTrace(string componentName)
		{
			this.m_ComponentName = componentName.ToLowerInvariant();
			this.m_componentTraceLevel = RSTrace.GetTraceLevel(this.m_ComponentName);
		}

		public void Trace(string message)
		{
			RSTrace.m_traceInternal.Trace(this.m_ComponentName, message);
			if (RSTrace.m_alternateTraceInternal != null)
			{
				RSTrace.m_alternateTraceInternal.Trace(this.m_ComponentName, message);
			}
		}

		public void Trace(TraceLevel traceLevel, string message)
		{
			RSTrace.m_traceInternal.Trace(traceLevel, this.m_ComponentName, message);
			if (RSTrace.m_alternateTraceInternal != null)
			{
				RSTrace.m_alternateTraceInternal.Trace(traceLevel, this.m_ComponentName, message);
			}
		}

		public void TraceWithDetails(TraceLevel traceLevel, string message, string details)
		{
			RSTrace.m_traceInternal.TraceWithDetails(traceLevel, this.m_ComponentName, message, details);
			if (RSTrace.m_alternateTraceInternal != null)
			{
				RSTrace.m_alternateTraceInternal.TraceWithDetails(traceLevel, this.m_ComponentName, message, details);
			}
		}

		public void TraceException(TraceLevel traceLevel, string message)
		{
			RSTrace.m_traceInternal.TraceException(traceLevel, this.m_ComponentName, message);
			if (RSTrace.m_alternateTraceInternal != null)
			{
				RSTrace.m_alternateTraceInternal.TraceException(traceLevel, this.m_ComponentName, message);
			}
		}

		public void Trace(string format, params object[] arg)
		{
			RSTrace.m_traceInternal.Trace(this.m_ComponentName, format, arg);
			if (RSTrace.m_alternateTraceInternal != null)
			{
				RSTrace.m_alternateTraceInternal.Trace(this.m_ComponentName, format, arg);
			}
		}

		public void Trace(TraceLevel traceLevel, string format, params object[] arg)
		{
			RSTrace.m_traceInternal.Trace(traceLevel, this.m_ComponentName, format, arg);
			if (RSTrace.m_alternateTraceInternal != null)
			{
				RSTrace.m_alternateTraceInternal.Trace(traceLevel, this.m_ComponentName, format, arg);
			}
		}

		public void TraceException(TraceLevel traceLevel, string format, params object[] arg)
		{
			if (this.IsTraceLevelEnabled(traceLevel))
			{
				string message = string.Format(CultureInfo.InvariantCulture, format, arg);
				RSTrace.m_traceInternal.TraceException(traceLevel, this.m_ComponentName, message);
				if (RSTrace.m_alternateTraceInternal != null)
				{
					RSTrace.m_alternateTraceInternal.TraceException(traceLevel, this.m_ComponentName, message);
				}
			}
		}

		public void TraceWithNoEventLog(TraceLevel traceLevel, string format, params object[] arg)
		{
			RSTrace.m_traceInternal.TraceWithNoEventLog(traceLevel, this.m_ComponentName, format, arg);
			if (RSTrace.m_alternateTraceInternal != null)
			{
				RSTrace.m_alternateTraceInternal.TraceWithNoEventLog(traceLevel, this.m_ComponentName, format, arg);
			}
		}

		public void Assert(bool condition)
		{
			if (!condition)
			{
				if (RSTrace.m_alternateTraceInternal != null)
				{
					try
					{
						RSTrace.m_alternateTraceInternal.Fail(this.m_ComponentName);
					}
					catch
					{
						RSTrace.m_traceInternal.Fail(this.m_ComponentName);
					}
				}
				RSTrace.m_traceInternal.Fail(this.m_ComponentName);
			}
		}

		public void Assert(bool condition, string message)
		{
			if (!condition)
			{
				if (RSTrace.m_alternateTraceInternal != null)
				{
					try
					{
						RSTrace.m_alternateTraceInternal.Fail(this.m_ComponentName, message);
					}
					catch
					{
						RSTrace.m_traceInternal.Fail(this.m_ComponentName, message);
					}
				}
				RSTrace.m_traceInternal.Fail(this.m_ComponentName, message);
			}
		}

		public void Assert(bool condition, string message, params object[] args)
		{
			if (!condition)
			{
				this.Assert(condition, string.Format(CultureInfo.InvariantCulture, message, args));
			}
		}

		[Conditional("DEBUG")]
		public void DebugAssert(bool condition)
		{
			this.Assert(condition);
		}

		[Conditional("DEBUG")]
		public void DebugAssert(string message)
		{
			this.Assert(false, message);
		}

		[Conditional("DEBUG")]
		public void DebugAssert(bool condition, string message)
		{
			this.Assert(condition, message);
		}

		[Conditional("DEBUG")]
		public void DebugAssert(bool condition, string message, object arg1)
		{
			this.Assert(condition, message, arg1);
		}

		private static TraceLevel GetTraceLevel(string componentName)
		{
			TraceLevel result = TraceLevel.Error;
			int num = default(int);
			if (RSTrace.m_traceInternal != null && !RSTrace.m_traceInternal.GetTraceLevel(componentName, out result) && !RSTrace.m_traceInternal.GetTraceLevel("all", out result) && int.TryParse(RSTrace.m_traceInternal.GetDefaultTraceLevel(), NumberStyles.None, (IFormatProvider)CultureInfo.InvariantCulture, out num) && num >= 0 && num <= 3)
			{
				result = (TraceLevel)num;
			}
			return result;
		}

		public bool IsTraceLevelEnabled(TraceLevel level)
		{
			TraceLevel componentTraceLevel = default(TraceLevel);
			if (RSTrace.m_traceInternal is IRSTraceInternalWithDynamicLevel && RSTrace.m_traceInternal.GetTraceLevel(this.m_ComponentName, out componentTraceLevel))
			{
				this.m_componentTraceLevel = componentTraceLevel;
			}
			if (level <= this.m_componentTraceLevel)
			{
				return true;
			}
			TraceLevel traceLevel = default(TraceLevel);
			if (RSTrace.m_alternateTraceInternal != null && ((IRSTraceInternal)RSTrace.m_alternateTraceInternal).GetTraceLevel(this.m_ComponentName, out traceLevel))
			{
				return level <= traceLevel;
			}
			return false;
		}

		public void ClearBuffer()
		{
			RSTrace.m_traceInternal.ClearBuffer();
		}

		public void WriteBuffer()
		{
			RSTrace.m_traceInternal.WriteBuffer();
		}

		public static void SetTrace(IRSTraceInternal trace)
		{
			if (trace == null)
			{
				throw new ArgumentNullException("trace");
			}
			RSTrace.m_traceInternal = trace;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static IRSTraceInternal GetTrace()
		{
			return RSTrace.m_traceInternal;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static IRSTraceInternal GetAlternateTrace()
		{
			return RSTrace.m_alternateTraceInternal;
		}

		public static void SetAlternateTrace(IRSTraceInternalWithDynamicLevel alternateTrace)
		{
			RSTrace.m_alternateTraceInternal = alternateTrace;
		}
	}
}
