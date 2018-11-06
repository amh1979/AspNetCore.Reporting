using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal abstract class RuntimeIncrementalDataSetWithProcessingController : RuntimeIncrementalDataSet
	{
		protected DataProcessingController m_dataProcessingController;

		internal IOnDemandScopeInstance GroupTreeRoot
		{
			get
			{
				return this.m_dataProcessingController.GroupTreeRoot;
			}
		}

		public RuntimeIncrementalDataSetWithProcessingController(DataSource dataSource, DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			: base(dataSource, dataSet, dataSetInstance, odpContext)
		{
		}

		protected override void InitializeDataSet()
		{
			base.InitializeDataSet();
			base.m_odpContext.SetComparisonInformation(base.m_dataSet.DataSetCore);
		}

		protected override void TeardownDataSet()
		{
			base.TeardownDataSet();
			this.CleanupController();
		}

		protected override void FinalCleanup()
		{
			base.FinalCleanup();
			if (base.m_dataSetInstance != null)
			{
				base.m_odpContext.SetTablixProcessingComplete(base.m_dataSet.IndexInCollection);
			}
		}

		private void CleanupController()
		{
			if (this.m_dataProcessingController != null)
			{
				this.m_dataProcessingController.TeardownDataProcessing();
			}
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			this.m_dataProcessingController = new DataProcessingController(base.m_odpContext, base.m_dataSet, base.m_dataSetInstance);
			base.PopulateFieldsWithReaderFlags();
			base.m_odpContext.ClrCompareOptions = base.m_dataSet.GetCLRCompareOptions();
			this.m_dataProcessingController.InitializeDataProcessing();
		}

		protected override void ProcessExtendedPropertyMappings()
		{
		}
	}
}
