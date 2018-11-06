namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class DataSourceImpl : DataSource
	{
		private AspNetCore.ReportingServices.ReportProcessing.DataSource m_dataSource;

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

		internal DataSourceImpl(AspNetCore.ReportingServices.ReportProcessing.DataSource dataSourceDef)
		{
			this.m_dataSource = dataSourceDef;
		}
	}
}
