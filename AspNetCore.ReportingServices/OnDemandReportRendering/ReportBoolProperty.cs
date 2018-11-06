using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportBoolProperty : ReportProperty
	{
		private bool m_value;

		public bool Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal ReportBoolProperty()
		{
			this.m_value = false;
		}

		internal ReportBoolProperty(bool value)
		{
			this.m_value = value;
		}

		internal ReportBoolProperty(AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo expression)
			: base(expression != null && expression.IsExpression, (expression == null) ? null : expression.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				this.m_value = expression.BoolValue;
			}
		}

		internal ReportBoolProperty(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
			: base(expression != null && expression.IsExpression, (expression == null) ? null : expression.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				this.m_value = expression.BoolValue;
			}
		}

		internal ReportBoolProperty(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression, bool value)
			: base(expression != null && expression.IsExpression, (expression == null) ? null : expression.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				this.m_value = value;
			}
		}
	}
}
