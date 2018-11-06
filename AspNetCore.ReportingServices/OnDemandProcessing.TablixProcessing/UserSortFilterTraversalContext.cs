namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal class UserSortFilterTraversalContext : ITraversalContext
	{
		private RuntimeSortFilterEventInfo m_eventInfo;

		private RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj m_expressionScope;

		public RuntimeSortFilterEventInfo EventInfo
		{
			get
			{
				return this.m_eventInfo;
			}
		}

		public RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj ExpressionScope
		{
			get
			{
				return this.m_expressionScope;
			}
			set
			{
				this.m_expressionScope = value;
			}
		}

		public UserSortFilterTraversalContext(RuntimeSortFilterEventInfo eventInfo)
		{
			this.m_eventInfo = eventInfo;
		}
	}
}
