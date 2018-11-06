using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class Filter
	{
		internal enum Operators
		{
			Equal,
			Like,
			GreaterThan,
			GreaterThanOrEqual,
			LessThan,
			LessThanOrEqual,
			TopN,
			BottomN,
			TopPercent,
			BottomPercent,
			In,
			Between,
			NotEqual
		}

		private ExpressionInfo m_expression;

		private Operators m_operator;

		private ExpressionInfoList m_values;

		private int m_exprHostID = -1;

		[NonSerialized]
		private FilterExprHost m_exprHost;

		internal ExpressionInfo Expression
		{
			get
			{
				return this.m_expression;
			}
			set
			{
				this.m_expression = value;
			}
		}

		internal Operators Operator
		{
			get
			{
				return this.m_operator;
			}
			set
			{
				this.m_operator = value;
			}
		}

		internal ExpressionInfoList Values
		{
			get
			{
				return this.m_values;
			}
			set
			{
				this.m_values = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return this.m_exprHostID;
			}
			set
			{
				this.m_exprHostID = value;
			}
		}

		internal FilterExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.FilterStart();
			if (this.m_expression != null)
			{
				this.m_expression.Initialize("FilterExpression", context);
				context.ExprHostBuilder.FilterExpression(this.m_expression);
			}
			if (this.m_values != null)
			{
				for (int i = 0; i < this.m_values.Count; i++)
				{
					ExpressionInfo expressionInfo = this.m_values[i];
					Global.Tracer.Assert(expressionInfo != null);
					expressionInfo.Initialize("FilterValue", context);
					context.ExprHostBuilder.FilterValue(expressionInfo);
				}
			}
			this.m_exprHostID = context.ExprHostBuilder.FilterEnd();
		}

		internal void SetExprHost(IList<FilterExprHost> filterHosts, ObjectModelImpl reportObjectModel)
		{
			if (this.ExprHostID >= 0)
			{
				Global.Tracer.Assert(filterHosts != null && reportObjectModel != null);
				this.m_exprHost = filterHosts[this.ExprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Expression, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Operator, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.Values, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
