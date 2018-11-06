using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class ScopeTreeBuilderForDataScopeDataSet : ScopeTreeBuilder
	{
		private readonly ErrorContext m_errorContext;

		private int m_nextDataPipelineId;

		public static ScopeTree BuildScopeTree(Report report, ErrorContext errorContext)
		{
			ScopeTreeBuilderForDataScopeDataSet scopeTreeBuilderForDataScopeDataSet = new ScopeTreeBuilderForDataScopeDataSet(report, errorContext);
			report.TraverseScopes(scopeTreeBuilderForDataScopeDataSet);
			report.DataPipelineCount = scopeTreeBuilderForDataScopeDataSet.m_nextDataPipelineId;
			return scopeTreeBuilderForDataScopeDataSet.Tree;
		}

		private ScopeTreeBuilderForDataScopeDataSet(Report report, ErrorContext errorContext)
			: base(report)
		{
			this.m_errorContext = errorContext;
			report.BindAndValidateDataSetDefaultRelationships(this.m_errorContext);
			this.m_nextDataPipelineId = report.DataSetCount;
		}

		public override void PreVisit(DataRegion dataRegion)
		{
			base.PreVisit(dataRegion);
			this.SetDataSetForScope(dataRegion);
		}

		private void SetDataSetForScope(IRIFReportDataScope scope)
		{
			if (scope != null && scope.DataScopeInfo != null)
			{
				scope.DataScopeInfo.ValidateDataSetBindingAndRelationships(base.m_tree, scope, this.m_errorContext);
				this.DetermineDataPipelineID(scope);
			}
		}

		private void DetermineDataPipelineID(IRIFReportDataScope scope)
		{
			if (scope.DataScopeInfo.DataSet != null)
			{
				DataSet dataSet = scope.DataScopeInfo.DataSet;
				int dataPipelineID;
				if (scope.DataScopeInfo.NeedsIDC)
				{
					if (base.m_tree.IsIntersectionScope(scope))
					{
						if (DataSet.AreEqualById(dataSet, base.m_tree.GetParentRowScopeForIntersection(scope).DataScopeInfo.DataSet) || DataSet.AreEqualById(dataSet, base.m_tree.GetParentColumnScopeForIntersection(scope).DataScopeInfo.DataSet))
						{
							IRIFDataScope canonicalCellScope = base.m_tree.GetCanonicalCellScope(scope);
							if (ScopeTree.SameScope(scope, canonicalCellScope))
							{
								dataPipelineID = this.m_nextDataPipelineId;
								this.m_nextDataPipelineId++;
							}
							else
							{
								dataPipelineID = canonicalCellScope.DataScopeInfo.DataPipelineID;
							}
						}
						else
						{
							dataPipelineID = dataSet.IndexInCollection;
						}
					}
					else
					{
						dataPipelineID = dataSet.IndexInCollection;
					}
				}
				else
				{
					IRIFDataScope iRIFDataScope = (!base.m_tree.IsIntersectionScope(scope)) ? base.m_tree.GetParentScope(scope) : base.m_tree.GetParentRowScopeForIntersection(scope);
					dataPipelineID = ((iRIFDataScope != null) ? iRIFDataScope.DataScopeInfo.DataPipelineID : dataSet.IndexInCollection);
				}
				scope.DataScopeInfo.DataPipelineID = dataPipelineID;
			}
		}

		public override void PreVisit(ReportHierarchyNode member)
		{
			base.PreVisit(member);
			this.SetDataSetForScope(member);
		}

		public override void PreVisit(Cell cell, int rowIndex, int colIndex)
		{
			base.PreVisit(cell, rowIndex, colIndex);
			this.SetDataSetForScope(cell);
		}
	}
}
