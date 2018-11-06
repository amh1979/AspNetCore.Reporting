using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RecordSetPropertyNamesList : ArrayList
	{
		internal new RecordSetPropertyNames this[int index]
		{
			get
			{
				return (RecordSetPropertyNames)base[index];
			}
		}

		internal RecordSetPropertyNamesList()
		{
		}

		internal RecordSetPropertyNamesList(int capacity)
			: base(capacity)
		{
		}

		internal StringList GetPropertyNames(int aliasIndex)
		{
			if (aliasIndex >= 0 && aliasIndex < this.Count)
			{
				return this[aliasIndex].PropertyNames;
			}
			return null;
		}

		internal string GetPropertyName(int aliasIndex, int propertyIndex)
		{
			StringList propertyNames = this.GetPropertyNames(aliasIndex);
			if (propertyNames != null && propertyIndex >= 0 && propertyIndex < propertyNames.Count)
			{
				return propertyNames[propertyIndex];
			}
			return null;
		}
	}
}
