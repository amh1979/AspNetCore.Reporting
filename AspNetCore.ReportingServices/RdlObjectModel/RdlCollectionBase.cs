using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal abstract class RdlCollectionBase<T> : Collection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, IContainedObject
	{
		private IContainedObject m_parent;

		[XmlIgnore]
		public IContainedObject Parent
		{
			get
			{
				return this.m_parent;
			}
			set
			{
				this.m_parent = value;
				if (typeof(IContainedObject).IsAssignableFrom(typeof(T)))
				{
					foreach (T item in this)
					{
						IContainedObject containedObject = (IContainedObject)(object)item;
						containedObject.Parent = value;
					}
				}
			}
		}

		object IList.this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				base[index] = (T)value;
			}
		}

		protected override void InsertItem(int index, T item)
		{
			if (((object)item) is IContainedObject)
			{
				((IContainedObject)(object)item).Parent = this.m_parent;
			}
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, T item)
		{
			if (((object)item) is IContainedObject)
			{
				((IContainedObject)(object)item).Parent = this.m_parent;
			}
			base.SetItem(index, item);
		}

		protected RdlCollectionBase()
		{
		}

		protected RdlCollectionBase(IContainedObject parent)
		{
			this.m_parent = parent;
		}

		int IList.Add(object item)
		{
			base.Add((T)item);
			return base.Count - 1;
		}
	}
}
