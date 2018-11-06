using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportDoubleProperty : ReportProperty
	{
		private double m_value;

		public double Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal ReportDoubleProperty(AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo expressionInfo)
			: base(expressionInfo != null && expressionInfo.IsExpression, (expressionInfo == null) ? null : expressionInfo.OriginalText)
		{
			if (expressionInfo != null && !expressionInfo.IsExpression && !double.TryParse(expressionInfo.Value, out this.m_value))
			{
				this.m_value = 0.0;
			}
		}

		internal ReportDoubleProperty(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo)
			: base(expressionInfo != null && expressionInfo.IsExpression, (expressionInfo == null) ? null : expressionInfo.OriginalText)
		{
			if (expressionInfo != null && !expressionInfo.IsExpression)
			{
				if (expressionInfo.ConstantType == DataType.Float)
				{
					this.m_value = expressionInfo.FloatValue;
				}
				else if (!double.TryParse(expressionInfo.StringValue, out this.m_value))
				{
					this.m_value = 0.0;
				}
			}
		}
	}
}
