using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ReportHierarchyNode : IDOwner, IPageBreakItem
	{
		protected Grouping m_grouping;

		protected Sorting m_sorting;

		protected ReportHierarchyNode m_innerHierarchy;

		[Reference]
		protected DataRegion m_dataRegionDef;

		[NonSerialized]
		private PageBreakStates m_pagebreakState;

		[NonSerialized]
		private DynamicGroupExprHost m_exprHost;

		internal Grouping Grouping
		{
			get
			{
				return this.m_grouping;
			}
			set
			{
				this.m_grouping = value;
				if (this.m_grouping != null)
				{
					this.m_grouping.Owner = this;
				}
			}
		}

		internal Sorting Sorting
		{
			get
			{
				return this.m_sorting;
			}
			set
			{
				this.m_sorting = value;
			}
		}

		internal ReportHierarchyNode InnerHierarchy
		{
			get
			{
				return this.m_innerHierarchy;
			}
			set
			{
				this.m_innerHierarchy = value;
			}
		}

		internal DataRegion DataRegionDef
		{
			get
			{
				return this.m_dataRegionDef;
			}
			set
			{
				this.m_dataRegionDef = value;
			}
		}

		internal ReportHierarchyNode()
		{
		}

		internal ReportHierarchyNode(int id, DataRegion dataRegionDef)
			: base(id)
		{
			this.m_dataRegionDef = dataRegionDef;
		}

		internal void Initialize(InitializationContext context)
		{
			if (this.m_grouping != null)
			{
				this.m_grouping.Initialize(context);
			}
			if (this.m_sorting != null)
			{
				this.m_sorting.Initialize(context);
			}
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			return false;
		}

		protected bool IgnorePageBreaks(Visibility visibility)
		{
			if (this.m_pagebreakState == PageBreakStates.Unknown)
			{
				if (SharedHiddenState.Never != Visibility.GetSharedHidden(visibility))
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
			if (this.m_grouping == null)
			{
				return false;
			}
			if (atStart && this.m_grouping.PageBreakAtStart)
			{
				goto IL_002a;
			}
			if (!atStart && this.m_grouping.PageBreakAtEnd)
			{
				goto IL_002a;
			}
			return false;
			IL_002a:
			return true;
		}

		protected void ReportHierarchyNodeSetExprHost(DynamicGroupExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			this.ReportHierarchyNodeSetExprHost(this.m_exprHost.GroupingHost, this.m_exprHost.SortingHost, reportObjectModel);
		}

		internal void ReportHierarchyNodeSetExprHost(GroupingExprHost groupingExprHost, SortingExprHost sortingExprHost, ObjectModelImpl reportObjectModel)
		{
			if (groupingExprHost != null)
			{
				Global.Tracer.Assert(this.m_grouping != null);
				this.m_grouping.SetExprHost(groupingExprHost, reportObjectModel);
			}
			if (sortingExprHost != null)
			{
				Global.Tracer.Assert(this.m_sorting != null);
				this.m_sorting.SetExprHost(sortingExprHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Grouping, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Grouping));
			memberInfoList.Add(new MemberInfo(MemberName.Sorting, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Sorting));
			memberInfoList.Add(new MemberInfo(MemberName.InnerHierarchy, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportHierarchyNode));
			memberInfoList.Add(new MemberInfo(MemberName.DataRegionDef, Token.Reference, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataRegion));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.IDOwner, memberInfoList);
		}
	}
}
