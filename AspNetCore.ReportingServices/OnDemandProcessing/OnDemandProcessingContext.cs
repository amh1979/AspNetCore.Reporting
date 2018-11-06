using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class OnDemandProcessingContext : IInternalProcessingContext, IStaticReferenceable
	{
		internal enum Mode
		{
			Full,
			Streaming,
			DefinitionOnly
		}

		private sealed class CommonInfo
		{
			private readonly bool m_enableDataBackedParameters;

			private readonly IChunkFactory m_chunkFactory;

			private readonly AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.OnDemandSubReportCallback m_subReportCallback;

			private readonly IGetResource m_getResourceCallback;

			private readonly AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters m_storeServerParameters;

			private readonly ReportRuntimeSetup m_reportRuntimeSetup;

			private readonly UserProfileState m_allowUserProfileState;

			private readonly string m_requestUserName;

			private readonly CultureInfo m_userLanguage;

			private readonly DateTime m_executionTime;

			private readonly bool m_reprocessSnapshot;

			private readonly bool m_processWithCachedData;

			private int m_uniqueIDCounter;

			private EventInformation m_userSortFilterInfo;

			private SortFilterEventInfoMap m_oldSortFilterEventInfo;

			private SortFilterEventInfoMap m_newSortFilterEventInfo;

			private List<IReference<AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo>> m_reportRuntimeUserSortFilterInfo;

			private EventInformation.OdpSortEventInfo m_newOdpSortEventInfo;

			private string m_userSortFilterEventSourceUniqueName;

			private readonly CreateAndRegisterStream m_createStreamCallback;

			private CustomReportItemControls m_criControls;

			private readonly IJobContext m_jobContext;

			private readonly IExtensionFactory m_extFactory;

			private readonly IDataProtection m_dataProtection;

			private readonly ExecutionLogContext m_executionLogContext;

			private readonly RuntimeDataSourceInfoCollection m_dataSourceInfos;

			private readonly RuntimeDataSetInfoCollection m_sharedDataSetReferences;

			private readonly IProcessingDataExtensionConnection m_createAndSetupDataExtensionFunction;

			private readonly IConfiguration m_configuration;

			private readonly Dictionary<ProcessingErrorCode, bool> m_hasTracedOneTimeMessage;

			private readonly AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable m_globalDataSourceInfo;

			private readonly ReportProcessingContext m_externalProcessingContext;

			private AbortHelper m_abortInfo;

			private readonly bool m_abortInfoInherited;

			private uint m_languageInstanceId;

			[NonSerialized]
			private readonly object m_hasUserProfileStateLock = new object();

			private UserProfileState m_hasUserProfileState;

			private bool m_hasRenderFormatDependencyInDocumentMap;

			private readonly OnDemandProcessingContext m_topLevelContext;

			private readonly Mode m_contextMode;

			private readonly ImageCacheManager m_imageCacheManager;

			internal IGetResource GetResourceCallback
			{
				get
				{
					return this.m_getResourceCallback;
				}
			}

			internal string RequestUserName
			{
				get
				{
					return this.m_requestUserName;
				}
			}

			internal DateTime ExecutionTime
			{
				get
				{
					return this.m_executionTime;
				}
			}

			internal CultureInfo UserLanguage
			{
				get
				{
					return this.m_userLanguage;
				}
			}

			internal AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.OnDemandSubReportCallback SubReportCallback
			{
				get
				{
					return this.m_subReportCallback;
				}
			}

			internal UserProfileState AllowUserProfileState
			{
				get
				{
					return this.m_allowUserProfileState;
				}
			}

			internal bool StreamingMode
			{
				get
				{
					return this.m_contextMode == Mode.Streaming;
				}
			}

			internal bool ReprocessSnapshot
			{
				get
				{
					return this.m_reprocessSnapshot;
				}
			}

			internal bool ProcessWithCachedData
			{
				get
				{
					return this.m_processWithCachedData;
				}
			}

			internal IChunkFactory ChunkFactory
			{
				get
				{
					return this.m_chunkFactory;
				}
			}

			internal ReportRuntimeSetup ReportRuntimeSetup
			{
				get
				{
					return this.m_reportRuntimeSetup;
				}
			}

			internal AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters StoreServerParameters
			{
				get
				{
					return this.m_storeServerParameters;
				}
			}

			internal EventInformation UserSortFilterInfo
			{
				get
				{
					return this.m_userSortFilterInfo;
				}
				set
				{
					this.m_userSortFilterInfo = value;
				}
			}

			internal SortFilterEventInfoMap OldSortFilterEventInfo
			{
				get
				{
					return this.m_oldSortFilterEventInfo;
				}
				set
				{
					this.m_oldSortFilterEventInfo = value;
				}
			}

			internal SortFilterEventInfoMap NewSortFilterEventInfo
			{
				get
				{
					return this.m_newSortFilterEventInfo;
				}
				set
				{
					this.m_newSortFilterEventInfo = value;
				}
			}

			internal string UserSortFilterEventSourceUniqueName
			{
				get
				{
					return this.m_userSortFilterEventSourceUniqueName;
				}
				set
				{
					this.m_userSortFilterEventSourceUniqueName = value;
				}
			}

			internal List<IReference<AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo>> ReportRuntimeUserSortFilterInfo
			{
				get
				{
					return this.m_reportRuntimeUserSortFilterInfo;
				}
				set
				{
					this.m_reportRuntimeUserSortFilterInfo = value;
				}
			}

			internal CreateAndRegisterStream CreateStreamCallback
			{
				get
				{
					return this.m_createStreamCallback;
				}
			}

			internal ReportProcessingContext ExternalProcessingContext
			{
				get
				{
					return this.m_externalProcessingContext;
				}
			}

			internal CustomReportItemControls CriProcessingControls
			{
				get
				{
					if (this.m_criControls == null)
					{
						this.m_criControls = new CustomReportItemControls();
					}
					return this.m_criControls;
				}
				set
				{
					this.m_criControls = value;
				}
			}

			internal bool EnableDataBackedParameters
			{
				get
				{
					return this.m_enableDataBackedParameters;
				}
			}

			internal IJobContext JobContext
			{
				get
				{
					return this.m_jobContext;
				}
			}

			internal IExtensionFactory ExtFactory
			{
				get
				{
					return this.m_extFactory;
				}
			}

			internal IDataProtection DataProtection
			{
				get
				{
					return this.m_dataProtection;
				}
			}

			internal ExecutionLogContext ExecutionLogContext
			{
				get
				{
					return this.m_executionLogContext;
				}
			}

			internal RuntimeDataSourceInfoCollection DataSourceInfos
			{
				get
				{
					return this.m_dataSourceInfos;
				}
			}

			internal RuntimeDataSetInfoCollection SharedDataSetReferences
			{
				get
				{
					return this.m_sharedDataSetReferences;
				}
			}

			internal IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction
			{
				get
				{
					return this.m_createAndSetupDataExtensionFunction;
				}
			}

			internal IConfiguration Configuration
			{
				get
				{
					return this.m_configuration;
				}
			}

			internal AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable GlobalDataSourceInfo
			{
				get
				{
					return this.m_globalDataSourceInfo;
				}
			}

			internal AbortHelper AbortInfo
			{
				get
				{
					return this.m_abortInfo;
				}
			}

			internal uint LanguageInstanceId
			{
				get
				{
					return this.m_languageInstanceId;
				}
				set
				{
					this.m_languageInstanceId = value;
				}
			}

			internal UserProfileState HasUserProfileState
			{
				get
				{
					return this.m_hasUserProfileState;
				}
			}

			internal bool HasRenderFormatDependencyInDocumentMap
			{
				get
				{
					return this.m_hasRenderFormatDependencyInDocumentMap;
				}
				set
				{
					this.m_hasRenderFormatDependencyInDocumentMap = value;
				}
			}

			internal OnDemandProcessingContext TopLevelContext
			{
				get
				{
					return this.m_topLevelContext;
				}
			}

			internal Mode ContextMode
			{
				get
				{
					return this.m_contextMode;
				}
			}

			internal ImageCacheManager ImageCacheManager
			{
				get
				{
					return this.m_imageCacheManager;
				}
			}

			internal CommonInfo(IChunkFactory chunkFactory, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.OnDemandSubReportCallback subReportCallback, IGetResource getResourceCallback, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters storeServerParameters, ReportRuntimeSetup reportRuntimeSetup, UserProfileState allowUserProfileState, string requestUserName, CultureInfo userLanguage, DateTime executionTime, bool reprocessSnapshot, bool processWithCachedData, CreateAndRegisterStream createStreamCallback, bool enableDataBackedParameters, IJobContext jobContext, IExtensionFactory extFactory, IDataProtection dataProtection, ExecutionLogContext executionLogContext, RuntimeDataSourceInfoCollection dataSourceInfos, RuntimeDataSetInfoCollection sharedDataSetReferences, IProcessingDataExtensionConnection createAndSetupDataExtensionFunction, IConfiguration configuration, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable globalDataSourceInfo, ReportProcessingContext externalProcessingContext, AbortHelper abortInfo, bool abortInfoInherited, UserProfileState hasUserProfileState, OnDemandProcessingContext topLevelContext, Mode contextMode, ImageCacheManager imageCacheManager)
			{
				this.m_chunkFactory = chunkFactory;
				this.m_subReportCallback = subReportCallback;
				this.m_getResourceCallback = getResourceCallback;
				this.m_storeServerParameters = storeServerParameters;
				this.m_reportRuntimeSetup = reportRuntimeSetup;
				this.m_allowUserProfileState = allowUserProfileState;
				this.m_requestUserName = requestUserName;
				this.m_userLanguage = userLanguage;
				this.m_executionTime = executionTime;
				this.m_reprocessSnapshot = reprocessSnapshot;
				this.m_processWithCachedData = processWithCachedData;
				this.m_createStreamCallback = createStreamCallback;
				this.m_enableDataBackedParameters = enableDataBackedParameters;
				this.m_jobContext = jobContext;
				this.m_extFactory = extFactory;
				this.m_dataProtection = dataProtection;
				this.m_executionLogContext = executionLogContext;
				this.m_dataSourceInfos = dataSourceInfos;
				this.m_sharedDataSetReferences = sharedDataSetReferences;
				this.m_createAndSetupDataExtensionFunction = createAndSetupDataExtensionFunction;
				this.m_configuration = configuration;
				this.m_hasTracedOneTimeMessage = new Dictionary<ProcessingErrorCode, bool>();
				this.m_globalDataSourceInfo = globalDataSourceInfo;
				this.m_externalProcessingContext = externalProcessingContext;
				this.m_abortInfo = abortInfo;
				this.m_abortInfoInherited = abortInfoInherited;
				this.m_hasUserProfileState = hasUserProfileState;
				this.m_topLevelContext = topLevelContext;
				this.m_contextMode = contextMode;
				this.m_imageCacheManager = imageCacheManager;
			}

			internal void MergeHasUserProfileState(UserProfileState newProfileStateFlags)
			{
				lock (this.m_hasUserProfileStateLock)
				{
					this.m_hasUserProfileState |= newProfileStateFlags;
				}
			}

			internal void UnregisterAbortInfo()
			{
				if (this.m_abortInfo != null && !this.m_abortInfoInherited)
				{
					this.m_abortInfo.Dispose();
					this.m_abortInfo = null;
				}
			}

			internal int CreateUniqueID()
			{
				return ++this.m_uniqueIDCounter;
			}

			internal EventInformation GetUserSortFilterInformation(out string sourceUniqueName)
			{
				sourceUniqueName = this.m_userSortFilterEventSourceUniqueName;
				if (this.m_newOdpSortEventInfo == null)
				{
					return null;
				}
				EventInformation eventInformation = new EventInformation();
				eventInformation.OdpSortInfo = this.m_newOdpSortEventInfo;
				return eventInformation;
			}

			internal void MergeNewUserSortFilterInformation()
			{
				int num = (this.m_reportRuntimeUserSortFilterInfo != null) ? this.m_reportRuntimeUserSortFilterInfo.Count : 0;
				if (num != 0)
				{
					if (this.m_newOdpSortEventInfo == null)
					{
						this.m_newOdpSortEventInfo = new EventInformation.OdpSortEventInfo();
					}
					for (int i = 0; i < num; i++)
					{
						IReference<AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo> reference = this.m_reportRuntimeUserSortFilterInfo[i];
						using (reference.PinValue())
						{
							AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo runtimeSortFilterEventInfo = reference.Value();
							if (runtimeSortFilterEventInfo.NewUniqueName == null)
							{
								runtimeSortFilterEventInfo.NewUniqueName = runtimeSortFilterEventInfo.OldUniqueName;
							}
							Hashtable hashtable = null;
							if (runtimeSortFilterEventInfo.PeerSortFilters != null)
							{
								int count = runtimeSortFilterEventInfo.PeerSortFilters.Count;
								if (count > 0)
								{
									hashtable = new Hashtable(count);
									IDictionaryEnumerator enumerator = runtimeSortFilterEventInfo.PeerSortFilters.GetEnumerator();
									while (enumerator.MoveNext())
									{
										if (enumerator.Value != null)
										{
											hashtable.Add(enumerator.Value, null);
										}
									}
								}
							}
							this.m_newOdpSortEventInfo.Add(runtimeSortFilterEventInfo.NewUniqueName, runtimeSortFilterEventInfo.SortDirection, hashtable);
							if (runtimeSortFilterEventInfo.OldUniqueName == this.m_userSortFilterEventSourceUniqueName)
							{
								this.m_userSortFilterEventSourceUniqueName = runtimeSortFilterEventInfo.NewUniqueName;
							}
						}
					}
					this.m_reportRuntimeUserSortFilterInfo = null;
				}
			}

			internal void TraceOneTimeWarning(ProcessingErrorCode errorCode, ICatalogItemContext itemContext)
			{
				if (this.m_hasTracedOneTimeMessage != null && !this.m_hasTracedOneTimeMessage.ContainsKey(errorCode))
				{
					string text = itemContext.ItemPathAsString.MarkAsPrivate();
					switch (errorCode)
					{
					case ProcessingErrorCode.rsSandboxingArrayResultExceedsMaximumLength:
						Global.Tracer.Trace(TraceLevel.Info, "RDL Sandboxing: Item: '{0}' attempted to use an array that violated the maximum allowed length.", text);
						break;
					case ProcessingErrorCode.rsSandboxingStringResultExceedsMaximumLength:
						Global.Tracer.Trace(TraceLevel.Info, "RDL Sandboxing: Item: '{0}' attempted to use a String that violated the maximum allowed length.", text);
						break;
					case ProcessingErrorCode.rsSandboxingExternalResourceExceedsMaximumSize:
						Global.Tracer.Trace(TraceLevel.Info, "RDL Sandboxing: Item: '{0}' attempted to reference an external resource larger than the maximum allowed size.", text);
						break;
					case ProcessingErrorCode.rsRenderingChunksUnavailable:
						Global.Tracer.Trace(TraceLevel.Info, "A rendering extension attempted to use Report.GetOrCreateChunk or Report.CreateChunk while rendering item '{0}'. Rendering chunks are not available using the current report execution method.", text);
						break;
					default:
						Global.Tracer.Assert(false, "Invalid error code: '{0}'.  Expected an error code", errorCode);
						break;
					}
					this.m_hasTracedOneTimeMessage[errorCode] = true;
				}
			}
		}

		internal sealed class CustomReportItemControls
		{
			private class CustomControlInfo
			{
				private bool m_valid;

				private ICustomReportItem m_instance;

				internal bool IsValid
				{
					get
					{
						return this.m_valid;
					}
					set
					{
						this.m_valid = value;
					}
				}

				internal ICustomReportItem Instance
				{
					get
					{
						return this.m_instance;
					}
					set
					{
						this.m_instance = value;
					}
				}
			}

			private Hashtable m_controls;

			internal CustomReportItemControls()
			{
				this.m_controls = new Hashtable();
			}

			internal ICustomReportItem GetControlInstance(string name, IExtensionFactory extFactory)
			{
				lock (this)
				{
					CustomControlInfo customControlInfo = this.m_controls[name] as CustomControlInfo;
					if (customControlInfo == null)
					{
						ICustomReportItem customReportItem = null;
						Global.Tracer.Assert(extFactory != null, "extFactory != null");
						customReportItem = (extFactory.GetNewCustomReportItemProcessingInstanceClass(name) as ICustomReportItem);
						customControlInfo = new CustomControlInfo();
						customControlInfo.Instance = customReportItem;
						customControlInfo.IsValid = (null != customReportItem);
						this.m_controls.Add(name, customControlInfo);
					}
					Global.Tracer.Assert(null != customControlInfo, "(null != info)");
					if (customControlInfo.IsValid)
					{
						return customControlInfo.Instance;
					}
					return null;
				}
			}
		}

		private readonly DataSetContext m_externalDataSetContext;

		private readonly OnDemandProcessingContext m_parentContext;

		private readonly CommonInfo m_commonInfo;

		private readonly ICatalogItemContext m_catalogItemContext;

		private ObjectModelImpl m_reportObjectModel;

		private readonly bool m_reportItemsReferenced;

		private bool m_reportItemThisDotValueReferenced;

		private readonly Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> m_embeddedImages;

		private bool m_processReportParameters;

		private AspNetCore.ReportingServices.RdlExpressions.ReportRuntime m_reportRuntime;

		private ParameterInfoCollection m_reportParameters;

		private readonly ErrorContext m_errorContext;

		private bool m_snapshotProcessing;

		private CultureInfo m_threadCulture;

		private CompareInfo m_compareInfo = Thread.CurrentThread.CurrentCulture.CompareInfo;

		private CompareOptions m_clrCompareOptions;

		private bool m_nullsAsBlanks;

		private bool m_useOrdinalStringKeyGeneration;

		private IDataComparer m_processingComparer;

		private StringKeyGenerator m_stringKeyGenerator;

		private readonly bool m_inSubreport;

		private readonly bool m_inSubreportInDataRegion;

		private bool m_isTablixProcessingMode;

		private bool m_isTopLevelSubReportProcessing;

		private bool m_isUnrestrictedRenderFormatReferenceMode;

		private readonly bool m_isSharedDataSetExecutionOnly;

		private readonly Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> m_reportAggregates = new Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo>();

		private bool m_errorSavingSnapshotData;

		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_reportDefinition;

		private readonly OnDemandMetadata m_odpMetadata;

		private bool m_hasBookmarks;

		private bool m_hasShowHide;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance m_currentReportInstance;

		private int m_currentDataSetIndex = -1;

		private List<object> m_groupExprValues = new List<object>();

		private bool m_peerOuterGroupProcessing;

		private string m_subReportInstanceOrSharedDatasetUniqueName;

		private bool m_foundExistingSubReportInstance;

		private string m_subReportDataChunkNameModifier;

		private SubReportInfo m_subReportInfo;

		private readonly bool m_specialRecursiveAggregates;

		private SecondPassOperations m_secondPassOperation;

		private List<Filters> m_specialDataRegionFilters;

		private List<IReference<IDataRowSortOwner>> m_dataRowSortOwners;

		private readonly AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext m_userSortFilterContext;

		private bool m_initializedRuntime;

		private readonly bool m_isPageHeaderFooter;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader[] m_dataSetToDataReader;

		private bool[] m_dataSetRetrievalComplete;

		private IScalabilityCache m_tablixProcessingScalabilityCache;

		private CommonRowCache m_tablixProcessingLookupRowCache;

		private int m_staticRefId = 2147483647;

		private DomainScopeContext m_domainScopeContext;

		private readonly OnDemandStateManager m_stateManager;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance m_currentDataSetInstance;

		internal UserProfileState HasUserProfileState
		{
			get
			{
				return this.m_commonInfo.HasUserProfileState;
			}
		}

		internal bool HasRenderFormatDependencyInDocumentMap
		{
			get
			{
				return this.m_commonInfo.HasRenderFormatDependencyInDocumentMap;
			}
			set
			{
				this.m_commonInfo.HasRenderFormatDependencyInDocumentMap = value;
			}
		}

		internal ExecutionLogContext ExecutionLogContext
		{
			get
			{
				return this.m_commonInfo.ExecutionLogContext;
			}
		}

		internal IJobContext JobContext
		{
			get
			{
				return this.m_commonInfo.JobContext;
			}
		}

		internal IExtensionFactory ExtFactory
		{
			get
			{
				return this.m_commonInfo.ExtFactory;
			}
		}

		internal IDataProtection DataProtection
		{
			get
			{
				return this.m_commonInfo.DataProtection;
			}
		}

		public bool EnableDataBackedParameters
		{
			get
			{
				return this.m_commonInfo.EnableDataBackedParameters;
			}
		}

		internal IChunkFactory ChunkFactory
		{
			get
			{
				return this.m_commonInfo.ChunkFactory;
			}
		}

		internal string RequestUserName
		{
			get
			{
				return this.m_commonInfo.RequestUserName;
			}
		}

		public DateTime ExecutionTime
		{
			get
			{
				return this.m_commonInfo.ExecutionTime;
			}
		}

		internal CultureInfo UserLanguage
		{
			get
			{
				return this.m_commonInfo.UserLanguage;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.OnDemandSubReportCallback SubReportCallback
		{
			get
			{
				return this.m_commonInfo.SubReportCallback;
			}
		}

		internal bool HasBookmarks
		{
			get
			{
				return this.m_hasBookmarks;
			}
			set
			{
				if (this.m_parentContext != null)
				{
					this.m_parentContext.HasBookmarks |= value;
				}
				else if (!this.SnapshotProcessing || this.m_commonInfo.ReprocessSnapshot)
				{
					this.m_odpMetadata.ReportSnapshot.HasBookmarks |= value;
				}
				this.m_hasBookmarks |= value;
			}
		}

		internal bool HasShowHide
		{
			get
			{
				return this.m_hasShowHide;
			}
			set
			{
				if (this.m_parentContext != null)
				{
					this.m_parentContext.HasShowHide |= value;
				}
				else if (!this.SnapshotProcessing || this.m_commonInfo.ReprocessSnapshot)
				{
					this.m_odpMetadata.ReportSnapshot.HasShowHide |= value;
				}
				this.m_hasShowHide |= value;
			}
		}

		internal bool HasUserSortFilter
		{
			get
			{
				if (this.m_reportDefinition == null)
				{
					return false;
				}
				return this.m_reportDefinition.ReportOrDescendentHasUserSortFilter;
			}
		}

		internal UserProfileState AllowUserProfileState
		{
			get
			{
				return this.m_commonInfo.AllowUserProfileState;
			}
		}

		public bool SnapshotProcessing
		{
			get
			{
				return this.m_snapshotProcessing;
			}
			set
			{
				this.m_snapshotProcessing = value;
			}
		}

		public bool ReprocessSnapshot
		{
			get
			{
				return this.m_commonInfo.ReprocessSnapshot;
			}
		}

		internal bool ProcessWithCachedData
		{
			get
			{
				return this.m_commonInfo.ProcessWithCachedData;
			}
		}

		internal bool StreamingMode
		{
			get
			{
				return this.m_commonInfo.StreamingMode;
			}
		}

		internal bool UseVerboseExecutionLogging
		{
			get
			{
				if (this.JobContext != null && this.JobContext.ExecutionLogLevel == ExecutionLogLevel.Verbose)
				{
					return this.m_commonInfo.StreamingMode;
				}
				return false;
			}
		}

		internal bool ShouldExecuteLiveQueries
		{
			get
			{
				if (!this.StreamingMode)
				{
					if (!this.SnapshotProcessing)
					{
						return !this.ReprocessSnapshot;
					}
					return false;
				}
				return true;
			}
		}

		internal ReportRuntimeSetup ReportRuntimeSetup
		{
			get
			{
				return this.m_commonInfo.ReportRuntimeSetup;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters StoreServerParameters
		{
			get
			{
				return this.m_commonInfo.StoreServerParameters;
			}
		}

		internal bool HasPreviousAggregates
		{
			get
			{
				if (this.m_reportDefinition == null)
				{
					return false;
				}
				return this.m_reportDefinition.HasPreviousAggregates;
			}
		}

		internal EventInformation UserSortFilterInfo
		{
			get
			{
				return this.m_commonInfo.UserSortFilterInfo;
			}
			set
			{
				this.m_commonInfo.UserSortFilterInfo = value;
			}
		}

		internal SortFilterEventInfoMap OldSortFilterEventInfo
		{
			get
			{
				return this.m_commonInfo.OldSortFilterEventInfo;
			}
			set
			{
				this.m_commonInfo.OldSortFilterEventInfo = value;
			}
		}

		internal string UserSortFilterEventSourceUniqueName
		{
			get
			{
				return this.m_commonInfo.UserSortFilterEventSourceUniqueName;
			}
			set
			{
				this.m_commonInfo.UserSortFilterEventSourceUniqueName = value;
			}
		}

		internal SortFilterEventInfoMap NewSortFilterEventInfo
		{
			get
			{
				return this.m_commonInfo.NewSortFilterEventInfo;
			}
			set
			{
				this.m_commonInfo.NewSortFilterEventInfo = value;
			}
		}

		internal List<IReference<AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo>> ReportRuntimeUserSortFilterInfo
		{
			get
			{
				return this.m_commonInfo.ReportRuntimeUserSortFilterInfo;
			}
			set
			{
				this.m_commonInfo.ReportRuntimeUserSortFilterInfo = value;
			}
		}

		internal CreateAndRegisterStream CreateStreamCallback
		{
			get
			{
				return this.m_commonInfo.CreateStreamCallback;
			}
		}

		internal bool IsPageHeaderFooter
		{
			get
			{
				return this.m_isPageHeaderFooter;
			}
		}

		internal Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo> ReportAggregates
		{
			get
			{
				return this.m_reportAggregates;
			}
		}

		internal OnDemandStateManager StateManager
		{
			get
			{
				return this.m_stateManager;
			}
		}

		internal ReportProcessingContext ExternalProcessingContext
		{
			get
			{
				return this.m_commonInfo.ExternalProcessingContext;
			}
		}

		internal DataSetContext ExternalDataSetContext
		{
			get
			{
				return this.m_externalDataSetContext;
			}
		}

		internal bool ErrorSavingSnapshotData
		{
			get
			{
				return this.m_errorSavingSnapshotData;
			}
			set
			{
				this.m_errorSavingSnapshotData = value;
			}
		}

		internal OnDemandProcessingContext ParentContext
		{
			get
			{
				return this.m_parentContext;
			}
		}

		internal OnDemandProcessingContext TopLevelContext
		{
			get
			{
				return this.m_commonInfo.TopLevelContext;
			}
		}

		internal RuntimeDataSourceInfoCollection DataSourceInfos
		{
			get
			{
				return this.m_commonInfo.DataSourceInfos;
			}
		}

		internal RuntimeDataSetInfoCollection SharedDataSetReferences
		{
			get
			{
				return this.m_commonInfo.SharedDataSetReferences;
			}
		}

		internal IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction
		{
			get
			{
				return this.m_commonInfo.CreateAndSetupDataExtensionFunction;
			}
		}

		internal CultureInfo ThreadCulture
		{
			get
			{
				return this.m_threadCulture;
			}
			set
			{
				this.m_threadCulture = value;
			}
		}

		internal uint LanguageInstanceId
		{
			get
			{
				return this.m_commonInfo.LanguageInstanceId;
			}
			set
			{
				this.m_commonInfo.LanguageInstanceId = value;
			}
		}

		internal ICatalogItemContext ReportContext
		{
			get
			{
				return this.m_catalogItemContext;
			}
		}

		internal AspNetCore.ReportingServices.RdlExpressions.ReportRuntime ReportRuntime
		{
			get
			{
				return this.m_reportRuntime;
			}
			set
			{
				this.m_reportRuntime = value;
			}
		}

		internal ObjectModelImpl ReportObjectModel
		{
			get
			{
				return this.m_reportObjectModel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Report ReportDefinition
		{
			get
			{
				return this.m_reportDefinition;
			}
		}

		internal OnDemandMetadata OdpMetadata
		{
			get
			{
				return this.m_odpMetadata;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance CurrentReportInstance
		{
			get
			{
				return this.m_currentReportInstance;
			}
			set
			{
				this.m_currentReportInstance = value;
			}
		}

		internal int CurrentDataSetIndex
		{
			get
			{
				if (this.m_reportObjectModel.CurrentFields != null && this.m_reportObjectModel.CurrentFields.DataSet != null)
				{
					return this.m_currentDataSetIndex;
				}
				return -1;
			}
		}

		internal ImageCacheManager ImageCacheManager
		{
			get
			{
				return this.m_commonInfo.ImageCacheManager;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance CurrentOdpDataSetInstance
		{
			get
			{
				return this.m_currentDataSetInstance;
			}
		}

		internal IReportScope CurrentReportScope
		{
			get
			{
				IReportScopeInstance lastROMInstance = this.m_stateManager.LastROMInstance;
				if (lastROMInstance == null)
				{
					return null;
				}
				return lastROMInstance.ReportScope;
			}
		}

		internal List<object> GroupExpressionValues
		{
			get
			{
				return this.m_groupExprValues;
			}
		}

		internal bool PeerOuterGroupProcessing
		{
			get
			{
				return this.m_peerOuterGroupProcessing;
			}
			set
			{
				this.m_peerOuterGroupProcessing = value;
			}
		}

		internal bool ReportItemsReferenced
		{
			get
			{
				return this.m_reportItemsReferenced;
			}
		}

		internal bool ReportItemThisDotValueReferenced
		{
			get
			{
				return this.m_reportItemThisDotValueReferenced;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable GlobalDataSourceInfo
		{
			get
			{
				return this.m_commonInfo.GlobalDataSourceInfo;
			}
		}

		internal Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.ImageInfo> EmbeddedImages
		{
			get
			{
				return this.m_embeddedImages;
			}
		}

		public ErrorContext ErrorContext
		{
			get
			{
				return this.m_errorContext;
			}
		}

		internal bool ProcessReportParameters
		{
			get
			{
				return this.m_processReportParameters;
			}
			set
			{
				this.m_processReportParameters = value;
			}
		}

		internal CompareInfo CompareInfo
		{
			get
			{
				return this.m_compareInfo;
			}
		}

		internal CompareOptions ClrCompareOptions
		{
			get
			{
				return this.m_clrCompareOptions;
			}
			set
			{
				this.SetComparisonInformation(this.m_compareInfo, value, this.m_nullsAsBlanks, this.m_useOrdinalStringKeyGeneration);
			}
		}

		internal bool NullsAsBlanks
		{
			get
			{
				return this.m_nullsAsBlanks;
			}
		}

		internal bool UseOrdinalStringKeyGeneration
		{
			get
			{
				return this.m_useOrdinalStringKeyGeneration;
			}
		}

		internal IDataComparer ProcessingComparer
		{
			get
			{
				if (this.m_processingComparer == null)
				{
					if (this.m_commonInfo.StreamingMode)
					{
						this.m_processingComparer = new CommonDataComparer(this.m_compareInfo, this.m_clrCompareOptions, this.m_nullsAsBlanks);
					}
					else
					{
						this.m_processingComparer = new AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ProcessingComparer(this.m_compareInfo, this.m_clrCompareOptions, this.m_nullsAsBlanks);
					}
				}
				return this.m_processingComparer;
			}
		}

		internal StringKeyGenerator StringKeyGenerator
		{
			get
			{
				if (this.m_stringKeyGenerator == null)
				{
					this.m_stringKeyGenerator = new StringKeyGenerator(this.m_compareInfo, this.m_clrCompareOptions, this.m_nullsAsBlanks, this.m_useOrdinalStringKeyGeneration);
				}
				return this.m_stringKeyGenerator;
			}
		}

		internal IEqualityComparer<object> EqualityComparer
		{
			get
			{
				return this.ProcessingComparer;
			}
		}

		internal string SubReportDataChunkNameModifier
		{
			get
			{
				return this.m_subReportDataChunkNameModifier;
			}
		}

		internal string ProcessingAbortItemUniqueIdentifier
		{
			get
			{
				if (!this.m_inSubreport && !this.m_isSharedDataSetExecutionOnly)
				{
					return null;
				}
				return this.m_subReportInstanceOrSharedDatasetUniqueName;
			}
		}

		internal bool FoundExistingSubReportInstance
		{
			get
			{
				return this.m_foundExistingSubReportInstance;
			}
		}

		internal string SubReportUniqueName
		{
			get
			{
				if (this.m_inSubreport && this.m_subReportInfo != null)
				{
					return this.m_subReportInfo.UniqueName;
				}
				return null;
			}
		}

		internal string ReportFolder
		{
			get
			{
				if (this.m_inSubreport)
				{
					string text = this.m_subReportInfo.CommonSubReportInfo.OriginalCatalogPath;
					if (!string.IsNullOrEmpty(text))
					{
						int num = text.LastIndexOf('/');
						if (num > 0)
						{
							return text.Substring(0, num);
						}
					}
					else
					{
						text = "/";
					}
					return text;
				}
				return this.m_catalogItemContext.ParentPath;
			}
		}

		internal bool InSubreport
		{
			get
			{
				return this.m_inSubreport;
			}
		}

		internal bool InSubreportInDataRegion
		{
			get
			{
				return this.m_inSubreportInDataRegion;
			}
		}

		internal AbortHelper AbortInfo
		{
			get
			{
				return this.m_commonInfo.AbortInfo;
			}
		}

		internal SecondPassOperations SecondPassOperation
		{
			get
			{
				return this.m_secondPassOperation;
			}
			set
			{
				this.m_secondPassOperation = value;
			}
		}

		internal bool SpecialRecursiveAggregates
		{
			get
			{
				return this.m_specialRecursiveAggregates;
			}
		}

		internal bool InitializedRuntime
		{
			get
			{
				return this.m_initializedRuntime;
			}
			set
			{
				this.m_initializedRuntime = value;
			}
		}

		internal bool[] DataSetRetrievalComplete
		{
			get
			{
				return this.m_dataSetRetrievalComplete;
			}
		}

		internal ParameterInfoCollection ReportParameters
		{
			get
			{
				return this.m_reportParameters;
			}
			set
			{
				this.m_reportParameters = value;
			}
		}

		internal IScalabilityCache TablixProcessingScalabilityCache
		{
			get
			{
				return this.m_tablixProcessingScalabilityCache;
			}
		}

		internal CommonRowCache TablixProcessingLookupRowCache
		{
			get
			{
				return this.m_tablixProcessingLookupRowCache;
			}
			set
			{
				this.m_tablixProcessingLookupRowCache = value;
			}
		}

		internal CustomReportItemControls CriProcessingControls
		{
			get
			{
				return this.m_commonInfo.CriProcessingControls;
			}
			set
			{
				this.m_commonInfo.CriProcessingControls = value;
			}
		}

		internal IConfiguration Configuration
		{
			get
			{
				return this.m_commonInfo.Configuration;
			}
		}

		internal DomainScopeContext DomainScopeContext
		{
			get
			{
				return this.m_domainScopeContext;
			}
			set
			{
				this.m_domainScopeContext = value;
			}
		}

		internal Mode ContextMode
		{
			get
			{
				return this.m_commonInfo.ContextMode;
			}
		}

		internal bool ProhibitSerializableValues
		{
			get
			{
				if (this.Configuration != null)
				{
					return this.Configuration.ProhibitSerializableValues;
				}
				return false;
			}
		}

		internal bool IsTablixProcessingMode
		{
			get
			{
				return this.m_isTablixProcessingMode;
			}
			set
			{
				this.m_isTablixProcessingMode = value;
				this.m_stateManager.ResetOnDemandState();
			}
		}

		internal bool IsUnrestrictedRenderFormatReferenceMode
		{
			get
			{
				return this.m_isUnrestrictedRenderFormatReferenceMode;
			}
			set
			{
				this.m_isUnrestrictedRenderFormatReferenceMode = value;
			}
		}

		internal bool IsTopLevelSubReportProcessing
		{
			get
			{
				return this.m_isTopLevelSubReportProcessing;
			}
			set
			{
				this.m_isTopLevelSubReportProcessing = value;
			}
		}

		internal bool IsSharedDataSetExecutionOnly
		{
			get
			{
				return this.m_isSharedDataSetExecutionOnly;
			}
		}

		internal IInstancePath LastRIFObject
		{
			get
			{
				return this.m_stateManager.LastRIFObject;
			}
			set
			{
				this.m_stateManager.LastRIFObject = value;
			}
		}

		internal QueryRestartInfo QueryRestartInfo
		{
			get
			{
				return this.m_stateManager.QueryRestartInfo;
			}
		}

		internal IRIFReportScope LastTablixProcessingReportScope
		{
			get
			{
				return this.m_stateManager.LastTablixProcessingReportScope;
			}
			set
			{
				this.m_stateManager.LastTablixProcessingReportScope = value;
			}
		}

		internal AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext UserSortFilterContext
		{
			get
			{
				return this.m_userSortFilterContext;
			}
		}

		internal List<IReference<AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo>> RuntimeSortFilterInfo
		{
			get
			{
				return this.m_userSortFilterContext.RuntimeSortFilterInfo;
			}
		}

		int IStaticReferenceable.ID
		{
			get
			{
				return this.m_staticRefId;
			}
		}

		internal OnDemandProcessingContext(ProcessingContext externalProcessingContext, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, OnDemandMetadata odpMetadata, ErrorContext errorContext, DateTime executionTime, bool snapshotProcessing, bool reprocessSnapshot, bool processWithCachedData, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters storeServerParameters, UserProfileState userProfileState, ExecutionLogContext executionLogContext, IConfiguration configuration, Mode contextMode, IAbortHelper abortHelper)
		{
			IJobContext jobContext = externalProcessingContext.JobContext;
			AbortHelper abortHelper2 = null;
			bool abortInfoInherited = false;
			abortHelper2 = (abortHelper as AbortHelper);
			if (abortHelper2 == null)
			{
				if (!snapshotProcessing && !reprocessSnapshot)
				{
					abortHelper2 = new ReportAbortHelper(externalProcessingContext.JobContext, contextMode == Mode.Streaming);
				}
			}
			else
			{
				abortInfoInherited = true;
			}
			this.m_commonInfo = new CommonInfo(externalProcessingContext.ChunkFactory, externalProcessingContext.OnDemandSubReportCallback, externalProcessingContext.GetResourceCallback, storeServerParameters, externalProcessingContext.ReportRuntimeSetup, externalProcessingContext.AllowUserProfileState, externalProcessingContext.RequestUserName, externalProcessingContext.UserLanguage, executionTime, reprocessSnapshot, processWithCachedData, externalProcessingContext.CreateStreamCallback, externalProcessingContext.EnableDataBackedParameters, externalProcessingContext.JobContext, externalProcessingContext.ExtFactory, externalProcessingContext.DataProtection, executionLogContext, externalProcessingContext.DataSources, externalProcessingContext.SharedDataSetReferences, externalProcessingContext.CreateAndSetupDataExtensionFunction, configuration, new AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable(), externalProcessingContext as ReportProcessingContext, abortHelper2, abortInfoInherited, userProfileState, this, contextMode, OnDemandProcessingContext.CreateImageCacheManager(contextMode, odpMetadata, externalProcessingContext.ChunkFactory));
			this.m_errorContext = errorContext;
			this.m_snapshotProcessing = snapshotProcessing;
			this.m_catalogItemContext = externalProcessingContext.ReportContext;
			this.m_reportDefinition = report;
			this.m_odpMetadata = odpMetadata;
			this.m_parentContext = null;
			this.m_reportItemsReferenced = report.HasReportItemReferences;
			this.m_reportItemThisDotValueReferenced = false;
			this.m_embeddedImages = report.EmbeddedImages;
			this.m_processReportParameters = false;
			this.m_reportRuntime = null;
			this.m_inSubreport = false;
			this.m_inSubreportInDataRegion = false;
			this.m_isSharedDataSetExecutionOnly = false;
			this.m_externalDataSetContext = null;
			this.m_stateManager = this.CreateStateManager(contextMode);
			this.m_reportObjectModel = new ObjectModelImpl(this);
			if (contextMode != Mode.DefinitionOnly)
			{
				this.m_specialRecursiveAggregates = report.HasSpecialRecursiveAggregates;
				this.m_userSortFilterContext = new AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext();
				this.InitializeDataSetMembers(report.MappingNameToDataSet.Count);
			}
			this.InitFlags(report);
			this.m_odpMetadata.OdpContexts.Add(this);
		}

		internal OnDemandProcessingContext(ProcessingContext externalProcessingContext, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, OnDemandMetadata odpMetadata, ErrorContext errorContext, DateTime executionTime, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.StoreServerParameters storeServerParameters, UserProfileState userProfileState, ExecutionLogContext executionLogContext, IConfiguration configuration, IAbortHelper abortHelper)
			: this(externalProcessingContext, report, odpMetadata, errorContext, executionTime, false, false, false, storeServerParameters, userProfileState, executionLogContext, configuration, Mode.DefinitionOnly, abortHelper)
		{
		}

		internal OnDemandProcessingContext(OnDemandProcessingContext aContext, bool aReportItemsReferenced, AspNetCore.ReportingServices.ReportIntermediateFormat.Report aReport)
		{
			this.m_isPageHeaderFooter = true;
			this.m_reportDefinition = aReport;
			this.m_parentContext = aContext;
			this.m_odpMetadata = aContext.OdpMetadata;
			this.m_commonInfo = aContext.m_commonInfo;
			this.m_errorContext = aContext.ErrorContext;
			this.m_inSubreport = aContext.m_inSubreport;
			this.m_inSubreportInDataRegion = aContext.m_inSubreportInDataRegion;
			this.m_isSharedDataSetExecutionOnly = false;
			this.m_externalDataSetContext = null;
			this.m_snapshotProcessing = aContext.m_snapshotProcessing;
			this.m_catalogItemContext = aContext.m_catalogItemContext;
			this.m_reportItemsReferenced = aReportItemsReferenced;
			this.m_reportItemThisDotValueReferenced = false;
			this.m_embeddedImages = aContext.m_embeddedImages;
			this.m_processReportParameters = false;
			this.m_initializedRuntime = false;
			this.m_specialRecursiveAggregates = false;
			this.m_userSortFilterContext = new AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext();
			this.m_threadCulture = aContext.m_threadCulture;
			this.m_compareInfo = aContext.m_compareInfo;
			this.m_clrCompareOptions = aContext.m_clrCompareOptions;
			this.m_stateManager = this.CreateStateManager(this.m_commonInfo.ContextMode);
			if (this.m_commonInfo.ContextMode != Mode.DefinitionOnly)
			{
				this.m_reportObjectModel = new ObjectModelImpl(aContext.ReportObjectModel, this);
				this.m_reportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.OnDemandExpressions);
				this.m_reportRuntime = new AspNetCore.ReportingServices.RdlExpressions.ReportRuntime(aReport.ObjectType, this.m_reportObjectModel, this.ErrorContext);
				this.m_reportRuntime.LoadCompiledCode(aReport, false, false, this.m_reportObjectModel, this.ReportRuntimeSetup);
				this.m_reportRuntime.CustomCodeOnInit(aReport);
			}
			this.m_isUnrestrictedRenderFormatReferenceMode = true;
			this.m_odpMetadata.OdpContexts.Add(this);
		}

		internal OnDemandProcessingContext(ProcessingContext originalProcessingContext, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, DateTime executionTime, bool snapshotProcessing, IConfiguration configuration)
		{
			this.m_commonInfo = new CommonInfo(null, null, null, null, originalProcessingContext.ReportRuntimeSetup, originalProcessingContext.AllowUserProfileState, originalProcessingContext.RequestUserName, originalProcessingContext.UserLanguage, executionTime, false, false, originalProcessingContext.CreateStreamCallback, originalProcessingContext.EnableDataBackedParameters, originalProcessingContext.JobContext, originalProcessingContext.ExtFactory, originalProcessingContext.DataProtection, new ExecutionLogContext(originalProcessingContext.JobContext), originalProcessingContext.DataSources, originalProcessingContext.SharedDataSetReferences, originalProcessingContext.CreateAndSetupDataExtensionFunction, configuration, new AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable(), originalProcessingContext as ReportProcessingContext, new ReportAbortHelper(originalProcessingContext.JobContext, false), false, UserProfileState.None, this, Mode.Full, null);
			this.m_errorContext = errorContext;
			this.m_snapshotProcessing = snapshotProcessing;
			this.m_catalogItemContext = originalProcessingContext.ReportContext;
			this.m_reportDefinition = report;
			this.m_odpMetadata = null;
			this.m_reportItemsReferenced = false;
			this.m_reportItemThisDotValueReferenced = false;
			this.m_embeddedImages = null;
			this.m_processReportParameters = true;
			this.m_initializedRuntime = false;
			this.m_specialRecursiveAggregates = false;
			this.m_userSortFilterContext = new AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext();
			this.m_inSubreport = false;
			this.m_inSubreportInDataRegion = false;
			this.m_isSharedDataSetExecutionOnly = false;
			this.m_externalDataSetContext = null;
			this.m_stateManager = new OnDemandStateManagerFull(this);
			if (report != null)
			{
				this.m_reportObjectModel = new ObjectModelImpl(this);
				this.m_reportObjectModel.Initialize(report, null);
				this.m_reportRuntime = new AspNetCore.ReportingServices.RdlExpressions.ReportRuntime(report.ObjectType, this.m_reportObjectModel, this.ErrorContext);
				this.m_reportRuntime.LoadCompiledCode(report, true, true, this.m_reportObjectModel, this.ReportRuntimeSetup);
				this.m_reportRuntime.CustomCodeOnInit(report);
			}
		}

		internal OnDemandProcessingContext(DataSetContext dc, DataSetDefinition dataSetDefinition, ErrorContext errorContext, IConfiguration configuration)
		{
			this.m_externalDataSetContext = dc;
			AbortHelper abortHelper = dc.JobContext.GetAbortHelper() as AbortHelper;
			bool abortInfoInherited;
			if (abortHelper != null)
			{
				abortInfoInherited = true;
			}
			else
			{
				abortHelper = new ReportAbortHelper(dc.JobContext, false);
				abortInfoInherited = false;
			}
			this.m_commonInfo = new CommonInfo(dc.CreateChunkFactory, null, null, null, dc.DataSetRuntimeSetup, dc.AllowUserProfileState, dc.RequestUserName, dc.Culture, dc.ExecutionTimeStamp, false, false, dc.CreateStreamCallbackForScalability, false, dc.JobContext, null, dc.DataProtection, new ExecutionLogContext(dc.JobContext), dc.DataSources, null, dc.CreateAndSetupDataExtensionFunction, configuration, new AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.DataSourceInfoHashtable(), null, abortHelper, abortInfoInherited, UserProfileState.None, this, Mode.Full, null);
			this.m_errorContext = errorContext;
			this.m_snapshotProcessing = false;
			this.m_isSharedDataSetExecutionOnly = true;
			this.m_catalogItemContext = dc.ItemContext;
			this.m_odpMetadata = null;
			this.m_reportItemsReferenced = false;
			this.m_reportItemThisDotValueReferenced = false;
			this.m_embeddedImages = null;
			this.m_processReportParameters = false;
			this.m_initializedRuntime = false;
			this.m_specialRecursiveAggregates = false;
			this.m_userSortFilterContext = new AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext();
			this.m_inSubreport = false;
			this.m_inSubreportInDataRegion = false;
			this.m_stateManager = new OnDemandStateManagerFull(this);
			this.m_reportObjectModel = new ObjectModelImpl(this);
			this.m_reportObjectModel.Initialize(dataSetDefinition);
			IExpressionHostAssemblyHolder dataSetCore = dataSetDefinition.DataSetCore;
			this.m_reportRuntime = new AspNetCore.ReportingServices.RdlExpressions.ReportRuntime(dataSetCore.ObjectType, this.m_reportObjectModel, this.ErrorContext);
			this.m_reportRuntime.LoadCompiledCode(dataSetCore, true, true, this.m_reportObjectModel, this.ReportRuntimeSetup);
		}

		internal OnDemandProcessingContext(OnDemandProcessingContext aContext, ICatalogItemContext reportContext, AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			this.m_parentContext = aContext;
			this.m_snapshotProcessing = aContext.SnapshotProcessing;
			this.m_reportDefinition = subReport.Report;
			this.m_odpMetadata = aContext.OdpMetadata;
			this.m_inSubreport = true;
			this.m_inSubreportInDataRegion = (aContext.InSubreportInDataRegion | subReport.InDataRegion);
			this.m_processReportParameters = false;
			this.m_isSharedDataSetExecutionOnly = false;
			this.m_initializedRuntime = false;
			this.m_catalogItemContext = reportContext;
			this.m_externalDataSetContext = null;
			this.m_errorContext = new ProcessingErrorContext();
			if (subReport.Report != null)
			{
				this.m_subReportInfo = this.m_odpMetadata.GetSubReportInfo(aContext.InSubreport, subReport.SubReportDefinitionPath, subReport.ReportName);
				int lastID = this.m_subReportInfo.LastID;
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report report = subReport.Report;
			this.m_commonInfo = aContext.m_commonInfo;
			this.m_stateManager = this.CreateStateManager(this.m_commonInfo.ContextMode);
			this.m_subReportInstanceOrSharedDatasetUniqueName = null;
			this.m_reportItemThisDotValueReferenced = aContext.m_reportItemThisDotValueReferenced;
			if (this.m_commonInfo.ContextMode != Mode.DefinitionOnly)
			{
				if (report != null)
				{
					this.m_reportItemsReferenced = report.HasReportItemReferences;
					this.m_embeddedImages = report.EmbeddedImages;
					this.InitializeDataSetMembers(report.MappingNameToDataSet.Count);
				}
				else
				{
					this.m_reportItemsReferenced = false;
					this.m_embeddedImages = null;
					this.InitializeDataSetMembers(-1);
				}
				this.m_reportObjectModel = new ObjectModelImpl(this);
				this.m_reportRuntime = null;
				this.m_userSortFilterContext = new AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.UserSortFilterContext(aContext.UserSortFilterContext, subReport);
			}
			this.m_compareInfo = aContext.m_compareInfo;
			this.m_clrCompareOptions = aContext.m_clrCompareOptions;
			this.m_threadCulture = aContext.m_threadCulture;
			this.InitFlags(report);
			this.m_odpMetadata.OdpContexts.Add(this);
		}

		private OnDemandStateManager CreateStateManager(Mode contextMode)
		{
			switch (contextMode)
			{
			case Mode.Streaming:
				return new OnDemandStateManagerStreaming(this);
			case Mode.Full:
				return new OnDemandStateManagerFull(this);
			case Mode.DefinitionOnly:
				return new OnDemandStateManagerDefinitionOnly(this);
			default:
				Global.Tracer.Assert(false, "CreateStateManager: invalid contextMode.");
				throw new InvalidOperationException("CreateStateManager: invalid contextMode.");
			}
		}

		private static ImageCacheManager CreateImageCacheManager(Mode contextMode, OnDemandMetadata odpMetadata, IChunkFactory chunkFactory)
		{
			switch (contextMode)
			{
			case Mode.Streaming:
				return new StreamingImageCacheManager(odpMetadata, chunkFactory);
			case Mode.Full:
			case Mode.DefinitionOnly:
				return new SnapshotImageCacheManager(odpMetadata, chunkFactory);
			default:
				Global.Tracer.Assert(false, "CreateImageCacheManager: invalid contextMode.");
				throw new InvalidOperationException("CreateImageCacheManager: invalid contextMode.");
			}
		}

		internal void MergeHasUserProfileState(UserProfileState newProfileStateFlags)
		{
			this.m_commonInfo.MergeHasUserProfileState(newProfileStateFlags);
		}

		internal void CreatedScopeInstance(IRIFReportDataScope scope)
		{
			this.m_stateManager.CreatedScopeInstance(scope);
		}

		internal void EnsureCultureIsSetOnCurrentThread()
		{
			if (this.m_threadCulture != null && Thread.CurrentThread.CurrentCulture.LCID != this.m_threadCulture.LCID)
			{
				Thread.CurrentThread.CurrentCulture = this.m_threadCulture;
			}
		}

		internal void SetupEnvironment(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			this.m_currentDataSetIndex = -1;
			this.m_currentReportInstance = reportInstance;
			reportInstance.SetupEnvironment(this);
		}

		internal void SetComparisonInformation(DataSetCore dataSet)
		{
			this.SetComparisonInformation(dataSet.CreateCultureInfoFromLcid().CompareInfo, dataSet.GetCLRCompareOptions(), dataSet.NullsAsBlanks, dataSet.UseOrdinalStringKeyGeneration);
		}

		internal void SetComparisonInformation(CompareInfo compareInfo, CompareOptions clrCompareOptions, bool nullsAsBlanks, bool useOrdinalStringKeyGeneration)
		{
			this.m_compareInfo = compareInfo;
			this.m_clrCompareOptions = clrCompareOptions;
			this.m_nullsAsBlanks = nullsAsBlanks;
			this.m_useOrdinalStringKeyGeneration = useOrdinalStringKeyGeneration;
			this.m_processingComparer = null;
			this.m_stringKeyGenerator = null;
		}

		internal void UnregisterAbortInfo()
		{
			this.m_commonInfo.UnregisterAbortInfo();
		}

		internal bool HasSecondPassOperation(SecondPassOperations op)
		{
			return (this.m_secondPassOperation & op) != SecondPassOperations.None;
		}

		internal void ResetUserSortFilterContext()
		{
			if (this.m_userSortFilterContext != null)
			{
				this.m_userSortFilterContext.ResetContextForTopLevelDataSet();
			}
		}

		internal bool IsRdlSandboxingEnabled()
		{
			if (this.Configuration != null)
			{
				return this.Configuration.RdlSandboxing != null;
			}
			return false;
		}

		internal int GetActiveCompatibilityVersion()
		{
			return ReportProcessingCompatibilityVersion.GetCompatibilityVersion(this.Configuration);
		}

		private void InitFlags(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report)
		{
			if (report != null)
			{
				if (!this.SnapshotProcessing || this.m_commonInfo.ReprocessSnapshot)
				{
					this.m_odpMetadata.ReportSnapshot.DefinitionTreeHasDocumentMap |= report.HasLabels;
					this.m_odpMetadata.ReportSnapshot.HasDocumentMap |= report.HasLabels;
				}
				this.HasBookmarks = report.HasBookmarks;
				this.HasShowHide = (report.ShowHideType == AspNetCore.ReportingServices.ReportIntermediateFormat.Report.ShowHideTypes.Interactive);
			}
		}

		internal void InitializeDataSetMembers(int dataSetCount)
		{
			if (dataSetCount >= 0)
			{
				this.m_dataSetRetrievalComplete = new bool[dataSetCount];
			}
			else
			{
				this.m_dataSetRetrievalComplete = null;
			}
		}

		internal void RuntimeInitializePageSectionVariables(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, object[] reportVariableValues)
		{
			if (report.Variables != null)
			{
				this.AddVariablesToReportObjectModel(report.Variables, null, report.ObjectType, null, reportVariableValues);
			}
			if (report.GroupsWithVariables != null)
			{
				int count = report.GroupsWithVariables.Count;
				for (int i = 0; i < count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = report.GroupsWithVariables[i].Grouping;
					this.AddVariablesToReportObjectModel(grouping.Variables, null, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, null);
				}
			}
		}

		internal void RuntimeInitializeVariables(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report)
		{
			if (report.Variables != null)
			{
				this.AddVariablesToReportObjectModel(report.Variables, (report.ReportExprHost == null) ? null : report.ReportExprHost.VariableValueHosts, report.ObjectType, report.Name, null);
			}
			if (report.GroupsWithVariables != null)
			{
				int count = report.GroupsWithVariables.Count;
				for (int i = 0; i < count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping = report.GroupsWithVariables[i].Grouping;
					this.AddVariablesToReportObjectModel(grouping.Variables, (grouping.ExprHost == null) ? null : grouping.ExprHost.VariableValueHosts, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Grouping, grouping.Name, null);
				}
			}
		}

		private void AddVariablesToReportObjectModel(List<AspNetCore.ReportingServices.ReportIntermediateFormat.Variable> variableDef, IndexedExprHost variableValuesHost, AspNetCore.ReportingServices.ReportProcessing.ObjectType parentObjectType, string parentObjectName, object[] variableValues)
		{
			if (variableDef != null)
			{
				int count = variableDef.Count;
				for (int i = 0; i < count; i++)
				{
					VariableImpl variableImpl = new VariableImpl(variableDef[i], variableValuesHost, parentObjectType, parentObjectName, this.m_reportRuntime, i);
					if (variableValues != null)
					{
						variableImpl.SetValue(variableValues[i], true);
					}
					this.m_reportObjectModel.VariablesImpl.Add(variableImpl);
				}
			}
		}

		internal void RuntimeInitializeLookups(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report)
		{
			if (report.DataSources != null)
			{
				for (int i = 0; i < report.DataSources.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource = report.DataSources[i];
					if (dataSource.DataSets != null)
					{
						for (int j = 0; j < dataSource.DataSets.Count; j++)
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = dataSource.DataSets[j];
							if (dataSet.Lookups != null)
							{
								for (int k = 0; k < dataSet.Lookups.Count; k++)
								{
									LookupInfo lookupInfo = dataSet.Lookups[k];
									LookupImpl lookup = new LookupImpl(lookupInfo, this.m_reportRuntime);
									this.m_reportObjectModel.LookupsImpl.Add(lookup);
								}
							}
						}
					}
				}
			}
		}

		internal void RuntimeInitializeTextboxObjs(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection reportItems, bool setExprHost)
		{
			for (int i = 0; i < reportItems.Count; i++)
			{
				this.RuntimeInitializeTextboxObjs(reportItems[i], setExprHost);
			}
		}

		internal void RuntimeInitializeTextboxObjs(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem, bool setExprHost)
		{
			if (setExprHost && this.m_reportRuntime.ReportExprHost != null)
			{
				reportItem.SetExprHost(this.m_reportRuntime.ReportExprHost, this.m_reportObjectModel);
			}
			switch (reportItem.ObjectType)
			{
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix:
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix tablix = (AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix)reportItem;
				if (tablix.Corner != null && tablix.Corner.Count != 0)
				{
					foreach (List<AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell> item in tablix.Corner)
					{
						foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCornerCell item2 in item)
						{
							if (item2.CellContents != null)
							{
								this.RuntimeInitializeTextboxObjs(item2.CellContents, setExprHost);
								if (item2.AltCellContents != null)
								{
									this.RuntimeInitializeTextboxObjs(item2.AltCellContents, setExprHost);
								}
							}
						}
					}
				}
				if (tablix.Rows != null && tablix.RowCount != 0)
				{
					for (int k = 0; k < tablix.RowCount; k++)
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.TablixRow tablixRow = tablix.TablixRows[k];
						for (int l = 0; l < tablix.ColumnCount; l++)
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.TablixCell tablixCell = tablixRow.TablixCells[l];
							if (tablixCell.CellContents != null)
							{
								this.RuntimeInitializeTextboxObjs(tablixCell.CellContents, setExprHost);
								if (tablixCell.AltCellContents != null)
								{
									this.RuntimeInitializeTextboxObjs(tablixCell.AltCellContents, setExprHost);
								}
							}
						}
					}
				}
				this.RuntimeInitializeTextboxObjsInMemberTree(tablix.ColumnMembers, setExprHost);
				this.RuntimeInitializeTextboxObjsInMemberTree(tablix.RowMembers, setExprHost);
				break;
			}
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle:
				this.RuntimeInitializeTextboxObjs(((AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)reportItem).ReportItems, setExprHost);
				break;
			case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Textbox:
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox textBox = (AspNetCore.ReportingServices.ReportIntermediateFormat.TextBox)reportItem;
				TextBoxImpl textBoxImpl = new TextBoxImpl(textBox, this.m_reportRuntime, this.m_reportRuntime);
				if (setExprHost)
				{
					if (textBox.ValueReferenced)
					{
						Global.Tracer.Assert(textBox.ExprHost != null, "(textBoxDef.ExprHost != null)");
						this.m_reportItemThisDotValueReferenced = true;
						textBox.TextBoxExprHost.SetTextBox(textBoxImpl);
					}
					if (textBox.TextRunValueReferenced)
					{
						for (int i = 0; i < textBox.Paragraphs.Count; i++)
						{
							AspNetCore.ReportingServices.ReportIntermediateFormat.Paragraph paragraph = textBox.Paragraphs[i];
							if (paragraph.TextRunValueReferenced)
							{
								for (int j = 0; j < paragraph.TextRuns.Count; j++)
								{
									AspNetCore.ReportingServices.ReportIntermediateFormat.TextRun textRun = paragraph.TextRuns[j];
									if (textRun.ValueReferenced)
									{
										Global.Tracer.Assert(textRun.ExprHost != null);
										this.m_reportItemThisDotValueReferenced = true;
										textRun.ExprHost.SetTextRun(textBoxImpl.Paragraphs[i].TextRuns[j]);
									}
								}
							}
						}
					}
				}
				this.m_reportObjectModel.ReportItemsImpl.Add(textBoxImpl);
				break;
			}
			}
		}

		private void RuntimeInitializeTextboxObjsInMemberTree(HierarchyNodeList memberNodes, bool setExprHost)
		{
			if (memberNodes != null)
			{
				for (int i = 0; i < memberNodes.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember tablixMember = (AspNetCore.ReportingServices.ReportIntermediateFormat.TablixMember)memberNodes[i];
					if (tablixMember.TablixHeader != null && tablixMember.TablixHeader.CellContents != null)
					{
						this.RuntimeInitializeTextboxObjs(tablixMember.TablixHeader.CellContents, setExprHost);
						if (tablixMember.TablixHeader.AltCellContents != null)
						{
							this.RuntimeInitializeTextboxObjs(tablixMember.TablixHeader.AltCellContents, setExprHost);
						}
					}
					if (tablixMember.InnerHierarchy != null)
					{
						this.RuntimeInitializeTextboxObjsInMemberTree(tablixMember.InnerHierarchy, setExprHost);
					}
				}
			}
		}

		internal void RuntimeInitializeReportItemObjs(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItemCollection reportItems, bool traverseDataRegions)
		{
			for (int i = 0; i < reportItems.Count; i++)
			{
				this.RuntimeInitializeReportItemObjs(reportItems[i], traverseDataRegions);
			}
		}

		internal void RuntimeInitializeReportItemObjs(List<AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion> mapDataRegions, bool traverseDataRegions)
		{
			for (int i = 0; i < mapDataRegions.Count; i++)
			{
				this.RuntimeInitializeReportItemObjs(mapDataRegions[i], traverseDataRegions);
			}
		}

		internal void RuntimeInitializeReportItemObjs(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportItem reportItem, bool traverseDataRegions)
		{
			if (reportItem.IsDataRegion)
			{
				if (traverseDataRegions)
				{
					if (this.m_reportRuntime.ReportExprHost != null)
					{
						reportItem.SetExprHost(this.m_reportRuntime.ReportExprHost, this.m_reportObjectModel);
					}
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = reportItem as AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion;
					dataRegion.DataRegionContentsSetExprHost(this.m_reportObjectModel, traverseDataRegions);
					switch (dataRegion.ObjectType)
					{
					case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Tablix:
						this.RuntimeInitializeMemberTree(dataRegion.ColumnMembers, (dataRegion.ExprHost == null) ? null : ((AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix)dataRegion).TablixExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						this.RuntimeInitializeMemberTree(dataRegion.RowMembers, (dataRegion.ExprHost == null) ? null : ((AspNetCore.ReportingServices.ReportIntermediateFormat.Tablix)dataRegion).TablixExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart:
						this.RuntimeInitializeMemberTree(dataRegion.ColumnMembers, (dataRegion.ExprHost == null) ? null : ((AspNetCore.ReportingServices.ReportIntermediateFormat.Chart)dataRegion).ChartExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						this.RuntimeInitializeMemberTree(dataRegion.RowMembers, (dataRegion.ExprHost == null) ? null : ((AspNetCore.ReportingServices.ReportIntermediateFormat.Chart)dataRegion).ChartExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ObjectType.GaugePanel:
						this.RuntimeInitializeMemberTree(dataRegion.ColumnMembers, (dataRegion.ExprHost == null) ? null : ((AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel)dataRegion).GaugePanelExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						this.RuntimeInitializeMemberTree(dataRegion.RowMembers, (dataRegion.ExprHost == null) ? null : ((AspNetCore.ReportingServices.ReportIntermediateFormat.GaugePanel)dataRegion).GaugePanelExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ObjectType.CustomReportItem:
						this.RuntimeInitializeMemberTree(dataRegion.ColumnMembers, (dataRegion.ExprHost == null) ? null : ((AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem)dataRegion).CustomReportItemExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						this.RuntimeInitializeMemberTree(dataRegion.RowMembers, (dataRegion.ExprHost == null) ? null : ((AspNetCore.ReportingServices.ReportIntermediateFormat.CustomReportItem)dataRegion).CustomReportItemExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						break;
					case AspNetCore.ReportingServices.ReportProcessing.ObjectType.MapDataRegion:
						this.RuntimeInitializeMemberTree(dataRegion.ColumnMembers, (dataRegion.ExprHost == null) ? null : ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion)dataRegion).MapDataRegionExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						this.RuntimeInitializeMemberTree(dataRegion.RowMembers, (dataRegion.ExprHost == null) ? null : ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataRegion)dataRegion).MapDataRegionExprHost.MemberTreeHostsRemotable, traverseDataRegions);
						break;
					}
				}
			}
			else if (reportItem.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Rectangle)
			{
				this.RuntimeInitializeReportItemObjs(((AspNetCore.ReportingServices.ReportIntermediateFormat.Rectangle)reportItem).ReportItems, traverseDataRegions);
			}
			else if (reportItem.ObjectType == AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map && ((AspNetCore.ReportingServices.ReportIntermediateFormat.Map)reportItem).MapDataRegions != null)
			{
				this.RuntimeInitializeReportItemObjs(((AspNetCore.ReportingServices.ReportIntermediateFormat.Map)reportItem).MapDataRegions, traverseDataRegions);
			}
		}

		private void RuntimeInitializeMemberTree(HierarchyNodeList memberNodes, IList<IMemberNode> memberExprHosts, bool traverseDataRegions)
		{
			if (memberNodes != null)
			{
				for (int i = 0; i < memberNodes.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode reportHierarchyNode = memberNodes[i];
					IList<IMemberNode> memberExprHosts2;
					if (reportHierarchyNode.ExprHostID >= 0 && memberExprHosts != null)
					{
						reportHierarchyNode.SetExprHost(memberExprHosts[reportHierarchyNode.ExprHostID], this.m_reportObjectModel);
						memberExprHosts2 = memberExprHosts[reportHierarchyNode.ExprHostID].MemberTreeHostsRemotable;
					}
					else
					{
						memberExprHosts2 = null;
					}
					if (reportHierarchyNode.InnerHierarchy != null && 0 < reportHierarchyNode.InnerHierarchy.Count)
					{
						this.RuntimeInitializeMemberTree(reportHierarchyNode.InnerHierarchy, memberExprHosts2, traverseDataRegions);
					}
					reportHierarchyNode.MemberContentsSetExprHost(this.m_reportObjectModel, traverseDataRegions);
				}
			}
		}

		internal void RuntimeInitializeAggregates<AggregateType>(List<AggregateType> aggregates) where AggregateType : AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo
		{
			if (aggregates != null)
			{
				int count = aggregates.Count;
				for (int i = 0; i < count; i++)
				{
					AggregateType val = aggregates[i];
					if (!this.m_reportAggregates.ContainsKey(val.Name))
					{
						this.m_reportAggregates.Add(val.Name, (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo)(object)val);
						if (val.DuplicateNames != null)
						{
							int count2 = val.DuplicateNames.Count;
							for (int j = 0; j < count2; j++)
							{
								this.m_reportAggregates.Add(val.DuplicateNames[j], (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo)(object)val);
							}
						}
					}
				}
			}
		}

		internal int RecursiveLevel(string scopeName)
		{
			return this.m_stateManager.RecursiveLevel(scopeName);
		}

		internal bool InScope(string scopeName)
		{
			return this.m_stateManager.InScope(scopeName);
		}

		internal Dictionary<string, object> GetCurrentSpecialGroupingValues()
		{
			return this.m_stateManager.GetCurrentSpecialGroupingValues();
		}

		internal IRecordRowReader CreateSequentialDataReader(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, out AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance)
		{
			return this.m_stateManager.CreateSequentialDataReader(dataSet, out dataSetInstance);
		}

		internal bool CalculateAggregate(string aggregateName)
		{
			return this.m_stateManager.CalculateAggregate(aggregateName);
		}

		internal bool CalculateLookup(LookupInfo lookup)
		{
			return this.m_stateManager.CalculateLookup(lookup);
		}

		internal bool PrepareFieldsCollectionForDirectFields()
		{
			return this.m_stateManager.PrepareFieldsCollectionForDirectFields();
		}

		internal void RestoreContext(IInstancePath originalObject)
		{
			this.m_stateManager.RestoreContext(originalObject);
		}

		internal void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance)
		{
			this.m_stateManager.SetupContext(rifObject, romInstance);
		}

		internal void SetupContext(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			this.m_stateManager.SetupContext(rifObject, romInstance, moveNextInstanceIndex);
		}

		internal void BindNextMemberInstance(IInstancePath rifObject, IReportScopeInstance romInstance, int moveNextInstanceIndex)
		{
			this.CheckAndThrowIfAborted();
			this.m_stateManager.BindNextMemberInstance(rifObject, romInstance, moveNextInstanceIndex);
		}

		internal void OnDemandProcessDataPipelineWithRestore(DataSetAggregateDataPipelineManager pipeline)
		{
			FieldsContext currentFields = this.ReportObjectModel.CurrentFields;
			IScalabilityCache tablixProcessingScalabilityCache = this.m_tablixProcessingScalabilityCache;
			this.m_tablixProcessingScalabilityCache = null;
			pipeline.StartProcessing();
			pipeline.StopProcessing();
			this.m_tablixProcessingScalabilityCache = tablixProcessingScalabilityCache;
			if (currentFields != null)
			{
				this.ReportObjectModel.RestoreFields(currentFields);
			}
		}

		internal void SetupEmptyTopLevelFields()
		{
			this.m_reportObjectModel.SetupEmptyTopLevelFields();
			this.m_currentDataSetIndex = -1;
			this.m_currentDataSetInstance = null;
		}

		internal void SetupFieldsForNewDataSetPageSection(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataset)
		{
			this.m_reportObjectModel.SetupPageSectionDataSetFields(dataset);
			this.m_currentDataSetIndex = dataset.IndexInCollection;
			this.m_currentDataSetInstance = null;
		}

		internal void SetupFieldsForNewDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataset, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance, bool addRowIndex, bool noRows)
		{
			this.m_reportObjectModel.SetupFieldsForNewDataSet(dataset, addRowIndex, noRows, false);
			this.m_currentDataSetIndex = dataset.IndexInCollection;
			this.m_currentDataSetInstance = dataSetInstance;
		}

		internal void EnsureRuntimeEnvironmentForDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, bool noRows)
		{
			if (this.m_currentDataSetIndex != dataSet.IndexInCollection)
			{
				dataSet.SetupRuntimeEnvironment(this);
				this.SetupFieldsForNewDataSet(dataSet, null, true, noRows);
			}
		}

		internal void AddSpecialDataRowSort(IReference<IDataRowSortOwner> sortOwner)
		{
			if (this.m_dataRowSortOwners == null)
			{
				this.m_dataRowSortOwners = new List<IReference<IDataRowSortOwner>>();
			}
			this.m_dataRowSortOwners.Add(sortOwner);
		}

		internal void AddSpecialDataRegionFilters(Filters filters)
		{
			if (this.m_specialDataRegionFilters == null)
			{
				this.m_specialDataRegionFilters = new List<Filters>();
			}
			this.m_specialDataRegionFilters.Add(filters);
		}

		private bool ProcessDataRegionsWithSpecialFiltersOrDataRowSorting()
		{
			bool flag = false;
			int num = (this.m_dataRowSortOwners != null) ? this.m_dataRowSortOwners.Count : 0;
			if (this.m_specialDataRegionFilters != null)
			{
				int count = this.m_specialDataRegionFilters.Count;
				for (int i = 0; i < count; i++)
				{
					this.m_specialDataRegionFilters[i].FinishReadingRows();
				}
				this.m_specialDataRegionFilters.RemoveRange(0, count);
				flag |= (this.m_specialDataRegionFilters.Count > 0);
			}
			if (num != 0)
			{
				for (int j = 0; j < num; j++)
				{
					using (this.m_dataRowSortOwners[j].PinValue())
					{
						this.m_dataRowSortOwners[j].Value().DataRowSortTraverse();
					}
				}
				this.m_dataRowSortOwners.RemoveRange(0, num);
				flag |= (this.m_dataRowSortOwners.Count > 0);
			}
			return flag;
		}

		internal bool PopulateRuntimeSortFilterEventInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			return this.m_userSortFilterContext.PopulateRuntimeSortFilterEventInfo(this, dataSet);
		}

		internal bool IsSortFilterTarget(bool[] isSortFilterTarget, IReference<IScope> outerScope, IReference<IHierarchyObj> target, ref AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeUserSortTargetInfo userSortTargetInfo)
		{
			return this.m_userSortFilterContext.IsSortFilterTarget(isSortFilterTarget, outerScope, target, ref userSortTargetInfo);
		}

		internal void ProcessUserSortForTarget(IReference<IHierarchyObj> target, ref ScalableList<DataFieldRow> dataRows, bool targetForNonDetailSort)
		{
			this.m_userSortFilterContext.ProcessUserSortForTarget(this.m_reportObjectModel, this.m_reportRuntime, target, ref dataRows, targetForNonDetailSort);
		}

		internal void RegisterSortFilterExpressionScope(IReference<IScope> container, IReference<RuntimeDataRegionObj> scopeObj, bool[] isSortFilterExpressionScope)
		{
			this.m_userSortFilterContext.RegisterSortFilterExpressionScope(container, scopeObj, isSortFilterExpressionScope);
		}

		internal EventInformation GetUserSortFilterInformation(out string oldUniqueName)
		{
			return this.m_commonInfo.GetUserSortFilterInformation(out oldUniqueName);
		}

		internal void MergeNewUserSortFilterInformation()
		{
			this.m_commonInfo.MergeNewUserSortFilterInformation();
		}

		internal void FirstPassPostProcess()
		{
			while (this.ProcessDataRegionsWithSpecialFiltersOrDataRowSorting())
			{
			}
		}

		internal void ApplyUserSorts()
		{
			while (this.m_userSortFilterContext.ProcessUserSort(this))
			{
			}
		}

		internal List<object>[] GetScopeValues(AspNetCore.ReportingServices.ReportIntermediateFormat.GroupingList containingScopes, IScope containingScope)
		{
			List<object>[] array = null;
			if (containingScopes != null && 0 < containingScopes.Count)
			{
				array = new List<object>[containingScopes.Count];
				int num = 0;
				containingScope.GetScopeValues(null, array, ref num);
			}
			return array;
		}

		internal ProcessingMessageList RegisterComparisonErrorForSortFilterEvent(string propertyName)
		{
			Global.Tracer.Assert(null != this.m_userSortFilterContext.CurrentSortFilterEventSource, "(null != m_userSortFilterContext.CurrentSortFilterEventSource)");
			this.ErrorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, this.m_userSortFilterContext.CurrentSortFilterEventSource.ObjectType, this.m_userSortFilterContext.CurrentSortFilterEventSource.Name, propertyName);
			return this.ErrorContext.Messages;
		}

		internal void CheckAndThrowIfAborted()
		{
			AbortHelper abortInfo = this.m_commonInfo.AbortInfo;
			if (abortInfo != null)
			{
				abortInfo.ThrowIfAborted(CancelationTrigger.ReportProcessing, this.m_subReportInstanceOrSharedDatasetUniqueName);
			}
		}

		internal void AddDataChunkReader(int dataSetIndexInCollection, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataReader)
		{
			if (this.m_dataSetToDataReader == null)
			{
				this.m_dataSetToDataReader = new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader[this.m_reportDefinition.MappingNameToDataSet.Count];
			}
			Global.Tracer.Assert(null == this.m_dataSetToDataReader[dataSetIndexInCollection], "(null == m_dataSetToDataReader[dataSetIndexInCollection])");
			this.m_dataSetToDataReader[dataSetIndexInCollection] = dataReader;
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader GetDataChunkReader(int dataSetIndex)
		{
			if (this.IsPageHeaderFooter)
			{
				return this.m_parentContext.GetDataChunkReader(dataSetIndex);
			}
			if (this.m_dataSetToDataReader != null && this.m_dataSetToDataReader[dataSetIndex] != null)
			{
				return this.m_dataSetToDataReader[dataSetIndex];
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = this.m_reportDefinition.MappingDataSetIndexToDataSet[dataSetIndex];
			string chunkName = default(string);
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = this.GetDataSetInstance(dataSet, out chunkName);
			Global.Tracer.Assert(null != dataSetInstance, "Missing expected DataSetInstance. Report: {0} DataSet: {1} DataSetIndex: {2}", this.m_reportDefinition.Name.MarkAsPrivate(), dataSet.Name.MarkAsModelInfo(), dataSetIndex);
			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader dataChunkReader = new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.DataChunkReader(dataSetInstance, this, chunkName);
			this.AddDataChunkReader(dataSetIndex, dataChunkReader);
			return dataChunkReader;
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance GetDataSetInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			if (this.StreamingMode)
			{
				return null;
			}
			string text = default(string);
			return this.GetDataSetInstance(dataSet, out text);
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance GetDataSetInstance(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, out string chunkName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = null;
			chunkName = null;
			if (dataSet.UsedOnlyInParameters)
			{
				return null;
			}
			chunkName = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.GenerateDataChunkName(this, dataSet.ID, this.m_inSubreport);
			Dictionary<string, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance> dataChunkMap = this.m_odpMetadata.DataChunkMap;
			if (dataChunkMap == null || !dataChunkMap.TryGetValue(chunkName, out dataSetInstance))
			{
				chunkName = AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.GenerateLegacySharedSubReportDataChunkName(this, dataSet.ID);
				if ((dataChunkMap == null || !dataChunkMap.TryGetValue(chunkName, out dataSetInstance)) && this.SnapshotProcessing)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Dataset not found in data chunk map. Name={0}, Chunkname={1}", dataSet.Name.MarkAsPrivate(), chunkName);
				}
			}
			if (dataSetInstance != null && dataSetInstance.DataSetDef == null)
			{
				dataSetInstance.DataSetDef = dataSet;
			}
			return dataSetInstance;
		}

		internal bool[] GenerateDataSetExclusionList(out int unprocessedDataSetCount)
		{
			int num = unprocessedDataSetCount = this.m_reportDefinition.DataSetCount;
			bool[] array = new bool[num];
			for (int i = 0; i < num; i++)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = this.m_reportDefinition.MappingDataSetIndexToDataSet[i];
				if (dataSet.UsedOnlyInParameters)
				{
					array[i] = true;
					unprocessedDataSetCount--;
				}
				else
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance dataSetInstance = this.m_currentReportInstance.GetDataSetInstance(dataSet, this);
					if (dataSetInstance == null || this.IsTablixProcessingComplete(i))
					{
						array[i] = true;
						unprocessedDataSetCount--;
					}
				}
			}
			return array;
		}

		internal void FreeAllResources()
		{
			if (this.m_odpMetadata == null)
			{
				this.FreeResources();
			}
			else
			{
				foreach (OnDemandProcessingContext odpContext in this.m_odpMetadata.OdpContexts)
				{
					odpContext.FreeResources();
				}
			}
		}

		private void FreeResources()
		{
			if (this.m_dataSetToDataReader != null)
			{
				for (int i = 0; i < this.m_dataSetToDataReader.Length; i++)
				{
					if (this.m_dataSetToDataReader[i] != null)
					{
						this.m_dataSetToDataReader[i].Close();
						this.m_dataSetToDataReader[i] = null;
					}
				}
				this.m_dataSetToDataReader = null;
			}
			if (this.m_stateManager != null)
			{
				this.m_stateManager.FreeResources();
			}
			this.EnsureScalabilityCleanup();
		}

		internal void EnsureScalabilitySetup()
		{
			if (this.m_tablixProcessingScalabilityCache == null)
			{
				this.m_tablixProcessingScalabilityCache = ScalabilityUtils.CreateCacheForTransientAllocations(this.CreateStreamCallback, "RGT", StorageObjectCreator.Instance, RuntimeReferenceCreator.Instance, ComponentType.Processing, 5);
				this.ExecutionLogContext.RegisterTablixProcessingScaleCache((!this.m_isSharedDataSetExecutionOnly) ? this.m_reportDefinition.GlobalID : 0);
			}
		}

		internal void EnsureScalabilityCleanup()
		{
			if (this.m_tablixProcessingScalabilityCache != null)
			{
				this.ExecutionLogContext.UnRegisterTablixProcessingScaleCache((!this.m_isSharedDataSetExecutionOnly) ? this.m_reportDefinition.GlobalID : 0, this.m_tablixProcessingScalabilityCache);
				this.m_tablixProcessingScalabilityCache.Dispose();
				this.m_tablixProcessingScalabilityCache = null;
			}
		}

		internal bool IsTablixProcessingComplete(int dataSetIndexInCollection)
		{
			return this.m_odpMetadata.IsTablixProcessingComplete(this, dataSetIndexInCollection);
		}

		internal void SetTablixProcessingComplete(int dataSetIndexInCollection)
		{
			this.m_odpMetadata.SetTablixProcessingComplete(this, dataSetIndexInCollection);
		}

		internal int CreateUniqueID()
		{
			int num = this.m_commonInfo.CreateUniqueID();
			if (this.m_subReportInfo != null)
			{
				this.m_odpMetadata.MetadataHasChanged = true;
				this.m_subReportInfo.LastID = num;
			}
			return num;
		}

		internal bool GetResource(string path, out byte[] resource, out string mimeType, out bool registerInvalidSizeWarning)
		{
			if (this.m_commonInfo.GetResourceCallback != null)
			{
				this.m_commonInfo.ExecutionLogContext.ExternalImageCount += 1L;
				this.m_commonInfo.ExecutionLogContext.StartExternalImageTimer();
				bool flag = default(bool);
				try
				{
					this.m_commonInfo.GetResourceCallback.GetResource(this.m_catalogItemContext, path, out resource, out mimeType, out flag, out registerInvalidSizeWarning);
					if (resource != null)
					{
						this.m_commonInfo.ExecutionLogContext.ExternalImageBytes += resource.LongLength;
					}
				}
				finally
				{
					this.m_commonInfo.ExecutionLogContext.StopExternalImageTimer();
				}
				if (flag)
				{
					this.ErrorContext.Register(ProcessingErrorCode.rsWarningFetchingExternalImages, Severity.Warning, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Report, null, null);
				}
				if (registerInvalidSizeWarning)
				{
					this.TraceOneTimeWarning(ProcessingErrorCode.rsSandboxingExternalResourceExceedsMaximumSize);
				}
				return true;
			}
			resource = null;
			mimeType = null;
			registerInvalidSizeWarning = false;
			return false;
		}

		internal void TraceOneTimeWarning(ProcessingErrorCode errorCode)
		{
			this.m_commonInfo.TraceOneTimeWarning(errorCode, this.m_catalogItemContext);
		}

		internal void LoadExistingSubReportDataChunkNameModifier(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance)
		{
			Global.Tracer.Assert(this.m_subReportInfo != null, "Cannot set DataChunkName modifier if the subreport definition could not be found");
			this.m_subReportDataChunkNameModifier = subReportInstance.GetChunkNameModifier(this.m_subReportInfo, true, false, out this.m_foundExistingSubReportInstance);
		}

		internal void SetSubReportNameModifierAndParameters(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance, bool addEntry)
		{
			Global.Tracer.Assert(this.m_subReportInfo != null, "Cannot set DataChunkName modifier and parameters if the subreport definition could not be found");
			this.m_subReportDataChunkNameModifier = subReportInstance.GetChunkNameModifier(this.m_subReportInfo, false, addEntry, out this.m_foundExistingSubReportInstance);
			if (addEntry && !this.m_foundExistingSubReportInstance)
			{
				this.m_odpMetadata.MetadataHasChanged = true;
			}
			if (this.SnapshotProcessing)
			{
				ParametersImpl parameters = subReportInstance.Parameters;
				if (parameters != null)
				{
					this.m_reportObjectModel.ParametersImpl = parameters;
				}
			}
		}

		internal void SetSharedDataSetUniqueName(string chunkName)
		{
			this.m_subReportInstanceOrSharedDatasetUniqueName = chunkName;
			AbortHelper abortInfo = this.m_commonInfo.AbortInfo;
			if (abortInfo != null)
			{
				abortInfo.AddSubreportInstanceOrSharedDataSet(this.m_subReportInstanceOrSharedDatasetUniqueName);
			}
		}

		internal void SetSubReportContext(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance, bool setupReportOM)
		{
			Global.Tracer.Assert(this.m_subReportInfo != null, "Cannot SetSubReportContext if the subreport definition could not be found");
			string text = this.m_subReportInfo.UniqueName + 'x' + subReportInstance.InstanceUniqueName;
			if (!this.SnapshotProcessing && this.m_reportDefinition != null)
			{
				this.InitializeDataSetMembers(this.m_reportDefinition.MappingNameToDataSet.Count);
			}
			if (this.m_subReportInstanceOrSharedDatasetUniqueName != text)
			{
				this.m_subReportInstanceOrSharedDatasetUniqueName = text;
				AbortHelper abortInfo = this.m_commonInfo.AbortInfo;
				if (abortInfo != null)
				{
					abortInfo.AddSubreportInstanceOrSharedDataSet(this.m_subReportInstanceOrSharedDatasetUniqueName);
				}
				this.ResetDataSetToDataReader();
				if (subReportInstance.ThreadCulture != null)
				{
					this.m_threadCulture = subReportInstance.ThreadCulture;
				}
				this.m_currentReportInstance = subReportInstance.ReportInstance.Value();
				this.m_currentDataSetIndex = -1;
				this.m_stateManager.ResetOnDemandState();
				if (setupReportOM && this.m_reportObjectModel != null)
				{
					this.m_reportObjectModel.SetForNewSubReportContext(subReportInstance.Parameters);
				}
			}
		}

		private void ResetDataSetToDataReader()
		{
			if (this.m_dataSetToDataReader != null && this.m_currentReportInstance != null)
			{
				for (int i = 0; i < this.m_dataSetToDataReader.Length; i++)
				{
					if (this.m_dataSetToDataReader[i] != null)
					{
						((IDisposable)this.m_dataSetToDataReader[i]).Dispose();
						this.m_dataSetToDataReader[i] = null;
					}
				}
			}
		}

		internal static ParameterInfoCollection EvaluateSubReportParameters(OnDemandProcessingContext parentContext, AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport)
		{
			ParameterInfoCollection parameterInfoCollection = new ParameterInfoCollection();
			if (subReport.Parameters != null && subReport.ParametersFromCatalog != null)
			{
				for (int i = 0; i < subReport.Parameters.Count; i++)
				{
					string name = subReport.Parameters[i].Name;
					ParameterInfo parameterInfo = subReport.ParametersFromCatalog[name];
					if (parameterInfo == null)
					{
						throw new UnknownReportParameterException(name);
					}
					parentContext.LastRIFObject = subReport;
					AspNetCore.ReportingServices.RdlExpressions.ParameterValueResult parameterValueResult = parentContext.ReportRuntime.EvaluateParameterValueExpression(subReport.Parameters[i], subReport.ObjectType, subReport.Name, "ParameterValue");
					if (parameterValueResult.ErrorOccurred)
					{
						throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, name);
					}
					object[] array = null;
					object[] array2 = parameterValueResult.Value as object[];
					array = ((array2 == null) ? new object[1]
					{
						parameterValueResult.Value
					} : array2);
					ParameterInfo parameterInfo2 = new ParameterInfo();
					parameterInfo2.Name = name;
					parameterInfo2.Values = array;
					parameterInfo2.DataType = parameterValueResult.Type;
					parameterInfoCollection.Add(parameterInfo2);
				}
			}
			ParameterInfoCollection parameterInfoCollection2 = new ParameterInfoCollection();
			subReport.ParametersFromCatalog.CopyTo(parameterInfoCollection2);
			return ParameterInfoCollection.Combine(parameterInfoCollection2, parameterInfoCollection, true, false, false, false, Localization.ClientPrimaryCulture);
		}

		internal bool StoreUpdatedVariableValue(int index, object value)
		{
			return this.m_odpMetadata.StoreUpdatedVariableValue(this, this.m_currentReportInstance, index, value);
		}

		internal int CompareAndStopOnError(object value1, object value2, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName, bool extendedTypeComparisons)
		{
			try
			{
				bool flag = default(bool);
				return this.ProcessingComparer.Compare(value1, value2, true, extendedTypeComparisons, out flag);
			}
			catch (ReportProcessingException_SpatialTypeComparisonError reportProcessingException_SpatialTypeComparisonError)
			{
				throw new ReportProcessingException(this.RegisterSpatialTypeComparisonError(objectType, objectName, reportProcessingException_SpatialTypeComparisonError.Type));
			}
			catch (ReportProcessingException_ComparisonError e)
			{
				throw new ReportProcessingException(this.RegisterComparisonError(e, objectType, objectName, propertyName));
			}
			catch (CommonDataComparerException e2)
			{
				throw new ReportProcessingException(this.RegisterComparisonError(e2, objectType, objectName, propertyName));
			}
		}

		internal ProcessingMessageList RegisterComparisonError(IDataComparisonError e, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string propertyName)
		{
			if (e == null)
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsComparisonError, Severity.Error, objectType, objectName, propertyName);
			}
			else
			{
				this.m_errorContext.Register(ProcessingErrorCode.rsComparisonTypeError, Severity.Error, objectType, objectName, propertyName, e.TypeX, e.TypeY);
			}
			return this.m_errorContext.Messages;
		}

		internal ProcessingMessageList RegisterSpatialTypeComparisonError(AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string spatialTypeName)
		{
			this.m_errorContext.Register(ProcessingErrorCode.rsCannotCompareSpatialType, Severity.Error, objectType, objectName, spatialTypeName);
			return this.m_errorContext.Messages;
		}

		void IStaticReferenceable.SetID(int id)
		{
			this.m_staticRefId = id;
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.OnDemandProcessingContext;
		}
	}
}
