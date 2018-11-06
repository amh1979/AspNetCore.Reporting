using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataSetInstance
	{
		private readonly DataSet m_dataSetDef;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataSetInstance m_dataSetInstance;

		private RowInstance m_row;

		private IRecordRowReader m_dataReader;

		private IDataComparer m_comparer;

		internal IDataComparer Comparer
		{
			get
			{
				if (this.m_dataSetInstance == null)
				{
					return null;
				}
				if (this.m_comparer == null)
				{
					this.m_comparer = this.m_dataSetInstance.CreateProcessingComparer(this.m_dataSetDef.RenderingContext.OdpContext);
				}
				return this.m_comparer;
			}
		}

		public RowInstance Row
		{
			get
			{
				if (this.m_dataReader == null)
				{
					return null;
				}
				if (this.m_row == null)
				{
					this.m_row = new RowInstance(this.m_dataSetInstance.FieldInfos, this.m_dataReader.RecordRow);
				}
				return this.m_row;
			}
		}

		internal DataSetInstance(DataSet dataSetDef)
		{
			this.m_dataSetDef = dataSetDef;
		}

		public void ResetContext()
		{
			if (this.m_dataReader == null)
			{
				this.CreateDataReader();
			}
			else if (!this.m_dataReader.MoveToFirstRow())
			{
				Global.Tracer.Trace(TraceLevel.Verbose, "OnDemandReportRendering.DataSetInstance triggered a second query execution or second chunk open for dataset: {0} in report {1}", this.m_dataSetDef.Name.MarkAsPrivate(), this.m_dataSetDef.RenderingContext.OdpContext.ReportContext.ItemPathAsString.MarkAsPrivate());
				this.Close();
				this.CreateDataReader();
			}
			this.m_row = null;
		}

		public bool MoveNext()
		{
			if (this.m_dataReader == null)
			{
				return false;
			}
			if (this.m_dataReader.GetNextRow())
			{
				if (this.m_row != null)
				{
					this.m_row.UpdateRecordRow(this.m_dataReader.RecordRow);
				}
				return true;
			}
			return false;
		}

		public void Close()
		{
			if (this.m_dataReader != null)
			{
				this.m_dataReader.Close();
				this.m_dataReader = null;
			}
			this.m_row = null;
		}

		private void CreateDataReader()
		{
			OnDemandProcessingContext odpContext = this.m_dataSetDef.RenderingContext.OdpContext;
			this.m_dataReader = odpContext.CreateSequentialDataReader(this.m_dataSetDef.DataSetDef, out this.m_dataSetInstance);
		}

		internal void SetNewContext()
		{
			this.Close();
		}
	}
}
