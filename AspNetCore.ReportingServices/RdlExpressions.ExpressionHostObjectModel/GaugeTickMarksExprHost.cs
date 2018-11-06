namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class GaugeTickMarksExprHost : TickMarkStyleExprHost
	{
		public virtual object IntervalExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object IntervalOffsetExpr
		{
			get
			{
				return null;
			}
		}
	}
}
