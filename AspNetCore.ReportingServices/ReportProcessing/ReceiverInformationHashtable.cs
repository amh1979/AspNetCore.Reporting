using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReceiverInformationHashtable : HashtableInstanceInfo
	{
		internal ReceiverInformation this[int key]
		{
			get
			{
				return (ReceiverInformation)base.m_hashtable[key];
			}
			set
			{
				base.m_hashtable[key] = value;
			}
		}

		internal ReceiverInformationHashtable()
		{
		}

		internal ReceiverInformationHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(int key, ReceiverInformation receiver)
		{
			base.m_hashtable.Add(key, receiver);
		}
	}
}
