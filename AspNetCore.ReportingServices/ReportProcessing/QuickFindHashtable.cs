using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	[HashtableOfReferences]
	internal sealed class QuickFindHashtable : HashtableInstanceInfo
	{
		internal ReportItemInstance this[int key]
		{
			get
			{
				return (ReportItemInstance)base.m_hashtable[key];
			}
		}

		internal QuickFindHashtable()
		{
		}

		internal QuickFindHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(int key, ReportItemInstance val)
		{
			base.m_hashtable.Add(key, val);
		}
	}
}
