using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[Serializable]
	internal class HistoryEntry : IComparable, ICloneable
	{
		private DateTime timestamp = DateTime.Now;

		private double value;

		[Browsable(false)]
		public virtual DateTime Timestamp
		{
			get
			{
				return this.timestamp;
			}
			set
			{
				this.timestamp = value;
			}
		}

		[Browsable(false)]
		public virtual double Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		public HistoryEntry()
		{
		}

		public HistoryEntry(DateTime timestamp, double value)
		{
			this.timestamp = timestamp;
			this.value = value;
		}

		public int CompareTo(object obj)
		{
			if (obj is HistoryEntry)
			{
				HistoryEntry historyEntry = (HistoryEntry)obj;
				if (this.Timestamp > historyEntry.Timestamp)
				{
					return 1;
				}
				if (this.Timestamp < historyEntry.Timestamp)
				{
					return -1;
				}
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is HistoryEntry))
			{
				return false;
			}
			return this.Equals((HistoryEntry)obj);
		}

		public bool Equals(HistoryEntry other)
		{
			return this.Timestamp.Equals(other.Timestamp);
		}

		public override int GetHashCode()
		{
			return this.Timestamp.GetHashCode();
		}

		public static bool operator ==(HistoryEntry he1, HistoryEntry he2)
		{
			return he1.Equals(he2);
		}

		public static bool operator !=(HistoryEntry he1, HistoryEntry he2)
		{
			return !(he1 == he2);
		}

		public static bool operator <(HistoryEntry he1, HistoryEntry he2)
		{
			return he1.CompareTo(he2) < 0;
		}

		public static bool operator >(HistoryEntry he1, HistoryEntry he2)
		{
			return he1.CompareTo(he2) > 0;
		}

		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
