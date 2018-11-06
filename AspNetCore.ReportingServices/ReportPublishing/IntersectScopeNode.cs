using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal class IntersectScopeNode : ScopeTreeNode
	{
		private readonly ScopeTreeNode m_parentRowScope;

		private readonly ScopeTreeNode m_parentColumnScope;

		private readonly List<IRIFDataScope> m_peerDataCells;

		internal ScopeTreeNode ParentRowScope
		{
			get
			{
				return this.m_parentRowScope;
			}
		}

		internal ScopeTreeNode ParentColumnScope
		{
			get
			{
				return this.m_parentColumnScope;
			}
		}

		internal override string ScopeName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder("(");
				stringBuilder.Append(this.m_parentRowScope.ScopeName);
				stringBuilder.Append(".");
				stringBuilder.Append(this.m_parentColumnScope.ScopeName);
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
		}

		internal IntersectScopeNode(IRIFDataScope scope, ScopeTreeNode parentRowScope, ScopeTreeNode parentColScope)
			: base(scope)
		{
			this.m_parentRowScope = parentRowScope;
			if (this.m_parentRowScope != null)
			{
				this.m_parentRowScope.AddChildScope(scope);
			}
			this.m_parentColumnScope = parentColScope;
			if (this.m_parentColumnScope != null)
			{
				this.m_parentColumnScope.AddChildScope(scope);
			}
			this.m_peerDataCells = new List<IRIFDataScope>();
		}

		internal override bool IsSameOrParentScope(IRIFDataScope parentScope, bool isProperParent)
		{
			if (this.HasCell(parentScope))
			{
				return true;
			}
			if (this.m_parentRowScope != null && this.m_parentColumnScope != null)
			{
				bool flag = this.m_parentRowScope.IsSameOrParentScope(parentScope, isProperParent);
				bool result = this.m_parentColumnScope.IsSameOrParentScope(parentScope, isProperParent);
				if (!isProperParent)
				{
					if (!flag)
					{
						return result;
					}
					return true;
				}
				if (flag)
				{
					return result;
				}
				return false;
			}
			return false;
		}

		internal override void Traverse(ScopeTree.ScopeTreeVisitor visitor, IRIFDataScope outerScope, bool visitOuterScope)
		{
			bool flag = this.HasCell(outerScope);
			if (visitOuterScope || !flag)
			{
				this.TraverseDefinitionCells(visitor);
			}
			if (!flag)
			{
				if (this.m_parentRowScope != null)
				{
					this.m_parentRowScope.Traverse(visitor, outerScope, visitOuterScope);
				}
				if (this.m_parentColumnScope != null)
				{
					this.m_parentColumnScope.Traverse(visitor, outerScope, visitOuterScope);
				}
			}
		}

		internal override bool Traverse(ScopeTree.DirectedScopeTreeVisitor visitor)
		{
			if (this.TraverseDefinitionCells(visitor) && ScopeTreeNode.TraverseNode(visitor, this.m_parentRowScope))
			{
				return ScopeTreeNode.TraverseNode(visitor, this.m_parentColumnScope);
			}
			return false;
		}

		private void TraverseDefinitionCells(ScopeTree.ScopeTreeVisitor visitor)
		{
			visitor(base.Scope);
			foreach (IRIFDataScope peerDataCell in this.m_peerDataCells)
			{
				visitor(peerDataCell);
			}
		}

		private bool TraverseDefinitionCells(ScopeTree.DirectedScopeTreeVisitor visitor)
		{
			if (!visitor(base.Scope))
			{
				return false;
			}
			foreach (IRIFDataScope peerDataCell in this.m_peerDataCells)
			{
				if (!visitor(peerDataCell))
				{
					return false;
				}
			}
			return true;
		}

		internal void AddCell(IRIFDataScope cell)
		{
			if (!this.HasCell(cell))
			{
				this.m_peerDataCells.Add(cell);
			}
		}

		internal bool HasCell(IRIFDataScope cell)
		{
			if (cell == null)
			{
				return false;
			}
			if (cell == base.Scope)
			{
				return true;
			}
			foreach (IRIFDataScope peerDataCell in this.m_peerDataCells)
			{
				if (ScopeTree.SameScope(cell, peerDataCell))
				{
					return true;
				}
			}
			return false;
		}
	}
}
