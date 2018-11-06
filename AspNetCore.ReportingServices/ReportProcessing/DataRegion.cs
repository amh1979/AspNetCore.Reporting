using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class DataRegion : ReportItem, IPageBreakItem, IAggregateHolder, ISortFilterScope
	{
		protected string m_dataSetName;

		protected ExpressionInfo m_noRows;

		protected bool m_pageBreakAtEnd;

		protected bool m_pageBreakAtStart;

		protected bool m_keepTogether;

		protected IntList m_repeatSiblings;

		protected FilterList m_filters;

		protected DataAggregateInfoList m_aggregates;

		protected DataAggregateInfoList m_postSortAggregates;

		protected ExpressionInfoList m_userSortExpressions;

		protected InScopeSortFilterHashtable m_detailSortFiltersInScope;

		[NonSerialized]
		protected ReportProcessing.RuntimeDataRegionObj m_runtimeDataRegionObj;

		[NonSerialized]
		protected PageBreakStates m_pagebreakState;

		[NonSerialized]
		protected Hashtable m_scopeNames;

		[NonSerialized]
		protected bool m_inPivotCell;

		[NonSerialized]
		protected bool[] m_isSortFilterTarget;

		[NonSerialized]
		protected bool[] m_isSortFilterExpressionScope;

		[NonSerialized]
		protected int[] m_sortFilterSourceDetailScopeInfo;

		[NonSerialized]
		protected int m_currentDetailRowIndex = -1;

		internal string DataSetName
		{
			get
			{
				return this.m_dataSetName;
			}
			set
			{
				this.m_dataSetName = value;
			}
		}

		internal ExpressionInfo NoRows
		{
			get
			{
				return this.m_noRows;
			}
			set
			{
				this.m_noRows = value;
			}
		}

		internal bool PageBreakAtEnd
		{
			get
			{
				return this.m_pageBreakAtEnd;
			}
			set
			{
				this.m_pageBreakAtEnd = value;
			}
		}

		internal bool PageBreakAtStart
		{
			get
			{
				return this.m_pageBreakAtStart;
			}
			set
			{
				this.m_pageBreakAtStart = value;
			}
		}

		internal bool KeepTogether
		{
			get
			{
				return this.m_keepTogether;
			}
			set
			{
				this.m_keepTogether = value;
			}
		}

		internal IntList RepeatSiblings
		{
			get
			{
				return this.m_repeatSiblings;
			}
			set
			{
				this.m_repeatSiblings = value;
			}
		}

		internal FilterList Filters
		{
			get
			{
				return this.m_filters;
			}
			set
			{
				this.m_filters = value;
			}
		}

		internal DataAggregateInfoList Aggregates
		{
			get
			{
				return this.m_aggregates;
			}
			set
			{
				this.m_aggregates = value;
			}
		}

		internal DataAggregateInfoList PostSortAggregates
		{
			get
			{
				return this.m_postSortAggregates;
			}
			set
			{
				this.m_postSortAggregates = value;
			}
		}

		internal ExpressionInfoList UserSortExpressions
		{
			get
			{
				return this.m_userSortExpressions;
			}
			set
			{
				this.m_userSortExpressions = value;
			}
		}

		internal InScopeSortFilterHashtable DetailSortFiltersInScope
		{
			get
			{
				return this.m_detailSortFiltersInScope;
			}
			set
			{
				this.m_detailSortFiltersInScope = value;
			}
		}

		internal ReportProcessing.RuntimeDataRegionObj RuntimeDataRegionObj
		{
			get
			{
				return this.m_runtimeDataRegionObj;
			}
			set
			{
				this.m_runtimeDataRegionObj = value;
			}
		}

		internal Hashtable ScopeNames
		{
			get
			{
				return this.m_scopeNames;
			}
			set
			{
				this.m_scopeNames = value;
			}
		}

		internal bool InPivotCell
		{
			get
			{
				return this.m_inPivotCell;
			}
			set
			{
				this.m_inPivotCell = value;
			}
		}

		internal bool[] IsSortFilterTarget
		{
			get
			{
				return this.m_isSortFilterTarget;
			}
			set
			{
				this.m_isSortFilterTarget = value;
			}
		}

		internal bool[] IsSortFilterExpressionScope
		{
			get
			{
				return this.m_isSortFilterExpressionScope;
			}
			set
			{
				this.m_isSortFilterExpressionScope = value;
			}
		}

		internal int[] SortFilterSourceDetailScopeInfo
		{
			get
			{
				return this.m_sortFilterSourceDetailScopeInfo;
			}
			set
			{
				this.m_sortFilterSourceDetailScopeInfo = value;
			}
		}

		internal int CurrentDetailRowIndex
		{
			get
			{
				return this.m_currentDetailRowIndex;
			}
			set
			{
				this.m_currentDetailRowIndex = value;
			}
		}

		protected abstract DataRegionExprHost DataRegionExprHost
		{
			get;
		}

		int ISortFilterScope.ID
		{
			get
			{
				return base.m_ID;
			}
		}

		string ISortFilterScope.ScopeName
		{
			get
			{
				return base.m_name;
			}
		}

		bool[] ISortFilterScope.IsSortFilterTarget
		{
			get
			{
				return this.m_isSortFilterTarget;
			}
			set
			{
				this.m_isSortFilterTarget = value;
			}
		}

		bool[] ISortFilterScope.IsSortFilterExpressionScope
		{
			get
			{
				return this.m_isSortFilterExpressionScope;
			}
			set
			{
				this.m_isSortFilterExpressionScope = value;
			}
		}

		ExpressionInfoList ISortFilterScope.UserSortExpressions
		{
			get
			{
				return this.m_userSortExpressions;
			}
			set
			{
				this.m_userSortExpressions = value;
			}
		}

		IndexedExprHost ISortFilterScope.UserSortExpressionsHost
		{
			get
			{
				if (this.DataRegionExprHost == null)
				{
					return null;
				}
				return this.DataRegionExprHost.UserSortExpressionsHost;
			}
		}

		protected DataRegion(ReportItem parent)
			: base(parent)
		{
		}

		protected DataRegion(int id, ReportItem parent)
			: base(id, parent)
		{
			this.m_aggregates = new DataAggregateInfoList();
			this.m_postSortAggregates = new DataAggregateInfoList();
		}

		internal override bool Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_filters != null)
			{
				for (int i = 0; i < this.m_filters.Count; i++)
				{
					this.m_filters[i].Initialize(context);
				}
			}
			if (this.m_noRows != null)
			{
				this.m_noRows.Initialize("NoRows", context);
				context.ExprHostBuilder.GenericNoRows(this.m_noRows);
			}
			if (this.m_userSortExpressions != null)
			{
				context.ExprHostBuilder.UserSortExpressionsStart();
				for (int j = 0; j < this.m_userSortExpressions.Count; j++)
				{
					ExpressionInfo expression = this.m_userSortExpressions[j];
					context.ExprHostBuilder.UserSortExpression(expression);
				}
				context.ExprHostBuilder.UserSortExpressionsEnd();
			}
			return false;
		}

		DataAggregateInfoList[] IAggregateHolder.GetAggregateLists()
		{
			return new DataAggregateInfoList[1]
			{
				this.m_aggregates
			};
		}

		DataAggregateInfoList[] IAggregateHolder.GetPostSortAggregateLists()
		{
			return new DataAggregateInfoList[1]
			{
				this.m_postSortAggregates
			};
		}

		void IAggregateHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_aggregates);
			if (this.m_aggregates.Count == 0)
			{
				this.m_aggregates = null;
			}
			Global.Tracer.Assert(null != this.m_postSortAggregates);
			if (this.m_postSortAggregates.Count == 0)
			{
				this.m_postSortAggregates = null;
			}
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			if (this.m_pagebreakState == PageBreakStates.Unknown)
			{
				if (SharedHiddenState.Never != Visibility.GetSharedHidden(base.m_visibility))
				{
					this.m_pagebreakState = PageBreakStates.CanIgnore;
				}
				else
				{
					this.m_pagebreakState = PageBreakStates.CannotIgnore;
				}
			}
			if (PageBreakStates.CanIgnore == this.m_pagebreakState)
			{
				return true;
			}
			return false;
		}

		bool IPageBreakItem.HasPageBreaks(bool atStart)
		{
			if (atStart && this.m_pageBreakAtStart)
			{
				goto IL_0016;
			}
			if (!atStart && this.m_pageBreakAtEnd)
			{
				goto IL_0016;
			}
			return false;
			IL_0016:
			return true;
		}

		protected void DataRegionSetExprHost(DataRegionExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null);
			base.ReportItemSetExprHost(exprHost, reportObjectModel);
			if (exprHost.FilterHostsRemotable != null)
			{
				Global.Tracer.Assert(this.m_filters != null);
				int count = this.m_filters.Count;
				for (int i = 0; i < count; i++)
				{
					this.m_filters[i].SetExprHost(exprHost.FilterHostsRemotable, reportObjectModel);
				}
			}
			if (exprHost.UserSortExpressionsHost != null)
			{
				exprHost.UserSortExpressionsHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DataSetName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.NoRows, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.PageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.KeepTogether, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RepeatSiblings, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.Filters, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.FilterList));
			memberInfoList.Add(new MemberInfo(MemberName.Aggregates, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.PostSortAggregates, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.UserSortExpressions, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DetailSortFiltersInScope, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InScopeSortFilterHashtable));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
