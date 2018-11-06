using System;
using System.Collections;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class EmbeddedImageHashtable : Hashtable
	{
		internal ImageInfo this[string index]
		{
			get
			{
				return (ImageInfo)base[index];
			}
		}

		internal EmbeddedImageHashtable()
		{
		}

		internal EmbeddedImageHashtable(int capacity)
			: base(capacity)
		{
		}

		private EmbeddedImageHashtable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
