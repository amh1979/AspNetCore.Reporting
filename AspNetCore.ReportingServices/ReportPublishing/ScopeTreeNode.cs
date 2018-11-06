using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal abstract class ScopeTreeNode
	{
		protected readonly IRIFDataScope m_scope;

		private readonly List<IRIFDataScope> m_childScopes = new List<IRIFDataScope>();

		internal IRIFDataScope Scope
		{
			get
			{
				return this.m_scope;
			}
		}

		internal List<IRIFDataScope> ChildScopes
		{
			get
			{
				return this.m_childScopes;
			}
		}

		internal abstract string ScopeName
		{
			get;
		}

		internal ScopeTreeNode(IRIFDataScope scope)
		{
			this.m_scope = scope;
		}

		internal void AddChildScope(IRIFDataScope child)
		{
			this.m_childScopes.Add(child);
		}

		internal abstract bool IsSameOrParentScope(IRIFDataScope parentScope, bool isProperParent);

		internal abstract void Traverse(ScopeTree.ScopeTreeVisitor visitor, IRIFDataScope outerScope, bool visitOuterScope);

		internal abstract bool Traverse(ScopeTree.DirectedScopeTreeVisitor visitor);

		protected static bool TraverseNode(ScopeTree.DirectedScopeTreeVisitor visitor, ScopeTreeNode node)
		{
			if (node != null)
			{
				return node.Traverse(visitor);
			}
			return true;
		}
	}
}
