using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class RuntimeIdcIncrementalDataSet : RuntimeIncrementalDataSet
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow m_currentRow;

		internal RuntimeIdcIncrementalDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, DataSetInstance dataSetInstance, OnDemandProcessingContext odpContext)
			: base(dataSource, dataSet, dataSetInstance, odpContext)
		{
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.RecordRow GetNextRow()
		{
			this.SetupNextRow();
			return this.m_currentRow;
		}

		private bool SetupNextRow()
		{
			try
			{
				int rowIndex = default(int);
				this.m_currentRow = base.ReadOneRow(out rowIndex);
				if (this.m_currentRow == null)
				{
					return false;
				}
				FieldsImpl fieldsImpl = base.m_odpContext.ReportObjectModel.FieldsImpl;
				fieldsImpl.NewRow();
				if (fieldsImpl.AddRowIndex)
				{
					fieldsImpl.SetRowIndex(rowIndex);
				}
				base.m_odpContext.ReportObjectModel.UpdateFieldValues(false, this.m_currentRow, base.m_dataSetInstance, base.HasServerAggregateMetadata);
				return true;
			}
			catch (Exception)
			{
				this.CleanupForException();
				this.FinalCleanup();
				throw;
			}
		}

		protected override void InitializeBeforeProcessingRows(bool aReaderExtensionsSupported)
		{
			Global.Tracer.Assert(base.m_odpContext.ReportObjectModel != null && base.m_odpContext.ReportRuntime != null);
			base.m_odpContext.SetupFieldsForNewDataSet(base.m_dataSet, base.m_dataSetInstance, true, true);
			base.PopulateFieldsWithReaderFlags();
			base.m_dataSet.SetFilterExprHost(base.m_odpContext.ReportObjectModel);
		}

		protected override void ProcessExtendedPropertyMappings()
		{
		}
	}
}
