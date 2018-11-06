using System;
using System.Collections.ObjectModel;

namespace AspNetCore.Reporting
{
	internal sealed class ReportParameterCollection : Collection<ReportParameter>
	{
		public ReportParameter this[string name]
		{
			get
			{
				foreach (ReportParameter item in this)
				{
					string name1 = item.Name;
					if (string.Compare(name1, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return item;
					}
				}
				return null;
			}
		}
	}
}
