using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportVariantProperty : ReportProperty
	{
		private object m_value;

		public object Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal ReportVariantProperty()
		{
			this.m_value = null;
		}

		internal ReportVariantProperty(bool isExpression)
			: base(isExpression, null)
		{
			this.m_value = null;
		}

		internal ReportVariantProperty(AspNetCore.ReportingServices.ReportProcessing.ExpressionInfo expression)
			: base(expression != null && expression.IsExpression, (expression == null) ? null : expression.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				this.m_value = expression.Value;
			}
		}

		internal ReportVariantProperty(AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
			: base(expression != null && expression.IsExpression, (expression == null) ? null : expression.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				this.m_value = expression.Value;
			}
		}
	}
}
