using System;
using System.Collections.ObjectModel;

namespace AspNetCore.Reporting
{
	internal sealed class DataSourceCredentialsCollection : Collection<DataSourceCredentials>
	{
		public DataSourceCredentials this[string name]
		{
			get
			{
				foreach (DataSourceCredentials item in this)
				{
					if (string.Compare(item.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return item;
					}
				}
				return null;
			}
		}
	}
}
