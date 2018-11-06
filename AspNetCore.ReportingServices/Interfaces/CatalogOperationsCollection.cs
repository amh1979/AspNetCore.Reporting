using System;
using System.Collections;

namespace AspNetCore.ReportingServices.Interfaces
{
	[Serializable]
	internal sealed class CatalogOperationsCollection : CollectionBase
	{
		public CatalogOperation this[int index]
		{
			get
			{
				return (CatalogOperation)base.InnerList[index];
			}
		}

		public int Add(CatalogOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
