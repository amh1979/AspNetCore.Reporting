namespace AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel
{
	public abstract class DataSourceExprHost : ReportObjectModelProxy
	{
		public virtual object ConnectStringExpr
		{
			get
			{
				return null;
			}
		}
	}
}
