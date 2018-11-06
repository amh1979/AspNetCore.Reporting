using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ScopeLookupTable
	{
		private object m_lookupTable;

		internal object LookupTable
		{
			get
			{
				return this.m_lookupTable;
			}
			set
			{
				this.m_lookupTable = value;
			}
		}

		internal void Clear()
		{
			Hashtable hashtable = this.m_lookupTable as Hashtable;
			if (hashtable != null)
			{
				hashtable.Clear();
			}
		}

		internal void Add(GroupingList scopeDefs, VariantList[] scopeValues, int value)
		{
			if (scopeValues == null || scopeValues.Length == 0)
			{
				Global.Tracer.Assert(scopeDefs == null || 0 == scopeDefs.Count, "(null == scopeDefs || 0 == scopeDefs.Count)");
				this.m_lookupTable = value;
			}
			else
			{
				bool flag = true;
				if (this.m_lookupTable == null)
				{
					this.m_lookupTable = new Hashtable();
					flag = false;
				}
				Hashtable hashtable = (Hashtable)this.m_lookupTable;
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < scopeValues.Length; i++)
				{
					VariantList variantList = scopeValues[i];
					if (variantList == null)
					{
						num2++;
					}
					else
					{
						num = variantList.Count;
						if (i == scopeValues.Length - 1)
						{
							num--;
						}
						this.GetNullScopeEntries(num2, ref hashtable, ref flag);
						for (int j = 0; j < num; j++)
						{
							Hashtable hashtable2 = (!flag) ? null : ((Hashtable)hashtable[((ArrayList)variantList)[j]]);
							if (hashtable2 == null)
							{
								hashtable2 = new Hashtable();
								hashtable.Add(((ArrayList)variantList)[j], hashtable2);
								flag = false;
							}
							hashtable = hashtable2;
						}
						num2 = 0;
					}
				}
				object key = 1;
				if (scopeValues[scopeValues.Length - 1] != null)
				{
					key = ((ArrayList)scopeValues[scopeValues.Length - 1])[num];
				}
				else
				{
					this.GetNullScopeEntries(num2, ref hashtable, ref flag);
				}
				Global.Tracer.Assert(!hashtable.Contains(key), "(!hashEntries.Contains(lastKey))");
				hashtable.Add(key, value);
			}
		}

		private void GetNullScopeEntries(int nullScopes, ref Hashtable hashEntries, ref bool lookup)
		{
			Hashtable hashtable = null;
			if (lookup)
			{
				hashtable = (Hashtable)hashEntries[nullScopes];
			}
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				hashEntries.Add(nullScopes, hashtable);
				lookup = false;
			}
			hashEntries = hashtable;
		}

		internal int Lookup(GroupingList scopeDefs, VariantList[] scopeValues)
		{
			object obj = null;
			if (scopeValues == null || scopeValues.Length == 0)
			{
				Global.Tracer.Assert(scopeDefs == null || 0 == scopeDefs.Count, "(null == scopeDefs || 0 == scopeDefs.Count)");
				obj = this.m_lookupTable;
			}
			else
			{
				Hashtable hashtable = (Hashtable)this.m_lookupTable;
				int num = 0;
				for (int i = 0; i < scopeValues.Length; i++)
				{
					VariantList variantList = scopeValues[i];
					if (variantList == null)
					{
						num++;
					}
					else
					{
						hashtable = (Hashtable)hashtable[num];
						for (int j = 0; j < variantList.Count; j++)
						{
							obj = hashtable[((ArrayList)variantList)[j]];
							if (i < scopeValues.Length - 1 || j < variantList.Count - 1)
							{
								hashtable = (Hashtable)obj;
								Global.Tracer.Assert(null != hashtable, "(null != hashEntries)");
							}
						}
						num = 0;
					}
				}
				if (scopeValues[scopeValues.Length - 1] == null)
				{
					hashtable = (Hashtable)hashtable[num];
					obj = hashtable[1];
				}
			}
			Global.Tracer.Assert(obj is int);
			return (int)obj;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.LookupTable, Token.Object));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
