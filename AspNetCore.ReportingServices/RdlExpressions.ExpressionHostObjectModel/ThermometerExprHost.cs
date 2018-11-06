namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ThermometerExprHost : StyleExprHost
	{
		public virtual object BulbOffsetExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object BulbSizeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object ThermometerStyleExpr
		{
			get
			{
				return null;
			}
		}
	}
}
