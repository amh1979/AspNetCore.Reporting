namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class MapSizeRuleExprHost : MapAppearanceRuleExprHost
	{
		public virtual object StartSizeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object EndSizeExpr
		{
			get
			{
				return null;
			}
		}
	}
}
