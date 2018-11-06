namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class DataValueExprHost : ReportObjectModelProxy
	{
		public virtual object DataValueNameExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object DataValueValueExpr
		{
			get
			{
				return null;
			}
		}
	}
}
