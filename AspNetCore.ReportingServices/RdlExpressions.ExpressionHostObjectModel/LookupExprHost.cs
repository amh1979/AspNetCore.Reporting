namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class LookupExprHost : ReportObjectModelProxy
	{
		public virtual object SourceExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ResultExpr
		{
			get
			{
				return null;
			}
		}
	}
}
