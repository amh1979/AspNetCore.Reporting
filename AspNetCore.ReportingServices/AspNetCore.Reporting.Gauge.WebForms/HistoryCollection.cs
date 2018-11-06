using System;
using System.Collections;
using System.Data;
using System.Globalization;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[Serializable]
	internal class HistoryCollection : CollectionBase, ICloneable
	{
		private long truncatedTicks;

		private double accumulatedValue;

		private ValueBase parent;

		public HistoryEntry this[int index]
		{
			get
			{
				return (HistoryEntry)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		internal HistoryEntry Top
		{
			get
			{
				return this[base.Count - 1];
			}
		}

		internal double AccumulatedValue
		{
			get
			{
				if (base.Count > 0 && this.truncatedTicks > 0)
				{
					return this.accumulatedValue + this[0].Value * (double)(this[0].Timestamp.Ticks - this.truncatedTicks);
				}
				return this.accumulatedValue;
			}
		}

		public HistoryCollection(ValueBase parent)
		{
			this.parent = parent;
		}

		protected override void OnClear()
		{
			while (base.Count > 0)
			{
				base.RemoveAt(0);
			}
			base.OnClear();
		}

		protected override void OnRemove(int index, object value)
		{
			if (index == 0)
			{
				this.accumulatedValue += ((HistoryEntry)value).Value * (double)(((HistoryEntry)value).Timestamp.Ticks - this.truncatedTicks);
				this.truncatedTicks = ((HistoryEntry)value).Timestamp.Ticks;
			}
		}

		public void Add(DateTime timestamp, double value)
		{
			base.List.Add(new HistoryEntry(timestamp, value));
			if (base.Count > 1 && this[base.Count - 1].Timestamp < this[base.Count - 2].Timestamp)
			{
				lock (this)
				{
					base.InnerList.Sort();
				}
			}
		}

		public void LoadEntries(HistoryCollection sourceHistory)
		{
			if (sourceHistory == null)
			{
				throw new ApplicationException(Utils.SRGetStr("ExceptionHistoryCannotNull"));
			}
			if (this.parent != null)
			{
				this.parent.IntState = ValueState.DataLoading;
				this.parent.Reset();
				foreach (HistoryEntry item in sourceHistory)
				{
					this.parent.SetValueInternal(item.Value, item.Timestamp);
				}
				this.parent.IntState = ValueState.Interactive;
				this.parent.Invalidate();
			}
			else
			{
				foreach (HistoryEntry item2 in sourceHistory)
				{
					base.InnerList.Add(item2.Clone());
				}
			}
		}

		internal void Truncate(GaugeDuration d)
		{
			if (!d.IsInfinity)
			{
				if (d.IsEmpty)
				{
					lock (this)
					{
						base.Clear();
					}
				}
				else if (d.IsCountBased)
				{
					if ((double)base.Count > d.Count)
					{
						lock (this)
						{
							while ((double)base.Count > d.Count)
							{
								base.RemoveAt(0);
							}
						}
					}
				}
				else
				{
					DateTime timestamp = this.Top.Timestamp;
					DateTime t = timestamp - d.ToTimeSpan();
					if (this[0].Timestamp < t)
					{
						lock (this)
						{
							while (this[0].Timestamp < t)
							{
								base.RemoveAt(0);
							}
						}
					}
				}
			}
		}

		internal int Locate(DateTime timestamp)
		{
			return this.SearchInternal(timestamp, false);
		}

		internal int SearchInternal(DateTime timestamp, bool exact)
		{
			int num = base.InnerList.BinarySearch(new HistoryEntry(timestamp, 0.0));
			if (num < 0 && !exact)
			{
				num = ~num;
			}
			return Math.Max(-1, num);
		}

		internal HistoryEntry[] Select()
		{
			return this.Select(0);
		}

		internal HistoryEntry[] Select(DateTime fromDate, DateTime toDate)
		{
			return this.Select(this.Locate(fromDate), this.Locate(toDate));
		}

		internal HistoryEntry[] Select(int fromPoint)
		{
			return this.Select(fromPoint, base.Count);
		}

		internal HistoryEntry[] Select(int fromPoint, int toPoint)
		{
			if (base.Count != 0 && fromPoint >= 0 && toPoint >= fromPoint)
			{
				lock (this)
				{
					int num = Math.Min(base.Count, toPoint + 1) - fromPoint;
					HistoryEntry[] array = new HistoryEntry[num];
					base.InnerList.CopyTo(fromPoint, array, 0, array.Length);
					return array;
				}
			}
			return new HistoryEntry[0];
		}

		internal HistoryEntry[] Select(GaugeDuration duration, DateTime currentDate)
		{
			if (!duration.IsInfinity)
			{
				if (duration.IsTimeBased)
				{
					DateTime fromDate = currentDate - duration.ToTimeSpan();
					return this.Select(fromDate, currentDate);
				}
				return this.Select((int)duration.Count);
			}
			return this.Select();
		}

		internal DataTable ToDataTable()
		{
			return this.ToDataTable(base.Count);
		}

		internal DataTable ToDataTable(DateTime toPoint)
		{
			return this.ToDataTable(base.Count - this.Locate(toPoint));
		}

		internal DataTable ToDataTable(int toPoint)
		{
			DataTable dataTable = new DataTable();
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add(new DataColumn("DateStamp", typeof(DateTime)));
			dataTable.Columns.Add(new DataColumn("Value", typeof(double)));
			lock (this)
			{
				toPoint = Math.Min(base.Count, toPoint);
				for (int i = base.Count - toPoint; i < base.Count; i++)
				{
					dataTable.Rows.Add(this[i].Timestamp, this[i].Value);
				}
				return dataTable;
			}
		}

		public object Clone()
		{
			HistoryCollection historyCollection = new HistoryCollection(this.parent);
			foreach (HistoryEntry item in this)
			{
				historyCollection.InnerList.Add(item.Clone());
			}
			return historyCollection;
		}

		public int Add(HistoryEntry value)
		{
			return base.List.Add(value);
		}

		public void Remove(HistoryEntry value)
		{
			base.List.Remove(value);
		}

		public bool Contains(HistoryEntry value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, HistoryEntry value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(HistoryEntry value)
		{
			return base.List.IndexOf(value);
		}
	}
}
