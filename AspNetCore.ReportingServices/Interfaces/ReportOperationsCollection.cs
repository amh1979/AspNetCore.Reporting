using System;
using System.Collections;

namespace AspNetCore.ReportingServices.Interfaces
{
	[Serializable]
	internal sealed class ReportOperationsCollection : CollectionBase
	{
		public ReportOperation this[int index]
		{
			get
			{
				return (ReportOperation)base.InnerList[index];
			}
		}

		public int Add(ReportOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
