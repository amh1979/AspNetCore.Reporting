using System;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class TablixRowList : RowList
	{
		internal new TablixRow this[int index]
		{
			get
			{
				return (TablixRow)base[index];
			}
		}

		public TablixRowList()
		{
		}

		internal TablixRowList(int capacity)
			: base(capacity)
		{
		}
	}
}
