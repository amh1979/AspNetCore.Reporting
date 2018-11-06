namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
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
