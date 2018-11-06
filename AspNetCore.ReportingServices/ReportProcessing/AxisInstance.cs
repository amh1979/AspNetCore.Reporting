using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class AxisInstance
	{
		private int m_uniqueName;

		private ChartTitleInstance m_title;

		private object[] m_styleAttributeValues;

		private object[] m_majorGridLinesStyleAttributeValues;

		private object[] m_minorGridLinesStyleAttributeValues;

		private object m_minValue;

		private object m_maxValue;

		private object m_crossAtValue;

		private object m_majorIntervalValue;

		private object m_minorIntervalValue;

		private DataValueInstanceList m_customPropertyInstances;

		internal int UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
			set
			{
				this.m_uniqueName = value;
			}
		}

		internal ChartTitleInstance Title
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

		internal object[] StyleAttributeValues
		{
			get
			{
				return this.m_styleAttributeValues;
			}
			set
			{
				this.m_styleAttributeValues = value;
			}
		}

		internal object[] MajorGridLinesStyleAttributeValues
		{
			get
			{
				return this.m_majorGridLinesStyleAttributeValues;
			}
			set
			{
				this.m_majorGridLinesStyleAttributeValues = value;
			}
		}

		internal object[] MinorGridLinesStyleAttributeValues
		{
			get
			{
				return this.m_minorGridLinesStyleAttributeValues;
			}
			set
			{
				this.m_minorGridLinesStyleAttributeValues = value;
			}
		}

		internal object MinValue
		{
			get
			{
				return this.m_minValue;
			}
			set
			{
				this.m_minValue = value;
			}
		}

		internal object MaxValue
		{
			get
			{
				return this.m_maxValue;
			}
			set
			{
				this.m_maxValue = value;
			}
		}

		internal object CrossAtValue
		{
			get
			{
				return this.m_crossAtValue;
			}
			set
			{
				this.m_crossAtValue = value;
			}
		}

		internal object MajorIntervalValue
		{
			get
			{
				return this.m_majorIntervalValue;
			}
			set
			{
				this.m_majorIntervalValue = value;
			}
		}

		internal object MinorIntervalValue
		{
			get
			{
				return this.m_minorIntervalValue;
			}
			set
			{
				this.m_minorIntervalValue = value;
			}
		}

		internal DataValueInstanceList CustomPropertyInstances
		{
			get
			{
				return this.m_customPropertyInstances;
			}
			set
			{
				this.m_customPropertyInstances = value;
			}
		}

		internal AxisInstance(ReportProcessing.ProcessingContext pc, Chart chart, Axis axisDef, Axis.Mode mode)
		{
			this.m_uniqueName = pc.CreateUniqueName();
			string text = mode.ToString();
			if (axisDef.Title != null)
			{
				this.m_title = new ChartTitleInstance(pc, chart, axisDef.Title, text);
			}
			this.m_styleAttributeValues = Chart.CreateStyle(pc, axisDef.StyleClass, chart.Name + "." + text, this.m_uniqueName);
			if (axisDef.MajorGridLines != null)
			{
				this.m_majorGridLinesStyleAttributeValues = Chart.CreateStyle(pc, axisDef.MajorGridLines.StyleClass, chart.Name + "." + text + ".MajorGridLines", this.m_uniqueName);
			}
			if (axisDef.MinorGridLines != null)
			{
				this.m_minorGridLinesStyleAttributeValues = Chart.CreateStyle(pc, axisDef.MinorGridLines.StyleClass, chart.Name + "." + text + ".MinorGridLines", this.m_uniqueName);
			}
			if (axisDef.Min != null && ExpressionInfo.Types.Constant != axisDef.Min.Type)
			{
				this.m_minValue = pc.ReportRuntime.EvaluateChartAxisValueExpression(axisDef.ExprHost, axisDef.Min, chart.Name, text + ".Min", Axis.ExpressionType.Min);
			}
			if (axisDef.Max != null && ExpressionInfo.Types.Constant != axisDef.Max.Type)
			{
				this.m_maxValue = pc.ReportRuntime.EvaluateChartAxisValueExpression(axisDef.ExprHost, axisDef.Max, chart.Name, text + ".Max", Axis.ExpressionType.Max);
			}
			if (axisDef.CrossAt != null && ExpressionInfo.Types.Constant != axisDef.CrossAt.Type)
			{
				this.m_crossAtValue = pc.ReportRuntime.EvaluateChartAxisValueExpression(axisDef.ExprHost, axisDef.CrossAt, chart.Name, text + ".CrossAt", Axis.ExpressionType.CrossAt);
			}
			if (axisDef.MajorInterval != null && ExpressionInfo.Types.Constant != axisDef.MajorInterval.Type)
			{
				this.m_majorIntervalValue = pc.ReportRuntime.EvaluateChartAxisValueExpression(axisDef.ExprHost, axisDef.MajorInterval, chart.Name, text + ".MajorInterval", Axis.ExpressionType.MajorInterval);
			}
			if (axisDef.MinorInterval != null && ExpressionInfo.Types.Constant != axisDef.MinorInterval.Type)
			{
				this.m_minorIntervalValue = pc.ReportRuntime.EvaluateChartAxisValueExpression(axisDef.ExprHost, axisDef.MinorInterval, chart.Name, text + ".MinorInterval", Axis.ExpressionType.MinorInterval);
			}
			if (axisDef.CustomProperties != null)
			{
				this.m_customPropertyInstances = axisDef.CustomProperties.EvaluateExpressions(chart.ObjectType, chart.Name, text + ".", pc);
			}
		}

		internal AxisInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Title, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartTitleInstance));
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.MajorGridLinesStyleAttributeValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.MinorGridLinesStyleAttributeValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.Min, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.Max, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.CrossAt, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.MajorInterval, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.MinorInterval, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.CustomPropertyInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
