using System.Collections;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal interface IPowerViewDataSourceCollection : IEnumerable
	{
		int Count
		{
			get;
		}

		void AddOrUpdate(string key, DataSourceInfo dsInfo);

		DataSourceInfo GetDataSourceFromKey(string key);
	}
}
