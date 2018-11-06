using System;
using System.Collections;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeStripLinesCollection_StripLinesCollection")]
	internal class StripLinesCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal Axis axis;

		public StripLine this[int index]
		{
			get
			{
				return (StripLine)this.array[index];
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

		private StripLinesCollection()
		{
		}

		public StripLinesCollection(Axis axis)
		{
			this.axis = axis;
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

		int IList.IndexOf(object value)
		{
			return this.array.IndexOf(value);
		}

		public bool Contains(StripLine value)
		{
			return this.array.Contains(value);
		}

		public int IndexOf(StripLine value)
		{
			return this.array.IndexOf(value);
		}

		public void Remove(StripLine value)
		{
			this.array.Remove(value);
		}

		public void Remove(object value)
		{
			this.array.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this.array.RemoveAt(index);
			this.Invalidate();
		}

		public int Add(StripLine value)
		{
			return this.Add((object)value);
		}

		public int Add(object value)
		{
			if (!(value is StripLine))
			{
				throw new ArgumentException(SR.ExceptionStripLineAddedHasWrongType);
			}
			((StripLine)value).axis = this.axis;
			this.Invalidate();
			return this.array.Add(value);
		}

		public void Insert(int index, StripLine value)
		{
			this.Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is StripLine))
			{
				throw new ArgumentException(SR.ExceptionStripLineAddedHasWrongType);
			}
			((StripLine)value).axis = this.axis;
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
