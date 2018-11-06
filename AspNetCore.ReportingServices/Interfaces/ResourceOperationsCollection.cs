using System;
using System.Collections;

namespace AspNetCore.ReportingServices.Interfaces
{
	[Serializable]
	internal sealed class ResourceOperationsCollection : CollectionBase
	{
		public ResourceOperation this[int index]
		{
			get
			{
				return (ResourceOperation)base.InnerList[index];
			}
		}

		public int Add(ResourceOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
