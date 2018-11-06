using System;

namespace AspNetCore.Reporting
{
	internal sealed class InitializeDataSourcesEventArgs : EventArgs
	{
		private ReportDataSourceCollection m_dataSources;

		public ReportDataSourceCollection DataSources
		{
			get
			{
				return this.m_dataSources;
			}
		}

		internal InitializeDataSourcesEventArgs(ReportDataSourceCollection dataSources)
		{
			this.m_dataSources = dataSources;
		}
	}
}
