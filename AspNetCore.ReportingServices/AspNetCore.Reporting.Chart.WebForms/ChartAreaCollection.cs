using System;
using System.Collections;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ChartAreaCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		private CommonElements common;

		private Chart chart;

		[SRDescription("DescriptionAttributeChartAreaCollection_Item")]
		public ChartArea this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (ChartArea)this.array[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (ChartArea item in this.array)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					if (this.chart == null && this.common != null)
					{
						this.chart = (Chart)this.common.container.GetService(typeof(Chart));
					}
					if ((string)parameter == "Default" && this.array.Count > 0 && this.chart != null && !this.chart.serializing)
					{
						return (ChartArea)this.array[0];
					}
					throw new ArgumentException(SR.ExceptionChartAreaAlreadyExistsInCollection((string)parameter));
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
						throw new ArgumentException(SR.ExceptionChartAreaAddedIsNotUnique(value.Name));
					}
					this.array[(int)parameter] = value;
					goto IL_00d6;
				}
				if (parameter is string)
				{
					int num = 0;
					foreach (ChartArea item in this.array)
					{
						if (item.Name == (string)parameter)
						{
							if (index != -1 && index != num)
							{
								throw new ArgumentException(SR.ExceptionChartAreaAddedIsNotUnique(value.Name));
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

		public bool IsReadOnly
		{
			get
			{
				return this.array.IsReadOnly;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return this.array.IsFixedSize;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.array.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this.array.Count;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.array.SyncRoot;
			}
		}

		internal ChartAreaCollection(CommonElements common)
		{
			this.common = common;
			common.chartAreaCollection = this;
		}

		private ChartAreaCollection()
		{
		}

		public int GetIndex(string name)
		{
			int result = -1;
			int num = 0;
			while (num < this.array.Count)
			{
				if (!(this[num].Name == name))
				{
					num++;
					continue;
				}
				result = num;
				break;
			}
			return result;
		}

		public ChartArea Add(string name)
		{
			ChartArea chartArea = new ChartArea();
			if (this.UniqueName(name))
			{
				chartArea.Name = name;
				chartArea.SetCommon(this.common);
				this.array.Add(chartArea);
				this.Invalidate();
				return chartArea;
			}
			throw new ArgumentException(SR.ExceptionChartAreaAddedIsNotUnique(name));
		}

		public int Add(object value)
		{
			if (!(value is ChartArea))
			{
				throw new ArgumentException(SR.ExceptionChartAreaObjectRequired);
			}
			if (((ChartArea)value).Name.Length == 0)
			{
				string text = this.CreateName(null);
				if (text == null)
				{
					throw new ArgumentException(SR.ExceptionChartAreaAddedIsNotUnique(text));
				}
				((ChartArea)value).Name = text;
			}
			if (this.GetIndex(((ChartArea)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionChartAreaAddedIsNotUnique(((ChartArea)value).Name));
			}
			((ChartArea)value).SetCommon(this.common);
			this.Invalidate();
			return this.array.Add(value);
		}

		public void Insert(int index, ChartArea value)
		{
			this.Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (value is ChartArea)
			{
				if (!(value is ChartArea))
				{
					throw new ArgumentException(SR.ExceptionChartAreaObjectRequired);
				}
				if (((ChartArea)value).Name.Length == 0)
				{
					string text = this.CreateName(null);
					if (text == null)
					{
						throw new ArgumentException(SR.ExceptionChartAreaInsertedIsNotUnique(text));
					}
					((ChartArea)value).Name = text;
				}
				((ChartArea)value).SetCommon(this.common);
				this.array.Insert(index, value);
				this.Invalidate();
				return;
			}
			throw new ArgumentException(SR.ExceptionChartAreaInsertedHasWrongType);
		}

		private string CreateName(string Name)
		{
			if (Name != null && this.UniqueName(Name))
			{
				return Name;
			}
			int num = 1;
			while (num < 2147483647)
			{
				string text = "Chart Area " + num.ToString(CultureInfo.InvariantCulture);
				num++;
				if (this.UniqueName(text))
				{
					return text;
				}
			}
			return null;
		}

		private bool UniqueName(string name)
		{
			foreach (ChartArea item in this.array)
			{
				if (item.Name == name)
				{
					return false;
				}
			}
			return true;
		}

		public bool Contains(ChartArea value)
		{
			return this.array.Contains(value);
		}

		public int IndexOf(ChartArea value)
		{
			return this.array.IndexOf(value);
		}

		public int IndexOf(string name)
		{
			int num = 0;
			foreach (ChartArea item in this.array)
			{
				if (item.Name == name)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public void Remove(ChartArea value)
		{
			this.array.Remove(value);
		}

		private void Invalidate()
		{
		}

		public void RemoveAt(int index)
		{
			this.array.RemoveAt(index);
			this.Invalidate();
		}

		public void Remove(object value)
		{
			this.array.Remove(value);
			this.Invalidate();
		}

		public int IndexOf(object value)
		{
			return this.array.IndexOf(value);
		}

		public bool Contains(object value)
		{
			return this.array.Contains(value);
		}

		public void Clear()
		{
			this.array.Clear();
			this.Invalidate();
		}

		public IEnumerator GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
			this.Invalidate();
		}
	}
}
