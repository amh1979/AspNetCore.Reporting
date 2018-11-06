using System;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class CustomDataRowList : RowList
	{
		internal new CustomDataRow this[int index]
		{
			get
			{
				return (CustomDataRow)base[index];
			}
		}

		public CustomDataRowList()
		{
		}

		internal CustomDataRowList(int capacity)
			: base(capacity)
		{
		}
	}
}
