using System;
using System.Collections;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeCustomLabelsCollection_CustomLabelsCollection")]
	internal class LegendItemsCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal CommonElements common;

		internal Legend legend;

		public LegendItem this[int index]
		{
			get
			{
				return (LegendItem)this.array[index];
			}
			set
			{
				value.Legend = this.legend;
				this.array[index] = value;
				this.Invalidate(false);
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

		public int Add(Color color, string text)
		{
			LegendItem legendItem = new LegendItem(text, color, "");
			legendItem.Legend = this.legend;
			if (this.common != null)
			{
				legendItem.common = this.common;
			}
			this.Invalidate(false);
			return this.Add(legendItem);
		}

		public void Insert(int index, Color color, string text)
		{
			LegendItem legendItem = new LegendItem(text, color, "");
			legendItem.Legend = this.legend;
			if (this.common != null)
			{
				legendItem.common = this.common;
			}
			this.Insert(index, legendItem);
			this.Invalidate(false);
		}

		public int Add(string image, string text)
		{
			LegendItem legendItem = new LegendItem(text, Color.Empty, image);
			legendItem.Legend = this.legend;
			if (this.common != null)
			{
				legendItem.common = this.common;
			}
			this.Invalidate(false);
			return this.Add(legendItem);
		}

		public void Insert(int index, string image, string text)
		{
			LegendItem legendItem = new LegendItem(text, Color.Empty, image);
			legendItem.Legend = this.legend;
			if (this.common != null)
			{
				legendItem.common = this.common;
			}
			this.Insert(index, legendItem);
			this.Invalidate(false);
		}

		public void Clear()
		{
			this.array.Clear();
			this.Invalidate(false);
		}

		bool IList.Contains(object value)
		{
			return this.array.Contains(value);
		}

		public bool Contains(LegendItem value)
		{
			return this.array.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return this.array.IndexOf(value);
		}

		public int IndexOf(LegendItem value)
		{
			return this.array.IndexOf(value);
		}

		void IList.Remove(object value)
		{
			this.array.Remove(value);
			this.Invalidate(false);
		}

		public void Remove(LegendItem value)
		{
			this.array.Remove(value);
			this.Invalidate(false);
		}

		public void RemoveAt(int index)
		{
			this.array.RemoveAt(index);
			this.Invalidate(false);
		}

		public int Add(object value)
		{
			if (!(value is LegendItem))
			{
				throw new ArgumentException(SR.ExceptionLegendItemAddedHasWrongType);
			}
			if (this.common != null)
			{
				((LegendItem)value).common = this.common;
			}
			((LegendItem)value).Legend = this.legend;
			this.Invalidate(false);
			return this.array.Add(value);
		}

		public void Insert(int index, LegendItem value)
		{
			this.Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is LegendItem))
			{
				throw new ArgumentException(SR.ExceptionLegendItemInsertedHasWrongType);
			}
			if (this.common != null)
			{
				((LegendItem)value).common = this.common;
			}
			((LegendItem)value).Legend = this.legend;
			this.array.Insert(index, value);
			this.Invalidate(false);
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
			this.Invalidate(false);
		}

		public IEnumerator GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		public void Reverse()
		{
			this.array.Reverse();
			this.Invalidate(false);
		}

		private void Invalidate(bool invalidateLegendOnly)
		{
		}
	}
}
