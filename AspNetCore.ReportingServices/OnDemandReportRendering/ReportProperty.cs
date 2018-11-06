namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class ReportProperty
	{
		private bool m_isExpression;

		private string m_expressionString;

		public bool IsExpression
		{
			get
			{
				return this.m_isExpression;
			}
		}

		public string ExpressionString
		{
			get
			{
				return this.m_expressionString;
			}
		}

		internal ReportProperty()
		{
			this.m_isExpression = false;
			this.m_expressionString = null;
		}

		internal ReportProperty(bool isExpression, string expressionString)
		{
			this.m_isExpression = isExpression;
			this.m_expressionString = expressionString;
		}
	}
}
