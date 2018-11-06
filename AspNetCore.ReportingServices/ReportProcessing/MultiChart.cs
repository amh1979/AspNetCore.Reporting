using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MultiChart : ReportHierarchyNode
	{
		internal enum Layouts
		{
			Automatic,
			Horizontal,
			Vertical
		}

		private Layouts m_layout;

		private int m_maxCount;

		private bool m_syncScale;

		internal Layouts Layout
		{
			get
			{
				return this.m_layout;
			}
			set
			{
				this.m_layout = value;
			}
		}

		internal int MaxCount
		{
			get
			{
				return this.m_maxCount;
			}
			set
			{
				this.m_maxCount = value;
			}
		}

		internal bool SyncScale
		{
			get
			{
				return this.m_syncScale;
			}
			set
			{
				this.m_syncScale = value;
			}
		}

		internal MultiChart()
		{
		}

		internal MultiChart(int id, Chart chartDef)
			: base(id, chartDef)
		{
		}

		internal void SetExprHost(MultiChartExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			exprHost.SetReportObjectModel(reportObjectModel);
			base.ReportHierarchyNodeSetExprHost(exprHost.GroupingHost, null, reportObjectModel);
		}

		internal new void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MultiChartStart();
			base.Initialize(context);
			context.ExprHostBuilder.MultiChartEnd();
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Layout, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.MaxCount, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.SyncScale, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportHierarchyNode, memberInfoList);
		}
	}
}
