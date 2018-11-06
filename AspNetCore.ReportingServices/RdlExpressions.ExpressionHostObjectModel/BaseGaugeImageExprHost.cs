namespace AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public abstract class BaseGaugeImageExprHost : ReportObjectModelProxy
	{
		public virtual object SourceExpr
		{
			get
			{
				return null;
			}
		}

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

		public virtual object TransparentColorExpr
		{
			get
			{
				return null;
			}
		}
	}
}
