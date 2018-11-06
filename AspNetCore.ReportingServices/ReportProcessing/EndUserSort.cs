using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class EndUserSort
	{
		private int m_dataSetID = -1;

		[Reference]
		private ISortFilterScope m_sortExpressionScope;

		[Reference]
		private GroupingList m_groupsInSortTarget;

		[Reference]
		private ISortFilterScope m_sortTarget;

		private int m_sortExpressionIndex = -1;

		private SubReportList m_detailScopeSubReports;

		[NonSerialized]
		private ExpressionInfo m_sortExpression;

		[NonSerialized]
		private int m_sortExpressionScopeID = -1;

		[NonSerialized]
		private IntList m_groupInSortTargetIDs;

		[NonSerialized]
		private int m_sortTargetID = -1;

		[NonSerialized]
		private string m_sortExpressionScopeString;

		[NonSerialized]
		private string m_sortTargetString;

		[NonSerialized]
		private bool m_foundSortExpressionScope;

		internal int DataSetID
		{
			get
			{
				return this.m_dataSetID;
			}
			set
			{
				this.m_dataSetID = value;
			}
		}

		internal ISortFilterScope SortExpressionScope
		{
			get
			{
				return this.m_sortExpressionScope;
			}
			set
			{
				this.m_sortExpressionScope = value;
			}
		}

		internal GroupingList GroupsInSortTarget
		{
			get
			{
				return this.m_groupsInSortTarget;
			}
			set
			{
				this.m_groupsInSortTarget = value;
			}
		}

		internal ISortFilterScope SortTarget
		{
			get
			{
				return this.m_sortTarget;
			}
			set
			{
				this.m_sortTarget = value;
			}
		}

		internal int SortExpressionIndex
		{
			get
			{
				return this.m_sortExpressionIndex;
			}
			set
			{
				this.m_sortExpressionIndex = value;
			}
		}

		internal SubReportList DetailScopeSubReports
		{
			get
			{
				return this.m_detailScopeSubReports;
			}
			set
			{
				this.m_detailScopeSubReports = value;
			}
		}

		internal ExpressionInfo SortExpression
		{
			get
			{
				return this.m_sortExpression;
			}
			set
			{
				this.m_sortExpression = value;
			}
		}

		internal int SortExpressionScopeID
		{
			get
			{
				return this.m_sortExpressionScopeID;
			}
			set
			{
				this.m_sortExpressionScopeID = value;
			}
		}

		internal IntList GroupInSortTargetIDs
		{
			get
			{
				return this.m_groupInSortTargetIDs;
			}
			set
			{
				this.m_groupInSortTargetIDs = value;
			}
		}

		internal int SortTargetID
		{
			get
			{
				return this.m_sortTargetID;
			}
			set
			{
				this.m_sortTargetID = value;
			}
		}

		internal string SortExpressionScopeString
		{
			get
			{
				return this.m_sortExpressionScopeString;
			}
			set
			{
				this.m_sortExpressionScopeString = value;
			}
		}

		internal string SortTargetString
		{
			get
			{
				return this.m_sortTargetString;
			}
			set
			{
				this.m_sortTargetString = value;
			}
		}

		internal bool FoundSortExpressionScope
		{
			get
			{
				return this.m_foundSortExpressionScope;
			}
			set
			{
				this.m_foundSortExpressionScope = value;
			}
		}

		internal void SetSortTarget(ISortFilterScope target)
		{
			Global.Tracer.Assert(null != target);
			this.m_sortTarget = target;
			if (target.UserSortExpressions == null)
			{
				target.UserSortExpressions = new ExpressionInfoList();
			}
			this.m_sortExpressionIndex = target.UserSortExpressions.Count;
			target.UserSortExpressions.Add(this.m_sortExpression);
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DataSetID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.SortExpressionScope, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ISortFilterScope));
			memberInfoList.Add(new MemberInfo(MemberName.GroupsInSortTarget, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.GroupingList));
			memberInfoList.Add(new MemberInfo(MemberName.SortTarget, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ISortFilterScope));
			memberInfoList.Add(new MemberInfo(MemberName.SortExpressionIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DetailScopeSubReports, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.SubReportList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
