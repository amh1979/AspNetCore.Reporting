using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableGroup : ReportHierarchyNode, IRunningValueHolder, IPageBreakItem
	{
		private TableRowList m_headerRows;

		private bool m_headerRepeatOnNewPage;

		private TableRowList m_footerRows;

		private bool m_footerRepeatOnNewPage;

		private Visibility m_visibility;

		private bool m_propagatedPageBreakAtStart;

		private bool m_propagatedPageBreakAtEnd;

		private RunningValueInfoList m_runningValues;

		private bool m_hasExprHost;

		[NonSerialized]
		private TableGroupExprHost m_exprHost;

		[NonSerialized]
		private bool m_startHidden;

		[NonSerialized]
		private string m_renderingModelID;

		[NonSerialized]
		private int m_startPage = -1;

		[NonSerialized]
		private int m_endPage = -1;

		internal TableGroup SubGroup
		{
			get
			{
				return (TableGroup)base.m_innerHierarchy;
			}
			set
			{
				base.m_innerHierarchy = value;
			}
		}

		internal TableRowList HeaderRows
		{
			get
			{
				return this.m_headerRows;
			}
			set
			{
				this.m_headerRows = value;
			}
		}

		internal bool HeaderRepeatOnNewPage
		{
			get
			{
				return this.m_headerRepeatOnNewPage;
			}
			set
			{
				this.m_headerRepeatOnNewPage = value;
			}
		}

		internal TableRowList FooterRows
		{
			get
			{
				return this.m_footerRows;
			}
			set
			{
				this.m_footerRows = value;
			}
		}

		internal bool FooterRepeatOnNewPage
		{
			get
			{
				return this.m_footerRepeatOnNewPage;
			}
			set
			{
				this.m_footerRepeatOnNewPage = value;
			}
		}

		internal Visibility Visibility
		{
			get
			{
				return this.m_visibility;
			}
			set
			{
				this.m_visibility = value;
			}
		}

		internal bool PropagatedPageBreakAtStart
		{
			get
			{
				return this.m_propagatedPageBreakAtStart;
			}
			set
			{
				this.m_propagatedPageBreakAtStart = value;
			}
		}

		internal bool PropagatedPageBreakAtEnd
		{
			get
			{
				return this.m_propagatedPageBreakAtEnd;
			}
			set
			{
				this.m_propagatedPageBreakAtEnd = value;
			}
		}

		internal RunningValueInfoList RunningValues
		{
			get
			{
				return this.m_runningValues;
			}
			set
			{
				this.m_runningValues = value;
			}
		}

		internal string RenderingModelID
		{
			get
			{
				return this.m_renderingModelID;
			}
			set
			{
				this.m_renderingModelID = value;
			}
		}

		internal bool HasExprHost
		{
			get
			{
				return this.m_hasExprHost;
			}
			set
			{
				this.m_hasExprHost = value;
			}
		}

		internal TableGroupExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal int StartPage
		{
			get
			{
				return this.m_startPage;
			}
			set
			{
				this.m_startPage = value;
			}
		}

		internal int EndPage
		{
			get
			{
				return this.m_endPage;
			}
			set
			{
				this.m_endPage = value;
			}
		}

		internal bool StartHidden
		{
			get
			{
				return this.m_startHidden;
			}
			set
			{
				this.m_startHidden = value;
			}
		}

		internal double HeaderHeightValue
		{
			get
			{
				if (this.m_headerRows != null)
				{
					return this.m_headerRows.GetHeightValue();
				}
				return 0.0;
			}
		}

		internal double FooterHeightValue
		{
			get
			{
				if (this.m_footerRows != null)
				{
					return this.m_footerRows.GetHeightValue();
				}
				return 0.0;
			}
		}

		internal TableGroup()
		{
		}

		internal TableGroup(int id, Table tableDef)
			: base(id, tableDef)
		{
			this.m_runningValues = new RunningValueInfoList();
		}

		internal void Initialize(int numberOfColumns, TableDetail tableDetail, TableGroup detailGroup, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			Global.Tracer.Assert(null != base.m_grouping);
			context.Location |= LocationFlags.InGrouping;
			context.ExprHostBuilder.TableGroupStart(base.m_grouping.Name);
			context.RegisterGroupingScope(base.m_grouping.Name, base.m_grouping.SimpleGroupExpressions, base.m_grouping.Aggregates, base.m_grouping.PostSortAggregates, base.m_grouping.RecursiveAggregates, base.m_grouping);
			base.Initialize(context);
			context.RegisterRunningValues(this.m_runningValues);
			this.RegisterHeaderAndFooter(context);
			if (this.m_visibility != null)
			{
				this.m_visibility.Initialize(context, true, false);
			}
			this.InitializeHeaderAndFooter(numberOfColumns, context, ref tableHeight, tableColumnVisibility);
			this.InitializeSubGroupsOrDetail(numberOfColumns, tableDetail, detailGroup, context, ref tableHeight, tableColumnVisibility);
			if (this.m_visibility != null)
			{
				this.m_visibility.UnRegisterReceiver(context);
			}
			this.UnRegisterHeaderAndFooter(context);
			context.UnRegisterRunningValues(this.m_runningValues);
			context.UnRegisterGroupingScope(base.m_grouping.Name);
			this.m_hasExprHost = context.ExprHostBuilder.TableGroupEnd();
		}

		internal void RegisterReceiver(InitializationContext context, TableDetail tableDetail)
		{
			this.RegisterHeaderAndFooter(context);
			if (this.m_visibility != null)
			{
				this.m_visibility.RegisterReceiver(context, true);
			}
			this.RegisterHeaderAndFooterReceiver(context);
			this.RegisterSubGroupsOrDetailReceiver(context, tableDetail);
			if (this.m_visibility != null)
			{
				this.m_visibility.UnRegisterReceiver(context);
			}
			this.UnRegisterHeaderAndFooter(context);
		}

		private void RegisterHeaderAndFooter(InitializationContext context)
		{
			if (this.m_headerRows != null)
			{
				this.m_headerRows.Register(context);
			}
			if (this.m_footerRows != null)
			{
				this.m_footerRows.Register(context);
			}
		}

		private void UnRegisterHeaderAndFooter(InitializationContext context)
		{
			if (this.m_footerRows != null)
			{
				this.m_footerRows.UnRegister(context);
			}
			if (this.m_headerRows != null)
			{
				this.m_headerRows.UnRegister(context);
			}
		}

		private void InitializeHeaderAndFooter(int numberOfColumns, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsStart();
			if (this.m_headerRows != null)
			{
				for (int i = 0; i < this.m_headerRows.Count; i++)
				{
					Global.Tracer.Assert(null != this.m_headerRows[i]);
					this.m_headerRows[i].Initialize(true, numberOfColumns, context, ref tableHeight, tableColumnVisibility);
				}
			}
			if (this.m_footerRows != null)
			{
				for (int j = 0; j < this.m_footerRows.Count; j++)
				{
					Global.Tracer.Assert(null != this.m_footerRows[j]);
					this.m_footerRows[j].Initialize(true, numberOfColumns, context, ref tableHeight, tableColumnVisibility);
				}
			}
			context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsEnd();
		}

		private void RegisterHeaderAndFooterReceiver(InitializationContext context)
		{
			if (this.m_headerRows != null)
			{
				for (int i = 0; i < this.m_headerRows.Count; i++)
				{
					Global.Tracer.Assert(null != this.m_headerRows[i]);
					this.m_headerRows[i].RegisterReceiver(context);
				}
			}
			if (this.m_footerRows != null)
			{
				for (int j = 0; j < this.m_footerRows.Count; j++)
				{
					Global.Tracer.Assert(null != this.m_footerRows[j]);
					this.m_footerRows[j].RegisterReceiver(context);
				}
			}
		}

		private void InitializeSubGroupsOrDetail(int numberOfColumns, TableDetail tableDetail, TableGroup detailGroup, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			if (detailGroup != null && this.SubGroup == null)
			{
				this.SubGroup = detailGroup;
				detailGroup = null;
			}
			if (this.SubGroup != null)
			{
				this.SubGroup.Initialize(numberOfColumns, tableDetail, detailGroup, context, ref tableHeight, tableColumnVisibility);
			}
			else if (tableDetail != null)
			{
				tableDetail.Initialize(numberOfColumns, context, ref tableHeight, tableColumnVisibility);
			}
		}

		private void RegisterSubGroupsOrDetailReceiver(InitializationContext context, TableDetail tableDetail)
		{
			if (this.SubGroup != null)
			{
				this.SubGroup.RegisterReceiver(context, tableDetail);
			}
			else if (tableDetail != null)
			{
				tableDetail.RegisterReceiver(context);
			}
		}

		internal void CalculatePropagatedFlags(out bool groupPageBreakAtStart, out bool groupPageBreakAtEnd)
		{
			if (this.SubGroup == null)
			{
				groupPageBreakAtStart = base.m_grouping.PageBreakAtStart;
				groupPageBreakAtEnd = base.m_grouping.PageBreakAtEnd;
			}
			else
			{
				this.SubGroup.CalculatePropagatedFlags(out groupPageBreakAtStart, out groupPageBreakAtEnd);
				groupPageBreakAtStart = (groupPageBreakAtStart || base.m_grouping.PageBreakAtStart);
				groupPageBreakAtEnd = (groupPageBreakAtEnd || base.m_grouping.PageBreakAtEnd);
				bool flag = true;
				if (this.SubGroup.HeaderRows != null)
				{
					flag = this.SubGroup.HeaderRepeatOnNewPage;
				}
				this.m_propagatedPageBreakAtStart = (this.SubGroup.Grouping.PageBreakAtStart || (this.SubGroup.PropagatedPageBreakAtStart && flag));
				flag = true;
				if (this.SubGroup.FooterRows != null)
				{
					flag = this.SubGroup.FooterRepeatOnNewPage;
				}
				this.m_propagatedPageBreakAtEnd = (this.SubGroup.Grouping.PageBreakAtEnd || (this.SubGroup.PropagatedPageBreakAtEnd && flag));
			}
		}

		RunningValueInfoList IRunningValueHolder.GetRunningValueList()
		{
			return this.m_runningValues;
		}

		void IRunningValueHolder.ClearIfEmpty()
		{
			Global.Tracer.Assert(null != this.m_runningValues);
			if (this.m_runningValues.Count == 0)
			{
				this.m_runningValues = null;
			}
		}

		bool IPageBreakItem.IgnorePageBreaks()
		{
			return base.IgnorePageBreaks(this.m_visibility);
		}

		internal void SetExprHost(TableGroupExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null && this.m_hasExprHost);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			base.ReportHierarchyNodeSetExprHost(this.m_exprHost, reportObjectModel);
			if (this.m_exprHost.TableRowVisibilityHiddenExpressions != null)
			{
				this.m_exprHost.TableRowVisibilityHiddenExpressions.SetReportObjectModel(reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRows, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowList));
			memberInfoList.Add(new MemberInfo(MemberName.HeaderRepeatOnNewPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRows, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowList));
			memberInfoList.Add(new MemberInfo(MemberName.FooterRepeatOnNewPage, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Visibility, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Visibility));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtStart, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.PropagatedPageBreakAtEnd, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.RunningValues, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.HasExprHost, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportHierarchyNode, memberInfoList);
		}
	}
}
