using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SortFilterEventInfoHashtable : HashtableInstanceInfo
	{
		internal SortFilterEventInfo this[int key]
		{
			get
			{
				return (SortFilterEventInfo)base.m_hashtable[key];
			}
		}

		internal SortFilterEventInfoHashtable()
		{
		}

		internal SortFilterEventInfoHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(int key, SortFilterEventInfo val)
		{
			base.m_hashtable.Add(key, val);
		}
	}
}
