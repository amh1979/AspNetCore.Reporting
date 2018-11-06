using System.Collections;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class DataSourcesImpl : DataSources
	{
		internal const string Name = "DataSources";

		internal const string FullName = "AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel.DataSources";

		private bool m_lockAdd;

		private Hashtable m_collection;

		public override DataSource this[string key]
		{
			get
			{
				if (key != null && this.m_collection != null)
				{
					try
					{
						DataSource dataSource = this.m_collection[key] as DataSource;
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

		internal void Add(AspNetCore.ReportingServices.ReportProcessing.DataSource dataSourceDef)
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
