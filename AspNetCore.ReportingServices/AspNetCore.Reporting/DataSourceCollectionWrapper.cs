using System.Collections;

namespace AspNetCore.Reporting
{
	internal sealed class DataSourceCollectionWrapper : IEnumerable
	{
		private readonly ReportDataSourceCollection m_dsCollection;

		internal DataSourceCollectionWrapper(ReportDataSourceCollection dsCollection)
		{
			this.m_dsCollection = dsCollection;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (ReportDataSource item in this.m_dsCollection)
			{
				yield return (object)new DataSourceWrapper(item);
			}
		}
	}
}
