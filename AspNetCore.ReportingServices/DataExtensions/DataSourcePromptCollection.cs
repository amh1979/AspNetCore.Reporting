using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSourcePromptCollection
	{
		private Hashtable m_collection = new Hashtable();

		private bool m_needPrompt;

		public bool NeedPrompt
		{
			get
			{
				return this.m_needPrompt;
			}
		}

		public int Count
		{
			get
			{
				return this.m_collection.Count;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.m_collection.Values.GetEnumerator();
		}

		internal void Add(DataSourceInfo dataSource, ServerDataSourceSettings serverDatasourceSettings)
		{
			string originalName = dataSource.OriginalName;
			Global.Tracer.Assert(this.m_collection[originalName] == null, "Collection already contains this data source.");
			dataSource.ThrowIfNotUsable(serverDatasourceSettings);
			this.m_collection.Add(originalName, dataSource);
			if (dataSource.NeedPrompt)
			{
				this.m_needPrompt = true;
			}
		}

		public void AddSingleIfPrompt(DataSourceInfo dataSource, ServerDataSourceSettings serverDatasourceSettings)
		{
			Global.Tracer.Assert(dataSource.OriginalName == null, "Data source has non-null name when adding single");
			if (this.m_collection.Count != 0)
			{
				throw new InternalCatalogException("Prompt collection is not empty when adding single data source");
			}
			if (dataSource.CredentialsRetrieval == DataSourceInfo.CredentialsRetrievalOption.Prompt)
			{
				dataSource.ThrowIfNotUsable(serverDatasourceSettings);
				this.m_collection.Add("", dataSource);
				if (dataSource.NeedPrompt)
				{
					this.m_needPrompt = true;
				}
			}
		}
	}
}
