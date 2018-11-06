namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class ImageExprHost : ReportItemExprHost
	{
		public virtual object ValueExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object MIMETypeExpr
		{
			get
			{
				return null;
			}
		}

		public virtual object TagExpr
		{
			get
			{
				return null;
			}
		}
	}
}
