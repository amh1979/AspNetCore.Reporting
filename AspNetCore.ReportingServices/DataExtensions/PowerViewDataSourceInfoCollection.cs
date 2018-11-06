using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal sealed class PowerViewDataSourceInfoCollection : IPowerViewDataSourceCollection, IEnumerable
	{
		private Dictionary<string, DataSourceInfo> m_dataSourceInfos = new Dictionary<string, DataSourceInfo>();

		public int Count
		{
			get
			{
				return this.m_dataSourceInfos.Count;
			}
		}

		public void AddOrUpdate(string key, DataSourceInfo dsInfo)
		{
			RSTrace.ProcessingTracer.Assert(key != null, "PowerViewDataSourceInfoCollection.AddOrUpdate: key != null");
			if (this.m_dataSourceInfos.ContainsKey(key))
			{
				this.m_dataSourceInfos.Remove(key);
			}
			this.m_dataSourceInfos.Add(key, dsInfo);
		}

		public IEnumerator GetEnumerator()
		{
			return (IEnumerator)(object)this.m_dataSourceInfos.Values.GetEnumerator();
		}

		public DataSourceInfo GetDataSourceFromKey(string key)
		{
			DataSourceInfo result = default(DataSourceInfo);
			this.m_dataSourceInfos.TryGetValue(key, out result);
			return result;
		}
	}
}
