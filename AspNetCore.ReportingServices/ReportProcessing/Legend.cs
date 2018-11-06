using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Legend
	{
		internal enum LegendLayout
		{
			Column,
			Row,
			Table
		}

		internal enum Positions
		{
			RightTop,
			TopLeft,
			TopCenter,
			TopRight,
			LeftTop,
			LeftCenter,
			LeftBottom,
			RightCenter,
			RightBottom,
			BottomLeft,
			BottomCenter,
			BottomRight
		}

		private bool m_visible;

		private Style m_styleClass;

		private Positions m_position;

		private LegendLayout m_layout;

		private bool m_insidePlotArea;

		internal bool Visible
		{
			get
			{
				return this.m_visible;
			}
			set
			{
				this.m_visible = value;
			}
		}

		internal Style StyleClass
		{
			get
			{
				return this.m_styleClass;
			}
			set
			{
				this.m_styleClass = value;
			}
		}

		internal Positions Position
		{
			get
			{
				return this.m_position;
			}
			set
			{
				this.m_position = value;
			}
		}

		internal LegendLayout Layout
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

		internal bool InsidePlotArea
		{
			get
			{
				return this.m_insidePlotArea;
			}
			set
			{
				this.m_insidePlotArea = value;
			}
		}

		internal void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && null != reportObjectModel);
			exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_styleClass != null)
			{
				this.m_styleClass.SetStyleExprHost(exprHost);
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendStart();
			if (this.m_styleClass != null)
			{
				this.m_styleClass.Initialize(context);
			}
			context.ExprHostBuilder.ChartLegendEnd();
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Visible, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.Position, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Layout, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.InsidePlotArea, Token.Boolean));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
