namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapBucketExprHost : ReportObjectModelProxy
	{
		public virtual object StartValueExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object EndValueExpr
		{
			get
			{
				return null;
			}
		}
	}
}
