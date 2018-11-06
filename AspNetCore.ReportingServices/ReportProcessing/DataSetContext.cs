using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Interfaces;
using System;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class DataSetContext
	{
		private string m_targetChunkNameInSnapshot;

		private string m_cachedDataChunkName;

		private bool m_mustCreateDataChunk;

		private IRowConsumer m_consumerRequest;

		private ICatalogItemContext m_itemContext;

		private RuntimeDataSourceInfoCollection m_dataSources;

		private string m_requestUserName;

		private DateTime m_executionTimeStamp;

		private ParameterInfoCollection m_parameters;

		private IChunkFactory m_createChunkFactory;

		private ReportProcessing.ExecutionType m_interactiveExecution;

		private CultureInfo m_culture;

		private UserProfileState m_allowUserProfileState;

		private UserProfileState m_initialUserProfileState;

		private IProcessingDataExtensionConnection m_createDataExtensionInstanceFunction;

		private CreateAndRegisterStream m_createStreamCallbackForScalability;

		private ReportRuntimeSetup m_dataSetRuntimeSetup;

		private IJobContext m_jobContext;

		private IDataProtection m_dataProtection;

		public string TargetChunkNameInSnapshot
		{
			get
			{
				return this.m_targetChunkNameInSnapshot;
			}
			set
			{
				this.m_targetChunkNameInSnapshot = value;
			}
		}

		public string CachedDataChunkName
		{
			get
			{
				return this.m_cachedDataChunkName;
			}
			set
			{
				this.m_cachedDataChunkName = value;
			}
		}

		public bool MustCreateDataChunk
		{
			get
			{
				return this.m_mustCreateDataChunk;
			}
			set
			{
				this.m_mustCreateDataChunk = value;
			}
		}

		public IRowConsumer ConsumerRequest
		{
			get
			{
				return this.m_consumerRequest;
			}
		}

		public ICatalogItemContext ItemContext
		{
			get
			{
				return this.m_itemContext;
			}
		}

		public RuntimeDataSourceInfoCollection DataSources
		{
			get
			{
				return this.m_dataSources;
			}
		}

		public string RequestUserName
		{
			get
			{
				return this.m_requestUserName;
			}
		}

		public DateTime ExecutionTimeStamp
		{
			get
			{
				return this.m_executionTimeStamp;
			}
		}

		public ParameterInfoCollection Parameters
		{
			get
			{
				return this.m_parameters;
			}
		}

		public IChunkFactory CreateChunkFactory
		{
			get
			{
				return this.m_createChunkFactory;
			}
			set
			{
				this.m_createChunkFactory = value;
			}
		}

		public ReportProcessing.ExecutionType InteractiveExecution
		{
			get
			{
				return this.m_interactiveExecution;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.m_culture;
			}
		}

		public UserProfileState AllowUserProfileState
		{
			get
			{
				return this.m_allowUserProfileState;
			}
		}

		public UserProfileState InitialUserProfileState
		{
			get
			{
				return this.m_initialUserProfileState;
			}
		}

		public IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction
		{
			get
			{
				return this.m_createDataExtensionInstanceFunction;
			}
		}

		public CreateAndRegisterStream CreateStreamCallbackForScalability
		{
			get
			{
				return this.m_createStreamCallbackForScalability;
			}
		}

		public ReportRuntimeSetup DataSetRuntimeSetup
		{
			get
			{
				return this.m_dataSetRuntimeSetup;
			}
		}

		public IJobContext JobContext
		{
			get
			{
				return this.m_jobContext;
			}
		}

		public IDataProtection DataProtection
		{
			get
			{
				return this.m_dataProtection;
			}
		}

		public DataSetContext(string targetChunkNameInSnapshot, string cachedDataChunkName, bool mustCreateDataChunk, IRowConsumer consumerRequest, ICatalogItemContext itemContext, RuntimeDataSourceInfoCollection dataSources, string requestUserName, DateTime executionTimeStamp, ParameterInfoCollection parameters, IChunkFactory createChunkFactory, ReportProcessing.ExecutionType interactiveExecution, CultureInfo culture, UserProfileState allowUserProfileState, UserProfileState initialUserProfileState, IProcessingDataExtensionConnection createDataExtensionInstanceFunction, CreateAndRegisterStream createStreamCallbackForScalability, ReportRuntimeSetup dataSetRuntimeSetup, IJobContext jobContext, IDataProtection dataProtection)
		{
			this.m_targetChunkNameInSnapshot = targetChunkNameInSnapshot;
			this.m_cachedDataChunkName = cachedDataChunkName;
			this.m_mustCreateDataChunk = mustCreateDataChunk;
			this.m_consumerRequest = consumerRequest;
			this.m_itemContext = itemContext;
			this.m_dataSources = dataSources;
			this.m_requestUserName = requestUserName;
			this.m_executionTimeStamp = executionTimeStamp;
			this.m_parameters = parameters;
			this.m_createChunkFactory = createChunkFactory;
			this.m_interactiveExecution = interactiveExecution;
			this.m_culture = culture;
			this.m_allowUserProfileState = allowUserProfileState;
			this.m_initialUserProfileState = initialUserProfileState;
			this.m_createDataExtensionInstanceFunction = createDataExtensionInstanceFunction;
			this.m_createStreamCallbackForScalability = createStreamCallbackForScalability;
			this.m_dataSetRuntimeSetup = dataSetRuntimeSetup;
			this.m_jobContext = jobContext;
			this.m_dataProtection = dataProtection;
		}
	}
}
