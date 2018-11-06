using System;
using System.Collections;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class StyleAttributeHashtable : Hashtable
	{
		internal AttributeInfo this[string index]
		{
			get
			{
				return (AttributeInfo)base[index];
			}
		}

		internal StyleAttributeHashtable()
		{
		}

		internal StyleAttributeHashtable(int capacity)
			: base(capacity)
		{
		}

		private StyleAttributeHashtable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
