using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataLabel
	{
		internal enum Positions
		{
			Auto,
			Top,
			TopLeft,
			TopRight,
			Left,
			Center,
			Right,
			BottomRight,
			Bottom,
			BottomLeft
		}

		private bool m_visible;

		private ExpressionInfo m_value;

		private Style m_styleClass;

		private Positions m_position;

		private int m_rotation;

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

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
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

		internal int Rotation
		{
			get
			{
				return this.m_rotation;
			}
			set
			{
				this.m_rotation = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			if (this.m_value != null)
			{
				this.m_value.Initialize("DataLabel", context);
				context.ExprHostBuilder.DataLabelValue(this.m_value);
			}
			if (this.m_styleClass != null)
			{
				context.ExprHostBuilder.DataLabelStyleStart();
				this.m_styleClass.Initialize(context);
				context.ExprHostBuilder.DataLabelStyleEnd();
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

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Visible, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.Position, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Rotation, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
