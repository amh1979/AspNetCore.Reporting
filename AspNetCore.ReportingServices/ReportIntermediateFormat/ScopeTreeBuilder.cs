using AspNetCore.ReportingServices.ReportPublishing;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal class ScopeTreeBuilder : IRIFScopeVisitor
	{
		protected ScopeTree m_tree;

		internal ScopeTree Tree
		{
			get
			{
				return this.m_tree;
			}
		}

		protected ScopeTreeBuilder(Report report)
		{
			this.m_tree = new ScopeTree(report);
		}

		public static ScopeTree BuildScopeTree(Report report)
		{
			ScopeTreeBuilder scopeTreeBuilder = new ScopeTreeBuilder(report);
			report.TraverseScopes(scopeTreeBuilder);
			return scopeTreeBuilder.Tree;
		}

		public virtual void PreVisit(DataRegion dataRegion)
		{
			this.m_tree.RegisterDataRegion(dataRegion);
		}

		public virtual void PostVisit(DataRegion dataRegion)
		{
			this.m_tree.UnRegisterDataRegion(dataRegion);
		}

		public virtual void PreVisit(ReportHierarchyNode member)
		{
			this.m_tree.RegisterGrouping(member);
		}

		public virtual void PostVisit(ReportHierarchyNode member)
		{
			this.m_tree.UnRegisterGrouping(member);
		}

		public virtual void PreVisit(Cell cell, int rowIndex, int colIndex)
		{
			this.m_tree.RegisterCell(cell);
		}

		public virtual void PostVisit(Cell cell, int rowIndex, int colIndex)
		{
			this.m_tree.UnRegisterCell(cell);
		}
	}
}
