using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DrillthroughHashtable : HashtableInstanceInfo
	{
		internal DrillthroughInformation this[string key]
		{
			get
			{
				return (DrillthroughInformation)base.m_hashtable[key];
			}
			set
			{
				base.m_hashtable[key] = value;
			}
		}

		internal DrillthroughHashtable()
		{
		}

		internal DrillthroughHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(string drillthroughId, DrillthroughInformation drillthroughInfo)
		{
			base.m_hashtable.Add(drillthroughId, drillthroughInfo);
		}
	}
}
