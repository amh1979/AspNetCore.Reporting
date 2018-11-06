using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public sealed class DataSourcesImpl : DataSources
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		public override AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSource this[string key]
		{
			get
			{
				if (key != null && this.m_collection != null)
				{
					try
					{
						AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSource dataSource = this.m_collection[key] as AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSource;
						if (dataSource == null)
						{
							throw new ReportProcessingException_NonExistingDataSourceReference(key);
						}
						return dataSource;
					}
					catch
					{
						throw new ReportProcessingException_NonExistingDataSourceReference(key);
					}
				}
				throw new ReportProcessingException_NonExistingDataSourceReference(key);
			}
		}

		internal DataSourcesImpl(int size)
		{
			this.m_lockAdd = (size > 1);
			this.m_collection = new Hashtable(size);
		}

		internal void Add(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSourceDef)
		{
			try
			{
				if (this.m_lockAdd)
				{
					Monitor.Enter(this.m_collection);
				}
				if (!this.m_collection.ContainsKey(dataSourceDef.Name))
				{
					this.m_collection.Add(dataSourceDef.Name, new DataSourceImpl(dataSourceDef));
				}
			}
			finally
			{
				if (this.m_lockAdd)
				{
					Monitor.Exit(this.m_collection);
				}
			}
		}
	}
}
