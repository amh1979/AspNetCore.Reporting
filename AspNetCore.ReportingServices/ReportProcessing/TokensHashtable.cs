using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TokensHashtable : HashtableInstanceInfo
	{
		internal object this[int key]
		{
			get
			{
				return base.m_hashtable[key];
			}
			set
			{
				base.m_hashtable[key] = value;
			}
		}

		internal TokensHashtable()
		{
		}

		internal TokensHashtable(int capacity)
			: base(capacity)
		{
		}

		internal void Add(int tokenID, object tokenValue)
		{
			base.m_hashtable.Add(tokenID, tokenValue);
		}
	}
}
