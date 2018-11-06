namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class PageBreakExprHost : ReportObjectModelProxy
	{
		public virtual object DisabledExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ResetPageNumberExpr
		{
			get
			{
				return null;
			}
		}
	}
}
