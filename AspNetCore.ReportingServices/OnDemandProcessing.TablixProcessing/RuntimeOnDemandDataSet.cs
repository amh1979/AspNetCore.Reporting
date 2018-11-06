using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RuntimeOnDemandDataSet : RuntimePrefetchDataSet
	{
		private bool? m_originalTablixProcessingMode;

		private bool m_processFromLiveDataReader;

		private readonly bool m_generateGroupTree;

		private DataProcessingController m_dataProcessingController;

		internal IOnDemandScopeInstance GroupTreeRoot
		{
			get
			{
				return this.m_dataProcessingController.GroupTreeRoot;
			}
		}

		internal override bool ProcessFromLiveDataReader
		{
			get
			{
				return this.m_processFromLiveDataReader;
			}
		}

		public RuntimeOnDemandDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext, bool processFromLiveDataReader, bool generateGroupTree, bool canWriteDataChunk)
			: base(dataSource, dataSet, dataSetInstance, odpContext, canWriteDataChunk, true)
		{
			this.m_processFromLiveDataReader = processFromLiveDataReader;
			this.m_generateGroupTree = generateGroupTree;
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			this.m_dataProcessingController = new DataProcessingController(base.m_odpContext, base.m_dataSet, base.m_dataSetInstance);
			if (this.m_processFromLiveDataReader)
			{
				base.InitializeBeforeProcessingRows(aReaderExtensionsSupported);
				base.m_odpContext.ClrCompareOptions = base.m_dataSet.GetCLRCompareOptions();
			}
			else
			{
				Global.Tracer.Assert(null == base.m_dataReader, "(null == m_dataReader)");
				if (!base.m_dataSetInstance.NoRows)
				{
					base.m_dataReader = new ProcessingDataReader(base.m_dataSetInstance, base.m_dataSet, base.m_odpContext, false);
				}
			}
			base.PopulateFieldsWithReaderFlags();
			this.m_dataProcessingController.InitializeDataProcessing();
		}

		protected override void InitializeRowSourceAndProcessRows(ExecutedQuery existingQuery)
		{
			if (this.m_processFromLiveDataReader)
			{
				base.InitializeRowSourceAndProcessRows(existingQuery);
			}
			else
			{
				this.InitializeBeforeProcessingRows(false);
				base.m_odpContext.CheckAndThrowIfAborted();
				base.ProcessRows();
			}
		}

		protected override void InitializeDataSet()
		{
			base.InitializeDataSet();
			this.m_originalTablixProcessingMode = base.m_odpContext.IsTablixProcessingMode;
			base.m_odpContext.IsTablixProcessingMode = true;
			base.m_odpContext.SetComparisonInformation(base.m_dataSet.DataSetCore);
		}

		protected override void CleanupDataReader()
		{
			if (this.m_processFromLiveDataReader)
			{
				base.CleanupDataReader();
			}
		}

		protected override void FinalCleanup()
		{
			base.FinalCleanup();
			if (this.m_generateGroupTree)
			{
				this.CleanupController();
			}
			if (this.m_originalTablixProcessingMode.HasValue)
			{
				base.m_odpContext.IsTablixProcessingMode = this.m_originalTablixProcessingMode.Value;
			}
			if (base.m_dataSetInstance != null)
			{
				base.m_odpContext.SetTablixProcessingComplete(base.m_dataSet.IndexInCollection);
			}
		}

		protected override void AllRowsRead()
		{
			base.AllRowsRead();
			this.m_dataProcessingController.AllRowsRead();
			if (this.m_generateGroupTree)
			{
				this.m_dataProcessingController.GenerateGroupTree();
			}
		}

		protected override void CleanupForException()
		{
			base.CleanupForException();
			this.CleanupController();
		}

		private void CleanupController()
		{
			if (this.m_dataProcessingController != null)
			{
				this.m_dataProcessingController.TeardownDataProcessing();
			}
		}

		protected override void ProcessRow(AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow row, int rowNumber)
		{
			if (this.m_processFromLiveDataReader && !base.m_dataSet.IsReferenceToSharedDataSet)
			{
				base.ProcessRow(row, rowNumber);
			}
			this.m_dataProcessingController.NextRow(row, rowNumber, this.m_processFromLiveDataReader && !base.m_canWriteDataChunk, base.HasServerAggregateMetadata);
		}

		protected override void ProcessExtendedPropertyMappings()
		{
			if (this.m_processFromLiveDataReader)
			{
				base.ProcessExtendedPropertyMappings();
			}
		}

		protected override void CleanupProcess()
		{
			if (this.m_processFromLiveDataReader)
			{
				base.CleanupProcess();
			}
		}

		internal override void EraseDataChunk()
		{
			if (this.m_processFromLiveDataReader)
			{
				base.EraseDataChunk();
			}
		}
	}
}
