namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ParamExprHost : ReportObjectModelProxy
	{
		public virtual object ValueExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object OmitExpr
		{
			get
			{
				return null;
			}
		}
	}
}
