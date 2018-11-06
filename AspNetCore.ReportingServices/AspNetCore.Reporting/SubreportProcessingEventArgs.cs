using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AspNetCore.Reporting
{
	internal sealed class SubreportProcessingEventArgs : EventArgs
	{
		private string m_subReportName;

		private ReportParameterInfoCollection m_paramMetaData;

		private IList<string> m_dsNames;

		private ReportParameter[] m_userParams = new ReportParameter[0];

		private ReportDataSourceCollection m_dataSources = new ReportDataSourceCollection(new object());

		public string ReportPath
		{
			get
			{
				return this.m_subReportName;
			}
		}

		public ReportParameterInfoCollection Parameters
		{
			get
			{
				return this.m_paramMetaData;
			}
		}

		public IList<string> DataSourceNames
		{
			get
			{
				return this.m_dsNames;
			}
		}

		public ReportDataSourceCollection DataSources
		{
			get
			{
				return this.m_dataSources;
			}
		}

		internal SubreportProcessingEventArgs(string subreportName, ReportParameterInfoCollection paramMetaData, string[] dataSetNames)
		{
			this.m_subReportName = subreportName;
			this.m_paramMetaData = paramMetaData;
			this.m_dsNames = new ReadOnlyCollection<string>(dataSetNames);
		}
	}
}
