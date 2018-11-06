using System;
using System.Collections;

namespace AspNetCore.ReportingServices.Interfaces
{
	[Serializable]
	internal sealed class ModelItemOperationsCollection : CollectionBase
	{
		public ModelItemOperation this[int index]
		{
			get
			{
				return (ModelItemOperation)base.InnerList[index];
			}
		}

		public int Add(ModelItemOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
