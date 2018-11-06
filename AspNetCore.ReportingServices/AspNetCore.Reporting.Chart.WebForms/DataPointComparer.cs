using System;
using System.Collections;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeDataPointComparer_DataPointComparer")]
	internal class DataPointComparer : IComparer
	{
		private Series series;

		private PointsSortOrder sortingOrder;

		private int sortingValueIndex = 1;

		private DataPointComparer()
		{
		}

		public DataPointComparer(Series series, PointsSortOrder order, string sortBy)
		{
			sortBy = sortBy.ToUpper(CultureInfo.InvariantCulture);
			if (string.Compare(sortBy, "X", StringComparison.Ordinal) == 0)
			{
				this.sortingValueIndex = -1;
				goto IL_00b0;
			}
			if (string.Compare(sortBy, "Y", StringComparison.Ordinal) == 0)
			{
				this.sortingValueIndex = 0;
				goto IL_00b0;
			}
			if (string.Compare(sortBy, "AXISLABEL", StringComparison.Ordinal) == 0)
			{
				this.sortingValueIndex = -2;
				goto IL_00b0;
			}
			if (sortBy.Length == 2 && sortBy.StartsWith("Y", StringComparison.Ordinal) && char.IsDigit(sortBy[1]))
			{
				this.sortingValueIndex = int.Parse(sortBy.Substring(1), CultureInfo.InvariantCulture) - 1;
				goto IL_00b0;
			}
			throw new ArgumentException(SR.ExceptionDataPointConverterInvalidSorting, "sortBy");
			IL_00b0:
			if (this.sortingValueIndex > 0 && this.sortingValueIndex >= series.YValuesPerPoint)
			{
				throw new ArgumentException(SR.ExceptionDataPointConverterUnavailableSorting(sortBy, series.YValuesPerPoint.ToString(CultureInfo.InvariantCulture)), "sortBy");
			}
			this.sortingOrder = order;
			this.series = series;
		}

		public int Compare(object point1, object point2)
		{
			int num = -1;
			if (point1 is DataPoint && point2 is DataPoint)
			{
				num = ((this.sortingValueIndex != -1) ? ((this.sortingValueIndex != -2) ? ((DataPoint)point1).YValues[this.sortingValueIndex].CompareTo(((DataPoint)point2).YValues[this.sortingValueIndex]) : string.Compare(((DataPoint)point1).AxisLabel, ((DataPoint)point2).AxisLabel, StringComparison.CurrentCultureIgnoreCase)) : ((DataPoint)point1).XValue.CompareTo(((DataPoint)point2).XValue));
				if (this.sortingOrder == PointsSortOrder.Descending)
				{
					num = -num;
				}
				return num;
			}
			throw new ArgumentException(SR.ExceptionDataPointConverterWrongTypes);
		}
	}
}
