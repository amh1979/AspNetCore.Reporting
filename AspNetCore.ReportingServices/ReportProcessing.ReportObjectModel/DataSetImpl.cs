using System;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class DataSetImpl : DataSet
	{
		private AspNetCore.ReportingServices.ReportProcessing.DataSet m_dataSet;

		public override string CommandText
		{
			get
			{
				return this.m_dataSet.Query.CommandTextValue;
			}
		}

		public override string RewrittenCommandText
		{
			get
			{
				return this.m_dataSet.Query.RewrittenCommandText;
			}
		}

		public override DateTime ExecutionTime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal DataSetImpl(AspNetCore.ReportingServices.ReportProcessing.DataSet dataSetDef)
		{
			this.m_dataSet = dataSetDef;
		}
	}
}
