namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class JoinConditionExprHost : ReportObjectModelProxy
	{
		public virtual object ForeignKeyExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object PrimaryKeyExpr
		{
			get
			{
				return null;
			}
		}
	}
}
