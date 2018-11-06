namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MatrixMemberInfoCache
	{
		private int m_startIndex;

		private int[] m_cellIndexes;

		private MatrixMemberInfoCache[] m_children;

		internal bool IsOptimizedNode
		{
			get
			{
				return this.m_startIndex >= 0;
			}
		}

		internal MatrixMemberInfoCache[] Children
		{
			get
			{
				return this.m_children;
			}
		}

		internal MatrixMemberInfoCache(int startIndex, int length)
		{
			this.m_startIndex = startIndex;
			if (!this.IsOptimizedNode)
			{
				this.m_cellIndexes = new int[length];
				this.m_children = new MatrixMemberInfoCache[length];
				for (int i = 0; i < length; i++)
				{
					this.m_cellIndexes[i] = -1;
				}
			}
		}

		internal int GetCellIndex(ShimMatrixMember member)
		{
			if (this.IsOptimizedNode)
			{
				return this.m_startIndex + member.AdjustedRenderCollectionIndex;
			}
			int adjustedRenderCollectionIndex = member.AdjustedRenderCollectionIndex;
			if (this.m_cellIndexes[adjustedRenderCollectionIndex] < 0)
			{
				this.m_cellIndexes[adjustedRenderCollectionIndex] = member.CurrentRenderMatrixMember.CachedMemberCellIndex;
			}
			return this.m_cellIndexes[adjustedRenderCollectionIndex];
		}
	}
}
