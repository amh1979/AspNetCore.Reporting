using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class DataSourceImpl : AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSource
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource m_dataSource;

		public override string DataSourceReference
		{
			get
			{
				if (this.m_dataSource.SharedDataSourceReferencePath == null)
				{
					return this.m_dataSource.DataSourceReference;
				}
				return this.m_dataSource.SharedDataSourceReferencePath;
			}
		}

		public override string Type
		{
			get
			{
				return this.m_dataSource.Type;
			}
		}

		internal DataSourceImpl(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSourceDef)
		{
			this.m_dataSource = dataSourceDef;
		}
	}
}
