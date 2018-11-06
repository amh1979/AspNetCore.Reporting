using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SenderInformationHashtable : HashtableInstanceInfo
	{
		internal SenderInformation this[int key]
		{
			get
			{
				return (SenderInformation)base.m_hashtable[key];
			}
			set
			{
				base.m_hashtable[key] = value;
			}
		}

		internal SenderInformationHashtable()
		{
		}

		internal SenderInformationHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(int key, SenderInformation sender)
		{
			base.m_hashtable.Add(key, sender);
		}
	}
}
