using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class DomainScopeContext
	{
		internal class DomainScopeInfo
		{
			private int m_currentKeyIndex = -1;

			private int m_keyCount;

			private object[] m_keys;

			internal DataFieldRow m_currentRow;

			internal object CurrentKey
			{
				get
				{
					return this.m_keys[this.m_currentKeyIndex];
				}
			}

			internal DataFieldRow CurrentRow
			{
				get
				{
					return this.m_currentRow;
				}
				set
				{
					this.m_currentRow = value;
				}
			}

			internal void InitializeKeys(int count)
			{
				this.m_keys = new object[count];
				this.m_keyCount = 0;
				this.m_currentKeyIndex = -1;
			}

			internal void AddKey(object key)
			{
				this.m_keys[this.m_keyCount++] = key;
			}

			internal void RemoveKey()
			{
				this.m_keyCount--;
			}

			internal void MoveNext()
			{
				this.m_currentKeyIndex++;
			}

			internal void MovePrevious()
			{
				this.m_currentKeyIndex--;
			}
		}

		private Dictionary<int, IReference<RuntimeGroupRootObj>> m_domainScopes = new Dictionary<int, IReference<RuntimeGroupRootObj>>();

		private DomainScopeInfo m_currentDomainScopeInfo;

		internal DomainScopeInfo CurrentDomainScope
		{
			get
			{
				return this.m_currentDomainScopeInfo;
			}
			set
			{
				this.m_currentDomainScopeInfo = value;
			}
		}

		internal Dictionary<int, IReference<RuntimeGroupRootObj>> DomainScopes
		{
			get
			{
				return this.m_domainScopes;
			}
		}

		internal void AddDomainScopes(IReference<RuntimeMemberObj>[] membersDef, int startIndex)
		{
			for (int i = startIndex; i < membersDef.Length; i++)
			{
				IReference<RuntimeMemberObj> reference = membersDef[i];
				using (reference.PinValue())
				{
					IReference<RuntimeGroupRootObj> groupRoot = ((RuntimeDataTablixMemberObj)reference.Value()).GroupRoot;
					using (groupRoot.PinValue())
					{
						this.m_domainScopes.Add(groupRoot.Value().HierarchyDef.OriginalScopeID, groupRoot);
					}
				}
			}
		}

		internal void RemoveDomainScopes(IReference<RuntimeMemberObj>[] membersDef, int startIndex)
		{
			for (int i = startIndex; i < membersDef.Length; i++)
			{
				IReference<RuntimeMemberObj> reference = membersDef[i];
				using (reference.PinValue())
				{
					IReference<RuntimeGroupRootObj> groupRoot = ((RuntimeDataTablixMemberObj)reference.Value()).GroupRoot;
					using (groupRoot.PinValue())
					{
						this.m_domainScopes.Remove(groupRoot.Value().HierarchyDef.OriginalScopeID);
					}
				}
			}
		}
	}
}
