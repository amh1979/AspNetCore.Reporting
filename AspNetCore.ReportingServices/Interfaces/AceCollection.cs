using System;
using System.Collections;

namespace AspNetCore.ReportingServices.Interfaces
{
	[Serializable]
	internal sealed class AceCollection : CollectionBase
	{
		public AceStruct this[int index]
		{
			get
			{
				return (AceStruct)base.InnerList[index];
			}
		}

		public int Add(AceStruct ace)
		{
			return base.InnerList.Add(ace);
		}
	}
}
