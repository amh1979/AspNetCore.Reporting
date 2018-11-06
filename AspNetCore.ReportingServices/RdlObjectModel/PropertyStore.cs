using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal class PropertyStore : IPropertyStore
	{
		private ReportObject m_owner;

		private IContainedObject m_parent;

		private Dictionary<int, object> m_objEntries;

		private Dictionary<int, int> m_intEntries;

		private Dictionary<int, bool> m_boolEntries;

		private Dictionary<int, ReportSize> m_sizeEntries;

		public ReportObject Owner
		{
			get
			{
				return this.m_owner;
			}
		}

		public IContainedObject Parent
		{
			get
			{
				return this.m_parent;
			}
			set
			{
				this.m_parent = value;
			}
		}

		public PropertyStore(ReportObject owner)
		{
			this.m_owner = owner;
		}

		internal PropertyStore()
		{
		}

		internal void SetOwner(ReportObject owner)
		{
			this.m_owner = owner;
		}

		public void RemoveProperty(int propertyIndex)
		{
			this.RemoveObject(propertyIndex);
			this.RemoveInteger(propertyIndex);
			this.RemoveBoolean(propertyIndex);
			this.RemoveSize(propertyIndex);
		}

		public object GetObject(int propertyIndex)
		{
			if (this.m_objEntries != null && this.m_objEntries.ContainsKey(propertyIndex))
			{
				return this.m_objEntries[propertyIndex];
			}
			return null;
		}

		public T GetObject<T>(int propertyIndex)
		{
			object @object = this.GetObject(propertyIndex);
			if (@object != null)
			{
				return (T)@object;
			}
			return default(T);
		}

		public void SetObject(int propertyIndex, object value)
		{
			if (this.m_objEntries == null)
			{
				this.m_objEntries = new Dictionary<int, object>();
			}
			if (value is IContainedObject)
			{
				((IContainedObject)value).Parent = this.Owner;
			}
			this.m_objEntries[propertyIndex] = value;
			if (this.m_owner != null)
			{
				this.m_owner.OnSetObject(propertyIndex);
			}
		}

		public void RemoveObject(int propertyIndex)
		{
			if (this.m_objEntries != null)
			{
				this.m_objEntries.Remove(propertyIndex);
			}
		}

		public bool ContainsObject(int propertyIndex)
		{
			if (this.m_objEntries != null)
			{
				return this.m_objEntries.ContainsKey(propertyIndex);
			}
			return false;
		}

		public int GetInteger(int propertyIndex)
		{
			if (this.m_intEntries != null && this.m_intEntries.ContainsKey(propertyIndex))
			{
				return this.m_intEntries[propertyIndex];
			}
			return 0;
		}

		public void SetInteger(int propertyIndex, int value)
		{
			if (this.m_intEntries == null)
			{
				this.m_intEntries = new Dictionary<int, int>();
			}
			this.m_intEntries[propertyIndex] = value;
		}

		public void RemoveInteger(int propertyIndex)
		{
			if (this.m_intEntries != null)
			{
				this.m_intEntries.Remove(propertyIndex);
			}
		}

		public bool ContainsInteger(int propertyIndex)
		{
			if (this.m_intEntries != null)
			{
				return this.m_intEntries.ContainsKey(propertyIndex);
			}
			return false;
		}

		public bool GetBoolean(int propertyIndex)
		{
			if (this.m_boolEntries != null && this.m_boolEntries.ContainsKey(propertyIndex))
			{
				return this.m_boolEntries[propertyIndex];
			}
			return false;
		}

		public void SetBoolean(int propertyIndex, bool value)
		{
			if (this.m_boolEntries == null)
			{
				this.m_boolEntries = new Dictionary<int, bool>();
			}
			this.m_boolEntries[propertyIndex] = value;
		}

		public void RemoveBoolean(int propertyIndex)
		{
			if (this.m_boolEntries != null)
			{
				this.m_boolEntries.Remove(propertyIndex);
			}
		}

		public bool ContainsBoolean(int propertyIndex)
		{
			if (this.m_boolEntries != null)
			{
				return this.m_boolEntries.ContainsKey(propertyIndex);
			}
			return false;
		}

		public ReportSize GetSize(int propertyIndex)
		{
			if (this.m_sizeEntries != null && this.m_sizeEntries.ContainsKey(propertyIndex))
			{
				return this.m_sizeEntries[propertyIndex];
			}
			return default(ReportSize);
		}

		public void SetSize(int propertyIndex, ReportSize value)
		{
			if (this.m_sizeEntries == null)
			{
				this.m_sizeEntries = new Dictionary<int, ReportSize>();
			}
			this.m_sizeEntries[propertyIndex] = value;
		}

		public void RemoveSize(int propertyIndex)
		{
			if (this.m_sizeEntries != null)
			{
				this.m_sizeEntries.Remove(propertyIndex);
			}
		}

		public bool ContainsSize(int propertyIndex)
		{
			if (this.m_sizeEntries != null)
			{
				return this.m_sizeEntries.ContainsKey(propertyIndex);
			}
			return false;
		}

		public void IterateObjectEntries(VisitPropertyObject visitObject)
		{
			if (this.m_objEntries != null)
			{
				foreach (int key in this.m_objEntries.Keys)
				{
					visitObject(key, this.m_objEntries[key]);
				}
			}
		}
	}
}
