namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
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
	}
}
