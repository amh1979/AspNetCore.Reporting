using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RowSkippingFilter
	{
		private readonly List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> m_expressions;

		private readonly List<object> m_values;

		private readonly OnDemandProcessingContext m_odpContext;

		private readonly IRIFReportDataScope m_scope;

		public RowSkippingFilter(OnDemandProcessingContext odpContext, IRIFReportDataScope scope, List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> expressions, List<object> values)
		{
			this.m_odpContext = odpContext;
			this.m_scope = scope;
			this.m_expressions = expressions;
			this.m_values = values;
		}

		[Conditional("DEBUG")]
		private void ValidateExpressionsAndValues(List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> expressions, List<object> values)
		{
			Global.Tracer.Assert(expressions != null && values != null && expressions.Count == values.Count, "Invalid expressions or values");
			foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expression in expressions)
			{
				Global.Tracer.Assert(expression.Type == AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field, "Only simple field reference expressions can be row skipping filters.");
			}
		}

		public bool ShouldSkipCurrentRow()
		{
			FieldsImpl fieldsImpl = this.m_odpContext.ReportObjectModel.FieldsImpl;
			bool flag = true;
			for (int i = 0; i < this.m_expressions.Count; i++)
			{
				if (!flag)
				{
					break;
				}
				AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = this.m_expressions[i];
				if (expressionInfo.FieldIndex < 0)
				{
					flag = false;
				}
				else
				{
					FieldImpl fieldImpl = fieldsImpl[expressionInfo.FieldIndex];
					if (fieldImpl.FieldStatus != 0)
					{
						return false;
					}
					int num = this.m_odpContext.CompareAndStopOnError(this.m_values[i], fieldImpl.Value, this.m_scope.DataScopeObjectType, this.m_scope.Name, "GroupExpression", false);
					flag = (num == 0);
				}
			}
			return flag;
		}
	}
}
