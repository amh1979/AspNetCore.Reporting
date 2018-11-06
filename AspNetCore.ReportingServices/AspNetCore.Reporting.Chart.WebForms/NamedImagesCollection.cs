using System;
using System.Collections;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class NamedImagesCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal Chart chart;

		[SRDescription("DescriptionAttributeNamedImagesCollection_Item")]
		public NamedImage this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (NamedImage)this.array[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (NamedImage item in this.array)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					throw new ArgumentException(SR.ExceptionNamedImageNotFound((string)parameter));
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
						throw new ArgumentException(SR.ExceptionNamedImageAddedIsNotUnique(value.Name));
					}
					this.array[(int)parameter] = value;
					return;
				}
				if (parameter is string)
				{
					int num = 0;
					foreach (NamedImage item in this.array)
					{
						if (item.Name == (string)parameter)
						{
							if (index != -1 && index != num)
							{
								throw new ArgumentException(SR.ExceptionNamedImageAddedIsNotUnique(value.Name));
							}
							this.array[num] = value;
							break;
						}
						num++;
					}
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
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

		internal NamedImagesCollection(Chart chart)
		{
			this.chart = chart;
		}

		private NamedImagesCollection()
		{
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

		public NamedImage Add(string name, Image image)
		{
			NamedImage namedImage = new NamedImage(name, image);
			if (this.UniqueName(name))
			{
				namedImage.Name = name;
				this.array.Add(namedImage);
				return namedImage;
			}
			throw new ArgumentException(SR.ExceptionNamedImageAddedIsNotUnique(name));
		}

		public int Add(NamedImage value)
		{
			return this.array.Add(value);
		}

		public int Add(object value)
		{
			if (!(value is NamedImage))
			{
				throw new ArgumentException(SR.ExceptionNamedImageObjectRequired);
			}
			if (((NamedImage)value).Name == "")
			{
				string text = this.CreateName(null);
				if (text == null)
				{
					throw new ArgumentException(SR.ExceptionNamedImageAddedIsNotUnique(text));
				}
				((NamedImage)value).Name = text;
			}
			if (this.GetIndex(((NamedImage)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionNamedImageAddedIsNotUnique(((NamedImage)value).Name));
			}
			return this.array.Add(value);
		}

		public bool Contains(NamedImage value)
		{
			return this.array.Contains(value);
		}

		public int IndexOf(NamedImage value)
		{
			return this.array.IndexOf(value);
		}

		public void Remove(NamedImage value)
		{
			this.array.Remove(value);
		}

		public void Insert(int index, NamedImage value)
		{
			this.Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (value is NamedImage)
			{
				if (!(value is NamedImage))
				{
					throw new ArgumentException(SR.ExceptionNamedImageObjectRequired);
				}
				if (((NamedImage)value).Name == "")
				{
					string text = this.CreateName(null);
					if (text == null)
					{
						throw new ArgumentException(SR.ExceptionNamedImageInsertedIsNotUnique(text));
					}
					((NamedImage)value).Name = text;
				}
				this.array.Insert(index, value);
				return;
			}
			throw new ArgumentException(SR.ExceptionNamedImageInsertedHasWrongType);
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
				string text = "Named Image " + num.ToString(CultureInfo.InvariantCulture);
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
			foreach (NamedImage item in this.array)
			{
				if (item.Name == name)
				{
					return false;
				}
			}
			return true;
		}

		public void RemoveAt(int index)
		{
			this.array.RemoveAt(index);
		}

		public void Remove(object value)
		{
			this.array.Remove(value);
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
		}

		public IEnumerator GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
		}
	}
}
