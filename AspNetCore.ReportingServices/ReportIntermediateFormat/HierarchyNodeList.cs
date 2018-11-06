using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class HierarchyNodeList : ArrayList
	{
		private List<int> m_leafCellIndexes;

		private int m_excludedIndex = -1;

		private List<int> m_leafCellIndexesWithoutExcluded;

		private int? m_domainScopeCount = null;

		[NonSerialized]
		private HierarchyNodeList m_staticMembersInSameScope;

		[NonSerialized]
		private HierarchyNodeList m_dynamicMembersAtScope;

		[NonSerialized]
		private bool m_hasStaticLeafMembers;

		internal new ReportHierarchyNode this[int index]
		{
			get
			{
				return base[index] as ReportHierarchyNode;
			}
		}

		internal List<int> LeafCellIndexes
		{
			get
			{
				if (this.m_leafCellIndexes == null)
				{
					HierarchyNodeList.CalculateLeafCellIndexes(this, ref this.m_leafCellIndexes, -1);
				}
				if (this.m_leafCellIndexes.Count != 0)
				{
					return this.m_leafCellIndexes;
				}
				return null;
			}
		}

		internal int OriginalNodeCount
		{
			get
			{
				return this.Count - this.DomainScopeCount;
			}
		}

		private int DomainScopeCount
		{
			get
			{
				if (!this.m_domainScopeCount.HasValue)
				{
					this.m_domainScopeCount = 0;
					foreach (ReportHierarchyNode item in this)
					{
						if (item.IsDomainScope)
						{
							this.m_domainScopeCount++;
						}
					}
				}
				return this.m_domainScopeCount.Value;
			}
		}

		public HierarchyNodeList StaticMembersInSameScope
		{
			get
			{
				if (this.m_staticMembersInSameScope == null)
				{
					this.CalculateDependencies();
				}
				return this.m_staticMembersInSameScope;
			}
		}

		public bool HasStaticLeafMembers
		{
			get
			{
				if (this.m_staticMembersInSameScope == null)
				{
					this.CalculateDependencies();
				}
				return this.m_hasStaticLeafMembers;
			}
		}

		public HierarchyNodeList DynamicMembersAtScope
		{
			get
			{
				if (this.m_dynamicMembersAtScope == null)
				{
					this.CalculateDependencies();
				}
				return this.m_dynamicMembersAtScope;
			}
		}

		internal HierarchyNodeList()
		{
		}

		internal HierarchyNodeList(int capacity)
			: base(capacity)
		{
		}

		public override int Add(object value)
		{
			if (((ReportHierarchyNode)value).IsDomainScope)
			{
				this.InitializeDomainScopeCount();
			}
			return base.Add(value);
		}

		private void InitializeDomainScopeCount()
		{
			this.m_domainScopeCount = null;
		}

		internal List<int> GetLeafCellIndexes(int excludedCellIndex)
		{
			if (excludedCellIndex < 0)
			{
				return this.LeafCellIndexes;
			}
			if (this.m_leafCellIndexesWithoutExcluded == null)
			{
				this.m_excludedIndex = excludedCellIndex;
				HierarchyNodeList.CalculateLeafCellIndexes(this, ref this.m_leafCellIndexesWithoutExcluded, excludedCellIndex);
			}
			else if (this.m_excludedIndex != excludedCellIndex)
			{
				this.m_excludedIndex = excludedCellIndex;
				this.m_leafCellIndexesWithoutExcluded = null;
				HierarchyNodeList.CalculateLeafCellIndexes(this, ref this.m_leafCellIndexesWithoutExcluded, excludedCellIndex);
			}
			if (this.m_leafCellIndexesWithoutExcluded.Count != 0)
			{
				return this.m_leafCellIndexesWithoutExcluded;
			}
			return null;
		}

		private static void CalculateLeafCellIndexes(HierarchyNodeList nodes, ref List<int> leafCellIndexes, int excludedCellIndex)
		{
			if (leafCellIndexes == null)
			{
				int count = nodes.Count;
				leafCellIndexes = new List<int>(count);
				for (int i = 0; i < count; i++)
				{
					ReportHierarchyNode reportHierarchyNode = nodes[i];
					if (reportHierarchyNode.InnerHierarchy == null && reportHierarchyNode.MemberCellIndex != excludedCellIndex)
					{
						leafCellIndexes.Add(reportHierarchyNode.MemberCellIndex);
					}
				}
			}
		}

		internal List<ReportHierarchyNode> GetLeafNodes()
		{
			List<ReportHierarchyNode> list = new List<ReportHierarchyNode>();
			this.FindLeafNodes(list);
			return list;
		}

		private void FindLeafNodes(List<ReportHierarchyNode> leafNodes)
		{
			for (int i = 0; i < base.Count; i++)
			{
				ReportHierarchyNode reportHierarchyNode = this[i];
				HierarchyNodeList innerHierarchy = reportHierarchyNode.InnerHierarchy;
				if (innerHierarchy == null)
				{
					leafNodes.Add(reportHierarchyNode);
				}
				else
				{
					innerHierarchy.FindLeafNodes(leafNodes);
				}
			}
		}

		internal int GetMemberIndex(ReportHierarchyNode node)
		{
			Global.Tracer.Assert(node.InnerHierarchy == null, "GetMemberIndex should not be called for non leaf node");
			int result = -1;
			this.GetMemberIndex(ref result, node);
			return result;
		}

		private bool GetMemberIndex(ref int index, ReportHierarchyNode node)
		{
			foreach (ReportHierarchyNode item in this)
			{
				if (item.InnerHierarchy == null)
				{
					index++;
					if (object.ReferenceEquals(node, item))
					{
						return true;
					}
				}
				else if (item.InnerHierarchy.GetMemberIndex(ref index, node))
				{
					return true;
				}
			}
			return false;
		}

		private void CalculateDependencies()
		{
			this.m_staticMembersInSameScope = new HierarchyNodeList();
			this.m_dynamicMembersAtScope = new HierarchyNodeList();
			this.m_hasStaticLeafMembers = HierarchyNodeList.CalculateDependencies(this, this.m_staticMembersInSameScope, this.m_dynamicMembersAtScope);
		}

		private static bool CalculateDependencies(HierarchyNodeList members, HierarchyNodeList staticMembersInSameScope, HierarchyNodeList dynamicMembers)
		{
			if (members == null)
			{
				return false;
			}
			bool flag = false;
			int count = members.Count;
			foreach (ReportHierarchyNode member in members)
			{
				if (!member.IsStatic)
				{
					dynamicMembers.Add(member);
				}
				else
				{
					staticMembersInSameScope.Add(member);
					flag = (member.InnerHierarchy == null || (flag | HierarchyNodeList.CalculateDependencies(member.InnerHierarchy, staticMembersInSameScope, dynamicMembers)));
				}
			}
			return flag;
		}
	}
}
