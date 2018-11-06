using AspNetCore.ReportingServices.Diagnostics;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class RenderingContext
	{
		private ICatalogItemContext m_reportContext;

		private string m_reportDescription;

		private EventInformation m_eventInfo;

		internal ReportProcessing.GetReportChunk m_getReportChunkCallback;

		internal ReportProcessing.GetChunkMimeType m_getChunkMimeType;

		private ReportProcessing.StoreServerParameters m_storeServerParameters;

		private UserProfileState m_allowUserProfileState;

		private ReportRuntimeSetup m_reportRuntimeSetup;

		private PaginationMode m_clientPaginationMode;

		private int m_previousTotalPages;

		internal string Format
		{
			get
			{
				return this.m_reportContext.RSRequestParameters.FormatParamValue;
			}
		}

		internal Uri ReportUri
		{
			get
			{
				string hostRootUri = this.m_reportContext.HostRootUri;
				if (string.IsNullOrEmpty(hostRootUri))
				{
					return null;
				}
				string uriString = new CatalogItemUrlBuilder(this.m_reportContext).ToString();
				return new Uri(uriString);
			}
		}

		internal string ShowHideToggle
		{
			get
			{
				return this.m_reportContext.RSRequestParameters.ShowHideToggleParamValue;
			}
		}

		internal ICatalogItemContext ReportContext
		{
			get
			{
				return this.m_reportContext;
			}
		}

		internal string ReportDescription
		{
			get
			{
				return this.m_reportDescription;
			}
		}

		internal EventInformation EventInfo
		{
			get
			{
				return this.m_eventInfo;
			}
			set
			{
				this.m_eventInfo = value;
			}
		}

		internal ReportProcessing.StoreServerParameters StoreServerParametersCallback
		{
			get
			{
				return this.m_storeServerParameters;
			}
		}

		internal UserProfileState AllowUserProfileState
		{
			get
			{
				return this.m_allowUserProfileState;
			}
		}

		internal ReportRuntimeSetup ReportRuntimeSetup
		{
			get
			{
				return this.m_reportRuntimeSetup;
			}
		}

		internal PaginationMode ClientPaginationMode
		{
			get
			{
				return this.m_clientPaginationMode;
			}
		}

		internal int PreviousTotalPages
		{
			get
			{
				return this.m_previousTotalPages;
			}
		}

		internal RenderingContext(ICatalogItemContext reportContext, string reportDescription, EventInformation eventInfo, ReportRuntimeSetup reportRuntimeSetup, ReportProcessing.StoreServerParameters storeServerParameters, UserProfileState allowUserProfileState, PaginationMode clientPaginationMode, int previousTotalPages)
		{
			Global.Tracer.Assert(null != reportContext, "(null != reportContext)");
			this.m_reportContext = reportContext;
			this.m_reportDescription = reportDescription;
			this.m_eventInfo = eventInfo;
			this.m_storeServerParameters = storeServerParameters;
			this.m_allowUserProfileState = allowUserProfileState;
			this.m_reportRuntimeSetup = reportRuntimeSetup;
			this.m_clientPaginationMode = clientPaginationMode;
			this.m_previousTotalPages = previousTotalPages;
		}

		internal Hashtable GetRenderProperties(bool reprocessSnapshot)
		{
			Hashtable hashtable = new Hashtable(4);
			if (reprocessSnapshot)
			{
				hashtable.Add("ClientPaginationMode", this.m_clientPaginationMode);
				hashtable.Add("PreviousTotalPages", 0);
			}
			else
			{
				hashtable.Add("ClientPaginationMode", this.m_clientPaginationMode);
				hashtable.Add("PreviousTotalPages", this.m_previousTotalPages);
			}
			return hashtable;
		}
	}
}
