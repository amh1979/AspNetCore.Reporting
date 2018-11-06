using System;
using System.Collections;

namespace AspNetCore.ReportingServices.Interfaces
{
	[Serializable]
	internal sealed class ModelOperationsCollection : CollectionBase
	{
		public ModelOperation this[int index]
		{
			get
			{
				return (ModelOperation)base.InnerList[index];
			}
		}

		public int Add(ModelOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
