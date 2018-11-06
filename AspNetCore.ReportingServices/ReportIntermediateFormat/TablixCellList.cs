using System;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class TablixCellList : CellList
	{
		internal new TablixCell this[int index]
		{
			get
			{
				return (TablixCell)base[index];
			}
		}

		public TablixCellList()
		{
		}

		internal TablixCellList(int capacity)
			: base(capacity)
		{
		}
	}
}
