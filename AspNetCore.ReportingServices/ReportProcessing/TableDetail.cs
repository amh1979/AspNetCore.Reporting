using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableDetail : IDOwner, IRunningValueHolder
	{
		private TableRowList m_detailRows;

		private Sorting m_sorting;

		private Visibility m_visibility;

		private RunningValueInfoList m_runningValues;

		private bool m_hasExprHost;

		private bool m_simpleDetailRows;

		[NonSerialized]
		private TableGroupExprHost m_exprHost;

		[NonSerialized]
		private bool m_startHidden;

		internal TableRowList DetailRows
		{
			get
			{
				return this.m_detailRows;
			}
			set
			{
				this.m_detailRows = value;
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

		internal bool SimpleDetailRows
		{
			get
			{
				return this.m_simpleDetailRows;
			}
			set
			{
				this.m_simpleDetailRows = value;
			}
		}

		internal TableGroupExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
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

		internal TableDetail()
		{
		}

		internal TableDetail(int id)
			: base(id)
		{
			this.m_runningValues = new RunningValueInfoList();
		}

		internal void Initialize(int numberOfColumns, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			context.Location |= LocationFlags.InDetail;
			context.DetailObjectType = ObjectType.Table;
			context.ExprHostBuilder.TableGroupStart("TableDetails");
			context.RegisterRunningValues(this.m_runningValues);
			if (this.m_sorting != null)
			{
				this.m_sorting.Initialize(context);
			}
			if (this.m_detailRows != null)
			{
				this.m_detailRows.Register(context);
			}
			if (this.m_visibility != null)
			{
				this.m_visibility.Initialize(context, true, false);
			}
			this.InitializeDetailRows(numberOfColumns, context, ref tableHeight, tableColumnVisibility);
			if (this.m_visibility != null)
			{
				this.m_visibility.UnRegisterReceiver(context);
			}
			if (this.m_detailRows != null)
			{
				this.m_detailRows.UnRegister(context);
			}
			context.UnRegisterRunningValues(this.m_runningValues);
			this.m_hasExprHost = context.ExprHostBuilder.TableGroupEnd();
		}

		private void InitializeDetailRows(int numberOfColumns, InitializationContext context, ref double tableHeight, bool[] tableColumnVisibility)
		{
			if (this.m_detailRows != null)
			{
				if (!context.MergeOnePass || 1 >= context.DataRegionCount)
				{
					this.m_simpleDetailRows = true;
				}
				context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsStart();
				for (int i = 0; i < this.m_detailRows.Count; i++)
				{
					Global.Tracer.Assert(null != this.m_detailRows[i]);
					if (!this.m_detailRows[i].Initialize(true, numberOfColumns, context, ref tableHeight, tableColumnVisibility))
					{
						this.m_simpleDetailRows = false;
					}
				}
				context.ExprHostBuilder.TableRowVisibilityHiddenExpressionsEnd();
			}
		}

		internal void RegisterReceiver(InitializationContext context)
		{
			if (this.m_detailRows != null)
			{
				this.m_detailRows.Register(context);
				if (this.m_visibility != null)
				{
					this.m_visibility.Initialize(context, true, false);
				}
				for (int i = 0; i < this.m_detailRows.Count; i++)
				{
					Global.Tracer.Assert(null != this.m_detailRows[i]);
					this.m_detailRows[i].RegisterReceiver(context);
				}
				if (this.m_visibility != null)
				{
					this.m_visibility.UnRegisterReceiver(context);
				}
				this.m_detailRows.UnRegister(context);
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

		internal void SetExprHost(TableGroupExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null && this.m_hasExprHost);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_exprHost.TableRowVisibilityHiddenExpressions != null)
			{
				this.m_exprHost.TableRowVisibilityHiddenExpressions.SetReportObjectModel(reportObjectModel);
			}
			if (this.m_exprHost.SortingHost != null)
			{
				Global.Tracer.Assert(this.m_sorting != null);
				this.m_sorting.SetExprHost(exprHost.SortingHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DetailRows, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.TableRowList));
			memberInfoList.Add(new MemberInfo(MemberName.Sorting, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Sorting));
			memberInfoList.Add(new MemberInfo(MemberName.Visibility, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Visibility));
			memberInfoList.Add(new MemberInfo(MemberName.RunningValues, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.RunningValueInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.HasExprHost, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.SimpleDetailRows, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
