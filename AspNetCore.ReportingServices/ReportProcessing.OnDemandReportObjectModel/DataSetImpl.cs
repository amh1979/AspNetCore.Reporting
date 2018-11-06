using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class DataSetImpl : AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSet
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		private DataSetInstance m_dataSetInstance;

		private DateTime m_dataSetExecutionTime;

		public override string CommandText
		{
			get
			{
				string text = null;
				if (this.m_dataSetInstance != null)
				{
					text = this.m_dataSetInstance.CommandText;
				}
				if (text == null && this.m_dataSet.Query != null && this.m_dataSet.Query.CommandText != null && !this.m_dataSet.Query.CommandText.IsExpression && this.m_dataSet.Query.CommandText.Value != null)
				{
					text = this.m_dataSet.Query.CommandText.Value.ToString();
				}
				return text;
			}
		}

		public override string RewrittenCommandText
		{
			get
			{
				string result = null;
				if (this.m_dataSetInstance != null)
				{
					result = this.m_dataSetInstance.RewrittenCommandText;
				}
				return result;
			}
		}

		public override DateTime ExecutionTime
		{
			get
			{
				return this.m_dataSetExecutionTime;
			}
		}

		internal DataSetImpl(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSetDef, DataSetInstance dataSetInstance, DateTime reportExecutionTime)
		{
			this.m_dataSet = dataSetDef;
			this.Update(dataSetInstance, reportExecutionTime);
		}

		internal void Update(DataSetInstance dataSetInstance, DateTime reportExecutionTime)
		{
			this.m_dataSetInstance = dataSetInstance;
			if (dataSetInstance != null)
			{
				this.m_dataSetExecutionTime = dataSetInstance.GetQueryExecutionTime(reportExecutionTime);
			}
			else
			{
				this.m_dataSetExecutionTime = reportExecutionTime;
			}
		}
	}
}
