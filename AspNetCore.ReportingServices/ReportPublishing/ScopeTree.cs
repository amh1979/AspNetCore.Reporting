using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal class ScopeTree
	{
		internal delegate void ScopeTreeVisitor(IRIFDataScope scope);

		internal delegate bool DirectedScopeTreeVisitor(IRIFDataScope scope);

		private Dictionary<IRIFDataScope, ScopeTreeNode> m_scopes;

		private Dictionary<string, ScopeTreeNode> m_scopesByName;

		private Dictionary<string, Dictionary<string, ScopeTreeNode>> m_canonicalCellScopes;

		private FunctionalList<ScopeTreeNode> m_dataRegionScopes;

		private FunctionalList<ScopeTreeNode> m_activeScopes;

		private FunctionalList<ScopeTreeNode> m_activeRowScopes;

		private FunctionalList<ScopeTreeNode> m_activeColumnScopes;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_report;

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Report Report
		{
			get
			{
				return this.m_report;
			}
		}

		internal ScopeTree()
		{
			this.m_scopes = new Dictionary<IRIFDataScope, ScopeTreeNode>();
			this.m_scopesByName = new Dictionary<string, ScopeTreeNode>(StringComparer.Ordinal);
			this.m_dataRegionScopes = FunctionalList<ScopeTreeNode>.Empty;
			this.m_activeScopes = FunctionalList<ScopeTreeNode>.Empty;
			this.m_activeRowScopes = FunctionalList<ScopeTreeNode>.Empty;
			this.m_activeColumnScopes = FunctionalList<ScopeTreeNode>.Empty;
			this.m_canonicalCellScopes = new Dictionary<string, Dictionary<string, ScopeTreeNode>>();
		}

		internal ScopeTree(AspNetCore.ReportingServices.ReportIntermediateFormat.Report report)
			: this()
		{
			this.m_report = report;
		}

		internal static bool SameScope(IRIFDataScope scope1, IRIFDataScope scope2)
		{
			return scope1 == scope2;
		}

		internal static bool SameScope(IRIFDataScope scope1, string scope2)
		{
			if (scope1 == null && scope2 == null)
			{
				return true;
			}
			if (scope1 != null && scope2 != null)
			{
				return ScopeTree.SameScope(scope1.Name, scope2);
			}
			return false;
		}

		internal static bool SameScope(string scope1, string scope2)
		{
			if (scope1 == null && scope2 == null)
			{
				return true;
			}
			if (scope1 != null && scope2 != null)
			{
				return 0 == string.CompareOrdinal(scope1, scope2);
			}
			return false;
		}

		internal string FindAncestorScopeName(string scopeName, int ancestorLevel)
		{
			ScopeTreeNode scopeTreeNode = default(ScopeTreeNode);
			if (this.m_scopesByName.TryGetValue(scopeName, out scopeTreeNode))
			{
				SubScopeNode subScopeNode = scopeTreeNode as SubScopeNode;
				if (subScopeNode != null)
				{
					SubScopeNode subScopeNode2 = subScopeNode;
					for (int i = 0; i < ancestorLevel; i++)
					{
						if (subScopeNode2.Scope is AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion)
						{
							break;
						}
						subScopeNode = (subScopeNode.ParentScope as SubScopeNode);
						if (subScopeNode == null)
						{
							break;
						}
						subScopeNode2 = subScopeNode;
					}
					return subScopeNode2.ScopeName;
				}
			}
			return null;
		}

		internal int MeasureScopeDistance(string innerScopeName, string outerScopeName)
		{
			ScopeTreeNode scopeTreeNode = default(ScopeTreeNode);
			if (this.m_scopesByName.TryGetValue(innerScopeName, out scopeTreeNode))
			{
				SubScopeNode subScopeNode = scopeTreeNode as SubScopeNode;
				if (subScopeNode != null)
				{
					int num = 0;
					SubScopeNode subScopeNode2 = subScopeNode;
					while (!string.Equals(subScopeNode2.ScopeName, outerScopeName, StringComparison.Ordinal))
					{
						subScopeNode = (subScopeNode.ParentScope as SubScopeNode);
						if (subScopeNode == null)
						{
							return -1;
						}
						subScopeNode2 = subScopeNode;
						num++;
					}
					return num;
				}
			}
			return -1;
		}

		internal bool IsSameOrProperParentScope(IRIFDataScope outerScope, IRIFDataScope innerScope)
		{
			ScopeTreeNode scopeTreeNode = default(ScopeTreeNode);
			if (this.m_scopes.TryGetValue(innerScope, out scopeTreeNode))
			{
				return scopeTreeNode.IsSameOrParentScope(outerScope, true);
			}
			return false;
		}

		internal bool IsSameOrParentScope(IRIFDataScope outerScope, IRIFDataScope innerScope)
		{
			ScopeTreeNode scopeTreeNode = default(ScopeTreeNode);
			if (this.m_scopes.TryGetValue(innerScope, out scopeTreeNode))
			{
				return scopeTreeNode.IsSameOrParentScope(outerScope, false);
			}
			return false;
		}

		internal bool IsParentScope(IRIFDataScope outerScope, IRIFDataScope innerScope)
		{
			if (outerScope == innerScope)
			{
				return false;
			}
			ScopeTreeNode scopeTreeNode = default(ScopeTreeNode);
			if (this.m_scopes.TryGetValue(innerScope, out scopeTreeNode))
			{
				return scopeTreeNode.IsSameOrParentScope(outerScope, false);
			}
			return false;
		}

		internal IEnumerable<IRIFDataScope> GetChildScopes(IRIFDataScope parentScope)
		{
			ScopeTreeNode scopeNodeOrAssert = this.GetScopeNodeOrAssert(parentScope);
			return scopeNodeOrAssert.ChildScopes;
		}

		internal bool IsIntersectionScope(IRIFDataScope scope)
		{
			ScopeTreeNode scopeNodeOrAssert = this.GetScopeNodeOrAssert(scope);
			return scopeNodeOrAssert is IntersectScopeNode;
		}

		internal IRIFDataScope GetParentScope(IRIFDataScope scope)
		{
			SubScopeNode subScopeNodeOrAssert = this.GetSubScopeNodeOrAssert(scope);
			ScopeTreeNode parentScope = subScopeNodeOrAssert.ParentScope;
			if (parentScope != null)
			{
				return parentScope.Scope;
			}
			return null;
		}

		internal IRIFDataScope GetParentRowScopeForIntersection(IRIFDataScope intersectScope)
		{
			IntersectScopeNode intersectScopeNodeOrAssert = this.GetIntersectScopeNodeOrAssert(intersectScope);
			return intersectScopeNodeOrAssert.ParentRowScope.Scope;
		}

		internal IRIFDataScope GetParentColumnScopeForIntersection(IRIFDataScope intersectScope)
		{
			IntersectScopeNode intersectScopeNodeOrAssert = this.GetIntersectScopeNodeOrAssert(intersectScope);
			return intersectScopeNodeOrAssert.ParentColumnScope.Scope;
		}

		internal void Traverse(ScopeTreeVisitor visitor, IRIFDataScope outerScope, IRIFDataScope innerScope, bool visitOuterScope)
		{
			ScopeTreeNode scopeNodeOrAssert = this.GetScopeNodeOrAssert(innerScope);
			scopeNodeOrAssert.Traverse(visitor, outerScope, visitOuterScope);
		}

		internal bool Traverse(DirectedScopeTreeVisitor visitor, IRIFDataScope startScope)
		{
			ScopeTreeNode scopeNodeOrAssert = this.GetScopeNodeOrAssert(startScope);
			return scopeNodeOrAssert.Traverse(visitor);
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion GetParentDataRegion(IRIFDataScope scope)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion parentDataRegion = null;
			DirectedScopeTreeVisitor visitor = delegate(IRIFDataScope candidate)
			{
				if (candidate != scope)
				{
					parentDataRegion = (candidate as AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion);
				}
				return parentDataRegion == null;
			};
			this.Traverse(visitor, scope);
			return parentDataRegion;
		}

		private SubScopeNode GetSubScopeNodeOrAssert(IRIFDataScope scope)
		{
			ScopeTreeNode scopeNodeOrAssert = this.GetScopeNodeOrAssert(scope);
			SubScopeNode subScopeNode = scopeNodeOrAssert as SubScopeNode;
			Global.Tracer.Assert(subScopeNode != null, "Specified scope was not a SubScope");
			return subScopeNode;
		}

		private IntersectScopeNode GetIntersectScopeNodeOrAssert(IRIFDataScope scope)
		{
			ScopeTreeNode scopeNodeOrAssert = this.GetScopeNodeOrAssert(scope);
			IntersectScopeNode intersectScopeNode = scopeNodeOrAssert as IntersectScopeNode;
			Global.Tracer.Assert(intersectScopeNode != null, "Specified scope was not an IntersectScopeNode ");
			return intersectScopeNode;
		}

		private ScopeTreeNode GetScopeNodeOrAssert(IRIFDataScope scope)
		{
			ScopeTreeNode result = default(ScopeTreeNode);
			if (this.m_scopes.TryGetValue(scope, out result))
			{
				return result;
			}
			Global.Tracer.Assert(false, "Could not find scope in tree: {0}", scope);
			throw new InvalidOperationException();
		}

		internal void RegisterGrouping(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member)
		{
			if (member.IsColumn)
			{
				this.AddGroupScope(member, ref this.m_activeColumnScopes);
			}
			else
			{
				this.AddGroupScope(member, ref this.m_activeRowScopes);
			}
		}

		private void AddGroupScope(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member, ref FunctionalList<ScopeTreeNode> axisScopes)
		{
			ScopeTreeNode scopeTreeNode = default(ScopeTreeNode);
			if (!this.m_scopes.TryGetValue((IRIFDataScope)member, out scopeTreeNode))
			{
				scopeTreeNode = (this.HasScope(axisScopes) ? new SubScopeNode(member, this.m_activeScopes.First) : new SubScopeNode(member, this.m_dataRegionScopes.First));
			}
			this.AddScope(scopeTreeNode);
			axisScopes = axisScopes.Add(scopeTreeNode);
		}

		internal void UnRegisterGrouping(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member)
		{
			if (member.IsColumn)
			{
				this.RemoveGroupScope(ref this.m_activeColumnScopes);
			}
			else
			{
				this.RemoveGroupScope(ref this.m_activeRowScopes);
			}
		}

		private void RemoveGroupScope(ref FunctionalList<ScopeTreeNode> axisScopes)
		{
			axisScopes = axisScopes.Rest;
			this.m_activeScopes = this.m_activeScopes.Rest;
		}

		internal void RegisterDataRegion(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion)
		{
			ScopeTreeNode scopeTreeNode = default(ScopeTreeNode);
			if (!this.m_scopes.TryGetValue((IRIFDataScope)dataRegion, out scopeTreeNode))
			{
				scopeTreeNode = new SubScopeNode(dataRegion, this.m_activeScopes.First);
			}
			this.AddScope(scopeTreeNode);
			this.m_dataRegionScopes = this.m_dataRegionScopes.Add(scopeTreeNode);
			this.m_activeRowScopes = this.m_activeRowScopes.Add(null);
			this.m_activeColumnScopes = this.m_activeColumnScopes.Add(null);
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet GetDataSet(IRIFDataScope dataScope, string dataSetName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = null;
			if (dataScope != null && dataScope.DataScopeInfo != null)
			{
				dataSet = dataScope.DataScopeInfo.DataSet;
			}
			if (dataSet != null)
			{
				return dataSet;
			}
			if (dataSetName == null)
			{
				return null;
			}
			return this.GetDataSet(dataSetName);
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet GetDataSet(string dataSetName)
		{
			if (string.IsNullOrEmpty(dataSetName))
			{
				return null;
			}
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet result = default(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet);
			this.m_report.MappingNameToDataSet.TryGetValue(dataSetName, out result);
			return result;
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet GetDefaultTopLevelDataSet()
		{
			if (this.m_report.OneDataSetName != null)
			{
				return this.GetDataSet(this.m_report.OneDataSetName);
			}
			return null;
		}

		internal void UnRegisterDataRegion(AspNetCore.ReportingServices.ReportIntermediateFormat.DataRegion dataRegion)
		{
			this.m_activeScopes = this.m_activeScopes.Rest;
			this.m_activeRowScopes = this.m_activeRowScopes.Rest;
			this.m_activeColumnScopes = this.m_activeColumnScopes.Rest;
			this.m_dataRegionScopes = this.m_dataRegionScopes.Rest;
		}

		internal IRIFDataScope RegisterCell(IRIFDataScope cell)
		{
			ScopeTreeNode scopeTreeNode = default(ScopeTreeNode);
			if (!this.m_scopes.TryGetValue(cell, out scopeTreeNode))
			{
				if (this.HasScope(this.m_activeRowScopes) && this.HasScope(this.m_activeColumnScopes))
				{
					ScopeTreeNode first = this.m_activeRowScopes.First;
					ScopeTreeNode first2 = this.m_activeColumnScopes.First;
					if (!this.TryGetCanonicalCellScope(first, first2, out scopeTreeNode))
					{
						scopeTreeNode = new IntersectScopeNode(cell, first, first2);
						this.AddCanonicalCellScope(first, first2, scopeTreeNode);
					}
					((IntersectScopeNode)scopeTreeNode).AddCell(cell);
				}
				else
				{
					scopeTreeNode = new SubScopeNode(cell, this.m_activeScopes.First);
				}
			}
			this.AddScope(scopeTreeNode, cell);
			this.m_activeRowScopes = this.m_activeRowScopes.Add(null);
			this.m_activeColumnScopes = this.m_activeColumnScopes.Add(null);
			return scopeTreeNode.Scope;
		}

		internal void UnRegisterCell(IRIFDataScope cell)
		{
			this.m_activeScopes = this.m_activeScopes.Rest;
			this.m_activeRowScopes = this.m_activeRowScopes.Rest;
			this.m_activeColumnScopes = this.m_activeColumnScopes.Rest;
		}

		internal IRIFDataScope GetCanonicalCellScope(IRIFDataScope cell)
		{
			ScopeTreeNode scopeTreeNode = default(ScopeTreeNode);
			if (!this.m_scopes.TryGetValue(cell, out scopeTreeNode))
			{
				Global.Tracer.Assert(false, "GetCanonicalCellScope must not be called for a cell outside the ScopeTree.");
			}
			return scopeTreeNode.Scope;
		}

		private bool TryGetCanonicalCellScope(ScopeTreeNode rowScope, ScopeTreeNode colScope, out ScopeTreeNode canonicalCellScope)
		{
			Dictionary<string, ScopeTreeNode> dictionary = default(Dictionary<string, ScopeTreeNode>);
			if (this.m_canonicalCellScopes.TryGetValue(rowScope.Scope.Name, out dictionary) && dictionary.TryGetValue(colScope.Scope.Name, out canonicalCellScope))
			{
				return true;
			}
			canonicalCellScope = null;
			return false;
		}

		private void AddCanonicalCellScope(ScopeTreeNode rowScope, ScopeTreeNode colScope, ScopeTreeNode cellScope)
		{
			Dictionary<string, ScopeTreeNode> dictionary = default(Dictionary<string, ScopeTreeNode>);
			if (!this.m_canonicalCellScopes.TryGetValue(rowScope.Scope.Name, out dictionary))
			{
				dictionary = new Dictionary<string, ScopeTreeNode>();
				this.m_canonicalCellScopes.Add(rowScope.Scope.Name, dictionary);
			}
			dictionary[colScope.Scope.Name] = cellScope;
		}

		private bool HasScope(FunctionalList<ScopeTreeNode> list)
		{
			if (!list.IsEmpty())
			{
				return list.First != null;
			}
			return false;
		}

		private void AddScope(ScopeTreeNode scopeNode, IRIFDataScope scope)
		{
			this.m_activeScopes = this.m_activeScopes.Add(scopeNode);
			this.m_scopes[scope] = scopeNode;
			string scopeName = scopeNode.ScopeName;
			if (!string.IsNullOrEmpty(scopeName))
			{
				this.m_scopesByName[scopeNode.ScopeName] = scopeNode;
			}
		}

		private void AddScope(ScopeTreeNode scopeNode)
		{
			this.AddScope(scopeNode, scopeNode.Scope);
		}

		internal string GetScopeName(IRIFDataScope scope)
		{
			string text = null;
			ScopeTreeNode scopeTreeNode = default(ScopeTreeNode);
			if (this.m_scopes.TryGetValue(scope, out scopeTreeNode))
			{
				return scopeTreeNode.ScopeName;
			}
			return scope.Name;
		}

		internal IRIFDataScope GetScopeByName(string scopeName)
		{
			ScopeTreeNode scopeTreeNode = default(ScopeTreeNode);
			if (this.m_scopesByName.TryGetValue(scopeName, out scopeTreeNode))
			{
				return scopeTreeNode.Scope;
			}
			return null;
		}
	}
}
