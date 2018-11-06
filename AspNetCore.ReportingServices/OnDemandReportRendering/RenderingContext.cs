using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class RenderingContext
	{
		private bool m_isSubReportContext;

		private bool m_subReportProcessedWithError;

		private bool m_instanceAccessDisallowed;

		private bool m_subReportHasNoInstance;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot m_reportSnapshot;

		private AspNetCore.ReportingServices.ReportProcessing.ReportSnapshot m_oldReportSnapshot;

		private EventInformation m_eventInfo;

		private OnDemandProcessingContext m_odpContext;

		private List<IDynamicInstance> m_dynamicInstances;

		private PageEvaluation m_pageEvaluation;

		private bool m_nativeAllCRITypes;

		private Hashtable m_nativeCRITypes;

		private RenderingChunkManager m_chunkManager;

		private string m_rendererID;

		internal bool IsSubReportContext
		{
			get
			{
				return this.m_isSubReportContext;
			}
		}

		internal bool SubReportProcessedWithError
		{
			get
			{
				return this.m_subReportProcessedWithError;
			}
			set
			{
				this.m_subReportProcessedWithError = value;
			}
		}

		internal bool SubReportHasNoInstance
		{
			get
			{
				return this.m_subReportHasNoInstance;
			}
			set
			{
				this.m_subReportHasNoInstance = value;
			}
		}

		internal bool SubReportHasErrorOrNoInstance
		{
			get
			{
				if (!this.m_subReportProcessedWithError)
				{
					return this.m_subReportHasNoInstance;
				}
				return true;
			}
		}

		internal bool InstanceAccessDisallowed
		{
			get
			{
				if (!this.m_instanceAccessDisallowed)
				{
					if (this.IsSubReportContext)
					{
						return this.SubReportHasErrorOrNoInstance;
					}
					return false;
				}
				return true;
			}
			set
			{
				this.m_instanceAccessDisallowed = value;
			}
		}

		internal OnDemandProcessingContext OdpContext
		{
			get
			{
				return this.m_odpContext;
			}
		}

		internal IErrorContext ErrorContext
		{
			get
			{
				if (this.m_odpContext != null)
				{
					return this.m_odpContext.ReportRuntime;
				}
				return null;
			}
		}

		internal EventInformation EventInfo
		{
			get
			{
				return this.m_eventInfo;
			}
		}

		internal bool EventInfoChanged
		{
			get
			{
				return this.m_eventInfo.Changed;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot ReportSnapshot
		{
			get
			{
				return this.m_reportSnapshot;
			}
		}

		internal bool NativeAllCRITypes
		{
			get
			{
				return this.m_nativeAllCRITypes;
			}
			set
			{
				this.m_nativeAllCRITypes = value;
			}
		}

		internal Hashtable NativeCRITypes
		{
			get
			{
				return this.m_nativeCRITypes;
			}
			set
			{
				this.m_nativeCRITypes = value;
			}
		}

		internal RenderingContext(string rendererID, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot, EventInformation eventInfo, OnDemandProcessingContext processingContext)
		{
			this.m_rendererID = rendererID;
			this.m_isSubReportContext = false;
			this.m_reportSnapshot = reportSnapshot;
			this.InitEventInfo(eventInfo);
			this.m_odpContext = processingContext;
			if (processingContext.ChunkFactory != null)
			{
				this.m_chunkManager = new RenderingChunkManager(rendererID, processingContext.ChunkFactory);
			}
		}

		internal RenderingContext(string rendererID, AspNetCore.ReportingServices.ReportProcessing.ReportSnapshot reportSnapshot, IChunkFactory chunkFactory, EventInformation eventInfo)
		{
			this.m_rendererID = rendererID;
			this.m_isSubReportContext = false;
			this.m_oldReportSnapshot = reportSnapshot;
			this.InitEventInfo(eventInfo);
			if (chunkFactory != null)
			{
				this.m_chunkManager = new RenderingChunkManager(rendererID, chunkFactory);
			}
		}

		internal RenderingContext(RenderingContext parentContext)
			: this(parentContext, false)
		{
		}

		internal RenderingContext(RenderingContext parentContext, bool hasReportItemReferences)
		{
			this.m_rendererID = parentContext.m_rendererID;
			this.m_isSubReportContext = true;
			this.m_pageEvaluation = null;
			this.m_dynamicInstances = null;
			this.m_eventInfo = parentContext.EventInfo;
			this.m_reportSnapshot = parentContext.m_reportSnapshot;
			this.m_oldReportSnapshot = parentContext.m_oldReportSnapshot;
			this.m_chunkManager = parentContext.m_chunkManager;
			if (this.m_oldReportSnapshot != null)
			{
				this.m_odpContext = parentContext.OdpContext;
			}
			else
			{
				this.m_odpContext = new OnDemandProcessingContext(parentContext.m_odpContext, hasReportItemReferences, this.m_reportSnapshot.Report);
			}
		}

		private void InitEventInfo(EventInformation eventInfo)
		{
			if (eventInfo == null)
			{
				this.m_eventInfo = new EventInformation();
			}
			else
			{
				this.m_eventInfo = eventInfo;
			}
			this.m_eventInfo.Changed = false;
		}

		internal RenderingContext(RenderingContext parentContext, OnDemandProcessingContext onDemandProcessingContext)
		{
			this.m_rendererID = parentContext.m_rendererID;
			this.m_isSubReportContext = true;
			this.m_pageEvaluation = null;
			this.m_dynamicInstances = null;
			this.m_oldReportSnapshot = parentContext.m_oldReportSnapshot;
			this.m_eventInfo = parentContext.EventInfo;
			this.m_reportSnapshot = parentContext.m_reportSnapshot;
			this.m_chunkManager = parentContext.m_chunkManager;
			this.m_odpContext = onDemandProcessingContext;
		}

		internal void AddDynamicInstance(IDynamicInstance instance)
		{
			if (this.m_dynamicInstances == null)
			{
				this.m_dynamicInstances = new List<IDynamicInstance>();
			}
			this.m_dynamicInstances.Add(instance);
		}

		internal void ResetContext()
		{
			if (this.m_dynamicInstances != null)
			{
				for (int i = 0; i < this.m_dynamicInstances.Count; i++)
				{
					this.m_dynamicInstances[i].ResetContext();
				}
			}
		}

		internal void SetPageEvaluation(PageEvaluation pageEvaluation)
		{
			this.m_pageEvaluation = pageEvaluation;
		}

		internal void AddToCurrentPage(string textboxName, object textboxValue)
		{
			if (this.m_pageEvaluation != null)
			{
				this.m_pageEvaluation.Add(textboxName, textboxValue);
			}
		}

		internal void AddDrillthroughAction(string drillthroughId, string reportName, DrillthroughParameters reportParameters)
		{
			if (this.m_rendererID != null)
			{
				this.CheckResetEventInfo();
				EventInformation.RendererEventInformation rendererEventInformation = this.m_eventInfo.GetRendererEventInformation(this.m_rendererID);
				if (rendererEventInformation.DrillthroughInfo == null)
				{
					rendererEventInformation.DrillthroughInfo = new Hashtable();
				}
				if (!rendererEventInformation.DrillthroughInfo.ContainsKey(drillthroughId))
				{
					rendererEventInformation.DrillthroughInfo.Add(drillthroughId, new DrillthroughInfo(reportName, reportParameters));
					this.m_eventInfo.Changed = true;
				}
			}
		}

		private void CheckResetEventInfo()
		{
			if (!this.m_eventInfo.Changed)
			{
				EventInformation.RendererEventInformation rendererEventInformation = this.m_eventInfo.GetRendererEventInformation(this.m_rendererID);
				rendererEventInformation.Reset();
				this.m_eventInfo.Changed = true;
			}
		}

		internal void AddValidToggleSender(string senderUniqueName)
		{
			this.CheckResetEventInfo();
			EventInformation.RendererEventInformation rendererEventInformation = this.m_eventInfo.GetRendererEventInformation(this.m_rendererID);
			if (rendererEventInformation.ValidToggleSenders == null)
			{
				rendererEventInformation.ValidToggleSenders = new Hashtable();
			}
			if (!rendererEventInformation.ValidToggleSenders.ContainsKey(senderUniqueName))
			{
				rendererEventInformation.ValidToggleSenders.Add(senderUniqueName, null);
				this.m_eventInfo.Changed = true;
			}
		}

		internal bool IsSenderToggled(string uniqueName)
		{
			EventInformation eventInfo = this.EventInfo;
			if (eventInfo != null)
			{
				Hashtable toggleStateInfo = eventInfo.ToggleStateInfo;
				if (toggleStateInfo != null)
				{
					return toggleStateInfo.ContainsKey(uniqueName);
				}
				return false;
			}
			return false;
		}

		internal SortOptions GetSortState(string eventSourceUniqueName)
		{
			if (this.m_eventInfo != null && this.m_eventInfo.OdpSortInfo != null)
			{
				return this.m_eventInfo.OdpSortInfo.GetSortState(eventSourceUniqueName);
			}
			return SortOptions.None;
		}

		internal string GenerateShimUniqueName(string baseUniqueName)
		{
			return 'x' + baseUniqueName;
		}

		internal Stream GetOrCreateChunk(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string chunkName, bool createChunkIfNotExists, out bool isNewChunk)
		{
			if (!this.IsChunkManagerValid())
			{
				isNewChunk = false;
				return null;
			}
			return this.m_chunkManager.GetOrCreateChunk(type, chunkName, createChunkIfNotExists, out isNewChunk);
		}

		internal Stream CreateChunk(AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes type, string chunkName)
		{
			if (!this.IsChunkManagerValid())
			{
				return null;
			}
			return this.m_chunkManager.CreateChunk(type, chunkName);
		}

		private bool IsChunkManagerValid()
		{
			bool result = true;
			if (this.m_chunkManager == null)
			{
				if (this.m_odpContext != null)
				{
					this.m_odpContext.ErrorContext.Register(ProcessingErrorCode.rsRenderingChunksUnavailable, Severity.Warning, ObjectType.Report, "Report", "Report");
					this.m_odpContext.TraceOneTimeWarning(ProcessingErrorCode.rsRenderingChunksUnavailable);
				}
				result = false;
			}
			return result;
		}

		internal void CloseRenderingChunkManager()
		{
			if (this.m_chunkManager != null)
			{
				this.m_chunkManager.CloseAllChunks();
			}
		}

		internal bool IsRenderAsNativeCri(AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem criDef)
		{
			if (this.m_nativeAllCRITypes)
			{
				return true;
			}
			if (this.m_nativeCRITypes != null && this.m_nativeCRITypes.ContainsKey(criDef.Type))
			{
				return true;
			}
			return false;
		}
	}
}
