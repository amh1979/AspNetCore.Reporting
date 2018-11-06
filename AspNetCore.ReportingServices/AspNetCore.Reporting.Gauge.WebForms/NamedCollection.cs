using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[ListBindable(false)]
	internal abstract class NamedCollection : CollectionBase, IDisposable, ICloneable
	{
		internal CommonElements common;

		internal Type elementType = typeof(NamedElement);

		internal NamedElement parent;

		internal bool editModeActive;

		private bool disposed;

		public NamedElement ParentElement
		{
			get
			{
				return this.parent;
			}
		}

		internal CommonElements Common
		{
			get
			{
				return this.common;
			}
			set
			{
				this.common = value;
				foreach (NamedElement item in this)
				{
					item.Common = this.common;
				}
			}
		}

		private NamedCollection()
		{
		}

		internal NamedCollection(NamedElement parent, CommonElements common)
		{
			this.Common = common;
			this.parent = parent;
		}

		public int GetIndex(string name)
		{
			int result = -1;
			int num = 0;
			while (num < base.List.Count)
			{
				if (string.Compare(((NamedElement)base.List[num]).Name, name, false, CultureInfo.CurrentCulture) != 0)
				{
					num++;
					continue;
				}
				result = num;
				break;
			}
			return result;
		}

		public NamedElement GetByName(string name)
		{
			int index = this.GetIndex(name);
			if (index != -1)
			{
				return (NamedElement)base.List[index];
			}
			return null;
		}

		public NamedElement GetByIndex(int index)
		{
			return (NamedElement)base.List[index];
		}

		public virtual int IndexOf(object value)
		{
			return base.List.IndexOf(value);
		}

		protected override void OnClear()
		{
			while (base.Count > 0)
			{
				base.RemoveAt(0);
			}
			base.OnClear();
		}

		protected override void OnInsert(int index, object value)
		{
			base.OnInsert(index, value);
			this.CheckForTypeDublicatesAndName(value);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			((NamedElement)value).Collection = this;
			((NamedElement)value).Common = this.common;
			this.Invalidate();
			base.OnInsertComplete(index, value);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			base.OnRemoveComplete(index, value);
			if (!this.editModeActive)
			{
				((NamedElement)value).Common = null;
				((NamedElement)value).Collection = null;
			}
			this.Invalidate();
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			base.OnSet(index, oldValue, newValue);
			if (oldValue != newValue)
			{
				this.IsValidNameCheck(((NamedElement)newValue).Name, (NamedElement)oldValue);
			}
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			((NamedElement)oldValue).Common = null;
			((NamedElement)oldValue).Collection = null;
			((NamedElement)newValue).Collection = this;
			((NamedElement)newValue).Common = this.common;
			this.Invalidate();
			base.OnSetComplete(index, oldValue, newValue);
		}

		private void CheckForTypeDublicatesAndName(object value)
		{
			if (!this.IsCorrectType(value))
			{
				throw new NotSupportedException(Utils.SRGetStr("ExceptionInvalidObjectType", this.GetCollectionName(), this.elementType.Name));
			}
			if (value is int && base.List.IndexOf(value) != -1)
			{
				throw new NotSupportedException(Utils.SRGetStr("ExceptionDuplicateObjectFailed", this.GetCollectionName()));
			}
			NamedElement namedElement = (NamedElement)value;
			if (namedElement.Name != null && !(namedElement.Name == string.Empty))
			{
				if (this.IsUniqueName(namedElement.Name))
				{
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionDuplicateNameFailed"));
			}
			if (base.Count == 0)
			{
				namedElement.Name = this.GetDefaultElementName(namedElement);
			}
			else
			{
				namedElement.Name = this.GenerateUniqueName(namedElement);
			}
		}

		internal virtual void IsValidNameCheck(string name, NamedElement element)
		{
			if (name != null && !(name == string.Empty))
			{
				NamedElement byName = this.GetByName(name);
				if (byName == null)
				{
					return;
				}
				if (byName == element)
				{
					return;
				}
				throw new NotSupportedException(Utils.SRGetStr("ExceptionDuplicateNameFailed"));
			}
			throw new ArgumentException(Utils.SRGetStr("ExceptionEmptyNameFailed", this.elementType.Name));
		}

		internal virtual bool IsUniqueName(string name)
		{
			return this.GetIndex(name) == -1;
		}

		internal string GenerateUniqueName(NamedElement element)
		{
			string elementNameFormat = this.GetElementNameFormat(element);
			for (int i = base.Count + 1; i < 2147483647; i++)
			{
				string text = string.Format(CultureInfo.InvariantCulture, elementNameFormat, i);
				if (this.IsUniqueName(text))
				{
					return text;
				}
			}
			throw new ApplicationException(Utils.SRGetStr("ExceptionGenerateNameFailed"));
		}

		internal NamedElement GetByNameCheck(string name)
		{
			NamedElement byName = this.GetByName(name);
			if (byName == null)
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionElementNotFound", this.elementType.Name, name, base.GetType().Name));
			}
			return byName;
		}

		internal bool SetByName(string name, NamedElement element)
		{
			int index = this.GetIndex(name);
			if (index != -1)
			{
				base.List[index] = element;
				return true;
			}
			return false;
		}

		internal void SetByNameCheck(string name, NamedElement element)
		{
			if (this.SetByName(name, element))
			{
				return;
			}
			throw new ArgumentException(Utils.SRGetStr("ExceptionElementNotFound", this.elementType.Name, name, base.GetType().Name));
		}

		internal virtual bool IsCorrectType(object value)
		{
			return this.elementType.IsInstanceOfType(value);
		}

		internal virtual void BeginInit()
		{
			foreach (NamedElement item in this)
			{
				item.BeginInit();
			}
		}

		internal virtual void EndInit()
		{
			foreach (NamedElement item in this)
			{
				item.EndInit();
			}
		}

		internal virtual void ReconnectData(bool exact)
		{
			foreach (NamedElement item in this)
			{
				item.ReconnectData(exact);
			}
		}

		internal virtual string GetElementNameFormat(NamedElement el)
		{
			string text = el.DefaultName;
			if (text == string.Empty)
			{
				text = el.GetType().Name;
			}
			return text + "{0}";
		}

		internal virtual string GetDefaultElementName(NamedElement el)
		{
			return "Default";
		}

		internal virtual void Invalidate()
		{
			if (this.Common != null)
			{
				this.Common.GaugeCore.Invalidate();
			}
		}

		internal virtual string GetCollectionName()
		{
			return base.GetType().Name.Replace("Collection", "s");
		}

		internal virtual void Notify(MessageType msg, NamedElement element, object param)
		{
			foreach (NamedElement item in this)
			{
				item.Notify(msg, element, param);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
				this.OnDispose();
			}
			this.disposed = true;
		}

		protected virtual void OnDispose()
		{
			foreach (NamedElement item in this)
			{
				item.Dispose();
			}
		}

		public virtual object Clone()
		{
			ConstructorInfo[] constructors = base.GetType().GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
			NamedCollection namedCollection = (NamedCollection)constructors[0].Invoke(new object[2]
			{
				this.parent,
				this.common
			});
			namedCollection.parent = this.parent;
			namedCollection.common = this.common;
			namedCollection.elementType = this.elementType;
			foreach (ICloneable item in this)
			{
				NamedElement namedElement = (NamedElement)item.Clone();
				namedCollection.InnerList.Add(namedElement);
				namedElement.collection = namedCollection;
			}
			return namedCollection;
		}
	}
}
