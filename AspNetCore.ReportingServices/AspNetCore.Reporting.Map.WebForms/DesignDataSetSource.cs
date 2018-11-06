using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class DesignDataSetSource : DataSet, IDesignTimeDataSource, IDisposable
	{
		public DesignDataSetSource(MapCore mapCore, object originalDataSource)
		{
			StringCollection dataSourceDataMembers = DataBindingHelper.GetDataSourceDataMembers(originalDataSource);
			StringEnumerator enumerator = dataSourceDataMembers.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					DataTable dataTable = new DataTable(current)
					{
						Locale = CultureInfo.CurrentCulture
					};
					DataBindingHelper.InitDesignDataTable(originalDataSource, current, dataTable);
					base.Tables.Add(dataTable);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
