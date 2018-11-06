using System;
using System.Collections;

namespace AspNetCore.ReportingServices.Interfaces
{
	[Serializable]
	internal sealed class DatasourceOperationsCollection : CollectionBase
	{
		public DatasourceOperation this[int index]
		{
			get
			{
				return (DatasourceOperation)base.InnerList[index];
			}
		}

		public int Add(DatasourceOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
