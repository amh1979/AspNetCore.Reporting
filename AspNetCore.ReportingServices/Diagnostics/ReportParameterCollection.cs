using System;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal class ReportParameterCollection : NameValueCollection
	{
		internal ReportParameterCollection(NameValueCollection other)
			: base(other)
		{
		}

		public override int GetHashCode()
		{
			int num = 0;
			StringCollection stringCollection = new StringCollection();
			for (int i = 0; i < this.Count; i++)
			{
				string key = this.GetKey(i);
				stringCollection.Add(key);
				string[] values = this.GetValues(i);
				if (values != null)
				{
					string[] array = values;
					foreach (string text in array)
					{
						if (text != null)
						{
							stringCollection.Add(text);
						}
					}
				}
			}
			StringEnumerator enumerator = stringCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					num ^= current.GetHashCode();
				}
				return num;
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

		public override bool Equals(object obj)
		{
			NameValueCollection nameValueCollection = obj as NameValueCollection;
			if (nameValueCollection == null)
			{
				return false;
			}
			if (this.Count != nameValueCollection.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Count; i++)
			{
				string key = this.GetKey(i);
				if (key != nameValueCollection.GetKey(i))
				{
					return false;
				}
				string[] values = this.GetValues(i);
				string[] values2 = nameValueCollection.GetValues(i);
				if (null == values != (null == values2))
				{
					return false;
				}
				if (values != null)
				{
					if (values.Length != values2.Length)
					{
						return false;
					}
					for (int j = 0; j < values.Length; j++)
					{
						if (values[j] != values2[j])
						{
							return false;
						}
					}
				}
			}
			return true;
		}
	}
}
