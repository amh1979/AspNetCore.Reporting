using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal class QueryRestartInfo
	{
		private bool m_queryRestartEnabled;

		private List<ScopeIDContext> m_queryRestartPosition;

		private Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet, List<RelationshipRestartContext>> m_relationshipRestartPositions = new Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet, List<RelationshipRestartContext>>();

		internal bool QueryRestartEnabled
		{
			get
			{
				return this.m_queryRestartEnabled;
			}
			set
			{
				this.m_queryRestartEnabled = value;
			}
		}

		internal List<ScopeIDContext> QueryRestartPosition
		{
			get
			{
				return this.m_queryRestartPosition;
			}
		}

		private ScopeIDContext LastScopeIDContext
		{
			get
			{
				if (this.m_queryRestartPosition.Count == 0)
				{
					return null;
				}
				return this.m_queryRestartPosition[this.m_queryRestartPosition.Count - 1];
			}
		}

		internal QueryRestartInfo()
		{
			this.m_queryRestartPosition = new List<ScopeIDContext>();
		}

		private bool IsRestartable()
		{
			for (int i = 0; i < this.QueryRestartPosition.Count; i++)
			{
				ScopeIDContext scopeIDContext = this.QueryRestartPosition[i];
				if (scopeIDContext.IsRowLevelRestart)
				{
					return true;
				}
			}
			return false;
		}

		internal void EnableQueryRestart()
		{
			if (this.IsRestartable())
			{
				this.m_queryRestartEnabled = true;
			}
		}

		public DataSetQueryRestartPosition GetRestartPositionForDataSet(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet targetDataSet)
		{
			if (!this.m_queryRestartEnabled)
			{
				return null;
			}
			List<RestartContext> list = new List<RestartContext>();
			List<RelationshipRestartContext> list2 = default(List<RelationshipRestartContext>);
			if (this.m_relationshipRestartPositions.TryGetValue(targetDataSet, out list2))
			{
				foreach (RelationshipRestartContext item in list2)
				{
					list.Add(item);
				}
			}
			foreach (ScopeIDContext item2 in this.m_queryRestartPosition)
			{
				if (item2.MemberDefinition.DataScopeInfo.DataSet == targetDataSet && item2.RestartMode != RestartMode.Rom)
				{
					list.Add(item2);
				}
			}
			DataSetQueryRestartPosition result = null;
			if (list.Count > 0)
			{
				result = new DataSetQueryRestartPosition(list);
			}
			return result;
		}

		public void AddRelationshipRestartPosition(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet, RelationshipRestartContext relationshipRestart)
		{
			List<RelationshipRestartContext> list = null;
			if (this.m_relationshipRestartPositions.TryGetValue(dataSet, out list))
			{
				this.m_relationshipRestartPositions[dataSet].Add(relationshipRestart);
			}
			else
			{
				list = new List<RelationshipRestartContext>();
				list.Add(relationshipRestart);
				this.m_relationshipRestartPositions.Add(dataSet, list);
			}
		}

		internal bool TryAddScopeID(ScopeID scopeID, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef, InternalStreamingOdpDynamicMemberLogic memberLogic)
		{
			if (this.IsParentScopeIDAlreadySet(memberDef))
			{
				RestartMode restartMode = (RestartMode)((!this.CanMarkRestartable(memberDef)) ? 1 : 0);
				this.m_queryRestartPosition.Add(new ScopeIDContext(scopeID, memberDef, memberLogic, restartMode));
				return true;
			}
			return false;
		}

		internal void RomBasedRestart()
		{
			for (int i = 0; i < this.m_queryRestartPosition.Count; i++)
			{
				ScopeIDContext scopeIDContext = this.m_queryRestartPosition[i];
				if (!scopeIDContext.IsRowLevelRestart && !scopeIDContext.RomBasedRestart())
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsRombasedRestartFailed, scopeIDContext.MemberDefinition.Grouping.Name.MarkAsModelInfo());
				}
			}
			this.ClearRomRestartScopeIDs();
		}

		private void ClearRomRestartScopeIDs()
		{
			for (int num = this.m_queryRestartPosition.Count - 1; num >= 0; num--)
			{
				if (!this.m_queryRestartPosition[num].IsRowLevelRestart)
				{
					this.m_queryRestartPosition.RemoveAt(num);
				}
			}
		}

		private bool CanMarkRestartable(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef)
		{
			if (memberDef.DataScopeInfo.IsDecomposable && memberDef.Sorting != null && memberDef.Sorting.NaturalSort)
			{
				if (this.LastScopeIDContext != null)
				{
					return this.LastScopeIDContext.IsRowLevelRestart;
				}
				return true;
			}
			return false;
		}

		private bool IsParentScopeIDAlreadySet(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode target)
		{
			if (this.m_queryRestartPosition.Count == 0)
			{
				return !this.ParentScopeIsDynamic(target);
			}
			return this.IsParentScopeAdded(target);
		}

		private bool ParentScopeIsDynamic(IRIFReportDataScope target)
		{
			for (IRIFReportDataScope parentReportScope = target.ParentReportScope; parentReportScope != null; parentReportScope = parentReportScope.ParentReportScope)
			{
				if (parentReportScope.IsGroup)
				{
					return true;
				}
				if (parentReportScope.IsDataIntersectionScope)
				{
					IRIFReportIntersectionScope iRIFReportIntersectionScope = (IRIFReportIntersectionScope)parentReportScope;
					if (!this.ParentScopeIsDynamic(iRIFReportIntersectionScope.ParentRowReportScope))
					{
						return this.ParentScopeIsDynamic(iRIFReportIntersectionScope.ParentColumnReportScope);
					}
					return true;
				}
			}
			return false;
		}

		private bool IsParentScopeAdded(AspNetCore.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode target)
		{
			if (target.DataScopeInfo.DataSet != this.LastScopeIDContext.MemberDefinition.DataScopeInfo.DataSet && !target.IsChildScopeOf(this.LastScopeIDContext.MemberDefinition) && !this.LastScopeIDContext.MemberDefinition.IsChildScopeOf(target))
			{
				return true;
			}
			if (target.IsChildScopeOf(this.LastScopeIDContext.MemberDefinition))
			{
				return true;
			}
			if (!target.DataScopeInfo.IsDecomposable)
			{
				for (int num = this.m_queryRestartPosition.Count - 2; num >= 0; num--)
				{
					if (target.IsChildScopeOf(this.m_queryRestartPosition[num].MemberDefinition))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
	}
}
