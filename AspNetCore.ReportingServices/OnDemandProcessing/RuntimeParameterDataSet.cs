using AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal class RuntimeParameterDataSet : RuntimeAtomicDataSet, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner
	{
		private ReportParameterDataSetCache m_parameterDataSetObj;

		protected bool m_mustEvaluateThroughReportObjectModel;

		private Filters m_filters;

		public RuntimeParameterDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext processingContext, bool mustEvaluateThroughReportObjectModel, ReportParameterDataSetCache aCache)
			: base(dataSource, dataSet, dataSetInstance, processingContext, true)
		{
			this.m_parameterDataSetObj = aCache;
			this.m_mustEvaluateThroughReportObjectModel = mustEvaluateThroughReportObjectModel;
		}

		protected override void ProcessRow(AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow row, int rowNumber)
		{
			FieldsImpl fieldsImpl = base.m_odpContext.ReportObjectModel.FieldsImpl;
			fieldsImpl.NewRow();
			base.m_odpContext.ReportObjectModel.UpdateFieldValues(false, row, base.m_dataSetInstance, base.HasServerAggregateMetadata);
			bool flag = true;
			if (this.m_filters != null)
			{
				flag = this.m_filters.PassFilters(new DataFieldRow(base.m_odpContext.ReportObjectModel.FieldsImpl, false));
			}
			if (flag)
			{
				this.PostFilterNextRow();
			}
		}

		protected override void ProcessExtendedPropertyMappings()
		{
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			Global.Tracer.Assert(base.m_odpContext.ReportObjectModel != null && base.m_odpContext.ReportRuntime != null);
			base.m_odpContext.SetupFieldsForNewDataSet(base.m_dataSet, base.m_dataSetInstance, false, true);
			base.m_dataSet.SetFilterExprHost(base.m_odpContext.ReportObjectModel);
			base.m_dataSet.SetupRuntimeEnvironment(base.m_odpContext);
			if (base.m_dataSet.Filters != null)
			{
				this.m_filters = new Filters(Filters.FilterTypes.DataSetFilter, this, base.m_dataSet.Filters, base.m_dataSet.ObjectType, base.m_dataSet.Name, base.m_odpContext, 0);
			}
		}

		protected override void AllRowsRead()
		{
			if (this.m_filters != null)
			{
				this.m_filters.FinishReadingRows();
			}
		}

		protected override void FinalCleanup()
		{
			base.FinalCleanup();
			base.m_odpContext.EnsureScalabilityCleanup();
		}

		public virtual void PostFilterNextRow()
		{
			if (this.m_parameterDataSetObj != null)
			{
				this.m_parameterDataSetObj.NextRow(base.m_odpContext.ReportObjectModel.FieldsImpl.GetAndSaveFields());
			}
		}
	}
}
