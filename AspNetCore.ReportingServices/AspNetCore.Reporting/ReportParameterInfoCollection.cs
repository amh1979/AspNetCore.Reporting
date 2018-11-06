using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace AspNetCore.Reporting
{
	[ComVisible(false)]
	internal sealed class ReportParameterInfoCollection : ReadOnlyCollection<ReportParameterInfo>
	{
		public ReportParameterInfo this[string name]
		{
			get
			{
				foreach (ReportParameterInfo item in this)
				{
					if (string.Compare(item.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return item;
					}
				}
				return null;
			}
		}

		internal ReportParameterInfoCollection(IList<ReportParameterInfo> parameterInfos)
			: base(parameterInfos)
		{
			foreach (ReportParameterInfo item in this)
			{
				item.SetDependencies(this);
			}
		}

		internal ReportParameterInfoCollection()
			: base((IList<ReportParameterInfo>)new ReportParameterInfo[0])
		{
		}
	}
}
