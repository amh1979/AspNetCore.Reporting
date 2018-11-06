using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Axis
	{
		internal enum TickMarks
		{
			None,
			Inside,
			Outside,
			Cross
		}

		internal enum Mode
		{
			CategoryAxis,
			CategoryAxisSecondary,
			ValueAxis,
			ValueAxisSecondary
		}

		internal enum ExpressionType
		{
			Min,
			Max,
			CrossAt,
			MajorInterval,
			MinorInterval
		}

		private bool m_visible;

		private Style m_styleClass;

		private ChartTitle m_title;

		private bool m_margin;

		private TickMarks m_majorTickMarks;

		private TickMarks m_minorTickMarks;

		private GridLines m_majorGridLines;

		private GridLines m_minorGridLines;

		private ExpressionInfo m_majorInterval;

		private ExpressionInfo m_minorInterval;

		private bool m_reverse;

		private ExpressionInfo m_crossAt;

		private bool m_autoCrossAt = true;

		private bool m_interlaced;

		private bool m_scalar;

		private ExpressionInfo m_min;

		private ExpressionInfo m_max;

		private bool m_autoScaleMin = true;

		private bool m_autoScaleMax = true;

		private bool m_logScale;

		private DataValueList m_customProperties;

		[NonSerialized]
		private AxisExprHost m_exprHost;

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

		internal ChartTitle Title
		{
			get
			{
				return this.m_title;
			}
			set
			{
				this.m_title = value;
			}
		}

		internal bool Margin
		{
			get
			{
				return this.m_margin;
			}
			set
			{
				this.m_margin = value;
			}
		}

		internal TickMarks MajorTickMarks
		{
			get
			{
				return this.m_majorTickMarks;
			}
			set
			{
				this.m_majorTickMarks = value;
			}
		}

		internal TickMarks MinorTickMarks
		{
			get
			{
				return this.m_minorTickMarks;
			}
			set
			{
				this.m_minorTickMarks = value;
			}
		}

		internal GridLines MajorGridLines
		{
			get
			{
				return this.m_majorGridLines;
			}
			set
			{
				this.m_majorGridLines = value;
			}
		}

		internal GridLines MinorGridLines
		{
			get
			{
				return this.m_minorGridLines;
			}
			set
			{
				this.m_minorGridLines = value;
			}
		}

		internal ExpressionInfo MajorInterval
		{
			get
			{
				return this.m_majorInterval;
			}
			set
			{
				this.m_majorInterval = value;
			}
		}

		internal ExpressionInfo MinorInterval
		{
			get
			{
				return this.m_minorInterval;
			}
			set
			{
				this.m_minorInterval = value;
			}
		}

		internal bool Reverse
		{
			get
			{
				return this.m_reverse;
			}
			set
			{
				this.m_reverse = value;
			}
		}

		internal ExpressionInfo CrossAt
		{
			get
			{
				return this.m_crossAt;
			}
			set
			{
				this.m_crossAt = value;
			}
		}

		internal bool AutoCrossAt
		{
			get
			{
				return this.m_autoCrossAt;
			}
			set
			{
				this.m_autoCrossAt = value;
			}
		}

		internal bool Interlaced
		{
			get
			{
				return this.m_interlaced;
			}
			set
			{
				this.m_interlaced = value;
			}
		}

		internal bool Scalar
		{
			get
			{
				return this.m_scalar;
			}
			set
			{
				this.m_scalar = value;
			}
		}

		internal ExpressionInfo Min
		{
			get
			{
				return this.m_min;
			}
			set
			{
				this.m_min = value;
			}
		}

		internal ExpressionInfo Max
		{
			get
			{
				return this.m_max;
			}
			set
			{
				this.m_max = value;
			}
		}

		internal bool AutoScaleMin
		{
			get
			{
				return this.m_autoScaleMin;
			}
			set
			{
				this.m_autoScaleMin = value;
			}
		}

		internal bool AutoScaleMax
		{
			get
			{
				return this.m_autoScaleMax;
			}
			set
			{
				this.m_autoScaleMax = value;
			}
		}

		internal bool LogScale
		{
			get
			{
				return this.m_logScale;
			}
			set
			{
				this.m_logScale = value;
			}
		}

		internal DataValueList CustomProperties
		{
			get
			{
				return this.m_customProperties;
			}
			set
			{
				this.m_customProperties = value;
			}
		}

		internal AxisExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal void SetExprHost(AxisExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_title != null && this.m_exprHost.TitleHost != null)
			{
				this.m_title.SetExprHost(this.m_exprHost.TitleHost, reportObjectModel);
			}
			if (this.m_styleClass != null)
			{
				this.m_styleClass.SetStyleExprHost(this.m_exprHost);
			}
			if (this.m_majorGridLines != null && this.m_majorGridLines.StyleClass != null && this.m_exprHost.MajorGridLinesHost != null)
			{
				this.m_majorGridLines.SetExprHost(this.m_exprHost.MajorGridLinesHost, reportObjectModel);
			}
			if (this.m_minorGridLines != null && this.m_minorGridLines.StyleClass != null && this.m_exprHost.MinorGridLinesHost != null)
			{
				this.m_minorGridLines.SetExprHost(this.m_exprHost.MinorGridLinesHost, reportObjectModel);
			}
			if (this.m_exprHost.CustomPropertyHostsRemotable != null)
			{
				Global.Tracer.Assert(null != this.m_customProperties);
				this.m_customProperties.SetExprHost(this.m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context, Mode mode)
		{
			if (this.m_styleClass != null)
			{
				this.m_styleClass.Initialize(context);
			}
			if (this.m_title != null)
			{
				this.m_title.Initialize(context);
			}
			if (this.m_minorGridLines != null)
			{
				context.ExprHostBuilder.MinorGridLinesStyleStart();
				this.m_minorGridLines.Initialize(context);
				context.ExprHostBuilder.MinorGridLinesStyleEnd();
			}
			if (this.m_majorGridLines != null)
			{
				context.ExprHostBuilder.MajorGridLinesStyleStart();
				this.m_majorGridLines.Initialize(context);
				context.ExprHostBuilder.MajorGridLinesStyleEnd();
			}
			string str = mode.ToString();
			if (this.m_min != null)
			{
				this.m_min.Initialize(str + ".Min", context);
				context.ExprHostBuilder.AxisMin(this.m_min);
			}
			if (this.m_max != null)
			{
				this.m_max.Initialize(str + ".Max", context);
				context.ExprHostBuilder.AxisMax(this.m_max);
			}
			if (this.m_crossAt != null)
			{
				this.m_crossAt.Initialize(str + ".CrossAt", context);
				context.ExprHostBuilder.AxisCrossAt(this.m_crossAt);
			}
			if (this.m_majorInterval != null)
			{
				this.m_majorInterval.Initialize(str + ".MajorInterval", context);
				context.ExprHostBuilder.AxisMajorInterval(this.m_majorInterval);
			}
			if (this.m_minorInterval != null)
			{
				this.m_minorInterval.Initialize(str + ".MinorInterval", context);
				context.ExprHostBuilder.AxisMinorInterval(this.m_minorInterval);
			}
			if (this.m_customProperties != null)
			{
				this.m_customProperties.Initialize(str + ".", true, context);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Visible, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.Title, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartTitle));
			memberInfoList.Add(new MemberInfo(MemberName.Margin, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.MajorTickMarks, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.MinorTickMarks, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.MajorGridLines, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.GridLines));
			memberInfoList.Add(new MemberInfo(MemberName.MinorGridLines, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.GridLines));
			memberInfoList.Add(new MemberInfo(MemberName.MajorInterval, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.MinorInterval, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Reverse, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.CrossAt, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.AutoCrossAt, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Interlaced, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Scalar, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.Min, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Max, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.AutoScaleMin, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.AutoScaleMax, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.LogScale, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.CustomProperties, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
