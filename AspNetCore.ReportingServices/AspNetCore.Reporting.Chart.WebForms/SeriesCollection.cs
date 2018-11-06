using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeSeriesCollection_SeriesCollection")]
	internal class SeriesCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		private IServiceContainer serviceContainer;

		[SRDescription("DescriptionAttributeSeriesCollection_Item")]
		public Series this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (Series)this.array[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (Series item in this.array)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					throw new ArgumentException(SR.ExceptionDataSeriesNameNotFound((string)parameter));
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
			set
			{
				int index = this.GetIndex(value.Name);
				if (parameter is int)
				{
					if (index != -1 && index != (int)parameter)
					{
						throw new ArgumentException(SR.ExceptionSeriesNameAddedIsNotUnique(value.Name));
					}
					this.array[(int)parameter] = value;
					goto IL_00d6;
				}
				if (parameter is string)
				{
					int num = 0;
					foreach (Series item in this.array)
					{
						if (item.Name == (string)parameter)
						{
							if (index != -1 && index != num)
							{
								throw new ArgumentException(SR.ExceptionSeriesNameAddedIsNotUnique(value.Name));
							}
							this.array[num] = value;
							break;
						}
						num++;
					}
					goto IL_00d6;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
				IL_00d6:
				this.Invalidate("");
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
				this.Invalidate("");
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

		private SeriesCollection()
		{
		}

		public SeriesCollection(IServiceContainer container)
		{
			this.serviceContainer = container;
		}

		public int GetIndex(string name)
		{
			int result = -1;
			int num = 0;
			while (num < this.array.Count)
			{
				if (string.Compare(this[num].Name, name, StringComparison.Ordinal) != 0)
				{
					num++;
					continue;
				}
				result = num;
				break;
			}
			return result;
		}

		public Series Add(string name)
		{
			Series series = new Series(name);
			this.Add(series);
			this.Invalidate(series.ChartArea);
			return series;
		}

		public Series Add(string name, int yValuesPerPoint)
		{
			Series series = new Series(name, yValuesPerPoint);
			this.Add(series);
			this.Invalidate(series.ChartArea);
			return series;
		}

		public void Clear()
		{
			this.array.Clear();
			this.Invalidate("");
		}

		bool IList.Contains(object value)
		{
			return this.array.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return this.array.IndexOf(value);
		}

		void IList.Remove(object value)
		{
			this.array.Remove(value);
			this.Invalidate("");
		}

		public bool Contains(Series value)
		{
			return this.array.Contains(value);
		}

		public int IndexOf(Series value)
		{
			return this.array.IndexOf(value);
		}

		public void Remove(Series value)
		{
			this.array.Remove(value);
			this.Invalidate("");
		}

		public void RemoveAt(int index)
		{
			this.array.RemoveAt(index);
			this.Invalidate("");
		}

		public int Add(object value)
		{
			if (!(value is Series))
			{
				throw new ArgumentException(SR.ExceptionDataSeriesObjectRequired);
			}
			if (((Series)value).Name.Length == 0)
			{
				int num = this.array.Count + 1;
				((Series)value).Name = "Series" + num.ToString(CultureInfo.InvariantCulture);
				while (this.GetIndex(((Series)value).Name) != -1)
				{
					num++;
					((Series)value).Name = "Series" + num.ToString(CultureInfo.InvariantCulture);
				}
			}
			if (this.GetIndex(((Series)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionSeriesNameAddedIsNotUnique(((Series)value).Name));
			}
			((Series)value).serviceContainer = this.serviceContainer;
			this.Invalidate(((Series)value).ChartArea);
			return this.array.Add(value);
		}

		public void Insert(int index, Series value)
		{
			this.Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is Series))
			{
				throw new ArgumentException(SR.ExceptionDataSeriesObjectRequired);
			}
			if (((Series)value).Name.Length == 0)
			{
				((Series)value).Name = "Series" + (this.array.Count + 1).ToString(CultureInfo.InvariantCulture);
			}
			if (this.GetIndex(((Series)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionSeriesNameAddedIsNotUnique(((Series)value).Name));
			}
			((Series)value).serviceContainer = this.serviceContainer;
			this.array.Insert(index, value);
			this.Invalidate(((Series)value).ChartArea);
		}

		public void CopyTo(Array array, int index)
		{
			((Series[])this.array.ToArray(typeof(Series))).CopyTo(array, index);
			this.Invalidate("");
		}

		public IEnumerator GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		private void Invalidate(string chartArea)
		{
		}
	}
}
