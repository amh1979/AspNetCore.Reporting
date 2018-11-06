using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class ZIndexComparer : IComparer<RPLItemMeasurement>, IComparer<Border>
	{
		public int Compare(RPLItemMeasurement x, RPLItemMeasurement y)
		{
			return ZIndexComparer.Compare(x.ZIndex, y.ZIndex);
		}

		public int Compare(Border x, Border y)
		{
			int num;
			if (x.CompareRowFirst || y.CompareRowFirst)
			{
				num = ZIndexComparer.Compare(x.RowZIndex, y.RowZIndex);
				if (num != 0)
				{
					return num;
				}
				num = ZIndexComparer.Compare(x.ColumnZIndex, y.ColumnZIndex);
				if (num != 0)
				{
					return num;
				}
			}
			else
			{
				num = ZIndexComparer.Compare(x.ColumnZIndex, y.ColumnZIndex);
				if (num != 0)
				{
					return num;
				}
				num = ZIndexComparer.Compare(x.RowZIndex, y.RowZIndex);
				if (num != 0)
				{
					return num;
				}
			}
			num = ZIndexComparer.Compare(x.RowIndex, y.RowIndex);
			if (num != 0)
			{
				return num;
			}
			num = ZIndexComparer.Compare(x.ColumnIndex, y.ColumnIndex);
			if (num != 0)
			{
				return num;
			}
			return 0;
		}

		public static int Compare(int x, int y)
		{
			if (x == y)
			{
				return 0;
			}
			if (x < y)
			{
				return -1;
			}
			return 1;
		}
	}
}
