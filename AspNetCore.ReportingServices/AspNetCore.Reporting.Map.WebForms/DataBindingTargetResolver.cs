using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class DataBindingTargetResolver
	{
		private StringCollection fieldNames = new StringCollection();

		private Hashtable fieldsCache = new Hashtable();

		private FieldCollection fields;

		private NamedCollection items;

		private bool createNewItemForUnresoved;

		private Type newItemsType;

		public readonly BindingType BindingType;

		private DataBindingTargetResolver(FieldCollection fields)
		{
			this.fields = fields;
			foreach (Field field in this.fields)
			{
				if (field.UniqueIdentifier)
				{
					this.fieldNames.Add(field.Name);
					this.fieldsCache.Add(field.Name, null);
				}
			}
		}

		public DataBindingTargetResolver(FieldCollection fields, ShapeCollection shapes)
			: this(fields)
		{
			this.items = shapes;
			this.BindingType = BindingType.Shapes;
		}

		public DataBindingTargetResolver(FieldCollection fields, SymbolCollection symbols)
			: this(fields)
		{
			this.items = symbols;
			this.createNewItemForUnresoved = true;
			this.newItemsType = typeof(Symbol);
			this.BindingType = BindingType.Symbols;
		}

		public DataBindingTargetResolver(FieldCollection fields, GroupCollection groups)
			: this(fields)
		{
			this.items = groups;
			this.BindingType = BindingType.Groups;
		}

		public DataBindingTargetResolver(FieldCollection fields, PathCollection paths)
			: this(fields)
		{
			this.items = paths;
			this.BindingType = BindingType.Paths;
		}

		public NamedElement GetItemById(object itemID)
		{
			NamedElement namedElement = null;
			if (itemID is string)
			{
				namedElement = this.items.GetByName((string)itemID);
			}
			if (namedElement == null)
			{
				object key = Field.ConvertToSupportedValue(itemID);
				StringEnumerator enumerator = this.fieldNames.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						Hashtable itemsByFiledName = this.GetItemsByFiledName(current);
						namedElement = (itemsByFiledName[key] as NamedElement);
						if (namedElement != null)
						{
							break;
						}
					}
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (namedElement == null && this.createNewItemForUnresoved)
			{
				namedElement = (NamedElement)Activator.CreateInstance(this.newItemsType);
				if (namedElement != null)
				{
					if (itemID is string)
					{
						namedElement.Name = (string)itemID;
					}
					else
					{
						object obj = Field.ConvertToSupportedValue(itemID);
						StringEnumerator enumerator2 = this.fieldNames.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								string current2 = enumerator2.Current;
								if (this.fields[current2].Type == obj.GetType())
								{
									this.SetFieldValue(namedElement, current2, obj);
									break;
								}
							}
						}
						finally
						{
							IDisposable disposable2 = enumerator2 as IDisposable;
							if (disposable2 != null)
							{
								disposable2.Dispose();
							}
						}
					}
					this.AddItem(namedElement);
				}
			}
			return namedElement;
		}

		public NamedElement GetItemByIndex(int index)
		{
			return this.items.GetByIndex(index);
		}

		public bool ContainsField(string name)
		{
			return this.fields.GetIndex(name) >= 0;
		}

		public Field GetFieldByName(string fieldName)
		{
			return this.fields.GetByName(fieldName) as Field;
		}

		public void AddItem(NamedElement item)
		{
			((IList)this.items).Add((object)item);
			StringEnumerator enumerator = this.fieldNames.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					Hashtable hashtable = this.fieldsCache[current] as Hashtable;
					if (hashtable != null)
					{
						PropertyInfo property = item.GetType().GetProperty("Item");
						if (property != null)
						{
							object value = property.GetValue(item, new object[1]
							{
								current
							});
							if (value != null)
							{
								hashtable[value] = item;
							}
						}
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public void AddField(Field field)
		{
			this.fields.Add(field);
		}

		public void SetFieldValue(NamedElement item, string fieldName, object value)
		{
			PropertyInfo property = item.GetType().GetProperty("Item");
			if (property != null)
			{
				property.SetValue(item, value, new object[1]
				{
					fieldName
				});
			}
		}

		private Hashtable GetItemsByFiledName(string fieldName)
		{
			if (!this.fieldNames.Contains(fieldName))
			{
				return null;
			}
			Hashtable hashtable = this.fieldsCache[fieldName] as Hashtable;
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				foreach (NamedElement item in this.items)
				{
					object obj = null;
					if (item is Shape)
					{
						obj = ((Shape)item)[fieldName];
					}
					else if (item is Group)
					{
						obj = ((Group)item)[fieldName];
					}
					else if (item is Symbol)
					{
						obj = ((Symbol)item)[fieldName];
					}
					else if (item is Path)
					{
						obj = ((Path)item)[fieldName];
					}
					if (obj != null)
					{
						hashtable[obj] = item;
					}
				}
				this.fieldsCache[fieldName] = hashtable;
			}
			return hashtable;
		}
	}
}
