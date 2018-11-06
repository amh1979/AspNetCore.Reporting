using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeCustomLabelsCollection_CustomLabelsCollection")]
	internal class CustomLabelsCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal Axis axis;

		public CustomLabel this[int index]
		{
			get
			{
				return (CustomLabel)this.array[index];
			}
			set
			{
				this.array[index] = value;
				this.Invalidate();
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.array[index];
			}
			set
			{
				this.array[index] = value;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return this.array.IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.array.IsReadOnly;
			}
		}

		public int Count
		{
			get
			{
				return this.array.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.array.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.array.SyncRoot;
			}
		}

		private CustomLabelsCollection()
		{
		}

		public CustomLabelsCollection(Axis axis)
		{
			this.axis = axis;
		}

		public int Add(double fromPosition, double toPosition, string text)
		{
			CustomLabel customLabel = new CustomLabel(fromPosition, toPosition, text, 0, LabelMark.None);
			customLabel.axis = this.axis;
			this.Invalidate();
			return this.array.Add(customLabel);
		}

		internal int Add(double fromPosition, double toPosition, string text, bool customLabel)
		{
			CustomLabel customLabel2 = new CustomLabel(fromPosition, toPosition, text, 0, LabelMark.None);
			customLabel2.customLabel = customLabel;
			customLabel2.axis = this.axis;
			this.Invalidate();
			return this.array.Add(customLabel2);
		}

		public int Add(double fromPosition, double toPosition, string text, LabelRow row, LabelMark mark)
		{
			CustomLabel customLabel = new CustomLabel(fromPosition, toPosition, text, row, mark);
			customLabel.axis = this.axis;
			this.Invalidate();
			return this.array.Add(customLabel);
		}

		public int Add(double fromPosition, double toPosition, string text, int rowIndex, LabelMark mark)
		{
			CustomLabel customLabel = new CustomLabel(fromPosition, toPosition, text, rowIndex, mark);
			customLabel.axis = this.axis;
			this.Invalidate();
			return this.array.Add(customLabel);
		}

		public int Add(double fromPosition, double toPosition, string text, LabelRow row, LabelMark mark, GridTicks gridTick)
		{
			CustomLabel customLabel = new CustomLabel(fromPosition, toPosition, text, row, mark, gridTick);
			customLabel.axis = this.axis;
			this.Invalidate();
			return this.array.Add(customLabel);
		}

		public int Add(double fromPosition, double toPosition, string text, int rowIndex, LabelMark mark, GridTicks gridTick)
		{
			CustomLabel customLabel = new CustomLabel(fromPosition, toPosition, text, rowIndex, mark, gridTick);
			customLabel.axis = this.axis;
			this.Invalidate();
			return this.array.Add(customLabel);
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType, double min, double max, string format, LabelRow row, LabelMark mark)
		{
			this.Add(labelsStep, intervalType, min, max, format, (row != 0) ? 1 : 0, mark);
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType, double min, double max, string format, int rowIndex, LabelMark mark)
		{
			if (min == 0.0 && max == 0.0 && this.axis != null && !double.IsNaN(this.axis.Minimum) && !double.IsNaN(this.axis.Maximum))
			{
				min = this.axis.Minimum;
				max = this.axis.Maximum;
			}
			double num = Math.Min(min, max);
			double num2 = Math.Max(min, max);
			double num3 = num;
			double num4 = 0.0;
			while (num3 < num2)
			{
				switch (intervalType)
				{
				case DateTimeIntervalType.Number:
					num4 = num3 + labelsStep;
					break;
				case DateTimeIntervalType.Milliseconds:
					num4 = DateTime.FromOADate(num3).AddMilliseconds(labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Seconds:
					num4 = DateTime.FromOADate(num3).AddSeconds(labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Minutes:
					num4 = DateTime.FromOADate(num3).AddMinutes(labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Hours:
					num4 = DateTime.FromOADate(num3).AddHours(labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Days:
					num4 = DateTime.FromOADate(num3).AddDays(labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Weeks:
					num4 = DateTime.FromOADate(num3).AddDays(7.0 * labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Months:
					num4 = DateTime.FromOADate(num3).AddMonths((int)labelsStep).ToOADate();
					break;
				case DateTimeIntervalType.Years:
					num4 = DateTime.FromOADate(num3).AddYears((int)labelsStep).ToOADate();
					break;
				default:
					throw new ArgumentException(SR.ExceptionAxisLabelsIntervalTypeUnsupported(intervalType.ToString()));
				}
				if (num4 > num2)
				{
					num4 = num2;
				}
				ChartValueTypes valueType = ChartValueTypes.Double;
				if (intervalType != DateTimeIntervalType.Number)
				{
					valueType = (ChartValueTypes)((this.axis.GetAxisValuesType() != ChartValueTypes.DateTimeOffset) ? 8 : 11);
				}
				string text = ValueConverter.FormatValue(this.axis.chart, this.axis, num3 + (num4 - num3) / 2.0, format, valueType, ChartElementType.AxisLabels);
				CustomLabel customLabel = new CustomLabel(num3, num4, text, rowIndex, mark);
				customLabel.axis = this.axis;
				this.array.Add(customLabel);
				num3 = num4;
			}
			this.Invalidate();
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType)
		{
			this.Add(labelsStep, intervalType, 0.0, 0.0, "", 0, LabelMark.None);
			this.Invalidate();
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType, string format)
		{
			this.Add(labelsStep, intervalType, 0.0, 0.0, format, 0, LabelMark.None);
			this.Invalidate();
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType, string format, LabelRow row, LabelMark mark)
		{
			this.Add(labelsStep, intervalType, 0.0, 0.0, format, row, mark);
			this.Invalidate();
		}

		public void Add(double labelsStep, DateTimeIntervalType intervalType, string format, int rowIndex, LabelMark mark)
		{
			this.Add(labelsStep, intervalType, 0.0, 0.0, format, rowIndex, mark);
			this.Invalidate();
		}

		public void Clear()
		{
			this.array.Clear();
			this.Invalidate();
		}

		bool IList.Contains(object value)
		{
			return this.array.Contains(value);
		}

		public bool Contains(CustomLabel value)
		{
			return this.array.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return this.array.IndexOf(value);
		}

		public int IndexOf(CustomLabel value)
		{
			return this.array.IndexOf(value);
		}

		void IList.Remove(object value)
		{
			this.array.Remove(value);
			this.Invalidate();
		}

		public void Remove(CustomLabel value)
		{
			this.array.Remove(value);
			this.Invalidate();
		}

		public void RemoveAt(int index)
		{
			this.array.RemoveAt(index);
			this.Invalidate();
		}

		public int Add(object value)
		{
			if (!(value is CustomLabel))
			{
				throw new ArgumentException(SR.ExceptionCustomLabelAddedHasWrongType);
			}
			((CustomLabel)value).axis = this.axis;
			this.Invalidate();
			return this.array.Add(value);
		}

		public void Insert(int index, CustomLabel value)
		{
			this.Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is CustomLabel))
			{
				throw new ArgumentException(SR.ExceptionCustomLabelInsertedHasWrongType);
			}
			((CustomLabel)value).axis = this.axis;
			this.array.Insert(index, value);
			this.Invalidate();
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
			this.Invalidate();
		}

		public IEnumerator GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		private void Invalidate()
		{
		}
	}
}
